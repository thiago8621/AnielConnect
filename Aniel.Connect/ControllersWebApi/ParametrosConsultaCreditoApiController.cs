using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Microsoft.AspNet.Identity;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    public class ParametrosConsultaCreditoApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/ParametrosConsultaCreditoapi/GetAll")]
        public async Task<DataTableResponse<CriteriosListaModel>> GetAll(CriteriosParamDataTableRequest request)
        {
            List<CriteriosListaModel> retorno = new List<CriteriosListaModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var com = session.Connection.CreateCommand())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();
                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT SUM(A.PONTOS) AS SOMA_PONTOS, SUM(A.VALOR) AS SOMA_VALOR, A.CRITERIO, A.ELIMINATORIO ");
                sql.AppendLine(" FROM TB_CRITERIO_PARAM_CREDITO A ");
                sql.AppendLine(" GROUP BY A.CRITERIO, A.ELIMINATORIO ");

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        retorno.Add(new CriteriosListaModel
                        {
                            Criterio = HtmlEnumExtensions.GetEnumDescription((CriteriosParametrosCredito)Convert.ToInt32(reader["CRITERIO"])),
                            Operador = HtmlEnumExtensions.GetEnumDescription((EnumOperadores)Convert.ToInt32(reader["OPERADOR"])),
                            Valor = reader["SOMA_VALOR"] != DBNull.Value ? Convert.ToInt64(reader["SOMA_VALOR"]) : 0,
                            Pontos = reader["SOMA_PONTOS"] != DBNull.Value ? Convert.ToInt64(reader["SOMA_PONTOS"]) : 0,
                            Eliminatorio = reader["ELIMINATORIO"] != DBNull.Value ? Convert.ToBoolean(reader["ELIMINATORIO"]) : false,

                        });
                    }
                }
            }
            return new DataTableResponse<CriteriosListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                //  recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };

        }


        [HttpGet()]
        [Route("api/ParametrosConsultaCreditoapi/FindById")]
        public async Task<GetApiRetorno<ParametrosConsultaCreditoCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<ParametrosConsultaCreditoCadastroModel> retorno = new GetApiRetorno<ParametrosConsultaCreditoCadastroModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<ParametrosConsultaCredito>(session))
            using (var repocriterios = new Conexao<CriteriosParamCredito>(session))
            {
                try
                {

                    ParametrosConsultaCredito param = await repository.selectFirst(p => p.Id == Id);

                    if (param == null)
                    {
                        param = new ParametrosConsultaCredito();
                    }

                    retorno.Objeto = GetViewObj(param, session);

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

        private ParametrosConsultaCreditoCadastroModel GetViewObj(ParametrosConsultaCredito obj, ISession session)
        {
            using (var repocriterios = new Conexao<CriteriosParamCredito>(session))
            {
                var retorno = new ParametrosConsultaCreditoCadastroModel();

                retorno.Id = obj.Id;
                retorno.PontosPrimeiraFaixaCriterio = obj.PontosPrimeiraFaixaCriterio;
                retorno.PontosSegundaFaixaCriterio = obj.PontosSegundaFaixaCriterio;
                retorno.PontosTerceiraFaixaCriterio = obj.PontosTerceiraFaixaCriterio;
                retorno.UtilizaFaixaCriterio1 = obj.UtilizaFaixaCriterio1 == true ? true : false;
                retorno.UtilizaFaixaCriterio2 = obj.UtilizaFaixaCriterio2 == true ? true : false;
                retorno.UtilizaFaixaCriterio3 = obj.UtilizaFaixaCriterio3 == true ? true : false;
                retorno.TipoAprovacaoFaixa1 = obj.TipoAprovacaoFaixa1;
                retorno.TipoAprovacaoFaixa2 = obj.TipoAprovacaoFaixa2;
                retorno.TipoAprovacaoFaixa3 = obj.TipoAprovacaoFaixa3;

                if (obj.Id != 0)
                {
                    var criterios = repocriterios.select(p => p.Id_ParametrosConsultaCredito == obj).Result;

                    if (criterios != null)
                    {
                        retorno.CriteriosLista = criterios.Select(o => new CriteriosListaModel
                        {
                            Id = o.Id,
                            Criterio = HtmlEnumExtensions.GetEnumDescription((CriteriosParametrosCredito)o.TipoCriterio),
                            TipoCriterio = o.TipoCriterio,
                            Operador = HtmlEnumExtensions.GetEnumDescription((EnumOperadores)o.TipoOperador),
                            TipoOperador = o.TipoOperador,
                            Pontos = o.Pontos,
                            Valor = o.Valor,
                            Eliminatorio = o.Eliminatorio
                        }).ToList();
                    }
                }

                return retorno;
            }
        }

        [HttpPost()]
        [Route("api/ParametrosConsultaCreditoapi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<ParametrosConsultaCreditoCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            using (ISession session = HibernateUtil.GetSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {

                    ParametrosConsultaCredito modelo = null;
                    using (var repository = new Conexao<ParametrosConsultaCredito>(session))
                    using (var repocriterios = new Conexao<CriteriosParamCredito>(session))
                    {
                        if (value.Objeto.Id != 0)
                            modelo = (await repository.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();


                        if (modelo == null)
                        {
                            modelo = new ParametrosConsultaCredito
                            {

                                UsuarioCriacao = value.Usuario
                            };

                        }
                        else
                        {
                            modelo.UsuarioAlteracao = value.Usuario;
                        }

                        List<CriteriosParamCredito> criterioslista = new List<CriteriosParamCredito>();

                        if (value.Objeto.CriteriosLista.Count > 0)
                        {
                            criterioslista = value.Objeto.CriteriosLista.Select(p => new CriteriosParamCredito
                            {
                                Id = p.Id,
                                TipoCriterio = p.TipoCriterio,
                                TipoOperador = p.TipoOperador,
                                Valor = p.Valor,
                                Pontos = p.Pontos,
                                UsuarioCriacao = User.Identity.GetUserId(),
                                DataHoraCriacao = DateTime.Now,
                                Id_ParametrosConsultaCredito = modelo,
                                Eliminatorio = p.Eliminatorio
                            }).ToList();
                        }

                        BindViewToObj(session, modelo, value.Objeto);

                        await repository.SaveOrUpdate(modelo);
                        await repocriterios.SaveOrUpdate(criterioslista);

                    }
                    retorno.IdObjeto = modelo.Id;
                    retorno.Ok = true;
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    retorno.IdObjeto = 0;
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                }

            }
            return retorno;
        }


        private void BindViewToObj(ISession session, ParametrosConsultaCredito objBanco, ParametrosConsultaCreditoCadastroModel view)
        {
            using (var repocriterios = new Conexao<CriteriosParamCredito>(session))
            {
                objBanco.PontosPrimeiraFaixaCriterio = view.PontosPrimeiraFaixaCriterio;
                objBanco.PontosSegundaFaixaCriterio = view.PontosSegundaFaixaCriterio;
                objBanco.PontosTerceiraFaixaCriterio = view.PontosTerceiraFaixaCriterio;
                objBanco.TipoAprovacaoFaixa1 = view.TipoAprovacaoFaixa1;
                objBanco.TipoAprovacaoFaixa2 = view.TipoAprovacaoFaixa2;
                objBanco.TipoAprovacaoFaixa3 = view.TipoAprovacaoFaixa3;
                objBanco.UtilizaFaixaCriterio1 = view.UtilizaFaixaCriterio1 == true ? true : false;
                objBanco.UtilizaFaixaCriterio2 = view.UtilizaFaixaCriterio2 == true ? true : false;
                objBanco.UtilizaFaixaCriterio3 = view.UtilizaFaixaCriterio3 == true ? true : false;
            }
        }



        [HttpGet()]
        [Route("api/ParametrosConsultaCreditoapi/ExcluirCriterios")]
        public async Task<DeleteApiRetorno<Int64>> ExcluirCriterios(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoCriterios = new Conexao<CriteriosParamCredito>(session))
                {

                    CriteriosParamCredito modelo = null;
                    if (value != 0)
                    {
                        modelo = (await repoCriterios.select(p => p.Id == value)).FirstOrDefault();

                        await repoCriterios.Remove(modelo);
                        retorno.IdObjeto = modelo.Id;
                        retorno.Ok = true;
                    }
                    else
                    {
                        throw new Exception(Resources.RsBase.NAOENCONTRADO);
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