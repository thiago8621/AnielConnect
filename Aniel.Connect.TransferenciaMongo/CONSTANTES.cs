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

        public static ILog log4Net = LogManager.GetLogger("Aniel.Connect.TransferenciaMongo");

        public static String UrlConnectAPI = ConfigurationManager.AppSettings["UrlConnectAPI"];
        
        public static String CaminhoFisico = ConfigurationManager.AppSettings["CaminhoFisico"];
        public static String DataInicio = ConfigurationManager.AppSettings["DataInicio"];
        public static String DataFim = ConfigurationManager.AppSettings["DataFim"];

        
    }
}
