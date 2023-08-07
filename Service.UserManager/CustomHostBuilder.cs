using Redis.OM;
using Service.UserManager.Repositories;
using Service.UserManager.Services;

namespace Service.UserManager;

public static class CustomHostBuilder
{
    public static WebApplication Build(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ConfigureKestrel(builder);
        AddConfiguration(builder);
        AddLogging(builder);
        AddServices(builder);

        return builder.Build();
    }

    private static void ConfigureKestrel(WebApplicationBuilder builder, string[]? args = null)
    {
        builder.WebHost.UseKestrel(options =>
        {
            options.ListenAnyIP(7331);
        });
    }

    private static void AddConfiguration(WebApplicationBuilder builder, string[]? args = null)
    {
        // Add custom configuration
    }

    private static void AddLogging(WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
    }

    private static void AddServices(WebApplicationBuilder builder)
    {
        builder.Services.AddMemoryCache(); // Memory key value store
        builder.Services.AddHealthChecks();
        builder.Services.AddSingleton(impl => new RedisConnectionProvider(builder.Configuration.GetConnectionString("RedisCloud")));
        builder.Services.AddControllers()
            .AddJsonOptions(opts => {
                opts.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                opts.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            });
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddScoped<ICryptographyService, CryptographyService>();
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddScoped<IUserAccountRepository, UserAccountRepository>();
    }
}
