using NHibernate;
using Sinapse.DAO.repository;
using System.Linq;

namespace Aniel.Connect.Repository.PerfilVisualizacaoVenda
{
    public class PerfilVisualizacaoVendaRepository : IPerfilVisualizacaoVendaRepository
    {
        private readonly ISession _sessionNh;

        public PerfilVisualizacaoVendaRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public IQueryable<Sinapse.Model.PerfilVisualizacaoVenda> GetAll()
        {
            using (var repoPerfil = new Conexao<Sinapse.Model.PerfilVisualizacaoVenda>(this._sessionNh))
            {
                return repoPerfil.QueryableSelect();
            }
        }

        public Sinapse.Model.PerfilVisualizacaoVenda GetById(long id)
        {
            return this.GetAll()
                       .Where(p => p.Id == id)
                       .FirstOrDefault();
        }

        public void Save(Sinapse.Model.PerfilVisualizacaoVenda perfil)
        {
            using (var repoPerfil = new Conexao<Sinapse.Model.PerfilVisualizacaoVenda>(this._sessionNh))
            {
                repoPerfil.Save(perfil).Wait();
            }
        }

        public void Update(Sinapse.Model.PerfilVisualizacaoVenda perfil)
        {
            using (var repoPerfil = new Conexao<Sinapse.Model.PerfilVisualizacaoVenda>(this._sessionNh))
            {
                repoPerfil.Update(perfil).Wait();
            }
        }
    }
}
