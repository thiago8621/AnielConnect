using NHibernate;
using Sinapse.DAO.repository;
using System.Linq;

namespace Aniel.Connect.Repository.Pais
{
    public class PaisRepository : IPaisRepository
    {
        private readonly ISession _sessionNh;

        public PaisRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public IQueryable<Aniel.Connect.Model.nhibernate.Pais> GetAll()
        {
            using (var repo = new Conexao<Aniel.Connect.Model.nhibernate.Pais>(this._sessionNh))
            {
                return repo.QueryableSelect();
            }
        }

        public Aniel.Connect.Model.nhibernate.Pais GetById(long id)
        {
            return this.GetAll()
                       .Where(p => p.Id == id)
                       .FirstOrDefault();
        }

        public Aniel.Connect.Model.nhibernate.Pais GetByName(string nome)
        {
            return this.GetAll()
                       .Where(p => p.Nome == nome)
                       .FirstOrDefault();
        }
    }
}
