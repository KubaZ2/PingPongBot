using NetCord;
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
        Context.Callback = InteractionCallback.UpdateMessage(PingHelper.CreateResponse(_latencyMonitor));
        return Task.CompletedTask;
    }
}
