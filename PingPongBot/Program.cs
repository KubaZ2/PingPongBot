using NetCord;
using NetCord.Hosting.AspNetCore;
using NetCord.Hosting.Rest;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Hosting.Services.Interactions;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.Interactions;

using PingPongBot;

var builder = WebApplication.CreateBuilder(args);

builder.Host
    .UseDiscordRest()
    .UseApplicationCommandService<SlashCommandInteraction, HttpSlashCommandContext>()
    .UseInteractionService<ButtonInteraction, HttpButtonInteractionContext>();

builder.Services
    .AddSingleton<LatencyMonitor>();

var app = builder.Build();

app.AddSlashCommand<HttpSlashCommandContext>("ping", "Ping!", (LatencyMonitor latencyMonitor, HttpSlashCommandContext context) => new InteractionMessageProperties().WithContent($"Pong! {Math.Round(latencyMonitor.Latency.TotalMilliseconds)} ms")
                                                                                                                                                                    .AddComponents(new ActionRowProperties([new ActionButtonProperties("ping",
                                                                                                                                                                                                                                       "Update",
                                                                                                                                                                                                                                       new EmojiProperties("🔄"),
                                                                                                                                                                                                                                       ButtonStyle.Primary)])));

app.AddInteraction<HttpButtonInteractionContext>("ping", (LatencyMonitor latencyMonitor, HttpButtonInteractionContext context) => InteractionCallback.ModifyMessage(m => m.WithContent($"Pong! {Math.Round(latencyMonitor.Latency.TotalMilliseconds)} ms")));

app.UseMiddleware<LatencyMonitor>();
app.UseHttpInteractions("/");

await app.RunAsync();
