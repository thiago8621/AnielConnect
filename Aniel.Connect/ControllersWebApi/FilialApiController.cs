using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
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
    public class FilialApiController : BaseApiController
    {
        [HttpGet()]
        [Route("api/Filialapi/busca")]
        //public async Task<List<ObjBusca<Int64>>> busca(String term, String id_empresa)
        public async Task<List<ObjBusca<Int64>>> busca(String term)
        {
            List<ObjBusca<Int64>> retorno = new List<ObjBusca<Int64>>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>()
                    {
                        //{ "@ID_EMPRESA" , Convert.ToInt64(id_empresa) }
                    };

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID, A.NOME");
                        sql.AppendLine("FROM TB_FILIAL A");
                        sql.AppendLine("WHERE (UPPER(A.NOME) LIKE @VALOR)");
                        parametros.Add("@VALOR", $"%{term.ToUpperInvariant()}%");

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new ObjBusca<Int64>
                                {
                                    Value = Convert.ToInt64(reader["ID"]),
                                    Label = Convert.ToString(reader["NOME"])
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
        [Route("api/FilialApi/GetTreeView")]
        public async Task<GetApiRetorno<List<FilialTreeViewModel>>> GetTreeView(Int64 IdEmpresa,Int64? IdFilial)
        {
            GetApiRetorno<List<FilialTreeViewModel>> retorno = new GetApiRetorno<List<FilialTreeViewModel>>
            {
                Objeto = new List<FilialTreeViewModel>()
            };
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        Dictionary<String, Object> parametros = new Dictionary<String, Object>();
                        StringBuilder sql = new StringBuilder($"SELECT ID, NOME FROM TB_FILIAL WHERE ID_EMPRESA = @ID_EMPRESA");

                        if (IdFilial.HasValue)
                        {
                            sql.AppendLine($" AND ID = @ID_FILIAL ");
                            parametros.Add("@ID_FILIAL", IdFilial.Value);
                        }

                        
                        parametros.Add("@ID_EMPRESA", IdEmpresa);
                        var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null);

                        while (await reader.ReadAsync())
                        {
                            retorno.Objeto.Add(new FilialTreeViewModel
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
        [Route("api/Filialapi/GetAll")]
        public async Task<DataTableResponse<FilialListaModel>> GetAll(Filial_DataTableRequest request)
        {
            List<FilialListaModel> retorno = new List<FilialListaModel>();
            IOrderedEnumerable<FilialListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_FILIAL ");

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

                    sql.AppendLine("SELECT A.ID, A.CODFILIAL, A.NOME, A.ID_EMPRESA, A.ID_FAIXA_COMISSIONAMENTO ");
                    sql.AppendLine("  FROM TB_FILIAL A  ");

                    //sql.AppendLine("LEFT JOIN TB_FUNCIONARIO B ");
                    //sql.AppendLine("ON (A.ID_VENDEDOR = B.ID) ");


                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine(" where (UPPER(A.NOME) LIKE UPPER(@SEARCH) ) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new FilialListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                CodFilial = Convert.ToString(reader["CODFILIAL"]),
                                Nome = Convert.ToString(reader["NOME"]),
                                //Empresa = Convert.ToString(reader["NOME"]),
                                //Faixa_Comissionamento = Convert.ToString(reader["NOME"]),

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
            return new DataTableResponse<FilialListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        private FilialCadastroModel GetViewObj(Filial obj)
        {
            var retorno = new FilialCadastroModel
            {
                Id = obj.Id,
                CodFilial = Convert.ToString(obj.CodFilial),
                Nome = obj.Nome,
                Empresa = obj.Empresa?.Nome,
            };

            return retorno;
        }

        //private void BindViewToObj(ISession session, ref Filial objBanco, FilialCadastroModel view)
        //{

        //}


        [HttpPost()]
        [Route("api/filialapi/AlterarStatus")]
        public async Task<PostApiRetorno<Int64>> AlterarStatus(AlterarStatusViewModel value)
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_FILIAL SET STATUS = {value.status} WHERE ID IN {_ids}");

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

        [HttpPost()]
        [Route("api/filialapi/excluirlista")]
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

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_FILIAL WHERE ID IN {_ids}");

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
        [Route("api/filialApi/FindById")]
        public async Task<GetApiRetorno<FilialCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<FilialCadastroModel> retorno = new GetApiRetorno<FilialCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoFilial = new Conexao<Filial>(session))
            {
                try
                {
                    retorno.Objeto = GetViewObj(await repoFilial.selectFirst(p => p.Id == Id));
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
        [Route("api/filialApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody]ParamWebApi<FilialCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Filial modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Filial>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new Filial
                            {
                                //Status = Status_Atendimento.A_Iniciar
                                UsuarioCriacao = value.Usuario
                            };

                        }
                        else
                        {
                            modelo.UsuarioAlteracao = value.Usuario;
                        }

                        //BindViewToObj(session, ref modelo, value.Objeto);


                        await repository.SaveOrUpdate(modelo);
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

        [Route("api/filialApi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody]ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Filial>(session))
                    {

                        Filial modelo = null;
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
        public async Task<GetApiRetorno<IEnumerable<FilialListaModel>>> GetList()
        {
            GetApiRetorno<IEnumerable<FilialListaModel>> retorno = new GetApiRetorno<IEnumerable<FilialListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoFilial = new Conexao<Filial>(session))
            {
                try
                {
                    var lista = await repoFilial.queryover().ListAsync();

                    retorno.Objeto = lista.Select(p => new FilialListaModel
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

        [HttpPost()]
        public async Task<GetApiRetorno<IEnumerable<FilialListaModel>>> GetListVerificacaoVazio()
        {
            GetApiRetorno<IEnumerable<FilialListaModel>> retorno = new GetApiRetorno<IEnumerable<FilialListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoFilial = new Conexao<Filial>(session))
            {
                try
                {
                    var lista = await repoFilial.queryover().ListAsync();
                    
                    var listaFiltrada =  lista.Where(a => !String.IsNullOrEmpty(a.Nome)).ToList();

                    retorno.Objeto = listaFiltrada.Select(p => new FilialListaModel
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

        [HttpPost()]
        [Route("api/FilialApi/GetComboDescricao")]
        public async Task<ObjCombo<String>> GetComboDescricao(RequestObjComboPai<Int64?, Int64?> request)
        {
            ObjCombo<String> retorno = new ObjCombo<string>
            {
                results = new List<IObjComboResult<string>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_FILIAL A ");

                    List<String> condicoes = new List<String>();

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request.idpai.HasValue)
                    {
                        condicoes.Add($"A.ID_EMPRESA = {request.idpai}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.NOME) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 40)}%')");
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
                        StringBuilder sql = new StringBuilder("SELECT DISTINCT A.ID, A.NOME ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.NOME)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["ID"]),
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
        [Route("api/FilialApi/GetCombo")]
        public async Task<ObjCombo<Int64>> GetCombo(RequestObjComboPai<String, String> request)
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_FILIAL A ");

                    List<String> condicoes = new List<String>();                  

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"ID = {request.id}");
                    }

                    if (!String.IsNullOrEmpty(request.idpai))
                    {
                        condicoes.Add($"ID_EMPRESA = {request.idpai}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.NOME) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE ( {String.Join(" AND ", condicoes) } ) ");
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
                                    Id = Convert.ToInt64(reader["ID"]),
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
        [Route("api/FilialApi/GetComboFilialAniel")]
        public async Task<ObjCombo<Int64>> GetComboFilialAniel(RequestObjComboPai<String,String> request)
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_FILIAL A ");

                    List<String> condicoes = new List<String>();

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"CODFILIAL = {request.id}");
                    }
                    if (!String.IsNullOrEmpty(request.idpai))
                    {
                        condicoes.Add($"CODEMPRESA = {request.idpai}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.NOME) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE ( {String.Join(" AND ", condicoes) } ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.CODFILIAL) AS CO ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.CODFILIAL, A.NOME ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.NOME)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["CODFILIAL"]),
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
        [Route("api/FilialApi/GetComboFilialConnect")]
        public async Task<ObjCombo<Int64>> GetComboFilialConnect(RequestObjComboPai<String, String> request)
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_FILIAL A ");

                    List<String> condicoes = new List<String>();

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"CODFILIAL = {request.id}");
                    }
                    if (!String.IsNullOrEmpty(request.idpai))
                    {
                        condicoes.Add($"ID_EMPRESA = {request.idpai}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.NOME) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE ( {String.Join(" AND ", condicoes) } ) ");
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
                                    Id = Convert.ToInt64(reader["ID"]),
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
        [Route("api/FilialApiCNPJ/GetComboFilialAniel")]
        public async Task<ObjCombo<Int64>> GetComboFilialAnielCNPJ(RequestObjComboPai<String, String> request)
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_FILIAL A ");

                    List<String> condicoes = new List<String>();

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"CODFILIAL = {request.id}");
                    }
                    if (!String.IsNullOrEmpty(request.idpai))
                    {
                        condicoes.Add($"CODEMPRESA = {request.idpai}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.CNPJ) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE ( {String.Join(" AND ", condicoes) } ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.CODFILIAL) AS CO ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.CODFILIAL, A.CNPJ ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.CNPJ)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["CODFILIAL"]),
                                    Text = Convert.ToString(reader["CNPJ"])
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
        [Route("api/FilialApiNumeroFistel/GetComboFilialAniel")]
        public async Task<ObjCombo<Int64>> GetComboFilialAnielFistel(RequestObjComboPai<String, String> request)
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_FILIAL A ");

                    List<String> condicoes = new List<String>();

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"CODFILIAL = {request.id}");
                    }
                    if (!String.IsNullOrEmpty(request.idpai))
                    {
                        condicoes.Add($"CODEMPRESA = {request.idpai}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.CNPJ) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE ( {String.Join(" AND ", condicoes) } ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.CODFILIAL) AS CO ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.CODFILIAL, A.NUMERO_FISTEL ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.CNPJ)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["CODFILIAL"]),
                                    Text = Convert.ToString(reader["NUMERO_FISTEL"])
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


    }
}
