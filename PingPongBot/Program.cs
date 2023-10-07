using System.Reflection;

using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.Interactions;

using PingPongBot.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(services => new HttpInteractionValidator(services.GetRequiredService<IConfiguration>()["BotPublicKey"]!))
                .AddTransient<RequestValidator>()
                .AddSingleton<LatencyMonitor>()
                .AddSingleton(services => new Token(TokenType.Bot, services.GetRequiredService<IConfiguration>()["BotToken"]!))
                .AddSingleton(services => new RestClient(services.GetRequiredService<Token>()))
                .AddSingleton(services => new ApplicationCommandService<HttpSlashCommandContext>())
                .AddSingleton(services => new InteractionService<HttpButtonInteractionContext>())
                .AddControllers();

var app = builder.Build();

await app.SetupInteractionServicesAsync();

app.UseMiddleware<RequestValidator>();
app.UseMiddleware<LatencyMonitor>();
app.MapControllers();

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
