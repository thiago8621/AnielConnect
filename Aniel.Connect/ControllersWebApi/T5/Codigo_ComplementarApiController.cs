using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Models.T5;
using FluentNHibernate.Utils;
using NHibernate;
using NHibernate.Criterion;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.Identity;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class Codigo_ComplementarApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/Codigo_ComplementarApi/GetCombo")]
        public async Task<ObjCombo<String>> GetCombo(RequestObjComboPaiConfig<String, String, Codigo_ComplementarConfigCombo> request)
        {
            ObjCombo<String> retorno = new ObjCombo<string>
            {
                results = new List<IObjComboResult<string>>()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder(@"FROM TB_CODIGO_COMPLEMENTAR A
                                                                INNER JOIN TB_MATERIAL B
                                                                ON(A.CODMAT = B.CODMAT)
                                                                LEFT JOIN TB_CONTRATO C
                                                                ON(A.CODCT = C.CODCT)
                                                                WHERE (A.SITUACAO <> 'I' AND ((C.TIPOSALDO = 'EMPRESA' or C.TIPOSALDOAPLIC = 'EMPRESA') OR
                                                                       ((C.TIPOSALDO = 'CONTRATO' or C.TIPOSALDOAPLIC = 'CONTRATO') AND A.CODCT = @CODCT) OR
                                                                        (A.CODCT = @CODCT AND A.PROJETO = @PROJETO)
                                                                      )) ");

                    List<String> condicoes = new List<String>();


                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add("A.CODCPL = @CODCPL");
                        parametros.Add("@CODCPL", request.id);
                    }

                    if (!String.IsNullOrEmpty(request.idpai))
                    {
                        condicoes.Add("B.CODMAT = @CODMAT");
                        parametros.Add("@CODMAT", request.idpai);
                    }

                    if (!String.IsNullOrEmpty(request.config.codct))
                    {
                        parametros.Add("@CODCT", request.config.codct);
                    }
                    else
                    {
                        return retorno;
                    }

                    if (!String.IsNullOrEmpty(request.config.projeto))
                    {
                        //Decodificando o HTML escape para a query funcionar com caracteres especiais neste parametro
                        String ProjetoDecode = HttpUtility.HtmlDecode(request.config.projeto);
                        parametros.Add("@PROJETO", ProjetoDecode);
                    }
                    else
                    {
                        return retorno;
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.CODCPL) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 100)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" AND ( {String.Join(" AND ", condicoes)} ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.CODCPL) AS CO ");
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
                        StringBuilder sql = new StringBuilder($@"SELECT A.CODMAT, A.CODCPL 
                                                                {sqlFrom.ToString()}
                                                                GROUP BY A.CODMAT, A.CODCPL                                                                
                                                                ORDER BY UPPER(A.CODCPL)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new Codigo_ComplementarListaModel
                                {
                                    Id = Convert.ToString(reader["CODCPL"]),
                                    Text = Convert.ToString(reader["CODCPL"]),
                                    CodMat = Convert.ToString(reader["CODMAT"])
                                };
                                retorno.results.Add(obj);
                            }
                        }

                        if (request != null && request.config.aceitaNovo && !String.IsNullOrEmpty(request.q))
                        {
                            retorno.results.Add(new Codigo_ComplementarListaModel
                            {
                                Id = request.q,
                                Text = request.q,
                                CodMat = request.idpai
                            });
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


        public async Task<Codigo_ComplementarTemEstoque> TemEstoque(ISession session, String CodMat, String CodCpl, String Classificacao)
        {
            Codigo_ComplementarTemEstoque retorno = null;

            try
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder(@"SELECT FIRST 1 A.CODCPL, A.CODMAT, A.CODCT, A.PROJETO, A.EQUIPE, B.NOME, A.DATA, 
                                                                A.DIA, A.HORA, A.SEQLIVRE, A.SEQRMDM, A.SEQTRANSF, A.ENTRADA, A.SAIDA, A.COD_TIPO_MOVTO, 
                                                                C.FORNECEDOR, A.USUARIO, A.PEDIDO, 
                                                                 CASE 
                                                                   WHEN(D.CLASSIFICACAO_RMDM IS NULL) OR(D.CLASSIFICACAO_RMDM = '') THEN E.CLASSIFICACAO 
                                                                   WHEN D.CLASSIFICACAO_RMDM IS NOT NULL THEN D.CLASSIFICACAO_RMDM
                                                                 END AS CLASSIFICACAO_RMDM, 
                                                                A.COD_FORNECEDOR, A.NOTAFISCAL FROM TB_ESTOQUE A
                                                                LEFT OUTER JOIN TB_EQUIPE B ON(A.EQUIPE = B.EQUIPE)
                                                                LEFT OUTER JOIN TB_FORNECEDOR C ON(A.COD_FORNECEDOR = C.COD_FORNECEDOR)
                                                                LEFT OUTER JOIN TB_MOVIMENTO_RMDM D ON(D.CODCT = A.CODCT AND D.PROJETO = A.PROJETO AND D.SEQRMDM = A.SEQRMDM
                                                                   AND D.CODMAT = A.CODMAT AND D.CODCPL = A.CODCPL)
                                                                LEFT OUTER JOIN TB_MATERIAL E ON(A.CODMAT = E.CODMAT)
                                                                WHERE A.CODCPL = @CODCPL ");

                    parametros.Add("@CODCPL", CodCpl);
                    
                    if (!String.IsNullOrEmpty(CodMat))
                    {
                        sql.AppendLine(" AND (A.CODMAT = @CODMAT OR A.CODMAT = @CODMATSEMR)");
                        parametros.Add("@CODMAT", CodMat);
                        parametros.Add("@CODMATSEMR", Regex.Replace(CodMat, "^R",""));
                    }

                    if (!String.IsNullOrEmpty(Classificacao))
                    {
                        sql.AppendLine(" AND E.CLASSIFICACAO = @CLASSIFICACAO");
                        parametros.Add("@CLASSIFICACAO", Classificacao);
                    }

                    sql.AppendLine(" ORDER BY A.DIA DESC, A.HORA DESC, A.ENTRADA DESC ");

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            retorno = new Codigo_ComplementarTemEstoque
                            {
                                CODCPL = reader["CODCPL"] != DBNull.Value ? Convert.ToString(reader["CODCPL"]) : "",
                                CODMAT = reader["CODMAT"] != DBNull.Value ? Convert.ToString(reader["CODMAT"]) : "",
                                CODCT = reader["CODCT"] != DBNull.Value ? Convert.ToString(reader["CODCT"]) : "",
                                PROJETO = reader["PROJETO"] != DBNull.Value ? Convert.ToString(reader["PROJETO"]) : "",
                                EQUIPE = reader["EQUIPE"] != DBNull.Value ? Convert.ToString(reader["EQUIPE"]) : "",
                                NOME = reader["NOME"] != DBNull.Value ? Convert.ToString(reader["NOME"]) : "",
                                DATA = reader["DATA"] != DBNull.Value ? Convert.ToDateTime(reader["DATA"]) : (DateTime?)null,
                                DIA = reader["DIA"] != DBNull.Value ? Convert.ToDateTime(reader["DIA"]) : (DateTime?)null,
                                HORA = reader["HORA"] != DBNull.Value ? Convert.ToDateTime(reader["DIA"]).Add((TimeSpan)(reader["HORA"])) : (DateTime?)null,
                                SEQLIVRE = reader["SEQLIVRE"] != DBNull.Value ? Convert.ToString(reader["SEQLIVRE"]) : "",
                                SEQRMDM = reader["SEQRMDM"] != DBNull.Value ? Convert.ToString(reader["SEQRMDM"]) : "",
                                SEQTRANSF = reader["SEQTRANSF"] != DBNull.Value ? Convert.ToString(reader["SEQTRANSF"]) : "",
                                ENTRADA = reader["ENTRADA"] != DBNull.Value ? Convert.ToDouble(reader["ENTRADA"]) : (Double?)null,
                                SAIDA = reader["SAIDA"] != DBNull.Value ? Convert.ToDouble(reader["SAIDA"]) : (Double?)null,
                                COD_TIPO_MOVTO = reader["COD_TIPO_MOVTO"] != DBNull.Value ? Convert.ToInt16(reader["COD_TIPO_MOVTO"]) : (Int16?)null,
                                FORNECEDOR = reader["FORNECEDOR"] != DBNull.Value ? Convert.ToString(reader["FORNECEDOR"]) : "",
                                USUARIO = reader["USUARIO"] != DBNull.Value ? Convert.ToString(reader["USUARIO"]) : "",
                                PEDIDO = reader["PEDIDO"] != DBNull.Value ? Convert.ToString(reader["PEDIDO"]) : "",
                                CLASSIFICACAO_RMDM = reader["CLASSIFICACAO_RMDM"] != DBNull.Value ? Convert.ToString(reader["CLASSIFICACAO_RMDM"]) : "",
                                COD_FORNECEDOR = reader["COD_FORNECEDOR"] != DBNull.Value ? Convert.ToString(reader["COD_FORNECEDOR"]) : "",
                                NOTAFISCAL = reader["NOTAFISCAL"] != DBNull.Value ? Convert.ToString(reader["NOTAFISCAL"]) : "",
                            };
                        }
                    }
                }
                return retorno;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Codigo_ComplementarTemAplicacao> TemAplicacao(ISession session, String CodMat, String CodCpl, String Classificacao)
        {
            Codigo_ComplementarTemAplicacao retorno = null;

            try
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder(@"SELECT FIRST 1 A.CODCT, A.PROJETO, A.TAREFA, A.NUM_OBRA, A.CODMAT, 
                                                                               A.CODCPL, A.NUM_PROTOCOLO, A.DIA, A.HORA, A.EQUIPE, B.NOME, A.DATA_APLIC, 
                                                                               A.USUARIO, A.NUM_PROTOCOLO, A.NUM_DOC, A.QUANT_REMOV, A.DATA_PROTOCOLO, 
                                                                               C.CLASSIFICACAO AS CLASSIFICACAO_RMDM 
                                                                  FROM TB_MATERIAL_TAREFA A 
                                                                  LEFT OUTER JOIN TB_EQUIPE B ON (A.EQUIPE = B.EQUIPE)
                                                                  LEFT OUTER JOIN TB_MATERIAL C ON (A.CODMAT = C.CODMAT)
                                                                 WHERE A.CODCPL = @CODCPL ");

                    parametros.Add("@CODCPL", CodCpl);

                    if (!String.IsNullOrEmpty(CodMat))
                    {
                        sql.AppendLine(" AND (A.CODMAT = @CODMAT OR A.CODMAT = @CODMATSEMR)");
                        parametros.Add("@CODMAT", CodMat);
                        parametros.Add("@CODMATSEMR", Regex.Replace(CodMat, "^R", ""));
                    }

                    if (!String.IsNullOrEmpty(Classificacao))
                    {
                        sql.AppendLine(" AND C.CLASSIFICACAO = @CLASSIFICACAO ");
                        parametros.Add("@CLASSIFICACAO", Classificacao);
                    }

                    sql.AppendLine(" ORDER BY A.DIA DESC, A.HORA DESC ");

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            retorno = new Codigo_ComplementarTemAplicacao
                            {
                                CODCT = reader["CODCT"] != DBNull.Value ? Convert.ToString(reader["CODCT"]) : "",
                                PROJETO = reader["PROJETO"] != DBNull.Value ? Convert.ToString(reader["PROJETO"]) : "",
                                TAREFA = reader["TAREFA"] != DBNull.Value ? Convert.ToString(reader["TAREFA"]) : "",
                                NUM_OBRA = reader["NUM_OBRA"] != DBNull.Value ? Convert.ToString(reader["NUM_OBRA"]) : "",
                                CODMAT = reader["CODMAT"] != DBNull.Value ? Convert.ToString(reader["CODMAT"]) : "",
                                CODCPL = reader["CODCPL"] != DBNull.Value ? Convert.ToString(reader["CODCPL"]) : "",
                                NUM_PROTOCOLO = reader["NUM_PROTOCOLO"] != DBNull.Value ? Convert.ToString(reader["NUM_PROTOCOLO"]) : "",
                                DIA = reader["DIA"] != DBNull.Value ? Convert.ToDateTime(reader["DIA"]) : (DateTime?)null,
                                HORA = reader["HORA"] != DBNull.Value ? Convert.ToDateTime(reader["DIA"]).Add((TimeSpan)reader["HORA"]) : (DateTime?)null,
                                EQUIPE = reader["EQUIPE"] != DBNull.Value ? Convert.ToString(reader["EQUIPE"]) : "",
                                NOME = reader["NOME"] != DBNull.Value ? Convert.ToString(reader["NOME"]) : "",
                                DATA_APLIC = reader["DATA_APLIC"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_APLIC"]) : (DateTime?)null,
                                USUARIO = reader["USUARIO"] != DBNull.Value ? Convert.ToString(reader["USUARIO"]) : "",
                                NUM_DOC = reader["NUM_DOC"] != DBNull.Value ? Convert.ToInt32(reader["NUM_DOC"]) : (Int32?)null,
                                QUANT_REMOV = reader["QUANT_REMOV"] != DBNull.Value ? Convert.ToDouble(reader["QUANT_REMOV"]) : (Double?)null,
                                DATA_PROTOCOLO = reader["DATA_PROTOCOLO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_PROTOCOLO"]) : (DateTime?)null,
                                CLASSIFICACAO_RMDM = reader["CLASSIFICACAO_RMDM"] != DBNull.Value ? Convert.ToString(reader["CLASSIFICACAO_RMDM"]) : ""
                            };
                        }
                    }
                }
                return retorno;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<Codigo_ComplementarLocalizacao> GetLocalizacao(ISession session, String CodMat, String CodCpl, String Classificacao)
        {
            Codigo_ComplementarLocalizacao retorno = new Codigo_ComplementarLocalizacao
            {
                Status = Codigo_ComplementarLocalizacaoEnum.NaoEncontrado
            };

            var Estoque = await TemEstoque(session, CodMat, CodCpl, Classificacao);
            var Aplicado = await TemAplicacao(session, CodMat, CodCpl, Classificacao);


            if (Estoque != null && Aplicado != null)
            {
                if (Estoque.DIA.Value.AddHours(Estoque.HORA.Value.Hour).AddMinutes(Estoque.HORA.Value.Minute) >
                    Aplicado.DIA.Value.AddHours(Aplicado.HORA.Value.Hour).AddMinutes(Aplicado.HORA.Value.Minute))
                    retorno.Status = Codigo_ComplementarLocalizacaoEnum.Fisico;
                else
                    retorno.Status = Codigo_ComplementarLocalizacaoEnum.Aplicado;
            }
            else if (Estoque != null)
            {
                retorno.Status = Codigo_ComplementarLocalizacaoEnum.Fisico;
            }
            else if (Aplicado != null)
            {
                retorno.Status = Codigo_ComplementarLocalizacaoEnum.Aplicado;
            }
            else if (Estoque == null && Aplicado == null)
            {
                retorno.Status = Codigo_ComplementarLocalizacaoEnum.SemSaldo;
            }

            if (retorno.Status == Codigo_ComplementarLocalizacaoEnum.Fisico && Estoque.COD_TIPO_MOVTO == 6 && !Estoque.COD_FORNECEDOR.In(null, "", "-"))
            {
                retorno.Status = Codigo_ComplementarLocalizacaoEnum.Fornecedor;
            }
            else if (retorno.Status == Codigo_ComplementarLocalizacaoEnum.Fisico && Estoque.SAIDA > 0 && !Estoque.EQUIPE.In(null, "", "-"))
            {
                retorno.Status = Codigo_ComplementarLocalizacaoEnum.Volante;
            }
            else if (retorno.Status == Codigo_ComplementarLocalizacaoEnum.Fisico && Estoque.SAIDA > 0 && Estoque.EQUIPE.In(null, "", "-"))
            {
                retorno.Status = Codigo_ComplementarLocalizacaoEnum.SemSaldo;
            }
            else if (retorno.Status == Codigo_ComplementarLocalizacaoEnum.Fisico && (Estoque.CLASSIFICACAO_RMDM == "13" || Estoque.CLASSIFICACAO_RMDM == "R"))
            {
                retorno.Status = Codigo_ComplementarLocalizacaoEnum.Removido;
            }

            if (retorno.Status == Codigo_ComplementarLocalizacaoEnum.SemSaldo && !(await Existe(session, CodCpl)))
            {
                retorno.Status = Codigo_ComplementarLocalizacaoEnum.NaoEncontrado;
            }

            retorno.Dia = Estoque?.DIA;
            retorno.Hora = Estoque?.HORA;
            retorno.Classificacao = Estoque?.CLASSIFICACAO_RMDM;

            return retorno;
        }

        public async Task<Boolean> Existe(ISession session, String CodCpl)
        {
            try
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder(@"SELECT COUNT(*) AS CO FROM TB_CODIGO_COMPLEMENTAR WHERE CODCPL = @CODCPL");

                    parametros.Add("@CODCPL", CodCpl);

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            return Convert.ToInt32(reader["CO"]) > 0;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task IncluirCodCpl(ISession session, String CodCt, String Projeto, String CodMat, String CodCpl, String Localizacao)
        {
            try
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder(@"EXECUTE PROCEDURE SP_CADASTRAR_CODCPL_PROJETO(@PCODCT,@PPROJETO,@PCODMAT,@PCODCPL,@PRIR,@PLOCALIZACAO)");

                    parametros.Add("@PCODCT", CodCt);
                    parametros.Add("@PPROJETO", Projeto);
                    parametros.Add("@PCODMAT", CodMat);
                    parametros.Add("@PCODCPL", CodCpl);
                    parametros.Add("@PRIR", "-");
                    parametros.Add("@PLOCALIZACAO", Localizacao);

                    await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<PosicaoEstoque> PosicaoEstoque(string codcpl)
        {
            PosicaoEstoque retorno = new PosicaoEstoque
            {
                Posicao = "",
                InfoAdicional = ""
            };

            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var com = sessionT5.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> param = new Dictionary<string, object>();

                sql.AppendLine("SELECT FIRST 1 RPOSICAO AS POSICAO_SALDO, RRESPONSAVEL AS INFO_ADICIONAL");
                sql.AppendLine("FROM SP_RASTREABILIDADE_CODCPL(@CODCPL)");
                sql.AppendLine("ORDER BY RDIA, RHORA DESC");

                param.Add("@CODCPL", codcpl);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, param, sessionT5.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        retorno.Posicao = Convert.ToString(reader["POSICAO_SALDO"]) ?? "";
                        retorno.InfoAdicional = Convert.ToString(reader["INFO_ADICIONAL"]) ?? "";
                    }
                }
            }

            return retorno;
        }
    }

}
