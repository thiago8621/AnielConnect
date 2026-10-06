using Aniel.Connect.Integracoes.Saperx.Models;
using Aniel.Connect.Model.nhibernate;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Integracoes.Saperx.Services
{
    public class BillingIntegracaoSaperxService : BaseIntegracaoSaperxService
    {
        public List<BillingResponse> Billing(BillingRequest request)
        {
            var json = JsonService.SerializeObject(request);
            var result = EnviarRequisicaoPost("/billing", json);
            return JsonService.DeserializeObject<List<BillingResponse>>(result);

        }
        public List<BillingResponse> BillingResume(BillingRequest request)
        {
            var json = JsonService.SerializeObject(request);
            var result = EnviarRequisicaoPost($"/billing_resume", json);
            return JsonService.DeserializeObject<List<BillingResponse>>(result);
        }

    }


}