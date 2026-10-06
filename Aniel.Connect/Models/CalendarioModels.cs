using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{

    public class CalendarioCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }
        
        public virtual String Atend_Domingo_Entrada { get; set; }
        
        public virtual String Atend_Domingo_Saida { get; set; }

        public virtual String Atend_Segunda_Entrada { get; set; }

        public virtual String Atend_Segunda_Saida { get; set; }

        public virtual String Atend_Terca_Entrada { get; set; }

        public virtual String Atend_Terca_Saida { get; set; }

        public virtual String Atend_Quarta_Entrada { get; set; }

        public virtual String Atend_Quarta_Saida { get; set; }

        public virtual String Atend_Quinta_Entrada { get; set; }

        public virtual String Atend_Quinta_Saida { get; set; }

        public virtual String Atend_Sexta_Entrada { get; set; }

        public virtual String Atend_Sexta_Saida { get; set; }

        public virtual String Atend_Sabado_Entrada { get; set; }

        public virtual String Atend_Sabado_Saida { get; set; }


        public virtual String Exec_Domingo_Entrada { get; set; }

        public virtual String Exec_Domingo_Saida { get; set; }

        public virtual String Exec_Segunda_Entrada { get; set; }

        public virtual String Exec_Segunda_Saida { get; set; }

        public virtual String Exec_Terca_Entrada { get; set; }

        public virtual String Exec_Terca_Saida { get; set; }

        public virtual String Exec_Quarta_Entrada { get; set; }

        public virtual String Exec_Quarta_Saida { get; set; }

        public virtual String Exec_Quinta_Entrada { get; set; }

        public virtual String Exec_Quinta_Saida { get; set; }

        public virtual String Exec_Sexta_Entrada { get; set; }

        public virtual String Exec_Sexta_Saida { get; set; }

        public virtual String Exec_Sabado_Entrada { get; set; }

        public virtual String Exec_Sabado_Saida { get; set; }

    }

    public class Calendario_DataTableRequest : DataTableRequest
    {
        
    }

}