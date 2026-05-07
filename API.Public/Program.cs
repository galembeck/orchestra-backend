using API.Public.Configuration;
using API.Public.Extensions;
using Domain.Constants;
using Domain.Services;
using Domain.Utils;
using Domain.Utils.Constants;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;
using System.Net;

// Force invariant culture globally so decimal values sent with '.' (e.g. 19.9)
// are never misread as thousands separators on pt-BR systems.
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

var builder = WebApplication.CreateBuilder(args);

#region .: CONFIGURATION :.

var env = builder.Environment;

builder.Configuration
    .SetBasePath(env.ContentRootPath)
    .AddJsonFile($"appsettings.json", optional: true)
    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: false)
    .AddEnvironmentVariables();

var settingsSection = builder.Configuration.GetSection("Settings");
var settings = settingsSection.Get<Settings>() 
    ?? throw new InvalidOperationException("Settings section is missing or invalid in configuration.");

Constant.SetSettings(settings);

if (Constant.Settings.Environment == EnvType.Production)
{
    await builder.Configuration.ConfigureEnvironmentAsync();
}
else
{
    builder.Configuration.ConfigureEnvironment();
}

#endregion .: CONFIGURATION :.



#region .: SERVICES :.

builder.Services.ConfigureCustomServices(builder.Configuration);



    #region .: WEB HOST SERVICES :.

    builder.WebHost
        .UseContentRoot(Directory.GetCurrentDirectory())
        .UseIISIntegration()
        .UseKestrel(options =>
        {
            options.AddServerHeader = false;

            options.Limits.MaxRequestBodySize = 50 * 1024 * 1024; // 50MB

            if (!Debugger.IsAttached)
            {
                options.Listen(IPAddress.IPv6Any, 80);
            }
        });

    var app = builder.Build();

    #endregion .: WEB HOST SERVICES :.



    #region .: STARTUP SEEDING :.

    // Idempotently populates the Permission table from PermissionKey constants.
    // Required before any company can be approved or any role assigned permissions.
    // If the database isn't reachable yet (e.g. migrations haven't run), log a
    // warning and continue so the dev can still boot the app and apply migrations.
    using (var scope = app.Services.CreateScope())
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        try
        {
            var rbac = scope.ServiceProvider.GetRequiredService<IRbacService>();
            await rbac.SeedSystemRolesAndPermissionsAsync();
            logger.LogInformation("RBAC permissions seed: completed.");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex,
                "RBAC permissions seed skipped: database not reachable. Run " +
                "`dotnet ef database update --project Repository --startup-project API.Public` " +
                "and restart to populate the Permission table.");
        }
    }

    #endregion .: STARTUP SEEDING :.



    #region .: APPLICATION :.

    app.ConfigureApplication();

    app.Run();

    #endregion .: APPLICATION :.



#endregion .: SERVICES :.