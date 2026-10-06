using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.T5.Model.enumerado;
using Microsoft.AspNet.Identity;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Padrao = Sinapse.Model.enumerados.Padrao;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class Modelo_PainelVendaApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/Modelo_PainelVendaApi/GetAll")]
        public async Task<DataTableResponse<Modelo_PainelVendaListaModel>> GetAll(Modelo_PainelVenda_DataTableRequest request)
        {
            List<Modelo_PainelVendaListaModel> retorno = new List<Modelo_PainelVendaListaModel>();
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                StringBuilder sqlFrom = new StringBuilder();
                sqlFrom.AppendLine("FROM TB_MODELO_PAINEL_VENDA A");

                if (!request.AdministradorModelos)
                {
                    sqlFrom.AppendLine("WHERE ((A.VISIBILIDADE = 0 AND A.USUARIOCRIACAO = @USER))");
                }

                if (!String.IsNullOrEmpty(request.search.value))
                {
                    if (sqlFrom.ToString().Contains("WHERE"))
                        sqlFrom.AppendLine(" AND (UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH)) ");
                    else
                        sqlFrom.AppendLine(" WHERE (UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH)) ");

                    parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                }

                parametros.Add("@USER", User.Identity.GetUserId());

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

                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT A.ID, A.DESCRICAO,A.STATUS,A.VISIBILIDADE,A.COLUNA_ORDENAR, A.PADRAO, ");
                    sql.AppendLine(HtmlEnumExtensions.getEnumDescriptionInSql<Status_Default>("A.STATUS", "STATUS_ORDENAR"));
                    sql.AppendLine("," + HtmlEnumExtensions.getEnumDescriptionInSql<Visibilidade>("A.VISIBILIDADE", "VISIBILIDADE_ORDENAR"));
                    sql.AppendLine("," + HtmlEnumExtensions.getEnumDescriptionInSql<Coluna_PainelVendaEnum>("A.COLUNA_ORDENAR", "ORDENAR_POR"));
                    sql.AppendLine("," + HtmlEnumExtensions.getEnumDescriptionInSql<Aniel.T5.Model.enumerado.Padrao>("A.PADRAO", "PADRAO"));
                    sql.AppendLine(sqlFrom.ToString());

                    if (request.order != null && request.order.Any())
                    {
                        int sortColumnIndex = request.order[0].column;
                        var coluna = request.columns[sortColumnIndex].data;

                        string sortDirection = request.order[0].dir;
                        switch (coluna)
                        {
                            case "descricao":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine("ORDER BY STATUS_ORDENAR");
                                    else
                                        sql.AppendLine("ORDER BY STATUS_ORDENAR DESC");
                                    break;
                                }
                            case "status":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine("ORDER BY COLUNA_ORDENAR");
                                    else
                                        sql.AppendLine("ORDER BY COLUNA_ORDENAR DESC");
                                    break;
                                }
                            case "ordenar_Por":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine("ORDER BY ORDENAR_POR");
                                    else
                                        sql.AppendLine("ORDER BY ORDENAR_POR DESC");
                                    break;
                                }
                            case "visibilidade":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine("ORDER BY VISIBILIDADE_ORDENAR");
                                    else
                                        sql.AppendLine("ORDER BY VISIBILIDADE_ORDENAR DESC");
                                    break;
                                }
                            case "padrao":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine("ORDER BY PADRAO");
                                    else
                                        sql.AppendLine("ORDER BY PADRAO DESC");
                                    break;
                                }
                            case "id":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine("ORDER BY A.ID");
                                    else
                                        sql.AppendLine("ORDER BY A.ID DESC");
                                    break;
                                }
                        }
                    }

                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Modelo_PainelVendaListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Visibilidade = HtmlEnumExtensions.GetEnumDescription<Visibilidade>(Convert.ToString(reader["VISIBILIDADE"])),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Ordenar_Por = HtmlEnumExtensions.GetEnumDescription<Coluna_PainelVendaEnum>(Convert.ToString(reader["COLUNA_ORDENAR"])),
                                Padrao = HtmlEnumExtensions.GetEnumDescription<Padrao>(Convert.ToString(reader["PADRAO"])),
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"]))
                            });
                        }
                    }
                }
            }

            return new DataTableResponse<Modelo_PainelVendaListaModel>
            {
                draw = request.draw,
                recordsFiltered = recordsTotal,
                data = retorno.ToList(),
                error = ""
            };
        }

        [HttpPost()]
        [Route("api/Modelo_PainelVendaApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<Modelo_PainelVendaCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            Modelo_PainelVenda modelo = null;

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Modelo_PainelVenda>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new Modelo_PainelVenda
                            {
                                Status = Status_Default.Ativo,
                                UsuarioCriacao = value.Usuario
                            };
                        }
                        else
                        {
                            modelo.Status = value.Objeto.Status ? Status_Default.Ativo : Status_Default.Inativo;
                            modelo.UsuarioAlteracao = value.Usuario;
                        }

                        BindViewToObj(session, modelo, value.Objeto);
                        await repository.SaveOrUpdate(modelo);
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
        [Route("api/Modelo_PainelVendaApi/FindById")]
        public async Task<GetApiRetorno<Modelo_PainelVendaCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<Modelo_PainelVendaCadastroModel> retorno = new GetApiRetorno<Modelo_PainelVendaCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Modelo_PainelVenda>(session))

            {
                try
                {
                    retorno.Objeto = GetViewObj(await repository.selectFirst(p => p.Id == Id), session);
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

        [Route("api/Modelo_PainelVendaApi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody] ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Modelo_PainelVenda>(session))
                    {
                        Modelo_PainelVenda modelo = null;
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

        [HttpGet()]
        [Route("api/Modelo_PainelVendaApi/ExcluirGridFiltro")]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGridFiltro(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Filtro_PainelVenda>(session))
                    {

                        Filtro_PainelVenda modelo = null;
                        if (value != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value)).FirstOrDefault();

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

        [Route("api/Modelo_PainelVendaApi/ExcluirGridColuna")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGridColuna(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Coluna_PainelVenda>(session))
                    {

                        Coluna_PainelVenda modelo = null;
                        if (value != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value)).FirstOrDefault();

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
        [Route("api/Modelo_PainelVendaApi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_MODELO_PAINEL_VENDA SET STATUS = {value.status} WHERE ID IN {_ids}");

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
        [Route("api/Modelo_PainelVendaApi/tornarpublico")]
        public async Task<PostApiRetorno<Int64>> TornarPublico(AlterarStatusViewModel value)
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_MODELO_PAINEL_VENDA SET VISIBILIDADE = 1 WHERE ID IN {_ids}");

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
        [Route("api/Modelo_PainelVendaApi/GetCombo")]
        public async Task<ObjComboOptGroup<Int64, String>> GetCombo(RequestObjCombo<Int64?> request)
        {
            ObjComboOptGroup<Int64, String> retorno = new ObjComboOptGroup<Int64, String>
            {
                results = new List<IObjComboOptGroupResult<Int64, String>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder(@" FROM TB_MODELO_PAINEL_VENDA A ");

                    List<String> condicoes = new List<String>();
                    condicoes.Add("((A.VISIBILIDADE = 0 AND A.USUARIOCRIACAO = @USER) OR (A.VISIBILIDADE = 1) )");
                    parametros.Add("@USER", request.id_usuario);

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }
                    else
                    {
                        if (request.soativos)
                        {
                            condicoes.Add("A.STATUS = 1");
                        }

                        if (request != null && !String.IsNullOrEmpty(request.q))
                        {
                            condicoes.Add($"(UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 255)}%')");
                        }
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

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine(" SELECT A.ID, A.DESCRICAO, A.VISIBILIDADE ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" ORDER BY A.VISIBILIDADE, A.DESCRICAO ");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                String cod_grupo = HtmlEnumExtensions.GetEnumDescription<Visibilidade>(Convert.ToString(reader["VISIBILIDADE"]));
                                var grupo_result = retorno.results.Find(res => res.Id_Grupo == cod_grupo);

                                var option = new ObjComboResult<Int64>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["DESCRICAO"]),
                                };

                                if (grupo_result == null)
                                {
                                    retorno.results.Add(new ObjComboOptGroupResult<Int64, String>
                                    {
                                        Id_Grupo = cod_grupo,
                                        Text = cod_grupo,
                                        Children = new List<IObjComboResult<long>> { option }
                                    });
                                }
                                else
                                {
                                    grupo_result.Children.Add(option);
                                }
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
        [Route("api/Modelo_PainelVendaApi/GetColunas")]
        public async Task<GetApiRetorno<GetColunas_Model>> GetColunas(Int64 id_modelo)
        {
            GetApiRetorno<GetColunas_Model> retorno = new GetApiRetorno<GetColunas_Model>();
            retorno.Objeto = new GetColunas_Model();
            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Modelo_PainelVenda>(session))
            {
                try
                {
                    var modelo = await repository.selectFirst(p => p.Id == id_modelo);
                    retorno.Objeto.colunas = modelo.Colunas.Select(p => Convert.ToInt32(p.Coluna)).ToList();
                    retorno.Objeto.ordenar_por = Convert.ToInt32(modelo.Coluna_Ordenar);
                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;
        }

        [HttpPost()]
        [Route("api/Modelo_PainelVendaApi/GetComboTipoCadastro")]
        public ObjCombo<Int64> GetComboTipoCadastro(RequestObjCombo<Int64?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            foreach (int item in Enum.GetValues(typeof(Tipo_Cliente)))
            {
                retorno.results.Add(new ObjComboResult<Int64>() { Id = item, Text = Enum.GetValues(typeof(Tipo_Cliente)).GetValue(item).ToString() });
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Modelo_PainelVendaApi/GetComboTipoPessoa")]
        public ObjCombo<Int64> GetComboTipoPessoa(RequestObjCombo<Int64?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            foreach (int item in Enum.GetValues(typeof(Tipo_Pessoa)))
            {
                retorno.results.Add(new ObjComboResult<Int64>() { Id = item, Text = Enum.GetValues(typeof(Tipo_Pessoa)).GetValue(item).ToString() });
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Modelo_PainelVendaApi/GetComboUF")]
        public ObjCombo<Int64> GetComboUF(RequestObjCombo<Int64?> request)
        {
            var value = request.id;

            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            if (value == null)
            {
                foreach (int item in Enum.GetValues(typeof(UF)))
                {
                    retorno.results.Add(new ObjComboResult<Int64>() { Id = item, Text = Enum.GetValues(typeof(UF)).GetValue(item).ToString() });
                }
            }
            else
            {
                retorno.results.Add(new ObjComboResult<Int64>() { Id = (long)request.id, Text = Enum.GetValues(typeof(UF)).GetValue((long)request.id).ToString() });
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Modelo_PainelVendaApi/GetComboStatusVenda")]
        public ObjCombo<Int64> GetComboStatusVenda(RequestObjCombo<Int64?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            foreach (int item in Enum.GetValues(typeof(Status_Venda)))
            {
                switch (item)
                {
                    case 0:
                        retorno.results.Add(new ObjComboResult<Int64>() { Id = item, Text = Enum.GetValues(typeof(Status_Venda)).GetValue(0).ToString() });
                        break;
                    case 2:
                        retorno.results.Add(new ObjComboResult<Int64>() { Id = item, Text = Enum.GetValues(typeof(Status_Venda)).GetValue(1).ToString() });
                        break;
                    case 3:
                        retorno.results.Add(new ObjComboResult<Int64>() { Id = item, Text = Enum.GetValues(typeof(Status_Venda)).GetValue(2).ToString() });
                        break;
                    case 4:
                        retorno.results.Add(new ObjComboResult<Int64>() { Id = item, Text = Enum.GetValues(typeof(Status_Venda)).GetValue(3).ToString() });
                        break;
                    case 5:
                        retorno.results.Add(new ObjComboResult<Int64>() { Id = item, Text = Enum.GetValues(typeof(Status_Venda)).GetValue(4).ToString() });
                        break;
                    case 6:
                        retorno.results.Add(new ObjComboResult<Int64>() { Id = item, Text = Enum.GetValues(typeof(Status_Venda)).GetValue(5).ToString() });
                        break;
                }
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Modelo_PainelVendaApi/GetComboMeioCaptacao")]
        public async Task<ObjCombo<Int64>> GetComboMeioCaptacao(RequestObjCombo<Int64?> request)
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_MEIO_CAPTACAO A  ");

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

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<long>
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

        [HttpPost()]
        [Route("api/Modelo_PainelVendaApi/GetComboPlanos")]
        public async Task<ObjCombo<Int64>> GetComboPlanos(RequestObjCombo<Int64?> request)
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_PLANO A ");

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

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<long>
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

        [HttpPost()]
        [Route("api/Modelo_PainelVendaApi/GetComboTipoImovel")]
        public async Task<ObjCombo<Int64>> GetComboTipoImovel(RequestObjCombo<Int64?> request)
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_TIPO_IMOVEL A ");

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
                        StringBuilder sql = new StringBuilder("SELECT DISTINCT A.ID, A.DESCRICAO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<long>
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

        [HttpPost()]
        [Route("api/Modelo_PainelVendaApi/GetComboProdutos")]
        public async Task<ObjCombo<Int64>> GetComboProdutos(RequestObjCombo<Int64?> request)
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_PRODUTO A ");

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

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<long>
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

        Modelo_PainelVendaCadastroModel GetViewObj(Modelo_PainelVenda obj, ISession session)
        {
            using (var repository = new Conexao<Modelo_PainelVenda>(session))
            {
                return new Modelo_PainelVendaCadastroModel
                {
                    Id = obj.Id,
                    Descricao = obj.Descricao,
                    Status = obj.Status == Status_Default.Ativo,
                    Visibilidade = obj.Visibilidade,
                    Coluna_Ordenar = obj.Coluna_Ordenar,
                    Padrao = obj.Padrao,
                    Filtros = obj.Filtros.Select(p => getFiltroViewObj(p, session)).ToList(),

                    Colunas = obj.Colunas.Select(p => new Coluna_PainelVendaModel
                    {
                        Coluna = p.Coluna,
                        Descricao = HtmlEnumExtensions.GetEnumDescription<Coluna_PainelVendaEnum>(p.Coluna),
                        Id = p.Id
                    }).ToList()

                };
            }
        }

        private void BindViewToObj(ISession session, Modelo_PainelVenda objBanco, Modelo_PainelVendaCadastroModel view)
        {
            using (var repository = new Conexao<Modelo_PainelVenda>(session))
            {
                objBanco.Descricao = view.Descricao;
                objBanco.Visibilidade = view.Visibilidade;
                objBanco.Coluna_Ordenar = view.Coluna_Ordenar;
                objBanco.Padrao = view.Padrao;
                objBanco.Filtros = view.Filtros.Where(p => p.Id == 0).Select(p =>
                {
                    var retorno = new Filtro_PainelVenda
                    {
                        Modelo_PainelVenda = objBanco,
                        Filtro = p.Filtro,
                        Valor = p.Valor
                    };

                    if (p.Filtro == Filtro_PainelVendaEnum.Data_Venda)
                        retorno.Data = (ModeloPainel_Data)Convert.ToInt32(p.Valor);

                    if (p.Filtro == Filtro_PainelVendaEnum.DataHora_Aprovacao)
                        retorno.Data = (ModeloPainel_Data)Convert.ToInt32(p.Valor);

                    if (p.Filtro == Filtro_PainelVendaEnum.DataHora_Cancelamento)
                        retorno.Data = (ModeloPainel_Data)Convert.ToInt32(p.Valor);

                    return retorno;

                }).ToList();

                objBanco.Colunas = view.Colunas.Where(p => p.Id == 0).Select(p => new Coluna_PainelVenda
                {
                    Modelo_PainelVenda = objBanco,
                    Coluna = p.Coluna
                }).ToList();
            }
        }

        private Filtro_PainelVendaModel getFiltroViewObj(Filtro_PainelVenda filtro, ISession session)
        {
            using (var repoCidade = new Conexao<Cidade>(session))
            {

                Filtro_PainelVendaModel viewobj = new Filtro_PainelVendaModel();
                viewobj.Id = filtro.Id;
                viewobj.Filtro = filtro.Filtro;
                viewobj.Filtro_Descricao = HtmlEnumExtensions.GetEnumDescription<Filtro_PainelVendaEnum>(filtro.Filtro);
                viewobj.Valor = filtro.Valor;


                switch (filtro.Filtro)
                {
                    case Filtro_PainelVendaEnum.Cidade:
                        var id_tipo_servico = Convert.ToInt64(filtro.Valor);
                        viewobj.Descricao = repoCidade.selectFirst(p => p.Id == filtro.Id).Result.Descricao;
                        break;
                    case Filtro_PainelVendaEnum.StatusVenda:
                        var status = Convert.ToInt64(filtro.Valor);
                        viewobj.Descricao = HtmlEnumExtensions.GetEnumDescription<Status_Venda>((Status_Venda)Convert.ToInt32(filtro.Valor));
                        break;
                    case Filtro_PainelVendaEnum.Data_Venda:
                        var data_venda = Convert.ToInt64(filtro.Valor);
                        viewobj.Descricao = HtmlEnumExtensions.GetEnumDescription<ModeloPainel_Data>((ModeloPainel_Data)Convert.ToInt32(filtro.Valor));
                        break;
                    case Filtro_PainelVendaEnum.DataHora_Aprovacao:
                        var dataHora_Aprovacao = Convert.ToInt64(filtro.Valor);
                        viewobj.Descricao = HtmlEnumExtensions.GetEnumDescription<ModeloPainel_Data>((ModeloPainel_Data)Convert.ToInt32(filtro.Valor));
                        break;
                    case Filtro_PainelVendaEnum.DataHora_Cancelamento:
                        var dataHora_Cancelamento = Convert.ToInt64(filtro.Valor);
                        viewobj.Descricao = HtmlEnumExtensions.GetEnumDescription<ModeloPainel_Data>((ModeloPainel_Data)Convert.ToInt32(filtro.Valor));
                        break;
                    case Filtro_PainelVendaEnum.Categoria_Cliente:
                        using (var repositoryCategoria = new Conexao<Categoria_Cliente>(session))
                        {
                            viewobj.Descricao = repositoryCategoria.selectFirst(p => p.Id == Convert.ToInt64(filtro.Valor)).Result?.Descricao;
                        }
                        break;
                    case Filtro_PainelVendaEnum.Forma_Pagamento:
                        using (var repositoryFormaPagamento = new Conexao<Forma_Pagamento>(session))
                        {
                            viewobj.Descricao = repositoryFormaPagamento.selectFirst(p => p.Id == Convert.ToInt64(filtro.Valor)).Result?.Descricao;
                        }
                        break;
                    case Filtro_PainelVendaEnum.MeioCaptacao:
                        using (var repositoryMeioCaptacao = new Conexao<Meio_Captacao>(session))
                        {
                            viewobj.Descricao = repositoryMeioCaptacao.selectFirst(p => p.Id == Convert.ToInt64(filtro.Valor)).Result?.Descricao;
                        }
                        break;
                    case Filtro_PainelVendaEnum.Plano:
                        using (var repositoryPlano = new Conexao<Plano>(session))
                        {
                            viewobj.Descricao = repositoryPlano.selectFirst(p => p.Id == Convert.ToInt64(filtro.Valor)).Result?.Descricao;
                        }
                        break;
                    case Filtro_PainelVendaEnum.Produto:
                        using (var repositoryProduto = new Conexao<Produto>(session))
                        {
                            viewobj.Descricao = repositoryProduto.selectFirst(p => p.Id == Convert.ToInt64(filtro.Valor)).Result?.Descricao;
                        }
                        break;
                    case Filtro_PainelVendaEnum.Supervisor:
                        using (var repositorySupervisor = new Conexao<Funcionario>(session))
                        {
                            viewobj.Descricao = repositorySupervisor.selectFirst(p => p.Id == Convert.ToInt64(filtro.Valor)).Result?.Nome;
                        }
                        break;
                    case Filtro_PainelVendaEnum.TipoCadastro:

                        viewobj.Descricao = HtmlEnumExtensions.GetEnumDescription<Tipo_Cliente>((Tipo_Cliente)Convert.ToInt32(filtro.Valor));

                        break;
                    case Filtro_PainelVendaEnum.TipoPessoa:

                        viewobj.Descricao = HtmlEnumExtensions.GetEnumDescription<Tipo_Pessoa>((Tipo_Pessoa)Convert.ToInt32(filtro.Valor));

                        break;
                    case Filtro_PainelVendaEnum.Tipo_Imovel:
                        using (var repositoryTipoImovel = new Conexao<Tipo_Imovel>(session))
                        {
                            viewobj.Descricao = repositoryTipoImovel.selectFirst(p => p.Id == Convert.ToInt64(filtro.Valor)).Result?.Descricao;
                        }
                        break;
                    case Filtro_PainelVendaEnum.UF:
                        viewobj.Descricao = HtmlEnumExtensions.GetEnumDescription<UF>((UF)Convert.ToInt32(filtro.Valor));
                        break;
                    case Filtro_PainelVendaEnum.Vendedor:
                        using (var repositoryVendedor = new Conexao<Funcionario>(session))
                        {
                            viewobj.Descricao = repositoryVendedor.selectFirst(p => p.Id == Convert.ToInt64(filtro.Valor)).Result?.Nome;
                        }
                        break;

                }

                return viewobj;
            }
        }
    }
}