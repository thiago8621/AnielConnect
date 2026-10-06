using NHibernate;
using Sinapse.DAO.repository;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.PerfilVisualizacaoVendaParamRegiao
{
    public class PerfilVisualizacaoVendaParamRegiaoRepository : IPerfilVisualizacaoVendaParamRegiaoRepository
    {
        private readonly ISession _sessionNh;

        public PerfilVisualizacaoVendaParamRegiaoRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public IQueryable<Sinapse.Model.PerfilVisualizacaoVendaParamRegiao> GetAll()
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.PerfilVisualizacaoVendaParamRegiao>(this._sessionNh))
            {
                return repoParamSetor.QueryableSelect();
            }
        }

        public Sinapse.Model.PerfilVisualizacaoVendaParamRegiao GetById(long id)
        {
            return this.GetAll()
                       .Where(p => p.Id == id)
                       .FirstOrDefault();
        }

        public IEnumerable<Sinapse.Model.PerfilVisualizacaoVendaParamRegiao> GetParamsByPerfil(long idPerfil)
        {
            return this.GetAll()
                       .Where(p => p.PerfilVisualizacao.Id == idPerfil)
                       .ToArray();
        }

        public void Save(Sinapse.Model.PerfilVisualizacaoVendaParamRegiao param)
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.PerfilVisualizacaoVendaParamRegiao>(this._sessionNh))
            {
                repoParamSetor.Save(param).Wait();
            }
        }

        public void Update(Sinapse.Model.PerfilVisualizacaoVendaParamRegiao param)
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.PerfilVisualizacaoVendaParamRegiao>(this._sessionNh))
            {
                repoParamSetor.Update(param).Wait();
            }
        }

        public void Delete(Sinapse.Model.PerfilVisualizacaoVendaParamRegiao param)
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.PerfilVisualizacaoVendaParamRegiao>(this._sessionNh))
            {
                repoParamSetor.Remove(param).Wait();
            }
        }
    }
}
