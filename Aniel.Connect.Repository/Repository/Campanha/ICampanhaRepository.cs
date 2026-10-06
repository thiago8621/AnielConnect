using System.Linq;

namespace Aniel.Connect.Repository.Campanha
{
    public interface ICampanhaRepository
    {
        /// <summary>
        ///     Retorna uma representação de uma consulta na TB_CAMPANHA
        /// </summary>
        /// <returns></returns>
        IQueryable<Connect.Model.nhibernate.Campanha> GetAll();

        /// <summary>
        ///     Retorna o campanha associado ao ID informado
        /// </summary>
        /// <param name="id">ID do cliente</param>
        Connect.Model.nhibernate.Campanha GetById(long id);

    }
}
