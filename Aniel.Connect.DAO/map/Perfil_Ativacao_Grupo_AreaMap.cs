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
    public class Perfil_Ativacao_Grupo_AreaMap : ClassMapBase<Perfil_Ativacao_Grupo_Area>
    {
        public Perfil_Ativacao_Grupo_AreaMap() : base()
        {
            Table("TB_PERFIL_ATIVACAO_GRUPO_AREA");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_PERFIL_ATIVACAO_GRUPO_AREA");

            Map(p => p.Cod_Escritorio, "COD_ESCRITORIO").Length(4);
            Map(p => p.Cod_Area, "COD_AREA").Length(4);
            Map(p => p.Desc_Escritorio, "DESC_ESCRITORIO");
            Map(p => p.Desc_Area, "DESC_AREA");

            Map(p => p.Status, "STATUS").CustomType<Status_Default>();

            References(x => x.Perfil_Ativacao, "ID_PERFIL_ATIVACAO").NotFound.Ignore().Fetch.Join().ForeignKey("FK_PERFIL_ATIVACAO_GRUPOAREA");
        }
    }
}
