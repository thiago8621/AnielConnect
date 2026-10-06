using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Repository.Model
{
    //public class OltRequest 
    //{
    //    public virtual Int64 Id { get; set; }

    //}
    public class OltListModel
    {   public string CODIGO_AUXILIAR { get;  set; }
        public string VLAN_UPLINK { get;  set; }
        public long Id { get; set; }
        public string OLTID { get; set; }
        public string Descricao { get; set; }
        public string NumOLT { get; set; }
        public string Tipo { get; set; }

        [Required(ErrorMessage = "O campo ip é obrigatório.")]
        public string Ip { get; set; }
        public string Situacao { get; set; }
        public string USUARIOCRIACAO { get; set; }
        public DateTime DATAHORACRIACAO { get; set; }
        public string USUARIOALTERACAO { get; set; }
        public DateTime DATAHORAALTERACAO { get; set; }
        public string TOKEN { get; set; }

        [Required(ErrorMessage = "O campo NOME é obrigatório.")]
        public int? MARCA_MODELO { get; set; }
        public string NUM_OLT { get; set; }
        public string LOGIN { get; set; }
        public string SENHA { get; set; }
        public int? PORTA_TELNET { get; set; }
        public int? PORTA_API { get; set; }
        public int? PORTA_HTTP { get; set; }
        public int? PORTA { get; set; }
        public string IP_TL1 { get; set; }
        public string LOGIN_TL1 { get; set; }
        public string SENHA_TL1 { get; set; }
        public int? PORTA_TL1 { get; set; }
        public string GABINETE { get; set; }
        public string SUBRACK { get; set; }
        public int? STATUS { get; set; }
        public string DHCP_DNS_PRIMARIO { get; set; }
        public string DHCP_DNS_SECUNDARIO { get; set; }
        public string DHCP_LAN_IP { get; set; }
        public string DHCP_LAN_MASCARA { get; set; }
        public string DHCP_INICIO_RANGE { get; set; }
        public string DHCP_FINAL_RANGE { get; set; }
        public string TESTE_IP1 { get; set; }
        public string TESTE_IP2 { get; set; }

        [Required(ErrorMessage = "O campo TIPO_IDENTIFICACAO é obrigatório.")]
        public int? TIPO_IDENTIFICACAO { get; set; }
        public int? ID_FILIAL { get; set; }
        public string GERENCIA_REDEGENERICA { get; set; }
        public string GERENCIA_MASCARAREDEGENERICA { get; set; }
        public string GERENCIA_LOGIN { get; set; }
        public string GERENCIA_SENHA { get; set; }
        public string GERENCIA_PORTA_GENERICA { get; set; }
        public int? GERENCIA_VLAN { get; set; }
        public int? GERENCIA_SINALMINIMO { get; set; }
        public int? GERENCIA_SINALMAXIMO { get; set; }
        public int? ID_SERVIDOR_GERENCIA { get; set; }
        public int? ID_POP { get; set; }
        public int? UTILIZA_CONVERSAO_MAC { get; set; }
        public int? DELAY { get; set; }
        public List<OLT_GrupoAreaListModel> GrupoAreaLista { get; set; }
    }

    public class OLT_GrupoAreaListModel
    {
        public int Id_Area { get; set; }
        public string Cod_Area { get; set; }
        public string Desc_Area { get; set; }
        public string Cod_Escritorio { get; set; }
        public string Desc_Escritorio { get; set; }

    }
    [Serializable]
    public class OltRequest
    {
        public int ID { get; set; }

        public string USUARIOCRIACAO { get; set; }
        public DateTime DATAHORACRIACAO { get; set; }

        public string USUARIOALTERACAO { get; set; }
        public DateTime DATAHORAALTERACAO { get; set; }
        public string TOKEN { get; set; }
        public int OLTID { get; set; }
        [Required(ErrorMessage = "O campo NOME é obrigatório.")]

        public string DESCRICAO { get; set; }
        public int MARCA_MODELO { get; set; }
        public string NUM_OLT { get; set; }
        [Required(ErrorMessage = "O campo ip é obrigatório.")]
        public string IP { get; set; }
        public string LOGIN { get; set; }
        public string SENHA { get; set; }
        public int PORTA_TELNET { get; set; }
        public int PORTA_API { get; set; }
        public int PORTA_HTTP { get; set; }
        public int PORTA { get; set; }
        public string IP_TL1 { get; set; }
        public string LOGIN_TL1 { get; set; }
        public string SENHA_TL1 { get; set; }
        public int PORTA_TL1 { get; set; }
        public string GABINETE { get; set; }
        public string SUBRACK { get; set; }
        public int STATUS { get; set; }
        public string DHCP_DNS_PRIMARIO { get; set; }
        public string DHCP_DNS_SECUNDARIO { get; set; }
        public string DHCP_LAN_IP { get; set; }
        public string DHCP_LAN_MASCARA { get; set; }
        public string DHCP_INICIO_RANGE { get; set; }
        public string DHCP_FINAL_RANGE { get; set; }
        public string TESTE_IP1 { get; set; }
        public string TESTE_IP2 { get; set; }

        [Required(ErrorMessage = "O campo TIPO_IDENTIFICACAO é obrigatório.")]
        public int TIPO_IDENTIFICACAO { get; set; }
        [Required(ErrorMessage = "O campo ID_FILIAL é obrigatório.")]

        public int? ID_FILIAL { get; set; }
        public int TIPO { get; set; }
        public string GERENCIA_REDEGENERICA { get; set; }
        public string GERENCIA_MASCARAREDEGENERICA { get; set; }
        public string GERENCIA_LOGIN { get; set; }
        public string GERENCIA_SENHA { get; set; }
        public string GERENCIA_PORTA_GENERICA { get; set; }
        public int GERENCIA_VLAN { get; set; }
        public int GERENCIA_SINALMINIMO { get; set; }
        public int GERENCIA_SINALMAXIMO { get; set; }
        public int ID_SERVIDOR_GERENCIA { get; set; }
        public int ID_POP { get; set; }
        public int UTILIZA_CONVERSAO_MAC { get; set; }
        public string CODIGO_AUXILIAR { get; set; }
        public int VLAN_UPLINK { get; set; }
        public int DELAY { get; set; }
        public List<OLT_GrupoAreaListModel> GrupoAreaLista;
    }
    public class OLTModels 
    {
        public int ID { get; set; }

        public string USUARIOCRIACAO { get; set; }
        public DateTime DATAHORACRIACAO { get; set; }

        public string USUARIOALTERACAO { get; set; }
        public DateTime DATAHORAALTERACAO { get; set; }
        public string TOKEN { get; set; }
        public int OLTID { get; set; }
        [Required(ErrorMessage = "O campo NOME é obrigatório.")]

        public string DESCRICAO { get; set; }
        public int MARCA_MODELO { get; set; }
        public string NUM_OLT { get; set; }
        [Required(ErrorMessage = "O campo ip é obrigatório.")]
        public string IP { get; set; }
        public string LOGIN { get; set; }
        public string SENHA { get; set; }
        public int? PORTA_TELNET { get; set; }
        public int? PORTA_API { get; set; }
        public int? PORTA_HTTP { get; set; }
        public int? PORTA { get; set; }
        public string IP_TL1 { get; set; }
        public string LOGIN_TL1 { get; set; }
        public string SENHA_TL1 { get; set; }
        public int? PORTA_TL1 { get; set; }
        public string GABINETE { get; set; }
        public string SUBRACK { get; set; }
        public int? STATUS { get; set; }
        public string DHCP_DNS_PRIMARIO { get; set; }
        public string DHCP_DNS_SECUNDARIO { get; set; }
        public string DHCP_LAN_IP { get; set; }
        public string DHCP_LAN_MASCARA { get; set; }
        public string DHCP_INICIO_RANGE { get; set; }
        public string DHCP_FINAL_RANGE { get; set; }
        public string TESTE_IP1 { get; set; }
        public string TESTE_IP2 { get; set; }

        [Required(ErrorMessage = "O campo TIPO_IDENTIFICACAO é obrigatório.")]
        public int? TIPO_IDENTIFICACAO { get; set; }
        [Required(ErrorMessage = "O campo ID_FILIAL é obrigatório.")]

        public int? ID_FILIAL { get; set; }
        public int? TIPO { get; set; }
        public string GERENCIA_REDEGENERICA { get; set; }
        public string GERENCIA_MASCARAREDEGENERICA { get; set; }
        public string GERENCIA_LOGIN { get; set; }
        public string GERENCIA_SENHA { get; set; }
        public string GERENCIA_PORTA_GENERICA { get; set; }
        public int? GERENCIA_VLAN { get; set; }
        public int? GERENCIA_SINALMINIMO { get; set; }
        public int? GERENCIA_SINALMAXIMO { get; set; }
        public int? ID_SERVIDOR_GERENCIA { get; set; }
        public int? ID_POP { get; set; }
        public int? UTILIZA_CONVERSAO_MAC { get; set; }
        public string CODIGO_AUXILIAR { get; set; }
        public int? VLAN_UPLINK { get; set; }
        public int? DELAY { get; set; }
        public List<OLT_GrupoAreaListModel> GrupoAreaLista;
    }


}
