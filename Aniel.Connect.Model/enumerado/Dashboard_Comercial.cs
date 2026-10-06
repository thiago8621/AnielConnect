using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Model.enumerado
{

    public enum Dashboard_Comercial
    {
        [Display(Name = "META_CONFIG", ResourceType = typeof(Resources.RsCadastros))]
        Minha_Meta = 0,
        [Display(Name = "VENDAS_APROVADAS", ResourceType = typeof(Resources.RsCadastros))]
        Vendas_Aprovadas = 1,        
        [Display(Name = "VENDAS_PENDENTES", ResourceType = typeof(Resources.RsCadastros))]
        Vendas_Pendentes = 12,
        [Display(Name = "DESISTENCIAS", ResourceType = typeof(Resources.RsCadastros))]
        Desistencias = 13,
        [Display(Name = "VENDAS_INSTALADAS", ResourceType = typeof(Resources.RsCadastros))]
        Vendas_Instaladas = 14,
        [Display(Name = "VENDAS_STATUS", ResourceType = typeof(Resources.RsCadastros))]
        Vendas_Status = 15,
        [Display(Name = "FUNIL_VENDAS", ResourceType = typeof(Resources.RsCadastros))]
        Funil_Vendas = 16
    }
}