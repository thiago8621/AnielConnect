using Aniel.Connect.Integracoes.Saperx.Models;
using Aniel.Connect.Model.nhibernate;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Integracoes.Saperx.Services
{
    public class FranquiaIntegracaoSaperxService : BaseIntegracaoSaperxService
    {
        public FranquiaIntegracaoResponse Cadastrar(Franquia franquia,Plano plano)
        {
            FranquiaIntegracaoRequest request = new FranquiaIntegracaoRequest();
            request.id_plan = Convert.ToInt32(plano.CodigoAuxiliar);
            request.franchise_minutes = franquia.Minutos.HasValue ? franquia.Minutos.Value : 0;
            request.LC = franquia.Lc;
            request.LDN = franquia.Ldn;
            request.name = franquia.Descricao;
            request.VC1 = franquia.Vc1;
            request.VC2 = franquia.Vc2;
            request.VC3 = franquia.Vc3;
            //request.id_plan = franquia.Vc3;

            var json = JsonService.SerializeObject(request);
            var response = EnviarRequisicaoPost("/get_plan_franchise/create", json);
            return JsonService.DeserializeObject<FranquiaIntegracaoResponse>(response);
        }

        public FranquiaIntegracaoResponse Atualizar(Franquia franquia, Plano plano)
        {
            FranquiaIntegracaoRequest request = new FranquiaIntegracaoRequest();
            request.id_plan = Convert.ToInt32(plano.CodigoAuxiliar);
            request.franchise_minutes = franquia.Minutos.HasValue ? franquia.Minutos.Value : 0;
            request.LC = franquia.Lc;
            request.LDN = franquia.Ldn;
            request.name = franquia.Descricao;
            request.VC1 = franquia.Vc1;
            request.VC2 = franquia.Vc2;
            request.VC3 = franquia.Vc3;

            var json = JsonService.SerializeObject(request);
            var response = EnviarRequisicaoPut($"/get_plan_franchise/update/{plano.CodigoAuxiliarFranquia}", json);
            return JsonService.DeserializeObject<FranquiaIntegracaoResponse>(response);

        }

        //public void Excluir(Did did)
        //{
        //    EnviarRequisicaoGet("teste");
        //}

    }


}