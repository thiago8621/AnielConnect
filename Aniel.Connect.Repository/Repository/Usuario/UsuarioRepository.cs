using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.Identity;
using System.Linq;

namespace Aniel.Connect.Repository.Usuario
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ISession _sessionNh;

        public UsuarioRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public IQueryable<IdentityUser> GetAll()
        {
            using (var repoUser = new Conexao<IdentityUser>(this._sessionNh))
            {
                return repoUser.QueryableSelect();
            }
        }

        public IdentityUser GetById(string usuario)
        {
            return this.GetAll()
                       .Where(u => u.Id == usuario)
                       .FirstOrDefault();
        }

    }
}
