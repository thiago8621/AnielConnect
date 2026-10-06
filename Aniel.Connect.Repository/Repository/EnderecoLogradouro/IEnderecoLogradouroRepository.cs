using Aniel.Connect.Model.nhibernate;
using System.Linq;

namespace Aniel.Connect.Repository.EnderecoLogradouro
{
    public interface IEnderecoLogradouroRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_ENDERECO_LOGRADOURO
        /// </summary>
        IQueryable<Endereco_Logradouro> GetAll();

        /// <summary>
        ///     Retorna o endereço associado ao ID informado
        /// </summary>
        /// <param name="id">ID do endereço</param>
        Endereco_Logradouro GetById(long id);
    }
}
