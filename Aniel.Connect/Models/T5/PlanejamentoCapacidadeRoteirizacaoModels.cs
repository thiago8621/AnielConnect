using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using Aniel.Connect.Core;
using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.enumerado;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.Models.T5
{

    public class GestaoCapacidadeRoteirizacaoResponse
    {
        public virtual List<TecnicoRoteirizar> tecnicos { get; set; } = new List<TecnicoRoteirizar>();
        public virtual List<DocumentoPlanejamento> servicos { get; set; } = new List<DocumentoPlanejamento>();
        public virtual double minutosHorarioTrabalhoPadrao { get; set; }
    }

    

    public class SalvarAjustePerfilRequest
    {
        public List<SalvarAjustePerfil> lista { get; set; } = new List<SalvarAjustePerfil>();
        public virtual string Data { get; set; }
        public virtual string Usuario { get; set; }
    }

    public class SalvarAjustePerfil
    {
        public string Equipe { get; set; }
        public long? IdPerfilAgendamento { get; set; }

    }

    

}