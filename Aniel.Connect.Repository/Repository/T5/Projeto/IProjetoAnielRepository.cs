using Aniel.T5.Model.nhibernate;
using System.Linq;

namespace Aniel.Connect.Repository.T5.Projeto
{
    public interface IProjetoAnielRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_PROJETO
        /// </summary>
        /// <returns></returns>
        IQueryable<ProjetoAniel> GetAll();

        /// <summary>
        ///     Retorna o projeto associado ao código informado
        /// </summary>
        /// <param name="codCt">Código do contrato</param>
        /// <param name="projeto">Código do projeto/</param>
        ProjetoAniel GetByCodigo(string codCt, string projeto);
    }
}
