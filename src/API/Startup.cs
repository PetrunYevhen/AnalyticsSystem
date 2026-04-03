using Autofac;
using Autofac.Extensions.DependencyInjection;
using Infrastructure.Configuration;

namespace API;

public class Startup
{
    private const string AnalyticsConnectionString = "AnalyticsConnectionString";
    private readonly IConfiguration _configuration;
    
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }
    
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
    {
        var container = app.ApplicationServices.GetAutofacRoot();
        
        InitializeModules(container);
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Analytics"));
        }
        
        app.UseRouting();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }

    private void InitializeModules(ILifetimeScope scope)
    {
        AnalyticsStartup.Initialize(
            _configuration.GetConnectionString(AnalyticsConnectionString));
    }
    
}