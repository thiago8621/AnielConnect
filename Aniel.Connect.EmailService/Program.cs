using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using NHibernate;
using Sinapse.DAO.repository;

namespace Aniel.Connect.EmailService
{
    static class Program
    {
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        static void Main()
        {
            try
            {
                ServiceBase[] ServicesToRun;

                ServicesToRun = new ServiceBase[]
                {
                new Service1()
                };

                log4net.Config.XmlConfigurator.Configure();

                //Debugger.Launch(); 
                //CONSTANTES.GetVersaoAnielCore();

                ServiceBase.Run(ServicesToRun);
            }
            catch (Exception e)
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                var caminhoFisico = Path.GetDirectoryName(path).Replace(@"\bin", "").Replace(@"\Debug", "").Replace(@"\Release", "");
                String _pasta = $@"{caminhoFisico}\logEmergencial";

                if (!Directory.Exists(_pasta))
                {
                    Directory.CreateDirectory(_pasta);
                }

                File.WriteAllText($"{_pasta}/log.txt", $"erro: {e.Message}. stack: {e.StackTrace}");
                Console.WriteLine($"erro: {e.Message} \n stackTrace: {e.ToString()} \n InnerException: {e.InnerException?.ToString()}");
            }

        }
    }
}
