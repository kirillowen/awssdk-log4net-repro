using System;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace AWSSDK.Log4Net.Repro
{
    public class TestService
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private CancellationTokenSource _cancel;

        public TestService()
        {
            _cancel = new CancellationTokenSource();

            Task.Run(() => Background(_cancel.Token));
        }

        public bool Start()
        {
            return true;
        }

        public bool Stop()
        {
            return true;
        }
        async Task Background(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000, cancellationToken);
                
                Log.Info($"Service running at: {DateTime.UtcNow}");
            }
        }
    }
}
