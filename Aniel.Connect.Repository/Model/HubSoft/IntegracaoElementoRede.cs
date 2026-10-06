using Aniel.Connect.Repository.Repository.HubSoft;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;

namespace Aniel.Connect.Repository.Model.Hubsoft
{
    public interface IIntegracaoElementoRede
    {
        List<ImportacaoElementoHubSoft> GetCaixas(string urlLogin, LoginHubsoftModel login, string elemento = "");
    }

    public class IntegracaoElementoRedeHubSoft : IIntegracaoElementoRede
    {
        private readonly string _url = CONSTANTES.ConexaoHubSoftApi;
        public IntegracaoElementoRedeHubSoft(string url)
        {
            this._url = url;
        }
        public List<ImportacaoElementoHubSoft> GetCaixas(string urlLogin, LoginHubsoftModel login, string elemento = "")
        {
            var listagemCaixas = new List<ImportacaoElementoHubSoft>();
            var token = AutenticacaoHubSoft.Login(urlLogin, login);
            var client = new RestClient(this._url);

            var request = new RestRequest($"api/v1/integracao/mapeamento/caixa_optica?busca=nome_caixa&termo_busca=" + elemento, Method.GET);

            request.AddHeader("Authorization", $"Bearer {token}");
            request.Timeout = -1; // Sem timeout'

            var response = client.Execute<ListarCaixasHubSoft>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                listagemCaixas = response.Data.caixas;
            }

            return listagemCaixas;
        }
    }

    public class ListarCaixasHubSoft
    {
        public string status { get; set; }
        public string msg { get; set; }
        public List<ImportacaoElementoHubSoft> caixas { get; set; }
    }
    public class ImportacaoElementoHubSoft
    {
        public long id_caixa_optica { get; set; }
        public int? id_tipo_caixa_optica { get; set; }
        public int? id_ponto_juncao { get; set; }
        public string nome { get; set; }
        public List<object> area_cobertura { get; set; }
        public object cor_mapa { get; set; }
        public object id_externo { get; set; }
        public bool atendimento { get; set; }
        public object parametros { get; set; }
        public object observacao { get; set; }
        public bool emenda { get; set; }
        public bool permite_viabilidade { get; set; }
        public int? portas_reservadas { get; set; }
        public int? portas_vinculadas { get; set; }
        public int? portas_disponiveis { get; set; }
        public string display { get; set; }
        public List<Porta> portas { get; set; }
        public PontoJuncao ponto_juncao { get; set; }
    }

    public class ResponseConsultaLogin
    {
        public string status { get; set; }
        public string msg { get; set; }
        public Paginacao paginacao { get; set; }
        public List<ImportacaoClienteHubSoft> clientes { get; set; }
    }
    public class ImportacaoClienteHubSoft
    {
        public int id_cliente { get; set; }
        public int codigo_cliente { get; set; }
        public string nome_razaosocial { get; set; }
        public object nome_fantasia { get; set; }
        public string tipo_pessoa { get; set; }
        public string cpf_cnpj { get; set; }
        public string telefone_primario { get; set; }
        public string telefone_secundario { get; set; }
        public object telefone_terciario { get; set; }
        public string email_principal { get; set; }
        public object email_secundario { get; set; }
        public string rg { get; set; }
        public object rg_emissao { get; set; }
        public object inscricao_municipal { get; set; }
        public string inscricao_estadual { get; set; }
        public string data_cadastro { get; set; }
        public object data_nascmento { get; set; }
        public object nome_pai { get; set; }
        public object nome_mae { get; set; }
        public object estado_civil { get; set; }
        public object genero { get; set; }
        public string nacionalidade { get; set; }
        public object profissao { get; set; }
        public List<Servico> servicos { get; set; }
    }

  

    public class PontoJuncao
    {
        public int? id_ponto_juncao { get; set; }
        public string nome { get; set; }
        public Coordenadas coordenadas { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        public string display { get; set; }
    }

    public class Porta
    {
        public int? id_porta_atendimento { get; set; }
        public int? sequencia { get; set; }
        public bool reservado { get; set; }
        public int? id_caixa_optica { get; set; }
        public object referencia { get; set; }
        public string display { get; set; }
        public List<ClientesServico> clientes_servicos { get; set; }
        public object saida_elemento_rede { get; set; }
        public object saida_splitter { get; set; }
        public object equipamento { get; set; }
    }

    public class ClientesServico
    {
        public int? id_cliente { get; set; }
        public int? id_servico { get; set; }
        public int? numero_plano { get; set; }
        public string display { get; set; }
        public Pivot pivot { get; set; }
    }
}