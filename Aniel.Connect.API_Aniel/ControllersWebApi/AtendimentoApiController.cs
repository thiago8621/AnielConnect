using Aniel.Connect.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using NHibernate;
using Sinapse.DAO.repository;
using System.Text;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.API_Aniel.Models;
using Tipo_Atendimento = Aniel.Connect.Model.nhibernate.Tipo_Atendimento;
using Categoria_Atendimento = Aniel.Connect.Model.nhibernate.Categoria_Atendimento;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class AtendimentoApiController : BaseApiController
    {
        /// <summary>
        /// Retorna a Listagem de Tipos de Atendimento.
        /// </summary>
        /// <param name="request">Objeto de request</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/AtendimentoApi/GetTipo")]
        public async Task<ApiResponseObj<List<Tipo_Atendimento>>> GetTipo(TipoAtendimento_ApiRequest request)
        {
            List<Tipo_Atendimento> retorno = new List<Tipo_Atendimento>();


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

                    #region //Select

                    using (var com = session.Connection.CreateCommand())
                    {

                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID,A.ID_SCRIPT, A.ID_SCRIPT_SUPORTETECNICO, A.ID_CATEGORIA_ATENDIMENTO, A.NOME, ");
                        sql.AppendLine(" A.DESC_TIPO_SERVICO, A.USUARIOCRIACAO, A.USUARIOALTERACAO, A.DATAHORACRIACAO, A.DATAHORAALTERACAO,");
                        sql.AppendLine("A.TOKEN, A.STATUS, A.CRIAR_OS_ENCERRAR, A.EXIBIR_APP_OLA, A.CODCT, A.PRIORIDADE_PADRAO");
                        sql.AppendLine("FROM TB_TIPO_ATENDIMENTO A");
                        if (request.Status == Status_Default.Ativo)
                        {
                            sql.AppendLine("WHERE A.STATUS <>0");
                        }

                        #endregion

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new Tipo_Atendimento
                                {
                                    #region Retorno_Add
                                    Id = Convert.ToInt64(reader["ID"]),
                                    UsuarioCriacao = Convert.ToString(reader["USUARIOCRIACAO"]),
                                    UsuarioAlteracao = Convert.ToString(reader["USUARIOALTERACAO"]),
                                    Token = Convert.ToString(reader["TOKEN"]),
                                    DataHoraAlteracao = Convert.ToDateTime(reader["DATAHORAALTERACAO"]),
                                    Nome = Convert.ToString(reader["NOME"]),


                                    #endregion
                                });
                            }
                        }

                    }

                    return new ApiResponseObj<List<Tipo_Atendimento>>()
                    {

                        data = retorno,
                        error = ""
                    };
                }
                catch (Exception e)
                {
                    return new ApiResponseObj<List<Tipo_Atendimento>>()
                    {

                        data = retorno,
                        error = e.Message
                    };
                }
            }
        }

        /// <summary>
        /// Retorna a Listagem de Categorias de Atendimento.
        /// </summary>
        /// <param name="request">Objeto de request</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/AtendimentoApi/GetCategoria")]
        public async Task<ApiResponseObj<List<Categoria_Atendimento>>> GetCategoria(CategoriaAtendimento_ApiRequest request)
        {
            List<Categoria_Atendimento> retorno = new List<Categoria_Atendimento>();


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

                    #region //Select

                    using (var com = session.Connection.CreateCommand())
                    {

                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.ID, A.USUARIOCRIACAO, A.DATAHORACRIACAO, A.USUARIOALTERACAO, A.DATAHORAALTERACAO, ");
                        sql.AppendLine("A.TOKEN, A.CODIGO, A.DESCRICAO, A.STATUS");
                        sql.AppendLine("FROM TB_CATEGORIA_ATENDIMENTO A");
                        if(request.Status == Status_Default.Ativo)
                            {
                            sql.AppendLine("WHERE A.STATUS <>0");
                        }

                        #endregion

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new Categoria_Atendimento
                                {
                                    #region Retorno_Add

                                    Id = Convert.ToInt64(reader["ID"]),
                                    Codigo = Convert.ToString(reader["CODIGO"]),
                                    DataHoraCriacao = Convert.ToDateTime(reader["DATAHORACRIACAO"]),
                                    DataHoraAlteracao = Convert.ToDateTime(reader["DATAHORAALTERACAO"]),
                                    Descricao = Convert.ToString(reader["DESCRICAO"]),
                                    Status = (Status_Default)reader["STATUS"],
                                    Token = Convert.ToString(reader["TOKEN"]),
                                    UsuarioAlteracao = Convert.ToString(reader["USUARIOALTERACAO"]),
                                    UsuarioCriacao = Convert.ToString(reader["USUARIOCRIACAO"])

                                    #endregion
                                });
                            }
                        }

                    }

                    return new ApiResponseObj<List<Categoria_Atendimento>>()
                    {

                        data = retorno,
                        error = ""
                    };
                }
                catch (Exception e)
                {
                    return new ApiResponseObj<List<Categoria_Atendimento>>()
                    {

                        data = retorno,
                        error = e.Message
                    };
                }
            }
        }
    }
}