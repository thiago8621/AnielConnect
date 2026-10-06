using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class Configuracao_Envio_MensagensApiController : BaseApiController
    {

        [HttpPost()]
        [AcceptVerbs("Get", "Post")]
        [Route("api/Configuracao_Envio_Mensagensapi/GetAll")]
        public async Task<DataTableResponse<Configuracao_Envio_MensagensListModel>> GetAll(Configuracao_Envio_Mensagens_DataTableRequest request)
        {
            List<Configuracao_Envio_MensagensListModel> retorno = new List<Configuracao_Envio_MensagensListModel>();
            IOrderedEnumerable<Configuracao_Envio_MensagensListModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {
                StringBuilder sqlFrom = new StringBuilder("* from TB_CONFIGURACAO_ENVIO_MSG A");

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_CONFIGURACAO_ENVIO_MSG");


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

                    sql.AppendLine("SELECT A.ID, A.DESCRICAO,A.STATUS ");
                    sql.AppendLine("FROM TB_CONFIGURACAO_ENVIO_MSG A");                   
                    sql.AppendLine("WHERE A.ID>0 ");

                    //Filtrando
                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine(" AND ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                        //sql.AppendLine(" AND ((UPPER(A.DESCRICAO) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    } 


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            var obj = new Configuracao_Envio_MensagensListModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),                               
                                Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"])),
                            };

                            retorno.Add(obj);
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

                    case 1:     // DESCIRCAO
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Descricao);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Descricao);
                            break;
                        }

                   
                    case 2:     // Status
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Status);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Status);
                            break;
                        }
                        
                }
            }            //Page e retornando DataTable
            return new DataTableResponse<Configuracao_Envio_MensagensListModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        [HttpPost()]
        [Route("api/Configuracao_Envio_Mensagensapi/GetCombo")]
        public async Task<ObjCombo<Int64>> GetCombo(RequestObjCombo<Int64?> request)
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_CONFIGURACAO_ENVIO_MSG A WHERE ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS = 'Ativo'");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" ( {String.Join(" AND ", condicoes)} ) ");
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

        [HttpGet]
        [Route("api/Configuracao_Envio_MensagensApi/FindById")]
        public async Task<GetApiRetorno<Configuracao_Envio_MensagensCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<Configuracao_Envio_MensagensCadastroModel> retorno = new GetApiRetorno<Configuracao_Envio_MensagensCadastroModel>();


            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Configuracao_Envio_Mensagens>(session))
            {
                try
                    {
                         retorno.Objeto = GetViewObj(await repository.selectFirst(p => p.Id == Id));
                         retorno.Ok = true; 
                    }
                catch(Exception ex)
                                   {
                                      retorno.Mensagem = ex.Message;
                                      if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                                    }

                return retorno;
            }
        }

        [HttpPost]
        [Route("api/Configuracao_Envio_MensagensApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<Configuracao_Envio_MensagensCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Configuracao_Envio_Mensagens modelo = null;
                using (ISession session = HibernateUtil.GetSession())
                { 
                    using (var repository = new Conexao<Configuracao_Envio_Mensagens>(session))
                    {
                         if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();


                    if (modelo == null)
                    {
                        modelo = new Configuracao_Envio_Mensagens
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

                        await BindViewToObj(session, modelo, value.Objeto);

                        await repository.SaveOrUpdate(modelo);
                    }
                }
                retorno.IdObjeto = modelo.Id;
                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        #region Mobile
        [AllowAnonymous]
        [HttpGet]
        [Route("api/Configuracao_Envio_MensagensApi/GetConfigEnvioMensagens")]
        public async Task<Response<Configuracao_Envio_MensagensCadastroModel>> GetConfigEnvioMensagens()
        {
            Response<Configuracao_Envio_MensagensCadastroModel> retorno = new Response<Configuracao_Envio_MensagensCadastroModel>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repository = new Conexao<Configuracao_Envio_Mensagens>(session))
                {
                    var objBanco = await repository.selectFirst();

                    if(objBanco != null)
                    retorno.valor = GetViewObjMobile(objBanco);
                }
                retorno.result = true;
            }
            catch (Exception ex)
            {
                retorno.message = ex.Message;
                if (ex.InnerException != null) retorno.exception += ex.InnerException.Message;
            }

            return retorno;
        }

        private Configuracao_Envio_MensagensCadastroModel GetViewObjMobile(Configuracao_Envio_Mensagens obj)
        {
            return new Configuracao_Envio_MensagensCadastroModel
            {
                Id = obj.Id,
                Utiliza_Envio_Email = obj.Utiliza_Envio_Email,
                Utiliza_Envio_SMS = obj.Utiliza_Envio_SMS,
                Descricao = obj.Descricao,
                Status = obj.Status == Status_Default.Ativo ? true : false,
                Envia_Msg_Ao_Iniciar_Deslocamento = obj.Envia_Msg_Ao_Iniciar_Deslocamento,
                Mensagem_SMS = obj.Mensagem_Corpo_SMS,
                Assunto_Email = obj.Assunto_Email,
                Mensagem_Corpo_Email = obj.Mensagem_Corpo_Email,
            };
        }
        #endregion

        
        private async Task BindViewToObj(ISession session, Configuracao_Envio_Mensagens objBanco, Configuracao_Envio_MensagensCadastroModel view)
        {
            using (var repository = new Conexao<Configuracao_Envio_Mensagens>(session))
            {
                objBanco.Id = view.Id;
                objBanco.Utiliza_Envio_Email = view.Utiliza_Envio_Email;
                objBanco.Utiliza_Envio_SMS = view.Utiliza_Envio_SMS;
                objBanco.Descricao = view.Descricao;
                objBanco.Envia_Msg_Ao_Iniciar_Deslocamento = view.Envia_Msg_Ao_Iniciar_Deslocamento;
                objBanco.Mensagem_Corpo_SMS = view.Mensagem_SMS;
                objBanco.Assunto_Email = view.Assunto_Email;
                objBanco.Mensagem_Corpo_Email = view.Mensagem_Corpo_Email;
                objBanco.DataHoraAlteracao = DateTime.Now;
            }
        }

        
        private Configuracao_Envio_MensagensCadastroModel GetViewObj(Configuracao_Envio_Mensagens obj)
        {            
            return new Configuracao_Envio_MensagensCadastroModel {
                Id = obj.Id,
                Utiliza_Envio_Email = obj.Utiliza_Envio_Email,
                Utiliza_Envio_SMS = obj.Utiliza_Envio_SMS,
                Envia_Msg_Ao_Iniciar_Deslocamento = obj.Envia_Msg_Ao_Iniciar_Deslocamento,
                Mensagem_SMS = obj.Mensagem_Corpo_SMS,
                Assunto_Email = obj.Assunto_Email,
                Mensagem_Corpo_Email =obj.Mensagem_Corpo_Email,
                Status = obj.Status == Status_Default.Ativo ? true : false,
                Descricao = obj.Descricao,
                //    Mensagem_SMS = Convert.ToBase64String(Encoding.Default.GetBytes(obj.Mensagem_Corpo_SMS)),
                //    Assunto_Email = Convert.ToBase64String(Encoding.Default.GetBytes(obj.Assunto_Email)),
                //    Mensagem_Corpo_Email = Convert.ToBase64String(Encoding.Default.GetBytes(obj.Mensagem_Corpo_Email)),
            };
        }
    }
}