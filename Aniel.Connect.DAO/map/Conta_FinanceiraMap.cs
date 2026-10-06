using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Conta_FinanceiraMap : ClassMapBase<Conta_Financeira>
    {
        public Conta_FinanceiraMap() : base()
        {
            Table("TB_CONTA_FINANCEIRA");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CONTA_FINANCEIRA");

            Map(x => x.Codigo, "CODIGO").Length(20);
            Map(x => x.Descricao, "DESCRICAO").Length(100);
        }
    }
}
