using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Model.enumerado
{
    public enum EtapaIntegracaoIXC
    {

        [Display(Name = "A_INICIAR", ResourceType = (typeof(Resources.RsCadastros)))]
        A_Iniciar = 0,
        [Display(Name = "CLIENTE", ResourceType = (typeof(Resources.RsCadastros)))]
        Cliente = 1,
        [Display(Name = "CONTRATO", ResourceType = (typeof(Resources.RsCadastros)))]
        Contrato = 2,
        [Display(Name = "LOGIN", ResourceType = (typeof(Resources.RsCadastros)))]
        Login = 3,
        [Display(Name = "ATENDIMENTO", ResourceType = (typeof(Resources.RsCadastros)))]
        Atendimento = 4,
        [Display(Name ="CONCLUIDO", ResourceType = (typeof(Resources.RsCadastros)))]
        Concluido = 5
            
    }
}
