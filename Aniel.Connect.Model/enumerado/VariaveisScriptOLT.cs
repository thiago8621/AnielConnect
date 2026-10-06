using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Model.enumerado
{
    public enum VariaveisScriptOLT
    {

        [Display(Name = "VAR_CONTRATO_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        Numero_Contrato,
        [Display(Name = "VAR_GABINETE", ResourceType = typeof(Resources.RsCadastros))]
        Gabinete,
        [Display(Name = "VAR_GEM_PORT", ResourceType = typeof(Resources.RsCadastros))]
        GemPort,
        
        [Display(Name = "VAR_IP_GERENCIA", ResourceType = typeof(Resources.RsCadastros))]
        IpGerencia,
        [Display(Name = "VAR_LOGIN_ONU_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        LoginOnuCliente,
        
        [Display(Name = "VAR_NOME", ResourceType = typeof(Resources.RsCadastros))]
        Nome,
        
        [Display(Name = "VAR_OLT_DOWNLOAD", ResourceType = typeof(Resources.RsCadastros))]
        OltDownload,
        [Display(Name = "VAR_OLT_UPLOAD", ResourceType = typeof(Resources.RsCadastros))]
        OltUpload,
        [Display(Name = "VAR_OLT_IP", ResourceType = typeof(Resources.RsCadastros))]
        OltIp,
        [Display(Name = "VAR_OLT_ID", ResourceType = typeof(Resources.RsCadastros))]
        OltId,
        
        [Display(Name = "VAR_ONU_MAC", ResourceType = typeof(Resources.RsCadastros))]
        OnuMac,
        [Display(Name = "VAR_ONU_TIPO", ResourceType = typeof(Resources.RsCadastros))]
        OnuTipo,

        [Display(Name ="VAR_PLANO_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        PlanoCliente,

        [Display(Name = "VAR_PON_ID", ResourceType = typeof(Resources.RsCadastros))]
        PonId,

        [Display(Name = "VAR_PORTA_TELNET_ONU", ResourceType = typeof(Resources.RsCadastros))]
        PortaTelnetOnuCliente,

        [Display(Name = "VAR_PPPOE_LOGIN", ResourceType = typeof(Resources.RsCadastros))]
        PppoeLogin,
        [Display(Name = "VAR_PPPOE_SENHA", ResourceType = typeof(Resources.RsCadastros))]
        PppoeSenha,

        [Display(Name = "VAR_ROUTER_LOGIN", ResourceType = typeof(Resources.RsCadastros))]
        RouterLogin,
        [Display(Name = "VAR_ROUTER_SENHA", ResourceType = typeof(Resources.RsCadastros))]
        RouterSenha,
        
        [Display(Name = "VAR_ROUTER_SENHA_WIFI", ResourceType = typeof(Resources.RsCadastros))]
        RouterSenhaWifi,

        [Display(Name = "VAR_ROUTER_SSID", ResourceType = typeof(Resources.RsCadastros))]
        RouterSSID,

        [Display(Name = "VAR_SENHA_ONU", ResourceType = typeof(Resources.RsCadastros))]
        SenhaOnuCliente,
        [Display(Name = "VAR_SENOR_ID", ResourceType = typeof(Resources.RsCadastros))]
        SenorId,
        [Display(Name = "VAR_SERVICE_PORT", ResourceType = typeof(Resources.RsCadastros))]
        ServicePort,
        [Display(Name = "VAR_SLOT_ID", ResourceType = typeof(Resources.RsCadastros))]
        SlotId,
        [Display(Name = "VAR_VLAN_UPLINK", ResourceType = typeof(Resources.RsCadastros))]
        VlanUplink,
        [Display(Name = "VAR_VLAN_LOGIN", ResourceType = typeof(Resources.RsCadastros))]
        VlanLogin,

        [Display(Name = "VAR_LINE_PROFILE", ResourceType = typeof(Resources.RsCadastros))]
        LineProfile,
        [Display(Name = "VAR_INTERFACE", ResourceType = typeof(Resources.RsCadastros))]
        Interface,

        [Display(Name = "VAR_ONT_ID", ResourceType = typeof(Resources.RsCadastros))]
        OntID,
        [Display(Name = "VAR_SUBRACK", ResourceType = typeof(Resources.RsCadastros))]
        Subrack,
        [Display(Name = "VAR_PORTA_HTTP_ONU", ResourceType = typeof(Resources.RsCadastros))]
        PortaHttpOnuCliente,
        [Display(Name = "VAR_VLAN_PROFILE", ResourceType = typeof(Resources.RsCadastros))]
        VlanProfile,
    }
}
