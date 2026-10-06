using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Filtro_PainelServicoEnum
    {
        [Display(Name = "TIPO_SERVICO", ResourceType = typeof(Resources.RsCadastros))]
        Tipo_Servico = 0,
        [Display(Name = "NATUREZA_PRODUCAO", ResourceType = typeof(Resources.RsCadastros))]
        Natureza_Producao = 1,
        [Display(Name = "STATUS", ResourceType = typeof(Resources.RsCadastros))]
        Status = 2,
        [Display(Name = "GRUPO_AREA", ResourceType = typeof(Resources.RsCadastros))]
        GrupoArea = 3,
        [Display(Name = "AREA", ResourceType = typeof(Resources.RsCadastros))]
        Area = 4,
        [Display(Name = "LOCALIDADE", ResourceType = typeof(Resources.RsCadastros))]
        Localidade = 5,
        [Display(Name = "RESPONSAVEL", ResourceType = typeof(Resources.RsCadastros))]
        Responsavel = 6,
        [Display(Name = "SUPERVISOR", ResourceType = typeof(Resources.RsCadastros))]
        Supervisor_Tecnico = 7,
        [Display(Name = "TIPO_IMOVEL", ResourceType = typeof(Resources.RsCadastros))]
        Tipo_Imovel = 8,
        [Display(Name = "UF", ResourceType = typeof(Resources.RsCadastros))]
        UF = 9,
        [Display(Name = "CIDADE", ResourceType = typeof(Resources.RsCadastros))]
        Cidade = 10,
        [Display(Name = "CATEGORIA_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        Categoria_Cliente = 11,
        [Display(Name = "CONTRATO", ResourceType = typeof(Resources.RsCadastros))]
        Contrato = 12,
        [Display(Name = "PROJETO", ResourceType = typeof(Resources.RsCadastros))]
        Projeto = 13,
        [Display(Name = "MONITOR", ResourceType = typeof(Resources.RsCadastros))]
        Monitor_Tecnico = 14,
        [Display(Name = "ENCERRAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        Encerramento = 15,
        [Display(Name = "ORIGEM", ResourceType = typeof(Resources.RsCadastros))]
        Origem = 16,
        [Display(Name = "EMPRESA", ResourceType = typeof(Resources.RsCadastros))]
        Empresa = 17,
        [Display(Name = "FILIAL", ResourceType = typeof(Resources.RsCadastros))]
        Filial = 18,
        [Display(Name = "SUBTIPO_SERVICO", ResourceType = typeof(Resources.RsCadastros))]
        Sub_Tipo_Servico = 19,
        [Display(Name = "USUARIO_ABERTURA_OS", ResourceType = typeof(Resources.RsCadastros))]
        UsuarioAbertura = 20,
        [Display(Name = "USUARIO_AGENDAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        UsuarioAgendamento = 21,
        [Display(Name = "DATA_AGENDAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        DataAgendamento = 22,
        [Display(Name = "DATA_CRIACAO", ResourceType = typeof(Resources.RsCadastros))]
        DataCriacao = 23,
        [Display(Name = "DATA_ENCERRAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        DataEncerramento = 24,
        [Display(Name = "PLANO_PRODUTO", ResourceType = typeof(Resources.RsCadastros))]
        PlanoProduto = 25,
        [Display(Name = "SLA", ResourceType = typeof(Resources.RsCadastros))]
        SLA = 26,
        [Display(Name = "DATA_VENCIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        DataVencimento = 27,
        [Display(Name = "STATUS_COBRANCA", ResourceType = typeof(Resources.RsCadastros))]
        Status_Cobranca = 28,
        [Display(Name = "PERIODO", ResourceType = typeof(Resources.RsCadastros))]
        Periodo = 29,

    }
}
