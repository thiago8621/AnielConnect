using Aniel.Connect.Core;
using Sinapse.Model.enumerados;
using System;

namespace Aniel.Connect.Models
{
    public class TrocaTitularidadeRequest : DataTableRequest
    {
        public TipoPeriodo? TipoPeriodo { get; set; }
        public string DataInicial { get; set; }
        public string DataFinal { get; set; }
        public StatusTrocaTitularidadeModels? StatusTrocaTitularidade { get; set; }
        public TipoPessoaTitular? TipoPessoaAtual { get; set; }
        public TipoPessoaTitular? TipoPessoaNovo { get; set; }
        public EnumEtapaVenda? EtapaAtual { get; set; }
        public string TermoAssinado { get; set; }
        public string ChecklistPreenchido { get; set; }
        public bool PossuiAcesso { get; set; }
    }
    
    public class TrocaTitularidadeResponse
    {
        public long Id { get; set; }
        public string Numero { get; set; }
        public string StatusTroca { get; set; }
        public string TipoPessoaTitularAtual { get; set; }
        public string NomeFantasiaTitularAtual { get; set; }
        public string NomeFantasiaTitularNovo { get; set; }
        public string DocumentoTitularAtual { get; set; }
        public string TipoPessoaTitularNovo { get; set; }
        public string DocumentoTitularNovo { get; set; }
        public string CategoriaTitularNovo { get; set; }
        public string UsuarioCriacao { get; set; }
        public string DataHoraCriacao { get; set; }
        public string EtapaAtual { get; set; }
        public string ChecklistPreenchido { get; set; }
        public string TermoTitularNovo { get; set; }
        public string DataAprovacao { get; set; }
        public string DataCancelamento { get; set; }
        public string UsuarioAprovacao { get; set; }
        public string UsuarioCancelamento { get; set; }
    }
}