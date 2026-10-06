using NHibernate;
using Sinapse.DAO.repository;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.PerfilVisualizacaoVendaParamEquipeVenda
{
    public class PerfilVisualizacaoVendaParamEquipeVendaRepository : IPerfilVisualizacaoVendaParamEquipeVendaRepository
    {
        private readonly ISession _sessionNh;

        public PerfilVisualizacaoVendaParamEquipeVendaRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public IQueryable<Sinapse.Model.PerfilVisualizacaoVendaParamEquipeVenda> GetAll()
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.PerfilVisualizacaoVendaParamEquipeVenda>(this._sessionNh))
            {
                return repoParamSetor.QueryableSelect();
            }
        }

        public Sinapse.Model.PerfilVisualizacaoVendaParamEquipeVenda GetById(long id)
        {
            return this.GetAll()
                       .Where(p => p.Id == id)
                       .FirstOrDefault();
        }

        public IEnumerable<Sinapse.Model.PerfilVisualizacaoVendaParamEquipeVenda> GetParamsByPerfil(long idPerfil)
        {
            return this.GetAll()
                       .Where(p => p.PerfilVisualizacao.Id == idPerfil)
                       .ToArray();
        }

        public void Save(Sinapse.Model.PerfilVisualizacaoVendaParamEquipeVenda param)
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.PerfilVisualizacaoVendaParamEquipeVenda>(this._sessionNh))
            {
                repoParamSetor.Save(param).Wait();
            }
        }

        public void Update(Sinapse.Model.PerfilVisualizacaoVendaParamEquipeVenda param)
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.PerfilVisualizacaoVendaParamEquipeVenda>(this._sessionNh))
            {
                repoParamSetor.Update(param).Wait();
            }
        }

        public void Delete(Sinapse.Model.PerfilVisualizacaoVendaParamEquipeVenda param)
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.PerfilVisualizacaoVendaParamEquipeVenda>(this._sessionNh))
            {
                repoParamSetor.Remove(param).Wait();
            }
        }
    }
}
