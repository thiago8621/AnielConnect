using Aniel.Connect.Model.nhibernate;
using System.Linq;

namespace Aniel.Connect.Repository.MeioCaptacao
{
    public interface IMeioCaptacaoRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_MEIO_CAPTACAO
        /// </summary>
        /// <returns></returns>
        IQueryable<Meio_Captacao> GetAll();

        /// <summary>
        ///     Retorna o meio de captação associado ao ID informado
        /// </summary>
        /// <param name="id">ID do meio de captação</param>
        Meio_Captacao GetById(long id);
    }
}
