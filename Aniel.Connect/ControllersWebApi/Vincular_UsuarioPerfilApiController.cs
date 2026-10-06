using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.ModelLog.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.Repository.BancoFoto.AssinaturaEquipe;
using Aniel.Connect.Repository.T5.Equipe;
using Aniel.T5.Model.nhibernate;
using DocumentFormat.OpenXml.Bibliography;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web;
using Aniel.Connect.Core.Helpers;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class Vincular_UsuarioPerfilApiController : BaseApiController
    {


        [HttpPost()]
        [Route("api/Vincular_UsuarioPerfilApi/GetAll")]
        public async Task<DataTableResponse<Vincular_UsuarioPerfilListaModel>> GetAll(Vinculo_UsuarioPerfil_DataTableRequest request)
        {

            try
            {
                List<Vincular_UsuarioPerfilListaModel> retorno = new List<Vincular_UsuarioPerfilListaModel>();
                IOrderedEnumerable<Vincular_UsuarioPerfilListaModel> listaOrdenada = null;
                Int32 recordsTotal = 0;

                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (ISession session = HibernateUtil.GetSession())
                {

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    bool filtros = false, multiploFiltro = false;

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {

                        StringBuilder sql = new StringBuilder();
                        StringBuilder sqlWhere = new StringBuilder();


                        sql.AppendLine(" SELECT A.ID AS LOGIN,  COALESCE(A.USERNAME,'') AS USUARIO, COALESCE(B.DESCRICAO,'') AS PERFIL,COALESCE(D.NOME,'') AS NOME, COALESCE(A.SECAO_CONNECT,'3') AS SECAO, ");
                        sql.AppendLine(" COALESCE(A.TIPO_USUARIO,'6') AS TIPO, COALESCE(C.DESCRICAO,'') AS AGENTE,D.E_SUPERVISOR,D.NUM_FRE,D.SITUACAO, E.DESCRICAO AS DEPARTAMENTO, F.DESCRICAO AS PERFIL_VIS_SERV, G.DESCRICAO AS PERFIL_VIS_VENDA ");

                        sql.AppendLine(" FROM ASPNETUSERS A ");

                        sql.AppendLine(" LEFT JOIN TB_PERFIL_DASHBOARD_CONNECT B ");
                        sql.AppendLine(" ON(A.ID_PERFIL_DASH_CONNECT = B.ID AND B.SISTEMA = 1) ");

                        sql.AppendLine(" LEFT JOIN TB_AGENTE C ");
                        sql.AppendLine(" ON(A.ID_PERFIL_AGENTE = C.ID) ");

                        sql.AppendLine(" LEFT JOIN TB_FUNCIONARIO D ");
                        sql.AppendLine(" ON(D.ID = A.ID_FUNCIONARIO) ");

                        sql.AppendLine(" LEFT JOIN TB_DEPARTAMENTO E ");
                        sql.AppendLine(" ON(E.ID = A.ID_DEPARTAMENTO) ");

                        sql.AppendLine(" LEFT JOIN TB_PERFIL_VIS_SERV F ");
                        sql.AppendLine(" ON(F.ID = A.ID_PERFIL_VIS_SERV) ");
                        
                        sql.AppendLine(" LEFT JOIN TB_PERFIL_VIS_VENDA G ");
                        sql.AppendLine(" ON(G.ID = A.ID_PERFIL_VIS_VENDA) ");

                        sql.AppendLine(" WHERE D.SITUACAO <> 2 AND A.ATIVO <> 0 ");

                        if (request.Configurados || request.NaoConfigurados || request.Perfil.HasValue || request.PerfilAgente.HasValue ||request.PerfilVenda.HasValue || request.Secao.HasValue || request.TipoUsuario.HasValue || !String.IsNullOrEmpty(request.Nome))
                        {
                            sqlWhere.AppendLine($" AND ");
                            filtros = true;
                        }

                        //Filtrando
                        if (!String.IsNullOrEmpty(request.search.value))
                        {
                            sqlWhere.AppendLine($" ((UPPER(A.USERNAME) LIKE '%{request.search.value.ToUpper()}%')) ");
                            multiploFiltro = true;
                        }

                        if (request.Configurados)
                        {
                            if (multiploFiltro)
                                sqlWhere.Append(" AND ");

                            sqlWhere.AppendLine($" A.SECAO_CONNECT IS NOT NULL AND A.TIPO_USUARIO IS NOT NULL ");
                            multiploFiltro = true;
                            filtros = true;
                        }

                        if (request.NaoConfigurados)
                        {
                            if (multiploFiltro)
                                sqlWhere.Append(" AND ");

                            sqlWhere.AppendLine($" A.SECAO_CONNECT IS NULL AND A.TIPO_USUARIO IS NULL ");
                            multiploFiltro = true;
                            filtros = true;
                        }

                        if (request.Perfil.HasValue)
                        {
                            if (multiploFiltro)
                                sqlWhere.Append(" AND ");

                            sqlWhere.AppendLine($" B.ID = {request.Perfil} ");
                            multiploFiltro = true;
                            filtros = true;
                        }

                        //if (request.PerfilVenda.HasValue)
                        //{
                        //    if (multiploFiltro)
                        //        sqlWhere.Append(" AND ");

                        //    sqlWhere.AppendLine($" G.ID = {request.PerfilVenda} ");
                        //    multiploFiltro = true;
                        //    filtros = true;
                        //}

                        if (!String.IsNullOrEmpty(request.Nome))
                        {
                            if (multiploFiltro)
                                sqlWhere.Append(" AND ");

                            sqlWhere.AppendLine($" (( UPPER(A.ID) LIKE '%{request.Nome.ToUpper()}%') OR ( UPPER(A.USERNAME) LIKE '%{request.Nome.ToUpper()}%') OR (UPPER(D.NOME) LIKE  '%{request.Nome.ToUpper()}%'))  ");
                            multiploFiltro = true;
                            filtros = true;
                        }

                        if (request.PerfilAgente.HasValue)
                        {
                            if (multiploFiltro)
                                sqlWhere.Append(" AND ");

                            sqlWhere.AppendLine($" C.ID = {request.PerfilAgente} ");
                            multiploFiltro = true;
                            filtros = true;
                        }

                        if (request.Secao.HasValue)
                        {
                            if (multiploFiltro)
                                sqlWhere.Append(" AND ");

                            sqlWhere.AppendLine($" A.SECAO_CONNECT = {request.Secao} ");
                            multiploFiltro = true;
                            filtros = true;
                        }

                        if (request.TipoUsuario.HasValue)
                        {
                            if (multiploFiltro)
                                sqlWhere.Append(" AND ");

                            sqlWhere.AppendLine($" A.TIPO_USUARIO = {request.TipoUsuario} ");
                            multiploFiltro = true;
                            filtros = true;
                        }
                        
                        if (filtros)
                            sql.Append(sqlWhere);

                        //busca todas equipas habilitadas no connect
                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                var num_fre = Convert.ToString(reader["NUM_FRE"]);
                                var situacao = Convert.ToString(reader["SITUACAO"]);

                                retorno.Add(new Vincular_UsuarioPerfilListaModel
                                {
                                    Id = Convert.ToString(reader["LOGIN"]) ?? "",
                                    Usuario = Convert.ToString(reader["USUARIO"]) ?? "",
                                    PerfilDashboard = Convert.ToString(reader["PERFIL"]) ?? "",
                                    Secao = Resources.RsCadastros.ResourceManager.GetString(Enum.GetValues(typeof(Sinapse.Model.enumerados.SecaoConnectPerfilEnum)).GetValue(Convert.ToInt32(reader["SECAO"])).ToString()) ?? "3",
                                    Tipo = reader["TIPO"] != DBNull.Value && Convert.ToInt32(reader["TIPO"]) == 1 ? "Supervisor de Atendente" : Resources.RsCadastros.ResourceManager.GetString(Enum.GetValues(typeof(Sinapse.Model.enumerados.TipoUsuarioEnum)).GetValue(Convert.ToInt32(reader["TIPO"])).ToString()) ?? "6",
                                    Nome = Convert.ToString(reader["NOME"]) ?? "",
                                    PerfilAgente = Convert.ToString(reader["AGENTE"]) ?? "",
                                    Departamento = Convert.ToString(reader["DEPARTAMENTO"]),
                                    PerfilVisualizacaoServico = Convert.ToString(reader["PERFIL_VIS_SERV"]),
                                    PerfilVisualizacaoVenda = Convert.ToString(reader["PERFIL_VIS_VENDA"]),                                    
                                });
                            }
                        }

                        recordsTotal = retorno.Count;
                        retorno = retorno.Skip(request.start).Take(request.length).ToList();

                    }

                    //Ordenacao
                    if (request.order != null && request.order.Any())
                    {
                        int sortColumnIndex = request.order[0].column;
                        string sortDirection = request.order[0].dir;
                        switch (sortColumnIndex)
                        {
                            case 1:     // Nome
                                {
                                    if (sortDirection == "asc")
                                        listaOrdenada = retorno.OrderBy(p => p.Usuario);
                                    else
                                        listaOrdenada = retorno.OrderByDescending(p => p.Usuario);
                                    break;
                                }
                            default:
                                {
                                    listaOrdenada = retorno.OrderBy(p => p.Id);
                                    break;
                                }
                        }
                    }



                    //Page e retornando DataTable
                    return new DataTableResponse<Vincular_UsuarioPerfilListaModel>
                    {
                        draw = request.draw,
                        //recordsTotal = recordsTotal,
                        recordsFiltered = recordsTotal,
                        data = listaOrdenada,
                        error = ""
                    };
                }
            }
            catch (Exception e)
            {
                return new DataTableResponse<Vincular_UsuarioPerfilListaModel>
                {
                    error = e.Message
                };
            }

        }

        private async Task<string> GetEquipeHabilitadaMobile(string num_fre, ISession sessionT5)
        {
            String retorno = null;
            Dictionary<String, Object> parametros = new Dictionary<string, object>();
            StringBuilder sql = new StringBuilder(" SELECT EQUIPE FROM TB_EQUIPE WHERE NUM_FRE = @NUM_FRE AND MOBILE = 'S'");
            parametros.Add("@NUM_FRE", num_fre);

            using (var com = sessionT5.Connection.CreateCommand())
            {
                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        retorno = Convert.ToString(reader["EQUIPE"]);
                    }
                }
            }

            return retorno;
        }

        private async Task<string> GetEquipeHabilitadaConnect(string num_fre, ISession sessionT5)
        {
            String retorno = null;
            Dictionary<String, Object> parametros = new Dictionary<string, object>();
            StringBuilder sql = new StringBuilder(" SELECT EQUIPE FROM TB_EQUIPE WHERE NUM_FRE = @NUM_FRE AND EQUIPE_CONNECT = 'S'");
            parametros.Add("@NUM_FRE", num_fre);

            using (var com = sessionT5.Connection.CreateCommand())
            {
                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        retorno = Convert.ToString(reader["EQUIPE"]);
                    }
                }
            }

            return retorno;
        }

        private async Task<List<string>> GetEquipeHabilitadaConnect(ISession sessionT5)
        {
            List<String> retorno = new List<string>();
            Dictionary<String, Object> parametros = new Dictionary<string, object>();
            StringBuilder sql = new StringBuilder(" SELECT NUM_FRE FROM TB_EQUIPE WHERE EQUIPE_CONNECT = 'S'");
            
            using (var com = sessionT5.Connection.CreateCommand())
            {
                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        retorno.Add(Convert.ToString(reader["NUM_FRE"]));
                    }
                }
            }

            return retorno;
        }

        private Vincular_UsuarioPerfilCadastroModel GetViewObj(IdentityUser obj)
        {
            Vincular_UsuarioPerfilCadastroModel retorno = new Vincular_UsuarioPerfilCadastroModel();

            retorno.Id = Convert.ToString(obj.Id);
            retorno.Usuario = obj.UserName;
            retorno.Id_Perfil = obj.Perfil_Dash_Connect?.Id;
            retorno.Perfil = obj.Perfil_Dash_Connect?.Descricao;
            retorno.SecaoConnect = obj.Secao_Connect?.ToString()?? "";
            retorno.TipoUsuario = obj.Tipo_Usuario?.ToString() ?? "";
            retorno.PerfilAgente = obj.Id_Perfil_Agente?.ToString() ?? "";
            retorno.Id_Departamento = obj.Departamento?.Id;
            retorno.IdPerfilVisualizacaoServico = obj.PerfilVisualizacaoServico?.Id;
            retorno.IdPerfilVisualizacaoVenda = obj.PerfilVisualizacaoVenda?.Id;
            retorno.IdParamPreQual = obj.ParametrosConsultaPreQual?.Id;
            retorno.Modelo_Unico_Permitido_ID = obj.Modelo_Unico_Permitido_ID;
            retorno.Bloqueia_Modelo_Visualizacao = obj.Bloqueia_Modelo_Visualizacao;
            return retorno;
        }

        private void BindViewToObj(ISession session, ISession sessionT5, IdentityUser objBanco, Vincular_UsuarioPerfilCadastroModel view)
        {
            using (var repoDepto = new Conexao<Departamento>(session))
            using (var repoPerfilDashConnect = new Conexao<Perfil_DashBoard_Connect>(session))
            {

                objBanco.Perfil_Dash_Connect = repoPerfilDashConnect.selectFirst(p => p.Id == view.Id_Perfil).Result;
                objBanco.Id_Perfil_Agente = view.TipoUsuario == "0" ? Convert.ToInt64(view.PerfilAgente) : (long?)null;
                objBanco.Id_Perfil_Agente = Convert.ToInt64(view.PerfilAgente);
                objBanco.Secao_Connect = String.IsNullOrEmpty(view.SecaoConnect) ? (Int64?)null : Convert.ToInt64(view.SecaoConnect);
                objBanco.Tipo_Usuario = String.IsNullOrEmpty(view.TipoUsuario) ? (Int64?)null : Convert.ToInt64(view.TipoUsuario);
                objBanco.Departamento = repoDepto.selectFirst(p => p.Id == view.Id_Departamento).Result;

                if (view.IdPerfilVisualizacaoServico.HasValue)
                {
                    using (var repoPerfilVisServ = new Conexao<PerfilVisualizacaoServico>(session))
                    {
                        objBanco.PerfilVisualizacaoServico = repoPerfilVisServ.selectFirst(p => p.Id == view.IdPerfilVisualizacaoServico.Value).Result;
                    }
                } 
                else
                {
                    objBanco.PerfilVisualizacaoServico = null;
                }
                
                if (view.IdPerfilVisualizacaoVenda.HasValue)
                {
                    using (var repoPerfilVisVenda = new Conexao<PerfilVisualizacaoVenda>(session))
                    {
                        objBanco.PerfilVisualizacaoVenda = repoPerfilVisVenda.selectFirst(p => p.Id == view.IdPerfilVisualizacaoVenda.Value).Result;
                    }
                } 
                else
                {
                    objBanco.PerfilVisualizacaoVenda = null;
                }
                
                if (view.IdParamPreQual.HasValue)
                {
                    using (var repoParamPreQual = new Conexao<ParametrosConsultaPreQual>(session))
                    {
                        objBanco.ParametrosConsultaPreQual = repoParamPreQual.selectFirst(p => p.Id == view.IdParamPreQual.Value).Result;
                    }
                } 
                else
                {
                    objBanco.ParametrosConsultaPreQual = null;
                }

                objBanco.Bloqueia_Modelo_Visualizacao = view.Bloqueia_Modelo_Visualizacao;

                // 2. Mapeia o ID do Modelo (Modelo_Unico_Permitido_ID)
                if (view.Bloqueia_Modelo_Visualizacao)
                {
                    // Se o bloqueio estiver TRUE, tentamos salvar o ID do modelo selecionado.
                    // Usamos .HasValue para garantir que null é salvo se o DropDown estiver vazio/não-selecionado.
                    objBanco.Modelo_Unico_Permitido_ID = view.Modelo_Unico_Permitido_ID.HasValue
                                                        ? view.Modelo_Unico_Permitido_ID
                                                        : null;
                }
                else
                {
                    // Se o bloqueio estiver FALSE, garantimos que o ID salvo seja NULL, 
                    // limpando qualquer configuração anterior.
                    objBanco.Modelo_Unico_Permitido_ID = null;
                }
            }
        }

        private List<IdentityUser> BindViewToObj(ISession session, List<IdentityUser> objBanco, Vincular_MultiploPerfilCadastroModel view, List<string> ids)
        {
            using (var repoUser = new Conexao<IdentityUser>(session))
            using (var repoPerfilDashConnect = new Conexao<Perfil_DashBoard_Connect>(session))
            {
                foreach (string id in ids)
                {
                    var modelo = repoUser.selectFirst(p => Convert.ToString(p.Id) == id).Result;

                    if (modelo != null)
                    {
                        modelo.Perfil_Dash_Connect = repoPerfilDashConnect.selectFirst(p => p.Id == view.Id_Perfil).Result;
                        modelo.Id_Perfil_Agente = view.TipoUsuario == "0" ? Convert.ToInt64(view.PerfilAgente) : (long?)null;
                        modelo.Secao_Connect = Convert.ToInt64(view.SecaoConnect);
                        modelo.Tipo_Usuario = Convert.ToInt64(view.TipoUsuario);

                        if (view.IdPerfilVisualizacaoServico.HasValue)
                        {
                            using (var repoPerfilVisServ = new Conexao<PerfilVisualizacaoServico>(session))
                            {
                                modelo.PerfilVisualizacaoServico = repoPerfilVisServ.selectFirst(p => p.Id == view.IdPerfilVisualizacaoServico.Value).Result;
                            }
                        }
                        else
                        {
                            modelo.PerfilVisualizacaoServico = null;
                        }
                        
                        if (view.IdPerfilVisualizacaoVenda.HasValue)
                        {
                            using (var repoPerfilVisVenda = new Conexao<PerfilVisualizacaoVenda>(session))
                            {
                                modelo.PerfilVisualizacaoVenda = repoPerfilVisVenda.selectFirst(p => p.Id == view.IdPerfilVisualizacaoVenda.Value).Result;
                            }
                        }
                        else
                        {
                            modelo.PerfilVisualizacaoVenda = null;
                        }
                    

                 modelo.Bloqueia_Modelo_Visualizacao = view.Bloqueia_Modelo_Visualizacao;

                        // 2. Mapeia o ID do Modelo (Modelo_Unico_Permitido_ID)
                        if (view.Bloqueia_Modelo_Visualizacao)
                        {
                            // Se o bloqueio estiver TRUE, tentamos salvar o ID do modelo selecionado.
                            // Usamos .HasValue para garantir que null é salvo se o DropDown estiver vazio/não-selecionado.
                            modelo.Modelo_Unico_Permitido_ID = view.Modelo_Unico_Permitido_ID.HasValue
                                                                ? view.Modelo_Unico_Permitido_ID
                                                                : null;
                        }
                        else
                        {
                            // Se o bloqueio estiver FALSE, garantimos que o ID salvo seja NULL, 
                            // limpando qualquer configuração anterior.
                            modelo.Modelo_Unico_Permitido_ID = null;
                        }
                        if (view.IdParamPreQual.HasValue)
                        {
                            using (var repoParamPreQual = new Conexao<ParametrosConsultaPreQual>(session))
                            {
                                modelo.ParametrosConsultaPreQual = repoParamPreQual.selectFirst(p => p.Id == view.IdParamPreQual.Value).Result;
                            }
                        }
                        else
                        {
                            modelo.ParametrosConsultaPreQual = null;
                        }

                        objBanco.Add(modelo);
                    }
                } 
            }

            return objBanco;
        }


        [HttpGet()]
        [Route("api/Vincular_UsuarioPerfilApi/FindById")]
        public async Task<GetApiRetorno<Vincular_UsuarioPerfilCadastroModel>> FindById(String Id)
        {
            GetApiRetorno<Vincular_UsuarioPerfilCadastroModel> retorno = new GetApiRetorno<Vincular_UsuarioPerfilCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            //using (var repositoryPerfil = new Conexao<VincularUsuarioPerfil>(session))
            using (var repositoryPerfilVinculado = new Conexao<Perfil_Agente>(session))
            using (var repository = new Conexao<IdentityUser>(session))     
            using (var repositoryPerfilAgente = new Conexao<Perfil_Agente>(session)) 

            {
                try
                {
                    retorno.Objeto = GetViewObj(await new Conexao<IdentityUser>(session).selectFirst(p => p.Id == Id));
                    //var vincularUsario = await repositoryPerfil.select(p => p.IdentityUser.Id == Id);

                    //retorno.Objeto.Vincular = vincularUsario.Select(p =>
                    //{                       
                    //    var obj = new VincularCadastroModel();
                    //    obj.Id = p.Id;
                    //    obj.PerfilAgente = p.Perfil_Vinculado;
                    //    obj.Desc_Perfil_Vinculado = String.IsNullOrEmpty(p.Tipo_Usuario) ? String.Empty : repositoryPerfilAgente.selectFirst(x => x.Id == Convert.ToInt32(p.Perfil_Vinculado)).Result?.Descricao;
                    //    obj.SecaoConnect = p.Secao;
                    //    obj.Desc_Secao = HtmlEnumExtensions.GetEnumDescription<Model.enumerado.SecaoConnectPerfilEnum>(Convert.ToString(p.Secao));
                    //    obj.TipoUsuario = p.Tipo_Usuario;
                    //    obj.Desc_Tipo_Usuario = HtmlEnumExtensions.GetEnumDescription<Model.enumerado.TipoUsuarioEnum>(Convert.ToString(p.Tipo_Usuario));

                    //    return obj;

                    //}).ToList();

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

        [HttpPost()]
        [Route("api/Vincular_UsuarioPerfilApi/GetCombo")]
        public async Task<ObjCombo<String>> GetCombo(RequestObjComboPai<String, String> request)
        {
            ObjCombo<String> retorno = new ObjCombo<String>
            {
                results = new List<IObjComboResult<String>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Perfil_Agente>(session))
            {
                var lista = await repository.select();

                if (!String.IsNullOrEmpty(request.id))
                {
                    lista = lista.ToList().FindAll(p => p.Id == Convert.ToInt64(request.id));
                }

                foreach (var agente in lista)
                {
                    var obj = new ObjComboResult<String>
                    {
                        Id = Convert.ToString(agente.Id),
                        Text = agente.Descricao
                    };

                    retorno.results.Add(obj);
                    retorno.total_count = retorno.results.Count;
                }

            }

            return retorno;

        }

        [HttpPost()]
        [Route("api/Vincular_UsuarioPerfilApi/Gravar")]
        public async Task<PostApiRetorno<String>> Gravar([FromBody] ParamWebApi<Vincular_UsuarioPerfilCadastroModel> value)
        {
            PostApiRetorno<String> retorno = new PostApiRetorno<String>();

            try
            {
                IdentityUser modelo = null;
                IdentityUser modeloAnterior = null;
                Perfil_DashBoard_Connect modeloVincular = null;
                //GerarLogUsuarioPerfil modeloGravarLog = null;


                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                using (ISession sessionUser = HibernateUtil.GetSession())
                {
                    //Connect
                    using (var repository = new Conexao<IdentityUser>(session))
                    using (var repoPerfilDashConnect = new Conexao<Perfil_DashBoard_Connect>(session))
                    //using (var repoVincularUsuario = new Conexao<VincularUsuarioPerfil>(session))

                    {
                        //IdentityUser usuario = (await repoUser.selectFirst(p => p.Id == value.Usuario));
                        if (!String.IsNullOrEmpty(value.Objeto.Id))
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();


                        if (modelo != null)
                        {
                            modeloAnterior = (IdentityUser)modelo.Clone();
                        }

                        BindViewToObj(session, sessionT5, modelo, value.Objeto);
                      

                        //foreach (var vicularUsuario in value.Objeto.Vincular)
                        //{
                            
                        //     var vincular = new VincularUsuarioPerfil
                        //    {
                        //        Id = vicularUsuario.Id,
                        //        Tipo_Usuario = vicularUsuario.TipoUsuario,
                        //        Perfil_Vinculado = vicularUsuario.PerfilAgente,
                        //        Secao = vicularUsuario.SecaoConnect,
                        //        Perfil_DashBoard_Connect = modeloVincular,
                        //        IdentityUser = modelo

                        //    };

                        //    await repoVincularUsuario.SaveOrUpdate(vincular);

                        //}

                        //Aniel Coordenador/Supersivor
                        using (var repositoryT5 = new Conexao<FuncionarioAniel>(sessionT5))
                        {
                            if (String.IsNullOrEmpty(modelo?.Funcionario?.Num_Fre) == false)
                            {
                                var modeloAniel = await repositoryT5.selectFirst(p => p.Num_Fre == modelo.Funcionario.Num_Fre);
                                if (modelo.Tipo_Usuario == 5)
                                {
                                    if (modeloAniel != null)
                                    {
                                        modeloAniel.Supervisor = "S";
                                    }
                                }

                                if (modeloAniel != null)
                                {
                                    modeloAniel.Cod_Depto = String.IsNullOrEmpty(modelo?.Departamento?.Cod_Depto) ? (Int16?)null : Convert.ToInt16(modelo?.Departamento?.Cod_Depto);
                                    await repositoryT5.SaveOrUpdate(modeloAniel);
                                };

                               
                            }

                        }



                        ////  log
                        try
                        {
                            if (modeloAnterior != null)
                            {
                                using (var repositoryLogs = new Conexao<LogVincularPerfilBase>(sessionLog))
                                {
                                    var logs = modelo.GeraLog(modeloAnterior, modelo.Id, modelo.Nome, Sinapse.ModelLog.TipoOperLog.Alteracao);
                                    await repositoryLogs.Save(logs);
                                }
                            }
                            else
                            {
                                using (var repositoryLogs = new Conexao<LogVincularPerfilBase>(sessionLog))
                                {
                                    var logs = modelo.GeraLog(modeloAnterior, modelo.Id, modelo.Nome, Sinapse.ModelLog.TipoOperLog.Insercao);
                                    await repositoryLogs.Save(logs);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            retorno.IdObjeto = null;
                            retorno.Mensagem = ex.Message;
                            if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                        }

                        repository.SaveOrUpdate(modelo).Wait();
                        await repository.SaveOrUpdate(modelo);
                    }

                    retorno.IdObjeto = Convert.ToString(modelo.Id);
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

        [HttpPost]
        [Route("api/Vincular_UsuarioPerfilApi/RemoverPerfis")]
        public async Task<PostApiRetorno<String>> RemoverPerfis(RemoveUsuario ids)
        {
            PostApiRetorno<String> retorno = new PostApiRetorno<String>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repository = new Conexao<IdentityUser>(session))
                {

                    var usuarios = await repository.select();
                    List<IdentityUser> usuariosBanco = new List<IdentityUser>();

                    foreach (string idUsu in ids.Ids)
                    {
                        var usuario = usuarios.Where(x => Convert.ToString(x.Id) == idUsu).FirstOrDefault();

                        usuario.Id_Perfil_Agente = null;
                        usuario.Perfil_Dash_Connect = null;
                        usuario.Secao_Connect = null;
                        usuario.Tipo_Usuario = null;

                        usuariosBanco.Add(usuario);

                    }

                    await repository.SaveOrUpdate(usuariosBanco);

                    retorno.IdObjeto = usuariosBanco.Count.ToString();
                    retorno.Ok = true;
                }

            }
            catch (Exception e)
            {

                retorno.IdObjeto = "0";
                retorno.Ok = false;
                retorno.InnerException = e.Message + " / " + e.InnerException.Message;

            }

            return retorno;
        }

        [HttpPost]
        [Route("api/Vincular_UsuarioPerfilApi/ResetarAssinatura")]
        public async Task<PostApiRetorno<String>> ResetarAssinatura(RemoveUsuario ids)
        {
            PostApiRetorno<String> retorno = new PostApiRetorno<String>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (ISession sessionFoto = HibernateUtil.GetSessionFoto())
                using (var repository = new Conexao<IdentityUser>(session))
                {



                    IEquipeAnielRepository equipeAnielRepository = new EquipeAnielRepository(sessionT5);

                    foreach (string idUsu in ids.Ids)
                    {
                        IdentityUser usuario = await repository.selectFirst(x => x.Id == idUsu);

                        EquipeAniel equipe =  equipeAnielRepository.GetAll().Where(x => x.Num_Fre == usuario.Funcionario.Num_Fre).FirstOrDefault();

                        IAssinaturaEquipeRepository assinaturaEquipeRepository = new AssinaturaEquipeRepository(sessionFoto);
                        var existeassinatura = assinaturaEquipeRepository.ExisteAssinaturaEquipe(equipe.Matricula);
                        if (existeassinatura == false) {

                            throw new Exception("Funcionário sem Assinatura Vinculada.");                            
                        }else
                        {
                            var resetaassinatura =  assinaturaEquipeRepository.ResetaAssinatura(equipe.Matricula);
                        }                  

                     
                    }

                    retorno.Ok = true;
                }

            }
            catch (Exception e)
            {

                retorno.IdObjeto = "0";
                retorno.Ok = false;
                retorno.Mensagem = e.Message;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Vincular_UsuarioPerfilApi/HistoricoAlteracao")]
        public async Task<DataTableResponse<VincularUsuarioVendaModel>> HistoricoAlteracao(HistoricoAlteracaoPVincularUsuario_DataTableRequest request)
        {
            List<VincularUsuarioVendaModel> retornoLog = new List<VincularUsuarioVendaModel>();
            List<VincularUsuarioVendaModel> retorno = new List<VincularUsuarioVendaModel>();

            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSessionLog())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sqlFrom = new StringBuilder();


                sqlFrom.AppendLine("FROM TB_VINCULAR_USUARIOlOG A ");

                if (!String.IsNullOrEmpty(request.search.value))
                {
                    sqlFrom.AppendLine(" AND ((UPPER(A.OBJETO) LIKE UPPER(@SEARCH))) ");
                    parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                }

                if (request.Id.HasValue)
                {
                    sqlFrom.AppendLine(" AND A.IDLOG = @ID ");
                    parametros.Add("@ID", request.Id);
                }

                if (request.TipoOperacao != null)
                {
                    sqlFrom.AppendLine(" AND A.TIPOOPERACAO = @TIPOOPERACAO ");
                    parametros.Add("@TIPOOPERACAO", Convert.ToInt32(request.TipoOperacao));
                }

                if (!String.IsNullOrEmpty(request.Objeto))
                {
                    sqlFrom.AppendLine(" AND A.OBJETO = @OBJETO ");
                    parametros.Add("@OBJETO", request.Objeto);
                }

                if (!String.IsNullOrEmpty(request.Campo))
                {
                    sqlFrom.AppendLine(" AND A.CAMPO = @CAMPO ");
                    parametros.Add("@CAMPO", request.Campo);
                }

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(COD_VINCULAR_USUARIO) CO ");

                    sql.Append(sqlFrom.ToString());

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

                    sql.AppendLine("SELECT A.COD_VINCULAR_USUARIO, A.NOMEUSUARIOCRIACAO, A.USUARIOCRIACAO, A.DATAHORACRIACAO, A.TIPOOPERACAO, A.IDLOG, A.OBJETO, A.CAMPO, A.CAMPO_VALORANTES, A.CAMPO_VALORDEPOIS, A.OBSERVACAO ");
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

                    sql.Append(" ORDER BY DATAHORACRIACAO DESC ");

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retornoLog.Add(new VincularUsuarioVendaModel
                            {
                                Id = Convert.ToInt64(reader["COD_VINCULAR_USUARIO"]),
                                Chave = Convert.ToString(reader["OBJETO"]),
                                Campo = Convert.ToString(reader["CAMPO"]),
                                Campo_ValorAntes = Convert.ToString(reader["CAMPO_VALORANTES"]),
                                Campo_ValorDepois = Convert.ToString(reader["CAMPO_VALORDEPOIS"]),
                                TipoOperacao = HtmlEnumExtensions.GetEnumDescription<TipoOperacao>(Convert.ToString(reader["TIPOOPERACAO"])),
                                UsuarioCriacao = Convert.ToString(reader["USUARIOCRIACAO"]),
                                Nome_usuario = Convert.ToString(reader["NOMEUSUARIOCRIACAO"]),
                                DataHoraCriacao = reader["DATAHORACRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORACRIACAO"]).ToString("dd/MM/yyyy HH:mm") : String.Empty,
                                Observacao = Convert.ToString(reader["OBSERVACAO"]),
                            });

                        }
                    }

                }
            }

            foreach (var ret in retornoLog)
            {
                if (ret.TipoOperacao == HtmlEnumExtensions.GetEnumDescription<TipoOperacao>(Convert.ToString(TipoOperacao.Importacao)))
                {
                    ret.Observacao = ret.Campo_ValorDepois;
                    ret.Campo_ValorDepois = "-";
                    ret.Campo_ValorAntes = "-";
                }

                if (ret.TipoOperacao == HtmlEnumExtensions.GetEnumDescription<TipoOperacao>(Convert.ToString(TipoOperacao.Insercao)))
                {
                    ret.Observacao = ret.Campo_ValorDepois;
                    ret.Campo_ValorDepois = "-";
                    ret.Campo_ValorAntes = "-";
                }

                if (ret.TipoOperacao == HtmlEnumExtensions.GetEnumDescription<TipoOperacao>(Convert.ToString(TipoOperacao.Alteracao)))
                {
                    using ISession session = HibernateUtil.GetSessionT5();
                    if (ret.Campo == "Grupo Área")
                    {
                        using (var repositoryGrupoArea = new Conexao<EscritorioAniel>(session))
                        {
                            var antes = (await repositoryGrupoArea.selectFirst(p => p.Cod_Escritorio == ret.Campo_ValorAntes));
                            var depois = (await repositoryGrupoArea.selectFirst(p => p.Cod_Escritorio == ret.Campo_ValorDepois));

                            ret.Campo_ValorAntes = antes.Descricao;
                            ret.Campo_ValorDepois = depois.Descricao;
                        }
                    }
                }

                retorno.Add(ret);
            }

            //Page e retornando DataTable
            return new DataTableResponse<VincularUsuarioVendaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };

        }

        [HttpPost()]
        [Route("api/Vincular_UsuarioPerfilApi/GravarMultiplo")]
        public async Task<PostApiRetorno<String>> GravarMultiplo([FromBody] ParamWebApi<Vincular_MultiploPerfilCadastroModel> value)
        {
            PostApiRetorno<String> retorno = new PostApiRetorno<String>();

            try
            {
                List<IdentityUser> modelo = new List<IdentityUser>();

                using (ISession session = HibernateUtil.GetSession())
                using (var repository = new Conexao<IdentityUser>(session))
                {

                    var usuarios = BindViewToObj(session, modelo, value.Objeto, value.Objeto.Id[0].Split(',').ToList());

                    await repository.SaveOrUpdate(usuarios);

                    retorno.IdObjeto = Convert.ToString(usuarios.Count);
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
     

        [HttpPost()]
        [Route("api/Vincular_UsuarioPerfilApi/ExcluirVinculo")]
        public async Task<PostApiRetorno<string>> ExcluirVinculo(RemoveUsuario ids)
        {
            PostApiRetorno<string> retorno = new PostApiRetorno<string>();
            string idVinculoParaExcluirStr = null;

            try
            {
               
                if (ids == null || ids.Ids == null || !ids.Ids.Any())
                {
                    retorno.Ok = false;                   
                    return retorno;
                }

                idVinculoParaExcluirStr = ids.Ids.First();

                if (!long.TryParse(idVinculoParaExcluirStr, out long idVinculo))
                {
                    retorno.Ok = false;                   
                    return retorno;
                }

               
                using (ISession session = HibernateUtil.GetSession())
                using (var repository = new Conexao<VincularUsuarioPerfil>(session))
                {
                   
                    var vinculo = await repository.selectFirst(p => p.Id == idVinculo);

                    if (vinculo == null)
                    {
                        retorno.Ok = false;
                        retorno.Mensagem = $"Registro não encontrado!";
                        return retorno;
                    }
                    
                    using (var transaction = session.BeginTransaction())
                    {
                        
                        await session.DeleteAsync(vinculo);

                       
                        await transaction.CommitAsync();
                    }

                    retorno.Ok = true;
                    retorno.Mensagem = "Registro excluído com sucesso!";
                    retorno.IdObjeto = idVinculoParaExcluirStr;
                }
            }
            catch (Exception e)
            {
                retorno.Ok = false;
                retorno.IdObjeto = idVinculoParaExcluirStr;
                retorno.Mensagem = "Erro ao excluir o registro.";
                if (e.InnerException != null)
                {
                    retorno.InnerException = e.Message + " / " + e.InnerException.Message;
                }
                else
                {
                    retorno.InnerException = e.Message;
                }
            }

            return retorno;
        }

        public class RemoveUsuario
        {
            public string[] Ids { get; set; }
        }

    }
}
