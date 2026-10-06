using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System.Linq;

namespace Aniel.Connect.Repository.T5.Usuario
{
    public class UsuarioAnielRepository : IUsuarioAnielRepository
    {
        private readonly ISession _sessionT5Nh;

        public UsuarioAnielRepository(ISession sessionT5Nh)
        {
            this._sessionT5Nh = sessionT5Nh;
        }

        public IQueryable<SenhaAniel> GetAll()
        {
            using (var repoSenha = new Conexao<SenhaAniel>(this._sessionT5Nh))
            {
                return repoSenha.QueryableSelect();
            }
        }

        public SenhaAniel GetByUsuario(string usuario)
        {
            return this.GetAll()
                       .Where(u => u.Usuario == usuario)
                       .FirstOrDefault();
        }
    }
}
