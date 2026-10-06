using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum Coluna_PainelVendaEnum
    {
        [Display(Name = "NUMERO", ResourceType = typeof(Resources.RsCadastros))]
        Numero = 0,
        [Display(Name = "NOMEFANTASIA", ResourceType = typeof(Resources.RsCadastros))]
        NomeFantasia = 1,
        [Display(Name = "CPFCNPJ", ResourceType = typeof(Resources.RsCadastros))]
        CpfCnpj = 2,
        [Display(Name = "TIPO_CADASTRO", ResourceType = typeof(Resources.RsCadastros))]
        TipoCadastro = 3,
        [Display(Name = "TIPOPESSOA", ResourceType = typeof(Resources.RsCadastros))]
        TipoPessoa = 4,
        [Display(Name = "TELEFONEPRINCIPAL", ResourceType = typeof(Resources.RsCadastros))]
        TelefonePrincipal = 5,
        [Display(Name = "TELEFONECELULAR", ResourceType = typeof(Resources.RsCadastros))]
        TelefoneCelular = 6,
        [Display(Name = "EMAIL", ResourceType = typeof(Resources.RsCadastros))]
        Email = 7,
        [Display(Name = "MEIO_CAPTACAO", ResourceType = typeof(Resources.RsCadastros))]
        MeioCaptacao = 8,
        [Display(Name = "QUEM_INDICOU", ResourceType = typeof(Resources.RsCadastros))]
        QuemIndicou = 9,
        [Display(Name = "BAIRRO", ResourceType = typeof(Resources.RsCadastros))]
        Bairro = 10,
        [Display(Name = "CIDADE", ResourceType = typeof(Resources.RsCadastros))]
        Cidade = 11,
        [Display(Name = "UF", ResourceType = typeof(Resources.RsCadastros))]
        UF = 12,
        [Display(Name = "LOGRADOURO", ResourceType = typeof(Resources.RsCadastros))]
        Logradouro = 13,
        [Display(Name = "NUMERO", ResourceType = typeof(Resources.RsCadastros))]
        NumeroEndereco = 14,
        [Display(Name = "COMPLEMENTO", ResourceType = typeof(Resources.RsCadastros))]
        Complemento = 15,
        [Display(Name = "VENDEDOR", ResourceType = typeof(Resources.RsCadastros))]
        Vendedor = 16,
        [Display(Name = "SUPERVISOR", ResourceType = typeof(Resources.RsCadastros))]
        Supervisor = 17,
        [Display(Name = "STATUS_VENDA", ResourceType = typeof(Resources.RsCadastros))]
        StatusVenda = 18,
        [Display(Name = "DATAHORA_CRIACAO", ResourceType = typeof(Resources.RsCadastros))]
        DataHoraCriacao = 19,
        [Display(Name = "DATA_VENDA", ResourceType = typeof(Resources.RsCadastros))]
        DataVenda = 20,
        [Display(Name = "ULTIMO_CONTATO", ResourceType = typeof(Resources.RsCadastros))]
        UltimoContato = 21,
        [Display(Name = "HISTORICO_CONTATO", ResourceType = typeof(Resources.RsCadastros))]
        HistoricoContato = 22,
        [Display(Name = "PLANO_PRODUTO", ResourceType = typeof(Resources.RsCadastros))]
        PlanoProduto = 23,
        [Display(Name = "TIPO_IMOVEL", ResourceType = typeof(Resources.RsCadastros))]
        Tipo_Imovel = 24,
        [Display(Name = "CATEGORIA_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        Categoria_Cliente = 25,
        [Display(Name = "FORMA_PAGAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        Forma_Pagamento = 26,
        [Display(Name = "IDENTIFICACAO_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        Identificacao_Cliente = 27,
        [Display(Name = "DEPARTAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        Departamento = 28, 
        [Display(Name = "QTDE_DIAS_SEM_CONTATO", ResourceType = typeof(Resources.RsCadastros))]
        Qtde_Dias_Sem_Contato = 29,
        [Display(Name = "EQUIPE_VENDAS", ResourceType = typeof(Resources.RsCadastros))]
        Equipe_Vendas = 30,
        [Display(Name = "DATAHORA_APROVACAO", ResourceType = typeof(Resources.RsCadastros))]
        DataHora_Aprovacao = 31,
        [Display(Name = "DATAHORA_CANCELAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        DataHora_Cancelamento = 32,
        [Display(Name = "TERMO_ADESAO", ResourceType = typeof(Resources.RsCadastros))]
        Termo_Adesao = 33,
        [Display(Name = "DATA_TERMO_ASSINADO", ResourceType = typeof(Resources.RsCadastros))]
        Data_Termo_Assinado = 34,
        [Display(Name = "PROFISSAO", ResourceType = typeof(Resources.RsCadastros))]
        Profissao = 35,
        [Display(Name = "ETAPA_ATUAL", ResourceType = typeof(Resources.RsCadastros))]
        Etapa_Atual = 36,
        [Display(Name = "MOTIVO_DESISTENCIA_VENDA", ResourceType = typeof(Resources.RsCadastros))]
        Motivo_Desistencia_Venda = 37,
        [Display(Name = "CHECKLIST", ResourceType = typeof(Resources.RsCadastros))]
        CheckList = 38,
        [Display(Name = "DATA_PREENCHIMENTO_CHECKLIST", ResourceType = typeof(Resources.RsCadastros))]
        Data_Preenchimento_CheckList = 39,
        [Display(Name = "USUARIO_APROVACAO_CANCELAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        UsuarioAprovacaoCancelamento = 42,
        [Display(Name = "SUB_VALOR_TOTAL", ResourceType = typeof(Resources.RsCadastros))]
        VALOR_TOTAL_VENDA = 43,
        [Display(Name = "VALOR_INSTALACAO", ResourceType = typeof(Resources.RsCadastros))]
        VALOR_INSTALACAO = 44,
        [Display(Name = "STATUS_AUDITORIA", ResourceType = typeof(Resources.RsCadastros))]
        STATUS_AUDITORIA = 45,
        [Display(Name = "SETOR_RESPONSAVEL", ResourceType = typeof(Resources.RsCadastros))]
        SETOR_RESPONSAVEL = 46,
        [Display(Name = "RESPONSAVEL", ResourceType = typeof(Resources.RsCadastros))]
        RESPONSAVEL = 47

    }
}
