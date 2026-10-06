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
    public class Acompanhamento_Venda_MapaModel
    {
        public virtual String Equipe { get; set; }

        public virtual String Nome { get; set; }

        public virtual String Cor_StatusEquipe { get; set; }

        public virtual String Icone_Bateria { get; set; }
        public virtual String Cor_Bateria { get; set; }
        public virtual String Bateria { get; set; }
        public virtual String TotalVisitas { get; set; }
        public virtual String BaixadasProdutivas { get; set; }
        public virtual String BaixadasImprodutivas { get; set; }
        public virtual String Restantes { get; set; }
        public virtual String Ultimo_Endereço { get; set; }
        public virtual String Atualizacao { get; set; }
        //public virtual String Equipe_Mapa { get; set; }
        //public virtual String Detalhes_Equipe { get; set; }

        public virtual String Status_App { get; set; }






    }


    
}