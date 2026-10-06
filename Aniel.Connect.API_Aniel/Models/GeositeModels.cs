using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Aniel.Connect.API_Aniel.Models
{
    public class ResponseGeosite
    {
        public bool success { get; set; }
        public long? fid { get; set; }
        public String message { get; set; }

    }
    public class ResponseGeosite<T> : ResponseGeosite where T : class
    {
        public string page { get; set; }
        public string total { get; set; }
        public List<T> records { get; set; }

    }

    public class ResponseGeositeReserva : ResponseGeosite
    {
        public String porta { get; set; }
        public String dataExpiracao { get; set; }

    }
    public class ResponseGeositeViabilidade : ResponseGeosite
    {
        private List<CaixaGeosite> caixas;
        public List<CaixaGeosite> caixasGeosite
        {
            get
            {
                if (caixas == null) { caixas = new List<CaixaGeosite>(); }
                return caixas;
            }
            set { caixas = value; }
        }

    }
    public class Request_ViabilidadeGeosite
    {
        public double Raio { get; set; }
        public String Endereco { get; set; }
        public Double? Latitude { get; set; }
        public Double? Longitude { get; set; }
        public String GooglePlaceId { get; set; }
        public viabilidade_GeositeParameter Mudanca_RequisicaoViabilidade { get; set; }
    }
    public class CaixaGeosite
    {
        public string tipoCodigo { get; set; }
        public string distancia { get; set; }
        public string qtdTotalDisponivel { get; set; }
        public string qtdSplitter { get; set; }
        public string qtdPortasSplitter { get; set; }
        public string qtdPortasSplitterDisp { get; set; }
        public string qtdPortasSplitterOcup { get; set; }
        public string qtdEquipamentosPacpon { get; set; }
        public string qtdPortasEthernet { get; set; }
        public string qtdPortasEthernetDisp { get; set; }
        public string qtdPortasEthernetOcup { get; set; }
    }
    public enum viabilidade_GeositeParameter
    {
        Endereco = 1,
        LatitudeLongitude = 2,
        GooglePlaceId = 3,
    }
    public class GeositeCaixaPortaFtthModel
    {

        public long? fidCaixa { get; set; }
        public long? fidCliente { get; set; }
    }
    public class Ligacao_Porta_GeositeRequestModel
    {
        public long porta { get; set; }
        public long cliente_PlanoProduto { get; set; }
    }
    public class ClienteGeosite
    {
        /*
         *                                     Campos Problemáticos para o Save do Cliente.

               ----------            {"fid","sinal","indDedicado","fidTipo","fidFluxoAtendimento",comprimentoFibra}           ----------        

         */

        public Int64 fid { get; set; }
        public Int64? fidTipo { get; set; }
        public string indDedicado { get; set; }
        public string fidFluxoAtendimento { get; set; }
        public Int64? sinal { get; set; }
        public string codigo { get; set; }
        public string nome { get; set; }
        public string documento { get; set; }
        public string contrato { get; set; }
        public Double? latitude { get; set; }
        public Double? longitude { get; set; }
        public string bairro { get; set; }
        public string endereco { get; set; }
        public Int32? numeroPorta { get; set; }
        public string complementoEndereco { get; set; }
        public string telefone { get; set; }

        public string macOnu { get; set; }
        public string codigoOnu { get; set; }
        public Int64? qtdPortasAtendimento { get; set; }
        public Int64 comprimentoFibra { get; set; }
        public string infoComplementar { get; set; }
        public string codigoCaixa { get; set; }
        public Int64? fidTipoCaixa { get; set; }
        public Int64? fidCaixa { get; set; }


    }
    public class SaveModelCliente
    {
        public string codigo { get; set; }
        public string nome { get; set; }
        public string documento { get; set; }
        public string contrato { get; set; }
        public Double? latitude { get; set; }
        public Double? longitude { get; set; }
        public string bairro { get; set; }
        public string endereco { get; set; }
        public Int32? numeroPorta { get; set; }
        public string complementoEndereco { get; set; }
        public string telefone { get; set; }

        public string macOnu { get; set; }
        public string codigoOnu { get; set; }
        public Int64? qtdPortasAtendimento { get; set; }
        public Int64 comprimentoFibra { get; set; }
        public string infoComplementar { get; set; }
        public string codigoCaixa { get; set; }
        public Int64? fidTipoCaixa { get; set; }
        public Int64? fidTipo { get; set; }
        public Int64? fidCaixa { get; set; }
    }

}