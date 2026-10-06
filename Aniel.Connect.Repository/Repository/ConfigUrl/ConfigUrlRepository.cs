using Aniel.T5.Model;
using NHibernate;
using Sinapse.DAO.repository;

namespace Aniel.Connect.Repository.ConfigUrl
{
    public class ConfigUrlRepository : IConfigUrlRepository
    {
        private readonly ISession _sessionNhT5;

        public ConfigUrlRepository(ISession sessionNh)
        {
            this._sessionNhT5 = sessionNh;
        }

        public string GetUrlConnect()
        {
            using (var repo = new Conexao<Tb_Config_Url>(_sessionNhT5))
            {
                return repo.selectFirst(p => p.Sistema == "CONNECT").Result?.Url;
            }
        }

        public string GetUrlEVA()
        {
            using (var repo = new Conexao<Tb_Config_Url>(_sessionNhT5)) {
                return repo.selectFirst(p => p.Sistema == "EVA").Result?.Url;
            }
        }

        public string GetUrlPremier()
        {
            using (var repo = new Conexao<Tb_Config_Url>(_sessionNhT5))
            {
                return repo.selectFirst(p => p.Sistema == "PREMIER").Result?.Url;
            }
        }

        public string GetUrlApiAniel()
        {
            using (var repo = new Conexao<Tb_Config_Url>(_sessionNhT5))
            {
                return repo.selectFirst(p => p.Sistema == "API_ANIEL").Result?.Url;
            }
        }

        public string GetUrlApiFinanceiro()
        {
            using (var repo = new Conexao<Tb_Config_Url>(_sessionNhT5))
            {
                return repo.selectFirst(p => p.Sistema == "EVA").Result?.Url;
			}
	    }
        
        public string GetUrlOSRM()
        {
            using (var repo = new Conexao<Tb_Config_Url>(_sessionNhT5))
            {
                return repo.selectFirst(p => p.Sistema == "MAPA").Result?.Url;
            }
        }

        public string GetUrlIntegra()
        {
            using (var repo = new Conexao<Tb_Config_Url>(_sessionNhT5))
            {
                return repo.selectFirst(p => p.Sistema == "INTEGRA").Result?.Url;
            }
        }
    }
}
