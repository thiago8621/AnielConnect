using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.ModelLog.nhibernate;
using Aniel.Connect.Models;
using Aniel.T5.Model.nhibernate;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using NHibernate;
using NHibernate.Criterion;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.Identity;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class IdentityUserApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/IdentityUserapi/GetAll")]
        public async Task<DataTableResponse<IdentityUserListaModel>>  GetAll(Ponto_Partida_DataTableRequest request)
        {
            List<IdentityUserListaModel> retorno = new List<IdentityUserListaModel>();
            IOrderedEnumerable<IdentityUserListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                StringBuilder sqlFrom = new StringBuilder();

                sqlFrom.AppendLine("  FROM ASPNETUSERS A ");
                sqlFrom.AppendLine("  WHERE A.E_MONITOR = 1 ");

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO ");
                    sql.AppendLine(sqlFrom.ToString());

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

                    sql.AppendLine("SELECT A.ID, A.NOME ");
                    sql.AppendLine(sqlFrom.ToString());

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine(" AND ((UPPER(A.NOME) LIKE UPPER(@SEARCH)) OR (UPPER(B.NOME) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new IdentityUserListaModel
                            {
                                Id = Convert.ToString(reader["ID"]),
                                Nome = Convert.ToString(reader["NOME"])

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
                    case 1:     // VENDEDOR
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Id);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Id);
                            break;
                        }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<IdentityUserListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }


        [HttpPost()]
        public async Task<GetApiRetorno<IEnumerable<IdentityUserListaModel>>> GetList()
        {
            GetApiRetorno<IEnumerable<IdentityUserListaModel>> retorno = new GetApiRetorno<IEnumerable<IdentityUserListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    var lista = await new Conexao<IdentityUser>(session).queryover().ListAsync();

                    retorno.Objeto = lista.Select(p => new IdentityUserListaModel
                    {
                        Id = p.Id,
                        Nome = p.UserName,
                        Departamento = Convert.ToString(p.Departamento?.Id)
                    });

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
        [Route("api/Identity_UserApi/GetCombo")]
        public async Task<ObjCombo<String>> GetCombo(RequestObjComboPai<String, Int64?> request)
        {
            ObjCombo<String> retorno = new ObjCombo<String>
            {
                results = new List<IObjComboResult<String>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM ASPNETUSERS A LEFT JOIN TB_FUNCIONARIO B ON A.ID_FUNCIONARIO = B.ID ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"B.SITUACAO IN (0,4)");
                    }

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"A.ID = '{request.id}'");
                    }

                    if (request.idpai.HasValue)
                    {
                        condicoes.Add($"A.ID_DEPARTAMENTO = {request.idpai}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER(A.USERNAME) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes) } ) ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.USERNAME ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.USERNAME)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new IdentityUserComboModel
                                {
                                    Id = Convert.ToString(reader["ID"]),
                                    Text = Convert.ToString(reader["USERNAME"]),
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
        [Route("api/Identity_UserApi/GetComboUserId")]
        public async Task<ObjCombo<String>> GetComboUserId(RequestObjComboPai<String, Int64?> request)
        {
            ObjCombo<String> retorno = new ObjCombo<String>
            {
                results = new List<IObjComboResult<String>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM ASPNETUSERS A LEFT JOIN TB_FUNCIONARIO B ON A.ID_FUNCIONARIO = B.ID ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"B.SITUACAO IN (0,4)");
                    }

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"A.ID = '{request.id}'");
                    }

                    if (request.idpai.HasValue)
                    {
                        condicoes.Add($"A.ID_DEPARTAMENTO = {request.idpai}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER(A.USERNAME) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes) } ) ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.USERNAME, A.NOME ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.USERNAME)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new IdentityUserComboModel
                                {
                                    Id = Convert.ToString(reader["ID"]),
                                    Text = String.Concat(Convert.ToString(reader["ID"]), " - ", Convert.ToString(reader["NOME"])),
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
        [Route("api/Identity_UserApi/GetComboVendedorId")]
        public async Task<ObjCombo<String>> GetComboVendedorId(RequestObjComboPai<String, Int64?> request)
        {
            ObjCombo<String> retorno = new ObjCombo<String>
            {
                results = new List<IObjComboResult<String>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM ASPNETUSERS A LEFT JOIN TB_FUNCIONARIO B ON A.ID_FUNCIONARIO = B.ID ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"B.SITUACAO IN (0,4)");
                    }

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"A.ID_FUNCIONARIO = '{request.id}'");
                    }

                    if (request.idpai.HasValue)
                    {
                        condicoes.Add($"A.ID_DEPARTAMENTO = {request.idpai}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER(A.USERNAME) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes) } ) ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.USERNAME, A.NOME ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.USERNAME)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new IdentityUserComboModel
                                {
                                    Id = Convert.ToString(reader["ID"]),
                                    Text = String.Concat(Convert.ToString(reader["USERNAME"]), " - ", Convert.ToString(reader["NOME"])),
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
        [Route("api/IdentityUserApi/GetListMonitor")]
        public async Task<GetApiRetorno<IEnumerable<IdentityUserListaModel>>> GetListMonitor(String term)
        {
            GetApiRetorno<IEnumerable<IdentityUserListaModel>> retorno = new GetApiRetorno<IEnumerable<IdentityUserListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    var lista = await new Conexao<IdentityUser>(session).select(p => p.E_Monitor && p.Nome.IsLike(term.ToUpperInvariant()));

                    retorno.Objeto = lista.Select(p => new IdentityUserListaModel
                    {
                        Id = p.Id,
                        Nome = p.UserName
                    });

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
        [Route("api/IdentityUserApi/GetComboMonitor")]
        public async Task<ObjCombo<String>> GetComboMonitor(RequestObjCombo<String> request)
        {
            ObjCombo<String> retorno = new ObjCombo<String>
            {
                results = new List<IObjComboResult<String>>()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_EQUIPE A ");
                    sqlFrom.AppendLine(" LEFT JOIN TB_SENHA B ")
                             .AppendLine(" ON A.USUARIO_MONITOR = B.USUARIO ")
                             .AppendLine(" LEFT JOIN TB_FUNCIONARIO C ")
                             .AppendLine(" ON(A.NUM_FRE = C.NUM_FRE) ")
                            .AppendLine(" WHERE C.SITUACAO != 2 ");
                    //.AppendLine(" WHERE C.SITUACAO != 2 AND A.EQUIPE_CONNECT = 'S' ")
                    //.AppendLine(" AND USUARIO_MONITOR IS NOT NULL ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        //condicoes.Add($"A.SITUACAO IN (0,4)");
                    }

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"UPPER(B.USUARIO) = '{request.id.ToUpperInvariant()}'");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(B.USUARIO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 6)}%' OR " +
                                      $" UPPER(B.NOME) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"AND ( {String.Join(" AND ", condicoes) } ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(DISTINCT B.USUARIO) AS CO ");
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
                        StringBuilder sql = new StringBuilder(" SELECT DISTINCT B.USUARIO, B.NOME");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" ORDER BY UPPER(B.NOME)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["USUARIO"]),
                                    Text = $"{Convert.ToString(reader["USUARIO"])} - {Convert.ToString(reader["NOME"])}"
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
        [Route("api/IdentityUserApi/GetComboUsuarioMonitorEquipe")]
        public async Task<ObjCombo<String>> GetComboUsuarioMonitorEquipe(RequestObjCombo<String> request)
        {
            ObjCombo<String> retorno = new ObjCombo<String>
            {
                results = new List<IObjComboResult<String>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder(@" FROM ASPNETUSERS A
                                                                 LEFT JOIN TB_FUNCIONARIO F 
                                                                 ON(F.ID = A.ID_FUNCIONARIO)
                                                                WHERE A.SECAO_CONNECT = 2 AND A.TIPO_USUARIO = 4 AND F.SITUACAO <> 2 ");


                    if (!String.IsNullOrEmpty(request.q))
                    {
                        sqlFrom.AppendLine(" AND ((UPPER(A.USERNAME) LIKE @MONITOR) ");
                        sqlFrom.AppendLine(" OR (UPPER(F.NOME) LIKE @MONITOR)) ");
                        parametros.Add("@MONITOR", String.Format("%{0}%", (request.q.ToUpperInvariant())));
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(DISTINCT A.USERNAME) AS CO ");
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
                        StringBuilder sql = new StringBuilder(" SELECT A.ID, F.NOME ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" ORDER BY UPPER(F.NOME)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["ID"]),
                                    Text = $"{Convert.ToString(reader["ID"])} - {Convert.ToString(reader["NOME"])}"
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
        [Route("api/IdentityUserApi/GetComboUsuarioSupervisorEquipe")]
        public async Task<ObjCombo<String>> GetComboUsuarioSupervisorEquipe(RequestObjCombo<String> request)
        {
            ObjCombo<String> retorno = new ObjCombo<String>
            {
                results = new List<IObjComboResult<String>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder(@" FROM ASPNETUSERS A
                                                                WHERE A.SECAO_CONNECT = 2 AND A.TIPO_USUARIO = 5 ");


                    if (!String.IsNullOrEmpty(request.q))
                    {
                        sqlFrom.AppendLine(" AND (UPPER(A.USERNAME) LIKE @SUPERVISOR) ");
                        parametros.Add("@SUPERVISOR", String.Format("%{0}%", (request.q.ToUpperInvariant())));
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(DISTINCT A.USERNAME) AS CO ");
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
                        StringBuilder sql = new StringBuilder(" SELECT A.ID, A.USERNAME");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" ORDER BY UPPER(A.USERNAME)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["ID"]),
                                    Text = $"{Convert.ToString(reader["ID"])} - {Convert.ToString(reader["USERNAME"])}"
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
        [Route("api/IdentityUserApi/busca")]
        public async Task<List<ObjBusca<String>>> busca(String term)
        {
            List<ObjBusca<String>> retorno = new List<ObjBusca<String>>();

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

                        sql.AppendLine("SELECT A.ID, A.USERNAME");
                        sql.AppendLine("FROM ASPNETUSERS A");


                        sql.AppendLine("WHERE (UPPER(A.USERNAME) LIKE @VALOR)");

                        parametros.Add("@VALOR", $"%{term.ToUpperInvariant()}%");

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new ObjBusca<String>
                                {
                                    Value = Convert.ToString(reader["ID"]),
                                    Label = Convert.ToString(reader["USERNAME"])
                                });
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
        [Route("api/IdentityUserapi/FindById")]
        public async Task<GetApiRetorno<IdentityUserMonitorModel>> FindById(String Id)
        {
            GetApiRetorno<IdentityUserMonitorModel> retorno = new GetApiRetorno<IdentityUserMonitorModel>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    retorno.Objeto = await GetViewObj(await new Conexao<IdentityUser>(session).selectFirst(p => p.Id == Id));
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

        private async Task<IdentityUserMonitorModel> GetViewObj(IdentityUser obj)
        {

            List<EquipeAniel> lista = new List<EquipeAniel>();

            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                using (var repositoryT5 = new Conexao<EquipeAniel>(sessionT5))
                {
                    lista = (await repositoryT5.select(p => p.Monitor == obj.Id)).ToList();
                }

            }

            var retorno = new IdentityUserMonitorModel
            {
                Id = obj.Id,
                Desc_User = !string.IsNullOrEmpty(obj.Nome) ? obj.Nome : string.Empty,
                Equipes = (lista.Count() == 0) ? new List<EquipeDataTableListaModel>() :
                                                   lista.Select(p => new EquipeDataTableListaModel
                                                   {
                                                       Id = Convert.ToInt64(p.Matricula),
                                                       Equipe = p.Matricula,
                                                       Nome = p.Nome

                                                   }).ToList()
            };

            return retorno;
        }

        [HttpPost()]
        [Route("api/IdentityUserApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<IdentityUserMonitorModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                IdentityUser modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                {
                    using (var repository = new Conexao<IdentityUser>(session))
                    using (var repositoryLog = new Conexao<LogVincularMonitorTecnico>(sessionLog))
                    {
                        if (!String.IsNullOrEmpty(value.Objeto.Id))
                        {
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();


                            modelo.E_Monitor = true;

                            List<EquipeAniel> equipesTmp = new List<EquipeAniel>();

                            var _arrayequipes = value.Objeto.Lista_Equipes.Split(',');

                            foreach (var equipe in _arrayequipes)
                            {

                                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                                {
                                    using (var repositoryT5 = new Conexao<EquipeAniel>(sessionT5))
                                    {
                                        var obj = (await repositoryT5.select(p => p.Matricula == equipe)).FirstOrDefault();
                                        var modeloAnterior = (EquipeAniel)obj.Clone();

                                        if (repositoryT5.select(p => p.Monitor == obj.Monitor && p.Matricula != obj.Matricula).Result.Count == 0)
                                        {
                                            var modeloAntigo = (await repository.select(p => p.Id == obj.Monitor)).FirstOrDefault();
                                            modeloAntigo.E_Monitor = false;
                                            await repository.SaveOrUpdate(modeloAntigo);
                                        }

                                        obj.Monitor = modelo.Id;
                                        await repositoryT5.SaveOrUpdate(obj);
                                        var logs = obj.GeraLogVincularMonitorTecnico(modeloAnterior, User.Identity.GetUserId(), modelo.Nome, Sinapse.ModelLog.TipoOperLog.Insercao);
                                        await repositoryLog.Save(logs);
                                    }

                                }

                            }

                            await repository.SaveOrUpdate(modelo);
                        }

                    }
                    retorno.Ok = true;
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
        #region Reformulação da Campanha

        [HttpPost()]
        [Route("api/IdentityUserApi/GetComboSupervisor")]
        public async Task<ObjCombo<String>> GetComboSupervisor(RequestObjCombo<String> request)
        {
            ObjCombo<String> retorno = new ObjCombo<String>
            {
                results = new List<IObjComboResult<String>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder(" FROM ASPNETUSERS A ");
                    //sqlFrom.AppendLine(" LEFT JOIN TB_SENHA B ")
                    //         .AppendLine(" ON A.USUARIO_MONITOR = B.USUARIO ")
                    //         .AppendLine(" LEFT JOIN TB_FUNCIONARIO C ")
                    sqlFrom.AppendLine(" WHERE A.SECAO_CONNECT = 1 AND A.TIPO_USUARIO = 2 ");
                    //.AppendLine(" AND USUARIO_MONITOR IS NOT NULL ");

                    List<String> condicoes = new List<String>();
                    if (request.soativos)
                    {
                        //condicoes.Add($"A.SITUACAO IN (0,4)");
                    }

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"UPPER(A.ID) = '{request.id.ToUpperInvariant()}'");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.USERNAME) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 6)}%' OR " +
                                      $" UPPER(A.NOME) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"AND ( {String.Join(" AND ", condicoes) } ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(DISTINCT A.ID) AS CO ");
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
                        StringBuilder sql = new StringBuilder(" SELECT DISTINCT A.ID, A.NOME, A.USERNAME");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" ORDER BY UPPER(A.ID)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["ID"]),
                                    Text = reader["NOME"] != DBNull.Value ? $"{Convert.ToString(reader["ID"])} - {Convert.ToString(reader["NOME"])}" :
                                    $"{Convert.ToString(reader["ID"])} - {Convert.ToString(reader["USERNAME"])}"
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
        [Route("api/IdentityUserApi/GetComboVendedor")]
        public async Task<ObjCombo<String>> GetComboVendedor(RequestObjCombo<String> request)
        {
            ObjCombo<String> retorno = new ObjCombo<String>
            {
                results = new List<IObjComboResult<String>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder(" FROM ASPNETUSERS A ");
                    //sqlFrom.AppendLine(" LEFT JOIN TB_SENHA B ")
                    //         .AppendLine(" ON A.USUARIO_MONITOR = B.USUARIO ")
                    //         .AppendLine(" LEFT JOIN TB_FUNCIONARIO C ")
                    sqlFrom.AppendLine(" WHERE A.SECAO_CONNECT = 1 AND A.TIPO_USUARIO = 3 ");
                    //.AppendLine(" AND USUARIO_MONITOR IS NOT NULL ");

                    List<String> condicoes = new List<String>();
                    if (request.soativos)
                    {
                        //condicoes.Add($"A.SITUACAO IN (0,4)");
                    }

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"UPPER(A.ID) = '{request.id.ToUpperInvariant()}'");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.USERNAME) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 6)}%' OR " +
                                      $" UPPER(A.NOME) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"AND ( {String.Join(" AND ", condicoes) } ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(DISTINCT A.ID) AS CO ");
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
                        StringBuilder sql = new StringBuilder(" SELECT DISTINCT A.ID, A.NOME, A.USERNAME");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" ORDER BY UPPER(A.ID)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["ID"]),
                                    Text = reader["NOME"] != DBNull.Value ? $"{Convert.ToString(reader["ID"])} - {Convert.ToString(reader["NOME"])}" :
                                    $"{Convert.ToString(reader["ID"])} - {Convert.ToString(reader["USERNAME"])}"
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
        #endregion


        [AcceptVerbs("GET", "POST")]
        [Route("api/IdentityUserApi/VincularMonitorAoTecnico")]
        public async Task<PostApiRetorno<Int64>> VincularMonitorAoTecnico(string ids)
        {
            List<String> ArgumentsIds = JsonConvert.DeserializeObject<List<String>>(ids);
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                using (var repository = new Conexao<EquipeAniel>(session))
                using (var repositoryLog = new Conexao<LogVincularMonitorTecnico>(sessionLog))
                {
                    EquipeAniel modelo = null;
                    EquipeAniel modeloAnterior = null;

                    List<EquipeListaModel> lista = new List<EquipeListaModel>();
                    List<EquipeAniel> equipe = (await repository.select(p => p.Matricula.IsIn(ArgumentsIds))).ToList();

                    foreach (var item in equipe)
                    {
                        modelo = (await repository.select(p => p.Matricula == item.Matricula)).FirstOrDefault();
                        modeloAnterior = (EquipeAniel)modelo.Clone();

                        item.Monitor = item.Monitor;
                        item.DataHoraAlteracao = DateTime.Now;
                        item.UsuarioAlteracao = User.Identity.GetUserId();

                        try
                        {
                            var usuarioAlteracao = User.Identity.GetUserId();

                            var logs = modelo.GeraLogVincularMonitorTecnico(modeloAnterior, usuarioAlteracao, modelo.Nome, Sinapse.ModelLog.TipoOperLog.Insercao);

                            await repository.Update(modelo);
                            await repositoryLog.Save(logs);

                        }
                        catch (Exception ex)
                        {
                            retorno.Mensagem = ex.Message;
                            if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                        }

                        var notificacoes = new NotificacoesApiController();
                        var retornoNotificacao = notificacoes.EnviaNotificacoesServico(11, null, null, null, null, modeloAnterior.Monitor).Result;

                        await repository.SaveOrUpdate(item);
                    }
                    //var lista_modificada = equipe.Select(p=>{ p.Monitor = null;p.DataHoraAlteracao = DateTime.Now; p.UsuarioAlteracao = User.Identity.GetUserId(); return p; }).ToList();

                    retorno.IdObjeto = lista.Count();
                    retorno.Ok = true;
                }

            }
            catch (Exception e)
            {

                retorno.IdObjeto = 0;
                retorno.Ok = false;
                retorno.Mensagem = e.Message;
                retorno.InnerException = e.InnerException?.Message;

            }

            return retorno;
        }


    }
}