using System.Text; 
using Microsoft.Extensions.Logging; 
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QQChannelFramework.Api.Types;
using QQChannelFramework.Models.MessageModels;
using QQChannelFramework.Models.Types;
using QQChannelFramework.Tools.TemplateHelper; 

namespace GuildBotTemplate.Modules;

public class EchoModule : Module {
    public override async Task OnUserMessage(Message msg) {
        
    }
}