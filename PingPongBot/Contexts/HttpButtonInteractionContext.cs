using NetCord;
using NetCord.Rest;
using NetCord.Services.Interactions;

namespace PingPongBot;

public class HttpButtonInteractionContext : BaseButtonInteractionContext, IHttpContext
{
    public HttpButtonInteractionContext(ButtonInteraction interaction, RestClient client) : base(interaction)
    {
        Client = client;
    }

    public RestClient Client { get; }

    public InteractionCallback? Callback { get; set; }
}
