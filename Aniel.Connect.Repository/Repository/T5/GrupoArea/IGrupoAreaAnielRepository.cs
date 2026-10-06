using Aniel.T5.Model.nhibernate;
using System.Linq;

namespace Aniel.Connect.Repository.T5.GrupoArea
{
    public interface IGrupoAreaAnielRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_ESCRITORIO
        /// </summary>
        IQueryable<EscritorioAniel> GetAll();

        /// <summary>
        ///     Retorna o grupo área associado ao código informado
        /// </summary>
        /// <param name="codEscritorio">código do grupo área</param>
        EscritorioAniel GetByCodigo(string codEscritorio);
    }
}
