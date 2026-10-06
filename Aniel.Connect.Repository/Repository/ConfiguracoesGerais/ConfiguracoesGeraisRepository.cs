using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System.Linq;

namespace Aniel.Connect.Repository.ConfiguracoesGerais
{
    public class ConfiguracoesGeraisRepository : IConfiguracoesGeraisRepository
    {
        private readonly ISession _sessionNh;

        public ConfiguracoesGeraisRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public Configuracoes_Gerais GetConfiguracoesGerais()
        {
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(this._sessionNh))
            {
                return repoConfig.QueryableSelect().FirstOrDefault();
            }
        }

        public string GetUrlOla()
        {
            using (var repoUrl = new Conexao<URL_Sistema>(this._sessionNh))
            {
                return repoUrl.selectFirst(s => s.Sistema == Sistema.OlaCliente).Result?.URL;
            }
        }

        public Configuracoes_Gerais GetFirstConfigGeral()
        {
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(this._sessionNh))
            {
                return repoConfig.selectFirst().Result;
            }
        }
    }
}
