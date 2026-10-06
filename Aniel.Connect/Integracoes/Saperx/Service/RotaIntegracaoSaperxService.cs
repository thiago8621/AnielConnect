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
    public class RotaIntegracaoSaperxService : BaseIntegracaoSaperxService
    {
        public RotaIntegracaoResponseResult Cadastrar(RotaTelefonia rota)
        {
            RotaIntegracaoRequest request = new RotaIntegracaoRequest();

            
            request.name = rota.Nome;
            

            var json = JsonService.SerializeObject(request);
            var result = EnviarRequisicaoPost("/get_custom_route/create", json);
            return JsonService.DeserializeObject<RotaIntegracaoResponseResult>(result);

        }
        public RotaIntegracaoResponseResult Atualizar(RotaTelefonia rota)
        {
            RotaIntegracaoRequest request = new RotaIntegracaoRequest();
            request.name = rota.Nome;
            

            var json = JsonService.SerializeObject(request);
            var result = EnviarRequisicaoPut($"/get_custom_route/update/{rota.CodigoAuxiliar}", json);
            return JsonService.DeserializeObject<RotaIntegracaoResponseResult>(result);
        }

        //public void Excluir(Did did)
        //{
        //    EnviarRequisicaoGet("teste");
        //}

    }


}