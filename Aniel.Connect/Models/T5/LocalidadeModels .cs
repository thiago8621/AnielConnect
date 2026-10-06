using Aniel.Connect.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models.T5
{
    

    public class LocalidadeListaModel
    {
        public String Localidade { get; set; }
        public String Grupo_Area { get; set; }
        public String Area { get; set; }

    }

    public class LogEquipeLocalidadeListaModel {

        public virtual String Usuario_Alteracao { get; set; }
        public virtual String Data_Alteracao { get; set; }
        public virtual String Tipo_Alteracao { get; set; }
        public virtual String Equipe { get; set; }
        public virtual String Grupo_Area { get; set; }
        public virtual String Area { get; set; }
        public virtual String Localidade { get; set; }
    }
    public class RelatorioLocalidadeListaModel
    {
        public virtual String Usuario_Criacao { get; set; }
        public virtual String Data_Criacao { get; set; }
        public virtual String Equipe { get; set; }
        public virtual String Grupo_Area { get; set; }
        public virtual String Area { get; set; }
        public virtual String Localidade { get; set; }
        public virtual String Matricula { get; set; }
        public virtual String Perfil { get; set; }
        public virtual String Habilidades { get; set; }
    }



    public class EquipeLocalidadeDataTableRequest : DataTableRequest
    {
        public String TipoRelatorio { get; set; }
        public String Equipe { get; set; }
        public String Grupo_Area { get; set; }
        public String Area { get; set; }
        public String Localidade { get; set; }
        public String Perfil { get; set; }
        public String Habilidades { get; set; }

    }

}