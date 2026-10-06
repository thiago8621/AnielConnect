using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.Model
{
    public class Config_Aniel_Integrator
    {
        public String URL { get; set; }
        public String UsuarioAniel { get; set; }
        public String CodCtPadrao { get; set; }
        public String ProjetoPadrao { get; set; }
        public String Unidade_Custo { get; set; }
        public String Token { get; set; }
        public String Setores { get; set; }
        public String UF { get; set; }
        public Int64? IdFilial { get; set; }
    }
}
