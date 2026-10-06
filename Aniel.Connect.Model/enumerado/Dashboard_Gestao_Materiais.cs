using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Model.enumerado
{

    public enum Dashboard_Gestao_Materiais
    {
        [Display(Name = "MATERIAIS_ESTOQUE_ALTO", ResourceType = typeof(Resources.RsCadastros))]
        Materiais_Estoque_Alto = 0,
        [Display(Name = "VALOR_MATERIAIS_ESTOQUE", ResourceType = typeof(Resources.RsCadastros))]
        Valor_Materiais_Estoque = 1,
        [Display(Name = "MATERIAIS_SEM_ESTOQUE", ResourceType = typeof(Resources.RsCadastros))]
        Materiais_Sem_Estoque = 2,
        [Display(Name = "MATERIAIS_MAIS_UTILIZADOS_MES", ResourceType = typeof(Resources.RsCadastros))]
        Materiais_Mais_Utilizados_Mes = 3,
        
    }
}