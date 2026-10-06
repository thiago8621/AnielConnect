using NHibernate;
using Sinapse.DAO.repository;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.PerfilVisualizacaoServicoParamSetorDepto
{
    public class PerfilVisualizacaoServicoParamSetorDeptoRepository : IPerfilVisualizacaoServicoParamSetorDeptoRepository
    {
        private readonly ISession _sessionNh;

        public PerfilVisualizacaoServicoParamSetorDeptoRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public IQueryable<Sinapse.Model.PerfilVisualizacaoServicoParamSetorDepto> GetAll()
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.PerfilVisualizacaoServicoParamSetorDepto>(this._sessionNh))
            {
                return repoParamSetor.QueryableSelect();
            }
        }

        public Sinapse.Model.PerfilVisualizacaoServicoParamSetorDepto GetById(long id)
        {
            return this.GetAll()
                       .Where(p => p.Id == id)
                       .FirstOrDefault();
        }

        public IEnumerable<Sinapse.Model.PerfilVisualizacaoServicoParamSetorDepto> GetParamsByPerfil(long idPerfil)
        {
            return this.GetAll()
                       .Where(p => p.PerfilVisualizacao.Id == idPerfil)
                       .ToArray();
        }

        public void Save(Sinapse.Model.PerfilVisualizacaoServicoParamSetorDepto param)
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.PerfilVisualizacaoServicoParamSetorDepto>(this._sessionNh))
            {
                repoParamSetor.Save(param).Wait();
            }
        }

        public void Update(Sinapse.Model.PerfilVisualizacaoServicoParamSetorDepto param)
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.PerfilVisualizacaoServicoParamSetorDepto>(this._sessionNh))
            {
                repoParamSetor.Update(param).Wait();
            }
        }

        public void Delete(Sinapse.Model.PerfilVisualizacaoServicoParamSetorDepto param)
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.PerfilVisualizacaoServicoParamSetorDepto>(this._sessionNh))
            {
                repoParamSetor.Remove(param).Wait();
            }
        }
    }
}
