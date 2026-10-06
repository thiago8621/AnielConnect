using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class Perfil_Ativacao_Grupo_AreaListModel
    {
        public long Id { get; set; }
        public string Cod_Area { get; set; }
        public string Desc_Area { get; set; }

        public string Cod_Escritorio { get; set; }
        public string Desc_Escritorio { get; set; }
    }
}