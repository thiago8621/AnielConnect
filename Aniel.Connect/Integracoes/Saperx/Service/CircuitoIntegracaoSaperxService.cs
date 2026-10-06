using Aniel.Connect.Integracoes.Saperx.Models;
using Aniel.Connect.Model.nhibernate;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Integracoes.Saperx.Services
{
    public class CircuitoIntegracaoSaperxService : BaseIntegracaoSaperxService
    {
        public CircuitoIntegracaoResponse Cadastrar(CircuitoTelefonia circuito)
        {
            CircuitoIntegracaoRequest request = new CircuitoIntegracaoRequest
            {
                call_LC = circuito.HabilitarLc,
                call_LDI = circuito.HabilitarLdi,
                call_LDN = circuito.HabilitarLdn,
                call_VC1 = circuito.HabilitarVc1,
                call_VC2 = circuito.HabilitarVc2,

                custom_route = String.IsNullOrEmpty(circuito?.RotaTelefonia?.CodigoAuxiliar) ? (int?)null : Convert.ToInt32(circuito?.RotaTelefonia?.CodigoAuxiliar),
                call_VC3 = circuito.HabilitarVc3,
                sip_password = circuito.Senha,
                cnpj = circuito?.Clientes_PlanosProdutos?.Cliente?.CpfCnpj,
                active = circuito.Status == Sinapse.Model.enumerados.Status_Default.Ativo ? "1" : "0",
                show_callerid = circuito.HabilitarBina,
                cod_cli = (circuito?.Clientes_PlanosProdutos?.Cliente?.Id != null) ? circuito?.Clientes_PlanosProdutos?.Cliente?.Id.ToString() : null,
                name = "teste",



            };

            if (circuito?.Clientes_PlanosProdutos?.Plano?.Formato_do_Plano == Model.enumerado.Formato_Plano.Nacional)
            {
                request.plan_national = String.IsNullOrEmpty(circuito.Clientes_PlanosProdutos?.Plano?.CodigoAuxiliar) ? (int?)null : Convert.ToInt32(circuito.Clientes_PlanosProdutos?.Plano?.CodigoAuxiliar);
            }
            if (circuito?.Clientes_PlanosProdutos?.Plano?.Formato_do_Plano == Model.enumerado.Formato_Plano.Internacional)
            {
                request.plan_ddi = String.IsNullOrEmpty(circuito.Clientes_PlanosProdutos?.Plano?.CodigoAuxiliar) ? (int?)null : Convert.ToInt32(circuito.Clientes_PlanosProdutos?.Plano?.CodigoAuxiliar);
            }

            if (circuito.TipoDiscagem == Model.enumerado.TipoDiscagem.STFC)
            {
                request.plan_type = "stfc";
            }
            else if (circuito.TipoDiscagem == Model.enumerado.TipoDiscagem.SCM)
            {
                request.plan_type = "scm";
            }
            else if (circuito.TipoDiscagem == Model.enumerado.TipoDiscagem.Dinamico)
            {
                request.plan_type = "custom-route";
            }

            var json = JsonService.SerializeObject(request);
            var response = EnviarRequisicaoPost("/get_sip/create", json);
            return JsonService.DeserializeObject<CircuitoIntegracaoResponse>(response);

        }
        public CircuitoIntegracaoResponse Atualizar(CircuitoTelefonia circuito)
        {

            CircuitoIntegracaoRequest request = new CircuitoIntegracaoRequest();
            request.call_LC = circuito.HabilitarLc;
            request.call_LDI = circuito.HabilitarLdi;
            request.call_LDN = circuito.HabilitarLdn;
            request.call_VC1 = circuito.HabilitarVc1;
            request.call_VC2 = circuito.HabilitarVc2;
            request.call_VC3 = circuito.HabilitarVc3;
            request.sip_password = circuito.Senha;
            request.cnpj = circuito?.Clientes_PlanosProdutos?.Cliente?.CpfCnpj;
            request.active = circuito.Status == Sinapse.Model.enumerados.Status_Default.Ativo ? "1" : "0";
            request.show_callerid = circuito.HabilitarBina;
            request.name = "teste";

            if (circuito?.Clientes_PlanosProdutos?.Plano?.Formato_do_Plano == Model.enumerado.Formato_Plano.Nacional)
            {
                request.plan_national = String.IsNullOrEmpty(circuito.Clientes_PlanosProdutos?.Plano?.CodigoAuxiliar) ? (int?)null : Convert.ToInt32(circuito.Clientes_PlanosProdutos?.Plano?.CodigoAuxiliar);
            }
            if (circuito?.Clientes_PlanosProdutos?.Plano?.Formato_do_Plano == Model.enumerado.Formato_Plano.Internacional)
            {
                request.plan_ddi = String.IsNullOrEmpty(circuito.Clientes_PlanosProdutos?.Plano?.CodigoAuxiliar) ? (int?)null : Convert.ToInt32(circuito.Clientes_PlanosProdutos?.Plano?.CodigoAuxiliar);
            }

            if (circuito.TipoDiscagem == Model.enumerado.TipoDiscagem.STFC)
            {
                request.plan_type = "stfc";
            }
            else if (circuito.TipoDiscagem == Model.enumerado.TipoDiscagem.SCM)
            {
                request.plan_type = "scm";
            }
            else if (circuito.TipoDiscagem == Model.enumerado.TipoDiscagem.Dinamico)
            {
                request.plan_type = "custom-route";
            }

            var json = JsonService.SerializeObject(request);
            var response = EnviarRequisicaoPut($"/get_sip/update/{circuito.Numero_Sip}", json);
            return JsonService.DeserializeObject<CircuitoIntegracaoResponse>(response);

        }

        //public void Excluir(Did did)
        //{
        //    EnviarRequisicaoGet("teste");
        //}

    }


}