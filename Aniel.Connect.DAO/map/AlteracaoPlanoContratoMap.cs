using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;

namespace Aniel.Connect.DAO.map
{
    public class AlteracaoPlanoContratoMap: ClassMapBase<AlteracaoPlanoContrato>
    {
        public AlteracaoPlanoContratoMap(): base() 
        {
            Table("TB_ALTER_PLAN_CONTRATO");
            
            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_ALT_PLAN_CONTRATO");

            Map(u => u.IdVenda, "ID_VENDA");            
            Map(u => u.IdContratoAnterior, "ID_CONTRATO_ANTERIOR");
            Map(u => u.IdPlanoAnterior, "ID_PLANO_ANTERIOR");

            Map(u => u.Alteracao, "TIPO_ALTERACAO").CustomType<TipoAlteracaoPlanoContratoEnum>();
            Map(u => u.Status, "STATUS").CustomType<StatusAlteracaoPlanoContratoEnum>();            
            Map(u => u.IdPlano, "ID_PLANO");
        }
    }
}
