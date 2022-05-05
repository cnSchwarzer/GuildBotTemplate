using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;  
using GuildBotTemplate;
using GuildBotTemplate.Modules;
using Microsoft.Extensions.Logging;

// Npgsql DateTimeOffset workaround
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true); 

// Disable proxy
HttpClient.DefaultProxy = new WebProxy();

// Register unhandled exceptions
AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) => {
    App.Logger.LogError(eventArgs.ExceptionObject as Exception, "Unhandled Exception");
}; 

// Create bot instance
var bot = new GuildBot();  

// Register modules
bot.RegisterModule(new EchoModule());  

// Run bot
await bot.Run();