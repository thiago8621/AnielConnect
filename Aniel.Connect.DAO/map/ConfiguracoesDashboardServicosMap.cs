using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class ConfiguracoesDashboardServicosMap : ClassMap<ConfiguracoesDashboardServicos>
    {
        public ConfiguracoesDashboardServicosMap() : base()
        {
            Table("TB_CONFIG_DASHBOARD_SERVICOS");
            Id(u => u.Id, "ID");
            Map(u => u.MetaAtendimentoJanelaInstalacao, "META_ATEND_INSTALACAO");
            Map(u => u.MetaAtendimentoJanelaMultiskill, "META_ATEND_MULTI");
            Map(u => u.MetaAtendimentoJanelaReparo, "META_ATEND_REPARO");
            Map(u => u.MetaBacklogReparo, "META_BACKLOG_REPARO");
            Map(u => u.MetaBancoInstalacao, "META_BANCO_INSTALACAO");
            Map(u => u.MetaCustoMedioInstalacao, "META_CUSTO_MEDIO_INST");
            Map(u => u.MetaCustoMedioReparo, "META_CUSTO_MEDIO_REPARO");
            Map(u => u.MetaEficienciaDiaInstalacao, "META_EFICIENCIA_DIA_INST");
            Map(u => u.MetaEficienciaDiaTotal, "META_EFICIENCIA_DIA_TOTAL");
            Map(u => u.MetaEficienciaDiaOutros, "META_EFICIENCIA_DIA_MULTI");
            Map(u => u.MetaEficienciaDiaReparo, "META_EFICIENCIA_DIA_REPARO");
            Map(u => u.MetaEficienciaMesInstalacao, "META_EFICIENCIA_MES_INST");
            Map(u => u.MetaEficienciaMesReparo, "META_EFICIENCIA_MES_REPARO");
            Map(u => u.MetaReclamacaoInicial, "META_RECLAMACAO_INICIAL");
            Map(u => u.MetaRepetidas, "META_REPETIDAS");



        }
    }
}
