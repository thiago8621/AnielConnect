using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.DAO.map
{
    public class PONMap : ClassMapBase<PON>
    {
        public PONMap() : base()
        {
            Table("TB_PON");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_PON");

            Map(x => x.Descricao, "DESCRICAO").Length(100);
            Map(x => x.Num_PON, "NUM_PON");
            
            Map(x => x.Interface, "INTERFACE");

            References(x => x.Slot, "ID_SLOT").NotFound.Ignore().Fetch.Join().ForeignKey("FK_PON_SLOT");
            References(x => x.OLT, "ID_OLT").NotFound.Ignore().Fetch.Join().ForeignKey("FK_PON_OLT");

            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
            
            Map(x => x.Potencia_Saida, "POTENCIA_SAIDA");
            Map(x => x.LimitePotenciaONUBaixa, "LIMITE_POTENCIA_SAIDA_BAIXA");

            Map(x => x.VLAN_Uplink, "VLAN_UPLINK");
            Map(x => x.VLAN_PPPOE, "VLAN_PPPOE");
            Map(x => x.VLAN_DHCP, "VLAN_DHCP");
            Map(x => x.VLAN_IPTV, "VLAN_IPTV");
            Map(x => x.VLAN_VOIP, "VLAN_VOIP");
            Map(x => x.VLAN_TR69, "VLAN_TR69");
            Map(x => x.VLAN_Outros, "VLAN_OUTROS");
            Map(x => x.VLAN_Profile, "VLAN_PROFILE");
            Map(x => x.CodigoAuxiliar, "CODIGO_AUXILIAR");

            #region Campos Antigos

            Map(x => x.PONID, "PONID");

            Map(x => x.Id_VLAN_Voz, "ID_VLAN_VOZ");
            Map(x => x.Id_VLAN_Dados, "ID_VLAN_DADOS");
            Map(x => x.Id_VLAN_Multicast, "ID_VLAN_MULTICAST");

            #endregion
        }
    }
}
