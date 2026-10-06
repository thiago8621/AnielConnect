using Aniel.Connect.Core;
using Aniel.Connect.Integracoes.Geosite.Models;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models.T5;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using RestSharp;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Aniel.Connect.Integracoes.Geosite.Service
{
    //public interface IIntegracaoExternaGeositeApiController
    //{
    //    Task<Object> Adicionar_ConexaoCliente(dynamic obj, dynamic portaObj, long Id_Clipp);
    //    Task<Object> Reservar_Porta(dynamic obj, dynamic portaObj, long Id_Clipp);
    //    Task<Object> Remover_ConexaoCliente(dynamic obj, dynamic portaObj, long Id_Clipp);
    //    Task<Object> Update_Reserva(dynamic obj, dynamic portaObj, long? Id_Clipp);
    //    //Dao
    //    Task<IEnumerable<Object>> GetListConfiguracoes(DbCommand com, IntegracaoExterna tipo);
    //}

    public class IntegracaoExternaGeositeService /*: IIntegracaoExternaGeositeApiController*/
    {
        private ISession _session;
        private ISession _sessionT5;
        public IntegracaoExternaGeositeService(ISession session)
        {
            _session = session;
        }
        public IntegracaoExternaGeositeService(ISession session, ISession sessionT5)
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

        internal  ResponseGeosite Remover_Cliente(IntegracoesExternas obj, long idClienteGeosite)
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
                fidTipo=cliente.fidTipo,
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






}