using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Newtonsoft.Json;
using Aniel.Connect.Model.nhibernate;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using Sinapse.Util.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Models
{
   
    public class OLTListaModel
    {
        public virtual Int64 Id { get; set; }
        public virtual Int64? Delay { get; set; }
        public virtual String OLTID { get; set; }
        public virtual String Descricao { get; set; }
        public String NumOLT { get; set; }
        public virtual String Tipo { get; set; }

        //Conexao Comunicacao SSH ********************************************************************     
        public virtual String Ip { get; set; }

        public virtual String Situacao { get; set; }

        public virtual Int64 Id_Grupo { get; set; }

        public virtual Int64 Id_Filial { get; set; }

        public virtual Int64 Id_Empresa { get; set; }

        public virtual Status_Default? Status { get; set; }
        public virtual Marca_Modelo_OLT? Marca_Modelo { get; set; }

    }

    public class OLTCadastroModel
    {
        public Int64? Delay { get; set; }
        public Int64 Id { get; set; }
        public String CodigoAuxiliar { get; set; }
        public String NumOLT { get; set; }
        public String OLTID { get; set; }

        public Int64 IdPOP { get; set; }

        public Int32? PortaTelnet { get; set; }
        public Int32? PortaAPI { get; set; }
        public Int32? PortaHTTP { get; set; }
        public Int32? PortaSSH { get; set; }


        public String Descricao { get; set; }

        public Marca_Modelo_OLT Marca_Modelo { get; set; }
        public Versao_OLT? Versao_OLT { get; set; }
       
        public String Ip { get; set; }

        public String Login { get; set; }

        public String Senha { get; set; }
        public string LimitePotenciaONUBaixa { get; set; }

        public Boolean Status { get; set; }
        public Boolean ConverteOlt { get; set; }
        public Int16? VLAN_Uplink { get; set; }
        public bool Config_PPPOE_Manual { get; set; }

        #region TL1

        public String IpTL1 { get; set; }
        public String LoginTL1 { get; set; }
        public String SenhaTL1 { get; set; }
        public Int32? PortaTL1 { get; set; }
        public Int32? PortaTL1_SSH { get; set; }
        public String Gabinete { get; set; }
        public String Subrack { get; set; }

        #endregion

        public ONU_Tipo_Identificacao? Tipo_Identificacao { get; set; }
        public virtual string Cod_Escritorio { get; set; }
        public virtual string Cod_Area { get; set; }
        private List<OLT_Grupo_AreaListModel> GrupoAreaLista;        
        public virtual List<OLT_Grupo_AreaListModel> GruposArea
        {
            get
            {
                if (GrupoAreaLista == null)
                    GrupoAreaLista = new List<OLT_Grupo_AreaListModel>();
                return GrupoAreaLista;
            }
            set
            {
                GrupoAreaLista = value;
            }
        }
        
    }
     [ExcelServiceKey("Id")]
    [ExcelServiceSheetName("OLT")]
    public class OLTImportacao: IExcelService
    {
        [ExcelService("Id")]
        public Int64? Id { get; set; }

        [ExcelService("CodigoAuxiliar")]
        public String CodigoAuxiliar { get; set; }

        [ExcelService("NumOLT")]
        public String NumOLT { get; set; }

        [ExcelService("OLTID")]
        public String OLTID { get; set; }

        [ExcelService("IdPOP")]
        public Int64 IdPOP { get; set; }

        [ExcelService("PortaTelnet")]
        public Int32? PortaTelnet { get; set; }

        [ExcelService("PortaAPI")]
        public Int32? PortaAPI { get; set; }

        [ExcelService("PortaHTTP")]
        public Int32? PortaHTTP { get; set; }

        [ExcelService("PortaSSH")]
        public Int32? PortaSSH { get; set; }
        [Required]      
        [ExcelService("Descricao")]
        public String Descricao { get; set; }

        [Required]
        [ExcelService("Marca_Modelo")]
        public int Marca_Modelo { get; set; }
         [Required]
        [ExcelService("Ip")]
        public String Ip { get; set; }
         [Required]
        [ExcelService("Login")]
        public String Login { get; set; }
         [Required]
        [ExcelService("Senha")]
        public String Senha { get; set; }

        [ExcelService("Status")]
        public Boolean Status { get; set; }

        [ExcelService("ConverteOlt")]
        public Boolean ConverteOlt { get; set; }

        [ExcelService("VLAN_Uplink")]
        public Int16? VLAN_Uplink { get; set; }

        #region TL1
        [ExcelService("IpTL1")]
        public String IpTL1 { get; set; }

        [ExcelService("LoginTL1")]
        public String LoginTL1 { get; set; }

        [ExcelService("SenhaTL1")]
        public String SenhaTL1 { get; set; }

        [ExcelService("PortaTL1")]
        public Int32? PortaTL1 { get; set; }

        [ExcelService("Gabinete")]
        public String Gabinete { get; set; }

        [ExcelService("Subrack")]
        public String Subrack { get; set; }

        #endregion
         [Required]
        [ExcelService("Tipo_Identificacao")]
        public Int32 Tipo_Identificacao { get; set; }

        [ExcelService("Cod_Escritorio")]
        public virtual string Cod_Escritorio { get; set; }

        [ExcelService("Cod_Area")]
        public virtual string Cod_Area { get; set; }
        public bool Importar { get; set; }
    }
    public class GetOLTApiResponse : OLTCadastroModel
    {
        public virtual List<PON_ModelList> ListaPON { get; set; }
        public virtual bool CriticarBaixaPotenciaONU { get; set; }
        public virtual bool CriticarBaixaLoginRadiusOffline { get; set; }
    }

    public class TestarConexaoRequest
    {
        public string IP { get; set; }
        public string Login { get; set; }
        public string Senha { get; set; }
        public int? PortaSsh { get; set; }
        public int? PortaTelnet { get; set; }
        public string IPTL1 { get; set; }
        public string UsuarioTL1 { get; set; }
        public string SenhaTL1 { get; set; }
        public int? PortaTL1 { get; set; }
    }

    public class TestarConexaoRetorno
    {
        public string Protocolo { get; set; }
        public bool Conectado { get; set; }
        public bool Erro { get; set; }
    }

    public class OLTComboResult : IObjComboResult<Int64>
    {
        public Int64 Id { get; set; }
        public String Text { get; set; }
        public String Ip { get; set; }
        public String Gabinete { get; set; }
        public String Subrack { get; set; }
        public String NumOlt { get; set; }
    }

    public class OLTTreeViewModel
    {
        public virtual Int64 Id { get; set; }
        public virtual String Descricao { get; set; }
    }

    public class OLT_DataTableRequest : DataTableRequest
    {
        public String Id_Olt { get; set; }
        public String Id_Empresa { get; set; }
        public String Id_Filial { get; set; }
        public String Status_Filtro { get; set; }
        public String Tipo_Olt { get; set; }

        public String Ip { get; set; }
    }

    public class CriarSlotsEPONsEmMassa_Request
    {
        public virtual Int64? Id_OLT { get; set; }
        public virtual Int64 Num_Slot_Inicial { get; set; }
        public virtual Int64? Num_Slot_Fim { get; set; }
        public virtual Int64 Num_PON_Inicial { get; set; }
        public virtual Int64? Num_PON_Fim { get; set; }
        public Int16? Vlan_PON_Inicial { get; set; }
        public Int16? Vlan_PON_Incremento { get; set; }
    }

    public class GetDelayOLT
    {
        public GetDelayOLT(long delay)
        {
            Delay = delay;
        }

        public virtual Int64? Delay { get; set; }

        public string IP { get; set; }
    }
}