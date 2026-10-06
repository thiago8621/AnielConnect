using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.PendenciaEnderecoAniel;
using Aniel.Connect.Repository.Repository.PendenciaFinanceiraAniel;
using System.Collections.Generic;

namespace Aniel.Connect.ConnectBusiness.Connect
{
    public class ConsultaPendenciasConnectBusiness
    {

        private readonly IPendenciaEnderecoAnielRepository _repoPendenciaEndereco;
        private readonly IPendenciaFinanceiraAnielRepository _repoPendenciaFinanceira;

        public ConsultaPendenciasConnectBusiness(
            IPendenciaEnderecoAnielRepository repoPendenciaEndereco,
            IPendenciaFinanceiraAnielRepository repoPendenciaFinanceira
        )
        {
            this._repoPendenciaEndereco = repoPendenciaEndereco;
            this._repoPendenciaFinanceira = repoPendenciaFinanceira;
        }

        public IEnumerable<PendenciaEnderecoListModel> GetPendenciasEndereco(string logradouro, string numero)
        {
            return this._repoPendenciaEndereco.GetPendenciasEndereco(logradouro, numero);
        }

        public IEnumerable<PendenciaFinanceiraListModel> GetPendenciasFinanceira(string cpfCnpj)
        {
            return this._repoPendenciaFinanceira.GetPendenciasFinanceira(cpfCnpj);
        }
    }
}
