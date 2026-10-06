using Aniel.Connect.Core;
using log4net;
using Sinapse.Util;
using System;
using System.Configuration;

namespace Aniel.Connect.ExecucaoService
{
    public static class CONSTANTES
    {        
        public static String NOME_APLICACAO = "Aniel Connect - Execucao Automática";

        public static ILog log4Net = LogManager.GetLogger("Aniel.Connect.ExecucaoService");
        public static ILog log4NetReincidencia = LogManager.GetLogger("Aniel.Connect.ExecucaoService.Reincidencia");
        

        public static String UrlConnectAPI = ConfigurationManager.AppSettings["UrlConnectAPI"];
        
        public static String CaminhoFisico = ConfigurationManager.AppSettings["CaminhoFisico"];
        

        public static String VersaoCore = XMLService.GetParametroXml($@"{CaminhoFisico}\Build.config", "/configuration/appSettings/add", "key", "BuildCore");

        public static String GetVersaoAnielCore()
        {
            String versaoAssemby = typeof(BaseController).Assembly.GetName().Version.ToString();

            if (FileUtil.VersaoMaior(versaoAssemby, VersaoCore))
            {
                throw new Exception($"Versão esperada do Core {VersaoCore}. Versão atual {versaoAssemby}. Atualize a versão do Aniel.ConnectCore");
            }

            return versaoAssemby;

        }
    }
}
