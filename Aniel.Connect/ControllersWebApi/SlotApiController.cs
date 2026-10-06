using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using NHibernate.Criterion;
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
    public class SlotApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/SlotApi/GetAll")]
        public async Task<DataTableResponse<SlotListaModel>> GetAll(Slot_DataTableRequest request)
        {

            List<SlotListaModel> retorno = new List<SlotListaModel>();

            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                StringBuilder sqlFrom = new StringBuilder();
                sqlFrom.AppendLine(" FROM TB_SLOT A ");
                sqlFrom.AppendLine(" INNER JOIN TB_OLT B ");
                sqlFrom.AppendLine(" ON A.ID_OLT = B.ID ");
                sqlFrom.AppendLine(" WHERE A.ID > 0 ");

                //Filtrando                                        
                if (!String.IsNullOrEmpty(request.search.value))
                {
                    if (request.search.option == "slot")
                    {
                        sqlFrom.AppendLine(" AND ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                    }
                    else if (request.search.option == "olt")
                    {
                        sqlFrom.AppendLine(" AND ((UPPER(B.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                    }
                }

                if (!String.IsNullOrEmpty(request.Id_Olt))
                {
                    sqlFrom.AppendLine(" AND (A.ID_OLT = @ID_OLT) ");
                    parametros.Add("@ID_OLT", Convert.ToInt32(request.Id_Olt));
                }

                if (!String.IsNullOrEmpty(request.Id_Slot))
                {
                    sqlFrom.AppendLine("AND (A.ID = @ID_SLOT)");
                    parametros.Add("@ID_SLOT", Convert.ToInt32(request.Id_Slot));
                }

                if (!String.IsNullOrEmpty(request.Status))
                {
                    sqlFrom.AppendLine("AND (A.STATUS = @STATUS)");
                    parametros.Add("@STATUS", Convert.ToInt32(request.Status));
                }

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT COUNT(A.ID) CO ");
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

                    sql.AppendLine(" SELECT A.ID, A.NUM_SLOT, A.DESCRICAO, B.DESCRICAO AS OLT, A.STATUS, A.DATAHORACRIACAO ");
                    sql.AppendLine(sqlFrom.ToString());

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new SlotListaModel
                            {
                                DATAHORACRIACAO = Convert.ToString(reader["DATAHORACRIACAO"]),
                                Id = Convert.ToInt64(reader["ID"]),
                                NumSlot = Convert.ToInt64(reader["NUM_SLOT"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                OLT = Convert.ToString(reader["OLT"]),
                                Status = Convert.ToString(reader["STATUS"]) == "1" ? Resources.RsCadastros.ATIVO : Resources.RsCadastros.INATIVO
                            });
                        }
                    }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<SlotListaModel>
            {
                draw = request.draw,
                recordsFiltered = recordsTotal,
                data = retorno.OrderBy(p => p.Id),
                error = ""
            };
        }

        [HttpPost()]
        [Route("api/SlotApi/GetCombo")]
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_SLOT A ");

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
                        condicoes.Add($"A.ID_OLT = {request.idpai}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 60)}%')");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.DESCRICAO, A.NUM_SLOT ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new SlotComboResult
                                {
                                    Id = Convert.ToInt32(reader["ID"]),
                                    Text = $"{reader["DESCRICAO"]}",
                                    NumSlot = Convert.ToString(reader["NUM_SLOT"])
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
        [Route("api/SlotApi/GetTreeView")]
        public async Task<GetApiRetorno<List<SlotTreeViewModel>>> GetTreeView(Int64 Id_OLT,Int64? Id_SLOT)
        {
            GetApiRetorno<List<SlotTreeViewModel>> retorno = new GetApiRetorno<List<SlotTreeViewModel>>
            {
                Objeto = new List<SlotTreeViewModel>()
            };

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        Dictionary<String, Object> parametros = new Dictionary<String, Object>
                        {
                            { "@ID_OLT", Id_OLT }
                        };

                        StringBuilder sql = new StringBuilder($"SELECT ID, DESCRICAO FROM TB_SLOT WHERE ID_OLT = @ID_OLT");

                        if (Id_SLOT.HasValue)
                        {
                            sql.Append(" AND ID = @ID_SLOT");
                            parametros.Add("@ID_SLOT", Id_SLOT);
                        }



                        

                        var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null);

                        while (await reader.ReadAsync())
                        {
                            retorno.Objeto.Add(new SlotTreeViewModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"])
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
        public async Task<GetApiRetorno<IEnumerable<SlotListaModel>>> GetList()
        {
            GetApiRetorno<IEnumerable<SlotListaModel>> retorno = new GetApiRetorno<IEnumerable<SlotListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Slot>(session))
            {
                try
                {
                    var lista = await repository.queryover().ListAsync();

                    retorno.Objeto = lista.Select(p => new SlotListaModel
                    {
                        Id = p.Id,
                        Descricao = p.Descricao,
                        //Grupo = $"{p.OLT?.Filial?.Empresa?.CodEmpresa}-{p.OLT?.Filial?.CodFilial}-{p.OLT?.OLTID}-{p.SlotID}",
                        //OLT = p.OLT?.Descricao
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

        private SlotCadastroModel GetViewObj(Slot obj)
        {
            return new SlotCadastroModel
            {
                Id = obj.Id,
                Status = obj.Status == Status_Default.Ativo,
                Descricao = obj.Descricao,

                Id_OLT = obj.OLT?.Id,
                
                LineProfile = obj.LineProfile,
                NumSlot = obj.NumSlot
            };
        }

        private async Task BindViewToObj(ISession session, Slot objBanco, SlotCadastroModel view)
        {
            using (var repoSlot = new Conexao<Slot>(session))
            using (var repoOLT = new Conexao<OLT>(session))
            {
                var SlotNumRepetido = 
                    await repoSlot.selectFirst(
                        p => p.NumSlot == view.NumSlot && 
                             p.OLT.Id == view.Id_OLT &&
                             p.Id != objBanco.Id);

                if (SlotNumRepetido != null)
                    throw new Exception("Num. Slot já cadastrado no sistema");
                else
                    objBanco.NumSlot = view.NumSlot;

                objBanco.Descricao = view.Descricao;
                objBanco.LineProfile = view.LineProfile;

                objBanco.NumSlot = view.NumSlot;
                objBanco.OLT = await repoOLT.selectFirst(p => p.Id == view.Id_OLT);
            }
        }

        [HttpPost()]
        [Route("api/SlotApi/excluirlista")]
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

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_SLOT WHERE ID IN {_ids}");

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
        [Route("api/SlotApi/FindById")]
        public async Task<GetApiRetorno<SlotCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<SlotCadastroModel> retorno = new GetApiRetorno<SlotCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Slot>(session))
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
        [Route("api/SlotApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody]ParamWebApi<SlotCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Slot modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Slot>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new Slot
                            {
                                UsuarioCriacao = value.Usuario,
                                Status = Status_Default.Ativo,
                                SlotID = (await repository.Max(p => p.SlotID, y => y.OLT.Id == value.Objeto.Id_OLT)) + 1
                            };
                        }
                        else
                        {
                            modelo.UsuarioAlteracao = value.Usuario;
                            modelo.Status = value.Objeto.Status ? Status_Default.Ativo : Status_Default.Inativo;
                        }

                        await BindViewToObj(session, modelo, value.Objeto);

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

        [HttpPost()]
        [Route("api/SLOTapi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_SLOT SET STATUS = {value.status} WHERE ID IN {_ids}");

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
    }
}
