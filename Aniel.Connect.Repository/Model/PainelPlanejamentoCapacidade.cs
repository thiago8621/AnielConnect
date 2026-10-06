using Aniel.Connect.Core;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Repository.Model
{
    public class PainelPlanejamentoCapacidadeRequest : DataTableRequest
    {
        public string GrupoArea { get; set; }
        public string Area { get; set; }
        public string Natureza { get; set; }

        public string DataInicio { get; set; }
        public string DataFim { get; set; }
    }

    public class PainelPlanejamentoCapacidadeModel
    {
        public string Data { get; set; }
        public string CodGrupoArea { get; set; }
        public string CodArea { get; set; }
        public string Grupo { get; set; }
        public string Area { get; set; }
        public string Tecnicos { get; set; }
        public string Escalados { get; set; }
        public string ExclusivosNatureza { get; set; }
        public string CapacidadeExclusiva { get; set; }
        public string Multiskill { get; set; }
        public string CapacidadeMultiskill { get; set; }
        public string CapacidadePlena { get; set; }
        public string LimiteMultiskillPerc { get; set; }
        public string CapacidadeLimitadaMultiskill { get; set; }
        public string TempoMedioServico { get; set; }

        public List<PainelPlanejamentoCapacidadeAgendamento> janelas { get; set; } = new List<PainelPlanejamentoCapacidadeAgendamento>();

        public string HorasAguardandoAgendamento { get; set; }
        public string HorasAgendado { get; set; }
        public string HorasEsforcoNecessario { get; set; }
        public string HorasAtribuidosTecnico { get; set; }
        public string RestaAtribuirTecnico { get; set; }
        public string RealizavelCapacidadePlena { get; set; }
        public string RealizavelCapacidadeLimitada { get; set; }
    }

    public class PainelPlanejamentoCapacidadeAgendamento
    {
        public string Janela { get; set; }
        public string AgendamentoMaximo { get; set; }
        public string AgendamentoRealizado { get; set; }
        public string PercOcupado { get; set; }
        public string AgendamentoRestante { get; set; }
        public string AgendamentoCompartilhadoNaturezaArea { get; set; }
    }

    public class PainelPlanejamentoCapacidadeDadosEscalaEquipe
    {
        public string NumFre { get; set; }
        public string Nome { get; set; }
        public bool Multiskill { get; set; }

        public bool Escalado { get; set; }
        public double HorasTrabalhadas { get; set; }
        public long? IdPerfilAgendamento { get; set; }
    }

    /// <summary>
    ///     Classe com os horários da escala do técnico para um dia específico
    /// </summary>
    public class HorariosTrabalho 
    {
        public DateTime? Inicio { get; set; }
        public DateTime? InicioIntervalo { get; set; }
        public DateTime? TerminoIntervalo { get; set; }
        public DateTime? Termino { get; set; }
    }

    /// <summary>
    ///     Classe que contém os horários agendados para a execução do serviço
    /// </summary>
    public class HorarioExecucaoServico
    {
        public DateTime? DataHoraInicio { get; set; }
        public DateTime? DataHoraFim { get; set; }
    }

    public class GetLimitacaoRequest : DataTableRequest
    {
        public List<GetLimitacaoArea> Areas { get; set; }
        public string Natureza { get; set; }
        public string Data { get; set; }
    }

    public class GravarLimitacaoRequest : DataTableRequest
    {
        public List<GravarLimitacaoArea> Areas { get; set; }
        public string Natureza { get; set; }
        public string Data { get; set; }
    }

    public class GravarLimitacaoArea
    {
        public string grupoArea { get; set; }
        public string area { get; set; }
        public string natureza { get; set; }
        public string percentual { get; set; }
    }

    public class GetLimitacaoArea
    {
        public string grupoArea { get; set; }
        public string area { get; set; }
    }

    public class GetLimitacaoResponse
    {
        public string grupoArea { get; set; }
        public string area { get; set; }
        public string grupoAreaDescricao { get; set; }
        public string areaDescricao { get; set; }
        public string percentual { get; set; }
        public string natureza { get; set; }
        public int idNatureza { get; set; }
    }
}
