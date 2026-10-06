using Aniel.T5.Model.nhibernate;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.AccessControl;
using System.Xml.Linq;

namespace Aniel.Connect.Model.nhibernate
{
    public class Faturamento_Cliente : BaseModel
    {
        IList<FaturamentoClientePlanos> _planos;

        public virtual Faturamento_Forma_Pagamento Faturamento_Forma_Pagto { get; set; }
        public virtual Forma_Pagamento_Cliente Forma_Pagamento_Cliente { get; set; } = new Forma_Pagamento_Cliente();
        public virtual Cliente Cliente { get; set; }
        public virtual DateTime? Vencimento { get; set; }
        public virtual DateTime? Data_Pagamento { get; set; }
        public virtual Boolean Alterado { get; set; }
        public virtual String Detalhes { get; set; }

        public virtual Double? Valor_Total { get; set; }
        //public virtual Double? Descontos_Total_Antecipacao { get; set; }
        public virtual Double? Descontos_Total_Plano { get; set; }
        public virtual Double? Descontos_Cliente_Valor { get; set; }
        public virtual Double? Descontos_Cliente_Perc { get; set; }
        public virtual Double? Descontos_Credito { get; set; }
        public virtual Double? Multa_Carencia { get; set; }
        public virtual Double? Multa { get; set; }
        public virtual Double? Juros { get; set; }
        public virtual Double? Valor_Negociacao { get; set; }
        public virtual Double? Valor_Telefonia { get; set; }
        public virtual Double? Valor_Final { get; set; }
        //public virtual Double? Valor_Final_Antecipado { get; set; }
        public virtual Double? Valor_Pago { get; set; }
        public virtual Double? Valor_Instalacao { get; set; }
        public virtual IList<FaturamentoClientePlanos> Planos
        {
            get
            {
                if (_planos == null)
                    _planos = new List<FaturamentoClientePlanos>();
                return _planos;
            }
            set
            {
                _planos = value;
            }
        }

        public virtual Status_Faturamento_Cliente Status { get; set; }
        public virtual Conta_Financeira Conta_Financeira { get; set; }
        public virtual String FaturaGerada { get; set; }
    }

    public enum Status_Faturamento_Cliente
    {
        [Display(Name = "CRIADO", ResourceType = typeof(Resources.RsFinanceiro))]
        Criado = 1,

        [Display(Name = "CALCULADO", ResourceType = typeof(Resources.RsFinanceiro))]
        Calculado = 2,

        [Display(Name = "PROCESSADO", ResourceType = typeof(Resources.RsFinanceiro))]
        Processado = 3,

        [Display(Name = "CANCELADO", ResourceType = typeof(Resources.RsFinanceiro))]
        Cancelado = 4,

        [Display(Name = "BOLETO_PENDENTE", ResourceType = typeof(Resources.RsFinanceiro))]
        Boleto_Pendente = 5,

        [Display(Name = "BOLETO_GERADO", ResourceType = typeof(Resources.RsFinanceiro))]
        Boleto_Gerado = 6,

        [Display(Name = "BOLETO_REGISTRADO", ResourceType = typeof(Resources.RsFinanceiro))]
        Boleto_Registrado = 7,

        [Display(Name = "BOLETO_REJEITADO", ResourceType = typeof(Resources.RsFinanceiro))]
        Boleto_Rejeitado = 8,

        [Display(Name = "PENDENTE", ResourceType = typeof(Resources.RsFinanceiro))]
        Pendente = 9
}
}
