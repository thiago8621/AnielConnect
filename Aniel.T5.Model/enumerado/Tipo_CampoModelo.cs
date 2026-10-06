using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Resources;

namespace Aniel.T5.Model.enumerado
{
    public enum Tipo_CampoModelo
    {
        [Display(Name ="TEXTO", ResourceType = typeof(RsCadastros))]
        Texto,
        [Display(Name = "INTEIRO" ,ResourceType = typeof(RsCadastros))] 
        Inteiro,
        [Display(Name ="DATA", ResourceType = typeof(RsCadastros))]
        Data,
        [Display(Name = "NUMERO", ResourceType = typeof(RsCadastros))] 
        Numero,
        [Display(Name = "HORA", ResourceType = typeof(RsCadastros))] 
        Hora,
        [Display(Name = "LISTA", ResourceType = typeof(RsCadastros))] 
        Lista
    }
}
