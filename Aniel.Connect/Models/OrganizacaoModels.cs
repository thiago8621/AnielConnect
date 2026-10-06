using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{

    public class OrganizacaoListaModel
    {

        public virtual Int64 Id { get; set; }
        public virtual String Nome_Fantasia { get; set; }
        public virtual String Razao_Social { get; set; }
        public virtual String Classificacao { get; set; }
        public virtual String CNPJ { get; set; }
        public virtual String Status { get; set; }
    }

    public class OrganizacaoCadastroModel : IViewModel<Int64>
    {

        public virtual Int64 Id { get; set; }        
        public virtual String Nome_Fantasia { get; set; }
        public virtual String Razao_Social { get; set; }
        public virtual String CNPJ { get; set; }
        public virtual Classificacao_Organizacao? Classificacao_Organizacao { get; set; }
        public virtual Boolean Status { get; set; }

        public virtual String Cep { get; set; }
        public virtual String Logradouro { get; set; } 
        public virtual Int32? Numero { get; set; }
        public virtual String Uf { get; set; }
        public virtual String Cidade { get; set; } 
        public virtual String Bairro { get; set; }
        public virtual String Complemento { get; set; }
        public virtual Int64? Id_Cidade { get; set; }
        public virtual Int64? Id_Bairro { get; set; }
        public virtual String Latitude { get; set; }
        public virtual String Longitude { get; set; }

        public virtual Tipo_Contato? Tipo_Contato { get; set; }


        private List<Organizacao_DominioModel> DominioLista;
        private List<Organizacao_ContatoModel> ContatoLista;

        public virtual List<Organizacao_DominioModel> Dominios
        {
            get
            {
                if (DominioLista == null)
                    DominioLista = new List<Organizacao_DominioModel>();
                return DominioLista;
            }
            set
            {
                DominioLista = value;
            }
        }

        

        public virtual List<Organizacao_ContatoModel> Contatos
        {
            get
            {
                if (ContatoLista == null)
                    ContatoLista = new List<Organizacao_ContatoModel>();
                return ContatoLista;
            }
            set
            {
                ContatoLista = value;
            }
        }

    }

    public class Organizacao_DominioModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Dominio { get; set; }
    }

    public class Organizacao_ContatoModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Contato { get; set; }
        public virtual String Tipo_Contato_Descricao { get; set; }
        public virtual String Informacao_Adicional { get; set; }
        public virtual Tipo_Contato Tipo_Contato { get; set; }
    }

    public class Organizacao_DataTableRequest : DataTableRequest
    {

    }



    

}