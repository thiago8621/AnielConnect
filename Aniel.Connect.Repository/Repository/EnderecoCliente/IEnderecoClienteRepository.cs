using Aniel.Connect.Model.nhibernate;

namespace Aniel.Connect.Repository.EnderecoCliente
{
    public interface IEnderecoClienteRepository
    {
        /// <summary>
        ///     Insere um registro na TB_ENDERECO_CLIENTE
        /// </summary>
        /// <param name="endereco">Objeto com os dados do endereço do cliente</param>
        void Save(Endereco_Cliente endereco);

        /// <summary>
        ///     Atualiza um registro na TB_ENDERECO_CLIENTE
        /// </summary>
        /// <param name="endereco">Objeto com os dados do endereço do cliente</param>
        void Update(Endereco_Cliente endereco);
    }
}
