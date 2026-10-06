using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum PesquisaSatisfacaoDisparoAuto
    {
        [Display(Name = "DISPARO_MANUAL_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        Disparo_Manual_Cliente = 0,
        [Display(Name = "ENVIO_AUTOMATICO_ATENDIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        Envio_Automatico_Atendimento = 1,
        [Display(Name = "ENVIO_AUTOMATICO_SERVICO", ResourceType = typeof(Resources.RsCadastros))]
        Envio_Automatico_Servico = 2
    }
}
