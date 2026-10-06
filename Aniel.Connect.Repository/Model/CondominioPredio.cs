using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.Repository.Model
{
    public class Condominio_PredioCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsBase))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Descricao { get; set; }

        public virtual String Cep { get; set; }


        [Display(Name = "ENDERECO_LOGRADOURO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual long? Endereco_Logradouro { get; set; }

        public virtual Tipo_Endereco_Logradouro Tipo_Endereco_Logradouro { get; set; }

        public virtual String Numero { get; set; }
        public virtual String Logradouro { get; set; }
        public virtual long? Id_Pais { get; set; }
        public virtual String Uf { get; set; }
        public virtual long? Id_Cidade { get; set; }
        public virtual String Bairro { get; set; }
        public bool Cadastro_Faixa { get; set; }
        public virtual bool Definir_Caixa_Condominio_Predio { get; set; }
        public virtual Int64? Id_Caixa { get; set; }

        public virtual Boolean Status { get; set; }

        private List<Condominio_PredioApartamentoListaModel> ApartamentosLista;
        public List<Condominio_PredioApartamentoListaModel> Apartamentos
        {
            get
            {
                if (ApartamentosLista == null) { ApartamentosLista = new List<Condominio_PredioApartamentoListaModel>(); }
                return ApartamentosLista;
            }
            set
            {
                ApartamentosLista = value;
            }
        }

        private List<Condominio_PredioCaixaListaModel> CaixaLista;

        public virtual List<Condominio_PredioCaixaListaModel> Caixas

        {
            get
            {
                if (CaixaLista == null)
                    CaixaLista = new List<Condominio_PredioCaixaListaModel>();
                return CaixaLista;
            }
            set
            {
                CaixaLista = value;
            }
        }
    }
    public class Condominio_PredioCaixaListaModel
    {
        public virtual Int64 Id { get; set; }
        public virtual Int64 Id_Caixa { get; set; }
        public virtual Int64 Id_Condominio_Predio { get; set; }
        public virtual String Descricao { get; set; }

    }

    public class Condominio_PredioApartamentoListaModel
    {
        public Int64 Id { get; set; }
        public String Descricao { get; set; }
        public long Id_Bloco { get; set; }
        public string Bloco { get; set; }
        public Status_Default? Status { get; set; }

    }
}
