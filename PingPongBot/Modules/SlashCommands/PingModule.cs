using NetCord;
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
        Context.Callback = InteractionCallback.ChannelMessageWithSource(PingHelper.CreateResponse(_latencyMonitor));
        return Task.CompletedTask;
    }
}
