using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Cliente;
using NHibernate;
using Sinapse.DAO.repository;
using System.Linq;

namespace Aniel.Connect.Repository.Plano
{
    public class PlanoRepository : IPlanoRepository
    {
        private ISession _sessionNh;
        public PlanoRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }
    
        IQueryable<Connect.Model.nhibernate.Plano> IPlanoRepository.GetAll()
        {
            using (var repo = new Conexao<Connect.Model.nhibernate.Plano>(this._sessionNh))
            {
                return repo.QueryableSelect();
            }
        }

        Connect.Model.nhibernate.Plano IPlanoRepository.GetById(long id)
        {
            using (var repo = new Conexao<Connect.Model.nhibernate.Plano>(this._sessionNh))
            {
                return repo.selectFirst(p => p.Id == id).Result;
            }
        }
    }
}
