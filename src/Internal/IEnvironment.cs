using System.Collections;

namespace Lambda.Native.Internal
{
    internal interface IEnvironment
    {
        void SetEnvironmentVariable(string key, string value);
        IDictionary GetEnvironmentVariables();
    }
}