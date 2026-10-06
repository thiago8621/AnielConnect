using Aniel.Connect.Core;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models.T5
{

    //elementos carregados na pagina index
    public class PeriodoListaModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Hora_Ini { get; set; }
        public virtual String Hora_Fim { get; set; }
        public virtual String Status { get; set; }

    }

    //elementos carregados na pagina cadastro / alteracao
    public class PeriodoCadastroModel 
    {
        
        public virtual Int32 Id { get; set; }
        public virtual String Descricao { get; set; }
        public virtual bool Status { get; set; }
        public virtual String Hora_Ini { get; set; }
        public virtual String Hora_Fim { get; set; }


    }

   

}