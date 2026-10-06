using NHibernate;
using Sinapse.DAO.repository;
using System.Linq;

namespace Aniel.Connect.Repository.Departamento
{
    public class DepartamentoRepository : IDepartamentoRepository
    {
        private readonly ISession _sessionNh;

        public DepartamentoRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public IQueryable<Sinapse.Model.Departamento> GetAll()
        {
            using (var repo = new Conexao<Sinapse.Model.Departamento>(this._sessionNh))
            {
                return repo.QueryableSelect();
            }
        }

        public Sinapse.Model.Departamento GetById(long id)
        {
            return this.GetAll()
                       .Where(d => d.Id == id)
                       .FirstOrDefault();
        }
    }
}
