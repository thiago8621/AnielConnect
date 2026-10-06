using NHibernate;
using Sinapse.DAO.repository;
using System.Linq;

namespace Aniel.Connect.Repository.Funcionario
{
    public class FuncionarioRepository : IFuncionarioRepository
    {
        private readonly ISession _sessionNh;

        public FuncionarioRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public IQueryable<Sinapse.Model.Funcionario> GetAll()
        {
            using (var repo = new Conexao<Sinapse.Model.Funcionario>(this._sessionNh))
            {
                return repo.QueryableSelect();
            }
        }

        public Sinapse.Model.Funcionario GetById(long id)
        {
            return this.GetAll()
                       .Where(f => f.Id == id)
                       .FirstOrDefault();
        }
    }
}
