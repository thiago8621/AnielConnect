using Aniel.Connect.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{   
    public class RelatorioLogProvisionamento_DataTableRequest : DataTableRequest
    {
        public virtual string Num_OS { get; set; }
        public virtual string Usuario { get; set; }
        public virtual string Data_Inicio { get; set; }
        public virtual string Data_Fim { get; set; }
        public virtual string Id_OLT { get; set; }
        public virtual string Serial { get; set; }
    }

    public class RelatorioLogProvisionamento_Model
    {
        public virtual long Id { get; set; }
        public virtual string Servico { get; set; }
        public virtual string Detalhe { get; set; }
        public virtual string Usuario { get; set; }
        public virtual string Etapa { get; set; }
        public virtual string Xml_Request { get; set; }
        public virtual string Xml_Response { get; set; }
        public virtual string Data_Hora_Str { get; set; }
        public virtual DateTime? Data_Hora { get; set; }
        public virtual bool Erro_Provisionamento { get; set; }
        public virtual string Msg_Erro { get; set; }
    }

    public class RegistroAtivacao 
    {
        public virtual bool Erro { get; set; }
        public virtual string Exception { get; set; }
        
    }
}