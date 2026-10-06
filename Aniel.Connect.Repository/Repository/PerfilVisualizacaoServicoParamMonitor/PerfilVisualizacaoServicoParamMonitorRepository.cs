using NHibernate;
using Sinapse.DAO.repository;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.PerfilVisualizacaoServicoParamMonitor
{
    public class PerfilVisualizacaoServicoParamMonitorRepository : IPerfilVisualizacaoServicoParamMonitorRepository
    {
        private readonly ISession _sessionNh;

        public PerfilVisualizacaoServicoParamMonitorRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public IQueryable<Sinapse.Model.PerfilVisualizacaoServicoParamMonitor> GetAll()
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.PerfilVisualizacaoServicoParamMonitor>(this._sessionNh))
            {
                return repoParamSetor.QueryableSelect();
            }
        }

        public Sinapse.Model.PerfilVisualizacaoServicoParamMonitor GetById(long id)
        {
            return this.GetAll()
                       .Where(p => p.Id == id)
                       .FirstOrDefault();
        }

        public IEnumerable<Sinapse.Model.PerfilVisualizacaoServicoParamMonitor> GetParamsByPerfil(long idPerfil)
        {
            return this.GetAll()
                       .Where(p => p.PerfilVisualizacao.Id == idPerfil)
                       .ToArray();
        }

        public void Save(Sinapse.Model.PerfilVisualizacaoServicoParamMonitor param)
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.PerfilVisualizacaoServicoParamMonitor>(this._sessionNh))
            {
                repoParamSetor.Save(param).Wait();
            }
        }

        public void Update(Sinapse.Model.PerfilVisualizacaoServicoParamMonitor param)
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.PerfilVisualizacaoServicoParamMonitor>(this._sessionNh))
            {
                repoParamSetor.Update(param).Wait();
            }
        }

        public void Delete(Sinapse.Model.PerfilVisualizacaoServicoParamMonitor param)
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.PerfilVisualizacaoServicoParamMonitor>(this._sessionNh))
            {
                repoParamSetor.Remove(param).Wait();
            }
        }
    }
}
