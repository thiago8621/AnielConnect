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
    public class Agente_Tipo_atendimentoMap : ClassMapBase<Agente_Tipo_atendimento>
    {
        public Agente_Tipo_atendimentoMap() : base()
        {
            Table("TB_AGENTE_TIPO_ATENDIMENTO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_AGENTE_TIPOATENDIMENTO");
            References(u => u.Agente, "ID_AGENTE").ForeignKey("FK_AGENTE_TIPOATENDIMENTO");
            References(u => u.Tipo_atendimento, "ID_TIPOATENDIMENTO").ForeignKey("FK_TIPOATENDIMENTO_AGENTE");



        }
    }
}
