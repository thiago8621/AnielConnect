using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.API_Aniel.Models
{
    public class PlanoProdutoComboPropostaVendaRequest : ApiRequest
    {
        public long? IdVenda { get; set; }
        public long? IdPlano { get; set; }
        public long? IdProduto { get; set; }
        public long? IdCombo { get; set; }
        public long? IdCampanha { get; set; }
        public long IdFormaPagamento { get; set; }
        public string Banco { get; set; }
        public string Conta_Corrente { get; set; }
        public string Agencia { get; set; }
        public string Digito { get; set; }
        public string Vencimento { get; set; }
        public string Dia_Vencimento { get; set; }
        public Cliente_FormaEnvio? Forma_Envio { get; set; }
        public bool End_Cobranca_Igual_Principal { get; set; }
        public EnderecoRequestModel Endereco_Cobranca { get; set; }
        public string DescricaoPlanoProduto { get; set; }
        public string Detalhes { get; set; }
        public long Numero_Contrato { get; set; }
        public string ValorPlanoProduto_Final { get; set; }
        public Status_PlanoProduto? StatusPlanoProduto { get; set; }
       
        public string Desc_Forma_Pagto { get; set; }
        public string NecessitaCriarOS { get; set; }
        public string PossuiOSCriada { get; set; }
        public string NumOS { get; set; }
        public string ValorPlanoProduto { get; set; }
        public string ValorTotal { get; set; }
        public string FormaEnvioFatura { get; set; }
        public DateTime DataVencimento { get; set; }
        public bool Teste_Gratis { get; set; }
        public bool Periodo_Carencia { get; set; }
        public long Id_Modelo_Contrato { get; set; }
        public long MesesCarencia { get; set; }
        public bool Combo { get; internal set; }
    }
    public class EnderecoRequestModel
    {
        public Pais? Pais { get; set; }
        public string UF { get; set; }
        public string Cidade { get; set; }
        public string Bairro { get; set; }
        public string Cep { get; set; }

        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Ponto_Referencia { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }

        public string Cod_Escritorio { get; set; }
        public string Cod_Area { get; set; }
        public string Cod_Localidade { get; set; }

        public int? Cod_TipoImovel { get; set; }
    }

    public class VincularPlanoProdutoComboLoteRequest : ApiRequest
    {
        public long IdVenda { get; set; }    
        public long IdFormaPagamento { get; set; }
        public string Desc_Forma_Pagto { get; set; }
        public string Banco { get; set; }
        public string Conta_Corrente { get; set; }
        public string Agencia { get; set; }
        public string Digito { get; set; }
        public string Dia_Vencimento { get; set; }
        public Cliente_FormaEnvio? Forma_Envio { get; set; }
        public bool End_Cobranca_Igual_Principal { get; set; }
        public EnderecoRequestModel Endereco_Cobranca { get; set; }
        public string FormaEnvioFatura { get; set; }
        public DateTime DataVencimento { get; set; }

        public List<VinculoPlanoProdutoComboItemRequest> Itens { get; set; }
    }

    public class VinculoPlanoProdutoComboItemRequest
    {
        public long? IdPlano { get; set; }
        public long? IdProduto { get; set; }
        public long? IdCombo { get; set; }
        public long? IdCampanha { get; set; }

        public string DescricaoPlanoProduto { get; set; }
        public string Detalhes { get; set; }

        public Status_PlanoProduto? StatusPlanoProduto { get; set; }
        public bool Teste_Gratis { get; set; }
        public bool Periodo_Carencia { get; set; }
        public long MesesCarencia { get; set; }
        public long Id_Modelo_Contrato { get; set; }
   
        public string NecessitaCriarOS { get; set; }
        public string PossuiOSCriada { get; set; }
        public string NumOS { get; set; }
      
        public string ValorPlanoProduto { get; set; }
        public string ValorTotal { get; set; }
        public string ValorPlanoProduto_Final { get; set; }
    }
}