using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using NHibernate.Util;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;



namespace Aniel.Connect.Repository.Model
{
    public class AgendamentoDadosModel
    {
        public virtual String Equipe { get; set; }
        public virtual String UsuarioCriacao { get; set; }
        public virtual String UsuarioAgendamento { get; set; }
        public virtual DateTime? DataHoraCriacao { get; set; }
        public virtual OrigemAgendamentoOS? Origem { get; set; }
        public virtual DateTime DataHoraInicio { get; set; }
        public virtual DateTime DataHoraFim { get; set; }
        public virtual double Deslocamento { get; set; }
        public virtual DadosServicoAgendamento DadosServico { get; set; }
        public virtual bool Temporario { get; set; }
        public virtual bool Capaz { get; set; }
        public virtual double DuracaoMinutos
        {
            get
            {
                return (this.DataHoraFim - this.DataHoraInicio).TotalMinutes;
            }
        }
        // campo apenas para controle de agrupamento de perfil
        public virtual long? IdPerfil { get; set; }
        public virtual string EquipeDefinitiva { get; set; }
        public virtual string EquipeTemporaria { get; set; }
    }

    public class TecnicoRoteirizar
    {
        public TecnicoRoteirizar()
        {

        }

        public virtual String Equipe { get; set; }
        public virtual String Nome { get; set; }

        public virtual String NumFre_Supervisor { get; set; }
        public virtual String Nome_Supervisor { get; set; }
        public virtual String Usuario_Monitor { get; set; }
        public virtual String Nome_Monitor { get; set; }
        public virtual long? IdPerfilAgendamento { get; set; }
        public virtual bool Escalado { get; set; }
        public virtual double HorasTrabalhadasHorario { get; set; }
        public virtual TipoHabilidade? TipoHabilidade { get; set; }
        public virtual Horario_Connect HorarioConnect { get; set; }
        public virtual String PontoPartida { get; set; }
        public virtual Double? Latitude { get; set; }
        public virtual Double? Longitude { get; set; }
        public virtual Double? LatitudeAtual { get; set; }
        public virtual Double? LongitudeAtual { get; set; }
        public virtual String Device_Id { get; set; }
        public virtual List<LocalidadeRoteirizacao> Localidades { get; set; } = new List<LocalidadeRoteirizacao>();
        public virtual List<Int64?> Naturezas { get; set; } = new List<long?>();

        public virtual Int64? Posicao_Fila { get; set; }
        public virtual String Cod_Fornecedor { get; set; }
        public virtual int? NivelLocalidadeEscolhido { get; set; }

        // utiliza só ao carregar o objeto do banco de dados
        public List<Painel_Servicos_AgendamentosEquipe> JaAgendados { get; set; }

        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class LocalidadeRoteirizacao
    {
        public virtual Int16? Nivel { get; set; }
        public virtual String Cod_Escritorio { get; set; }
        public virtual String Descricao_Escritorio { get; set; }
        public virtual String Cod_Area { get; set; }
        public virtual String Descricao_Area { get; set; }
        public virtual String LocalidadeStr { get; set; }
        public virtual Int32 Cod_Localidade { get; set; }
        public virtual bool Todas_Localidades { get; set; }
        public virtual bool Todas_Areas { get; set; }
    }

    public class Painel_Servicos_AgendamentosEquipe
    {
        public virtual Int64 Id { get; set; }
        public virtual String Equipe { get; set; }
        public virtual DateTime? Data_Inicial { get; set; }
        public virtual DateTime? Data_Final { get; set; }
        public virtual Double Deslocamento { get; set; }
        public virtual Double? Latitude { get; set; }
        public virtual Double? Longitude { get; set; }
        public virtual Boolean Intervalo { get; set; }
        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Numero_Obra { get; set; }
        public virtual Int32 Num_Doc { get; set; }
        public virtual String Localidade { get; set; }
        public virtual String GrupoArea { get; set; }
        public virtual String Area { get; set; }
        public virtual String CodLocalidade { get; set; }
        public virtual bool ReferenciaAgrupamento { get; set; }


    }

    public class DadosServicoAgendamento
    {
        public virtual String NumObra { get; set; }
        public virtual Int64? Cod_Natureza { get; set; }
        public virtual String Cod_Escritorio { get; set; }
        public virtual String TipoServico { get; set; }
        public virtual String Cod_Area { get; set; }
        public virtual String Periodo { get; set; }
        public virtual String DescricaoNatureza { get; set; }
        public virtual String DescricaoEscritorio { get; set; }
        public virtual String DescricaoArea { get; set; }
        public virtual String LocalidadeStr { get; set; }
        public virtual Int32? Cod_Localidade { get; set; }
        public DateTime? TempoExecucaoMinutos { get; set; }
        public virtual long? IdPeriodo { get; set; }
        public virtual String Status { get; set; }
    }

