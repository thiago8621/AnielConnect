using Aniel.Connect.Model.nhibernate;
using Aniel.T5.Model.nhibernate;
using System;
using System.Linq;

namespace Aniel.Connect.Repository.TipoImovel
{
    public interface ITurnoRepository
    {
        /// <summary>
        /// busca turno que encaixa naquele horario passado
        /// </summary>
        /// <param name="dataAgendamento"></param>
        /// <param name="TempoExecucao"></param>
        Turno_ServicoAniel BuscarTurno(DateTime dataAgendamento, TimeSpan? TempoExecucao);
        IQueryable<Turno_ServicoAniel> GetAll();
        Turno_ServicoAniel SaveOrUpdate(Turno_ServicoAniel turno);
    }
}
