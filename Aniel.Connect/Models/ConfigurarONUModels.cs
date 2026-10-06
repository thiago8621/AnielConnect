using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    [Obsolete("Usar a model de mesmo nome do Core.")]
    public class ConfigurarONUModel
    {
        public Int64 Id { get; set; }
        public Marca_Modelo_OLT? MarcaModelo { get; set; }
        public String Usuario { get; set; }
        public String Descricao { get; set; }

        public String Serial { get; set; }
        public String Mac { get; set; }

        public Int64? IdOLT { get; set; }
        public Int64? IdSlot { get; set; }
        public Int64? IdPon { get; set; }
        public Int64? IdCliente { get; set; }
        public Int64? IdPlano { get; set; }
        public Int64? IdLogin { get; set; }
        public Int64? IdPerfilAtivacao { get; set; }
        public Int64? IdCaixaFTTH { get; set; }
        public Int64? IdPortaFTTH { get; set; }
        
        public Int32? GemPort { get; set; }
        public Int32? ServicePort { get; set; }
        public Int32? SenorId { get; set; }
        
        public String IpGerencia { get; set; }
        public String Login { get; set; }
        public String Senha { get; set; }
        public Int32? PortaTelnet { get; set; }

        public Int16? VLAN_Uplink { get; set; }
        public Int16? VLAN_PPPOE { get; set; }
        public Int16? VLAN_DHCP { get; set; }
        public Int16? VLAN_IPTV { get; set; }
        public Int16? VLAN_VOIP { get; set; }
        public Int16? VLAN_TR69 { get; set; }
        public Int16? VLAN_Outros { get; set; }

        public String ScriptOLT { get; set; }
        public String ScriptONU { get; set; }

    }

    public class DiagnosticoONU
    {
        public string Rx { get; set; }
        public string Tx { get; set; }
        public string Temperatura { get; set; }
        public string Voltagem { get; set; }
    }
}