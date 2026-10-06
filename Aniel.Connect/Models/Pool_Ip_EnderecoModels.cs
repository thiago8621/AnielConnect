using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class Pool_Ip_EnderecoModels
    {
        public virtual Int64 Id { get; set; }
        public virtual String Ip { get; set; }
        public virtual Pool_Ip_Range_StatusEnum? Status_Ip { get; set; }
        public virtual String Desc_Status_Ip { get; set; }
        public virtual Int64? Id_Login_Radius { get; set; }
        public virtual String Usuario_Login_Radius { get; set; }
        public virtual String Ip_Concentradora { get; set; }
        public virtual String Desc_Pool_Ip { get; set; }
        public virtual String Mac { get; set; }
        public virtual String Conexao { get; set; }
    }

    public class Pool_Ip_Endereco_VincularLoginModels
    {
        public virtual Int64? Id_Ip { get; set; }
        public virtual Int64 Id_Cliente { get; set; }
        public virtual Int64 Id_ClientePlanoProduto { get; set; }
        public virtual long? Id_Pool_Ip { get; set; }
        public virtual Boolean? IsPoolIpFixo { get; set; }

    }

    public class Pool_Ip_Endereco_DataTableRequest: DataTableRequest
    {
       public virtual Int64? Id_Pool_Ip { get; set; }
       public virtual Int64? Id_Concentradora { get; set; }
       public virtual Int16? Status_Ip { get; set; }
    }    

    public class Pool_Ip_Endereco_DesvincularLoginModel
    {
        public virtual List<Int64> ids { get; set; }
    }
}