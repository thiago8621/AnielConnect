using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Aniel.Connect.Model.enumerado
{
    public enum Renova_FidelidadeEnum
    {
        [Display(Name = "NAO_COM_ALTERACAO", ResourceType = typeof(Resources.RsCadastros))]
        Nao_com_Alteracao = 0,

        [Display(Name = "NAO_SEM_ALTERACAO", ResourceType = typeof(Resources.RsCadastros))]
        Nao_sem_Alteracao = 1,

        [Display(Name = "SIM", ResourceType = typeof(Resources.RsCadastros))]
        Sim = 2
    }
}
