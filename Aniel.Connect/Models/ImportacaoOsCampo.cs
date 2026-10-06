using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class ImportacaoOsCampo
    {
        public long Id { get; set; }
        public string CampoAniel { get; set; }
        public string CampoPlanilha { get; set; }
        public string Apelido { get; set; }
        public bool Importar { get; set; }
        public bool Chave { get; set; }
    }
}