using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;

namespace Aniel.Connect.DAO.map
{
    public class Script_Atendimento_QuestionMap : ClassMapBase<ScriptAtendimentoQuestion>
    {
        public Script_Atendimento_QuestionMap() : base()
        {
            Table("TB_SCRIPT_ATENDIMENTO_QUESTION");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_SCRIPT_ATEND_QUESTION");

            Map(x => x.Pergunta, "PERGUNTA").Length(16000);
            Map(x => x.Explicacao_Cliente, "EXPLICACAO_CLIENTE").Length(16000);
            Map(x => x.Explicacao_Operador, "EXPLICACAO_OPERADOR").Length(16000);
            Map(x => x.Ordem, "ORDEM");
            Map(x => x.Tipo_Resposta, "TIPO_RESPOSTA").CustomType<Script_Tipo_Resposta>();
            Map(x => x.Respostas_Possiveis, "RESPOSTAS_POSSIVEIS").Length(5000);
            Map(x => x.Observacoes, "OBSERVACOES").Length(16000);

            References(x => x.Script, "ID_SCRIPT");
        }
    }
}
