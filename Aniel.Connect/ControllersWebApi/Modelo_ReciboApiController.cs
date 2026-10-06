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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class Modelo_ReciboApiController : BaseApiController
    {


        [HttpPost()]
        [Route("api/Modelo_Reciboapi/GetAll")]
        public async Task<DataTableResponse<Modelo_ReciboListaModel>> GetAll(Modelo_Recibo_DataTableRequest request)
        {

            List<Modelo_ReciboListaModel> retorno = new List<Modelo_ReciboListaModel>();
            IOrderedEnumerable<Modelo_ReciboListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_MODELO_RECIBO"); //WHERE STATUS = 1  

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

                    sql.AppendLine("SELECT A.ID,A.DESCRICAO,A.NOME_ARQUIVO,A.EXTENSAO_ARQUIVO,A.STATUS ");
                    sql.AppendLine("FROM TB_MODELO_RECIBO A");


                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine("WHERE ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Modelo_ReciboListaModel
                            {

                                Id = Convert.ToInt64(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Nome_Arquivo = Convert.ToString(reader["NOME_ARQUIVO"]),
                                Extensao_Arquivo = Convert.ToString(reader["EXTENSAO_ARQUIVO"]),
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
                                listaOrdenada = retorno.OrderBy(p => p.Descricao);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Descricao);
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
            return new DataTableResponse<Modelo_ReciboListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        [HttpPost()]
        public async Task<GetApiRetorno<IEnumerable<Modelo_ReciboListaModel>>> GetList()
        {
            GetApiRetorno<IEnumerable<Modelo_ReciboListaModel>> retorno = new GetApiRetorno<IEnumerable<Modelo_ReciboListaModel>>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Modelo_Recibo>(session))
            {
                try
                {
                    var lista = await repository.select(p => p.Status == Status_Default.Ativo);

                    retorno.Objeto = lista.Select(p => new Modelo_ReciboListaModel
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

        private Modelo_ReciboCadastroModel GetViewObj(Modelo_Recibo obj)
        {
            return new Modelo_ReciboCadastroModel
            {
                Id = obj.Id,
                Descricao = obj.Descricao,
                Nome_Arquivo = obj.Nome_Arquivo,
                Extensao_Arquivo = obj.Extensao_Arquivo,


                Status = obj.Status.Value == Status_Default.Ativo ? true : false

            };

        }

        private void BindViewToObj(ISession session, ref Modelo_Recibo objBanco, Modelo_ReciboCadastroModel view)
        {
            objBanco.Id = view.Id;
            objBanco.Descricao = view.Descricao;
            if (view.ArquivoAlterado)
            {
                objBanco.Nome_Arquivo = view.Nome_Arquivo;
                objBanco.Extensao_Arquivo = view.Extensao_Arquivo;
            }


        }

        [HttpPost()]
        [Route("api/Modelo_Reciboapi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_MODELO_RECIBO SET STATUS = {value.status} WHERE ID IN {_ids}");

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
        [Route("api/Modelo_Reciboapi/excluirlista")]
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

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_MODELO_RECIBO WHERE ID IN {_ids}");

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
        [Route("api/Modelo_Reciboapi/FindById")]
        public async Task<GetApiRetorno<Modelo_ReciboCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<Modelo_ReciboCadastroModel> retorno = new GetApiRetorno<Modelo_ReciboCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Modelo_Recibo>(session))
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
        [Route("api/Modelo_Reciboapi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody]ParamWebApi<Modelo_ReciboCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Modelo_Recibo modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Modelo_Recibo>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new Modelo_Recibo
                            {
                                UsuarioCriacao = value.Usuario,
                                Status = Status_Default.Ativo
                            };

                        }

                        else
                        {
                            modelo.UsuarioAlteracao = value.Usuario;
                            modelo.Status = Convert.ToBoolean(value.Objeto.Status) ? Status_Default.Ativo : Status_Default.Inativo;
                        }

                        BindViewToObj(session, ref modelo, value.Objeto);

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

        [Route("api/Modelo_Reciboapi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody]ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Modelo_Recibo>(session))
                    {

                        Modelo_Recibo modelo = null;
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
        [Route("api/Modelo_Reciboapi/ExcluirRecibo")]
        public async Task<DeleteApiRetorno<Int64>> ExcluirRecibo(Int64 Id)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Modelo_Recibo>(session))
                    {
                        Modelo_Recibo modelo = (await repository.select(p => p.Id == Id)).FirstOrDefault();

                        try
                        {
                            String caminhoArquivo = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Modelo_Recibo\{modelo.Id}.{modelo.Extensao_Arquivo}";

                            if (File.Exists(caminhoArquivo))
                                File.Delete(caminhoArquivo);
                        }
                        catch { }

                        await repository.Remove(modelo);

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
        [Route("api/Modelo_ReciboApi/GetCombo")]
        public async Task<ObjCombo<String>> GetCombo(RequestObjComboPai<String, Int64?> request)
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_MODELO_RECIBO A ");

                    List<String> condicoes = new List<String>();

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"A.ID = '{request.id}'");
                    }


                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 40)}%')");
                    }

                    condicoes.Add("A.STATUS = 1");

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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.DESCRICAO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["ID"]),
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


    }
}
