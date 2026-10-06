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
    public class NASMap : ClassMapBase<NAS>
    {
        public NASMap() : base()
        {
            Table("TB_NAS");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_NAS");

            Map(x => x.Codigo , "CODIGO").Length(20);
            Map(x => x.Codigo_Radius, "CODIGO_RADIUS").Length(20);
            Map(x => x.Tecnologia , "TECNOLOGIA").CustomType<Tecnologia_Nas>();
            Map(x => x.Secret , "SECRET");
            Map(x => x.Porta, "PORTA");
            Map(x => x.Ip , "IP");
            Map(x => x.Nome, "NOME");
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
        }
    }
}
