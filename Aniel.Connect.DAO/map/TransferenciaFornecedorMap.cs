using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;

namespace Aniel.Connect.DAO.map
{
    public class TransferenciaFornecedorMap : ClassMapBase<TransferenciaFornecedor>
    {
        public TransferenciaFornecedorMap() : base()
        {
            Table("TB_TRANSF_FORNEC");

            Id(x => x.Id, "ID");
            Map(x => x.Alterado, "ALTERADO");
            
        }
    }
}
