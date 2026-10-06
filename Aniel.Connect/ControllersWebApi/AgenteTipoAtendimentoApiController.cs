using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class AgenteTipoAtendimentoApiController : BaseApiController
    {
        public List<AgenteTipo_atendimentoModel> GetViewObj(List<Agente_Tipo_atendimento> objBanco)
        {
            var retorno = new List<AgenteTipo_atendimentoModel>();

            foreach (var obj in objBanco)
            {
                retorno.Add(new AgenteTipo_atendimentoModel
                {
                    Id_Tipo_atendimento = obj.Tipo_atendimento.Id,
                    Tipo_atendimento = obj.Tipo_atendimento.Nome,
                    Id = obj.Id
                });
            }

            return retorno;
        }
    }
}