    public class Painel_ReteirizacaoDoc_Agrupado
    {
        public virtual Int64? Cod_Natureza { get; set; }
        public virtual String Cod_Escritorio { get; set; }
        public virtual String Cod_Area { get; set; }
        public virtual String LocalidadeStr { get; set; }
        public virtual Int32? Cod_Localidade { get; set; }
        public virtual DateTime? SLA { get; set; }
  
    }

    public class AgendamentoEquipeHorarioModel
    {
        public virtual String Equipe { get; set; }
        public virtual String CodSuperv { get; set; }
        public virtual String Nome { get; set; }
        public virtual Double? Latitude { get; set; }
        public virtual Double? Longitude { get; set; }
        public virtual Int64? Id_Horario_Connect { get; set; }
        public virtual Int64? IdHorarioConnectPoint { get; set; }
        public virtual Int64? IdPerfilAgendamento { get; set; }
        public virtual Double? Distancia { get; set; }
        public virtual List<LocalidadeRoteirizacao> Localidades { get; set; }
        public virtual List<Int64?> Naturezas { get; set; }
        public virtual bool Disponivel { get; set; }
        
    }

    public class AgendamentoDiaModel
    {
        private List<AgendamentoHorasDisponiveis> _horasTurnoTotal { get; set; }
        private List<AgendamentoHorasDisponiveis> _horasTurno { get; set; }
        private List<AgendamentoHorasDisponiveis> _horas { get; set; }
        private List<AgendamentoHorasDisponiveis> _horasCapacidade { get; set; }
        public List<String> _diasHoras { get; set; }

        List<AgendamentoDadosModel> _listaJaAgendadosForaFaixa;

        public List<AgendamentoDadosModel> ListaJaAgendadosForaFaixa
        {
            get
            {
                if (_listaJaAgendadosForaFaixa == null)
                    _listaJaAgendadosForaFaixa = new List<AgendamentoDadosModel>();
                return _listaJaAgendadosForaFaixa;
            }
            set
            {
                _listaJaAgendadosForaFaixa = value;
            }
        }

        public DayOfWeek DiaDaSemana { get; set; }

        public String Data { get; set; }
        public String Equipe { get; set; }


        public Boolean Feriado { get; set; }

        public DateTime? HoraInicio { get; set; }
        public DateTime? HoraTermino { get; set; }
        public Double Deslocamento { get; set; }
        public DateTime? HoraInicioIntervalo { get; set; }
        public DateTime? HoraTerminoIntervalo { get; set; }
        public int? PercentualLimiteAgendamento { get; set; }

        /// <summary>
        ///     Informa se há somente técnicos multiskill disponíveis para atender o serviço naqueal data
        /// </summary>
        public bool SomenteTecnicosMultiskillDisponiveis { get; set; } 

        /// <summary>
        ///     Informa se o percentual do limite de agendamento foi estourado naquela data, caso esteja configurado para a
        ///     área e a natureza do serviço que será agendado
        /// </summary>
        public bool LimiteAgendamentoUltrapassadoDia
        {
            get
            {
                return this.Horas.Exists(h => h.LimiteAgendamentoUltrapassado) ||
                       this.HorasCapacidade.Exists(h => h.LimiteAgendamentoUltrapassado);
            }
        }

        public List<AgendamentoHorasDisponiveis> Horas
        {
            get
            {
                if (_horas == null)
                    _horas = new List<AgendamentoHorasDisponiveis>();
                return _horas;
            }
            set
            {
                _horas = value;
            }
        }

        public List<AgendamentoHorasDisponiveis> HorasCapacidade
        {
            get
            {
                if (_horasCapacidade == null)
                    _horasCapacidade = new List<AgendamentoHorasDisponiveis>();
                return _horasCapacidade;
            }
            set
            {
                _horasCapacidade = value;
            }
        }

        public List<AgendamentoHorasDisponiveis> HorasTurno
        {
            get
            {
                if (_horasTurno == null)
                    _horasTurno = new List<AgendamentoHorasDisponiveis>();
                return _horasTurno;
            }
            set
            {
                _horasTurno = value;
            }
        }

