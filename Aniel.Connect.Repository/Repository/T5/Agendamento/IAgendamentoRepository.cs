using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.T5.Agendamento
{
    public interface IAgendamentoRepository
    {
        Task<List<AgendamentoEquipeHorarioModel>> GetForcaTrabalho(DateTime dia, Configuracoes_Gerais config);
        Task<List<AgendamentoDadosModel>> GetJaAgendadosDetalhamento(DateTime dataInicio, DateTime dataFim);
        Task<Boolean> PerfilAgendamentoAtivado(long idPerfilAgendamento);
    }
}
