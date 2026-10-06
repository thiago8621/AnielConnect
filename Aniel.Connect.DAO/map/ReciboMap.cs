using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class ReciboMap : ClassMapBase<Recibo>
    {
        public ReciboMap() : base()
        {
            Table("TB_RECIBO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_RECIBO");

            Map(x => x.Conteudo, "CODIGO").Length(20);
            Map(x => x.Descricao, "DESCRICAO").Length(100);
        }
    }
}
