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
    public class Meio_CaptacaoMap : ClassMapBase<Meio_Captacao>
    {
        public Meio_CaptacaoMap() : base()
        {

            Table("TB_MEIO_CAPTACAO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_MEIO_CAPTACAO");

            Map(x => x.Descricao, "DESCRICAO").Length(100);
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();

            //Map(x => x.Desconto_Captacao, "DESCONTO_CAPTACAO").CustomType<Desconto_Captacao>();
            //Map(x => x.Tem_Desconto, "TEM_DESCONTO");
            //Map(x => x.Valor_Desconto, "VALOR_DESCONTO");
            //Map(x => x.Percentual_Desconto, "PERCENTUAL_DESCONTO");
            //Map(x => x.Repeticoes_Desconto, "REPETICOES_DESCONTO");
            //References(x => x.Plano, "ID_PLANO");
            //References(x => x.Produto, "ID_PRODUTO");

        }
    }
}
