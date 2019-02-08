using System;

namespace LambdaNative.Internal
{
    internal static class LoggingExtensions
    {
        private static readonly Lazy<bool> ShouldLogDebugLazy = new Lazy<bool>(
            () => !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("LAMBDA_NATIVE_DEBUG")));

        private static void LogDebug(string name, string message)
        {
            if (!ShouldLogDebugLazy.Value) return;

            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            Console.WriteLine($"[{timestamp}] [DEBUG] [{name}] {message}");
        }

        public static void LogDebug(this object obj, string message)
        {
            LogDebug(obj?.GetType().Name, message);
        }
    }
}