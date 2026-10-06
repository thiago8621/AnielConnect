using System.ComponentModel.DataAnnotations;

namespace Aniel.Connect.Model.enumerado
{
    public enum VariaveisModeloContrato
    {
        [Display(Name = "VAR_NOME_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        VAR_NOME_CLIENTE,
        [Display(Name = "VAR_CPF", ResourceType = typeof(Resources.RsCadastros))]
        VAR_CPF,
        [Display(Name = "VAR_RG", ResourceType = typeof(Resources.RsCadastros))]
        VAR_RG,
        [Display(Name = "VAR_DATA_NASCIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        VAR_DATA_NASCIMENTO,
        [Display(Name = "VAR_DID_VINCULADO", ResourceType = typeof(Resources.RsCadastros))]
        VAR_DID_VINCULADO,
        [Display(Name = "VAR_FORMA_PAGAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        VAR_FORMA_PAGAMENTO,
        [Display(Name = "VAR_TESTE_GRATIS", ResourceType = typeof(Resources.RsCadastros))]
        VAR_TESTE_GRATIS,
        [Display(Name = "VAR_DATA_APROVACAO_VENDA", ResourceType = typeof(Resources.RsCadastros))]
        VAR_DATA_APROVACAO_VENDA,
        [Display(Name = "VAR_VENDEDOR", ResourceType = typeof(Resources.RsCadastros))]
        VAR_VENDEDOR,
        [Display(Name = "VAR_IDENTIFICACAO_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        VAR_IDENTIFICACAO_CLIENTE,
        [Display(Name = "VAR_DESCONTO", ResourceType = typeof(Resources.RsCadastros))]
        VAR_DESCONTO,
        [Display(Name = "VAR_NOME_FANTASIA_EMPRESA", ResourceType = typeof(Resources.RsCadastros))]
        VAR_NOME_FANTASIA_EMPRESA,
        [Display(Name = "VAR_PLANOS_PRODUTOS", ResourceType = typeof(Resources.RsCadastros))]
        VAR_PLANOS_PRODUTOS,
        [Display(Name = "VAR_PERIODO_DE_CARENCIA", ResourceType = typeof(Resources.RsCadastros))]
        VAR_PERIODO_DE_CARENCIA,
        [Display(Name = "VAR_MULTA_CARENCIA", ResourceType = typeof(Resources.RsCadastros))]
        VAR_MULTA_CARENCIA,
        [Display(Name = "VAR_RAZAO_SOCIAL", ResourceType = typeof(Resources.RsCadastros))]
        VAR_RAZAO_SOCIAL,
        //Teste
        [Display(Name = "VAR_DATA_EMISSAO", ResourceType = typeof(Resources.RsCadastros))]
        VAR_DATA_EMISSAO,
        [Display(Name = "VAR_FILIACAO", ResourceType = typeof(Resources.RsCadastros))]
        VAR_FILIACAO,
        [Display(Name = "VAR_TELEFONE_FIXO_1", ResourceType = typeof(Resources.RsCadastros))]
        VAR_TELEFONE_FIXO_1,
        [Display(Name = "VAR_TELEFONE_CELULAR", ResourceType = typeof(Resources.RsCadastros))]
        VAR_TELEFONE_CELULAR,
        [Display(Name = "VAR_EMAIL", ResourceType = typeof(Resources.RsCadastros))]
        VAR_EMAIL,
        [Display(Name = "VAR_TELEFONE_DE_RECADO", ResourceType = typeof(Resources.RsCadastros))]
        VAR_TELEFONE_DE_RECADO,
        [Display(Name = "VAR_NOME_PARA_RECADO", ResourceType = typeof(Resources.RsCadastros))]
        VAR_NOME_PARA_RECADO,
        [Display(Name = "VAR_ENDERECO_PRINCIPAL", ResourceType = typeof(Resources.RsCadastros))]
        VAR_ENDERECO_PRINCIPAL,
        [Display(Name = "VAR_DATA_DE_ABERTURA", ResourceType = typeof(Resources.RsCadastros))]
        VAR_DATA_DE_ABERTURA,
        [Display(Name = "VAR_INSCRICAO_ESTADUAL", ResourceType = typeof(Resources.RsCadastros))]
        VAR_INSCRICAO_ESTADUAL,
        [Display(Name = "VAR_INSCRICAO_MUNICIPAL", ResourceType = typeof(Resources.RsCadastros))]
        VAR_INSCRICAO_MUNICIPAL,
        [Display(Name = "VAR_PORTE", ResourceType = typeof(Resources.RsCadastros))]
        VAR_PORTE,
        [Display(Name = "VAR_RESPONSAVEL", ResourceType = typeof(Resources.RsCadastros))]
        VAR_RESPONSAVEL,
        [Display(Name = "VAR_CPF_RESPONSAVEL", ResourceType = typeof(Resources.RsCadastros))]
        VAR_CPF_RESPONSAVEL,
        [Display(Name = "VAR_CARGO_RESPONSAVEL", ResourceType = typeof(Resources.RsCadastros))]
        VAR_CARGO_RESPONSAVEL,
        [Display(Name = "VAR_NUMERO_DA_VENDA", ResourceType = typeof(Resources.RsCadastros))]
        VAR_NUMERO_DA_VENDA,
        [Display(Name = "VAR_DATA_DE_CRIACAO_VENDA", ResourceType = typeof(Resources.RsCadastros))]
        VAR_DATA_DE_CRIACAO_VENDA,
        [Display(Name = "VAR_DATA_DE_ENCERRAMENTO", ResourceType = typeof(Resources.RsCadastros))]
        VAR_DATA_DE_ENCERRAMENTO,
        [Display(Name = "VAR_SUPERVISOR_DO_VENDEDOR", ResourceType = typeof(Resources.RsCadastros))]
        VAR_SUPERVISOR_DO_VENDEDOR,
        [Display(Name = "VAR_JUSTIFICATIVA_DA_APROVCAO_OU_CANCELAMENTO_DA_VENDA", ResourceType = typeof(Resources.RsCadastros))]
        VAR_JUSTIFICATIVA_DA_APROVCAO_OU_CANCELAMENTO_DA_VENDA,
        [Display(Name = "VAR_BANCO", ResourceType = typeof(Resources.RsCadastros))]
        VAR_BANCO,
        [Display(Name = "VAR_AGENCIA", ResourceType = typeof(Resources.RsCadastros))]
        VAR_AGENCIA,
        [Display(Name = "VAR_CONTA_CORRENTE", ResourceType = typeof(Resources.RsCadastros))]
        VAR_CONTA_CORRENTE,
        [Display(Name = "VAR_DIGITO", ResourceType = typeof(Resources.RsCadastros))]
        VAR_DIGITO,
        [Display(Name = "VAR_DIA_DE_VENCIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        VAR_DIA_DE_VENCIMENTO,
        [Display(Name = "VAR_FORMA_ENVIO_FATURA", ResourceType = typeof(Resources.RsCadastros))]
        VAR_FORMA_ENVIO_FATURA,
        [Display(Name = "VAR_ENDERECO_COBRANCA", ResourceType = typeof(Resources.RsCadastros))]
        VAR_ENDERECO_COBRANCA,
        [Display(Name = "VAR_PLANO", ResourceType = typeof(Resources.RsCadastros))]
        VAR_PLANO,
        [Display(Name = "VAR_NUMERO_CONTRATO", ResourceType = typeof(Resources.RsCadastros))]
        VAR_NUMERO_CONTRATO,
        [Display(Name = "VAR_VALOR_FINAL_TOTAL", ResourceType = typeof(Resources.RsCadastros))]
        VAR_VALOR_FINAL_TOTAL,
        [Display(Name = "VAR_VALOR_INSTALACAO", ResourceType = typeof(Resources.RsCadastros))]
        VAR_VALOR_INSTALACAO,
        [Display(Name = "VAR_STATUS", ResourceType = typeof(Resources.RsCadastros))]
        VAR_STATUS,
        [Display(Name = "VAR_ENDERECO_DE_INSTALACAO", ResourceType = typeof(Resources.RsCadastros))]
        VAR_ENDERECO_DE_INSTALACAO,
        [Display(Name = "VAR_DIDS_VINCULADOS_A_VENDA", ResourceType = typeof(Resources.RsCadastros))]
        VAR_DIDS_VINCULADOS_A_VENDA,
        [Display(Name = "VAR_MENSALIDADE_DE_CADA_DID", ResourceType = typeof(Resources.RsCadastros))]
        VAR_MENSALIDADE_DE_CADA_DID,
        [Display(Name = "VAR_SERVICOS_AGREGADOS", ResourceType = typeof(Resources.RsCadastros))]
        VAR_SERVICOS_AGREGADOS,
        [Display(Name = "VAR_FRANQUIA", ResourceType = typeof(Resources.RsCadastros))]
        VAR_FRANQUIA,
        [Display(Name = "VAR_CNPJ_DA_EMPRESA", ResourceType = typeof(Resources.RsCadastros))]
        VAR_CNPJ_DA_EMPRESA,
        [Display(Name = "VAR_NOME_DA_EMPRESA", ResourceType = typeof(Resources.RsCadastros))]
        VAR_NOME_DA_EMPRESA,
        [Display(Name = "VAR_ENDERECO_DA_EMPRESA", ResourceType = typeof(Resources.RsCadastros))]
        VAR_ENDERECO_DA_EMPRESA,
        [Display(Name = "VAR_VALOR_MENSALIDADE", ResourceType = typeof(Resources.RsCadastros))]
        VAR_VALOR_MENSALIDADE,
        [Display(Name = "VAR_FORMA_COBRANCA", ResourceType = typeof(Resources.RsCadastros))]
        VAR_FORMA_COBRANCA,
        [Display(Name = "VAR_VELOCIDADE_DOWNLOAD", ResourceType = typeof(Resources.RsCadastros))]
        VAR_VELOCIDADE_DOWNLOAD,
        [Display(Name = "VAR_VELOCIDADE_UPLOAD", ResourceType = typeof(Resources.RsCadastros))]
        VAR_VELOCIDADE_UPLOAD,
        [Display(Name = "VAR_TERMO_ASSINATURA", ResourceType = typeof(Resources.RsCadastros))]
        VAR_TERMO_ASSINATURA,
        [Display(Name = "VAR_LOGO", ResourceType = typeof(Resources.RsCadastros))]
        VAR_LOGO,

        [Display(Name = "VAR_FOOTER", ResourceType = typeof(Resources.RsCadastros))]
        VAR_FOOTER,
        [Display(Name = "VAR_DETALHES_PLANO", ResourceType = typeof(Resources.RsCadastros))]
        VAR_DETALHES_PLANO,
        [Display(Name = "VAR_OBSERVACAO_CONTRATO", ResourceType = typeof(Resources.RsCadastros))]
        VAR_OBSERVACAO_CONTRATO,
        [Display(Name = "VAR_DESCRICAO_CONTRATO", ResourceType = typeof(Resources.RsCadastros))]
        VAR_DESCRICAO_CONTRATO,
        [Display(Name = "VAR_VALOR_DESCONTO_VITALICIO", ResourceType = typeof(Resources.RsCadastros))]
        VAR_VALOR_DESCONTO_VITALICIO,
        [Display(Name = "VAR_VALOR_DESCONTO_TEMPORARIO", ResourceType = typeof(Resources.RsCadastros))]
        VAR_VALOR_DESCONTO_TEMPORARIO,
        [Display(Name = "VAR_DURACAO_MAX_DESCONTO", ResourceType = typeof(Resources.RsCadastros))]
        VAR_DURACAO_MAX_DESCONTO,
        [Display(Name = "VAR_DETALHES_COMBO", ResourceType = typeof(Resources.RsCadastros))]
        VAR_DETALHES_COMBO,
        [Display(Name = "VAR_COMPOSICAO_COMBO", ResourceType = typeof(Resources.RsCadastros))]
        VAR_COMPOSICAO_COMBO,
        //CARD 104 APOIO IMPLANTACAO
        [Display(Name = "VAR_LOGIN_RADIUS", ResourceType = typeof(Resources.RsCadastros))]
        VAR_LOGIN_RADIUS,
        [Display(Name = "VAR_SENHA_RADIUS", ResourceType = typeof(Resources.RsCadastros))]
        VAR_SENHA_RADIUS,       
        [Display(Name = "VAR_MODELO_CONTRATO", ResourceType = typeof(Resources.RsCadastros))]
        VAR_MODELO_CONTRATO,
        [Display(Name = "VAR_TIPO_PLANO", ResourceType = typeof(Resources.RsCadastros))]
        VAR_TIPO_PLANO,

        [Display(Name = "VAR_ENDERECO_PRINCIPAL_UF", ResourceType = typeof(Resources.RsCadastros))]
        VAR_ENDERECO_PRINCIPAL_UF,
        [Display(Name = "VAR_ENDERECO_PRINCIPAL_CIDADE", ResourceType = typeof(Resources.RsCadastros))]
        VAR_ENDERECO_PRINCIPAL_CIDADE,
        [Display(Name = "VAR_ENDERECO_PRINCIPAL_BAIRRO", ResourceType = typeof(Resources.RsCadastros))]
        VAR_ENDERECO_PRINCIPAL_BAIRRO,
        [Display(Name = "VAR_ENDERECO_PRINCIPAL_RUA", ResourceType = typeof(Resources.RsCadastros))]
        VAR_ENDERECO_PRINCIPAL_RUA,
        [Display(Name = "VAR_ENDERECO_PRINCIPAL_NUMERO", ResourceType = typeof(Resources.RsCadastros))]
        VAR_ENDERECO_PRINCIPAL_NUMERO,
        [Display(Name = "VAR_ENDERECO_PRINCIPAL_CEP", ResourceType = typeof(Resources.RsCadastros))]
        VAR_ENDERECO_PRINCIPAL_CEP,


        [Display(Name = "VAR_ENDERECO_COBRANCA_UF", ResourceType = typeof(Resources.RsCadastros))]
        VAR_ENDERECO_COBRANCA_UF,
        [Display(Name = "VAR_ENDERECO_COBRANCA_CIDADE", ResourceType = typeof(Resources.RsCadastros))]
        VAR_ENDERECO_COBRANCA_CIDADE,
        [Display(Name = "VAR_ENDERECO_COBRANCA_BAIRRO", ResourceType = typeof(Resources.RsCadastros))]
        VAR_ENDERECO_COBRANCA_BAIRRO,
        [Display(Name = "VAR_ENDERECO_COBRANCA_RUA", ResourceType = typeof(Resources.RsCadastros))]
        VAR_ENDERECO_COBRANCA_RUA,
        [Display(Name = "VAR_ENDERECO_COBRANCA_NUMERO", ResourceType = typeof(Resources.RsCadastros))]
        VAR_ENDERECO_COBRANCA_NUMERO,
        [Display(Name = "VAR_ENDERECO_COBRANCA_CEP", ResourceType = typeof(Resources.RsCadastros))]
        VAR_ENDERECO_COBRANCA_CEP,

        [Display(Name = "VAR_ENDERECO_INSTALACAO_UF", ResourceType = typeof(Resources.RsCadastros))]
        VAR_ENDERECO_INSTALACAO_UF,
        [Display(Name = "VAR_ENDERECO_INSTALACAO_CIDADE", ResourceType = typeof(Resources.RsCadastros))]
        VAR_ENDERECO_INSTALACAO_CIDADE,
        [Display(Name = "VAR_ENDERECO_INSTALACAO_BAIRRO", ResourceType = typeof(Resources.RsCadastros))]
        VAR_ENDERECO_INSTALACAO_BAIRRO,
        [Display(Name = "VAR_ENDERECO_INSTALACAO_RUA", ResourceType = typeof(Resources.RsCadastros))]
        VAR_ENDERECO_INSTALACAO_RUA,
        [Display(Name = "VAR_ENDERECO_INSTALACAO_NUMERO", ResourceType = typeof(Resources.RsCadastros))]
        VAR_ENDERECO_INSTALACAO_NUMERO,
        [Display(Name = "VAR_ENDERECO_INSTALACAO_CEP", ResourceType = typeof(Resources.RsCadastros))]
        VAR_ENDERECO_INSTALACAO_CEP,



    }
}
