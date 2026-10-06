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
    public class Cobranca_ServicoMap : ClassMapBase<Cobranca_Servico>
    {
        public Cobranca_ServicoMap():base()
        {
            Table("TB_COBRANCA_SERVICO");
            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_COBRANCA_SERVICO");
            Map(m => m.Descricao, "DESCRICAO").Length(150);
            Map(m => m.Escolha_Cobranca, "ESCOLHA_COBRANCA").CustomType<Escolha_Cobranca>();
           
            
            Map(m => m.Status, "STATUS").CustomType<Status_Default>();

        }
    }
}
