using NetCord.Rest;

namespace PingPongBot.Middlewares;

internal partial class RequestValidator : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var request = context.Request;

        MemoryStream memoryStream = new((int)request.ContentLength.GetValueOrDefault());
        await request.Body.CopyToAsync(memoryStream);
        memoryStream.Position = 0;
        request.Body = memoryStream;

        if (request.Headers.TryGetValue("X-Signature-Ed25519", out var signature)
            && request.Headers.TryGetValue("X-Signature-Timestamp", out var timestamp)
            && context.RequestServices.GetRequiredService<HttpInteractionValidator>().Validate(signature[0], timestamp[0], memoryStream.GetBuffer().AsSpan(0, (int)memoryStream.Length)))
            await next(context);
        else
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
    }
}
