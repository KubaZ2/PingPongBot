using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

using PingPongBot.Middlewares;

namespace PingPongBot.Modules.SlashCommands;

public class PingModule : BaseApplicationCommandModule<HttpSlashCommandContext>
{
    private readonly LatencyMonitor _latencyMonitor;

    public PingModule(LatencyMonitor latencyMonitor)
    {
        _latencyMonitor = latencyMonitor;
    }

    [SlashCommand("ping", "Ping!")]
    public Task PingAsync()
    {
        Context.Callback = InteractionCallback.ChannelMessageWithSource(new InteractionMessageProperties().WithContent($"Pong! {Math.Round(_latencyMonitor.Latency.TotalMilliseconds)} ms")
                                                                                                          .AddComponents(new ActionRowProperties(
                                                                                                                             new ButtonProperties[]
                                                                                                                             {
                                                                                                                                 new ActionButtonProperties("ping", "Update", new EmojiProperties("🔄"), ButtonStyle.Primary),
                                                                                                                             })));
        return Task.CompletedTask;
    }
}
