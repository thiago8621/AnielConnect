using Aniel.T5.Model.enumerado;
using System;

namespace Aniel.Connect.Models.T5
{
    public class AgendamentoDadosModel
    {
        public virtual String Equipe { get; set; }
        public virtual String UsuarioCriacao { get; set; }
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



    }
}