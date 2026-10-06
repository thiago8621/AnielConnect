using Aniel.Connect.Repository.Model.Hubsoft.ClienteModel;
using Aniel.Connect.Repository.Repository.HubSoft;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.Model.Hubsoft
{
    public interface IIntegracaoLogin
    {
        List<ImportacaoClienteHubSoft> GetLogin(string urlLogin, LoginHubsoftModel login, string loginStr);
        
    }

    public class IntegracaoLoginHubSoft : IIntegracaoLogin
    {
        private readonly string _url = CONSTANTES.ConexaoHubSoftApi;
        public IntegracaoLoginHubSoft(string url)
        {
            this._url = url;
        }
        public List<ImportacaoClienteHubSoft> GetLogin(string urlLogin, LoginHubsoftModel login, string loginStr)
        {
            var listagemCliente = new List<ImportacaoClienteHubSoft>();
            var token = AutenticacaoHubSoft.Login(urlLogin, login);
            var client = new RestClient(this._url);

            var request = new RestRequest($"/api/v1/integracao/cliente?busca=id_cliente_servico&termo_busca=" + loginStr + "&inativo=todos", Method.GET);

            request.AddHeader("Authorization", $"Bearer {token}");
            request.Timeout = -1; // Sem timeout'

            var response = client.Execute<ResponseConsultaLogin>(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                listagemCliente = response.Data.clientes;
            }

            return listagemCliente;
        }

        
    }

    //public class ImportacaoClienteHubSoft
    //{
    //    public int id_cliente { get; set; }
    //    public int codigo_cliente { get; set; }
    //    public string nome_razaosocial { get; set; }
    //    public object nome_fantasia { get; set; }
    //    public string tipo_pessoa { get; set; }
    //    public string cpf_cnpj { get; set; }
    //    public string telefone_primario { get; set; }
    //    public string telefone_secundario { get; set; }
    //    public object telefone_terciario { get; set; }
    //    public string email_principal { get; set; }
    //    public object email_secundario { get; set; }
    //    public string rg { get; set; }
    //    public object rg_emissao { get; set; }
    //    public object inscricao_municipal { get; set; }
    //    public string inscricao_estadual { get; set; }
    //    public string data_cadastro { get; set; }
    //    public object data_nascmento { get; set; }
    //    public object nome_pai { get; set; }
    //    public object nome_mae { get; set; }
    //    public object estado_civil { get; set; }
    //    public object genero { get; set; }
    //    public string nacionalidade { get; set; }
    //    public object profissao { get; set; }
    //    public List<Servico> servicos { get; set; }
    //}

}