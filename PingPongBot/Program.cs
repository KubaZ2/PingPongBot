using NetCord;
using NetCord.Hosting.AspNetCore;
using NetCord.Hosting.Rest;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Hosting.Services.ComponentInteractions;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.ComponentInteractions;

using PingPongBot;

var builder = WebApplication.CreateBuilder(args);

builder.Host
    .UseDiscordRest()
    .UseApplicationCommands<SlashCommandInteraction, HttpSlashCommandContext>()
    .UseComponentInteractions<ButtonInteraction, HttpButtonInteractionContext>();

builder.Services
    .AddSingleton<LatencyMonitor>();

var app = builder.Build();

app.AddSlashCommand<HttpSlashCommandContext>("ping", "Ping!",
    (LatencyMonitor latencyMonitor, HttpSlashCommandContext context) =>
    {
        ButtonProperties updateButton = new("ping", "Update", new("🔄"), ButtonStyle.Primary);

        return new InteractionMessageProperties()
            .WithContent($"Pong! {Math.Round(latencyMonitor.Latency.TotalMilliseconds)} ms")
            .AddComponents(new ActionRowProperties([updateButton]));
    });

app.AddComponentInteraction<HttpButtonInteractionContext>("ping",
    (LatencyMonitor latencyMonitor, HttpButtonInteractionContext context) =>
    {
        return InteractionCallback.ModifyMessage(m => m.WithContent($"Pong! {Math.Round(latencyMonitor.Latency.TotalMilliseconds)} ms"));
    });

app.UseMiddleware<LatencyMonitor>();
app.UseHttpInteractions("/");

await app.RunAsync();
