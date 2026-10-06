using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.T5.Model.nhibernate;
using Newtonsoft.Json;
using NHibernate;
using RestSharp;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static Aniel.Connect.API_Aniel.Models.GeoGridModels;
using System.Text.Json;
using static Aniel.Connect.API_Aniel.Models.HubSoftModels;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;
using log4net;

public class IntegracaoExternaHelper /*: IIntegracaoExternaGeositeApiController*/
{
    private ISession _session;
    private ISession _sessionT5;
    public IntegracaoExternaHelper(ISession session)
    {
        _session = session;
    }
    public IntegracaoExternaHelper(ISession session, ISession sessionT5)
    {
        _session = session;
        _sessionT5 = sessionT5;
    }
    internal GetApiRetorno<long> CheckConnectionsCliente(long id)
    {
        GetApiRetorno<long> retorno = new GetApiRetorno<Int64>();
        using (_sessionT5)
        using (Conexao<Elemento_Rede_Porta> repositorio = new Conexao<Elemento_Rede_Porta>(_sessionT5))
        {
            StringBuilder sql = new StringBuilder();
            var question = repositorio.select(p => p.Cliente_Plano_Prod == id).Result;
            retorno.Objeto = question?.Count() ?? 0;
        }
        return retorno;
    }
    //internal ClienteGeosite BindClienteToGeositeObj(Cliente_PlanoProduto clientePlanoProduto)
    //{
    //    var client = new ClienteGeosite();

    //    //client.codigo = clientePlanoProduto. ;
    //    //client.codigoCaixa = clientePlanoProduto. ;
    //    //client.codigoOnu = clientePlanoProduto. ;
    //    client.documento = clientePlanoProduto.Cliente.CpfCnpj;
    //    client.complementoEndereco = clientePlanoProduto.Cliente.Endereco_Principal.Complemento;

    //    client.latitude = clientePlanoProduto.Endereco_Instalacao.Latitude;
    //    client.longitude = clientePlanoProduto.Endereco_Instalacao.Longitude;
    //    client.nome = clientePlanoProduto.Cliente.Razao_Social;
    //    client.contrato = clientePlanoProduto.ClienteContrato?.NumeroContrato;


    //    return client;
    //}

    internal async Task<ResponseGeosite> Adicionar_ConexaoCliente(IntegracoesExternas obj, Elemento_Rede_Porta portaObj, long Id_Clipp)
    {
        String response = String.Empty;

        using (Conexao<Cliente_PlanoProduto> repoClipp = new Conexao<Cliente_PlanoProduto>(_session))
        {
            String url = $"{obj.UrlApi}/fusoesCaixa/adicionarConexao";

            long? clientePlanoProduto = portaObj?.Cliente_Plano_Prod;
            GeositeCaixaPortaFtthModel Param = new GeositeCaixaPortaFtthModel();

            //jsonParam.fidCaixa = portaObj?.Caixa.Id;
            Param.fidCliente = (await repoClipp.selectFirst(p => p.Id == clientePlanoProduto))?.Cliente.Id;

            String jsonParam = JsonService.SerializeObject(Param);

            response = this.RequestTemplate(obj, url, Method.POST, jsonParam);

        }

        return JsonService.DeserializeObject<ResponseGeosite>(response);
    }

    internal ResponseGeogrid Reservar_PortaGeoGrid(IntegracoesExternas obj, Elemento_Rede_Porta portaObj)
    {

        var objeto = new
        {
            reservas = new[]
                   {
                                       new
                                       {
                                        idPorta = portaObj.Porta,
                                        idCliente = portaObj.Login
                                       }
                                    }
        };
        string json = JsonConvert.SerializeObject(objeto);
        var client = new RestClient();
        var request = new RestRequest(obj.UrlApi + "api/v3/diagrama/reserva", Method.PUT);
        request.AddHeader("api-key", obj.Senha);
        request.AddJsonBody(json);
        var response = (RestResponse)client.Execute(request);
        var reserva = JsonConvert.DeserializeObject<ResponseGeogrid>(response.Content);

        return JsonService.DeserializeObject<ResponseGeogrid>(response.Content);
    }


