using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Aniel.Connect.Model.enumerado
{
    public enum Coluna_PainelServicoEnum
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
        [Display(Name = "SUBTIPO_SERVICO", ResourceType = typeof(Resources.RsCadastros))]
        Subtipo_Servico = 5,
        [Display(Name = "TIPO_IMOVEL", ResourceType = typeof(Resources.RsCadastros))]
        Tipo_Imovel = 6,
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
        [Display(Name = "ENDERECO", ResourceType = typeof(Resources.RsCadastros))]
        Endereco = 12,
        [Display(Name = "NUMERO", ResourceType = typeof(Resources.RsCadastros))]
        Numero = 13,
        [Display(Name = "COMPLEMENTO", ResourceType = typeof(Resources.RsCadastros))]
        Complemento = 14,
        [Display(Name = "BAIRRO", ResourceType = typeof(Resources.RsCadastros))]
        Bairro = 15,
        [Display(Name = "CIDADE", ResourceType = typeof(Resources.RsCadastros))]
        Cidade = 16,
        [Display(Name = "UF", ResourceType = typeof(Resources.RsCadastros))]
        UF = 17,
        [Display(Name = "PAIS", ResourceType = typeof(Resources.RsCadastros))]
        Pais = 18,
        [Display(Name = "TELEFONEPRINCIPAL", ResourceType = typeof(Resources.RsCadastros))]
        telefone_Principal = 19,
        [Display(Name = "TELEFONECELULAR", ResourceType = typeof(Resources.RsCadastros))]
        telefone_Celular = 20,
        [Display(Name = "USUARIO_ABERTURA_OS", ResourceType = typeof(Resources.RsCadastros))]
        Usuario_Abertura_OS = 21,
        [Display(Name = "DATA_HORA_ABERTURA", ResourceType = typeof(Resources.RsCadastros))]
        DataHora_Criacao = 22,
        [Display(Name = "USUARIO_AGENDAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        Usuario_Agendamento = 23,
        [Display(Name = "DATA_HORA_AGENDAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        DataHora_Agendamento = 24,
        [Display(Name = "DESCRICAO_ENCERRAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        Descricao_Encerramento = 25,
        [Display(Name = "USUARIO_ENCERRAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        Usuario_Encerramento = 26,
        [Display(Name = "DATA_HORA_ENCERRAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        DataHora_Encerramento = 27,
        [Display(Name = "ATENDIMENTO_FINALIZADO", ResourceType = typeof(Resources.RsCadastros))]
        Atendimento_Finalizado = 28,
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
        [Display(Name = "PLANO_PRODUTO", ResourceType = typeof(Resources.RsCadastros))]
        Plano_Produto = 34,
        [Display(Name = "SLA", ResourceType = typeof(Resources.RsCadastros))]
        SLA = 35,
        [Display(Name = "PERIODO", ResourceType = typeof(Resources.RsCadastros))]
        PERIODO = 38,
        [Display(Name = "NUMERO_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        Contrato_Cliente = 36,
        [Display(Name = "CPFCNPJ_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        Cpf_Cnpj_Cliente = 37,
        [Display(Name = "OBSERVACAO", ResourceType = typeof(Resources.RsCadastros))]
        Observacao2 = 39,
        [Display(Name = "MOTIVO_PARALIZACAO", ResourceType = typeof(Resources.RsCadastros))]
        MOTIVO_PARALIZACAO = 40,
        [Display(Name = "OBSERVACAO_TECNICO", ResourceType = typeof(Resources.RsCadastros))]
        OBSERVACAO_TECNICO = 41,
        [Display(Name = "MATRICULA_TECNICO", ResourceType = typeof(Resources.RsCadastros))]
        MATRICULA_TECNICO = 42,
        [Display(Name = "STATUS_AUDITORIA", ResourceType = typeof(Resources.RsCadastros))]
        STATUS_AUDITORIA = 43,
        [Display(Name = "LATITUDE_LONGITUDE", ResourceType = typeof(Resources.RsCadastros))]
        LATITUDE_LONGITUDE = 44,
        [Display(Name = "QUANTIDADE_REABERTURA", ResourceType = typeof(Resources.RsCadastros))]
        QUANTIDADE_REABERTURA = 45,
        [Display(Name = "DATA_HORA_ABERTURA_ORIGINAL", ResourceType = typeof(Resources.RsCadastros))]
        DATA_HORA_ABERTURA_ORIGINAL = 46,
        [Display(Name = "NUMERO_ATENDIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        NUMERO_ATENDIMENTO = 47,
        [Display(Name = "WO", ResourceType = typeof(Resources.RsCadastros))]
        WO = 48,
        [Display(Name = "TEM_OSDIGITAL", ResourceType = typeof(Resources.RsCadastros))]
        TEM_OSDIGITAL = 49,
        [Display(Name = "DEPARTAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        DEPARTAMENTO = 50,
        [Display(Name = "DATA_HORA_VENCIMENTO_OS", ResourceType = typeof(Resources.RsCadastros))]
        DATA_HORA_VENCIMENTO_OS = 51,
        [Display(Name = "TIPO_PRIORIDADE", ResourceType = typeof(Resources.RsCadastros))]
        TIPO_PRIORIDADE = 52,
        [Display(Name = "STATUS_COBRANCA", ResourceType = typeof(Resources.RsCadastros))]
        STATUS_COBRANCA = 53,
        [Display(Name = "RESPONSABILIDADE", ResourceType = typeof(Resources.RsCadastros))]
        RESPONSABILIDADE = 54,
        [Display(Name = "SEGMENTO", ResourceType = typeof(Resources.RsCadastros))]
        SEGMENTO = 55,
        [Display(Name = "DATAHORA_DESLOCAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        DATAHORA_DESLOCAMENTO = 56,
        [Display(Name = "DATAHORA_INICIO_ATENDIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        DATAHORA_INICIO_ATENDIMENTO = 57,
        [Display(Name = "DATAHORA_PAUSA_ATENDIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        DATAHORA_PAUSA_ATENDIMENTO = 58,
        [Display(Name = "DATAHORA_REINICIO_ATENDIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        DATAHORA_REINICIO_ATENDIMENTO = 59,
        [Display(Name = "DATAHORA_FIM_ATENDIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        DATAHORA_FIM_ATENDIMENTO = 60,
        [Display(Name = "TECNICO_AUXILIAR", ResourceType = typeof(Resources.RsCadastros))]
        TECNICO_AUXILIAR = 61,



    }
}
