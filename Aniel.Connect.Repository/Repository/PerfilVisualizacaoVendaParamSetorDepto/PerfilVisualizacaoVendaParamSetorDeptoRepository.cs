using NHibernate;
using Sinapse.DAO.repository;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.PerfilVisualizacaoVendaParamSetorDepto
{
    public class PerfilVisualizacaoVendaParamSetorDeptoRepository : IPerfilVisualizacaoVendaParamSetorDeptoRepository
    {
        private readonly ISession _sessionNh;

        public PerfilVisualizacaoVendaParamSetorDeptoRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public IQueryable<Sinapse.Model.PerfilVisualizacaoVendaParamSetorDepto> GetAll()
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.PerfilVisualizacaoVendaParamSetorDepto>(this._sessionNh))
            {
                return repoParamSetor.QueryableSelect();
            }
        }

        public Sinapse.Model.PerfilVisualizacaoVendaParamSetorDepto GetById(long id)
        {
            return this.GetAll()
                       .Where(p => p.Id == id)
                       .FirstOrDefault();
        }

        public IEnumerable<Sinapse.Model.PerfilVisualizacaoVendaParamSetorDepto> GetParamsByPerfil(long idPerfil)
        {
            return this.GetAll()
                       .Where(p => p.PerfilVisualizacao.Id == idPerfil)
                       .ToArray();
        }

        public void Save(Sinapse.Model.PerfilVisualizacaoVendaParamSetorDepto param)
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.PerfilVisualizacaoVendaParamSetorDepto>(this._sessionNh))
            {
                repoParamSetor.Save(param).Wait();
            }
        }

        public void Update(Sinapse.Model.PerfilVisualizacaoVendaParamSetorDepto param)
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.PerfilVisualizacaoVendaParamSetorDepto>(this._sessionNh))
            {
                repoParamSetor.Update(param).Wait();
            }
        }

        public void Delete(Sinapse.Model.PerfilVisualizacaoVendaParamSetorDepto param)
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.PerfilVisualizacaoVendaParamSetorDepto>(this._sessionNh))
            {
                repoParamSetor.Remove(param).Wait();
            }
        }
    }
}
