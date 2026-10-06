using Aniel.Connect.API_Aniel.ControllersWebApi;
using Aniel.Connect.Core;
using log4net;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Web;
using System.Linq;
using WebApiThrottle;

namespace Aniel.Connect.API_Aniel
{
    public static partial class CONSTANTES
    {
        public static ILog log4Net
        {
            get
            {
                return LogManager.GetLogger("Aniel.Connect.API_Aniel");
            }
        }

        public static String SIGLA_MODULO = Convert.ToString(ConfigurationManager.AppSettings["SIGLA_MODULO"]);

        public static String URLPortal
        {
            get
            {
                #if DEBUG
                    return "http://localhost/Aniel.Connect.API_Aniel";
                #else
                    return GetBaseUrl();
                #endif
            }
        }

        public static String URLFinanceiro = Convert.ToString(ConfigurationManager.AppSettings["URLPortalFinanceiro"]);

        //public static String CaminhoFisico = ConfigurationManager.AppSettings["CaminhoFisico"];
        public static string CaminhoFisico
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path).Replace(@"\bin", "").Replace(@"\Debug", "").Replace(@"\Release", "");
            }
        }

        public static string MKConnectionString = Convert.ToString(ConfigurationManager.AppSettings["MKConnectionString"]);

        public static String Versao = XMLService.GetParametroXml($@"{CaminhoFisico}\Build.config", "/configuration/appSettings/add", "key", "Build");
        public static String VersaoCore = XMLService.GetParametroXml($@"{CaminhoFisico}\Build.config", "/configuration/appSettings/add", "key", "BuildCore");

        public static Int32 PaginacaoRegistros = Int32.Parse(Convert.ToString(ConfigurationManager.AppSettings["PaginacaoRegistros"]));

        private static String nomeEmpresaConfig;
        public static String GetEmpresaConfig()
        {
            try
            {
                if (String.IsNullOrEmpty(nomeEmpresaConfig))
                    nomeEmpresaConfig = $@"{CaminhoFisico}\Empresa.config";

                if (!File.Exists(nomeEmpresaConfig))
                {
                    String xml = @"<?xml version='1.0' encoding='utf-8'?><configuration><appSettings></appSettings></configuration>";
                    File.WriteAllText(nomeEmpresaConfig, xml);
                }
            }
            catch (Exception ex)
            {
                log4Net.Error(ex);
            }
            return nomeEmpresaConfig;
        }
        //Configuracoes para o SERASA EXPERIAN
        public static String URLHomologacaoAuth = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "URLHomologacaoAuth");
        public static String URLProducaoAuth = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "URLProducaoAuth");
        public static String URLHomologacaoSerasa = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "URLHomologacaoSerasa");
        public static String URLProducaoSerasaExperian = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "URLProducaoSerasaExperian");
        public static String AmbienteTestesSerasa = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "AmbienteTestesSerasa");
        public static String Email_UsaSsl = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "Email_UsaSsl"); // Convert.ToString(ConfigurationManager.AppSettings["Email_Senha"]);
        public static String Email_De = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "Email_De"); // Convert.ToString(ConfigurationManager.AppSettings["Email_De"]);
        public static String Email_Smtp = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "Email_Smtp"); // Convert.ToString(ConfigurationManager.AppSettings["Email_Smtp"]);
        public static String Email_Usuario = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "Email_Usuario");
        public static String Email_Senha = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "Email_Senha"); // Convert.ToString(ConfigurationManager.AppSettings["Email_Senha"]);
        public static String CaminhoProjetoWebApi = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "CaminhoProjetoWebApi"); //Convert.ToString(ConfigurationManager.AppSettings["CaminhoProjetoWebApi"]);
        public static String URLConnect = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "Url_Connect");
        public static Boolean TrataErro = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "TrataErro") == "S"; //(ConfigurationManager.AppSettings["TrataErro"] ?? "S") == "S";
        public static String CloudPortNat = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "CloudPortNat"); //Convert.ToString(ConfigurationManager.AppSettings["CaminhoProjetoWebApi"]);
        public static String CaminhoProjetoPremier = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "CaminhoProjetoPremier");
        public static String CaminhoProjetoWebApiPremier = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "CaminhoProjetoWebApiPremier");
        public static String KeyUrlPreQual = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "KeyUrlPreQual");
        public static string contractid = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "contractid");
        public static string keyblip = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "keyblip");
        public static string AnielRelatorios = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "AnielRelatorios");        
        public static string CaminhoSavePdf = $"{CONSTANTES.CaminhoFisico}/Arquivos/Checklist";
        public static string ChaveBaseIManager = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "ChaveBaseIManager");
        public static String TokenGoogleAPI = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "TokenGoogleAPI"); // Convert.ToString(ConfigurationManager.AppSettings["Email_De"]);
        /// <summary>
        /// String de conexão para o banco Postgre do Voalle.
        /// </summary>
        public static String VoalleConnectionString = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "VoalleConnectionString");
        public static int? PorSegundo = Convert.ToInt32(XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "PorSegundo")); 
        public static int? PorMinuto = Convert.ToInt32(XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "PorMinuto")); 
        public static int? PorHora = Convert.ToInt32(XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "PorHora"));
        public static int? PorDia = Convert.ToInt32(XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "PorDia"));
        public static Boolean? ClienteRateLimit = Convert.ToBoolean (Convert.ToInt64(XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "ClienteRateLimit")));
        public static List<string> IpWhitelist = (XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "IpWhitelist") ?? string.Empty)
        .Split(',')
        .Select(ip => ip.Trim())
        .Where(ip => !string.IsNullOrWhiteSpace(ip))
        .ToList();
        

        public static Dictionary<string, RateLimits> CarregarEndpointRules()
        {
            var json = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "EndpointRateLimits");
            return JsonService.DeserializeObject<Dictionary<string, RateLimits>>(json);
        }

        public static Int32 Email_Porta
        {
            get
            {
                try
                {
                    String temp = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "Email_Porta");
                    return Convert.ToInt32(temp);
                }
                catch
                {
                    return 0;
                }
            }
        }

        public static String UrlBaseSerasaAuth()
        {
            String urlBase = "";
            try
            {
                if (CONSTANTES.AmbienteTestesSerasa == "S")
                    urlBase = CONSTANTES.URLHomologacaoAuth;
                else
                    urlBase = CONSTANTES.URLProducaoAuth;
            }
            catch (Exception ex)
            {
                log4Net.Error(ex);
            }

            return urlBase;
        }

        public static String UrlBaseSerasaExperian()
        {
            String urlBase = "";
            try
            {
                if (CONSTANTES.AmbienteTestesSerasa == "S")
                    urlBase = CONSTANTES.URLHomologacaoSerasa;
                else
                    urlBase = CONSTANTES.URLProducaoSerasaExperian;
            }
            catch (Exception ex)
            {
                log4Net.Error(ex);
            }

            return urlBase;
        }

        //public static List<SistemaWebHabilitado> Sistemas = JsonService.DeserializeJasonMapper<List<SistemaWebHabilitado>>(ConfigurationManager.AppSettings["Sistemas"].ToString());

        public static String CaminhoUpLoad = CaminhoFisico + "\\UpLoad";

        public static String URLHUBDESENVOLVEDOR_BASE = "https://ws.hubdodesenvolvedor.com.br";

        public static String URLHUBDESENVOLVEDOR_CNPJ = "http://ws.hubdodesenvolvedor.com.br/v2/cnpj/?token=26937230TpcxDSwjKu48634352&cnpj={0}";

        public static String URLHUBDESENVOLVEDOR_CPF = "https://ws.hubdodesenvolvedor.com.br/v2/cpf/?token=26937230TpcxDSwjKu48634352&cpf={0}&data={1}";

        public static String URLROUTE = ConfigurationManager.AppSettings["URLROUTE"] != null ? Convert.ToString(ConfigurationManager.AppSettings["URLROUTE"]) : "http://router.project-osrm.org/route/v1/driving/";

        public static Int32 SLA_PADRAO_ATENDIMENTO = 480;
        public static Int32 PAGE_LENGTH = 3;

        public static String USUARIOOLA = "OLA";
        public static String OBSERVACAOOLA = "SERVICO INSERIDO PELO CLIENTE";
        public static Boolean EnableOptimizations = (ConfigurationManager.AppSettings["EnableOptimizations"] != null ?
                                                                          Convert.ToString(ConfigurationManager.AppSettings["EnableOptimizations"])
                                                                          : "N") == "S";

        public static String GetVersaoAnielCore()
        {
            String versaoAssemby = typeof(BaseController).Assembly.GetName().Version.ToString();

            if (FileUtil.VersaoMaior(versaoAssemby, VersaoCore))
            {
                throw new Exception($"Versão esperada do Core {VersaoCore}. Versão atual {versaoAssemby}. Atualize a versão do Aniel.ConnectCore");
            }

            return versaoAssemby;

        }

        public static String LogoCliente64
        {
            get
            {
                try
                {
                    var image = Image.FromFile(Path.Combine(CaminhoFisico, @"Arquivos\LogoCliente.jpg"));
                    String retorno = ImageService.ConvertTo64(image);
                    image.Dispose();
                    return retorno;
                }
                catch
                {
                    return String.Empty;
                }
            }
        }

        public static string GetBaseUrlConnect()
        {
            try
            {
                return Configuracoes_GeraisApiController.GetUrlConnect();
            }
            catch
            {
                return XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "Url_Connect");
            }
        }

        public static string GetBaseUrl()
        {
            try
            {
                if (String.IsNullOrEmpty(CloudPortNat))
                {
                    var request = HttpContext.Current.Request;
                    var appUrl = HttpRuntime.AppDomainAppVirtualPath;
                    var baseUrl = $@"{request.Url.Scheme}://{request.Url.Authority}{appUrl}";

                    return baseUrl;
                }
                else
                {
                    var request = HttpContext.Current.Request;
                    var appUrl = HttpRuntime.AppDomainAppVirtualPath;
                    var port = CloudPortNat;
                    var baseUrl = $@"{request.Url.Scheme}://{request.Url.Host}:{port}{appUrl}";

                    return baseUrl;
                }
            }
            catch
            {
                return "";
            }
        }

        

    }
}