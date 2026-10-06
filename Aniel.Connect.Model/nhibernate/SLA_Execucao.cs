using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class SLA_Execucao : Calendario
    {

        public virtual Tipo_Atendimento Tipo_Atendimento { get; set; }
        public virtual Categoria_Cliente Categoria_Cliente { get; set; }
        public virtual Int32 Minutos { get; set; }
        public virtual Boolean Calendario_Padrao { get; set; }

    }
}
