using Aniel.Connect.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class Historico_Nf_DataTableRequest : DataTableRequest
    {
        public virtual Int64 Id_Cliente { get; set; }

        public virtual Int64 Id { get; set; }
        public virtual String Codigo { get; set; }
        public virtual String Cnpj { get; set; }
        public virtual String Nome_Empresa { get; set; }
        public virtual DateTime Data_Emissao { get; set; }
        public virtual Double Valor { get; set; }

        public virtual String Data_Ini { get; set; }
        public virtual String Data_Fim { get; set; }
    }

    public class Historico_NfViewModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Codigo { get; set; }
        public virtual String Cnpj { get; set; }
        public virtual String Nome_Empresa { get; set; }
        public virtual String Data_Emissao { get; set; }
        public virtual Double Valor { get; set; }

        public virtual String Data_Ini { get; set; }
        public virtual String Data_Fim { get; set; }
    }
}