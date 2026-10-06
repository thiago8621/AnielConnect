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
    public class Desconto_RegioesMap : ClassMapBase<Desconto_Regioes>
    {
        public Desconto_RegioesMap() : base()
        {
            Table("TB_DESCONTO_REGIOES");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_DESCONTO_REGIOES");

            Map(x => x.Pais, "PAIS");
            Map(x => x.Estado, "UF");
            Map(x => x.Cidade, "CIDADE");
            
            References(x => x.Desconto, "ID_DESCONTO").ForeignKey("FK_DESCONTOREGIOES_DESCONTO");
            


        }
    }
}
