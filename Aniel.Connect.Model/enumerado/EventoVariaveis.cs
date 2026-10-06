using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Model.enumerado
{
    public enum EventoVariaveis
    {

        /// <summary>
        /// STRING_VAZIA: Usado para enum removido ou desativado
        /// </summary>
        [Display(Name = "@PROTOCOLO", ResourceType = typeof(Resources.RsCadastros))]
        @PROTOCOLO = 0,

        [Display(Name = "@NOME", ResourceType = typeof(Resources.RsCadastros))]
        @NOME = 1,

        [Display(Name = "@CPF_CNPJ", ResourceType = typeof(Resources.RsCadastros))]
        @CPF_CNPJ = 2,

        [Display(Name = "@DATA_NASCIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        @DATA_NASCIMENTO = 3,

        [Display(Name = "@DATA_ABERTURA", ResourceType = typeof(Resources.RsCadastros))]
        @DATA_ABERTURA = 4,

        [Display(Name = "@SEXO", ResourceType = typeof(Resources.RsCadastros))]
        @SEXO = 5,

        [Display(Name = "@CATEGORIA", ResourceType = typeof(Resources.RsCadastros))]
        @CATEGORIA = 6,

        [Display(Name = "@ID_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        @ID_CLIENTE = 7,

        [Display(Name = "@PLANO_PRODUTO", ResourceType = typeof(Resources.RsCadastros))]
        @PLANO_PRODUTO = 8,

        [Display(Name = "@TELEFONE_FIXO", ResourceType = typeof(Resources.RsCadastros))]
        @TELEFONE_FIXO = 9,

        [Display(Name = "@TELEFONE_CELULAR", ResourceType = typeof(Resources.RsCadastros))]
        @TELEFONE_CELULAR = 10,

        [Display(Name = "@ENDERECO", ResourceType = typeof(Resources.RsCadastros))]
        @ENDERECO = 11,

        [Display(Name = "@DATA_ENCERRAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        @DATA_ENCERRAMENTO = 12,

        [Display(Name = "@NOME_CONTATO", ResourceType = typeof(Resources.RsCadastros))]        
        @NOME_CONTATO = 13,

    }
}
