using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using NHibernate.Criterion;
using Sinapse.DAO.repository;
using Sinapse.Model;
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
    public class EmpresaApiController : BaseApiController
    {

        [HttpGet()]
        [Route("api/EmpresaApi/GetTreeView")]
        public async Task<GetApiRetorno<List<EmpresaTreeViewModel>>> GetTreeView(Int64? IdEmpresa)
        {
            GetApiRetorno<List<EmpresaTreeViewModel>> retorno = new GetApiRetorno<List<EmpresaTreeViewModel>>
            {
                Objeto = new List<EmpresaTreeViewModel>()
            };
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder($"SELECT ID, NOME FROM TB_EMPRESA ");

                        if (IdEmpresa.HasValue)
                        {
                            sql.AppendLine($"WHERE ID = {IdEmpresa}");
                        }

                        sql.AppendLine(" ORDER BY NOME ");

                        var reader = await CommandHelper.ExecuteReader(com, sql, new Dictionary<String, Object>(), session.Connection, null);

                        while (await reader.ReadAsync())
                        {
                            retorno.Objeto.Add(new EmpresaTreeViewModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Nome = Convert.ToString(reader["NOME"])
                            });
                        }

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

        [HttpPost()]
        [Route("api/Empresaapi/GetCombo")]
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_EMPRESA A ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        //condicoes.Add($"A.STATUS = 1");
                    }


                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.NOME) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 60)}%')");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.NOME ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.NOME)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt32(reader["ID"]),
                                    Text = Convert.ToString(reader["NOME"])
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
        [Route("api/Empresaapi/GetComboEmpresaAniel")]
        public async Task<ObjCombo<Int64>> GetComboEmpresaAniel(RequestObjCombo<Int64?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {

                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_EMPRESA A ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        //condicoes.Add($"A.STATUS = 1");
                    }


                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.CODEMPRESA = {request.id}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.RAZAO_SOCIAL) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 60)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes) } ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.CODEMPRESA) AS CO ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.CODEMPRESA, A.RAZAO_SOCIAL ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.RAZAO_SOCIAL)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt32(reader["CODEMPRESA"]),
                                    Text = Convert.ToString(reader["RAZAO_SOCIAL"])
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
        [Route("api/Empresaapi/GetComboCNPJEmpresa")]
        public async Task<ObjCombo<Int64>> GetComboCNPJEmpresa(RequestObjCombo<Int64?> request)
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_EMPRESA A ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        //condicoes.Add($"A.STATUS = 1");
                    }


                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.CODEMPRESA = {request.id}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.RAZAO_SOCIAL) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 60)}%') OR A.CNPJ LIKE '%{request.q}%'");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes)} ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.CODEMPRESA) AS CO ");
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
                        StringBuilder sql = new StringBuilder(" SELECT A.CODEMPRESA, A.RAZAO_SOCIAL, A.CNPJ ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.RAZAO_SOCIAL)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt32(reader["CODEMPRESA"]),
                                    Text = Convert.ToString(reader["CNPJ"]) + " - " + Convert.ToString(reader["RAZAO_SOCIAL"])
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
        [Route("api/Empresaapi/GetComboEmpresaConnect")]
        public async Task<ObjCombo<Int64>> GetComboEmpresaConnect(RequestObjCombo<Int64?> request)
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_EMPRESA A ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        //condicoes.Add($"A.STATUS = 1");
                    }


                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.CODEMPRESA = {request.id}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.RAZAO_SOCIAL) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 60)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes) } ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.CODEMPRESA) AS CO ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.CODEMPRESA, A.RAZAO_SOCIAL ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.RAZAO_SOCIAL)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt32(reader["CODEMPRESA"]),
                                    Text = Convert.ToString(reader["RAZAO_SOCIAL"])
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
        public async Task<GetApiRetorno<IEnumerable<EmpresaListaModel>>> GetList()
        {
            GetApiRetorno<IEnumerable<EmpresaListaModel>> retorno = new GetApiRetorno<IEnumerable<EmpresaListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoEmpresa = new Conexao<Empresa>(session))
            {
                try
                {
                    var lista = await repoEmpresa.queryover().ListAsync();

                    var listaFiltrada = lista.Where(a => !String.IsNullOrEmpty(a.Nome)).ToList();

                    retorno.Objeto = listaFiltrada.Select(p => new EmpresaListaModel
                    {
                        Id = p.Id,
                        Nome = p.Nome
                        //Descricao = p.Descricao

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

    }
}