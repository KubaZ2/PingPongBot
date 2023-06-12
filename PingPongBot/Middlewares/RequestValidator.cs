using System.Runtime.InteropServices;
using System.Text;

namespace PingPongBot.Middlewares;

internal partial class RequestValidator : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (await IsSignatureValid(context.Request, context.RequestServices.GetRequiredService<IConfiguration>()["BotPublicKey"]!))
            await next(context);
        else
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
    }

    private static async ValueTask<bool> IsSignatureValid(HttpRequest request, string publicKey)
    {
        if (!request.Headers.TryGetValue("X-Signature-Ed25519", out var signature) || !request.Headers.TryGetValue("X-Signature-Timestamp", out var timestamp))
            return false;

        MemoryStream messageStream = new();
        var timestampBytes = Encoding.UTF8.GetBytes(timestamp!);

        await messageStream.WriteAsync(timestampBytes);
        await request.Body.CopyToAsync(messageStream);

        messageStream.Position = timestampBytes.Length;
        request.Body = messageStream;

        var result = CryptoSignEd25519VerifyDetached(ref MemoryMarshal.GetArrayDataReference(Convert.FromHexString(signature!)),
                                                               ref MemoryMarshal.GetArrayDataReference(messageStream.GetBuffer()),
                                                               (ulong)messageStream.Length,
                                                               ref MemoryMarshal.GetArrayDataReference(Convert.FromHexString(publicKey)));
        return result is 0;
    }

    [LibraryImport("libsodium", EntryPoint = "crypto_sign_ed25519_verify_detached")]
    [UnmanagedCallConv(CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    public static partial int CryptoSignEd25519VerifyDetached(ref byte sig, ref byte m, ulong mlen, ref byte pk);
}
