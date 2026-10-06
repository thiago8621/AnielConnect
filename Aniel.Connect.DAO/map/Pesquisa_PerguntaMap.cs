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
    public class Pesquisa_PerguntaMap : ClassMapBase<Pesquisa_Pergunta>
    {
        public Pesquisa_PerguntaMap() : base()
        {

            Table("TB_PESQUISA_PERGUNTA");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_PESQUISA_PERGUNTA");


            Map(x => x.Descricao, "DESCRICAO").Length(300);

            Map(x => x.Status_Pesquisa_Pergunta, "STATUS");

            Map(x => x.Obrigatorio, "OBRIGATORIO");

            Map(x => x.Ordem, "ORDEM");

            Map(x => x.Escala_Linear_Ini, "ESCALA_LINEAR_INI");
            Map(x => x.Escala_Emoticos, "EMOTICO");

            Map(x => x.Escala_Linear_Fin, "ESCALA_LINEAR_FIN");

            References(x => x.Pesquisa_Satisfacao, "ID_PESQUISA_SATISFACAO").ForeignKey("FK_PESQPERG_PESSAT");

            Map(x => x.Respostas_Possiveis, "RESPOSTAS_POSSIVEIS").Length(5000);

            Map(x => x.Tipo_Resposta, "TIPO_RESPOSTA").CustomType<Pesquisa_Tipo_Resposta>();
        }
    }
}