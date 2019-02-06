using System.Collections;

namespace LambdaNative.Internal
{
    internal interface IEnvironment
    {
        void SetEnvironmentVariable(string key, string value);
        IDictionary GetEnvironmentVariables();
    }
}