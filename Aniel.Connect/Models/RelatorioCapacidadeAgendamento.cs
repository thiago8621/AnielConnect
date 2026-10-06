using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class RelatorioCapacidadeAgendamentoListModel
    {
        public string Data { get; set; }
        public string Turno { get; set; }
        public string CapacidadePlena { get; set; }
        public string TempoExecucao { get; set; }
        public string CapacidadeUtil { get; set; }
        public string LimiteMultiskill { get; set; }
        public string CapacidadeComLimite { get; set; }
        public string HorasAgendadas { get; set; }
        public string HorasDeslocamento { get; set; }
        public string CapacidadeDisponivel { get; set; }

        public string PercAproveitamentoCapacidade { get; set; }
        public string CapacidadeDisponivelComLimite { get; set; }
        public string  PercAproveitamentoCapacidadeUtil { get; set; }
        public string GrupoArea { get; set; }
        public string Area { get; set; }
        public string TecnicosEscalados { get; set; }
        public int TotalServicosAgendados { get; internal set; }
        public string ServicosAgendadosNatureza { get; set; }
        public string OutrosServicosAgendados { get; set; }
    }

    public class RelatorioCapacidadeAgendamentoDataTableRequest : DataTableRequest
    {
        public string DataInicio { get; set; }
        public string DataFim { get; set; }
        public string FormatoVisualizacao { get; set; }
        public string CodEscritorio { get; set; }
        public string CodArea { get; set; }
        public string CodLocalidade { get; set; }
        public string Localidade { get; set; }
        public string CodNatureza { get; set; }

        public string CodSuperv { get; set; }
        public string Equipe { get; set; }
        public string AgruparPorPeriodo { get; set; }
        public string TipoServico { get; set; }
        public long? IdPeriodo { get; set; }

    }
}