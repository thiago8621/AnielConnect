using Aniel.T5.Model.nhibernate;
using System.Linq;

namespace Aniel.Connect.Repository.T5.Setor
{
    public interface ISetorAnielRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_FOLHA_DEPTO
        /// </summary>
        /// <returns></returns>
        IQueryable<FolhaDeptoAniel> GetAll();

        /// <summary>
        ///     Retorna o setor associado ao código informado
        /// </summary>
        /// <param name="codDepto">Código do setor</param>
        FolhaDeptoAniel GetByCodigo(short codDepto);
    }
}
