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
    public class TecnologiaMap : ClassMapBase<Tecnologia>
    {
        public TecnologiaMap() : base()
        {
            Table("TB_TECNOLOGIA");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_TECNOLOGIA");

            Map(x => x.Codigo, "CODIGO").Length(20);
            Map(x => x.Descricao, "DESCRICAO").Length(100);
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();

        }
    }
}
