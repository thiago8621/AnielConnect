using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class CreditoMap : ClassMapBase<Credito>
    {
        public CreditoMap() : base()
        {
            Table("TB_CREDITO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CREDITO");

            Map(x => x.Motivo_Credito, "MOTIVO_CREDITO").Length(100);
            Map(x => x.Documento, "DOCUMENTO");
            Map(x => x.Data_Fechamento, "DATA_FECHAMENTO").Length(20);
            Map(x => x.Valor, "VALOR");
            Map(x => x.Faturamento_Cliente, "ID_FATURAMENTO_CREDITO");

            Map(x => x.Status, "STATUS").CustomType<Status_Credito>();

            References(x => x.Cliente, "ID_CLIENTE").ForeignKey("FK_CREDITO_CLIENTE");
        }
    }
}
