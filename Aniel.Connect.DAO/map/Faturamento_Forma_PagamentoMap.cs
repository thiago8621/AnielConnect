using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Faturamento_Forma_PagamentoMap : ClassMapBase<Faturamento_Forma_Pagamento>
    {
        public Faturamento_Forma_PagamentoMap() : base()
        {
            Table("TB_FATURAMENTO_FORMA_PAGAMENTO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_FATURAMENTO_FORMA_PAGAMENTO");

            References(x => x.Faturamento_Protocolo, "ID_FATURAMENTO_PROTOCOLO").ForeignKey("FK_FAT_FORMPAGTOFATPROT");
            References(x => x.Forma_pagto, "ID_FORMA_PAGAMENTO").ForeignKey("FK_FAT_FORMPAGTOFORMPAGTO");         
        }
    }
}
