using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Produto : BaseModel
    {
        private IList<Config_NF_PlanoProduto> config_NF_PlanoProduto;
        private IList<Faixas_Mensalidades> faixas_Mensalidades;

        public virtual String Codigo { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Nome_Venda { get; set; }
        public virtual String Detalhes { get; set; }

        public virtual Status_PlanoProduto Status_Inicial { get; set; }
        public virtual Situacao_Produto Situacao { get; set; }

        public virtual Double? ValorProduto { get; set; }
        public virtual Double? ValorProdutoFinal { get; set; }

        public virtual Boolean AtivarValidade { get; set; }
        public virtual DateTime? Data_Validade { get; set; }

        public virtual Boolean OsPremier { get; set; }
        public virtual Boolean Tem_Pro_Rata { get; set; }
        public virtual Boolean Pro_Rata_Habilitacao { get; set; }
        public virtual Boolean Pro_Rata_Interrupcao { get; set; }
        public virtual Boolean Pro_Rata_Cancelamento { get; set; }
        public virtual Forma_Pagamento FormaPagamentoCob { get; set; }
        public virtual Boolean EmitirContratoProduto { get; set; }

        public virtual Modelo_Contrato ModeloContratoProduto { get; set; }

        public virtual Conta_Financeira Conta_Financeira { get; set; }
        public virtual Frequencia_Cobranca? FrequenciaCobrancaProduto { get; set; }
        public virtual Comprovante_Pagamento? ComprovantePagamentoProduto { get; set; }

        public virtual Cfop Cfop { get; set; }
        public virtual Formato_Cobranca? FormatoCobrancaProduto { get; set; }
        public virtual Modelo_Recibo Modelo_Recibo { get; set; }

        //public virtual Boolean Desconto_Valor_Integral { get; set; } 

        //public virtual Boolean Desconto_Pagamento_Antecipado { get; set; }
        //public virtual Int32 Qtde_Dias_Vencimento { get; set; }
        //public virtual Double? ValorDescontoAntecipadoProduto { get; set; }
        //public virtual String Percentual_Desconto_Antecipado_Produto { get; set; }

        public virtual String Percentual_Desconto { get; set; }
        public virtual Boolean DescontoProgressivoProduto { get; set; }

        public virtual String MensalidadeInicialProduto { get; set; }
        public virtual String MensalidadeFinalProduto { get; set; }

        public virtual String PercentualDescontoProgressivo { get; set; }
        public virtual Double? ValorDescontoProgressivo { get; set; }

        public virtual Atividade_Nf Atividade_Nf { get; set; }

        public virtual long? Id_Tipo_Servico { get; set; }
        public virtual long? Id_Subtipo_Servico { get; set; }

        
        public virtual Boolean Comerciavel { get; set; }
        
        public virtual Tipo_Produto? Tipo_Produto { get; set; }
        public virtual Boolean UtilizaDesconto { get; set; }
        public virtual long? Desconto { get; set; }

        public virtual Int64? Dias_Vencimento { get; set; }
        public virtual bool Plano_Comercializavel { get; set; }

        public virtual Momento_Cobranca? Momento_Cobranca { get; set; }

        public virtual Cobranca_Realizada? Cobranca_Realizada { get; set; }
        ///Venda Agrupada
        public virtual Boolean UtilizaVendaAgroup { get; set; }
        public virtual Double ValorVendaAgrupada { get; set; }

        public virtual Boolean UsaPlanoAdicional { get; set; }
        #region Dados da Instalação

        public virtual bool CobrancaInstalacao { get; set; }
        public virtual double? ValorInstalacao { get; set; }
        public virtual Conta_Financeira ContaFinanceiraInstalacao { get; set; }
        public virtual Momento_Cobranca? MomentoCobrancaInstalacao { get; set; }
        public virtual Comprovante_Pagamento? ComprovantePagamentoInstalacao { get; set; }
        public virtual bool OSTrocaEndereco { get; set; }

        #endregion

        public virtual IList<Config_NF_PlanoProduto> Config_NF_PlanoProduto
        {
            get
            {
                if (config_NF_PlanoProduto == null)
                    config_NF_PlanoProduto = new List<Config_NF_PlanoProduto>();
                return config_NF_PlanoProduto;
            }
            set
            {
                config_NF_PlanoProduto = value;
            }
        }

        public virtual IList<Faixas_Mensalidades> Faixas_Mensalidades
        {
            get
            {
                if (faixas_Mensalidades == null)
                    faixas_Mensalidades = new List<Faixas_Mensalidades>();
                return faixas_Mensalidades;
            }
            set
            {
                faixas_Mensalidades = value;
            }
        }
    }
}
