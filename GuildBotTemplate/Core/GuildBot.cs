using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using QQChannelFramework.Api;
using QQChannelFramework.Api.Types;
using QQChannelFramework.Expansions.Bot;
using QQChannelFramework.Models;
using QQChannelFramework.Models.MessageModels;
using QQChannelFramework.Models.WsModels;
using GuildBotTemplate.Modules;

namespace GuildBotTemplate {
    public class GuildBot {
        public static GuildBot Instance { get; private set; }
        private ILogger _logger;

        private bool _allModuleLoaded;
        private List<Module> _modules = new();
        public string BotId { get; private set; }

        public ChannelBot bot { get; private set; }
        public QQChannelApi api { get; private set; }

        public GuildBot() {
            Instance = this;

            _logger = App.LogFactory.CreateLogger<GuildBot>();

            api = new QQChannelApi(App.AccessInfo);
            api.UseBotIdentity();

            if (App.SandBox)
#pragma warning disable CS0162
                api.UseSandBoxMode();
#pragma warning restore CS0162

            bot = new ChannelBot(api);
            bot.UsePrivateBot();
            bot.RegisterAtMessageEvent();
            bot.RegisterDirectMessageEvent();
            bot.RegisterGuildMembersEvent();
            bot.RegisterGuildsEvent();
            bot.RegisterAuditEvent();

            bot.OnConnected += () => { _logger.LogInformation("连接成功"); };
            bot.AuthenticationSuccess += () => { _logger.LogInformation("机器人已上线"); };
            bot.AuthenticationError += () => { _logger.LogInformation("机器人鉴权失败"); };
            bot.OnError += ex => { _logger.LogError(ex, "机器人出现错误"); };
            bot.OnClose += () => { _logger.LogInformation("连接关闭"); };
            bot.ConnectBreak += () => { _logger.LogInformation("连接断开"); };
            bot.Reconnecting += () => { _logger.LogInformation("主动重连中"); };

            bot.ReceivedAtMessage += BotOnReceivedAtMessage;
            bot.ReceivedDirectMessage += BotOnReceivedDirectMessage;
            bot.NewMemberJoin += BotOnNewMemberJoin;
            bot.BotAreAddedToTheGuild += BotOnAddedToGuild;
            bot.MessageAuditPass += audit => { _logger.LogInformation($"消息审核通过 {audit.AuditId}"); };
            bot.MessageAuditReject += audit => { _logger.LogInformation($"消息审核不通过 {audit.AuditId}"); };
            bot.BotBeRemoved += BotOnRemoved;

            Console.CancelKeyPress += OnConsoleCancelKeyPress;
        }

        private async void BotOnAddedToGuild(WsGuild guild) {
            if (!_allModuleLoaded)
                return;
            if (_logger.IsEnabled(LogLevel.Trace))
                _logger.LogTrace($"AddedToGuild {guild.Name}");
            foreach (var module in _modules) {
                try {
                    await module.OnAddedToGuild(guild).ConfigureAwait(false);
                } catch (Exception exception) {
                    _logger.LogError(exception, "BotOnAddedToGuild 处理出现异常");
                }
            }
        }

        private async void BotOnRemoved(WsGuild guild) {
            if (!_allModuleLoaded)
                return;
            if (_logger.IsEnabled(LogLevel.Trace))
                _logger.LogTrace($"RemoveFromGuild {guild.Name}");
            foreach (var module in _modules) {
                try {
                    await module.OnRemovedFromGuild(guild).ConfigureAwait(false);
                } catch (Exception exception) {
                    _logger.LogError(exception, "BotOnRemoved 处理出现异常");
                }
            }
        }

        private async void BotOnMessageReactionIsRemoved(MessageReaction reaction) {
            if (!_allModuleLoaded)
                return;
            if (_logger.IsEnabled(LogLevel.Trace))
                _logger.LogTrace($"Reaction Remove {reaction.UserId} {reaction.Target.Type} {reaction.Target.Id}");
            foreach (var module in _modules) {
                try {
                    await module.OnReactionRemoved(reaction).ConfigureAwait(false);
                } catch (Exception exception) {
                    _logger.LogError(exception, "BotOnMessageReactionIsRemoved 处理出现异常");
                }
            }
        }

        private async void BotOnMessageReactionIsAdded(MessageReaction reaction) {
            if (!_allModuleLoaded)
                return;
            if (_logger.IsEnabled(LogLevel.Trace))
                _logger.LogTrace($"Reaction Added {reaction.UserId} {reaction.Target.Type} {reaction.Target.Id}");
            foreach (var module in _modules) {
                try {
                    await module.OnReactionAdded(reaction).ConfigureAwait(false);
                } catch (Exception exception) {
                    _logger.LogError(exception, "BotOnMessageReactionIsAdded 处理出现异常");
                }
            }
        }

