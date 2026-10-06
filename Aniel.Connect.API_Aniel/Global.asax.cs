using Aniel.Connect.API_Aniel;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace AnielConnectIntegracoes
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            log4net.Config.XmlConfigurator.Configure();
        }

        public override void Init()
        {
            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (ISession sessionLog = HibernateUtil.GetSessionLog())
            using (ISession sessionFoto = HibernateUtil.GetSessionFoto())
            {

            }
        }
    }
}
