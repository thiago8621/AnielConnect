using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Faturamento_Cliente_Plano : BaseModel
    {
        IList<Faturamento_Cliente_Plano_Det> _detalhes;

        public virtual Faturamento_Cliente Faturamento_Cliente { get; set; }
        public virtual Cliente_PlanoProduto PlanoProduto { get; set; }
        public virtual Double? Valor_Plano { get; set; }
        public virtual Double? Valor_Cliente { get; set; }
        public virtual Double? Valor_Instalacao { get; set; }
        //public virtual Double? Desconto_Antecipado_Valor { get; set; }
        //public virtual Double? Desconto_Antecipado_Perc { get; set; }
        public virtual Double? Desconto_Progressivo_Valor { get; set; }
        public virtual Double? Desconto_Progressivo_Perc { get; set; }
        public virtual Double? Multa_Carencia { get; set; }
        public virtual Boolean EntradaInstalacao { get; set; }

        public virtual IList<Faturamento_Cliente_Plano_Det> detalhes
        {
            get
            {
                if (_detalhes == null)
                    _detalhes = new List<Faturamento_Cliente_Plano_Det>();
                return _detalhes;
            }
            set
            {
                _detalhes = value;
            }
        }
    }
}
