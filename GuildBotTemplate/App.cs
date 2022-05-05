using System.Diagnostics;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using QQChannelFramework.Api.Types;

namespace GuildBotTemplate {
    public static class App {
        public const bool SandBox = false;

        public static readonly OpenApiAccessInfo AccessInfo = new() {
            BotAppId = "YourAppId",
            BotToken = "YourToken",
            BotSecret = "YourSecret"
        }; 

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