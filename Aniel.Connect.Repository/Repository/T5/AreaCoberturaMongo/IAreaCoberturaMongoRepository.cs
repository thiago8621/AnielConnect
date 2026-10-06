using Aniel.T5.Model.nhibernate.mongo;
using System.Linq;

namespace Aniel.Connect.Repository.T5.AreaCoberturaMongo
{
    public interface IAreaCoberturaMongoRepository
    {        
        IQueryable<Area_CoberturaMongo> GetAll();
    }
}
