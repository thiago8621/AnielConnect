using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Model.enumerado
{

    public enum Recurso_PainelAtendimentoEnum
    {
        [Display(Name = "TIPO_ATENDIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        Tipo_Atendimento = 0,
        [Display(Name = "SLA_ATENDIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        SLA_Atendimento = 1,
        [Display(Name = "PRIORIDADE_ATENDIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        Prioridade_Atendimento = 2,
        [Display(Name = "CATEGORIA_ATENDIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        Categoria_Atendimento = 3,
        [Display(Name = "CATEGORIA_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        Categoria_Cliente = 4,
        [Display(Name = "STATUS", ResourceType = typeof(Resources.RsCadastros))]
        Status = 5,
        [Display(Name = "MOTIVO_STATUS", ResourceType = typeof(Resources.RsCadastros))]
        Motivo_Status = 6,
        [Display(Name = "SETOR", ResourceType = typeof(Resources.RsCadastros))]
        Setor = 7,
        [Display(Name = "ORIGEM", ResourceType = typeof(Resources.RsCadastros))]
        Origem = 8
    }
}