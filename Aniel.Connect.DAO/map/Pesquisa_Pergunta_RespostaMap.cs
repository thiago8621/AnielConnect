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
    public class Pesquisa_Pergunta_RespostaMap : ClassMapBase<Pesquisa_Pergunta_Resposta>
    {
        public Pesquisa_Pergunta_RespostaMap() : base()
        {

            Table("TB_PESQUISA_PERGUNTA_RESPOSTA");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_PESQUISA_PERGUNTA_RESPOSTA");

            Map(x => x.Resposta, "RESPOSTA").Length(200);

            References(x => x.Pesquisa_Pergunta, "ID_PESQUISA_PERGUNTA").ForeignKey("FK_PESQPERGRESP_PESQPERG");

            References(x => x.Pesquisa_Resposta, "ID_PESQUISA_RESPOSTA").ForeignKey("FK_PESQPERGRESP_RESP");
        }
    }
}