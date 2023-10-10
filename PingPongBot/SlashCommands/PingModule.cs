using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace PingPongBot.SlashCommands;

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
        var message = new InteractionMessageProperties()
            .WithContent($"Pong! {Math.Round(_latencyMonitor.Latency.TotalMilliseconds)} ms")
            .AddComponents(new ActionRowProperties(
                               new ButtonProperties[]
                               {
                                   new ActionButtonProperties("ping", "Update", new EmojiProperties("🔄"), ButtonStyle.Primary),
                               }));

        Context.Callback = InteractionCallback.ChannelMessageWithSource(message);
        return Task.CompletedTask;
    }
}
