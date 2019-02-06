using System;

namespace LambdaNative.Internal
{
    internal class LambdaBootstrap : ILambdaBootstrap
    {
        private readonly ILambdaRuntime _runtime;
        private readonly IHandlerRunner _runner;

        public LambdaBootstrap(ILambdaRuntime runtime, IHandlerRunner runner)
        {
            _runtime = runtime;
            _runner = runner;
        }

        public void Initialize()
        {
            try
            {
                _runner.Initialize();
            }
            catch (Exception ex)
            {
                _runtime.ReportInitializationError(ex);
                throw;
            }
        }

        public void Run()
        {
            while (_runtime.KeepInvokeLoopRunning())
            {
                var requestId = string.Empty;

                try
                {
                    var invokeData = _runtime.GetNextInvocation();
                    requestId = invokeData.RequestId;

                    _runtime.Environment.SetEnvironmentVariable("_X_AMZN_TRACE_ID", invokeData.XAmznTraceId);

                    var outputStream = _runner.Handle(invokeData.InputStream, invokeData.LambdaContext);
                    _runtime.ReportInvocationSuccess(requestId, outputStream);
                }
                catch (Exception ex)
                {
                    _runtime.ReportInvocationError(requestId, ex);
                }
            }
        }
    }
}