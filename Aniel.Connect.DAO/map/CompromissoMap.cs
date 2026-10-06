using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class CompromissoMap: ClassMapBase<Compromisso>
    {
        public CompromissoMap():base()
        {
            Table("TB_COMPROMISSO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_COMPROMISSO");

            Map(x => x.Assunto, "ASSUNTO");
            Map(x => x.DataInicioDefault, "DATA_INICIO_DEFAULT");
            Map(x => x.DataFimDefault, "DATA_FIM_DEFAULT");
            Map(x => x.HoraInicioDefault, "HORA_INICIO_DEFAULT");
            Map(x => x.HoraFimDefault, "HORA_FIM_DEFAULT");
            Map(x => x.TipoRecorrencia, "TIPO_RECORRENCIA");
            Map(x => x.QtdTempoEvento, "QTD_TEMPO_EVENTO");
            Map(x => x.IsConcluido, "IS_CONCLUIDO");
            Map(x => x.Id_funcionario, "ID_FUNCIONARIO");
            Map(x => x.Id_Evento, "ID_EVENTO");

            Map(x => x.Local, "LOCAL").Length(100);
            Map(x => x.Observacoes, "OBSERVACOES").Length(5000);
            Map(x => x.Deletado, "DELETADO");

            #region Mobile
            Map(x => x.Id_Tarefa, "ID_TIPO_TAREFA");
            Map(x => x.Tarefa_Descricao, "TAREFA_DESC");
            Map(x => x.Id_Venda, "ID_VENDA");
            Map(x => x.Prioridades, "PRIORIDADES").CustomType<Model.enumerado.NivelPrioridadeTarefaVendedorEnum>();
            #endregion
        }
    }
}
