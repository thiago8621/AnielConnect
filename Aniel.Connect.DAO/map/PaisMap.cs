using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class PaisMap : ClassMapBase<Pais>
    {
        public PaisMap() : base()
        {
            Table("TB_PAIS");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_PAIS");

            Map(x => x.Nome, "NOME").Length(50);
        }
    }
}
