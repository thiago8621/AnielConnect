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
    public class Pesquisa_RespostaMap : ClassMapBase<Pesquisa_Resposta>
    {
        public Pesquisa_RespostaMap() : base()
        {

            Table("TB_PESQUISA_RESPOSTA");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_PESQUISA_RESPOSTA");

            References(x => x.Cliente, "ID_CLIENTE").Not.LazyLoad().ForeignKey("FK_PESQRESP_CLIENTE");
            References(x => x.Atendimento, "ID_ATENDIMENTO").ForeignKey("FK_PESQRESP_ATEND");
            References(x => x.Pesquisa_Satisfacao, "ID_PESQUISA_SATISFACAO").Not.LazyLoad().ForeignKey("FK_PESQRESP_PESQSAT");

            Map(x => x.DataHora_Respondida, "DATAHORA_RESPONDIDA");

            Map(x => x.Categoria, "CATEGORIA").CustomType<PesquisaSatisfacaoCategoriaCaixaTexto>();
            Map(x => x.Obs, "OBS").Length(5000);

            Map(x => x.CodCt, "CODCT").Length(4);
            Map(x => x.Projeto, "PROJETO").Length(20);
            Map(x => x.Num_Obra, "NUM_OBRA").Length(50);
            Map(x => x.Num_Doc, "NUM_DOC");
        }
    }
}