using Aniel.Connect.Model.nhibernate;
using NHibernate.Criterion;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Script_AtendimentoMap : ClassMapBase<Script_Atendimento>
    {
        public Script_AtendimentoMap() : base()
        {
            Table("TB_SCRIPT_ATENDIMENTO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_SCRIPT_ATENDIMENTO");

            Map(x => x.Nome, "NOME").Length(50);
            Map(x => x.Numero, "NUMERO");
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
            HasMany(x => x.Perguntas).KeyColumn("ID_SCRIPT").Cascade.All().Inverse();
        }
    }
}