        public List<AgendamentoHorasDisponiveis> HorasTurnoTotal
        {
            get
            {
                if (_horasTurnoTotal == null)
                    _horasTurnoTotal = new List<AgendamentoHorasDisponiveis>();
                return _horasTurnoTotal;
            }
            set
            {
                _horasTurnoTotal = value;
            }
        }
    }

    public class AgendamentoHorasDisponiveis
    {
        private List<AgendamentoDadosModel> lista_Ja_Agendados;
        private List<String> lista_Forca_Trabalho;
        List<String> lista_Forca_TrabalhoComLimite;

        public String Display { get; set; }
        public DateTime Hora_Inicio { get; set; }
        public DateTime Hora_Final { get; set; }
        public string DataHoraInicioStr { get { return this.Hora_Inicio.ToString("dd/MM/yyyy HH:mm:ss"); } }
        public string DataHoraFinalStr { get { return this.Hora_Final.ToString("dd/MM/yyyy HH:mm:ss"); } }
        public long? IdPerfil { get; set; }
        public Double Deslocamento { get; set; }

        public String Hora_InicioStr { get; set; }
        public String Hora_FinalStr { get; set; }
        public String Turno { get; set; }
        public String InicioTurno { get; set; }
        public String FimTurno { get; set; }
        [JsonIgnore]
        public Int64 Ja_Agendados { get; set; }
        [JsonIgnore]
        public Int64 Forca_Trabalho { get; set; }
        /// <summary>
        ///     Define quantidade de pessoas previstas para realizar agendamento
        ///     Propriedade criada para realizar o calculo de Capacidade seguindo a regra do Limite de Agendamento
        /// </summary>
        [JsonIgnore]
        public Int64 ForcaTrabalhoLimiteAgendamento { get; set; }
        [JsonIgnore]
        public double ForcaTrabalhoMinutosPrevistos { get; set; }
        [JsonIgnore]
        public double JaAgendadosMinutosConsumidos { get; set; }

        public Int64 Aptos { get; set; }

        public String Equipe_Tmp { get; set; }
        public bool AgendamentoAntigo { get; set; }

        /// <summary>
        ///     Define os minutos previstos que a forca de trabalho ira usar  pra realizar o agendamento
        ///     Propriedade criada para realizar o calculo de Capacidade seguindo a regra do Limite de Agendamento
        /// </summary>
        [JsonIgnore]
        public virtual double ForcaTrabalhoMinutosPrevistosComLimite { get; set; }

        /// <summary>
        ///     Define um tecnico temporario que pode ser usado seguindo a regra do limite de agendamento
        ///     caso a propriedade LimiteAgendamentoUltrapassado, esse campo deve ser usado no lugar do EquipeTmp
        /// </summary>
        public virtual String EquipeTmpComLimite { get; set; }

        /// <summary>
        ///     Define o numero de tecnicos aptos apos aplicada regra de limite de agendamento
        /// </summary>
        
        public virtual Int64 AptosPorLimite { get; set; }

        /// <summary>
        ///     Define se o percentual do limite de agendamento foi ultrapassado, 
        ///     caso esteja configurado para a natureza na área do serviço que
        ///     será agendado
        /// </summary>
        public bool LimiteAgendamentoUltrapassado { get; set; }

        public long MinutosAgendamentoDentroFaixa { get; set; }
        public long MinutosAgendamentoForaFaixa { get; set; }
        public long MinutosCapacidadeRestante { get; set; }
        [JsonIgnore]
        public List<AgendamentoDadosModel> Lista_Ja_Agendados
        {
            get
            {
                if (lista_Ja_Agendados == null)
                    lista_Ja_Agendados = new List<AgendamentoDadosModel>();
                return lista_Ja_Agendados;
            }
            set
            {
                lista_Ja_Agendados = value;
            }
        }
        [JsonIgnore]
        public List<String> Lista_Forca_Trabalho
        {
            get
            {
                if (lista_Forca_Trabalho == null)
                    lista_Forca_Trabalho = new List<String>();
                return lista_Forca_Trabalho;
            }
            set
            {
                lista_Forca_Trabalho = value;
            }
        }
        [JsonIgnore]
        public virtual List<String> ListaForcaTrabalhoComLimite
        {
            get
            {
                if (lista_Forca_TrabalhoComLimite == null)
                    lista_Forca_TrabalhoComLimite = new List<String>();
                return lista_Forca_TrabalhoComLimite;
            }
            set
            {
                lista_Forca_TrabalhoComLimite = value;
            }
        }
    }

}
