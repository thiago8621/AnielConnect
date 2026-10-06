using Aniel.Connect.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models.T5
{
    

    public class Codigo_ComplementarListaModel : ObjComboResult<String>
    {
        public String CodMat { get; set; }
    }

    public class PosicaoEstoque
    {
        public string Posicao { get; set; }
        public string InfoAdicional { get; set; }
    }

    public class Codigo_ComplementarConfigCombo
    {
        public String codct { get; set; }
        public String codMat { get; set; }
        public String projeto { get; set; }
        public Boolean aceitaNovo { get; set; }
    }

    public class Codigo_ComplementarTemEstoque
    {
        public String CODCPL { get; set; }
        public String CODMAT { get; set; }
        public String CODCT { get; set; }
        public String PROJETO { get; set; }
        public String EQUIPE { get; set; }
        public String NOME { get; set; }
        public DateTime? DATA { get; set; }
        public DateTime? DIA { get; set; }
        public DateTime? HORA { get; set; }
        public String SEQLIVRE { get; set; }
        public String SEQRMDM { get; set; }
        public String SEQTRANSF { get; set; }
        public Double? ENTRADA { get; set; }
        public Double? SAIDA { get; set; }
        public Int16? COD_TIPO_MOVTO { get; set; }
        public String FORNECEDOR { get; set; }
        public String USUARIO { get; set; }
        public String PEDIDO { get; set; }
        public String CLASSIFICACAO_RMDM { get; set; }
        public String COD_FORNECEDOR { get; set; }
        public String NOTAFISCAL { get; set; }
    }

    public class Codigo_ComplementarTemAplicacao
    {
        public String CODCT { get; set; }
        public String PROJETO { get; set; }
        public String TAREFA { get; set; }
        public String NUM_OBRA { get; set; }
        public String CODMAT { get; set; }
        public String CODCPL { get; set; }
        public String NUM_PROTOCOLO { get; set; }
        public DateTime? DIA { get; set; }
        public DateTime? HORA { get; set; }
        public String EQUIPE { get; set; }
        public String NOME { get; set; }
        public DateTime? DATA_APLIC { get; set; }
        public String USUARIO { get; set; }

        public Int32? NUM_DOC { get; set; }
        public Double? QUANT_REMOV { get; set; }

        public DateTime? DATA_PROTOCOLO { get; set; }
        public String CLASSIFICACAO_RMDM { get; set; }
    }

    public enum Codigo_ComplementarLocalizacaoEnum
    {
        [Display(Name = "FISICO", ResourceType = typeof(Resources.RsCadastros))]
        Fisico = 0,
        [Display(Name = "VOLANTE", ResourceType = typeof(Resources.RsCadastros))]
        Volante = 1,
        [Display(Name = "SEMSALDO", ResourceType = typeof(Resources.RsCadastros))]
        SemSaldo = 2,
        [Display(Name = "FORNECEDOR", ResourceType = typeof(Resources.RsCadastros))]
        Fornecedor = 3,
        [Display(Name = "REMOVIDO", ResourceType = typeof(Resources.RsCadastros))]
        Removido = 4,
        [Display(Name = "NAOENCONTRADO", ResourceType = typeof(Resources.RsCadastros))]
        NaoEncontrado = 5,
        [Display(Name = "APLICADO", ResourceType = typeof(Resources.RsCadastros))]
        Aplicado = 6,
    }

    public class Codigo_ComplementarLocalizacao
    {
        public Codigo_ComplementarLocalizacaoEnum Status { get; set; }
        public DateTime? Dia {get; set;}
        public DateTime? Hora { get; set; }
        public String Classificacao { get; set; }
    }

}