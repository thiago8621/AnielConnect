using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.PendenciaFinanceira;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.ConnectBusiness.PendenciaFinanceira.Connect
{
    public interface IntegracaoPendenciaFinanceira
    {
        IEnumerable<PendenciaFinanceiraListModel> GetPendenciasFinanceira(string cpfCnpj);
    }
}
