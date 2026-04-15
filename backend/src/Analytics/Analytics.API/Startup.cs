using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using Analytics.API.Middlewares;
using Analytics.Application.Auth.ApiKey;
using Analytics.Domain.Entities.User;
using Analytics.Infrastructure;
using Autofac;
using Analytics.Infrastructure.Configuration.Auth;
using Analytics.Infrastructure.Configuration.Cache;
using Analytics.Infrastructure.Configuration.Data;
using Analytics.Infrastructure.Configuration.Mediation;
using Analytics.Infrastructure.Configuration.Metrics;
using Analytics.Infrastructure.Configuration.Processing;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Analytics.API;

public class Startup
{
    private const string AnalyticsConnectionString = "AnalyticsConnectionString";
    private const string RedisConnectionString = "RedisConnectionString";
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _env;

    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        _configuration = configuration;
        _env = env;
    }
    
    public void ConfigureServices(IServiceCollection services)
    {
        var jwtSecret = _configuration["JwtSettings:SecretKey"]; 
        if (string.IsNullOrEmpty(jwtSecret))
            throw new InvalidOperationException("Критична помилка: JwtSettings:Secret не знайдено в конфігурації.");
        var key = Encoding.ASCII.GetBytes(jwtSecret);
        
        services.Configure<ApiKeySettings>(_configuration.GetSection("ApiKeySettings"));

        
        services.AddAuthentication(authOptions =>
        {
            authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            
        .AddJwtBearer(bearerOptions =>
        {
            bearerOptions.RequireHttpsMetadata = !_env.IsDevelopment();
            bearerOptions.SaveToken = true;
            bearerOptions.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidIssuer = _configuration["JwtSettings:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["JwtSettings:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });
        
        services.AddDbContext<AnalyticsDbContext>(opt => opt
            .UseNpgsql(AnalyticsConnectionString)
            .LogTo(Console.WriteLine)
            .EnableSensitiveDataLogging());
        
        services.AddHttpContextAccessor();
        services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin",  policy => policy.RequireRole("Admin"));
            options.AddPolicy("AnalystOrAbove", p => p.RequireRole("Admin", "Analyst"));
        });
        
        services.AddCors(options =>
        {
            options.AddPolicy("AllowReactApp", policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
        
        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;

        });
        services.AddEndpointsApiExplorer();
        
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer"
            });
            options.AddSecurityRequirement(new  OpenApiSecurityRequirement
            {
                { new OpenApiSecurityScheme {
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
    }
    
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AnalyticsDbContext>();
            db.Database.Migrate();
        }
        
        if (env.IsDevelopment())
        {
            
        }
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Analytics");
            c.DisplayRequestDuration();
        });

        app.UseRouting();
        app.UseCors("AllowReactApp");
        app.UseMiddleware<ApiKeyMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        RuntimeHelpers.RunClassConstructor(typeof(UserRole).TypeHandle);

    }

    public void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterModule(new AuthModule(_configuration));
        builder.RegisterModule(new DataAccessModule(
            _configuration.GetConnectionString(AnalyticsConnectionString)));
        builder.RegisterModule(new MediatorModule());
        builder.RegisterModule(new ProcessingModule());
        builder.RegisterModule(new MetricsModule());
        builder.RegisterModule(new CachingModule(
            _configuration.GetConnectionString(RedisConnectionString)));
    }
    
}