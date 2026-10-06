
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;

namespace Aniel.T5.DAO.map
{
    public class Agendamento_EquipeAnielMap : ClassMapBase<Agendamento_EquipeAniel>
    {
        public Agendamento_EquipeAnielMap() : base()
        {
            Table("TB_AGENDAMENTO_EQUIPEANIEL");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_AGENDAMENTO_EQUIPEANIEL");

            References(p => p.Equipe, "EQUIPE").ForeignKey("FK_AGENDAMENTO_EQUIPE").ForeignKey("FK_AG_EQUIPE_EQUIPE");

            Map(p => p.Equipe_Tmp, "EQUIPE_TMP").Length(10);

            Map(p => p.DataHora_Inicio, "DATAHORA_INICIO").Index("IDX_AGENDAMENTO_EQUIPEANIEL_DTI");
            Map(p => p.DataHora_Fim, "DATAHORA_FIM").Index("IDX_AGENDAMENTO_EQUIPEANIEL_DTF");
            Map(p => p.Deslocamento, "DESLOCAMENTO");

            References(p => p.Tipo_Imovel, "ID_TIPO_IMOVEL").ForeignKey("FK_AGENDAMENTO_TIPO_IMOVEL");
            References(p => p.Tipo_Servico, "ID_TIPO_SERV").ForeignKey("FK_AGENDAMENTO_TIPO_SERVICO");

            Map(p => p.Cod_Escritorio, "COD_ESCRITORIO").Index("IDX_AGEND_LOCALIDADE");
            Map(p => p.Cod_Area, "COD_AREA").Index("IDX_AGEND_LOCALIDADE");
            Map(p => p.Cod_Localidade, "COD_LOCALIDADE").Index("IDX_AGEND_LOCALIDADE");

            Map(p => p.CodCt, "CODCT").Length(4).Index("IDX_AGEND_DOC_PROD");
            Map(p => p.Projeto, "PROJETO").Length(20).Index("IDX_AGEND_DOC_PROD");
            Map(p => p.Num_Obra, "NUM_OBRA").Length(50).Index("IDX_AGEND_DOC_PROD");
            Map(p => p.Num_Doc, "NUM_DOC").Index("IDX_AGEND_DOC_PROD");

            Map(x => x.Origem, "ORIGEM").CustomType<OrigemAgendamentoOS>();
            Map(x => x.Latitude, "LATITUDE");
            Map(x => x.Longitude, "LONGITUDE");
            Map(x => x.Flexivel, "FLEXIVEL");
            Map(x => x.IdJobNotificacaoAtraso, "ID_JOB_NOTIFICACAO_ATRASO");

            Map(x => x.Log, "LOG").CustomSqlType("BLOB SUB_TYPE 1 SEGMENT SIZE 16384");
        }
    }
}
