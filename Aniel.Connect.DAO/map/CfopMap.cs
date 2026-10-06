using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class CfopMap : ClassMapBase<Cfop>
    {
        public CfopMap() : base()
        {
            Table("TB_CFOP");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CFOP");

            Map(x => x.Codigo, "CODIGO").Length(20);
            Map(x => x.Descricao, "DESCRICAO").Length(1000);
        }
    }
}
