using WebApi.Configuration.MongoDb;
using WebApi.Context;
using WebApi.Services.Abstract;
using WebApi.Services;

namespace WebApi.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection ConfigureAppServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Mail configuration
        services.Configure<MailConfiguration>(configuration.GetSection("SmtpSettings"));

        // MongoDB configuration
        services.Configure<MongoDbConfiguration>(configuration.GetSection("MongoDb"));

        // Add MongoDB context
        services.AddSingleton<MongoDbContext>();

        // Add CORS policy
        services.AddCors(options =>
        {
            options.AddPolicy("AllowReactApp",
                policyBuilder => policyBuilder
                    .WithOrigins("http://localhost:5173") // React app URL
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });

        return services;
    }
}
