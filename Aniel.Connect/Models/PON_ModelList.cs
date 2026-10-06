using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class PON_ModelList
    {
        public virtual Int64 PONID { get; set; }
        public Int64? Num_PON { get; set; }
        public String Descricao { get; set; }
        public Int64? NumSlot { get; set; }
        public Int16? VLAN_Uplink { get; set; }
        public double? LimitePotenciaONUBaixa { get; set; }
    }
}