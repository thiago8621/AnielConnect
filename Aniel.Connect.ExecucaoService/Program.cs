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
        static void Main()
        {
            ServiceBase[] ServicesToRun;

            ServicesToRun = new ServiceBase[]
            {
                new Service1()
            };

            log4net.Config.XmlConfigurator.Configure();

            Debugger.Launch();

            try
            {
                CONSTANTES.GetVersaoAnielCore();
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex);
            }

            ServiceBase.Run(ServicesToRun);
        }
    }
}
