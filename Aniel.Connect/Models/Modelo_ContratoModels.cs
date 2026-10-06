using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Service;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Microsoft.Owin.Security;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Models
{
    public class Modelo_ContratoListaModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
                       ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Descricao { get; set; }
        public virtual String Arquivo_Doc { get; set; }
        public virtual String Extensao_Arquivo { get; set; }
        public virtual Boolean Contrato_Automatico { get; set; }
        public virtual Boolean Tem_Carencia { get; set; }
        public virtual Int32? Periodo_Carencia { get; set; }
        public virtual Double? Multa_Carencia { get; set; }

        public virtual String Status { get; set; }
        public virtual Status_Default? Status_Default { get; set; }

        public virtual Boolean Tem_Pro_Rata { get; set; }

    }


    public class Modelo_ContratoCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        [Display(Name = "DESCRICAO", ResourceType = typeof(Resources.RsBase))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                  ErrorMessageResourceName = "REQUIRED")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resources.RsBase),
                        ErrorMessageResourceName = "MAXLENGHT")]
        public virtual String Descricao { get; set; }

        //[Required(ErrorMessageResourceType = typeof(Resources.RsBase),
        //        ErrorMessageResourceName = "REQUIRED")]
        //[Display(Name = "NOME_ARQUIVO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual String Arquivo_Doc { get; set; }

        public virtual Boolean ArquivoAlterado { get; set; }
        public virtual String Extensao_Arquivo { get; set; }

        public virtual Boolean Status { get; set; }

        public virtual Boolean Contrato_Automatico { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "TEM_CARENCIA", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Boolean Tem_Carencia { get; set; }

        [Display(Name = "PERIODO_CARENCIA", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Int64? Periodo_Carencia { get; set; }


        [Display(Name = "MULTA_CARENCIA", ResourceType = typeof(Resources.RsCadastros))]
        public virtual String Multa_Carencia { get; set; }
        public virtual Boolean MultaFidelidade { get; set; }
        public virtual Boolean Tem_Reajuste { get; set; }
        public virtual Tipo_Reajuste_Contratual? Tipo_Reajuste { get; set; }
        public virtual Boolean Cancela_Contrato_Automatico_Final_Carencia { get; set; }
        public virtual Renova_FidelidadeEnum? Renova_Carencia_Automaticamente { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "TEM_PRO_RATA", ResourceType = typeof(Resources.RsCadastros))]
        public virtual Boolean Tem_Pro_Rata { get; set; }
        public virtual Boolean RenovarAutomaticamente { get; set; }
        public virtual String Valor_Reajuste_Contratual { get; set; }
        public virtual String Percentual_Reajuste_Contratual { get; set; }
        public virtual String bloqueios { get; set; }
        public virtual String cobrancas { get; set; }
        public virtual String cobrancasPre { get; set; }
        public virtual String negativacao { get; set; }
        public virtual String removerNegativacao { get; set; }
        public virtual Boolean DisponibilizaTelefonia { get; set; }
        public virtual Boolean Desbloqueio_Conf { get; set; }
        public virtual Boolean BloqueioAutomaticoDebito { get; set; }
        public Int16 Dias_desbloq { get; set; }
        public virtual Frequencia_Reajuste? Frequencia_Reajuste { get; set; }
        public virtual Frequencia_Desbloqueio? Frequencia_Desbloqueio { get; set; }
        //- Projeto Cobranca
        public virtual Boolean Bloqueio_Atraso { get; set; }
        public virtual Boolean Bloqueio_Atraso_Renegociado { get; set; }

        public virtual Boolean ImpedirBloqueio_Automatico { get; set; }
        public virtual Boolean ImpedirBloqueio_Segunda { get; set; }
        public virtual Boolean ImpedirBloqueio_Terca { get; set; }
        public virtual Boolean ImpedirBloqueio_Quarta { get; set; }
        public virtual Boolean ImpedirBloqueio_Quinta { get; set; }
        public virtual Boolean ImpedirBloqueio_Sexta { get; set; }
        public virtual Boolean ImpedirBloqueio_Sabado { get; set; }
        public virtual Boolean ImpedirBloqueio_Domingo { get; set; }
        public virtual Boolean ImpedirBloqueio_Feriados { get; set; }
        public virtual Boolean Utiliza_Desconto { get; set; }
        public virtual Int64? Desconto { get; set; }
        public virtual IntegracaoTelefonia? IntegracaoTelefonia { get; set; }
        public virtual Formato_Cobranca? FormatoCobrancaTelefonia { get; set; }
        public virtual DidFormatoCobrar? DidFormatoCobrar { get; set; }
        public virtual Int64? QuantidadeDid { get; set; }
        [Display(Name = "TESTEGRATIS", ResourceType = typeof(Resources.RsConfiguracao))]
        public bool TesteGratis { get; set; }

        [Display(Name = "DIASTESTE", ResourceType = typeof(Resources.RsConfiguracao))]
        public Int16? DiasTeste { get; set; }

        public virtual long IdPlanoValorInstalacao { get; set; }
        public virtual IList<AvisoModeloContrato> ListaCobrancasPre { get; set; }
        public virtual IList<AvisoModeloContrato> ListaCobrancas { get; set; }
        public virtual IList<AvisoModeloContrato> ListaBloqueios { get; set; }
        public virtual int QuantidadeDiasBloqueio { get; set; }
        public virtual int BloquearNumeroDias { get; set; }
        public virtual int BloquearAtrasarRenegociadas { get; set; }
        public virtual int MaximoTitulosAberto { get; set; }
        public virtual long FrequenciaPermitida { get; set; }
        public virtual long IdAvisoExecucaoNegativacao { get; set; }
        public virtual long DiasNegativacao { get; set; }
        public virtual bool NegativarAutomaticamente { get; set; }
        public virtual bool RemoverNegativacaoAutomaticamente { get; set; }
        public virtual string CodigoIntegracao { get; set; }
        public virtual string Observacao_ModeloContrato { get; set; }
    }
    public class PlanoProduto_CombosAdicionadosViewModel
    {
        public virtual long? Id { get; set; }
        public virtual PlanoProdutoEnum? PlanoProduto { get; set; }
        //Plano ou Produto?
        public virtual long? Tipo_Combo { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Tipo { get; set; }
        public virtual long? Id_Plano_ou_Produto { get; set; }
        public virtual long? sequencia { get; set; }
        public virtual Frequencia_SelecaoPlano Selecionado { get; set; }

        //public virtual long? Minutos_Franquia { get; set; }
    }

    public class PdfModeloContrato
    {
        public virtual String Descricao_Modelo_Contrato { get; set; }
        public virtual String Nome_Pdf { get; set; }
    }

    public class EnviarContrato
    {
        public virtual String Caminho_Contrato { get; set; }
        public virtual String Nome_Contrato { get; set; }
        public virtual String Caminho_Contrato_Pasta_Cliente_pdf { get; set; }
    }

    public class Modelo_Contrato_DataTableRequest : DataTableRequest
    {
        public virtual long? Id { get; set; }
    }

    public class ModeloContratoComboModel : ObjComboResult<Int64>
    {
        public bool Contrato_Automatico { get; set; }
        public bool PossuiCarencia { get; set; }
        public bool PossuiTesteGratis { get; set; }
        public int? PeriodoCarencia { get; set; }
        public int? DiasTesteGratis { get; set; }
    }
    public class DepartamentoComboModel : ObjComboResult<Int64>
    {
        public virtual String Cod_Depto { get; set; }
    }
    public class Monte_seu_Combo
    {
        public virtual List<String> Contratos { get; set; }
        public virtual Int32? Num_Planos { get; set; }
        //public virtual String Explanation { get; set; }

    }
    public class Get_Item_ContratoObjCombo : ObjComboResult<long?>
    {
        public virtual Tipo_Tecnologia? Tipo_Tecnologia { get; set; }

    }
    public class Modelo_PlanoObjCombo : ObjComboResult<long>
    {
        public long? Id_Modelo_Contrato { get; set; }
        public string Valor { get; set; }
        public string ValorInstalacao { get; set; }
        public bool Check_DiasTesteGratis { get; set; }
        public string QtdeDiasGratis { get; set; }
        public bool CheckVigenciaContrato { get; set; }
        public string PeriodoFidelidade { get; set; }
        public string UsaTecnologiaInternet { get; set; }
    }

    public class Modelo_ContratoOptGroupAvulso_Combo
    {
        /// <summary> true para modelos de Contrato Combo / false para modelos de Contrato Avulso </summary>
        public virtual bool Combo { get; set; }

    }
    public class Modelo_ContratoGenerateVariable
    {
        private List<PostApiMensagem> pMensagens;
        private IEnumerable<Cliente_PlanoProduto> pPlanos;
        public List<PostApiMensagem> mensagens
        {
            get
            {
                if (pMensagens == null)
                {
                    pMensagens = new List<PostApiMensagem>();

                }
                return pMensagens;
            }
            set { pMensagens = value; }
        }
        public IEnumerable<Cliente_PlanoProduto> planos
        {
            get
            {
                if (pPlanos == null)
                {
                    pPlanos = new List<Cliente_PlanoProduto>();

                }
                return pPlanos;
            }
            set { pPlanos = value; }
        }
        public ClienteContrato clienteContrato { get; set; }
        public VariaveisModeloContrato item { get; set; }
        public Cliente_PlanoProduto cpp { get; set; }
        public Venda Venda { get; set; }
        public String? Observacao { get; set; }
        public TrocaTitularidade TrocaTitularidade { get; set; }

    }
    #region garbage
    //    public class TesteOptGroup<T,Y> : ObjComboOptGroup<T, Y> 
    //    { public List<ObjComboOptGroupResult<T, Y>> results { get; set; } }
    //    public class Contrato_Combo_AvulsoObjComboResult<T> : ObjComboResult<T>
    //    {
    //        public virtual long Id_Pai{get;set;}
    //}
    #endregion

}