using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Repository.ConfigUrl;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class Configuracoes_GeraisApiController: BaseApiController
    {
        public static string GetUrlConnect()
        {
            using (var session = HibernateUtil.GetSessionT5())
            {
                IConfigUrlRepository repo = new ConfigUrlRepository(session);
                ConfigUrlBusiness business = new ConfigUrlBusiness(repo);
                return business.GetUrlConnect();
            }
        }
    }
}