using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sinapse.Model.enumerado;
using Sinapse.Model.enumerados;
using Aniel.Connect.Model.enumerado;

namespace Aniel.Connect.DAO.map
{
    public class Atividade_StatusVendaMap : ClassMapBase<Atividade_StatusVenda>
    {
        public Atividade_StatusVendaMap() : base()
        {
            Table("TB_ATIVSTATUS_VENDA");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_ATIVSTATUS_VENDA");
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
            Map(x => x.StatusPadrao, "STATUS_PADRAO").CustomType<Status_Venda?>();
            Map(x => x.Descricao, "DESCRICAO").Length(100);


        }


    }
}




