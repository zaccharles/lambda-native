using System;
using System.Collections;
using System.Runtime.InteropServices;

namespace LambdaNative.Internal
{
    internal class SystemEnvironment : IEnvironment
    {
        [DllImport("c")]
        private static extern void setenv(string key, string value);

        public void SetEnvironmentVariable(string key, string value)
        {
            // Workaround because Environment.SetEnvironmentVariable is not implemented in CoreRT.
            // See https://github.com/dotnet/corert/issues/6971 for more details.
            try { Environment.SetEnvironmentVariable(key, value); }
            catch { setenv(key, value); }
        }

        public IDictionary GetEnvironmentVariables()
        {
            return Environment.GetEnvironmentVariables();
        }
    }
}