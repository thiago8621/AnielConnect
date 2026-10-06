using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Ponto_Acesso_Radio_Grupo_AreaMap : ClassMapBase<Ponto_Acesso_Radio_Grupo_Area>
    {
        public Ponto_Acesso_Radio_Grupo_AreaMap() : base()
        {
            Table("TB_PONTOACESSO_GRUPO_AREA");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_PONTOACESSO_GRUPO_AREA");

            Map(p => p.Cod_Escritorio, "COD_ESCRITORIO").Length(4);
            Map(p => p.Cod_Area, "COD_AREA").Length(4);
            Map(p => p.Desc_Escritorio, "DESC_ESCRITORIO");
            Map(p => p.Desc_Area, "DESC_AREA");

            Map(p => p.Status, "STATUS").CustomType<Status_Default>();

            References(x => x.Ponto_Acesso_Radio, "ID_PONTO_ACESSO_RADIO").NotFound.Ignore().Fetch.Join().ForeignKey("FK_PONTOACESSO_GRUPOAREA");
        }
    }
}
