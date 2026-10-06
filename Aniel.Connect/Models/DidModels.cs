using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using Sinapse.Util;
using Sinapse.Util.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace Aniel.Connect.Models
{

    public class DidListaModel
    {
        public virtual String Id { get; set; }
        public virtual String Numero { get; set; }
        public virtual String Pais { get; set; }
        public virtual String Uf { get; set; }
        public virtual String Cidade { get; set; }
        public virtual String Status { get; set; }
        public virtual String Reservado_DDR { get; set; }

    }
    /// <summary>
    /// Modelo de Requisicao para a Api GetRandomDid
    /// </summary>
    /// <param>Id do Pais</param>   <param> String da Uf</param>   <param> Id da cidade</param> <param>Numero de Dids</param>
    public class Did_Random_ApiRequest
    {
        public virtual long? NumeroDids { get; set; }
        public virtual String Id_Pais { get; set; }
        public virtual String UF { get; set; }
        public virtual String Id_Cidade { get; set; }
    }

    [ExcelServiceKey("Numero")]
    [ExcelServiceSheetName("Did")]

    public class Did_PlanilhaImportacao : IExcelService
    {
        [ExcelService("TIPO DE DISCAGEM")]
        public virtual TipoDiscagem TipoDiscagem { get; set; }
        [ExcelService("NUMERO DE TELEFONE")]
        public virtual String Numero { get; set; }
        [ExcelService("STATUS")]
        public virtual StatusDid StatusDid { get; set; }
        [ExcelService("DIA DE LIBERACAO")]
        public virtual DateTime? DiaLiberado { get; set; }
        [ExcelService("VALOR A COBRAR")]
        public virtual Double? Valor { get; set; }

        [ExcelService("COBRAR CHAMADA?")]
        public virtual bool ChamadaCobrar { get; set; }
        [ExcelService("CIDADE")]
        public virtual String Cidade { get; set; }
        [ExcelService("UF")]
        public virtual String UF { get; set; }
        [ExcelService("PAIS")]
        public virtual String Pais { get; set; }
        [ExcelService("ID DA VENDA")]
        public virtual Int64? IdVenda { get; set; }
        [ExcelService("MOTIVO")]
        //public virtual RotaTelefonia RotaTelefonia { get; set; }
        public virtual String MotivoIndisponivel { get; set; }

        public Boolean Importar { get; set; }
    }
    public class DidCadastroModel : IViewModel<Int64>
    {
        public virtual Int64 Id { get; set; }

        //[Display(Name = "CODIGO", ResourceType = typeof(Resources.RsFinanceiro))]
        //[Required(ErrorMessageResourceType = typeof(Resources.RsBase),
        //          ErrorMessageResourceName = "REQUIRED")]
        //[MaxLength(5, ErrorMessageResourceType = typeof(Resources.RsBase),
        //                ErrorMessageResourceName = "MAXLENGHT")]
        //public virtual String Codigo { get; set; }


        [Display(Name = "NUMERO", ResourceType = typeof(Resources.RsFinanceiro))]
        public virtual String Numero { get; set; }

        [Display(Name = "STATUS", ResourceType = typeof(Resources.RsFinanceiro))]
        [Required(ErrorMessageResourceType = typeof(Resources.RsBase),
                      ErrorMessageResourceName = "REQUIRED")]
        public virtual StatusDid StatusDid { get; set; }
        public virtual String DiaLiberado { get; set; }
        public virtual String Valor { get; set; }
        public virtual String MotivoIndisponivel { get; set; }
        public virtual long? Id_Venda { get; set; }
        public virtual bool ChamadaCobrar { get; set; }
        public virtual String UF { get; set; }
        public virtual Int64? IdPais { get; set; }
        public virtual Int64? IdRota { get; set; }
        public virtual TipoDiscagem TipoDiscagem { get; set; }
        public virtual Int64? IdCidade { get; set; }
        public virtual Padrao Reservado_Ddr { get; set; }

    }
    public class Did_CadastroIntervaloModel : DidCadastroModel
    {
        //public virtual List<String> Numeros { get; set; }
        //[Required(ErrorMessageResourceType = typeof(Resources.RsBase),ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "PREFIXO_NUMERO_INICIAL", ResourceType = typeof(Resources.RsCadastros))]
        public virtual String Numero_Inicial { get; set; }

        //[Required(ErrorMessageResourceType = typeof(Resources.RsBase),ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "PREFIXO_NUMERO_FINAL", ResourceType = typeof(Resources.RsCadastros))]
        public virtual String Numero_Final { get; set; }
        //[Required(ErrorMessageResourceType = typeof(Resources.RsBase), ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "PREFIXO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual String Prefixo { get; set; }
        private List<Did_LogMensagem> Log;
        public virtual List<Did_LogMensagem> Log_DidIntervalo
        {
            get
            {
                Log = Log != null ? Log : new List<Did_LogMensagem>();
                return Log;
            }
            set
            {
                Log = value;
            }
        }

    }
    public class Did_LogMensagem
    {
        public virtual String Numero { get; set; }
        public virtual String Mensagem { get; set; }
    }

    public class DidComboResult : ObjComboResult<long>
    {
        public string Valor { get; set; }
    }

    public class Did_DataTableRequest : DataTableRequest
    {
        public virtual String Status { get; set; }
        public virtual String Numero { get; set; }
        public virtual String Pais { get; set; }
        public virtual String Cidade { get; set; }
        public virtual String Uf { get; set; }

    }

    public class Relatorio_Historico_ChamadaModel
    {
        public virtual String numeroCircuito { get; set; }
        public virtual String data { get; set; }
        public virtual String didOrigem { get; set; }
        public virtual String didDestino { get; set; }
        public virtual String sentido { get; set; }
        public virtual String horaInicio { get; set; }
        public virtual String horaFim { get; set; }
        public virtual String duracao { get; set; }
        public virtual String custoLigacao { get; set; }
        public virtual String cidade { get; set; }

    }

    public class Relatorio_Historico_ChamadaRequest : DataTableRequest
    {
        public virtual String dataini { get; set; }
        public virtual String datafim { get; set; }
        public virtual String id_did { get; set; }
        public virtual String id_didOrigem { get; set; }
        public virtual String id_didDestino { get; set; }
        public virtual Int64? idClientePlanoProduto { get; set; }


    }

    public class CircuitoDidPlano : DataTableRequest
    {
        public int Id_did { get; set; }
        public string Mensalidade { get; set; }
        public int NumeroDid { get; set; }
        public bool LC { get; set; }
        public bool LDN { get; set; }
        public bool VC1 { get; set; }
        public bool VC2 { get; set; }
        public bool VC3 { get; set; }
        public bool Bina { get; set; }
        public bool Blacklist { get; set; }
        public bool SigaMe { get; set; }
        public int idClientePlanoProduto { get; set; }
        public int IdPlano { get; set; }

    }
    public class CircuitoDidPlanoListaModel
    {
        public int Id_did { get; set; }
        public string Mensalidade { get; set; }
        public int NumeroDid { get; set; }
        public bool LC { get; set; }
        public bool LDN { get; set; }
        public bool VC1 { get; set; }
        public bool VC2 { get; set; }
        public bool VC3 { get; set; }
        public bool Bina { get; set; }
        public bool Blacklist { get; set; }
        public bool SigaMe { get; set; }
        public int idClientePlanoProduto { get; set; }
    }
}