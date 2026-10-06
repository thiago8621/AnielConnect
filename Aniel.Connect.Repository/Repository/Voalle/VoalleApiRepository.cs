using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Exceptions;
using Aniel.Connect.Repository.Exceptions.Voalle;
using Aniel.Connect.Repository.Model;
using log4net;
using Npgsql;
using RestSharp;
using Sinapse.DAO.repository;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.Voalle
{
    /// <summary>
    ///     Classe para realizar requisições para a API da Voalle
    /// </summary>
    public class VoalleApiRepository : IVoalleRepository
    {
        /// <summary>
        ///     Client HTTP com a URL base da API da Voalle
        /// </summary>
        private readonly RestClient _httpClient;

        private readonly string _apiKey;
        private readonly string _apiSecret;
        private readonly string _urlClient;
        private readonly string _username;
        private readonly string _password;
        private readonly string _token;

        private string JwtToken
        {
            get
            {
                return this.GetAccessToken().access_token;
            }
        }

        /// <param name="apiKey">Chave necessária para autenticação das requisições</param>
        /// <param name="apiSecret">Secret necessário para autenticação das requisições</param>
        public VoalleApiRepository(IntegracoesExternas dadosIntegracaoVoalle)
        {
            if (String.IsNullOrEmpty(dadosIntegracaoVoalle.ApiKey))
                throw new ArgumentNullException("apiKey");

            if (String.IsNullOrEmpty(dadosIntegracaoVoalle.ApiSecret))
                throw new ArgumentNullException("apiSecret");

            if (String.IsNullOrEmpty(dadosIntegracaoVoalle.UrlApi))
                throw new ArgumentNullException("urlCLient");

            this._urlClient = dadosIntegracaoVoalle.UrlApi;
            this._httpClient = new RestClient(_urlClient);
            this._apiKey = dadosIntegracaoVoalle.ApiKey;
            this._apiSecret = dadosIntegracaoVoalle.ApiSecret;
            this._password = dadosIntegracaoVoalle.Senha;
            this._username = dadosIntegracaoVoalle.Usuario;
            this._token = dadosIntegracaoVoalle.TokenAcesso;
        }

        /// <summary>
        ///     Acessa o endpoint legado de autenticação da API do Voalle para oter o token de acesso
        /// </summary>
        public VoalleAuthResponse GetAccessToken()
        {
            var client = new RestClient(this._urlClient + ":45700/connect/token")
            {
                Timeout = -1,
            };

            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("grant_type", "password");
            request.AddParameter("scope", "syngw synpaygw offline_access");
            request.AddParameter("client_id", $"{this._apiKey}");
            request.AddParameter("client_secret", $"{this._apiSecret}");
            request.AddParameter("password", $"{this._password}");
            request.AddParameter("syndata", $"{this._token}");
            request.AddParameter("username", $"{this._username}");

            IRestResponse<VoalleAuthResponse> response = client.Execute<VoalleAuthResponse>(request);

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new VoalleRequestProcessingException();
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new VoalleRequestProcessingException();
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new VoalleAuthenticationException();
            }

            VoalleAuthResponse responseBody = JsonService.DeserializeObject<VoalleAuthResponse>(response.Content);

            return responseBody;
        }

        /// <summary>
        ///     Acessa o novo endpoint de autenticação da API do Voalle para oter o token de acesso
        /// </summary>
        public VoalleAuthResponse GetAccessTokenV2()
        {
            var client = new RestClient(this._urlClient + ":45700/connect/token")
            {
                Timeout = -1,
            };

            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("scope", "syngw");
            request.AddParameter("client_id", $"{this._apiKey}");
            request.AddParameter("client_secret", $"{this._apiSecret}");           
            request.AddParameter("syndata", $"{this._token}");            

            IRestResponse<VoalleAuthResponse> response = client.Execute<VoalleAuthResponse>(request);

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new VoalleRequestProcessingException();
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new VoalleRequestProcessingException();
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new VoalleAuthenticationException();
            }

            VoalleAuthResponse responseBody = JsonService.DeserializeObject<VoalleAuthResponse>(response.Content);

            return responseBody;
        }

        /// <summary>
        ///     Atualiza o token de autenticação anterior para que ele possa ser renovado
        /// </summary>
        public VoalleAuthResponse RefreshAcessToken()
        {
            var client = new RestClient(this._urlClient)
            {
                Timeout = -1,
            };

            var request = new RestRequest(":45700/connect/token", Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("grant_type", "refresh_token");
            request.AddParameter("client_id", "synauth");
            request.AddParameter("client_secret", "df956154024a425eb80f1a2fc12fef0c");
            request.AddParameter("refresh_token", $"{this.JwtToken}");
            IRestResponse<VoalleAuthResponse> response = client.Execute<VoalleAuthResponse>(request);

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new VoalleRequestProcessingException();
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new VoalleAuthenticationException();
            }

            VoalleAuthResponse responseBody = JsonService.DeserializeObject<VoalleAuthResponse>(response.Content);

            return responseBody;
        }

        #region LEAD
        public void CreateLead(CriarLeadVoalle criarLeadVoalle)
        {
            var url = $"{this._urlClient}:45715/external/crm/leads/create";
            var token = this.JwtToken;

            var client = new RestClient(url);
            var request = new RestRequest("", Method.POST);

            request.AddHeader("accept", "application/json");
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddHeader("Content-Type", "application/json");

            request.AddJsonBody(criarLeadVoalle);

            var response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new VoalleRequestProcessingException();
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new VoalleAuthenticationException();
            }
        }

        #endregion


        #region CUSTOMER 
        public VoalleCustomer GetCustomerCPFCNPJ(string cpf_cnpj)
        {
            cpf_cnpj = Regex.Replace(cpf_cnpj, @"[^\d]", "");

            var client = new RestClient(this._urlClient)
            {
                Timeout = -1,
            };

            var token = this.JwtToken;

            var request = new RestRequest($"/external/integrations/thirdparty/people/txid/{cpf_cnpj}", Method.GET);
            request.AddHeader("Authorization", $"Bearer {token}");

            IRestResponse response = client.Execute(request);


            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new VoalleAuthenticationException();
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new VoalleRequestProcessingException();
            }

            return JsonService.DeserializeObject<VoalleCustomer>(response.Content);
        }

        public VoalleCustomer GetCustomer(string username)
        {
            var client = new RestClient(this._urlClient)
            {
                Timeout = -1
            };

            var request = new RestRequest("/external/integrations/thirdparty/getclient", Method.GET);

            request.AddHeader("Authorization", "Bearer " + this.JwtToken);
            request.AddHeader("Accept", "application/json");

            IRestResponse response = client.Execute(request);


            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new VoalleAuthenticationException();
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new VoalleRequestProcessingException();
            }

            return JsonService.DeserializeObject<VoalleCustomer>(response.Content);
        }

        public void CreateCustomer(CriarPessoaVoalle criarPessoaVoalle)
        {
            var client = new RestClient(this._urlClient)
            {
                Timeout = -1,
            };

            var token = this.JwtToken;

            var request = new RestRequest("/external/integrations/thirdparty/people", Method.POST);
            request.AddHeader("Authorization", $"Bearer {token}");
            request.AddHeader("Content-Type", "application/json");

            request.AddJsonBody(criarPessoaVoalle);

            var response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new VoalleRequestProcessingException();
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new VoalleAuthenticationException();
            }
        }

        public void UpdateCustomer(string username, VoalleCustomer customer)
        {
            if (username == null)
                throw new ArgumentNullException("username");

            if (customer == null)
                throw new ArgumentNullException("customer");

            var request = new RestRequest("customers/" + username, Method.PUT);

            request.AddHeader("Authorization", "Bearer " + this.JwtToken);
            request.AddHeader("Accept", "application/json");

            request.AddJsonBody(customer);

            IRestResponse response = this._httpClient.Execute(request);

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new VoalleRequestProcessingException();
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new VoalleCustomerNotFoundException(username);
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new VoalleAuthenticationException();
            }
        }
        #endregion

        #region PROVEDOR ISP
        public VoalleConnectionResponse UpdateConnection(UpdateConnectionVoalle updateConnectionVoalle, string usuario, string detalhe, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null)
        {
            if (updateConnectionVoalle == null)
                throw new ArgumentNullException("updateConnectionVoalle");

            var access_token = this.JwtToken;
            if (String.IsNullOrEmpty(access_token))
                access_token = this.GetAccessToken().access_token;

            var client = new RestClient(this._urlClient + ":45715/external/integrations/thirdparty/updateconnection/" + updateConnectionVoalle.id);
            var request = new RestRequest(Method.PUT);

            request.AddHeader("Authorization", "Bearer " + access_token);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");

            request.AddJsonBody(updateConnectionVoalle);

            var json = JsonSerializer.Serialize(updateConnectionVoalle, new JsonSerializerOptions { WriteIndented = true });
            var idLog = enviarLog($"Integração Voalle [updateconnection] -> {json}", usuario, detalhe, null);

            IRestResponse response = client.Execute(request);

            enviarLog(response.Content, usuario, detalhe, idLog.Result);

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new VoalleRequestProcessingException();
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new VoalleClientConnectionNotFoundException(updateConnectionVoalle.id);
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new VoalleRefusedException();
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new VoalleAuthenticationException();
            }

            VoalleConnectionResponse responseBody = JsonService.DeserializeObject<VoalleConnectionResponse>(response.Content);
            

            return responseBody;
        }
        #endregion

        public VoalleAuthResponse AuthenticateVoalle(IntegracoesExternas dadosIntegracaoVoalle)
        {
            var voalleAuthResponse = new VoalleAuthResponse();
            voalleAuthResponse = this.GetAccessToken();
            return voalleAuthResponse;
        }

        public VoalleResponseOS GetOsVoalle(string VoalleConnectionString, string numOs)
        {
            using (var connMS = new NpgsqlConnection(VoalleConnectionString))
            {
                var parametros = new Dictionary<string, object>();
                StringBuilder sqlOrdemServico = new StringBuilder();

                List<VoalleResponseOS> listaOsVoalle = new List<VoalleResponseOS>();

                // TODO: ADICIONAR O SELECT DA VOALLE
                sqlOrdemServico.Append(GetPostgresQueryOrdemServico(numOs));
                using (DbDataReader _reader = CommandHelper.ExecuteReaderPostgres(sqlOrdemServico, parametros, connMS))
                {
                    // TODO: LER OS CAMPOS DO SELECT DA VOALLE
                    while (_reader.Read())
                    {
                        var cpf_cnpj = _reader["cpf"] != DBNull.Value ? Convert.ToString(_reader["cpf"]) : String.Empty;

                        if (!String.IsNullOrEmpty(cpf_cnpj))
                        {
                            cpf_cnpj = cpf_cnpj.Replace(".", String.Empty).Replace("-", String.Empty).Replace("/", String.Empty);
                        }

                        var dataAgendamento = _reader["data_inicio_agendamento"] != DBNull.Value ? Convert.ToDateTime(_reader["data_inicio_agendamento"]) : (DateTime?)null;                       

                        var voalleResponse = new VoalleResponseOS();
                        voalleResponse.Protocolo = _reader["protocolo"] != DBNull.Value ? Convert.ToString(_reader["protocolo"]) : null;
                        voalleResponse.Cliente = _reader["id_cliente"] != DBNull.Value ? Convert.ToInt64(_reader["id_cliente"]) : (long?)null;
                        voalleResponse.NomeCliente = _reader["nome_cliente"] != DBNull.Value ? Convert.ToString(_reader["nome_cliente"]) : String.Empty;
                        voalleResponse.Contrato = _reader["id_contrato"] != DBNull.Value ? Convert.ToInt64(_reader["id_contrato"]) : (long?)null;
                        voalleResponse.Data_Prox = dataAgendamento;
                        voalleResponse.Cnpj_Cnpf = cpf_cnpj;
                        voalleResponse.Endereco = _reader["endereco"] != DBNull.Value ? Convert.ToString(_reader["endereco"]) : String.Empty;
                        voalleResponse.Cep = _reader["cep"] != DBNull.Value ? Convert.ToString(_reader["cep"]) : String.Empty;
                        voalleResponse.Complemento = _reader["complemento"] != DBNull.Value ? Convert.ToString(_reader["complemento"]) : String.Empty;
                        voalleResponse.Cidade = _reader["cidade"] != DBNull.Value ? Convert.ToString(_reader["cidade"]) : String.Empty;
                        voalleResponse.Bairro = _reader["bairro"] != DBNull.Value ? Convert.ToString(_reader["bairro"]) : String.Empty;
                        voalleResponse.UF = _reader["uf"] != DBNull.Value ? Convert.ToString(_reader["uf"]) : String.Empty;
                        voalleResponse.Data_HoraAbertura = _reader["data_criacao"] != DBNull.Value ? Convert.ToDateTime(_reader["data_criacao"]) : (DateTime?)null;
                        voalleResponse.Solicitacao = _reader["solicitacao"] != DBNull.Value ? Convert.ToString(_reader["solicitacao"]) : String.Empty;
                        voalleResponse.Problema = _reader["problema"] != DBNull.Value ? Convert.ToString(_reader["problema"]) : String.Empty;
                        voalleResponse.Numero = _reader["numero"] != DBNull.Value ? Convert.ToString(_reader["numero"]) : String.Empty;
                        voalleResponse.Telefone_Celular = _reader["telefone_celular"] != DBNull.Value ? Convert.ToString(_reader["telefone_celular"]) : String.Empty;
                        voalleResponse.Telefone_Residencial = _reader["telefone"] != DBNull.Value ? Convert.ToString(_reader["telefone"]) : String.Empty;
                        voalleResponse.Descricao = Convert.ToString(_reader["descricao"]);
                        voalleResponse.Latitude = Convert.ToString(_reader["latitude"]);
                        voalleResponse.Longitude = Convert.ToString(_reader["longitude"]);
                        voalleResponse.LoginRadius = Convert.ToString(_reader["pppoe"]);
                        voalleResponse.SenhaRadius = Convert.ToString(_reader["senhapppoe"]);
                        voalleResponse.DescricaoPlanoProduto = Convert.ToString(_reader["planoProduto"]);
                        voalleResponse.IdCliente = Convert.ToString(_reader["CLIENT_ID"]);
                        voalleResponse.assignmentId = Convert.ToString(_reader["assignmentId"]);
                        voalleResponse.incident_status_id = Convert.ToInt16(_reader["incident_status_id"]);
                        voalleResponse.sector_area_id = Convert.ToString(_reader["sector_area_id"]);
                        voalleResponse.CodigoLogin = Convert.ToString(_reader["id_login"]);
                        listaOsVoalle.Add(voalleResponse);
                    }
                }

                return listaOsVoalle.FirstOrDefault();

            }
        }

        private String GetPostgresQueryOrdemServico(string numOs)
        {
            StringBuilder sql = new StringBuilder();

            sql.AppendLine(@"select distinct 
requestor_id as id_cliente,
assignments.id as assignmentId,
assignment_incidents.sector_area_id,
assignment_incidents.protocol as protocolo,
assignment_incidents.incident_status_id,
assignment_incidents.incident_type_id,
ASSIGNMENT_INCIDENTS.CLIENT_ID,
UPPER(incident_status.title) as status,
UPPER(incident_types.title) as solicitacao,
UPPER(solicitation_classifications.title) as contexto,
UPPER(solicitation_problems.title) as problema,
TO_CHAR(assignments.beginning_date::DATE,'YYYY-MM-DD') as data_criacao,
authentication_contracts.people_address_id as id_endereco,
coalesce(contract_service_tags.contract_id,'000000') as id_contrato,
cliente.phone as telefone,
cliente.cell_phone_1 as telefone_celular,
cliente.cell_phone_2 as telefone_celular2,
cliente.tx_id as cpf,
cliente.name as nome_cliente,
coalesce(people_addresses.street,cliente.street) AS endereco,
sp.title as planoProduto,
coalesce(people_addresses.number,cliente.number) AS numero,
coalesce(people_addresses.address_complement,cliente.address_complement) AS complemento,
coalesce(people_addresses.address_reference,cliente.address_reference) AS referencia,
coalesce(people_addresses.neighborhood,cliente.neighborhood) AS bairro,
coalesce(people_addresses.city,cliente.city) AS cidade,
coalesce(people_addresses.state,cliente.state) AS uf,
coalesce(people_addresses.latitude,cliente.lat) AS latitude,
coalesce(people_addresses.longitude,cliente.lng) AS longitude,
assignments.description as descricao,
authentication_contracts.user as pppoe,
authentication_contracts.password as senhapppoe,
authentication_contracts.id as id_login,
assignments.beginning_date,
cliente.postal_code AS cep,
(
	select s.start_date  
	from erp.schedules s
	where s.assignment_id = assignments.id order by s.id desc limit 1
    ) as data_inicio_agendamento
from assignments 
inner join assignment_incidents on (assignment_incidents.assignment_id = assignments.id )
inner join incident_types on (incident_types.id = assignment_incidents.incident_type_id)
inner join incident_status on (assignment_incidents.incident_status_id = incident_status.id)
left join people cliente ON (cliente.id = assignment_incidents.client_id)
left join solicitation_classifications on (solicitation_classifications.id = assignment_incidents.solicitation_classification_id)
left join solicitation_problems on (assignment_incidents.solicitation_problem_id = solicitation_problems.id)
left join contract_service_tags on (assignment_incidents.contract_service_tag_id = contract_service_tags.id)
left join authentication_contracts on (authentication_contracts.service_tag_id = contract_service_tags.id)
inner join people on (assignments.requestor_id = people.id)
left Join service_products sp on (sp.id = coalesce(contract_service_tags.service_product_id,authentication_contracts.service_product_id))
left Join people_addresses on (people_addresses.id = authentication_contracts.people_address_id)
where incident_types.active = '1' and assignments.deleted = '0' and incident_types.deleted = '0'
and DATE_FORMAT( assignments.final_date, '%Y-%m-%d' ) <> '0000-00-00' and people.deleted = '0'");


            sql.AppendLine($"AND assignment_incidents.protocol = '{numOs}'");
            return sql.ToString();
        }

        public List<(string idCliente, double? finalLatitude, double? finalLongitude)> GetFinalGeoPostgresQuery(DbConnection connMS)
        {


            List<(string idCliente, double? finalLatitude, double? finalLongitude)> valores = new List<(string idCliente, double? finalLatitude, double? finalLongitude)>();
            StringBuilder sql = new StringBuilder();

            sql.AppendLine(@"WITH ULTIMAMOVIMENTACAO AS (SELECT 
                            MAX(REPORTS.ID) AS ID_REPORT,
                            ASSIGNMENT_INCIDENTS.CLIENT_ID AS CLIENT_ID
                            FROM REPORTS 
                            INNER JOIN ASSIGNMENT_INCIDENTS ON (REPORTS.ASSIGNMENT_ID = ASSIGNMENT_INCIDENTS.ASSIGNMENT_ID)
                            WHERE REPORTS.FINAL_GEOPOSITION_LATITUDE IS NOT NULL
                            GROUP BY ASSIGNMENT_INCIDENTS.CLIENT_ID)


                            SELECT 
                            ULTIMAMOVIMENTACAO.CLIENT_ID AS ID_CLIENTE,
                            REPORTS.FINAL_GEOPOSITION_LATITUDE AS LATITUDE,
                            REPORTS.FINAL_GEOPOSITION_LONGITUDE AS LONGITUDE
                            FROM REPORTS
                            INNER JOIN ULTIMAMOVIMENTACAO ON (ULTIMAMOVIMENTACAO.ID_REPORT = REPORTS.ID)
                            ORDER BY ULTIMAMOVIMENTACAO.CLIENT_ID");
            /*Correção do Davisson que vou descomentar assim que posível*/
            //using (DbDataReader _reader = CommandHelper.ExecuteReaderPostgresAsync(sql, new Dictionary<string, object>(), connMS).Result)
            //{
            //    while (_reader.Read())
            //    {
            //        var idCliente = _reader["ID_CLIENTE"] != DBNull.Value ? Convert.ToString(_reader["ID_CLIENTE"]) : null;
            //        var finalLatitude = _reader["LATITUDE"] != DBNull.Value ? Convert.ToDouble(_reader["LATITUDE"]) : (double?)null;
            //        var finalLongitude = _reader["LONGITUDE"] != DBNull.Value ? Convert.ToDouble(_reader["LONGITUDE"]) : (double?)null;
            //        valores.Add((idCliente, finalLatitude, finalLongitude));
            //    }
            //}


            return valores;
        }

        /// <summary>
        /// Retorna dados do Login consultados na base do Voalle
        /// </summary>
        /// <param name="VoalleConnectionString"></param>
        /// <param name="idLoginVoalle"></param>
        /// <returns></returns>
        public UpdateConnectionVoalle GetLoginVoalle(string VoalleConnectionString, string idLoginVoalle)
        {
            using (var connMS = new NpgsqlConnection(VoalleConnectionString))
            {
                var parametros = new Dictionary<string, object>();
                StringBuilder sqlLogin = new StringBuilder();

                UpdateConnectionVoalle loginVoalle = null;

                sqlLogin.Append(GetPostgresQueryLogin(idLoginVoalle));
                using (DbDataReader _reader = CommandHelper.ExecuteReaderPostgres(sqlLogin, parametros, connMS))
                {
                    if (_reader.Read())
                    {
                        loginVoalle = new UpdateConnectionVoalle();
                        loginVoalle.id = Convert.ToInt64(_reader["id"]);
                        loginVoalle.fiberMac = Convert.ToString(_reader["fiber_mac"]);
                        loginVoalle.mac = Convert.ToString(_reader["mac"]);
                        loginVoalle.password = Convert.ToString(_reader["password"]);
                        loginVoalle.equipmentType = _reader["equipment_type"] != DBNull.Value ? (EquipmentTypeVoalle?)Enum.Parse(typeof(EquipmentTypeVoalle), Convert.ToString(_reader["equipment_type"])) : (EquipmentTypeVoalle?)null;
                        loginVoalle.oltId = _reader["olt_id"] != DBNull.Value ? Convert.ToInt64(_reader["olt_id"]) : 0;
                        loginVoalle.slotOlt = _reader["slot_olt"] != DBNull.Value ? Convert.ToInt64(_reader["slot_olt"]) : 0;
                        loginVoalle.portOlt = _reader["port_olt"] != DBNull.Value ? Convert.ToInt64(_reader["port_olt"]) : 0;
                        loginVoalle.equipmentSerialNumber = Convert.ToString(_reader["equipment_serial_number"]);
                        loginVoalle.ipType = _reader["ip_type"] != DBNull.Value ? Convert.ToInt64(_reader["ip_type"]) : 0;
                        loginVoalle.equipmentUser = Convert.ToString(_reader["equipment_user"]);
                        loginVoalle.equipmentPassword = Convert.ToString(_reader["equipment_password"]);
                        loginVoalle.shouldMacUpdate = false;                                               
                        loginVoalle.wifiName = Convert.ToString(_reader["wifi_name"]);
                        loginVoalle.wifiPassword = Convert.ToString(_reader["wifi_password"]);
                        loginVoalle.technologyType = _reader["technology_type"] != DBNull.Value ? Convert.ToInt64(_reader["technology_type"]) : 8; //8 (Fibra - FTTH)
                        loginVoalle.authenticationAccessPointId = _reader["authentication_access_point_id"] != DBNull.Value ? Convert.ToInt64(_reader["authentication_access_point_id"]) : 0;
                        loginVoalle.user = Convert.ToString(_reader["user"]);
                        loginVoalle.complement = Convert.ToString(_reader["complement"]);                        
                        loginVoalle.isIPoE = false;
                        loginVoalle.authenticationSplitterId = _reader["authentication_splitter_id"] != DBNull.Value ? Convert.ToInt64(_reader["authentication_splitter_id"]) : (long?)null;
                        loginVoalle.port = _reader["authentication_splitter_port"] != DBNull.Value ? Convert.ToInt64(_reader["authentication_splitter_port"]) : (long?)null;
                    }
                }

                return loginVoalle;
            }
        }

        /// <summary>
        /// Retorna dados de Caixa/Porta consultados na base do Voalle
        /// </summary>
        /// <param name="VoalleConnectionString"></param>
        /// <param name="idCaixa"></param>
        /// <param name="numPorta"></param>
        /// <returns></returns>
        public CaixaPortaVoalle GetCaixaPortaVoalle(string VoalleConnectionString, long? idCaixa, long? numPorta, long? idCaixaGeogrid)
        {
            using (var connMS = new NpgsqlConnection(VoalleConnectionString))
            {
                var parametros = new Dictionary<string, object>();
                StringBuilder sqlLogin = new StringBuilder();

                CaixaPortaVoalle caixaPortaVoalle = null;

                sqlLogin.Append(GetPostgresQueryCaixaPorta(idCaixa, numPorta, idCaixaGeogrid));
                using (DbDataReader _reader = CommandHelper.ExecuteReaderPostgres(sqlLogin, parametros, connMS))
                {
                    if (_reader.Read())
                    {
                        caixaPortaVoalle = new CaixaPortaVoalle();
                        caixaPortaVoalle.id = Convert.ToInt64(_reader["id"]);
                        caixaPortaVoalle.title = Convert.ToString(_reader["title"]);
                        caixaPortaVoalle.geogrid_id = Convert.ToString(_reader["geogrid_id"]);
                        caixaPortaVoalle.port_id = _reader["port_id"] != DBNull.Value ? Convert.ToInt64(_reader["port_id"]) : (long?)null;
                        caixaPortaVoalle.port = _reader["port"] != DBNull.Value ? Convert.ToInt64(_reader["port"]) : (long?)null;
                        caixaPortaVoalle.authentication_contract_id = _reader["authentication_contract_id"] != DBNull.Value ? Convert.ToInt64(_reader["authentication_contract_id"]) : (long?)null;
                    }
                }

                return caixaPortaVoalle;
            }
        }

        /// <summary>
        /// Realiza a ocupação da Caixa/Porta, vinculando ao Login no Voalle.
        /// </summary>
        /// <param name="requestBody"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public VoalleConnectionResponse ReservarCaixaPorta(UpdateConnectionVoalle requestBody, ILog log = null)
        {
            VoalleConnectionResponse response = null;

            try
            {
                response = this.AtualizarVinculoCaixaPorta(requestBody);
                (log ?? LogManager.GetLogger("Aniel.Connect.API_Aniel.Voalle")).Error($"[Voalle - ReservarCaixaPorta] - Json Request: {JsonService.SerializeObject(requestBody)} \n  Json Response: {JsonService.SerializeObject(response)} {Environment.NewLine} {Environment.NewLine}");
            }
            catch (Exception ex) 
            {
                (log ?? LogManager.GetLogger("Aniel.Connect.API_Aniel.Voalle")).Error($"[Voalle - ReservarCaixaPorta] - Json Request: {JsonService.SerializeObject(requestBody)} \n  Json Response: {ex.Message} {Environment.NewLine} {Environment.NewLine}");
                response = new VoalleConnectionResponse();
                response.success = false;
                response.response = false;
                response.messages = new List<VoalleConnectionMessages> { new VoalleConnectionMessages() { message = ex.Message} };
            }

            return response;
        }

        /// <summary>
        /// Limpa a Caixa/Porta, desfazendo o vínculo com o Login no Voalle.
        /// </summary>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        public VoalleConnectionResponse LimparCaixaPorta(UpdateConnectionVoalle requestBody, ILog log = null)
        {
            VoalleConnectionResponse response = null;

            try
            {
                response = this.AtualizarVinculoCaixaPorta(requestBody);
                (log ?? LogManager.GetLogger("Aniel.Connect.API_Aniel.Voalle")).Error($"[Voalle - LimparCaixaPorta] - Json Request: {JsonService.SerializeObject(requestBody)} \n  Json Response: {JsonService.SerializeObject(response)} {Environment.NewLine} {Environment.NewLine}");
            }
            catch (Exception ex)
            {
                (log ?? LogManager.GetLogger("Aniel.Connect.API_Aniel.Voalle")).Error($"[Voalle - LimparCaixaPorta] - Json Request: {JsonService.SerializeObject(requestBody)} \n  Json Response: {ex.Message} {Environment.NewLine} {Environment.NewLine}");
                response = new VoalleConnectionResponse();
                response.success = false;
                response.response = false;
                response.messages = new List<VoalleConnectionMessages> { new VoalleConnectionMessages() { message = ex.Message } };
            }

            return response;
        }

        /// <summary>
        /// Realiza ou limpa o vínculo do Login com a Caixa/Porta no Voalle.
        /// </summary>
        /// <param name="requestBody"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="VoalleRequestProcessingException"></exception>
        /// <exception cref="VoalleClientConnectionNotFoundException"></exception>
        /// <exception cref="VoalleRefusedException"></exception>
        /// <exception cref="VoalleAuthenticationException"></exception>
        private VoalleConnectionResponse AtualizarVinculoCaixaPorta(UpdateConnectionVoalle requestBody)
        {
            if (requestBody == null)
                throw new ArgumentNullException("updateConnectionVoalle nulo");

            string access_token = null;

            try
            {
                access_token = this.JwtToken;
            }
            catch(Exception ex)
            {
                //Se der erro na autenticação legada, tenta com o novo padrão de autenticação.
                if (String.IsNullOrEmpty(access_token))
                    access_token = this.GetAccessTokenV2().access_token;
            }                      

            var client = new RestClient(this._urlClient + ":45715/external/integrations/thirdparty/updateconnection/" + requestBody.id);
            var request = new RestRequest(Method.PUT);

            request.AddHeader("Authorization", "Bearer " + access_token);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json");

            request.AddJsonBody(requestBody);

            IRestResponse response = client.Execute(request);            

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new VoalleRequestProcessingException();
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new VoalleClientConnectionNotFoundException(requestBody.id);
            }

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                throw new VoalleRefusedException();
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new VoalleAuthenticationException();
            }

            VoalleConnectionResponse responseBody = JsonService.DeserializeObject<VoalleConnectionResponse>(response.Content);

            if (responseBody.success == false || responseBody.response == false)
                throw new Exception(responseBody.messages?.FirstOrDefault()?.message ?? responseBody.dataResponseType ?? "Ocorreu um erro ao atualizar a Caixa/Porta");

            return responseBody;
        }

        private String GetPostgresQueryLogin(string idLoginVoalle)
        {
            StringBuilder sql = new StringBuilder();
          
            sql.AppendLine("select ac.id, ac.fiber_mac, ac.mac , ac.mac_password, ");
            sql.AppendLine("ac.equipment_type, ac.olt_id, ac.slot_olt, ac.port_olt, ");
            sql.AppendLine("ac.equipment_serial_number, ac.ip_type, ac.equipment_user, ac.equipment_password, ");
            sql.AppendLine("ac.wifi_name, ac.wifi_password, ac.technology_type, ");
            sql.AppendLine("ac.authentication_access_point_id, ac.user, ac.complement, ac.password, ");
            sql.AppendLine("asp.authentication_splitter_id, asp.port as authentication_splitter_port ");
            sql.AppendLine("from authentication_contracts ac  ");
            sql.AppendLine("left join authentication_splitter_ports asp ");
            sql.AppendLine("on(ac.id = asp.authentication_contract_id)  ");
            sql.AppendLine($"where ac.id = '{idLoginVoalle}' ");
            sql.AppendLine("limit 1 ");
            
            return sql.ToString();
        }

        /// <summary>
        /// Busca o registro de Caixa e Porta no Voalle, baseado no Id da Caixa no Voalle ou Id de Caixa integrada com o Geogrid.
        /// </summary>
        /// <param name="idCaixaVoalle">Id da Caixa no Voalle</param>
        /// <param name="numPortaVoalle">Número da porta no Voalle</param>
        /// <param name="idCaixaGeogrid">Id da Caixa do Geogrid integrada no Voalle</param>
        /// <returns></returns>
        private String GetPostgresQueryCaixaPorta(long? idCaixaVoalle, long? numPortaVoalle, long? idCaixaGeogrid)
        {
            StringBuilder sql = new StringBuilder();

            if (!idCaixaVoalle.HasValue && !idCaixaGeogrid.HasValue)
                throw new Exception("Não foi informado Id da Caixa no Voalle ou Id da Caixa no Geogrid");

            if (!numPortaVoalle.HasValue)
                throw new Exception("Não foi informado numero da Porta no Voalle");

            string condicaoCaixa = idCaixaVoalle.HasValue ? $"cto.id = {idCaixaVoalle}" : $"cto.integrated_equipment_id = '{idCaixaGeogrid}'";

            sql.AppendLine("select cto.id, cto.title, cto.geogrid_id, ");
            sql.AppendLine("asp.id as port_id, asp.authentication_contract_id, asp.port ");            
            sql.AppendLine("from authentication_splitters cto ");            
            sql.AppendLine("inner join authentication_splitter_ports asp ");            
            sql.AppendLine("on(cto.id = asp.authentication_splitter_id) ");                                 
            sql.AppendLine($"where {condicaoCaixa} and asp.port = {numPortaVoalle} ");
            sql.AppendLine("limit 1 ");

            return sql.ToString();
        }
    }
}
