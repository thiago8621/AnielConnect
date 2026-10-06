using Sinapse.DAO.map;
using Aniel.Connect.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.DAO.map
{
    public class Motivo_InviabilidadeMap : ClassMapBase<Motivo_Inviabilidade>
    {
        public Motivo_InviabilidadeMap() : base()
        {
            Table("TB_MOTIVO_INVIABILIDADE");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_MOTIVO_INVIABILIDADE");

            Map(x => x.Motivo, "MOTIVO").Length(100);
            Map(x => x.Motivo_Principal, "MOTIVO_PRINCIPAL").CustomType<Motivo_Principal_InviabilidadeEnum>();
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
        }
    }
}
