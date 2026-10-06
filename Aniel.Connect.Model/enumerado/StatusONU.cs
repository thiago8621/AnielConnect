using Resources;
using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Model.enumerado
{
    public enum StatusONU
    {
        [Display(Name = "ONLINE", ResourceType = typeof(RsCadastros))]
        Online,
        [Display(Name = "OFFLINE", ResourceType = typeof(RsCadastros))]
        Offline,
        [Display(Name = "NAO_DISPONIVEL", ResourceType = typeof(RsCadastros))]
        Indisponivel,
        }
}
