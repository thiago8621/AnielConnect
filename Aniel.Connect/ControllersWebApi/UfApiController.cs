using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.ModelLog.nhibernate;
using Aniel.Connect.Models;
using Aniel.T5.Model.mongo;
using NHibernate;
using NHibernate.Criterion;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.repository;
using Sinapse.DAO.Repository;
using Sinapse.Model.enumerados;
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
    public class UfApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/Ufapi/GetAll")]
        public async Task<DataTableResponse<UfListaModel>> GetAll(Uf_DataTableRequest request)
        {
            List<UfListaModel> retorno = new List<UfListaModel>();
            IOrderedEnumerable<UfListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_UF");

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

                    sql.AppendLine(" SELECT A.ID, A.DESCRICAO, A.STATUS, A.ID_PAIS, A.CODIGO_IBGE, A.SIGLA, B.NOME AS PAIS");
                    sql.AppendLine(" FROM TB_UF A ");
                    sql.AppendLine(" LEFT JOIN TB_PAIS B");
                    sql.AppendLine(" ON A.ID_PAIS = B.ID");
                    sql.AppendLine(" WHERE A.ID > 0");


                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine(" AND ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }

                    if (!String.IsNullOrEmpty(request.UF))
                    {
                        sql.AppendLine("AND (A.SIGLA = @SIGLA)");
                        parametros.Add("@SIGLA", Convert.ToString(request.UF));
                    }

                    if (!String.IsNullOrEmpty(request.Pais))
                    {
                        sql.AppendLine("AND (A.ID_PAIS = @ID_PAIS)");
                        parametros.Add("@ID_PAIS", Convert.ToInt32(request.Pais));
                    }

                    if (!String.IsNullOrEmpty(request.Status_Filtro))
                    {
                        sql.AppendLine("AND (A.STATUS = @STATUS)");
                        parametros.Add("@STATUS", Convert.ToInt32(request.Status_Filtro));
                    }

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new UfListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Codigo_IBGE = Convert.ToString(reader["CODIGO_IBGE"]),
                                Sigla = Convert.ToString(reader["SIGLA"]),
                                Pais = Convert.ToString(reader["PAIS"]),
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"]))
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
                    case 1:     // Id
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Id);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Id);
                            break;
                        }

                    case 2:     // Codigo_IBGE
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Codigo_IBGE);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Codigo_IBGE);
                            break;
                        }

                    case 3:     // Descricao
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Descricao);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Descricao);
                            break;
                        }

                    case 4:     // UF
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Sigla);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Sigla);
                            break;
                        }

                    case 5:     // Pais
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Pais);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Pais);
                            break;
                        }

                    case 6:     // Status
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Status);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Status);
                            break;
                        }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<UfListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        private UfCadastroModel GetViewObj(Uf obj)
        {
            return new UfCadastroModel
            {
                Id = obj.Id,
                Codigo_IBGE = obj.Codigo_IBGE,
                Descricao = obj.Descricao,
                Sigla = obj.Sigla,
                Pais = obj.Pais.Id,
                Status = obj.Status == Status_Default.Ativo,
                CodigoAuxiliar = obj.CodAuxiliar
            };
        }

        private void BindViewToObj(ISession session, ref Uf objBanco, UfCadastroModel view)
        {
            using (var repoPais = new Conexao<Pais>(session))
            {
                objBanco.Pais = repoPais.selectFirst(p => p.Id == view.Pais).Result;
            }
            objBanco.Codigo_IBGE = view.Codigo_IBGE;
            objBanco.Descricao = view.Descricao;
            objBanco.Sigla = view.Sigla;
            objBanco.CodAuxiliar = view.CodigoAuxiliar;

            if (objBanco.Id != 0)
            {
                objBanco.Status = view.Status ? Status_Default.Ativo : Status_Default.Inativo;
            }


        }

        [HttpPost()]
        [Route("api/Ufapi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_UF SET STATUS = {value.status} WHERE ID IN {_ids}");

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
        [Route("api/Ufapi/excluirlista")]
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

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_UF WHERE ID IN {_ids}");

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
        [Route("api/Ufapi/FindById")]
        public async Task<GetApiRetorno<UfCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<UfCadastroModel> retorno = new GetApiRetorno<UfCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Uf>(session))
            {
                try
                {
                    retorno.Objeto = GetViewObj(await repository.selectFirst(p => p.Id == Id));
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
        [Route("api/Ufapi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<UfCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Uf modelo = null, modelo_Anterior = null;
                using (var sessionLog = HibernateUtil.GetSessionLog())
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Uf>(session))

                    {
                        if (value.Objeto.Id != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();
                            modelo_Anterior = (Uf)modelo.Clone();
                        }
                        else
                        {
                            var UF_Existente = repository.selectFirst(p => p.Codigo_IBGE == value.Objeto.Codigo_IBGE).Result;
                            if (UF_Existente != null)
                            {
                                throw new Exception("IBGE Já cadastrada !");
                            }
                            modelo = new Uf();
                        }

                        if (modelo == null)
                        {

                            modelo = new Uf
                            {
                                Status = Status_Default.Ativo,
                                UsuarioCriacao = value.Usuario
                            };

                        }
                        else
                        {

                            modelo.UsuarioAlteracao = value.Usuario;
                        }

                        BindViewToObj(session, ref modelo, value.Objeto);

                        await repository.SaveOrUpdate(modelo);

                        if (modelo_Anterior != null)
                        {
                            using (var repositoryLog = new Conexao<Log_Uf>(sessionLog))
                            {
                                modelo.UsuarioAlteracao = value.Usuario;
                                var log = modelo.GeraLog(modelo_Anterior, value.Usuario, value.NomeUsuario, Sinapse.ModelLog.TipoOperLog.Alteracao);
                                await repositoryLog.Save(log);
                            }
                        }
                        else
                        {
                            using (var repositoryLog = new Conexao<Log_Uf>(sessionLog))
                            {
                                var log = modelo.GeraLog(modelo_Anterior, value.Usuario, value.NomeUsuario, Sinapse.ModelLog.TipoOperLog.Insercao);
                                await repositoryLog.Save(log);
                            }

                        }
                    }
                    retorno.IdObjeto = modelo.Id;
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

        [HttpGet()]
        [Route("api/Ufapi/busca")]
        public async Task<List<ObjBusca<Int64>>> busca(String term)
        {
            List<ObjBusca<Int64>> retorno = new List<ObjBusca<Int64>>();

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

                        sql.AppendLine("SELECT A.ID, A.DESCRICAO");
                        sql.AppendLine("FROM TB_UF A");
                        sql.AppendLine("WHERE (UPPER(A.DESCRICAO) LIKE @VALOR) ");
                        parametros.Add("@VALOR", $"%{term.ToUpperInvariant()}%");

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new ObjBusca<Int64>
                                {
                                    Value = Convert.ToInt64(reader["ID"]),
                                    Label = Convert.ToString(reader["DESCRICAO"])
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

        [Route("api/Ufapi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody] ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Uf>(session))
                    {

                        Uf modelo = null;
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
        public async Task<GetApiRetorno<IEnumerable<UfListaModel>>> GetList()
        {
            GetApiRetorno<IEnumerable<UfListaModel>> retorno = new GetApiRetorno<IEnumerable<UfListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Uf>(session))
            {
                try
                {
                    var lista = await repository.queryover().ListAsync();

                    retorno.Objeto = lista.Select(p => new UfListaModel
                    {
                        Id = p.Id,
                        Descricao = p.Descricao
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
        [Route("api/Ufapi/GetCombo")]
        public async Task<ObjCombo<Int64>> GetCombo(RequestObjComboPai<Object, Int64?> request)
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_UF A WHERE ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS = 1");
                    }

                    if (int.TryParse((string)request.id, out int value)) {
                        condicoes.Add($" A.ID = {request.id} ");
                    } else {
                        condicoes.Add($" A.SIGLA LIKE '%{request.id}%' ");
                    }


                    if (request.idpai.HasValue)
                    {
                        condicoes.Add($"A.ID_PAIS = {request.idpai}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.SIGLA) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.SIGLA ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.SIGLA)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["SIGLA"])
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

        public async Task<List<ExcelServiceLogItem>> ImportacaoModeloBase(ISession session, UfImportacao item, Uf ufs)
        {
            List<ExcelServiceLogItem> retorno = new List<ExcelServiceLogItem>();
            using (Conexao<Uf> Uf = new Conexao<Uf>(session))
            using (var repoPais = new Conexao<Pais>(session))
            using (UfApiController Uf_Api = new UfApiController())
            {

                if ((await Uf.selectFirst(p => p.Descricao == item.Descricao_UF)) != null)
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Codigo_Oficial_IBGE,
                        Erro = "UF já Existente. UF não foi importado."
                    });

                if (String.IsNullOrEmpty(item.Sigla_UF))
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Sigla_UF,
                        Erro = "Sigla_UF não foi importado."
                    });
                if (String.IsNullOrEmpty(item.Pais))
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Pais,
                        Erro = "País não foi importado."
                    });


                if (retorno.Count() > 0)
                    return retorno;

                ufs.Codigo_IBGE = item.Codigo_Oficial_IBGE;
                ufs.Status = Status_Default.Ativo;
                ufs.Descricao = item.Descricao_UF;

                ufs.Pais = item.Pais.Length <= 2 ?
                    repoPais.selectFirst(p => p.Id == Convert.ToInt64(item.Pais)).Result :
                        ufs.Pais = repoPais.selectFirst(p => p.Nome == item.Pais).Result;


                ufs.Sigla = item.Sigla_UF;

            }
            return retorno;
        }

        public async Task<List<ExcelServiceLogItem>> ImportacaoModelo(UfImportacao item)
        {
            List<ExcelServiceLogItem> retorno = new List<ExcelServiceLogItem>();
            using (NHibernate.ISession session = HibernateUtil.GetSession())
            using (Conexao<Uf> Uf = new Conexao<Uf>(session))
            using (var repoPais = new Conexao<Pais>(session))
            {
                Uf ufs = new Uf();

                retorno = await ImportacaoModeloBase(session, item, ufs);
                ufs.Codigo_IBGE = item.Codigo_Oficial_IBGE;
                ufs.Status = Status_Default.Ativo;
                ufs.Descricao = item.Descricao_UF;

                ufs.Pais.Id = item.Pais.Length <= 2 ?
                    repoPais.selectFirst(p => p.Id == Convert.ToInt64(item.Pais)).Result.Id :
                        ufs.Pais.Id = repoPais.selectFirst(p => p.Nome == item.Pais).Result.Id;

                ufs.Sigla = item.Sigla_UF;

                if (retorno.Count() > 0)
                    return retorno;


                await Uf.SaveOrUpdate(ufs);
                retorno.Add(new ExcelServiceLogItem
                {
                    Chave = item.Codigo_Oficial_IBGE,
                    Erro = "Inserido com sucesso"
                });
            }

            return retorno;
        }

        [HttpPost]
        [Route("api/Ufapi/GetListMongo")]
        public async Task<GetApiRetorno<List<UfCadastroModel>>> GetListMongo()
        {
            var retorno = new GetApiRetorno<List<UfCadastroModel>>();
            try
            {
                using (var connection = new ConexaoMongo())
                using (var repoGeojson = new RepositoryMongo<UfMongo>(connection))
                {
                    List<UfMongo> states = await repoGeojson.FindAll();

                    retorno.Objeto = states.Select(mongoObject =>

                        new UfCadastroModel
                        {
                            Codigo_IBGE = mongoObject.Codigo_IBGE,
                            Descricao = mongoObject.Descricao,
                            Id = mongoObject.Id,
                            Pais = mongoObject.Id_Pais,
                            Sigla = mongoObject.Sigla

                        }).ToList();

                    var statesCount = states.Count;
                    retorno.Ok = true;

                }
            }
            catch (Exception)
            {
                retorno.Ok = false;
            }

            return retorno;
        }
    }
}
