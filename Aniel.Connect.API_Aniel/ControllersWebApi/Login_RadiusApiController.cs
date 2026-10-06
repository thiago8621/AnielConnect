using Aniel.Connect.API_Aniel.Helper.IManagerHelper;
using Aniel.Connect.API_Aniel.Helper.IxcHelper;
using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.API_Aniel.Models.IManager;
using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.IntegracaoExterna;
using Aniel.Connect.Repository.Voalle;
using DocumentFormat.OpenXml.Drawing;
using log4net;
using NHibernate;
using NHibernate.Criterion;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using static Aniel.Connect.API_Aniel.Models.GeoGridModels;
using static Aniel.Connect.API_Aniel.Models.HubSoftModels;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class Login_RadiusApiController : BaseApiController
    {
        /// <summary>
        /// Método de Busca de Logins da Tabela Login_Radius do banco Connect 
        /// </summary>
        /// <param name="Obj"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/Login_RadiusApi/GetList")]
        public async Task<GetApiRetorno<IEnumerable<LoginRadiusApiModel>>> GetList(ObjApiRequest<LoginRadiusApiModel> Obj)
        {
            GetApiRetorno<IEnumerable<LoginRadiusApiModel>> retorno = new GetApiRetorno<IEnumerable<LoginRadiusApiModel>>
            {
                Objeto = new List<LoginRadiusApiModel>()
            };
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (DbCommand com = session.Connection.CreateCommand())

                {
                    using (UsuarioApiController userapi = new UsuarioApiController())
                    {
                        if (!(await userapi.GetUsuarioValido(Obj.settings)).Ok)
                        {
                            throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                        }
                    }
                    List<LoginRadiusApiModel> lista = new List<LoginRadiusApiModel>();
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine("SELECT A.ID, A.ID_CAIXA_FTTH, A.ID_PORTA_FTTH, A.ID_PLANO_PRODUTO, A.ID_CLIENTE,")
                        .AppendLine(" A.LOGIN FROM TB_LOGIN_RADIUS A");
                    using (DbDataReader reader = await CommandHelper.ExecuteReader(com, sql, new Dictionary<String, Object>(), session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(new LoginRadiusApiModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Id_Caixa_FTTH = Convert.ToInt64(reader["ID_CAIXA_FTTH"]),
                                Id_Porta_FTTH = Convert.ToInt64(reader["ID_PORTA_FTTH"]),
                                Id_Cliente_PlanoProduto = Convert.ToInt64(reader["ID_PLANO_PRODUTO"]),
                                Id_Cliente = Convert.ToInt64(reader["ID_CLIENTE"]),
                                Login = Convert.ToString(reader["LOGIN"]),

                            });
                        }

                    }
                    retorno.Objeto = lista;
                }
                //IEnumerable<LoginRadius> repoRadius = (await RadiusRepository.select()).ToList();


                retorno.Mensagem = "Êxito na Consulta dos Logins";
                retorno.Ok = true;
            }
            catch (Exception erro)
            {
                retorno.Mensagem = String.IsNullOrEmpty(erro.InnerException?.Message) ? $"Erro: {erro.Message}" :
                       $"Erros:\r\n{erro.Message}\r\n{erro.InnerException?.Message}";
                retorno.Mensagem += "\r\n Url: api/Login_RadiusApi/GetList";
                retorno.Ok = false;
            }

            return retorno;

        }

        /// <summary>
        /// Método de Inserção de Registros de Login na tabela TB_Login_Radius no banco do Connect
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/Login_RadiusApi/Insert")]
        public async Task<PostApiRetorno> Insert(ObjApiRequest<LoginRadiusApiModel> value)
        {
            PostApiRetorno retorno = new PostApiRetorno();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repoPorta = new Conexao<Elemento_Rede_Porta>(sessionT5))
                using (DbCommand com = session.Connection.CreateCommand())
                //using (Conexao<LoginRadius> RadiusRepository = new Conexao<LoginRadius>(session))
                {
                    using (UsuarioApiController userapi = new UsuarioApiController())
                    {
                        if (!(await userapi.GetUsuarioValido(value.settings)).Ok)
                        {
                            throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                        }
                    }

                    if (value.Obj.Id_Caixa_FTTH == 0)
                        value.Obj.Id_Caixa_FTTH = null;

                    if (value.Obj.Id_Porta_FTTH == 0)
                        value.Obj.Id_Porta_FTTH = null;


                    StringBuilder sql = new StringBuilder();
                    Dictionary<String, object> dict = new Dictionary<String, object>();

                    Elemento_Rede_Porta portaEncontrada = null;

                    if (value.Obj.Id_Porta_FTTH.HasValue == false && value.Obj.Id_Caixa_FTTH.HasValue && value.Obj.NumeroPorta.HasValue)
                    {
                        portaEncontrada = await repoPorta.selectFirst(p => p.Caixa.Id == value.Obj.Id_Caixa_FTTH && p.Porta == value.Obj.NumeroPorta);
                        value.Obj.Id_Porta_FTTH = portaEncontrada?.Id;
                    }
                    else if (value.Obj.Id_Porta_FTTH.HasValue)
                    {
                        portaEncontrada = await repoPorta.selectFirst(p => p.Id == value.Obj.Id_Porta_FTTH);
                    }

                    FillDictionaryLogin(value.Obj, dict);
                    dict = dict.Where(p => (p.Value != null)).ToDictionary(p => p.Key, p => p.Value);
                    var arguments = dict.Select(p => { return p.Key.Replace("@", ""); }).ToList();

                    sql.AppendFormat("INSERT INTO TB_LOGIN_RADIUS ({0}) VALUES({1})", String.Join(",", arguments), String.Join(",", dict.Keys));

                    //LoginRadius obj = this.BindViewModelHibernate(value.Obj, session);
                    await CommandHelper.ExecuteNonQuery(com, sql, dict, session.Connection, null);

                    if (portaEncontrada != null)
                    {
                        portaEncontrada.Status = Sinapse.Model.enumerados.Situacao_Porta.Ocupado;
                        await repoPorta.SaveOrUpdate(portaEncontrada);
                    }


                    //await RadiusRepository.SaveOrUpdate(obj);

                    retorno.Ok = true;
                    retorno.Mensagem = $"Objeto cadastrado com êxito";


                }
            }
            catch (Exception erro)
            {
                retorno.Mensagem = String.IsNullOrEmpty(erro.InnerException?.Message) ? $"Erro: {erro.Message}" :
                     $"Erros:\r\n{erro.Message}\r\n{erro.InnerException?.Message}";
                retorno.Mensagem += "\r\n Url: api/Login_RadiusApi/Insert";

            }

            return retorno;

        }

        [HttpPost()]
        [Route("api/Login_RadiusApi/GetUsuarioRadius")]
        public async Task<GetApiRetorno<LoginRadiusApiModel>> GetUsuario_Radius(ObjApiRequest<string> value)
        {
            GetApiRetorno<LoginRadiusApiModel> retorno = new GetApiRetorno<LoginRadiusApiModel>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionNH = HibernateUtil.GetSession())
                using (DbCommand com = session.Connection.CreateCommand())
                using (Conexao<Cliente_PlanoProduto> repo_Cli_Plan_Prod = new Conexao<Cliente_PlanoProduto>(sessionNH))

                {
                    using (UsuarioApiController userApi = new UsuarioApiController())
                    {
                        if (!(await userApi.GetUsuarioValido(value.settings)).Ok)
                        {
                            throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                        }
                    }
                    Dictionary<String, Object> parametros = new Dictionary<String, Object>();
                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine($"SELECT A.ID, A.ID_CAIXA_FTTH, A.ID_PORTA_FTTH, A.ID_PLANO_PRODUTO FROM TB_LOGIN_RADIUS A WHERE A.ID = {value.Obj}");

                    using (DbDataReader reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 1, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            retorno.Objeto = new LoginRadiusApiModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Id_Caixa_FTTH = Convert.IsDBNull(reader["ID_CAIXA_FTTH"]) ? (long?)null : Convert.ToInt64(reader["ID_CAIXA_FTTH"]),

                                Id_Porta_FTTH = Convert.IsDBNull(reader["ID_PORTA_FTTH"]) ? (long?)null : Convert.ToInt64(reader["ID_PORTA_FTTH"]),
                                Id_Cliente_PlanoProduto = Convert.IsDBNull(reader["ID_PLANO_PRODUTO"]) ? (long?)null : Convert.ToInt64(reader["ID_PLANO_PRODUTO"]),
                                //Cliente = Convert.IsDBNull()

                            };
                        }
                    }
                    if (retorno.Objeto == null)
                    {
                        throw new Exception("Login não encontrado");
                    }

                    retorno.Mensagem = "Usuário Encontrado com êxito";
                    retorno.Ok = true;
                }
            }
            catch (Exception erro)
            {
                retorno.Ok = false;

                retorno.Mensagem = String.IsNullOrEmpty(erro.InnerException?.Message) ? $"Erro: {erro.Message}" :
                    $"Erros:\r\n{erro.Message}\r\n{erro.InnerException?.Message}";
                retorno.Mensagem += "\r\n Url: api/Login_RadiusApi/GetUsuarioRadius";
            }

            return retorno;
        }

        /// <summary>
        /// Atualizar caixa e porta no cadastro de Login das integrações.
        /// </summary>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/Login_RadiusApi/AtualizarCaixaPorta")]
        public async Task<PostApiRetorno> AtualizarCaixaPorta(ObjApiRequest<LoginRadiusAtualizacao> request)
        {
            //Lógica para retrocompatibilidade temporária até que todas as aplicações estejam olhando o novo campo TIPO_INTEGRACAO_CTO da TB_MASTER   
            if (!request.Tipo_Integracao.HasValue)
                request.Tipo_Integracao = IntegracaoExterna.IXC;

            var retorno = new PostApiRetorno();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())

                using (DbCommand comT5 = sessionT5.Connection.CreateCommand())
                using (Conexao<Elemento_Rede> repoElemento = new Conexao<Elemento_Rede>(sessionT5))
                using (Conexao<Elemento_Rede_Porta> repoElementoPorta = new Conexao<Elemento_Rede_Porta>(sessionT5))
                using (Conexao<Documento_ProducaoAniel> repoDocumento = new Conexao<Documento_ProducaoAniel>(sessionT5))
                using (Conexao<LoginRadius> repoLogin = new Conexao<LoginRadius>(session))
                {
                    using (UsuarioApiController userApi = new UsuarioApiController())
                    {
                        if (!(await userApi.GetUsuarioValido(request.settings)).Ok)
                        {
                            throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                        }
                    }

                    switch (request.Tipo_Integracao)
                    {
                        case Model.enumerado.IntegracaoExterna.IXC:
                            retorno = await this.AtualizacaoIXC(session, sessionT5, request);
                            break;
                        case Model.enumerado.IntegracaoExterna.GEOSITE:
                            retorno = await this.AtualizacaoGeosite(session, sessionT5, request);
                            break;
                        case Model.enumerado.IntegracaoExterna.IMANAGER:
                            retorno = await this.AtualizacaoIManager(session, sessionT5, request);
                            break;
                        case Model.enumerado.IntegracaoExterna.GEOGRID:
                            retorno = await this.AtualizacaoGeoGrid(session, sessionT5, request);
                            break;

                        case Model.enumerado.IntegracaoExterna.HUBSOFT:
                            retorno = await this.AtualizacaoHubSoft(session, sessionT5, request);
                            break;
                        case Model.enumerado.IntegracaoExterna.VOALLE:
                            retorno = await this.AtualizacaoVoalle(session, sessionT5, request);
                            break;
                    }


                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                retorno.Mensagens = new List<PostApiMensagem> {
                    new PostApiMensagem { Mensagem = ex.StackTrace }
                };
                retorno.InnerException = ex.InnerException?.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }
        public async Task<PostApiRetorno> AtualizacaoIXC(ISession session,
                                                         ISession sessionT5,
                                                         ObjApiRequest<LoginRadiusAtualizacao> request)
        {
            var retorno = new PostApiRetorno();
            try
            {
                using (DbCommand comT5 = sessionT5.Connection.CreateCommand())
                using (Conexao<Elemento_Rede> repoElemento = new Conexao<Elemento_Rede>(sessionT5))
                using (Conexao<Elemento_Rede_Porta> repoElementoPorta = new Conexao<Elemento_Rede_Porta>(sessionT5))
                using (Conexao<Documento_ProducaoAniel> repoDocumento = new Conexao<Documento_ProducaoAniel>(sessionT5))
                using (Conexao<LoginRadius> repoLogin = new Conexao<LoginRadius>(session))
                {

                    Config_Aniel_Integrator Config = new Config_Aniel_Integrator();
                    StringBuilder sql = new StringBuilder();
                    Dictionary<String, Object> parametros = new Dictionary<String, Object>();

                    //Capturar configurações da Integração

                    sql.AppendLine($"SELECT * ");
                    sql.AppendLine(" FROM TB_CONFIG_ANIEL_INTEGRATOR ");
                    //DAO IXC usando o Integrator
                    using (var reader = await CommandHelper.ExecuteReaderFirst(comT5, sql, parametros, sessionT5.Connection, null, 1, null))
                    {
                        if (await reader.ReadAsync())
                        {

                            Config.URL = Convert.ToString(reader["URL"]);
                            Config.UsuarioAniel = Convert.ToString(reader["USUARIO_ANIEL"]);
                            Config.CodCtPadrao = Convert.ToString(reader["CODCT_PADRAO"]);
                            Config.ProjetoPadrao = Convert.ToString(reader["PROJETO_PADRAO"]);
                            Config.Unidade_Custo = Convert.ToString(reader["UNIDADE_CUSTO"]);
                            Config.Token = Convert.ToString(reader["TOKEN"]);
                            Config.Setores = Convert.ToString(reader["SETORES"]);

                        }

                    }

                    LoginIXCHelper acessoLoginIXC = new LoginIXCHelper();
                    Elemento_Rede_Porta porta = request.Obj.IdCaixa.HasValue && request.Obj.numeroPorta.HasValue ? await repoElementoPorta.selectFirst(p => p.Caixa.Id == request.Obj.IdCaixa && p.Porta == request.Obj.numeroPorta) : null;
                    LoginRadius loginAnterior = porta != null ? await repoLogin.selectFirst(p => p.Id_Caixa_FTTH == request.Obj.IdCaixa && p.Id_Porta_FTTH == porta.Id && p.Id != request.Obj.IdLogin) : null;

                    // Se o login não é nulo, anular caixa e porta para o login para então atualizar dos objetos de Elemento de Rede e Porta
                    if (loginAnterior != null)
                    {
                        loginAnterior.Id_Caixa_FTTH = null;
                        loginAnterior.Id_Porta_FTTH = null;


                        if (Config?.URL != null)
                        {
                            LoginIXC retornoLoginAnteriorIXC = acessoLoginIXC.GetLogin($"{loginAnterior.Id}", Config)
                                                                             .registros.FirstOrDefault();
                            LoginIXC objAnterior = retornoLoginAnteriorIXC;

                            bool limparLoginAnteriorIxc = objAnterior != null && porta != null && objAnterior.id_caixa_ftth == Convert.ToString(porta?.Caixa?.Id) && objAnterior.ftth_porta == Convert.ToString(porta?.Porta);

                            if (limparLoginAnteriorIxc)
                            {
                                objAnterior.id_caixa_ftth = null;
                                objAnterior.ftth_porta = null;
                                ResponsePostIXC atualizacaoCaixaPortaAnterior = acessoLoginIXC.AtualizarCaixaPorta(Config, objAnterior);

                                if (atualizacaoCaixaPortaAnterior.type.ToUpperInvariant().Contains("success".ToUpperInvariant()))
                                {
                                    IEnumerable<ClienteFibraIXC> retornoFibraAnterior = acessoLoginIXC.
                                                                                                 GetClienteFibra(Convert.ToString(loginAnterior.Id), Config).
                                                                                                 registros ?? new List<ClienteFibraIXC>();

                                    ClienteFibraIXC clienteFibraAnteriorIXC = retornoFibraAnterior.FirstOrDefault();
                                    if (clienteFibraAnteriorIXC != null)
                                    {
                                        clienteFibraAnteriorIXC.id_caixa_ftth = null;
                                        clienteFibraAnteriorIXC.porta_ftth = null;

                                        ResponsePostIXC atualizacaoClienteFibraAnterior = acessoLoginIXC.AtualizarClienteFibra(Config, clienteFibraAnteriorIXC);

                                        if (!atualizacaoClienteFibraAnterior.type.ToUpperInvariant().Contains("success".ToUpperInvariant()))
                                            throw new Exception($"Erro ao atualizar ClienteFibra do Login anterior. Response IXC: {JsonService.SerializeObject(atualizacaoClienteFibraAnterior)}");

                                        //await repoLogin.SaveOrUpdate(loginAnterior);
                                    }
                                }
                                else
                                {
                                    throw new Exception($"Erro ao atualizar Caixa e Porta do Login anterior. Response IXC: {JsonService.SerializeObject(atualizacaoCaixaPortaAnterior)}");
                                }
                            }
                            await repoLogin.SaveOrUpdate(loginAnterior);
                        }
                        else
                        {
                            throw new Exception($"Erro ao obter url da TB_CONFIG_ANIEL_INTEGRATOR");
                        }


                    }


                    LoginRadius login = await repoLogin.selectFirst(p => p.Id == request.Obj.IdLogin);

                    if (login != null)
                    {
                        login.Id_Caixa_FTTH = porta?.Caixa?.Id;
                        login.Id_Porta_FTTH = porta?.Id;

                        if (Config?.URL != null)
                        {
                            var retornoIXC = acessoLoginIXC.GetLogin($"{request.Obj.IdLogin}", Config).registros.FirstOrDefault();
                            LoginIXC obj = retornoIXC;
                            obj.id_caixa_ftth = Convert.ToString(porta?.Caixa?.Id);
                            obj.ftth_porta = Convert.ToString(porta?.Porta);

                            ResponsePostIXC atualizacaoCaixaPorta = acessoLoginIXC.AtualizarCaixaPorta(Config, obj);
                            if (atualizacaoCaixaPorta.type.ToUpperInvariant().Contains("success".ToUpperInvariant()))
                            {
                                IEnumerable<ClienteFibraIXC> retornoFibraIXC = acessoLoginIXC.GetClienteFibra(Convert.ToString(login.Id), Config).
                                                                                                           registros ?? new List<ClienteFibraIXC>();

                                ClienteFibraIXC clienteFibraIXC = retornoFibraIXC.FirstOrDefault();
                                if (clienteFibraIXC != null)
                                {
                                    clienteFibraIXC.id_caixa_ftth = Convert.ToString(porta?.Caixa?.Id);
                                    clienteFibraIXC.porta_ftth = Convert.ToString(porta?.Porta);
                                    ResponsePostIXC atualizacaoClienteFibra = acessoLoginIXC.AtualizarClienteFibra(Config, clienteFibraIXC);

                                    if (!atualizacaoClienteFibra.type.ToUpperInvariant().Contains("success".ToUpperInvariant()))
                                        throw new Exception($"Erro ao atualizar ClienteFibra do Login. Response IXC: {JsonService.SerializeObject(atualizacaoClienteFibra)}");

                                    await repoLogin.SaveOrUpdate(login);
                                }
                                else
                                {
                                    CONSTANTES.log4Net.Error("[GetClienteFibra] - clienteFibraIXC retornou nulo");
                                }
                            }
                            else
                            {
                                throw new Exception($"Erro ao atualizar Caixa e Porta. Response IXC: {JsonService.SerializeObject(atualizacaoCaixaPorta)}");
                            }
                        }
                        else
                        {
                            throw new Exception($"Erro ao obter url da TB_CONFIG_ANIEL_INTEGRATOR");
                        }
                    }
                    else
                    {

                        throw new Exception($"Login Radius não encontrado.");
                    }
                }
                retorno.Ok = true;

            }

            catch (Exception exception)
            {
                retorno.Ok = false;
                retorno.Mensagem = exception.Message;
                retorno.InnerException = exception.InnerException?.Message;
                CONSTANTES.log4Net.Error(exception.Message, exception);
            }



            return retorno;
        }
        public async Task<PostApiRetorno> AtualizacaoGeosite(ISession session,
                                                             ISession sessionT5,
                                                             ObjApiRequest<LoginRadiusAtualizacao> request)
        {
            var retorno = new PostApiRetorno();
            try
            {
                using (DbCommand comT5 = sessionT5.Connection.CreateCommand())
                using (Conexao<Elemento_Rede> repoElemento = new Conexao<Elemento_Rede>(sessionT5))
                using (Conexao<Elemento_Rede_Porta> repoElementoPorta = new Conexao<Elemento_Rede_Porta>(sessionT5))
                using (Conexao<Documento_ProducaoAniel> repoDocumento = new Conexao<Documento_ProducaoAniel>(sessionT5))
                using (Conexao<IntegracoesExternas> repoIntegracoes = new Conexao<IntegracoesExternas>(session))
                using (Conexao<Cliente_PlanoProduto> repoCpp = new Conexao<Cliente_PlanoProduto>(session))
                using (Conexao<LoginRadius> repoLogin = new Conexao<LoginRadius>(session))
                {
                    IntegracoesExternas integracoes = (await repoIntegracoes.select(p => p.IntegracaoExterna == request.Tipo_Integracao))
                                                                            .FirstOrDefault();

                    Elemento_Rede_Porta porta = await repoElementoPorta.selectFirst(p => p.Caixa.Id == request.Obj.IdCaixa &&
                                                                                         p.Id == request.Obj.numeroPorta);

                    LoginRadius loginAnterior = await repoLogin.selectFirst(p => p.Id_Caixa_FTTH == request.Obj.IdCaixa &&
                                                                                 p.Id_Porta_FTTH == porta.Id);
                    if (loginAnterior != null)
                    {
                        loginAnterior.Id_Caixa_FTTH = null;
                        loginAnterior.Id_Porta_FTTH = null;

                        await repoLogin.Save(loginAnterior);

                    }

                    LoginRadius login = await repoLogin.selectFirst(p => p.Id == request.Obj.IdLogin);
                    login.Id_Caixa_FTTH = porta?.Caixa?.Id;
                    login.Id_Porta_FTTH = porta?.Id;
                    await repoLogin.Save(login);

                    //Geosite
                    IntegracaoExternaHelper helper = new IntegracaoExternaHelper(session, sessionT5);

                    Cliente_PlanoProduto cpp = await repoCpp.selectFirst(p => p.Id == porta.Cliente_Plano_Prod);
                    ResponseGeosite<ClienteGeosite> response = helper.CheckClienteGeosite(integracoes, cpp.Cliente.CpfCnpj);
                    if (response.total != "0" || response.total != null)
                    {
                        ResponseGeosite responseConexao = await helper.Adicionar_ConexaoCliente(integracoes, porta, cpp.Id);
                        if (!responseConexao.success)
                        {
                            throw new Exception($"Erro ao Adicionar Conexão. \r\n{responseConexao.message}");
                        }
                    }

                    else
                    {
                        ClienteGeosite clienteGeosite = helper.BindClienteGeosite(cpp.Cliente, porta);
                        //SaveModelCliente ObjBind = helper.BindSaveModel(clienteGeosite);
                        ResponseGeosite responseSave = helper.SaveCliente(integracoes, clienteGeosite);

                        if (!responseSave.success)
                        {
                            throw new Exception($"Erro ao Salvar Cliente. \r\n{responseSave.message}");
                        }
                    }

                }

                retorno.Ok = true;

            }
            catch (Exception exception)
            {
                retorno.Ok = false;
                retorno.Mensagem = exception.Message;
                retorno.InnerException = exception?.InnerException?.Message;
            }
            return retorno;
        }

        public async Task<PostApiRetorno> AtualizacaoHubSoft(ISession session,
                                                            ISession sessionT5,
                                                            ObjApiRequest<LoginRadiusAtualizacao> request)

        {
            var retorno = new PostApiRetorno();
            try
            {
                using (DbCommand comT5 = sessionT5.Connection.CreateCommand())
                using (Conexao<Elemento_Rede> repoElemento = new Conexao<Elemento_Rede>(sessionT5))
                using (Conexao<Elemento_Rede_Porta> repoElementoPorta = new Conexao<Elemento_Rede_Porta>(sessionT5))
                using (Conexao<Documento_ProducaoAniel> repoDocumento = new Conexao<Documento_ProducaoAniel>(sessionT5))
                using (Conexao<IntegracoesExternas> repoIntegracoes = new Conexao<IntegracoesExternas>(session))
                using (Conexao<Cliente_PlanoProduto> repoCpp = new Conexao<Cliente_PlanoProduto>(session))
                using (Conexao<Endereco_Cliente> repoEndereco = new Conexao<Endereco_Cliente>(session))

                using (Conexao<LoginRadius> repoLogin = new Conexao<LoginRadius>(session))
                {
                    IntegracoesExternas integracoes = (await repoIntegracoes.select(p => p.IntegracaoExterna == request.Tipo_Integracao))
                                                                            .FirstOrDefault();
                    Elemento_Rede_Porta porta = request.Obj.IdCaixa.HasValue && request.Obj.numeroPorta.HasValue ? await repoElementoPorta.selectFirst(p => p.Caixa.Id == request.Obj.IdCaixa && p.Porta == request.Obj.numeroPorta) : null;

                    Elemento_Rede elemento = await repoElemento.selectFirst(p => p.Id == request.Obj.IdCaixa);


                    //   Endereco_Cliente endereco = await repoEndereco.selectFirst(e => e.Id_Cliente_Integracao == request.Obj.IdLogin);
                    LoginRadius login = await repoLogin.selectFirst(p => p.Id == request.Obj.IdLogin);

                    IntegracaoExternaHelper helper = new IntegracaoExternaHelper(session, sessionT5);

                    PortaHubSoftRequest portaHub = new PortaHubSoftRequest();

                    portaHub.Login = login?.Login;
                    portaHub.Porta = Convert.ToInt64(porta?.CodigoAuxiliar);
                    portaHub.IdClienteServico = Convert.ToInt64(login?.CodigoAuxiliar);
                    portaHub.IdPortaAtendimento = Convert.ToInt64(porta?.CodigoAuxiliar);
                    LoginRadius loginAnterior = porta != null ? await repoLogin.selectFirst(p => p.Id_Caixa_FTTH == request.Obj.IdCaixa && p.Id_Porta_FTTH == porta.Id) : null;

                    // Se o login anterior não é nulo, anular caixa e porta para o login para então atualizar os objetos de Elemento de Rede e Porta
                    if (loginAnterior != null && loginAnterior.Id != login?.Id)
                    {
                        loginAnterior.Id_Caixa_FTTH = null;
                        loginAnterior.Id_Porta_FTTH = null;

                        await repoLogin.SaveOrUpdate(loginAnterior);
                        var idClienteServico = loginAnterior.CodigoAuxiliar;

                        var response = helper.LimpezaPortaHubsoft(integracoes, idClienteServico).Result;

                        if (!response.Ok)
                        {
                            throw new Exception($"Erro ao Vincular Porta. \r\n{response.Mensagem}");
                        }
                    }

                    if (porta == null && login != null)
                    {
                        login.Id_Caixa_FTTH = null;
                        login.Id_Porta_FTTH = null;

                        await repoLogin.SaveOrUpdate(login);


                        var response = helper.LimpezaPortaHubsoft(integracoes, login.CodigoAuxiliar).Result;

                        if (!response.Ok)
                        {
                            throw new Exception($"Erro ao Vincular Porta. \r\n{response.Mensagem}");
                        }
                    }
                    if (login != null && porta!=null)
                    {
                        PostApiRetorno RESPONSE = helper.VincularPortaHubsoft(integracoes, portaHub).Result;

                        if (!RESPONSE.Ok)
                        {
                            throw new Exception($"Erro ao Vincular Porta. \r\n{RESPONSE.Mensagem}");
                        }

                        login.Id_Porta_FTTH = porta?.Id;
                        login.Id_Caixa_FTTH = porta?.Caixa?.Id;

                        await repoLogin.SaveOrUpdate(login);
                    }
                }

                retorno.Ok = true;

            }
            catch (Exception exception)
            {
                CONSTANTES.log4Net.Error(exception.Message + $" | payload: {JsonService.SerializeObject(request)}", exception);
                retorno.Ok = false;
                retorno.Mensagem = exception.Message;
                retorno.InnerException = exception?.InnerException?.Message;
            }
            return retorno;
        }
        public async Task<PostApiRetorno> AtualizacaoGeoGrid(ISession session,
                                                    ISession sessionT5,
                                                    ObjApiRequest<LoginRadiusAtualizacao> request)
        {
            var retorno = new PostApiRetorno();
            try
            {
                using (DbCommand comT5 = sessionT5.Connection.CreateCommand())
                using (Conexao<Elemento_Rede> repoElemento = new Conexao<Elemento_Rede>(sessionT5))
                using (Conexao<Elemento_Rede_Porta> repoElementoPorta = new Conexao<Elemento_Rede_Porta>(sessionT5))
                using (Conexao<Documento_ProducaoAniel> repoDocumento = new Conexao<Documento_ProducaoAniel>(sessionT5))
                using (Conexao<IntegracoesExternas> repoIntegracoes = new Conexao<IntegracoesExternas>(session))
                using (Conexao<Cliente_PlanoProduto> repoCpp = new Conexao<Cliente_PlanoProduto>(session))
                using (Conexao<Endereco_Cliente> repoEndereco = new Conexao<Endereco_Cliente>(session))

                using (Conexao<LoginRadius> repoLogin = new Conexao<LoginRadius>(session))
                {
                    IntegracoesExternas integracoes = (await repoIntegracoes.select(p => p.IntegracaoExterna == request.Tipo_Integracao))
                                                                            .FirstOrDefault();

                    Elemento_Rede elemento = await repoElemento.selectFirst(p => p.Id == request.Obj.IdCaixa);

                    if (elemento == null)
                    {
                        throw new Exception($"Erro ao Vincular Porta. \r\n Caixa {request.Obj.IdCaixa} não encontrada no Aniel");

                    }

                    Elemento_Rede_Porta porta = await repoElementoPorta.selectFirst(p => p.Caixa.Id == request.Obj.IdCaixa &&
                                                                                         p.Porta == request.Obj.numeroPorta);

                    if (porta == null)
                    {
                        throw new Exception($"Erro ao Vincular Porta. \r\n Porta nº {request.Obj.numeroPorta} não encontrada para a Caixa {request.Obj.IdCaixa} no Aniel");

                    }

                    //   Endereco_Cliente endereco = await repoEndereco.selectFirst(e => e.Id_Cliente_Integracao == request.Obj.IdLogin);
                    LoginRadius login = await repoLogin.selectFirst(p => p.Id == request.Obj.IdLogin);
                    

                    if (login == null)
                    {
                        throw new Exception($"Erro ao Vincular Porta. \r\n login {request.Obj.IdLogin} não encontrado no Aniel");

                    }
                    IntegracaoExternaHelper helper = new IntegracaoExternaHelper(session, sessionT5);

                    
                    LoginRadius loginAnterior = porta != null ? await repoLogin.selectFirst(p => p.Id_Caixa_FTTH == request.Obj.IdCaixa && p.Id_Porta_FTTH == porta.Id && p.Id != request.Obj.IdLogin) : null;
                    ResponseGeogrid responseConexao = new ResponseGeogrid();
                    // Se o login anterior não é nulo, anular caixa e porta para o login para então atualizar os objetos de Elemento de Rede e Porta
                    if (loginAnterior != null)
                    {
                        VincluaPorta limpezaPorta = new VincluaPorta();
                        limpezaPorta.Login = loginAnterior.CodigoAuxiliar; //Esse Cod. Auxiliar não correspónde ao idCliente do Geogrid. Qual dado usar?
                        limpezaPorta.Porta = Convert.ToInt64(porta.CodigoAuxiliar);
                        loginAnterior.Id_Caixa_FTTH = null;
                        loginAnterior.Id_Porta_FTTH = null;
                        
                        await repoLogin.SaveOrUpdate(loginAnterior);
                        responseConexao = helper.LimpezaPortaGeoGrid(integracoes, limpezaPorta);

                       if (!responseConexao.success)
                       {
                           throw new Exception($"Erro ao limpar Porta do login anterior. \r\n{responseConexao.message}");
                       }
                    }

                    if(login != null)
                    {
                        //Ocupação de Caixa/Porta
                        if (request.Obj.IdCaixa.HasValue && request.Obj.numeroPorta.HasValue)
                        {
                            VincluaPorta vinculaPorta = new VincluaPorta();
                            vinculaPorta.Login = login.CodigoAuxiliar; //Esse Cod. Auxiliar não correspónde ao idCliente do Geogrid. Qual dado usar?
                            vinculaPorta.Porta = Convert.ToInt64(porta.CodigoAuxiliar);
                            vinculaPorta.Longitude = elemento.Longitude.ToString();
                            vinculaPorta.Latitude = elemento.Latitude.ToString();

                            responseConexao = helper.VinculaPortaGeoGrid(integracoes, vinculaPorta);
                            if (!responseConexao.success)
                            {
                                retorno.Ok = false;

                                throw new Exception($"Erro ao Vincular Porta. \r\n Response GeoGrid: {responseConexao.message}");

                            }
                        }
                        //Limpeza de Caixa/Porta
                        else
                        {
                            VincluaPorta limpezaPorta = new VincluaPorta();
                            limpezaPorta.Login = login.CodigoAuxiliar; //Esse Cod. Auxiliar não correspónde ao idCliente do Geogrid. Qual dado usar?
                            limpezaPorta.Porta = Convert.ToInt64(porta.CodigoAuxiliar);
                            
                            responseConexao = helper.LimpezaPortaGeoGrid(integracoes, limpezaPorta);

                            if (!responseConexao.success)
                            {
                                throw new Exception($"Erro ao limpar Porta. \r\n{responseConexao.message}");
                            }
                        }
                    }                  
                }

                retorno.Ok = true;

            }
            catch (Exception exception)
            {
                CONSTANTES.log4Net.Error(exception.Message, exception);
                retorno.Ok = false;
                retorno.Mensagem = exception.Message;
                retorno.InnerException = exception?.InnerException?.Message;
            }
            return retorno;
        }

        public async Task<PostApiRetorno> AtualizacaoIManager(ISession session,
                                                             ISession sessionT5,
                                                             ObjApiRequest<LoginRadiusAtualizacao> request)
        {
            var retorno = new PostApiRetorno();
            try
            {
                using (DbCommand comT5 = sessionT5.Connection.CreateCommand())
                using (Conexao<Elemento_Rede> repoElemento = new Conexao<Elemento_Rede>(sessionT5))
                using (Conexao<Elemento_Rede_Porta> repoElementoPorta = new Conexao<Elemento_Rede_Porta>(sessionT5))
                using (Conexao<Documento_ProducaoAniel> repoDocumento = new Conexao<Documento_ProducaoAniel>(sessionT5))
                using (Conexao<LoginRadius> repoLogin = new Conexao<LoginRadius>(session))
                {

                    Config_Aniel_Integrator Config = new Config_Aniel_Integrator();
                    StringBuilder sql = new StringBuilder();
                    Dictionary<String, Object> parametros = new Dictionary<String, Object>();

                    //Capturar configurações da Integração
                    sql.AppendLine($"SELECT URL, USUARIO_ANIEL, USUARIO_INTEGRATOR, CODCT_PADRAO, PROJETO_PADRAO, UNIDADE_CUSTO, TOKEN, SETORES ");
                    sql.AppendLine(" FROM TB_CONFIG_ANIEL_INTEGRATOR ");
                    sql.AppendLine(" WHERE TIPO_INTEGRACAO = 1 ");

                    using (var reader = await CommandHelper.ExecuteReaderFirst(comT5, sql, parametros, sessionT5.Connection, null, 1, null))
                    {
                        if (await reader.ReadAsync())
                        {

                            Config.URL = Convert.ToString(reader["URL"]);
                            Config.UsuarioAniel = Convert.ToString(reader["USUARIO_ANIEL"]);
                            Config.UsuarioIntegrator = Convert.ToString(reader["USUARIO_INTEGRATOR"]);
                            Config.CodCtPadrao = Convert.ToString(reader["CODCT_PADRAO"]);
                            Config.ProjetoPadrao = Convert.ToString(reader["PROJETO_PADRAO"]);
                            Config.Unidade_Custo = Convert.ToString(reader["UNIDADE_CUSTO"]);
                            Config.Token = Convert.ToString(reader["TOKEN"]);
                            Config.Setores = Convert.ToString(reader["SETORES"]);

                        }
                    }

                    if (string.IsNullOrEmpty(Config.URL))
                        throw new Exception($"Erro ao obter url da TB_CONFIG_ANIEL_INTEGRATOR");

                    if (string.IsNullOrEmpty(Config.Token))
                        throw new Exception($"Erro ao obter Token (IntegrationSecret) da TB_CONFIG_ANIEL_INTEGRATOR");

                    if (string.IsNullOrEmpty(CONSTANTES.ChaveBaseIManager))
                        throw new Exception($"Erro ao obter ChaveBaseIManager do CONSTANTES");

                    IManagerHelper iManagerHelper = new IManagerHelper();
                    Elemento_Rede_Porta porta = request.Obj.IdCaixa.HasValue && request.Obj.numeroPorta.HasValue ? await repoElementoPorta.selectFirst(p => p.Caixa.Id == request.Obj.IdCaixa && p.Porta == request.Obj.numeroPorta) : null;
                    LoginRadius loginAnterior = porta != null ? await repoLogin.selectFirst(p => p.Id_Caixa_FTTH == request.Obj.IdCaixa && p.Id_Porta_FTTH == porta.Id && p.Id != request.Obj.IdLogin) : null;

                    // Se o login anterior não é nulo, anular caixa e porta para o login para então atualizar os objetos de Elemento de Rede e Porta
                    if (loginAnterior != null)
                    {
                        loginAnterior.Id_Caixa_FTTH = null;
                        loginAnterior.Id_Porta_FTTH = null;

                        ConsultaCTOIndividualRequest requestLoginAnteriorIManager = new ConsultaCTOIndividualRequest();
                        requestLoginAnteriorIManager.idcontrato = !String.IsNullOrEmpty(loginAnterior.CodigoAuxiliar) ? Convert.ToInt64(loginAnterior.CodigoAuxiliar) : Convert.ToInt64(loginAnterior.Login);
                        requestLoginAnteriorIManager.usuario = Config.UsuarioIntegrator;
                        requestLoginAnteriorIManager.chavebase = CONSTANTES.ChaveBaseIManager;

                        ConsultaCTOIndividualResponse retornoLoginAnteriorIManager = iManagerHelper.GetCTO(Config, requestLoginAnteriorIManager);

                        bool limparLoginAnteriorIManager = retornoLoginAnteriorIManager != null && porta != null && retornoLoginAnteriorIManager.idcto == porta?.Caixa?.Id && retornoLoginAnteriorIManager.idportacto.ToString() == porta?.CodigoAuxiliar;

                        if (limparLoginAnteriorIManager)
                        {
                            LimparPortaRequest requestLimpezaLoginAnterior = new LimparPortaRequest();
                            requestLimpezaLoginAnterior.idContrato = !String.IsNullOrEmpty(loginAnterior.CodigoAuxiliar) ? Convert.ToInt64(loginAnterior.CodigoAuxiliar) : Convert.ToInt64(loginAnterior.Login);

                            LimparPortaResponse limpezaCaixaPortaAnterior = iManagerHelper.LimparCaixaPorta(Config, requestLimpezaLoginAnterior);

                            if (limpezaCaixaPortaAnterior.cancelamentoEfetuado == false && limpezaCaixaPortaAnterior.reservaCancelada == false)
                            {
                                throw new Exception($"Erro ao limpar Caixa e Porta do Login anterior. Response IManager: {JsonService.SerializeObject(limpezaCaixaPortaAnterior)}");
                            }

                        }
                        await repoLogin.SaveOrUpdate(loginAnterior);
                    }

                    LoginRadius login = await repoLogin.selectFirst(p => p.Id == request.Obj.IdLogin);

                    if (login != null)
                    {
                        //Ocupação de Caixa/Porta
                        if (request.Obj.IdCaixa.HasValue && request.Obj.numeroPorta.HasValue)
                        {
                            login.Id_Caixa_FTTH = porta?.Caixa?.Id;
                            login.Id_Porta_FTTH = porta?.Id;

                            ReservarPortaRequest obj = new ReservarPortaRequest();
                            obj.idContrato = !String.IsNullOrEmpty(login.CodigoAuxiliar) ? Convert.ToInt64(login.CodigoAuxiliar) : Convert.ToInt64(login.Login);
                            obj.idCto = porta.Caixa.Id;
                            obj.numPorta = porta?.Porta ?? request.Obj.numeroPorta;

                            ReservarPortaResponse atualizacaoCaixaPorta = iManagerHelper.ReservarCaixaPorta(Config, obj);
                            if (atualizacaoCaixaPorta.reservaEfetuada == false)
                            {
                                throw new Exception($"Erro ao atualizar Caixa e Porta. Response IManager: {JsonService.SerializeObject(atualizacaoCaixaPorta)}");
                            }
                        }
                        //Limpeza de Caixa/Porta
                        else
                        {
                            login.Id_Caixa_FTTH = null;
                            login.Id_Porta_FTTH = null;

                            LimparPortaRequest requestLimpezaLogin = new LimparPortaRequest();
                            requestLimpezaLogin.idContrato = !String.IsNullOrEmpty(login.CodigoAuxiliar) ? Convert.ToInt64(login.CodigoAuxiliar) : Convert.ToInt64(login.Login);

                            LimparPortaResponse limpezaCaixaPorta = iManagerHelper.LimparCaixaPorta(Config, requestLimpezaLogin);

                            if (limpezaCaixaPorta.cancelamentoEfetuado == false && limpezaCaixaPorta.reservaCancelada == false)
                            {
                                throw new Exception($"Erro ao limpar Caixa e Porta do Login. Response IManager: {JsonService.SerializeObject(limpezaCaixaPorta)}");
                            }
                        }

                        await repoLogin.SaveOrUpdate(login);
                    }
                    else
                    {
                        throw new Exception($"Login Radius não encontrado.");
                    }
                }
                retorno.Ok = true;
            }
            catch (Exception exception)
            {
                CONSTANTES.log4Net.Error(exception.Message, exception);
                retorno.Ok = false;
                retorno.Mensagem = exception.Message;
                retorno.InnerException = exception.InnerException?.Message;
            }

            return retorno;
        }

        public async Task<PostApiRetorno> AtualizacaoVoalle(ISession session,
                                                             ISession sessionT5,
                                                             ObjApiRequest<LoginRadiusAtualizacao> request)
        {
            var retorno = new PostApiRetorno();
            try
            {                
                using (Conexao<Elemento_Rede> repoElemento = new Conexao<Elemento_Rede>(sessionT5))
                using (Conexao<Elemento_Rede_Porta> repoElementoPorta = new Conexao<Elemento_Rede_Porta>(sessionT5))
                using (Conexao<Documento_ProducaoAniel> repoDocumento = new Conexao<Documento_ProducaoAniel>(sessionT5))
                using (Conexao<LoginRadius> repoLogin = new Conexao<LoginRadius>(session))
                {
                    IIntegracaoExternaRepository repoIntegracaoExterna = new IntegracaoExternaRepository(session);
                    var integracaoExternaBusiness = new IntegracaoExternaBusiness(repoIntegracaoExterna);
                    IntegracoesExternas integracaoExterna =
                    integracaoExternaBusiness.GetIntegracaoExterna(EnumTipoIntegracao.OUTROS, IntegracaoExterna.VOALLE);

                    if(integracaoExterna == null)                    
                        throw new Exception("Não foi encontrada Integração Externa configurada para o Voalle.");
                    
                    var stringConnectionVoalle = CONSTANTES.VoalleConnectionString;

                    if(string.IsNullOrEmpty(stringConnectionVoalle))
                        throw new Exception("String de conexão do Voalle não configurada no CONSTANTES.");

                    IVoalleRepository repoVoalle = new VoalleApiRepository(integracaoExterna);
                    var voalleBusiness = new ConnectBusiness.MK.VoalleBusiness(null, repoVoalle);

                    Elemento_Rede_Porta porta = request.Obj.IdCaixa.HasValue && request.Obj.numeroPorta.HasValue ? await repoElementoPorta.selectFirst(p => p.Caixa.Id == request.Obj.IdCaixa && p.Porta == request.Obj.numeroPorta) : null;
                    LoginRadius loginAnterior = porta != null ? await repoLogin.selectFirst(p => p.Id_Caixa_FTTH == request.Obj.IdCaixa && p.Id_Porta_FTTH == porta.Id && p.Id != request.Obj.IdLogin) : null;

                    // Se o login anterior não é nulo, anular caixa e porta para o login para então atualizar os objetos de Elemento de Rede e Porta
                    if (loginAnterior != null)
                    {
                        loginAnterior.Id_Caixa_FTTH = null;
                        loginAnterior.Id_Porta_FTTH = null;

                        var loginAnteriorVoalle = voalleBusiness.GetLoginVoalle(stringConnectionVoalle, loginAnterior.CodigoAuxiliar);

                        //TODO: O Id da Caixa importada do Geogrid corresponde ao authenticationSplitterId do Voalle?
                        bool limparLoginAnteriorVoalle = loginAnteriorVoalle != null && porta != null && loginAnteriorVoalle.authenticationSplitterId == porta?.Caixa?.Id && loginAnteriorVoalle.port == porta?.Porta;

                        if (limparLoginAnteriorVoalle)
                        {
                            loginAnteriorVoalle.authenticationSplitterId = null;
                            loginAnteriorVoalle.port = null;
                            
                            var limpezaCaixaPortaAnterior = voalleBusiness.LimparCaixaPorta(loginAnteriorVoalle, LogManager.GetLogger("Aniel.Connect.API_Aniel.Voalle"));

                            if (limpezaCaixaPortaAnterior.success == false)
                            {
                                throw new Exception($"Erro ao limpar Caixa e Porta do Login anterior. Response Voalle: {JsonService.SerializeObject(limpezaCaixaPortaAnterior)}");
                            }

                        }
                        await repoLogin.SaveOrUpdate(loginAnterior);
                    }

                    LoginRadius login = await repoLogin.selectFirst(p => p.Id == request.Obj.IdLogin);

                    if (login != null)
                    {
                        //Ocupação de Caixa/Porta
                        if (request.Obj.IdCaixa.HasValue && request.Obj.numeroPorta.HasValue)
                        {
                            login.Id_Caixa_FTTH = porta?.Caixa?.Id;
                            login.Id_Porta_FTTH = porta?.Id;

                            if (string.IsNullOrEmpty(login.CodigoAuxiliar))
                                throw new Exception($"Login não possui Cod. Auxiliar no Connect. Id Login {login.Id} | Login PPPoE: {login.Login}");

                            var loginVoalle = voalleBusiness.GetLoginVoalle(stringConnectionVoalle, login.CodigoAuxiliar);

                            if (loginVoalle == null)
                                throw new Exception($"Não foi possível localizar login no Voalle. Cod. Auxiliar do login no Connect: {login.CodigoAuxiliar}");

                            //Passando Id da Caixa do Voalle nulo e buscando pelo Id da Caixa no Geogrid
                            var caixaVoalle = voalleBusiness.GetCaixaPortaVoalle(stringConnectionVoalle, idCaixa: null, porta.Porta, idCaixaGeogrid: porta?.Caixa?.Id);

                            if (caixaVoalle == null)
                                throw new Exception($"Não foi possível localizar CTO no Voalle. Id da Caixa no Geogrid: {porta?.Caixa?.Id}");

                            loginVoalle.authenticationSplitterId = caixaVoalle.id;
                            loginVoalle.port = porta.Porta;                  

                            var atualizacaoCaixaPorta = voalleBusiness.ReservarCaixaPorta(loginVoalle, LogManager.GetLogger("Aniel.Connect.API_Aniel.Voalle"));
                            if (atualizacaoCaixaPorta.success == false)
                            {
                                throw new Exception($"Erro ao atualizar Caixa e Porta. Response Voalle: {JsonService.SerializeObject(atualizacaoCaixaPorta)}");
                            }
                        }
                        //Limpeza de Caixa/Porta
                        else
                        {
                            login.Id_Caixa_FTTH = null;
                            login.Id_Porta_FTTH = null;

                            if (string.IsNullOrEmpty(login.CodigoAuxiliar))
                                throw new Exception($"Login não possui Cod. Auxiliar no Connect. Id Login {login.Id} | Login PPPoE: {login.Login}");

                            var loginVoalle = voalleBusiness.GetLoginVoalle(stringConnectionVoalle, login.CodigoAuxiliar);

                            if (loginVoalle == null)
                                throw new Exception($"Não foi possível localizar login no Voalle. Cod. Auxiliar do login no Connect: {login.CodigoAuxiliar}");

                            loginVoalle.authenticationSplitterId = null;
                            loginVoalle.port = null;                            

                            var limpezaCaixaPorta = voalleBusiness.LimparCaixaPorta(loginVoalle, LogManager.GetLogger("Aniel.Connect.API_Aniel.Voalle"));

                            if (limpezaCaixaPorta.success == false)
                            {
                                throw new Exception($"Erro ao limpar Caixa e Porta do Login. Response Voalle: {JsonService.SerializeObject(limpezaCaixaPorta)}");
                            }
                        }

                        await repoLogin.SaveOrUpdate(login);
                    }
                    else
                    {
                        throw new Exception($"Login Radius não encontrado.");
                    }

                    retorno.Ok = true;
                }

            }
            catch (Exception exception)
            {
                CONSTANTES.log4Net.Error(exception.Message, exception);
                retorno.Ok = false;
                retorno.Mensagem = exception.Message;
                retorno.InnerException = exception.InnerException?.Message;
            }

            return retorno;
        }

        [HttpGet()]
        [Route("api/Login_RadiusApi/FillDictionaryLogin")]
        private void FillDictionaryLogin(LoginRadiusApiModel login, Dictionary<String, object> parametros)
        {
            parametros.Add("@ID", login.Id);
            parametros.Add("@LOGIN", StringService.Truncate(login.Login, 255));
            parametros.Add("@STATUS", login.Status);
            parametros.Add("@ENDERECO_IPV4", StringService.Truncate(login.EnderecoIPV4, 255));
            parametros.Add("@ENDERECO_IPV6", StringService.Truncate(login.EnderecoIPV6, 255));
            parametros.Add("@ID_CAIXA_FTTH", login.Id_Caixa_FTTH.HasValue ? StringService.Truncate(Convert.ToString(login.Id_Caixa_FTTH), 19) : null);
            parametros.Add("@ID_PORTA_FTTH", login.Id_Porta_FTTH.HasValue ? StringService.Truncate(Convert.ToString(login.Id_Porta_FTTH), 19) : null);
            parametros.Add("@ID_CLIENTE", login.Id_Cliente);

        }

        /// <summary>
        /// Busca os dados do Login
        /// </summary>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/Login_RadiusApi/BuscarDadosLogin")]
        public async Task<GetApiRetorno<List<DadosLoginResponseModel>>> BuscarDadosLogin(ObjApiRequest<DadosLoginRequestModel> request)
        {
            var retorno = new GetApiRetorno<List<DadosLoginResponseModel>>();
            retorno.Objeto = new List<DadosLoginResponseModel>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (Conexao<Elemento_Rede_Porta> repoPorta = new Conexao<Elemento_Rede_Porta>(sessionT5))
                using (Conexao<LoginRadius> repoLogin = new Conexao<LoginRadius>(session))
                using (Conexao<Cliente> repoCliente = new Conexao<Cliente>(session))
                {

                    Elemento_Rede_Porta portaEncontrada = null;

                    if (request.Obj.NumeroPorta.HasValue && request.Obj.IdCaixa.HasValue)
                    {
                        portaEncontrada = await repoPorta.selectFirst(p => p.Caixa.Id == request.Obj.IdCaixa && p.Porta == request.Obj.NumeroPorta);
                    }

                    if (portaEncontrada != null)
                    {
                        var login = await repoLogin.selectFirst(p => p.Id_Caixa_FTTH == request.Obj.IdCaixa && p.Id_Porta_FTTH == portaEncontrada.Id);
                        retorno.Objeto.Add(new DadosLoginResponseModel
                        {
                            CpfCliente = login?.Cliente?.CpfCnpj,
                            IdCaixa = portaEncontrada?.Caixa?.Id,
                            NomeCliente = login?.Cliente?.NomeFantasia,
                            NumeroPorta = portaEncontrada.Porta,
                            DescricaoCaixa = portaEncontrada?.Caixa?.Descricao,
                            Login = login?.Login,
                            IdPorta = portaEncontrada.Id,
                            IdCliente = login?.Cliente?.Id,
                            IdLogin = login?.Id,
                        });
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(request.Obj.Cliente) == false)
                        {
                            List<Cliente> clientes = new List<Cliente>();
                            if (request.Obj.Cliente.Any(x => !char.IsLetter(x) && !char.IsWhiteSpace(x)))
                            {
                                var cpf = StringService.Truncate(request.Obj.Cliente.Replace(".", String.Empty).Replace("-", String.Empty).Replace("/", String.Empty), 18);
                                clientes = (await repoCliente.select(p => p.CpfCnpj.IsLike(cpf, MatchMode.Anywhere))).ToList();
                            }
                            else
                            {
                                clientes = (await repoCliente.select(p => p.NomeFantasia.IsInsensitiveLike(request.Obj.Cliente, MatchMode.Anywhere))).ToList();
                            }

                            var logins = await repoLogin.select(p => p.Cliente.Id.IsIn(clientes.Select(p => p.Id).ToList()) || p.Login.IsInsensitiveLike(request.Obj.Cliente, MatchMode.Anywhere));


                            foreach (var login in logins)
                            {
                                portaEncontrada = null;
                                portaEncontrada = await repoPorta.selectFirst(p => p.Caixa.Id == login.Id_Caixa_FTTH && p.Id == login.Id_Porta_FTTH);
                                retorno.Objeto.Add(new DadosLoginResponseModel
                                {
                                    CpfCliente = login?.Cliente?.CpfCnpj,
                                    IdCaixa = login?.Id_Caixa_FTTH,
                                    NomeCliente = login.Cliente?.NomeFantasia,
                                    DescricaoCaixa = portaEncontrada?.Caixa?.Descricao,
                                    Login = login?.Login,
                                    NumeroPorta = portaEncontrada?.Porta,
                                    IdPorta = login?.Id_Porta_FTTH,
                                    IdCliente = login?.Cliente?.Id,
                                    IdLogin = login?.Id,
                                });

                            }


                        }
                        else if (request.Obj.IdLogin.HasValue)
                        {
                            var login = await repoLogin.selectFirst(p => p.Id == request.Obj.IdLogin);


                            portaEncontrada = null;
                            portaEncontrada = await repoPorta.selectFirst(p => p.Caixa.Id == login.Id_Caixa_FTTH && p.Id == login.Id_Porta_FTTH);
                            retorno.Objeto.Add(new DadosLoginResponseModel
                            {
                                CpfCliente = login?.Cliente?.CpfCnpj,
                                IdCaixa = login?.Id_Caixa_FTTH,
                                NomeCliente = login.Cliente?.NomeFantasia,
                                DescricaoCaixa = portaEncontrada?.Caixa?.Descricao,
                                Login = login?.Login,
                                NumeroPorta = portaEncontrada?.Porta,
                                IdPorta = login?.Id_Porta_FTTH,
                                IdCliente = login?.Cliente?.Id,
                                IdLogin = login?.Id,
                            });
                        }

                    }

                }

                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                retorno.InnerException = ex.InnerException?.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }

        /// <summary>
        /// Verifica se há clientes vinculados a caixa e porta no IXC
        /// </summary>
        /// <returns></returns>
        [HttpPost()]
        [Route("api/Login_RadiusApi/VerificarClienteExistente")]
        public async Task<GetApiRetorno<List<DadosCaixaResponseModel>>> VerificarClienteExistente(ObjApiRequest<DadosCaixaRequestModel> request)
        {
            var retorno = new GetApiRetorno<List<DadosCaixaResponseModel>>();
            retorno.Objeto = new List<DadosCaixaResponseModel>();

            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (DbCommand comT5 = sessionT5.Connection.CreateCommand())
                {
                    Config_Aniel_Integrator Config = new Config_Aniel_Integrator();
                    StringBuilder sql = new StringBuilder();
                    Dictionary<String, Object> parametros = new Dictionary<String, Object>();

                    //Capturar configurações da Integração

                    sql.AppendLine($"SELECT * ");
                    sql.AppendLine(" FROM TB_CONFIG_ANIEL_INTEGRATOR ");

                    using (var reader = await CommandHelper.ExecuteReaderFirst(comT5, sql, parametros, sessionT5.Connection, null, 1, null))
                    {
                        if (await reader.ReadAsync())
                        {

                            Config.URL = Convert.ToString(reader["URL"]);
                            Config.UsuarioAniel = Convert.ToString(reader["USUARIO_ANIEL"]);
                            Config.CodCtPadrao = Convert.ToString(reader["CODCT_PADRAO"]);
                            Config.ProjetoPadrao = Convert.ToString(reader["PROJETO_PADRAO"]);
                            Config.Unidade_Custo = Convert.ToString(reader["UNIDADE_CUSTO"]);
                            Config.Token = Convert.ToString(reader["TOKEN"]);
                            Config.Setores = Convert.ToString(reader["SETORES"]);

                        }

                    }

                    List<DadosCaixaResponseModel> registroIXC = new LoginIXCHelper().GetClienteFibraByCaixa(request.Obj.IdCaixa, Config).registros ?? new List<DadosCaixaResponseModel>();

                    retorno.Objeto = registroIXC;
                }

                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                retorno.InnerException = ex.InnerException?.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Login_RadiusApi/BuscarDadosOS")]
        public async Task<GetApiRetorno<DadosOSResponseModel>> BuscarDadosOS(ObjApiRequest<DadosOSModel> request)
        {
            var retorno = new GetApiRetorno<DadosOSResponseModel>();
            retorno.Objeto = new DadosOSResponseModel();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (Conexao<Documento_ProducaoAniel> repoDocProd = new Conexao<Documento_ProducaoAniel>(sessionT5))
                using (Conexao<OLT> repoOLT = new Conexao<OLT>(session))
                using (Conexao<POP> repoPOP = new Conexao<POP>(session))
                using (Conexao<Cliente_PlanoProduto> repoPlanoProd = new Conexao<Cliente_PlanoProduto>(session))
                using (Conexao<Plano> repoPlano = new Conexao<Plano>(session))
                {

                    var documentoProducao = await repoDocProd.selectFirst(p => p.CodCt == request.Obj.codct
                                        && p.Num_Obra == request.Obj.numObra
                                        && p.Projeto == request.Obj.projeto
                                        && p.Num_Doc == request.Obj.numdoc);

                    if (documentoProducao.Id_Olt.HasValue)
                    {
                        retorno.Objeto.Olt = repoOLT.selectFirst(p => p.Id == documentoProducao.Id_Olt).Result.Descricao;
                    }
                    else
                    {
                        retorno.Objeto.Olt = "-";
                    }

                    if (documentoProducao.Id_Pop.HasValue)
                    {
                        retorno.Objeto.Pop = repoPOP.selectFirst(p => p.Id == documentoProducao.Id_Pop).Result.Nome;
                    }
                    else
                    {
                        retorno.Objeto.Pop = "-";
                    }

                    if (documentoProducao.Id_Cliente_PlanoProdutoConnect.HasValue)
                    {
                        retorno.Objeto.Plano = repoPlanoProd.selectFirst(p => p.Id == documentoProducao.Id_Cliente_PlanoProdutoConnect).Result.Plano.Descricao;
                    }
                    else
                    {
                        retorno.Objeto.Plano = "-";
                    }
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