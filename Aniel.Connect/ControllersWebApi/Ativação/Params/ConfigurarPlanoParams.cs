using Aniel.Connect.Model.nhibernate;
using Sinapse.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.ControllersWebApi.Ativacao.Params
{
    public class ConfigurarPlanoParams
    {
        public Concentradora concentradora { get; set; }
        public Plano plano { get; set; }
    }
}