    internal ResponseGeositeReserva Reservar_Porta(IntegracoesExternas obj, Elemento_Rede_Porta portaObj, long IdClienteGeosite)
    {
        String response = String.Empty;


        using (Conexao<Cliente_PlanoProduto> repoClipp = new Conexao<Cliente_PlanoProduto>(_session))
        {
            string url = $"{obj.UrlApi}/fusoesCaixa/reservarPorta";

            RestClient cliente = new RestClient(url);
            RestRequest request = new RestRequest(Method.POST);
            //Cliente_PlanoProduto objeto = (await repoClipp.selectFirst(p => p.Id == Id_Clipp));
            GeositeCaixaPortaFtthModel Param = new GeositeCaixaPortaFtthModel();

            Param.fidCaixa = portaObj?.Caixa.Id;
            Param.fidCliente = IdClienteGeosite;

            String jsonParam = JsonService.SerializeObject(Param);

            response = this.RequestTemplate(obj, url, Method.POST, jsonParam);
        }

        return JsonService.DeserializeObject<ResponseGeositeReserva>(response);
    }

    internal ResponseGeosite Remover_Cliente(IntegracoesExternas obj, long idClienteGeosite)
    {
        string response = String.Empty;

        using (Conexao<Cliente_PlanoProduto> repoClipp = new Conexao<Cliente_PlanoProduto>(_session))
        {
            long? fidCliente = Convert.ToInt64(idClienteGeosite);
            string url = $"{obj.UrlApi}/fusoesCaixa/removerCliente?fidCliente={fidCliente}";


            response = this.RequestTemplate(obj, url, Method.POST);
        }
        return JsonService.DeserializeObject<ResponseGeosite>(response);
    }

    internal ResponseGeosite Remover_ConexaoCliente(IntegracoesExternas obj, Elemento_Rede_Porta portaObj, long Id_Clipp)
    {
        string response = String.Empty;

        using (Conexao<Cliente_PlanoProduto> repoClipp = new Conexao<Cliente_PlanoProduto>(_session))
        {

            string url = $"{obj.UrlApi}/fusoesCaixa/removerConexaoCliente?fidCliente={Id_Clipp}";

            long? clientePlanoProduto = portaObj?.Cliente_Plano_Prod;

            response = this.RequestTemplate(obj, url, Method.POST);
        }
        return JsonService.DeserializeObject<ResponseGeosite>(response);
    }

    internal ResponseGeosite SaveCliente(IntegracoesExternas obj, ClienteGeosite Cliente)
    {

        string url = $"{obj.UrlApi}/cliente/save";
        SaveModelCliente clienteRequisicao = this.BindSaveModel(Cliente);

        string param = JsonService.SerializeObject(clienteRequisicao);

        string response = this.RequestTemplate(obj, url, Method.POST, param);

        return JsonService.DeserializeObject<ResponseGeosite>(response);
    }

    internal SaveModelCliente BindSaveModel(ClienteGeosite cliente)
    {
        return new SaveModelCliente
        {
            bairro = cliente.bairro,
            codigo = cliente.codigo,
            codigoCaixa = cliente.codigoCaixa,
            codigoOnu = cliente.codigoOnu,
            complementoEndereco = cliente.complementoEndereco,
            comprimentoFibra = cliente.comprimentoFibra,
            contrato = cliente.contrato,
            documento = cliente.documento,
            endereco = cliente.endereco,
            fidCaixa = cliente.fidCaixa,
            fidTipoCaixa = cliente.fidTipoCaixa,
            infoComplementar = cliente.infoComplementar,
            latitude = cliente.latitude,
            longitude = cliente.longitude,
            macOnu = cliente.macOnu,
            nome = cliente.nome,
            numeroPorta = cliente.numeroPorta,
            qtdPortasAtendimento = cliente.qtdPortasAtendimento,
            telefone = cliente.telefone,
            fidTipo = cliente.fidTipo,
        };
    }

