using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.EnderecoLogradouro;
using System.Linq;

namespace Aniel.Connect.ConnectBusiness.Connect
{
    public class EnderecoLogradouroBusiness
    {
        private readonly IEnderecoLogradouroRepository _repository;

        public EnderecoLogradouroBusiness(IEnderecoLogradouroRepository repository)
        {
            this._repository = repository;
        }

        /// <summary>
        ///     Procura no sistema um endereço registrado com base em strings informando partes do endereço
        /// </summary>
        /// <param name="uf">Sigla da UF</param>
        /// <param name="cidade">Nome da cidade</param>
        /// <param name="bairro">Nome do bairro</param>
        /// <param name="logradouro">Logradouro do endereco</param>
        public Endereco_Logradouro GetByNomes(string uf, string cidade, string bairro, string logradouro)
        {
            return this._repository
                       .GetAll()
                       .Where(e => e.Descricao == logradouro &&
                                   e.Cidade.UF == uf &&
                                   e.Cidade.Descricao == cidade &&
                                   e.Bairro.Descricao == bairro)
                       .FirstOrDefault();
        }
    }
}
