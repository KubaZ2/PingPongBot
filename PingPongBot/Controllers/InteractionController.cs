using Microsoft.AspNetCore.Mvc;

using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.Interactions;

namespace PingPongBot.Controllers;

[Route("/")]
public class InteractionController : ControllerBase
{
    [HttpPost]
    public async Task HandleInteractionAsync([ModelBinder(typeof(IInteractionModelBinder))] IInteraction interaction)
    {
        InteractionCallback callback;
        try
        {
            switch (interaction)
            {
                case PingInteraction:
                    {
                        callback = InteractionCallback.Pong;
                    }
                    break;
                case SlashCommandInteraction slashCommandInteraction:
                    {
                        var services = HttpContext.RequestServices;
                        HttpSlashCommandContext context = new(slashCommandInteraction, services.GetRequiredService<RestClient>());
                        var slashCommandService = services.GetRequiredService<ApplicationCommandService<HttpSlashCommandContext>>();
                        await slashCommandService.ExecuteAsync(context, services);
                        callback = context.Callback ?? InteractionCallback.ChannelMessageWithSource("An error occurred!");
                    }
                    break;
                case ButtonInteraction buttonInteraction:
                    {
                        var services = HttpContext.RequestServices;
                        HttpButtonInteractionContext context = new(buttonInteraction, services.GetRequiredService<RestClient>());
                        var buttonService = services.GetRequiredService<InteractionService<HttpButtonInteractionContext>>();
                        await buttonService.ExecuteAsync(context, services);
                        callback = context.Callback ?? InteractionCallback.ChannelMessageWithSource("An error occurred!");
                    }
                    break;
                default:
                    {
                        callback = CreateErrorCallback("Unknown interaction!");
                    }
                    break;
            }
        }
        catch (Exception ex)
        {
            callback = CreateErrorCallback(ex.Message);
        }

        using var content = callback.Serialize();
        var response = HttpContext.Response;
        response.ContentType = content.Headers.ContentType!.ToString();
        await content.CopyToAsync(response.Body);
    }

    private static InteractionCallback CreateErrorCallback(string message)
    {
        return InteractionCallback.ChannelMessageWithSource($"Error: **{message}**");
    }
}
