using Aniel.Connect.Model.enumerado;
using Aniel.Connect.ModelLog.nhibernate;
using Resources;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using Sinapse.ModelLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Model.nhibernate
{
    public class ClienteContrato : BaseModel
    {
        #region Propriedades

        public virtual Cliente Cliente { get; set; }
        public virtual Cliente_Anexo Cliente_Anexo { get; set; }
        public virtual Modelo_Contrato ModeloContrato { get; set; }
        public virtual Endereco_Cliente Endereco_Cliente { get; set; }
        public virtual Desconto Desconto { get; set; }
        public virtual string NumeroContrato { get; set; }
        public virtual string Url_Assinar_Documento { get; set; }
        public virtual string Url_Documento_Assinado { get; set; }
        public virtual string Token_Documento_Assinatura { get; set; }
        public virtual string Descricao { get; set; }

        /// <summary>
        ///     Arquivo gerado a partir do modelo com os dados do contrato
        /// </summary>
        public virtual byte[] Arquivo { get; set; }
        public virtual string NomeArquivo { get; set; }

        public virtual bool ContratoAutomatico { get; set; }
        public virtual bool TemCarencia { get; set; }
        public virtual long? PeriodoCarencia { get; set; }
        public virtual double? MultaCarencia { get; set; }
        public virtual bool TemReajuste { get; set; }
        public virtual bool RenovarAutomaticamente { get; set; }
        public virtual Renova_FidelidadeEnum? RenovaCarenciaAutomaticamente { get; set; }
        public virtual Tipo_Reajuste_Contratual? TipoReajuste { get; set; }
        public virtual Frequencia_Reajuste? FrequenciaReajuste { get; set; }
        public virtual bool CancelaContratoAutomaticoFinalCarencia { get; set; }

        public virtual bool TemProRata { get; set; }
        public virtual bool MultaFidelidade { get; set; }

        public virtual double PercentualReajusteContratual { get; set; }
        public virtual double ValorReajusteContratual { get; set; }

        public virtual bool UtilizaDesconto { get; set; }
        public virtual Status_Aceite_Contrato? StatusAceiteContrato { get; set; }
        public virtual DateTime? DataAceiteContrato { get; set; }
        #region AceiteDigitalContrato
        public virtual String IpOrigemAceite { get; set; }
        public virtual String UserAgentContratoAceite { get; set; }
        public virtual String CodigoHashContratoAceite { get; set; }
        public virtual String UrlAceiteContrato { get; set; }
        public virtual String PortaOrigemAceite { get; set; }
        public virtual Boolean AdicionadoAceiteDigital { get; set; }
        #endregion
        public virtual TipoIntegracaoAssinaturaEnum? Tipo_Integracao_Assinatura { get; set; }
        public virtual string CodIntegracao { get; set; }
        public virtual Boolean IsContratoAditivo { get; set; }
        

        #region Impedir Bloqueio

        public virtual bool ImpedirBloqueioAutomatico { get; set; }
        public virtual bool ImpedirBloqueioSegunda { get; set; }
        public virtual bool ImpedirBloqueioTerca { get; set; }
        public virtual bool ImpedirBloqueioQuarta { get; set; }
        public virtual bool ImpedirBloqueioQuinta { get; set; }
        public virtual bool ImpedirBloqueioSexta { get; set; }
        public virtual bool ImpedirBloqueioSabado { get; set; }
        public virtual bool ImpedirBloqueioDomingo { get; set; }
        public virtual bool ImpedirBloqueioFeriados { get; set; }

        #endregion

        #region Teste Grátis

        public virtual bool TesteGratis { get; set; }
        public virtual short? DiasTeste { get; set; }

        #endregion

        #region Desbloqueio Confiança

        public virtual bool UtilizaDesbloqueioConfianca { get; set; }
        public virtual Frequencia_Desbloqueio? FrequenciaDesbloqueio { get; set; }
        public virtual short QtdDiasDesbloqueados { get; set; }

        public virtual Status_Contrato? Status_Contrato { get; set; }
        public virtual DateTime? Data_Desbloqueio_Confianca { get; set; }

        #endregion

        #endregion

        /// <summary>
        ///     Cria um objeto de contrato para um cliente baseado no modelo especificado
        /// </summary>
        /// <param name="modelo">Modelo de contrato que será utilizado como referência</param>
        /// <param name="cliente">Cliente que será amarrado ao contrato</param>
        public static ClienteContrato FromModelo(Modelo_Contrato modelo, Cliente cliente)
        {
            string nomeArquivo = "";

            // remove a extensão do arquivo e deixa somente o nome
            List<string> nomeArquivoSplit = new List<string>();
            if (modelo.Arquivo_Doc != null)
            {
                nomeArquivoSplit = modelo.Arquivo_Doc.Split('.').ToList();
                nomeArquivoSplit.RemoveAt(nomeArquivoSplit.Count - 1);
                nomeArquivo = String.Join(".", nomeArquivoSplit);
            }
                  
            var retorno = new ClienteContrato();

            retorno.Cliente = cliente;
            retorno.ModeloContrato = modelo;
            retorno.Descricao = modelo.Descricao;

            retorno.NomeArquivo = nomeArquivo;

            retorno.ContratoAutomatico = modelo.Contrato_Automatico;
            retorno.TemCarencia = modelo.Tem_Carencia;
            retorno.PeriodoCarencia = modelo.Periodo_Carencia;
            retorno.MultaCarencia = modelo.Multa_Carencia;
            retorno.TemReajuste = modelo.Tem_Reajuste;

            retorno.RenovarAutomaticamente = modelo.RenovarAutomaticamente;
            retorno.RenovaCarenciaAutomaticamente = modelo.Renova_Carencia_Automaticamente;
            retorno.TipoReajuste = modelo.Tipo_Reajuste;

            retorno.FrequenciaReajuste = modelo.Frequencia_Reajuste;
            retorno.CancelaContratoAutomaticoFinalCarencia = modelo.Cancela_Contrato_Automatico_Final_Carencia;

            retorno.TemProRata = modelo.Tem_Pro_Rata;
            retorno.MultaFidelidade = modelo.MultaFidelidade;

            retorno.PercentualReajusteContratual = modelo.Percentual_Reajuste_Contratual;
            retorno.ValorReajusteContratual = modelo.Valor_Reajuste_Contratual;

            retorno.UtilizaDesconto = modelo.Utiliza_Desconto;
            retorno.Desconto = modelo?.Desconto;

            retorno.ImpedirBloqueioAutomatico = modelo.ImpedirBloqueio_Automatico;
            retorno.ImpedirBloqueioSegunda = modelo.ImpedirBloqueio_Segunda;
            retorno.ImpedirBloqueioTerca = modelo.ImpedirBloqueio_Terca;
            retorno.ImpedirBloqueioQuarta = modelo.ImpedirBloqueio_Quarta;

            retorno.ImpedirBloqueioQuinta = modelo.ImpedirBloqueio_Quinta;
            retorno.ImpedirBloqueioSexta = modelo.ImpedirBloqueio_Sexta;
            retorno.ImpedirBloqueioSabado = modelo.ImpedirBloqueio_Sabado;
            retorno.ImpedirBloqueioDomingo = modelo.ImpedirBloqueio_Domingo;

            retorno.TesteGratis = modelo.TesteGratis;
            retorno.DiasTeste = modelo.DiasTeste;

            retorno.UtilizaDesbloqueioConfianca = modelo.UtilizaDesbloqueioConfianca;
            retorno.FrequenciaDesbloqueio = modelo.FrequenciaDesbloqueio;
            retorno.QtdDiasDesbloqueados = modelo.QtdDiasdesbloqueados;

            retorno.Status_Contrato = Sinapse.Model.enumerados.Status_Contrato.Aguardando_Ativacao;

            return retorno;
        }


        public virtual List<Log_ClienteContrato> GeraLog(ClienteContrato pOld, String pUsuario, String pNomeUsuario, TipoOperLog pTipoOperLog)
        {
            List<Log_ClienteContrato> logs = new List<Log_ClienteContrato>();

            if (pTipoOperLog != TipoOperLog.Alteracao)
            {
                Log_InsereTipoOper<ClienteContrato, Log_ClienteContrato>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString());
    }
            else
            {
                Log_ComparaProperty<ClienteContrato, Log_ClienteContrato, Cliente>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.CLIENTE, pOld, this, p => p.Cliente, y => $"{y.Id} - {y.NomeFantasia}");                
}

            return logs;
        }
    }
}
