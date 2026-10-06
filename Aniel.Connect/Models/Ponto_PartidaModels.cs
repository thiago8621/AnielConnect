using Aniel.Connect.Core;
using Aniel.Connect.Core.Service;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.Model;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class Ponto_PartidaListaModel
    {
        public virtual Int64 Id { get; set; }





        public virtual String Descricao { get; set; }
        public virtual String Cep { get; set; }
        public virtual String Logradouro { get; set; }
        public virtual String Numero { get; set; }
        public virtual String Bairro { get; set; }
        public virtual String Cidade { get; set; }
        public virtual String Complemento { get; set; }

        public virtual String Id_Equipe { get; set; }

        public virtual String Desc_Equipe { get; set; }

    }


    public class Ponto_PartidaCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        public virtual String Descricao { get; set; }

        //[Display(Name = "CEP", ResourceType = typeof(Resources.RsCadastros))]
        //[Required(ErrorMessageResourceType = typeof(Resources.RsBase),
        //          ErrorMessageResourceName = "REQUIRED")]        
        public virtual String Cep { get; set; }

        public virtual String Logradouro { get; set; }

        public virtual String Numero { get; set; }

        public virtual Int64? Id_Bairro { get; set; }

        public virtual String Bairro { get; set; }

        public virtual Int64? Id_Cidade { get; set; }

        public virtual String Cidade { get; set; }

        public virtual UF? UF { get; set; }

        public virtual String Complemento { get; set; }

        public virtual String Latitude { get; set; }

        public virtual String Longitude { get; set; }

        public virtual String Id_Equipe { get; set; }

        public virtual String Desc_Equipe { get; set; }

        private List<Equipe_Ponto_PartidaListaModel> EquipesLista;

        public virtual List<Equipe_Ponto_PartidaListaModel> Equipes

        {
            get
            {
                if (EquipesLista == null)
                    EquipesLista = new List<Equipe_Ponto_PartidaListaModel>();
                return EquipesLista;
            }
            set
            {
                EquipesLista = value;
            }
        }

    }

    public class Ponto_PartidaMotivoCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        
    }


    public class Ponto_Partida_DataTableRequest : DataTableRequest
    {
        public String Descricao { get; set; }

        public String Equipe { get; set; }

        public String Estado { get; set; }

        public String Cidade { get; set; }

        public String Bairro { get; set; }

        public String Nome_Equipe { get; set; }

        public virtual String Id_Equipe { get; set; }

        public virtual String Desc_Equipe { get; set; }

    }

    public class Equipe_Ponto_PartidaListaModel
    {
        public virtual String Descricao { get; set; }
        public virtual String Id { get; set; }
        public virtual String Id_Equipe { get; set; }



    }


}