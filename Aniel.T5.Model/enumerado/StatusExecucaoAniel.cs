using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.enumerado
{
    public enum StatusExecucaoAniel
    {
        [Display(Name = "ABERTA_AGUARD_ATENDIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        Aberta_Aguard_Atendimento = 0,

        [Display(Name = "FECHADA_IMPRODUTIVA", ResourceType = typeof(Resources.RsCadastros))]
        Fechada_Improdutiva = 1,

        [Display(Name = "FECHADA_PRODUTIVA", ResourceType = typeof(Resources.RsCadastros))]
        Fechada_Produtiva = 2,

        [Display(Name = "ATENDIMENTO_INICIADO_REINICIADO", ResourceType = typeof(Resources.RsCadastros))]
        Atendimento_Iniciado_Reiniciado = 3,

        [Display(Name = "ABERTA_AGUARD_AGENDAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        Aberta_Aguard_Agendamento = 4,

        [Display(Name = "OS_EM_DESLOCAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        OS_Em_deslocamento = 6,

        //[Display(Name = "ABERTA_AGUARD_FECHAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        //Aberta_Aguard_Fechamento = 7,

        [Display(Name = "ATENDIMENTO_FINALIZADO", ResourceType = typeof(Resources.RsCadastros))]
        Atendimento_Finalizado = 8,

        [Display(Name = "CANCELADO", ResourceType = typeof(Resources.RsCadastros))]
        Cancelado = 9,

        [Display(Name = "ATENDIMENTO_PARALIZADO", ResourceType = typeof(Resources.RsCadastros))]
        Atendimento_Paralizado = 10,

        [Display(Name = "REINICIADO_ATENDIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        Reiniciado_Atendimento = 11,

        [Display(Name = "INTERROMPER_DESLOCAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        Interromper_Deslocamento = 12,

        [Display(Name = "ABERTA_AGUARD_RESPONSAVEL", ResourceType = typeof(Resources.RsCadastros))]
        Aberta_Aguard_Responsavel = 13,

       


    }
}
