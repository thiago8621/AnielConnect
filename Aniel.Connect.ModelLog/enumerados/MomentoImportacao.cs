using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.ModelLog.enumerados
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
