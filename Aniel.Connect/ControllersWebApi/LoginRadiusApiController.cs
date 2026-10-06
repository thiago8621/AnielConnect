using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.ControllersWebApi.Ativacao.Repositorio;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.ModelLog.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.T5.ElementoRedePorta;
using Microsoft.AspNet.Identity;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Integracao.Ativacao.Concentradoras;
using Sinapse.Integracao.Ativacao.Models;
using Sinapse.Integracao.Ativacao.Radius;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;


namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class LoginRadiusApiController : BaseApiController
    {
        [HttpPost()]
        [Route("api/LoginRadiusapi/GetAll")]
        public async Task<DataTableResponse<LoginRadiusListaModel>> GetAll(LoginRadiusDatatableRequest request)
        {
            List<LoginRadiusListaModel> retorno = new List<LoginRadiusListaModel>();
            IOrderedEnumerable<LoginRadiusListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sqlFrom = new StringBuilder();
                sqlFrom.AppendLine(" FROM TB_LOGIN_RADIUS A ");
                sqlFrom.AppendLine(" LEFT JOIN TB_OLT B ON (B.ID = A.ID_OLT) ");
                sqlFrom.AppendLine(" LEFT JOIN TB_PON D ON (D.ID = A.ID_PON) ");
                sqlFrom.AppendLine(" LEFT JOIN TB_SLOT E ON (E.ID = A.ID_SLOT) ");
                sqlFrom.AppendLine(" LEFT JOIN TB_POP F ON (F.ID = A.ID_POP) ");
                sqlFrom.AppendLine(" LEFT JOIN TB_CLIENTE_PLANO_PRODUTO G ON (A.ID_PLANO_PRODUTO = G.ID) ");
                sqlFrom.AppendLine(" LEFT JOIN TB_CLIENTE H ON (G.ID_CLIENTE = H.ID) ");


                StringBuilder sqlWhere = new StringBuilder();
                sqlWhere.AppendLine(" WHERE A.STATUS IN (0,1)");
                bool condicoes = false;
                //Filtrando
                if (!String.IsNullOrEmpty(request.search.value))
                {
                    sqlWhere.AppendLine(" AND UPPER(A.LOGIN) LIKE UPPER(@SEARCH) OR UPPER(H.NOMEFANTASIA) LIKE UPPER(@SEARCH)");
                    parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    condicoes = true;
                }
                if (!String.IsNullOrEmpty(request.Id_Olt))
                {
                    sqlWhere.AppendLine(" AND (A.ID_OLT = @ID_OLT) ");
                    parametros.Add("@ID_OLT", Convert.ToInt32(request.Id_Olt));
                    condicoes = true;
                }

                if (!String.IsNullOrEmpty(request.Id_Cliente))
                {
                    sqlWhere.AppendLine(" AND (H.ID = @CLIENTE) ");
                    parametros.Add("@CLIENTE", Convert.ToInt32(request.Id_Cliente));
                    condicoes = true;
                }

                if (!String.IsNullOrEmpty(request.Id_ClientePlanoProduto))
                {
                    sqlWhere.AppendLine(" AND (G.ID = @PLANOPRODUTO) ");
                    parametros.Add("@PLANOPRODUTO", Convert.ToInt32(request.Id_ClientePlanoProduto));
                    condicoes = true;
                }

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO ");
                    sql.Append(sqlFrom);
                    if (condicoes)
                        sql.Append(sqlWhere);

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.ID, A.LOGIN, A.ID_CAIXA_FTTH, A.ID_PORTA_FTTH, A.STATUS, ");
                    sql.AppendLine("        B.DESCRICAO AS DESCRICAO_OLT, D.DESCRICAO AS DESCRICAO_PON, ");
                    sql.AppendLine("        E.DESCRICAO AS DESCRICAO_SLOT, F.NOME AS DESCRICAO_POP, A.ID_PLANO_PRODUTO, G.ID_CLIENTE_CONTRATO, H.NOMEFANTASIA AS CLIENTE");
                    sql.Append(sqlFrom);
                    if (condicoes)
                        sql.Append(sqlWhere);

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new LoginRadiusListaModel
                            {
                                id = Convert.ToInt64(reader["ID"]),
                                porta_FTTH = String.Empty,
                                caixa_FTTH = String.Empty,
                                olt = Convert.ToString(reader["DESCRICAO_OLT"]),
                                pon = Convert.ToString(reader["DESCRICAO_PON"]),
                                pop = Convert.ToString(reader["DESCRICAO_POP"]),
                                slot = Convert.ToString(reader["DESCRICAO_SLOT"]),
                                status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"])),                                
                                login = Convert.ToString(reader["LOGIN"]),
                                idClientePlanoProduto = reader["ID_PLANO_PRODUTO"] != DBNull.Value ? Convert.ToInt64(reader["ID_PLANO_PRODUTO"]) : (long?)null,
                                idClienteContrato = reader["ID_CLIENTE_CONTRATO"] != DBNull.Value ? Convert.ToInt64(reader["ID_CLIENTE_CONTRATO"]) : (long?)null,
                                cliente = Convert.ToString(reader["CLIENTE"]),
                            });
                        }
                    }
                }
            }

            //Ordenacao
            if (request.order != null && request.order.Any())
            {
                int sortColumnIndex = request.order[0].column;
                string sortDirection = request.order[0].dir;
                switch (sortColumnIndex)
                {
                    case 1:     // CODIGO
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.status);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.status);
                            break;
                        }
                    case 2:     // DESCIRCAO
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.login);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.login);
                            break;
                        }
                    case 3:     // POP
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.pop);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.pop);
                            break;
                        }
                    case 4:     // OLT
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.olt);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.olt);
                            break;
                        }
                    case 5:     // SLOT
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.slot);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.slot);
                            break;
                        }
                    case 6:     // PON
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.pon);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.pon);
                            break;
                        }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<LoginRadiusListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        public LoginRadiusCadastroModel GetViewObj(Model.nhibernate.LoginRadius obj, ONU objOnu, bool ignorarCriticaCaixaPorta = false)
        {
            return new LoginRadiusCadastroModel
            {
                Id = obj.Id,
                Login = obj.Login,
                Tipo_Autenticacao_Login = obj.Tipo_Autenticacao_Login,
                EnderecoIPV4 = obj.EnderecoIPV4,
                EnderecoIPV6 = obj.EnderecoIPV6,
                Id_Caixa_FTTH = obj.Id_Caixa_FTTH,
                Id_Porta_FTTH = obj.Id_Porta_FTTH,
                Id_Cliente = obj.Cliente?.Id,
                Id_Concentradora = obj.Concentradora?.Id,
                Id_OLT = objOnu?.OLT?.Id,
                Id_PlanoProduto = obj.Cliente_PlanoProduto?.Id,
                Id_Slot = objOnu?.Slot?.Id,
                Id_PON = objOnu?.PON?.Id,
                Id_POP = obj.POP?.Id,
                UsuarioRoteador = obj.UsuarioRoteador,
                MAC = obj.MAC,
                SenhaRoteador = obj.SenhaRoteador,
                AgentCircuit_ID = obj.Agent_Circuit_Id,
                Atributo_IPOE = obj.Atributo_IPOE?.Id,
                SenhaPPPOE = obj.SenhaPPPOE,
                Senha_WIFI = obj.Senha_WIFI,
                Serial = obj.Serial,
                SSID_WIFI = obj.SSID_WIFI,
                VLAN = obj.VLAN,
                Status = obj.Status == Status_Default.Ativo ? true : false,
                Id_Pool = obj.Cliente_PlanoProduto?.Pool_Ip?.Id,
                Id_Pool_Ip_Endereco = obj.Pool_Ip_Endereco?.Id ?? 0,
                Observacoes = obj.Observacoes,
                IgnorarCriticaCaixaPorta = ignorarCriticaCaixaPorta
            };
        }

        private async Task BindViewToObj(ISession session, Model.nhibernate.LoginRadius objBanco, LoginRadiusCadastroModel view)
        {
            using (var repository = new Conexao<Model.nhibernate.LoginRadius>(session))
            using (var repositoryCliente = new Conexao<Cliente>(session))
            using (var repositoryConcentradora = new Conexao<Concentradora>(session))
            using (var repositoryClientePlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
            using (var repositoryPOP = new Conexao<POP>(session))
            using (var repositoryIPOE = new Conexao<Atributo_IPOE>(session))
            using (var repositoryPoolIpEndereco = new Conexao<Model.nhibernate.Pool_Ip_Endereco>(session))
            {
                if (objBanco.Id == 0)
                {
                    var loginExiste = repository.selectFirst(p => p.Login == view.Login).Result != null;

                    if (loginExiste)
                        throw new Exception("O login informado já foi cadastrado");
                    else
                        objBanco.Login = view.Login;
                }
                else
                {
                    objBanco.Login = view.Login;
                }

                objBanco.SenhaPPPOE = view.SenhaPPPOE;

                objBanco.Cliente = view.Id_Cliente.HasValue ? await repositoryCliente.selectFirst(p => p.Id == view.Id_Cliente) : null;
                objBanco.Cliente_PlanoProduto = view.Id_PlanoProduto.HasValue ? await repositoryClientePlanoProduto.selectFirst(p => p.Id == view.Id_PlanoProduto) : null;

                objBanco.POP = view.Id_POP.HasValue ? await repositoryPOP.selectFirst(p => p.Id == view.Id_POP) : null;

                objBanco.Concentradora = view.Id_Concentradora.HasValue ? await repositoryConcentradora.selectFirst(p => p.Id == view.Id_Concentradora) : null;

                objBanco.Id_Caixa_FTTH = view.Id_Caixa_FTTH;
                objBanco.Id_Porta_FTTH = view.Id_Porta_FTTH;

                objBanco.Tipo_Autenticacao_Login = view.Tipo_Autenticacao_Login;

                //objBanco.EnderecoIPV4 = view.EnderecoIPV4;
                //objBanco.EnderecoIPV6 = view.EnderecoIPV6;

                objBanco.MAC = view.MAC;

                objBanco.UsuarioRoteador = view.UsuarioRoteador;
                objBanco.SenhaRoteador = view.SenhaRoteador;

                objBanco.Senha_WIFI = view.Senha_WIFI;
                objBanco.SSID_WIFI = view.SSID_WIFI;

                objBanco.Serial = view.Serial;
                objBanco.VLAN = view.VLAN;
                objBanco.Atributo_IPOE = view.Atributo_IPOE.HasValue ? await repositoryIPOE.selectFirst(p => p.Id == view.Atributo_IPOE) : null;
                objBanco.Agent_Circuit_Id = view.AgentCircuit_ID;

                var Endereco_Ip = view.Id_Pool_Ip_Endereco.HasValue ? await repositoryPoolIpEndereco.selectFirst(p => p.Id == view.Id_Pool_Ip_Endereco.Value) : null;
                if (Endereco_Ip != null)
                {
                    objBanco.Pool_Ip_Endereco = Endereco_Ip;
                    if (Endereco_Ip.Pool_Ip.Padrao_Ip == Tipo_Ip.IPv4)
                        objBanco.EnderecoIPV4 = Endereco_Ip.Ip;
                    else
                        objBanco.EnderecoIPV6 = Endereco_Ip.Ip;
                }
                objBanco.Observacoes = view.Observacoes;

            }
        }

        [HttpPost()]
        [Route("api/LoginRadiusapi/AlterarStatus")]
        public async Task<PostApiRetorno<Int64>> AlterarStatus(LoginRadiusAlterarStatus value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoClientePlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
                using (var repository = new Conexao<Model.nhibernate.LoginRadius>(session))
                using (var api = new IntegracaoRadiusApiController())
                {
                    foreach (var loginPlanoProduto in value.ListLoginPlanoProduto)
                    {
                        var apiRadiusParam = new PlanoProdutoRadiusRequest
                        {
                            IdClientePlanoProduto = loginPlanoProduto.idPlanoProduto
                        };

                        var planoProduto = await repoClientePlanoProduto.selectFirst(p => p.Id == loginPlanoProduto.idPlanoProduto);
                        if(planoProduto == null)
                        {
                            throw new Exception("Não foi encontrado Plano Produto vinculado ao login.");
                        }

                        if (value.Status == Status_Default.Inativo)
                        {
                            await api.BloquearUsuario(apiRadiusParam);
                        }
                        else
                        {
                            if(planoProduto.Status.HasValue)
                            {
                                if (planoProduto.Status.Value != Status_PlanoProduto.Aguardando_Criacao_OS && planoProduto.Status.Value != Status_PlanoProduto.Habilitado)
                                {
                                    throw new Exception("O plano não está habilitado. Por favor, habilite o plano do cliente e tente novamente.");
                                }
                            }
                            
                            await api.DesbloquearUsuario(apiRadiusParam);
                        }
                           
                        var login = await repository.selectFirst(p => p.Id == loginPlanoProduto.idLogin);

                        login.DataHoraAlteracao = DateTime.Now;
                        login.Status = value.Status;

                        await repository.SaveOrUpdate(login);
                    }

                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;

                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/LoginRadiusapi/excluirlista")]
        public async Task<PostApiRetorno<Int64>> ExcluirLista(ExcluirListaViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        String _ids = $"({String.Join(",", value.ids)})";

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_LOGIN_RADIUS WHERE ID IN {_ids}");

                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, null);

                        retorno.Ok = true;
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
            }

            return retorno;
        }

        [HttpGet()]
        [Route("api/LoginRadiusapi/FindById")]
        public async Task<GetApiRetorno<LoginRadiusCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<LoginRadiusCadastroModel> retorno = new GetApiRetorno<LoginRadiusCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Model.nhibernate.LoginRadius>(session))
            using (var repoOnu = new Conexao<ONU>(session))
            {
                try
                {
                    var loginRadius = await repository.selectFirst(p => p.Id == Id);
                    var onu = await repoOnu.selectFirst(p => p.LoginRadius.Id == loginRadius.Id);
                    retorno.Objeto = GetViewObj(loginRadius, onu);
                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;
        }


        [HttpGet]
        [Route("api/LoginRadiusApi/ExisteLogin")]
        public async Task<GetApiRetorno<Boolean>> ExisteLogin(String login)
        {
            var retorno = new GetApiRetorno<Boolean>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();
                    Dictionary<string, object> param = new Dictionary<string, object>();

                    sql.AppendLine("SELECT COUNT(ID) AS QTD");
                    sql.AppendLine("FROM TB_LOGIN_RADIUS");
                    sql.AppendLine("WHERE LOGIN = @LOGIN");

                    param.Add("@LOGIN", login);

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, param, session.Connection, null))
                    {
                        if (reader.Read())
                        {
                            if (Convert.ToInt32(reader["QTD"]) == 0)
                                retorno.Objeto = false;
                            else
                                retorno.Objeto = true;
                        }
                    }
                }

                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/LoginRadiusapi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<LoginRadiusCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Model.nhibernate.LoginRadius modelo = null;
                Model.nhibernate.LoginRadius modeloAnterior = null;
                Model.nhibernate.Pool_Ip poolAnterior = null;
                string loginAntigo = null;
                bool planoUsaRadius = false;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Model.nhibernate.LoginRadius>(session))
                    using (var repoClientePlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
                    using (var repoPoolIp = new Conexao<Model.nhibernate.Pool_Ip>(session))
                    using (var repoPoolIpEndereco = new Conexao<Model.nhibernate.Pool_Ip_Endereco>(session))
                    using (var configuracoesGerais = new Conexao<Configuracoes_Gerais>(session))
                    {
                        bool statusAlterado = false;
                        bool atualizarFTTH = false;
                        var statusView = value.Objeto.Status ? Status_Default.Ativo : Status_Default.Inativo;

                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();


                        if (modelo == null)
                        {
                            modelo = new Model.nhibernate.LoginRadius
                            {
                                //Status = Status_Default.Ativo,
                                UsuarioCriacao = value.Usuario
                            };
                        }
                        else
                        {
                            modeloAnterior = (Model.nhibernate.LoginRadius)modelo.Clone();

                            statusAlterado = statusView != modelo.Status;

                            modelo.Status = statusView;
                            modelo.UsuarioAlteracao = value.Usuario;

                            loginAntigo = modelo.Login;
                        }

                        var novoRegistro = modelo.Id == 0;
                        var obrigaCaixaPorta = await configuracoesGerais.selectFirst();

                  
                        

                        //Validações de campos essenciais para o cadastro do login

                        var count = repository.Count(p => p.Login == value.Objeto.Login && p.Id != value.Objeto.Id).Result;
                        if (count > 0)
                            throw new Exception("Login já cadastrado para outro cliente e plano. Por favor informar outro login que ainda não tenha sido utilizado.");

                        if (String.IsNullOrEmpty(value.Objeto.Login))
                        {
                            throw new Exception("Login não informado. Por favor, informe o login e tente novamente.");
                        }

                        if (novoRegistro && value.Objeto.Id_PlanoProduto.HasValue)
                        {
                            var countMesmoPlano = repository.Count(p => p.Cliente_PlanoProduto.Id == value.Objeto.Id_PlanoProduto).Result;
                            if (countMesmoPlano > 0)
                                throw new Exception("Já existe um Login vinculado ao Plano informado.");
                        }

                        /*if(value.Objeto.Login.Any(char.IsUpper))
                        {
                            throw new Exception("Não são permitidas letras maiúsculas no login. Por favor, utilize letras minúsculas.");
                        }*/

                        if (String.IsNullOrEmpty(value.Objeto.SenhaPPPOE))
                        {
                            throw new Exception("Senha não informada. Por favor, informe a senha e tente novamente.");
                        }

                        /* if (value.Objeto.SenhaPPPOE.Any(char.IsUpper))
                         {
                             throw new Exception("Não são permitidas letras maiúsculas na senha. Por favor, utilize letras minúsculas.");
                         } */

                        if (!value.Objeto.Id_Cliente.HasValue || value.Objeto.Id_Cliente.Value == 0)
                        {
                            throw new Exception("Cliente não informado. Por favor, informe o cliente e tente novamente.");
                        }

                        if (!value.Objeto.Id_PlanoProduto.HasValue || value.Objeto.Id_PlanoProduto.Value == 0)
                        {
                            throw new Exception("Plano não informado. Por favor, informe o plano e tente novamente.");
                        }

                        if (!value.Objeto.Tipo_Autenticacao_Login.HasValue || value.Objeto.Tipo_Autenticacao_Login.Value == 0)
                        {
                            throw new Exception("Tipo de autenticação não informado. Por favor, informe o tipo de autenticação e tente novamente.");
                        }

                        if (obrigaCaixaPorta.ObrigaCaixaPorta == true && value.Objeto.IgnorarCriticaCaixaPorta == false)
                        {
                            if (!value.Objeto.Id_Caixa_FTTH.HasValue || value.Objeto.Id_Caixa_FTTH == 0)
                            {
                                throw new Exception("Caixa não informada. Por favor, informe a caixa e tente novamente.");
                            }
                            else if (!value.Objeto.Id_Porta_FTTH.HasValue || value.Objeto.Id_Porta_FTTH == 0)
                            {
                                throw new Exception("Porta não informada. Por favor, informe a porta e tente novamente.");
                            }
                        }

                        //Verifica se o plano do cliente utiliza radius. Se sim, irá impedir o cadastro se
                        //o objeto vier sem o dado da concentradora
                        var planoProduto = await repoClientePlanoProduto.selectFirst(p => p.Id == value.Objeto.Id_PlanoProduto);

                        if (planoProduto != null)
                        {
                            //Insere status do login
                            var planoProdutoStatus = planoProduto.Status.Value;

                            if (novoRegistro)
                            {
                                if (planoProdutoStatus == Status_PlanoProduto.Aguardando_Criacao_OS || planoProdutoStatus == Status_PlanoProduto.Habilitado)
                                {
                                    modelo.Status = Status_Default.Ativo;
                                }
                                else
                                {
                                    modelo.Status = Status_Default.Inativo;
                                }
                            }

                            planoUsaRadius = planoProduto.Plano.AutenticacaoRadius;

                            if (planoUsaRadius == true)
                            {
                                if (!value.Objeto.Id_Concentradora.HasValue || value.Objeto.Id_Concentradora.Value == 0)
                                {
                                    throw new Exception("Concentradora não informada. Por favor, informe a concentradora e tente novamente.");
                                }

                                if (!value.Objeto.Id_Pool.HasValue || value.Objeto.Id_Pool.Value == 0)
                                {
                                    throw new Exception("Pool de IPs não informado. Por favor, informe o Pool de IPs e tente novamente.");
                                }
                            }
                        }


                        if (value.Objeto.Id_Pool.HasValue)
                        {
                            planoProduto = await repoClientePlanoProduto.selectFirst(p => p.Id == value.Objeto.Id_PlanoProduto);
                            poolAnterior = planoProduto.Pool_Ip;
                            planoProduto.Pool_Ip = await repoPoolIp.selectFirst(p => p.Id == value.Objeto.Id_Pool);

                            await repoClientePlanoProduto.SaveOrUpdate(planoProduto);

                            if (planoProduto.Pool_Ip.IsPoolIpFixo)
                            {
                                if (!value.Objeto.Id_Pool_Ip_Endereco.HasValue || value.Objeto.Id_Pool_Ip_Endereco.Value == 0)
                                {
                                    throw new Exception("Endereço IP não informado. Por favor, informe o Endereço IP e tente novamente.");
                                }
                            }
                        }


                        Model.nhibernate.Pool_Ip_Endereco Endereco_IP_Antigo = null;
                        if (modelo.Pool_Ip_Endereco != null)
                        {
                            Endereco_IP_Antigo = modelo.Pool_Ip_Endereco;
                        }

                        if (modelo.Id_Caixa_FTTH != value.Objeto.Id_Caixa_FTTH || modelo.Id_Porta_FTTH != value.Objeto.Id_Porta_FTTH)
                        {
                            atualizarFTTH = true;
                        }

                        await BindViewToObj(session, modelo, value.Objeto);

                        await repository.SaveOrUpdate(modelo);

                        if (value.Objeto.Id_Pool_Ip_Endereco.HasValue && value.Objeto.Id_Pool_Ip_Endereco.Value != 0) // Novo Endereço IP
                        {
                            Model.nhibernate.Pool_Ip_Endereco Endereco_IP = await repoPoolIpEndereco.selectFirst(p => p.Id == value.Objeto.Id_Pool_Ip_Endereco);
                            Endereco_IP.Status_Ip = Model.enumerado.Pool_Ip_Range_StatusEnum.Ocupado;
                            Endereco_IP.Login = modelo;
                            await repoPoolIpEndereco.SaveOrUpdate(Endereco_IP);

                            if (Endereco_IP_Antigo != null && Endereco_IP_Antigo.Id != Endereco_IP.Id) //Libera o IP usado anteriormente
                            {
                                Endereco_IP_Antigo.Status_Ip = Model.enumerado.Pool_Ip_Range_StatusEnum.Disponivel;
                                Endereco_IP_Antigo.Login = null;
                                await repoPoolIpEndereco.SaveOrUpdate(Endereco_IP_Antigo);

                                if (Endereco_IP.Pool_Ip.Padrao_Ip == Tipo_Ip.IPv4)
                                {
                                    modelo.EnderecoIPV4 = Endereco_IP.Ip;
                                }
                                else
                                {
                                    modelo.EnderecoIPV6 = Endereco_IP.Ip;
                                }

                                await repository.SaveOrUpdate(modelo);
                            }
                        }
                        else if (value.Objeto.Id_Pool.HasValue) //Novo Pool IP sem definir Endereço IP específico
                        {
                            if (Endereco_IP_Antigo != null) //Libera o IP usado anteriormente
                            {
                                Endereco_IP_Antigo.Status_Ip = Model.enumerado.Pool_Ip_Range_StatusEnum.Disponivel;
                                Endereco_IP_Antigo.Login = null;
                                await repoPoolIpEndereco.SaveOrUpdate(Endereco_IP_Antigo);
                            }

                            modelo.EnderecoIPV4 = null;
                            modelo.EnderecoIPV6 = null;
                            modelo.Pool_Ip_Endereco = null;
                            await repository.SaveOrUpdate(modelo);
                        }

                        #region Log
                        if (modeloAnterior != null)
                        {
                            using (ISession sessionLog = HibernateUtil.GetSessionLog())
                            using (var repositoryLogs = new Conexao<LogLoginRadius>(sessionLog))
                            {
                                var logs = modelo.GeraLog(modeloAnterior, value.Usuario, User.Identity.GetUserName(), Sinapse.ModelLog.TipoOperLog.Alteracao);
                                logs.AddRange(modelo.GeraLogPool(poolAnterior, value.Usuario, User.Identity.GetUserName(), Sinapse.ModelLog.TipoOperLog.Alteracao));
                                await repositoryLogs.Save(logs);
                            }
                        }
                        else
                        {
                            using (ISession sessionLog = HibernateUtil.GetSessionLog())
                            using (var repositoryLogs = new Conexao<LogLoginRadius>(sessionLog))
                            {
                                var logs = modelo.GeraLog(modeloAnterior, value.Usuario, User.Identity.GetUserName(), Sinapse.ModelLog.TipoOperLog.Insercao);
                                await repositoryLogs.Save(logs);
                            }

                        }
                        #endregion

                        //Realiza as operações no servidor radius
                        //caso o plano utilize radius
                        if (planoUsaRadius == true)
                        {
                            try
                            {
                                using (var api = new IntegracaoRadiusApiController())
                                {
                                    var param = new PlanoProdutoRadiusRequest
                                    {
                                        IdClientePlanoProduto = modelo.Cliente_PlanoProduto.Id,
                                        LoginAntigo = loginAntigo
                                    };

                                    if (novoRegistro)
                                    {
                                        var cadastroUsuario = await api.CadastrarUsuario(param);
                                        if (cadastroUsuario.result == false)
                                        {
                                            throw new Exception("Não foi possível cadastrar os dados do usuário no Radius. Por favor, verifique os dados do cadastro e tente novamente.");
                                        }

                                        var configPlano = await api.ConfigurarPlanoUsuario(param);
                                        if (configPlano.result == false)
                                        {
                                            throw new Exception("Não foi possível configurar o plano do usuário no Radius. Por favor, verifique os dados do cadastro e tente novamente.");
                                        }

                                        var associaPlano = await api.AssociaUsuarioAoPlano(param);
                                        if (associaPlano.result == false)
                                        {
                                            throw new Exception("Não foi possível associar o usuário ao plano no Radius. Por favor, verifique os dados do cadastro e tente novamente.");
                                        }

                                        if (modelo.Status == Status_Default.Inativo)
                                        {
                                            var bloqueiaUsuario = await api.BloquearUsuario(param);
                                            if (bloqueiaUsuario.result == false)
                                            {
                                                throw new Exception("Não foi possível inativar o usuário no Radius. Por favor, verifique os dados do cadastro e tente novamente.");
                                            }

                                        }

                                    }
                                    else
                                    {
                                        var editaUsuario = await api.EditarUsuario(param);
                                        if (editaUsuario.result == false)
                                        {
                                            throw new Exception("Não foi possível editar os dados do usuário no Radius. Por favor, verifique os dados do cadastro e tente novamente.");
                                        }

                                        if (statusAlterado)
                                        {
                                            if (modelo.Status == Status_Default.Inativo)
                                            {

                                                var bloqueiaUsuario = await api.BloquearUsuario(param);
                                                if (bloqueiaUsuario.result == false)
                                                {
                                                    throw new Exception("Não foi possível inativar o usuário no Radius. Por favor, verifique os dados do cadastro e tente novamente.");
                                                }
                                            }
                                            else
                                            {
                                                var planoStatus = planoProduto.Status;
                                                if (planoStatus != Status_PlanoProduto.Aguardando_Criacao_OS && planoStatus != Status_PlanoProduto.Habilitado)
                                                {
                                                    modelo.Status = Status_Default.Inativo;
                                                    await repository.SaveOrUpdate(modelo);
                                                    throw new Exception("O plano não está habilitado. Por favor, habilite o plano do cliente e tente novamente.");
                                                }

                                                var desbloqueiaUsuario = await api.DesbloquearUsuario(param);
                                                if (desbloqueiaUsuario.result == false)
                                                {
                                                    throw new Exception("Não foi possível ativar o usuário no Radius. Por favor, verifique os dados do cadastro e tente novamente.");
                                                }
                                            }


                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                retorno.Mensagens.Add(new PostApiMensagem { Mensagem = ex.Message, Tipo = PostApiMensagemType.Info });
                            }
                        }
                       

                        #region Caixa e Porta FTTH
                        if (atualizarFTTH && modelo.Cliente_PlanoProduto != null)
                        {
                            var requestPorta = new Elemento_Reserva();
                            requestPorta.Cliente_PlanoProduto = modelo.Cliente_PlanoProduto.Id;
                            requestPorta.Id_Porta = modelo.Id_Porta_FTTH;
                            requestPorta.Id_Login = modelo.Id;
                            requestPorta.Login = modelo.Login;

                            if (!modelo.Id_Caixa_FTTH.HasValue || !modelo.Id_Porta_FTTH.HasValue)
                                requestPorta.Reset = true;

                            await new Elemento_Rede_PortaApiController().Atribuir_Porta(requestPorta);
                        }
                        #endregion

                    }
                    retorno.IdObjeto = modelo.Id;
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [Route("api/LoginRadiusapi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody] ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Categoria_Atendimento>(session))
                    {

                        Categoria_Atendimento modelo = null;
                        if (value.Objeto != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value.Objeto)).FirstOrDefault();

                            await repository.Remove(modelo);

                            retorno.IdObjeto = modelo.Id;
                            retorno.Ok = true;
                        }
                        else
                        {
                            throw new Exception(Resources.RsBase.NAOENCONTRADO);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/LoginRadiusapi/GetCombo")]
        public async Task<ObjCombo<Int64>> GetCombo(RequestObjCombo<Int64?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_LOGIN_RADIUS A WHERE ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS = 1");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }


                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.LOGIN) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" ( {String.Join(" AND ", condicoes)} ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) AS CO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.LOGIN ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.LOGIN)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["LOGIN"])
                                };
                                retorno.results.Add(obj);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return retorno;

        }

        [HttpPost()]
        [Route("api/LoginRadiusapi/GetComboVinculoCliente")]
        public async Task<ObjCombo<Int64>> GetComboVinculoCliente(RequestObjComboPai<Int64?, Int64?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_LOGIN_RADIUS A WHERE ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS = 1");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request.idpai.HasValue)
                    {
                        condicoes.Add($"A.ID_CLIENTE = {request.idpai}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.LOGIN) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" ( {String.Join(" AND ", condicoes)} ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) AS CO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.LOGIN ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.LOGIN)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["LOGIN"])
                                };
                                retorno.results.Add(obj);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return retorno;

        }

        [HttpPost()]
        [Route("api/LoginRadiusapi/GetComboVinculoIdPlanoProduto")]
        public async Task<ObjCombo<Int64>> GetComboVinculoIdPlanoProduto(RequestObjComboPai<Int64?, Int64?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_LOGIN_RADIUS A WHERE ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS = 1");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request.idpai.HasValue)
                    {
                        condicoes.Add($"A.ID_PLANO_PRODUTO = {request.idpai}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.LOGIN) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" ( {String.Join(" AND ", condicoes)} ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) AS CO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.LOGIN ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.LOGIN)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["LOGIN"])
                                };
                                retorno.results.Add(obj);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return retorno;

        }

        [HttpGet()]
        [Route("api/LoginRadiusapi/HistoricoConsumoRadius")]
        public async Task<GetApiRetorno<HistoricoConsumoRadiusModel>> HistoricoConsumoRadius(Int64 Id)
        {
            GetApiRetorno<HistoricoConsumoRadiusModel> retorno = new GetApiRetorno<HistoricoConsumoRadiusModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Model.nhibernate.LoginRadius>(session))
            {
                try
                {
                    var login = await repository.selectFirst(p => p.Id == Id);


                    //retorno.Objeto = GetViewObj(await repository.selectFirst(p => p.Id == Id));
                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;
        }

        public async Task<LoginRadiusCadastroModel> FindByLogin(string login)
        {
            LoginRadiusCadastroModel retorno = null;
            Dictionary<string, object> parametros = new Dictionary<string, object>();
            using (var session = HibernateUtil.GetSession())
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT FIRST 1 ID, STATUS, LOGIN, SENHA_PPPOE, ENDERECO_IPV4, ENDERECO_IPV6, USUARIO_ROTEADOR, SENHA_ROTEADOR, ");
                sql.AppendLine(" SSID_WIFI, SENHA_WIFI, VLAN, SERIAL, MAC, OBSERVACOES, TIPO_AUTENTICACAO_LOGIN, ID_CAIXA_FTTH, ID_PORTA_FTTH, ID_ATRIBUTO_IPOE, ");
                sql.AppendLine(" ID_POP, ID_CONCENTRADORA, ID_OLT, ID_CLIENTE, ID_PLANO_PRODUTO, ID_SLOT, ID_PON, ID_POOL_IP_ENDERECO, CODIGO_AUXILIAR");
                sql.AppendLine(" FROM TB_LOGIN_RADIUS ");
                sql.AppendLine(" WHERE LOGIN = @LOGIN ");

                parametros.Add("@LOGIN", login);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        retorno = new LoginRadiusCadastroModel();
                        retorno.Id = Convert.ToInt64(reader["ID"]);
                        retorno.Status = Convert.ToBoolean(reader["STATUS"]);
                        retorno.Login = Convert.ToString(reader["LOGIN"]);
                        retorno.SenhaPPPOE = Convert.ToString(reader["SENHA_PPPOE"]);
                        retorno.EnderecoIPV4 = Convert.ToString(reader["ENDERECO_IPV4"]);
                        retorno.EnderecoIPV6 = Convert.ToString(reader["ENDERECO_IPV6"]);
                        retorno.UsuarioRoteador = Convert.ToString(reader["USUARIO_ROTEADOR"]);
                        retorno.SenhaRoteador = Convert.ToString(reader["SENHA_ROTEADOR"]);
                        retorno.SSID_WIFI = Convert.ToString(reader["SSID_WIFI"]);
                        retorno.Senha_WIFI = Convert.ToString(reader["SENHA_WIFI"]);
                        retorno.VLAN = Convert.ToString(reader["VLAN"]);
                        retorno.Serial = Convert.ToString(reader["SERIAL"]);
                        retorno.MAC = Convert.ToString(reader["MAC"]);
                        retorno.Observacoes = Convert.ToString(reader["OBSERVACOES"]);
                        //retorno.Tipo_Autenticacao_Login = 
                        retorno.Id_Caixa_FTTH = reader["ID_CAIXA_FTTH"] != DBNull.Value ? Convert.ToInt64(reader["ID_CAIXA_FTTH"]) : (long?)null;
                        retorno.Id_Porta_FTTH = reader["ID_PORTA_FTTH"] != DBNull.Value ? Convert.ToInt64(reader["ID_PORTA_FTTH"]) : (long?)null;
                        retorno.Atributo_IPOE = reader["ID_ATRIBUTO_IPOE"] != DBNull.Value ? Convert.ToInt64(reader["ID_ATRIBUTO_IPOE"]) : (long?)null;
                        retorno.Id_POP = reader["ID_POP"] != DBNull.Value ? Convert.ToInt64(reader["ID_POP"]) : (long?)null;
                        retorno.Id_Concentradora = reader["ID_CONCENTRADORA"] != DBNull.Value ? Convert.ToInt64(reader["ID_CONCENTRADORA"]) : (long?)null;
                        retorno.Id_OLT = reader["ID_OLT"] != DBNull.Value ? Convert.ToInt64(reader["ID_OLT"]) : (long?)null;
                        retorno.Id_Slot = reader["ID_SLOT"] != DBNull.Value ? Convert.ToInt64(reader["ID_SLOT"]) : (long?)null;
                        retorno.Id_PON = reader["ID_PON"] != DBNull.Value ? Convert.ToInt64(reader["ID_PON"]) : (long?)null;
                        retorno.Id_Cliente = reader["ID_CLIENTE"] != DBNull.Value ? Convert.ToInt64(reader["ID_CLIENTE"]) : (long?)null;
                        retorno.Id_PlanoProduto = reader["ID_PLANO_PRODUTO"] != DBNull.Value ? Convert.ToInt64(reader["ID_PLANO_PRODUTO"]) : (long?)null;
                        retorno.Id_Pool_Ip_Endereco = reader["ID_POOL_IP_ENDERECO"] != DBNull.Value ? Convert.ToInt64(reader["ID_POOL_IP_ENDERECO"]) : (long?)null;
                    }
                }
            }

            return retorno;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/LoginRadiusapi/InserirLoginsRadiusEmMassa")]
        public async Task<PostApiRetorno> InserirLoginsRadiusEmMassa(List<LoginRadiusCadastroModel> value)
        {
            PostApiRetorno retorno = new PostApiRetorno();
            retorno.Mensagens = new List<PostApiMensagem>();

            try
            {
                if (value.Count == 0)
                    throw new Exception("Lista de Planos vazia.");

                foreach (var item in value)
                {
                    try
                    {
                        if (!item.Id_PlanoProduto.HasValue)
                            throw new Exception("Sem valor de Id Plano Produto");

                        using (var api = new IntegracaoRadiusApiController())
                        {
                            var param = new PlanoProdutoRadiusRequest
                            {
                                IdClientePlanoProduto = item.Id_PlanoProduto.Value
                            };


                            await api.CadastrarUsuario(param);
                            await api.ConfigurarPlanoUsuario(param);
                            await api.AssociaUsuarioAoPlano(param);
                            //await api.BloquearUsuario(param);
                            await api.DesbloquearUsuario(param);
                        }
                    }
                    catch (Exception ex)
                    {
                        retorno.Mensagens.Add(new PostApiMensagem() { Mensagem = $"Erro ao inserir login do Plano {item.Id_PlanoProduto} no Radius", Tipo = PostApiMensagemType.Erro });
                        CONSTANTES.log4Net.Error($"Erro ao inserir login do Plano {item.Id_PlanoProduto} no Radius. - Erro: {ex.Message}", ex);
                    }
                }

                retorno.Ok = true;
                retorno.Mensagem = retorno.Mensagens.Count == 0 ? "Logins inseridos com sucesso no Radius" : "Alguns Logins foram inseridos com sucesso no Radius, analise a lista de mensagens para ver os erros.";
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }



        public async Task<string> GerarLoginComSequencial(string login)
        {
            try
            {
                List<string> logins = new List<string>();
                string loginRetornado = "";
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                using (var session = HibernateUtil.GetSession())
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT LOGIN ");
                    sql.AppendLine("FROM TB_LOGIN_RADIUS ");
                    sql.AppendLine("WHERE LOGIN LIKE @LOGIN ");

                    parametros.Add("@LOGIN", $"%{login}%");

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            string loginValue = reader.GetString(reader.GetOrdinal("LOGIN"));
                            logins.Add(loginValue);
                        }
                    }

                    if (logins.Count == 0)
                    {
                        loginRetornado = login;
                        return loginRetornado;
                    }

                    // Regex para dar match em valores .número no final do login (exemplo: .1, .2)
                    Regex regex = new Regex(@"\.(\d+)$");
                    int maxSequencial = 1;

                    foreach (string loginValue in logins)
                    {

                        if (loginValue.StartsWith(login + "."))
                        {

                            Match match = regex.Match(loginValue);
                            if (match.Success)
                            {
                                maxSequencial++;
                            }
                        }

                    }

                    loginRetornado = $"{login}.{(maxSequencial)}";

                }

                return loginRetornado;
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex);
                throw new Exception("Ocorreu um erro durante a operação: ", ex);
            }
        }


        public async Task<string> GerarSenhaComSequencial(string senha)
        {
            try
            {
                List<string> senhas = new List<string>();
                string senhaRetornada = "";
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                using (var session = HibernateUtil.GetSession())
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT SENHA_PPPOE ");
                    sql.AppendLine("FROM TB_LOGIN_RADIUS ");
                    sql.AppendLine("WHERE SENHA_PPPOE LIKE @SENHA ");

                    parametros.Add("@SENHA", $"%{senha}%");

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            string senhaValue = reader.GetString(reader.GetOrdinal("SENHA_PPPOE"));
                            senhas.Add(senhaValue);
                        }
                    }

                    if (senhas.Count == 0)
                    {
                        //Caso não haja senha duplicada, será retornada a senha que está sendo passada no parâmetro
                        senhaRetornada = senha;
                        return senhaRetornada;
                    }

                    // Regex para dar match em valores .número no final do login (exemplo: .1, .2)
                    Regex regex = new Regex(@"\.(\d+)$");
                    int maxSequencial = 1;

                    foreach (string senhaValue in senhas)
                    {

                        if (senhaValue.StartsWith(senha + "."))
                        {

                            Match match = regex.Match(senhaValue);
                            if (match.Success)
                            {
                                maxSequencial++;
                            }
                        }

                    }

                    senhaRetornada = $"{senha}.{(maxSequencial)}";


                }

                return senhaRetornada;
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex);
                throw new Exception("Ocorreu um erro durante a operação: ", ex);
            }
        }

        public async Task<GetApiRetorno> AtualizacaoModelo(AtualizacaoLogins item)
        {
            GetApiRetorno retorno = new GetApiRetorno();

            try
            {

                //VALIDAÇÕES

                //Procura os dados do login pelo login e senha para validar 
                //se o usuário existe no Connect
                if (String.IsNullOrEmpty(item.Login) || String.IsNullOrEmpty(item.SenhaPPPOE) || String.IsNullOrEmpty(item.Concentradora))
                {
                    throw new Exception("Não é possível atualizar o login sem informações do Login, Senha e Concentradora");
                }

                var retornoLogin = await FindByLoginSenha(item.Login.Trim(), item.SenhaPPPOE.Trim());
                

                if (retornoLogin == null)
                {
                    throw new Exception("Login não encontrado no sistema");
                }


                //Validações Concentradora
                var concentradoraExiste = await GetConcentradoraByNome(item.Concentradora.Trim());

                if (concentradoraExiste == null)
                {
                    throw new Exception("Concentradora não encontrada no sistema");
                }

                var dadosLoginAntesAtualizar = $"Concentradora anterior:{retornoLogin.Concentradora} / " +
                    $"Pool anterior: {retornoLogin.Descricao_Pool} / IP anterior: {retornoLogin.IP}";

                var IdLogin = retornoLogin.Id;
                var idPlanoProduto = retornoLogin.Id_Plano_Produto;
                var concentradoraId = concentradoraExiste.Id;

                await AtualizaConcentradoraLogin(IdLogin, concentradoraId, User.Identity.GetUserName());

                long id_pool;
                var enderecoIPDetalhe = "";
                //Validações Pool IP
                if (!String.IsNullOrEmpty(item.Pool))
                {
                    var poolExiste = await GetPoolByDescricao(item.Pool.Trim());

                    if (poolExiste == null)
                    {
                        throw new Exception("Pool não encontrado no sistema");
                    }

                    if (poolExiste.Status == false)
                    {
                        throw new Exception("Pool desativado. Por favor, ative o Pool na tela de Cadastro de Pool de IP e tente novamente. ");
                    }

                    id_pool = poolExiste.Id;
                    

                    //Validações IP fixo
                    if (poolExiste.IP_Fixo == true)
                    {
                        if (String.IsNullOrEmpty(item.IP))
                        {
                            throw new Exception("Pool de IP fixo. Por favor, informe o IP do cliente no campo IP_FIXO da planilha");
                        }


                        //Verifica se o IP fixo está disponível
                        //Caso não esteja, irá jogar uma exception
                        //Informando que o IP não está disponível
                        //Este método foi desenvolvido apenas para
                        //Impedir que dois logins tenham o mesmo IP
                        var verificaIPDisponivel = await HandleVerificacaoIPdisponivel(item.IP.Trim());
                        if(verificaIPDisponivel == false)
                        {
                            throw new Exception("IP já está em uso. Por favor, escolha outro IP.");
                        }
                        else if(verificaIPDisponivel == true)
                        {
                            await AtualizaPoolPlanoProduto(idPlanoProduto, id_pool, User.Identity.GetUserName(), IdLogin);
                            await AtualizaIPFixoUsuario(IdLogin, id_pool, item.IP.Trim(), User.Identity.GetUserName());
                            enderecoIPDetalhe = item.IP.Trim();
                        }

                    }
                    else
                    {
                        await AtualizaPoolPlanoProduto(idPlanoProduto, id_pool, User.Identity.GetUserName(), IdLogin);
                    }
                    


                }

                //Atualização dos dados
              using (var api = new IntegracaoRadiusApiController())
                {
                    var param = new PlanoProdutoRadiusRequest
                    {
                        IdClientePlanoProduto = idPlanoProduto
                    };


                    var editaUsuario = await api.EditarUsuario(param);
                    if(editaUsuario.result == true)
                    {
                        var detalheSucesso = dadosLoginAntesAtualizar + $" | Concentradora nova: {item.Concentradora.Trim()} / Pool nova: {item.Pool?.Trim() ?? ""} / IP novo: {enderecoIPDetalhe}";


                        retorno.Ok = true;
                        var logAtualizacaoLogins = new LogAtualizacaoLogins
                        {
                            Login = item.Login,
                            Data = DateTime.Now.Date.ToString("dd/MM/yyyy"),
                            Hora = DateTime.Now.ToString("HH:mm"),
                            Detalhe = detalheSucesso,
                            Usuario = User.Identity.GetUserId(),
                            Sucesso = "Sim"
                        };

                        await GravaLogAtualizacao(logAtualizacaoLogins);
                    }
                    else
                    {                        
                        throw new Exception($"Falha ao atualizar login no radius: {editaUsuario.message}");
                    }
                }
                

       
            }
            catch (Exception e)
            {
                var logAtualizacaoLogins = new LogAtualizacaoLogins
                {
                    Login = item.Login?.Trim() ?? "",
                    Data = DateTime.Now.Date.ToString("dd/MM/yyyy"),
                    Hora = DateTime.Now.ToString("HH:mm"),
                    Detalhe = e.InnerException?.Message != null ? (e.Message + e.InnerException?.Message) : e.Message,
                    Usuario = User.Identity.GetUserId(),
                    Sucesso = "Não"
                };

                await GravaLogAtualizacao(logAtualizacaoLogins);

                retorno.Mensagem = e.Message;
                retorno.InnerException = e.InnerException?.Message;
            }

            return retorno;
        }

        public async Task GravaLogAtualizacao(LogAtualizacaoLogins log)
        {
            using (var session = HibernateUtil.GetSessionLog())
            using (var com = session.Connection.CreateCommand())
            {

                StringBuilder sql = new StringBuilder();
                sql.AppendLine(" INSERT INTO TB_LOG_ATUALIZACAO_LOGINS (ID,LOGIN,DATA,HORA,DETALHE,SUCESSO,USUARIO)");
                sql.AppendLine($" VALUES ((SELECT GEN_ID(GN_LOG_ATUALIZACAO_LOGINS, 1) FROM RDB$DATABASE),@LOGIN,@DATA,@HORA,@DETALHE,@SUCESSO,@USUARIO)");

                Dictionary<string, object> parametros = new Dictionary<string, object>();
                parametros.Add("@LOGIN", log.Login);
                parametros.Add("@DATA", log.Data.Replace('/', '-'));
                parametros.Add("@HORA", log.Hora);
                parametros.Add("@DETALHE", log.Detalhe);
                parametros.Add("@SUCESSO", log.Sucesso);
                parametros.Add("@USUARIO", log.Usuario);
                await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
            }
        }

        [HttpPost]
        [Route("api/LoginRadiusapi/LogAtualizacaoLogins")]
        public async Task<DataTableResponse<LogAtualizacaoLogins>> Log(LogAtualizacaoLoginsRequest request)
        {
            List<LogAtualizacaoLogins> retorno = new List<LogAtualizacaoLogins>();
            var recordsTotal = 0;

            using (var session = HibernateUtil.GetSessionLog())
            {
                StringBuilder sqlWhere = new StringBuilder();
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                if (!String.IsNullOrEmpty(request.Data))
                {
                    parametros.Add("DATA", Convert.ToDateTime(request.Data).ToString("dd-MM-yyyy"));

                    sqlWhere.AppendLine(" WHERE DATA = @DATA ");
                }

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sqlCount = new StringBuilder("SELECT COUNT(ID) AS CO FROM TB_LOG_ATUALIZACAO_LOGINS ");
                    sqlCount.AppendLine(sqlWhere.ToString());

                    using (var reader = await CommandHelper.ExecuteReader(com, sqlCount, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                //SQL
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT * FROM TB_LOG_ATUALIZACAO_LOGINS ");
                    sql.AppendLine(sqlWhere.ToString());

                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new LogAtualizacaoLogins { Login = reader["LOGIN"].ToString(), Data = Convert.ToDateTime(reader["DATA"]).ToString("dd/MM/yyyy"), Hora = Convert.ToDateTime(reader["HORA"].ToString()).ToString("HH:mm"), Detalhe = reader["DETALHE"].ToString(), Sucesso = reader["SUCESSO"].ToString(), Usuario = reader["USUARIO"].ToString() });
                        }
                    }
                }
            }

            return new DataTableResponse<LogAtualizacaoLogins>
            {
                data = retorno,
                recordsFiltered = recordsTotal
            };
        }

        public async Task<GetDadosLoginAtual> FindByLoginSenha(string login, string senha)
        {
            GetDadosLoginAtual retorno = null;
            try
            {

                Dictionary<string, object> parametros = new Dictionary<string, object>();
                using (var session = HibernateUtil.GetSession())
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT FIRST 1 TB_LOGIN_RADIUS.ID, TB_LOGIN_RADIUS.LOGIN, SENHA_PPPOE, TB_LOGIN_RADIUS.ID_CONCENTRADORA, ");
                    sql.AppendLine(" ID_POOL_IP_ENDERECO, ID_PLANO_PRODUTO, TB_CONCENTRADORA.NOME, TB_CLIENTE_PLANO_PRODUTO.ID_POOL_IP, ");
                    sql.AppendLine(" TB_POOL_IP.DESCRICAO, TB_POOL_IP.POOL_IP_FIXO, TB_POOL_IP_ENDERECO.IP ");
                    sql.AppendLine(" FROM TB_LOGIN_RADIUS ");
                    sql.AppendLine(" LEFT JOIN TB_CLIENTE_PLANO_PRODUTO ON TB_LOGIN_RADIUS.ID_PLANO_PRODUTO = TB_CLIENTE_PLANO_PRODUTO.ID ");
                    sql.AppendLine(" LEFT JOIN TB_CONCENTRADORA ON TB_LOGIN_RADIUS.ID_CONCENTRADORA = TB_CONCENTRADORA.ID ");
                    sql.AppendLine(" LEFT JOIN TB_POOL_IP ON TB_CLIENTE_PLANO_PRODUTO.ID_POOL_IP = TB_POOL_IP.ID ");
                    sql.AppendLine(" LEFT JOIN TB_POOL_IP_ENDERECO ON TB_LOGIN_RADIUS.ID_POOL_IP_ENDERECO = TB_POOL_IP_ENDERECO.ID ");
                    sql.AppendLine(" WHERE TB_LOGIN_RADIUS.LOGIN = @LOGIN AND SENHA_PPPOE = @SENHA_PPPOE ");


                    parametros.Add("@LOGIN", login);
                    parametros.Add("@SENHA_PPPOE", senha);

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            retorno = new GetDadosLoginAtual();
                            if (reader["ID"] == DBNull.Value)
                                throw new InvalidOperationException("ID não pode ser null.");
                            retorno.Id = Convert.ToInt64(reader["ID"]);
                            retorno.Login = reader["LOGIN"] != DBNull.Value ? Convert.ToString(reader["LOGIN"]) : null;
                            retorno.SenhaPPPOE = reader["SENHA_PPPOE"] != DBNull.Value ? Convert.ToString(reader["SENHA_PPPOE"]) : null;
                            retorno.Concentradora = reader["NOME"] != DBNull.Value ? Convert.ToString(reader["NOME"]) : null;
                            retorno.Id_Concentradora = reader["ID_CONCENTRADORA"] != DBNull.Value ? Convert.ToInt64(reader["ID_CONCENTRADORA"]) : (long?)null;
                            retorno.Id_Pool = reader["ID_POOL_IP"] != DBNull.Value ? Convert.ToInt64(reader["ID_POOL_IP"]) : (long?)null;
                            retorno.Descricao_Pool = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : null;
                            retorno.Id_Pool_Ip_Endereco = reader["ID_POOL_IP_ENDERECO"] != DBNull.Value ? Convert.ToInt64(reader["ID_POOL_IP_ENDERECO"]) : (long?)null;
                            if (reader["ID_PLANO_PRODUTO"] == DBNull.Value)
                                throw new InvalidOperationException("ID_PLANO_PRODUTO não pode ser null.");
                            retorno.Id_Plano_Produto = Convert.ToInt64(reader["ID_PLANO_PRODUTO"]);
                            retorno.IP = reader["IP"] != DBNull.Value ? Convert.ToString(reader["IP"]) : null;
                            retorno.IP_Fixo = reader["POOL_IP_FIXO"] != DBNull.Value && Convert.ToBoolean(reader["POOL_IP_FIXO"]);
                        }

                    }
                }

                return retorno;

            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex);
                throw new Exception("Ocorreu um erro durante a operação: ", ex);
            }

        }

        public async Task<GetDadosConcentradora> GetConcentradoraByNome(string concentradora)
        {
            GetDadosConcentradora retorno = null;
            try
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();
                using (var session = HibernateUtil.GetSession())
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT FIRST 1 NOME, ID ");
                    sql.AppendLine(" FROM TB_CONCENTRADORA ");
                    sql.AppendLine(" WHERE NOME = @NOME ");

                    parametros.Add("@NOME", concentradora);

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            retorno = new GetDadosConcentradora();
                            if (reader["ID"] == DBNull.Value)
                                throw new InvalidOperationException("ID não pode ser null.");
                            retorno.Id = Convert.ToInt64(reader["ID"]);
                            retorno.Nome = Convert.ToString(reader["NOME"]);
                        }

                    }
                }

                return retorno;
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex);
                throw new Exception("Ocorreu um erro durante a operação: ", ex);
            }

        }

        public async Task<GetDadosPoolIp> GetPoolByDescricao(string descricao)
        {
            GetDadosPoolIp retorno = null;
            try
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();
                using (var session = HibernateUtil.GetSession())
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT FIRST 1 DESCRICAO, ID, POOL_IP_FIXO, STATUS ");
                    sql.AppendLine(" FROM TB_POOL_IP ");
                    sql.AppendLine(" WHERE DESCRICAO = @DESCRICAO ");

                    parametros.Add("@DESCRICAO", descricao);

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            retorno = new GetDadosPoolIp();
                            if (reader["ID"] == DBNull.Value)
                                throw new InvalidOperationException("ID não pode ser null.");
                            retorno.Id = Convert.ToInt64(reader["ID"]);
                            retorno.Descricao_Pool = Convert.ToString(reader["DESCRICAO"]);
                            retorno.IP_Fixo = reader["POOL_IP_FIXO"] != DBNull.Value && Convert.ToBoolean(reader["POOL_IP_FIXO"]);
                            retorno.Status = reader["STATUS"] != DBNull.Value && Convert.ToBoolean(reader["STATUS"]);
                        }

                    }
                }

                return retorno;
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex);
                throw new Exception("Ocorreu um erro durante a operação: ", ex);
            }

        }

        public async Task<bool> HandleVerificacaoIPdisponivel(string IP)
        {
            bool retorno = false;
            try
            {
                using (ISession sessionRadius = HibernateUtil.GetSessionRadius())
                using (ISession session = HibernateUtil.GetSession())
                using (var repoRadius = new Conexao<Sinapse.Model.Radius>(session))
                {
                    var radius = await repoRadius.selectFirst();
                    var retornoIpDisponivel = await new AnielRadius(radius).VerificaIPDisponivel(IP);

                    if (retornoIpDisponivel == false)
                    {
                        return retorno;
                    }
                    else
                    {
                        retorno = true;
                    }
                }

                return retorno;
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex);
                throw new Exception("Ocorreu um erro durante a operação: ", ex);
            }

        }

        public async Task AtualizaIPFixoUsuario(long id, long id_pool, string ip, string usuarioAlteracao)
        {
            try
            {
                long id_pool_ip_endereco;

                
                var parametros = new Dictionary<string, object>
                {
                   { "@ID_LOGIN_RADIUS", id },
                   { "@ID_POOL", id_pool },
                   { "@IP", ip },
                   { "@USUARIOALTERACAO", usuarioAlteracao}
                };

                using (var session = HibernateUtil.GetSession())
                using (var com = session.Connection.CreateCommand())
                {
                    // SELECT ID_POOL_IP_ENDERECO
                    var selectSql = new StringBuilder();
                    selectSql.AppendLine("SELECT ID FROM TB_POOL_IP_ENDERECO");
                    selectSql.AppendLine("WHERE IP = @IP AND ID_POOL_IP = @ID_POOL");

                    using (var reader = await CommandHelper.ExecuteReader(com, selectSql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            id_pool_ip_endereco = Convert.ToInt64(reader["ID"]);
                            parametros["@ID_POOL_IP_ENDERECO"] = id_pool_ip_endereco; 
                        }
                        else
                        {
                            throw new Exception("Endereço IP não encontrado.");
                        }
                    }

                    // Update TB_POOL_IP_ENDERECO
                    var updateSqlPool = new StringBuilder();
                    updateSqlPool.AppendLine("UPDATE TB_POOL_IP_ENDERECO");
                    updateSqlPool.AppendLine("SET ID_LOGIN_RADIUS = @ID_LOGIN_RADIUS,");
                    updateSqlPool.AppendLine("    STATUS_IP = 0, ");
                    updateSqlPool.AppendLine("    USUARIOALTERACAO = @USUARIOALTERACAO, ");
                    updateSqlPool.AppendLine("    DATAHORAALTERACAO = CURRENT_TIMESTAMP ");
                    updateSqlPool.AppendLine("WHERE ID_POOL_IP = @ID_POOL");
                    updateSqlPool.AppendLine("  AND IP = @IP");
                    updateSqlPool.AppendLine("  AND ID = @ID_POOL_IP_ENDERECO");


                    await CommandHelper.ExecuteNonQuery(com, updateSqlPool, parametros, session.Connection, null);

                    // Update TB_LOGIN_RADIUS
                    var updateSqlLogin = new StringBuilder();
                    updateSqlLogin.AppendLine("UPDATE TB_LOGIN_RADIUS");
                    updateSqlLogin.AppendLine("SET ID_POOL_IP_ENDERECO = @ID_POOL_IP_ENDERECO,");
                    updateSqlLogin.AppendLine(" ENDERECO_IPV4 = @IP,");
                    updateSqlLogin.AppendLine("    USUARIOALTERACAO = @USUARIOALTERACAO, ");
                    updateSqlLogin.AppendLine("    DATAHORAALTERACAO = CURRENT_TIMESTAMP ");
                    updateSqlLogin.AppendLine("WHERE ID = @ID_LOGIN_RADIUS");


                    await CommandHelper.ExecuteNonQuery(com, updateSqlLogin, parametros, session.Connection, null);
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex);
                throw new Exception("Ocorreu um erro durante a operação: ", ex);
            }
        }


        public async Task AtualizaConcentradoraLogin(long IdLogin, long idConcentradora, string usuarioAlteracao)
        {
            try
            {
                using (var session = HibernateUtil.GetSession())
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();
                    Dictionary<string, object> parametros = new Dictionary<string, object>();

                    sql.AppendLine(" UPDATE TB_LOGIN_RADIUS ");
                    sql.AppendLine(" SET ID_CONCENTRADORA = @ID_CONCENTRADORA, ");
                    sql.AppendLine(" USUARIOALTERACAO = @USUARIOALTERACAO, ");
                    sql.AppendLine(" DATAHORAALTERACAO = CURRENT_TIMESTAMP ");
                    sql.AppendLine(" WHERE ID = @ID_LOGIN_RADIUS ");

                    parametros.Add("@ID_CONCENTRADORA", idConcentradora);
                    parametros.Add("@ID_LOGIN_RADIUS", IdLogin);
                    parametros.Add("@USUARIOALTERACAO", usuarioAlteracao);


                    await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex);
                throw new Exception("Ocorreu um erro durante a operação: ", ex);
            }
        }


        public async Task AtualizaPoolPlanoProduto(long idPlanoProduto, long idPool, string usuarioAlteracao, long idLogin)
        {
            try
            {
                var parametros = new Dictionary<string, object>
                    {
                      { "@ID_POOL_IP", idPool },
                      { "@ID_PLANO_PRODUTO", idPlanoProduto },
                      { "@USUARIOALTERACAO", usuarioAlteracao },
                      { "@ID_LOGIN", idLogin }
                    };

                using (var session = HibernateUtil.GetSession())
                using (var com = session.Connection.CreateCommand())
                {
                    // Update TB_CLIENTE_PLANO_PRODUTO
                    var sqltbClientePlanoProduto = new StringBuilder();
                    sqltbClientePlanoProduto.AppendLine("UPDATE TB_CLIENTE_PLANO_PRODUTO");
                    sqltbClientePlanoProduto.AppendLine("SET ID_POOL_IP = @ID_POOL_IP,");
                    sqltbClientePlanoProduto.AppendLine("    USUARIOALTERACAO = @USUARIOALTERACAO,");
                    sqltbClientePlanoProduto.AppendLine("    DATAHORAALTERACAO = CURRENT_TIMESTAMP");
                    sqltbClientePlanoProduto.AppendLine("WHERE ID = @ID_PLANO_PRODUTO;");

                    await CommandHelper.ExecuteNonQuery(com, sqltbClientePlanoProduto, parametros, session.Connection, null);

                    // Update TB_LOGIN_RADIUS
                    var sqltbLoginRadius = new StringBuilder();
                    sqltbLoginRadius.AppendLine("UPDATE TB_LOGIN_RADIUS");
                    sqltbLoginRadius.AppendLine("SET ENDERECO_IPV4 = NULL,");
                    sqltbLoginRadius.AppendLine("    ID_POOL_IP_ENDERECO = NULL,");
                    sqltbLoginRadius.AppendLine("    USUARIOALTERACAO = @USUARIOALTERACAO,");
                    sqltbLoginRadius.AppendLine("    DATAHORAALTERACAO = CURRENT_TIMESTAMP");
                    sqltbLoginRadius.AppendLine("WHERE ID = @ID_LOGIN;");

                    await CommandHelper.ExecuteNonQuery(com, sqltbLoginRadius, parametros, session.Connection, null);

                    // Update TB_POOL_IP_ENDERECO
                    var sqltbPoolIpEndereco = new StringBuilder();
                    sqltbPoolIpEndereco.AppendLine("UPDATE TB_POOL_IP_ENDERECO");
                    sqltbPoolIpEndereco.AppendLine("SET ID_LOGIN_RADIUS = NULL,");
                    sqltbPoolIpEndereco.AppendLine("    STATUS_IP = 1,");
                    sqltbPoolIpEndereco.AppendLine("    USUARIOALTERACAO = @USUARIOALTERACAO,");
                    sqltbPoolIpEndereco.AppendLine("    DATAHORAALTERACAO = CURRENT_TIMESTAMP");
                    sqltbPoolIpEndereco.AppendLine("WHERE ID_LOGIN_RADIUS = @ID_LOGIN;");

                                
                    await CommandHelper.ExecuteNonQuery(com, sqltbPoolIpEndereco, parametros, session.Connection, null);

                   

                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex);
                throw new Exception("Ocorreu um erro durante a operação: ", ex);
            }
        }

        [HttpPost()]
        [Route("api/LoginRadiusapi/HistoricoLoginAlteracao")]
        public async Task<DataTableResponse<LogGenericoModel>> HistoricoLoginAlteracao(HistoricoAlteracaoLogin_DataTableRequest request)
        {
            List<LogGenericoModel> retorno = new List<LogGenericoModel>();
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSessionLog())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                StringBuilder sqlFrom = new StringBuilder();

                sqlFrom.AppendLine("    FROM TB_LOGIN_RADIUS");
                sqlFrom.AppendLine("    WHERE DATAHORACRIACAO BETWEEN @DATAINI AND @DATAFIM ");               

                parametros.Add("@DATAINI", DateTime.Parse(request.DataIni + " 00:00:00"));
                parametros.Add("@DATAFIM", DateTime.Parse(request.DataFim + " 23:59:59"));

                if (!String.IsNullOrEmpty(request.search.value))
                {
                    sqlFrom.AppendLine("  AND ((UPPER(OBJETO) LIKE UPPER(@SEARCH))) ");
                    parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                }

                if (request.Id.HasValue)
                {
                    sqlFrom.AppendLine("  AND OBJETO = @ID ");                    
                    parametros.Add("@ID", request.Id.ToString());
                }

                if (request.TipoOperacao != null)
                {
                    sqlFrom.AppendLine("  AND TIPOOPERACAO = @TIPOOPERACAO ");                    
                    parametros.Add("@TIPOOPERACAO", Convert.ToInt32(request.TipoOperacao));
                }

                if (!String.IsNullOrEmpty(request.Campo))
                {
                    sqlFrom.AppendLine("  AND CAMPO = @CAMPO ");                    
                    parametros.Add("@CAMPO", request.Campo);
                }                

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sqlCount = new StringBuilder("SELECT COUNT(ID) CO ");                                       
                    sqlCount.AppendLine(sqlFrom.ToString());
                    

                    using (var reader = await CommandHelper.ExecuteReader(com, sqlCount, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                // Select
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT ID, NOMEUSUARIOCRIACAO, USUARIOCRIACAO, DATAHORACRIACAO, TIPOOPERACAO, IDLOG, OBJETO, CAMPO, CAMPO_VALORANTES, CAMPO_VALORDEPOIS ");
                    sql.AppendLine(sqlFrom.ToString());                   

                    //Ordenacao
                    if (request.order != null && request.order.Any())
                    {
                        int sortColumnIndex = request.order[0].column;
                        string sortDirection = request.order[0].dir;
                        switch (sortColumnIndex)
                        {
                            case 0:     // CHAVE
                                {
                                    if (sortDirection == "asc")
                                        sql.Append(" ORDER BY OBJETO ");
                                    else
                                        sql.Append(" ORDER BY OBJETO DESC ");
                                    break;
                                }
                            case 1:     // CAMPO
                                {
                                    if (sortDirection == "asc")
                                        sql.Append(" ORDER BY CAMPO ");
                                    else
                                        sql.Append(" ORDER BY CAMPO DESC ");
                                    break;
                                }
                            case 6: //Data Hora
                                {
                                    if (sortDirection == "asc")
                                        sql.Append(" ORDER BY DATAHORACRIACAO ");
                                    else
                                        sql.Append(" ORDER BY DATAHORACRIACAO DESC ");
                                    break;
                                }
                        }
                    }

                    // Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new LogGenericoModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Chave = Convert.ToString(reader["OBJETO"]),
                                Campo = Convert.ToString(reader["CAMPO"]),

                                Campo_ValorAntes = Convert.ToString(reader["CAMPO_VALORANTES"]),
                                Campo_ValorDepois = Convert.ToString(reader["CAMPO_VALORDEPOIS"]),
                                TipoOperacao = HtmlEnumExtensions.GetEnumDescription<TipoOperacao>(Convert.ToString(reader["TIPOOPERACAO"])),

                                UsuarioCriacao = Convert.ToString(reader["USUARIOCRIACAO"]),
                                Nome_usuario = Convert.ToString(reader["NOMEUSUARIOCRIACAO"]),
                                DataHoraCriacao = reader["DATAHORACRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORACRIACAO"]).ToString("dd/MM/yyyy HH:mm") : String.Empty,
                            });
                        }
                    }
                }
            }

            // Page e retornando DataTable
            return new DataTableResponse<LogGenericoModel>
            {
                draw = request.draw,
                recordsFiltered = recordsTotal,
                data = retorno.OrderByDescending(p => p.DataHoraCriacao),
                error = ""
            };
        }

        /// <summary>
        /// Valida e realiza a troca de plano no Login se necessário.
        /// </summary>
        /// <param name="planoProduto"></param>
        /// <returns></returns>
        public async Task ValidarTrocaPlanoLogin(long idClientePlanoProduto)
        {
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repositoryLogin = new Conexao<Model.nhibernate.LoginRadius>(session))
                using (var repoPoolIpEndereco = new Conexao<Model.nhibernate.Pool_Ip_Endereco>(session))
                using (var repository_cliente_plano_produto = new Conexao<Cliente_PlanoProduto>(session))
                {
                    var planoProduto = await repository_cliente_plano_produto.selectFirst(p => p.Id == idClientePlanoProduto);

                    if(planoProduto != null && planoProduto.Id_TrocaPlano.HasValue)
                    {
                        var objPlanoDescontinuado = await repository_cliente_plano_produto.selectFirst(x => x.Id == planoProduto.Id_TrocaPlano);

                        //Se o plano descontinuado continua vigente
                        if(objPlanoDescontinuado != null && 
                            objPlanoDescontinuado.Status != Status_PlanoProduto.Cancelado_Mudanca_Plano && 
                            objPlanoDescontinuado.Motivo_Mudanca_PP != Motivo_MudancaPlano_Produto.Motivo_Mudanca_PP)                        
                        {
                            #region Descontinuação do Plano antigo                               
                            objPlanoDescontinuado.Status = Status_PlanoProduto.Cancelado_Mudanca_Plano;
                            objPlanoDescontinuado.Motivo_Mudanca_PP = Motivo_MudancaPlano_Produto.Motivo_Mudanca_PP;
                            objPlanoDescontinuado.Login_Radius = null;
                            objPlanoDescontinuado.Senha_Radius = null;
                            await repository_cliente_plano_produto.SaveOrUpdate(objPlanoDescontinuado);
                            #endregion

                            var objLoginRadius = await repositoryLogin.selectFirst(x => x.Cliente_PlanoProduto.Id == planoProduto.Id_TrocaPlano);
                            //Se o login ainda está vinculado ao plano descontinuado
                            if(objLoginRadius != null)
                            {
                                #region Exclusão do Login novo
                                //Deleta o login criado para o plano novo pois deve permanecer o login original do plano antigo
                                var objLoginRadius_PlanoNovo = await repositoryLogin.selectFirst(x => x.Cliente_PlanoProduto.Id == planoProduto.Id);
                                if (objLoginRadius_PlanoNovo != null)
                                {
                                    objLoginRadius_PlanoNovo.Cliente_PlanoProduto = null;

                                    if (objLoginRadius_PlanoNovo.Pool_Ip_Endereco != null)
                                    {
                                        objLoginRadius_PlanoNovo.Pool_Ip_Endereco.Login = null;
                                        objLoginRadius_PlanoNovo.Pool_Ip_Endereco.Status_Ip = Model.enumerado.Pool_Ip_Range_StatusEnum.Disponivel;
                                        await repoPoolIpEndereco.Update(objLoginRadius_PlanoNovo.Pool_Ip_Endereco);
                                    }
                                    await repositoryLogin.Remove(objLoginRadius_PlanoNovo);
                                }
                                #endregion                                

                                #region [Objeto - ClienteContrato]
                                planoProduto.ClienteContrato = objPlanoDescontinuado.ClienteContrato;
                                #endregion

                                #region Troca do Plano no Login                            
                                if (objLoginRadius != null)
                                {
                                    objLoginRadius.Cliente_PlanoProduto = planoProduto;
                                    /*
                                     * Limpeza da porta do novo plano, pois está reservada automaticamente
                                     * E é necessário, que a porta, esteja na mesma do antigo. 
                                     * */
                                    Elemento_ReservaCadastroModel elemento_ = new Elemento_ReservaCadastroModel();
                                    IElementoRedePortaAnielRepository repository = new ElementoRedePortaAnielRepository(session, sessionT5);
                                    ElementoRedePortaAnielBusiness elementoRedePortaBusiness = new ElementoRedePortaAnielBusiness(repository);
                                    elemento_.Login = objLoginRadius.Login;
                                    elemento_.Cliente_PlanoProduto_Novo = planoProduto.Id;
                                    await repository.LimparPorta(elemento_);

                                    elemento_.Cliente_PlanoProduto_Antigo = planoProduto.Id_TrocaPlano.Value;
                                    await repository.AlterarPortaPlano(elemento_, false);

                                    await repository_cliente_plano_produto.SaveOrUpdate(planoProduto);
                                    await repositoryLogin.SaveOrUpdate(objLoginRadius);

                                    if (planoProduto?.Plano?.AutenticacaoRadius == true)
                                        await new IntegracaoRadiusApiController().EditarUsuario(new PlanoProdutoRadiusRequest
                                        {
                                            IdClientePlanoProduto = planoProduto.Id
                                        });                                                                       
                                }
                                #endregion
                            }
                        }
                    }                    
                }
            }
            catch(Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }            
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("api/LoginRadiusapi/InserirLoginsRadiusEmMassaSemFiltro")]
        public async Task<PostApiRetorno> InserirLoginsRadiusEmMassaSemFiltro(LoginsRadiusEmMassaSemFiltroRequest value)
        {
            PostApiRetorno retorno = new PostApiRetorno();
            retorno.Mensagens = new List<PostApiMensagem>();

            try
            {
                using (ISession sessionConnect = HibernateUtil.GetSession())
                {

                    List<LoginRadiusCadastroModel> lista = new List<LoginRadiusCadastroModel>();
                    using (var com = sessionConnect.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        Dictionary<string, object> parametros = new Dictionary<string, object>();

                        sql.AppendLine(" SELECT ID_PLANO_PRODUTO ");
                        sql.AppendLine(" FROM TB_LOGIN_RADIUS ");

                        if (value.IdParou.HasValue)
                        {
                            sql.AppendLine(" WHERE ID > @ID ");
                            parametros.Add("@ID", value.IdParou.Value);

                        }

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionConnect.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                if (reader["ID_PLANO_PRODUTO"] != DBNull.Value)
                                    lista.Add(new LoginRadiusCadastroModel { Id_PlanoProduto = Convert.ToInt64(reader["ID_PLANO_PRODUTO"]) });
                            }
                        }
                    }

                    retorno = await this.InserirLoginsRadiusEmMassaV2(lista);
                }



                retorno.Ok = true;
                retorno.Mensagem = retorno.Mensagens.Count == 0 ? "Logins inseridos com sucesso no Radius" : "Alguns Logins foram inseridos com sucesso no Radius, analise a lista de mensagens para ver os erros.";
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/LoginRadiusapi/InserirLoginsRadiusEmMassaV2")]
        public async Task<PostApiRetorno> InserirLoginsRadiusEmMassaV2(List<LoginRadiusCadastroModel> value)
        {
            PostApiRetorno retorno = new PostApiRetorno();
            retorno.Mensagens = new List<PostApiMensagem>();

            try
            {
                if (value.Count == 0)
                    throw new Exception("Lista de Planos vazia.");

                foreach (var item in value)
                {
                    try
                    {
                        if (!item.Id_PlanoProduto.HasValue)
                            throw new Exception("Sem valor de Id Plano Produto");

                        using (var api = new IntegracaoRadiusApiController())
                        {
                            var param = new PlanoProdutoRadiusRequest
                            {
                                IdClientePlanoProduto = item.Id_PlanoProduto.Value
                            };

                            using (ISession sessionRadius = HibernateUtil.GetSessionRadius())
                            using (ISession sessionConnect = HibernateUtil.GetSession())
                            using (var repoLoginRadius = new Conexao<Model.nhibernate.LoginRadius>(sessionConnect))
                            using (var repoConcentradora = new Conexao<Concentradora>(sessionConnect))
                            {
                                var loginRadius = await repoLoginRadius.selectFirst(p => p.Cliente_PlanoProduto.Id == item.Id_PlanoProduto);

                                var anielRadius = new Sinapse.Integracao.Ativacao.Radius.AnielRadius(loginRadius.Concentradora.Radius);

                                if (loginRadius.Concentradora.Tipo == null)
                                    throw new Exception("Marca não informado da concentradora.");

                                var loginModel = api.BindObjToLoginRadiusModel(loginRadius);
                                var nomePlano = loginRadius.Cliente_PlanoProduto?.Plano?.Descricao;

                                switch (loginRadius.Concentradora.Tipo)
                                {
                                    case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Mikrotik:
                                        RadiusRepository.CadastrarUsuario(
                                            anielRadius,
                                            sessionRadius,
                                            loginModel
                                        );

                                        RadiusRepository.ConfigurarPlanoCliente(new Mikrotik(), sessionRadius, loginModel);
                                        RadiusRepository.AssociaUsuarioAoPlano(anielRadius, sessionRadius, loginRadius.Login, nomePlano, "1", item.Id_PlanoProduto.Value);
                                        await RadiusRepository.DesbloquearUsuario(anielRadius, sessionRadius, loginRadius.Login, loginRadius.Cliente_PlanoProduto.Plano.Descricao);

                                        if (loginRadius.Status == Status_Default.Inativo)
                                        {
                                            loginRadius.Cliente_PlanoProduto.VelocidadeDownloadReduzida = "0,01"; //10 Kbps
                                            loginRadius.Cliente_PlanoProduto.VelocidadeUploadReduzida = "0,01"; //10 Kbps
                                            RadiusRepository.ReduzirVelocidade(new Mikrotik(), sessionRadius, sessionConnect, api.BindObjToLoginRadiusModel(loginRadius));

                                        }
                                        break;
                                    case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Huawei:
                                        RadiusRepository.CadastrarUsuario(
                                            anielRadius,
                                            sessionRadius,
                                            loginModel
                                        );

                                        RadiusRepository.ConfigurarPlanoCliente(new Huawei(), sessionRadius, loginModel);
                                        RadiusRepository.AssociaUsuarioAoPlano(anielRadius, sessionRadius, loginRadius.Login, nomePlano, "1", item.Id_PlanoProduto.Value);
                                        await RadiusRepository.DesbloquearUsuario(anielRadius, sessionRadius, loginRadius.Login, loginRadius.Cliente_PlanoProduto.Plano.Descricao);

                                        if (loginRadius.Status == Status_Default.Inativo)
                                        {
                                            loginRadius.Cliente_PlanoProduto.VelocidadeDownloadReduzida = "0,01"; //10 Kbps
                                            loginRadius.Cliente_PlanoProduto.VelocidadeUploadReduzida = "0,01"; //10 Kbps                                         
                                            RadiusRepository.ReduzirVelocidade(new Huawei(), sessionRadius, sessionConnect, api.BindObjToLoginRadiusModel(loginRadius));
                                        }
                                        break;
                                    case Sinapse.Model.enumerados.Marca_Modelo_Concentradora.Juniper:
                                        RadiusRepository.CadastrarUsuario(
                                            anielRadius,
                                            sessionRadius,
                                            loginModel
                                        );

                                        RadiusRepository.ConfigurarPlanoCliente(new Juniper(), sessionRadius, loginModel);
                                        RadiusRepository.AssociaUsuarioAoPlano(anielRadius, sessionRadius, loginRadius.Login, nomePlano, "1", item.Id_PlanoProduto.Value);
                                        await RadiusRepository.DesbloquearUsuario(anielRadius, sessionRadius, loginRadius.Login, loginRadius.Cliente_PlanoProduto.Plano.Descricao);

                                        if (loginRadius.Status == Status_Default.Inativo)
                                        {
                                            loginRadius.Cliente_PlanoProduto.VelocidadeDownloadReduzida = "0,01"; //10 Kbps
                                            loginRadius.Cliente_PlanoProduto.VelocidadeUploadReduzida = "0,01"; //10 Kbps
                                            RadiusRepository.ReduzirVelocidade(new Juniper(), sessionRadius, sessionConnect, api.BindObjToLoginRadiusModel(loginRadius));
                                        }
                                        break;
                                }

                            }
                        }



                        #region Bloqueio de Login CyberFly
                        //TODO: NÃO COMMITAR, REMOVER TODA ESSA REGION DEPOIS
                        //Fluxo de bloqueio dos logins na CyberFly não bloqueia de fato o login no Radius,
                        //só aplica uma velocidade reduzida extremamente baixa, mantendo a mesma Pool e mesmo IP                            
                        #endregion

                    }
                    catch (Exception ex)
                    {
                        retorno.Mensagens.Add(new PostApiMensagem() { Mensagem = $"Erro ao inserir login do Plano {item.Id_PlanoProduto} no Radius", Tipo = PostApiMensagemType.Erro });
                        CONSTANTES.log4Net.Error($"Erro ao inserir login do Plano {item.Id_PlanoProduto} no Radius. - Erro: {ex.Message}", ex);
                    }
                }

                retorno.Ok = true;
                retorno.Mensagem = retorno.Mensagens.Count == 0 ? "Logins inseridos com sucesso no Radius" : "Alguns Logins foram inseridos com sucesso no Radius, analise a lista de mensagens para ver os erros.";
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }
    }
}
