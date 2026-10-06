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
    public class Campanha_AreaCoberturaMap : ClassMapBase<Campanha_AreaCobertura>
    {
        public Campanha_AreaCoberturaMap() : base()
        {
            Table("TB_CAMPANHA_AREA_COBERTURA");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CAMPANHA_AREA_COBERTURA");

            Map(x => x.Area_Cobertura, "AREA_COBERTURA");
            //Map(x => x.Definir_Area_Cobertura, "DEFINIR_AREA_COBERTURA");
            References(x => x.Campanha, "ID_CAMPANHA").ForeignKey("FK_CAMPANHA");



        }
    }
}