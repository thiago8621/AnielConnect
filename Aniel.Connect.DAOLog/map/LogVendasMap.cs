using Aniel.Connect.ModelLog.nhibernate;
using Sinapse.DAO.map;
using Sinapse.DAOLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAOLog.map
{
    public class LogVendasMap : LogGenericoMap<LogVendas,Int64>
    {
        public LogVendasMap() : base()
        {
            Table("TB_VENDAS");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_VENDAS");

            Map(p => p.Chave, "OBJETO").Index("IDX_VENDAS_OBJ");
            Map(p => p.Campo, "CAMPO").Index("IDX_VENDAS_CAMPO");
            Map(p => p.Log_Integracao, "LOG_INTEGRACAO").Length(5000);
            Map(p => p.StatusVenda, "STATUSVENDA");
            Map(p => p.Id_Cliente_Plano_Produto, "ID_CLIENTE_PLANO_PRODUTO");
            Map(p => p.Id_Forma_Pagamento_Cliente, "ID_FORMA_PAGTO_CLIENTE");
        }
    }

}
