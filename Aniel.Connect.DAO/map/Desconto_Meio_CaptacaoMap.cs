using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Desconto_Meio_CaptacaoMap : ClassMapBase<Desconto_Meio_Captacao>
    {
        Desconto_Meio_CaptacaoMap() : base()
        {
            Table("TB_DESCONTO_MEIO_CAPTACAO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_DESCONTO_MEIO_CAPTACAO");

            References(x => x.Desconto, "ID_DESCONTO").ForeignKey("FK_DESCONTO_MEIO_DESCONTO");
            References(x => x.Meio_Captacao,"ID_MEIO_CAPTACAO").ForeignKey("FK_DESCONTO_MEIO_MEIO_CAPT");

        }


    }
}
