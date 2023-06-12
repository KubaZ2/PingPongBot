using System.Text.Json;

using Microsoft.AspNetCore.Mvc.ModelBinding;

using NetCord;
using NetCord.JsonModels;
using NetCord.Rest;

namespace PingPongBot;

internal class IInteractionModelBinder : IModelBinder
{
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var model = (await JsonSerializer.DeserializeAsync<JsonInteraction>(bindingContext.HttpContext.Request.Body, Serialization.Options))!;
        var interaction = IInteraction.CreateFromJson(model, bindingContext.HttpContext.RequestServices.GetRequiredService<RestClient>());
        bindingContext.Result = ModelBindingResult.Success(interaction);
    }
}
