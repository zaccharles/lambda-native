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
            this.LogDebug($"Setting environment variable {key} to '{value}'");

            // Workaround because Environment.SetEnvironmentVariable is not implemented in CoreRT.
            // See https://github.com/dotnet/corert/issues/6971 for more details.
            try
            {
                Environment.SetEnvironmentVariable(key, value);
            }
            catch (Exception ex)
            {
                this.LogDebug($"Environment threw {ex.GetType().Name}. Falling back to PInvoke");
                setenv(key, value);
            }
        }

        public IDictionary GetEnvironmentVariables()
        {
            this.LogDebug("Getting environment variables");

            var variables = Environment.GetEnvironmentVariables();

            foreach (string key in variables.Keys)
            {
                if (key == "AWS_SESSION_TOKEN" || key == "AWS_SECRET_ACCESS_KEY")
                {
                    this.LogDebug($"{key} = <hidden>");
                    continue;
                }

                this.LogDebug($"{key} = {variables[key]}");
            }

            return variables;
        }
    }
}