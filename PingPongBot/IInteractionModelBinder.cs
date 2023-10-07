using Microsoft.AspNetCore.Mvc.ModelBinding;

using NetCord.Rest;

namespace PingPongBot;

internal class IInteractionModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var httpContext = bindingContext.HttpContext;
        bindingContext.Result = ModelBindingResult.Success(HttpInteractionFactory.Create(httpContext.Request.Body, httpContext.RequestServices.GetRequiredService<RestClient>()));
        return Task.CompletedTask;
    }
}
