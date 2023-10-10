using System.Reflection;

using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.Interactions;

using PingPongBot;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(services => new HttpInteractionValidator(services.GetRequiredService<IConfiguration>()["BotPublicKey"]!))
                .AddSingleton<LatencyMonitor>()
                .AddSingleton(services => new Token(TokenType.Bot, services.GetRequiredService<IConfiguration>()["BotToken"]!))
                .AddSingleton(services => new RestClient(services.GetRequiredService<Token>()))
                .AddSingleton(services => new ApplicationCommandService<HttpSlashCommandContext>())
                .AddSingleton(services => new InteractionService<HttpButtonInteractionContext>())
                .AddControllers();

var app = builder.Build();

await app.SetupInteractionServicesAsync();

app.Use(async (HttpContext httpContext, RequestDelegate next) =>
{
    var request = httpContext.Request;

    MemoryStream memoryStream = new((int)request.ContentLength.GetValueOrDefault());
    await request.Body.CopyToAsync(memoryStream);
    memoryStream.Position = 0;
    request.Body = memoryStream;

    if (request.Headers.TryGetValue("X-Signature-Ed25519", out var signature)
        && request.Headers.TryGetValue("X-Signature-Timestamp", out var timestamp)
        && httpContext.RequestServices.GetRequiredService<HttpInteractionValidator>().Validate(signature[0], timestamp[0], memoryStream.GetBuffer().AsSpan(0, (int)memoryStream.Length)))
        await next(httpContext);
    else
        httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
});

app.UseMiddleware<LatencyMonitor>();

app.MapPost("/", async httpContext =>
{
    var client = httpContext.RequestServices.GetRequiredService<RestClient>();
    var interaction = HttpInteractionFactory.Create(httpContext.Request.Body, client);

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
                    var services = httpContext.RequestServices;
                    HttpSlashCommandContext context = new(slashCommandInteraction, client);
                    var slashCommandService = services.GetRequiredService<ApplicationCommandService<HttpSlashCommandContext>>();
                    await slashCommandService.ExecuteAsync(context, services);
                    callback = context.Callback ?? InteractionCallback.ChannelMessageWithSource("An error occurred!");
                }
                break;
            case ButtonInteraction buttonInteraction:
                {
                    var services = httpContext.RequestServices;
                    HttpButtonInteractionContext context = new(buttonInteraction, client);
                    var buttonService = services.GetRequiredService<InteractionService<HttpButtonInteractionContext>>();
                    await buttonService.ExecuteAsync(context, services);
                    callback = context.Callback ?? InteractionCallback.ChannelMessageWithSource("An error occurred!");
                }
                break;
            default:
                {
                    callback = InteractionCallback.ChannelMessageWithSource("Unknown interaction!");
                }
                break;
        }
    }
    catch (Exception ex)
    {
        callback = InteractionCallback.ChannelMessageWithSource(ex.Message);
    }

    using var content = callback.Serialize();
    var response = httpContext.Response;
    response.ContentType = content.Headers.ContentType!.ToString();
    await content.CopyToAsync(response.Body);
});

await app.RunAsync();

file static class WebApplicationExtensions
{
    public static Task SetupInteractionServicesAsync(this WebApplication app)
    {
        var services = app.Services;
        var slashCommandService = services.GetRequiredService<ApplicationCommandService<HttpSlashCommandContext>>();
        var buttonService = services.GetRequiredService<InteractionService<HttpButtonInteractionContext>>();

        var assembly = Assembly.GetEntryAssembly()!;
        slashCommandService.AddModules(assembly);
        buttonService.AddModules(assembly);

        return slashCommandService.CreateCommandsAsync(services.GetRequiredService<RestClient>(), services.GetRequiredService<Token>().Id);
    }
}
