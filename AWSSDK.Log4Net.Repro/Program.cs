using log4net;
using log4net.Config;
using System;
using System.IO;
using Topshelf;

namespace AWSSDK.Log4Net.Repro
{
    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            XmlConfigurator.ConfigureAndWatch(new FileInfo("Log4Net.Config"));
            try
            {
                Log.Info("Starting...");
                HostFactory.Run(x =>
                {
                    x.SetDescription("AWSSDK.Log4Net.Repro.Service");
                    x.SetDisplayName("AWSSDK.Log4Net.Repro.Service");
                    x.SetServiceName("AWSSDK.Log4Net.Repro.Service");
                    x.StartAutomatically();

                    x.RunAsLocalSystem();
                    x.UseLog4Net();
                    
                    x.EnableServiceRecovery(r =>
                    {
                        r.RestartService(1);
                        r.SetResetPeriod(1); 
                    });

                    x.OnException(ex =>
                    {
                        File.AppendAllText(@"C:\Temp\Service.txt", ex.ToString());
                    });

                    x.Service<TestService>(s =>
                    {
                        s.ConstructUsing(name =>
                        {
                              return new TestService();
                        });
                        s.WhenStarted(acs => acs.Start());
                        s.WhenStopped(acs => acs.Stop());
                        s.WhenShutdown(acs => acs.Stop());
                    });
                });
            }
            catch (Exception e)
            {
                Log.ErrorFormat("A Fatal error has occured while initilaising the service it will restart \r\n\n{0}", e);
            }
        }
    }
}
