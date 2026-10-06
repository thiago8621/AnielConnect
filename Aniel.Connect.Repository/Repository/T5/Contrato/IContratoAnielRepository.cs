using Aniel.T5.Model.nhibernate;
using System.Linq;

namespace Aniel.Connect.Repository.T5.Contrato
{
    public interface IContratoAnielRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_CONTRATO
        /// </summary>
        IQueryable<ContratoAniel> GetAll();

        /// <summary>
        ///     Retorna o contrato associado ao código informado
        /// </summary>
        /// <param name="codCt">Código do contrato</param>
        ContratoAniel GetByCodCt(string codCt);

    }
}
