using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using log4net;
using Sinapse.Util;
using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace Aniel.Connect
{
    public static partial class CONSTANTES 
    {
        public static ILog log4Net
        {
            get
            {
                return LogManager.GetLogger(typeof(CONSTANTES).Assembly.GetName().Name);
            }
        }

        public static ILog logIntegracaoComercial
        {
            get
            {
                return LogManager.GetLogger("Comercial");
            }
        }

        public static string MensagemPadraoConclusaoChecklist = "Obrigado por preencher nosso formulário!";

        public static String SIGLA_MODULO = Convert.ToString(ConfigurationManager.AppSettings["SIGLA_MODULO"]);


        #region URL PORTAL

        #if DEBUG
            //Publicação local
            public static String URLPortal = "http://localhost/Aniel.Connect";
            public static String URLPortalInterno = "http://localhost/Aniel.Connect";
            public static String URLPortalApiAniel = "http://localhost/Aniel.Connect.API_Aniel";
            public static String URLPortalEVA = "http://localhost/Aniel.EVA";
        #else
            //Publicação do Premier
            //public static String URLPortal = Configuracoes_GeraisApiController.GetUrlPremier();

            //Publicação do Connect
            public static String URLPortal = Configuracoes_GeraisApiController.GetUrlConnect();
            public static String URLPortalInterno = String.IsNullOrEmpty(XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "URLPortalInterno")) ? Configuracoes_GeraisApiController.GetUrlConnect() : XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "URLPortalInterno")
            ;
            public static String URLPortalApiAniel = Configuracoes_GeraisApiController.GetUrlApiAniel();

            //Publicacao do EVA
            public static String URLPortalEVA = Configuracoes_GeraisApiController.GetUrlEVA();
        #endif

        #endregion

        public static String URLFinanceiro = GetBaseUrlFinanceiro();
        public static String HangfireDBConnectionString = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "HangfireDBConnectionString");
        public static String CaminhoLogIntegracaoConfigIntegrator
        {
            get
            {
                return XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "CaminhoLogIntegracaoConfigIntegrator");
            }
        } 

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

        public static String UrlBaseSerasaAuth()
        {
            String urlBase = "";
            try
            {
                if (CONSTANTES.AmbienteTestesSerasa == "S")
                    urlBase = CONSTANTES.URLHomologacaoSerasaAuth;
                else
                    urlBase = CONSTANTES.URLProducaoSerasaAuth;
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

        public static String TokenGoogleAPI = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "TokenGoogleAPI"); // Convert.ToString(ConfigurationManager.AppSettings["Email_De"]);
        public static String Email_De = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "Email_De"); // Convert.ToString(ConfigurationManager.AppSettings["Email_De"]);
        public static String Email_Smtp = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "Email_Smtp"); // Convert.ToString(ConfigurationManager.AppSettings["Email_Smtp"]);
        public static String Email_Usuario = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "Email_Usuario"); // Convert.ToString(ConfigurationManager.AppSettings["Email_Senha"]);
        public static String Email_Senha = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "Email_Senha"); // Convert.ToString(ConfigurationManager.AppSettings["Email_Senha"]);
        public static String Email_UsaSsl = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "Email_UsaSsl"); // Convert.ToString(ConfigurationManager.AppSettings["Email_Senha"]);
        public static String CaminhoProjetoTouch = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "CaminhoProjetoTouch"); //Convert.ToString(ConfigurationManager.AppSettings["CaminhoProjetoWebApi"]);
        public static String CaminhoProjetoWebApi = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "CaminhoProjetoWebApi"); //Convert.ToString(ConfigurationManager.AppSettings["CaminhoProjetoWebApi"]);
        // Caminho do Aniel.Connect no qual serão salvos arquivos gerados pelo Premier
        public static String CaminhoProjetoPremier = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "CaminhoProjetoPremier");
        // Url que será utilizada para acesso externo aos arquivos salvos pelo Premier
        public static String CaminhoProjetoWebApiPremier = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "CaminhoProjetoWebApiPremier");
        public static String CloudPortNat = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "CloudPortNat"); //Convert.ToString(ConfigurationManager.AppSettings["CaminhoProjetoWebApi"]);
        public static String LogExecucaoAnielChannel = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "LogExecucaoAnielChannel");

        public static String KeyUrlPreQual = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "KeyUrlPreQual");
        

        public static String MKConnectionString = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "MKConnectionString");
        public static String VoalleConnectionString = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "VoalleConnectionString");
        public static String VoalleUtilizaDw = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "VoalleUtilizaDw");

        public static String crmContactOriginCode = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "crmContactOriginCode");
        public static String integratorAlias = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "integratorAlias");
        public static String integrationCode = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "integrationCode");

        public static String PlayhubApiKey = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "PlayhubApiKey");
        public static String PlayhubApiSecret = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "PlayhubApiSecret");

        public static Boolean TrataErro = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "TrataErro") == "S"; //(ConfigurationManager.AppSettings["TrataErro"] ?? "S") == "S";

        public static Boolean UsaRadiusSemContainer = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "UsaRadiusSemContainer") == "S";
        public static string contractid = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "contractid");
        public static string keyblip = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "keyblip");
        public static string BloquearContratoTask = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "BloquearContratoTask");
        public static string AgendarVerificacaoAgendamentoTask = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "AgendarVerificacaoAgendamentoTask");
        public static string AtualizarVeiculoTask = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "AtualizarVeiculoTask");
        public static string PreencherQuilometragemTask = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "PreencherQuilometragemTask");
        public static string UtilizaBaixaOSAutomatica = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "UtilizaBaixaOSAutomatica");
        public static string UtilizaEnvioLogsEmail = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "UtilizaEnvioLogsEmail");
        public static string RoteirizacaoSlaTask = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "RoteirizacaoSlaTask");
        public static string RoteirizacaoEncaixe = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "RoteirizacaoEncaixe");
        public static string EmailFilialEnvio = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "EmailFilial");
        public static string TelefoneFilial = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "TelefoneFilial");
        public static string UsaRadius = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "UsaRadius");
        public static string UtilizarTransAut = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "UtilizarTransAut");
        public static string FinalizarAtendimentoWhatsApp = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "FinalizarAtendimentoWhatsApp");
        public static string EnviarSMSAlertaSOS = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "EnviarSMSAlertaSOS");


        //Configuracoes para o SERPRO - GOV
        public static String URLProducaoSERPRO = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "UrlProducaoSERPRO");
        public static String AmbienteTestesSERPRO = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "AmbienteTestesSERPRO");
        public static String TokenConsultaTrial = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "TokenConsultaTrial");

        //Configuracoes para o SERASA EXPERIAN
        public static String URLHomologacaoSerasaAuth = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "URLHomologacaoAuth");
        public static String URLProducaoSerasaAuth = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "URLProducaoAuth");
        public static String URLHomologacaoSerasa = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "URLHomologacaoSerasa");
        public static String URLProducaoSerasaExperian = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "URLProducaoSerasaExperian");
        public static String AmbienteTestesSerasa = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "AmbienteTestesSerasa");
        public static String ID_MOTIVO_INCLUSAO_IXC = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "ID_MOTIVO_INCLUSAO_IXC");


        public static string LoginHubSoft = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "AutenticacaoSoftHub");
        public static string CpfCnpjSemCaracteres = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "CpfCnpjSemCaracteres");
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

        public static string URLPortalWebSocket
        {
            get
            {
                return URLPortal.Replace("http://", "ws://").Replace("https://", "wss://");
            }
        }

        //public static List<SistemaWebHabilitado> Sistemas = JsonService.DeserializeJasonMapper<List<SistemaWebHabilitado>>(ConfigurationManager.AppSettings["Sistemas"].ToString());

        public static String CaminhoUpLoad = CaminhoFisico + "\\UpLoad";

        public static String URLHUBDESENVOLVEDOR_BASE = "https://ws.hubdodesenvolvedor.com.br";

        public static String URLHUBDESENVOLVEDOR_CNPJ = "http://ws.hubdodesenvolvedor.com.br/v2/cnpj/?token=26937230TpcxDSwjKu48634352&cnpj={0}";

        public static String URLHUBDESENVOLVEDOR_CPF = "https://ws.hubdodesenvolvedor.com.br/v2/cpf/?token=26937230TpcxDSwjKu48634352&cpf={0}&data={1}";
       
    

        public static String URLROUTE = ConfigurationManager.AppSettings["URLROUTE"] != null ? Convert.ToString(ConfigurationManager.AppSettings["URLROUTE"]) : "https://router.project-osrm.org/";

        public static String URLROUTEOSRM = ConfigurationManager.AppSettings["URLROUTEOSRM"] != null ? Convert.ToString(ConfigurationManager.AppSettings["URLROUTEOSRM"]) : "https://router.project-osrm.org/table/v1/driving/";
        
        public static String URLGRAFANA = Convert.ToString(XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "URL_Grafana"));
        public static String URLGRAFANALOCAL = Convert.ToString(XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "URL_Grafana_Local"));
               
        public static String APIKEY_GRAFANA = Convert.ToString(XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "ApiKeyGrafna"));

        public static Int32 SLA_PADRAO_ATENDIMENTO = 480;

        public static String USUARIOOLA = "OLA";
        public static String OBSERVACAOOLA = "SERVICO INSERIDO PELO CLIENTE";
        public static Boolean EnableOptimizations = (ConfigurationManager.AppSettings["EnableOptimizations"] != null ?
                                                                          Convert.ToString(ConfigurationManager.AppSettings["EnableOptimizations"])
                                                                          : "N") == "S";

        public static string SenhaOla = Convert.ToString(XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "SenhaOla"));
        public static bool UtilizaOla = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "UtilizaOla") == "S";
        public static String PadraoIntegracaoTipo = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "PadraoIntegracaoTipo");

        //Utilizado em integração com o HubSoft no método MovimentoCliente, 
        //responsável por vincular o material ao contrato do cliente
        public static Int32 IdTipoMovimentoComodato
        {
            get
            {
                try
                {
                    String temp = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "IdTipoMovimentoComodato");
                    return Convert.ToInt32(temp);
                }
                catch
                {
                    return 0;
                }
            }
        }

        public static Boolean UtilizaLocalEstoqueTecnico = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "UtilizaLocalEstoqueTecnico") == "S";

        public static Boolean DesativaCompraAplicacao = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "DESATIVA_COMPRA_APLIC") == "S";

        //Ativa movimentação de material no Connect durante o provisionamento.
        public static Boolean AtivaMovimentacaoMat = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "ATIVA_MOV_MAT") == "S";

        #region Credenciais Firebase Cloud Messaging
        /*public static string ProjectIdGoogle_Connect = Convert.ToString(XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "ProjectIdGoogle_Connect"));
        public static string JsonCredencialGoogle_Connect = Convert.ToString(XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "CaminhoJsonCredencialGoogle_Connect"));
        public static string ProjectIdGoogle_Touch = Convert.ToString(XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "ProjectIdGoogle_Touch"));
        public static string JsonCredencialGoogle_Touch = Convert.ToString(XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "CaminhoJsonCredencialGoogle_Touch"));*/
        public static string ProjectIdGoogle_Connect = "anielconnect-4ec81";
        public static string JsonCredencialGoogle_Connect = $@"{CaminhoFisico}\anielconnect-4ec81-firebase-adminsdk-n5jra-4c33219fd8.json";
        public static string ProjectIdGoogle_Touch = "anieltouch-440b6";
        public static string JsonCredencialGoogle_Touch = $@"{CaminhoFisico}\anieltouch-440b6-firebase-adminsdk-0maz9-5492907e1c.json";
        public static string ProjectIdGoogle_Point = "anielpointnovo";
        public static string JsonCredencialGoogle_Point = $@"{CaminhoFisico}\anielpointnovo-firebase-adminsdk-c52x2-f55e8e98cf.json";
        public static string ProjectIdGoogle_Ola = "ola-cliente";
        public static string JsonCredencialGoogle_Ola = $@"{CaminhoFisico}\ola-cliente-firebase-adminsdk-qrjch-4ad9965b4f.json";
        #endregion

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

        public static bool ExibeVencimentoEmail = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "ExibeVencimentoEmail") == "S";
        public static bool ExibeDiasGratisEmail = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "ExibeDiasGratisEmail") == "S";
        public static bool ExibeValorEmail = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "ExibeValorEmail") == "S";
        public static bool ExibeApenasComboEmail = XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "ExibeApenasComboEmail") == "S";

        public static string GetBaseUrl()
        {
            try
            {
                return Configuracoes_GeraisApiController.GetUrlConnect();
            }
            catch
            {
                return XMLService.GetParametroXml(GetEmpresaConfig(), "/configuration/appSettings/add", "key", "UrlPortal");
            }
        }


        public static string GetBaseUrlFinanceiro()
        {
            return Configuracoes_GeraisApiController.GetUrlEVA();
    	}

    }
}