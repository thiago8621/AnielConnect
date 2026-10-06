using System;

namespace Aniel.Connect.Repository.Model
{
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
    }
}
