using System;
using Amazon.Lambda.Core;

namespace Lambda.Native.Internal
{
    internal class LambdaLogger : ILambdaLogger
    {
        public static readonly LambdaLogger Instance = new LambdaLogger();

        public void Log(string message)
        {
            Console.Write(message);
        }

        public void LogLine(string message)
        {
            Console.WriteLine(message);
        }
    }
}