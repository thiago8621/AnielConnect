using System.Linq;

namespace Aniel.Connect.Repository.Cliente
{
    public interface IClienteRepository
    {
        /// <summary>
        ///     Retorna uma representação de uma consulta na TB_CLIENTE
        /// </summary>
        /// <returns></returns>
        IQueryable<Aniel.Connect.Model.nhibernate.Cliente> GetAll();

        /// <summary>
        ///     Retorna o cliente associado ao ID informado
        /// </summary>
        /// <param name="id">ID do cliente</param>
        Aniel.Connect.Model.nhibernate.Cliente GetById(long id);

        /// <summary>
        ///     Retorna o cliente associado ao CPF ou CNPJ informado
        /// </summary>
        /// <param name="cpfCnpj">CPF ou CNPJ do cliente</param>
        Aniel.Connect.Model.nhibernate.Cliente GetByCpfCnpj(string cpfCnpj);
        /// <summary>
        ///     Retorna o número de contrato mais alto registrado na TB_CLIENTE para
        ///     uso em caso de geração automática de número de contrato
        /// </summary>
        long GetMaxNumeroContrato();

        /// <summary>
        ///     Atualiza um registro de cliente já presente no banco
        /// </summary>
        /// <param name="cliente">Objeto com os dados do cliente</param>
        void Update(Aniel.Connect.Model.nhibernate.Cliente cliente);

        /// <summary>
        ///     Atualiza o campo INTEGRADO_OLA no registro de cliente.
        /// </summary>
        /// <param name="IdCliente">Id do cliente</param>
        void AtualizarIntegradoOla(long IdCliente);
    }
}
