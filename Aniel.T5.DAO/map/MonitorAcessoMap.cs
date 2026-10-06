using Aniel.Connect.Model.nhibernate;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;

namespace Aniel.T5.DAO.map
{
    public class MonitorAcessoMap : ClassMap<MonitorAcesso>
    {
        public MonitorAcessoMap() : base()
        {
            Table("TB_MONITOR_ACESSO");

            CompositeId()
            .KeyProperty(x => x.Usuario, "USUARIO")
            .KeyProperty(x => x.Hostname, "HOSTNAME")
            .KeyProperty(x => x.DataEntrada, "DATA_ENTRADA")
            .KeyProperty(x => x.HoraEntrada, "HORA_ENTRADA");


            Map(x => x.DataSaida, "DATA_SAIDA");
            Map(x => x.HoraSaida, "HORA_SAIDA").CustomType("Time"); 
            Map(x => x.Ativo, "ATIVO");
            Map(x => x.ID, "ID");
            Map(x => x.DataPing, "DATA_PING");
            Map(x => x.HoraPing, "HORA_PING").CustomType("Time");
            Map(x => x.Build, "BUILD").Length(10);
            Map(x => x.IpLocal, "IP_LOCAL").Length(30);
            Map(x => x.Recurso, "RECURSO").Length(30);

        }

    }
}
