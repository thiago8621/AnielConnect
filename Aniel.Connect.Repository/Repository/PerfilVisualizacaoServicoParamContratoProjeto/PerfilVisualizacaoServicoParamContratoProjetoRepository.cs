using NHibernate;
using Sinapse.DAO.repository;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.PerfilVisualizacaoServicoParamContratoProjeto
{
    public class PerfilVisualizacaoServicoParamContratoProjetoRepository : IPerfilVisualizacaoServicoParamContratoProjetoRepository
    {
        private readonly ISession _sessionNh;

        public PerfilVisualizacaoServicoParamContratoProjetoRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public IQueryable<Sinapse.Model.PerfilVisualizacaoServicoParamContratoProjeto> GetAll()
        {
            using (var repoParamContratoProjeto = new Conexao<Sinapse.Model.PerfilVisualizacaoServicoParamContratoProjeto>(this._sessionNh))
            {
                return repoParamContratoProjeto.QueryableSelect();
            }
        }

        public Sinapse.Model.PerfilVisualizacaoServicoParamContratoProjeto GetById(long id)
        {
            return this.GetAll()
                       .Where(p => p.Id == id)
                       .FirstOrDefault();
        }

        public IEnumerable<Sinapse.Model.PerfilVisualizacaoServicoParamContratoProjeto> GetParamsByPerfil(long idPerfil)
        {
            return this.GetAll()
                       .Where(p => p.PerfilVisualizacao.Id == idPerfil)
                       .ToArray();
        }

        public void Save(Sinapse.Model.PerfilVisualizacaoServicoParamContratoProjeto param)
        {
            using (var repoParamContratoProjeto = new Conexao<Sinapse.Model.PerfilVisualizacaoServicoParamContratoProjeto>(this._sessionNh))
            {
                repoParamContratoProjeto.Save(param).Wait();
            }
        }

        public void Update(Sinapse.Model.PerfilVisualizacaoServicoParamContratoProjeto param)
        {
            using (var repoParamContratoProjeto = new Conexao<Sinapse.Model.PerfilVisualizacaoServicoParamContratoProjeto>(this._sessionNh))
            {
                repoParamContratoProjeto.Update(param).Wait();
            }
        }

        public void Delete(Sinapse.Model.PerfilVisualizacaoServicoParamContratoProjeto param)
        {
            using (var repoParamContratoProjeto = new Conexao<Sinapse.Model.PerfilVisualizacaoServicoParamContratoProjeto>(this._sessionNh))
            {
                repoParamContratoProjeto.Remove(param).Wait();
            }
        }
    }
}
