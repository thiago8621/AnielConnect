using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.PendenciaFinanceiraIntegracao;
using Aniel.Connect.Repository.Repository.Azza;
using Aniel.Connect.Repository.Repository.Azza.PendenciaEndereco;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aniel.Connect.ConnectBusiness.Azza
{
    public class IXCBusiness
    {
        private readonly IPendenciaFinanceiraIntegracaoRepository _repoPendenciaFinanceira;
        private readonly IPendenciaEnderecoIntegracaoRepository _repoPendenciaEndereco;
        private readonly IAzzaRepository _repoAzza;

        public IXCBusiness(IPendenciaFinanceiraIntegracaoRepository repoPendenciaFinanceira, IAzzaRepository repoAzza, IPendenciaEnderecoIntegracaoRepository repoPendenciaEndereco)
        {
            _repoPendenciaFinanceira = repoPendenciaFinanceira;
            _repoAzza = repoAzza;
            _repoPendenciaEndereco = repoPendenciaEndereco;
        }

        public IEnumerable<PendenciaFinanceiraListModel> GetPendenciasFinanceira(string cpfCnpj)
        {
            return this._repoPendenciaFinanceira.GetPendenciasFinanceira(cpfCnpj);
        }

        public async Task<IEnumerable<PendenciaEnderecoListModel>> GetPendenciasEndereco(string endereco, string numero, string id_cidade, string cep)
        {
            return await this._repoPendenciaEndereco.GetPendenciasEndereco(endereco, numero, id_cidade, cep);
        }
    }
}
