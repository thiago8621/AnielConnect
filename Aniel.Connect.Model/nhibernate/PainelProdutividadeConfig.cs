using Sinapse.Model.nhibernate;
using System;

namespace Aniel.Connect.Model.nhibernate
{
    public class PainelProdutividadeConfig : BaseModel
    {
//Configs Tecnico
        public virtual Int64? ProducaoPorcentTec { get; set; }
        public virtual Double? MetaProdutivaTec { get; set; }
        public virtual Int64? RevisitaPorcentTec { get; set; }
        public virtual Double? MetaRevisitaTec { get; set; }
        public virtual Int64? BaixasPrazoPorcentTec { get; set; }
        public virtual Double? MetaBaixasPrazoTec { get; set; }
        public virtual Int64? VistoriaPorcentTec { get; set; }
        public virtual Double? MetaVistoriaTec { get; set; }
        public virtual Int64? SLAPorcentTec { get; set; }
        public virtual Double? MetaSLATec { get; set; }
//Configs Supervisor
        public virtual Int64? ProducaoPorcentSup { get; set; }
        public virtual Double? MetaProdutivaSup { get; set; }
        public virtual Int64? RevisitaPorcentSup { get; set; }
        public virtual Double? MetaRevisitaSup { get; set; }
        public virtual Int64? BaixasPrazoPorcentSup { get; set; }
        public virtual Double? MetaBaixasPrazoSup { get; set; }
        public virtual Int64? QualidadePorcentSup { get; set; }
        public virtual Double? MetaQualidadeSup { get; set; }
        public virtual Int64? SLAPorcentSup { get; set; }
        public virtual Double? MetaSLASup { get; set; }
 //Configs Monitor
        public virtual Int64? ProducaoPorcentMon { get; set; }
        public virtual Double? MetaProdutivaMon { get; set; }
        public virtual Int64? RevisitaPorcentMon { get; set; }
        public virtual Double? MetaRevisitaMon { get; set; }
        public virtual Int64? RTIPorcentMon { get; set; }
        public virtual Double? MetaRTIMon { get; set; }
        public virtual Int64? VistoriaPorcentMon { get; set; }
        public virtual Double? MetaVistoriaMon { get; set; }
        public virtual Int64? TECPorcentMon { get; set; }
        public virtual Double? MetaTECMon { get; set; }
        public virtual Int64? SLAPorcentMon { get; set; }
        public virtual Double? MetaSLAMon { get; set; }
    }
}