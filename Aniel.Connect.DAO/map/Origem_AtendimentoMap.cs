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
    public class Origem_AtendimentoMap : ClassMapBase<Origem_Atendimento>
    {
        public Origem_AtendimentoMap() : base()
        {
            Table("TB_ORIGEM_ATENDIMENTO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_ORIGEM_ATENDIMENTO");

            Map(x => x.Codigo, "CODIGO").Length(20);
            Map(x => x.Descricao, "DESCRICAO").Length(100);

            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
        }
    }
}
