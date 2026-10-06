using System.Linq;

namespace Aniel.Connect.Repository.ModeloContrato
{
    public interface IModeloContratoRepository
    {
        /// <summary>
        ///     Retorna uma representação de uma consulta na TB_MODELO_CONTRATO
        /// </summary>
        /// <returns></returns>
        IQueryable<Aniel.Connect.Model.nhibernate.Modelo_Contrato> GetAll();

        /// <summary>
        ///     Retorna o modelo de contrato associado ao ID informado
        /// </summary>
        /// <param name="id">ID do modelo de contrato</param>
        Connect.Model.nhibernate.Modelo_Contrato GetById(long id);
    }
}
