using Aniel.Connect.Core;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using HibernatingRhinos.Profiler.Appender.NHibernate;
using NHibernate;
using Sinapse.Core;
using Sinapse.DAO.repository;
using System;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Aniel.Connect
{
    
    public class MvcApplication : System.Web.HttpApplication
    {
        private BackgroundJobServer _hangfireJobServer;
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            System.Web.Http.GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            UserConnectionControlConfig.tipoConnection = TipoCC.Web;
            UserConnectionControlConfig.sistema = SistemaWeb.Connect;

            #if !DEBUG
                Hangfire.GlobalConfiguration.Configuration
                    .UseRecommendedSerializerSettings()
                    .UseLog4NetLogProvider()
                    .UseMongoStorage(
                        CONSTANTES.HangfireDBConnectionString,
                        new MongoStorageOptions
                        {
                            MigrationOptions = new MongoMigrationOptions
                            {
                                MigrationStrategy = new MigrateMongoMigrationStrategy(), 
                                BackupStrategy = new CollectionMongoBackupStrategy(),
                                BackupPostfix = "backup"
                            }
                        }
                    );
            #endif

            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pt-BR");

            log4net.Config.XmlConfigurator.Configure();
            try
            {
                NHibernateProfiler.Initialize();
            }
            catch
            {

            }

            CONSTANTES.GetVersaoAnielCore();
            //_hangfireJobServer = new BackgroundJobServer();
        }

        public override void Init()
        {
            using (ISession session = HibernateUtil.GetSession())
            {

            }

            using (ISession session = HibernateUtil.GetSessionT5())
            {

            }

            using (ISession session = HibernateUtil.GetSessionLog())
            {

            }

            using (ISession session = HibernateUtil.GetSessionFoto())
            {

            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            HttpContext.Current.Session.Timeout = 240;

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            Response.AppendHeader("Cache-Control", "no-cache, no-store, must-revalidate"); // HTTP 1.1.
            Response.AppendHeader("Pragma", "no-cache"); // HTTP 1.0.
            Response.AppendHeader("Expires", "0"); // Proxies.
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            if (CONSTANTES.TrataErro)
            {
                Exception ex = Server.GetLastError(); // Obtém o erro detectado.

                String url;
                if (ex is HttpException && ((HttpException)ex).GetHttpCode() == 404)
                {
                    url = String.Concat(CONSTANTES.URLPortal, "/", CultureHelper.GetCurrentCulture(), "/Home/Erro404") + "?url=" + Request.Url.AbsoluteUri;
                    //EmailUtil.EnviarEmail("Erro 404 - Página não encontrada", "Usuário ou sistema tentou acessar página inexistente:" + Request.Url.AbsoluteUri);
                }
                else
                {
                    url = String.Concat(CONSTANTES.URLPortal, "/", CultureHelper.GetCurrentCulture(), "/Home/Erro500");
                    //EmailUtil.EnviarEmail("Erro 500", User?.Identity?.GetUserId(), ex);
                }

                this.Response.RedirectPermanent(url); // Encaminha a requisição para a página de erro adequada.
            }
        }

        protected void Application_End(object sender, EventArgs e)
        {
            //_hangfireJobServer.Dispose();
        }

    }
}
