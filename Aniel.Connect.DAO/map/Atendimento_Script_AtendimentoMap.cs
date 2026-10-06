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
    public class Atendimento_Script_AtendimentoMap : ClassMapBase<Atendimento_Script_Atendimento>
    {
        public Atendimento_Script_AtendimentoMap() : base()
        {
            Table("TB_ATEND_SCRIPT_ATEND");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_ATEND_SCRIPT_ATEND");

            References(x => x.Tipo_Atendimento, "ID_TIPO_ANTENDIMENTO").ForeignKey("FK_ATENDIMENTO_SCRIPT_TIPOAT");
            References(x => x.Atendimento, "ID_ATENDIMENTO").ForeignKey("FK_ATENDIMENTO_SCRIPT_AT");
            References(x => x.Script_Atendimento, "ID_SCRIPT_ATENDIMENTO").ForeignKey("FK_ATENDIMENTO_SCRIPT_SA");
            References(x => x.ScriptAtendimentoQuestion, "ID_SCRIPT_ATENDIMENTO_QUESTION").ForeignKey("FK_ATEND_SCRIPT_QUESTION");

            Map(x => x.Pergunta, "PERGUNTA").Length(5000);
            Map(x => x.Resposta, "RESPOSTA").Length(5000);

            Map(x => x.Observacao, "OBSERVACAO").Length(5000);
        }
    }
}
