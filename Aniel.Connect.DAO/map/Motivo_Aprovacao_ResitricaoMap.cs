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
    public class Motivo_Aprovacao_ResitricaoMap : ClassMapBase<Motivo_Aprovacao_Resticao>
    {
        public Motivo_Aprovacao_ResitricaoMap() : base()
        {
            Table("TB_MOTIVO_APROVACAO_RESTRICAO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_MOTIVO_APROVACAO_RESTRICAO");

            Map(x => x.Descricao, "DESCRICAO").Length(100);          
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
            Map(x => x.Tipo_Restricao, "TIPO_RESTRICAO").CustomType<EnumMotivo_Aprovacao_Restricao>();
            Map(x => x.ExibeAppOla, "EXIBEAPPOLA").CustomType<EnumExibeAppOla>();
        }


    }
}
