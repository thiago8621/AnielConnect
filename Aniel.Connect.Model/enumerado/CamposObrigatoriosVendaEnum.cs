using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.enumerado
{
    public enum CamposObrigatoriosVendaEnum
    {

        [Display(Name = "CPF_RESPONSAVEL_EMPRESA", ResourceType = typeof(Resources.RsCadastros))]
        CPF_RESPONSAVEL_EMPRESA = 0,
        [Display(Name = "CARGO_RESPONSAVEL_EMPRESA", ResourceType = typeof(Resources.RsCadastros))]
        CARGO_RESPONSAVEL_EMPRESA = 1,
        [Display(Name = "CATEGORIA_CLIENTE", ResourceType = typeof(Resources.RsCadastros))]
        CATEGORIA_CLIENTE = 2,
        [Display(Name = "TELEFONECELULAR", ResourceType = typeof(Resources.RsCadastros))]
        TELEFONE_CELULAR = 3,
        [Display(Name = "CELULAR_REPONSAVEL", ResourceType = typeof(Resources.RsCadastros))]
        CELULAR_REPONSAVEL = 4,
        /// <summary>
        /// STRING_VAZIA: Usado para enum removido ou desativado
        /// </summary>
        [Display(Name = "COMO_FICOU_SABENDO", ResourceType = typeof(Resources.RsCadastros))]
        COMO_FICOU_SABENDO = 5,
        [Display(Name = "EMAIL_AUXILIAR", ResourceType = typeof(Resources.RsCadastros))]
        EMAIL_AUXILIAR = 6,
        [Display(Name = "EMAIL_PRINCIPAL", ResourceType = typeof(Resources.RsCadastros))]
        EMAIL_PRINCIPAL = 7,
        [Display(Name = "ESTADO_CIVIL", ResourceType = typeof(Resources.RsCadastros))]
        ESTADO_CIVIL = 8,
        [Display(Name = "GRAU_DE_INSTRUCAO", ResourceType = typeof(Resources.RsCadastros))]
        GRAU_DE_INSTRUCAO = 9,
        [Display(Name = "INSCRICAO_ESTADUAL", ResourceType = typeof(Resources.RsCadastros))]
        INSCRICAO_ESTADUAL = 10,
        [Display(Name = "INSCRICAO_MUNICIPAL", ResourceType = typeof(Resources.RsCadastros))]
        INSCRICAO_MUNICIPAL = 11,
        [Display(Name = "MEIO_CAPTACAO", ResourceType = typeof(Resources.RsCadastros))]
        MEIO_CAPTACAO = 12,
        [Display(Name = "NACIONALIDADE", ResourceType = typeof(Resources.RsCadastros))]
        NACIONALIDADE = 13,
        [Display(Name = "NOME_MAE", ResourceType = typeof(Resources.RsCadastros))]
        NOME_MAE = 14,
        [Display(Name = "NOME_PAI", ResourceType = typeof(Resources.RsCadastros))]
        NOME_PAI = 15,
        [Display(Name = "PESSOA_RECADO", ResourceType = typeof(Resources.RsCadastros))]
        PESSOA_RECADO = 16,
        [Display(Name = "PORTE", ResourceType = typeof(Resources.RsCadastros))]
        PORTE = 17,
        [Display(Name = "PROFISSAO", ResourceType = typeof(Resources.RsCadastros))]
        PROFISSAO = 18,
        [Display(Name = "RAMO_ATIVIDADE", ResourceType = typeof(Resources.RsCadastros))]
        RAMO_ATIVIDADE = 19,
        [Display(Name = "REPRESENTANTE", ResourceType = typeof(Resources.RsCadastros))]
        REPRESENTANTE = 20,
        [Display(Name = "RESPONSAVEL_FINANCEIRO", ResourceType = typeof(Resources.RsCadastros))]
        RESPONSAVEL_FINANCEIRO = 21,
        [Display(Name = "RG", ResourceType = typeof(Resources.RsCadastros))]
        RG = 22,
        [Display(Name = "SEXO", ResourceType = typeof(Resources.RsCadastros))]
        SEXO = 23,
        [Display(Name = "SITE", ResourceType = typeof(Resources.RsCadastros))]
        SITE = 24,
        [Display(Name = "TELEFONE1", ResourceType = typeof(Resources.RsCadastros))]
        TELEFONE1 = 25,
        /// <summary>
        /// STRING_VAZIA: Usado para enum removido ou desativado
        /// </summary>
        [Display(Name = "TELEFONE2", ResourceType = typeof(Resources.RsCadastros))]
        TELEFONE2 = 26,
        [Display(Name = "TELEFONE3", ResourceType = typeof(Resources.RsCadastros))]
        TELEFONE3 = 27,
        [Display(Name = "TELEFONERECADO", ResourceType = typeof(Resources.RsCadastros))]
        TELEFONERECADO = 28,
        [Display(Name = "NOMERECADO", ResourceType = typeof(Resources.RsCadastros))]
        NOMERECADO = 29,
        [Display(Name = "TIPO_IMOVEL", ResourceType = typeof(Resources.RsCadastros))]
        TIPO_IMOVEL = 30,
        [Display(Name = "DATANASCIMENTO", ResourceType = typeof(Resources.RsCadastros))]
        DATA_NASCIMENTO = 31,
        [Display(Name = "DATAEMISSAO", ResourceType = typeof(Resources.RsCadastros))]
        DATA_EMISSAO = 32,
        [Display(Name = "ORGAO", ResourceType = typeof(Resources.RsCadastros))]
        ORGAO = 33,
        [Display(Name = "PAIS", ResourceType = typeof(Resources.RsCadastros))]
        PAIS = 34,
        [Display(Name = "COMPLEMENTO", ResourceType = typeof(Resources.RsCadastros))]
        COMPLEMENTO = 35,
        [Display(Name = "PONTO_REFERENCIA", ResourceType = typeof(Resources.RsCadastros))]
        PONTO_REFERENCIA = 36,
        [Display(Name = "DATA_ABERTURA", ResourceType = typeof(Resources.RsCadastros))]
        DATA_ABERTURA = 37,
        [Display(Name = "CEP", ResourceType = typeof(Resources.RsCadastros))]
        CEP = 38,
        [Display(Name = "GRUPO_AREA", ResourceType = typeof(Resources.RsCadastros))]
        GRUPO_AREA = 39,
        [Display(Name = "AREA", ResourceType = typeof(Resources.RsCadastros))]
        AREA = 40,
        [Display(Name = "LOCALIDADE", ResourceType = typeof(Resources.RsCadastros))]
        LOCALIDADE = 41,
        [Display(Name = "BAIRRO", ResourceType = typeof(Resources.RsCadastros))]
        BAIRRO = 42,
    }
}
