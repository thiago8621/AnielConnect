using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.API_Aniel.Models
{
    public class FaturamentoListModel
    {
        public string Protocolo { get; set; }
        public string IdFatura { get; set; }
        public string IdCliente { get; set; }
        public string CPFCNPJ{ get; set; }
        public string DataEmissao { get; set; }
        public string DataFechamento { get; set; }
        public string DataVencimento { get; set; }
        public string Valor { get; set; }
        public string CNPJFilial { get; set; }
    }
    
    public class ComposicaoFaturaListModel
    {
        public string IdFatura { get; set; }
        public string ModeloNF { get; set; }
        public string IdClassificacaoTributaria{ get; set; }
        public string CFOP { get; set; }
        public double Valor { get; set; }
    }

    public class FaturamentoApiRequest : ApiRequest
    {
        public string IdCliente { get; set; }
        public string DataInicial { get; set; }
        public string DataFinal { get; set; }
    }

}