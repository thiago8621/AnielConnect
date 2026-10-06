using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;

namespace Aniel.Connect.DAO
{
    public class PerfilAtivacaoMap : ClassMapBase<PerfilAtivacao>
    {
        public PerfilAtivacaoMap() : base()
        {
            Table("TB_PERFIL_ATIVACAO");

            Id(p => p.Id).GeneratedBy.Sequence("GN_PERFIL_ATIVACAO");

            Map(p => p.Nome, "NOME");
            Map(p => p.ScriptOLT, "SCRIPT_OLT");
            Map(p => p.ScriptOLTVoz, "SCRIPT_OLT_VOZ");

            Map(p => p.IpGerencia, "IP_GERENCIA");
            Map(p => p.Login, "LOGIN");
            Map(p => p.Senha, "SENHA");
            Map(p => p.PortaTelnet, "PORTA_TELNET");
            Map(p => p.PortaHttp, "PORTA_HTTP");

            Map(p => p.GemPort, "GEM_PORT");
            Map(p => p.ServicePort, "SERVICE_PORT");
            Map(p => p.SenorId, "SENOR_ID");

            Map(p => p.ScriptONU, "SCRIPT_ONU");
            Map(p => p.LineProfile, "LINE_PROFILE");
            Map(p => p.ObrigaWifi, "OBRIGA_WIFI");
            HasMany(x => x.GruposArea).KeyColumn("ID_PERFIL_ATIVACAO").Cascade.All().Inverse();
        }
    }
}
