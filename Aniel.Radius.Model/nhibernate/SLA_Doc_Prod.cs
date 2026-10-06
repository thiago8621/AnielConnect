using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class SLA_Doc_Prod : BaseModel
    {
        public virtual Tipo_Servico_Equipe Tipo_Servico { get; set; }
        public virtual Categoria_Cliente Categoria_Cliente { get; set; }
        public virtual Criticidade_Doc_Prod Criticidade { get; set; }
        public virtual Int32 Porc_Ini { get; set; }
        public virtual Int32 Porc_Fim { get; set; }
        public virtual Int16 Cor { get; set; }
    }
}
