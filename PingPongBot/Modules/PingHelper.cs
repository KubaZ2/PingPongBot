using NetCord;
using NetCord.Rest;

using PingPongBot.Middlewares;

namespace PingPongBot.Modules;

public static class PingHelper
{
    public static InteractionMessageProperties CreateResponse(LatencyMonitor latencyMonitor)
    {
        return new()
        {
            Content = $"Pong! {Math.Round(latencyMonitor.Latency.TotalMilliseconds)} ms",
            Components = new ComponentProperties[]
            {
                new ActionRowProperties(new ButtonProperties[]
                {
                    new ActionButtonProperties("ping", "Update", new EmojiProperties("🔄"), ButtonStyle.Primary),
                }),
            },
        };
    }
}
