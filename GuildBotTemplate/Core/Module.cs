using Microsoft.Extensions.Logging;
using QQChannelFramework.Models;
using QQChannelFramework.Models.MessageModels;
using QQChannelFramework.Models.WsModels;
using GuildBotTemplate.Modules;
using QQChannelFramework.Api;

namespace GuildBotTemplate {
    public abstract class Module {
        private static readonly Dictionary<string, Module> Instances = new();

        public T GetModule<T>() where T : Module {
            return Instances[typeof(T).Name] as T ?? throw new InvalidOperationException();
        }

        protected string botId => bot.BotId;

        protected readonly ILogger log;
        protected internal QQChannelApi api => bot.api;
        internal GuildBot bot { private get; set; }

        protected Module() {
            log = App.LogFactory.CreateLogger(GetType().Name!);
            Instances[GetType().Name] = this;
        }

        public virtual Task OnModuleLoad() {
            return Task.CompletedTask;
        }

        public virtual Task OnModuleUnload() {
            return Task.CompletedTask;
        }

        public virtual Task OnUserMessage(Message msg) {
            return Task.CompletedTask;
        }

        public virtual Task OnDirectMessage(Message msg) {
            return Task.CompletedTask;
        }

        public virtual Task OnAtMessage(Message msg) {
            return Task.CompletedTask;
        }

        public virtual Task OnNewMemberJoin(MemberWithGuildID member) {
            return Task.CompletedTask;
        }

        public virtual Task OnAddedToGuild(WsGuild guild) {
            return Task.CompletedTask;
        }

        public virtual Task OnRemovedFromGuild(WsGuild guild) {
            return Task.CompletedTask;
        }

        public virtual Task OnReactionAdded(MessageReaction reaction) {
            return Task.CompletedTask;
        }

        public virtual Task OnReactionRemoved(MessageReaction reaction) {
            return Task.CompletedTask;
        }
    }
}