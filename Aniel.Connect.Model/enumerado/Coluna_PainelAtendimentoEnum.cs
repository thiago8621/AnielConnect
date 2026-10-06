using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Aniel.Connect.Model.enumerado
{

    public enum Coluna_PainelAtendimentoEnum
    {
        IMAGEM = 0,
        VIDEO = 1,
        ARQUIVO = 2,

        [Display(Name = "NUMERO_ATENDIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        Numero_Atendimento = 0,
        [Display(Name = "ASSUNTO", ResourceType = typeof(Resources.RsCadastros))]
        Assunto = 1,
        [Display(Name = "TIPO_ATENDIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        Tipo_Atendimento = 2,
        [Display(Name = "SLA", ResourceType = typeof(Resources.RsCadastros))]
        SLA = 3,
        [Display(Name = "STATUS", ResourceType = typeof(Resources.RsCadastros))]
        Status = 4,
        [Display(Name = "MOTIVO_STATUS", ResourceType = typeof(Resources.RsCadastros))]
        Motivo_Status = 5,
        [Display(Name = "RESPONSAVEL_ATUAL", ResourceType = typeof(Resources.RsCadastros))]
        Responsavel_Atual = 6,
        [Display(Name = "DATAHORA_ABERTURA", ResourceType = typeof(Resources.RsCadastros))]
        Datahora_Abertura = 7,
        [Display(Name = "DATAHORA_ENCERRAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        Datahora_Encerramento = 8,
        [Display(Name = "SETOR", ResourceType = typeof(Resources.RsCadastros))]
        Setor = 9,
        [Display(Name = "PRIORIDADE_ATENDIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        Prioridade = 10,
        [Display(Name = "CATEGORIA_ATENDIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        Categoria_Atendimento = 11,
        [Display(Name = "CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        Cliente = 12,
        [Display(Name = "CATEGORIA_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        Categoria_Cliente = 13,
        [Display(Name = "LOGIN", ResourceType = typeof(Resources.RsCadastros))]
        Login = 14,
        [Display(Name = "EMAIL", ResourceType = typeof(Resources.RsCadastros))]
        Email = 15,
        [Display(Name = "ORIGEM", ResourceType = typeof(Resources.RsCadastros))]
        Origem = 16,
        [Display(Name = "TELEFONE", ResourceType = typeof(Resources.RsCadastros))]
        Telefone = 17,
        [Display(Name = "CELULAR", ResourceType = typeof(Resources.RsCadastros))]
        Celular = 18,
        [Display(Name = "PLANO", ResourceType = typeof(Resources.RsCadastros))]
        Plano = 19,
        [Display(Name = "ETAPA_ATUAL", ResourceType = typeof(Resources.RsCadastros))]
        ETAPA_ATUAL = 21,
        [Display(Name = "PRAZO_ETAPA_ATUAL", ResourceType = typeof(Resources.RsCadastros))]
        PRAZO_ETAPA_ATUAL = 22,
        [Display(Name = "TEMPO_GASTO", ResourceType = typeof(Resources.RsCadastros))]
        TEMPO_GASTO = 23,
        [Display(Name = "DATA_SLA_ETAPA", ResourceType = typeof(Resources.RsCadastros))]
        DATA_SLA_ETAPA = 24,
        [Display(Name = "ORDEM_DE_SERVICO", ResourceType = typeof(Resources.RsCadastros))]
        ORDEM_SERVICO = 25,
        [Display(Name = "STATUS_AUDITORIA", ResourceType = typeof(Resources.RsCadastros))]
        STATUS_AUDITORIA = 26


    }
}