    internal ClienteGeosite BindClienteGeosite(Cliente cliente, Elemento_Rede_Porta porta)
    {
        return new ClienteGeosite
        {
            //codigo =,
            //codigoCaixa =porta.Caixa.,
            //codigoOnu =,
            //comprimentoFibra =,
            nome = cliente.Razao_Social,
            fidTipo = 1,
            fidTipoCaixa = 2,
            endereco = cliente?.Endereco_Principal?.Logradouro,
            bairro = cliente?.Endereco_Principal?.Bairro,
            telefone = cliente.getTelefone(),
            qtdPortasAtendimento = porta.Caixa.Capacidade,
            documento = cliente.CpfCnpj,

            contrato = cliente.Numero_Contrato,
            numeroPorta = porta.Porta,

            complementoEndereco = cliente.Endereco_Principal.Complemento,
            fidCaixa = porta.Caixa.Id,

            latitude = cliente.Endereco_Principal.Latitude,
            longitude = cliente.Endereco_Principal.Longitude,

        };
    }
    internal ResponseGeogrid VinculaPortaGeoGrid(IntegracoesExternas obj, VincluaPorta objVinculo)
    {
        ResponseGeogrid retorno = new ResponseGeogrid();
        //   var endereco = clientdaoConnect.GetClienteByCodCliente(item.Login);
        var objeto = new
        {
            idPorta = objVinculo.Porta,
            idCliente = objVinculo.Login,
            local = new
            {
                latitude = objVinculo.Latitude.ToString().Replace(",", "."),
                longitude = objVinculo.Longitude.ToString().Replace(",", ".")
            },
            pontos = new[]
    {
        new
        {
            latitude = objVinculo.Latitude.ToString().Replace(",", "."),
            longitude = objVinculo.Longitude.ToString().Replace(",", ".")
        }
    }
        };
        var url = obj.UrlApi + "api/v3/integracao/atender";

        string json = JsonConvert.SerializeObject(objeto);
        var client = new RestClient();
        var request = new RestRequest(url, Method.POST);
        request.AddHeader("api-key", obj.Senha);
        request.AddJsonBody(json);

        var response = client.Execute(request);
        retorno.message = response.Content;

        LogManager.GetLogger("Aniel.Connect.API_Aniel.LogGeoGrid").Error($"[GeoGrid - VinculaPortaGeoGrid] - Json Request: {JsonService.SerializeObject(json)} \n  Json Response: {response.Content} {Environment.NewLine} {Environment.NewLine}");

        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
           
            retorno.success = true;
        }
        else
        { retorno.success = false; }
        return retorno;

    }


