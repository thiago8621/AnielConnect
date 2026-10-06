using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;

//using Aniel.Connect.Models;
using Sinapse.Integracao.Ativacao.Interfaces;
using Sinapse.Integracao.Ativacao.Models;
using Sinapse.Integracao.Ativacao.OLTs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aniel.Connect.ControllersWebApi.Ativacao.Repositorio
{
    public class AtivacaoRepository
    {
        public async Task<List<AutorizarONUListModel>> ONUsPendentesRegistro(IntegracaoAtivacao integracao, string Usuario, string Detalhe, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null) 
        {
            return await integracao.ONUsPendentesRegistro(Usuario, Detalhe, enviarLog);
        }

        public static Response AtivarEquipamento(IntegracaoAtivacao integracao, ConfigurarONUModel onu, String scriptOLT, String scriptONU)
        {
            return integracao.AutorizarONU(onu, scriptOLT, scriptONU);
        }

        public async Task<Response> AtivarEquipamentoAsync(IntegracaoAtivacao integracao, ConfigurarONUModel onu, String scriptOlt, String scriptONU, String Detalhe, String Usuario, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null)
        {
            return await integracao.AutorizarONUAsync(onu, scriptOlt, scriptONU, Detalhe, Usuario, enviarLog);
        }

        public async Task<List<Sinapse.Integracao.Ativacao.Models.StatusONU>> GetStatusONUsByPON(IntegracaoAtivacao integracao, String numSlot, String numPon, List<String> listParams, List<(String, String)> listParamsAux = null, Func<string, string> consultarOntId = null)
        {
            return await integracao.GetStatusONUsByPON(numSlot, numPon, listParams, listParamsAux, consultarOntId);
        }

        public async Task<DiagnosticoONU> DiagnosticoONU(IntegracaoAtivacao integracao, ConfigurarONUModel onu)
        {
            return await integracao.DiagnosticoONU(onu);
        }

        public async Task<List<object>> ListaOnusConectadas(IntegracaoAtivacao integracao)
        {
            return await integracao.ListaOnusConectadas();
        }

        public async Task <Response> ReiniciarONU(IntegracaoAtivacao integracao, ConfigurarONUModel onu) 
        {
            return await integracao.ReiniciarONU(onu);
        }

        public async Task<Response> ResetarONU(IntegracaoAtivacao integracao, ConfigurarONUModel onu)
        {
            return await integracao.ResetarONU(onu);
        }

        public async Task<Response> DesautorizarONU(IntegracaoAtivacao integracao, ConfigurarONUModel onu, string Detalhe, string Usuario, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null) 
        {
            return await integracao.DesautorizarONU(onu, Detalhe, Usuario, enviarLog);
        }
        
        public async Task<InfoConfigONU> GetConfigEquipamento(IntegracaoAtivacao integracao, ConfigurarONUModel onu) 
        {
            return await integracao.GetConfigEquipamento(onu);
        }

        public async Task<InfoHardwareONU> GetInfoHardwareONU(IntegracaoAtivacao integracao, ConfigurarONUModel onu)
        {
            return await integracao.GetInfoHardware(onu);
        }

        public async Task<Response> ConfigurarONUAsync(IntegracaoAtivacao integracao, ConfigurarONUModel onu, String scriptONU, string Detalhe, string Usuario, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null)
        {
            return await integracao.ConfigurarONUAsync(onu, scriptONU, Detalhe, Usuario, enviarLog);
        }

        public async Task<GetDadosONUOLT> GetDadosONUnaOLT(IntegracaoAtivacao integracao, string Serial_Convertido, string Serial, string Usuario, string Detalhe, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null)
        {
            return await integracao.GetDadosONUnaOLT(Serial_Convertido, Serial, Usuario, Detalhe, enviarLog);
        }
    }
}