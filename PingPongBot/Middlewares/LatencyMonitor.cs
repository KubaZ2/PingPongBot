using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PingPongBot.Middlewares;

public class LatencyMonitor : IMiddleware
{
    private readonly Stopwatch _stopwatch = new();
    private TimeSpan _latency;

    public TimeSpan Latency => _latency;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        _stopwatch.Restart();
        await next(context);
        var latency = _stopwatch.Elapsed;
        Interlocked.Exchange(ref Unsafe.As<TimeSpan, long>(ref _latency), Unsafe.As<TimeSpan, long>(ref latency));
    }
}
