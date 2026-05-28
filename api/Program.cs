using AccidentDetectionSysrem.modelsef;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =========================================================================
// CONFIGURATION VALIDATION & HARDENING ASSERTIONS (net10.0 Resiliency)
// =========================================================================

// 1. Database Connection String Validation
var connectionString = builder.Configuration.GetConnectionString("AccidentDetectionConnect");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException(
        "CRITICAL STARTUP ERROR: The database connection string 'AccidentDetectionConnect' is missing or empty in 'appsettings.json'. " +
        "Ensure your target configuration has a valid SQLite provider path specified."
    );
}

// 2. JWT Configuration Key Validation
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException(
        "CRITICAL STARTUP ERROR: The JWT Signature signing key 'Jwt:Key' is missing in 'appsettings.json'. " +
        "Please declare a secure authentication signing key before starting up the server."
    );
}

// 3. Cryptographic Key Length Validation (HS256 requires >= 256 bits / 32 bytes)
int jwtKeyByteCount = Encoding.UTF8.GetByteCount(jwtKey);
if (jwtKeyByteCount < 32)
{
    throw new InvalidOperationException(
        $"CRITICAL SECURITY ERROR: The configured 'Jwt:Key' is insecure ({jwtKeyByteCount} bytes / {jwtKeyByteCount * 8} bits). " +
        "For secure HS256-based JWT authentication, the signing key MUST be at least 32 bytes (256 bits) wide. " +
        "Please provide a wider cryptographic key in 'appsettings.json' to satisfy .NET Core 10 security constraints."
    );
}

// =========================================================================
// SERVICES REGISTRATION
// =========================================================================
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();

// Hardened Swagger / OpenAPI Documentation
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Accident Detection System API",
        Version = "v1",
        Description = "API for detecting road accidents and structural anomalies using AI.",
        Contact = new OpenApiContact
        {
            Name = "Malak Elgendy",
            Email = "malakelgendy02@gmail.com"
        }
    });
});

// SQLite Context Registration
builder.Services.AddDbContext<AccidentDetectionSystemContect>(options =>
    options.UseSqlite(connectionString)
);

// Cross-Origin Resource Sharing (CORS) policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// JWT Authentication Service Setup
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// =========================================================================
// PIPELINE MIDDLEWARE INITIALIZATION
// =========================================================================
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Accident Detection API v1");
    });
}

app.UseHttpsRedirection();

// CORS policy MUST be invoked before Authentication and Authorization
app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();