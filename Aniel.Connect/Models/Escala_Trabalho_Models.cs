using Aniel.Connect.Core;
using Aniel.T5.Model.enumerado;
using Sinapse.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Models
{
    public class Escala_Trabalho_FiltroModel : IViewModel<Int64>
    {
        //obrigatorio para implementar a interface IViewModel

        public virtual Int64 Id { get; set; }

        public virtual Int32 Ano { get; set; }
        public virtual Mes Mes { get; set; }
        public virtual Situacao_Escala_Trabalho? Situacao_Equipe { get; set; }
        public virtual Int64? Id_Supervisor { get; set; }
        public virtual Int64? Id_Monitor { get; set; }
        public virtual String Desc_Supervisor { get; set; }
        public virtual String Id_Equipe { get; set; } //recebido url
    }


    public class Escala_TrabalhoCadastroModel
    {
        //obrigatorio para implementar a interface IViewModel
        public virtual Int64? Id { get; set; }

        public virtual String Equipe { get; set; }
        public virtual String Nome { get; set; }
        public virtual Int16? Ano { get; set; }
        public virtual Mes? Mes { get; set; }
        public virtual Int16? Dia { get; set; }
        public virtual Int64? Id_Horario { get; set; }
        public virtual Int64? Id_PerfilAgendamento { get; set; }
        public virtual Boolean Apenas_Dia { get; set; }
        public virtual Boolean Indisponivel { get; set; }
        public virtual Boolean EscalarEmMassa { get; set; }
        public virtual Boolean PerfilEmMassa { get; set; }

        /// <summary>
        ///     Data utilizada para alterar em massa a escala de um único dia
        /// </summary>
        public virtual String Data_Selecionada { get; set; }
        public virtual String Data_Inicial { get; set; }
        public virtual DateTime Hora_Inicial { get; set; }
        public virtual DateTime Hora_Final{ get; set; }
        public virtual String Data_Final { get; set; }
        public virtual String Tecnicos { get; set; }
        public virtual HorarioAtribuido? HorarioServicoAtribuido { get; set; }
    }

    public class Escala_TrabalhoListaModel
    {
        public virtual String Id { get; set; }
        public virtual String Numfre { get; set; }
        public virtual String Equipe { get; set; }
        public virtual Boolean Feriado { get; set; }
        public virtual String IdSupervisor { get; set; }
        public virtual String IdMonitor { get; set; }
        public virtual String Nome { get; set; }
        public virtual String Situacao { get; set; }
        public virtual String HorarioServicoAtribuido { get; set; }
        public virtual string HorarioServicoAtribuidoValor { get; set; }
        public virtual Int64 Id_Dia01 { get; set; }
        public virtual String Dia01 { get; set; }

        public virtual Int64 Id_Dia02 { get; set; }
        public virtual String Dia02 { get; set; }

        public virtual Int64 Id_Dia03 { get; set; }
        public virtual String Dia03 { get; set; }

        public virtual Int64 Id_Dia04 { get; set; }
        public virtual String Dia04 { get; set; }

        public virtual Int64 Id_Dia05 { get; set; }
        public virtual String Dia05 { get; set; }

        public virtual Int64 Id_Dia06 { get; set; }
        public virtual String Dia06 { get; set; }

        public virtual Int64 Id_Dia07 { get; set; }
        public virtual String Dia07 { get; set; }

        public virtual Int64 Id_Dia08 { get; set; }
        public virtual String Dia08 { get; set; }

        public virtual Int64 Id_Dia09 { get; set; }
        public virtual String Dia09 { get; set; }

        public virtual Int64 Id_Dia10 { get; set; }
        public virtual String Dia10 { get; set; }

        public virtual Int64 Id_Dia11 { get; set; }
        public virtual String Dia11 { get; set; }

        public virtual Int64 Id_Dia12 { get; set; }
        public virtual String Dia12 { get; set; }

        public virtual Int64 Id_Dia13 { get; set; }
        public virtual String Dia13 { get; set; }

        public virtual Int64 Id_Dia14 { get; set; }
        public virtual String Dia14 { get; set; }

        public virtual Int64 Id_Dia15 { get; set; }
        public virtual String Dia15 { get; set; }

        public virtual Int64 Id_Dia16 { get; set; }
        public virtual String Dia16 { get; set; }

        public virtual Int64 Id_Dia17 { get; set; }
        public virtual String Dia17 { get; set; }

        public virtual Int64 Id_Dia18 { get; set; }
        public virtual String Dia18 { get; set; }

        public virtual Int64 Id_Dia19 { get; set; }
        public virtual String Dia19 { get; set; }

        public virtual Int64 Id_Dia20 { get; set; }
        public virtual String Dia20 { get; set; }

        public virtual Int64 Id_Dia21 { get; set; }
        public virtual String Dia21 { get; set; }

        public virtual Int64 Id_Dia22 { get; set; }
        public virtual String Dia22 { get; set; }

        public virtual Int64 Id_Dia23 { get; set; }
        public virtual String Dia23 { get; set; }

        public virtual Int64 Id_Dia24 { get; set; }
        public virtual String Dia24 { get; set; }

        public virtual Int64 Id_Dia25 { get; set; }
        public virtual String Dia25 { get; set; }

        public virtual Int64 Id_Dia26 { get; set; }
        public virtual String Dia26 { get; set; }

        public virtual Int64 Id_Dia27 { get; set; }
        public virtual String Dia27 { get; set; }

        public virtual Int64 Id_Dia28 { get; set; }
        public virtual String Dia28 { get; set; }

        public virtual Int64 Id_Dia29 { get; set; }
        public virtual String Dia29 { get; set; }

        public virtual Int64 Id_Dia30 { get; set; }
        public virtual String Dia30 { get; set; }

        public virtual Int64 Id_Dia31 { get; set; }
        public virtual String Dia31 { get; set; }

    }

    public class EscalaTrabalhoRelatorioModel
    {
        public virtual string Matricula { get; set; }
        public virtual string Tecnico { get; set; }
        public virtual string Supervisor { get; set; }
        public virtual string Indisponivel { get; set; }
        public virtual string Data { get; set; }
        public virtual string Horario { get; set; }
        public virtual string Expediente { get; set; }
        public virtual string PerfilRoteirizacao { get; set; }
        public virtual long? IdPerfilRoteirizacao { get; set; }
        public virtual string PontoPartida { get; set; }
    }
    public class Escala_Trabalho_DataTableRequest : DataTableRequest
    {
        public virtual Int32 Ano { get; set; }

        public virtual Mes Mes { get; set; }

        public virtual String IdSupervisor { get; set; }
        public virtual String IdMonitor { get; set; }
        public virtual String IdTecnico { get; set; }

        public virtual SituacaoFuncionarioAniel? Situacao_Equipe { get; set; }
        public virtual HorarioAtribuido HorarioServicoAtribuido { get; set; }

    }

    public class RelatorioEscalaTrabalhoDataTableRequest : DataTableRequest
    {
        public virtual String DataInicioPeriodo { get; set; }
        public virtual String DataFimPeriodo { get; set; }
        public virtual String IdSupervisor { get; set; }
        public virtual String IdMonitor { get; set; }
        public virtual String IdTecnico { get; set; }
        public virtual String CodEscritorio { get; set; }
        public virtual String CodArea { get; set; }
        public virtual String Situacao { get; set; }

        public virtual SituacaoFuncionarioAniel? Situacao_Equipe { get; set; }
        public virtual HorarioAtribuido HorarioServicoAtribuido { get; set; }

    }

    public enum Situacao_Escala_Trabalho
    {

        [Display(Name = "FUNCIONARIO_ATIVO", ResourceType = typeof(Resources.RsCadastros))]
        Ativo = 0,
        [Display(Name = "FUNCIONARIO_AFASTADO", ResourceType = typeof(Resources.RsCadastros))]
        Afastado = 1,
        [Display(Name = "FUNCIONARIO_FERIAS", ResourceType = typeof(Resources.RsCadastros))]
        Ferias = 3,

    }

    public class LogHistoricoEscalaTrabalho : DataTableRequest
    {
        public virtual string  Tecnico { get; set; }
        public virtual string  DataInicio { get; set; }
        public virtual string  DataFim { get; set; }
        public virtual string  DataHoraAlteracao { get; set; }
        public virtual string  HorarioAnterior { get; set; }
        public virtual string  HorarioAtual { get; set; }
        public virtual string PerfilAnterior { get; set; }
        public virtual string PerfilAtual { get; set; }
        public virtual string  UsuarioAlteracao { get; set; }
        public virtual string  Usuario { get; set; }
        public virtual string IndisponivelAtual { get; set; }
        public virtual string IndisponivelAnterior { get; set; }
    }
}


