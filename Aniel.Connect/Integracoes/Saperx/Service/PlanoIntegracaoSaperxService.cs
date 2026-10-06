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
    public class PlanoIntegracaoSaperxService : BaseIntegracaoSaperxService
    {
        public PlanoIntegracaoResponse Cadastrar(Plano plano)
        {
            var specifier = "F";
            var culture = CultureInfo.CreateSpecificCulture("en-CA");
            PlanoIntegracaoRequest request = new PlanoIntegracaoRequest
            {
                name = plano.Descricao,
                obs = plano.DetalhesPlano,
                minimum_consumption = plano.PossuiConsumoMinimo,
                active = plano.SituacaoPlano == Model.enumerado.Situacao_plano.Ativo,
                consumption_acb = plano.CobrarBina,
                consumption_blacklist = plano.CobrarBlacklist,
                charge_intragroup = !plano.IntragrupoGratuita,
                charge_intragroup_lc = !plano.IntragrupoLcGratuita,
                consumption_sigame = plano.CobrarSigame,
                minimum_consumption_value = plano.ConsumoMinimo.HasValue ? plano.ConsumoMinimo.Value.ToString(specifier, culture) : null,
                consumption_callerid = 0,
                consumption_callerid_value = null,
                vc1_cad = Convert.ToString(plano.CadenciaVc1),
                vc1_value = Convert.ToString(plano.ValorVc1),
                lc_cad = plano.CadenciaLc,
                ldn_cad = plano.CadenciaLdn,
                vc2_cad = plano.CadenciaVc2,
                vc3_cad = plano.CadenciaVc3,
                lc_value = Convert.ToString(plano.ValorLc),
                vc2_value = Convert.ToString(plano.ValorVc2),
                vc3_value = Convert.ToString(plano.ValorVc1),
                ldn_value = Convert.ToString(plano.ValorLdn),
            };

            var json = JsonService.SerializeObject(request);
            if (plano.Formato_do_Plano == Model.enumerado.Formato_Plano.Nacional)
            {
                var response = EnviarRequisicaoPost("/get_plan/create", json);
                return JsonService.DeserializeObject<PlanoIntegracaoResponse>(response);
            }
            else
            {
                var response = EnviarRequisicaoPost($"/get_plan_international/create", json);
                return JsonService.DeserializeObject<PlanoIntegracaoResponse>(response);
            }

        }
        public PlanoIntegracaoResponse Atualizar(Plano plano)
        {
            var specifier = "F";
            var culture = CultureInfo.CreateSpecificCulture("en-CA");
            PlanoIntegracaoRequest request = new PlanoIntegracaoRequest
            {
                name = plano.Descricao,
                obs = plano.DetalhesPlano,
                minimum_consumption = plano.PossuiConsumoMinimo,
                active = plano.SituacaoPlano == Model.enumerado.Situacao_plano.Ativo,
                consumption_acb = plano.CobrarBina,
                consumption_blacklist = plano.CobrarBlacklist,
                charge_intragroup = !plano.IntragrupoGratuita,
                charge_intragroup_lc = !plano.IntragrupoLcGratuita,
                consumption_sigame = plano.CobrarSigame,
                minimum_consumption_value = plano.ConsumoMinimo.HasValue ? plano.ConsumoMinimo.Value.ToString(specifier, culture) : null,
                consumption_callerid = 0,
                consumption_callerid_value = null,
                vc1_cad = Convert.ToString(plano.CadenciaVc1),
                vc1_value = Convert.ToString(plano.ValorVc1),
                lc_cad = plano.CadenciaLc,
                ldn_cad = plano.CadenciaLdn,
                vc2_cad = plano.CadenciaVc2,
                vc3_cad = plano.CadenciaVc3,
                lc_value = Convert.ToString(plano.ValorLc),
                vc2_value = Convert.ToString(plano.ValorVc2),
                vc3_value = Convert.ToString(plano.ValorVc1),
                ldn_value = Convert.ToString(plano.ValorLdn),
            };



            var json = JsonService.SerializeObject(request);
            if (plano.Formato_do_Plano == Model.enumerado.Formato_Plano.Nacional)
            {
                var response = EnviarRequisicaoPut($"/get_plan/update/{plano.CodigoAuxiliar}", json);
                return JsonService.DeserializeObject<PlanoIntegracaoResponse>(response);
            }
            else {
                var response = EnviarRequisicaoPut($"/get_plan_international/update/{plano.CodigoAuxiliar}", json);
                return JsonService.DeserializeObject<PlanoIntegracaoResponse>(response);
            }
            

        }

        //public void Excluir(Did did)
        //{
        //    EnviarRequisicaoGet("teste");
        //}

    }


}