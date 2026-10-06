using NHibernate;
using Sinapse.DAO.repository;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.PerfilVisualizacaoServicoParamArea
{
    public class PerfilVisualizacaoServicoParamAreaRepository : IPerfilVisualizacaoServicoParamAreaRepository
    {
        private readonly ISession _sessionNh;

        public PerfilVisualizacaoServicoParamAreaRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public IQueryable<Sinapse.Model.PerfilVisualizacaoServicoParamArea> GetAll()
        {
            using (var repoParamArea = new Conexao<Sinapse.Model.PerfilVisualizacaoServicoParamArea>(this._sessionNh))
            {
                return repoParamArea.QueryableSelect();
            }
        }

        public Sinapse.Model.PerfilVisualizacaoServicoParamArea GetById(long id)
        {
            return this.GetAll()
                       .Where(p => p.Id == id)
                       .FirstOrDefault();
        }

        public IEnumerable<Sinapse.Model.PerfilVisualizacaoServicoParamArea> GetParamsByPerfil(long idPerfil)
        {
            return this.GetAll()
                       .Where(p => p.PerfilVisualizacao.Id == idPerfil)
                       .ToArray();
        }

        public void Save(Sinapse.Model.PerfilVisualizacaoServicoParamArea param)
        {
            using (var repoParamArea = new Conexao<Sinapse.Model.PerfilVisualizacaoServicoParamArea>(this._sessionNh))
            {
                repoParamArea.Save(param).Wait();
            }
        }

        public void Update(Sinapse.Model.PerfilVisualizacaoServicoParamArea param)
        {
            using (var repoParamArea = new Conexao<Sinapse.Model.PerfilVisualizacaoServicoParamArea>(this._sessionNh))
            {
                repoParamArea.Update(param).Wait();
            }
        }

        public void Delete(Sinapse.Model.PerfilVisualizacaoServicoParamArea param)
        {
            using (var repoParamArea = new Conexao<Sinapse.Model.PerfilVisualizacaoServicoParamArea>(this._sessionNh))
            {
                repoParamArea.Remove(param).Wait();
            }
        }
    }
}
