using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aniel.Connect.ControllersWebApi.Ativacao
{
    public interface IntegracaoAtivacao
    {
        Task<List<AutorizarONUListModel>> ONUsPendentesRegistro();
        Task<List<StatusONU>> GetStatusONUsByPON(string numSlot, string numPON, List<string> listSerial);

        Response AutorizarONU(ONU onu, string scriptOLT, string scriptONU);

        Task<Response> AutorizarONUAsync(ONU onu, string scriptOLT, string scriptONU);

        Task<Response> DesautorizarONU(ONU onu);

        Task<DiagnosticoONU> DiagnosticoONU(ONU onu);

        Task<List<object>> ListaOnusConectadas();

        Task<Response> ReiniciarONU(ONU onu, PON pon);

        Response ResetarONU(ONU onu, PON pon);    
    }
}
