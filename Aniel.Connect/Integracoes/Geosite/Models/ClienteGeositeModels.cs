using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Integracoes.Geosite.Models
{

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