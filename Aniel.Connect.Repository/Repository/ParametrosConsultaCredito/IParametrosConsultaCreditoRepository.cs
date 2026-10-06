using System.Linq;

namespace Aniel.Connect.Repository.ParametrosConsultaCredito
{
    public interface IParametrosConsultaCreditoRepository
    {
        /// <summary>
        ///     Retorna uma representação de uma consulta na TB_PARAMETROS_CONSULTA_CREDITO
        /// </summary>
        IQueryable<Sinapse.Model.ParametrosConsultaCredito> GetAll();

        /// <summary>
        ///     Retorna o registro com os parâmetros configurados para a consulta de crédito
        /// </summary>
        Sinapse.Model.ParametrosConsultaCredito GetParametrosConsultaCredito();
    }
}
