using Aniel.Radius.Model.nhibernate;
using FluentNHibernate.Mapping;

namespace Aniel.Radius.DAO.map
{
    public class RadacctMap : ClassMap<Radacct>
    {
        public RadacctMap()
        {
            Table("radacct");

            Id(x => x.radacctid, "radacctid").GeneratedBy.Native();

            Map(x => x.acctsessionid, "acctsessionid");
            Map(x => x.acctuniqueid, "acctuniqueid");
            Map(x => x.username, "username");
            Map(x => x.groupname, "groupname");
            Map(x => x.realm, "realm");
            Map(x => x.nasipaddress, "nasipaddress");
            Map(x => x.nasportid, "nasportid");
            Map(x => x.nasporttype, "nasporttype");
            Map(x => x.acctstarttime, "acctstarttime");
            Map(x => x.acctupdatetime, "acctupdatetime");
            Map(x => x.acctstoptime, "acctstoptime");
            Map(x => x.acctinterval, "acctinterval");
            Map(x => x.acctsessiontime, "acctsessiontime");
            Map(x => x.acctauthentic, "acctauthentic");
            Map(x => x.connectinfo_start, "connectinfo_start");
            Map(x => x.connectinfo_stop, "connectinfo_stop");
            Map(x => x.acctinputoctets, "acctinputoctets");
            Map(x => x.acctoutputoctets, "acctoutputoctets");
            Map(x => x.calledstationid, "calledstationid");
            Map(x => x.callingstationid, "callingstationid");
            Map(x => x.acctterminatecause, "acctterminatecause");
            Map(x => x.servicetype, "servicetype");
            Map(x => x.framedprotocol, "framedprotocol");
            Map(x => x.framedipaddress, "framedipaddress");


        }
    }
}
