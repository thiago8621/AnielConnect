using Aniel.Connect.Model.enumerado;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;

namespace Aniel.Connect.Model.nhibernate
{
    public class ONU : BaseModel
    {
        /// <summary>
        ///     Campo utilizado nos equipamentos para identificar a ONU
        /// </summary>
        public virtual Int32? OntID  { get; set; }
        public virtual String Descricao  { get; set; }

        public virtual String Mac { get; set; }
        public virtual String Serial { get; set; }
        /// <summary>
        ///     Campo utilizado para guardar temporariamente o serial da ONU convertido quando 
        ///     a OLT utiliza conversão de Serial.
        /// </summary>
        public virtual string SerialConvertido { get; set; }
        public virtual Cliente_PlanoProduto Cliente_PlanoProduto { get; set; }

        public virtual PON PON { get; set; }
        public virtual OLT OLT { get; set; }
        public virtual Slot Slot { get; set; }
        public virtual PerfilAtivacao PerfilAtivacao { get; set; }
        public virtual LoginRadius LoginRadius { get; set; }

        public virtual Int32? GemPort { get; set; }
        public virtual Int32? ServicePort { get; set; }
        public virtual Int32? SenorId { get; set; }

        public virtual String IpGerencia { get; set; }
        public virtual String Login { get; set; }
        public virtual String Senha { get; set; }
        public virtual Int32? PortaTelnet { get; set; }
        public virtual Int32? PortaHttp { get; set; }

        public virtual Int16? VLAN_Uplink { get; set; }
        public virtual Int16? VLAN_PPPOE { get; set; }
        public virtual Int16? VLAN_DHCP { get; set; }
        public virtual Int16? VLAN_IPTV { get; set; }
        public virtual Int16? VLAN_VOIP { get; set; }
        public virtual Int16? VLAN_TR69 { get; set; }
        public virtual Int16? VLAN_Outros { get; set; }
        public virtual string VLAN_Profile { get; set; }

        public virtual byte[] ScriptOLT { get; set; }
        public virtual byte[] ScriptONU { get; set; }

        #region Campos Antigos
        public virtual Boolean Registrado { get; set; }

        public virtual Int32 PortaVLan { get; set; }

        public virtual String Wireless_SSID { get; set; }
        public virtual String Senha_WPA2 { get; set; }


        public virtual String Estatico_Nro_Ip { get; set; }
        public virtual String Estatico_Ip_Mask { get; set; }
        public virtual String Estatico_Gatway { get; set; }

        public virtual String Nro_Ip { get; set; }
        public virtual String Ip_Inicio { get; set; }
        public virtual String Ip_Final { get; set; }
        public virtual String Ip_Mask { get; set; }

        public virtual ONU_Tipo_Identificacao? Tipo_Identificacao { get; set; }
        public virtual Tipo_Conexao_ONU Tipo_Conexao { get; set; }
        public virtual StatusONU? StatusOnu { get; set; }
        public virtual DateTime? UltimaAtualizacaoStatus { get; set; }

        #endregion
    }
}
