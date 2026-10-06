using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.T5.DAO.map
{
    public class Tipo_ElementoMaterialMap :ClassMap<Tipo_ElementoMaterial>
    {
        public Tipo_ElementoMaterialMap() 
        {
            Table("TB_TIPO_ELEMENTO_MATERIAL");

            Id(x => x.Id, "ID").GeneratedBy.Sequence("GN_TIPO_ELEMENTO_MATERIAL_ID");

            Map(x => x.CodMat, "CODMAT").Length(15); ;
            References(x=>x.Tipo,"ID_TIPO").ForeignKey("FK_TIPO_ELEMENTO_MATERIAL_TIPO");
        }
    }
}
