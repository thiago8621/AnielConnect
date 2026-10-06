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
    public class DidIntegracaoSaperxService : BaseIntegracaoSaperxService
    {
        public DidIntegracaoResponse Cadastrar(Did did)
        {
            DidIntegracaoRequest request = new DidIntegracaoRequest();

            var specifier = "F";
            var culture = CultureInfo.CreateSpecificCulture("en-CA");
            request.client_value = did.Valor.HasValue ? did.Valor.Value.ToString(specifier, culture) : "0";
            request.ACB = did.ChamadaCobrar;
            request.can_sell = true;
            request.number = did.Numero;
            request.city = $"{did.Cidade.Descricao} / {did.UF}";
            request.city = $"{did.Cidade.Descricao} / {did.UF}";
            request.id_sip = (int?)did?.Circuito?.Id;


            var json = JsonService.SerializeObject(request);
            var result = EnviarRequisicaoPost("/get_did/create", json);
            return JsonService.DeserializeObject<DidIntegracaoResponse>(result);

        }
        public DidIntegracaoResponse Atualizar(Did did)
        {
            DidIntegracaoRequest request = new DidIntegracaoRequest();

            var specifier = "F";
            var culture = CultureInfo.CreateSpecificCulture("en-CA");
            request.client_value = did.Valor.HasValue ? did.Valor.Value.ToString(specifier, culture) : "0";
            request.ACB = did.ChamadaCobrar;
            request.can_sell = did.StatusDid == Model.enumerado.StatusDid.Disponivel;
            request.number = did.Numero;
            request.city = $"{did.Cidade.Descricao} / {did.UF}";
            request.ddr_reserved = did.Reservado_Ddr;
            request.id_sip = (int?)did?.Circuito?.Id;


            var json = JsonService.SerializeObject(request);
            var result = EnviarRequisicaoPut($"/get_did/update/{did.CodigoAuxiliar}", json);
            return JsonService.DeserializeObject<DidIntegracaoResponse>(result);
        }

        //public void Excluir(Did did)
        //{
        //    EnviarRequisicaoGet("teste");
        //}

    }


}