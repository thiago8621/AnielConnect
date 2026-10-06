using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Models
{
    public class FaturamentoModels : ParamWebApi<DateTime>
    {
        public virtual Int64? Id_FormaPagamento { get; set; }
        public virtual List<Int64?> FormasPagamento { get; set; }
        public virtual Int64? Id_Cliente { get; set; }
        public virtual String Data_Inicial { get; set; }
        public virtual String Data_Vencimento { get; set; }
        public virtual String Data_Final { get; set; }
        public virtual String Valor_Avulso { get; set; }
        public virtual Boolean Fat_PlanosProdutos { get; set; }
        public virtual Int64? Id_ContaFinanceira { get; set; }
        public virtual Int64? Id_Forma_Pagamento_Cliente { get; set; }
        public virtual Boolean Recorrente { get; set; } = false;
        public virtual Int64? Quantidade_Meses { get; set; } = 0;
        public virtual List<Int64> IdsPlanoProdutoCobranca { get; set; }
        public virtual string MesCompetenciaInicio { get; set; }
        public virtual string MesCompetenciaFim { get; set; }
        public virtual PeriodoRecorrencia PeriodoRecorrencia { get; set; }
    }


    public class FaturamentoProcessarRequestModels : ParamWebApi<DateTime>
    {
        public virtual Int64? Id_Faturamento_Forma { get; set; }
        public virtual Int64? Id_Cliente { get; set; }
        public virtual List<Int64?> Ids_Faturamento_Cliente { get; set; }
        public virtual bool ProcessarBoletoInstall { get; set; }

    }
}