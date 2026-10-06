using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using Resources;
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
    public class BotApiController : BaseApiController
    {
        [HttpPost()]
        [Route("api/BotApi/GetAll")]
        public async Task<DataTableResponse<Bot_ListModel>> GetAll(Bot_DataTableRequest request)
        {

            List<Bot_ListModel> retorno = new List<Bot_ListModel>();
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_BOT ");

                    using var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null);
                    if (await reader.ReadAsync())
                    {
                        recordsTotal = Convert.ToInt32(reader["CO"]);
                    }
                }

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.ID, A.AVATAR, A.NOME,A.STATUS, A.HABILITADO ");
                    sql.AppendLine(" FROM TB_BOT A");


                    //Filtrando
                    List<String> filtros = new List<string>();
                    if (!String.IsNullOrEmpty(request.Status))
                    {
                        filtros.Add("A.STATUS = @STATUS");
                        parametros.Add("@STATUS", request.Status);
                    }

                    if (!String.IsNullOrEmpty(request.Habilitado))
                    {
                        filtros.Add("A.HABILITADO = @HABILITADO");
                        parametros.Add("@HABILITADO", request.Habilitado);
                    }

                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        filtros.Add("UPPER(A.NOME) LIKE UPPER(@SEARCH)");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }

                    if (filtros.Count > 0)
                    {
                        sql.AppendLine(" WHERE " + String.Join(" AND ", filtros));
                    }

                    ////Ordenacao
                    if (request.order != null && request.order.Any())
                    {
                        int sortColumnIndex = request.order[0].column;
                        var coluna = request.columns[sortColumnIndex].data;
                        string sortDirection = request.order[0].dir;
                        switch (coluna)
                        {
                            case "nome":
                                {
                                    if (sortDirection == "asc")
                                        sql.AppendLine(" ORDER BY A.NOME ");

                                    else
                                        sql.AppendLine(" ORDER BY A.NOME DESC ");
                                    break;
                                }
                                
                        }
                    }


                    //Busca com paginacao
                    using var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start);
                    while (await reader.ReadAsync())
                    {
                        retorno.Add(new Bot_ListModel() 
                        {
                            Id = Convert.ToInt64(reader["ID"]),
                            Avatar = Convert.ToString(reader["AVATAR"]),
                            Nome = Convert.ToString(reader["NOME"]),
                            Status = Convert.ToString(reader["STATUS"]) == "1" ? RsCadastros.ATIVO : RsCadastros.INATIVO,
                            Habilitado = Convert.ToString(reader["STATUS"]) == "1" ? RsCadastros.SIM : RsCadastros.NAO,
                        });
                    }

                }
            }

            
            //Page e retornando DataTable
            return new DataTableResponse<Bot_ListModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };

        }

        [HttpPost()]
        [Route("api/BotApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<Bot_CadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Bot modelo = null;

                using ISession session = HibernateUtil.GetSession();
                using (var repository = new Conexao<Bot>(session))
                {
                    if (value.Objeto.Id != 0)
                        modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();


                    if (modelo == null)
                    {
                        modelo = new Bot
                        {
                            UsuarioCriacao = value.Usuario,
                            Status = Status_Default.Ativo,
                            Habilitado = Habilitado_Bot.Ativo
                        };
                    }
                    else
                    {
                        modelo.UsuarioAlteracao = value.Usuario;
                        modelo.DataHoraAlteracao = DateTime.Now;
                    }

                    BindViewToObj(session, ref modelo, value.Objeto);

                    //seta o nome como Bot
                    if(modelo.Nome == null)
                    {
                        modelo.Nome = "Bot";
                    }

                    //envia a imagem
                    if (modelo.Avatar == null)
                    {
                        modelo.Avatar = $@"{CONSTANTES.URLPortal}/Arquivos/Bot/Default.jpg";
                    }

                    await repository.SaveOrUpdate(modelo);

                }
                retorno.IdObjeto = modelo.Id;
                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = value.Objeto.Id;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [HttpGet()]
        [Route("api/BotApi/FindById")]
        public async Task<GetApiRetorno<Bot_CadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<Bot_CadastroModel> retorno = new GetApiRetorno<Bot_CadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<Bot>(session))
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
        [Route("api/Botapi/AlterarStatus")]
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

                        StringBuilder sql = new StringBuilder($"UPDATE TB_BOT SET STATUS = {value.status} WHERE ID IN {_ids}");

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

        private void BindViewToObj(ISession session, ref Bot objBanco, Bot_CadastroModel view)
        {
            objBanco.Id = view.Id;
            objBanco.Avatar = view.Avatar;
            objBanco.Nome = view.Nome;

            if (objBanco.Id != 0)
            {
                objBanco.Status = view.Status == true ? Status_Default.Ativo : Status_Default.Inativo;
                objBanco.Habilitado = view.Habilitado == true ? Habilitado_Bot.Ativo : Habilitado_Bot.Inativo;
            }
        }

        private Bot_CadastroModel GetViewObj(Bot obj)
        {
            return new Bot_CadastroModel
            {
                Id = obj.Id,
                Avatar = obj.Avatar,
                Nome = obj.Nome,
                Status = obj.Status == Status_Default.Ativo,
                Habilitado = obj.Habilitado == Habilitado_Bot.Ativo
            };
        }

        [HttpPost()]
        [Route("api/Botapi/GetCombo")]
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_BOT A WHERE ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        condicoes.Add($"A.STATUS = 1");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.NOME) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 50)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" ( {String.Join(" AND ", condicoes) } ) ");
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
    }
}