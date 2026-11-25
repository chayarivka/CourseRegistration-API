
using CourseRegistration.Core.Interfaces;
using CourseRegistration.Data.Repositories;
using CourseRegistration.Data.Context;
using Microsoft.EntityFrameworkCore;
using CourseRegistration.Service.Services;
using CourseRegistration.Service.Mappings;
using CourseRegistration.API.Middleware;
using CourseRegistration.Core.Settings;
using CourseRegistration.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using NLog.Web;
using CourseRegistration.Data.Seed;







var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Host.UseNLog();

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt"));

var jwtSettings = builder.Configuration
    .GetSection("Jwt")
    .Get<JwtSettings>()
    ?? throw new InvalidOperationException("JWT settings are missing.");

if (string.IsNullOrWhiteSpace(jwtSettings.Key) ||
    Encoding.UTF8.GetByteCount(jwtSettings.Key) < 32)
{
    throw new InvalidOperationException(
        "JWT signing key must contain at least 32 bytes.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>

    {

     
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,

            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Key)),

            ClockSkew = TimeSpan.Zero
        };


        

    });

builder.Services.AddAuthorization();
var clientOrigin = builder.Configuration["Cors:ClientOrigin"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientPolicy", policy =>
    {
        policy.SetIsOriginAllowed(origin =>
            new Uri(origin).IsLoopback ||
            string.Equals(origin, clientOrigin,
                StringComparison.OrdinalIgnoreCase))
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorNumbersToAdd: new[] { 1205 }
        )));
builder.Services.AddScoped<ICourseRepository, CourseRepository>();

builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IRegistrationRepository, RegistrationRepository>();
builder.Services.AddScoped<IRegistrationService, RegistrationService>();



builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();


builder.Services.AddAutoMapper(cfg => { }, typeof(CourseMappingProfile));



builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();



// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<AppDbContext>();

    var passwordHasher = scope.ServiceProvider
        .GetRequiredService<IPasswordHasher<User>>();

    await AdminSeeder.SeedAsync(
       context,
       passwordHasher,
       app.Configuration,
       app.Environment.IsDevelopment(),
       app.Lifetime.ApplicationStopping);
}

    app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<CorrelationIdMiddleware>();


// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("ClientPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
