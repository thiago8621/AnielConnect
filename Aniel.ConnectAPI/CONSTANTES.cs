using Aniel.ConnectAPI.Dal.Service;
using System;


namespace Aniel.ConnectAPI
{
    public static class CONSTANTES
    {
        public static String Versao = ConfigurationManager.Build["Versao"];
        public static String TrataErro = ConfigurationManager.AppSetting["TrataErro"];
    }
}