public class TokenResponse
{
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }
}
public async Task<PostApiRetorno> VincularPortaHubsoft(IntegracoesExternas integracao, PortaHubSoftRequest request)
    {
        var retorno = new PostApiRetorno();

        try
        {
            //using var repoIntegracoes = new Conexao<IntegracoesExternas>(session);

            //var integracoes = (await repoIntegracoes.select(p => p.IntegracaoExterna == request.Tipo_Integracao)).FirstOrDefault();

            //if (integracoes == null || string.IsNullOrWhiteSpace(integracoes.Token))
            //    throw new Exception("Token de integração não encontrado ou inválido.");

            // Criação do cliente RestSharp
            var client = new RestClient(integracao.UrlApi);
            var token = ObterTokenAsync(integracao);
            // Endpoint com ID
            var requestRest = new RestRequest($"/api/v1/integracao/mapeamento/projeto/cliente_servico/porta_atendimento/{request.IdClienteServico}", Method.PUT);

            // Headers
            requestRest.AddHeader("Authorization", $"Bearer {token}");
            requestRest.AddHeader("Content-Type", "application/json");

            // Corpo da requisição
            requestRest.AddJsonBody(new
            {
                id_porta_atendimento = request.IdPortaAtendimento ,
                forcar_troca_porta = true,
                forcar_remocao_reserva = true
            });

            // Envia requisição
            var response =  client.ExecuteAsync(requestRest).Result;

            if (!response.IsSuccessful)
            {
                throw new Exception($"Erro ao integrar com Hubsoft: {response.StatusCode}\n{response.Content}");
            }

            retorno.Ok = true;
        }
        catch (Exception ex)
        {
            retorno.Ok = false;
            retorno.Mensagem = ex.Message;
            retorno.InnerException = ex.InnerException?.Message;
        }

        return retorno;
    }

    public async Task<PostApiRetorno> LimpezaPortaHubsoft(IntegracoesExternas integracao, string IdClienteServico)
    {
        var retorno = new PostApiRetorno();

        try
        {
            
            // Criação do cliente RestSharp
            var client = new RestClient(integracao.UrlApi);
            var token = ObterTokenAsync(integracao);
            // Endpoint com ID
            var requestRest = new RestRequest($"/api/v1/integracao/mapeamento/projeto/cliente_servico/porta_atendimento/{IdClienteServico}", Method.PUT);

            // Headers
            requestRest.AddHeader("Authorization", $"Bearer {token}");
            requestRest.AddHeader("Content-Type", "application/json");

            // Corpo da requisição
            requestRest.AddJsonBody(new
            {                
                remover = true
                
            });

            // Envia requisição
            var response =  client.ExecuteAsync(requestRest).Result;

            if (!response.IsSuccessful)
            {
                throw new Exception($"Erro ao desvincular porta Hubsoft: {response.StatusCode}\n{response.Content}");
            }

            retorno.Ok = true;
        }
        catch (Exception ex)
        {
            retorno.Ok = false;
            retorno.Mensagem = ex.Message;
            retorno.InnerException = ex.InnerException?.Message;
        }

        return retorno;
    }
    public class ResponseWrapper
    {
        public string Content { get; set; }
    }


    public string ObterTokenAsync(IntegracoesExternas integracao)
    {
        var client = new RestClient(integracao.UrlApi);
        var request = new RestRequest("/oauth/token", Method.POST);

        request.AddHeader("Content-Type", "application/json");

        var body = new
        {
            client_id = integracao.TokenAcesso,
            client_secret = integracao.ApiSecret,
            username = integracao.Usuario,
            password = integracao.Senha,
            grant_type = "password"
        };

        request.AddJsonBody(body);
        var response =  client.Execute(request);

        if (!response.IsSuccessful)
        {
            throw new Exception($"Erro ao autenticar: {response.StatusCode}\n{response.Content}");
        }
        var json = response.Content;
        var jObj = JObject.Parse(json);
        var token = jObj["access_token"];

        return token.ToString();
    }

    internal ResponseGeogrid LimpezaPortaGeoGrid(IntegracoesExternas obj, VincluaPorta objVinculo)
    {
        ResponseGeogrid retorno = new ResponseGeogrid();
        //   var endereco = clientdaoConnect.GetClienteByCodCliente(item.Login);

        var objeto = new
        {
            idPorta = objVinculo.Porta,
            idCliente = objVinculo.Login
           
        };
        var url = obj.UrlApi + $"api/v3/integracao/atender/{objVinculo.Porta}/{objVinculo.Login}";

        string json = JsonConvert.SerializeObject(objeto);
        var client = new RestClient();
        var request = new RestRequest(url, Method.DELETE);
        request.AddHeader("api-key", obj.Senha);
        request.AddJsonBody(json);

        var response = client.Execute(request);
        retorno.message = response.Content;

        LogManager.GetLogger("Aniel.Connect.API_Aniel.LogGeoGrid").Error($"[GeoGrid - LimpezaPortaGeoGrid] - Json Request: {JsonService.SerializeObject(json)} \n  Json Response: {response.Content} {Environment.NewLine} {Environment.NewLine}");

        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {

            retorno.success = true;
        }
        else
        { retorno.success = false; }
        return retorno;
    }

    internal ResponseGeosite<ClienteGeosite> CheckClienteGeosite(IntegracoesExternas obj, string cpfCnpj)
    {
        string response = String.Empty;

        using (Conexao<Cliente_PlanoProduto> repoClipp = new Conexao<Cliente_PlanoProduto>(_session))
        {
            //long? fidCliente = (await repoClipp.selectFirst(p => p.Id == Id_Clipp))?.Cliente.Id;
            string url = $"{obj.UrlApi}/cliente/list?limit=1&columns=fid,documento&sorter=fid&filter=documento='{cpfCnpj}'";

            response = this.RequestTemplate(obj, url, Method.GET);
        }

        return JsonService.DeserializeObject<ResponseGeosite<ClienteGeosite>>(response);

    }

    internal ResponseGeosite Update_Reserva(IntegracoesExternas obj, Elemento_Rede_Porta portaObj, long? Id_ClienteGeosite)
    {
        string response = String.Empty;

        using (Conexao<Cliente_PlanoProduto> repoClipp = new Conexao<Cliente_PlanoProduto>(_session))
        {
            //long? fidCliente = (await repoClipp.selectFirst(p => p.Id == Id_Clipp))?.Cliente.Id;
            string url = $"{obj.UrlApi}/fusoesCaixa/updateReserva?fidCliente={Id_ClienteGeosite}&fidCaixa={portaObj.Caixa.Id}";


            long? clientePlanoProduto = portaObj?.Cliente_Plano_Prod;

            response = this.RequestTemplate(obj, url, Method.POST);
        }
        return JsonService.DeserializeObject<ResponseGeosite>(response);
    }

    [Obsolete]
    internal ResponseGeositeViabilidade Check_Viabilidade(IntegracoesExternas obj, Request_ViabilidadeGeosite requestObject)
    {
        string response = String.Empty;

        using (Conexao<Cliente_PlanoProduto> repoClipp = new Conexao<Cliente_PlanoProduto>(_session))
        {
            //long? fidCliente = (await repoClipp.selectFirst(p => p.Id == Id_Clipp))?.Cliente.Id;
            String url = $"{obj.UrlApi}/viabilidade/caixas?raio={requestObject.Raio}&";

            switch (requestObject.Mudanca_RequisicaoViabilidade)
            {
                case viabilidade_GeositeParameter.LatitudeLongitude:

                    url = $"{url}latitude={requestObject.Latitude.Value.ToString().Replace(",", ".")}&longitude={requestObject.Longitude.Value.ToString().Replace(",", ".")}";
                    break;

                case viabilidade_GeositeParameter.Endereco:

                    url = $"{url}endereco={requestObject.Endereco}";
                    break;

                case viabilidade_GeositeParameter.GooglePlaceId:

                    url = $"{url}googlePlaceId={requestObject.GooglePlaceId}";
                    break;

            }

            response = this.RequestTemplate(obj, url, Method.GET);
        }
        return JsonService.DeserializeObject<ResponseGeositeViabilidade>(response);
    }

    internal String RequestTemplate(IntegracoesExternas obj, String url, Method metodo, String param = "")
    {
        string response = String.Empty;
        RestClient cliente = new RestClient(url);
        RestRequest request = new RestRequest(metodo);


        request.AddHeader("Authorization", $"Digicade-Rest-API:{obj.TokenAcesso}");
        request.AddHeader("Content-Type", "application/json");
        if (!String.IsNullOrEmpty(param))
        {
            request.AddParameter("application/json", param, ParameterType.RequestBody);
        }

        request.AddHeader("cache-control", "no-cache");
        response = cliente.Execute(request).Content;
        return response;
    }
    internal async Task<IEnumerable<IntegracoesExternas>> GetListConfiguracoes(DbCommand com, IntegracaoExterna tipoIntegracao)
    {
        var retorno = new List<IntegracoesExternas>();
        StringBuilder sql = new StringBuilder();

        sql.AppendLine("  SELECT A.ID, A.TOKEN_ACESSO, A.URL_API")
           .AppendLine($" FROM TB_INTEGRACOES_EXTERNAS A WHERE A.INTEGRACAO_EXTERNA = '{Convert.ToInt64(tipoIntegracao)}' ");


        using (DbDataReader reader = await CommandHelper.ExecuteReader(com, sql, _session.Connection, null))
        {
            if (await reader.ReadAsync())
            {
                retorno.Add(new IntegracoesExternas
                {
                    TokenAcesso = Convert.ToString(reader["TOKEN_ACESSO"]),
                    UrlApi = Convert.ToString(reader["URL_API"]),
                    Id = Convert.ToInt64(reader["ID"]),
                });
            }

        }
        return retorno;
    }
    [Obsolete]
    internal viabilidade_GeositeParameter SelectParameterRequisicao(Request_ViabilidadeGeosite requestViabilidade)
    {
        if (requestViabilidade.Latitude != null && requestViabilidade.Longitude != null)
        {
            requestViabilidade.Mudanca_RequisicaoViabilidade = viabilidade_GeositeParameter.LatitudeLongitude;
        }
        else if (!String.IsNullOrEmpty(requestViabilidade.Endereco))
        {
            requestViabilidade.Mudanca_RequisicaoViabilidade = viabilidade_GeositeParameter.Endereco;
        }
        else
        {
            requestViabilidade.Mudanca_RequisicaoViabilidade = viabilidade_GeositeParameter.GooglePlaceId;
        }
        return requestViabilidade.Mudanca_RequisicaoViabilidade;
    }

    public bool ValidateIntegracaoByUsuario(Elemento_Rede request)
    {
        bool pertencente_A_Integracao = false;
        List<string> fatoresIntegracao = new List<string>();
        try
        {
            string campoUtilizado = !string.IsNullOrEmpty(request.UsuarioAlteracao) ? request.UsuarioAlteracao : request.UsuarioCriacao;
            if (!String.IsNullOrEmpty(campoUtilizado))
            {
                fatoresIntegracao.AddRange(campoUtilizado.Split(':'));
            }

            if (fatoresIntegracao.Count > 0)
            {
                fatoresIntegracao = fatoresIntegracao.Select(p =>
                {
                    return p.Trim(' ');
                }).ToList();

                if (fatoresIntegracao[0].ToUpper() == "INTEGRACAO")
                {
                    List<string> Integracoes = Enum.GetNames(typeof(IntegracaoExterna)).ToList();

                    string Integracao = Integracoes.Find(p => p == fatoresIntegracao[1]);

                    pertencente_A_Integracao = !string.IsNullOrEmpty(Integracao) ? true : false;
                }
            }
        }
        catch (Exception)
        {
            //  Dev Tip: Use SHIFT + F9 para acessar quick watch e então avaliar com exatidão o erro gerado.
            pertencente_A_Integracao = false;
        }

        return pertencente_A_Integracao;
    }

    public void Dispose() { }
}





