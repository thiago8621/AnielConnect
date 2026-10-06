using Aniel.Connect.Model.nhibernate;
using FluentNHibernate.Mapping;

namespace Aniel.Connect.DAO.map
{
    public class PainelProdutividadeConfigMap: ClassMap<PainelProdutividadeConfig>
    {

        public PainelProdutividadeConfigMap() : base()
        {
            Table("TB_PAINELPROD_CONFIG");
            Id(x => x.Id, "ID").GeneratedBy.Sequence("GN_PAINELPROD_CONFIG");
            //Tecnico
            Map(x => x.ProducaoPorcentTec, "PROD_TEC_PERC");
            Map(x => x.MetaProdutivaTec, "META_PROD_TEC_VAL");
            Map(x => x.RevisitaPorcentTec, "REV_TEC_PERC");
            Map(x => x.MetaRevisitaTec, "META_REV_TEC_VAL");
            Map(x => x.BaixasPrazoPorcentTec, "BP_TEC_PERC");
            Map(x => x.MetaBaixasPrazoTec, "META_BP_TEC_VAL");
            Map(x => x.VistoriaPorcentTec, "VIS_TEC_PERC");
            Map(x => x.MetaVistoriaTec, "META_VIS_TEC_VAL");
            Map(x => x.SLAPorcentTec, "SLA_TEC_PERC");
            Map(x => x.MetaSLATec, "META_SLA_TEC_VAL");
            //Supervisor
            Map(x => x.ProducaoPorcentSup, "PROD_SUP_PERC");
            Map(x => x.MetaProdutivaSup, "META_PROD_SUP_VAL");
            Map(x => x.RevisitaPorcentSup, "REV_SUP_PERC");
            Map(x => x.MetaRevisitaSup, "META_REV_SUP_VAL");
            Map(x => x.BaixasPrazoPorcentSup, "BP_SUP_PERC");
            Map(x => x.MetaBaixasPrazoSup, "META_BP_SUP_VAL");
            Map(x => x.QualidadePorcentSup, "QLD_SUP_PERC");
            Map(x => x.MetaQualidadeSup, "META_QLD_SUP_VAL");
            Map(x => x.SLAPorcentSup, "SLA_SUP_PERC");
            Map(x => x.MetaSLASup, "META_SLA_SUP_VAL");
            //Monitor
            Map(x => x.ProducaoPorcentMon, "PROD_MON_PERC");
            Map(x => x.MetaProdutivaMon, "META_PROD_MON_VAL");
            Map(x => x.RevisitaPorcentMon, "REV_MON_PERC");
            Map(x => x.MetaRevisitaMon, "META_REV_MON_VAL");
            Map(x => x.RTIPorcentMon, "RTI_MON_PERC");
            Map(x => x.MetaRTIMon, "META_RTI_MON_VAL");
            Map(x => x.VistoriaPorcentMon, "VIS_MON_PERC");
            Map(x => x.MetaVistoriaMon, "META_VIS_MON_VAL");
            Map(x => x.TECPorcentMon, "TEC_MON_PERC");
            Map(x => x.MetaTECMon, "META_TEC_MON_VAL");
            Map(x => x.SLAPorcentMon, "SLA_MON_PERC");
            Map(x => x.MetaSLAMon, "META_SLA_MON_VAL");
        }
    }
}
