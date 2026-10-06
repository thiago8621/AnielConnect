using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.T5.Equipe;
using Aniel.Connect.Repository.T5.TipoServico;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using Sinapse.DAO.repository;
using System.Threading.Tasks;
using System;
using System.Linq;
using Aniel.Connect.Repository.T5.DocumentoProducao;



namespace Aniel.Connect.ConnectBusiness.T5
{
    public class EquipeAnielBusiness
    {
        private readonly IEquipeAnielRepository _repoEquipe;
        private readonly ITipoServicoAnielRepository _repoTipoServico;
        private readonly IDocumentoProducaoAnielRepository _repoDocumento;

        public EquipeAnielBusiness(IEquipeAnielRepository repoEquipe,
                                   ITipoServicoAnielRepository repoTipoServico,
                                   IDocumentoProducaoAnielRepository repoDocumento)
        {
            this._repoEquipe = repoEquipe;
            this._repoTipoServico = repoTipoServico;
            this._repoDocumento = repoDocumento;
        }

        public string GetNome(string equipe)
        {
            return this._repoEquipe.GetNome(equipe);
        }

        public EquipeAniel GetByEquipe(string equipe) 
        {
            return this._repoEquipe.GetAll().Where(x => x.Matricula == equipe || x.EquipeAux == equipe).FirstOrDefault();
		}

        public EquipeStatusTipo GetStatusPersonalizado(DateTime data,
                                                       Horario_Connect horario_modelo,
                                                       bool feriado,
                                                       Horario_Equipe horarioEquipeTecnico,
                                                       EquipeStatusTipo tipo,
                                                       PerfilAgendamento perfilAgendamento,
                                                       string equipe,
                                                       TimeSpan Tempo_Execucao_Padrao)
        {

            if (horarioEquipeTecnico?.Indisponivel == true)
            {
                return EquipeStatusTipo.Indisponivel;
            }


            var horarioTecnico = horarioEquipeTecnico?.Horario;
            var horario = horarioTecnico != null ? RetornaHorarioDia(horarioTecnico, data, feriado) : RetornaHorarioDia(horario_modelo, data, feriado);

            if (horario == (null, null, null, null))
            {
                if(tipo == EquipeStatusTipo.Em_Atendimento)
                    return EquipeStatusTipo.Atendimento_Fora_Horario;
                if (tipo == EquipeStatusTipo.Disponivel || tipo == EquipeStatusTipo.Indisponivel || tipo == EquipeStatusTipo.Em_Intervalo)
                    return EquipeStatusTipo.Fora_Horario_Trabalho;

                return tipo;
            }


            DateTime dataAtual = DateTime.Now;
            var foraHorario = false;



            DateTime InicioDia = data.Date;

            if (horario_modelo != null)
            {
                if (horario.inicio.HasValue && horario.termino.HasValue)
                {
                    if (BetweenIgual(dataAtual, horario.inicio, horario.termino) == false)

                    {
                        foraHorario = true;
                    }
                }

                //renato pediu para ignorar intervalo nessa verificao
                //if (horario.inicio_intervalo.HasValue && horario.termino_intervalo.HasValue)
                //{
                //    if (HorarioApiController.BetweenIgual(dataAtual, horario.inicio_intervalo, horario.termino_intervalo))
                //    {
                //        foraHorario = true;
                //    }
                //}
            }

            if (tipo == EquipeStatusTipo.Em_Atendimento && foraHorario)
            {
                return EquipeStatusTipo.Atendimento_Fora_Horario;
            }

            if (tipo == EquipeStatusTipo.Disponivel || tipo == EquipeStatusTipo.Indisponivel || tipo == EquipeStatusTipo.Em_Intervalo)
            {
                if (foraHorario)
                    return EquipeStatusTipo.Fora_Horario_Trabalho;
                else
                    tipo = EquipeStatusTipo.Disponivel;
            }

            if (tipo == EquipeStatusTipo.Disponivel)
            {
                var proximoServicoDiaAtual = _repoDocumento.GetProximaOSAgendadaPraTecnico(equipe);
                var tempoDisponivel = DateTime.Today.Add((proximoServicoDiaAtual?.Hora_Maxima ?? horario.termino).Value.TimeOfDay) - DateTime.Now;
                TimeSpan tempoExecucao = Tempo_Execucao_Padrao;
                if (perfilAgendamento != null && perfilAgendamento.Naturezas.Count > 0)
                {
                    var tempoMinimoNaturezas = _repoTipoServico.TempoServicoMinimoNaturezas(perfilAgendamento.Naturezas.Select(p => p.Natureza.Id).ToList());
                    if (tempoMinimoNaturezas.HasValue)
                    {
                        tempoExecucao = tempoMinimoNaturezas.Value;
                    }
                }

                if (tempoDisponivel.TotalMinutes < tempoExecucao.TotalMinutes)
                {
                    return EquipeStatusTipo.Sem_Agenda;

                }

            }

            return tipo;

        }

