using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.Model.AplicacaoMaterialHubsoft;
using Aniel.Connect.Repository.Model.Hubsoft;
using Aniel.Connect.Repository.Model.HubSoft;
using Aniel.Connect.Repository.Model.HubSoft.MovimentoCliente;
using Aniel.Connect.Repository.Model.HubSoft.MovimentoTecnico;
using Aniel.Connect.Repository.Model.RetiradaMaterialHubsoft;
using HubSoftService.Model.HubSoft.Entrada;
using MongoDB.Driver;
using RestSharp;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.Repository.HubSoft
{
    public class EstoqueHubSoftDao
    {
        public static DataConsultaEstoque GetEstoque(string urlBase, string estoque, LoginHubsoftModel login, string detalheHubsoft, string usuarioHubsoft, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null)
        {
            DataConsultaEstoque retorno;

            var client = new RestClient(urlBase);
            var request = new RestRequest($"/api/v1/estoque/local_estoque/paginado/300?page=1", Method.POST);

            var token = AutenticacaoHubSoft.Login(urlBase, login);
            request.AddHeader("Authorization", $"Bearer {token}");
            var idLog = enviarLog($"Integração HubSoft [local_estoque/paginado/] -> Realizando consulta de estoque", usuarioHubsoft, detalheHubsoft, null);

            var response = client.Execute<ConsultaEstoques>(request);
            enviarLog(response.Content, usuarioHubsoft, detalheHubsoft, idLog.Result);

            //retorno = response.Data.locais_estoque.data.Where(x => x.id_local_estoque == Convert.ToInt32(estoque)).FirstOrDefault();

            if (response?.Data?.locais_estoque?.data == null)
                return null;

            if (!int.TryParse(estoque, out var estoqueId))
                return null;

            retorno = response.Data.locais_estoque.data
                .FirstOrDefault(x => x.id_local_estoque == estoqueId);

            return retorno;
        }

        public static RetornoBuscaPatrimonio GetPatrimonio(string codigo_Complementar, LoginHubsoftModel login, string urlBase, string detalheHubsoft, string usuarioHubsoft, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null)
        {
            var client = new RestClient(urlBase);
            var request = new RestRequest($"/api/v1/integracao/estoque/produto_item/consultar", Method.GET);
            request.AddParameter("busca", "numero_serie");
            request.AddParameter("termo_busca", codigo_Complementar);

            var token = AutenticacaoHubSoft.Login(urlBase, login);

            request.AddHeader("Authorization", $"Bearer {token}");

            var idLog = enviarLog($"Integração HubSoft [produto_item/consultar] -> Consultando patrimônio pelo numero_serie {codigo_Complementar}", usuarioHubsoft, detalheHubsoft, null);

            var response = client.Execute<RetornoBuscaPatrimonio>(request);
            enviarLog(response.Content, usuarioHubsoft, detalheHubsoft, idLog.Result);


            return response.Data;
        }

        public static bool MovimentoCliente(MovimentoEstoqueCliente model, LoginHubsoftModel login, string urlBase, string detalheHubsoft, string usuarioHubsoft, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null)
        {
            var client = new RestClient(urlBase);
            var request = new RestRequest($"/api/v1/estoque/movimento_usuario_vinculo", Method.POST);

            request.AddJsonBody(model);

            var token = AutenticacaoHubSoft.Login(urlBase, login);

            request.AddHeader("Authorization", $"Bearer {token}");
            string json = JsonSerializer.Serialize(model);
            var idLog = enviarLog($"Integração HubSoft [movimento_usuario_vinculo] -> {json}", usuarioHubsoft, detalheHubsoft, null);

            var response = client.Execute(request);

            enviarLog(response.Content, usuarioHubsoft, detalheHubsoft, idLog.Result);

            if (!response.Content.Contains("success"))
            {
                return false;
            }
                

            return true;
        }

        public static bool SerialJaExiste(string serial, LoginHubsoftModel login, string urlBase, string detalheHubsoft, string usuarioHubsoft, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null)
        {
            var retorno = GetPatrimonio(serial, login, urlBase, detalheHubsoft, usuarioHubsoft, enviarLog).produto.Any();
            return retorno;
        }

        public static HubSoftService.Model.HubSoft.Entrada.Retorno.RetornoCriacaoEntrada CriarEntradaEstoque(CompraEstoqueModel model, LoginHubsoftModel login, 
            string urlBase, string detalheHubsoft, string usuarioHubsoft, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null)
        {
            var client = new RestClient(urlBase);
            var request = new RestRequest($"/api/v1/estoque/movimento_estoque", Method.POST);

            request.AddJsonBody(model);
            string json = JsonSerializer.Serialize(model);
            var idLog = enviarLog($"Integração HubSoft [movimento_estoque] -> {json}", usuarioHubsoft, detalheHubsoft, null);

            var token = AutenticacaoHubSoft.Login(urlBase, login);

            request.AddHeader("Authorization", $"Bearer {token}");

            var response = client.Execute<HubSoftService.Model.HubSoft.Entrada.Retorno.RetornoCriacaoEntrada>(request);
            enviarLog(response.Content, usuarioHubsoft, detalheHubsoft, idLog.Result);

            return response.Data;
        }

        public static HubSoftService.Model.HubSoft.Entrada.Consulta.RetornoConsultaEntrada ConsultaEntradaEstoque(int id_vinculo_origem, LoginHubsoftModel login,
            string urlBase, string detalheHubsoft, string usuarioHubsoft, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null)
        {
            var client = new RestClient(urlBase);
            var request = new RestRequest($"/api/v1/estoque/produto/itens/paginado/50?page=1", Method.POST);

            request.AddJsonBody(new { id_produto_compra = id_vinculo_origem, ignorar_agrupado = true });

            var token = AutenticacaoHubSoft.Login(urlBase, login);

            request.AddHeader("Authorization", $"Bearer {token}");
            var idLog = enviarLog($"Integração HubSoft [ConsultaEntradaEstoque] -> id_produto_compra {id_vinculo_origem}", usuarioHubsoft, detalheHubsoft, null);

            var response = client.Execute<HubSoftService.Model.HubSoft.Entrada.Consulta.RetornoConsultaEntrada>(request);
            enviarLog(response.Content, usuarioHubsoft, detalheHubsoft, idLog.Result);

            return response.Data;
        }

        public static bool CadastrarSeriais(HubSoftService.Model.HubSoft.Entrada.CadastroSerial.CadastrarSerialModel model, LoginHubsoftModel login,
            string urlBase, string detalheHubsoft, string usuarioHubsoft, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null)
        {
            var client = new RestClient(urlBase);
            var request = new RestRequest($"/api/v1/estoque/produto_item/update_many", Method.POST);

            request.AddJsonBody(model);
            string json = JsonSerializer.Serialize(model);

            var token = AutenticacaoHubSoft.Login(urlBase, login);

            var idLog = enviarLog($"Integração HubSoft [CadastrarSeriais] -> {json}", usuarioHubsoft, detalheHubsoft, null);

            request.AddHeader("Authorization", $"Bearer {token}");

            var response = client.Execute(request);

            enviarLog(response.Content, usuarioHubsoft, detalheHubsoft, idLog.Result);

            if (!response.Content.Contains("success"))
                if (response.Content.Contains("j\\u00e1 possui o n\\u00famero de s\\u00e9rie"))
                    return true;
                else
                    return false;

            return true;
        }


        public static bool MovimentoTecnico(MovimentoTecnicoModel model, LoginHubsoftModel login,
            string urlBase, string detalheHubsoft, string usuarioHubsoft, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null)
        {
            var client = new RestClient(urlBase);
            var request = new RestRequest($"/api/v1/integracao/estoque/movimento_estoque/saida/usuario ", Method.POST);

            request.AddJsonBody(model);
            string json = JsonSerializer.Serialize(model);

            var token = AutenticacaoHubSoft.Login(urlBase, login);

            var idLog = enviarLog($"Integração HubSoft [MovimentoTecnico] -> {json}", usuarioHubsoft, detalheHubsoft, null);
            request.AddHeader("Authorization", $"Bearer {token}");

            var response = client.Execute(request);
            enviarLog(response.Content, usuarioHubsoft, detalheHubsoft, idLog.Result);

            if (!response.Content.Contains("success"))
                return false;

            return true;
        }

        public static string MovimentoSaidaPop(MovimentoProdutoUtilizadoFecharOSPop model, LoginHubsoftModel login,
            string urlBase, string detalheHubsoft, string usuarioHubsoft, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null)
        {
            var client = new RestClient(urlBase);
            var request = new RestRequest($"api/v1/integracao/estoque/movimento_estoque/saida/pop_conexao", Method.POST);

            request.AddJsonBody(model);
            string json = JsonSerializer.Serialize(model);

            var token = AutenticacaoHubSoft.Login(urlBase, login);

            var idLog = enviarLog($"Integração HubSoft [movimento_estoque/saida/pop_conexao] -> {json}", usuarioHubsoft, detalheHubsoft, null);
            request.AddHeader("Authorization", $"Bearer {token}");

            var response = client.Execute(request);
            enviarLog(response.Content, usuarioHubsoft, detalheHubsoft, idLog.Result);

            return response.Content;
        }

        public static string MovimentoSaidaClienteServico(MovimentoProdutoUtilizadoFecharOS model, LoginHubsoftModel login,
            string urlBase, string detalheHubsoft, string usuarioHubsoft, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null)
        {
            var client = new RestClient(urlBase);
            var request = new RestRequest($"api/v1/integracao/estoque/movimento_estoque/saida/cliente_servico", Method.POST);

            request.AddJsonBody(model);
            string json = JsonSerializer.Serialize(model);

            var token = AutenticacaoHubSoft.Login(urlBase, login);

            var idLog = enviarLog($"Integração HubSoft [movimento_estoque/saida/cliente_servico] -> {json}", usuarioHubsoft, detalheHubsoft, null);
            request.AddHeader("Authorization", $"Bearer {token}");

            var response = client.Execute(request);
            enviarLog(response.Content, usuarioHubsoft, detalheHubsoft, idLog.Result);

            return response.Content;
        }

        public static string RetornarComodatoEstoque(RetiradaHubsoftRequest model, LoginHubsoftModel login,
            string urlBase, string detalheHubsoft, string usuarioHubsoft, Func<String, String, String, Int64?, Task<Int64>> enviarLog = null)
        {
            var client = new RestClient(urlBase);
            var request = new RestRequest($"api/v1/integracao/estoque/movimento_estoque/retorno_estoque/cliente_servico", Method.POST);

            request.AddJsonBody(model);

            string json = JsonSerializer.Serialize(model);

            var token = AutenticacaoHubSoft.Login(urlBase, login);

            var idLog = enviarLog($"Integração HubSoft [movimento_estoque/retorno_estoque/cliente_servico] -> {json}", usuarioHubsoft, detalheHubsoft, null);
            request.AddHeader("Authorization", $"Bearer {token}");

            var response = client.Execute(request);
            enviarLog(response.Content, usuarioHubsoft, detalheHubsoft, idLog.Result);

            if (!response.Content.Contains("success"))
                throw new Exception($"Erro ao retirar material " + response.Content);

            return response.Content;
        }


    }
}
