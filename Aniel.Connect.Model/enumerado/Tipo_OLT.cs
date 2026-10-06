using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Tipo_OLT
    {
        [Display(Name = "FIBERHOME", ResourceType = typeof(Resources.RsCadastros))]
        FiberHome = 0,
        Huawei = 1
    }
}
