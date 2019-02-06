using System;
using System.Collections;

namespace LambdaNative.Internal
{
    internal class SystemEnvironment : IEnvironment
    {
        public void SetEnvironmentVariable(string key, string value)
        {
            Environment.SetEnvironmentVariable(key, value);
        }

        public IDictionary GetEnvironmentVariables()
        {
            return Environment.GetEnvironmentVariables();
        }
    }
}