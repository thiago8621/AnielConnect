using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.ModelLog.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using NHibernate.Criterion;
using Sinapse.DAO.repository;
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
    public class CidadeApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/Cidadeapi/GetAll")]
        public async Task<DataTableResponse<CidadeListaModel>> GetAll(Cidade_DataTableRequest request)
        {
            List<CidadeListaModel> retorno = new List<CidadeListaModel>();
            IOrderedEnumerable<CidadeListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_CIDADE");

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

                    sql.AppendLine(" SELECT A.ID, A.DESCRICAO, A.STATUS, A.ID_PAIS, A.CODIGO_IBGE,  C.NOME AS PAIS, B.SIGLA AS UF")
                    .AppendLine(" FROM TB_CIDADE A ")
                    .AppendLine(" Left JOIN TB_UF B ")
                    .AppendLine(" ON A.ID_UF = B.ID  ")
                    .AppendLine(" Left JOIN TB_PAIS C ")
                    .AppendLine(" ON A.ID_PAIS = C.ID ")
                    .AppendLine(" WHERE A.ID > 0");

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine(" AND ((UPPER(A.DESCRICAO) LIKE @SEARCH)) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                    }

                    if (!String.IsNullOrEmpty(request.UF))
                    {
                        sql.AppendLine("AND (B.SIGLA = @SIGLA)");
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
                            retorno.Add(new CidadeListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Codigo_IBGE = Convert.ToString(reader["CODIGO_IBGE"]),
                                UF = Convert.ToString(reader["UF"]),
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
                                listaOrdenada = retorno.OrderBy(p => p.UF);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.UF);
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
            return new DataTableResponse<CidadeListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        private CidadeCadastroModel GetViewObj(Cidade obj)
        {
            return new CidadeCadastroModel
            {
                Id = obj.Id,
                Codigo_IBGE = obj.Codigo_IBGE,
                Descricao = obj.Descricao,
                UF = obj.Id_UF?.Id,
                Pais = obj.Pais?.Id,
                Status = obj.Status == Status_Default.Ativo,
                CodigoAuxiliar = obj.CodigoAuxiliar
            };
        }

        private void BindViewToObj(ISession session, ref Cidade objBanco, CidadeCadastroModel view)
        {
            using (var repoUf = new Conexao<Uf>(session))
            using (var repoPais = new Conexao<Pais>(session))
            {
                Uf uf = repoUf.selectFirst(p => p.Id == view.UF).Result;
                
                objBanco.Codigo_IBGE = view.Codigo_IBGE;
                objBanco.Descricao = view.Descricao;
                objBanco.UF = uf?.Sigla;
                objBanco.Id_UF = uf;
                objBanco.Pais = repoPais.selectFirst(p => p.Id == view.Pais).Result;
                objBanco.CodigoAuxiliar = view.CodigoAuxiliar;

                if (objBanco.Id != 0)
                {
                    objBanco.Status = view.Status ? Status_Default.Ativo : Status_Default.Inativo;
                }
            }

        }

        [HttpPost()]
        [Route("api/Cidadeapi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_CIDADE SET STATUS = {value.status} WHERE ID IN {_ids}");

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
        [Route("api/Cidadeapi/excluirlista")]
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

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_CIDADE WHERE ID IN {_ids}");

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
        [Route("api/Cidadeapi/FindById")]
        public async Task<GetApiRetorno<CidadeCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<CidadeCadastroModel> retorno = new GetApiRetorno<CidadeCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Cidade>(session))
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
        [Route("api/Cidadeapi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<CidadeCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Cidade modelo = null, modelo_Anterior = null;

                using (ISession session = HibernateUtil.GetSession())
                using (var sessionLog = HibernateUtil.GetSessionLog())
                {
                    using (var repository = new Conexao<Cidade>(session))
                    {
                        if (value.Objeto.Id != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();
                            modelo_Anterior = (Cidade)modelo.Clone();
                        }
                        else
                        {
                            var cidadeByIBGE = (await repository.selectFirst(p => p.Codigo_IBGE == value.Objeto.Codigo_IBGE));
                            if (cidadeByIBGE != null)
                            {
                                if (value.Objeto.Codigo_IBGE != "0")
                                {
                                    throw new Exception("Código IBGE já foi criado");
                                }
                            }

                        }

                        if (modelo == null)
                        {
                            modelo = new Cidade
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
                            using (var repositoryLog = new Conexao<Log_Cidade>(sessionLog))
                            {
                                modelo.UsuarioAlteracao = value.Usuario;
                                var log = modelo.GeraLog(modelo_Anterior, value.Usuario, value.NomeUsuario, Sinapse.ModelLog.TipoOperLog.Alteracao);
                                await repositoryLog.Save(log);
                            }
                        }

                        else
                        {
                            using (var repositoryLog = new Conexao<Log_Cidade>(sessionLog))
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
        [Route("api/Cidadeapi/busca")]
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
                        sql.AppendLine("FROM TB_CIDADE A");
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

        [Route("api/Cidadeapi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody] ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Cidade>(session))
                    {

                        Cidade modelo = null;
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
        public async Task<GetApiRetorno<IEnumerable<CidadeListaModel>>> GetList()
        {
            GetApiRetorno<IEnumerable<CidadeListaModel>> retorno = new GetApiRetorno<IEnumerable<CidadeListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Cidade>(session))
            {
                try
                {
                    var lista = await repository.queryover().ListAsync();

                    retorno.Objeto = lista.Select(p => new CidadeListaModel
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
        [Route("api/Cidadeapi/GetCombo")]
        public async Task<ObjCombo<Int64>> GetCombo(RequestObjComboPai<Int64?, Int64?> request)
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_CIDADE A WHERE ");

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
                        condicoes.Add($"A.ID_UF = {request.idpai}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" ( {String.Join(" AND ", condicoes) } ) ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.DESCRICAO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["DESCRICAO"])
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
        public async Task<List<ExcelServiceLogItem>> ImportacaoModeloBase(ISession session, CidadeImportacao item, Cidade cidades)
        {
            List<ExcelServiceLogItem> retorno = new List<ExcelServiceLogItem>();
            using (Conexao<Cidade> Cidade = new Conexao<Cidade>(session))
            using (var repoUf = new Conexao<Uf>(session))
            using (var repoPais = new Conexao<Pais>(session))
            using (CidadeApiController Cidade_Api = new CidadeApiController())
            {

                if ((await Cidade.selectFirst(p => p.Codigo_IBGE == item.Codigo_Oficial_IBGE)) != null)
                {
                    if (item.Codigo_Oficial_IBGE != "0")
                    {
                        retorno.Add(new ExcelServiceLogItem
                        {
                            Chave = item.Codigo_Oficial_IBGE,
                            Erro = "O Codigo_IBGE de Cidade já está criado. Cidade não foi importada."
                        });
                    }
                }


                if (String.IsNullOrEmpty(item.Descricao_Cidade))
                    retorno.Add(new ExcelServiceLogItem
                    {
                        Chave = item.Descricao_Cidade,
                        Erro = "Descrição não foi importado."
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

                cidades.Codigo_IBGE = item.Codigo_Oficial_IBGE;
                cidades.Status = Status_Default.Ativo;
                cidades.Descricao = item.Descricao_Cidade.ToUpper();

                cidades.Pais = item.Pais.Length <= 2 ?
                    repoPais.selectFirst(p => p.Id == Convert.ToInt64(item.Pais)).Result :
                        cidades.Pais = repoPais.selectFirst(p => p.Nome == item.Pais).Result;

                cidades.Id_UF = repoUf.selectFirst(p => p.Sigla == item.Sigla_UF).Result;

            }
            return retorno;
        }

        public async Task<List<ExcelServiceLogItem>> ImportacaoModelo(CidadeImportacao item)
        {
            List<ExcelServiceLogItem> retorno = new List<ExcelServiceLogItem>();
            using (NHibernate.ISession session = HibernateUtil.GetSession())
            using (Conexao<Cidade> Cidade = new Conexao<Cidade>(session))
            using (var repoUf = new Conexao<Uf>(session))
            using (var repoPais = new Conexao<Pais>(session))
            {
                Cidade cidades = new Cidade();

                retorno = await ImportacaoModeloBase(session, item, cidades);
                cidades.Codigo_IBGE = item.Codigo_Oficial_IBGE;
                cidades.Status = Status_Default.Ativo;
                cidades.Descricao = item.Descricao_Cidade.ToUpper();

                cidades.Pais.Id = item.Pais.Length <= 2 ?
                    repoPais.selectFirst(p => p.Id == Convert.ToInt64(item.Pais)).Result.Id :
                        cidades.Pais.Id = repoPais.selectFirst(p => p.Nome == item.Pais).Result.Id;

                cidades.Id_UF.Id = repoUf.selectFirst(p => p.Sigla == item.Sigla_UF).Result.Id;
                //cidades.UF = repoUf.selectFirst(p => p.Sigla == item.Sigla_UF).Result.Sigla;

                if (retorno.Count() > 0)
                    return retorno;


                await Cidade.SaveOrUpdate(cidades);
                retorno.Add(new ExcelServiceLogItem
                {
                    Chave = item.Codigo_Oficial_IBGE,
                    Erro = "Inserido com sucesso"
                });
            }

            return retorno;
        }

        [HttpGet()]
        [Route("api/Cidadeapi/ConsultaCidadePorIBGE")]
        public async Task<CidadeListaModel> ConsultaCidadePorIBGE(String Codigo_IBGE)
        {
            var retorno = new CidadeListaModel();
            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        Dictionary<String, Object> parametros = new Dictionary<string, object>();
                        StringBuilder sql = new StringBuilder($@"
                        SELECT A.ID, A.DESCRICAO, A.UF, A.CODIGO_IBGE, A.STATUS, A.ID_UF, A.ID_PAIS
                        FROM TB_CIDADE A
                        WHERE CODIGO_IBGE = {Codigo_IBGE}");
                        
                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, 1))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno = new CidadeListaModel
                                {
                                    Id = Convert.IsDBNull(reader["ID"]) ? 0 : Convert.ToInt64(reader["ID"]),
                                    Descricao = Convert.IsDBNull(reader["DESCRICAO"]) ? String.Empty : Convert.ToString(reader["DESCRICAO"]),
                                    UF = Convert.IsDBNull(reader["UF"]) ? String.Empty : Convert.ToString(reader["UF"]),
                                    Codigo_IBGE = Convert.IsDBNull(reader["CODIGO_IBGE"]) ? String.Empty : Convert.ToString(reader["CODIGO_IBGE"]),
                                    Status = Convert.IsDBNull(reader["STATUS"]) ? String.Empty : HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"])),
                                    Id_UF = Convert.IsDBNull(reader["ID_UF"]) ? (Int64?)null : Convert.ToInt64(reader["ID_UF"]),
                                    Id_Pais = Convert.IsDBNull(reader["ID_PAIS"]) ? (Int64?)null : Convert.ToInt64(reader["ID_PAIS"])
                                };
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
