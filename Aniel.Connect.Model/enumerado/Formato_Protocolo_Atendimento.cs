using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Formato_Protocolo_Atendimento
    {

        [Display(Name = "SEQUENCIAL", ResourceType = typeof(Resources.RsCadastros))]
        Sequencial = 0,
        [Display(Name = "ANO_MES_DIA_HORA_SEQUENCIAL", ResourceType = typeof(Resources.RsCadastros))]
        Ano_Mes_Dia_Hora_Sequencial = 1,

    }
}
