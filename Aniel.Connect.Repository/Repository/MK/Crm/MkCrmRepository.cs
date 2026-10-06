using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Exceptions.Mk;
using Aniel.Connect.Repository.Model.Mk;
using log4net;
using RestSharp;
using Sinapse.DAO.repository;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aniel.Connect.Repository.MK.Crm
{
    public class MkCrmRepository : IMkCrmRepository
    {
        private readonly RestClient _httpClient;

        /// <summary>
        ///     Token usado para obter o token que dá acesso às APIs do CRM
        /// </summary>
        private readonly string _authToken;
        /// <summary>
        ///     Senha usada para obter o token que dá acesso às APIs do CRM
        /// </summary>
        private readonly string _authPassword;

        /// <summary>
        ///     Token utilizado para ter acesso às APIs do CRM.
        ///     Campo para uso exclusivo para getters da classe, para acessar o token utilize a propriedade 
        ///     <see cref="MkCrmRepository.Token"/>
        /// </summary>
        private string _token;

        /// <summary>
        ///     Data e hora de expiração do token
        /// </summary>
        private DateTime _tokenExpiration = DateTime.MinValue;

        ILog _log;

        private long _IdIntegracaoMk;

        public MkCrmRepository(string urlApiCrmMk, string token, string password, ILog log = null, string tokenDb = null, DateTime? dthExpire = null, long? IdIntegracaoMk = null)
        {
            if (String.IsNullOrEmpty(urlApiCrmMk))
            {
                throw new ArgumentNullException("urlApiCrmMk");
            }

            if (String.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException("token");
            }

            if (String.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("password");
            }

            this._httpClient = new RestClient(urlApiCrmMk);

            this._authToken = token;
            this._authPassword = password;
            this._log = log;
            this._token = tokenDb;
            this._tokenExpiration = dthExpire ?? DateTime.MinValue;
            this._IdIntegracaoMk = IdIntegracaoMk ?? 0;
        }

        /// <summary>
        ///     Token utilizado para ter acesso às APIs do CRM
        /// </summary>
        private string Token
        {
            get
            {
                if (DateTime.Now >= this._tokenExpiration)
                {
                    AutenticacaoCrmResponse authResponse = this.GetTokenAutenticacao(this._authToken, this._authPassword);

                    if (authResponse == null || authResponse.status == "ERRO")
                    {
                        throw new MkCrmObterTokenAutenticacaoException(authResponse?.Mensagem);
                    }

                    using (var session = HibernateUtil.GetSession())
                    using (var repository = new Conexao<IntegracoesExternas>(session))
                    {
                        var obj = repository.selectFirst(x => x.Id == this._IdIntegracaoMk).Result;

                        if (obj != null)
                        {
                            obj.Token = authResponse.Token;
                            obj.ExpirationToken = DateTime.Parse(authResponse.Expire);

                            repository.SaveOrUpdate(obj).Wait();
                        }
                    }

                    this._token = authResponse.Token;
                    this._tokenExpiration = DateTime.Parse(authResponse.Expire);
                }

                return this._token;
            }
        }

        /// <summary>
        ///     Obtém o token de autenticação para uso nas APIs do CRM do MK Solutions
        /// </summary>
        /// <param name="token">Token configurado nas configurações de integração da empresa</param>
        /// <param name="password">Senha configurada nas configurações de integração da empresa</param>
        private AutenticacaoCrmResponse GetTokenAutenticacao(string token, string password)
        {
            var request = new RestRequest("mk/WSAutenticacao.rule", Method.GET);

            request.AddHeader("Accept", "application/json");

            request.AddQueryParameter("sys", "MK0");
            request.AddQueryParameter("token", token);
            request.AddQueryParameter("password", password);
            request.AddQueryParameter("cd_servico", "9999");

            IRestResponse response = this._httpClient.Execute(request);

            return JsonService.DeserializeObject<AutenticacaoCrmResponse>(response.Content);
        }

        public IEnumerable<DadosEstruturaEndereco> GetEnderecos()
        {
            var request = new RestRequest("mk/WSMKListaEstruturaEnderecos.rule", Method.GET);

            request.AddHeader("Accept", "application/json");

            request.AddQueryParameter("sys", "MK0");
            request.AddQueryParameter("token", this.Token);

            IRestResponse response = this._httpClient.Execute(request);

            try
            {
                ListaEstruturaEnderecosResponse responseContent = JsonService.DeserializeObject<ListaEstruturaEnderecosResponse>(response.Content);

                if (responseContent == null || responseContent.status == "ERRO")
                {
                    throw new MkCrmRequestException(responseContent?.Mensagem);
                }

                return responseContent.ListaEnderecos;
            }
            catch
            {
                throw new MkCrmRequestException("Erro ao consultar dados do endereço no MK Solutions");
            }
        }

        public InserirCidadeResponse InserirCidade(InserirCidadeRequest dadosCidade)
        {
            var request = new RestRequest("core-api/cidades", Method.POST);

            request.AddHeader("Accept", "application/json");

            request.AddQueryParameter("token", this.Token);
            request.AddJsonBody(dadosCidade);

            IRestResponse response = this._httpClient.Execute(request);

            try
            {
                InserirCidadeResponse responseContent = JsonService.DeserializeObject<InserirCidadeResponse>(response.Content);

                if (responseContent == null || !String.IsNullOrEmpty(responseContent.message))
                {
                    throw new MkCrmRequestException(responseContent?.message);
                }

                return responseContent;
            }
            catch
            {
                throw new MkCrmRequestException("Erro ao inserir cidade no MK Solutions");
            }
        }

        public InserirBairroResponse InserirBairro(InserirBairroRequest dadosBairro)
        {
            var request = new RestRequest("core-api/bairros", Method.POST);

            request.AddHeader("Accept", "application/json");

            request.AddQueryParameter("token", this.Token);
            request.AddJsonBody(dadosBairro);

            IRestResponse response = this._httpClient.Execute(request);

            try
            {
                InserirBairroResponse responseContent = JsonService.DeserializeObject<InserirBairroResponse>(response.Content);

                if (responseContent == null || !String.IsNullOrEmpty(responseContent.message))
                {
                    throw new MkCrmRequestException(responseContent?.message);
                }

                return responseContent;
            }
            catch
            {
                throw new MkCrmRequestException("Erro ao inserir bairro no MK Solutions");
            }
        }

        public InserirLogradouroResponse InserirLogradouro(InserirLogradouroRequest dadosLogradouro)
        {
            var request = new RestRequest("core-api/logradouros", Method.POST);

            request.AddHeader("Accept", "application/json");

            request.AddQueryParameter("token", this.Token);
            request.AddJsonBody(dadosLogradouro);

            IRestResponse response = this._httpClient.Execute(request);

            try
            {
                InserirLogradouroResponse responseContent = JsonService.DeserializeObject<InserirLogradouroResponse>(response.Content);

                if (responseContent == null || !String.IsNullOrEmpty(responseContent.message))
                {
                    throw new MkCrmRequestException(responseContent?.message);
                }

                return responseContent;
            }
            catch
            {
                throw new MkCrmRequestException($"Erro ao inserir logradouro no MK Solutions {response.Content}");
            }
        }

        public GetDadosClienteByDocMkResponse GetDadosClienteByDocumento(string documento)
        {
            if (String.IsNullOrEmpty(documento))
            {
                throw new ArgumentNullException("documento");
            }

            var request = new RestRequest("mk/WSMKConsultaDoc.rule", Method.POST);

            request.AddHeader("Accept", "application/json");

            request.AddQueryParameter("sys", "MK0");
            request.AddQueryParameter("token", this.Token);
            request.AddQueryParameter("doc", documento);

            IRestResponse response = this._httpClient.Execute(request);

            try
            {
                GetDadosClienteByDocMkResponse dadosCliente = JsonService.DeserializeObject<GetDadosClienteByDocMkResponse>(response.Content);

                if (dadosCliente == null)
                {
                    throw new MkCrmRequestException();
                }

                if (!String.IsNullOrEmpty(dadosCliente.Mensagem) && dadosCliente.Mensagem != "Documento não localizado.")
                {
                    throw new MkCrmRequestException(dadosCliente.Mensagem);
                }

                if (dadosCliente.Mensagem == "Documento não localizado.")
                {
                    // Caso a mensagem de erro indique que o cliente não existe no sistema retornamos nulo.
                    return null;
                }

                return dadosCliente;
            }
            catch (MkCrmRequestException e)
            {
                throw new MkCrmRequestException($"Erro ao consultar dados do cliente no MK Solutions: \n\n {e.Message}");
            }
        }

        public InserirClienteResponse InserirCliente(InserirClienteRequest dadosCliente)
        {
            var request = new RestRequest("mk/WSMKNovaPessoa.rule", Method.GET);

            request.AddHeader("Accept", "application/json");

            request.AddQueryParameter("sys", "MK0");
            request.AddQueryParameter("token", this.Token);

            // Dados do cliente
            request.AddQueryParameter("doc", dadosCliente.doc);
            request.AddQueryParameter("nome", dadosCliente.nome);

            request.AddQueryParameter("cep", dadosCliente.cep);
            request.AddQueryParameter("cd_uf", dadosCliente.cd_uf);
            request.AddQueryParameter("cd_cidade", dadosCliente.cd_cidade);
            request.AddQueryParameter("cd_bairro", dadosCliente.cd_bairro);
            request.AddQueryParameter("cd_logradouro", dadosCliente.cd_logradouro);
            request.AddQueryParameter("numero", dadosCliente.numero);
            request.AddQueryParameter("complemento", dadosCliente.complemento);

            request.AddQueryParameter("cd_empresa", dadosCliente.cd_empresa);
            request.AddQueryParameter("email", dadosCliente.email);
            request.AddQueryParameter("nascimento", dadosCliente.nascimento);
            request.AddQueryParameter("fone", dadosCliente.fone);

            request.AddQueryParameter("lat", dadosCliente.lat);
            request.AddQueryParameter("lon", dadosCliente.lon);

            request.AddQueryParameter("nome_mae", dadosCliente.nome_mae);
            request.AddQueryParameter("rg", dadosCliente.rg);
            request.AddQueryParameter("user_sac", dadosCliente.user_sac);
            request.AddQueryParameter("senha_sac", dadosCliente.senha_sac);

            request.AddQueryParameter("cfop", "5307");

            request.AddQueryParameter("tipo_cadastro", dadosCliente.tipo_cadastro.ToString());
            request.AddQueryParameter("cliente_com", dadosCliente.cliente_com);
            request.AddQueryParameter("revenda", dadosCliente.revenda);
            request.AddQueryParameter("grupo_atend", dadosCliente.grupo_atend);


            IRestResponse response = this._httpClient.Execute(request);

            SalvaLog($"\n\n criando cliente \n {response.ResponseUri.AbsoluteUri}");

            try
            {
                if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    throw new Exception(" \n O MK retornou erro 500");

                InserirClienteResponse responseContent = JsonService.DeserializeObject<InserirClienteResponse>(response.Content);

                if (responseContent == null ||
                    (responseContent.status != "OK" && !String.IsNullOrEmpty(responseContent.Mensagem)))
                {
                    throw new MkCrmRequestException(responseContent?.Mensagem);
                }

                return responseContent;
            }
            catch (Exception e)
            {
                throw new MkCrmRequestException($"Erro ao inserir cliente no MK Solutions {e.Message}");
            }
        }

        public InserirContratoResponse InserirContrato(InserirContratoRequest dadosContrato)
        {
            var request = new RestRequest("mk/WSMKNovoContrato.rule", Method.GET);

            request.AddHeader("Accept", "application/json");

            request.AddQueryParameter("sys", "MK0");
            request.AddQueryParameter("token", this.Token);

            // Dados do contrato
            request.AddQueryParameter("CodigoCliente", dadosContrato.CodigoCliente);
            request.AddQueryParameter("CodigoTipoPlano", dadosContrato.CodigoTipoPlano);
            request.AddQueryParameter("CodigoPlanoAcesso", dadosContrato.CodigoPlanoAcesso);
            request.AddQueryParameter("CodigoRegraVencimento", dadosContrato.CodigoRegraVencimento);
            request.AddQueryParameter("CodigoSLA", dadosContrato.CodigoSLA);
            request.AddQueryParameter("IgnorarRenovacaoAutomatica", dadosContrato.IgnorarRenovacaoAutomatica);
            request.AddQueryParameter("ValorSugeridoRenovacao", dadosContrato.ValorSugeridoRenovacao);
            request.AddQueryParameter("CodigoRegraBloqueio", dadosContrato.CodigoRegraBloqueio);
            request.AddQueryParameter("CodigoFormaPagamento", dadosContrato.CodigoFormaPagamento);
            request.AddQueryParameter("CodigoProfilePagamento", dadosContrato.CodigoProfilePagamento);
            request.AddQueryParameter("CodigoMetodoFaturamento", dadosContrato.CodigoMetodoFaturamento);
            request.AddQueryParameter("CodigoPlanoContas", dadosContrato.CodigoPlanoContas);
            request.AddQueryParameter("ConsultorCRM", dadosContrato.ConsultorCRM);
            request.AddQueryParameter("AguardaAtivacao", dadosContrato.AguardaAtivacao);
            request.AddQueryParameter("cd_empresa", dadosContrato.cd_empresa);


            if (!string.IsNullOrEmpty(dadosContrato.CodigoDescontoAplicado))
                request.AddQueryParameter("CodigoDescontoAplicado", dadosContrato.CodigoDescontoAplicado);

            IRestResponse response = this._httpClient.Execute(request);

            SalvaLog($"\n\n criando contrato \n {response.ResponseUri.AbsoluteUri}");

            try
            {
                InserirContratoResponse responseContent = JsonService.DeserializeObject<InserirContratoResponse>(response.Content);

                if (responseContent == null ||
                    (responseContent.status != "OK" && !String.IsNullOrEmpty(responseContent.Mensagem)))
                {
                    throw new MkCrmRequestException(responseContent?.Mensagem);
                }

                return responseContent;
            }
            catch (Exception e)
            {
                throw new MkCrmRequestException($"Erro ao inserir contrato no MK Solutions {e.Message}");
            }
        }

        public InserirConexaoResponse InserirConexao(InserirConexaoRequest dadosConexao)
        {
            var request = new RestRequest("mk/WSMKCriarConexao.rule", Method.GET);

            request.AddHeader("Accept", "application/json");

            request.AddQueryParameter("sys", "MK0");
            request.AddQueryParameter("token", this.Token);

            // Dados da conexão
            request.AddQueryParameter("CodigoCliente", dadosConexao.CodigoCliente);
            request.AddQueryParameter("CodigoPontoAcesso", dadosConexao.CodigoPontoAcesso);
            request.AddQueryParameter("CodigoContrato", dadosConexao.CodigoContrato);
            request.AddQueryParameter("CodigoServidor", dadosConexao.CodigoServidor);
            request.AddQueryParameter("CodigoPlanoAcesso", dadosConexao.CodigoPlanoAcesso);
            request.AddQueryParameter("CodigoTipoIP", dadosConexao.CodigoTipoIP);
            request.AddQueryParameter("Tecnologia", dadosConexao.Tecnologia);
            request.AddQueryParameter("AutoDesbloqueio", dadosConexao.AutoDesbloqueio);
            request.AddQueryParameter("MacAddress", dadosConexao.MacAddress);
            request.AddQueryParameter("EnderecoCadastro", dadosConexao.EnderecoCadastro);
            request.AddQueryParameter("TipoConexao", dadosConexao.TipoConexao);

            if (!String.IsNullOrEmpty(dadosConexao.PoolMKCentral))
                request.AddQueryParameter("PoolMKCentral", dadosConexao.PoolMKCentral);
            if (!String.IsNullOrEmpty(dadosConexao.TpAutenticacao))
                request.AddQueryParameter("TpAutenticacao", dadosConexao.TpAutenticacao);

            request.AddQueryParameter("Username", dadosConexao.Username);
            request.AddQueryParameter("Password", dadosConexao.Password);

            if (dadosConexao.EnderecoCadastro != "true")
            {
                request.AddQueryParameter("CodigoCidade", dadosConexao.CodigoCidade);
                request.AddQueryParameter("CodigoEstado", dadosConexao.CodigoEstado);
                request.AddQueryParameter("CodigoBairro", dadosConexao.CodigoBairro);
                request.AddQueryParameter("CodigoLogradouro", dadosConexao.CodigoLogradouro);
                request.AddQueryParameter("EnderecoNumero", dadosConexao.EnderecoNumero);
                request.AddQueryParameter("Complemento", dadosConexao.Complemento);
                request.AddQueryParameter("CEP", new string(dadosConexao.CEP.Where(char.IsDigit).ToArray()));
            }

            var curl = ToCurl(request, _httpClient);
            IRestResponse response = this._httpClient.Execute(request);

            SalvaLog($"\n\n criando conexao \n {response.ResponseUri.AbsoluteUri}");

            try
            {
                InserirConexaoResponse responseContent = JsonService.DeserializeObject<InserirConexaoResponse>(response.Content);

                if (responseContent == null ||
                    (responseContent.status != "OK" && !String.IsNullOrEmpty(responseContent.Mensagem)))
                {
                    throw new MkCrmRequestException(responseContent?.Mensagem);
                }

                return responseContent;
            }
            catch (Exception e)
            {
                throw new MkCrmRequestException($"Erro ao inserir conexão no MK Solutions {e.Message}");
            }
        }

        string ToCurl(RestRequest request, RestClient client = null)
        {
            var sb = new StringBuilder();
            sb.Append("curl ");

            var method = request.Method.ToString().ToUpper();
            sb.Append($"-X {method} ");

            var url = client != null ? $"{client.BaseUrl}{request.Resource}" : request.Resource;

            // Query parameters
            var queryParams = string.Join("&", request.Parameters
                .Where(p => p.Type == ParameterType.QueryString)
                .Select(p => $"{Uri.EscapeDataString(p.Name)}={Uri.EscapeDataString(p.Value?.ToString() ?? "")}"));

            if (!string.IsNullOrEmpty(queryParams))
                url += (url.Contains("?") ? "&" : "?") + queryParams;

            sb.Append($"\"{url}\" ");

            // Headers
            foreach (var header in request.Parameters.Where(p => p.Type == ParameterType.HttpHeader))
            {
                sb.Append($"-H \"{header.Name}: {header.Value}\" ");
            }

            // Body
            var bodyParam = request.Parameters.FirstOrDefault(p => p.Type == ParameterType.RequestBody);
            if (bodyParam != null && bodyParam.Value != null)
            {
                var bodyContent = bodyParam.Value.ToString();
                if (bodyContent.Contains("\n") || bodyContent.Contains("'"))
                    sb.Append($"--data-binary ${bodyContent.Replace("'", "\\'")} ");
                else
                    sb.Append($"--data '{bodyContent}' ");
            }

            return sb.ToString().Trim();
        }

        public InserirAtendimentoResponse InserirAtendimento(InserirAtendimentoRequest dadosAtendimento)
        {
            var request = new RestRequest("mk/WSMKNovoAtendimento.rule", Method.GET);

            request.AddHeader("Accept", "application/json");

            request.AddQueryParameter("sys", "MK0");
            request.AddQueryParameter("token", this.Token);

            request.AddQueryParameter("cd_contrato", dadosAtendimento.cd_contrato);
            request.AddQueryParameter("cd_cliente", dadosAtendimento.cd_cliente);
            request.AddQueryParameter("cd_processo", dadosAtendimento.cd_processo);
            request.AddQueryParameter("cd_classificacao_ate", dadosAtendimento.cd_classificacao_ate);
            request.AddQueryParameter("origem_contato", "2");
            request.AddQueryParameter("cd_grupo_visualizadores", dadosAtendimento.cd_grupo_visualizadores);
            request.AddQueryParameter("info", dadosAtendimento.info);
            request.AddQueryParameter("op_abertura", dadosAtendimento.op_abertura);
            request.AddQueryParameter("conexao_associada", dadosAtendimento.conexaoId);

            IRestResponse response = this._httpClient.Execute(request);

            SalvaLog($"\n\n criando atendimento \n {response.ResponseUri.AbsoluteUri}");

            try
            {
                InserirAtendimentoResponse responseContent = JsonService.DeserializeObject<InserirAtendimentoResponse>(response.Content);

                if (responseContent == null ||
                    (responseContent.status != "OK" && !String.IsNullOrEmpty(responseContent.Mensagem)))
                    throw new MkCrmRequestException(responseContent?.Mensagem);

                return responseContent;
            }
            catch (Exception e)
            {
                throw new MkCrmRequestException($"Erro ao inserir atendimento no MK Solutions {e.Message}");
            }
        }

        public InserirOrdemServicoResponse InserirOrdemServico(CriarOrdemServicoRequest parametros)
        {
            var request = new RestRequest("mk/WSMKCriarOrdemServico.rule", Method.GET);

            request.AddHeader("Accept", "application/json");

            request.AddQueryParameter("sys", "MK0");
            request.AddQueryParameter("token", Token);

            request.AddParameter("CodigoCliente", parametros.CodigoCliente);
            request.AddParameter("CodigoConexao", parametros.CodigoConexao);
            request.AddParameter("CodigoContrato", parametros.CodigoContrato);
            request.AddParameter("DescricaoProblema", parametros.DescricaoProblema, ParameterType.QueryString);
            request.AddParameter("CodigoTipoOS", parametros.CodigoTipoOS);
            request.AddParameter("CodigoTecnico", parametros.CodigoTecnico);
            request.AddParameter("CodigoGrupoServico", parametros.CodigoGrupoServico);
            request.AddParameter("cidade", parametros.Cidade);
            request.AddParameter("bairro", parametros.Bairro);
            request.AddParameter("logradouro", parametros.Logradouro);
            request.AddParameter("numero", parametros.Numero);
            request.AddParameter("CodigoAtendimento", parametros.CodigoAtendimento);

            request.AddParameter("categoria", "1");

            IRestResponse response = _httpClient.Execute(request);

            SalvaLog($"\n\n criando ordem de serviço \n {response.ResponseUri.AbsoluteUri}");

            try
            {
                InserirOrdemServicoResponse responseContent = JsonService.DeserializeObject<InserirOrdemServicoResponse>(response.Content);

                if (responseContent == null ||
                    (responseContent.status != "OK" && !String.IsNullOrEmpty(responseContent.Mensagem)))
                    throw new MkCrmRequestException(responseContent?.Mensagem);

                return responseContent;
            }
            catch (Exception e)
            {
                throw new MkCrmRequestException($"Erro ao inserir ordem de serviço no MK Solutions {e.Message}");
            }
        }

        void SalvaLog(string value)
        {
            _log?.Info(value);
        }
    }
}
