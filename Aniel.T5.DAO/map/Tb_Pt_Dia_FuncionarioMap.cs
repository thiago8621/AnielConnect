
using Aniel.T5.Model.nhibernate;
using Sinapse.DAO.map;

namespace Aniel.T5.DAO.map
{
    public class Tb_Pt_Dia_FuncionarioMap : ClassMapBase<Tb_Pt_Dia_Funcionario>
    {
        public Tb_Pt_Dia_FuncionarioMap() : base() 
        {
            Table("TB_PT_DIA_FUNCIONARIO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_PT_DIA_FUNCIONARIO");
            
            Map(x => x.Data, "DATA").Index("IDX_DIA_FUNC_DATA").Index("IDX_DIA_FUNC_DATA_FRE");
            
            Map(x => x.NumFre, "NUM_FRE").Index("IDX_DIA_FUNC_NUM_FRE").Index("IDX_DIA_FUNC_DATA_FRE");
            References(x => x.Evento, "ID_EVENTO").ForeignKey("FK_DIAFUNC_EVENTO");

            Map(x => x.Ciclos, "CICLOS");
            
            Map(x => x.Alterado, "ALTERADO");
            Map(x => x.Horas_Abonadas, "HORAS_ABONADAS");

            Map(x => x.Hora_Extra_50, "HE_50");
            Map(x => x.Hora_Extra_100, "HE_100");
            Map(x => x.Adicional_Noturno, "ADIC_NOTURNO");
            Map(x => x.Periodo_Fechado, "PERIODO_FECHADO").Index("IDX_DIA_FUNC_PERIODOFECHADO");

            Map(x => x.Empresa, "EMPRESA");
            Map(x => x.Filial, "FILIAL");
            Map(x => x.Funcao, "FUNCAO");
            Map(x => x.Departamento, "DEPARTAMENTO");
            Map(x => x.CentroCusto, "CENTRO_CUSTO");
            Map(x => x.ValorHH, "VALORHH_FUNCIONARIO");
        }
    }
}
