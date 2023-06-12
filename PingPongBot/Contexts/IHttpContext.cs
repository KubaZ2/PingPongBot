using NetCord;

namespace PingPongBot;

public interface IHttpContext
{
    public InteractionCallback? Callback { get; set; }
}
