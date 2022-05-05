using System.Diagnostics;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using QQChannelFramework.Api.Types;

namespace GuildBotTemplate {
    public static class App {
        // Sandbox mode
        public const bool SandBox = false;

        // Your App auth info
        public static readonly OpenApiAccessInfo AccessInfo = new() {
            BotAppId = "YourAppId",
            BotToken = "YourToken",
            BotSecret = "YourSecret"
        };

        // Database
        private static string NpgsqlAddress =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? 
                "127.0.0.1" :
                "<Database IP>";
        private static int NpgsqlPort => 5432;
        private static string NpgsqlDatabase => "<Database Name>";
        private static string NpgsqlUsername => "<Database Username>";  
        private static string NpgsqlPassword => "<Database Password>";

        public static string PostgreSqlConnectionString =
            $"Server={NpgsqlAddress};Port={NpgsqlPort};Database={NpgsqlDatabase};Uid={NpgsqlUsername};Pwd={NpgsqlPassword};";

        // Logging
        public static readonly ILoggerFactory LogFactory =
            LoggerFactory.Create(builder => {
                builder.AddSimpleConsole(o => {
                    o.IncludeScopes = true;
                    o.TimestampFormat = "HH:mm:ss ";
                    o.SingleLine = true;
                });
                builder.AddFile("logs/{Date}.log", LogLevel.Trace);
                builder.SetMinimumLevel(
                    LogLevel.Trace
                );
            });
        public static readonly ILogger Logger = LogFactory.CreateLogger("App");
    }
}