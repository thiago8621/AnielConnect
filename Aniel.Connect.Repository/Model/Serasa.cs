using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Repository.Model
{
    public interface ISerasaPadraoRetorno
    {
        string Protocolo { get; set; }
        SerasaPadraoEnum TipoProtocolo { get; set; }
        string SubTipo { get; set; }
        string Mensagem { get; set; }        
        List<string> GetTotal();
        List<string> GetComplemento();
        List<string> GetItens();
    }
    /// <summary>
    /// N200
    /// </summary>
    public class Confirmei : ISerasaPadraoRetorno
    {
        public Confirmei()
        {
            this.Protocolo = "N200";
            this.TipoProtocolo = SerasaPadraoEnum.N200;
        }
        public string SituacaoDocumento { get; set; }
        public string NomeMae { get; set; }
        public string Protocolo { get; set; }
        public SerasaPadraoEnum TipoProtocolo { get; set; }
        public string SubTipo { get; set; }
        public string Mensagem { get; set; }               
        public List<string> GetItens()
        {
            string situação = this.SituacaoDocumento == "Regular" ? $"Situação: {this.SituacaoDocumento}" :$"{RsBase.SITUACAO_SERASA_REPROVADA}: {this.SituacaoDocumento}";
            return new List<string> 
            { 
                $"{situação}",                                
            };
        }
        public List<string> GetTotal()
        {
            return new List<string>
            {
                $"Nome da Mãe: {NomeMae}"
            };
        }
          public List<string> GetComplemento()
        {
            return new List<string>();
        }
    }
    /// <summary>
    /// N240
    /// </summary>
    public class PendenciaFinanceiraSerasa : ISerasaPadraoRetorno
    {
        public PendenciaFinanceiraSerasa()
        {
            this.Protocolo = "N240";
            this.TipoProtocolo = SerasaPadraoEnum.N240;
        }
        public string ValorDebito { get; set; }
        public string ValorTotal { get; set; }
        public string DebitosTotais { get; set; }
        public string Modalidade { get; set; }
        public string Avalista { get; set; }
        public string Moeda { get; set; }
        public string Contrato { get; set; }
        public string Origem { get; set; }
        public string SubJudice { get; set; }
        public string MensagemSubJudice { get; set; }
        public string TotalOcorrenciasDebitos { get; set; }
        public string Dataultimadebito { get; set; }
        public string Protocolo { get; set; }
        public SerasaPadraoEnum TipoProtocolo { get; set; }
        public string SubTipo { get; set; }
        public string Mensagem { get; set; }
        public List<String> GetTotal() 
        {
            var total = String.IsNullOrEmpty(this.DebitosTotais) ? Convert.ToString(decimal.Parse("0,00")) :
                                                             Convert.ToString(decimal.Parse(this.DebitosTotais));

            return new List<string> 
            {
                total,
                $"{Convert.ToInt32(this.TotalOcorrenciasDebitos)}",
            };
        }
        public List<string> GetComplemento()
        {          
            var eSubjudice = this.SubJudice == "N" || this.SubJudice == " " ? "Não" : "Sim";
            return new List<string>
            {
                $"Subjudice: {eSubjudice}",
                eSubjudice == "Não" ? this.MensagemSubJudice : "",                                               
            };
        }
        public List<string> GetItens()
        {
            var valorDebito = String.IsNullOrEmpty(this.ValorDebito) ? Convert.ToString(decimal.Parse("0,00")) :
                                                               Convert.ToString(decimal.Parse(this.ValorDebito));
            var avalista = this.Avalista == "N" || this.Avalista == " " ? "Não" : "Sim"; 
            return new List<string>
            {
                $"Modalidade: {this.Modalidade}", 
                $"Avalista: {avalista}", 
                $"Data do Último Débito: {this.Dataultimadebito}", 
                $"Valor do Débito: {this.Moeda}{valorDebito}",
                $"Contrato: {this.Contrato}", 
                $"Origem: {this.Origem}",
                "<br/>"
            };
        }
    }
    /// <summary>
    /// N250
    /// </summary>    
    public class Protesto : ISerasaPadraoRetorno
    {
        public Protesto()
        {
            this.Protocolo = "N250";
            this.TipoProtocolo = SerasaPadraoEnum.N250;
        }
        public string ProtestoDataOcorrencia { get; set; }
        public string ProtestosMoeda { get; set; }
        public string ProtestoCartorio { get; set; }
        public string ProtestoCidadeOrigem { get; set; }
        public string ProtestoUf { get; set; }
        public string ProtestoSubjudice { get; set; }
        public string ProtestoMsgSubjudice { get; set; }
        public string ProtestoProtestoTotalOcorrencia { get; set; }
        public string ProtestoDataOcorrenciaAntiga { get; set; }
        public string ProtestoDataOcorrenciaRecente { get; set; }
        public string ProtestosValor { get; set; }
        public string ProtestosValorTotal { get; set; }
        public string ProtestosMoedaTotal { get; set; }
        public string Protocolo { get; set; }
        public SerasaPadraoEnum TipoProtocolo { get; set; }
        public string SubTipo { get; set; }
        public string Mensagem { get; set; }

        public List<string> GetComplemento() 
        {
            var eSubjudice = this.ProtestoSubjudice == "N" || this.ProtestoSubjudice == " " ? "Não" : "Sim";
            return new List<string>
            {
                $"Subjudice: {eSubjudice}",
                eSubjudice == "Não" ? this.ProtestoMsgSubjudice : "",                
            };
        }
        public List<string> GetItens()
        {
            var dataOcorrencia = DateTime.ParseExact(this.ProtestoDataOcorrencia, "ddmmyyyy", null).ToString("dd/mm/yyyy");
            var valor = String.IsNullOrEmpty(this.ProtestosValor) ? Convert.ToString(decimal.Parse("0,00")) :
                                                              Convert.ToString(decimal.Parse(this.ProtestosValor));
            return new List<string>
            {                
                $"Data de Ocorrência: {dataOcorrencia}",
                $"Valor do Protesto: {this.ProtestosMoeda}{valor}",
                $"Cartório: {this.ProtestoCartorio}",
                $"Protestos Cidade de Origem: {this.ProtestoCidadeOrigem}",
                $"Protestos UF de Origem: {this.ProtestoUf}",                                
            };
        }
        public List<string> GetTotal()
        {
            var dataOcorrenciaAntiga = DateTime.ParseExact(this.ProtestoDataOcorrenciaAntiga, "MMyyyy", null)
                .ToString("MM/yyyy");
            var dataOcorrenciaRecente = DateTime.ParseExact(this.ProtestoDataOcorrenciaRecente, "MMyyyy", null)
                .ToString("MM/yyyy");
            var valorTotal = String.IsNullOrEmpty(this.ProtestosValorTotal) ? Convert.ToString(decimal.Parse("0,00")) :
                                                              Convert.ToString(decimal.Parse(this.ProtestosValorTotal));

            return new List<string> 
            {
                $"Protestos Data de Ocorrência mais Antiga: {dataOcorrenciaAntiga}",
                $"Protestos Data de Ocorrência mais Recente: {dataOcorrenciaRecente}",
                $"Protestos Valor Total: {this.ProtestosMoedaTotal}{valorTotal}",  
                $"Total de Ocorrências de Protesto: {Convert.ToInt32(this.ProtestoProtestoTotalOcorrencia)}",
            };            
        }
    }
    /// <summary>
    /// N270
    /// </summary>
    public class ChequeBacen : ISerasaPadraoRetorno
    {
        public ChequeBacen()
        {
            this.Protocolo = "N270";
            this.TipoProtocolo = SerasaPadraoEnum.N270;
        }
        public string DataOcorrencia { get; set; }
        public string NumCheque { get; set; }
        public string AlineaCheque { get; set; }
        public string QdadeCCFparaBanco { get; set; }
        public string ValorTotalCheque { get; set; }
        public string NumBanco { get; set; }
        public string NomeBanco { get; set; }
        public string NumAgencia { get; set; }
        public string CidadeAgencia { get; set; }
        public string UfAgencia { get; set; }
        public string Protocolo { get; set; }
        public SerasaPadraoEnum TipoProtocolo { get; set; }
        public string SubTipo { get; set; }
        public string Mensagem { get; set; }
        public List<string> GetComplemento()
        {
            return new List<string>();
        }
        public List<string> GetItens()
        {
            var dataOcorrencia = DateTime.ParseExact(this.DataOcorrencia, "ddmmyyyy", null).ToString("dd/mm/yyyy");            
            return new List<string> 
            { 
                $"Data da Ocorrência: {dataOcorrencia}",
                $"Número do Cheque: {this.NumCheque}",
                $"Número do Banco: {this.NumBanco}",
                $"Nome do Banco: {this.NomeBanco}",
                $"Uf da Agencia: {this.UfAgencia}",
                $"Cidade da Agencia: {this.CidadeAgencia}",
                $"Número da Agencia: {this.NumAgencia}",
            };
        }
        public List<string> GetTotal()
        {
            return new List<string> 
            {

            };
        }
    }
    /// <summary>
    /// N440
    /// </summary>
    public class ConsultasA_Serasa : ISerasaPadraoRetorno
    {
        public ConsultasA_Serasa()
        {
            this.Protocolo = "N440";
            this.TipoProtocolo = SerasaPadraoEnum.N440;
        }
        public string TotalChequesEmitidosaVista15Dias { get; set; }
        public string TotalChequesEmitidosaPrazo30Dias { get; set; }
        public string TotalChequesEmitidosaPrazo60Dias { get; set; }
        public string TotalChequesEmitidosaPrazo { get; set; }
        public string ConsultasSerasaMercadoRefComercial { get; set; }
        public string ConsultasSerasaSemCheques { get; set; }
        public string Protocolo { get; set; }
        public SerasaPadraoEnum TipoProtocolo { get; set; }
        public string SubTipo { get; set; }
        public string Mensagem { get; set; }
        public List<string> GetComplemento()
        {
            return new List<string>();
        }
        public List<string> GetItens()
        {
            return new List<string> 
            {
               
            };
        }
        public List<string> GetTotal()
        {
            return new List<string> 
            {
                $"Total de Cheques Emitidos à vista em 15 dias: {this.TotalChequesEmitidosaVista15Dias}",
                $"Total de Cheques Emitidos à prazo em 30 dias: {this.TotalChequesEmitidosaPrazo30Dias}",
                $"Total de Cheques Emitidos à prazo em 60 dias: {this.TotalChequesEmitidosaPrazo60Dias}",
            };

        }
    }
    /// <summary>
    /// N700
    /// </summary>
    public class SocioAcionista : ISerasaPadraoRetorno
    {
        public SocioAcionista()
        {
            this.Protocolo = "N700";
            this.TipoProtocolo = SerasaPadraoEnum.N700;
        }

        public string Protocolo { get; set; }
        public SerasaPadraoEnum TipoProtocolo { get; set; }
        public string SubTipo { get; set; }
        public string Mensagem { get; set; }
        public string TipoPessoa { get; set; }
        public string SocioDocumento { get; set; }
        public string SocioNome { get; set; }
        public string SocioPercentualCapital { get; set; }
        public string SocioRestricao { get; set; }
        public List<string> GetComplemento()
        {
            return new List<string>();
        }
        public List<string> GetItens()
        {
            return new List<string> 
            {             
                $"Sócios: {this.Mensagem}",
                $"Sócios Nome: {this.SocioNome}",
                $"Sócios Tipo Pessoa: {this.TipoPessoa}",
                $"Sócios Documento: {this.SocioDocumento}",
                $"Sócios % Capital: {this.SocioPercentualCapital}",
                $"Sócios Restrições: {this.SocioRestricao}",            
            };
        }
        public List<string> GetTotal()
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// F900
    /// </summary>
    public class Score : ISerasaPadraoRetorno
    {
        public Score(string documento)
        {
            this.Protocolo = "F900";
            this.TipoProtocolo = SerasaPadraoEnum.F900;
            this.PessoaFisica = documento.Length == 11;
        }
        public bool PessoaFisica { get; set; }
        public string ScoreSerasa { get; set; }
        public string FaixaScore { get; set; }
        public string InadimplenciaProbab { get; set; }
        public string MsgScore { get; set; }
        public string ScoreRetornoInformacoes { get; set; }
        public string ScoreDataConsulta { get; set; }
        public string ScoreHoraConsulta { get; set; }
        public string ClasseTabelaScore { get; set; }
        public string MsgInformativaPj { get; set; }
        public string CalculoRisco { get; set; }
        public string Protocolo { get; set; }
        public SerasaPadraoEnum TipoProtocolo { get; set; }
        public string SubTipo { get; set; }
        public string Mensagem { get; set; }
        public List<string> GetComplemento()
        {
            return new List<string>();
        }
        public List<string> GetItens()
        {
            var retorno = new List<string>();
            if (PessoaFisica)
            {
                retorno.AddRange(new List<string>
                {
                    $"Score Positivo: {ScoreSerasa}",
                    $"Classificação Faixa Score: {FaixaScore}",
                    $"Probabilidade de Inadimplência: {InadimplenciaProbab}",
                    //$"Mensagem Score Positivo: {MsgScore}",
                });
            }
            else
            {
                retorno.AddRange(new List<string>
                {
                    $"Score Positivo: {ScoreSerasa}",
                    $"Classificação Faixa Score: {FaixaScore}",
                    $"Probabilidade de Inadimplência: {InadimplenciaProbab}",
                    //$"Mensagem Score Positivo: {MsgInformativaPj}",                                      
                    $"Retorno de Informacoes: {ScoreRetornoInformacoes}",
                    $"Data/Hora da Consulta: {ScoreDataConsulta} {ScoreHoraConsulta}",
                    $"Calculo do Risco: {CalculoRisco}",
                   //ClasseTabelaScore,
                });
            }
            return retorno;
        }
        public List<string> GetTotal()
        {
            throw new NotImplementedException();
        }
    }
    public class T999 : ISerasaPadraoRetorno
    {
        public T999()
        {
            this.Protocolo = "T999";
            this.TipoProtocolo = SerasaPadraoEnum.T999;
        }
        public string Protocolo { get; set; }
        public SerasaPadraoEnum TipoProtocolo { get; set; }
        public string SubTipo { get; set; }
        public string Mensagem { get; set; }
        public List<string> GetItens()
        {
            return new List<string> 
            { 
                $"Mensagem Final: {this.Mensagem}" 
            };
        }
        public List<string> GetTotal()
        {
            throw new NotImplementedException();
        }
        public List<string> GetComplemento()
        {
            return new List<string>();
        }
    }
    public enum SerasaPadraoEnum
    {
        [Display(Name = "N200", ResourceType = typeof(Resources.RsCadastros))]
        N200,
        [Display(Name = "N240", ResourceType = typeof(Resources.RsCadastros))]
        N240,
        [Display(Name = "N250", ResourceType = typeof(Resources.RsCadastros))]
        N250,
        [Display(Name = "N270", ResourceType = typeof(Resources.RsCadastros))]
        N270,
        [Display(Name = "N440", ResourceType = typeof(Resources.RsCadastros))]
        N440,
        [Display(Name = "N700", ResourceType = typeof(Resources.RsCadastros))]
        N700,
        [Display(Name = "F900", ResourceType = typeof(Resources.RsCadastros))]
        F900,
        [Display(Name = "T999", ResourceType = typeof(Resources.RsCadastros))]
        T999,
    }
}
