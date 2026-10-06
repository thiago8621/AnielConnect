using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;

namespace Aniel.T5.DAO.map
{
    public class QuizAnswerCapaMap : ClassMap<QuizAnswerCapa>
    {
        public QuizAnswerCapaMap() : base()
        {
            Table("TB_QUIZ_ANSWER_CAPA");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GEN_TB_QUIZ_ANSWER_CAPA_ID");

            Map(x => x.DiaInicio, "DIA_INICIO");
            Map(x => x.HoraInicio, "HORA_INICIO").CustomType("Time");
            Map(x => x.UsuarioResposta, "USUARIO_RESPOSTA").Length(10);

            References(x => x.Quiz, "ID_QUIZ");
            
            Map(x => x.Titulo, "TITULO");
            Map(x => x.Protocolo, "PROTOCOLO");
            Map(x => x.ProtocoloNomeOriginal, "PROTOCOLO_NOME_ORIGINAL");
            Map(x => x.Repeticao, "REPETICAO");
            Map(x => x.ObrigarImprodutivo, "OBRIGAR_IMPRODUTIVO").Length(1);
            Map(x => x.QuizInicioAtendimento, "QUIZ_INICIO_ATENDIMENTO").Length(1);
        }
    }
}
