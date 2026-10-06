using Aniel.Connect.Repository.Repository.Pool_Ip;
using NHibernate;
using Sinapse.DAO.repository;
using System.Linq;

namespace Aniel.Connect.Repository.Pool_Ip
{
    public class Pool_IpRepository : IPool_IpRepository
    {
        private readonly ISession _sessionNh;

        public Pool_IpRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }
        public IQueryable<Connect.Model.nhibernate.Pool_Ip> GetAll()
        {
            using (var repo = new Conexao<Connect.Model.nhibernate.Pool_Ip>(this._sessionNh))
            {
                return repo.QueryableSelect();
            }
        }

        public Connect.Model.nhibernate.Pool_Ip GetById(long Id)
        {
            using (var repo = new Conexao<Connect.Model.nhibernate.Pool_Ip>(this._sessionNh))
            {
                return repo.selectFirst(p => p.Id == Id).Result;
            }
        }
    }
}