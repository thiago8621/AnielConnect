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
    public class Cobranca_ServicoListaModel
    {
        public virtual Int64 Id { get; set; }

        public virtual String Descricao { get; set; }

        public virtual String Escolha_Cobranca { get; set; }

        public virtual String Segmento { get; set; }

        public virtual String Segmento_Desc { get; set; }

        public virtual String Desc_Tipo_Servico { get; set; }
        public virtual String Tipo_Servico { get; set; }

        public virtual String Desc_Sub_Tipo_Servico { get; set; }
        public virtual String Sub_Tipo_Servico { get; set; }

        public virtual String Cod_Encerramento { get; set; }

        public virtual String Plano_Produto { get; set; }
        public virtual Double Valor_Cobrado { get; set; }

        public virtual Boolean TodasSubTipoServico { get; set; }

        public virtual Boolean TodasCod_Encerramento { get; set; }


        public virtual String Status { get; set; }

        private List<Cobranca_ServicoTipoServicoCadastroModel> TipoServicoLista;
        public virtual List<Cobranca_ServicoTipoServicoCadastroModel> TipoServico
        {
            get
            {
                if (TipoServicoLista == null)
                    TipoServicoLista = new List<Cobranca_ServicoTipoServicoCadastroModel>();
                return TipoServicoLista;

            }
            set
            {
                TipoServicoLista = value;
            }

        }


        private List<Cobranca_ServicoPlanoProdutoCadastroModel> PlanoProdutoLsta;
        public virtual List<Cobranca_ServicoPlanoProdutoCadastroModel> PlanoProduto
        {
            get
            {
                if (PlanoProdutoLsta == null)
                    PlanoProdutoLsta = new List<Cobranca_ServicoPlanoProdutoCadastroModel>();
                return PlanoProdutoLsta;

            }
            set
            {
                PlanoProdutoLsta = value;
            }

        }


    }




    public class Cobranca_ServicoCadastroModel : IViewModel<Int64>
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

        public virtual Escolha_Cobranca? Escolha_Cobranca { get; set; }

        public virtual String Segmento { get; set; }

        public virtual String Desc_Tipo_Servico { get; set; }

        public virtual String Tipo_Servico { get; set; }

        public virtual String Desc_Sub_Tipo_Servico { get; set; }
        public virtual String Sub_Tipo_Servico { get; set; }

        public virtual String Cod_Encerramento { get; set; }

        public virtual String Plano_Produto { get; set; }

        public virtual Boolean TodasSubTipoServico { get; set; }

        public virtual Boolean TodasCod_Encerramento { get; set; }


        public virtual Boolean Status { get; set; }

        private List<Cobranca_ServicoTipoServicoCadastroModel> TipoServicoLista;
        public virtual List<Cobranca_ServicoTipoServicoCadastroModel> TipoServico
        {
            get
            {
                if (TipoServicoLista == null)
                    TipoServicoLista = new List<Cobranca_ServicoTipoServicoCadastroModel>();
                return TipoServicoLista;

            }
            set
            {
                TipoServicoLista = value;
            }

        }
        private List<Cobranca_ServicoPlanoProdutoCadastroModel> PlanoProdutoLsta;
        public virtual List<Cobranca_ServicoPlanoProdutoCadastroModel> PlanoProduto
        {
            get
            {
                if (PlanoProdutoLsta == null)
                    PlanoProdutoLsta = new List<Cobranca_ServicoPlanoProdutoCadastroModel>();
                return PlanoProdutoLsta;

            }
            set
            {
                PlanoProdutoLsta = value;
            }

        }



    }

    public class CobrancaInstalacaoList
    {
        public string Id { get; set; }
        public string Parcela { get; set; }
        public decimal ValorParcela { get; set; }
    }

    public class Cobranca_ServicoTipoServicoCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }
        public virtual String Segmento { get; set; }
        public virtual String Segmento_Desc { get; set; }
        public virtual String Desc_Tipo_Servico { get; set; }
        public virtual String Tipo_Servico { get; set; }
        public virtual String Desc_Sub_Tipo_Servico { get; set; }
        public virtual String Sub_Tipo_Servico { get; set; }      
        public virtual String Cod_Encerramento { get; set; }
        public virtual String Desc_Cod_Encerramento { get; set; }
        public virtual String Plano_Produto { get; set; }
        public virtual Boolean TodasSubTipoServico { get; set; }
        public virtual Boolean TodasCod_Encerramento { get; set; }
        public virtual String Cod_Cobranca { get; set; }
        public virtual String Desc_Forma_Cobranca { get; set; }
    }

    public class Cobranca_ServicoPlanoProdutoCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        public virtual String Plano_Produto { get; set; }
        public virtual String Valor_Cobrado { get; set; }
        public virtual String Desc_Plano_Produto { get; set; }


    }

    public class Cobranca_Servico_DataTableRequest : DataTableRequest
    {

        public virtual String Segmento { get; set; }
        public virtual String Id_Segmento { get; set; }
        public virtual String Id_Tipo_Servico { get; set; }
        public virtual String Id_Cod_Encerramento { get; set; }
        public virtual Int64? Id_Plano_Produto { get; set; }
        //public virtual Int64? Id_Produto { get; set; }
        //public virtual Int64? Id_Plano { get; set; }
        public virtual String Id_Sub_Tipo_Servico { get; set; }
    }

    public class ObjetosCobranca_Plano_Cliente
    {
        public virtual Plano Plano { get; set; }
        public virtual Cliente Cliente { get; set; }
        public virtual Faturamento_Cliente Faturamento_Cliente { get; set; }
        public virtual Forma_Pagamento Forma_Pagamento { get; set; }
        public virtual Documento_ProducaoAniel Documento_Producao { get; set; }
    }

    public class ObjetoTipoServico
    {
        public virtual String Id_Segmento { get; set; }
        public virtual String Id_Tipo_Servico { get; set; }
        public virtual String Id_Sub_Tipo_Servico { get; set; }
        public virtual String Id_Cod_Encerramento { get; set; }
        public virtual String Id_Cobranca { get; set; }
    }
}