using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.T5.Model.nhibernate;
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

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class Categoria_Cliente_ConnectApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/Categoria_Cliente_ConnectApi/GetList")]
        public async Task<GetApiRetorno<IEnumerable<Categoria_ClienteListaModel>>> GetList()
        {
            GetApiRetorno<IEnumerable<Categoria_ClienteListaModel>> retorno = new GetApiRetorno<IEnumerable<Categoria_ClienteListaModel>>();

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    var lista = await new Conexao<Categoria_ClienteAniel>(session).queryover().ListAsync();

                    retorno.Objeto = lista.Select(p => new Categoria_ClienteListaModel
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

        [HttpPost()]
        [Route("api/Categoria_Cliente_ConnectApi/GetCombo")]
        public async Task<ObjCombo<Int64>> GetCombo(RequestObjCombo<Int64?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_CATEGORIA_CLIENTE_CONNECT A WHERE ");

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

        [HttpPost()]
        [Route("api/Categoria_Cliente_ConnectApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<Categoria_ClienteCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Categoria_ClienteAniel modelo = null;

                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    using (var repository = new Conexao<Categoria_ClienteAniel>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new Categoria_ClienteAniel
                            {
                                Status = Status_Default.Ativo,
                                UsuarioCriacao = value.Usuario
                            };
                        }
                        else
                        {
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

        private void BindViewToObj(ISession session, ref Categoria_ClienteAniel objBanco, Categoria_ClienteCadastroModel view)
        {
            objBanco.Descricao = view.Descricao;

            if (objBanco.Id != 0)
            {
                objBanco.Id = view.Id;
                objBanco.Status = view.Status ? Status_Default.Ativo : Status_Default.Inativo;
            }
            else
            {
                objBanco.Id = view.Id_Categoria_Aniel;
            }

            if (view.Tipo_Prioridade == Tipo_Prioridade.Baixa)
                objBanco.Tipo_Prioridade = Tipo_PrioridadeT5.Baixa;
            else if (view.Tipo_Prioridade == Tipo_Prioridade.Media)
                objBanco.Tipo_Prioridade = Tipo_PrioridadeT5.Media;
            else if (view.Tipo_Prioridade == Tipo_Prioridade.Alta)
                objBanco.Tipo_Prioridade = Tipo_PrioridadeT5.Alta;
            else
                objBanco.Tipo_Prioridade = null;


        }

    }
}
