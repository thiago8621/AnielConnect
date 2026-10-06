using Aniel.Connect.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Aniel.Connect.API_Aniel.Models;
using NHibernate;
using Sinapse.DAO.repository;
using System.Text;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Aniel.T5.Model.nhibernate;
using System.Data.Common;
using Aniel.T5.Model.enumerado;
using Sinapse.Util;
using Aniel.Connect.Models.T5;
using Newtonsoft.Json;
using Microsoft.AspNet.Identity;
using Aniel.Connect.Model.nhibernate;
using DocumentFormat.OpenXml.Drawing.Charts;
using System.Runtime.CompilerServices;
using Sinapse.Model;
using System.Data.SqlClient;
using NHibernate.Hql.Ast;
using static Aniel.Connect.API_Aniel.Models.GeoGridModels;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    /// <summary>
    /// Classe atuante na integração com o Ixc
    /// </summary>
    ///<seealso href="link">https://wikiapiprovedor.ixcsoft.com.br/#</seealso>
    public class Elemento_RedeApiController : BaseApiController
    {
        /// <summary>
        /// Inserção de uma novo Elemento de Rede no banco
        /// </summary>
        /// <param name="request">Objeto em json</param>

        /// <returns></returns>

        #region exampleRequisicao 
        //{

        //"settings": {
        //"start": 0,
        //"length": 0,
        //"search": "string",
        //"order": {
        //"column": 0,
        //"dir": "string"
        //},
        //"user": "string",
        //"password": "string",
        //"token": "string"
        //},"obj": {
        //"bairro": "string",
        //"cabo": {},
        //"capacidade": 0,
        //"cep": "string",
        //"certificado": true,
        //"cidade": "string",
        //"complemento": "string",
        //"contrato": "string",
        //"cor_linha_Ativa": "string",
        //"cor_linha_Inativa": "string",
        //"descricao": "string",
        //"endereco": "string",
        //"id_POP": 0,
        //"id_OLT": 0,
        //"latitude": 0,
        //"longitude": 0,
        //"numero": "string",
        //"observacao": "string",
        //"poste": {},
        //"projeto": "string",
        //"situacao": 0,
        //"tipo": {
        //"descricao": "string",
        //"icone": {
        //"descricao": "string",
        //"tipo": "string",
        //"url": "string",
        //"arquivo": "string",
        //"caminho": "string",
        //"padrao_Connect": 0,
        //"id": 0,
        //"usuarioCriacao": "string",
        //"dataHoraCriacao": "2022-05-24T15:03:31.962Z",
        //"usuarioAlteracao": "string",
        //"dataHoraAlteracao": "2022-05-24T15:03:31.962Z",
        //"token": "string"
        //},
        //"cor": "string",
        //"situacao": 0,
        //"fabricante": "string",
        //"capacidade": 0,
        //"e_Cabo": true,
        //"e_Caixa": true,
        //"tipo_Cabo": {
        //"descricao": "string",
        //"id": 0,
        //"usuarioCriacao": "string",
        //"dataHoraCriacao": "2022-05-24T15:03:31.963Z",
        //"usuarioAlteracao": "string",
        //"dataHoraAlteracao": "2022-05-24T15:03:31.963Z",
        //"token": "string"
        //},
        //"tipo_de_Cabo": 0,
        //"espessura": "string",
        //"espessura_Linha": "string",
        //"num_Pares_Fibra": 0,
        //"num_Tubos_Loose": 0,
        //"cor_Linha_Ativa": "string",
        //"cor_Linha_Inativa": "string",
        //"estilo_Linha_Ativa": 0,
        //"estilo_Linha_Inativa": 0,
        //"modelo": {
        //"descricao": "string",
        //"status": 0,
        //"id": 0,
        //"usuarioCriacao": "string",
        //"dataHoraCriacao": "2022-05-24T15:03:31.963Z",
        //"usuarioAlteracao": "string",
        //"dataHoraAlteracao": "2022-05-24T15:03:31.963Z",
        //"token": "string"
        //},
        //"padrao": 0,
        //"id": 0,
        //"usuarioCriacao": "string",
        //"dataHoraCriacao": "2022-05-24T15:03:31.963Z",
        //"usuarioAlteracao": "string",
        //"dataHoraAlteracao": "2022-05-24T15:03:31.963Z",
        //"token": "string"
        //},
        //"modelo": {
        //"descricao": "string",
        //"status": 0,
        //"id": 0,
        //"usuarioCriacao": "string",
        //"dataHoraCriacao": "2022-05-24T15:03:31.963Z",
        //"usuarioAlteracao": "string",
        //"dataHoraAlteracao": "2022-05-24T15:03:31.963Z",
        //"token": "string"
        //},
        //"uf": "string",
        //"coordenadas": "string",
        //"num_Coordenadas": 0,
        //"metragem": 0,
        //"id": 0,
        //"usuarioCriacao": "string",
        //"dataHoraCriacao": "2022-05-24T15:03:31.963Z",
        //"usuarioAlteracao": "string",
        //"dataHoraAlteracao": "2022-05-24T15:03:31.963Z",
        //"token": "string"
        //}
        //}
        #endregion
        [HttpPost]

        [Route("api/Elemento_RedeApi/InsertElementoRede")]
        public async Task<PostApiRetorno> InsertElementoRede(ObjApiRequest<Elemento_Rede> request)
        {
            PostApiRetorno retorno = new PostApiRetorno();
            try
            {

                using (UsuarioApiController api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                Elemento_Rede param = request.Obj;

                /*SQLPARAMS*/
                Dictionary<String, object> dict = new Dictionary<String, object>();

                FillDictionaryElemento_Rede(param, dict);
                dict = dict.Where(p => p.Value != null && Convert.ToString(p.Value) != String.Empty).ToDictionary(p => p.Key, p => p.Value);

                using (ISession session = HibernateUtil.GetSessionT5())
                using (var repositoryElemento = new Conexao<Elemento_Rede>(session))
                using (DbCommand com = session.Connection.CreateCommand())
                {

                    List<String> arguments = dict.Keys.Select(p => { return p.Replace("@", ""); }).ToList();
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine($"UPDATE OR INSERT INTO TB_ELEMENTO_REDE (ID, DATAHORACRIACAO, {string.Join(",", arguments)})");
                    sql.AppendLine($"VALUES({param.Id}, CURRENT_TIMESTAMP, {string.Join(",", dict.Keys)})");

                    await CommandHelper.ExecuteNonQuery(com, sql, dict, session.Connection, null);
                    retorno.Ok = true;
                    retorno.Mensagem = "Elemento inserido com êxito";
                }
            }
            catch (Exception Erro)
            {

                retorno.Ok = false;
                retorno.Mensagem = String.IsNullOrEmpty(Erro.InnerException?.Message) ? $"Erro: {Erro.Message}" :
                    $"Erros:\r\n{Erro.Message}\r\n{Erro.InnerException?.Message}";
            }
            return retorno;
        }



        public async Task FillListagem_Enderecos(IEnumerable<Elemento_Rede_Enderecos> vinculo_enderecos)
        {
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repoLogradouro = new Conexao<Endereco_Logradouro>(session))
                using (var repo = new Conexao<Elemento_Rede_Enderecos>(sessionT5))
                {
                    var Ids_Endereco = vinculo_enderecos.Select(p => p.Id_EnderecoLogradouro).ToList();
                    //var logradouros = repoLogradouro.select(p => p.Id);
                    repo.SaveOrUpdate(vinculo_enderecos);
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// Preenchimento do dicionario com os valores do Elemento de Rede
        /// </summary>
        /// <param name="param">objeto com os dados do Elemento de Rede</param>
        /// <param name="dict">Dicionário a ser preenchido</param>
        public void FillDictionaryElemento_Rede(Elemento_Rede param, Dictionary<String, object> dict)
        {
            dict.Add(key: "@COD_CAIXA", value: StringService.Truncate(Convert.ToString(param?.Cod_Caixa), 19));
            dict.Add(key: "@ID_OLT", value: StringService.Truncate(Convert.ToString(param?.Id_OLT), 19));
            dict.Add(key: "@ID_SLOT", value: StringService.Truncate(Convert.ToString(param?.Id_Slot), 19));
            dict.Add(key: "@ID_PON", value: StringService.Truncate(Convert.ToString(param?.Id_Pon), 19));
            dict.Add(key: "@ID_POP", value: StringService.Truncate(Convert.ToString(param?.Id_POP), 19));
            dict.Add(key: "@BAIRRO", value: StringService.Truncate(param?.Bairro, 50));
            dict.Add(key: "@ID_CABO", value: StringService.Truncate(Convert.ToString(param?.Cabo?.Id), 19));
            dict.Add(key: "@CAPACIDADE", value: StringService.Truncate(Convert.ToString(param?.Capacidade), 10));
            dict.Add(key: "@CEP", value: StringService.Truncate(param?.CEP, 10));
            dict.Add(key: "@CERTIFICADO", value: StringService.Truncate(Convert.ToInt32(param?.Certificado).ToString(), 5));
            dict.Add(key: "@CIDADE", value: StringService.Truncate(param?.Cidade, 50));
            dict.Add(key: "@COMPLEMENTO", value: StringService.Truncate(param?.Complemento, 100));
            dict.Add(key: "@CONTRATO", value: StringService.Truncate(param?.Contrato, 90));
            dict.Add(key: "@COORDENADAS", value: StringService.Truncate(param?.Coordenadas, 5000));

            dict.Add(key: "@COR_LINHA_ATIVA", value: StringService.Truncate(param?.Cor_linha_Ativa, 50));
            dict.Add(key: "@COR_LINHA_INATIVA", value: StringService.Truncate(param?.Cor_linha_Inativa, 50));
            dict.Add(key: "@DATAHORAALTERACAO", value: StringService.Truncate(Convert.ToString(param?.DataHoraAlteracao), 19));
            dict.Add(key: "@DATAHORACRIACAO", value: StringService.Truncate(Convert.ToString(param?.DataHoraCriacao), 19));
            dict.Add(key: "@DESCRICAO", value: StringService.Truncate(param?.Descricao, 50));
            dict.Add(key: "@ENDERECO", value: StringService.Truncate(param?.Endereco, 50));
            dict.Add(key: "@LATITUDE", value: param?.Latitude);
            dict.Add(key: "@LONGITUDE", value: param?.Longitude);
            dict.Add(key: "@METRAGEM", value: StringService.Truncate(Convert.ToString(param?.Metragem), 15));
            dict.Add(key: "@NUMERO", value: StringService.Truncate(param?.Numero, 10));
            dict.Add(key: "@NUM_COORDENADAS", value: StringService.Truncate(Convert.ToString(param?.Num_Coordenadas), 19));
            dict.Add(key: "@OBS", value: StringService.Truncate(param?.Observacao, 255));

            dict.Add(key: "@ID_POSTE", value: StringService.Truncate(Convert.ToString(param?.Poste?.Id), 19));
            dict.Add(key: "@PROJETO", value: StringService.Truncate(param?.Projeto, 90));
            dict.Add(key: "@SITUACAO", value: StringService.Truncate(Convert.ToInt32(param?.Situacao).ToString(), 10));
            dict.Add(key: "@ID_TIPO", value: StringService.Truncate(Convert.ToString(param?.Tipo?.Id), 19));
            dict.Add(key: "@TOKEN", value: StringService.Truncate(param?.Token, 50));
            dict.Add(key: "@UF", value: StringService.Truncate(param?.UF, 2));
            dict.Add(key: "@USUARIOALTERACAO", value: StringService.Truncate(param?.UsuarioAlteracao, 30));
            dict.Add(key: "@USUARIOCRIACAO", value: StringService.Truncate(param?.UsuarioCriacao, 30));
            dict.Add(key: "@ID_INTEGRADOR", value: StringService.Truncate(param?.Id_Integrador, 255));


            if (string.IsNullOrEmpty(param.AreaAtendimento) == false)
            {
                var wktReader = new NetTopologySuite.IO.WKTReader();
                var geom = wktReader.Read(param.AreaAtendimento);
                if (geom.Coordinates.Count() > 0)
                {
                    List<List<double>> area = new List<List<double>>();
                    var poligono = new List<double>();
                    foreach (var item in geom.Coordinates)
                    {
                        poligono.Add(item.Y);
                        poligono.Add(item.X);
                    }
                    area.Add(poligono);
                    dict.Add(key: "@AREA_ATENDIMENTO", value: JsonService.SerializeObject(area));
                }
            }

            //dict.Add(key: "@ID_TIPO", value: StringService.Truncate(Convert.ToString(param?.Tipo?.Id), 30));
        }

        /// <summary>
        /// Preenchimento do dicionario com os valores das Portas do Elemento de Rede
        /// </summary>
        /// <param name="param"></param>
        /// <param name="dict"></param>
        public void FillDictionaryElemento_Rede_Porta(Elemento_Rede_Porta param, Dictionary<String, String> dict)
        {

            dict.Add(key: "@ID", value: param.Id == 0 ? String.Empty : StringService.Truncate(param?.Id.ToString(), 19));
            dict.Add(key: "@PORTA", value: StringService.Truncate(param?.Porta.ToString(), 10)); ;
            dict.Add(key: "@STATUS", value: StringService.Truncate($"{Convert.ToInt32(param?.Status)}", 10));
            dict.Add(key: "@ID_CAIXA", value: StringService.Truncate(param?.Caixa?.Id.ToString(), 19));
            dict.Add(key: "@LOGIN", value: StringService.Truncate(param?.Login, 100));
            dict.Add(key: "@CLIENTE_PLANO_PRODUTO", value: StringService.Truncate(param?.Cliente_Plano_Prod.ToString(), 19));

        }

        /// <summary>
        /// Inserção de uma nova porta de Elemento de Rede no banco
        /// </summary>
        /// <param name="request">Objeto em json</param>
        /// <returns></returns>
        #region exampleRequisicao 
        //{
        //  "settings": {
        //    "start": 0,
        //    "length": 0,
        //    "search": "string",
        //    "order": {
        //      "column": 0,
        //      "dir": "string"
        //    },
        //    "user": "string",
        //    "password": "string",
        //    "token": "string"
        //  },  "obj": {
        //    "caixa": {
        //      "bairro": "string",
        //      "cabo": {},
        //      "capacidade": 0,
        //      "cep": "string",
        //      "certificado": true,
        //      "cidade": "string",
        //      "complemento": "string",
        //      "contrato": "string",
        //      "cor_linha_Ativa": "string",
        //      "cor_linha_Inativa": "string",
        //      "descricao": "string",
        //      "endereco": "string",
        //      "id_POP": 0,
        //      "id_OLT": 0,
        //      "latitude": 0,
        //      "longitude": 0,
        //      "numero": "string",
        //      "observacao": "string",
        //      "poste": {},
        //      "projeto": "string",
        //      "situacao": 0,
        //      "tipo": {
        //        "descricao": "string",
        //        "icone": {
        //          "descricao": "string",
        //          "tipo": "string",
        //          "url": "string",
        //          "arquivo": "string",
        //          "caminho": "string",
        //          "padrao_Connect": 0,
        //          "id": 0,
        //          "usuarioCriacao": "string",
        //          "dataHoraCriacao": "2022-05-24T15:17:27.362Z",
        //          "usuarioAlteracao": "string",
        //          "dataHoraAlteracao": "2022-05-24T15:17:27.362Z",
        //          "token": "string"
        //        },
        //        "cor": "string",
        //        "situacao": 0,
        //        "fabricante": "string",
        //        "capacidade": 0,
        //        "e_Cabo": true,
        //        "e_Caixa": true,
        //        "tipo_Cabo": {
        //          "descricao": "string",
        //          "id": 0,
        //          "usuarioCriacao": "string",
        //          "dataHoraCriacao": "2022-05-24T15:17:27.362Z",
        //          "usuarioAlteracao": "string",
        //          "dataHoraAlteracao": "2022-05-24T15:17:27.362Z",
        //          "token": "string"
        //        },
        //        "tipo_de_Cabo": 0,
        //        "espessura": "string",
        //        "espessura_Linha": "string",
        //        "num_Pares_Fibra": 0,
        //        "num_Tubos_Loose": 0,
        //        "cor_Linha_Ativa": "string",
        //        "cor_Linha_Inativa": "string",
        //        "estilo_Linha_Ativa": 0,
        //        "estilo_Linha_Inativa": 0,
        //        "modelo": {
        //          "descricao": "string",
        //          "status": 0,
        //          "id": 0,
        //          "usuarioCriacao": "string",
        //          "dataHoraCriacao": "2022-05-24T15:17:27.362Z",
        //          "usuarioAlteracao": "string",
        //          "dataHoraAlteracao": "2022-05-24T15:17:27.362Z",
        //          "token": "string"
        //        },
        //        "padrao": 0,
        //        "id": 0,
        //        "usuarioCriacao": "string",
        //        "dataHoraCriacao": "2022-05-24T15:17:27.362Z",
        //        "usuarioAlteracao": "string",
        //        "dataHoraAlteracao": "2022-05-24T15:17:27.362Z",
        //        "token": "string"
        //      },
        //      "modelo": {
        //        "descricao": "string",
        //        "status": 0,
        //        "id": 0,
        //        "usuarioCriacao": "string",
        //        "dataHoraCriacao": "2022-05-24T15:17:27.362Z",
        //        "usuarioAlteracao": "string",
        //        "dataHoraAlteracao": "2022-05-24T15:17:27.362Z",
        //        "token": "string"
        //      },
        //      "uf": "string",
        //      "coordenadas": "string",
        //      "num_Coordenadas": 0,
        //      "metragem": 0,
        //      "id": 0,
        //      "usuarioCriacao": "string",
        //      "dataHoraCriacao": "2022-05-24T15:17:27.362Z",
        //      "usuarioAlteracao": "string",
        //      "dataHoraAlteracao": "2022-05-24T15:17:27.362Z",
        //      "token": "string"
        //    },
        //    "cliente_Plano_Prod": 0,
        //    "porta": 0,
        //    "login": "string",
        //    "status": 0,
        //    "id": 0,
        //    "usuarioCriacao": "string",
        //    "dataHoraCriacao": "2022-05-24T15:17:27.362Z",
        //    "usuarioAlteracao": "string",
        //    "dataHoraAlteracao": "2022-05-24T15:17:27.362Z",
        //    "token": "string"
        //  },
        //}
        #endregion

        [HttpPost]
        [Route("api/Elemento_RedeApi/InsertPortasElementoRede")]
        public async Task<PostApiRetorno> InsertPortasElementoRede(ObjApiRequest<ElementoRedePortaInsertRequestModel> request)
        {
            PostApiRetorno retorno = new PostApiRetorno();
            try
            {

                using (UsuarioApiController api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                if (request.Obj == null)
                {
                    throw new Exception("Objeto de requisição não preenchido");
                }

                if (request.Obj.Id_Caixa == 0)
                {
                    throw new Exception("Parametros da requisição não foram preenchidos corretamente");
                }

                using (ISession session = HibernateUtil.GetSessionT5())
                using (Conexao<Elemento_Rede_Porta> repoPorta = new Conexao<Elemento_Rede_Porta>(session))
                using (Conexao<Elemento_Rede> repoElemento = new Conexao<Elemento_Rede>(session))
                {
                    var porta = new Elemento_Rede_Porta();

                    porta.Caixa = await repoElemento.selectFirst(p => p.Id == request.Obj.Id_Caixa);
                    if (porta.Caixa == null)
                        throw new Exception("caixa nao encontrada.");

                    porta.Status = request.Obj.Status;
                    porta.Cliente_Plano_Prod = request.Obj.Cliente_Plano_Prod;
                    porta.Login = request.Obj.Login;
                    porta.Porta = request.Obj.Porta;
                    porta.DataHoraCriacao = DateTime.Now;

                    await repoPorta.SaveOrUpdate(porta);

                    retorno.Ok = true;
                    retorno.Mensagem = "Porta inserida/atualizada com êxito";
                }
            }
            catch (Exception Erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = String.IsNullOrEmpty(Erro.InnerException?.Message) ? $"Erro: {Erro.Message}" :
                    $"Erros:\r\n{Erro.Message}\r\n{Erro.InnerException?.Message}";
            }
            return retorno;
        }

        [HttpPost]
        [Route("api/Elemento_RedeApi/InsertEnderecosElementoRede")]
        public async Task<PostApiRetorno> InsertEnderecosElementoRede(ObjApiRequest<ElementoRedeEnderecoInsertRequestModel> request)
        {
            PostApiRetorno retorno = new PostApiRetorno();
            try
            {

                using (UsuarioApiController api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                if (request.Obj == null)
                {
                    throw new Exception("Objeto de requisição não preenchido");
                }

                if (request.Obj.IdEnderecoLogradouro == 0 && request.Obj.IdElementoRede == 0)
                {
                    throw new Exception("Parametros da requisição não foram preenchidos corretamente");
                }

                using (ISession session = HibernateUtil.GetSessionT5())
                using (Conexao<Elemento_Rede_Enderecos> repoEnd = new Conexao<Elemento_Rede_Enderecos>(session))
                using (Conexao<Elemento_Rede> repoElemento = new Conexao<Elemento_Rede>(session))
                {
                    var enderecoCaixaAniel = new Elemento_Rede_Enderecos();

                    enderecoCaixaAniel.Elemento_Rede = await repoElemento.selectFirst(p => p.Id == request.Obj.IdElementoRede);
                    enderecoCaixaAniel.Id_EnderecoLogradouro = request.Obj.IdEnderecoLogradouro;
                    enderecoCaixaAniel.Numero = request.Obj.Numero;

                    await repoEnd.SaveOrUpdate(enderecoCaixaAniel);

                    retorno.Ok = true;
                }
            }
            catch (Exception Erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = String.IsNullOrEmpty(Erro.InnerException?.Message) ? $"Erro: {Erro.Message}" :
                    $"Erros:\r\n{Erro.Message}\r\n{Erro.InnerException?.Message}";
            }
            return retorno;
        }

        /// <summary>
        /// Captura de elemento de Rede guiada por Id do Elemento
        /// Para melhores resultados, utilize o banco ISP temporariamente para testes
        /// </summary>
        /// <param name="request">Requisição padrão contendo o Id do Elemento de Rede</param>
        /// <returns>Retorna o Elemento de Rede buscado pelo Id</returns>

        #region exampleRequisicao
        //{
        //  "settings": {
        //    "start": 0,
        //    "length": 0,
        //    "search": "string",
        //    "order": {
        //      "column": 0,
        //      "dir": "string"
        //    },
        //    "user": "string",
        //    "password": "string",
        //    "token": "string"
        //  }
        //}
        #endregion

        [HttpPost()]
        [Route("api/Elemento_RedeApi/GetIdByCodCaixa")]
        public async Task<GetApiRetorno<long?>> GetIdByCodCaixa(ObjApiRequest<long> request)
        {
            GetApiRetorno<long?> retorno = new GetApiRetorno<long?>();

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (DbCommand com = session.Connection.CreateCommand())
                {
                    using (UsuarioApiController usuarioapi = new UsuarioApiController())
                    {
                        if (!(await usuarioapi.GetUsuarioValido(request.settings)).Ok)
                        {
                            throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                        }
                    }

                    long codigo = request.Obj;
                    StringBuilder sql = new StringBuilder();

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    sql.AppendLine("SELECT FIRST 1 A.ID");
                    sql.AppendLine("FROM TB_ELEMENTO_REDE A");
                    sql.AppendLine("WHERE A.COD_CAIXA = @COD_CAIXA");
                    parametros.Add("@COD_CAIXA", codigo);

                    using (DbDataReader _reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await _reader.ReadAsync())
                        {
                            retorno.Objeto = _reader["ID"] != DBNull.Value ? Convert.ToInt64(_reader["ID"]) : 0;
                        }
                    }
                    retorno.Ok = true;
                }
            }
            catch (Exception Erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = String.IsNullOrEmpty(Erro.InnerException?.Message) ? $"Erro: {Erro.Message}" :
                   $"Erros: \r\n{Erro.Message}\r\n{Erro.InnerException?.Message}";
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Elemento_RedeApi/GetElementoRede")]
        public async Task<GetApiRetorno<Elemento_Rede>> GetElementoRede(ObjApiRequest<ElementoRedeRequestModel> request)
        {
            GetApiRetorno<Elemento_Rede> retorno = new GetApiRetorno<Elemento_Rede>();

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (DbCommand com = session.Connection.CreateCommand())
                {
                    using (UsuarioApiController usuarioapi = new UsuarioApiController())
                    {
                        if (!(await usuarioapi.GetUsuarioValido(request.settings)).Ok)
                        {
                            throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                        }
                    }


                    StringBuilder sql = new StringBuilder();
                    StringBuilder sqlWhere = new StringBuilder();

                    List<string> condicoes = new List<string>();
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    sql.AppendLine(@"SELECT A.ID_INTEGRADOR,A.ID, A.DESCRICAO,A.SITUACAO, A.ID_TIPO, A.CEP, A.BAIRRO, 
                                     A.ENDERECO, A.CERTIFICADO, A.CIDADE, A.CAPACIDADE, A.COMPLEMENTO, 
                                     A.CONTRATO,A.PROJETO, A.OBS, A.UF, A.LATITUDE, A.LONGITUDE");
                    sql.AppendLine("FROM TB_ELEMENTO_REDE A");

                    sql.Append(" WHERE 1=1 ");
                    if (request.Obj.Situacao > 0)
                    {
                        sql.Append(" AND A.SITUACAO = @SITUACAO");
                        parametros.Add("@SITUACAO", request.Obj.Situacao);
                    }
                    if (request.Obj.Id > 0)
                    {
                        sql.Append(" AND A.ID = @ID");
                        parametros.Add("@ID", request.Obj.Id);
                    }
                    if (!string.IsNullOrEmpty(request.Obj.Id_Tipo))
                    {
                        sql.Append(" AND A.ID_TIPO = @ID_TIPO");
                        parametros.Add("@Bairro", request.Obj.Id_Tipo);
                    }
                    if (!string.IsNullOrEmpty(request.Obj.Bairro))
                    {
                        sql.Append(" AND A.Bairro = @Bairro");
                        parametros.Add("@Bairro", request.Obj.Bairro);
                    }
                    if (!string.IsNullOrEmpty(request.Obj.UF))
                    {
                        sql.Append(" AND A.UF = @UF");
                        parametros.Add("@UF", request.Obj.UF);
                    }

                    if (!string.IsNullOrEmpty(request.Obj.Pais))
                    {
                        sql.Append(" AND A.Pais = @Pais");
                        parametros.Add("@Pais", request.Obj.Pais);
                    }
                    if (!string.IsNullOrEmpty(request.Obj.Coordenadas))
                    {
                        sql.Append(" AND A.Coordenadas = @Coordenadas");
                        parametros.Add("@Coordenadas", request.Obj.Coordenadas);
                    }

                    if (request.Obj.Cod_Caixa.HasValue)
                    {
                        sql.Append(" AND A.Cod_Caixa = @Cod_Caixa");
                        parametros.Add("@Cod_Caixa", request.Obj.Cod_Caixa);
                    }


                    if (request.Obj.Cabo != null)
                    {
                        sql.Append(" AND A.Cabo = @Cabo");
                        parametros.Add("@Cabo", request.Obj.Cabo);
                    }

                    if (request.Obj.Caixa != null)
                    {
                        sql.Append(" AND A.Caixa = @Caixa");
                        parametros.Add("@Caixa", request.Obj.Caixa);
                    }

                    if (request.Obj.Poste != null)
                    {
                        sql.Append(" AND A.Poste = @Poste");
                        parametros.Add("@Poste", request.Obj.Poste);
                    }

                    if (request.Obj.Capacidade > 0)
                    {
                        sql.Append(" AND A.Capacidade = @Capacidade");
                        parametros.Add("@Capacidade", request.Obj.Capacidade);
                    }

                    if (!string.IsNullOrEmpty(request.Obj.CEP))
                    {
                        sql.Append(" AND A.CEP = @CEP");
                        parametros.Add("@CEP", request.Obj.CEP);
                    }

                    if (request.Obj.Certificado)
                    {
                        sql.Append(" AND A.Certificado = @Certificado");
                        parametros.Add("@Certificado", request.Obj.Certificado);
                    }

                    if (!string.IsNullOrEmpty(request.Obj.Cidade))
                    {
                        sql.Append(" AND A.Cidade = @Cidade");
                        parametros.Add("@Cidade", request.Obj.Cidade);
                    }

                    if (!string.IsNullOrEmpty(request.Obj.Complemento))
                    {
                        sql.Append(" AND A.Complemento = @Complemento");
                        parametros.Add("@Complemento", request.Obj.Complemento);
                    }

                    if (!string.IsNullOrEmpty(request.Obj.Contrato))
                    {
                        sql.Append(" AND A.Contrato = @Contrato");
                        parametros.Add("@Contrato", request.Obj.Contrato);
                    }

                    if (!string.IsNullOrEmpty(request.Obj.Cor_linha_Ativa))
                    {
                        sql.Append(" AND A.Cor_linha_Ativa = @Cor_linha_Ativa");
                        parametros.Add("@Cor_linha_Ativa", request.Obj.Cor_linha_Ativa);
                    }

                    if (!string.IsNullOrEmpty(request.Obj.Cor_linha_Inativa))
                    {
                        sql.Append(" AND A.Cor_linha_Inativa = @Cor_linha_Inativa");
                        parametros.Add("@Cor_linha_Inativa", request.Obj.Cor_linha_Inativa);
                    }

                    if (!string.IsNullOrEmpty(request.Obj.Descricao))
                    {
                        sql.Append(" AND A.Descricao = @Descricao");
                        parametros.Add("@Descricao", request.Obj.Descricao);
                    }

                    if (!string.IsNullOrEmpty(request.Obj.Endereco))
                    {
                        sql.Append(" AND A.Endereco = @Endereco");
                        parametros.Add("@Endereco", request.Obj.Endereco);
                    }

                    if (request.Obj.Id_POP.HasValue)
                    {
                        sql.Append(" AND A.Id_POP = @Id_POP");
                        parametros.Add("@Id_POP", request.Obj.Id_POP);
                    }

                    if (request.Obj.Id_OLT.HasValue)
                    {
                        sql.Append(" AND A.Id_OLT = @Id_OLT");
                        parametros.Add("@Id_OLT", request.Obj.Id_OLT);
                    }
                    if (request.Obj.Id_Slot.HasValue)
                    {
                        sql.Append(" AND A.Id_Slot = @Id_Slot");
                        parametros.Add("@Id_Slot", request.Obj.Id_Slot);
                    }
                    if (request.Obj.Id_Pon.HasValue)
                    {
                        sql.Append(" AND A.Id_Pon = @Id_Pon");
                        parametros.Add("@Id_Pon", request.Obj.Id_Pon);
                    }

                    if (request.Obj.Latitude.HasValue)
                    {
                        sql.Append(" AND A.Latitude = @Latitude");
                        parametros.Add("@Latitude", request.Obj.Latitude);
                    }

                    if (request.Obj.Longitude.HasValue)
                    {
                        sql.Append(" AND A.Longitude = @Longitude");
                        parametros.Add("@Longitude", request.Obj.Longitude);
                    }

                    if (!string.IsNullOrEmpty(request.Obj.Numero))
                    {
                        sql.Append(" AND A.Numero = @Numero");
                        parametros.Add("@Numero", request.Obj.Numero);
                    }

                    if (!string.IsNullOrEmpty(request.Obj.Observacao))
                    {
                        sql.Append(" AND A.Obs = @Obs");
                        parametros.Add("@Obs", request.Obj.Observacao);
                    }

                    if (!string.IsNullOrEmpty(request.Obj.Projeto))
                    {
                        sql.Append(" AND A.Projeto = @Projeto");
                        parametros.Add("@Projeto", request.Obj.Projeto);
                    }


                    using (DbDataReader _reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 1, null))
                    {
                        if (await _reader.ReadAsync())
                        {
                            Elemento_Rede obj = new Elemento_Rede()
                            {
                                Id = Convert.ToInt64(_reader["ID"]),
                                Descricao = Convert.ToString(_reader["DESCRICAO"]),
                                CEP = Convert.IsDBNull(_reader["CEP"]) ? String.Empty : Convert.ToString(_reader["CEP"]),
                                Bairro = Convert.IsDBNull(_reader["BAIRRO"]) ? String.Empty : Convert.ToString(_reader["BAIRRO"]),
                                Endereco = Convert.IsDBNull(_reader["ENDERECO"]) ? String.Empty : Convert.ToString(_reader["ENDERECO"]),
                                Latitude = Convert.IsDBNull(_reader["LATITUDE"]) ? 0 : Convert.ToDouble(_reader["LATITUDE"]),
                                Longitude = Convert.IsDBNull(_reader["LONGITUDE"]) ? 0 : Convert.ToDouble(_reader["LONGITUDE"]),
                                Observacao = Convert.IsDBNull(_reader["OBS"]) ? String.Empty : Convert.ToString(_reader["OBS"]),
                                Contrato = Convert.IsDBNull(_reader["CONTRATO"]) ? String.Empty : Convert.ToString(_reader["CONTRATO"]),
                                Projeto = Convert.IsDBNull(_reader["PROJETO"]) ? String.Empty : Convert.ToString(_reader["PROJETO"]),
                                UF = Convert.IsDBNull(_reader["UF"]) ? String.Empty : Convert.ToString(_reader["UF"]),
                                Cidade = Convert.IsDBNull(_reader["CIDADE"]) ? String.Empty : Convert.ToString(_reader["CIDADE"]),
                                Id_Integrador = Convert.IsDBNull(_reader["ID_INTEGRADOR"]) ? String.Empty : Convert.ToString(_reader["ID_INTEGRADOR"]),
                                Capacidade = Convert.IsDBNull(_reader["CAPACIDADE"]) ? 0 : Convert.ToInt32(_reader["CAPACIDADE"]),
                                Situacao = Convert.IsDBNull(_reader["SITUACAO"]) ? 0 : (Status_Elemento)Enum.Parse(typeof(Status_Elemento), Convert.ToString(_reader["SITUACAO"])),
                            };
                            retorno.Objeto = obj;
                            retorno.Ok = retorno.Objeto != null ? true : false;

                        }
                    }

                }
            }
            catch (Exception Erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = String.IsNullOrEmpty(Erro.InnerException?.Message) ? $"Erro: {Erro.Message}" :
                   $"Erros: \r\n{Erro.Message}\r\n{Erro.InnerException?.Message}";
            }

            return retorno;
        }

        /// <summary>
        /// Endpoint Captura os do Elemento de Rede com base no Id_Integrador do Elemento de Rede
        /// </summary>
        
        [HttpPost()]
        [Route("api/Elemento_RedeApi/GetElementoRedeIdIntegrador")]
        public async Task<GetApiRetorno<Elemento_Rede>> GetElementoRedeIdIntegrador(ObjApiRequest<String> request)
        {
            GetApiRetorno<Elemento_Rede> retorno = new GetApiRetorno<Elemento_Rede>();

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (DbCommand com = session.Connection.CreateCommand())
                {
                    using (UsuarioApiController usuarioapi = new UsuarioApiController())
                    {
                        if (!(await usuarioapi.GetUsuarioValido(request.settings)).Ok)
                        {
                            throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                        }
                    }
                    String Id = request.Obj;
                    StringBuilder sql = new StringBuilder();

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    sql.AppendLine(@"SELECT A.ID, A.DESCRICAO,A.SITUACAO, A.ID_TIPO, A.CEP, A.BAIRRO, 
                                     A.ENDERECO, A.CERTIFICADO, A.CIDADE, A.CAPACIDADE, A.COMPLEMENTO, 
                                     A.CONTRATO, A.OBS, A.UF, A.LATITUDE, A.LONGITUDE");
                    sql.AppendLine("FROM TB_ELEMENTO_REDE A");
                    sql.Append(" WHERE A.ID_INTEGRADOR = @ID");
                    //sql.AppendLine(@"SELECT  A.NOMEFANTASIA, A.ID FROM TB_CLIENTE A  WHERE A.ID = @ID");
                    parametros.Add("@ID", Id);

                    using (DbDataReader _reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 1, null))
                    {
                        if (await _reader.ReadAsync())
                        {
                            Elemento_Rede obj = new Elemento_Rede()
                            {
                                Id = Convert.ToInt64(_reader["ID"]),
                                Descricao = Convert.ToString(_reader["DESCRICAO"]),
                                CEP = Convert.IsDBNull(_reader["CEP"]) ? String.Empty : Convert.ToString(_reader["CEP"]),
                                Bairro = Convert.IsDBNull(_reader["BAIRRO"]) ? String.Empty : Convert.ToString(_reader["BAIRRO"]),
                                Endereco = Convert.IsDBNull(_reader["ENDERECO"]) ? String.Empty : Convert.ToString(_reader["ENDERECO"]),
                                Latitude = Convert.IsDBNull(_reader["LATITUDE"]) ? 0 : Convert.ToDouble(_reader["LATITUDE"]),
                                Longitude = Convert.IsDBNull(_reader["LONGITUDE"]) ? 0 : Convert.ToDouble(_reader["LONGITUDE"]),
                                Observacao = Convert.IsDBNull(_reader["OBS"]) ? String.Empty : Convert.ToString(_reader["OBS"]),
                                UF = Convert.IsDBNull(_reader["UF"]) ? String.Empty : Convert.ToString(_reader["UF"]),
                                Cidade = Convert.IsDBNull(_reader["CIDADE"]) ? String.Empty : Convert.ToString(_reader["CIDADE"]),
                                Capacidade = Convert.IsDBNull(_reader["CAPACIDADE"]) ? 0 : Convert.ToInt32(_reader["CAPACIDADE"]),
                                Situacao = Convert.IsDBNull(_reader["SITUACAO"]) ? 0 : (Status_Elemento)Enum.Parse(typeof(Status_Elemento), Convert.ToString(_reader["SITUACAO"])),
                            };
                            retorno.Objeto = obj;
                            retorno.Ok = retorno.Objeto != null ? true : false;

                        }
                    }

                }
            }
            catch (Exception Erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = String.IsNullOrEmpty(Erro.InnerException?.Message) ? $"Erro: {Erro.Message}" :
                   $"Erros: \r\n{Erro.Message}\r\n{Erro.InnerException?.Message}";
            }

            return retorno;
        }

        /// <summary>
        /// Endpoint Captura as portas do Elemento de Rede com base no Id do Elemento de Rede
        /// </summary>
        /// <param name="request">Requisição nas normas de Autenticação com Id da Caixa de Elemento de Rede</param> 
        /// <returns>Retorna uma Listagem das Portas de Elemento de Rede</returns>
        [HttpPost()]
        [Route("api/Elemento_RedeApi/GetPortas")]
        public async Task<GetApiRetorno<List<Elemento_Rede_Porta>>> GetPortasElementoRede(ObjApiRequest<long> request)
        {

            GetApiRetorno<List<Elemento_Rede_Porta>> retorno = new GetApiRetorno<List<Elemento_Rede_Porta>>
            {
                Objeto = new List<Elemento_Rede_Porta>()
            };
            try
            {

                using (UsuarioApiController usuarioapi = new UsuarioApiController())
                {
                    if (!(await usuarioapi.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                long Id_Caixa = request.Obj;
                StringBuilder sql = new StringBuilder(),
                              sqlCount = new StringBuilder();

                using (ISession session = HibernateUtil.GetSessionT5())
                using (DbCommand com = session.Connection.CreateCommand())
                {
                    //List<Elemento_Rede_Porta> retorno = new List<Elemento_Rede_Porta>();
                    long portas = 0;
                    sqlCount.AppendLine($"SELECT A.CAPACIDADE FROM TB_ELEMENTO_REDE A WHERE A.ID = {Id_Caixa}");
                    using (DbDataReader _reader = await CommandHelper.ExecuteReader(com, sqlCount, new Dictionary<String, Object>(), session.Connection, null))
                    {
                        while (await _reader.ReadAsync())
                        {
                            portas = _reader["CAPACIDADE"] != DBNull.Value ? Convert.ToInt64(_reader["CAPACIDADE"]) : 0;
                        }
                    }
                    sql.AppendLine($"SELECT FIRST {portas} A.ID, A.ID_CAIXA, A.PORTA, A.LOGIN, A.CLIENTE_PLANO_PRODUTO, A.STATUS");

                    sql.AppendLine("FROM TB_ELEMENTO_REDE_PORTA A ");
                    sql.AppendLine("LEFT JOIN TB_ELEMENTO_REDE B ");
                    sql.AppendLine("  ON(A.ID_CAIXA = B.ID) ");

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    sql.Append(" WHERE A.ID_CAIXA = @ID_CAIXA");
                    //AND DATA_MAXIMA = @DATA_MAXIMA
                    parametros.Add("@ID_CAIXA", Id_Caixa);

                    //parametros.Add("@DATA_MAXIMA", DataMaxima.ToString("dd.MM.yyyy"));

                    using (DbDataReader _reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await _reader.ReadAsync())
                        {
                            retorno.Objeto.Add(new Elemento_Rede_Porta()
                            {
                                Id = Convert.ToInt64(_reader["ID"]),
                                Cliente_Plano_Prod = _reader["CLIENTE_PLANO_PRODUTO"] != DBNull.Value ? Convert.ToInt64(_reader["CLIENTE_PLANO_PRODUTO"]) : 0,
                                Login = _reader["LOGIN"] != DBNull.Value ? Convert.ToString(_reader["LOGIN"]) : String.Empty,
                                Porta = Convert.ToInt32(_reader["PORTA"]),
                                Status = (Sinapse.Model.enumerados.Situacao_Porta)Enum.Parse(typeof(Sinapse.Model.enumerados.Situacao_Porta), Convert.ToString(_reader["STATUS"])),
                            });
                        }
                    }
                    retorno.Ok = true;
                }
            }
            catch (Exception erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = String.IsNullOrEmpty(erro.InnerException?.Message) ? $"Erro: {erro.Message}" :
                   $"Erros: \r\n{erro.Message}\r\n{erro.InnerException?.Message}";
            }
            return retorno;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/Elemento_RedeApi/GetList")]
        public async Task<GetApiRetorno<IEnumerable<Elemento_Rede>>> GetList(ApiRequest request)
        {
            GetApiRetorno<IEnumerable<Elemento_Rede>> retorno = new GetApiRetorno<IEnumerable<Elemento_Rede>>();

            using (ISession session = HibernateUtil.GetSessionT5())
            using (Conexao<Elemento_Rede> repository = new Conexao<Elemento_Rede>(session))
            using (Conexao<Tipo_Elemento_Rede> repotipo = new Conexao<Tipo_Elemento_Rede>(session))
            {
                try
                {

                    using (UsuarioApiController api = new UsuarioApiController())
                    {
                        if (!(await api.GetUsuarioValido(request.settings)).Ok)
                        {
                            throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                        }
                    }

                    List<Elemento_Rede> lista = (await repository.select(p => p.Id != 0)).Where(p => p.Tipo.E_Caixa == true).ToList();

                    retorno.Objeto = lista.Select(p => new Elemento_Rede
                    {
                        Id = p.Id,
                        Descricao = p.Descricao,
                        Id_Integrador = p.Id_Integrador

                    });

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Mensagem = String.IsNullOrEmpty(ex.InnerException?.Message) ? $"Erro: {ex.Message}" :
                    $"Erros: \r\n{ex.Message}\r\n{ex.InnerException?.Message}";
                }
            }
            return retorno;

        }
        /// <summary>
        /// Capturar portas com o login passado por requisição
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet()]
        [Route("api/Elemento_RedeApi/GetPortasFromLogin")]
        public async Task<GetApiRetorno<List<Elemento_Rede_Porta>>> GetPortasFromLogin(ObjApiRequest<string> request)
        {
            GetApiRetorno<List<Elemento_Rede_Porta>> retorno = new GetApiRetorno<List<Elemento_Rede_Porta>>
            {
                Objeto = new List<Elemento_Rede_Porta>()
            };
            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (DbCommand com = session.Connection.CreateCommand())

                {

                    String login = JsonConvert.DeserializeObject<String>(request.Obj);
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(@$"SELECT A.ID, A.ID_CAIXA, A.CLIENTE_PLANO_PRODUTO, A.PORTA, A.LOGIN, A.STATUS
                                     FROM TB_ELEMENTO_REDE_PORTA A
                                     WHERE A.LOGIN LIKE '{string.Format("%{0}%", login)}'");
                    using (DbDataReader reader = await CommandHelper.ExecuteReader(com, sql, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Objeto.Add(
                         new Elemento_Rede_Porta
                         {

                             Id = Convert.ToInt64(reader["ID"]),
                             Login = Convert.IsDBNull(reader["LOGIN"]) ? string.Empty : Convert.ToString(reader["LOGIN"]),
                             Status = (Sinapse.Model.enumerados.Situacao_Porta)Enum.Parse(typeof(Sinapse.Model.enumerados.Situacao_Porta), Convert.ToString(reader["STATUS"])),
                             Cliente_Plano_Prod = Convert.IsDBNull(reader["CLIENTE_PLANO_PRODUTO"]) ? default : Convert.ToInt64(reader["CLIENTE_PLANO_PRODUTO"]),
                             Porta = Convert.ToInt32(reader["PORTA"])
                         });

                        }
                    }

                }
            }
            catch (Exception erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = String.IsNullOrEmpty(erro.InnerException?.Message) ? $"Erro: {erro.Message}" :
                    $"Erros: \r\n{erro.Message}\r\n{erro.InnerException?.Message}";
            }
            return retorno;
        }


        /// <summary>
        /// Endpoint busca atualizar a caixa do Aniel com o status da Caixa no IXC 
        /// </summary>
        /// <param name="request">caixa FTTH</param>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/Elemento_RedeApi/AtualizarStatusElemento")]

        public async Task<PostApiRetorno> AtualizarStatusElemento(ObjApiRequest<Caixa_FTTH_IXC> request)
        {
            PostApiRetorno<Elemento_Rede> retorno = new PostApiRetorno<Elemento_Rede>();
            Dictionary<String, object> parametros = new Dictionary<string, Object>();

            try
            {

                using (UsuarioApiController api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }
                Status_Elemento statusAniel = switchStatusElemento(request.Obj.status);

                bool Certificado = false;

                StringBuilder sqlUp = new StringBuilder(" UPDATE TB_ELEMENTO_REDE SET SITUACAO = @STATUS ");
                StringBuilder sql = new StringBuilder();
                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    //string ids = $"({String.Join(",", request.id)})";
                    if (statusAniel == Status_Elemento.Ativo)
                    {
                        Certificado = true;
                    }
                    sqlUp.AppendLine($", CERTIFICADO='{Convert.ToInt32(Certificado)}'");

                    using (DbCommand com = session.Connection.CreateCommand())
                    {
                        sql.AppendLine(sqlUp.ToString());
                        sql.AppendLine($" WHERE ID = {request.Obj.id} ");


                        parametros.Add("@STATUS", statusAniel);

                        await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
                    }

                }
                retorno.Ok = true;
                retorno.Mensagem = "Status Atualizado com exito";
            }
            catch (Exception erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = String.IsNullOrEmpty(erro.InnerException?.Message) ? $"Erro: {erro.Message}" :
                   $"Erros: \r\n{erro.Message}\r\n{erro.InnerException?.Message}";
            }
            return retorno;
        }
        private Sinapse.Model.enumerados.Status_Elemento switchStatusElemento(string request)
        {
            Status_Elemento statusAniel = Status_Elemento.Inativo;
            switch (request)
            {
                case "A":
                    statusAniel = Status_Elemento.Ativo;
                    break;

                case "I": break;

                default:
                    throw new Exception("valor inválido para Status");
                    //break;
            }
            return statusAniel;
        }

        [HttpPost()]
        [Route("api/Elemento_RedeApi/AtualizarLoginPorta")]
        public async Task<PostApiRetorno<string>> AtualizarLoginPorta(ObjApiRequest<LoginRadiusAtualizacao> request)
        {
            PostApiRetorno<String> retorno = new PostApiRetorno<String>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repoPorta = new Conexao<Elemento_Rede_Porta>(sessionT5))
                using (var repoLoginRadius = new Conexao<LoginRadius>(session))
                {
                    using (UsuarioApiController api = new UsuarioApiController())
                    {
                        if (!(await api.GetUsuarioValido(request.settings)).Ok)
                        {
                            throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                        }
                    }

                    if (request.Obj == null)
                    {
                        throw new Exception("Dados da requisição não foram preenchidos");
                    }

                    long? numeroPorta = request.Obj.numeroPorta;
                    long? IdLogin = request.Obj.IdLogin;
                    long? IdCaixa = request.Obj.IdCaixa;

                    var login = await repoLoginRadius.selectFirst(p => p.Id == IdLogin);
                    var porta = await repoPorta.selectFirst(p => p.Caixa.Id == IdCaixa && p.Id == numeroPorta);

                    if (porta != null && login != null)
                    {

                        porta.IdLogin = login.Id;
                        porta.Login = login.Login;
                        porta.Status = Sinapse.Model.enumerados.Situacao_Porta.Ocupado;
                        await repoPorta.Update(porta);

                        login.Id_Caixa_FTTH = porta.Caixa?.Id;
                        login.Id_Porta_FTTH = porta.Id;
                        await repoLoginRadius.Update(login);

                    }
                }
                retorno.Ok = true;
            }
            catch (Exception erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = String.IsNullOrEmpty(erro.InnerException?.Message) ? $"Erro: {erro.Message}" :
                    $"Erros:\r\n{erro.Message}\r\n{erro.InnerException?.Message}";
            }

            return retorno;
        }
        [HttpPost()]
        [Route("api/Elemento_RedeApi/GetTipo_Modelo")]
        public async Task<GetApiRetorno<Tipo_Elemento_Rede>> GetTipo_Modelo(ObjApiRequest<RequestTipo_Modelo> request)
        {
            GetApiRetorno<Tipo_Elemento_Rede> retorno = new GetApiRetorno<Tipo_Elemento_Rede>();
            try
            {

                using (ISession session = HibernateUtil.GetSessionT5())
                using (DbCommand com = session.Connection.CreateCommand())
                {
                    using (UsuarioApiController api = new UsuarioApiController())
                    {
                        if (!(await api.GetUsuarioValido(request.settings)).Ok)
                        {
                            throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                        }
                    }

                    //tipo                
                    retorno.Objeto = await GetTipoElemento(request);
                    if (retorno.Objeto == null)
                    {
                        throw new Exception("Nenhum registro encontrado com os parâmetros passados.");
                    }
                    retorno.Ok = true;
                }
            }
            catch (Exception erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = String.IsNullOrEmpty(erro.InnerException?.Message) ? $"Erro: {erro.Message}" :
                    $"Erros:\r\n{erro.Message}\r\n{erro.InnerException?.Message}";
            }


            return retorno;
        }
        /// <summary>
        /// Endpoint Captura as portas do Elemento de Rede com base no campo ALTERADO_INTEGRA
        /// </summary>
        /// <param name="request">Requisição nas normas de Autenticação com Id da Caixa de Elemento de Rede</param> 
        /// <returns>Retorna uma Listagem das Portas de Elemento de Rede</returns>
        [HttpPost()]
        [Route("api/Elemento_RedeApi/GetPortasAlteradas")]
        public async Task<GetApiRetorno<List<Elemento_Rede_Porta>>> GetPortasAlteradas(ObjApiRequest<long> request)
        {

            GetApiRetorno<List<Elemento_Rede_Porta>> retorno = new GetApiRetorno<List<Elemento_Rede_Porta>>
            {
                Objeto = new List<Elemento_Rede_Porta>()
            };
            try
            {

                using (UsuarioApiController usuarioapi = new UsuarioApiController())
                {
                    if (!(await usuarioapi.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                long Id_Caixa = request.Obj;
                StringBuilder sql = new StringBuilder(),
                              sqlCount = new StringBuilder();

                using (ISession session = HibernateUtil.GetSessionT5())
                using (DbCommand com = session.Connection.CreateCommand())
                {
                    sql.AppendLine($"SELECT A.ID, A.ID_CAIXA, A.PORTA, A.LOGIN, A.CLIENTE_PLANO_PRODUTO, A.STATUS");
                    sql.AppendLine("FROM TB_ELEMENTO_REDE_PORTA A ");
                    sql.AppendLine("LEFT JOIN TB_ELEMENTO_REDE B ");
                    sql.AppendLine("  ON(A.ID_CAIXA = B.ID) ");

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    sql.Append(" WHERE A.ALTERADO_INTEGRA = 1");

                    if (Id_Caixa != 0)
                    {
                        sql.Append(" AND A.ID_CAIXA = @ID_CAIXA");
                        parametros.Add("@ID_CAIXA", Id_Caixa);
                    }

                    //parametros.Add("@DATA_MAXIMA", DataMaxima.ToString("dd.MM.yyyy"));

                    using (DbDataReader _reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await _reader.ReadAsync())
                        {
                            retorno.Objeto.Add(new Elemento_Rede_Porta()
                            {
                                Id = Convert.ToInt64(_reader["ID"]),
                                Cliente_Plano_Prod = _reader["CLIENTE_PLANO_PRODUTO"] != DBNull.Value ? Convert.ToInt64(_reader["CLIENTE_PLANO_PRODUTO"]) : 0,
                                Login = _reader["LOGIN"] != DBNull.Value ? Convert.ToString(_reader["LOGIN"]) : String.Empty,
                                Porta = Convert.ToInt32(_reader["PORTA"]),
                                Status = (Sinapse.Model.enumerados.Situacao_Porta)Enum.Parse(typeof(Sinapse.Model.enumerados.Situacao_Porta), Convert.ToString(_reader["STATUS"])),
                            });
                        }
                    }
                    retorno.Ok = true;
                }
            }
            catch (Exception erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = String.IsNullOrEmpty(erro.InnerException?.Message) ? $"Erro: {erro.Message}" :
                   $"Erros: \r\n{erro.Message}\r\n{erro.InnerException?.Message}";
            }
            return retorno;
        }
        private async Task<Modelo_Elemento_Rede> GetModeloElemento(long? Id)
        {
            Modelo_Elemento_Rede modelo = new Modelo_Elemento_Rede();
            StringBuilder sql = new StringBuilder();
            using (ISession session = HibernateUtil.GetSessionT5())
            using (DbCommand com = session.Connection.CreateCommand())
            {

                sql.AppendLine($"SELECT A.ID,A.DESCRICAO FROM TB_MODELO_ELEMENTO_REDE A WHERE A.ID = '{Id}'");

                using (DbDataReader reader = await CommandHelper.ExecuteReader(com, sql, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {

                        modelo.Id = Convert.ToInt64(reader["ID"]);
                        modelo.Descricao = Convert.ToString(reader["DESCRICAO"]);

                    }

                }
            }
            return modelo;

        }
        private async Task<Tipo_Elemento_Rede> GetTipoElemento(ObjApiRequest<RequestTipo_Modelo> request)
        {
            Tipo_Elemento_Rede tipo = null;
            StringBuilder sql = new StringBuilder();
            List<String> condicoes = new List<String>();
            using (ISession session = HibernateUtil.GetSessionT5())
            using (DbCommand com = session.Connection.CreateCommand())
            {

                sql.AppendLine($"SELECT  FIRST 1 A.ID,A.DESCRICAO,A.CAPACIDADE FROM TB_TIPO_ELEMENTO_REDE A");

                if (request.Obj.Capacidade > 0)
                {
                    condicoes.Add($"A.CAPACIDADE = {request.Obj.Capacidade}");
                }

                if (!String.IsNullOrEmpty(request.Obj.Fabricante) && request.Obj.Fabricante != "Desconhecido")
                {
                    condicoes.Add($"A.FABRICANTE LIKE UPPER'{request.Obj.Fabricante}'");
                }
                if (request.Obj.Id_Tipo.HasValue)
                {
                    condicoes.Add($"A.ID = '{request.Obj.Id_Tipo}'");
                }
                if (request.Obj.E_Caixa)
                {
                    condicoes.Add($"A.E_CAIXA = 1");
                }

                if (condicoes.Count > 0)
                {
                    sql.AppendLine($"WHERE {String.Join(" AND ", condicoes)}");
                }

                using (DbDataReader reader = await CommandHelper.ExecuteReader(com, sql, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        tipo = new Tipo_Elemento_Rede();
                        tipo.Capacidade = Convert.ToInt64(reader["CAPACIDADE"]);
                        tipo.Id = Convert.ToInt64(reader["ID"]);
                        tipo.Descricao = Convert.ToString(reader["DESCRICAO"]);
                    }

                    tipo.Modelo = await this.GetModeloElemento(request.Obj.Id_Modelo);
                    tipo.Icone = await this.GetIcone(request.Obj.Id_Icone);

                }
            }
            return tipo;

        }
        private async Task<Tipo_ElementoIcone> GetIcone(long request)
        {

            Tipo_ElementoIcone Icon = new Tipo_ElementoIcone();
            using (ISession session = HibernateUtil.GetSessionT5())
            using (DbCommand com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendLine("SELECT * FROM TB_TIPO_ELEMENTO_ICONE A ");
                sql.AppendLine($"WHERE  A.ID='{request}'");
                using (DbDataReader reader = await CommandHelper.ExecuteReader(com, sql, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        Icon.Arquivo = Convert.ToString(reader["ARQUIVO"]);
                        Icon.Caminho = Convert.ToString(reader["CAMINHO"]);
                        Icon.Descricao = Convert.ToString(reader["DESCRICAO"]);
                        Icon.Id = Convert.ToInt64(reader["ID"]);
                        Icon.Url = Convert.ToString(reader["URL"]);
                    }
                }
            }
            return Icon;
        }

        [HttpPost()]
        [Route("api/Elemento_RedeApi/SaveTipo_Modelo")]
        public async Task<PostApiRetorno> SaveTipo_Modelo(ObjApiRequest<RequestTipo_Modelo> request)
        {
            var retorno = new PostApiRetorno();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (Conexao<Tipo_Elemento_Rede> repositoryTipo = new Conexao<Tipo_Elemento_Rede>(sessionT5))
                using (Conexao<Tipo_ElementoIcone> repositoryIcon = new Conexao<Tipo_ElementoIcone>(sessionT5))
                using (Conexao<Modelo_Elemento_Rede> repositoryModelo = new Conexao<Modelo_Elemento_Rede>(sessionT5))
                {
                    Tipo_Elemento_Rede tipo = new Tipo_Elemento_Rede
                    {
                        UsuarioCriacao = User.Identity.GetUserId(),
                        DataHoraCriacao = DateTime.Now,

                        Capacidade = request.Obj.Capacidade ?? 0,
                        E_Caixa = request.Obj.E_Caixa,

                        Fabricante = request.Obj.Fabricante,
                        Situacao = Status_Default.Ativo,

                        Modelo = await repositoryModelo.selectFirst(p => p.Id == request.Obj.Id_Modelo.Value),
                        Icone = await repositoryIcon.selectFirst(p => p.Id == request.Obj.Id_Icone),

                    };
                    await repositoryTipo.Save(tipo);
                    retorno.Ok = true;
                }
            }
            catch (Exception erro)
            {
                retorno.Ok = false;
                retorno.Mensagem = String.IsNullOrEmpty(erro.InnerException?.Message) ? $"Erro: {erro.Message}" :
                  $"Erros:\r\n{erro.Message}\r\n{erro.InnerException?.Message}";
            }
            return retorno;
        }



        [HttpPost()]
        [Route("api/Elemento_RedeApi/UpdateElementoRede")]
        public async Task<PostApiRetorno> UpdateElementoRede(ElementoRedeRequestModel dados)
        {
            var retorno = new PostApiRetorno();
            try
            {
                // Verifica a validade do usuário
                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(dados.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Inválidos");
                    }
                }

                if (dados.Id_Tipo == null)
                {
                    throw new Exception("Tipo de rede não preenchido");
                }

                if (dados.Descricao == null)
                {
                    throw new Exception("Descrição de rede não preenchido");
                }
                if (dados.Situacao == null)
                {
                    throw new Exception("Situacao não preenchido");
                }

                if (dados.Projeto == null)
                {
                    throw new Exception("Projeto não preenchido");
                }

                // Prepara os parâmetros para o comando SQL
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (DbCommand com = session.Connection.CreateCommand())
                {
                    // Monta a query de UPDATE
                    StringBuilder sql = new StringBuilder();
                    sql.Append("UPDATE TB_ELEMENTO_REDE SET ");
                    sql.Append("Bairro = @Bairro, ");
                    sql.Append("Pais = @Pais, ");
                    sql.Append("Cod_Caixa = @Cod_Caixa, ");
                    sql.Append("Cabo = @Cabo, ");
                    sql.Append("Caixa = @Caixa, ");
                    sql.Append("Poste = @Poste, ");
                    sql.Append("Capacidade = @Capacidade, ");
                    sql.Append("CEP = @CEP, ");
                    sql.Append("Certificado = @Certificado, ");
                    sql.Append("Cidade = @Cidade, ");
                    sql.Append("Complemento = @Complemento, ");
                    sql.Append("Contrato = @Contrato, ");
                    sql.Append("Cor_linha_Ativa = @Cor_linha_Ativa, ");
                    sql.Append("Cor_linha_Inativa = @Cor_linha_Inativa, ");
                    sql.Append("Descricao = @Descricao, ");
                    sql.Append("Endereco = @Endereco, ");
                    sql.Append("Id_POP = @Id_POP, ");
                    sql.Append("Id_OLT = @Id_OLT, ");
                    sql.Append("Id_Slot = @Id_Slot, ");
                    sql.Append("Id_Pon = @Id_Pon, ");
                    sql.Append("Latitude = @Latitude, ");
                    sql.Append("Longitude = @Longitude, ");
                    sql.Append("Numero = @Numero, ");
                    sql.Append("Observacao = @Observacao, ");
                    sql.Append("Projeto = @Projeto, ");
                    sql.Append("Situacao = @Situacao, ");
                    sql.Append("Id_Tipo = @Id_Tipo, ");
                    sql.Append("UF = @UF, ");
                    sql.Append("Coordenadas = @Coordenadas, ");
                    sql.Append("AreaAtendimento = @AreaAtendimento, ");
                    sql.Append("Num_Coordenadas = @Num_Coordenadas, ");
                    sql.Append("Metragem = @Metragem, ");
                    sql.Append("Padrao_Cores = @Padrao_Cores ");
                    sql.Append("WHERE Id = @Id");
                    parametros.Add("@Id", dados.Id);
                    parametros.Add("@Bairro", dados.Bairro);
                    parametros.Add("@Pais", dados.Pais);
                    parametros.Add("@Cod_Caixa", dados.Cod_Caixa);
                    parametros.Add("@Cabo", dados.Cabo);
                    parametros.Add("@Caixa", dados.Caixa);
                    parametros.Add("@Poste", dados.Poste);
                    parametros.Add("@Capacidade", dados.Capacidade);
                    parametros.Add("@CEP", dados.CEP);
                    parametros.Add("@Certificado", dados.Certificado);
                    parametros.Add("@Cidade", dados.Cidade);
                    parametros.Add("@Complemento", dados.Complemento);
                    parametros.Add("@Contrato", dados.Contrato);
                    parametros.Add("@Cor_linha_Ativa", dados.Cor_linha_Ativa);
                    parametros.Add("@Cor_linha_Inativa", dados.Cor_linha_Inativa);
                    parametros.Add("@Descricao", dados.Descricao);
                    parametros.Add("@Endereco", dados.Endereco);
                    parametros.Add("@Id_POP", dados.Id_POP);
                    parametros.Add("@Id_OLT", dados.Id_OLT);
                    parametros.Add("@Id_Slot", dados.Id_Slot);
                    parametros.Add("@Id_Pon", dados.Id_Pon);
                    parametros.Add("@Latitude", dados.Latitude);
                    parametros.Add("@Longitude", dados.Longitude);
                    parametros.Add("@Numero", dados.Numero);
                    parametros.Add("@Observacao", dados.Observacao);
                    parametros.Add("@Projeto", dados.Projeto);
                    parametros.Add("@Situacao", dados.Situacao);
                    parametros.Add("@Id_Tipo", dados.Id_Tipo);
                    parametros.Add("@UF", dados.UF);
                    parametros.Add("@Coordenadas", dados.Coordenadas);
                    parametros.Add("@AreaAtendimento", dados.AreaAtendimento);
                    parametros.Add("@Num_Coordenadas", dados.Num_Coordenadas);
                    parametros.Add("@Metragem", dados.Metragem);
                    parametros.Add("@Padrao_Cores", dados.Padrao_Cores);
                    await CommandHelper.ExecuteNonQuery(com, sql, parametros, sessionT5.Connection, null);
                    retorno.Mensagem = "OK";
                    retorno.Ok = true;
                    return retorno;
                }
            }
            catch (Exception erro)
            {
                retorno.InnerException = erro.Message;
                retorno.Mensagem = "erro";
                retorno.Ok = false;
                return retorno;
            }
        }
        [HttpPost()]
        [Route("api/Elemento_RedeApi/ReservarPortaGeoGrid")]
        public async Task<PostApiRetorno> ReservarPortaGeoGrid(ReservaGeoGrid request)
        {
            var retorno = new PostApiRetorno();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (DbCommand com = session.Connection.CreateCommand())
                {
                    using (var repoLogin = new Conexao<LoginRadius>(session))
                    using (var repoPorta = new Conexao<Elemento_Rede_Porta>(sessionT5))
                    using (var repoIntegracao = new Conexao<IntegracoesExternas>(session))
                    using (var repoClipp = new Conexao<Cliente_PlanoProduto>(sessionT5))
                    {
                        var integracao = (await repoIntegracao.select(p => p.Id == 2)).FirstOrDefault();
                        if (integracao == null)
                            throw new Exception("Integração GeoGrid não encontrada.");

                        var porta = await repoPorta.selectFirst(p => p.Id == request.IdPorta);
                        if (porta == null)
                            throw new Exception($"Porta { request.IdPorta } não encontrada. ");


                        var helper = new IntegracaoExternaHelper(session, sessionT5);

                        var responseReserva = helper.Reservar_PortaGeoGrid(integracao, porta);

                        if (!responseReserva.success)
                        {
                            throw new Exception($"Erro ao Adicionar Conexão. \r\n{responseReserva.message}");
                        }
                      
                    }
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException?.Message;
            }

            return retorno;
        }


    }
}


