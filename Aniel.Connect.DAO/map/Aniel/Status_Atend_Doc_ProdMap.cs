using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Status_Atend_Doc_ProdMap : ClassMapBase<Status_Atend_Doc_Prod>
    {
        public Status_Atend_Doc_ProdMap() : base()
        {
            Table("TB_STATUS_ATEND_DOC_PROD");

            Id(x => x.Id);

            Map(x => x.Status, "STATUS");
            Map(x => x.Descricao, "DESCRICAO").Length(60);
        }
    }
}
