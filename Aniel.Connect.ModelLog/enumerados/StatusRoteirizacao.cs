using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.ModelLog
{
    public enum StatusRoteirizacao
    {
        [Display(Name = "CRIADO", ResourceType = typeof(Resources.RsCadastros))]
        Criado = 0,
        [Display(Name = "PROCESSANDO ", ResourceType = typeof(Resources.RsCadastros))]
        Processando = 1,
        [Display(Name = "PROCESSADO", ResourceType = typeof(Resources.RsCadastros))]
        Processado = 2,
        [Display(Name = "ERRO", ResourceType = typeof(Resources.RsCadastros))]
        Erro = 3
    }
}
