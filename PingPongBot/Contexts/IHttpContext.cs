using NetCord.Rest;

namespace PingPongBot;

public interface IHttpContext
{
    public InteractionCallback? Callback { get; set; }
}
