using Aniel.Connect.Core;
using Sinapse.Model.enumerados;
using System;

namespace Aniel.Connect.Models
{
    public class PonListaModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Num_PON { get; set; }
        public virtual String Interface { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Slot { get; set; }
        public virtual String OLT { get; set; }
        public virtual String Situacao { get; set; }

        public virtual Int64 Id_Slot { get; set; }

        public virtual Int64 Id_Olt { get; set; }

        public Status_Default? Status_Filtro { get; set; }
    }

    public class PonCadastroModel
    {
        public Int64 Id { get; set; }
        public Int64? NumPON { get; set; }
        
        public String Interface { get; set; }

        public String Descricao { get; set; }
        public Int64? Id_OLT { get; set; }
        public Int64? Id_Slot { get; set; }

        public Boolean Status { get; set; }
        public Int32? PotenciaSaida { get; set; }
        public string LimitePotenciaONUBaixa { get; set; }

        public Int16? VLAN_Uplink { get; set; }
        public Int16? VLAN_PPPOE { get; set; }
        public Int16? VLAN_DHCP { get; set; }
        public Int16? VLAN_IPTV { get; set; }
        public Int16? VLAN_VOIP { get; set; }
        public Int16? VLAN_TR69 { get; set; }
        public Int16? VLAN_Outros { get; set; }
        public string VLAN_Profile { get; set; }
        public int ONU_Tipo_Identificacao { get; set; }
        public String CodigoAuxiliar { get; set; }
    }

    public class Pon_DataTableRequest : DataTableRequest
    {
        public String Id_Pon { get; set; }
        public String Id_Olt { get; set; }
        public String Id_Slot { get; set; }
        public String Status_Filtro { get; set; }
    }

    public class CriarPONSsEmMassa_Request
    {
        public virtual Int64? Id_OLT { get; set; }
        public virtual Int64? Id_Slot { get; set; }        
        public virtual Int64 Num_PON_Inicial { get; set; }
        public virtual Int64? Num_PON_Fim { get; set; }
        public Int16? Vlan_PON { get; set; }
    }

    public class GetPonByInterfaceModel
    {
        public Int64 Id { get; set; }
        public Int64? NumPON { get; set; }
        public String Interface { get; set; }
        public String Descricao { get; set; }
        public Int64? Id_OLT { get; set; }
        public Int64? Id_Slot { get; set; }
   
        public int ONU_Tipo_Identificacao { get; set; }
    }
}