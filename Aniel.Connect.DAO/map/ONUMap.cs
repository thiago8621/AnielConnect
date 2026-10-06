using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class ONUMap : ClassMapBase<ONU>
    {
        public ONUMap() : base()
        {
            Table("TB_ONU");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_ONU");

            Map(x => x.OntID, "ONT_ID");

            Map(x => x.Descricao, "DESCRICAO").Length(100);
            Map(x => x.Mac, "MAC").Length(20);
            Map(x => x.Serial, "SERIAL").Length(100);
            Map(x => x.SerialConvertido, "SERIAL_CONVERTIDO").Length(100);

            References(x => x.Cliente_PlanoProduto, "ID_CLIENTE_PLANO_PRODUTO").ForeignKey("FK_ONU_CLIENTE_PP");

            References(x => x.PON, "ID_PON").ForeignKey("FK_ONU_PON");
            References(x => x.OLT, "ID_OLT").ForeignKey("FK_ONU_OLT");
            References(x => x.Slot, "ID_SLOT").ForeignKey("FK_ONU_SLOT");
            References(x => x.PerfilAtivacao, "ID_PERFIL_ATIVACAO").ForeignKey("FK_ONU_PERFIL_ATIVACAO");
            References(x => x.LoginRadius, "ID_LOGIN_RADIUS").ForeignKey("FK_ONU_LOGIN_RADIUS");

            Map(x => x.GemPort, "GEM_PORT");
            Map(x => x.ServicePort, "SERVICEPORT");
            Map(x => x.SenorId, "SENORID");

            Map(x => x.Login, "LOGIN").Length(50);
            Map(x => x.Senha, "SENHA").Length(50);
            Map(x => x.PortaTelnet, "PORTA_TELNET");
            Map(p => p.PortaHttp, "PORTA_HTTP");

            # region VLAN
            Map(x => x.VLAN_Uplink, "VLAN_UPLINK");
            Map(x => x.VLAN_PPPOE, "VLAN_PPPOE");
            Map(x => x.VLAN_DHCP, "VLAN_DHCP");
            Map(x => x.VLAN_IPTV, "VLAN_IPTV");
            Map(x => x.VLAN_VOIP, "VLAN_VOIP");
            Map(x => x.VLAN_TR69, "VLAN_TR69");
            Map(x => x.VLAN_Outros, "VLAN_OUTROS");
            Map(x => x.VLAN_Profile, "VLAN_PROFILE");
            #endregion

            Map(x => x.ScriptOLT, "SCRIPT_OLT");
            Map(x => x.ScriptONU, "SCRIPT_ONU");

            #region Campos Antigos

            Map(x => x.Tipo_Conexao, "TIPO_CONEXAO");
            Map(x => x.PortaVLan, "PORTAVLAN");
            Map(x => x.Wireless_SSID, "WIRELESS_SSID");
            Map(x => x.Senha_WPA2, "SENHA_WPA2").Length(50);

            Map(x => x.Estatico_Nro_Ip, "ESTATICO_NRO_IP").Length(15);
            Map(x => x.Estatico_Ip_Mask, "ESTATICO_IP_MASK").Length(15);
            Map(x => x.Estatico_Gatway, "ESTATICO_GATWAY").Length(15);

            Map(x => x.Nro_Ip, "NRO_IP").Length(15);
            Map(x => x.Ip_Inicio, "IP_INICIO").Length(15);
            Map(x => x.Ip_Final, "IP_FINAL").Length(15);
            Map(x => x.Ip_Mask, "IP_MASK").Length(15);
            Map(x => x.Registrado, "REGISTRADO");

            Map(x => x.Tipo_Identificacao, "TIPO_IDENTIFICACAO").CustomType<ONU_Tipo_Identificacao>();
            Map(x => x.StatusOnu, "STATUS_ONU").CustomType<StatusONU>();
            Map(x => x.UltimaAtualizacaoStatus, "ULTIMA_ATUALIZACAO_STATUS");

            #endregion
        }
    }
}
