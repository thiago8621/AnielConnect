using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.T5.DAO.map
{
    public class ProgramacaoRoteirizacaoAutomaticaMap : ClassMapBase<ProgramacaoRoteirizacaoAutomatica>
    {
        public ProgramacaoRoteirizacaoAutomaticaMap() : base()
        {
            Table("TB_PROGRAMACAO_ROTEIRIZACAO");
            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_PROGRAMACAO_ROTEIRIZACAO");
            Map(x => x.Descricao, "DESCRICAO").Length(100);
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
            Map(x => x.DataHoraInicio, "DATAHORA_INICIO");
            Map(x => x.FormatoDisparoAutomaticoRoteirizacao, "FORMATO").CustomType<FormatoDisparoAutomaticoRoteirizacao>();
            Map(x => x.Servico_SemAgendamento, "SERVICO_SEMAGENDAMENTO");
            Map(x => x.Servico_Agendados, "SERVICO_AGENDADOS");
            Map(x => x.Servico_Paralisados, "SERVICO_PARALISADOS");
            Map(x => x.Servico_Nao_Executados, "SERVICO_NAO_EXECUTADOS");
            Map(x => x.Servico_Roteirizados, "SERVICO_ROTEIRIZADOS");
            Map(x => x.NumPrimeirosDias, "NUM_PRIMEIROS_DIAS");
            Map(x => x.NumUltimosDias, "NUM_ULTIMOS_DIAS");

            Map(x => x.NoDia, "NO_DIA");
            Map(x => x.NaSemana, "NA_SEMANA");
            Map(x => x.NoMes, "NO_MES");
            Map(x => x.NosPrimeiros, "NOS_PRIMEIROS");
            Map(x => x.NosUltimos, "NOS_ULTIMOS");

            Map(x => x.DiasSemana, "DIAS_SEMANA");
            Map(x => x.DiasMes, "DIAS_MES");
            Map(x => x.Meses, "MESES");
            Map(x => x.DataExpira, "DATA_EXPIRA");
            Map(x => x.AlterarDataHoraSemAgendamentoAniel, "ALTERAR_DATAHORA_SEM_AG");
            Map(x => x.NumeroMaximoAtendimento, "NUM_MAX_ATEND");

            HasMany(x => x.CategoriasCliente).KeyColumn("ID_PROGRAMACAO").Cascade.All().Inverse();
            HasMany(x => x.Supervisores).KeyColumn("ID_PROGRAMACAO").Cascade.All().Inverse();
            HasMany(x => x.Tecnicos).KeyColumn("ID_PROGRAMACAO").Cascade.All().Inverse();
            HasMany(x => x.Naturezas).KeyColumn("ID_PROGRAMACAO").Cascade.All().Inverse();
            HasMany(x => x.Projetos).KeyColumn("ID_PROGRAMACAO").Cascade.All().Inverse();
            HasMany(x => x.Filiais).KeyColumn("ID_PROGRAMACAO").Cascade.All().Inverse();
            HasMany(x => x.Localidades).KeyColumn("ID_PROGRAMACAO").Cascade.All().Inverse();
        }
    }
}
