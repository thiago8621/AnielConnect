using NHibernate;
using Sinapse.DAO.repository;
using System.Linq;

namespace Aniel.Connect.Repository.PerfilVisualizacaoServico
{
    public class PerfilVisualizacaoServicoRepository : IPerfilVisualizacaoServicoRepository
    {
        private readonly ISession _sessionNh;

        public PerfilVisualizacaoServicoRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public IQueryable<Sinapse.Model.PerfilVisualizacaoServico> GetAll()
        {
            using (var repoPerfil = new Conexao<Sinapse.Model.PerfilVisualizacaoServico>(this._sessionNh))
            {
                return repoPerfil.QueryableSelect();
            }
        }

        public Sinapse.Model.PerfilVisualizacaoServico GetById(long id)
        {
            return this.GetAll()
                       .Where(p => p.Id == id)
                       .FirstOrDefault();
        }

        public void Save(Sinapse.Model.PerfilVisualizacaoServico perfil)
        {
            using (var repoPerfil = new Conexao<Sinapse.Model.PerfilVisualizacaoServico>(this._sessionNh))
            {
                repoPerfil.Save(perfil).Wait();
            }
        }

        public void Update(Sinapse.Model.PerfilVisualizacaoServico perfil)
        {
            using (var repoPerfil = new Conexao<Sinapse.Model.PerfilVisualizacaoServico>(this._sessionNh))
            {
                repoPerfil.Update(perfil).Wait();
            }
        }
    }
}
