using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;

namespace Aniel.T5.DAO.map
{
    public class QuizQuestionMap : ClassMap<QuizQuestion>
    {
        public QuizQuestionMap() : base()
        {
            Table("TB_QUIZ_QUESTION");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GEN_TB_QUIZ_QUESTION_ID");

            Map(x => x.UsuarioExclusao, "USUARIO_EXCLUSAO").Length(10);
            Map(x => x.DiaExclusao, "DIA_EXCLUSAO");
            Map(x => x.HoraExclusao, "HORA_EXCLUSAO").CustomType("Time");

            References(x => x.Quiz, "ID_QUIZ");
            
            Map(x => x.Ordem, "ORDEM");
            Map(x => x.Tipo, "TIPO").CustomType<EnumQuizTipo>();
            Map(x => x.Pergunta, "PERGUNTA").Length(300);
            Map(x => x.Obrigatoria, "OBRIGATORIA").Length(1);
            Map(x => x.LiberaGaleria, "LIBERA_GALERIA").Length(1);
            Map(x => x.ExibirRvt, "EXIBIR_RVT").Length(1);
            
            Map(x => x.VinculaRestricao, "VINCULA_RESTRICAO").Length(1);
            Map(x => x.Restricao, "RESTRICAO").CustomType<ConsultasRestricaoVenda?>();

            #region Campos SISOP
            Map(x => x.DadosSISOP, "DADOS_SISOP").CustomType<DadosSISOP?>().Nullable();
            #endregion

            HasMany(x => x.RespostasPossiveis).KeyColumn("ID_QUIZ_QUESTION").Inverse().Cascade.All();
        }
    }
}
