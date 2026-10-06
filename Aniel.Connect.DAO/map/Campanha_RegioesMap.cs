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
    public class Campanha_RegioesMap : ClassMapBase<Campanha_Regioes>
    {
        public Campanha_RegioesMap() : base()
        {
            Table("TB_CAMPANHA_REGIOES");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CAMPANHA_REGIOES");

            Map(x => x.Pais, "PAIS");
            Map(x => x.Estado, "UF");
            Map(x => x.Cidade, "CIDADE");
            Map(x => x.Estado_Desc, "UF_DESC");
            //Map(x => x.CampanhaRegioes, "CAMPANHA_REGIOES");
            References(x => x.Campanha, "ID_CAMPANHA").ForeignKey("FK_CAMPANHA");
            


        }
    }
}
