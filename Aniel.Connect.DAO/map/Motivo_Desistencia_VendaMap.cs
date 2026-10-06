using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Motivo_Desistencia_VendaMap : ClassMapBase<Motivo_Desistencia_Venda>
    {
        public Motivo_Desistencia_VendaMap() : base()
        {
            Table("TB_MOTIVO_DESISTENCIA_VENDA");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_MOTIVO_DESISTENCIA_VENDA");

            Map(x => x.Descricao, "DESCRICAO").Length(100);
            Map(x => x.Descricao_Outro, "DESCRICAO_OUTRO").Length(100);
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
            Map(x => x.Motivo, "MOTIVO").CustomType<EnumMotivo_Desistencia_Venda>();
            Map(x => x.ExibeAppOla, "EXIBEAPPOLA").CustomType<EnumExibeAppOla>();
        }


    }
}
