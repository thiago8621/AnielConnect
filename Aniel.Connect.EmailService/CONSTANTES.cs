using Aniel.Connect.Core;
using log4net;
using Sinapse.Util;
using System;
using System.Configuration;

namespace Aniel.Connect.EmailService
{
    public static class CONSTANTES
    {        
        public static String NOME_APLICACAO = "Aniel Connect - Email Service";

        public static ILog log4Net = LogManager.GetLogger("Aniel.Connect.EmailService");

        public static String UrlConnectAPI = ConfigurationManager.AppSettings["UrlConnectAPI"];

        public static String CaminhoFisico = ConfigurationManager.AppSettings["CaminhoFisico"];
        public static String CaminhoFisicoConnect = ConfigurationManager.AppSettings["CaminhoFisicoConnect"];
        public static Int32 SLA_PADRAO_ATENDIMENTO = 480;

        public static String VersaoCore = XMLService.GetParametroXml($@"{CaminhoFisico}\Build.config", "/configuration/appSettings/add", "key", "BuildCore");

        public static String URLPortalConnect = ConfigurationManager.AppSettings["UrlPortalConnect"];

        public static String EMAILS_OSDIGITAL = ConfigurationManager.AppSettings["EMAILS_OSDIGITAL"];
        

        public static String IMPORTAR_ATENDIMENTO = ConfigurationManager.AppSettings["IMPORTAR_ATENDIMENTO"];
        public static String IMPORTAR_OSDIGITAL = ConfigurationManager.AppSettings["IMPORTAR_OSDIGITAL"];
        public static String IMPORTAR_OSDIGITAL_APENASDATA = ConfigurationManager.AppSettings["IMPORTAR_OSDIGITAL_APENASDATA"];





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
