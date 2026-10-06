using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.nhibernate;
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

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class ConfiguracaoTributariaApiController : BaseApiController
    {
        [HttpPost]
        [Route("api/ConfiguracaoTributariaApi/GetAll")]
        public async Task<ApiResponseObj<List<ConfiguracaoTributariaListModel>>> GetAll(ConfiguracaoTributariaApiRequest request)
        {
            using (var api = new UsuarioApiController())
            {
                if (!(await api.GetUsuarioValido(request.settings)).Ok)
                {
                    throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                }
            }

            List<ConfiguracaoTributariaListModel> retorno = new List<ConfiguracaoTributariaListModel>();
            Dictionary<string, object> parametros = new Dictionary<string, object>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.ID,A.CODIGO, A.DESCRICAO, A.STATUS, A.TIPO_TRIBUTACAO,A.ID_CFOP,A.ID_SPED, ");
                    sql.AppendLine(" A.SERIE_NFSE, A.RETENCAO_IMPOSTO, A.DEDUZIR_MATERIAL,A.DEDUZIR_VALOR_LIQUIDO, ");
                    sql.AppendLine(" A.INFORMACOES_NF, A.ID_CST_IPI, A.ID_CST_PIS,A.ID_CST_COFINS,A.ALIQUOTA_IPI, ");
                    sql.AppendLine(" A.ALIQUOTA_PIS,A.ALIQUOTA_COFINS,A.ALIQUOTA_IRRF,A.ALIQUOTA_CSLL,A.ALIQUOTA_INSS, ");
                    sql.AppendLine(" A.REDUCAO_BASE_CALCULO_IPI,A.REDUCAO_BASE_CALCULO_PIS,A.REDUCAO_BASE_CALCULO_COFINS ");

                    sql.AppendLine(" FROM TB_CONFIGURACAO_TRIBUTARIA  A ");

                    if(request.Alterado)
                        sql.AppendLine(" WHERE A.ALTERADO = 1 ");

                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.settings.length, request.settings.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            var obj = new ConfiguracaoTributariaListModel
                            {
                                Id = reader["DESCRICAO"] != DBNull.Value ? Convert.ToInt64(reader["ID"]) : 0,
                                Descricao = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : "",
                                Codigo = reader["CODIGO"] != DBNull.Value ? Convert.ToString(reader["CODIGO"]) : "",
                                CFOP = reader["ID_CFOP"] != DBNull.Value ? Convert.ToInt64(reader["ID_CFOP"]) : 0,
                                CST_COFINS = reader["ID_CST_COFINS"] != DBNull.Value ? Convert.ToInt64(reader["ID_CST_COFINS"]) : 0,
                                CST_IPI = reader["ID_CST_IPI"] != DBNull.Value ? Convert.ToInt64(reader["ID_CST_IPI"]) : 0,
                                CST_PIS = reader["ID_CST_PIS"] != DBNull.Value ? Convert.ToInt64(reader["ID_CST_PIS"]) : 0,
                                Deduzir_Material = reader["DEDUZIR_MATERIAL"] != DBNull.Value ? Convert.ToBoolean(reader["DEDUZIR_MATERIAL"]) : false,
                                Deduzir_Valor_Liquido = reader["DEDUZIR_VALOR_LIQUIDO"] != DBNull.Value ? Convert.ToBoolean(reader["DEDUZIR_VALOR_LIQUIDO"]) : false,
                                Retencao_Imposto = reader["RETENCAO_IMPOSTO"] != DBNull.Value ? Convert.ToBoolean(reader["RETENCAO_IMPOSTO"]) : false,
                                Serie_Nfse = reader["SERIE_NFSE"] != DBNull.Value ? Convert.ToString(reader["SERIE_NFSE"]) : "",
                                SPED = reader["ID_SPED"] != DBNull.Value ? Convert.ToInt64(reader["ID_SPED"]) : 0,
                                Status = reader["STATUS"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"])) : "",
                                Tipo_Tributacao = reader["TIPO_TRIBUTACAO"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Tipo_Tributacao>(Convert.ToString(reader["TIPO_TRIBUTACAO"])) : "",
                                Informacoes_NF = reader["INFORMACOES_NF"] != DBNull.Value ? Convert.ToString(reader["INFORMACOES_NF"]) : "",
                                Aliquota_COFINS = reader["ALIQUOTA_COFINS"] != DBNull.Value ? Convert.ToString(reader["ALIQUOTA_COFINS"]) : "",
                                Aliquota_CSLL = reader["ALIQUOTA_CSLL"] != DBNull.Value ? Convert.ToString(reader["ALIQUOTA_CSLL"]) : "",
                                Aliquota_INSS = reader["ALIQUOTA_INSS"] != DBNull.Value ? Convert.ToString(reader["ALIQUOTA_INSS"]) : "",
                                Aliquota_IPI = reader["ALIQUOTA_IPI"] != DBNull.Value ? Convert.ToString(reader["ALIQUOTA_IPI"]) : "",
                                Aliquota_IRRF = reader["ALIQUOTA_IRRF"] != DBNull.Value ? Convert.ToString(reader["ALIQUOTA_IRRF"]) : "",
                                Aliquota_PIS = reader["ALIQUOTA_PIS"] != DBNull.Value ? Convert.ToString(reader["ALIQUOTA_PIS"]) : "",
                                Reducao_Base_Calculo_COFINS = reader["REDUCAO_BASE_CALCULO_COFINS"] != DBNull.Value ? Convert.ToString(reader["REDUCAO_BASE_CALCULO_COFINS"]) : "",
                                Reducao_Base_Calculo_IPI = reader["REDUCAO_BASE_CALCULO_IPI"] != DBNull.Value ? Convert.ToString(reader["REDUCAO_BASE_CALCULO_IPI"]) : "",
                                Reducao_Base_Calculo_PIS = reader["REDUCAO_BASE_CALCULO_PIS"] != DBNull.Value ? Convert.ToString(reader["REDUCAO_BASE_CALCULO_PIS"]) : "",
                            };

                            var icms = await GetICMS(obj.Id, session);

                            if (icms != null)
                            {
                                obj.ICMS_Aliquota = icms.ICMS_Aliquota;
                                obj.ICMS_Atividade_Nf = icms.Atividade;
                                obj.ICMS_CFOP = icms.ICMS_CFOP;
                                obj.ICMS_CSOSN_ICMS = icms.ICMS_CSOSN_ICMS;
                                obj.ICMS_CST_ICMS = icms.ICMS_CST_ICMS;
                                obj.ICMS_Origem_Mercadoria = icms.ICMS_Origem_Mercadoria;
                                obj.ICMS_Reducao_Base_Calculo = icms.ICMS_Reducao_Base_Calculo;
                                obj.ICMS_Regime_Tributacao = icms.ICMS_Regime_Tributacao;
                                obj.ICMS_UF = icms.ICMS_UF;
                            }

                            var issqn = await GetISSQN(obj.Id, session);

                            if(issqn != null)
                            {
                                obj.ISSQN_Aliquota = issqn.ISSQN_Aliquota;
                                obj.ISSQN_Atividade_Nf = issqn.ISSQN_Atividade_Nf;
                                obj.ISSQN_CFOP = issqn.ISSQN_CFOP;
                                obj.ISSQN_Cod_Tributacao_Municipio = issqn.ISSQN_Cod_Tributacao_Municipio;
                                obj.ISSQN_Municipio = issqn.ISSQN_Municipio;
                                obj.ISSQN_Natureza_Tributacao = issqn.ISSQN_Natureza_Tributacao;
                                obj.ISSQN_UF = issqn.ISSQN_UF;
                                obj.TipoTributacaoISS = issqn.TipoTributacaoISS;
                            }

                            retorno.Add(obj);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return new ApiResponseObj<List<ConfiguracaoTributariaListModel>>
                {
                    error = e.Message + e.InnerException,
                    ok = false
                };
            }

            return new ApiResponseObj<List<ConfiguracaoTributariaListModel>>
            {
                data = retorno,
                ok = true
            };
        }

        /// <summary>
        /// Retorna a Listagem de Configurações Tributárias e Atualiza o Campo ALTERADO para 0
        /// </summary>
        /// <param name="request">Objeto de request</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ConfiguracaoTributariaApi/GetIntegracao")]
        public async Task<ApiResponseObj<List<object>>> GetIntegracao(ApiRequest request)
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

                List<object> retorno = new List<object>();

                using (ISession session = HibernateUtil.GetSession())
                using (var configs = new Conexao<Model.nhibernate.Configuracao_Tributaria>(session))
                {
                    var result = await configs.select(x => x.Alterado);

                    foreach (var configuracao in result)
                    {
                        configuracao.Alterado = false;

                        retorno.Add(new { IdConfiguracao = configuracao.Id });
                    }

                    await configs.SaveOrUpdate(result.ToList());
                }

                return new ApiResponseObj<List<object>> { ok = true, data = retorno };
            }
            catch (Exception e)
            {
                return new ApiResponseObj<List<object>> { ok = false, error = e.Message };
            }

        }

        async Task<ICMS> GetICMS(long idConfigTributaria, ISession session)
        {
            var retorno = new ICMS();

            try
            {
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();
                    Dictionary<string, object> parametros = new Dictionary<string, object>();
                    parametros.Add("ID", idConfigTributaria);

                    sql.AppendLine(" SELECT ALIQUOTA,ID_CFOP,ID_CSOSN_ICMS,ID_CST_ICMS,ID_ORIGEM_MERCADORIA,REDUCAO_BASE_CALCULO, ");
                    sql.AppendLine(" UF,REGIME_TRIBUTACAO, ID_ATIVIDADE ");
                    sql.AppendLine(" FROM TB_CONFIGURACAO_TRIBUTARIAICMS ");
                    sql.AppendLine(" WHERE ID_CONFIG_TRIBUTARIA = @ID ");

                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 0, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            retorno.ICMS_Aliquota = reader["ALIQUOTA"] != DBNull.Value ? Convert.ToString(reader["ALIQUOTA"]) : "";
                            retorno.ICMS_CFOP = reader["ID_CFOP"] != DBNull.Value ? Convert.ToInt64(reader["ID_CFOP"]) : 0;
                            retorno.ICMS_CSOSN_ICMS = reader["ID_CSOSN_ICMS"] != DBNull.Value ? Convert.ToInt64(reader["ID_CSOSN_ICMS"]) : 0;
                            retorno.ICMS_CST_ICMS = reader["ID_CST_ICMS"] != DBNull.Value ? Convert.ToInt64(reader["ID_CST_ICMS"]) : 0;
                            retorno.ICMS_Origem_Mercadoria = reader["ID_ORIGEM_MERCADORIA"] != DBNull.Value ? Convert.ToInt64(reader["ID_ORIGEM_MERCADORIA"]) : 0;
                            retorno.ICMS_Reducao_Base_Calculo = reader["REDUCAO_BASE_CALCULO"] != DBNull.Value ? Convert.ToString(reader["REDUCAO_BASE_CALCULO"]) : "";
                            retorno.ICMS_Regime_Tributacao = reader["REGIME_TRIBUTACAO"] != DBNull.Value ? Convert.ToString(reader["REGIME_TRIBUTACAO"]) : "";
                            retorno.ICMS_UF = reader["UF"] != DBNull.Value ? Convert.ToString(reader["UF"]) : "";
                            retorno.Atividade = reader["ID_ATIVIDADE"] != DBNull.Value ? Convert.ToInt64(reader["ID_ATIVIDADE"]) : 0;
                        }
                    }
                }

                return retorno;
            }
            catch
            {
                return null;
            }
        }

        async Task<ISSQN> GetISSQN(long idConfigTributaria, ISession session)
        {
            var retorno = new ISSQN();

            try
            {
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();
                    Dictionary<string, object> parametros = new Dictionary<string, object>();
                    parametros.Add("ID", idConfigTributaria);

                    sql.AppendLine(" SELECT ALIQUOTA,ID_ATIVIDADE,ID_CFOP,COD_TRIBUTACAO_MUNICIPIO,NATUREZA_TRIBUTACAO, ");
                    sql.AppendLine(" UF,TIPO_TRIBUTACAO_ISS, ID_CIDADE ");
                    sql.AppendLine(" FROM TB_CONFIGURACAO_TRIBUTARIAISSQN ");
                    sql.AppendLine(" WHERE ID_CONFIG_TRIBUTARIA = @ID ");

                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 0, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            retorno.ISSQN_Aliquota = reader["ALIQUOTA"] != DBNull.Value ? Convert.ToString(reader["ALIQUOTA"]) : "";
                            retorno.ISSQN_Atividade_Nf = reader["ID_ATIVIDADE"] != DBNull.Value ? Convert.ToInt64(reader["ID_ATIVIDADE"]) : 0;
                            retorno.ISSQN_CFOP = reader["ID_CFOP"] != DBNull.Value ? Convert.ToInt64(reader["ID_CFOP"]) : 0;
                            retorno.ISSQN_Cod_Tributacao_Municipio = reader["COD_TRIBUTACAO_MUNICIPIO"] != DBNull.Value ? Convert.ToString(reader["COD_TRIBUTACAO_MUNICIPIO"]) : "";
                            retorno.ISSQN_Municipio = reader["ID_CIDADE"] != DBNull.Value ? Convert.ToInt64(reader["ID_CIDADE"]) : 0;
                            retorno.ISSQN_Natureza_Tributacao = reader["NATUREZA_TRIBUTACAO"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Natureza_TributacaoEnum>(Convert.ToString(reader["NATUREZA_TRIBUTACAO"])) : "";
                            retorno.ISSQN_UF = reader["UF"] != DBNull.Value ? Convert.ToString(reader["UF"]) : "";
                            retorno.TipoTributacaoISS = reader["TIPO_TRIBUTACAO_ISS"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<TipoTributacaoISS>(Convert.ToString(reader["TIPO_TRIBUTACAO_ISS"])) : "";
                        }
                    }
                }

                return retorno;
            }
            catch
            {
                return null;
            }
        }

    }

    public class ConfiguracaoTributariaApiRequest : ApiRequest
    {
        public bool Alterado { get; set; }
    }
}