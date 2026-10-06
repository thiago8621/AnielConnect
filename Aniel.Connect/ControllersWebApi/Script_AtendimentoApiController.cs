using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using NHibernate.Criterion;
using Resources;
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
    public class Script_AtendimentoApiController : BaseApiController
    {
        [HttpPost()]
        [Route("api/script_atendimentoapi/GetAll")]
        public async Task<DataTableResponse<Script_AtendimentoListaModel>> GetAll(Script_Atendimento_DataTableRequest request)
        {
            List<Script_AtendimentoListaModel> retorno = new List<Script_AtendimentoListaModel>();
            IOrderedEnumerable<Script_AtendimentoListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_SCRIPT_ATENDIMENTO");

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
                    StringBuilder sqlWhere = new StringBuilder(" WHERE A.ID > 0 ");

                    sql.AppendLine(" SELECT A.ID, A.NOME, A.STATUS, A.NUMERO ");
                    sql.AppendLine(" FROM TB_SCRIPT_ATENDIMENTO A ");

                    if(!string.IsNullOrEmpty(request.Status))
                    {
                        sqlWhere.AppendLine(" AND A.STATUS = @status ");
                        parametros.Add("status", request.Status);
                    }

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sqlWhere.AppendLine(" AND ((UPPER(A.NOME) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }

                    sql.Append(sqlWhere);

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Script_AtendimentoListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Nome = Convert.ToString(reader["NOME"]),
                                Status = Convert.ToInt32(reader["STATUS"]) == 0 ? RsCadastros.INATIVO : RsCadastros.ATIVO,
                                Numero = Convert.ToString(reader["NUMERO"])
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
                    case 1:     // Nome
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Nome);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Nome);
                            break;
                        }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<Script_AtendimentoListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        [HttpPost()]
        [Route("api/Script_AtendimentoApi/GetCombo")]
        public async Task<ObjCombo<Int64>> GetCombo(RequestObjCombo<String> request)
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_SCRIPT_ATENDIMENTO A ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"STATUS = 1");
                    }

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"ID = {request.id}");
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

        [HttpGet()]
        [Route("api/Script_AtendimentoApi/GetInformacoesByAtenidmento")]
        public async Task<List<ObjBusca<Int64>>> GetInformacoesByAtenidmento(Int64 Id_Atendimento)
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
                        StringBuilder sql = new StringBuilder("select B.ID, B.INFORMACAO ")
                                            .AppendLine("from TB_TIPO_ATENDIMENTO A ")
                                            .AppendLine("INNER JOIN TB_SCRIPT_SUPORTETECNICOITEM B ")
                                            .AppendLine("ON(A.ID_SCRIPT_SUPORTETECNICO = B.ID_SCRIPT) ")
                                            .AppendLine("WHERE A.ID = @ID ")
                                            .AppendLine("ORDER BY UPPER(B.ORDEM) ");
                        parametros.Add("@ID", Id_Atendimento);

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new ObjBusca<Int64>
                                {
                                    Value = Convert.ToInt64(reader["ID"]),
                                    Label = Convert.ToString(reader["INFORMACAO"])
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

        [HttpPost()]
        public async Task<GetApiRetorno<IEnumerable<Script_AtendimentoListaModel>>> GetList()
        {
            GetApiRetorno<IEnumerable<Script_AtendimentoListaModel>> retorno = new GetApiRetorno<IEnumerable<Script_AtendimentoListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Script_Atendimento>(session))
            {
                try
                {
                    var lista = await repository.select(p => p.Status == Status_Default.Ativo);

                    retorno.Objeto = lista.Select(p => new Script_AtendimentoListaModel
                    {                        
                        Id = p.Id,
                        Nome = p.Nome
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

        private Script_AtendimentoCadastroModel GetViewObj(Script_Atendimento obj)
        {
            return new Script_AtendimentoCadastroModel
            {
                Id = obj.Id,
                Nome = obj.Nome,
                Status = obj.Status == Status_Default.Ativo,
                Perguntas = obj.Perguntas.Select(p => new Script_AtendimentoItemModel
                {
                    Id = p.Id,
                    Ordem = p.Ordem,
                    Pergunta = p.Pergunta,
                    Explicacao_Cliente = p.Explicacao_Cliente,
                    Explicacao_Operador = p.Explicacao_Operador,
                    Tipo_Resposta = p.Tipo_Resposta,
                    Respostas_Possiveis = p.Respostas_Possiveis,
                    Observacoes = p.Observacoes
                }).OrderBy(p => p.Ordem).ToList()
            };
        }

        private void BindViewToObj(ISession session, Script_Atendimento objBanco, Script_AtendimentoCadastroModel view)
        {
            objBanco.Nome = view.Nome;

            objBanco.Perguntas = view.Perguntas.Select(p => new Script_AtendimentoItem
            {
                Id = p.Id,
                Ordem = p.Ordem,
                Explicacao_Cliente = p.Explicacao_Cliente,
                Explicacao_Operador = p.Explicacao_Operador,
                Pergunta = p.Pergunta,
                Tipo_Resposta = p.Tipo_Resposta,
                Respostas_Possiveis = p.Respostas_Possiveis,
                Observacoes = p.Observacoes,
                Script = objBanco
            }).ToList();
        }

        [HttpPost()]
        [Route("api/Script_Atendimentoapi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_SCRIPT_ATENDIMENTO SET STATUS = {value.status} WHERE ID IN {_ids}");

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
        [Route("api/Script_Atendimentoapi/excluirlista")]
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

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_Script_Atendimento WHERE ID IN {_ids}");

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
        [Route("api/Script_AtendimentoApi/FindById")]
        public async Task<GetApiRetorno<Script_AtendimentoCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<Script_AtendimentoCadastroModel> retorno = new GetApiRetorno<Script_AtendimentoCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoScript_Atendimento = new Conexao<Script_Atendimento>(session))
            {
                try
                {
                    retorno.Objeto = GetViewObj(await repoScript_Atendimento.selectFirst(p => p.Id == Id));
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
        [Route("api/Script_AtendimentoApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody]ParamWebApi<Script_AtendimentoCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Script_Atendimento modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Script_Atendimento>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            Int64? Num_Max = session.Query<Script_Atendimento>().Max(p => (Int64?)p.Numero);

                            if (Num_Max == null)
                                Num_Max = 1;
                            else
                                Num_Max++;

                            modelo = new Script_Atendimento
                            {
                                Numero = (Int64)Num_Max,
                                UsuarioCriacao = value.Usuario,
                                Status = Status_Default.Ativo
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

        [Route("api/Script_AtendimentoApi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody]ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Script_Atendimento>(session))
                    {

                        Script_Atendimento modelo = null;
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

        [Route("api/Script_AtendimentoApi/ExcluirGrid")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGrid(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Script_AtendimentoItem>(session))
                    {

                        Script_AtendimentoItem modelo = null;
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
