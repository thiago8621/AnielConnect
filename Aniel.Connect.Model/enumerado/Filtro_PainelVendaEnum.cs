using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Model.enumerado
{
    public enum Filtro_PainelVendaEnum
    {
        [Display(Name = "CIDADE", ResourceType = typeof(Resources.RsCadastros))]
        Cidade = 0,
        //[Display(Name = "NOMEFANTASIA", ResourceType = typeof(Resources.RsCadastros))]
        //NomeFantasia = 1,
        //[Display(Name = "CPFCNPJ", ResourceType = typeof(Resources.RsCadastros))]
        //CpfCnpj = 2,
        [Display(Name = "TIPO_CADASTRO", ResourceType = typeof(Resources.RsCadastros))]
        TipoCadastro = 3,
        [Display(Name = "TIPOPESSOA", ResourceType = typeof(Resources.RsCadastros))]
        TipoPessoa = 4,
        [Display(Name = "MEIO_CAPTACAO", ResourceType = typeof(Resources.RsCadastros))]
        MeioCaptacao = 8,
        //[Display(Name = "QUEM_INDICOU", ResourceType = typeof(Resources.RsCadastros))]
        //QuemIndicou = 9,
        //[Display(Name = "BAIRRO", ResourceType = typeof(Resources.RsCadastros))]
        //Bairro = 10,
        [Display(Name = "UF", ResourceType = typeof(Resources.RsCadastros))]
        UF = 12,
        [Display(Name = "VENDEDOR", ResourceType = typeof(Resources.RsCadastros))]
        Vendedor = 16,
        [Display(Name = "SUPERVISOR", ResourceType = typeof(Resources.RsCadastros))]
        Supervisor = 17,
        [Display(Name = "STATUS_VENDA", ResourceType = typeof(Resources.RsCadastros))]
        StatusVenda = 18,
        //[Display(Name = "ULTIMO_CONTATO", ResourceType = typeof(Resources.RsCadastros))]
        //UltimoContato = 21,
        //[Display(Name = "HISTORICO_CONTATO", ResourceType = typeof(Resources.RsCadastros))]
        //HistoricoContato = 22,
        [Display(Name = "PLANO", ResourceType = typeof(Resources.RsCadastros))]
        Plano = 23,
        [Display(Name = "PRODUTO", ResourceType = typeof(Resources.RsCadastros))]
        Produto = 24,
        [Display(Name = "TIPO_IMOVEL", ResourceType = typeof(Resources.RsCadastros))]
        Tipo_Imovel = 25,
        [Display(Name = "CATEGORIA_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        Categoria_Cliente = 26,
        [Display(Name = "FORMA_PAGAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        Forma_Pagamento = 27,
        [Display(Name = "DATA_VENDA", ResourceType = typeof(Resources.RsCadastros))]
        Data_Venda = 28,
        [Display(Name = "DATAHORA_APROVACAO", ResourceType = typeof(Resources.RsCadastros))]
        DataHora_Aprovacao = 29,
        [Display(Name = "DATAHORA_CANCELAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        DataHora_Cancelamento = 30,
        [Display(Name = "MOTIVO_DESISTENCIA_VENDA", ResourceType = typeof(Resources.RsCadastros))]
        Motivo_Desistencia_Venda = 31,
        [Display(Name = "ETAPA_VENDA", ResourceType = typeof(Resources.RsCadastros))]
        EtapaVenda = 32,
        [Display(Name = "SETOR_RESPONSAVEL", ResourceType = typeof(Resources.RsCadastros))]
        SETOR_RESPONSAVEL = 33,
        [Display(Name = "RESPONSAVEL", ResourceType = typeof(Resources.RsCadastros))]
        RESPONSAVEL = 34

    }
}
