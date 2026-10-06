using Aniel.Connect.ConnectBusiness.PendenciaFinanceira.Connect;
using Aniel.Connect.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.ConnectBusiness.Connect
{
    public class IPendenciaFinanceiraIntegracaoBusiness
    {
        public IEnumerable<PendenciaFinanceiraListModel> GetPendenciasFinanceiras(IntegracaoPendenciaFinanceira integracao, String CpfCnpj) 
        {
            return integracao.GetPendenciasFinanceira(CpfCnpj);
        }

    }
}
