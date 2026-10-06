using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Util.Excel;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Models
{
    public class LoginRadiusListaModel
    {
        public long id { get; set; }
        public string cliente { get; set; }
        public string login { get; set; }
        public string status { get; set; }
        public string pop { get; set; }
        public string olt { get; set; }
        public string slot { get; set; }
        public string pon { get; set; }
        public string caixa_FTTH { get; set; }
        public string porta_FTTH { get; set; }        
        public long? idClientePlanoProduto { get; set; }
        public long? idClienteContrato { get; set; }
        public string Observacao_Motivo_Suspensao_Plano_Login_Outros { get; set; }
    }

    public class LoginRadiusCadastroModel
    {
        public Int64 Id { get; set; }
        public String Login { get; set; }
        public String Situacao { get; set; }
        public Boolean Status { get; set; }
        public Int64? Id_PlanoProduto { get; set; }
        public String EnderecoIPV4 { get; set; }
        public String EnderecoIPV6 { get; set; }
        public String SenhaPPPOE { get; set; }
        public String UsuarioRoteador { get; set; }
        public String SenhaRoteador { get; set; }
        public String AgentCircuit_ID { get; set; }
        public Int64? Atributo_IPOE { get; set; }
        public String SSID_WIFI { get; set; }
        public String Senha_WIFI { get; set; }
        public String DataHora_UltimaConexao { get; set; }
        public String TotalHorasConectado { get; set; }
        public String TotalDownload { get; set; }
        public String TotalUpload { get; set; }
        public String Contrato { get; set; }
        public Tipo_Autenticacao_Login? Tipo_Autenticacao_Login { get; set; }

        #region informacoes tecnicas
        public Int64? Id_POP { get; set; }
        public Int64? Id_Concentradora { get; set; }
        public Int64? Id_OLT { get; set; }
        public Int64? Id_Cliente { get; set; }
        public Int64? Id_Slot { get; set; }
        public Int64? Id_PON { get; set; }
        public Int64? Id_Caixa_FTTH { get; set; }
        public Int64? Id_Porta_FTTH { get; set; }
        public String VLAN { get; set; }
        public String Serial { get; set; }
        public String MAC { get; set; }
        public Int64? Id_Pool { get; set; }
        public Int64? Id_Pool_Ip_Endereco { get; set; }
        public String Observacoes {  get; set; }
        #endregion

        public bool IgnorarCriticaCaixaPorta { get; set; }
    }

    public class HistoricoConsumoRadiusModel
    {
        public DateTime Data_Inicio { get; set; }
        public DateTime Data_Fim { get; set; }
        public string Tempo_Uso { get; set; }
        public string MacUser { get; set; }
        public string IpUser { get; set; }
        public string Trafego_Total { get; set; }
        public string Trafego_Download { get; set; }
        public string Trafego_Upload { get; set; }
    }
        public class LoginPlanoProduto
    {
        public long idLogin { get; set; }
        public long idPlanoProduto { get; set; }
    }

    public class LoginRadiusAlterarStatus
    {
        public List<LoginPlanoProduto> ListLoginPlanoProduto { get; set; }
        public Status_Default Status { get; set; }
    }

    public class LoginRadiusDatatableRequest : DataTableRequest
    {
        public Int64? IdEmpresa { get; set; }
        public Int64? IdFilial { get; set; }

        public String CodCt { get; set; }
        public String Projeto { get; set; }

        public String UF { get; set; }
        public Int64? IdCidade { get; set; }
        public Int64? IdBairro { get; set; }
        public String Id_Olt { get; set; }
        public String Id_Cliente { get; set; }
        public String Id_ClientePlanoProduto { get; set; }
    }

    public class HistoricoAlteracaoLogin_DataTableRequest : DataTableRequest
    {
        public virtual Int64? Id { get; set; }
        public virtual String DataIni { get; set; }
        public virtual String DataFim { get; set; }
        public virtual String Objeto { get; set; }
        public virtual String TipoOperacao { get; set; }
        public virtual String Campo { get; set; }
    }

    [ExcelServiceKey("Login")]
    [ExcelServiceSheetName("Atualiza_Logins")]
    public class AtualizacaoLogins : IExcelService
    {
  
         [Required]
         [ExcelService("LOGIN")]
         [MaxLength(100)]
         public virtual String Login { get; set; }

        [Required]
        [ExcelService("SENHA")]
        [MaxLength(100)]
        public virtual String SenhaPPPOE { get; set; }

        [Required]
        [ExcelService("CONCENTRADORA")]
        [MaxLength(100)]
        public virtual String Concentradora { get; set; }

        [ExcelService("POOL_DE_IP")]
        [MaxLength(100)]
        public virtual String Pool { get; set; }

        [ExcelService("IP_FIXO")]
        [MaxLength(100)]
        public virtual String IP { get; set; }
       
        public Boolean Importar { get; set; }

          
        }

    public class LogAtualizacaoLoginsRequest : DataTableRequest
    {
        public string Data { get; set; }
    }

    //Utilizado para as validações do método responsável por
    //atualizar logins em massa
    public class GetDadosLoginAtual
    {
        public Int64 Id {get; set;}
        public String Login { get; set;}
        public String SenhaPPPOE { get;set;}
        public String Concentradora { get;set;}
        public Int64? Id_Concentradora { get; set;}
        public Int64 Id_Plano_Produto { get; set;}
        public Int64? Id_Pool_Ip_Endereco { get; set;}
        public Int64? Id_Pool { get; set;}
        public String Descricao_Pool { get; set;}
        public Boolean IP_Fixo { get; set;}
        public String IP {  get; set;}

    }

    public class GetDadosConcentradora
    {
        public Int64 Id { get; set; }
        public String Nome { get; set; }

    }

    public class GetDadosPoolIp
    {
        public Int64 Id { get; set; }
        public String Descricao_Pool { get;set;}
        public Boolean IP_Fixo { get;set;}
        public Boolean Status { get; set;}
     }

    public class LoginsRadiusEmMassaSemFiltroRequest
    {
        public virtual long? IdParou { get; set; }
    }
}
