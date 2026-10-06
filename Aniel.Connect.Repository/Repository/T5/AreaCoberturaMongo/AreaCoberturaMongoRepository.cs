using Aniel.T5.Model.nhibernate;
using Aniel.T5.Model.nhibernate.mongo;
using MongoDB.Driver;
using NHibernate;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.repository;
using Sinapse.DAO.Repository;
using System.Linq;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.T5.AreaCoberturaMongo
{
    public class AreaCoberturaMongoRepository : IAreaCoberturaMongoRepository
    {
        private readonly IConexaoMongo _session;
        public AreaCoberturaMongoRepository(ConexaoMongoGeography session) 
        {
            _session = session;
        }
        public IQueryable<Area_CoberturaMongo> GetAll()
        {
            using (var repo = new RepositoryMongo<Area_CoberturaMongo>(_session)) 
            {                               
                return repo.GetQueryable();
            }
        }
    }
}
