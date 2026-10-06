using Aniel.Connect.Model.enumerado;
using Aniel.Connect.ModelLog.nhibernate;
using Resources;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using Sinapse.ModelLog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aniel.Connect.Model.nhibernate
{
    public class Cliente_PlanoProduto : BaseModel
    {
        public Cliente_PlanoProduto()
        {
            Endereco_Instalacao = new Endereco_Cliente();
        }

        //private IList<Cliente_PlanoProduto> trocaPlano;

        public virtual String VelocidadeDownlaod { get; set; }
        public virtual String Caixa_Ftth_Reservado { get; set; }
        public virtual String Porta_Ftth_Reservado { get; set; }
        public virtual String VelocidadeNominal { get; set; }

        public virtual String Nas { get; set; }
        public virtual String Login_Radius { get; set; }
        public virtual String Senha_Radius { get; set; }
        public virtual DateTime? Data_Habilitado { get; set; }
        public virtual DateTime? Data_Desabilitado { get; set; }

        public virtual ClienteContrato ClienteContrato { get; set; }

        public virtual Int64? Id_Venda { get; set; }
        public virtual Campanha Campanha { get; set; }
        public virtual Cliente Cliente { get; set; }
        public virtual Plano Plano { get; set; }
        public virtual Produto Produto { get; set; }
        public virtual Combo Combo { get; set; }
        public virtual Int64? IdFormaPagamentoVenda { get; set; }
        public virtual Status_PlanoProduto? Status { get; set; }
        public virtual Motivo_MudancaPlano_Produto? Motivo_Mudanca_PP { get; set; }
        public virtual Momento_Cobra_Instalacao? Momento_Cobra_Instalacao { get; set; }

        public virtual DateTime? Data_Desbloqueio_Confianca { get; set; }
        public virtual Boolean Velocidade_Reduzida { get; set; }
        public virtual Int32? Quantidade_Produto_Variavel { get; set; }
        public virtual Double? Valor_Total_Plano_Variavel { get; set; }
        public virtual String Cod_Escritorio { get; set; }
        public virtual String Cod_Area { get; set; }
        public virtual String Localidade { get; set; }
        public virtual Localidade_Anatel? Localidade_Anatel { get; set; }
        public virtual Regime_Anatel? Regime { get; set; }
        public virtual Int32? Cod_Localidade { get; set; }
        public virtual Endereco_Cliente Endereco_Instalacao { get; set; }
        public virtual Endereco_Cliente Endereco_Mudanca_Cliente { get; set; }
        public virtual Boolean End_Instalacao_Igual_Principal { get; set; }
        public virtual Boolean End_Instalacao_Igual_Prospecto { get; set; }

        public virtual Double? Valor_Final_Total { get; set; }
        public virtual Double? Valor_Real { get; set; }
        public virtual Double? Valor_Final { get; set; }
        public virtual Double? Valor_Instalacao_Real { get; set; }
        public virtual Double? Valor_Instalacao_Final { get; set; }

        public virtual Int64? Parcelas_Instalacao { get; set; }
        public virtual Int64? Dias_Teste_Gratis { get; set; }
        public virtual Boolean Periodo_Carencia { get; set; }
        public virtual Int64? Meses_Carencia { get; set; }
        public virtual Double? Multa_Carencia_Reais { get; set; }
        public virtual Boolean CancelaContratoFimCarencia { get; set; }
        public virtual Renova_FidelidadeEnum? Renovar_CarenciaAuto { get; set; }
        public virtual Boolean Tem_Reajuste { get; set; }
        public virtual Frequencia_Reajuste? Frequencia_Reajuste { get; set; }
        public virtual String Multa_Rescisao { get; set; }

        public virtual Boolean? Atualiza_Ola { get; set; }
        public virtual Boolean CobrarInstalacaoEntrada { get; set; }
        public virtual Pool_Ip Pool_Ip { get; set; }
        public virtual NAS NasObjeto { get; set; }
        public virtual Motivo_Cancelamento_PlanoProduto Motivo_Cancelamento { get; set; }
        public virtual Motivo_Suspensao_Contratual Motivo_Suspensao { get; set; }
        public virtual Forma_Pagamento_Cliente Forma_Pagamento_Cliente { get; set; }

        public virtual Int64? Id_ElementoRede_Porta { get; set; }
        public virtual Boolean ReducaoVelocidade { get; set; }
        public virtual Int16 QtdDiasAtraso { get; set; }

        //campos adicionais 
        public virtual String VelocidadeDownloadReduzida { get; set; }
        public virtual String VelocidadeUploadReduzida { get; set; }
        public virtual String JsonTelefonia { get; set; }
        public virtual Boolean Telefonia { get; set; }
        public virtual Boolean ImpedirBloqueio_Automatico { get; set; }
        public virtual Boolean ImpedirBloqueio_Sexta { get; set; }
        public virtual Boolean ImpedirBloqueio_Sabado { get; set; }
        public virtual Boolean ImpedirBloqueio_Domingo { get; set; }
        public virtual Boolean ImpedirBloqueio_Feriados { get; set; }
        public virtual Boolean UtilizaDesbloqueioConfianca { get; set; }
        public virtual Boolean UtilizaBloqueioDebito { get; set; }
        public virtual Frequencia_Desbloqueio? FrequenciaDesbloqueio { get; set; }
        public virtual Int16 QtdDiasbloqueados { get; set; }
        public virtual Int16 QtdDiasdesbloqueados { get; set; }
        public virtual long? Id_TrocaPlano { get; set; }
        public virtual bool ContaReceberInstalacao { get; set; }
        public virtual bool BaixadoInstalacao { get; set; }
        public virtual Int64? Id_TrocaPlanoCancelados { get; set; }
        public virtual Int16? VLAN_Uplink { get; set; }
        public virtual Boolean UtilizaVendaAgroup { get; set; }
        public virtual Boolean isPlanoAdicional { get; set; }
        public virtual Boolean possuiPlanosAdicionais { get; set; }
        public virtual Int64? Id_Plano_Atrelado { get; set; }
        public virtual Int64? Parcela_Entrada { get; set; }
        public virtual Double? Valor_Entrada { get; set; }
        public virtual String CodigoIntegracao { get; set; }
        public virtual bool FlagTrocaPlano { get; set; }
        public virtual bool FlagVendaFormatoAvulso { get; set; }
        public virtual Double? ValorInstalacaoEntrada { get; set; }
        public virtual Cliente ClienteAnteriorTroca { get; set; }
        public virtual string OrdemServicoIntegracao { get; set; }
        public virtual string CodContratoIntegracao { get; set; }
        public virtual string CodigoAgrupamento { get; set; }

        public virtual List<Log_ClientePlanoProduto> GeraLog(Cliente_PlanoProduto pOld, String pUsuario, String pNomeUsuario, TipoOperLog pTipoOperLog)
        {
            List<Log_ClientePlanoProduto> logs = new List<Log_ClientePlanoProduto>();

            if (pTipoOperLog != TipoOperLog.Alteracao)
            {
                Log_InsereTipoOper<Cliente_PlanoProduto, Log_ClientePlanoProduto>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString());
    }
            else
            {
                Log_ComparaProperty<Cliente_PlanoProduto, Log_ClientePlanoProduto, Cliente>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.CLIENTE, pOld, this, p => p.Cliente, y => $"{y.Id} - {y.NomeFantasia}");                
}

            return logs;
        }
        public virtual List<LogAlteracaoPlanoContrato> GeraLogAlteracao(Cliente_PlanoProduto pOld, string pUsuario, string pNomeUsuario, TipoOperLog pTipoOperLog)
        {
            List<LogAlteracaoPlanoContrato> logs = new List<LogAlteracaoPlanoContrato>();
            if (pTipoOperLog == TipoOperLog.Insercao)
            {
                Log_InsereTipoOper<Cliente_PlanoProduto, LogAlteracaoPlanoContrato>(logs, pUsuario, pNomeUsuario, pTipoOperLog, String.Format("{0}-{1}",this.Id_Venda.ToString(), this.ClienteContrato.Id));
            }
            else if (pTipoOperLog == TipoOperLog.Exclusao)
            {

            }
            else
            {
                Log_ComparaProperty<Cliente_PlanoProduto, LogAlteracaoPlanoContrato, Plano>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.PLANO, pOld, this, p => p.Plano, y => $"{y.Id} - {y.Descricao}");
            }
            return logs;
        }
    }
}
