using NHibernate;

using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Aniel.Connect.API_Aniel.Models
{
    public class Config_Aniel_Integrator
    {

        public String URL { get; set; }
        public String UsuarioAniel { get; set; }
        public String UsuarioIntegrator { get; set; }
        public String CodCtPadrao { get; set; }

        public String ProjetoPadrao { get; set; }
        public String Unidade_Custo { get; set; }
        public String Token { get; set; }

        public String Setores { get; set; }


    }
}