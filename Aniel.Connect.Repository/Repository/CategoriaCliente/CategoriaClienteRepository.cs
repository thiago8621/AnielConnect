using Aniel.Connect.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System.Linq;

namespace Aniel.Connect.Repository.CategoriaCliente
{
    public class CategoriaClienteRepository : ICategoriaClienteRepository
    {
        private readonly ISession _sessionNh;

        public CategoriaClienteRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public IQueryable<Categoria_Cliente> GetAll()
        {
            using (var repo = new Conexao<Categoria_Cliente>(this._sessionNh))
            {
                return repo.QueryableSelect();
            }
        }

        public Categoria_Cliente GetById(long id)
        {
            return this.GetAll()
                       .Where(c => c.Id == id)
                       .FirstOrDefault();
        }
    }
}
