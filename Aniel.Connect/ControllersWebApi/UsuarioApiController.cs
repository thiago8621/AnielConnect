using Aniel.Connect.Core;
using Aniel.Connect.Models;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using NHibernate.Criterion;
using Sinapse.Core;
using Sinapse.DAO.repository;
using Sinapse.DAO.service;
using Sinapse.Model;
using Sinapse.Model.Identity;
using Sinapse.Model.nhibernate;
using Sinapse.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;


namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class UsuarioApiController : BaseApiController
    {

        #region AutoComplete
        [Route("api/UsuarioApi/AutoComplete")]
        [HttpPost]
        public async Task<IEnumerable<JsonModel>> AutoComplete([FromBody]ParamWebApi<String> Objeto)
        {
            IEnumerable<IdentityUser> usuarios = new List<IdentityUser>();

            using (ISession session = HibernateUtil.GetSession())
            {
                using (IdentityUserRepository service = new IdentityUserRepository(session, null))
                {
                    if (String.IsNullOrEmpty(Objeto.Token))
                        Objeto.Token = await GetTokenByLogin(Objeto.Usuario);

                    usuarios = await service.GetByLoginContains(Objeto.Token, Objeto.Objeto);
                }

                return (from o in usuarios
                        select new JsonModel
                        {
                            id = o.Id,
                            label = o.Nome
                        });
            }
        }

        #endregion


        public async Task<GetApiRetorno<IEnumerable<UsuarioListagemViewModel>>> Get(String t, Int32 p = 1)
        {
            GetApiRetorno<IEnumerable<UsuarioListagemViewModel>> retorno = new GetApiRetorno<IEnumerable<UsuarioListagemViewModel>>();

            Int32 skip = (p - 1) * CONSTANTES.PaginacaoRegistros;
            Int32 take = CONSTANTES.PaginacaoRegistros;

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<IdentityUser>(session))
            {
                try
                {
                    var lista = repository.select(x => x.Token == t);
                    retorno.Objeto = from o in await lista
                                     select new UsuarioListagemViewModel
                                     {
                                         Id = o.Id,
                                         Email = o.Email,
                                         Nome = o.Nome,
                                         //Perfis = String.Join(",", from l in perfilService.Get(o)
                                         //                          select l.Descricao),
                                         Login = o.UserName
                                     };
                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Objeto = new List<UsuarioListagemViewModel>();
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                }
            }
            return retorno;
        }

        // GET: api/UsuarioApi/5
        [Route("api/UsuarioApi/Get/{id}")]
        public async Task<GetApiRetorno<UsuarioViewModel>> Get(String id)
        {
            GetApiRetorno<UsuarioViewModel> retorno = new GetApiRetorno<UsuarioViewModel>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repository = new Conexao<IdentityUser>(session))
                {
                    var lista = await repository.select(p => p.Id == id);

                    List<UsuarioViewModel> objs = (from o in lista
                                                   select new UsuarioViewModel
                                                   {
                                                       Email = o.Email,
                                                       Login = o.UserName,
                                                       Nome = o.Nome,
                                                       Id = o.Id
                                                   }).ToList();

                    if (objs.Count == 0)
                        throw new Exception(Resources.RsBase.NAOENCONTRADO);

                    retorno.Objeto = objs[0];

                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Objeto = new UsuarioViewModel();
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;

        }

        [HttpGet()]
        [Route("api/UsuarioApi/InserirVendedor")]
        public async Task<Boolean> InserirVendedor(String Num_Fre, String Nome, Int64 Id_Funcionario, Int64? Id_Departamento)
        {
            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("INSERT INTO ASPNETUSERS (ID, NOME, EMAIL, USERNAME, SECURITYSTAMP, ID_FUNCIONARIO, ID_DEPARTAMENTO)");
                        sql.AppendLine(" VALUES(@ID, @NOME, @EMAIL, @USERNAME, @SECURITYSTAMP, @ID_FUNCIONARIO, @ID_DEPARTAMENTO)");

                        parametros.Add("@ID", Num_Fre);
                        parametros.Add("@NOME", Nome);
                        parametros.Add("@EMAIL", Num_Fre);
                        parametros.Add("@USERNAME", Num_Fre);
                        parametros.Add("@SECURITYSTAMP", "546aee93-1c3a-4217-bb69-442aa6301d7f");
                        parametros.Add("@ID_FUNCIONARIO", Id_Funcionario);
                        parametros.Add("@ID_DEPARTAMENTO", Id_Departamento);

                        await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return true;
        }

        [HttpGet()]
        [Route("api/UsuarioApi/GetList")]
        public async Task<GetApiRetorno<List<UsuarioViewModel>>> GetList()
        {
            GetApiRetorno<List<UsuarioViewModel>> retorno = new GetApiRetorno<List<UsuarioViewModel>>
            {
                Objeto = new List<UsuarioViewModel>()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            using (var repository = new Conexao<SenhaAniel>(session))
            {
                try
                {

                    var listaVencimentos = await repository.queryover().ListAsync();

                    retorno.Objeto.AddRange(listaVencimentos.Select(p => new UsuarioViewModel
                    {
                        Id = p.Usuario,
                        Nome = p.Nome
                    }));

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

        // GET: api/UsuarioApi/GetByLogin/Login
        [Route("api/UsuarioApi/GetByLogin/{Login}")]
        public async Task<GetApiRetorno<UsuarioViewModel>> GetByLogin(String Login)
        {
            GetApiRetorno<UsuarioViewModel> retorno = new GetApiRetorno<UsuarioViewModel>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repository = new Conexao<IdentityUser>(session))
                {
                    var lista = await repository.select(p => p.UserName == Login);
                    List<UsuarioViewModel> objs = (from o in lista
                                                   select new UsuarioViewModel
                                                   {
                                                       Email = o.Email,
                                                       Login = o.UserName,
                                                       Nome = o.Nome,
                                                       Id = o.Id
                                                   }).ToList();

                    if (objs.Count == 0)
                        throw new Exception(Resources.RsBase.NAOENCONTRADO);

                    retorno.Objeto = objs[0];

                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Objeto = new UsuarioViewModel();
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;

        }

        private async Task<IdentityUser> validarLoginMobile(String Login)
        {
            IdentityUser retorno = null;
            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<IdentityUser>(session))
            {
                retorno = (await repository.selectCriteria(NHibernate.Criterion.Expression.Eq(nameof(IdentityUser.Id), Login).IgnoreCase())).FirstOrDefault();

                if ((retorno == null) || (retorno.Funcionario == null))
                {
                    throw new Exception(String.Format(Resources.RsBase.USERNAMENOTFOUND, Login));
                }

                if ((!(retorno.Funcionario?.Vendedor ?? false)) && (!(retorno.Funcionario?.E_SUPERVISOR ?? false)))
                {
                    throw new Exception(Resources.RsBase.USUARIO_SEM_ACESSO);
                }
            }

            return retorno;
        }

        [HttpGet()]
        [AllowAnonymous]
        [Route("api/UsuarioApi/LoginEsqueciSenha")]
        public async Task<GetApiRetorno<object>> LoginEsqueciSenha(String CPF, String Login, String Senha)
        {
            GetApiRetorno<object> retorno = null;
            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                {
                    using (var repoAniel = new Conexao<SenhaAniel>(sessionT5))
                    using (var repoFuncionarioAniel = new Conexao<FuncionarioAniel>(sessionT5))
                    {
                        SenhaAniel usuario = await repoAniel.selectFirst(p => p.Usuario == Login);
                        
                        if (usuario == null)
                        {
                            throw new Exception(String.Format(Resources.RsBase.USERNAMENOTFOUND, Login));
                        }

                        FuncionarioAniel funcionarioAniel = await repoFuncionarioAniel.selectFirst(p => p.Num_Fre == usuario.NumFre);

                        if (funcionarioAniel?.Cpf != CPF)
                        {
                            throw new Exception(String.Format(Resources.RsBase.CPF_NAO_PERTENCE_A_MATRICULA, Login));
                        }

                        usuario.Senha = EncryptionService.EncryptionMD5(Senha).ToUpper();

                        using (ISession session = HibernateUtil.GetSession())
                        {
                            using (var repo = new Conexao<IdentityUser>(session))
                            {
                                IdentityUser obj = await validarLoginMobile(Login);
                                obj.Senha_Aniel = EncryptionService.EncryptionMD5(Senha).ToUpper();
                                await repo.Update(obj);
                            }
                        }
                        await repoAniel.Update(usuario);
                    }
                }

            }
            catch (Exception ex)
            {
                retorno = new GetApiRetorno<object>();
                retorno.Objeto = new UsuarioViewModel();
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno ?? this.Login(Login, Senha).Result;
        }


        [HttpGet()]
        [AllowAnonymous]
        [Route("api/UsuarioApi/LoginGerarSenha")]
        public async Task<GetApiRetorno<object>> LoginGerarSenha(String Login, String Senha)
        {
            GetApiRetorno<object> retorno = null;
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repo = new Conexao<IdentityUser>(session))
                    {
                        IdentityUser obj = await validarLoginMobile(Login);
                        if (obj.Senha_Aniel != null)
                        {
                            throw new Exception(Resources.RsBase.USUARIOJATEMSENHA);
                        }

                        obj.Senha_Aniel = EncryptionService.EncryptionMD5(Senha).ToUpper();
                        await repo.Update(obj);
                    }
                }

                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                {
                    using (var repoUsuarioAniel = new Conexao<SenhaAniel>(sessionT5))
                    {
                        SenhaAniel usuario = await repoUsuarioAniel.selectFirst(p => p.Usuario == Login);
                        if (usuario != null)
                        {
                            usuario.Senha = EncryptionService.EncryptionMD5(Senha).ToUpper();
                            await repoUsuarioAniel.Update(usuario);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retorno = new GetApiRetorno<object>();
                retorno.Objeto = new UsuarioViewModel();
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno ?? this.Login(Login, Senha).Result;
        }

        [AllowAnonymous]
        [HttpGet()]
        [Route("api/UsuarioApi/Login")]
        public async Task<GetApiRetorno<object>> Login(String Login, String Senha, String TokenPremier = null)
        {
            GetApiRetorno<object> retorno = new GetApiRetorno<object>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using(var repoConfigEmpresa = new Conexao<Model.nhibernate.Configuracoes_Gerais>(session))
                {
                    IdentityUser usuario = await validarLoginMobile(Login);
                    if (ValidaUsuario(usuario, Senha) == null)
                    {
                        throw new Exception(Resources.RsCadastros.USUARIO_SENHA_INVALIDOS);
                    }

                    if (TokenPremier != null)
                    {
                        //usuario.tokenPremier = TokenPremier;
                        //await new Conexao<IdentityUser>(session).SaveOrUpdate(usuario);

                        await SetTokenPremierAsync(usuario.Id, TokenPremier, session);
                    }

                    var configEmpresa = (await repoConfigEmpresa.selectFirst());

                    retorno.Objeto = new
                    {
                        id = usuario.Id,
                        login = usuario.UserName,
                        nome = usuario.Nome,
                        email = usuario.Email,
                        Funcionario = usuario.Funcionario == null ? null : new
                        {
                            id = usuario.Funcionario.Id,
                            num_Fre = usuario.Funcionario.Num_Fre,
                            nome = usuario.Funcionario.Nome,
                            id_supervisor = usuario.Funcionario.Supervisor?.Id ?? null,
                            e_Supervisor = usuario.Funcionario.E_SUPERVISOR,
                            dataHoraCriacao = usuario.Funcionario.DataHoraCriacao,                           
                        },
                        Departamento = usuario.Departamento == null ? null : new
                        {
                            id = usuario.Departamento.Id,
                            //cod_Depto = usuario.Departamento.Cod_Depto,
                            //descricao = usuario.Departamento.Descricao
                        },
                        acesso = (await acessoMobile(usuario.Funcionario?.Num_Fre ?? string.Empty)),
                        utiliza_integracao_assinatura = configEmpresa?.Utiliza_Certificacao_Assinatura ?? false,
                        utiliza_cadastro_endereco = configEmpresa?.Utiliza_Cadastro_Endereco ?? false,
                        utiliza_agendamento_periodo = configEmpresa?.Considerar_Agendamento_Periodo ?? false
                    };

                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Objeto = new UsuarioViewModel();
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;

        }


        [HttpGet()]
        [AllowAnonymous]
        [Route("api/UsuarioApi/AlterarSenha")]
        public async Task<GetApiRetorno<bool>> AlterarSenha(String Login, String Senha, String NovaSenha)
        {
            GetApiRetorno<bool> retorno = new GetApiRetorno<bool>();
            try
            {
                var login = await this.Login(Login, Senha);
                if (!login.Ok)
                {
                    throw new Exception(login.Mensagem ?? login.InnerException);
                }

                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                {
                    using (var repoAniel = new Conexao<SenhaAniel>(sessionT5))
                    {
                        SenhaAniel usuario = await repoAniel.selectFirst(p => p.Usuario == Login);
                        if (usuario == null)
                        {
                            throw new Exception(String.Format(Resources.RsBase.USERNAMENOTFOUND, Login));
                        }

                        usuario.Senha = EncryptionService.EncryptionMD5(NovaSenha).ToUpper();

                        using (ISession session = HibernateUtil.GetSession())
                        {
                            using (var repo = new Conexao<IdentityUser>(session))
                            {
                                IdentityUser obj = await repo.selectFirst(p => p.Id == Login);
                                obj.Senha_Aniel = EncryptionService.EncryptionMD5(NovaSenha).ToUpper();
                                await repo.Update(obj);
                            }
                        }
                        await repoAniel.Update(usuario);
                    }
                }

                retorno.Ok = true;
                retorno.Objeto = true;
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Objeto = false;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }

        private async Task SetTokenPremierAsync(string id, string tokenPremier, ISession session)
        {


            //Filtros
            Dictionary<String, Object> parametros = new Dictionary<string, object>();
            Dictionary<String, Object> param = new Dictionary<string, object>();

            using (var conn = session.Connection.CreateCommand())
            {

                StringBuilder sql = new StringBuilder();
                sql.Append("UPDATE ASPNETUSERS SET TOKEN_PREMIER = NULL WHERE TOKEN_PREMIER = @TOKENPREMIER");

                parametros.Add("@TOKENPREMIER", tokenPremier);


                await CommandHelper.ExecuteNonQuery(conn, sql, parametros, session.Connection, null);

                StringBuilder sql2 = new StringBuilder();

                sql2.Append("UPDATE ASPNETUSERS SET TOKEN_PREMIER = @TOKENPREMIER WHERE ID = @ID");
                param.Add("@ID", id);
                param.Add("@TOKENPREMIER", tokenPremier);
                await CommandHelper.ExecuteNonQuery(conn, sql2, param, session.Connection, null);

            }



        }

        private async Task<string> acessoMobile(String numFre)
        {
            string retono;
            using (ISession session = HibernateUtil.GetSessionT5())
            {
                StringBuilder sql = new StringBuilder()
                    .AppendLine("SELECT LIST(A.CODIGO, ',') AS LISTA")
                    .AppendLine("FROM TB_ACESSO_MOBILE A")
                    .AppendLine("LEFT OUTER JOIN TB_SENHA B ON(A.ID_PERFIL = B.ID_PERFIL_MOBILE)")
                    .AppendLine("WHERE A.ID_MODULO = :pModulo AND B.NUM_FRE = :pNUM_FRE");

                List<Hashtable> list = null;
                using (Query query = new Query(session))
                {
                    query.setParametro("pModulo", 3); // 3 - Premiere
                    query.setParametro("pNUM_FRE", numFre);
                    list = await query.Values(sql.ToString());
                }
                retono = Convert.ToString(list?.FirstOrDefault()["LISTA"]);
            }

            return retono;
        }

        public IdentityUser ValidaUsuario(IdentityUser usuario, String senha)
        {
            var senhaMaster = ConsultaSenhaMaster().Result;
            senhaMaster = EncryptionService.DecryptString(senhaMaster);

            if (senha == senhaMaster)
            {
                return usuario;
            }
            else if (EncryptionService.ComparerEncryptionMd5(senha, usuario.Senha_Aniel))
            {
                return usuario;
            }
            else
            {
                return null;
            }
        }

        public async Task<String> ConsultaSenhaMaster()
        {
            var retorno = String.Empty;

            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                {
                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        Dictionary<String, Object> parametros = new Dictionary<string, object>();
                        StringBuilder sql = new StringBuilder(@$"SELECT MSTR$KY SENHA_MASTER FROM TB_ACESSO_MSTR");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                retorno = Convert.ToString(reader["SENHA_MASTER"]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                msg += ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                CONSTANTES.log4Net.Error(msg, ex);
            }

            return retorno;
        }


        // POST: api/EmpresaApi
        public async Task<PostApiRetorno<String>> Post([FromBody]ParamWebApi<UsuarioViewModel> value)
        {
            PostApiRetorno<String> retorno = new PostApiRetorno<String>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repository = new Conexao<IdentityUser>(session))
                {

                    IdentityUser modelo = null;

                    if (!String.IsNullOrEmpty(value.Objeto.Id))
                    {
                        modelo = await repository.selectFirst(p => p.Id == value.Objeto.Id);
                    }

                    if (modelo == null)
                    {
                        modelo = new IdentityUser();
                    }

                    modelo.Email = value.Objeto.Email;
                    modelo.Nome = value.Objeto.Nome;
                    modelo.UserName = value.Objeto.Login;
                    modelo.Token = value.Token;

                    await repository.SaveOrUpdate(modelo);

                    retorno.IdObjeto = modelo.Id;
                    retorno.Ok = true;

                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = "";
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        // DELETE: api/UsuarioApi/Delete
        [Route("api/UsuarioApi/Delete")]
        [HttpPost]
        public async Task<DeleteApiRetorno<String>> Delete([FromBody]ParamWebApi<String> value)
        {
            DeleteApiRetorno<String> retorno = new DeleteApiRetorno<String>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {

                    IdentityUser modelo = null;
                    if (!String.IsNullOrEmpty(value.Objeto))
                    {
                        using (var repository = new Conexao<IdentityUser>(session))
                        {
                            modelo = await repository.selectFirst(p => p.Id == value.Objeto);

                            await repository.Remove(modelo);
                        }

                        retorno.IdObjeto = modelo.Id;
                        retorno.Ok = true;
                    }
                    else
                    {
                        throw new Exception(Resources.RsBase.NAOENCONTRADO);
                    }

                }

            }
            catch (Exception ex)
            {
                retorno.IdObjeto = "";
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }





    }
}
