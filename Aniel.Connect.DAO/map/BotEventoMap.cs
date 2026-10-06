using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.DAO.map
{
    public class BotEventoMap : ClassMapBase<BotEvento>
    {
        public BotEventoMap() : base()
        {
            Table("TB_BOT_EVENTO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_BOT_EVENTO");

            Map(x => x.Descricao, "DESCRICAO");
            Map(x => x.TextoEnviado, "TEXTO_ENVIADO");
            Map(x => x.VariaveisDisponiveis, "VARIAVEIS_DISPONIVEIS").Length(16000);
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
            Map(x => x.DisparoAutomatico, "DISPARO_AUTOMATICO");
            Map(x => x.EventoInicial, "EVENTO_INICIAL");
            Map(x => x.AcaoExecutada, "ACAO_EXECUTADA").CustomType<Tipo_Bot_Evento_Acoes>();
            References(x => x.Bot, "ID_BOT").ForeignKey("FK_EVENTO_BOT");
            HasMany(x => x.Respostas).KeyColumn("ID_BOT_EVENTO").Cascade.All().Inverse();
            HasMany(x => x.Anexos).KeyColumn("ID_BOT_EVENTO").Cascade.All().Inverse();
            Map(x => x.UtilizaAcao, "UTILIZA_ACAO");
            Map(x => x.PesquisaSatisfacao, "PESQUISA_SATISFACAO");
            Map(x => x.CondicaoEvento, "UTILIZA_CONDICAO");
            References(x => x.AutoProximoEvento, "ID_EVENTO_AUTO").ForeignKey("FK_EVENTO_AUTO_BOT");
            References(x => x.EventoFalso, "ID_EVENTO_FALSO").ForeignKey("FK_EVENTO_FALSO_BOT");
            References(x => x.EventoVerdadeiro, "ID_EVENTO_VERDAD").ForeignKey("FK_EVENTO_VERDAD_BOT");
            References(x => x.SetorAtendimento, "ID_DEPTO_ATENDIMENTO").ForeignKey("FK_DEPTO_ATENDIMENTO");
            Map(x => x.EventoArmazenamento, "EV_ARMAZENAMENTO").CustomType<EventoArmazenamento>();
        }
    }
}
