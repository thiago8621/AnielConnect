using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Mensagem_SistemaMap : ClassMapBase<Mensagem_Sistema>
    {
        public Mensagem_SistemaMap() : base()
        {
            Table("TB_MENSAGEM_SISTEMA");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_MENSAGEM_SISTEMA");

            Map(x => x.Titulo, "TITULO").Length(100);
            Map(x => x.Mensagem, "MENSAGEM").Length(100);

            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
        }
    }
}
