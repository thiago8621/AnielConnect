using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Models
{
    public enum TipoValor_DescontoCadastroModel
    {
        Valor = 1,
        Percentual = 2
    }

    public class DescontoCadastroModel : IViewModel<Int64>
    {

        public virtual Int64 Id { get; set; }

        // [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsBase))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Descricao { get; set; }



        //[Required(ErrorMessageResourceType = typeof(Resources.RsBase),
        //          ErrorMessageResourceName = "REQUIRED")]
        public virtual TipoValor_DescontoCadastroModel TipoValor { get; set; }

        public virtual Double? Percentual_Desconto { get; set; }


        //[Required(ErrorMessageResourceType = typeof(Resources.RsBase),
        //          ErrorMessageResourceName = "REQUIRED")]

        public virtual String Valor_Desconto { get; set; }

        public virtual Int64? Id_Tipo_Desconto { get; set; }


        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        public virtual Desconto_Captacao? Desconto_Captacao { get; set; }

        public virtual Int64? Repeticoes_Desconto { get; set; }

        public virtual String CondominiosPredios { get; set; }
        public virtual String CodigoAuxiliar { get; set; }
        public virtual Boolean Status { get; set; }

        public virtual Boolean Desconto_Tempo_Indeterminado { get; set; }
        public virtual Boolean Valor_Unico { get; set; }
        public virtual Boolean Automatico { get; set; }

        /// <summary>
        /// Temporário para alimentar a grid
        /// </summary>
        public virtual Int64? Id_Meio { get; set; }
        public virtual Int64? Pais { get; set; }
        public virtual Int64? Uf { get; set; }

        private List<Meio_CaptacaoListaModel> MeiosLista;

        public virtual List<Meio_CaptacaoListaModel> Meios

        {
            get
            {
                if (MeiosLista == null)
                    MeiosLista = new List<Meio_CaptacaoListaModel>();
                return MeiosLista;
            }
            set
            {
                MeiosLista = value;
            }
        }

        private List<DescontoRegiaoListaModel> RegioesLista;

        public virtual List<DescontoRegiaoListaModel> Regioes

        {
            get
            {
                if (RegioesLista == null)
                    RegioesLista = new List<DescontoRegiaoListaModel>();
                return RegioesLista;
            }
            set
            {
                RegioesLista = value;
            }
        }


        private List<CondominiosCadastroModel> Condominio_PrediosLista;

        public virtual List<CondominiosCadastroModel> Condominio_Predios
        {
            get
            {
                if (Condominio_PrediosLista == null)
                    Condominio_PrediosLista = new List<CondominiosCadastroModel>();
                return Condominio_PrediosLista;
            }
            set
            {
                Condominio_PrediosLista = value;
            }
        }


        private List<DescontoValoresListaModel> ValoresLista;

        public virtual List<DescontoValoresListaModel> Valores

        {
            get
            {
                if (ValoresLista == null)
                    ValoresLista = new List<DescontoValoresListaModel>();
                return ValoresLista;
            }
            set
            {
                ValoresLista = value;
            }
        }

        private List<DescontoCondicaoListaModel> CondicoesCombinadasLista;

        public virtual List<DescontoCondicaoListaModel> CondicoesCombinadas

        {
            get
            {
                if (CondicoesCombinadasLista == null)
                    CondicoesCombinadasLista = new List<DescontoCondicaoListaModel>();
                return CondicoesCombinadasLista;
            }
            set
            {
                CondicoesCombinadasLista = value;
            }
        }

        private List<DescontoCondicaoListaModel> CondicoesIndependentesLista;

        public virtual List<DescontoCondicaoListaModel> CondicoesIndependentes

        {
            get
            {
                if (CondicoesIndependentesLista == null)
                    CondicoesIndependentesLista = new List<DescontoCondicaoListaModel>();
                return CondicoesIndependentesLista;
            }
            set
            {
                CondicoesIndependentesLista = value;
            }
        }
    }

    public class CondominiosCadastroModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Id_Condominio { get; set; }
        public virtual String Cond_Desc { get; set; }
    }

    public class DescontoCondicaoListaModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String TipoCondicao { get; set; }
        public virtual String ValorCondicao { get; set; }
        public virtual String ValorCondicaoMinimo { get; set; }
        public virtual String ValorMeses { get; set; }
        public virtual String ValorCondicaoMaximo { get; set; }
    }

    public class DescontoRegiaoListaModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Pais { get; set; }
        public virtual String Estado { get; set; }
        public virtual String Cidade { get; set; }
        public virtual String Pais_Desc { get; set; }
        public virtual String Estado_Desc { get; set; }
        public virtual String Cidade_Desc { get; set; }
    }

    public class DescontoValoresListaModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String MesInicial { get; set; }
        public virtual String MesFinal { get; set; }
        public virtual String Valor { get; set; }
        public virtual String Percentual { get; set; }
    }

    public class Desconto_DataTableRequest : DataTableRequest
    {
        public long? IdClienteDesconto { get; set; }
    }
    public class ClientePlanoDescontoValores
    {
      
        public int IdClienteContrato { get; set; }
        public String ValorFinalTotal { get; set; }
        public String ValorInstalacaoFinal { get; set; }
        public String TotalDesconto { get; set; }
        public String ValorSubTotal { get; set; }

        public String ValorAPagarSemInstalacao { get; set; }
    }

    public class DescontoClientePlanoProdutoList
    {
        public virtual Int64 Id { get; set; }
        public virtual Int64? IdDesconto { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String TipoDesconto { get; set; }
        public virtual String NumeroContratoVinculado { get; set; }
        public virtual String Formato { get; set; }
        public virtual String ValorDesconto { get; set; }
        public virtual String PercentualDesconto { get; set; }
    }
    public class DescontoClientePlanoProdutoRequest : DataTableRequest
    {
        public virtual Int64? IdVenda { get; set; }
    }
    public class Desconto_ComboModel : ObjComboResult<Int64?>
    {
        public virtual String Percentual_Desconto { get; set; }
        public virtual String Valor_Desconto { get; set; }
        public virtual String Tipo_Desconto { get; set; }
        public virtual TipoValor_DescontoCadastroModel TipoValor { get; set; }
        public virtual Int64? Id_Tipo_Desconto { get; set; }
        public virtual Desconto_Captacao? Desconto_Captacao { get; set; }
        public virtual Int64? Repeticoes_Desconto { get; set; }
        public virtual Boolean Status { get; set; }
        public virtual Boolean Desconto_Tempo_Indeterminado { get; set; }
        public virtual Boolean Valor_Unico { get; set; }
        public virtual Int64? Formato { get; set; }
        public virtual Int64? Id_Meio { get; set; }
        public virtual DateTime? Data_Inicio { get; set; }
    }

    public class DescontoDetalhes : DescontoListaModel
    {
        public virtual String MesInicio { get; set; }
        public virtual String MesFinal { get; set; }
        public virtual String Valor { get; set; }
        public virtual String Percentual { get; set; }
        public virtual String DuracaoDesconto { get; set; }
        public virtual String ValorUnico { get; set; }
        public virtual String UsuarioCriacao { get; set; }
        private IEnumerable<DescontoDetalhesPrazos> _meses;
        public IEnumerable<DescontoDetalhesPrazos> Meses 
        { 
            get 
            { 
                if (_meses == null) 
                { 
                    return new List<DescontoDetalhesPrazos>(); 
                } 
                return _meses; 
            }
            set { _meses = value; } }
    }
    public class DescontoDetalhesPrazos 
    {
        public String MesReferencia { get; set; }
        public Double ValorMes { get; set; }
    }

    public class ValidaDescontosModel
    {
        public virtual List<DescontoListaModel> Descontos { get; set; }
        public virtual Int64? IdMeioCaptacao { get; set; }
        public virtual Int64? IdCategoriaCliente { get; set; }
        public virtual List<PlanoProdutoListModel> Planos { get; set; } = new List<PlanoProdutoListModel>();


    }
    //GetCombo
    public class RequestObjComboCliente : RequestObjCombo<long?>
    {
        public virtual DescontoParametros Config { get; set; }
    }

    public class DescontoParametros
    {
        public Int64? Id_Cliente { get; set; }
        public virtual String Pais { get; set; }
        public virtual String Estado { get; set; }
        public virtual String Cidade { get; set; }
        public Tipo_Pessoa? Tipo_Cliente { get; set; }
        public string Categoria_Cliente { get; set; }
        public string Meio_Captacao { get; set; }
        public List<string> Plano_Produto { get; set; } = new List<string>();
        public List<string> Plano_Produto_Combo { get; set; } = new List<string>();
        public List<double?> Valor_Plano_Final { get; set; } = new List<double?>();
        public string Valor_Final_Total { get; set; }
        public Int32 Pag_Sem_Atraso_Meses { get; set; }
        public List<string> Forma_Pagamento { get; set; } = new List<string>();
        public List<long?> Periodo_Fidelidade_Meses { get; set; } = new List<long?>();
        public List<Tipo_Tecnologia?> Tipo_Tecnologia { get; set; } = new List<Tipo_Tecnologia?>();
        public List<Tipo_Forma_Pagamento?> Tipo_Forma_Pagamento { get; set; } = new List<Tipo_Forma_Pagamento?>();
        public Condominio_Predio TemCondomioPredio { get; set; }

    }

    public class RequestFiltroDesconto<T>
    {
        public T registros { get; set; }
        public bool Meio_Captacao { get; set; }
        public bool Categoria_Cliente { get; set; }
        public bool Plano { get; set; }
        public bool Valor_max { get; set; }
    }
    public class AdicionarClienteDescontoModel
    {
        public List<PostApiMensagem> retorno { get; set; }
        public List<Cliente_Desconto> descontostemp { get; set; }
        public List<Tipo_Desconto> Tipo_Desconto { get; set; }
        public Desconto Desconto { get; set; }
        public Desconto_Captacao? Desconto_Captacao { get; set; }
        public Cliente objBanco { get; set; }
        public bool DescontoJaCadastrado { get; set; }
        public bool novaCondicao { get; set; }
        public bool Acumula_Descontos_Mesmo_Tipo { get; set; }
        public double Valor_Com_Desconto { get; set; }
        public bool Descontos_Tipo_Diferente { get; set; }
    }

}