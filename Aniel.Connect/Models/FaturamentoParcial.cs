using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class FaturamentoParcial
    {
        public virtual Int64? Id_FormaPagamento { get; set; }
        public virtual Int64? Id_Cliente { get; set; }
        public virtual String Data_Inicial { get; set; }
        public virtual String Data_Vencimento { get; set; }
        public virtual String Data_Final { get; set; }
        public virtual String Valor_Avulso { get; set; }
        public virtual Boolean Fat_PlanosProdutos { get; set; }
        public virtual Int64? Id_ContaFinanceira { get; set; }
        public virtual Int64? Id_Forma_Pagamento_Cliente { get; set; }
        public virtual bool Automatico { get; set; }
        public virtual Boolean Recorrente { get; set; }
        public virtual Int64? Quantidade_Meses { get; set; }
        public virtual PeriodoRecorrencia? Periodo_Recorrencia { get; set; }
        public virtual List<Int64> IdsClientePlanoProduto { get; set; }
        public virtual string Usuario { get; set; }
    }

    /*public class FaturamentoProcessarRequestModels
    {
        public virtual Int64? Id_Faturamento_Forma { get; set; }
        public virtual List<Int64?> Ids_Faturamento_Cliente { get; set; }
        public virtual bool ProcessarBoletoInstall { get; set; }


    }*/
}