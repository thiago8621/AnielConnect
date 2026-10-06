using NHibernate;
using Sinapse.DAO.repository;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.PerfilVisualizacaoVendaParamMeioCaptacao
{
    public class PerfilVisualizacaoVendaParamMeioCaptacaoRepository : IPerfilVisualizacaoVendaParamMeioCaptacaoRepository
    {
        private readonly ISession _sessionNh;

        public PerfilVisualizacaoVendaParamMeioCaptacaoRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public IQueryable<Sinapse.Model.PerfilVisualizacaoVendaParamMeioCaptacao> GetAll()
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.PerfilVisualizacaoVendaParamMeioCaptacao>(this._sessionNh))
            {
                return repoParamSetor.QueryableSelect();
            }
        }

        public Sinapse.Model.PerfilVisualizacaoVendaParamMeioCaptacao GetById(long id)
        {
            return this.GetAll()
                       .Where(p => p.Id == id)
                       .FirstOrDefault();
        }

        public IEnumerable<Sinapse.Model.PerfilVisualizacaoVendaParamMeioCaptacao> GetParamsByPerfil(long idPerfil)
        {
            return this.GetAll()
                       .Where(p => p.PerfilVisualizacao.Id == idPerfil)
                       .ToArray();
        }

        public void Save(Sinapse.Model.PerfilVisualizacaoVendaParamMeioCaptacao param)
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.PerfilVisualizacaoVendaParamMeioCaptacao>(this._sessionNh))
            {
                repoParamSetor.Save(param).Wait();
            }
        }

        public void Update(Sinapse.Model.PerfilVisualizacaoVendaParamMeioCaptacao param)
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.PerfilVisualizacaoVendaParamMeioCaptacao>(this._sessionNh))
            {
                repoParamSetor.Update(param).Wait();
            }
        }

        public void Delete(Sinapse.Model.PerfilVisualizacaoVendaParamMeioCaptacao param)
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.PerfilVisualizacaoVendaParamMeioCaptacao>(this._sessionNh))
            {
                repoParamSetor.Remove(param).Wait();
            }
        }
    }
}
