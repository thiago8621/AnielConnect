using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Model.enumerado
{
    public enum IntegracaoExterna
    {
        [Display(Name = "SAPERX", ResourceType = typeof(Resources.RsCadastros))]
        SAPERX = 0,             
        [Display(Name = "COBLI", ResourceType = typeof(Resources.RsCadastros))]
        COBLI = 1,
        [Display(Name = "IXC", ResourceType = typeof(Resources.RsCadastros))]      
        IXC = 2,
        [Display(Name = "SERASA", ResourceType = typeof(Resources.RsCadastros))]
        SERASA = 3,
        [Display(Name = "MK_SOLUTION", ResourceType = typeof(Resources.RsCadastros))]
        MK_SOLUTION = 4,
        [Display(Name = "GEOSITE", ResourceType = typeof(Resources.RsCadastros))]
        GEOSITE = 5,
        [Display(Name = "PLAYHUB", ResourceType = typeof(Resources.RsCadastros))]
        PLAYHUB = 6,
        [Display(Name = "OZMAP", ResourceType = typeof(Resources.RsCadastros))]
        OZMAP = 7,
        [Display(Name = "VOALLE", ResourceType = typeof(Resources.RsCadastros))]
        VOALLE = 8,
        [Display(Name = "MOSTQI", ResourceType = typeof(Resources.RsCadastros))]
        MOSTQI = 9,
        [Display(Name = "PREQUAL", ResourceType = typeof(Resources.RsCadastros))]
        PREQUAL = 10,
        [Display(Name = "HUBSOFT", ResourceType = typeof(Resources.RsCadastros))]
        HUBSOFT = 11,
        [Display(Name = "IMANAGER", ResourceType = typeof(Resources.RsCadastros))]
        IMANAGER = 12,
        [Display(Name = "GEOGRID", ResourceType = typeof(Resources.RsCadastros))]
        GEOGRID = 13,
        [Display(Name = "SERASA_EXPERIAN", ResourceType = typeof(Resources.RsCadastros))]
        SERASA_EXPERIAN = 14,
		[Display(Name = "ALCANS", ResourceType = typeof(Resources.RsCadastros))]
        ALCANS = 15
    }
}
