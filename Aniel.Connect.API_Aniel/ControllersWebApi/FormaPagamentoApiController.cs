using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class FormaPagamentoApiController : BaseApiController
    {
        /// <summary>
        /// Retorna a Listagem de Formas de Pagamento
        /// </summary>
        /// <param name="request">Objeto de request</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/FormaPagamentoApi/GetAll")]
        public async Task<ApiResponseObj<List<FormaPamentoApiList>>> GetAll(FormaPagamentoApiRequest request)
        {
            List<FormaPamentoApiList> retorno = new List<FormaPamentoApiList>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    using (var api = new UsuarioApiController())
                    {
                        if (!(await api.GetUsuarioValido(request.settings)).Ok)
                        {
                            throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                        }
                    }

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        List<string> condicoes = new List<string>();
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine(" SELECT ID, DESCRICAO, TIPO_FORMA_PAGAMENTO ");
                        sql.AppendLine(" FROM TB_FORMA_PAGAMENTO A ");

                        if (request.Alterado)
                            condicoes.Add("  A.ALTERADO = 1 ");
                        if (request.SoAtivos)
                            condicoes.Add("  A.STATUS = 1 ");
                        if (condicoes.Count > 0)
                        {
                            sql.AppendLine(" WHERE ")
                               .AppendLine(String.Join("AND", condicoes));
                        }
                        if (!string.IsNullOrEmpty(request.settings.search))
                        {
                            parametros.Add("DESCRICAO", request.settings.search);
                            sql.AppendLine($" AND A.DESCRICAO LIKE '%@DESCRICAO%' ");
                        }

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.settings.start, request.settings.length))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new FormaPamentoApiList
                                {
                                    Id = Convert.ToInt64(reader["ID"]),
                                    Descricao = Convert.ToString(reader["DESCRICAO"]),
                                    Tipo_Forma_Pagamento = HtmlEnumExtensions.GetEnumDescription<Tipo_Forma_Pagamento>(Convert.ToString(reader["TIPO_FORMA_PAGAMENTO"])),
                                    Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["TIPO_FORMA_PAGAMENTO"]))
                                });
                            }
                        }

                    }

                    return new ApiResponseObj<List<FormaPamentoApiList>>() { ok = true, data = retorno };
                }
                catch (Exception e)
                {
                    return new ApiResponseObj<List<FormaPamentoApiList>>() { ok = false, error = e.Message };
                }
            }
        }

        /// <summary>
        /// Retorna a Listagem de Formas de Pagamento Alterados e Atualiza o Campo ALTERADO para 0
        /// </summary>
        /// <param name="request">Objeto de request</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/FormaPagamentoApi/GetIntegracao")]
        public async Task<ApiResponseObj<List<long>>> GetIntegracao(ApiRequest request)
        {
            List<Forma_Pagamento> listaBanco = new List<Forma_Pagamento>();
            List<long> retorno = new List<long>();

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    using (var api = new UsuarioApiController())
                    {
                        if (!(await api.GetUsuarioValido(request.settings)).Ok)
                        {
                            throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                        }
                    }

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    using (var formas = new Conexao<Forma_Pagamento>(session))
                    {

                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine(" SELECT ID ");
                        sql.AppendLine(" FROM TB_FORMA_PAGAMENTO A ");
                        sql.AppendLine(" WHERE A.ALTERADO = 1 ");

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.settings.start, request.settings.length))
                        {
                            while (await reader.ReadAsync())
                            {
                                var forma = formas.select(x => x.Id == Convert.ToInt64(reader["ID"])).Result.FirstOrDefault();
                                forma.Alterado = false;

                                listaBanco.Add(forma);

                                retorno.Add(Convert.ToInt64(reader["ID"]));
                            }
                        }

                        await formas.SaveOrUpdate(listaBanco);

                    }

                    return new ApiResponseObj<List<long>>() { ok = true, data = retorno };
                }
                catch (Exception e)
                {
                    return new ApiResponseObj<List<long>>() { ok = false, error = e.Message };
                }
            }
        }
    }        
    public class FormaPagamentoApiRequest : ApiRequest
    {
        public bool Alterado { get; set; }
        public bool SoAtivos { get; set; }
    }

}