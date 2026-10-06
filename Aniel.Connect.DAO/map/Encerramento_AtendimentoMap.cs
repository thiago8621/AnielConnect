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
   public class Encerramento_AtendimentoMap : ClassMapBase<EncerramentoAtendimento>
    {

        public Encerramento_AtendimentoMap(): base()
           {
            Table("TB_ENCERRAMENTO_ATENDIMENTO");
            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_ENCERRAMENTO_ATENDIMENTO");
            
            Map(m => m.Contrato, "CONTRATO");
            Map(m => m.Cod_Encerramento, "COD_ENCERRAMENTO");
            Map(m => m.IdFluxoEtapa, "ID_FLUXO_ETAPA");

            References(m => m.FluxoAtendimento, "ID_FLUXO_ATENDIMENTO").ForeignKey("FK_LISTA_ENCERRAMENTO");

        }
    }
}

