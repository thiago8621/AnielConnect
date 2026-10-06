using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Padrao_LoginMap : ClassMapBase<Padrao_login>
    {
        public Padrao_LoginMap() : base()
        {
            Table("TB_PADRAO_LOGIN");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_PADRAO_LOGIN");
            Map(m => m.Descricao, "DESCRICAO").Length(150);



        }
    }
}
