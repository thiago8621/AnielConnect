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
    public class Perfil_VendedorModels
    {
        public virtual Int64 Id { get; set; }

        public virtual String Descricao { get; set; }

        public virtual Tipo_Pessoa TipoPessoaSel { get; set; }

        public virtual String Tipo_Especificas { get; set; }
        public virtual String Regioes_Espeficias { get; set; }

         public virtual String Categoria_Especificas { get; set; }

        public virtual String MeioEspecificos { get; set; }
        public virtual String Meio_Captacao { get; set; }
        public virtual String Pais { get; set; }
        public virtual String Uf { get; set; }
        public virtual String Cidade { get; set; }

        public virtual String Categoria { get; set; }
        public virtual String Area { get; set; }

        public virtual String Area_Especificas { get; set; }      


        public virtual String Status { get; set; }
         public virtual Status_Default? StatusFiltro { get; set; }
         public virtual Padrao? Tipo_PessoaFiltro { get; set; }
        public virtual Padrao? MeioCaptacaoFiltro { get; set; }
        public virtual Padrao ?Categoria_ClienteFiltro { get; set; }
         public virtual Padrao? RegiaoFiltro { get; set; }

        public virtual Padrao? Area_Cobertura { get; set; }


        private List<Perfil_VendedorTipoPessoaCadastroModel> TipoPessoaLista;
        public virtual List<Perfil_VendedorTipoPessoaCadastroModel> TipòPessoa1
        {
            get
            {
                if (TipoPessoaLista == null)
                    TipoPessoaLista = new List<Perfil_VendedorTipoPessoaCadastroModel>();
                return TipoPessoaLista;

            }
            set
            {
                TipoPessoaLista = value;
            }

        }
        private List<Perfil_VendedorTipoCategoriaCadastroModel> TipoCategoriaLista;
        public virtual List<Perfil_VendedorTipoCategoriaCadastroModel> TipoCategoria
        {
            get
            {
                if (TipoCategoriaLista == null)
                    TipoCategoriaLista = new List<Perfil_VendedorTipoCategoriaCadastroModel>();
                return TipoCategoriaLista;

            }
            set
            {
                TipoCategoriaLista = value;
            }

        }
            private List<Perfil_VendedorTipoEnderecoCadastroModel> TipoEnderecoLista;
        public virtual List<Perfil_VendedorTipoEnderecoCadastroModel> TipoEndereco
        {
            get
            {
                if (TipoEnderecoLista == null)
                    TipoEnderecoLista = new List<Perfil_VendedorTipoEnderecoCadastroModel>();
                return TipoEnderecoLista;

            }
            set
            {
                TipoEnderecoLista = value;
            }

        }


        private List<Perfil_VendedorMeioCaptacaoCadastroModel> MeioCaptacaoLista;
        public virtual List<Perfil_VendedorMeioCaptacaoCadastroModel> MeioCaptacao
        {
            get
            {
                if (MeioCaptacaoLista == null)
                    MeioCaptacaoLista = new List<Perfil_VendedorMeioCaptacaoCadastroModel>();
                return MeioCaptacaoLista;

            }
            set
            {
                MeioCaptacaoLista = value;
            }

        }


    }




    public class Perfil_VendedorCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        [Display(Name = "CODIGO", ResourceType = typeof(Resources.RsCadastros))]

        public virtual String Codigo { get; set; }

        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsCadastros))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Descricao { get; set; }
        public virtual Boolean Tipo_Especificas { get; set; }
        public virtual Boolean Pessoa_Especificas { get; set; }

        public virtual Boolean Regioes_Espeficias { get; set; }
        public virtual Tipo_Pessoa Tipo_Pessoa { get; set; }

        public virtual Boolean Categoria_Especificas { get; set; }

        public virtual Boolean MeioEspecificos { get; set; }
        public virtual Boolean Area_Especificas { get; set; }
        public virtual String Meio_Captacao { get; set; }
        public virtual String Pais { get; set; }
        public virtual String Uf { get; set; }
        public virtual String Cidade { get; set; }

        public virtual String Categoria { get; set; }
        public virtual String Area { get; set; }



        public virtual Boolean Status { get; set; }
         public virtual Boolean Autorizacao_Vendas { get; set; }


        private List<Perfil_VendedorTipoPessoaCadastroModel> TipoPessoaLista;
        public virtual List<Perfil_VendedorTipoPessoaCadastroModel> TipoPessoa
        {
            get
            {
                if (TipoPessoaLista == null)
                    TipoPessoaLista = new List<Perfil_VendedorTipoPessoaCadastroModel>();
                return TipoPessoaLista;

            }
            set
            {
                TipoPessoaLista = value;
            }

        }
        private List<Perfil_VendedorTipoCategoriaCadastroModel> TipoCategoriaLista;
        public virtual List<Perfil_VendedorTipoCategoriaCadastroModel> TipoCategoria
        {
            get
            {
                if (TipoCategoriaLista == null)
                    TipoCategoriaLista = new List<Perfil_VendedorTipoCategoriaCadastroModel>();
                return TipoCategoriaLista;

            }
            set
            {
                TipoCategoriaLista = value;
            }

        }

        private List<Perfil_VendedorTipoAreaCadastroModel> TipoAreaLista;
        public virtual List<Perfil_VendedorTipoAreaCadastroModel> TipoArea
        {
            get
            {
                if (TipoAreaLista == null)
                    TipoAreaLista = new List<Perfil_VendedorTipoAreaCadastroModel>();
                return TipoAreaLista;

            }
            set
            {
                TipoAreaLista = value;
            }

        }

        private List<Perfil_VendedorTipoEnderecoCadastroModel> TipoEnderecoLista;
        public virtual List<Perfil_VendedorTipoEnderecoCadastroModel> TipoEndereco
        {
            get
            {
                if (TipoEnderecoLista == null)
                    TipoEnderecoLista = new List<Perfil_VendedorTipoEnderecoCadastroModel>();
                return TipoEnderecoLista;

            }
            set
            {
                TipoEnderecoLista = value;
            }

        }

        private List<Perfil_VendedorMeioCaptacaoCadastroModel> MeioCaptacaooLista;
        public virtual List<Perfil_VendedorMeioCaptacaoCadastroModel> MeioCaptacao
        {
            get
            {
                if (MeioCaptacaooLista == null)
                    MeioCaptacaooLista = new List<Perfil_VendedorMeioCaptacaoCadastroModel>();
                return MeioCaptacaooLista;

            }
            set
            {
                MeioCaptacaooLista = value;
            }

        }



    }

    public class Perfil_VendedorTipoPessoaCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        public virtual String Tipo_Pessoa { get; set; }
        public virtual String Tipo_Pessoa_Desc { get; set; }

      
    }
        public class Perfil_VendedorTipoEnderecoCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        public virtual String Pais { get; set; }
        public virtual String Pais_Desc { get; set; }

        public virtual String Cidade { get; set; }
        public virtual String Cidade_Desc { get; set; }

        public virtual String Uf { get; set; }
         public virtual String Uf_Desc { get; set; }

        public virtual String Tipo_Pessoa_Desc { get; set; }

      
    }

      public class Perfil_VendedorTipoCategoriaCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        public virtual String Tipo_Categoria { get; set; }
        public virtual String Tipo_Categoria_Desc { get; set; }

      
    }
       public class Perfil_VendedorTipoAreaCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        public virtual String Tipo_Area { get; set; }
        public virtual String Tipo_Area_Desc { get; set; }

      
    }

    public class Perfil_VendedorMeioCaptacaoCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        public virtual String Meio_Captacao { get; set; }
       
        public virtual String Desc_Meio_Captacao { get; set; }


    }
    public class LogHistoricoVendaModel
    {
        public virtual long Id { get; set; }
        public virtual long IdLog { get; set; }
        public virtual String Nome_usuario { get; set; }
        public virtual String UsuarioCriacao { get; set; }
        public virtual String DataHoraCriacao { get; set; }
        public virtual String TipoOperacao { get; set; }

        /// <summary>
        /// Identificador do Objeto
        /// </summary>
        public virtual String Chave { get; set; }

        /// <summary>
        /// Texto que representa o Identificador do Objeto 
        /// </summary>
        public virtual String DescricaoResourse { get; set; }

        public virtual String Campo { get; set; }
        public virtual String Campo_ValorAntes { get; set; }
        public virtual String Campo_ValorDepois { get; set; }
        public virtual String Observacao { get; set; }
    }

    public class HistoricoAlteracaoPerfil_Vendedor_DataTableRequest : DataTableRequest
    {
        public virtual Int64? Id { get; set; }
        public virtual String DataIni { get; set; }
        public virtual String DataFim { get; set; }
        public virtual String Objeto { get; set; }
        public virtual String TipoOperacao { get; set; }
        public virtual String Campo { get; set; }
        public virtual String Observacao { get; set; }
    }



    public class Perfil_Vendedor_DataTableRequest : DataTableRequest
    {

        
        public virtual String id_Status { get; set; }
        public virtual String id_Area_Cobertura { get; set; }
        public virtual String id_Meio_Captacao { get; set; }
        public virtual String id_Regiao { get; set; }
        //public virtual Int64? Id_Produto { get; set; }
        //public virtual Int64? Id_Plano { get; set; }
        public virtual String id_Tipo_Pessoa { get; set; }
         public virtual String id_Categoria_Cliente { get; set; }

    }
}