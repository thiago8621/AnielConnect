using Aniel.Connect.Model.enumerado;
using System;
using Sinapse.Model.nhibernate;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Calendario : BaseModel
    {
        public virtual DateTime? Domingo_Entrada { get; set; }

        public virtual DateTime? Domingo_Saida { get; set; }

        public virtual DateTime? Segunda_Entrada { get; set; }

        public virtual DateTime? Segunda_Saida { get; set; }

        public virtual DateTime? Terca_Entrada { get; set; }

        public virtual DateTime? Terca_Saida { get; set; }

        public virtual DateTime? Quarta_Entrada { get; set; }

        public virtual DateTime? Quarta_Saida { get; set; }

        public virtual DateTime? Quinta_Entrada { get; set; }

        public virtual DateTime? Quinta_Saida { get; set; }

        public virtual DateTime? Sexta_Entrada { get; set; }

        public virtual DateTime? Sexta_Saida { get; set; }

        public virtual DateTime? Sabado_Entrada { get; set; }

        public virtual DateTime? Sabado_Saida { get; set; }

        public virtual Tipo_Calendario Tipo_Calendario { get; set; }
    }
}
