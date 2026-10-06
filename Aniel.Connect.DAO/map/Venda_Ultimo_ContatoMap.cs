using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Venda_Ultimo_ContatoMap : ClassMapBase<Venda_Ultimo_Contato>
    {
        public Venda_Ultimo_ContatoMap() : base()
        {
            Table("TB_VENDA_HISTORICO_CONTATO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_VENDA_HISTORICO_CONTATO");

            Map(x => x.Id_Venda, "ID_VENDA").Index("IDX_VENDA_CONTATO");

            Map(x => x.Data_Ultimo_Contato, "DATA_ULTIMO_CONTATO");

            Map(x => x.Forma_Contato, "FORMA_CONTATO");

            Map(x => x.Observacao, "OBSERVACAO");

            Map(x => x.Status_Venda, "STATUS_VENDA");

            Map(x => x.Acao, "ACAO");

            Map(x => x.Valor, "VALOR");

             Map(x => x.Valor_Atualizado, "VALOR_ATUALIZADO");

        }
    }
}
