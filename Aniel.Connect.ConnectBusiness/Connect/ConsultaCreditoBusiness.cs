using Aniel.Connect.ConnectBusiness.Exceptions.ConsultaCredito;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Integracoes.SerasaExperian.Models;
using Aniel.Connect.Integracoes.SerasaExperian.Services;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository;
using Aniel.Connect.Repository.Cliente;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.ConsultaCredito;
using Aniel.Connect.Repository.CriteriosParamCredito;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.ParametrosConsultaCredito;
using Aniel.Connect.Repository.ParametrosConsultaPreQual;
using Aniel.Connect.Repository.Venda;
using NetTopologySuite.CoordinateSystems;
using Newtonsoft.Json;
using Resources;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.SerasaApi;
using Sinapse.SerasaApi.Server.Responses;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aniel.Connect.ConnectBusiness.Connect
{
    public class ConsultaCreditoBusiness
    {
        private readonly IConsultaCreditoRepository _repository;
        private readonly ICriteriosParamCreditoRepository _repoCriterios;
        private readonly IParametrosConsultaCreditoRepository _repoParamsConsulta;

        private readonly IConfiguracoesGeraisRepository _repoConfig;

        private readonly string _usuarioApiSerasa;
        private readonly string _senhaApiSerasa;
        private readonly string _urlApiSerasa;

        public ConsultaCreditoBusiness(
            IConsultaCreditoRepository repository,
            ICriteriosParamCreditoRepository repoCriterios,
            IParametrosConsultaCreditoRepository repoParamsConsulta,
            IConfiguracoesGeraisRepository repoConfig,
            string usuarioApiSerasa,
            string senhaApiSerasa,
            string urlApiSerasa)
        {
            this._repository = repository;
            this._repoCriterios = repoCriterios;
            this._repoParamsConsulta = repoParamsConsulta;

            this._repoConfig = repoConfig;

            if (String.IsNullOrEmpty(usuarioApiSerasa))
            {
                throw new ArgumentNullException("usuarioApiSerasa");
            }

            if (String.IsNullOrEmpty(senhaApiSerasa))
            {
                throw new ArgumentNullException("senhaApiSerasa");
            }
            if (String.IsNullOrEmpty(urlApiSerasa))
            {
                throw new ArgumentNullException("urlApiSerasa");
            }

            if (String.IsNullOrEmpty(urlApiSerasa))
            {
                throw new ArgumentNullException("urlApiSerasa");
            }

            this._usuarioApiSerasa = usuarioApiSerasa;
            this._senhaApiSerasa = senhaApiSerasa;
            this._urlApiSerasa = urlApiSerasa;
        }

        public ConsultaCreditoBusiness(
            IConsultaCreditoRepository repository,
            ICriteriosParamCreditoRepository repoCriterios,
            IParametrosConsultaCreditoRepository repoParamsConsulta,
            IConfiguracoesGeraisRepository repoConfig)
        {
            this._repository = repository;
            this._repoCriterios = repoCriterios;
            this._repoParamsConsulta = repoParamsConsulta;

            this._repoConfig = repoConfig;
        }

        /// <summary>
        ///     Chama a API de consulta de crédito do Serasa para pessoa física, 
        ///     grava o resultado da consulta em banco e retorna a string da
        ///     consulta do Serasa.
        /// </summary>
        /// <param name="cpf">CPF da pessoa física que será consultada</param>
        public async Task<ConsultaSerasaDados> ExecutarConsultaCreditoCpf(string cpf, string usuarioExecucao = null)
        {
            if (String.IsNullOrEmpty(this._usuarioApiSerasa))
            {
                throw new UsuarioApiSerasaNaoInformadoException();
            }

            if (String.IsNullOrEmpty(this._senhaApiSerasa))
            {
                throw new SenhaApiSerasaNaoInformadoException();
            }

            string retornoConsultaSerasa =
                await new SerasaDados().RetornoDadosCpf(
                    cpf,
                    this._usuarioApiSerasa,
                    this._senhaApiSerasa,
                    this._urlApiSerasa
                );

            if (String.IsNullOrEmpty(retornoConsultaSerasa))
            {
                throw new ConsultaSerasaSemRetornoException();
            }

            var consultaSerasa = new ConsultaSerasaDados
            {
                UsuarioCriacao = usuarioExecucao,
                DataHoraCriacao = DateTime.Now,
                consulta = retornoConsultaSerasa,
                cpf = cpf,
                TipoIntegracao = IntegracaoExternaConsultaCredEnum.SERASA
            };

            this._repository.Save(consultaSerasa);

            return consultaSerasa;
        }

        /// <summary>
        ///    Realiza consultas de crédito do Serasa para pessoa física na API SERASA EXPERIAN
        /// </summary>
        /// <param name="cpf">CPF da pessoa física que será consultada</param>
        public async Task<ConsultaSerasaDados> ConsultarSerasaPF(string cpf, string authToken, string urlBase)
        {
            if (String.IsNullOrEmpty(this._usuarioApiSerasa))
                throw new UsuarioApiSerasaNaoInformadoException();

            if (String.IsNullOrEmpty(this._senhaApiSerasa))
                throw new SenhaApiSerasaNaoInformadoException();

            var consultaService = new ConsultarSerasaService(authToken, urlBase,CONSTANTES.SerasaExperianReportParameterPJ);
            var consultaResponse = await consultaService.ConsultarSerasaPF(cpf);

            var consultaSerasa = new ConsultaSerasaDados();
            consultaSerasa.DataHoraCriacao = DateTime.Now;
            consultaSerasa.consulta = JsonService.SerializeObject(consultaResponse);
            consultaSerasa.cpf = cpf;
            consultaSerasa.TipoIntegracao = IntegracaoExternaConsultaCredEnum.SERASA_EXPERIAN;

            this._repository.Save(consultaSerasa);
            return consultaSerasa;
        }

        /// <summary>
        ///    Realiza consultas de crédito do Serasa para pessoa jurídica na API SERASA EXPERIAN
        /// </summary>
        /// <param name="cnpj">CNPJ da pessoa física que será consultada</param>
        public async Task<ConsultaSerasaDados> ConsultarSerasaPJ(string cnpj, string authToken, string urlBase)
        {
            if (String.IsNullOrEmpty(this._usuarioApiSerasa))
                throw new UsuarioApiSerasaNaoInformadoException();

            if (String.IsNullOrEmpty(this._senhaApiSerasa))
                throw new SenhaApiSerasaNaoInformadoException();

            var consultaService = new ConsultarSerasaService(authToken, urlBase,CONSTANTES.SerasaExperianReportParameterPJ);
            var consultaResponse = await consultaService.ConsultarSerasaPJ(cnpj);

            if (consultaResponse == null || consultaResponse?.Erros?.Mensagem != null)
            {
                CONSTANTES.log4Net.Error(consultaResponse?.Erros?.Mensagem ?? "consultaResponse null");
                throw new Exception("Erro ao consultar crédito do Serasa Experian para pessoa jurídica");
            }

            var consultaSerasa = new ConsultaSerasaDados();
            consultaSerasa.DataHoraCriacao = DateTime.Now;
            consultaSerasa.consulta = JsonService.SerializeObject(consultaResponse);
            consultaSerasa.cnpj = cnpj;
            consultaSerasa.TipoIntegracao = IntegracaoExternaConsultaCredEnum.SERASA_EXPERIAN;

            this._repository.Save(consultaSerasa);
            return consultaSerasa;
        }

        /// <summary>
        ///    Obtem o Token de autenticação para realizar consultas de crédito na API SERASA EXPERIAN
        /// </summary>
        /// <param name="cpf">CPF da pessoa física que será consultada</param>
        public async Task<string> ObterTokenAuth(string urlBase)
        {
            if (String.IsNullOrEmpty(this._usuarioApiSerasa))
                throw new UsuarioApiSerasaNaoInformadoException();

            if (String.IsNullOrEmpty(this._senhaApiSerasa))
                throw new SenhaApiSerasaNaoInformadoException();

            var authService = new AuthService(this._usuarioApiSerasa, this._senhaApiSerasa, urlBase);
            var authToken = await authService.ObterTokenAuth();

            if (authToken == null && authToken?.Erros?.Mensagem != null)
                throw new Exception("Erro ao obter token de autenticação da API Serasa Experian");

            return authToken.accessToken;
        }

        /// <summary>
        ///     Chama a API de consulta de crédito do Serasa para pessoa jurídica, 
        ///     grava o resultado da consulta em banco e retorna a string da
        ///     consulta do Serasa.
        /// </summary>
        /// <param name="cnpj">CNPJ da pessoa jurídica que será consultada</param>
        public async Task<ConsultaSerasaDados> ExecutarConsultaCreditoCnpj(string cnpj,string usuarioExecucao = null)
        {
            if (String.IsNullOrEmpty(this._usuarioApiSerasa))
            {
                throw new UsuarioApiSerasaNaoInformadoException();
            }

            if (String.IsNullOrEmpty(this._senhaApiSerasa))
            {
                throw new SenhaApiSerasaNaoInformadoException();
            }

            string retornoConsultaSerasa = await new SerasaDados().RetornoDadosCnpj(
                cnpj,
                this._usuarioApiSerasa,
                this._senhaApiSerasa,
                this._urlApiSerasa
            );

            if (String.IsNullOrEmpty(retornoConsultaSerasa))
            {
                throw new ConsultaSerasaSemRetornoException();
            }

            var consultaSerasa = new ConsultaSerasaDados
            {
                UsuarioCriacao = usuarioExecucao,
                DataHoraCriacao = DateTime.Now,
                consulta = retornoConsultaSerasa,
                cnpj = cnpj,
                TipoIntegracao = IntegracaoExternaConsultaCredEnum.SERASA
            };

            this._repository.Save(consultaSerasa);

            return consultaSerasa;
        }

        /// <summary>
        ///     Pega o total de pontos de uma determinada consulta de crédito para uma pessoa física
        /// </summary>
        /// <param name="id">ID da consulta de crédito</param>
        /// <param name="cpf">CPF da pessoa física que será consultada</param>
        public int GetTotalPontosConsultaCpf(long id, string cpf)
        {
            ConsultaSerasaDados consultaSerasa = this._repository.GetById(id);

            int pontos = 0;

            if (consultaSerasa == null)
            {
                throw new ConsultaSerasaNaoEncontradaException(cpf);
            }

            if (consultaSerasa.TipoIntegracao == IntegracaoExternaConsultaCredEnum.PREQUAL)
            {
                return pontos;
            }
            else if (consultaSerasa.TipoIntegracao == IntegracaoExternaConsultaCredEnum.SERASA_EXPERIAN)
            {
                if (consultaSerasa == null)
                    throw new ConsultaSerasaNaoEncontradaException(cpf);

                var serasaResponse = JsonService.DeserializeObject<ConsultaSerasaResponse>(consultaSerasa.consulta);

                int scoreSerasa = 0;
                double debitosTotais = 0;
                int dataUltimoDebito = 0;
                double protestoTotais = 0;
                Report infosCliSerasa = null;
                string dataUltimoDebitoString = string.Empty;

                if (serasaResponse != null)
                    infosCliSerasa = serasaResponse.reports.FirstOrDefault();

                if (infosCliSerasa.negativeData.pefin != null) // Pendência financeira
                {
                    if (infosCliSerasa?.negativeData?.pefin?.summary?.balance != null)
                        debitosTotais += infosCliSerasa.negativeData.pefin.summary.balance;

                    dataUltimoDebitoString = infosCliSerasa.negativeData.pefin.summary.firstOccurrence; //debito mais recente

                    if (!String.IsNullOrEmpty(dataUltimoDebitoString))
                    {
                        Int32.TryParse(dataUltimoDebitoString, out dataUltimoDebito);
                    }
                }

                if (serasaResponse.reports.FirstOrDefault() != null)
                    scoreSerasa += serasaResponse.reports.FirstOrDefault().score.score;


                if (infosCliSerasa.negativeData.notary != null)//Valor total dos protestos
                {
                    if (infosCliSerasa?.negativeData?.notary?.summary?.balance != null)
                        protestoTotais += infosCliSerasa.negativeData.notary.summary.balance;
                }

                IEnumerable<CriteriosParamCredito> criterios = this._repoCriterios.GetAll().ToArray();

                foreach (CriteriosParamCredito criterio in criterios)
                {
                    var operador = (EnumOperadores)criterio.TipoOperador;
                    var dataParametro = DateTime.Now.AddDays(-criterio.Valor); //valor marcado como dias (refencia data atual - dias marcados);
                    bool criterioEliminatorioReprovado = false;

                    if (criterio.TipoCriterio == (int)CriteriosParametrosCredito.SCORE_POSITIVO)
                    {
                        switch (operador)
                        {
                            case EnumOperadores.MaiorQue:
                                {
                                    if (scoreSerasa > criterio.Valor)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                            case EnumOperadores.MenorQue:
                                {
                                    if (scoreSerasa < criterio.Valor)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                            case EnumOperadores.MaiorouIgual:
                                {
                                    if (scoreSerasa >= criterio.Valor)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                            case EnumOperadores.MenorouIgual:
                                {
                                    if (scoreSerasa <= criterio.Valor)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                        }
                    }
                    else if (criterio.TipoCriterio == (int)CriteriosParametrosCredito.ULTIMA_DATA_PENDENCIA)
                    {
                        switch (operador)
                        {
                            case EnumOperadores.MaiorQue:
                                {
                                    if (!string.IsNullOrEmpty(dataUltimoDebitoString))
                                    {
                                        var dataUltimoDeb = Convert.ToDateTime(dataUltimoDebitoString);

                                        if (dataUltimoDeb > dataParametro)
                                        {
                                            pontos += Convert.ToInt32(criterio.Pontos);
                                        }
                                        else if (criterio.Eliminatorio)
                                            criterioEliminatorioReprovado = true;
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                            case EnumOperadores.MenorQue:
                                {

                                    if (!string.IsNullOrEmpty(dataUltimoDebitoString))
                                    {
                                        var dataUltimoDeb = Convert.ToDateTime(dataUltimoDebitoString);

                                        if (dataUltimoDeb < dataParametro)
                                        {
                                            pontos += Convert.ToInt32(criterio.Pontos);
                                        }
                                        else if (criterio.Eliminatorio)
                                            criterioEliminatorioReprovado = true;
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                            case EnumOperadores.MaiorouIgual:
                                {
                                    if (!string.IsNullOrEmpty(dataUltimoDebitoString))
                                    {
                                        var dataUltimoDeb = Convert.ToDateTime(dataUltimoDebitoString);

                                        if (dataUltimoDeb >= dataParametro)
                                        {
                                            pontos += Convert.ToInt32(criterio.Pontos);
                                        }
                                        else if (criterio.Eliminatorio)
                                            criterioEliminatorioReprovado = true;
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                            case EnumOperadores.MenorouIgual:
                                {
                                    if (!string.IsNullOrEmpty(dataUltimoDebitoString))
                                    {
                                        var dataUltimoDeb = Convert.ToDateTime(dataUltimoDebitoString);

                                        if (dataUltimoDeb <= dataParametro)
                                        {
                                            pontos += Convert.ToInt32(criterio.Pontos);
                                        }
                                        else if (criterio.Eliminatorio)
                                            criterioEliminatorioReprovado = true;
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                            case EnumOperadores.Igual:
                                {
                                    if (!string.IsNullOrEmpty(dataUltimoDebitoString))
                                    {
                                        var dataUltimoDeb = Convert.ToDateTime(dataUltimoDebitoString);

                                        if (dataUltimoDeb == dataParametro)
                                        {
                                            pontos += Convert.ToInt32(criterio.Pontos);
                                        }
                                        else if (criterio.Eliminatorio)
                                            criterioEliminatorioReprovado = true;
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                        }
                    }
                    else if (criterio.TipoCriterio == (int)CriteriosParametrosCredito.VALOR_TOTAL_DEBITO)
                    {
                        switch (operador)
                        {
                            case EnumOperadores.MaiorQue:
                                {
                                    if (debitosTotais > criterio.Valor)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                            case EnumOperadores.MenorQue:
                                {
                                    if (debitosTotais < criterio.Valor)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                            case EnumOperadores.MaiorouIgual:
                                {
                                    if (debitosTotais >= criterio.Valor)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                            case EnumOperadores.MenorouIgual:
                                {
                                    if (debitosTotais <= criterio.Valor)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                        }
                    }
                    else if (criterio.TipoOperador == (int)CriteriosParametrosCredito.PROTESTO)
                    {
                        switch (operador)
                        {
                            case EnumOperadores.MaiorQue:
                                {
                                    if (protestoTotais > criterio.Valor)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                            case EnumOperadores.MenorQue:
                                {
                                    if (protestoTotais < criterio.Valor)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                            case EnumOperadores.MaiorouIgual:
                                {
                                    if (protestoTotais >= criterio.Valor)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                            case EnumOperadores.MenorouIgual:
                                {
                                    if (protestoTotais <= criterio.Valor)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                        }
                    }
                    if (criterioEliminatorioReprovado)
                    {
                        // Caso algum critério eliminatório tenha sido reprovado retornamos o Int32.MinValue
                        // como um indicador que a soma dos pontos é irrelevante devido à quebra de um critério 
                        // eliminatório
                        return Int32.MinValue;
                    }
                }


                return pontos;
            }
            else
            {

                //Condição comentada, foi ajustado para verificar se o CPF é sem titular na API do Premier 
                /*if (consultaSerasa.consulta.Contains("CPF SEM TITULAR"))
                    throw new Exception("CPF SEM TITULAR");
                else*/

                int scoreSerasa = 0;
                double debitosTotais = 0;
                int dataUltimoDebito = 0;
                double protestoTotais = 0;

                if (consultaSerasa.consulta.Contains("N200"))
                {
                    int indexBloco = consultaSerasa.consulta.IndexOf("N200");
                    int tamanhoBloco = 115;

                    string blocoN200 = consultaSerasa.consulta.Substring(indexBloco, tamanhoBloco);

                    // Removendo espaço em branco da string do score para evitar erro ao converter para int
                    var statusPessoa = blocoN200.Substring(84, 2);

                    var situacao = this.GetSituacaoDocumentoSerasa(cpf, Convert.ToInt32(statusPessoa));

                    if (blocoN200.Contains("MENOR DE IDADE PROTEGIDO LEGALMENTE"))
                        throw new ConsultaCreditoMenoridadeException();

                    if (situacao != HtmlEnumExtensions.GetEnumDescription(SituacaoSerasaPessoaFisica.Regular))
                    {
                        throw new Exception($"CPF IRREGULAR");
                    }
                }
                if (consultaSerasa.consulta.Contains("F900"))
                {
                    int indexBloco = consultaSerasa.consulta.IndexOf("F900");
                    int tamanhoBloco = 115;

                    string blocoF900 = consultaSerasa.consulta.Substring(indexBloco, tamanhoBloco);

                    // Removendo espaço em branco da string do score para evitar erro ao converter para int
                    string scoreSerasaString = blocoF900.Substring(9, 5).Trim();

                    if (!String.IsNullOrEmpty(scoreSerasaString) && Int32.TryParse(scoreSerasaString, out scoreSerasa))
                    {
                        scoreSerasa = Convert.ToInt32(scoreSerasaString);
                    }
                }

                if (consultaSerasa.consulta.Contains("N240")) // Pendência financeira
                {
                    //Verifica quantos indexes N24090 existem
                    var listaIndex = StringService.GetAllIndexes(consultaSerasa.consulta, "N24090");

                    if (listaIndex.Any())
                    {
                        foreach (var index in listaIndex)
                        {
                            int tamBlocoIndex90 = 115;
                            string resultIndex90 = consultaSerasa.consulta.Substring(index, tamBlocoIndex90);


                            // Removendo espaço em branco da string do score para evitar erro ao converter para int
                            string debitosTotaisString = resultIndex90.Substring(24, 14).Trim();

                            if (!String.IsNullOrEmpty(debitosTotaisString))
                            {
                                debitosTotais = debitosTotais + this.ConvertPendenciaString(debitosTotaisString).Objeto;
                            }
                        }
                    }

                    int indexN240 = consultaSerasa.consulta.IndexOf("N240");
                    int tamBloco = 21;
                    string result = consultaSerasa.consulta.Substring(indexN240, tamBloco);
                    // Removendo espaço em branco da string do score para evitar erro ao converter para int
                    string dataUltimoDebitoString = result.Substring(6, 8).Trim();

                    if (!String.IsNullOrEmpty(dataUltimoDebitoString))
                    {
                        Int32.TryParse(dataUltimoDebitoString, out dataUltimoDebito);
                    }

                    debitosTotais = debitosTotais / 100;
                }

                if (consultaSerasa.consulta.Contains("N25090"))//Valor total dos protestos
                {
                    var listaIndexProtesto = StringService.GetAllIndexes(consultaSerasa.consulta, "N25090");

                    if (listaIndexProtesto.Any())
                    {
                        foreach (var indexPro in listaIndexProtesto)
                        {
                            int tamBlocoIndexPro = 115;
                            string resultIndexPro = consultaSerasa.consulta.Substring(indexPro, tamBlocoIndexPro);

                            // Removendo espaço em branco da string do score para evitar erro ao converter para int
                            string protestosTotaisString = resultIndexPro.Substring(25, 16).Trim();

                            if (!String.IsNullOrEmpty(protestosTotaisString))
                            {
                                protestoTotais = protestoTotais + this.ConvertPendenciaString(protestosTotaisString).Objeto;
                            }
                        }
                    }

                    //int indexN250 = consultaSerasa.consulta.IndexOf("N250");
                    //int tamBloco250 = 21;
                    //string result250 = consultaSerasa.consulta.Substring(indexN250, tamBloco250);
                    //// Removendo espaço em branco da string do score para evitar erro ao converter para int
                    //string dataUltimoDebitoProtestoString = result250.Substring(6, 8).Trim();

                    //if (!String.IsNullOrEmpty(dataUltimoDebitoProtestoString))
                    //{
                    //    dataUltimoDebitoProtesto = Convert.ToInt32(dataUltimoDebitoProtestoString);
                    //}

                    protestoTotais = protestoTotais / 100;
                }

                IEnumerable<CriteriosParamCredito> criterios = this._repoCriterios.GetAll().ToArray();

                foreach (CriteriosParamCredito criterio in criterios)
                {
                    var operador = (EnumOperadores)criterio.TipoOperador;

                    bool criterioEliminatorioReprovado = false;

                    if (criterio.TipoCriterio == (int)CriteriosParametrosCredito.SCORE_POSITIVO)
                    {
                        switch (operador)
                        {
                            case EnumOperadores.MaiorQue:
                                {
                                    if (scoreSerasa > criterio.Valor)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                            case EnumOperadores.MenorQue:
                                {
                                    if (scoreSerasa < criterio.Valor)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                            case EnumOperadores.MaiorouIgual:
                                {
                                    if (scoreSerasa >= criterio.Valor)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                            case EnumOperadores.MenorouIgual:
                                {
                                    if (scoreSerasa <= criterio.Valor)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                        }
                    }
                    else if (criterio.TipoCriterio == (int)CriteriosParametrosCredito.ULTIMA_DATA_PENDENCIA)
                    {
                        switch (operador)
                        {
                            case EnumOperadores.MaiorQue:
                                {
                                    if (dataUltimoDebito > criterio.Valor)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                            case EnumOperadores.MenorQue:
                                {
                                    if (dataUltimoDebito < criterio.Valor)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                            case EnumOperadores.MaiorouIgual:
                                {
                                    if (dataUltimoDebito >= criterio.Valor)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                            case EnumOperadores.MenorouIgual:
                                {
                                    if (dataUltimoDebito <= criterio.Valor)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                        }
                    }
                    else if (criterio.TipoCriterio == (int)CriteriosParametrosCredito.VALOR_TOTAL_DEBITO)
                    {
                        switch (operador)
                        {
                            case EnumOperadores.MaiorQue:
                                {
                                    if (debitosTotais > criterio.Valor)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                            case EnumOperadores.MenorQue:
                                {
                                    if (debitosTotais < criterio.Valor)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                            case EnumOperadores.MaiorouIgual:
                                {
                                    if (debitosTotais >= criterio.Valor)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                            case EnumOperadores.MenorouIgual:
                                {
                                    if (debitosTotais <= criterio.Valor)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                        }
                    }
                    else if (criterio.TipoOperador == (int)CriteriosParametrosCredito.PROTESTO)
                    {
                        switch (operador)
                        {
                            case EnumOperadores.MaiorQue:
                                {
                                    if (protestoTotais > criterio.Valor)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                            case EnumOperadores.MenorQue:
                                {
                                    if (protestoTotais < criterio.Valor)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                            case EnumOperadores.MaiorouIgual:
                                {
                                    if (protestoTotais >= criterio.Valor)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                            case EnumOperadores.MenorouIgual:
                                {
                                    if (protestoTotais <= criterio.Valor)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                        }
                    }
                    if (criterioEliminatorioReprovado)
                    {
                        // Caso algum critério eliminatório tenha sido reprovado retornamos o Int32.MinValue
                        // como um indicador que a soma dos pontos é irrelevante devido à quebra de um critério 
                        // eliminatório
                        return Int32.MinValue;
                    }
                }
            }

            return pontos;
        }

        /// <summary>
        ///     Pega o total de pontos de uma determinada consulta de crédito para uma pessoa física
        /// </summary>
        /// <param name="id">ID da consulta de crédito</param>
        /// <param name="cpf">CPF da pessoa física que será consultada</param>
        public int GetTotalPontosSerasaExpCpf(long id, string cpf, ConsultaSerasaResponse serasaResponse)
        {
            ConsultaSerasaDados consultaSerasa = this._repository.GetById(id);

            int pontos = 0;

            if (consultaSerasa == null)
                throw new ConsultaSerasaNaoEncontradaException(cpf);

            int scoreSerasa = 0;
            double debitosTotais = 0;
            double protestoTotais = 0;
            Report infosCliSerasa = null;
            string dataUltimoDebitoString = string.Empty;

            if (serasaResponse.reports == null)
                return pontos;

            if (serasaResponse != null)
                infosCliSerasa = serasaResponse.reports.FirstOrDefault();

            if (infosCliSerasa.negativeData.pefin != null) // Pendência financeira
            {
                if (infosCliSerasa?.negativeData?.pefin?.summary?.balance != null)
                    debitosTotais += infosCliSerasa.negativeData.pefin.summary.balance;

                dataUltimoDebitoString = infosCliSerasa.negativeData.pefin.summary.firstOccurrence; //debito mais recente
            }

            if (serasaResponse.reports.FirstOrDefault() != null)
                scoreSerasa += serasaResponse.reports.FirstOrDefault().score.score;


            if (infosCliSerasa.negativeData.notary != null)//Valor total dos protestos
            {
                if (infosCliSerasa?.negativeData?.notary?.summary?.balance != null)
                    protestoTotais += infosCliSerasa.negativeData.notary.summary.balance;
            }

            IEnumerable<CriteriosParamCredito> criterios = this._repoCriterios.GetAll().ToArray();

            foreach (CriteriosParamCredito criterio in criterios)
            {
                var operador = (EnumOperadores)criterio.TipoOperador;

                var dataParametro = DateTime.Now.AddDays(-criterio.Valor); //valor marcado como dias (refencia data atual - dias marcados);

                bool criterioEliminatorioReprovado = false;

                if (criterio.TipoCriterio == (int)CriteriosParametrosCredito.SCORE_POSITIVO)
                {
                    switch (operador)
                    {
                        case EnumOperadores.MaiorQue:
                            {
                                if (scoreSerasa > criterio.Valor)
                                {
                                    pontos += Convert.ToInt32(criterio.Pontos);
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                        case EnumOperadores.MenorQue:
                            {
                                if (scoreSerasa < criterio.Valor)
                                {
                                    pontos += Convert.ToInt32(criterio.Pontos);
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                        case EnumOperadores.MaiorouIgual:
                            {
                                if (scoreSerasa >= criterio.Valor)
                                {
                                    pontos += Convert.ToInt32(criterio.Pontos);
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                        case EnumOperadores.MenorouIgual:
                            {
                                if (scoreSerasa <= criterio.Valor)
                                {
                                    pontos += Convert.ToInt32(criterio.Pontos);
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                        case EnumOperadores.Igual:
                            {
                                if (scoreSerasa == criterio.Valor)
                                {
                                    pontos += Convert.ToInt32(criterio.Pontos);
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                    }
                }
                else if (criterio.TipoCriterio == (int)CriteriosParametrosCredito.ULTIMA_DATA_PENDENCIA)
                {
                    switch (operador)
                    {
                        case EnumOperadores.MaiorQue:
                            {                          
                                if (!string.IsNullOrEmpty(dataUltimoDebitoString))
                                {
                                    var dataUltimoDeb = Convert.ToDateTime(dataUltimoDebitoString);

                                    if (dataUltimoDeb > dataParametro)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                        criterioEliminatorioReprovado = true;
                                }                               
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                        case EnumOperadores.MenorQue:
                            {

                                if (!string.IsNullOrEmpty(dataUltimoDebitoString))
                                {
                                    var dataUltimoDeb = Convert.ToDateTime(dataUltimoDebitoString);

                                    if (dataUltimoDeb < dataParametro)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                        criterioEliminatorioReprovado = true;
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                        case EnumOperadores.MaiorouIgual:
                            {
                                if (!string.IsNullOrEmpty(dataUltimoDebitoString))
                                {
                                    var dataUltimoDeb = Convert.ToDateTime(dataUltimoDebitoString);

                                    if (dataUltimoDeb >= dataParametro)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                        criterioEliminatorioReprovado = true;
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                        case EnumOperadores.MenorouIgual:
                            {
                                if (!string.IsNullOrEmpty(dataUltimoDebitoString))
                                {
                                    var dataUltimoDeb = Convert.ToDateTime(dataUltimoDebitoString);

                                    if (dataUltimoDeb <= dataParametro)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                        criterioEliminatorioReprovado = true;
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                        case EnumOperadores.Igual:
                            {
                                if (!string.IsNullOrEmpty(dataUltimoDebitoString))
                                {
                                    var dataUltimoDeb = Convert.ToDateTime(dataUltimoDebitoString);

                                    if (dataUltimoDeb == dataParametro)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                        criterioEliminatorioReprovado = true;
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                    }
                }
                else if (criterio.TipoCriterio == (int)CriteriosParametrosCredito.VALOR_TOTAL_DEBITO)
                {
                    switch (operador)
                    {
                        case EnumOperadores.MaiorQue:
                            {
                                if (debitosTotais > criterio.Valor)
                                {
                                    pontos += Convert.ToInt32(criterio.Pontos);
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                        case EnumOperadores.MenorQue:
                            {
                                if (debitosTotais < criterio.Valor)
                                {
                                    pontos += Convert.ToInt32(criterio.Pontos);
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                        case EnumOperadores.MaiorouIgual:
                            {
                                if (debitosTotais >= criterio.Valor)
                                {
                                    pontos += Convert.ToInt32(criterio.Pontos);
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                        case EnumOperadores.MenorouIgual:
                            {
                                if (debitosTotais <= criterio.Valor)
                                {
                                    pontos += Convert.ToInt32(criterio.Pontos);
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                        case EnumOperadores.Igual:
                            {
                                if (debitosTotais == criterio.Valor)
                                {
                                    pontos += Convert.ToInt32(criterio.Pontos);
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                    }
                }
                else if (criterio.TipoOperador == (int)CriteriosParametrosCredito.PROTESTO)
                {
                    switch (operador)
                    {
                        case EnumOperadores.MaiorQue:
                            {
                                if (protestoTotais > criterio.Valor)
                                {
                                    pontos += Convert.ToInt32(criterio.Pontos);
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                        case EnumOperadores.MenorQue:
                            {
                                if (protestoTotais < criterio.Valor)
                                {
                                    pontos += Convert.ToInt32(criterio.Pontos);
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                        case EnumOperadores.MaiorouIgual:
                            {
                                if (protestoTotais >= criterio.Valor)
                                {
                                    pontos += Convert.ToInt32(criterio.Pontos);
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                        case EnumOperadores.MenorouIgual:
                            {
                                if (protestoTotais <= criterio.Valor)
                                {
                                    pontos += Convert.ToInt32(criterio.Pontos);
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                        case EnumOperadores.Igual:
                            {
                                if (protestoTotais == criterio.Valor)
                                {
                                    pontos += Convert.ToInt32(criterio.Pontos);
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                    }
                }
                if (criterioEliminatorioReprovado)
                {
                    // Caso algum critério eliminatório tenha sido reprovado retornamos o Int32.MinValue
                    // como um indicador que a soma dos pontos é irrelevante devido à quebra de um critério 
                    // eliminatório
                    return Int32.MinValue;
                }
            }


            return pontos;
        }

        /// <summary>
        ///     Pega o total de pontos de uma pessoa jurídica que já teve sua consulta de crédito
        ///     realizada pelo sistema.
        /// </summary>
        /// <param name="cnpj">CNPJ da pessoa jurídica que será consultada</param>
        public int GetTotalPontosSerasaExpCnpj(long id, string cpf, ConsultaSerasaResponse serasaResponse)
        {
            ConsultaSerasaDados consultaSerasa = this._repository.GetById(id);

            int pontos = 0;

            if (consultaSerasa == null)
                throw new ConsultaSerasaNaoEncontradaException(cpf);

            Report infosCliSerasa = null;

            if (serasaResponse != null)
                infosCliSerasa = serasaResponse.reports.FirstOrDefault();

            int scoreSerasa = 0;
            double debitosTotais = 0;
            string dataUltimoDebitoString = string.Empty;
            double protestoTotais = 0;

            if (infosCliSerasa.negativeData.pefin != null) // Pendência financeira
            {
                if (infosCliSerasa?.negativeData?.pefin?.summary?.balance != null)
                    debitosTotais += infosCliSerasa.negativeData.pefin.summary.balance;

                dataUltimoDebitoString = infosCliSerasa.negativeData.pefin.summary.firstOccurrence; //debito mais recente
            }

            if (serasaResponse.reports.FirstOrDefault() != null)
                scoreSerasa += serasaResponse.reports.FirstOrDefault().score.score;


            if (infosCliSerasa.negativeData.notary != null)//Valor total dos protestos
            {
                if (infosCliSerasa?.negativeData?.notary?.summary?.balance != null)
                    protestoTotais += infosCliSerasa.negativeData.notary.summary.balance;
            }

            IEnumerable<Sinapse.Model.CriteriosParamCredito> criterios = this._repoCriterios.GetAll();

            foreach (CriteriosParamCredito criterio in criterios)
            {
                var operador = (EnumOperadores)criterio.TipoOperador;
                var dataParametro = DateTime.Now.AddDays(-criterio.Valor); //valor marcado como dias (refencia data atual - dias marcados);
                bool criterioEliminatorioReprovado = false;

                if (criterio.TipoCriterio == (int)CriteriosParametrosCredito.SCORE_POSITIVO)
                {
                    switch (operador)
                    {
                        case EnumOperadores.MaiorQue:
                            {
                                if (scoreSerasa > criterio.Valor)
                                {
                                    pontos += Convert.ToInt32(criterio.Pontos);
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                        case EnumOperadores.MenorQue:
                            {
                                if (scoreSerasa < criterio.Valor)
                                {
                                    pontos += Convert.ToInt32(criterio.Pontos);
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                        case EnumOperadores.MaiorouIgual:
                            {
                                if (scoreSerasa >= criterio.Valor)
                                {
                                    pontos += Convert.ToInt32(criterio.Pontos);
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                        case EnumOperadores.MenorouIgual:
                            {
                                if (scoreSerasa <= criterio.Valor)
                                {
                                    pontos += Convert.ToInt32(criterio.Pontos);
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                    }
                }
                else if (criterio.TipoOperador == (int)CriteriosParametrosCredito.PROTESTO)
                {
                    switch (operador)
                    {
                        case EnumOperadores.MaiorQue:
                            {
                                if (protestoTotais > criterio.Valor)
                                {
                                    pontos += Convert.ToInt32(criterio.Pontos);
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                        case EnumOperadores.MenorQue:
                            {
                                if (protestoTotais < criterio.Valor)
                                {
                                    pontos += Convert.ToInt32(criterio.Pontos);
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                        case EnumOperadores.MaiorouIgual:
                            {
                                if (protestoTotais >= criterio.Valor)
                                {
                                    pontos += Convert.ToInt32(criterio.Pontos);
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                        case EnumOperadores.MenorouIgual:
                            {
                                if (protestoTotais <= criterio.Valor)
                                {
                                    pontos += Convert.ToInt32(criterio.Pontos);
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                    }
                }
                else if (criterio.TipoCriterio == (int)CriteriosParametrosCredito.ULTIMA_DATA_PENDENCIA)
                {
                    switch (operador)
                    {
                        case EnumOperadores.MaiorQue:
                            {
                                if (!string.IsNullOrEmpty(dataUltimoDebitoString))
                                {
                                    var dataUltimoDeb = Convert.ToDateTime(dataUltimoDebitoString);

                                    if (dataUltimoDeb > dataParametro)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                        criterioEliminatorioReprovado = true;
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                        case EnumOperadores.MenorQue:
                            {

                                if (!string.IsNullOrEmpty(dataUltimoDebitoString))
                                {
                                    var dataUltimoDeb = Convert.ToDateTime(dataUltimoDebitoString);

                                    if (dataUltimoDeb < dataParametro)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                        criterioEliminatorioReprovado = true;
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                        case EnumOperadores.MaiorouIgual:
                            {
                                if (!string.IsNullOrEmpty(dataUltimoDebitoString))
                                {
                                    var dataUltimoDeb = Convert.ToDateTime(dataUltimoDebitoString);

                                    if (dataUltimoDeb >= dataParametro)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                        criterioEliminatorioReprovado = true;
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                        case EnumOperadores.MenorouIgual:
                            {
                                if (!string.IsNullOrEmpty(dataUltimoDebitoString))
                                {
                                    var dataUltimoDeb = Convert.ToDateTime(dataUltimoDebitoString);

                                    if (dataUltimoDeb <= dataParametro)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                        criterioEliminatorioReprovado = true;
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                        case EnumOperadores.Igual:
                            {
                                if (!string.IsNullOrEmpty(dataUltimoDebitoString))
                                {
                                    var dataUltimoDeb = Convert.ToDateTime(dataUltimoDebitoString);

                                    if (dataUltimoDeb == dataParametro)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                        criterioEliminatorioReprovado = true;
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                    }
                }
                else if (criterio.TipoCriterio == (int)CriteriosParametrosCredito.VALOR_TOTAL_DEBITO)
                {
                    switch (operador)
                    {
                        case EnumOperadores.MaiorQue:
                            {
                                if (debitosTotais > criterio.Valor)
                                {
                                    pontos += Convert.ToInt32(criterio.Pontos);
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                        case EnumOperadores.MenorQue:
                            {
                                if (debitosTotais < criterio.Valor)
                                {
                                    pontos += Convert.ToInt32(criterio.Pontos);
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                        case EnumOperadores.MaiorouIgual:
                            {
                                if (debitosTotais >= criterio.Valor)
                                {
                                    pontos += Convert.ToInt32(criterio.Pontos);
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                        case EnumOperadores.MenorouIgual:
                            {
                                if (debitosTotais <= criterio.Valor)
                                {
                                    pontos += Convert.ToInt32(criterio.Pontos);
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                    }
                }

                if (criterioEliminatorioReprovado)
                {
                    // Caso algum critério eliminatório tenha sido reprovado retornamos o Int32.MinValue
                    // como um indicador que a soma dos pontos é irrelevante devido à quebra de um critério 
                    // eliminatório
                    return Int32.MinValue;
                }
            }

            return pontos;
        }


        /// <summary>
        ///     Pega o total de pontos de uma determinada consulta de crédito para uma pessoa jurídica
        /// </summary>
        /// <param name="id">ID da consulta de crédito</param>
        /// <param name="cnpj">CNPJ da pessoa jurídica</param>
        public int GetTotalPontosConsultaCnpj(long id, string cnpj)
        {
            ConsultaSerasaDados consultaSerasa = this._repository.GetById(id);

            int pontos = 0;

            if (consultaSerasa == null)
            {
                throw new ConsultaSerasaNaoEncontradaException(cnpj);
            }

            if (consultaSerasa.TipoIntegracao == IntegracaoExternaConsultaCredEnum.PREQUAL)
            {
                return pontos;
            }
            else
            {

                int scoreSerasa = 0;
                double debitosTotais = 0;
                int dataUltimoDebito = 0;

                if (consultaSerasa.consulta.Contains("N200"))
                {
                    int indexBloco = consultaSerasa.consulta.IndexOf("N200");
                    int tamanhoBloco = 115;

                    string blocoN200 = consultaSerasa.consulta.Substring(indexBloco, tamanhoBloco);

                    // Removendo espaço em branco da string do score para evitar erro ao converter para int
                    var statusPessoa = blocoN200.Substring(84, 2);

                    var situacao = this.GetSituacaoDocumentoSerasa(cnpj, Convert.ToInt32(statusPessoa));
                    if (situacao != HtmlEnumExtensions.GetEnumDescription(SituacaoSerasaPessoaFisica.Regular))
                    {
                        throw new Exception($"{RsBase.SITUACAO_SERASA_REPROVADA}: {situacao.ToUpper()}.");
                    }
                }
                if (consultaSerasa.consulta.Contains("F900"))
                {
                    int indexBloco = consultaSerasa.consulta.IndexOf("F900");
                    int tamanhoBloco = 115;

                    string blocoF900 = consultaSerasa.consulta.Substring(indexBloco, tamanhoBloco);

                    // Removendo espaço em branco da string do score para evitar erro ao converter para int
                    string scoreSerasaString = blocoF900.Substring(28, 4).Trim();

                    if (!String.IsNullOrEmpty(scoreSerasaString) && Int32.TryParse(scoreSerasaString, out scoreSerasa))
                    {
                        scoreSerasa = Convert.ToInt32(scoreSerasaString);
                    }
                }

                IEnumerable<Sinapse.Model.CriteriosParamCredito> criterios = this._repoCriterios.GetAll();

                foreach (CriteriosParamCredito criterio in criterios)
                {
                    var operador = (EnumOperadores)criterio.TipoOperador;

                    bool criterioEliminatorioReprovado = false;

                    if (criterio.TipoCriterio == (int)CriteriosParametrosCredito.SCORE_POSITIVO)
                    {
                        switch (operador)
                        {
                            case EnumOperadores.MaiorQue:
                                {
                                    if (scoreSerasa > criterio.Valor)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                            case EnumOperadores.MenorQue:
                                {
                                    if (scoreSerasa < criterio.Valor)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                            case EnumOperadores.MaiorouIgual:
                                {
                                    if (scoreSerasa >= criterio.Valor)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                            case EnumOperadores.MenorouIgual:
                                {
                                    if (scoreSerasa <= criterio.Valor)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                        }
                    }
                    else if (criterio.TipoCriterio == (int)CriteriosParametrosCredito.ULTIMA_DATA_PENDENCIA)
                    {
                        switch (operador)
                        {
                            case EnumOperadores.MaiorQue:
                                {
                                    if (dataUltimoDebito > criterio.Valor)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                            case EnumOperadores.MenorQue:
                                {
                                    if (dataUltimoDebito < criterio.Valor)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                            case EnumOperadores.MaiorouIgual:
                                {
                                    if (dataUltimoDebito >= criterio.Valor)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                            case EnumOperadores.MenorouIgual:
                                {
                                    if (dataUltimoDebito <= criterio.Valor)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                        }
                    }
                    else if (criterio.TipoCriterio == (int)CriteriosParametrosCredito.VALOR_TOTAL_DEBITO)
                    {
                        switch (operador)
                        {
                            case EnumOperadores.MaiorQue:
                                {
                                    if (debitosTotais > criterio.Valor)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                            case EnumOperadores.MenorQue:
                                {
                                    if (debitosTotais < criterio.Valor)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                            case EnumOperadores.MaiorouIgual:
                                {
                                    if (debitosTotais >= criterio.Valor)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                            case EnumOperadores.MenorouIgual:
                                {
                                    if (debitosTotais <= criterio.Valor)
                                    {
                                        pontos += Convert.ToInt32(criterio.Pontos);
                                    }
                                    else if (criterio.Eliminatorio)
                                    {
                                        criterioEliminatorioReprovado = true;
                                    }

                                    break;
                                }
                        }
                    }

                    if (criterioEliminatorioReprovado)
                    {
                        // Caso algum critério eliminatório tenha sido reprovado retornamos o Int32.MinValue
                        // como um indicador que a soma dos pontos é irrelevante devido à quebra de um critério 
                        // eliminatório
                        return Int32.MinValue;
                    }
                }

            }
            return pontos;
        }

        /// <summary>
        ///     Pega o total de pontos de uma pessoa jurídica que já teve sua consulta de crédito
        ///     realizada pelo sistema.
        /// </summary>
        /// <param name="cnpj">CNPJ da pessoa jurídica que será consultada</param>
        public int GetTotalPontosCnpj(string cnpj)
        {
            Sinapse.Model.ConsultaSerasaDados consultaSerasa = this._repository.GetByCnpj(cnpj);

            if (consultaSerasa == null)
            {
                throw new ConsultaSerasaNaoEncontradaException(cnpj);
            }

            int scoreSerasa = 0;
            double debitosTotais = 0;
            int dataUltimoDebito = 0;

            if (consultaSerasa.consulta.Contains("F900"))
            {
                int indexBloco = consultaSerasa.consulta.IndexOf("F900");
                int tamanhoBloco = 115;

                string blocoF900 = consultaSerasa.consulta.Substring(indexBloco, tamanhoBloco);

                // Removendo espaço em branco da string do score para evitar erro ao converter para int
                string scoreSerasaString = blocoF900.Substring(28, 4).Trim();

                if (!String.IsNullOrEmpty(scoreSerasaString) && Int32.TryParse(scoreSerasaString, out scoreSerasa))
                {
                    scoreSerasa = Convert.ToInt32(scoreSerasaString);
                }
            }

            IEnumerable<Sinapse.Model.CriteriosParamCredito> criterios = this._repoCriterios.GetAll();

            int pontos = 0;

            foreach (CriteriosParamCredito criterio in criterios)
            {
                var operador = (EnumOperadores)criterio.TipoOperador;

                bool criterioEliminatorioReprovado = false;

                if (criterio.TipoCriterio == (int)CriteriosParametrosCredito.SCORE_POSITIVO)
                {
                    switch (operador)
                    {
                        case EnumOperadores.MaiorQue:
                            {
                                if (scoreSerasa > criterio.Valor)
                                {
                                    pontos += Convert.ToInt32(criterio.Pontos);
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                        case EnumOperadores.MenorQue:
                            {
                                if (scoreSerasa < criterio.Valor)
                                {
                                    pontos += Convert.ToInt32(criterio.Pontos);
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                        case EnumOperadores.MaiorouIgual:
                            {
                                if (scoreSerasa >= criterio.Valor)
                                {
                                    pontos += Convert.ToInt32(criterio.Pontos);
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                        case EnumOperadores.MenorouIgual:
                            {
                                if (scoreSerasa <= criterio.Valor)
                                {
                                    pontos += Convert.ToInt32(criterio.Pontos);
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                    }
                }
                else if (criterio.TipoCriterio == (int)CriteriosParametrosCredito.ULTIMA_DATA_PENDENCIA)
                {
                    switch (operador)
                    {
                        case EnumOperadores.MaiorQue:
                            {
                                if (dataUltimoDebito > criterio.Valor)
                                {
                                    pontos += Convert.ToInt32(criterio.Pontos);
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                        case EnumOperadores.MenorQue:
                            {
                                if (dataUltimoDebito < criterio.Valor)
                                {
                                    pontos += Convert.ToInt32(criterio.Pontos);
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                        case EnumOperadores.MaiorouIgual:
                            {
                                if (dataUltimoDebito >= criterio.Valor)
                                {
                                    pontos += Convert.ToInt32(criterio.Pontos);
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                        case EnumOperadores.MenorouIgual:
                            {
                                if (dataUltimoDebito <= criterio.Valor)
                                {
                                    pontos += Convert.ToInt32(criterio.Pontos);
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                    }
                }
                else if (criterio.TipoCriterio == (int)CriteriosParametrosCredito.VALOR_TOTAL_DEBITO)
                {
                    switch (operador)
                    {
                        case EnumOperadores.MaiorQue:
                            {
                                if (debitosTotais > criterio.Valor)
                                {
                                    pontos += Convert.ToInt32(criterio.Pontos);
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                        case EnumOperadores.MenorQue:
                            {
                                if (debitosTotais < criterio.Valor)
                                {
                                    pontos += Convert.ToInt32(criterio.Pontos);
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                        case EnumOperadores.MaiorouIgual:
                            {
                                if (debitosTotais >= criterio.Valor)
                                {
                                    pontos += Convert.ToInt32(criterio.Pontos);
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                        case EnumOperadores.MenorouIgual:
                            {
                                if (debitosTotais <= criterio.Valor)
                                {
                                    pontos += Convert.ToInt32(criterio.Pontos);
                                }
                                else if (criterio.Eliminatorio)
                                {
                                    criterioEliminatorioReprovado = true;
                                }

                                break;
                            }
                    }
                }

                if (criterioEliminatorioReprovado)
                {
                    // Caso algum critério eliminatório tenha sido reprovado retornamos o Int32.MinValue
                    // como um indicador que a soma dos pontos é irrelevante devido à quebra de um critério 
                    // eliminatório
                    return Int32.MinValue;
                }
            }

            return pontos;
        }

        /// <summary>
        ///     Retorna o status da consulta de crédito com o ID especificado
        /// </summary>
        /// <param name="id">ID da consulta de crédito</param>
        /// <param name="cpf">CPF da pessoa física</param>
        public Response<Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito> GetStatusConsultaCreditoCpfByConsulta(long id, string cpf)
        {
            try
            {
                var statusConsultaCredito = Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito.REPROVADO;

                ConsultaSerasaDados consultaSerasa = this._repository.GetById(id);
                var config = this._repoConfig.GetConfiguracoesGerais();

                if (consultaSerasa == null)
                {
                    throw new ConsultaSerasaNaoEncontradaException(cpf);
                }

                if (consultaSerasa.consulta.Contains("CPF SEM TITULAR"))
                    throw new ConsultaCreditoDocumentoSemTitularException();
                else if (consultaSerasa.consulta.Contains("MENOR DE IDADE PROTEGIDO LEGALMENTE"))
                    throw new ConsultaCreditoMenoridadeException();


                if (consultaSerasa.TipoIntegracao == IntegracaoExternaConsultaCredEnum.PREQUAL)
                {

                    statusConsultaCredito = GetStatusConsultaCreditoCpfByConsultaPreQual(id, cpf);
                }
                else if (consultaSerasa.TipoIntegracao == IntegracaoExternaConsultaCredEnum.SERASA_EXPERIAN)
                {
                    statusConsultaCredito = (GetStatusPFSerasaExperian(consultaSerasa.consulta, consultaSerasa.Id, cpf)).valor;
                }
                else
                {

                    int? pontos = this.GetTotalPontosConsultaCpf(id, cpf);

                    // Caso algum critério eliminatório tenha sido violado (representado pelo valor Int32.MinValue retornado do GetTotalPontosCpf)
                    // a consulta de crédito será reprovada
                    if (pontos == Int32.MinValue)
                        return new Response<AprovacaoCriterioParametroCredito>
                        {
                            valor = AprovacaoCriterioParametroCredito.REPROVADO
                        };

                    Sinapse.Model.ParametrosConsultaCredito param = this._repoParamsConsulta.GetParametrosConsultaCredito();

                    if (param == null)
                    {
                        throw new ParametrosConsultaCreditoNaoConfiguradosException();
                    }


                    Dictionary<long, Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito> paramsFaixa = preencheListagem(param);

                    paramsFaixa = paramsFaixa.OrderByDescending(dict => dict.Key)
                                             .ToDictionary(dict => dict.Key, dict => dict.Value);



                    foreach (KeyValuePair<long, Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito> faixa in paramsFaixa)
                    {
                        long pontosFaixa = faixa.Key;
                        Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito statusFaixa = faixa.Value;

                        if (pontos >= pontosFaixa)
                        {
                            statusConsultaCredito = statusFaixa;
                            break;
                        }
                    }
                }
                if (statusConsultaCredito == Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito.REPROVADO && !config.ConsultaCreditoRestringeVenda)
                {
                    statusConsultaCredito = Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito.AGUARDA_AVALIACAO_GESTOR;
                }

                return new Response<AprovacaoCriterioParametroCredito>
                {
                    valor = statusConsultaCredito
                };
            }
            catch (Exception ex)
            {
                return new Response<AprovacaoCriterioParametroCredito>
                {
                    valor = AprovacaoCriterioParametroCredito.REPROVADO,
                    message = ex.Message
                };
            }
        }

        /// <summary>
        ///     Retorna o status da consulta de crédito com o ID especificado
        /// </summary>
        /// <param name="id">ID da consulta de crédito</param>
        /// <param name="cpf">CPF da pessoa física</param>
        public Response<Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito> GetStatusPFSerasaExperian(string responseString, long idConsulta, string documento)
        {
            try
            {
                if (string.IsNullOrEmpty(responseString))
                    throw new Exception("Resposta da consulta Serasa Experian está vazia.");

                ConsultaSerasaDados consultaSerasa = this._repository.GetById(idConsulta);
                var config = this._repoConfig.GetConfiguracoesGerais();

                if (consultaSerasa == null)
                    throw new Exception("Consulta Serasa não encontrada.");

                var statusConsultaCredito = AprovacaoCriterioParametroCredito.REPROVADO;

                var serasaResponse = JsonConvert.DeserializeObject<ConsultaSerasaResponse>(responseString);


                if (serasaResponse == null)
                    throw new Exception("Não foi possível desserializar a resposta da Serasa Experian.");

                if (serasaResponse == null || serasaResponse?.Erros?.Mensagem != null)
                {
                    var texto = String.Empty;
                    if (serasaResponse.Erros.Mensagem.Contains("DOCUMENT_NOT_FOUND"))
                    {
                        texto = "Não foi possível encontrar o CPF digitado na base do Serasa.";
                    }
                    else if (serasaResponse.Erros.Mensagem.Contains("PERSON_DECEASED"))
                    {
                        texto = "Pessoa falecida na base da Serasa Experian.";
                    }
                    else if (serasaResponse.Erros.Mensagem.Contains("UNDERAGE"))
                    {
                        texto = "Documento pertence a menor de idade, sem informações disponíveis.";
                    }
                    else
                    {
                        texto = serasaResponse?.Erros?.Mensagem;
                    }

                    throw new Exception(texto);
                }

                var infosCliSerasa = serasaResponse?.reports?.FirstOrDefault(x => x.registration.documentNumber == documento);

                if (infosCliSerasa is null)
                    throw new Exception("Não foi possível localizar as informações do cliente na resposta da Serasa Experian.");



                if (string.IsNullOrEmpty(infosCliSerasa?.registration?.statusRegistration) ||
                    infosCliSerasa.registration.statusRegistration.ToUpper() != "REGULAR")
                    throw new Exception("CPF com situação irregular na Serasa Experian.");

                int? pontos = this.GetTotalPontosSerasaExpCpf(idConsulta, documento, serasaResponse);

                // Caso algum critério eliminatório tenha sido violado (representado pelo valor Int32.MinValue retornado do GetTotalPontosCpf)
                // a consulta de crédito será reprovada
                if (pontos == Int32.MinValue)
                    return new Response<AprovacaoCriterioParametroCredito>
                    {
                        valor = AprovacaoCriterioParametroCredito.REPROVADO
                    };

                Sinapse.Model.ParametrosConsultaCredito param = this._repoParamsConsulta.GetParametrosConsultaCredito();

                if (param == null)
                {
                    throw new ParametrosConsultaCreditoNaoConfiguradosException();
                }


                Dictionary<long, Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito> paramsFaixa = preencheListagem(param);

                paramsFaixa = paramsFaixa.OrderByDescending(dict => dict.Key)
                                         .ToDictionary(dict => dict.Key, dict => dict.Value);



                foreach (KeyValuePair<long, Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito> faixa in paramsFaixa)
                {
                    long pontosFaixa = faixa.Key;
                    Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito statusFaixa = faixa.Value;

                    if (pontos >= pontosFaixa)
                    {
                        statusConsultaCredito = statusFaixa;
                        break;
                    }
                }

                if (statusConsultaCredito == Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito.REPROVADO && !config.ConsultaCreditoRestringeVenda)
                {
                    statusConsultaCredito = Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito.AGUARDA_AVALIACAO_GESTOR;
                }

                return new Response<AprovacaoCriterioParametroCredito>
                {
                    valor = statusConsultaCredito
                };
            }
            catch (Exception ex)
            {
                return new Response<AprovacaoCriterioParametroCredito>
                {
                    valor = AprovacaoCriterioParametroCredito.REPROVADO,
                    message = ex.Message
                };
            }
        }

        /// <summary>
        ///     Retorna o status da consulta de crédito com o ID especificado
        /// </summary>
        /// <param name="responseString">consulta em formato de string</param>
        /// <param name="idConsulta">ID da consulta de crédito</param>
        /// <param name="documento">CNPJ da pessoa física</param>
        public Response<Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito> GetStatusPJSerasaExperian(string responseString, long idConsulta, string documento)
        {
            try
            {
                if (string.IsNullOrEmpty(responseString))
                    throw new Exception("Resposta da consulta Serasa Experian está vazia.");

                ConsultaSerasaDados consultaSerasa = this._repository.GetById(idConsulta);
                var config = this._repoConfig.GetConfiguracoesGerais();

                if (consultaSerasa == null)
                    throw new Exception("Consulta Serasa não encontrada.");

                var statusConsultaCredito = AprovacaoCriterioParametroCredito.REPROVADO;

                var serasaResponse = JsonConvert.DeserializeObject<ConsultaSerasaResponse>(responseString);

                if (serasaResponse == null)
                    throw new Exception("Não foi possível desserializar a resposta da Serasa Experian.");

                var infosCliSerasa = serasaResponse.reports.FirstOrDefault(x => x.registration.companyDocument == documento);

                if (infosCliSerasa is null)
                    throw new Exception("Não foi possível localizar as informações do cliente na resposta da Serasa Experian.");

                if (string.IsNullOrEmpty(infosCliSerasa?.registration?.statusRegistration) ||
                    infosCliSerasa.registration.statusRegistration.ToUpper().Contains("INAPTA"))
                    throw new Exception("CNJP com situação INAPTA na Serasa Experian.");

                int? pontos = this.GetTotalPontosSerasaExpCnpj(idConsulta, documento, serasaResponse);

                // Caso algum critério eliminatório tenha sido violado (representado pelo valor Int32.MinValue retornado do GetTotalPontosCpf)
                // a consulta de crédito será reprovada
                if (pontos == Int32.MinValue)
                    return new Response<AprovacaoCriterioParametroCredito>
                    {
                        valor = AprovacaoCriterioParametroCredito.REPROVADO
                    };

                Sinapse.Model.ParametrosConsultaCredito param = this._repoParamsConsulta.GetParametrosConsultaCredito();

                if (param == null)
                {
                    throw new ParametrosConsultaCreditoNaoConfiguradosException();
                }


                Dictionary<long, Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito> paramsFaixa = preencheListagem(param);

                paramsFaixa = paramsFaixa.OrderByDescending(dict => dict.Key)
                                         .ToDictionary(dict => dict.Key, dict => dict.Value);



                foreach (KeyValuePair<long, Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito> faixa in paramsFaixa)
                {
                    long pontosFaixa = faixa.Key;
                    Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito statusFaixa = faixa.Value;

                    if (pontos >= pontosFaixa)
                    {
                        statusConsultaCredito = statusFaixa;
                        break;
                    }
                }

                if (statusConsultaCredito == Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito.REPROVADO && !config.ConsultaCreditoRestringeVenda)
                {
                    statusConsultaCredito = Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito.AGUARDA_AVALIACAO_GESTOR;
                }

                return new Response<AprovacaoCriterioParametroCredito>
                {
                    valor = statusConsultaCredito
                };
            }
            catch (Exception ex)
            {
                return new Response<AprovacaoCriterioParametroCredito>
                {
                    valor = AprovacaoCriterioParametroCredito.REPROVADO,
                    message = ex.Message
                };
            }
        }

        private Dictionary<long, Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito> preencheListagem(Sinapse.Model.ParametrosConsultaCredito parametros)
        {
            var parametrosCreditoUtilizados = new Dictionary<long, Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito>();

            if (parametros.UtilizaFaixaCriterio1 && parametros.TipoAprovacaoFaixa1 != AprovacaoCriterioParametroCredito.NENHUM)
            {
                parametrosCreditoUtilizados.Add(parametros.PontosPrimeiraFaixaCriterio, parametros.TipoAprovacaoFaixa1);
            }
            if (parametros.UtilizaFaixaCriterio2 && parametros.TipoAprovacaoFaixa2 != AprovacaoCriterioParametroCredito.NENHUM)
            {
                parametrosCreditoUtilizados.Add(parametros.PontosSegundaFaixaCriterio, parametros.TipoAprovacaoFaixa2);
            }
            if (parametros.UtilizaFaixaCriterio3 && parametros.TipoAprovacaoFaixa3 != AprovacaoCriterioParametroCredito.NENHUM)
            {
                parametrosCreditoUtilizados.Add(parametros.PontosTerceiraFaixaCriterio, parametros.TipoAprovacaoFaixa3);
            }

            return parametrosCreditoUtilizados;
        }


        /// <summary>
        ///     Retorna o status da consulta de crédito com o ID especificado
        /// </summary>
        /// <param name="id">ID da consulta de crédito</param>
        /// <param name="cnpj">CNPJ da pessoa jurídica</param>
        public Response<Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito> GetStatusConsultaCreditoCnpjByConsulta(long id, string cnpj)
        {
            try
            {

                var statusConsultaCredito = Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito.REPROVADO;

                ConsultaSerasaDados consultaSerasa = this._repository.GetById(id);
                var config = this._repoConfig.GetConfiguracoesGerais();

                if (consultaSerasa.TipoIntegracao == IntegracaoExternaConsultaCredEnum.PREQUAL)
                {

                    statusConsultaCredito = GetStatusConsultaCreditoCnpjByConsultaPreQual(id, cnpj);
                }
                else if (consultaSerasa.TipoIntegracao == IntegracaoExternaConsultaCredEnum.SERASA_EXPERIAN)
                {
                    statusConsultaCredito = (GetStatusPJSerasaExperian(consultaSerasa.consulta, consultaSerasa.Id, cnpj)).valor;
                }
                else
                {

                    int pontos = this.GetTotalPontosConsultaCnpj(id, cnpj);

                    // Caso algum critério eliminatório tenha sido violado (representado pelo valor Int32.MinValue retornado do GetTotalPontosCnpj)
                    // a consulta de crédito será reprovada
                    if (pontos == Int32.MinValue)
                        return new Response<AprovacaoCriterioParametroCredito> { valor = AprovacaoCriterioParametroCredito.REPROVADO };


                    Sinapse.Model.ParametrosConsultaCredito param = this._repoParamsConsulta.GetParametrosConsultaCredito();

                    if (param == null)
                    {
                        throw new ParametrosConsultaCreditoNaoConfiguradosException();
                    }

                    var paramsFaixa = this.preencheListagem(param);

                    paramsFaixa = paramsFaixa.OrderByDescending(dict => dict.Key)
                                             .ToDictionary(dict => dict.Key, dict => dict.Value);


                    foreach (KeyValuePair<long, Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito> faixa in paramsFaixa)
                    {
                        long pontosFaixa = faixa.Key;
                        Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito statusFaixa = faixa.Value;

                        if (pontos >= pontosFaixa)
                        {
                            statusConsultaCredito = statusFaixa;
                            break;
                        }
                    }
                }
                if (statusConsultaCredito == Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito.REPROVADO && !config.ConsultaCreditoRestringeVenda)
                {
                    statusConsultaCredito = Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito.AGUARDA_AVALIACAO_GESTOR;
                }

                return new Response<AprovacaoCriterioParametroCredito> { valor = statusConsultaCredito };
            }
            catch (Exception ex)
            {
                return new Response<AprovacaoCriterioParametroCredito> { valor = AprovacaoCriterioParametroCredito.REPROVADO, message = ex.Message };
            }
        }


        /// <summary>
        ///     Retorna o status da consulta de crédito com o ID especificado
        /// </summary>
        /// <param name="id">ID da consulta de crédito</param>
        /// <param name="cpf">CPF da pessoa física</param>
        public Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito GetStatusConsultaCreditoCpfByConsultaPreQual(long id, string cpf)
        {
            ConsultaSerasaDados consultaSerasa = this._repository.GetById(id);

            if (consultaSerasa == null)
            {
                throw new ConsultaSerasaNaoEncontradaException(cpf);
            }

            ResponsePreQual retorno = JsonConvert.DeserializeObject<ResponsePreQual>(consultaSerasa.consulta);


            var statusConsultaCredito = Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito.REPROVADO;

            if (string.Equals(retorno.data?.DVApplication?.DecisaoProposta?.DecisaoElegibilidadeCredito, "Crédito elegível", StringComparison.OrdinalIgnoreCase))
            {
                statusConsultaCredito = Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito.APROVADO;
            }

            return statusConsultaCredito;

        }


        /// <summary>
        ///     Retorna o status da consulta de crédito com o ID especificado
        /// </summary>
        /// <param name="id">ID da consulta de crédito</param>
        /// <param name="cnpj">CNPJ da pessoa física</param>
        public Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito GetStatusConsultaCreditoCnpjByConsultaPreQual(long id, string cnpj)
        {
            ConsultaSerasaDados consultaSerasa = this._repository.GetById(id);

            if (consultaSerasa == null)
            {
                throw new ConsultaSerasaNaoEncontradaException(cnpj);
            }

            ResponsePreQual retorno = JsonConvert.DeserializeObject<ResponsePreQual>(consultaSerasa.consulta);


            var statusConsultaCredito = Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito.REPROVADO;

            if (string.Equals(retorno.data?.DVApplication?.DecisaoProposta?.DecisaoElegibilidadeCredito, "Crédito elegível", StringComparison.OrdinalIgnoreCase))
            {
                statusConsultaCredito = Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito.APROVADO;
            }

            return statusConsultaCredito;

        }

        /// <summary>
        ///     Retorna o status da consulta de crédito para uma pessoa física com consulta de crédito já realizada
        /// </summary>
        /// <param name="cpf">CPF da pessoa física que será consultada</param>
        public Response<Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito> GetStatusConsultaCreditoCpf(string cpf)
        {
            try
            {
                ConsultaSerasaDados consultaSerasa = this._repository.GetByCpf(cpf);
                Configuracoes_Gerais config = this._repoConfig.GetConfiguracoesGerais();


                if (consultaSerasa == null)
                {
                    throw new ConsultaSerasaNaoEncontradaException(cpf);
                }

                //Condição comentada, foi ajustado para verificar se o CPF é sem titular na API do Premier 
                /* if (consultaSerasa.consulta.Contains("CPF SEM TITULAR"))
                     throw new Exception("CPF SEM TITULAR");
                 else*/
                if (consultaSerasa.consulta.Contains("MENOR DE IDADE PROTEGIDO LEGALMENTE"))
                    throw new ConsultaCreditoMenoridadeException();


                int pontos = this.GetTotalPontosConsultaCpf(consultaSerasa.Id, cpf);

                // Caso algum critério eliminatório tenha sido violado (representado pelo valor Int32.MinValue retornado do GetTotalPontosCpf)
                // a consulta de crédito será reprovada
                if (pontos == Int32.MinValue)
                    return new Response<AprovacaoCriterioParametroCredito> { valor = AprovacaoCriterioParametroCredito.REPROVADO };

                Sinapse.Model.ParametrosConsultaCredito param = this._repoParamsConsulta.GetParametrosConsultaCredito();

                if (param == null)
                {
                    throw new ParametrosConsultaCreditoNaoConfiguradosException();
                }

                var paramsFaixa = this.preencheListagem(param);

                paramsFaixa = paramsFaixa.OrderByDescending(parametro => parametro.Key).ToDictionary(p => p.Key, p => p.Value);

                var statusConsultaCredito = Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito.REPROVADO;

                foreach (KeyValuePair<long, AprovacaoCriterioParametroCredito> faixa in paramsFaixa)
                {
                    long pontosFaixa = faixa.Key;
                    Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito statusFaixa = faixa.Value;

                    if (pontos >= pontosFaixa)
                    {
                        statusConsultaCredito = statusFaixa;
                        break;
                    }
                }
                if (statusConsultaCredito == Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito.REPROVADO && !config.ConsultaCreditoRestringeVenda)
                {
                    statusConsultaCredito = Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito.AGUARDA_AVALIACAO_GESTOR;
                }

                return new Response<AprovacaoCriterioParametroCredito> 
                {
                    valor = statusConsultaCredito 
                };
            }
            catch (Exception ex) 
            {
                return new Response<AprovacaoCriterioParametroCredito> 
                { 
                    valor = AprovacaoCriterioParametroCredito.REPROVADO, 
                    message = ex.Message
                };
            }
        }

        /// <summary>
        ///     Retorna o status da consulta de crédito para uma pessoa jurídica com consulta de crédito já realizada
        /// </summary>
        /// <param name="cnpj">CNPJ da pessoa física que será consultada</param>
        public Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito GetStatusConsultaCreditoCnpj(string cnpj)
        {
            int pontos = this.GetTotalPontosCnpj(cnpj);

            // Caso algum critério eliminatório tenha sido violado (representado pelo valor Int32.MinValue retornado do GetTotalPontosCnpj)
            // a consulta de crédito será reprovada
            if (pontos == Int32.MinValue)
                return AprovacaoCriterioParametroCredito.REPROVADO;

            Sinapse.Model.ParametrosConsultaCredito param = this._repoParamsConsulta.GetParametrosConsultaCredito();

            if (param == null)
            {
                throw new ParametrosConsultaCreditoNaoConfiguradosException();
            }

            var paramsFaixa = this.preencheListagem(param);

            paramsFaixa = paramsFaixa.OrderByDescending(parametro => parametro.Key).ToDictionary(p => p.Key, p => p.Value);

            var statusConsultaCredito = Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito.REPROVADO;

            foreach (KeyValuePair<long, Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito> faixa in paramsFaixa)
            {
                long pontosFaixa = faixa.Key;
                Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito statusFaixa = faixa.Value;

                if (pontos >= pontosFaixa)
                {
                    statusConsultaCredito = statusFaixa;
                    break;
                }
            }

            return statusConsultaCredito;
        }

        /// <summary>
        ///     Retorna o status que indica se há uma consulta de crédito válida 
        ///     registrada no sistema para o CPF informado
        /// </summary>
        /// <param name="cpf">CPF da consulta de crédito</param>
        public Sinapse.Model.enumerados.StatusEtapaVenda GetStatusConsultaCreditoRestricaoByCpf(string cpf)
        {
            try
            {
                using (var session = HibernateUtil.GetSession())
                using (var repoIntegracao = new Conexao<IntegracoesExternas>(session))
                {
                    var existeIntegracao = repoIntegracao.selectFirst(p => p.IntegracaoExterna == IntegracaoExterna.SERASA || p.IntegracaoExterna == IntegracaoExterna.PREQUAL || p.IntegracaoExterna == IntegracaoExterna.SERASA_EXPERIAN).Result;
                    if (existeIntegracao == null)
                    {
                        throw new Exception("Não foi possível realizar a consulta no Serasa. Por favor, verifique as configurações da API do Serasa e tente novamente.");
                    }
                }

                bool hasConsultaCreditoValida = this.GetConsultaCreditoByCpf(cpf) != null;

                if (hasConsultaCreditoValida)
                {
                    Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito statusConsulta = this.GetStatusConsultaCreditoCpf(cpf).valor;
                    switch (statusConsulta)
                    {
                        case Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito.APROVACAO_COM_INSTALACAO:
                        case Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito.APROVACAO_SEM_INSTALACAO:
                            {
                                return Sinapse.Model.enumerados.StatusEtapaVenda.Aprovado;
                            }
                        case Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito.AGUARDA_AVALIACAO_GESTOR:
                            {
                                return Sinapse.Model.enumerados.StatusEtapaVenda.AguardandoAprovacaoGestor;
                            }
                        case Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito.REPROVADO:
                            {
                                Configuracoes_Gerais config = this._repoConfig.GetConfiguracoesGerais();
                                
                                    return Sinapse.Model.enumerados.StatusEtapaVenda.Reprovado;                                
                            }
                        default:
                            {

                                return Sinapse.Model.enumerados.StatusEtapaVenda.Pendente;
                            }
                    }
                }
                else
                {
                    return Sinapse.Model.enumerados.StatusEtapaVenda.Pendente;
                }

            }
            catch (Exception ex)
            {
                if (ex.Message == "CPF SEM TITULAR")
                {
                    return Sinapse.Model.enumerados.StatusEtapaVenda.Reprovado;
                }
                else if (ex.Message == "MENOR DE IDADE PROTEGIDO LEGALMENTE")
                {
                    return Sinapse.Model.enumerados.StatusEtapaVenda.Reprovado;
                }
                else if (ex.Message == "Não foi possível realizar a consulta no Serasa. Por favor, verifique as configurações da API do Serasa e tente novamente.")
                {                    
                    return Sinapse.Model.enumerados.StatusEtapaVenda.Reprovado;
                }
                else if (ex.Message.Contains(RsBase.SITUACAO_SERASA_REPROVADA))
                {
                    return Sinapse.Model.enumerados.StatusEtapaVenda.Reprovado;
                }

                throw new ParametrosConsultaCreditoNaoConfiguradosException();

            }
        }

        /// <summary>
        ///     Retorna o status que indica se há uma consulta de crédito válida 
        ///     registrada no sistema para o CPF informado
        /// </summary>
        /// <param name="cpf">CPF da consulta de crédito</param>
        public Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito SerasaStatusRetorno(string consulta, long idConsulta, string documento, bool pessoaFisica)
        {
            try
            {
                using (var session = HibernateUtil.GetSession())
                using (var repoIntegracao = new Conexao<IntegracoesExternas>(session))
                {
                    var existeIntegracao = repoIntegracao.selectFirst(p => p.IntegracaoExterna == IntegracaoExterna.SERASA_EXPERIAN).Result;

                    if (existeIntegracao == null)
                        throw new Exception("Não foi possível realizar a consulta no Serasa. Por favor, verifique as configurações da API do Serasa e tente novamente.");

                }

                bool hasConsultaCreditoValida = pessoaFisica ? this.GetConsultaCreditoByCpf(documento) != null : this.GetConsultaCreditoByCnpj(documento) != null;

                if (hasConsultaCreditoValida)
                {
                    AprovacaoCriterioParametroCredito statusConsulta = pessoaFisica ? this.GetStatusPFSerasaExperian(consulta, idConsulta, documento).valor : this.GetStatusPJSerasaExperian(consulta, idConsulta, documento).valor;
                    switch (statusConsulta)
                    {
                        case AprovacaoCriterioParametroCredito.APROVACAO_COM_INSTALACAO:
                        case AprovacaoCriterioParametroCredito.APROVADO:
                        case AprovacaoCriterioParametroCredito.APROVACAO_SEM_INSTALACAO:
                            {
                                return AprovacaoCriterioParametroCredito.APROVADO;
                            }
                        case AprovacaoCriterioParametroCredito.AGUARDA_AVALIACAO_GESTOR:
                            {
                                return AprovacaoCriterioParametroCredito.AGUARDA_AVALIACAO_GESTOR;
                            }
                        case AprovacaoCriterioParametroCredito.REPROVADO:
                            {
                                Configuracoes_Gerais config = this._repoConfig.GetConfiguracoesGerais();

                                return AprovacaoCriterioParametroCredito.REPROVADO;
                            }
                        default:
                            {
                                return AprovacaoCriterioParametroCredito.REPROVADO;
                            }
                    }
                }
                else
                {
                    return AprovacaoCriterioParametroCredito.REPROVADO;
                }

            }
            catch (Exception ex)
            {
                return AprovacaoCriterioParametroCredito.REPROVADO;
                throw new ParametrosConsultaCreditoNaoConfiguradosException();
            }
        }

        /// <summary>
        ///     Retorna o status que indica se há uma consulta de crédito válida 
        ///     registrada no sistema para o CNPJ informado
        /// </summary>
        /// <param name="cnpj">CNPJ da consulta de crédito</param>
        public Sinapse.Model.enumerados.StatusEtapaVenda GetStatusConsultaCreditoRestricaoByCnpj(string cnpj)
        {
            try
            {
                using (var session = HibernateUtil.GetSession())
                using (var repoIntegracao = new Conexao<IntegracoesExternas>(session))
                {
                    var existeIntegracao = repoIntegracao.selectFirst(p => p.IntegracaoExterna == IntegracaoExterna.SERASA || p.IntegracaoExterna == IntegracaoExterna.PREQUAL || p.IntegracaoExterna == IntegracaoExterna.SERASA_EXPERIAN).Result;
                    if (existeIntegracao == null)
                    {
                        throw new Exception("Não foi possível realizar a consulta no Serasa. Por favor, verifique as configurações da API do Serasa e tente novamente.");
                    }
                }
                bool hasConsultaCreditoValida = this.GetConsultaCreditoByCnpj(cnpj) != null;

                if (hasConsultaCreditoValida)
                {
                    Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito statusConsulta = this.GetStatusConsultaCreditoCnpj(cnpj);

                    switch (statusConsulta)
                    {
                        case Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito.APROVACAO_COM_INSTALACAO:
                        case Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito.APROVACAO_SEM_INSTALACAO:
                            {
                                return Sinapse.Model.enumerados.StatusEtapaVenda.Aprovado;
                            }
                        case Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito.AGUARDA_AVALIACAO_GESTOR:
                            {
                                return Sinapse.Model.enumerados.StatusEtapaVenda.AguardandoAprovacaoGestor;
                            }
                        case Sinapse.Model.enumerados.AprovacaoCriterioParametroCredito.REPROVADO:
                            {
                                Configuracoes_Gerais config = this._repoConfig.GetConfiguracoesGerais();

                                if (config.ConsultaCreditoRestringeVenda)
                                    return Sinapse.Model.enumerados.StatusEtapaVenda.Reprovado;
                                else
                                    return Sinapse.Model.enumerados.StatusEtapaVenda.AguardandoAprovacaoGestor;
                            }
                        default:
                            {
                                return Sinapse.Model.enumerados.StatusEtapaVenda.Pendente;
                            }
                    }
                }
                else
                {
                    return Sinapse.Model.enumerados.StatusEtapaVenda.Pendente;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "CPF SEM TITULAR")
                {
                    return Sinapse.Model.enumerados.StatusEtapaVenda.Reprovado;
                }
                else if (ex.Message == "MENOR DE IDADE PROTEGIDO LEGALMENTE")
                {
                    return Sinapse.Model.enumerados.StatusEtapaVenda.Reprovado;
                }
                else if (ex.Message == "Não foi possível realizar a consulta no Serasa.Por favor, verifique as configurações da API do Serasa e tente novamente.")
                {
                    return Sinapse.Model.enumerados.StatusEtapaVenda.Reprovado;
                }

                throw new ParametrosConsultaCreditoNaoConfiguradosException();
            }
        }

        /// <summary>
        ///     Retorna a consulta de crédito válida mais recente para o CPF informado
        ///     registrada no sistema
        /// </summary>
        /// <param name="cpf">CPF da consulta de crédito</param>
        public ConsultaSerasaDados GetConsultaCreditoByCpf(string cpf)
        {
            bool consultaDentroValidade = false;
            Configuracoes_Gerais config = this._repoConfig.GetConfiguracoesGerais();
            ConsultaSerasaDados consultaSerasa = this._repository.GetByCpf(cpf);

            if (consultaSerasa != null)
            {
                if (config.ConsultaCreditoTempoValidade.HasValue)
                {
                    DateTime dataMaxValidadeConsulta = consultaSerasa.DataHoraCriacao.Value.Date.AddDays(config.ConsultaCreditoTempoValidade.Value);
                    consultaDentroValidade = DateTime.Now.Date <= dataMaxValidadeConsulta.Date;

                    if (consultaSerasa == null || !consultaDentroValidade)
                    {
                        consultaSerasa = null;
                    }
                }
            }

            return consultaSerasa;
        }

        /// <summary>
        ///     Retorna a consulta de crédito válida mais recente para o CNPJ informado
        ///     registrada no sistema
        /// </summary>
        /// <param name="cnpj">CNPJ da consulta de crédito</param>
        public ConsultaSerasaDados GetConsultaCreditoByCnpj(string cnpj)
        {
            bool consultaDentroValidade = false;
            ConsultaSerasaDados consultaSerasa = this._repository.GetByCnpj(cnpj);
            Configuracoes_Gerais config = this._repoConfig.GetConfiguracoesGerais();

            if (consultaSerasa != null)
            {
                if (config.ConsultaCreditoTempoValidade.HasValue)
                {
                    DateTime dataMaxValidadeConsulta = consultaSerasa.DataHoraCriacao.Value.Date.AddDays(config.ConsultaCreditoTempoValidade.Value);
                    consultaDentroValidade = DateTime.Now.Date <= dataMaxValidadeConsulta.Date;

                    if (consultaSerasa == null || !consultaDentroValidade)
                    {
                        consultaSerasa = null;
                    }
                }
            }

            return consultaSerasa;
        }
        public GetApiRetorno<Double> ConvertPendenciaString(string Pendencia)
        {
            var retorno = new GetApiRetorno<double>();
            double value;
            try
            {
                var valorPendencia = Pendencia.Insert((Pendencia.Length - 2), ".");
                if (double.TryParse(valorPendencia, out value))
                {
                    retorno.Objeto = value;
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
            }
            return retorno;
        }

        #region [SituacaoSerasa]
        public string GetSituacaoDocumentoSerasa(string documento, int situacao)
        {
            var retorno = String.Empty;
            bool pessoaFisica = documento.Length == 11;
            if (pessoaFisica)
            {
                retorno = HtmlEnumExtensions.GetEnumDescription((SituacaoSerasaPessoaFisica)situacao);
            }
            else
            {
                retorno = HtmlEnumExtensions.GetEnumDescription((SituacaoSerasaPessoaJuridica)situacao);
            }
            return retorno;
        }
        #endregion

        #region [Html]
        public async Task<string> GetHtmlResumoConsulta(string consulta, string documento)
        {
            var retorno = "";

            var faixaConteudo = new Dictionary<string, List<ISerasaPadraoRetorno>>();
            //Captura por protocolo do bloco do Serasa. Tantos os valores 24000, como 24090 e 24099 seriam relevantes para o cenário
            var regex = new Regex(@"([N,T]{1}\d{3}[0,1,4,9]{2})|F900", RegexOptions.IgnoreCase).Matches(consulta).Cast<Match>().ToDictionary(p => p.Index, p => p.Value);

            var listagem = regex.Select(p => { return (p.Value.Substring(0, 4), p.Value.Length < 6 ? "" : p.Value.Substring(4, 2), p.Key); }).GroupBy(p => p.Item1).ToList();
            foreach (var captura in listagem)
            {
                int tamanhoBloco = 115;
                if (!faixaConteudo.Any(p => p.Key == captura.Key))
                {
                    faixaConteudo.Add(captura.Key, new List<ISerasaPadraoRetorno>());
                }

                foreach (var protocolo in captura)
                {
                    string bloco = consulta.Substring(protocolo.Key, tamanhoBloco);
                    switch (protocolo.Item2)
                    {
                        //Descrição do Item
                        case "00":
                            if (captura.Key == "N200")
                            {
                                var statusPessoa = bloco.Substring(84, 2);
                                faixaConteudo[captura.Key].Add(new Confirmei
                                {
                                    SubTipo = protocolo.Item2,
                                    SituacaoDocumento = this.GetSituacaoDocumentoSerasa(documento, Convert.ToInt32(statusPessoa))
                                });
                            }

                            else if (captura.Key == "N240")
                            {
                                faixaConteudo[captura.Key].Add(new PendenciaFinanceiraSerasa
                                {
                                    SubTipo = protocolo.Item2,
                                    Modalidade = bloco.Substring(14, 30),
                                    Avalista = bloco.Substring(44, 1),
                                    Moeda = bloco.Substring(45, 3),
                                    Contrato = bloco.Substring(63, 16),
                                    Origem = bloco.Substring(79, 30),
                                    ValorDebito = !String.IsNullOrEmpty(bloco.Substring(48, 15)) ?
                                                  Convert.ToString((decimal?)decimal.Parse(bloco.Substring(48, 15)) / 100) : "0"
                                });
                            }

                            else if (captura.Key == "N250")
                            {
                                faixaConteudo[captura.Key].Add(new Protesto
                                {
                                    SubTipo = protocolo.Item2,
                                    ProtestoDataOcorrencia = bloco.Substring(6, 8),
                                    ProtestosMoeda = bloco.Substring(14, 3),
                                    ProtestoCartorio = bloco.Substring(32, 2),
                                    ProtestoCidadeOrigem = bloco.Substring(34, 30),
                                    ProtestoUf = bloco.Substring(64, 2),
                                    ProtestosValor = !String.IsNullOrEmpty(bloco.Substring(17, 15)) ? Convert.ToString((decimal?) decimal.Parse(bloco.Substring(17, 15)) / 100)  : "0",
                                });
                            }

                            else if (captura.Key == "N270")
                            {
                                faixaConteudo[captura.Key].Add(new ChequeBacen
                                {
                                    SubTipo = protocolo.Item2,
                                    DataOcorrencia = bloco.Substring(7, 8),
                                    NumCheque = bloco.Substring(15, 10),
                                    AlineaCheque = bloco.Substring(25, 5),
                                    QdadeCCFparaBanco = bloco.Substring(30, 5),
                                    ValorTotalCheque = bloco.Substring(35, 15),
                                    NumBanco = bloco.Substring(50, 3),
                                    NomeBanco = bloco.Substring(53, 14),
                                    NumAgencia = bloco.Substring(67, 4),
                                    CidadeAgencia = bloco.Substring(71, 30),
                                    UfAgencia = bloco.Substring(101, 2),
                                });
                            }

                            else if (captura.Key == "N440")
                            {
                                faixaConteudo[captura.Key].Add(new ConsultasA_Serasa
                                {
                                    SubTipo = protocolo.Item2,
                                    TotalChequesEmitidosaVista15Dias = bloco.Substring(6, 10),
                                    TotalChequesEmitidosaPrazo30Dias = bloco.Substring(6, 10),
                                    TotalChequesEmitidosaPrazo60Dias = bloco.Substring(6, 10),
                                    TotalChequesEmitidosaPrazo = bloco.Substring(7, 8),
                                    ConsultasSerasaMercadoRefComercial = bloco.Substring(7, 8),
                                });
                            }

                            else if (captura.Key == "N700")
                            {
                                faixaConteudo[captura.Key].Add(new SocioAcionista
                                {
                                    SubTipo = protocolo.Item2,
                                    TipoPessoa = bloco.Substring(6, 1),
                                    SocioDocumento = bloco.Substring(7, 14),
                                    SocioNome = bloco.Substring(21, 60),
                                    SocioPercentualCapital = bloco.Substring(81, 3),
                                    SocioRestricao = bloco.Substring(85, 1),
                                });
                            }

                            else if (captura.Key == "T999")
                            {
                                faixaConteudo[captura.Key].Add(new T999 { SubTipo = protocolo.Item2, Mensagem = bloco.Substring(7, 70) });
                            }
                            break;
                        //Informações Adicionais
                        case "01":
                            if (captura.Key == "N200")
                            {
                                faixaConteudo[captura.Key].Add(new Confirmei
                                {
                                    SubTipo = protocolo.Item2,
                                    NomeMae = bloco.Substring(6, 40)
                                });
                            }
                            else if (captura.Key == "N240")
                            {
                                faixaConteudo[captura.Key].Add(new PendenciaFinanceiraSerasa
                                {
                                    SubTipo = protocolo.Item2,
                                    SubJudice = bloco.Substring(6, 1),
                                    MensagemSubJudice = bloco.Substring(7, 76)
                                });
                            }
                            else if (captura.Key == "N250")
                            {
                                faixaConteudo[captura.Key].Add(new Protesto
                                {
                                    SubTipo = protocolo.Item2,
                                    ProtestoSubjudice = bloco.Substring(6, 1),
                                    ProtestoMsgSubjudice = bloco.Substring(8, 76)
                                });
                            }
                            else if (captura.Key == "N440")
                            {
                                faixaConteudo[captura.Key].Add(new ConsultasA_Serasa
                                {
                                    SubTipo = protocolo.Item2,
                                    TotalChequesEmitidosaVista15Dias = bloco.Substring(6, 10),
                                    TotalChequesEmitidosaPrazo30Dias = bloco.Substring(6, 10),
                                    TotalChequesEmitidosaPrazo60Dias = bloco.Substring(6, 10),
                                    TotalChequesEmitidosaPrazo = bloco.Substring(7, 8),
                                    ConsultasSerasaMercadoRefComercial = bloco.Substring(7, 8),
                                });
                            }
                            break;

                        case "03":
                            if (captura.Key == "N440")
                            {
                                faixaConteudo[captura.Key].Add(new ConsultasA_Serasa
                                {
                                    SubTipo = protocolo.Item2,
                                    ConsultasSerasaMercadoRefComercial = bloco.Substring(7, 8),
                                });
                            }
                            break;
                        case "04":
                            if (captura.Key == "T999")
                            {

                                faixaConteudo[captura.Key].Add(new T999
                                {
                                    SubTipo = protocolo.Item2,
                                    Mensagem = bloco.Substring(7, 70)
                                });
                            }
                            break;

                        //Descrição do item iterado
                        case "90":
                            if (captura.Key == "N240")
                            {
                                faixaConteudo[captura.Key].Add(new PendenciaFinanceiraSerasa
                                {
                                    SubTipo = protocolo.Item2,
                                    TotalOcorrenciasDebitos = bloco.Substring(6, 5),
                                    Dataultimadebito = bloco.Substring(11, 6),
                                    DebitosTotais = Convert.ToString((decimal?)decimal.Parse(bloco.Substring(23, 15)) / 100),
                                });
                            }

                            else if (captura.Key == "N250")
                            {
                                faixaConteudo[captura.Key].Add(new Protesto
                                {
                                    SubTipo = protocolo.Item2,
                                    ProtestoProtestoTotalOcorrencia = bloco.Substring(6, 5),
                                    ProtestoDataOcorrenciaAntiga = bloco.Substring(11, 6),
                                    ProtestoDataOcorrenciaRecente = bloco.Substring(17, 6),
                                    ProtestosMoedaTotal = bloco.Substring(23, 3),
                                    ProtestosValorTotal = !String.IsNullOrEmpty(bloco.Substring(26, 15)) ? Convert.ToString((decimal?)decimal.Parse(bloco.Substring(26, 15)) / 100) : "0",
                                }) ;
                            }

                            else if (captura.Key == "N270")
                            {
                                faixaConteudo[captura.Key].Add(new ChequeBacen
                                {
                                    SubTipo = protocolo.Item2,
                                    DataOcorrencia = bloco.Substring(7, 8),
                                    NumCheque = bloco.Substring(15, 10),
                                    AlineaCheque = bloco.Substring(25, 5),
                                    QdadeCCFparaBanco = bloco.Substring(30, 5),
                                    ValorTotalCheque = bloco.Substring(35, 15),
                                    NumBanco = bloco.Substring(50, 3),
                                    NomeBanco = bloco.Substring(53, 14),
                                    NumAgencia = bloco.Substring(67, 4),
                                    CidadeAgencia = bloco.Substring(71, 30),
                                    UfAgencia = bloco.Substring(101, 2),
                                });
                            }
                            break;
                        //Descrição caso sem registro
                        case "99":

                            if (captura.Key == "N240")
                            {
                                faixaConteudo[captura.Key].Add(new PendenciaFinanceiraSerasa
                                {
                                    SubTipo = protocolo.Item2,
                                    Mensagem = bloco.Substring(6, 23)
                                });
                            }

                            else if (captura.Key == "N250")
                            {
                                faixaConteudo[captura.Key].Add(new Protesto
                                {
                                    SubTipo = protocolo.Item2,
                                    Mensagem = bloco.Substring(6, 23)
                                });
                            }

                            else if (captura.Key == "N270")
                            {
                                faixaConteudo[captura.Key].Add(new ChequeBacen
                                {
                                    SubTipo = protocolo.Item2,
                                    Mensagem = bloco.Substring(6, 23)
                                });
                            }
                            else if (captura.Key == "T999")
                            {
                                if (bloco.Contains("MENOR DE IDADE PROTEGIDO LEGALMENTE"))
                                {
                                    faixaConteudo[captura.Key].Add(new Confirmei
                                    {
                                        SubTipo = "00",
                                        Mensagem = HtmlEnumExtensions.GetEnumDescription<SituacaoSerasaPessoaFisica>((SituacaoSerasaPessoaFisica)1),
                                    });
                                }
                                faixaConteudo[captura.Key].Add(new T999 { SubTipo = protocolo.Item2, Mensagem = bloco.Substring(7, 70) });
                            }
                            break;
                        default:
                            if (captura.Key == "F900")
                            {
                                var objScore = new Repository.Model.Score(documento);
                                objScore.SubTipo = protocolo.Item2;
                                if (documento.Length == 11)
                                {
                                    /*  ---         Pessoa Física         --- */
                                    objScore.ScoreSerasa = bloco.Substring(9, 5).ToString();
                                    objScore.FaixaScore = bloco.Substring(14, 6);
                                    objScore.InadimplenciaProbab = bloco.Substring(20, 4);
                                    objScore.MsgScore = bloco.Substring(25, 49);
                                }
                                else
                                {
                                    /*  ---         Pessoa Jurídica         --- */
                                    objScore.ScoreSerasa = bloco.Substring(32, 4);
                                    objScore.MsgScore = bloco.Substring(41, 74);
                                    objScore.ScoreRetornoInformacoes = bloco.Substring(12, 8);
                                    objScore.ScoreDataConsulta = bloco.Substring(12, 8);
                                    objScore.ScoreHoraConsulta = bloco.Substring(20, 8);
                                    objScore.ClasseTabelaScore = bloco.Substring(28, 4);
                                    objScore.MsgInformativaPj = bloco.Substring(114, 1);
                                    objScore.CalculoRisco = bloco.Substring(0, 4);
                                }

                                faixaConteudo[captura.Key].Add(objScore);
                            }
                            break;
                    }
                }
            }
            var rows = await GenerateHtmlData(faixaConteudo.Where(p => p.Value.Count > 0).ToDictionary(p => p.Key, p => p.Value));
            retorno += rows;
            return retorno;
        }

        private async Task<string> GenerateHtmlData(Dictionary<string, List<ISerasaPadraoRetorno>> parametros)
        {
            var retorno = "";
            var blocos = new Dictionary<string, string>();

            foreach (var conteudo in parametros.OrderBy(p => p.Key))
            {
                string bloco = $"<div class='row align-text-center'><b>{String.Format("{0} | {1}", HtmlEnumExtensions.GetEnumDescription(conteudo.Value[0].TipoProtocolo), conteudo.Key)}</b></div>";
                if (conteudo.Value.Any(p => p.SubTipo == "99"))
                {
                    string mensagem = conteudo.Value.Select(p => p.Mensagem).FirstOrDefault();
                    bloco += $"<div class='row'>{mensagem}</div>";
                    blocos.Add(conteudo.Key, bloco);
                    continue;
                    /*      ---     Nesse caso quer dizer que não houve Ocorrencia do protocolo mencionado     ---     
                                        ---     restando apenas retornar uma mensagem     ---     
                    */
                }
                if (conteudo.Key == "T999")
                {
                    bloco += $"<div class='row'>{conteudo.Value[0].GetItens()[0]}</div>";
                    blocos.Add(conteudo.Key, bloco);
                    continue;
                }
                //Total                        
                var totalProtocolos = conteudo.Value.Where(p => p.Protocolo == conteudo.Key && p.SubTipo == "90").ToList();

                decimal valor = 0;
                int ocorrencias = 0;
                if (totalProtocolos.Any(p => p.Protocolo == "N240"))
                {
                    foreach (var totalProtocolo in totalProtocolos)
                    {
                        var total = totalProtocolo.GetTotal();
                        valor += (decimal.Parse(total[0]));
                        ocorrencias += Convert.ToInt32(total[1]);

                    }
                    bloco += $"<div class='row'>Valor Total: R${Convert.ToString(valor)}</div> \r\n <div class='row'>Total de Ocorrências de Débito: {ocorrencias} </div> \r\n";
                }
                else
                {
                    bloco += String.Join("\r\n", totalProtocolos.SelectMany(p => p.GetTotal())
                                                                                  .Select(p => p != "<br/>" ? $"<div class='row'> ° {p} </div>" : p)
                                                                                  .ToList());
                }

                //Itens
                var itens = conteudo.Key == "N240" ? conteudo.Value.Where(p => p.Protocolo == conteudo.Key && p.SubTipo == "00").Reverse().Take(10).ToList() :

                                                     conteudo.Value.Where(p => p.Protocolo == conteudo.Key && (p.SubTipo == "00" || p.Protocolo == "F900"))
                                                                   .Reverse().Take(1).ToList();

                bloco += String.Join("\r\n", itens.SelectMany(p => p.GetItens())
                                                                    .Select(p => p != "<br/>" ? $"<div class='row'> ° {p} </div>" : p)
                                                                    .ToList());

                //var complementos = conteudo.Key == "N240" ? conteudo.Value.Where(p => p.Protocolo == conteudo.Key && p.SubTipo == "01").Reverse().Take(10).ToList() :
                //                                     conteudo.Value.Where(p => p.Protocolo == conteudo.Key && (p.SubTipo == "01"))
                //                                                   .Reverse().Take(1).ToList();

                //bloco += String.Join("\r\n", itens.SelectMany(p => p.GetComplemento()).Select(p => p != "<br/>" ? $"<div class='row'> ° {p} </div>" : p)
                //               .ToList());

                blocos.Add(conteudo.Key, bloco);
            }
            retorno += String.Join("<hr/>\r\n", blocos.Select(p => p.Value).ToList());
            return retorno;
        }


        public async Task<string> GetHtmlResumoConsultaSerasaExperian(string consulta, string documento, bool pessoaFisica)
        {
            if (string.IsNullOrEmpty(consulta))
                throw new Exception("Consulta não pode estar vazio ou nulo.");

            ConsultaSerasaResponse consultaSerasa = JsonConvert.DeserializeObject<ConsultaSerasaResponse>(consulta);

            if (consultaSerasa is null)
                throw new Exception("Consulta não pode estar vazio ou nulo.");

            ConsultaSerasaExperianHtml consultaSerasaModelo = null;
            string rows = String.Empty;
            if (consultaSerasa.reports != null)
            {
            foreach (var report in consultaSerasa.reports)
            {
                if (consultaSerasaModelo == null)
                {
                    consultaSerasaModelo = new ConsultaSerasaExperianHtml
                    {
                        Documento = pessoaFisica ? report?.registration?.documentNumber : report?.registration?.companyDocument,
                        SituacaoDocumento = report?.registration?.statusRegistration,
                        NomeMae = pessoaFisica ? report?.registration?.motherName : "Não consta.",
                        NomeConsumidor = pessoaFisica ? report?.registration?.consumerName : report?.registration?.companyName,
                        PessoaFisica = pessoaFisica ? true : false,

                        Score = new ScoreHtml()
                        {

                            ScoreSerasa = report?.score?.score != null ? report?.score?.score.ToString() : "Não informado pela base de consulta.",
                            FaixaScore = report?.score?.range != null ? report?.score?.range.ToString() : "Não informado pela base de consulta.",
                            ModeloScore = report?.score?.scoreModel != null ? report?.score?.scoreModel.ToString() : "Não informado pela base de consulta.",
                            MsgScore = report?.score?.message != null ? report?.score?.message : "Não informado pela base de consulta."
                        },

                        PendenciasFinanceiras = new PendenciasFinanceirasHtml()
                        {
                            ValorTotalPendenciasFinanceiras = report.negativeData.pefin.summary?.balance != null ? report.negativeData.pefin.summary?.balance.ToString("C", new CultureInfo("pt-BR")) : "R$ 0,00",
                            TotalPendenciasDebitos = report.negativeData.pefin.summary?.count != null ? report.negativeData.pefin.summary?.count.ToString() : "0",
                            UltimaOcorrenciaPendencias = report.negativeData.pefin.summary?.lastOccurrence != null ? Convert.ToDateTime(report.negativeData.pefin.summary?.lastOccurrence).ToString("dd/MM/yyyy") : "Não informado pela base de consulta.",
                        },

                        Checks = new ChecksHtml()
                        {
                            ValorTotalTotalChecks = report.negativeData.check.summary?.balance != null ? report.negativeData.check.summary?.balance.ToString("C", new CultureInfo("pt-BR")) : "R$ 0,00",
                            TotalChecks = report.negativeData.check.summary?.count != null ? report.negativeData.check.summary?.count.ToString() : "0",
                            UltimaOcorrenciaCheck = report.negativeData.check.summary?.lastOccurrence != null ? Convert.ToDateTime(report.negativeData.check.summary?.lastOccurrence).ToString("dd/MM/yyyy") : "Não informado pela base de consulta.",
                        },

                        Protestos = new ProtestosHtml()
                        {
                            ValorTotalProtestos = report.negativeData.notary.summary?.balance != null ? report.negativeData.notary.summary?.balance.ToString("C", new CultureInfo("pt-BR")) : "R$ 0,00",
                            CidadesProtestosUF = new List<string>(),
                            CidadesProtestos = new List<string>(),
                            UltimaOcorrenciaProtestos = string.Empty
                            }
                    };
                    }

                    if (report.negativeData.notary.notaryResponse != null)
					{
                        foreach (var item in report.negativeData.notary.notaryResponse)
                        {
                            if (!string.IsNullOrEmpty(item.city))
                                consultaSerasaModelo.Protestos.CidadesProtestos.Add(item.city);

                            if (!string.IsNullOrEmpty(item.federalUnit))
                                consultaSerasaModelo.Protestos.CidadesProtestosUF.Add(item.federalUnit);

                            if (string.IsNullOrEmpty(consultaSerasaModelo.Protestos.UltimaOcorrenciaProtestos))
                                consultaSerasaModelo.Protestos.UltimaOcorrenciaProtestos = Convert.ToDateTime(item.occurrenceDate).ToString("dd/MM/yyyy");
                            else if (Convert.ToDateTime(consultaSerasaModelo.Protestos.UltimaOcorrenciaProtestos) < Convert.ToDateTime(item.occurrenceDate))
                                consultaSerasaModelo.Protestos.UltimaOcorrenciaProtestos = Convert.ToDateTime(item.occurrenceDate).ToString("dd/MM/yyyy");
                        }
				}
            }
                rows = await GenerateHtmlDataExperian(consultaSerasaModelo);
            }
            else if (consultaSerasa.Erros != null)
            {
                if (consultaSerasa?.Erros?.Mensagem != null)
                {
                    var texto = String.Empty;
                    if (consultaSerasa.Erros.Mensagem.Contains("DOCUMENT_NOT_FOUND"))
                    {
                        rows = "<p>Não foi possível encontrar o CPF digitado na base do Serasa.</p>";
                    }
                    else if (consultaSerasa.Erros.Mensagem.Contains("PERSON_DECEASED"))
                    {
                        rows = "<p>Pessoa falecida na base da Serasa Experian.</p>";
                    }
                    else if (consultaSerasa.Erros.Mensagem.Contains("UNDERAGE"))
                    {
                        rows = "<p>Documento pertence a menor de idade, sem informações disponíveis.</p>";
                    }
                    else
                    {
                        rows = $"<p>{consultaSerasa?.Erros?.Mensagem}</p>";
                    }
                }
            }

            return rows;

        }

        private async Task<string> GenerateHtmlDataExperian(ConsultaSerasaExperianHtml consultaModeloHtml)
        {
            var htmlRetorno = new StringBuilder();

            htmlRetorno.AppendLine("<br/>");
            htmlRetorno.AppendLine("<br/>");
            htmlRetorno.AppendLine("<h4 style=\"text-align:center\">Consulta Serasa Exp. - @TIPOPESSOA - @NOMECONSUMIDOR</h4>");
            htmlRetorno.AppendLine("<hr/>");
            htmlRetorno.AppendLine("<h5 style=\"margin-top: 30px;\"><b>Informações Básicas Cadastrais</b></h5>");        
            htmlRetorno.AppendLine("<p>Documento: @DOCUMENTO </p>");
            htmlRetorno.AppendLine("<p>Situação Documento: @SITUACAODOCUMENTO </p>");
            htmlRetorno.AppendLine("<p>Nome Mãe: @NOMEMAE </p>");
            htmlRetorno.AppendLine("<hr/>");
            htmlRetorno.AppendLine("<h5 style=\"margin-top: 30px;\"><b>SCORE</b></h5>");
            htmlRetorno.AppendLine("<p>Score Serasa: @SCORESERASA </p>");
            htmlRetorno.AppendLine("<p>Faixa de Score: @FAIXASCORE </p>");
            htmlRetorno.AppendLine("<p>Modelo Score: @MODELOSCORE </p>");
            htmlRetorno.AppendLine("<p>Mensagem Score: @MSGSCORE </p>");
            htmlRetorno.AppendLine("<hr/>");
            htmlRetorno.AppendLine("<h5 style=\"margin-top: 30px;\"><b>Pêndencias Financeiras</b></h5>");
            htmlRetorno.AppendLine("<p>Valor Total das Pêndencias Financeiras: @VALORTOT_PEN </p>");
            htmlRetorno.AppendLine("<p>Quantidade Total de Pêndencias: @TOTAL_PEN </p>");
            htmlRetorno.AppendLine("<p>Última Ocorrência de Pêndencia: @ULTOCORREN_PEN </p>");
            htmlRetorno.AppendLine("<hr/>");
            htmlRetorno.AppendLine("<h5 style=\"margin-top: 30px;\"><b>Cheques</b></h5>");
            htmlRetorno.AppendLine("<p>Valor Total dos Cheques: @VALORTOT_CHECKS </p>");
            htmlRetorno.AppendLine("<p>Quantidade Total de Cheques: @TOTAL_CHECKS </p>");
            htmlRetorno.AppendLine("<p>Última Ocorrência de Cheques: @ULTOCORREN_CHECK </p>");
            htmlRetorno.AppendLine("<hr/>");
            htmlRetorno.AppendLine("<h5 style=\"margin-top: 30px;\"><b>Protestos</b></h5>");
            htmlRetorno.AppendLine("<p>Valor Total dos Protestos: @VALORTOT_PROT </p>");
            htmlRetorno.AppendLine("<p style=\"margin-top:5px;\">UF das Cidades em que ocorreram os Protestos: </p>");
            htmlRetorno.AppendLine("<ul style=\"margin-top:5px;\">");

            foreach (var uf in consultaModeloHtml.Protestos.CidadesProtestosUF.Distinct())
            {
                htmlRetorno.AppendLine($"<li>{uf}</li>");
            }

            htmlRetorno.AppendLine("</ul>");
            htmlRetorno.AppendLine("<p>Última Ocorrência dos Protestos: @ULTOCORREN_PROT </p>");

            string retornoValidado = htmlRetorno.ToString();

            retornoValidado = retornoValidado.Replace("@TIPOPESSOA", consultaModeloHtml.PessoaFisica ? "PESSOA FÍSICA" : "PESSOA JURÍDICA")
                                 .Replace("@NOMECONSUMIDOR", consultaModeloHtml.NomeConsumidor)
                                 .Replace("@DOCUMENTO", consultaModeloHtml.Documento)
                                 .Replace("@SITUACAODOCUMENTO", consultaModeloHtml.SituacaoDocumento)
                                 .Replace("@NOMEMAE", consultaModeloHtml.NomeMae)
                                 .Replace("@SCORESERASA", consultaModeloHtml.Score.ScoreSerasa)
                                 .Replace("@FAIXASCORE", consultaModeloHtml.Score.FaixaScore)
                                 .Replace("@MODELOSCORE", consultaModeloHtml.Score.ModeloScore)
                                 .Replace("@MSGSCORE", consultaModeloHtml.Score.MsgScore)
                                 .Replace("@VALORTOT_PEN", consultaModeloHtml.PendenciasFinanceiras.ValorTotalPendenciasFinanceiras)
                                 .Replace("@TOTAL_PEN", consultaModeloHtml.PendenciasFinanceiras.TotalPendenciasDebitos)
                                 .Replace("@ULTOCORREN_PEN", consultaModeloHtml.PendenciasFinanceiras.UltimaOcorrenciaPendencias)
                                 .Replace("@VALORTOT_CHECKS", consultaModeloHtml.Checks.ValorTotalTotalChecks)
                                 .Replace("@TOTAL_CHECKS", consultaModeloHtml.Checks.TotalChecks)
                                 .Replace("@ULTOCORREN_CHECK", consultaModeloHtml.Checks.UltimaOcorrenciaCheck)
                                 .Replace("@VALORTOT_PROT", consultaModeloHtml.Protestos.ValorTotalProtestos)
                                 .Replace("@ULTOCORREN_PROT", consultaModeloHtml.Protestos.UltimaOcorrenciaProtestos);


            return retornoValidado;
        }
        #endregion
    }
}
