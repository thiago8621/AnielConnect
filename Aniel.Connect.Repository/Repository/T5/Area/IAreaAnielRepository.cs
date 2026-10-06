using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.nhibernate;
using System.Linq;


namespace Aniel.Connect.Repository.T5.Area
{
    public interface IAreaAnielRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta a TB_AREA do banco do T5
        /// </summary>
        IQueryable<AreaAniel> GetAll();

        /// <summary>
        ///     Retorna a área associado ao código informado
        /// </summary>
        /// <param name="codEscritorio">código do grupo área</param>
        /// <param name="codArea">código da área</param>
        AreaAniel GetByCodigo(string codEscritorio, string codArea);

        /// <summary>
        ///    Retorna dados da area filtrando por cidade
        /// </summary>
        /// <param name="codEscritorio">código do grupo área</param>
        AreaResponse GetByCidade(string cidade);
    }
}
