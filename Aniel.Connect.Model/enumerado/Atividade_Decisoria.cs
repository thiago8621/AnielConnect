using System.ComponentModel.DataAnnotations;


namespace Aniel.Connect.Model.enumerado
{
    public enum Atividade_Decisoria
    {
        [Display(Name = "SIM", ResourceType = typeof(Resources.RsCadastros))]
        Sim = 0,
        [Display(Name = "NAO", ResourceType = typeof(Resources.RsCadastros))]
        Não = 1
    }

    public enum Tipo_Atividade_Decisoria
    {
        [Display(Name = "MANUAL", ResourceType = typeof(Resources.RsCadastros))]
        Sim = 0,
        [Display(Name = "AUTOMATICA", ResourceType = typeof(Resources.RsCadastros))]
        Não = 1
    }
}

