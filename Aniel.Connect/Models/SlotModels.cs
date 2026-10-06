using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{

    public class SlotListaModel
    {
        public virtual Int64 Id { get; set; }
        public virtual Int64 NumSlot { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String DATAHORACRIACAO { get; set; }
        public virtual String OLT { get; set; }
        public virtual String Status { get; set; }

        public virtual Status_Default? Status_Filtro { get; set; }
    }


    public class SlotCadastroModel
    {
        public Int64? Id { get; set; }

        public String Descricao { get; set; }
        public Boolean Status { get; set; }

        public Int64? NumSlot { get; set; }
        public string LineProfile { get; set; }

        public Int64? Id_OLT { get; set; }
    }

    public class SlotComboResult : IObjComboResult<Int64>
    {
        public Int64 Id { get; set; }
        public String Text { get; set; }       
        public String NumSlot { get; set; }
    }

    public class SlotTreeViewModel
    {
        public virtual Int64 Id { get; set; }

        public virtual String Descricao { get; set; }
    }

    public class Slot_DataTableRequest : DataTableRequest
    {
        public String Id_Slot { get; set; }
        public String Id_Olt { get; set; }
        public String Status { get; set; }
    }

    public class CriarSlotsEmMassa_Request
    {
        public virtual Int64? Id_OLT { get; set; }
        public virtual string Slot_Inicial { get; set; }
        public virtual string Num_Slot_Fim { get; set; }
        public virtual string Num_PON_Inicial { get; set; }
        public virtual string Num_PON_Fim { get; set; }
    }
}