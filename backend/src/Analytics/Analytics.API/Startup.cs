using Autofac;
using Autofac.Extensions.DependencyInjection;
using Analytics.Infrastructure.Configuration;

namespace Analytics.API;

public class Startup
{
    private const string AnalyticsConnectionString = "AnalyticsConnectionString";
    private readonly IConfiguration _configuration;

    public Startup()
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
        
        
    }
    
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

    public void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterModule(new AnalyticsAutofacModule());
    }

    private void InitializeModules(ILifetimeScope scope)
    {
        AnalyticsStartup.Initialize(
            _configuration.GetConnectionString(AnalyticsConnectionString));
    }
    
}