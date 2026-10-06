using System.Linq;

namespace Aniel.Connect.Repository.CriteriosParamCredito
{
    public interface ICriteriosParamCreditoRepository
    {
        /// <summary>
        ///     Retorna uma representação de uma consulta na TB_CRITERIO_PARAM_CREDITO
        /// </summary>
        IQueryable<Sinapse.Model.CriteriosParamCredito> GetAll();
    }
}
