using Microsoft.Extensions.Logging;

namespace GuildBotTemplate {
    public class TimerTask : IDisposable {
        private Task _loop;
        private static ILogger _logger = App.LogFactory.CreateLogger<TimerTask>();
        private CancellationTokenSource _source = new ();

        public TimerTask(Action<CancellationToken> action, TimeSpan duration) {
            _loop = Task.Run(async () => {
                await Task.Delay(3000, _source.Token).ConfigureAwait(false);
                while (!_source.Token.IsCancellationRequested) {
                    try {
                        using var ts = new CancellationTokenSource(TimeSpan.FromSeconds(300));
                        action(ts.Token);
                    } catch (Exception ex) when (ex is AggregateException or TaskCanceledException) {
                        _logger.LogError("定时任务超时");
                    } catch (Exception ex) {
                        _logger.LogError(ex, "定时任务异常");
                    }

                    await Task.Delay(duration, _source.Token).ConfigureAwait(false);
                }
            });
        }
        
        public TimerTask(Action action, TimeSpan duration) {
            _loop = Task.Run(async () => {
                await Task.Delay(3000, _source.Token).ConfigureAwait(false);
                while (!_source.Token.IsCancellationRequested) {
                    try {
                        using var ts = new CancellationTokenSource(TimeSpan.FromSeconds(300));
                        action();
                    } catch (Exception ex) when (ex is AggregateException or TaskCanceledException) {
                        _logger.LogError("定时任务超时");
                    } catch (Exception ex) {
                        _logger.LogError(ex, "定时任务异常");
                    }

                    await Task.Delay(duration, _source.Token).ConfigureAwait(false);
                }
            });
        }
        
        public TimerTask(Func<Task> action, TimeSpan duration) {
            _loop = Task.Run(async () => {
                await Task.Delay(3000, _source.Token).ConfigureAwait(false);
                while (!_source.Token.IsCancellationRequested) {
                    try {
                        using var ts = new CancellationTokenSource(TimeSpan.FromSeconds(300));
                        await action();
                    } catch (Exception ex) when (ex is AggregateException or TaskCanceledException) {
                        _logger.LogError("定时任务超时");
                    } catch (Exception ex) {
                        _logger.LogError(ex, "定时任务异常");
                    }

                    await Task.Delay(duration, _source.Token).ConfigureAwait(false);
                }
            });
        }
        
        public TimerTask(Func<CancellationToken, Task> action, TimeSpan duration) {
            _loop = Task.Run(async () => {
                await Task.Delay(3000, _source.Token).ConfigureAwait(false);
                while (!_source.Token.IsCancellationRequested) {
                    try {
                        using var ts = new CancellationTokenSource(TimeSpan.FromSeconds(300));
                        await action(ts.Token);
                    } catch (Exception ex) when (ex is AggregateException or TaskCanceledException) {
                        _logger.LogError("定时任务超时");
                    } catch (Exception ex) {
                        _logger.LogError(ex, "定时任务异常");
                    }

                    await Task.Delay(duration, _source.Token).ConfigureAwait(false);
                }
            });
        }
        
        public TimerTask(TimeSpan firstWait, Func<Task> action, TimeSpan duration) {
            _loop = Task.Run(async () => {
                await Task.Delay(firstWait, _source.Token).ConfigureAwait(false);
                while (!_source.Token.IsCancellationRequested) {
                    try {
                        using var ts = new CancellationTokenSource(TimeSpan.FromSeconds(300));
                        await action();
                    } catch (Exception ex) when (ex is AggregateException or TaskCanceledException) {
                        _logger.LogError("定时任务超时");
                    } catch (Exception ex) {
                        _logger.LogError(ex, "定时任务异常");
                    }

                    await Task.Delay(duration, _source.Token).ConfigureAwait(false);
                }
            });
        }

        public void Dispose() {
            try {
                _source.Cancel();
                _loop.Wait(1000, _source.Token);
                _source.Dispose();
                _loop.Dispose();
            } catch (OperationCanceledException) { } catch (Exception ex) {
                _logger.LogDebug(ex, "退出定时异常");
            }
        }
    }
}