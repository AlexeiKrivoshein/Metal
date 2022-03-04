using MetalServer.Handler;
using System;
using System.Threading.Tasks;
using MetalTransport.Handler;
using MetalServer.Server;
using MetalDiagnostic.Logger;
using MetalDAL.Manager;
using System.ServiceProcess;
using System.Threading;
using System.Diagnostics;

namespace MetalServer
{
    class Program
    {
        static void Main()
        {
            LogService.InitLogger(LoggerType.Server);

            if (Environment.UserInteractive)
            {
                using (var service = new MetallServerHost())
                {
                    service.StartService();
                    Console.ReadLine();
                    service.StopService();
                }
            }
            else
            {
                var service = new MetallServerHost();
                ServiceBase.Run(new ServiceBase[] { service });
            }
        }
    }
}
