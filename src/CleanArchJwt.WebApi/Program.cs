using CleanArchJwt.Application;
using CleanArchJwt.Infrastructure;
using CleanArchJwt.Infrastructure.Auth;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Serilog
var logPath = Path.Combine(AppContext.BaseDirectory, "logs", "log.txt");
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();

// Add Application & Infrastructure layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Controllers + Validation + ProblemDetails
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = null);
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddProblemDetails();

// CORS for Angular (default http://localhost:4200)
const string AngularPolicy = "AngularPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(AngularPolicy, p =>
        p.WithOrigins("http://localhost:4200")
         .AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials());
});

// Swagger with JWT auth
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "CleanArchJwt API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new()
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new()
    {
        {
            new() { Reference = new() { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

// JWT Authentication
var jwtSection = builder.Configuration.GetSection("Jwt").Get<JwtSettings>() ?? new JwtSettings();
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection.Issuer,
            ValidAudience = jwtSection.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection.Secret)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSerilogRequestLogging();

// Global error handling to ProblemDetails
app.UseExceptionHandler("/error");
app.Map("/error", (HttpContext httpContext) =>
{
    var feature = httpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
    var ex = feature?.Error;
    var status = ex switch
    {
        UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
        InvalidOperationException => StatusCodes.Status400BadRequest,
        KeyNotFoundException => StatusCodes.Status404NotFound,
        _ => StatusCodes.Status500InternalServerError
    };

    var problem = new ProblemDetails
    {
        Status = status,
        Title = ex?.GetType().Name,
        Detail = ex?.Message,
        Instance = httpContext.Request.Path
    };
    return Results.Problem(problem.Detail, problem.Instance, problem.Status, problem.Title);
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(AngularPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
