using Aniel.Connect.Repository.ConfigUrl;
using Aniel.Connect.Repository.T5.Natureza;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.ConnectBusiness.T5
{
    public class PeriodoBusiness
    {
        public static Turno_ServicoAniel BuscarTurno(ISession session, DateTime dataAgendamento, TimeSpan? TempoExecucao)
        {
            using (var repository = new Conexao<Turno_ServicoAniel>(session))
            {

                var turnos = repository.select().Result;
                var dataFimAgendamento = dataAgendamento.Add(TempoExecucao.Value).TimeOfDay;
                var turno = turnos.FirstOrDefault(p => dataAgendamento.TimeOfDay >= p.Hora_Ini?.TimeOfDay && dataAgendamento.TimeOfDay <= p.Hora_Fim?.TimeOfDay);

                return turno;
            }
        }

        public static Turno_ServicoAniel BuscarTurno(List<Turno_ServicoAniel> turnos, DateTime dataAgendamento, TimeSpan? TempoExecucao)
        {
            var turno = turnos.FirstOrDefault(p => dataAgendamento.TimeOfDay >= p.Hora_Ini?.TimeOfDay && dataAgendamento.TimeOfDay <= p.Hora_Fim?.TimeOfDay);
            return turno;

        }

        public static Turno_ServicoAniel BuscarTurno(ISession session, DateTime dataInicio, DateTime dataFim)
        {
            using (var repository = new Conexao<Turno_ServicoAniel>(session))
            {

                var turnos = repository.select().Result;
                var turno = turnos.FirstOrDefault(p => dataInicio.TimeOfDay >= p.Hora_Ini?.TimeOfDay && dataFim.TimeOfDay <= p.Hora_Fim?.TimeOfDay);
                return turno;
            }
        }

        public static bool Between(DateTime? input, DateTime? date1, DateTime? date2)
        {
            return (input.Value.TimeOfDay >= date1.Value.TimeOfDay && input.Value.TimeOfDay <= date2.Value.TimeOfDay);
        }
    }
}
