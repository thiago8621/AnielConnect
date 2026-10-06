using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Model.enumerado
{

    public enum Dashboard_Servicos
    {
        [Display(Name = "SERVICOS_SEM_AGENDAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        Serviços_Sem_agendamento = 0,
        [Display(Name = "SERVICOS_NAO_ROTEIRIZADOS", ResourceType = typeof(Resources.RsCadastros))]
        Serviços_Nao_Roteirizados = 1,
        [Display(Name = "SERVICOS_NAO_INCIADOS", ResourceType = typeof(Resources.RsCadastros))]
        Serviços_Nao_Iniciados = 2,
        [Display(Name = "SERVICOS_PARALISADOS", ResourceType = typeof(Resources.RsCadastros))]
        Serviços_Paralisados = 3,
        [Display(Name = "SERVICOS_EM_EXECUCAO", ResourceType = typeof(Resources.RsCadastros))]
        Serviços_Execução = 4,
        [Display(Name = "SERVICOS_FORA_PRAZO", ResourceType = typeof(Resources.RsCadastros))]
        Serviços_Fora_Prazo = 5,
        [Display(Name = "SERVICOS_ENCERRADOS", ResourceType = typeof(Resources.RsCadastros))]
        Serviços_Encerrados = 6,
        [Display(Name = "PRODUTIVIDADE_MES", ResourceType = typeof(Resources.RsCadastros))]
        Produtividade_Mes = 7,
        [Display(Name = "CUSTO_MEDIO_SERVICO", ResourceType = typeof(Resources.RsCadastros))]
        Custo_Medio_Serviço = 8,
        [Display(Name = "RANKING_PROD_MES", ResourceType = typeof(Resources.RsCadastros))]
        Ranking_Produtividade_Mes = 9,
        [Display(Name = "PCT_SERVICO_FORA_PRAZO", ResourceType = typeof(Resources.RsCadastros))]
        Percentual_Serviços_Fora_Prazo = 10,
        [Display(Name = "MEDIA_ATRASO", ResourceType = typeof(Resources.RsCadastros))]
        Media_Atraso = 11,
        [Display(Name = "CHAT", ResourceType = typeof(Resources.RsCadastros))]
        Chat = 12,
        [Display(Name = "NOVIDADES_SINAPSE", ResourceType = typeof(Resources.RsCadastros))]
        Novidades_Sinapse = 13,
    }

    public enum Dashboard_Servicos_Modelo2
    {
        [Display(Name = "PAINEL_INDICADORES", ResourceType = typeof(Resources.RsCadastros))]
        Painel_Indicadores = 0,
        [Display(Name = "SERVICOS_PERDA_PRAZO", ResourceType = typeof(Resources.RsCadastros))]
        Servicos_Perda_Prazo = 1,
        [Display(Name = "SERVICOS_FECHADOS_PROD_IMPROD", ResourceType = typeof(Resources.RsCadastros))]
        Servicos_Fechados_Prod_Improd = 2,
        [Display(Name = "SERVICOS_STATUS_EXECUCAO", ResourceType = typeof(Resources.RsCadastros))]
        Servicos_Status_Execucao = 3,
        [Display(Name = "TOP_ENCERRAMENTOS", ResourceType = typeof(Resources.RsCadastros))]
        Top_Encerramentos = 4
    }

}