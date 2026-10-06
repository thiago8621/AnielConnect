using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Microsoft.AspNet.Identity;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model;
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
    public class Modelo_PainelAtendimentoApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/Modelo_PainelAtendimentoApi/GetAll")]
        public async Task<DataTableResponse<Modelo_PainelAtendimentoListaModel>> GetAll(Modelo_PainelAtendimento_DataTableRequest request)
        {
            List<Modelo_PainelAtendimentoListaModel> retorno = new List<Modelo_PainelAtendimentoListaModel>();
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                StringBuilder sqlFrom = new StringBuilder();
                sqlFrom.AppendLine("FROM TB_MODELO_PAINEL_ATENDIMENTO A");

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

                    parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpperInvariant()));
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

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT A.ID, A.DESCRICAO,A.STATUS,A.VISIBILIDADE,A.COLUNA_ORDENAR, A.PADRAO, ");
                    sql.AppendLine(HtmlEnumExtensions.getEnumDescriptionInSql<Status_Default>("A.STATUS", "STATUS_ORDENAR"));
                    sql.AppendLine("," + HtmlEnumExtensions.getEnumDescriptionInSql<Visibilidade>("A.VISIBILIDADE", "VISIBILIDADE_ORDENAR"));
                    sql.AppendLine("," + HtmlEnumExtensions.getEnumDescriptionInSql<Coluna_PainelAtendimentoEnum>("A.COLUNA_ORDENAR", "ORDENAR_POR"));
                    sql.AppendLine("," + HtmlEnumExtensions.getEnumDescriptionInSql<Padrao>("A.PADRAO", "PADRAO"));
                    sql.AppendLine(sqlFrom.ToString());


                    //Ordenacao
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


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Modelo_PainelAtendimentoListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Visibilidade = HtmlEnumExtensions.GetEnumDescription<Visibilidade>(Convert.ToString(reader["VISIBILIDADE"])),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Ordenar_Por = HtmlEnumExtensions.GetEnumDescription<Coluna_PainelAtendimentoEnum>(Convert.ToString(reader["COLUNA_ORDENAR"])),
                                Padrao = HtmlEnumExtensions.GetEnumDescription<Padrao>(Convert.ToString(reader["PADRAO"])),
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"]))
                            });
                        }
                    }

                }
            }



            //Page e retornando DataTable
            return new DataTableResponse<Modelo_PainelAtendimentoListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = retorno.ToList(),
                error = ""
            };

        }

        private Modelo_PainelAtendimentoCadastroModel GetViewObj(Modelo_PainelAtendimento obj, ISession session)
        {
            using (var repository = new Conexao<Modelo_PainelAtendimento>(session))
            using (var repository_SLA_Atendimento = new Conexao<SLA_Atendimento>(session))
            using (var repository_Categoria_Cliente = new Conexao<Categoria_Cliente>(session))
            using (var repository_Categoria_Atendimento = new Conexao<Categoria_Atendimento>(session))
            using (var repository_Tipo_Atendimento = new Conexao<Tipo_Atendimento>(session))
            using (var repository_Motivo_Status_Atendimento = new Conexao<Motivo_Status_Atendimento>(session))
            using (var repository_Departamento = new Conexao<Departamento>(session))
            using (var repository_Origem_Atendimento = new Conexao<Origem_Atendimento>(session))
            {
                return new Modelo_PainelAtendimentoCadastroModel
                {
                    Id = obj.Id,
                    Descricao = obj.Descricao,
                    Status = obj.Status == Status_Default.Ativo,
                    Visibilidade = obj.Visibilidade,
                    Coluna_Ordenar = obj.Coluna_Ordenar,
                    Padrao = obj.Padrao,
                    Filtros = obj.Filtros.Select(p => getFiltroViewObj(p, repository_SLA_Atendimento, repository_Categoria_Cliente,
                                                                        repository_Categoria_Atendimento, repository_Tipo_Atendimento,
                                                                        repository_Motivo_Status_Atendimento, repository_Departamento, repository_Origem_Atendimento)).ToList(),

                    Colunas = obj.Colunas.Select(p => new Coluna_PainelAtendimentoModel
                    {
                        Coluna = p.Coluna,
                        Descricao = HtmlEnumExtensions.GetEnumDescription<Coluna_PainelAtendimentoEnum>(p.Coluna),
                        Id = p.Id
                    }).ToList()

                };
            }
        }

        private Filtro_PainelAtendimentoModel getFiltroViewObj(Recurso_PainelAtendimento filtro, Conexao<SLA_Atendimento> repository_SLA_Atendimento,
            Conexao<Categoria_Cliente> repository_Categoria_Cliente, Conexao<Categoria_Atendimento> repository_Categoria_Atendimento,
            Conexao<Tipo_Atendimento> repository_Tipo_Atendimento, Conexao<Motivo_Status_Atendimento> repository_Motivo_Status_Atendimento,
            Conexao<Departamento> repository_Departamento, Conexao<Origem_Atendimento> repository_Origem_Atendimento)
        {

            Filtro_PainelAtendimentoModel viewobj = new Filtro_PainelAtendimentoModel();
            viewobj.Id = filtro.Id;
            viewobj.Filtro = filtro.Recurso;
            viewobj.Filtro_Descricao = HtmlEnumExtensions.GetEnumDescription<Recurso_PainelAtendimentoEnum>(filtro.Recurso);
            viewobj.Valor = filtro.Valor;


            switch (filtro.Recurso)
            {
                case Recurso_PainelAtendimentoEnum.Categoria_Atendimento:
                    var id_categoria_atendimento = Convert.ToInt64(filtro.Valor);
                    viewobj.Descricao = repository_Categoria_Atendimento.selectFirst(p => p.Id == id_categoria_atendimento).Result.Descricao;
                    break;
                case Recurso_PainelAtendimentoEnum.Categoria_Cliente:
                    var id_categoria_cliente = Convert.ToInt64(filtro.Valor);
                    viewobj.Descricao = repository_Categoria_Cliente.selectFirst(p => p.Id == id_categoria_cliente).Result.Descricao;
                    break;
                case Recurso_PainelAtendimentoEnum.Motivo_Status:
                    var id_motivo = Convert.ToInt64(filtro.Valor);
                    viewobj.Descricao = repository_Motivo_Status_Atendimento.selectFirst(p => p.Id == id_motivo).Result.Descricao;
                    break;
                case Recurso_PainelAtendimentoEnum.Prioridade_Atendimento:
                    viewobj.Descricao = HtmlEnumExtensions.GetEnumDescription<Prioridade_Atendimento>((Prioridade_Atendimento)Convert.ToInt32(filtro.Valor));
                    break;
                case Recurso_PainelAtendimentoEnum.Setor:
                    var id_setor = Convert.ToInt64(filtro.Valor);
                    viewobj.Descricao = repository_Departamento.selectFirst(p => p.Id == id_setor).Result.Descricao;
                    break;
                case Recurso_PainelAtendimentoEnum.SLA_Atendimento:
                    viewobj.Descricao = HtmlEnumExtensions.GetEnumDescription<Status_SLA_Atendimento>((Status_SLA_Atendimento)Convert.ToInt32(filtro.Valor));
                    break;
                case Recurso_PainelAtendimentoEnum.Status:
                    viewobj.Descricao = HtmlEnumExtensions.GetEnumDescription<Status_Atendimento>((Status_Atendimento)Convert.ToInt32(filtro.Valor));
                    break;
                case Recurso_PainelAtendimentoEnum.Tipo_Atendimento:
                    var id_tipo_atendimento = Convert.ToInt64(filtro.Valor);
                    viewobj.Descricao = repository_Tipo_Atendimento.selectFirst(p => p.Id == id_tipo_atendimento).Result.Nome;
                    break;
                case Recurso_PainelAtendimentoEnum.Origem:
                    var id_origem_atendimento = Convert.ToInt64(filtro.Valor);
                    viewobj.Descricao = repository_Origem_Atendimento.selectFirst(p => p.Id == id_origem_atendimento).Result.Descricao;
                    break;
            }

            return viewobj;

        }

        private void BindViewToObj(ISession session, Modelo_PainelAtendimento objBanco, Modelo_PainelAtendimentoCadastroModel view)
        {
            using (var repository = new Conexao<Modelo_PainelAtendimento>(session))
            {
                objBanco.Descricao = view.Descricao;
                objBanco.Visibilidade = view.Visibilidade;
                objBanco.Coluna_Ordenar = view.Coluna_Ordenar;
                objBanco.Padrao = view.Padrao;
                objBanco.Filtros = view.Filtros.Where(p => p.Id == 0).Select(p => new Recurso_PainelAtendimento
                {
                    Modelo_PainelAtendimento = objBanco,
                    Recurso = p.Filtro,
                    Valor = p.Valor
                }).ToList();

                objBanco.Colunas = view.Colunas.Where(p => p.Id == 0).Select(p => new Coluna_PainelAtendimento
                {
                    Modelo_PainelAtendimento = objBanco,
                    Coluna = p.Coluna
                }).ToList();
            }
        }

        [HttpPost()]
        [Route("api/Modelo_PainelAtendimentoApi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_MODELO_PAINEL_ATENDIMENTO SET STATUS = {value.status} WHERE ID IN {_ids}");

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
        [Route("api/Modelo_PainelAtendimentoApi/tornarpublico")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_MODELO_PAINEL_ATENDIMENTO SET VISIBILIDADE = 1 WHERE ID IN {_ids}");

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
        [Route("api/Modelo_PainelAtendimentoApi/excluirlista")]
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

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_MODELO_PAINEL_ATENDIMENTO WHERE ID IN {_ids}");

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
        [Route("api/Modelo_PainelAtendimentoApi/FindById")]
        public async Task<GetApiRetorno<Modelo_PainelAtendimentoCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<Modelo_PainelAtendimentoCadastroModel> retorno = new GetApiRetorno<Modelo_PainelAtendimentoCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Modelo_PainelAtendimento>(session))

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

        [HttpGet()]
        [Route("api/Modelo_PainelAtendimentoApi/GetColunas")]
        public async Task<GetApiRetorno<GetColunas_Model>> GetColunas(Int64 id_modelo)
        {
            GetApiRetorno<GetColunas_Model> retorno = new GetApiRetorno<GetColunas_Model>();
            retorno.Objeto = new GetColunas_Model();
            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Modelo_PainelAtendimento>(session))
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
        [Route("api/Modelo_PainelAtendimentoApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<Modelo_PainelAtendimentoCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Modelo_PainelAtendimento modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Modelo_PainelAtendimento>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new Modelo_PainelAtendimento
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

        [Route("api/Modelo_PainelAtendimentoApi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody] ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Modelo_PainelAtendimento>(session))
                    {

                        Modelo_PainelAtendimento modelo = null;
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
        [Route("api/Modelo_PainelAtendimentoApi/GetCombo")]
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
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder(@" FROM TB_MODELO_PAINEL_ATENDIMENTO A ");


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

                    //Select
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

        [Route("api/Modelo_PainelAtendimentoApi/ExcluirGridFiltro")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGridFiltro(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Recurso_PainelAtendimento>(session))
                    {

                        Recurso_PainelAtendimento modelo = null;
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

        [Route("api/Modelo_PainelAtendimentoApi/ExcluirGridColuna")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGridColuna(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Coluna_PainelAtendimento>(session))
                    {

                        Coluna_PainelAtendimento modelo = null;
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

    }
}
