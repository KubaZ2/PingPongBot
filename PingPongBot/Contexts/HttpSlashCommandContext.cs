using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace PingPongBot;

public class HttpSlashCommandContext : BaseSlashCommandContext, IHttpContext
{
    public HttpSlashCommandContext(SlashCommandInteraction interaction, RestClient client) : base(interaction)
    {
        Client = client;
    }

    public RestClient Client { get; }

    public InteractionCallback? Callback { get; set; }
}
