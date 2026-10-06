using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
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
using Resources;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class Atividade_StatusVendaApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/Atividade_StatusVendaApi/GetAll")]
        public async Task<DataTableResponse<Atividade_StatusVendaListModels>> GetAll(Atividade_StatusVenda_DataTableRequest request)
        {

            List<Atividade_StatusVendaListModels> retorno = new List<Atividade_StatusVendaListModels>();
            IOrderedEnumerable<Atividade_StatusVendaListModels> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO FROM TB_ATIVSTATUS_VENDA A");
                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine(" WHERE ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }

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
                    parametros = new Dictionary<string, object>();

                    sql.AppendLine("SELECT A.ID, A.DESCRICAO, A.STATUS_PADRAO, A.STATUS ");
                    sql.AppendLine("FROM TB_ATIVSTATUS_VENDA A");

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine(" WHERE ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null,request.length,request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Atividade_StatusVendaListModels
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                StatusPadrao = HtmlEnumExtensions.GetEnumDescription<Status_Venda>(Convert.ToString(reader["STATUS_PADRAO"])),
                                Status = Convert.ToString(reader["STATUS"]) == "0" ? RsCadastros.INATIVO : RsCadastros.ATIVO, 
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
                                listaOrdenada = retorno.OrderBy(p => p.Id);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Id);
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
            return new DataTableResponse<Atividade_StatusVendaListModels>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        private Atividade_StatusVendaCadastroModels GetViewObj(Atividade_StatusVenda obj)
        {

            var statusView = new Atividade_StatusVendaCadastroModels();
            statusView.Descricao = obj.Descricao;
            statusView.StatusPadrao = obj.StatusPadrao;
            statusView.Status = obj.Status == Status_Default.Ativo ? true : false;
            statusView.Id = obj.Id;



            return statusView;
        }

        private void BindViewToObj(ISession session, Atividade_StatusVenda objBanco, Atividade_StatusVendaCadastroModels view)
        {

            objBanco.Descricao = view.Descricao;
            objBanco.StatusPadrao = view.StatusPadrao.Value;
            objBanco.Status = view.Status ? Status_Default.Ativo : Status_Default.Inativo;
        }

        [HttpPost()]
        [Route("api/Atividade_StatusVendaApi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_ATIVSTATUS_VENDA SET STATUS = {value.status} WHERE ID IN {_ids}");

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
        [Route("api/Atividade_StatusVendaApi/excluirlista")]
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

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_ATIVSTATUS_VENDA WHERE ID IN {_ids}");

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
        [Route("api/Atividade_StatusVendaApi/GetList")]
        public async Task<GetApiRetorno<IEnumerable<Atividade_StatusVendaListModels>>> GetList()
        {
            GetApiRetorno<IEnumerable<Atividade_StatusVendaListModels>> retorno = new GetApiRetorno<IEnumerable<Atividade_StatusVendaListModels>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoAtividade_StatusVenda = new Conexao<Atividade_StatusVenda>(session))
            {
                try
                {
                    var lista = await repoAtividade_StatusVenda.queryover().ListAsync();

                    retorno.Objeto = lista.Select(p => new Atividade_StatusVendaListModels
                    {
                        Id = p.Id,
                        Descricao = p.Descricao,
                        
                        

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

        [HttpGet()]
        [Route("api/Atividade_StatusVendaApi/FindById")]
        public async Task<GetApiRetorno<Atividade_StatusVendaCadastroModels>> FindById(Int64 Id)
        {
            GetApiRetorno<Atividade_StatusVendaCadastroModels> retorno = new GetApiRetorno<Atividade_StatusVendaCadastroModels>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Atividade_StatusVenda>(session))
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
        [Route("api/Atividade_StatusVendaApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<Atividade_StatusVendaCadastroModels> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Atividade_StatusVenda modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Atividade_StatusVenda>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new Atividade_StatusVenda
                            {
                                UsuarioCriacao = value.Usuario,
                            };
                        }

                        else
                        {
                            // modelo.Status = value.Objeto.Status ? Status_Default.Ativo : Status_Default.Inativo;
                            modelo.UsuarioAlteracao = value.Usuario;
                        }

                        BindViewToObj(session, modelo, value.Objeto);

                        await repository.SaveOrUpdate(modelo);
                        //await repository.Save(modelo);
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

        //[HttpPost()]
        //[Route("api/Atividade_StatusVendaApi/GetCombo")]
        //public async Task<ObjCombo<Int64>> GetCombo(RequestObjCombo<Int64?> request)
        //{

        //    ObjCombo<Int64> retorno = new ObjCombo<Int64>
        //    {
        //        results = new List<IObjComboResult<Int64>>()
        //    };

        //    using (ISession session = HibernateUtil.GetSession())
        //    {
        //        try
        //        {
        //            //Filtros 
        //            Dictionary<String, Object> parametros = new Dictionary<string, object>();

        //            StringBuilder sqlFromStatusVenda = new StringBuilder(" FROM TB_ATIVSTATUS_VENDA A ");
        //            List<String> statusVenda = new List<String>();

        //            if (request.soativos)
        //            {
        //                statusVenda.Add($"A.STATUS = 1");
        //            }

        //            if (request.id.HasValue)
        //            {
        //                statusVenda.Add($"A.ID = {request.id}");
        //            }

        //            if (request != null && !String.IsNullOrEmpty(request.q))
        //            {
        //                statusVenda.Add($"(UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 255)}%')");
        //            }

        //            if (statusVenda.Count() > 0)
        //            {
        //                sqlFromStatusVenda.AppendLine($" WHERE A.DESCRICAO != '' AND ( {String.Join(" AND ", statusVenda) } ) ");
        //            }

        //            using (var com = session.Connection.CreateCommand())
        //            {
        //                StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) AS CO ");
        //                sql.AppendLine(sqlFromStatusVenda.ToString());

        //                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
        //                {
        //                    if (await reader.ReadAsync())
        //                        retorno.total_count = Convert.ToInt32(reader["CO"]);
        //                }
        //            }

        //            //Select
        //            using (var com = session.Connection.CreateCommand())
        //            {
        //                StringBuilder sql = new StringBuilder("SELECT A.ID, A.DESCRICAO ");
        //                sql.AppendLine(sqlFromStatusVenda.ToString());
        //                sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

        //                using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
        //                {
        //                    while (await reader.ReadAsync())
        //                    {

        //                        if (!String.IsNullOrEmpty(Convert.ToString(reader["DESCRICAO"])))
        //                        {
        //                            retorno.results.Add(new ObjComboResult<Int64>
        //                            {
        //                                Id = Convert.ToInt64(reader["ID"]),
        //                                Text = Convert.ToString(reader["DESCRICAO"])
        //                            });

        //                        }
        //                    }
        //                }
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //    }

        //    return retorno;
        //}

        [HttpPost()]
        [Route("api/Atividade_StatusVendaApi/GetCombo")]
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

                    StringBuilder sqlFrom = new StringBuilder(@" FROM TB_ATIVSTATUS_VENDA A ");

                    List<String> condicoes = new List<String>();
                    //condicoes.Add("((A.VISIBILIDADE = 0 AND A.USUARIOCRIACAO = @USER) OR (A.VISIBILIDADE = 1) )");
                    //parametros.Add("@USER", request.id_usuario);

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

                        sql.AppendLine(" SELECT A.ID, A.DESCRICAO, A.STATUS_PADRAO, A.STATUS ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" ORDER BY A.STATUS_PADRAO, A.DESCRICAO ");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                String cod_grupo = HtmlEnumExtensions.GetEnumDescription<Status_Venda>(Convert.ToString(reader["STATUS_PADRAO"]));
                                var grupo_result = retorno.results.Find(res => res.Id_Grupo == cod_grupo);

                                var option = new ObjComboStatusVenda<Int64>
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Text = Convert.ToString(reader["DESCRICAO"]),
                                    StatusFixo = cod_grupo,
                                    IdStatusPadrao = Convert.ToInt64(reader["STATUS_PADRAO"]),
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









        [Route("api/Atividade_StatusVendaApi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody] ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Atividade_StatusVenda>(session))
                    {

                        Atividade_StatusVenda modelo = null;
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


    }
}
