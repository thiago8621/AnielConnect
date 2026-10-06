using Aniel.Connect.Integracoes.SERPRO.Models;
using System.Threading.Tasks;

namespace Aniel.Connect.Services.Interfaces
{
    public interface IConsultaService
    {
        Task<ConsultaCnpj> ObterCadastroCNPJ(string cnpj);
        Task<ConsultaCpf> ObterCadastroCPF(string cpf);
        Task<bool> VerificarStatusAPI(string uri);
    }
}
