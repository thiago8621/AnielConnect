using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using NHibernate.Criterion;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.Identity;
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
    public class Atendimento_AcaoApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/Atendimento_Acaoapi/GetAll")]
        [AcceptVerbs("Get", "Post")]
        public async Task<DataTableResponse<Atendimento_AcaoListaModel>> GetAll(Atendimento_Acao_DataTableRequest request)
        {
            List<Atendimento_AcaoListaModel> retorno = new List<Atendimento_AcaoListaModel>();
            IOrderedEnumerable<Atendimento_AcaoListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {
                StringBuilder sqlFrom = new StringBuilder(" FROM TB_ATENDIMENTO_ACAO A ");

                sqlFrom.AppendLine("LEFT JOIN TB_CLIENTE B ");
                sqlFrom.AppendLine("ON (A.ID_CLIENTE = B.ID) ");

                sqlFrom.AppendLine("WHERE A.STATUS = 1 ");


                Dictionary<String, Object> parametros = new Dictionary<string, object>();

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

                    sql.AppendLine("SELECT A.ID, A.DESCRICAO, A.STATUS_ATENDIMENTO ");
                    sql.AppendLine(sqlFrom.ToString());

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        //sql.AppendLine("AND ((UPPER(A.NUMERO) LIKE UPPER(@SEARCH))) ");
                        //parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Atendimento_AcaoListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),                               
                               
                                //Ultima_Acao = Convert.ToString(reader["ULTIMA_ACAO"].ToString()),
                                                                                               
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
                    case 0:     // ID
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Id);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Id);
                            break;
                        }

                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<Atendimento_AcaoListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        private Atendimento_AcaoCadastroModel GetViewObj(Atendimento_Acao obj)
        {
            return new Atendimento_AcaoCadastroModel
            {
                Id = obj.Id,
               
            };
        }

        private void BindViewToObj(ISession session, ref Atendimento_Acao objBanco, Atendimento_AcaoCadastroModel view)
        {
            objBanco.Id = view.Id;

            //objBanco.Plano = view.Id_Origem_Atendimento.HasValue ? new BuscaGenericaRepository<plano>(session, null).FindById(view.Id_Origem_Atendimento.Value).Result : null;

            //objBanco.Origem_Atendimento = view.Id_Origem_Atendimento.HasValue ? new BuscaGenericaRepository<Origem_Atendimento>(session, null).FindById(view.Id_Origem_Atendimento.Value).Result : null;
            
            //objBanco.Ultima_Acao = view.Ultima_Acao;
                                   
        }

        [HttpPost()]
        [Route("api/Atendimento_Acaoapi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_ATENDIMENTO_ACAO SET STATUS = {value.status} WHERE ID IN {_ids}");

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
        [Route("api/Atendimento_Acaoapi/excluirlista")]
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

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_ATENDIMENTO_ACAO WHERE ID IN {_ids}");

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
        [Route("api/Atendimento_AcaoApi/FindById")]
        public async Task<GetApiRetorno<Atendimento_AcaoCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<Atendimento_AcaoCadastroModel> retorno = new GetApiRetorno<Atendimento_AcaoCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoAtendimentoAcao = new Conexao<Atendimento_Acao>(session))
            {
                try
                {
                    retorno.Objeto = GetViewObj(await repoAtendimentoAcao.selectFirst(p => p.Id == Id));
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
        [Route("api/Atendimento_AcaoApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody]ParamWebApi<Atendimento_AcaoCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Atendimento_Acao modelo = null;

                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Atendimento_Acao>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new Atendimento_Acao
                            {
                                //Status = Status_Default.Ativo
                                UsuarioCriacao = value.Usuario
                            };
                            
                        }
                        else
                        {
                            //modelo.Status = value.Objeto.Status? Status_Default.Ativo : Status_Default.Inativo;
                            modelo.UsuarioAlteracao = value.Usuario;
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

        [Route("api/Atendimento_AcaoApi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody]ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Atendimento_Acao>(session))
                    {

                        Atendimento_Acao modelo = null;
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
