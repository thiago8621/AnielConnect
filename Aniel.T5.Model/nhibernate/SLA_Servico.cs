using Aniel.Connect.Model.enumerado;
using Aniel.T5.Model.enumerado;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Aniel.T5.Model.nhibernate
{
    public class SLA_Servico : BaseModel
    {
        //public virtual Int64? Numero { get; set; }
        public virtual String CodCt { get; set; }
        public virtual Tipo_Servico_EquipeAniel Tipo_Servico { get; set; }
        public virtual Categoria_ClienteAniel Categoria_Cliente { get; set; }
        //public virtual Int32? Horas { get; set; }
        public virtual Int32? Minutos { get; set; }
        public virtual Tipo_SLA Tipo { get; set; }               
        public virtual Status_Default? Status { get; set; }
        public virtual Horario_Connect Horario_Trabalho { get; set; }
    }
}
