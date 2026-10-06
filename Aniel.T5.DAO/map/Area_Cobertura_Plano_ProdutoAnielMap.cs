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
    public class Area_Cobertura_Plano_ProdutoAnielMap : ClassMap<Area_Cobertura_Plano_Produto>
    {
        public Area_Cobertura_Plano_ProdutoAnielMap()
        {
            Table("TB_AREA_COBERTURA_PLANO_PROD");
            Id(p => p.Id, "ID").GeneratedBy.Sequence("GN_AREA_COBERTURA_PL_PR");
            Map(p => p.Descricao, "DESCRICAO").Length(100);
            Map(p=>p.Codigo_Plano_Produto, "COD_PLANO_PRODUTO").Length(20);
            References(p=>p.Area, "ID_AREA_COBERTURA").ForeignKey("FK_AREA_COBERTURA_PL_PR_AREA");
            
             
        }
    }
}
