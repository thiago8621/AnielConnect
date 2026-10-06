using Aniel.Connect.Model.nhibernate;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Linq;

namespace Aniel.Connect.Repository.TipoImovel
{
    public class TurnoRepository : ITurnoRepository
    {
        private readonly ISession _sessionNh;

        public TurnoRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public Turno_ServicoAniel BuscarTurno(DateTime dataAgendamento, TimeSpan? TempoExecucao)
        {
            using (var repository = new Conexao<Turno_ServicoAniel>(this._sessionNh))
            {

                var turnos = repository.select().Result;
                var dataFimAgendamento = dataAgendamento.Add(TempoExecucao.Value).TimeOfDay;
                var turno = turnos.FirstOrDefault(p => dataAgendamento.TimeOfDay >= p.Hora_Ini?.TimeOfDay && dataAgendamento.TimeOfDay <= p.Hora_Fim?.TimeOfDay);

                return turno;
            }
        }

        public IQueryable<Turno_ServicoAniel> GetAll()
        {
            using (var repo = new Conexao<Turno_ServicoAniel>(this._sessionNh))
            {
                return repo.QueryableSelect();
            }
        }

        public Turno_ServicoAniel SaveOrUpdate(Turno_ServicoAniel turno)
        {
            using (var repo = new Conexao<Turno_ServicoAniel>(this._sessionNh))
            {
                repo.SaveOrUpdate(turno).Wait();
                return turno;
            }
        }
    }
}
