using System.Linq;

namespace Aniel.Connect.Repository.Repository.Pool_Ip
{
    public interface IPool_IpRepository
    {
        /// <summary>
        ///     Retorna uma representação de uma consulta na Pool IP
        /// </summary>
        /// <returns></returns>
        IQueryable<Connect.Model.nhibernate.Pool_Ip> GetAll();

        /// <summary>
        ///     Retorna uma representação de uma consulta na Pool Ip por ID
        /// </summary>
        /// <returns></returns> 
        Connect.Model.nhibernate.Pool_Ip GetById(long Id);
    }
}
