using NHibernate;
using Sinapse.DAO.repository;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.PerfilVisualizacaoVendaParamCategoriaCliente
{
    public class PerfilVisualizacaoVendaParamCategoriaClienteRepository : IPerfilVisualizacaoVendaParamCategoriaClienteRepository
    {
        private readonly ISession _sessionNh;

        public PerfilVisualizacaoVendaParamCategoriaClienteRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public IQueryable<Sinapse.Model.PerfilVisualizacaoVendaParamCategoriaCliente> GetAll()
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.PerfilVisualizacaoVendaParamCategoriaCliente>(this._sessionNh))
            {
                return repoParamSetor.QueryableSelect();
            }
        }

        public Sinapse.Model.PerfilVisualizacaoVendaParamCategoriaCliente GetById(long id)
        {
            return this.GetAll()
                       .Where(p => p.Id == id)
                       .FirstOrDefault();
        }

        public IEnumerable<Sinapse.Model.PerfilVisualizacaoVendaParamCategoriaCliente> GetParamsByPerfil(long idPerfil)
        {
            return this.GetAll()
                       .Where(p => p.PerfilVisualizacao.Id == idPerfil)
                       .ToArray();
        }

        public void Save(Sinapse.Model.PerfilVisualizacaoVendaParamCategoriaCliente param)
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.PerfilVisualizacaoVendaParamCategoriaCliente>(this._sessionNh))
            {
                repoParamSetor.Save(param).Wait();
            }
        }

        public void Update(Sinapse.Model.PerfilVisualizacaoVendaParamCategoriaCliente param)
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.PerfilVisualizacaoVendaParamCategoriaCliente>(this._sessionNh))
            {
                repoParamSetor.Update(param).Wait();
            }
        }

        public void Delete(Sinapse.Model.PerfilVisualizacaoVendaParamCategoriaCliente param)
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.PerfilVisualizacaoVendaParamCategoriaCliente>(this._sessionNh))
            {
                repoParamSetor.Remove(param).Wait();
            }
        }
    }
}
