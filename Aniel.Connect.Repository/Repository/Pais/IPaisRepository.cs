using System.Linq;

namespace Aniel.Connect.Repository.Pais
{
    public interface IPaisRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_PAIS
        /// </summary>
        IQueryable<Aniel.Connect.Model.nhibernate.Pais> GetAll();

        /// <summary>
        ///     Retorna o país associado ao ID informado
        /// </summary>
        /// <param name="id">ID do país</param>
        Aniel.Connect.Model.nhibernate.Pais GetById(long id);

        /// <summary>
        ///     Retorna o país com o nome informado
        /// </summary>
        /// <param name="nome">Nome do país</param>
        Aniel.Connect.Model.nhibernate.Pais GetByName(string nome);
    }
}
