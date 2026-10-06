using Aniel.Connect.Model.nhibernate;
using System.Linq;

namespace Aniel.Connect.Repository.CategoriaCliente
{
    public interface ICategoriaClienteRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_CATEGORIA_CLIENTE
        /// </summary>
        IQueryable<Categoria_Cliente> GetAll();

        /// <summary>
        ///     Retorna a categoria de cliente associada ao ID informado
        /// </summary>
        /// <param name="id">ID da categoria de cliente</param>
        Categoria_Cliente GetById(long id);
    }
}
