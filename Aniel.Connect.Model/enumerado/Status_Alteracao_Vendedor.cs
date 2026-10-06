using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Status_Alteracao_Vendedor
    {
        Aguadando_Aprovacao = 0,

        Aprovado = 1,

        Reprovado = 2,
    }
}
