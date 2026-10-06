using NHibernate;
using Sinapse.DAO.repository;
using System.Linq;

namespace Aniel.Connect.Repository.Produto
{
    public class ProdutoRepository : IProdutoRepository
    {
        private ISession _sessionNh;
        public ProdutoRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }
        public IQueryable<Connect.Model.nhibernate.Produto> GetAll()
        {
            using (var repo = new Conexao<Connect.Model.nhibernate.Produto>(this._sessionNh))
            {
                return repo.QueryableSelect();
            }
        }

        public Connect.Model.nhibernate.Produto GetById(long id)
        {
            using (var repo = new Conexao<Connect.Model.nhibernate.Produto>(this._sessionNh))
            {
                return repo.selectFirst(p => p.Id == id).Result;
            }
        }

    }
}
