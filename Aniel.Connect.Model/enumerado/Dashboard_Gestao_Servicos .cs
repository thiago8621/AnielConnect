using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Model.enumerado
{

    public enum Dashboard_Gestao_Servicos
    {
        [Display(Name = "TOTAL_CONTRATADOS_CANCELADOS", ResourceType = typeof(Resources.RsCadastros))]
        Total_Contratos_Cancelados = 0,
        [Display(Name = "RESUMO_CONEXOES_BLOQUEADAS", ResourceType = typeof(Resources.RsCadastros))]
        Resumo_Conexoes_Bloqueadas = 1,
        [Display(Name = "EVOLUCAO_INSTALACOES_MES", ResourceType = typeof(Resources.RsCadastros))]
        Evolucao_Instalacoes_Mes = 2,
        [Display(Name = "ORDENS_SERVICOS_ABERTAS_TIPO_SERVICOS", ResourceType = typeof(Resources.RsCadastros))]
        Ordens_Servicos_Abertas_Tipo_Servicos = 3,
        [Display(Name = "TOP_DEZ_PRODUTIVIDADE", ResourceType = typeof(Resources.RsCadastros))]
        Top_Dez_Produtividade = 4,
    }
}