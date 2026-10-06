using System.Linq;

namespace Aniel.Connect.Repository.Cliente
{
    public interface IPlanoRepository
    {
        /// <summary>
        ///     Retorna uma representação de uma consulta na TB_PLANO
        /// </summary>
        /// <returns></returns>
        IQueryable<Aniel.Connect.Model.nhibernate.Plano> GetAll();

        /// <summary>
        ///     Retorna o plano associado ao ID informado
        /// </summary>
        /// <param name="id">ID do plano</param>
        Aniel.Connect.Model.nhibernate.Plano GetById(long id);

    }
}
