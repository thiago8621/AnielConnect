
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
    public class Atividade_NfMap : ClassMapBase<Atividade_Nf>
    {
        public Atividade_NfMap() : base()
        {
            Table("TB_ATIVIDADE_NF");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_ATIVIDADE_NF");

            Map(x => x.Codigo, "CODIGO").Length(20);

            Map(x => x.Descricao, "DESCRICAO").Length(100);
            
            Map(x => x.Tipo_Atividade, "TIPO_ATIVIDADE").CustomType<Tipo_Atividade_Nf>();

            Map(x => x.Status, "STATUS").CustomType<Status_Default>();

        }
    }
}
