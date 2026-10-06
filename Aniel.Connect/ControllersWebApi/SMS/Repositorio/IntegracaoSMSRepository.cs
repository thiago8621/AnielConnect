//using Aniel.Connect.ControllersWebApi.SMS.Interface;
using Sinapse.Integracao.SMS.Integracoes;
using Sinapse.Integracao.SMS.Models;
//using Aniel.Connect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Sinapse.Integracao.SMS.Interface;

namespace Aniel.Connect.ControllersWebApi.SMS.Repositorio
{
    public class IntegracaoSMSRepository
    {
        public async Task<EnviarSMS_ResponseModel> EnviarSMS(IntegracaoSMS integracao, Sinapse.Model.SMS value)
        {
            return integracao.EnviarSMS(value);
        }

        public async Task<EnviarLoteSMS_ResponseModel> EnviarLoteSMS(IntegracaoSMS integracao, List<Sinapse.Model.SMS> value)
        {
            return integracao.EnviarLoteSMS(value);
        }
    }
}