using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;

namespace Aniel.T5.DAO.map
{
    public class QuizAnswerItemMap : ClassMap<QuizAnswerItem>
    {
        public QuizAnswerItemMap() : base()
        {
            Table("TB_QUIZ_ANSWER_ITEM");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GEN_TB_QUIZ_ANSWER_ITEM_ID");

            References(x => x.AnswerCapa, "ID_ANSWER_CAPA");
            References(x => x.QuizQuestion, "ID_QUESTION");
            References(x => x.QuizQuestionAnswer, "ID_QUESTION_ANSWER");

            HasMany(x => x.Alternativas).KeyColumn("ID_QUIZ_ANSWER_ITEM").Cascade.All().Inverse();
            
            Map(x => x.LiberaGaleria, "LIBERA_GALERIA").Length(1);

            Map(x => x.ExibirRvt, "EXIBIR_RVT").Length(1);
            Map(u => u.Id_Assinatura, "ID_ASSINATURA");

            Map(x => x.VinculaRestricao, "VINCULA_RESTRICAO").Length(1);
            Map(x => x.Restricao, "RESTRICAO").CustomType<ConsultasRestricaoVenda?>();

            Map(x => x.Obrigatoria, "OBRIGATORIA").Length(1);
            Map(x => x.OpcoesResposta, "OPCOES_RESPOSTA").Length(8000);
            Map(x => x.Ordem, "ORDEM");
            Map(x => x.Pergunta, "PERGUNTA").Length(300);
            Map(x => x.Respondido, "RESPONDIDO").Length(1);
            Map(x => x.Resposta, "RESPOSTA").Length(300);
            Map(x => x.RespostaData, "RESPOSTA_DATA");
            Map(x => x.RespostaFoto, "RESPOSTA_FOTO");
            Map(x => x.RespostaTexto, "RESPOSTA_TEXTO").Length(300);
            Map(x => x.RespostaValor, "RESPOSTA_VALOR");
            Map(x => x.RespostaCodBarra, "RESPOSTA_CODBAR");
            
            Map(x => x.Tipo, "TIPO").CustomType<EnumQuizTipo?>();
        }
    }
}
