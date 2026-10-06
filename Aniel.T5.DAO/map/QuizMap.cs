using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.Model.enumerados;

namespace Aniel.T5.DAO.map
{
    public class QuizMap : ClassMap<Quiz>
    {
        public QuizMap() : base()
        {
            Table("TB_QUIZ");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GEN_TB_QUIZ_ID");

            Map(x => x.Dia, "DIA");
            Map(x => x.Hora, "HORA").CustomType("Time");
            Map(x => x.Usuario, "USUARIO").Length(10);

            Map(x => x.Titulo, "TITULO").Length(200);
            
            Map(x => x.Tipo, "TIPO").CustomType<TipoChecklist>();

            #region Campos de serviço

            Map(x => x.Codct, "CODCT").Length(4);
            Map(x => x.ObrigarImprodutivo, "OBRIGAR_IMPRODUTIVO").Length(1);
            Map(x => x.PreencherInicioAtividade, "PREENCHER_NO_INICIO_ATIVIDADE").Length(1);
            Map(x => x.ChecklistNatureza, "CHECKLIST_POR_NATUREZA").Length(1);
            Map(x => x.IdNatureza, "IDNATUREZA");

            #endregion

            #region Campos do comercial

            Map(x => x.TipoPessoa, "TIPO_PESSOA").CustomType<Tipo_Pessoa?>();
            Map(x => x.IdCategoriaCliente, "ID_CATEGORIA_CLIENTE");
            Map(x => x.IdMeioCaptacao, "ID_MEIO_CAPTACAO");
            Map(x => x.IdUf, "ID_UF");
            Map(x => x.IdCidade, "ID_CIDADE");
            Map(x => x.MensagemConcluido, "MENSAGEM_CONCLUIDO").Length(500);

            #endregion

            #region Campos SISOP
            Map(x => x.TipoApiSISOP, "TIPO_API_SISOP").CustomType<TipoApiSISOP?>().Nullable();
            #endregion

            Map(x => x.Ativo, "ATIVO").Length(1);

            HasMany(x => x.Questoes).KeyColumn("ID_QUIZ").Inverse().Cascade.All();
        }
    }
}
