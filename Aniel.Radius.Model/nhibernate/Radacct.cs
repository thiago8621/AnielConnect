using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Radius.Model.nhibernate
{
    public class Radacct
    {
        public virtual Int64 radacctid { get; set; }
        public virtual String acctsessionid { get; set; }
        public virtual String acctuniqueid { get; set; }
        public virtual String username { get; set; }
        public virtual String groupname { get; set; }
        public virtual String realm { get; set; }
        public virtual String nasipaddress { get; set; }
        public virtual String nasportid { get; set; }
        public virtual String nasporttype { get; set; }
        public virtual String acctstarttime { get; set; }
        public virtual String acctupdatetime { get; set; }
        public virtual String acctstoptime { get; set; }
        public virtual Int32 acctinterval { get; set; }
        public virtual Int32 acctsessiontime { get; set; }
        public virtual String acctauthentic { get; set; }
        public virtual String connectinfo_start { get; set; }
        public virtual String connectinfo_stop { get; set; }
        public virtual Int64 acctinputoctets { get; set; }
        public virtual Int64 acctoutputoctets { get; set; }
        public virtual String calledstationid { get; set; }
        public virtual String callingstationid { get; set; }
        public virtual String acctterminatecause { get; set; }
        public virtual String servicetype { get; set; }
        public virtual String framedprotocol { get; set; }
        public virtual String framedipaddress { get; set; }
    }
}
