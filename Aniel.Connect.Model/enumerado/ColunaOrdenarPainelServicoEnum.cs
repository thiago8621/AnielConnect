using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum ColunaOrdenarPainelServicoEnum
    {
        [Display(Name = "NUMERO_ORDEM_SERVICO", ResourceType = typeof(Resources.RsCadastros))]
        Numero_Ordem_Servico = 0,
        [Display(Name = "IDENTIFICACAO_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        Identificacao_Cliente = 1,
        [Display(Name = "STATUS_OS", ResourceType = typeof(Resources.RsCadastros))]
        Status_OS = 2,
        [Display(Name = "RESPONSAVEL", ResourceType = typeof(Resources.RsCadastros))]
        Responsavel = 3,
        [Display(Name = "TIPO_SERVICO", ResourceType = typeof(Resources.RsCadastros))]
        Tipo_Servico = 4,
        [Display(Name = "SUPERVISOR", ResourceType = typeof(Resources.RsCadastros))]
        Supervisor = 7,
        [Display(Name = "GRUPO_AREA", ResourceType = typeof(Resources.RsCadastros))]
        Grupo_Area = 8,
        [Display(Name = "AREA", ResourceType = typeof(Resources.RsCadastros))]
        Area = 9,
        [Display(Name = "LOCALIDADE", ResourceType = typeof(Resources.RsCadastros))]
        Localidade = 10,
        [Display(Name = "CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        Cliente = 11,
        [Display(Name = "BAIRRO", ResourceType = typeof(Resources.RsCadastros))]
        Bairro = 15,
        [Display(Name = "CIDADE", ResourceType = typeof(Resources.RsCadastros))]
        Cidade = 16,
        [Display(Name = "UF", ResourceType = typeof(Resources.RsCadastros))]
        UF = 17,
        [Display(Name = "DATA_HORA_ABERTURA", ResourceType = typeof(Resources.RsCadastros))]
        DataHora_Criacao = 22,
        [Display(Name = "DATA_HORA_AGENDAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        DataHora_Agendamento = 24,
        [Display(Name = "DATA_HORA_ENCERRAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        DataHora_Encerramento = 27,
        [Display(Name = "CONTRATO", ResourceType = typeof(Resources.RsCadastros))]
        Contrato = 29,
        [Display(Name = "PROJETO", ResourceType = typeof(Resources.RsCadastros))]
        Projeto = 30,
        [Display(Name = "CATEGORIA_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        CategoriaCliente = 31,
        [Display(Name = "MONITOR", ResourceType = typeof(Resources.RsCadastros))]
        Monitor = 32,
        [Display(Name = "NATUREZA", ResourceType = typeof(Resources.RsCadastros))]
        Natureza = 33,
        [Display(Name = "CPFCNPJ_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        Cpf_Cnpj_Cliente = 37,
        [Display(Name = "OBSERVACAO", ResourceType = typeof(Resources.RsCadastros))]
        Observacao2 = 39,
        [Display(Name = "PERIODO", ResourceType = typeof(Resources.RsCadastros))]
        PERIODO = 50,
        [Display(Name = "DESLOCAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        Deslocamento = 58,
        [Display(Name = "INICIO_ATENDIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        Inicio_Atendimento = 59,
        [Display(Name = "FINAL_ATENDIMENTO", ResourceType = typeof(Resources.RsConfiguracao))]
        Final_Atendimento = 60,

    }
}
