using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{



    public class Natureza_ProducaoListaModel
    {
        public virtual Int64? Id { get; set; }

        public virtual String Descricao { get; set; }
        
    }

    public class LogEquipeNaturezaListaModel {
        public virtual String Usuario_Alteracao { get; set; }
        public virtual String Data_Alteracao { get; set; }
        public virtual String Tipo_Alteracao { get; set; }
        public virtual String Equipe { get; set; }
        public virtual String Natureza { get; set; }
    }

    public class RelatorioEquipeNaturezaListaModel
    {
        public virtual String Usuario_Criacao { get; set; }
        public virtual String Data_Criacao { get; set; }
        public virtual String Equipe { get; set; }
        public virtual String Natureza { get; set; }
        public virtual String Num_Fre { get; set; }
    }


    public class EquipeNaturezaDataTableRequest : DataTableRequest
    {
        public String Equipe { get; set; }
        public String Natureza { get; set; }

    }

    public class Natureza_Producao_DataTableRequest : DataTableRequest
    {

    }
}