using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using NHibernate.Criterion;
using OpenXmlPowerTools;
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
    public class Motivo_Aprovacao_RestricaoApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/Motivo_Aprovacao_RestricaoApi/GetAll")]
        public async Task<DataTableResponse<Motivo_Aprovacao_RestricaoListaModel>> GetAll(Motivo_Aprovacao_Restricao_DataTableRequest request)
        {

            List<Motivo_Aprovacao_RestricaoListaModel> retorno = new List<Motivo_Aprovacao_RestricaoListaModel>();
            IOrderedEnumerable<Motivo_Aprovacao_RestricaoListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO FROM TB_MOTIVO_APROVACAO_RESTRICAO A");

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

                    sql.AppendLine("SELECT A.STATUS, A.ID, A.DESCRICAO, A.TIPO_RESTRICAO, A.EXIBEAPPOLA,A.USUARIOCRIACAO ");
                    sql.AppendLine(" FROM TB_MOTIVO_APROVACAO_RESTRICAO A");

                    // Filtrando
                    var condicoes = new List<string>();

                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        condicoes.Add("UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH)");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                    }

                    if (!String.IsNullOrEmpty(request.Status))
                    {
                        condicoes.Add("A.STATUS = @STATUS");
                        parametros.Add("@STATUS", request.Status);
                    }

                    if (!String.IsNullOrEmpty(request.Tipo_Restricao))
                    {
                        if (int.TryParse(request.Tipo_Restricao, out int tipo))
                        {
                            condicoes.Add("A.TIPO_RESTRICAO = @TIPO_RESTRICAO");
                            parametros.Add("@TIPO_RESTRICAO", tipo);
                        }
                    }

                    if (condicoes.Any())
                        sql.AppendLine(" WHERE " + String.Join(" AND ", condicoes));

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Motivo_Aprovacao_RestricaoListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Usuario_Criacao = Convert.ToString(reader["USUARIOCRIACAO"]),
                                Tipo_Restricao = HtmlEnumExtensions.GetEnumDescription<EnumMotivo_Aprovacao_Restricao>(Convert.ToString(reader["TIPO_RESTRICAO"])),
                                ExibeAppOla = HtmlEnumExtensions.GetEnumDescription<EnumExibeAppOla>(Convert.ToString(reader["EXIBEAPPOLA"])),
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
                    case 1:     // Nome
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Id);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Id);
                            break;
                        }
                    case 2:     // Nome
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Status);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Status);
                            break;
                        }

                    default:
                        {
                            listaOrdenada = retorno.OrderBy(p => p.Descricao);
                            break;
                        }
                }
            }
            //Page e retornando DataTable
            return new DataTableResponse<Motivo_Aprovacao_RestricaoListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };
        }

        private Motivo_Aprovacao_RestricaoCadastroModel GetViewObj(Motivo_Aprovacao_Resticao obj)
        {
            return new Motivo_Aprovacao_RestricaoCadastroModel
            {

                Id = obj.Id,
                Descricao = obj.Descricao,
                Status = Convert.ToBoolean(obj.Status == Status_Default.Ativo),
                Tipo_Restricao = obj.Tipo_Restricao,
                ExibeAppOla = obj.ExibeAppOla == EnumExibeAppOla.Não

            };

        }

        private void BindViewToObj(ISession session, Motivo_Aprovacao_Resticao objBanco, Motivo_Aprovacao_RestricaoCadastroModel view)
        {
            objBanco.Descricao = view.Descricao;            
            objBanco.ExibeAppOla = Convert.ToBoolean(view.ExibeAppOla) ? EnumExibeAppOla.Sim : EnumExibeAppOla.Não;
            objBanco.Tipo_Restricao = (EnumMotivo_Aprovacao_Restricao)view.Tipo_Restricao;
        }

        [HttpPost()]
        [Route("api/Motivo_Aprovacao_RestricaoApi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_MOTIVO_APROVACAO_RESTRICAO SET STATUS = {value.status} WHERE ID IN {_ids}");

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
        [Route("api/Motivo_Aprovacao_RestricaoApi/excluirlista")]
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

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_MOTIVO_APROVACAO_RESTRICAO WHERE ID IN {_ids}");

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
        [Route("api/Motivo_Aprovacao_RestricaoApi/GetList")]
        public async Task<GetApiRetorno<IEnumerable<Motivo_Aprovacao_RestricaoListaModel>>> GetList()
        {
            GetApiRetorno<IEnumerable<Motivo_Aprovacao_RestricaoListaModel>> retorno = new GetApiRetorno<IEnumerable<Motivo_Aprovacao_RestricaoListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoTipo_Desconto = new Conexao<Motivo_Aprovacao_Resticao>(session))
            {
                try
                {
                    var lista = await repoTipo_Desconto.queryover().ListAsync();

                    retorno.Objeto = lista.Select(p => new Motivo_Aprovacao_RestricaoListaModel
                    {
                        Id = p.Id,
                        Descricao = p.Descricao,
                        Tipo_Restricao = p.Tipo_Restricao.ToString(),
                        ExibeAppOla = p.ExibeAppOla.ToString()
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
        [Route("api/Motivo_Aprovacao_RestricaoApi/FindById")]
        public async Task<GetApiRetorno<Motivo_Aprovacao_RestricaoCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<Motivo_Aprovacao_RestricaoCadastroModel> retorno = new GetApiRetorno<Motivo_Aprovacao_RestricaoCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Motivo_Aprovacao_Resticao>(session))
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
        [Route("api/Motivo_Aprovacao_RestricaoApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<Motivo_Aprovacao_RestricaoCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Motivo_Aprovacao_Resticao modelo = null;
                Motivo_Aprovacao_Resticao verificador = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Motivo_Aprovacao_Resticao>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();
                      

                        if (modelo == null)
                        {
                            modelo = new Motivo_Aprovacao_Resticao
                            {
                                Status = Status_Default.Ativo,
                                UsuarioCriacao = value.Usuario,
                                DataHoraCriacao = DateTime.Now  

                            };
                        }
                        else
                        {                           
                            modelo.UsuarioAlteracao = value.Usuario;
                            modelo.DataHoraAlteracao = DateTime.Now;
                            modelo.Status = Convert.ToBoolean(value.Objeto.Status) ? Status_Default.Ativo : Status_Default.Inativo;
                        }

                        BindViewToObj(session, modelo, value.Objeto);
                        try
                        {
                            await repository.SaveOrUpdate(modelo);
                        }
                        catch (Exception ex)
                        {
                            retorno.IdObjeto = 0;
                            retorno.Mensagem = ex.Message;
                            if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
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

        [Route("api/Motivo_Aprovacao_RestricaoApi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody] ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Motivo_Aprovacao_Resticao>(session))
                    {

                        Motivo_Aprovacao_Resticao modelo = null;
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
        [Route("api/Motivo_Aprovacao_RestricaoApi/GetCombo")]
        public async Task<ObjCombo<string>> GetCombo(RequestObjComboPai<string,string> request)
        {
            ObjCombo<string> retorno = new ObjCombo<string>
            {
                results = new List<IObjComboResult<string>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_MOTIVO_APROVACAO_RESTRICAO A ");


                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS = 1");
                    }
                    if (request.id != null)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }
                    if (request.idpai != null)
                    {
                        condicoes.Add($"A.TIPO_RESTRICAO = {request.idpai}");
                    }
                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"((UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 255)}%' ))");
                    }
                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE ( {String.Join(" AND ", condicoes)} ) ");
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

                        sql.AppendLine("SELECT A.ID, A.DESCRICAO, A.TIPO_RESTRICAO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" ORDER BY A.DESCRICAO ");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                           

                            while (await reader.ReadAsync())
                            {
                                var lista = new ObjComboResult<string>
                                {
                                    Id = Convert.ToString(reader["ID"]),
                                    Text = String.Format("{0}", reader["DESCRICAO"]),

                                };
                                retorno.results.Add(lista);

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
        [Route("api/Motivo_Aprovacao_RestricaoApi/GetComboAprovacao")]
        public async Task<ObjCombo<string>> GetComboAprovacao(RequestObjCombo<string> request)
        {
            ObjCombo<string> retorno = new ObjCombo<string>
            {
                results = new List<IObjComboResult<string>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_MOTIVO_APROVACAO_RESTRICAO A ");


                    List<String> condicoes = new List<String>();
                    condicoes.Add($"A.TIPO_RESTRICAO = 0");

                    if (request.soativos)
                    {
                        condicoes.Add($"(A.STATUS = 1)");
                    }
                    if (request.id != null)
                    {
                        condicoes.Add($"(A.ID = {request.id})");
                    }
                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"((UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 255)}%' ))");
                    }
                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes)} ) ");
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

                        sql.AppendLine("SELECT A.ID, A.DESCRICAO, A.TIPO_RESTRICAO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" ORDER BY A.DESCRICAO ");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            var opcaoOutros = new ObjComboResult<string>() { Id = "0", Text = String.Format("{0}", "Outro") };

                            while (await reader.ReadAsync())
                            {
                                var lista = new ObjComboResult<string>
                                {
                                    Id = Convert.ToString(reader["ID"]),
                                    Text = String.Format("{0}", reader["DESCRICAO"]),

                                };
                                retorno.results.Add(lista);

                            }
                            if (request.page == 1) 
                            {
                                retorno.results.Add(opcaoOutros); // opção OUTROS do dropdown de motivos de aprovação
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
        [Route("api/Motivo_Aprovacao_RestricaoApi/GetComboMotivos")]
        public async Task<ObjCombo<string>> GetComboMotivos(RequestObjCombo<string> request)
        {
            ObjCombo<string> retorno = new ObjCombo<string>
            {
                results = new List<IObjComboResult<string>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_MOTIVO_APROVACAO_RESTRICAO A ");


                    List<String> condicoes = new List<String>();

                    if (request.id != null)
                    {
                        condicoes.Add($"(A.ID = {request.id})");
                    }
                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"((UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 255)}%' ))");
                    }
                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes)} ) ");
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

                        sql.AppendLine("SELECT A.ID, A.DESCRICAO, A.TIPO_RESTRICAO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" ORDER BY A.DESCRICAO ");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            var opcaoOutros = new ObjComboResult<string>() { Id = "0", Text = String.Format("{0}", "Outro") };

                            while (await reader.ReadAsync())
                            {
                                var lista = new ObjComboResult<string>
                                {
                                    Id = Convert.ToString(reader["ID"]),
                                    Text = String.Format("{0}", reader["DESCRICAO"]),

                                };
                                retorno.results.Add(lista);

                            }
                            if (request.page == 1)
                            {
                                retorno.results.Add(opcaoOutros); // opção OUTROS do dropdown de motivos de aprovação
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
