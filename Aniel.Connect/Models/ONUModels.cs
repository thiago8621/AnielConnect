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
    public class VinculoONUCliente : IViewModel<long>
    {
        public virtual long Id { get; set; }
        public virtual long? Id_Pon { get; set; }
        public virtual string DescricaoPon { get; set; }
        public virtual string Mac { get; set; }
        public virtual string Serial { get; set; }
        public virtual string TipoONU { get; set; }
        public virtual string PONID { get; set; }

        public virtual string CodCt { get; set; }
        public virtual string Projeto { get; set; }
        public virtual int? Num_Doc { get; set; }
        public virtual string Num_Obra { get; set; }

        public virtual string Equipe { get; set; }

        public virtual long? Id_Cliente { get; set; }
        public virtual long? Id_PlanoProduto { get; set; }
        public virtual long? Id_Cliente_PlanoProduto { get; set; }

        public virtual string CodMat { get; set; }
        public virtual string CodCpl { get; set; }

        public virtual Tipo_Conexao_ONU Tipo_Conexao { get; set; }
        public virtual string Login { get; set; }
        public virtual string Senha { get; set; }

        public virtual string LoginAdmin { get; set; }
        public virtual string SenhaAdmin { get; set; }

        public virtual int PortaVLan { get; set; }

        public virtual string Wireless_SSID { get; set; }
        public virtual string Senha_WPA2 { get; set; }

        public virtual string Estatico_Nro_Ip { get; set; }
        public virtual string Estatico_Ip_Mask { get; set; }
        public virtual string Estatico_Gatway { get; set; }

        public virtual string Nro_Ip { get; set; }
        public virtual string Ip_Inicio { get; set; }
        public virtual string Ip_Final { get; set; }
        public virtual string Ip_Mask { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.RsBase), ErrorMessageResourceName = "REQUIRED")]
        [Display(Name = "TIPO_IDENTIFICACAO", ResourceType = typeof(Resources.RsCadastros))]
        public virtual ONU_Tipo_Identificacao? Tipo_Identificacao { get; set; }
    }

    public class AtivacaoONU
    {
        public virtual string CodCt { get; set; }
        public virtual string Projeto { get; set; }
        public virtual int Num_Doc { get; set; }
        public virtual string Num_Obra { get; set; }

        public virtual string CodMat { get; set; }
        public virtual string CodCpl { get; set; }
        public virtual string DescricaoMat { get; set; }

        public virtual long Id_Cliente_Plano_Produto { get; set; }
    }

    public class TesteCompleto
    {
        public Int64 IdClientePlanoProduto { get; set; }
    }

    public class AtivaEquipamento
    {
        public virtual long IdOLT { get; set; }
        public virtual List <long> IdsOLts {get; set;}
        public virtual long IdPON { get; set; }
        public virtual long IdONU { get; set; }
        public virtual long NumSlot { get; set; }
        public virtual long NumPon { get; set; }
        public virtual long OntId { get; set; }
        public virtual string Usuario { get; set; }
        public virtual string Detalhe { get; set; }
        public virtual string CodMat { get; set; }
        public virtual string CodCpl { get; set; }

        public virtual string ScriptOLT { get; set; }
        public virtual string ScriptONU { get; set; }
        public virtual bool Origem_Mobile { get; set; }
        public virtual bool Origem_Web { get; set; }
        public virtual string Serial { get; set; }
        public virtual string Serial_Convertido { get; set; }
    }

    public class DesautorizarONUEmMassaRequest
    {
        public List<long> ListIdONU { get; set; }
    }

    public class ReiniciarONUEmMassaRequest
    {
        public List<long> ListIdONU { get; set; }
    }

    public class GetIdONURequest
    {
        //public Int64 Id_Cliente_Plano_Produto { get; set; }
        public String Mac { get; set; }
        public String serialConvertido { get; set; }
    }

    public class AtualizarInfoWifiRequest
    {
        public Int64 Id_Cliente_Plano_Produto { get; set; }
        public String SSID_Wifi { get; set; }
        public String Senha_Wifi { get; set; }
    }

    public class LimparMacRequest
    {
        public String Num_Obra { get; set; }
        public String Usuario { get; set; }
    }

    public class EstruturaFibraDataTableRequest : DataTableRequest
    {
        public long? Id_Empresa { get; set; }
        public long? Id_Filial { get; set; }
        public long? Id_OLT { get; set; }
        public long? Id_SLOT { get; set; }
        public long? Id_PON { get; set; }
    }


    [ExcelServiceKey("Serial")]
    [ExcelServiceSheetName("ONU")]
    public class ONU_Importacao : IExcelService
    {
        [Required]
        [ExcelService("OLT")]
        [MaxLength(100)]
        public virtual String Descricao_OLT { get; set; }

        [Required]
        [ExcelService("NUM_SLOT")]
        public virtual Int32? Num_Slot { get; set; }

        [Required]
        [ExcelService("NUM_PON")]
        public virtual Int32? Num_Pon { get; set; }
       
        [ExcelService("ONT_ID")]
        [MaxLength(8)]
        public virtual String ONTID { get; set; }

        [MaxLength(100)]
        [ExcelService("DESCRICAO_ONT")]
        public virtual String Descricao_ONT { get; set; }

        [Required]
        [MaxLength(100)]
        [ExcelService("SERIAL_EQUIPAMENTO")]
        public virtual String Serial { get; set; }

        [MaxLength(20)]
        [ExcelService("MAC_EQUIPAMENTO")]
        public virtual String MAC { get; set; }

        [Required]
        [MaxLength(255)]
        [ExcelService("LOGIN_RADIUS")]
        public virtual String Login_Radius { get; set; }

        //[MaxLength(255)]
        //[ExcelService("SENHA_RADIUS")]
        //public virtual String Senha_Radius { get; set; }

        [ExcelService("ID_PERFIL_ATIVACAO")]
        public virtual Int64? Id_Perfil_Ativacao { get; set; }

        //[ExcelService("ID_CAIXA_FTTH")]        
        //public virtual Int64? Id_Caixa_FTTH { get; set; }        

        //[ExcelService("PORTA_FTTH")]        
        //public virtual Int32? Porta_FTTH { get; set; }

        [ExcelService("GEM_PORT")]
        public virtual Int32? Gem_Port { get; set; }

        [ExcelService("SERVICE_PORT")]
        public virtual Int32? Service_Port { get; set; }

        [ExcelService("SENOR_ID")]
        public virtual Int32? Senor_Id { get; set; }

        [ExcelService("PORTA_TELNET")]
        public virtual Int32? Porta_Telnet { get; set; }

        [ExcelService("PORTA_HTTP")]
        public virtual Int32? Porta_Http { get; set; }

        [ExcelService("VLAN_UPLINK")]
        public virtual Int16? Vlan_Uplink { get; set; }

        [ExcelService("VLAN_PPPOE")]
        public virtual Int16? Vlan_PPPOE { get; set; }

        [ExcelService("VLAN_DHCP")]
        public virtual Int16? Vlan_DHCP { get; set; }

        [ExcelService("VLAN_IPTV")]
        public virtual Int16? Vlan_IPTV { get; set; }

        [ExcelService("VLAN_VOIP")]
        public virtual Int16? Vlan_VOIP { get; set; }

        [ExcelService("VLAN_TR69")]
        public virtual Int16? Vlan_TR69 { get; set; }

        [ExcelService("VLAN_OUTROS")]
        public virtual Int16? Vlan_Outros { get; set; }

        [ExcelService("VLAN_PROFILE")]
        [MaxLength(255)]
        public virtual String Vlan_Profile { get; set; }

        [ExcelService("GERAR_INV_MATERIAL")]
        [MaxLength(1)]
        public virtual String Gerar_Inventario_Mat { get; set; }

        [ExcelService("CODMAT")]
        [MaxLength(12)]
        public virtual String CodMat { get; set; }

        public Boolean Importar { get; set; }

        public List<ExcelServiceLogItem> validaMaterial()
        {
            List<ExcelServiceLogItem> validacoes = new List<ExcelServiceLogItem>();
            if (!String.IsNullOrEmpty(Gerar_Inventario_Mat) && Gerar_Inventario_Mat.ToUpperInvariant().Equals("S"))
            {
                if (String.IsNullOrEmpty(CodMat))
                {
                    validacoes.Add(new ExcelServiceLogItem { Chave = Serial, Erro = "Código do Material é obrigatório pois foi preenchida a opção de gerar inventário para o material." });
                }
            }
            return validacoes;
        }
    }

    public class LogImportacaoONURequest : DataTableRequest
    {
        public string Data { get; set; }
    }


    public class OnuExistenteModel
    {
        public bool MesmoSerial { get; set; }
        public bool MesmoLogin { get; set; }
    }


    public class ImportacaoONUDadosLoginModel
    {
        public string Login { get; set; }
        public long Id { get; set; }
        public long? Id_Cliente_PlanoProduto { get; set; }
        public string CpfCnpj_Cliente{ get; set; }
    }


    [ExcelServiceKey("Serial")]
    [ExcelServiceSheetName("ExclusaoONU")]
    public class ONU_Exclusao : IExcelService
    {

        [Required]
        [MaxLength(100)]
        [ExcelService("SERIAL_EQUIPAMENTO")]
        public virtual String Serial { get; set; }

        public Boolean Importar { get; set; }

    }

    public class ExclusaoONUDadosEquipamento
    {
        public string Login { get; set; }
        public long Id { get; set; }
        public string Serial { get; set; }
        public string NumSlot { get; set; }
        public string NumPon { get; set; }
        public string DescOLT { get; set; }
        public string Ont_Id {  get; set; }

    }

    public class LogExclusaoONURequest : DataTableRequest
    {
        public string Data { get; set; }
    }

    public class ExcluirONURegistroRequest
    {
        public string id { get; set; }
    }

}