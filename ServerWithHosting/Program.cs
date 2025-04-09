﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using ServerWithHosting.Services;
using ServerWithHosting.Tools;

Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Verbose()
           //    .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "TestServer_.log"),
           //        rollingInterval: RollingInterval.Day,
           //        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
           .WriteTo.Debug()
           .WriteTo.Console(standardErrorFromLevel: Serilog.Events.LogEventLevel.Verbose)
           .CreateLogger();

try
{
    Log.Information("Starting server...");

    var builder = Host.CreateApplicationBuilder(args);
    
    builder.Configuration
    .AddUserSecrets<Program>() 
    .AddEnvironmentVariables();

    builder.Services.Configure<MyAnimeListOptions>(builder.Configuration.GetSection(nameof(MyAnimeListOptions)));
    builder.Services.AddHttpClient();
    builder.Services.AddSingleton<AnimeListService>();
    builder.Services.AddSerilog();
    builder.Services.AddMcpServer()
        .WithStdioServerTransport()
        .WithToolsFromAssembly();

    var app = builder.Build();

    await app.RunAsync();
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    await Log.CloseAndFlushAsync();
}

