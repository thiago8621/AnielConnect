using Aniel.T5.Model.nhibernate;
using System.Linq;

namespace Aniel.Connect.Repository.T5.Cliente
{
    public interface IClienteAnielRepository
    {
        /// <summary>
        ///     Retorna uma representação de uma query na TB_CLIENTE do aniel
        /// </summary>
        IQueryable<ClienteAniel> GetAll();

        /// <summary>
        ///     Retorna o nome do cliente
        /// </summary>
        /// <param name="codCliente">ID do cliente</param>
        string GetNome(int codCliente);
    }
}
