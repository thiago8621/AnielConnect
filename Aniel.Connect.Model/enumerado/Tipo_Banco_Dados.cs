using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Tecnologia_Banco_Dados
    {

        [Display(Name = "MYSQL", ResourceType = typeof(Resources.RsCadastros))]
        Mysql = 0,
    }
}
