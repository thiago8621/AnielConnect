using NHibernate;
using Sinapse.DAO.repository;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.PerfilVisualizacaoServicoParamSupervisor
{
    public class PerfilVisualizacaoServicoParamSupervisorRepository : IPerfilVisualizacaoServicoParamSupervisorRepository
    {
        private readonly ISession _sessionNh;

        public PerfilVisualizacaoServicoParamSupervisorRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public IQueryable<Sinapse.Model.PerfilVisualizacaoServicoParamSupervisor> GetAll()
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.PerfilVisualizacaoServicoParamSupervisor>(this._sessionNh))
            {
                return repoParamSetor.QueryableSelect();
            }
        }

        public Sinapse.Model.PerfilVisualizacaoServicoParamSupervisor GetById(long id)
        {
            return this.GetAll()
                       .Where(p => p.Id == id)
                       .FirstOrDefault();
        }

        public IEnumerable<Sinapse.Model.PerfilVisualizacaoServicoParamSupervisor> GetParamsByPerfil(long idPerfil)
        {
            return this.GetAll()
                       .Where(p => p.PerfilVisualizacao.Id == idPerfil)
                       .ToArray();
        }

        public void Save(Sinapse.Model.PerfilVisualizacaoServicoParamSupervisor param)
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.PerfilVisualizacaoServicoParamSupervisor>(this._sessionNh))
            {
                repoParamSetor.Save(param).Wait();
            }
        }

        public void Update(Sinapse.Model.PerfilVisualizacaoServicoParamSupervisor param)
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.PerfilVisualizacaoServicoParamSupervisor>(this._sessionNh))
            {
                repoParamSetor.Update(param).Wait();
            }
        }

        public void Delete(Sinapse.Model.PerfilVisualizacaoServicoParamSupervisor param)
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.PerfilVisualizacaoServicoParamSupervisor>(this._sessionNh))
            {
                repoParamSetor.Remove(param).Wait();
            }
        }
    }
}
