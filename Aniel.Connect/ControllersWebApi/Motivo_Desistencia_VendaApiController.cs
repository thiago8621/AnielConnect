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
    public class Motivo_Desistencia_VendaApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/Motivo_Desistencia_VendaApi/GetAll")]
        public async Task<DataTableResponse<Motivo_Desistencia_VendaListaModel>> GetAll(Motivo_Desistencia_Venda_DataTableRequest request)
        {

            List<Motivo_Desistencia_VendaListaModel> retorno = new List<Motivo_Desistencia_VendaListaModel>();
            IOrderedEnumerable<Motivo_Desistencia_VendaListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) CO FROM TB_MOTIVO_DESISTENCIA_VENDA A");

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

                    sql.AppendLine("SELECT A.STATUS, A.ID, A.DESCRICAO, A.MOTIVO, A.EXIBEAPPOLA");
                    sql.AppendLine(" FROM TB_MOTIVO_DESISTENCIA_VENDA A");

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine(" WHERE ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value.ToUpper()));
                    }

                    if (!String.IsNullOrEmpty(request.Status))
                    {
                        sql.AppendLine(" WHERE A.STATUS = @STATUS");
                        parametros.Add("@STATUS", request.Status);
                    }

                    if (!String.IsNullOrEmpty(request.Motivo))
                    {
                        if (request.Motivo == "0")
                            sql.AppendLine(" WHERE A.MOTIVO = 0; ");
                        else if (request.Motivo == "1")
                            sql.AppendLine("  WHERE A.MOTIVO = 1; ");
                    }
                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Motivo_Desistencia_VendaListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Motivo = HtmlEnumExtensions.GetEnumDescription<EnumMotivo_Desistencia_Venda>(Convert.ToString(reader["MOTIVO"])),
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
            return new DataTableResponse<Motivo_Desistencia_VendaListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };
        }

        private Motivo_Desistencia_VendaCadastroModel GetViewObj(Motivo_Desistencia_Venda obj)
        {
            return new Motivo_Desistencia_VendaCadastroModel
            {

                Id = obj.Id,
                Descricao = obj.Descricao,
                Status = Convert.ToBoolean(obj.Status == Status_Default.Ativo),
                Motivo = obj.Motivo,
                //Motivo = obj.Motivo == EnumMotivo_Desistencia_Venda.Desistencia,

                ExibeAppOla = obj.ExibeAppOla == EnumExibeAppOla.Não

            };

        }

        private void BindViewToObj(ISession session, Motivo_Desistencia_Venda objBanco, Motivo_Desistencia_VendaCadastroModel view)
        {

            objBanco.Descricao = view.Descricao;
        }

        [HttpPost()]
        [Route("api/Motivo_Desistencia_VendaApi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_MOTIVO_DESISTENCIA_VENDA SET STATUS = {value.status} WHERE ID IN {_ids}");

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
        [Route("api/Motivo_Desistencia_VendaApi/excluirlista")]
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

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_MOTIVO_DESISTENCIA_VENDA WHERE ID IN {_ids}");

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
        [Route("api/Motivo_Desistencia_VendaApi/GetList")]
        public async Task<GetApiRetorno<IEnumerable<Motivo_Desistencia_VendaListaModel>>> GetList()
        {
            GetApiRetorno<IEnumerable<Motivo_Desistencia_VendaListaModel>> retorno = new GetApiRetorno<IEnumerable<Motivo_Desistencia_VendaListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoTipo_Desconto = new Conexao<Motivo_Desistencia_Venda>(session))
            {
                try
                {
                    var lista = await repoTipo_Desconto.queryover().ListAsync();

                    retorno.Objeto = lista.Select(p => new Motivo_Desistencia_VendaListaModel
                    {
                        Id = p.Id,
                        Descricao = p.Descricao,
                        Motivo = p.Motivo.ToString(),
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
        [Route("api/Motivo_Desistencia_VendaApi/FindById")]
        public async Task<GetApiRetorno<Motivo_Desistencia_VendaCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<Motivo_Desistencia_VendaCadastroModel> retorno = new GetApiRetorno<Motivo_Desistencia_VendaCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Motivo_Desistencia_Venda>(session))
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
        [Route("api/Motivo_Desistencia_VendaApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<Motivo_Desistencia_VendaCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Motivo_Desistencia_Venda modelo = null;
                Motivo_Desistencia_Venda verificador = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Motivo_Desistencia_Venda>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        var Descrição_Motivo_Iguais = repository.select(p => p.Descricao == value.Objeto.Descricao && p.Motivo == (EnumMotivo_Desistencia_Venda)value.Objeto.Motivo).Result.Count();

                        if (Descrição_Motivo_Iguais >= 1)
                        {
                            retorno.IdObjeto = 0;
                            throw new Exception(" Descrição já existente com motivos iguais. Escolha outra descrição ou outro motivo. ");
                            //retorno.Mensagem = (" Descrição já existente com motivos iguais. Escolha outra descrição ou outro motivo. ");
                        }

                        if (modelo == null)
                        {
                            modelo = new Motivo_Desistencia_Venda
                            {
                                Status = Status_Default.Ativo,
                                UsuarioCriacao = value.Usuario,
                                Descricao = value.Objeto.Descricao,
                                Motivo = Convert.ToBoolean(value.Objeto.Motivo) ? EnumMotivo_Desistencia_Venda.Desistencia : EnumMotivo_Desistencia_Venda.Aprovacao

                            };
                        }
                        else
                        {
                            modelo.Status = Convert.ToBoolean(value.Objeto.Status) ? Status_Default.Ativo : Status_Default.Inativo;
                            modelo.ExibeAppOla = Convert.ToBoolean(value.Objeto.ExibeAppOla) ? EnumExibeAppOla.Sim : EnumExibeAppOla.Não;
                            modelo.Motivo = (EnumMotivo_Desistencia_Venda)value.Objeto.Motivo;
                            //modelo.Motivo = EnumMotivo_Desistencia_Venda.Aprovacao != null ? EnumMotivo_Desistencia_Venda.Aprovacao : EnumMotivo_Desistencia_Venda.Desistencia;
                            modelo.UsuarioAlteracao = value.Usuario;
                            modelo.DataHoraAlteracao = modelo.DataHoraAlteracao;
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

        [Route("api/Motivo_Desistencia_VendaApi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody] ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Motivo_Desistencia_Venda>(session))
                    {

                        Motivo_Desistencia_Venda modelo = null;
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
        [Route("api/Motivo_Desistencia_VendaApi/GetCombo")]
        public async Task<ObjCombo<string>> GetCombo(RequestObjCombo<string> request)
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

                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_MOTIVO_DESISTENCIA_VENDA A ");


                    List<String> condicoes = new List<String>();
                    condicoes.Add($"A.MOTIVO = 1");

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS = 1");
                    }
                    if (request.id != null)
                    {
                        condicoes.Add($"A.ID = {request.id}");
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

                        sql.AppendLine("SELECT A.ID, A.DESCRICAO, A.MOTIVO ");
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
                                retorno.results.Add(opcaoOutros);
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
        [Route("api/Motivo_Desistencia_VendaApi/GetComboAprovacao")]
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

                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_MOTIVO_DESISTENCIA_VENDA A ");


                    List<String> condicoes = new List<String>();
                    condicoes.Add($"A.MOTIVO = 0");

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

                        sql.AppendLine("SELECT A.ID, A.DESCRICAO, A.MOTIVO ");
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
        [Route("api/Motivo_Desistencia_VendaApi/GetComboMotivos")]
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

                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_MOTIVO_DESISTENCIA_VENDA A ");


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

                        sql.AppendLine("SELECT A.ID, A.DESCRICAO, A.MOTIVO ");
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
