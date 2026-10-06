using System;
using System.Diagnostics;
using System.ServiceProcess;

namespace Aniel.Connect.ExecucaoService
{
    static class Program
    {
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        static void Main(string[] args)
        {
            Service1 service = new Service1();
            if (Environment.UserInteractive)
            {
                Console.WriteLine("teste");
                service.RunAsConsole(args);
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] { service };
                Debugger.Launch();
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
