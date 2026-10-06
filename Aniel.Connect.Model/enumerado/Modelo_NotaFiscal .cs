using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Aniel.Connect.Model.enumerado
{
    public enum Modelo_NotaFiscal : int
    {
        [Display(Name = "MODELO13", ResourceType = typeof(Resources.RsCadastros))]
        Modelo13 = 0,
        [Display(Name = "MODELO21", ResourceType = typeof(Resources.RsCadastros))]
        Modelo21 = 1,
        [Display(Name = "MODELO22", ResourceType = typeof(Resources.RsCadastros))]
        Modelo22 = 2,
        [Display(Name = "MODELO55", ResourceType = typeof(Resources.RsCadastros))]
        Modelo55 = 3,
        [Display(Name = "NOTA_FISCAL_DEBITO", ResourceType = typeof(Resources.RsCadastros))]
        NotaFiscalDebito = 4,
        [Display(Name = "NOTA_FISCAL_62", ResourceType = typeof(Resources.RsCadastros))]
        NotaFiscal62 = 5
    }
}
