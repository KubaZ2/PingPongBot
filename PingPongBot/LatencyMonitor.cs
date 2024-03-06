using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PingPongBot;

public class LatencyMonitor : IMiddleware
{
    private readonly Stopwatch _stopwatch = new();
    private TimeSpan _latency;

    public TimeSpan Latency
    {
        get
        {
            var latency = Interlocked.Read(ref Unsafe.As<TimeSpan, long>(ref _latency));
            return Unsafe.As<long, TimeSpan>(ref latency);
        }
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        _stopwatch.Restart();
        await next(context);
        var latency = _stopwatch.Elapsed;
        Interlocked.Exchange(ref Unsafe.As<TimeSpan, long>(ref _latency), Unsafe.As<TimeSpan, long>(ref latency));
    }
}
