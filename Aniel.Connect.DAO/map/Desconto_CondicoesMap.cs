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
    public class Desconto_CondicoesMap : ClassMapBase<Desconto_Condicoes>
    {
        public Desconto_CondicoesMap() : base()
        {
            Table("TB_DESCONTO_CONDICOES");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_DESCONTO_CONDICOES");

            Map(x => x.ValorCondicao, "VALOR_CONDICAO");
            Map(x => x.ValorCondicaoMaximo, "VALOR_CONDICAO_MAXIMO");
            Map(x => x.CondicaoCombinada, "CONDICAO_COMBINADA");
            Map(x => x.TipoCondicao, "TIPO_CONDICAO");
            
            References(x => x.Desconto, "ID_DESCONTO").ForeignKey("FK_DESCONTOCONDICOES_DESCONTO");
            


        }
    }
}
