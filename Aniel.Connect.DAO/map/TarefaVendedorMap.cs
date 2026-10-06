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
    public class TarefaVendedorMap : ClassMapBase<TarefaVendedor>
    {
        public TarefaVendedorMap() : base()
        {
            Table("TB_TAFERA");

            Id(x => x.Id);

            Map(x => x.Responsavel,"RESPONSAVEL");
            Map(x => x.IdVendedor,"ID_VENDEDOR");
            Map(x => x.IdVenda,"ID_VENDA");
            Map(x => x.IdTipoTarefa,"ID_TIPO_TAREFA");
            Map(x => x.Assunto,"ASSUNTO");
            Map(x => x.DataInicio,"DATA_INICIO");
            Map(x => x.DataFim,"DATA_FIM");
            Map(x => x.HorarioInicio,"HORA_INICIO");
            Map(x => x.HorarioFim,"HORA_FIM");
            Map(x => x.Local,"LOCAL");
            Map(x => x.Observacao,"OBSERVACAO");
            Map(x => x.Prioridade, "PRIORIDADE").CustomType<NivelPrioridadeTarefaVendedorEnum>();
            Map(x => x.Status, "STATUS").CustomType<StatusTarefaEnum>();
        }
    }

    public class ParticipanteTarefaMap : ClassMapBase<ParticipanteTarefa>
    {
        public ParticipanteTarefaMap() : base()
        {
            Table("TB_PARTICIPANTES_TAFERA");

            Id(x => x.Id);

            Map(x => x.IdTarefa, "ID_TAREFA");

            Map(x => x.Email,"EMAIL");
        }
    }
}
