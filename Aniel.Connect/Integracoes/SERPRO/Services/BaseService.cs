using Aniel.Connect.Integracoes.SERPRO.Models;
using Aniel.Connect.Integracoes.SERPRO.Models.Enums;
using System;
using System.Threading.Tasks;

namespace Aniel.Connect.Services.Services
{
    public abstract class BaseService
    {
        protected ValidacaoErros ValidarErrosResponse(int statusCode)
        {
            switch (statusCode)
            {
                case 200:
                    return new ValidacaoErros { Mensagem = "Validação dos dados foi realizado com sucesso!", Valido = true };
                case 206:
                    return new ValidacaoErros { Mensagem = "As informações foram retornadas, mas não completamente", Valido = false };
                case 400:
                    return new ValidacaoErros { Mensagem = "O número de CPF/CNPJ informado não é válido.", Valido = false };
                case 401:
                    return new ValidacaoErros { Mensagem = "Problemas durante a autenticação.", Valido = false };
                case 403:
                    return new ValidacaoErros { Mensagem = "Caminho errado na requisição", Valido = false };
                case 404:
                    return new ValidacaoErros { Mensagem = "Não existe CPF/CNPJ com o número de inscrição informado.", Valido = false };
                case 422:
                    return new ValidacaoErros { Mensagem = "Dados de menor de idade bloqueados em atendimento à Lei Geral de Proteção de Dados - LGPD.", Valido = false };
                case 500:
                    return new ValidacaoErros { Mensagem = "Ocorreu algum erro interno no Servidor.", Valido = false };
                case 504:
                    return new ValidacaoErros { Mensagem = "Ocorreu algum erro de rede e o gateway não respondeu a tempo. A requisição não chegou até a API Consulta CPF.", Valido = false };
                default:
                    return new ValidacaoErros { Mensagem = "Código de retorno não registrado", Valido = false };
            }
        }

        public SituacaoFisicaEnum ConsultarSituacaoFisica(string codigo)
        {
            switch (codigo)
            {
                case "0":
                    return SituacaoFisicaEnum.Regular;
                case "2":
                    return SituacaoFisicaEnum.Suspensa;
                case "3":
                    return SituacaoFisicaEnum.TitularFalecido;
                case "4":
                    return SituacaoFisicaEnum.PendenteRegularizacao;
                case "5":
                    return SituacaoFisicaEnum.CanceladaMultiplicidade;
                case "8":
                    return SituacaoFisicaEnum.Nula;
                case "9":
                    return SituacaoFisicaEnum.CanceladaOficio;
                default:
                    return SituacaoFisicaEnum.NaoIdentificado;
            }
        }

        public SituacaoJuridicaEnum ConsultarSituacaoJuridica(string codigo)
        {
            switch (codigo)
            {
                case "1":
                    return SituacaoJuridicaEnum.Nula;
                case "2":
                    return SituacaoJuridicaEnum.Ativa;
                case "3":
                    return SituacaoJuridicaEnum.Suspensa;
                case "4":
                    return SituacaoJuridicaEnum.Inapta;
                case "5":
                    return SituacaoJuridicaEnum.AtivaNaoRegular;
                case "8":
                    return SituacaoJuridicaEnum.Baixada;
                default:
                    return SituacaoJuridicaEnum.NaoIdentificado;
            }
        }
    }
}