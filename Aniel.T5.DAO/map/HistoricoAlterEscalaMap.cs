using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.DAO.map
{
    public class HistoricoAlterEscalaMap : ClassMapBase<HistoricoAlterEscala>
    {
        public HistoricoAlterEscalaMap() : base()
        {
            Table("TB_HISTORICO_ALTER_ESCALA");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_HISTORICO_ALTER_ESCALA");
            Map(x => x.Data, "DATA");
            Map(x => x.Num_Fre, "NUM_FRE");
            Map(x => x.DataInicio, "DATAINICIO");

            References(x => x.Horario_Atual, "ID_HORARIO_ATUAL").ForeignKey("FK_HISTORICO_HORARIO_ATL");
            References(x => x.Horario_Anterior, "ID_HORARIO_ANTERIOR").ForeignKey("FK_HISTORICO_HORARIO_ANT");
            References(x => x.PerfilAgendamentoAtual, "ID_PERFIL_ATUAL").ForeignKey("FK_HISTORICO_PERFIL_ATL");
            References(x => x.PerfilAgendamentoAnterior, "ID_PERFIL_ANTERIOR").ForeignKey("FK_HISTORICO_PERFIL_ANT");
            Map(x => x.IndisponivelAtual, "INDISPONIVEL_ATUAL");
            Map(x => x.IndisponivelAnterior, "INDISPONIVEL_ANTERIOR");


        }
    }
}
