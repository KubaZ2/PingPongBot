using NetCord;
using NetCord.Rest;
using NetCord.Services.Interactions;

using PingPongBot.Middlewares;

namespace PingPongBot.Modules.ButtonInteractions;

public class PingModule : InteractionModule<HttpButtonInteractionContext>
{
    private readonly LatencyMonitor _latencyMonitor;

    public PingModule(LatencyMonitor latencyMonitor)
    {
        _latencyMonitor = latencyMonitor;
    }

    [Interaction("ping")]
    public Task PingAsync()
    {
        Context.Callback = InteractionCallback.ModifyMessage(m => m.WithContent($"Pong! {Math.Round(_latencyMonitor.Latency.TotalMilliseconds)} ms"));
        return Task.CompletedTask;
    }
}
