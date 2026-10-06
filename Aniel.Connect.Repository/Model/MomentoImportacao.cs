using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Repository.Model
{
    public enum MomentoImportacao : int
    {
        [Display(Name = "Integração Atualização")]
        Integracao_Atualizacao = 0,
        [Display(Name = "Integração Manual")]
        Integracao_Manual,
        [Display(Name = "Integração Gravação")]
        Integracao_Gravacao,
    }
}
