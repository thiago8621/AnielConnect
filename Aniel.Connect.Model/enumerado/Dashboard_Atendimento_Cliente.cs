using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Model.enumerado
{

    public enum Dashboard_Inicial_Cliente
    {
        [Display(Name = "TOTAL_ATENDIMENTOS_USUARIO", ResourceType = typeof(Resources.RsCadastros))]
        Total_Atendimentos_Usuario = 0,
        [Display(Name = "ATENDIMENTOS_ENCERRADOS", ResourceType = typeof(Resources.RsCadastros))]
        Atendimentos_Encerrados = 1,
        [Display(Name = "ATENDIMENTOS_CONCLUIDO_USUARIO", ResourceType = typeof(Resources.RsCadastros))]
        Atendimentos_Concluido_Usuario = 2,
        [Display(Name = "ATENDIMENTOS_ABERTO_USUARIO", ResourceType = typeof(Resources.RsCadastros))]
        Atendimentos_Aberto_Usuario = 3,
        //[Display(Name = "ATENDIMENTOS_PENDENTES_EQUIPE", ResourceType = typeof(Resources.RsCadastros))]
        //Atendimentos_Pendentes_Equipe = 4,
        [Display(Name = "TOTAL_GERAL_ATENDIMENTOS", ResourceType = typeof(Resources.RsCadastros))]
        Total_Geral_Atendimentos = 5,
        [Display(Name = "ATENDIMENTOS_FILA", ResourceType = typeof(Resources.RsCadastros))]
        Atendimentos_Fila = 6,
        [Display(Name = "EM_ATENDIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        Em_Atendimento = 7,
        [Display(Name = "SLA", ResourceType = typeof(Resources.RsCadastros))]
        SLA = 8,
        [Display(Name = "SLA_GRAFICO", ResourceType = typeof(Resources.RsCadastros))]
        SLA_Grafico = 9,
        //[Display(Name = "CHAT", ResourceType = typeof(Resources.RsCadastros))]
        //Chat = 10,
        //[Display(Name = "NOVIDADES_SINAPSE", ResourceType = typeof(Resources.RsCadastros))]
        //Novidades_Sinapse = 11,
        [Display(Name = "TOTAL_ATEND_ENC_DENTRO_PRAZO", ResourceType = typeof(Resources.RsCadastros))]
        Total_Atend_Enc_Dentro_Prazo = 12,
        [Display(Name = "TOTAL_ATEND_ENC_FORA_PRAZO", ResourceType = typeof(Resources.RsCadastros))]
        Total_Atend_Enc_Fora_Prazo = 13,
        [Display(Name = "TEMPO_MEDIO_FILA", ResourceType = typeof(Resources.RsCadastros))]
        Tempo_Medio_Fila = 14,
        [Display(Name = "TEMPO_MEDIO_ATENDIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        Tempo_Medio_Atendimento = 15,
        [Display(Name = "ATEND_ENC_INTERV_HUMANA", ResourceType = typeof(Resources.RsCadastros))]
        Atend_Enc_Intervencao_Humana = 16,
        [Display(Name = "NIVEL_SOLUCAO_PRIM_CTT", ResourceType = typeof(Resources.RsCadastros))]
        Nivel_Solucao_Primeiro_Contato = 17,
        [Display(Name = "PESQUISA_SATISFACAO_RESP", ResourceType = typeof(Resources.RsCadastros))]
        Pesquisa_Satisfacao_Respondida = 18,

    }
}