using System.Linq;

namespace Aniel.Connect.Repository.Produto
{
    public interface IProdutoRepository
    {
        /// <summary>
        ///     Retorna uma representação de uma consulta na TB_PRODUTO
        /// </summary>
        /// <returns></returns>
        IQueryable<Aniel.Connect.Model.nhibernate.Produto> GetAll();

        /// <summary>
        ///     Retorna o produto associado ao ID informado
        /// </summary>
        /// <param name="id">ID do cliente</param>
        Aniel.Connect.Model.nhibernate.Produto GetById(long id);

    }
}