        public (DateTime? inicio, DateTime? inicio_intervalo, DateTime? termino_intervalo, DateTime? termino) RetornaHorarioDia(Horario_Connect horario, DateTime date, bool feriado)
        {
            if (feriado)
                return (horario.Inicio_Feriado, horario.Inicio_Intervalo_Feriado, horario.Termino_Intervalo_Feriado, horario.Termino_Feriado);

            switch (date.Date.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return (horario.Inicio_Domingo, horario.Inicio_Intervalo_Domingo, horario.Termino_Intervalo_Domingo, horario.Termino_Domingo);
                case DayOfWeek.Monday:
                    return (horario.Inicio_Segunda, horario.Inicio_Intervalo_Segunda, horario.Termino_Intervalo_Segunda, horario.Termino_Segunda);
                case DayOfWeek.Tuesday:
                    return (horario.Inicio_Terca, horario.Inicio_Intervalo_Terca, horario.Termino_Intervalo_Terca, horario.Termino_Terca);
                case DayOfWeek.Wednesday:
                    return (horario.Inicio_Quarta, horario.Inicio_Intervalo_Quarta, horario.Termino_Intervalo_Quarta, horario.Termino_Quarta);
                case DayOfWeek.Thursday:
                    return (horario.Inicio_Quinta, horario.Inicio_Intervalo_Quinta, horario.Termino_Intervalo_Quinta, horario.Termino_Quinta);
                case DayOfWeek.Friday:
                    return (horario.Inicio_Sexta, horario.Inicio_Intervalo_Sexta, horario.Termino_Intervalo_Sexta, horario.Termino_Sexta);
                case DayOfWeek.Saturday:
                    return (horario.Inicio_Sabado, horario.Inicio_Intervalo_Sabado, horario.Termino_Intervalo_Sabado, horario.Termino_Sabado);
                default:
                    return (null, null, null, null);

            }
        }

        private bool Between(DateTime? input, DateTime? date1, DateTime? date2)
        {
            date1 = date1.Value.AddMinutes(1);
            date2 = date2.Value.AddMinutes(-1);
            return (input?.TimeOfDay > date1.Value.TimeOfDay && input.Value.TimeOfDay < date2.Value.TimeOfDay);
        }

        private bool BetweenIgual(DateTime? input, DateTime? date1, DateTime? date2)
        {
            return (input.Value.TimeOfDay >= date1.Value.TimeOfDay && input.Value.TimeOfDay <= date2.Value.TimeOfDay);

        }

		public bool AdicionarTecnicoAuxiliar(string equipe,Documento_ProducaoAniel doc)
        {
            return this._repoEquipe.AdicionarTecnicoAuxiliar(equipe,doc);
        }

		public bool RemoverTecnicoAuxiliar(string equipe,Documento_ProducaoAniel doc)
        {
            return this._repoEquipe.RemoverTecnicoAuxiliar(equipe, doc);
        }
    }
}