        private async void BotOnNewMemberJoin(MemberWithGuildID member) {
            if (!_allModuleLoaded)
                return;
            if (_logger.IsEnabled(LogLevel.Trace))
                _logger.LogTrace($"MemberJoin {member.User.UserName}");
            foreach (var module in _modules) {
                try {
                    await module.OnNewMemberJoin(member).ConfigureAwait(false);
                } catch (Exception exception) {
                    _logger.LogError(exception, "OnBotNewMemberJoin 处理出现异常");
                }
            }
        }

        private void OnConsoleCancelKeyPress(object sender, ConsoleCancelEventArgs e) {
            _logger.LogInformation("关闭中");
            Task.Run(async () => {
                await bot.OfflineAsync().ConfigureAwait(false);
                foreach (var module in _modules) {
                    await module.OnModuleUnload().ConfigureAwait(false);
                }
            }).Wait();
            _logger.LogInformation("退出");
        }

        private async void BotOnReceivedDirectMessage(Message message) {
            if (!_allModuleLoaded)
                return;
            if (_logger.IsEnabled(LogLevel.Trace))
                _logger.LogTrace($"DirectMessage {message.Author.UserName}: {message.Content}");
            foreach (var module in _modules) {
                try {
                    await module.OnDirectMessage(message).ConfigureAwait(false);
                } catch (Exception exception) {
                    _logger.LogError(exception, "BotOnReceivedDirectMessage 处理出现异常");
                }
            }
            // if (_logger.IsEnabled(LogLevel.Trace))
            //     _logger.LogTrace($"DirectMessage End {message.Author.UserName}: {message.Content}");
        }

        private async void BotOnReceivedUserMessage(Message message) {
            if (!_allModuleLoaded)
                return;
            // Remove At
            RemoveAtTags(message);

            if (_logger.IsEnabled(LogLevel.Trace))
                _logger.LogTrace($"UserMsg {message.Author.UserName}: {message.Content}");
            foreach (var module in _modules) {
                try {
                    await module.OnUserMessage(message).ConfigureAwait(false);
                } catch (Exception exception) {
                    _logger.LogError(exception, "BotOnReceivedUserMessage 处理出现异常");
                }
            }
            // if (_logger.IsEnabled(LogLevel.Trace))
            //     _logger.LogTrace($"UserMsg End {message.Author.UserName}: {message.Content}");
        }

        private void RemoveAtTags(Message message) {
            var original = message.Content;
            try {
                if (message.Mentions is {Count: > 0}) {
                    foreach (var user in message.Mentions.Where(user => user != null)) {
                        message.Content = message.Content.Replace($"<@!{user.Id}> ", string.Empty);
                        message.Content = message.Content.Replace($"<@!{user.Id}>", string.Empty);
                    }

                    if (!string.IsNullOrWhiteSpace(message.Content))
                        message.Content = message.Content.Trim();
                }

                if (message.MentionEveryone) {
                    if (message.Content != null) {
                        message.Content = message.Content.Replace($"@everyone ", string.Empty);
                        message.Content = message.Content.Replace($"@everyone", string.Empty);

                        if (!string.IsNullOrWhiteSpace(message.Content))
                            message.Content = message.Content.Trim();
                    }
                }
            } catch (Exception ex) {
                _logger.LogError(ex, "Remove At Tag");
                message.Content = original;
            }
            message.Content ??= string.Empty;
        }

        private async void BotOnReceivedAtMessage(Message message) {
            if (!_allModuleLoaded)
                return;

            if (_logger.IsEnabled(LogLevel.Trace))
                _logger.LogTrace($"AtMsg {message.Author.UserName}: {message.Content}");
            // Remove At
            RemoveAtTags(message);
            foreach (var module in _modules) {
                try {
                    await module.OnAtMessage(message).ConfigureAwait(false);
                } catch (Exception exception) {
                    _logger.LogError(exception, "BotOnReceivedAtMessage 处理出现异常");
                }
            }
            // if (_logger.IsEnabled(LogLevel.Trace))
            //     _logger.LogTrace($"AtMsg End {message.Author.UserName}: {message.Content}");
        }

        public void RegisterModule(Module module) {
            _modules.Add(module);
            module.bot = this;
        }

        public async Task Run() {
            try {
                await bot.OnlineAsync().ConfigureAwait(false);

                var info = await api.GetUserApi().GetCurrentUserAsync();
                BotId = info.Id;
            } catch (Exception exception) {
                _logger.LogError(exception, "启动异常");
            }

            foreach (var module in _modules) {
                try {
                    await module.OnModuleLoad().ConfigureAwait(false);
                } catch (Exception exception) {
                    _logger.LogError(exception, "处理出现异常");
                }
            }

            _allModuleLoaded = true;
            await Task.Delay(-1).ConfigureAwait(false);
        }
    }
}