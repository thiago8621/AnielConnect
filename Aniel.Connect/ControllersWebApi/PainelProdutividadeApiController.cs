using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using OpenXmlPowerTools;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class PainelProdutividadeApiController : BaseApiController
    {
        [HttpPost]
        [Route("api/PainelProdutividadeApi/GetAllTecnicos")]
        public async Task<ICollection<ListaProdutividadeModels>> GetAllTecnicos(ProdutividadeDataTableRequest request)
        {
            List<ListaProdutividadeModels> leituraBanco = new List<ListaProdutividadeModels>();
            List<ListaProdutividadeModels> retorno = new List<ListaProdutividadeModels>();

            try
            {
                using (var session = HibernateUtil.GetSession())
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                using (var repoConfigEmpresa = new Conexao<Configuracoes_Gerais>(session))
                using (var repoConfigProdutividade = new Conexao<PainelProdutividadeConfig>(session))
                {
                    Dictionary<string, object> parametros = new Dictionary<string, object>();
                    var horaPadrao = 0;
                    var dateInicial = Convert.ToDateTime(request.DataInicio).Date.ToString("yyyy-MM-dd");
                    var dateFim = Convert.ToDateTime(request.DataFim).Date.ToString("yyyy-MM-dd");
                    var tempoExecucaoPadrao = repoConfigEmpresa.selectFirst().Result.Tempo_Execucao_Padrao;

                    if (tempoExecucaoPadrao.HasValue)
                        horaPadrao = (int)tempoExecucaoPadrao.Value.TimeOfDay.TotalMinutes;

                    var configProdutiviade = await repoConfigProdutividade.selectFirst();

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT DOCP.EQUIPE, EQUI.NOME AS TECNICO, REG.NOME AS REGIAO, ");
                    sql.AppendLine(" SUM(CASE WHEN DOCP.STATUS_EXECUCAO = 2 THEN 1 ELSE 0 END) AS OSPROD, ");
                    sql.AppendLine(" SUM(CASE WHEN DOCP.STATUS_EXECUCAO = 1 THEN 1 ELSE 0 END) AS OSIMPRD, ");
                    sql.AppendLine(" COUNT(DOCP.EQUIPE) AS OSTOTAL, ");
                    sql.AppendLine(" COUNT(CASE WHEN DOCP.STATUS_EXECUCAO = 2 THEN REINC.NUM_OBRA END) AS REINCIDENCIASPROD, ");
                    sql.AppendLine(" COUNT(REINC.NUM_OBRA) AS REINCIDENCIASTOT, ");
                    sql.AppendLine(" SUM(CASE WHEN DOCP.STATUS_EXECUCAO = 2 AND (ROUND((AGEND.DATAHORA_FIM - AGEND.DATAHORA_INICIO) * 60)) <= COALESCE(((EXTRACT(HOUR FROM TSERV.HORAS)*60)+(EXTRACT(MINUTE FROM TSERV.HORAS))), @VALOR) THEN 1 ELSE 0 END) AS BAIXAPRAZOPROD, ");
                    sql.AppendLine(" SUM(CASE WHEN DOCP.STATUS_EXECUCAO = 1 AND (ROUND((AGEND.DATAHORA_FIM - AGEND.DATAHORA_INICIO) * 60)) <= COALESCE(((EXTRACT(HOUR FROM TSERV.HORAS)*60)+(EXTRACT(MINUTE FROM TSERV.HORAS))), @VALOR) THEN 1 ELSE 0 END) AS BAIXAPRAZOIMPROD, ");
                    sql.AppendLine(" COUNT(CASE WHEN (ROUND((AGEND.DATAHORA_FIM - AGEND.DATAHORA_INICIO) * 60)) <= COALESCE(((EXTRACT(HOUR FROM TSERV.HORAS)*60)+(EXTRACT(MINUTE FROM TSERV.HORAS))), @VALOR) THEN AGEND.DATAHORA_FIM END) AS TOTBAIXASPRAZO, ");
                    sql.AppendLine(" SUM(CASE WHEN DOCP.STATUS_AUDITORIA = 1 AND DOCP.STATUS_EXECUCAO = 2 THEN 1 ELSE 0 END) AS AUD_APROVADA, ");
                    sql.AppendLine(" SUM(CASE WHEN DOCP.STATUS_AUDITORIA = 0 AND DOCP.STATUS_EXECUCAO = 2 THEN 1 ELSE 0 END) AS AUD_REPROVADO, ");
                    sql.AppendLine(" SUM(CASE WHEN DOCP.STATUS_EXECUCAO = 2 AND DOCP.STATUS_AUDITORIA = 2 OR DOCP.STATUS_AUDITORIA IS NULL THEN 1 ELSE 0 END) AS AUD_NFEITAS, ");
                    sql.AppendLine(" COUNT(CASE WHEN DOCP.STATUS_EXECUCAO = 2 AND DOCP.STATUS_AUDITORIA IS NULL OR DOCP.STATUS_AUDITORIA IN(0,1,2) THEN 1 ELSE 0 END) AS AUD_TOTAIS, ");
                    sql.AppendLine(" COUNT(CASE WHEN (DOCP.DATA_INDICADOR + DOCP.HORA_INDICADOR) >= (DOCP.DATA_PROD + DOCP.HORA_PROD) THEN DOCP.DATA_INDICADOR END) AS SLA_PRAZO, ");
                    sql.AppendLine(" COUNT(CASE WHEN (DOCP.DATA_INDICADOR + DOCP.HORA_INDICADOR) <= (DOCP.DATA_PROD + DOCP.HORA_PROD) THEN DOCP.DATA_INDICADOR END) AS SLA_FPRAZO, ");
                    sql.AppendLine(" COUNT(DOCP.DATA_INDICADOR) AS SLA_TOTAL ");
                    sql.AppendLine(" FROM TB_DOCUMENTO_PRODUCAO DOCP ");
                    sql.AppendLine(" LEFT JOIN TB_EQUIPE EQUI ");
                    sql.AppendLine(" ON EQUI.EQUIPE = DOCP.EQUIPE ");
                    sql.AppendLine(" LEFT JOIN TB_REGIAO REG ");
                    sql.AppendLine(" ON REG.COD_REGIAO  = EQUI.COD_REGIAO ");
                    sql.AppendLine(" LEFT JOIN TB_REINCIDENCIA_DOCPROD REINC ");
                    sql.AppendLine(" ON REINC.CODCT  = DOCP.CODCT AND REINC.PROJETO = DOCP.PROJETO AND REINC.NUM_OBRA = DOCP.NUM_OBRA AND REINC.NUM_DOC  = DOCP.NUM_DOC ");
                    sql.AppendLine(" LEFT JOIN TB_AGENDAMENTO_EQUIPEANIEL AGEND ON AGEND.CODCT  = DOCP.CODCT AND AGEND.PROJETO = DOCP.PROJETO AND AGEND.NUM_OBRA = DOCP.NUM_OBRA AND AGEND.NUM_DOC = DOCP.NUM_DOC ");
                    sql.AppendLine(" LEFT JOIN TB_TIPO_SERVICO_EQUIPE_IMOVEL TSERV ON TSERV.ID_TIPO_IMOVEL = AGEND.ID_TIPO_SERV ");
                    sql.AppendLine(" WHERE DOCP.STATUS_EXECUCAO IS NOT NULL AND EQUI.NOME IS NOT NULL  ");
                    parametros.Add("@VALOR", horaPadrao);

                    if (!string.IsNullOrEmpty(request.TipoData))
                    {
                        if (request.TipoData == "0")
                        {
                            sql.AppendLine(" AND DOCP.DIA BETWEEN @DATAINICIO AND @DATAFIM ");
                        }
                        else if (request.TipoData == "1")
                        {
                            sql.AppendLine(" AND DOCP.DATA_MAXIMA BETWEEN @DATAINICIO AND @DATAFIM ");
                        }
                        else
                        {
                            sql.AppendLine(" AND DOCP.DATA_PROD BETWEEN @DATAINICIO AND @DATAFIM ");
                        }

                        parametros.Add("@DATAINICIO", dateInicial);
                        parametros.Add("@DATAFIM", dateFim);
                    }

                    if (!string.IsNullOrEmpty(request.PessoaAgrupamento))
                    {
                        sql.AppendLine(" AND EQUI.EQUIPE = @IDEQUIPE ");
                        parametros.Add("@IDEQUIPE", request.PessoaAgrupamento);
                    }

                    if (!string.IsNullOrEmpty(request.ContratoValor))
                    {
                        sql.AppendLine(" AND DOCP.CODCT = @IDCONTRATO ");
                        parametros.Add("@IDCONTRATO", request.ContratoValor);            
                    }

                    if (!string.IsNullOrEmpty(request.ProjetoValor))
                    {
                        if (!string.IsNullOrEmpty(request.ContratoValor))
                        {
                            sql.AppendLine(" AND DOCP.PROJETO = @PROJETO");
                            parametros.Add("@PROJETO", request.ProjetoValor);
                        }                         
                    }

                    if (!string.IsNullOrEmpty(request.GrupoArea))
                    {
                        sql.AppendLine(" AND DOCP.COD_ESCRITORIO = @COD_ESCRITORIO ");
                        parametros.Add("@COD_ESCRITORIO", request.GrupoArea);
                    }

                    sql.AppendLine(" GROUP BY DOCP.EQUIPE, EQUI.NOME, REG.NOME ");
                    sql.AppendLine(" ORDER BY EQUI.NOME ASC ");


                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                leituraBanco.Add(new ListaProdutividadeModels()
                                {
                                    tecnico = reader["TECNICO"] != DBNull.Value ? Convert.ToString(reader["TECNICO"]) : "-",
                                    grupoArea = reader["REGIAO"] != DBNull.Value ? Convert.ToString(reader["REGIAO"]) : "-",
                                    osprodutiva = reader["OSPROD"] != DBNull.Value ? Convert.ToInt64(reader["OSPROD"]) : 0,
                                    osimprodutiva = reader["OSIMPRD"] != DBNull.Value ? Convert.ToInt64(reader["OSIMPRD"]) : 0,
                                    osTotal = reader["OSTOTAL"] != DBNull.Value ? Convert.ToInt64(reader["OSTOTAL"]) : 0,
                                    quantidadeRevisita = reader["REINCIDENCIASPROD"] != DBNull.Value ? Convert.ToInt64(reader["REINCIDENCIASPROD"]) : 0,
                                    revisitaTotal = reader["REINCIDENCIASTOT"] != DBNull.Value ? Convert.ToInt64(reader["REINCIDENCIASTOT"]) : 0,
                                    baixaProdutivaPrazo = reader["BAIXAPRAZOPROD"] != DBNull.Value ? Convert.ToInt64(reader["BAIXAPRAZOPROD"]) : 0,
                                    baixaImprodutivaPrazo = reader["BAIXAPRAZOIMPROD"] != DBNull.Value ? Convert.ToInt64(reader["BAIXAPRAZOIMPROD"]) : 0,
                                    baixaPrazoTotal = reader["TOTBAIXASPRAZO"] != DBNull.Value ? Convert.ToInt64(reader["TOTBAIXASPRAZO"]) : 0,
                                    vistoriaAprovada = reader["AUD_APROVADA"] != DBNull.Value ? Convert.ToInt64(reader["AUD_APROVADA"]) : 0,
                                    vistoriaReprovada = reader["AUD_REPROVADO"] != DBNull.Value ? Convert.ToInt64(reader["AUD_REPROVADO"]) : 0,
                                    vistoriaNaoFeitas = reader["AUD_NFEITAS"] != DBNull.Value ? Convert.ToInt64(reader["AUD_NFEITAS"]) : 0,
                                    vistoriaTotal = reader["AUD_TOTAIS"] != DBNull.Value ? Convert.ToInt64(reader["AUD_TOTAIS"]) : 0,
                                    dentroPrazoSLA = reader["SLA_PRAZO"] != DBNull.Value ? Convert.ToInt64(reader["SLA_PRAZO"]) : 0,
                                    foraPrazoSLA = reader["SLA_FPRAZO"] != DBNull.Value ? Convert.ToInt64(reader["SLA_FPRAZO"]) : 0,
                                    totalSLA = reader["SLA_TOTAL"] != DBNull.Value ? Convert.ToInt64(reader["SLA_TOTAL"]) : 0,
                                });
                            }
                        }
                    }

                    foreach (var item in leituraBanco)
                    {
                        var porcentagemProd = item.osprodutiva.Value != 0 ? ((item.osprodutiva * 100) / (float)(item.osimprodutiva + item.osprodutiva)) : 0;
                        var porcentagemRevisita = item.quantidadeRevisita != 0 ? ((item.quantidadeRevisita * 100) / (float)item.revisitaTotal) : 0;
                        var porcentagemBaixaPrazo = item.baixaProdutivaPrazo != 0 ? ((item.baixaProdutivaPrazo * 100) / (float)item.baixaPrazoTotal) : 0;
                        var porcentegemVistoria = item.vistoriaAprovada != 0 ? ((item.vistoriaAprovada * 100) / (float)item.vistoriaTotal) : 0;
                        var porcentagemSLA = item.dentroPrazoSLA != 0 ? ((item.dentroPrazoSLA * 100) / (float)item.totalSLA) : 0;

                        item.osporcentagemProducao = porcentagemProd.Value.ToString("F2");
                        item.porcentagemRevisita = porcentagemRevisita.Value.ToString("F2");
                        item.porcentagemBaixaNoPrazo = porcentagemBaixaPrazo.Value.ToString("F2");
                        item.porcentagemVistoria = porcentegemVistoria.Value.ToString("F2");
                        item.porcentagemSLA = porcentagemSLA.Value.ToString("F2");

                        Double total = 0;

                        
                        if(configProdutiviade != null)
                        {
                            if (configProdutiviade.ProducaoPorcentTec != null)
                            {
                                if (porcentagemProd >= configProdutiviade.ProducaoPorcentTec.Value)
                                {
                                    item.osprecoProducao = configProdutiviade.MetaProdutivaTec.Value.ToString("F2");
                                    total = total + configProdutiviade.MetaProdutivaTec.Value;
                                }
                            }

                            if (configProdutiviade.RevisitaPorcentTec != null)
                            {
                                if (porcentagemRevisita >= configProdutiviade.RevisitaPorcentTec.Value)
                                {
                                    item.precoRevisita = configProdutiviade.MetaRevisitaTec.Value.ToString("F2");
                                    total = total + configProdutiviade.MetaRevisitaTec.Value;
                                }
                            }

                            if (configProdutiviade.BaixasPrazoPorcentTec != null)
                            {
                                if (porcentagemBaixaPrazo >= configProdutiviade.BaixasPrazoPorcentTec.Value)
                                {
                                    item.precoBaixaTEC = configProdutiviade.MetaBaixasPrazoTec.Value.ToString("F2");
                                    total = total + configProdutiviade.MetaBaixasPrazoTec.Value;
                                }
                            }

                            if (configProdutiviade.VistoriaPorcentTec != null)
                            {
                                if (porcentegemVistoria >= configProdutiviade.VistoriaPorcentTec.Value)
                                {
                                    item.precoVistoria = configProdutiviade.VistoriaPorcentTec.Value.ToString("F2");
                                    total = total + configProdutiviade.MetaVistoriaTec.Value;
                                }
                            }

                            if (configProdutiviade.SLAPorcentTec != null)
                            {
                                if (porcentagemSLA >= configProdutiviade.SLAPorcentTec.Value)
                                {
                                    item.precoSLA = configProdutiviade.MetaSLATec.Value.ToString("F2");
                                    total = total + configProdutiviade.MetaSLATec.Value;
                                }
                            }
                        }
                        
                        item.totalPrecoSomado = total.ToString("F2");

                        retorno.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retorno;
        }

        [HttpPost]
        [Route("api/PainelProdutividadeApi/GetAllSupervisores")]
        public async Task<ICollection<ListaProdutividadeModels>> GetAllSupervisores(ProdutividadeDataTableRequest request)
        {
            List<ListaProdutividadeModels> leituraBanco = new List<ListaProdutividadeModels>();
            List<ListaProdutividadeModels> retorno = new List<ListaProdutividadeModels>();

            try
            {
                using (var session = HibernateUtil.GetSession())
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                using (var repoConfigEmpresa = new Conexao<Configuracoes_Gerais>(session))
                using (var repoConfigProdutividade = new Conexao<PainelProdutividadeConfig>(session))
                {
                    Dictionary<string, object> parametros = new Dictionary<string, object>();
                    var horaPadrao = 0;
                    var dateInicial = Convert.ToDateTime(request.DataInicio).Date.ToString("yyyy-MM-dd");
                    var dateFim = Convert.ToDateTime(request.DataFim).Date.ToString("yyyy-MM-dd");
                    var tempoExecucaoPadrao = repoConfigEmpresa.selectFirst().Result.Tempo_Execucao_Padrao;

                    if (tempoExecucaoPadrao.HasValue)
                        horaPadrao = (int)tempoExecucaoPadrao.Value.TimeOfDay.TotalMinutes;

                    var configProdutiviade = await repoConfigProdutividade.selectFirst();

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT FUNC.NOME  AS SUPERVISOR, REG.NOME AS REGIAO, ");
                    sql.AppendLine(" SUM(CASE WHEN DOCP.STATUS_EXECUCAO = 2 THEN 1 ELSE 0 END) AS OSPROD, ");
                    sql.AppendLine(" SUM(CASE WHEN DOCP.STATUS_EXECUCAO = 1 THEN 1 ELSE 0 END) AS OSIMPRD, ");
                    sql.AppendLine(" COUNT(DOCP.EQUIPE) AS OSTOTAL, ");
                    sql.AppendLine(" COUNT(CASE WHEN DOCP.STATUS_EXECUCAO = 2 THEN REINC.NUM_OBRA END) AS REINCIDENCIASPROD, ");
                    sql.AppendLine(" COUNT(REINC.NUM_OBRA) AS REINCIDENCIASTOT, ");
                    sql.AppendLine(" SUM(CASE WHEN DOCP.STATUS_EXECUCAO = 2 AND (ROUND((AGEND.DATAHORA_FIM - AGEND.DATAHORA_INICIO) * 60)) <= COALESCE(((EXTRACT(HOUR FROM TSERV.HORAS)*60)+(EXTRACT(MINUTE FROM TSERV.HORAS))), @VALOR) THEN 1 ELSE 0 END) AS BAIXAPRAZOPROD, ");
                    sql.AppendLine(" SUM(CASE WHEN DOCP.STATUS_EXECUCAO = 1 AND (ROUND((AGEND.DATAHORA_FIM - AGEND.DATAHORA_INICIO) * 60)) <= COALESCE(((EXTRACT(HOUR FROM TSERV.HORAS)*60)+(EXTRACT(MINUTE FROM TSERV.HORAS))), @VALOR) THEN 1 ELSE 0 END) AS BAIXAPRAZOIMPROD, ");
                    sql.AppendLine(" COUNT(CASE WHEN (ROUND((AGEND.DATAHORA_FIM - AGEND.DATAHORA_INICIO) * 60)) <= COALESCE(((EXTRACT(HOUR FROM TSERV.HORAS)*60)+(EXTRACT(MINUTE FROM TSERV.HORAS))), @VALOR) THEN AGEND.DATAHORA_FIM END) AS TOTBAIXASPRAZO, ");
                    sql.AppendLine(" COUNT(CASE WHEN (DOCP.DATA_INDICADOR + DOCP.HORA_INDICADOR) >= (DOCP.DATA_PROD + DOCP.HORA_PROD) THEN DOCP.DATA_INDICADOR END) AS SLA_PRAZO, ");
                    sql.AppendLine(" COUNT(CASE WHEN (DOCP.DATA_INDICADOR + DOCP.HORA_INDICADOR) <= (DOCP.DATA_PROD + DOCP.HORA_PROD) THEN DOCP.DATA_INDICADOR END) AS SLA_FPRAZO, ");
                    sql.AppendLine(" COUNT(DOCP.DATA_INDICADOR) AS SLA_TOTAL ");
                    sql.AppendLine(" FROM TB_DOCUMENTO_PRODUCAO DOCP ");
                    sql.AppendLine(" LEFT JOIN TB_EQUIPE EQUI ");
                    sql.AppendLine(" ON EQUI.EQUIPE = DOCP.EQUIPE ");
                    sql.AppendLine(" LEFT JOIN TB_REGIAO REG ");
                    sql.AppendLine(" ON REG.COD_REGIAO  = EQUI.COD_REGIAO ");
                    sql.AppendLine(" LEFT JOIN TB_REINCIDENCIA_DOCPROD REINC ");
                    sql.AppendLine(" ON REINC.CODCT  = DOCP.CODCT AND REINC.PROJETO = DOCP.PROJETO AND REINC.NUM_OBRA = DOCP.NUM_OBRA AND REINC.NUM_DOC  = DOCP.NUM_DOC ");
                    sql.AppendLine(" LEFT JOIN TB_AGENDAMENTO_EQUIPEANIEL AGEND ON AGEND.CODCT  = DOCP.CODCT AND AGEND.PROJETO = DOCP.PROJETO AND AGEND.NUM_OBRA = DOCP.NUM_OBRA AND AGEND.NUM_DOC = DOCP.NUM_DOC ");
                    sql.AppendLine(" LEFT JOIN TB_TIPO_SERVICO_EQUIPE_IMOVEL TSERV ON TSERV.ID_TIPO_IMOVEL = AGEND.ID_TIPO_SERV ");
                    sql.AppendLine(" LEFT JOIN TB_FUNCIONARIO FUNC ON FUNC.NUM_FRE = EQUI.COD_SUPERV  ");
                    sql.AppendLine(" WHERE DOCP.STATUS_EXECUCAO IS NOT NULL ");
                    sql.AppendLine(" AND EQUI.COD_SUPERV IS NOT NULL ");
                    parametros.Add("@VALOR", horaPadrao);

                    if (!string.IsNullOrEmpty(request.TipoData))
                    {
                        if (request.TipoData == "0")
                        {
                            sql.AppendLine(" AND DOCP.DIA BETWEEN @DATAINICIO AND @DATAFIM ");
                        }
                        else if (request.TipoData == "1")
                        {
                            sql.AppendLine(" AND DOCP.DATA_MAXIMA BETWEEN @DATAINICIO AND @DATAFIM ");
                        }
                        else
                        {
                            sql.AppendLine(" AND DOCP.DATA_PROD BETWEEN @DATAINICIO AND @DATAFIM ");
                        }

                        parametros.Add("@DATAINICIO", dateInicial);
                        parametros.Add("@DATAFIM", dateFim);
                    }

                    if (!string.IsNullOrEmpty(request.PessoaAgrupamento))
                    {
                        sql.AppendLine(" AND FUNC.NUM_FRE = @IDSUPERV ");
                        parametros.Add("@IDSUPERV", request.PessoaAgrupamento);
                    }

                    if (!string.IsNullOrEmpty(request.ContratoValor))
                    {
                        if (!string.IsNullOrEmpty(request.ProjetoValor))
                        {
                            sql.AppendLine(" AND DOCP.CODCT = @IDCONTRATO AND DOCP.PROJETO = @PROJETO");
                            parametros.Add("@IDCONTRATO", request.ContratoValor);
                            parametros.Add("@PROJETO", request.ProjetoValor);
                        }
                        else
                            throw new ArgumentException("Não é possivel realizar a consulta de um contrato sem projeto");
                    }

                    if (!string.IsNullOrEmpty(request.GrupoArea))
                    {
                        sql.AppendLine(" AND DOCP.COD_ESCRITORIO = @COD_ESCRITORIO ");
                        parametros.Add("@COD_ESCRITORIO", request.GrupoArea);
                    }

                    sql.AppendLine(" GROUP BY FUNC.NOME, REG.NOME ");
                    sql.AppendLine(" ORDER BY FUNC.NOME ASC ");


                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                leituraBanco.Add(new ListaProdutividadeModels()
                                {
                                    supervisor = reader["SUPERVISOR"] != DBNull.Value ? Convert.ToString(reader["SUPERVISOR"]) : "-",
                                    grupoArea = reader["REGIAO"] != DBNull.Value ? Convert.ToString(reader["REGIAO"]) : "-",
                                    osprodutiva = reader["OSPROD"] != DBNull.Value ? Convert.ToInt64(reader["OSPROD"]) : 0,
                                    osimprodutiva = reader["OSIMPRD"] != DBNull.Value ? Convert.ToInt64(reader["OSIMPRD"]) : 0,
                                    osTotal = reader["OSTOTAL"] != DBNull.Value ? Convert.ToInt64(reader["OSTOTAL"]) : 0,
                                    quantidadeRevisita = reader["REINCIDENCIASPROD"] != DBNull.Value ? Convert.ToInt64(reader["REINCIDENCIASPROD"]) : 0,
                                    revisitaTotal = reader["REINCIDENCIASTOT"] != DBNull.Value ? Convert.ToInt64(reader["REINCIDENCIASTOT"]) : 0,
                                    baixaProdutivaPrazo = reader["BAIXAPRAZOPROD"] != DBNull.Value ? Convert.ToInt64(reader["BAIXAPRAZOPROD"]) : 0,
                                    baixaImprodutivaPrazo = reader["BAIXAPRAZOIMPROD"] != DBNull.Value ? Convert.ToInt64(reader["BAIXAPRAZOIMPROD"]) : 0,
                                    baixaPrazoTotal = reader["TOTBAIXASPRAZO"] != DBNull.Value ? Convert.ToInt64(reader["TOTBAIXASPRAZO"]) : 0,
                                    dentroPrazoSLA = reader["SLA_PRAZO"] != DBNull.Value ? Convert.ToInt64(reader["SLA_PRAZO"]) : 0,
                                    foraPrazoSLA = reader["SLA_FPRAZO"] != DBNull.Value ? Convert.ToInt64(reader["SLA_FPRAZO"]) : 0,
                                    totalSLA = reader["SLA_TOTAL"] != DBNull.Value ? Convert.ToInt64(reader["SLA_TOTAL"]) : 0,
                                });
                            }
                        }
                    }

                    foreach (var item in leituraBanco)
                    {
                        var porcentagemProd = item.osprodutiva.Value != 0 ? ((item.osprodutiva * 100) / (float)item.osTotal) : 0;
                        var porcentagemRevisita = item.quantidadeRevisita != 0 ? ((item.quantidadeRevisita * 100) / (float)item.revisitaTotal) : 0;
                        var porcentagemBaixaPrazo = item.baixaProdutivaPrazo != 0 ? ((item.baixaProdutivaPrazo * 100) / (float)item.baixaPrazoTotal) : 0;
                        var porcentagemSLA = item.dentroPrazoSLA != 0 ? ((item.dentroPrazoSLA * 100) / (float)item.totalSLA) : 0;

                        item.osporcentagemProducao = porcentagemProd.Value.ToString("F2");
                        item.porcentagemRevisita = porcentagemRevisita.Value.ToString("F2");
                        item.porcentagemBaixaNoPrazo = porcentagemBaixaPrazo.Value.ToString("F2");
                        item.porcentagemSLA = porcentagemSLA.Value.ToString("F2");

                        Double total = 0;

                        if(configProdutiviade != null)
                        {
                            if (configProdutiviade.ProducaoPorcentSup != null)
                            {
                                if (porcentagemProd >= configProdutiviade.ProducaoPorcentSup.Value)
                                {
                                    item.osprecoProducao = configProdutiviade.MetaProdutivaSup.Value.ToString("F2");
                                    total = total + configProdutiviade.MetaProdutivaSup.Value;
                                }
                            }

                            if (configProdutiviade.RevisitaPorcentSup != null)
                            {
                                if (porcentagemRevisita >= configProdutiviade.RevisitaPorcentSup.Value)
                                {
                                    item.precoRevisita = configProdutiviade.MetaRevisitaSup.Value.ToString("F2");
                                    total = total + configProdutiviade.MetaRevisitaSup.Value;
                                }
                            }

                            if (configProdutiviade.BaixasPrazoPorcentSup != null)
                            {
                                if (porcentagemBaixaPrazo >= configProdutiviade.BaixasPrazoPorcentSup.Value)
                                {
                                    item.precoBaixaTEC = configProdutiviade.MetaBaixasPrazoSup.Value.ToString("F2");
                                    total = total + configProdutiviade.MetaBaixasPrazoSup.Value;
                                }
                            }

                            if (configProdutiviade.SLAPorcentSup != null)
                            {
                                if (porcentagemSLA >= configProdutiviade.SLAPorcentSup.Value)
                                {
                                    item.precoSLA = configProdutiviade.MetaSLASup.Value.ToString("F2");
                                    total = total + configProdutiviade.MetaSLASup.Value;
                                }
                            }
                        }

                        item.totalPrecoSomado = total.ToString("F2");

                        retorno.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retorno;
        }

        [HttpPost]
        [Route("api/PainelProdutividadeApi/GetAllMonitores")]
        public async Task<ICollection<ListaProdutividadeModels>> GetAllMonitores(ProdutividadeDataTableRequest request)
        {
            List<ListaProdutividadeModels> leituraBanco = new List<ListaProdutividadeModels>();
            List<ListaProdutividadeModels> retorno = new List<ListaProdutividadeModels>();

            try
            {
                using (var session = HibernateUtil.GetSession())
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                using (var repoConfigEmpresa = new Conexao<Configuracoes_Gerais>(session))
                using (var repoConfigProdutividade = new Conexao<PainelProdutividadeConfig>(session))
                {
                    Dictionary<string, object> parametros = new Dictionary<string, object>();
                    var horaPadrao = 0;
                    var dateInicial = Convert.ToDateTime(request.DataInicio).Date.ToString("yyyy-MM-dd");
                    var dateFim = Convert.ToDateTime(request.DataFim).Date.ToString("yyyy-MM-dd");
                    var tempoExecucaoPadrao = repoConfigEmpresa.selectFirst().Result.Tempo_Execucao_Padrao;

                    if (tempoExecucaoPadrao.HasValue)
                        horaPadrao = (int)tempoExecucaoPadrao.Value.TimeOfDay.TotalMinutes;

                    var configProdutiviade = await repoConfigProdutividade.selectFirst();

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT SENHA.NOME AS MONITOR, REG.NOME AS REGIAO, ");
                    sql.AppendLine(" SUM(CASE WHEN DOCP.STATUS_EXECUCAO = 2 THEN 1 ELSE 0 END) AS OSPROD, ");
                    sql.AppendLine(" SUM(CASE WHEN DOCP.STATUS_EXECUCAO = 1 THEN 1 ELSE 0 END) AS OSIMPRD, ");
                    sql.AppendLine(" COUNT(DOCP.EQUIPE) AS OSTOTAL, ");
                    sql.AppendLine(" COUNT(CASE WHEN DOCP.STATUS_EXECUCAO = 2 THEN REINC.NUM_OBRA END) AS REINCIDENCIASPROD, ");
                    sql.AppendLine(" COUNT(REINC.NUM_OBRA) AS REINCIDENCIASTOT, ");
                    sql.AppendLine(" SUM(CASE WHEN DOCP.STATUS_EXECUCAO = 2 AND (ROUND((AGEND.DATAHORA_FIM - AGEND.DATAHORA_INICIO) * 60)) <= COALESCE(((EXTRACT(HOUR FROM TSERV.HORAS)*60)+(EXTRACT(MINUTE FROM TSERV.HORAS))), @VALOR) THEN 1 ELSE 0 END) AS BAIXAPRAZOPROD, ");    // tec1 - produtivas no prazo 
                    sql.AppendLine(" SUM(CASE WHEN DOCP.STATUS_EXECUCAO = 1 AND (ROUND((AGEND.DATAHORA_FIM - AGEND.DATAHORA_INICIO) * 60)) <= COALESCE(((EXTRACT(HOUR FROM TSERV.HORAS)*60)+(EXTRACT(MINUTE FROM TSERV.HORAS))), @VALOR) THEN 1 ELSE 0 END) AS BAIXAPRAZOIMPROD, ");  // tec1 - produtivas no prazo 
                    sql.AppendLine(" COUNT(CASE WHEN (ROUND((AGEND.DATAHORA_FIM - AGEND.DATAHORA_INICIO) * 60)) <= COALESCE(((EXTRACT(HOUR FROM TSERV.HORAS)*60)+(EXTRACT(MINUTE FROM TSERV.HORAS))), @VALOR) THEN AGEND.DATAHORA_FIM END) AS TOTBAIXASPRAZO, ");                     // tec1 - produtivas no prazo 
                    sql.AppendLine(" SUM(CASE WHEN DOCP.STATUS_AUDITORIA = 1 AND DOCP.STATUS_EXECUCAO = 2 THEN 1 ELSE 0 END) AS AUD_APROVADA, "); //V.FECHADAS 
                    sql.AppendLine(" SUM(CASE WHEN DOCP.STATUS_AUDITORIA = 0 AND DOCP.STATUS_EXECUCAO = 2 THEN 1 ELSE 0 END) AS AUD_REPROVADO, "); //V.FECHADAS
                    sql.AppendLine(" SUM(CASE WHEN DOCP.STATUS_EXECUCAO = 2 AND DOCP.STATUS_AUDITORIA = 2 OR DOCP.STATUS_AUDITORIA IS NULL THEN 1 ELSE 0 END) AS AUD_NFEITAS, "); //V.ABERTAS
                    sql.AppendLine(" COUNT(CASE WHEN DOCP.STATUS_EXECUCAO = 2 AND DOCP.STATUS_AUDITORIA IS NULL OR DOCP.STATUS_AUDITORIA IN(0,1,2) THEN 1 ELSE 0 END) AS AUD_TOTAIS, "); //V.TOTAIS
                    sql.AppendLine(" COUNT(CASE WHEN (DOCP.DATA_INDICADOR + DOCP.HORA_INDICADOR) >= (DOCP.DATA_PROD + DOCP.HORA_PROD) THEN DOCP.DATA_INDICADOR END) AS SLA_PRAZO, "); // sla
                    sql.AppendLine(" COUNT(CASE WHEN (DOCP.DATA_INDICADOR + DOCP.HORA_INDICADOR) <= (DOCP.DATA_PROD + DOCP.HORA_PROD) THEN DOCP.DATA_INDICADOR END) AS SLA_FPRAZO, "); // sla
                    sql.AppendLine(" COUNT(DOCP.DATA_INDICADOR) AS SLA_TOTAL "); //sla
                    sql.AppendLine(" FROM TB_DOCUMENTO_PRODUCAO DOCP ");
                    sql.AppendLine(" LEFT JOIN TB_EQUIPE EQUI ");
                    sql.AppendLine(" ON EQUI.EQUIPE = DOCP.EQUIPE ");
                    sql.AppendLine(" LEFT JOIN TB_REGIAO REG ");
                    sql.AppendLine(" ON REG.COD_REGIAO  = EQUI.COD_REGIAO ");
                    sql.AppendLine(" LEFT JOIN TB_REINCIDENCIA_DOCPROD REINC ");
                    sql.AppendLine(" ON REINC.CODCT  = DOCP.CODCT AND REINC.PROJETO = DOCP.PROJETO AND REINC.NUM_OBRA = DOCP.NUM_OBRA AND REINC.NUM_DOC  = DOCP.NUM_DOC ");
                    sql.AppendLine(" LEFT JOIN TB_AGENDAMENTO_EQUIPEANIEL AGEND ON AGEND.CODCT  = DOCP.CODCT AND AGEND.PROJETO = DOCP.PROJETO AND AGEND.NUM_OBRA = DOCP.NUM_OBRA AND AGEND.NUM_DOC = DOCP.NUM_DOC ");
                    sql.AppendLine(" LEFT JOIN TB_TIPO_SERVICO_EQUIPE_IMOVEL TSERV ON TSERV.ID_TIPO_IMOVEL = AGEND.ID_TIPO_SERV ");
                    sql.AppendLine(" LEFT JOIN TB_SENHA SENHA ON SENHA.USUARIO = EQUI.USUARIO_MONITOR ");
                    sql.AppendLine(" LEFT JOIN TB_FUNCIONARIO FUNC ON EQUI.NUM_FRE = FUNC.NUM_FRE  ");
                    sql.AppendLine(" WHERE DOCP.STATUS_EXECUCAO IS NOT NULL AND FUNC.SITUACAO != 2 AND SENHA.NOME IS NOT NULL ");
                    parametros.Add("@VALOR", horaPadrao);

                    if (!string.IsNullOrEmpty(request.TipoData))
                    {
                        if (request.TipoData == "0")
                        {
                            sql.AppendLine(" AND DOCP.DIA BETWEEN @DATAINICIO AND @DATAFIM ");
                        }
                        else if (request.TipoData == "1")
                        {
                            sql.AppendLine(" AND DOCP.DATA_MAXIMA BETWEEN @DATAINICIO AND @DATAFIM ");
                        }
                        else
                        {
                            sql.AppendLine(" AND DOCP.DATA_PROD BETWEEN @DATAINICIO AND @DATAFIM ");
                        }

                        parametros.Add("@DATAINICIO", dateInicial);
                        parametros.Add("@DATAFIM", dateFim);
                    }

                    if (!string.IsNullOrEmpty(request.PessoaAgrupamento))
                    {
                        sql.AppendLine(" AND SENHA.USUARIO = @IDMONITOR ");
                        parametros.Add("@IDMONITOR", request.PessoaAgrupamento);
                    }

                    if (!string.IsNullOrEmpty(request.ContratoValor))
                    {
                        sql.AppendLine(" AND DOCP.CODCT = @IDCONTRATO ");
                        parametros.Add("@IDCONTRATO", request.ContratoValor);
                    }

                    if (!string.IsNullOrEmpty(request.ProjetoValor))
                    {
                        if (!string.IsNullOrEmpty(request.ContratoValor))
                        {
                            sql.AppendLine(" AND DOCP.PROJETO = @PROJETO");
                            parametros.Add("@PROJETO", request.ProjetoValor);
                        }
                    }

                    if (!string.IsNullOrEmpty(request.GrupoArea))
                    {
                        sql.AppendLine(" AND DOCP.COD_ESCRITORIO = @COD_ESCRITORIO ");
                        parametros.Add("@COD_ESCRITORIO", request.GrupoArea);
                    }

                    sql.AppendLine(" GROUP BY SENHA.NOME, REG.NOME ");
                    sql.AppendLine(" ORDER BY SENHA.NOME ASC ");


                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                leituraBanco.Add(new ListaProdutividadeModels()
                                {
                                    monitor = reader["MONITOR"] != DBNull.Value ? Convert.ToString(reader["MONITOR"]) : "-",
                                    grupoArea = reader["REGIAO"] != DBNull.Value ? Convert.ToString(reader["REGIAO"]) : "-",
                                    osprodutiva = reader["OSPROD"] != DBNull.Value ? Convert.ToInt64(reader["OSPROD"]) : 0,
                                    osimprodutiva = reader["OSIMPRD"] != DBNull.Value ? Convert.ToInt64(reader["OSIMPRD"]) : 0,
                                    osTotal = reader["OSTOTAL"] != DBNull.Value ? Convert.ToInt64(reader["OSTOTAL"]) : 0,
                                    quantidadeRevisita = reader["REINCIDENCIASPROD"] != DBNull.Value ? Convert.ToInt64(reader["REINCIDENCIASPROD"]) : 0,
                                    revisitaTotal = reader["REINCIDENCIASTOT"] != DBNull.Value ? Convert.ToInt64(reader["REINCIDENCIASTOT"]) : 0,
                                    baixaProdutivaPrazo = reader["BAIXAPRAZOPROD"] != DBNull.Value ? Convert.ToInt64(reader["BAIXAPRAZOPROD"]) : 0,
                                    baixaImprodutivaPrazo = reader["BAIXAPRAZOIMPROD"] != DBNull.Value ? Convert.ToInt64(reader["BAIXAPRAZOIMPROD"]) : 0,
                                    baixaPrazoTotal = reader["TOTBAIXASPRAZO"] != DBNull.Value ? Convert.ToInt64(reader["TOTBAIXASPRAZO"]) : 0,
                                    vistoriaAprovada = reader["AUD_APROVADA"] != DBNull.Value ? Convert.ToInt64(reader["AUD_APROVADA"]) : 0,
                                    vistoriaReprovada = reader["AUD_REPROVADO"] != DBNull.Value ? Convert.ToInt64(reader["AUD_REPROVADO"]) : 0,
                                    vistoriaNaoFeitas = reader["AUD_NFEITAS"] != DBNull.Value ? Convert.ToInt64(reader["AUD_NFEITAS"]) : 0,
                                    vistoriaTotal = reader["AUD_TOTAIS"] != DBNull.Value ? Convert.ToInt64(reader["AUD_TOTAIS"]) : 0,
                                    dentroPrazoSLA = reader["SLA_PRAZO"] != DBNull.Value ? Convert.ToInt64(reader["SLA_PRAZO"]) : 0,
                                    foraPrazoSLA = reader["SLA_FPRAZO"] != DBNull.Value ? Convert.ToInt64(reader["SLA_FPRAZO"]) : 0,
                                    totalSLA = reader["SLA_TOTAL"] != DBNull.Value ? Convert.ToInt64(reader["SLA_TOTAL"]) : 0,
                                });
                            }
                        }
                    }

                    foreach (var item in leituraBanco)
                    {

                        var vistoriaFechadas = item.vistoriaAprovada + item.vistoriaReprovada;
                        var porcentagemProd = item.osprodutiva.Value != 0 ? ((item.osprodutiva * 100) / (float)item.osTotal) : 0;
                        var porcentagemRevisita = item.quantidadeRevisita != 0 ? ((item.quantidadeRevisita * 100) / (float)item.revisitaTotal) : 0;
                        var porcentagemBaixaPrazo = item.baixaProdutivaPrazo != 0 ? ((item.baixaProdutivaPrazo * 100) / (float)item.baixaPrazoTotal) : 0;
                        var porcentegemVistoria = item.vistoriaAprovada != 0 ? ((item.vistoriaAprovada * 100) / (float)item.vistoriaTotal) : 0;
                        var porcentagemSLA = item.dentroPrazoSLA != 0 ? ((item.dentroPrazoSLA * 100) / (float)item.totalSLA) : 0;
                        //var porcentagem RTI

                        item.osporcentagemProducao = porcentagemProd.Value.ToString("F2");
                        item.porcentagemRevisita = porcentagemRevisita.Value.ToString("F2");
                        item.porcentagemBaixaNoPrazo = porcentagemBaixaPrazo.Value.ToString("F2");
                        item.porcentagemVistoria = porcentegemVistoria.Value.ToString("F2");
                        item.porcentagemSLA = porcentagemSLA.Value.ToString("F2");
                        item.vistoriasFechadas = vistoriaFechadas;
                        Double total = 0;

                        if(configProdutiviade != null)
                        {
                            if (configProdutiviade.ProducaoPorcentMon != null)
                            {
                                if (porcentagemProd >= configProdutiviade.ProducaoPorcentMon.Value)
                                {
                                    item.osprecoProducao = configProdutiviade.MetaProdutivaMon.Value.ToString("F2");
                                    total = total + configProdutiviade.MetaProdutivaMon.Value;
                                }
                            }

                            if (configProdutiviade.RevisitaPorcentMon != null)
                            {
                                if (porcentagemRevisita >= configProdutiviade.RevisitaPorcentMon.Value)
                                {
                                    item.precoRevisita = configProdutiviade.RevisitaPorcentMon.Value.ToString("F2");
                                    total = total + configProdutiviade.RevisitaPorcentMon.Value;
                                }
                            }


                            if (configProdutiviade.VistoriaPorcentMon != null)
                            {
                                if (porcentegemVistoria >= configProdutiviade.VistoriaPorcentMon.Value)
                                {
                                    item.precoVistoria = configProdutiviade.MetaVistoriaMon.Value.ToString("F2");
                                    total = total + configProdutiviade.MetaVistoriaMon.Value;
                                }
                            }

                            if (configProdutiviade.TECPorcentMon != null)
                            {
                                if (porcentagemBaixaPrazo >= configProdutiviade.TECPorcentMon.Value)
                                {
                                    item.precoBaixaTEC = configProdutiviade.MetaTECMon.Value.ToString("F2");
                                    total = total + configProdutiviade.MetaTECMon.Value;
                                }
                            }

                            if (configProdutiviade.SLAPorcentMon != null)
                            {
                                if (porcentagemSLA >= configProdutiviade.SLAPorcentMon.Value)
                                {
                                    item.precoSLA = configProdutiviade.MetaSLAMon.Value.ToString("F2");
                                    total = total + configProdutiviade.MetaSLAMon.Value;
                                }
                            }
                        }
                     
                        item.totalPrecoSomado = total.ToString("F2");

                        retorno.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retorno;
        }


        [HttpPost]
        [Route("api/PainelProdutividadeApi/GravarConfiguracoes")]
        public async Task<PostApiRetorno<bool>> GravarConfiguracoes(PainelProdutividadeConfigModels request)
        {
            PostApiRetorno<bool> retorno = new PostApiRetorno<bool>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var painelProdConfig = new Conexao<PainelProdutividadeConfig>(session))
                {                
                    PainelProdutividadeConfig config = new PainelProdutividadeConfig
                    {
                        ProducaoPorcentTec  = Convert.ToInt64(request.ProducaoPorcentTec),
                        MetaProdutivaTec    = Convert.ToDouble(request.MetaProdutivaTec),
                        RevisitaPorcentTec  = Convert.ToInt64(request.RevisitaPorcentTec),
                        MetaRevisitaTec     = Convert.ToDouble(request.MetaRevisitaTec),
                        BaixasPrazoPorcentTec = Convert.ToInt64(request.BaixasPrazoPorcentTec),
                        MetaBaixasPrazoTec  = Convert.ToDouble(request.MetaBaixasPrazoTec),
                        VistoriaPorcentTec  = Convert.ToInt64(request.VistoriaPorcentTec),
                        MetaVistoriaTec     = Convert.ToDouble(request.MetaVistoriaTec),
                        SLAPorcentTec       = Convert.ToInt64(request.SLAPorcentTec),
                        MetaSLATec          = Convert.ToDouble(request.MetaSLATec),
                        //supervisor
                        ProducaoPorcentSup  = Convert.ToInt64(request.ProducaoPorcentSup),
                        MetaProdutivaSup    = Convert.ToDouble(request.MetaProdutivaSup),
                        RevisitaPorcentSup  = Convert.ToInt64(request.RevisitaPorcentSup),
                        MetaRevisitaSup      = Convert.ToDouble(request.MetaRevisitaSup),
                        BaixasPrazoPorcentSup = Convert.ToInt64(request.BaixasPrazoPorcentSup),
                        MetaBaixasPrazoSup  = Convert.ToDouble(request.MetaBaixasPrazoSup),
                        QualidadePorcentSup = Convert.ToInt64(request.QualidadePorcentSup),
                        MetaQualidadeSup    = Convert.ToDouble(request.MetaQualidadeSup),
                        SLAPorcentSup       = Convert.ToInt64(request.SLAPorcentSup),
                        MetaSLASup          = Convert.ToDouble(request.MetaSLASup),
                        //monitor
                        ProducaoPorcentMon  = Convert.ToInt64(request.ProducaoPorcentMon),
                        MetaProdutivaMon    = Convert.ToDouble(request.MetaProdutivaMon),
                        RevisitaPorcentMon  = Convert.ToInt64(request.RevisitaPorcentMon),
                        MetaRevisitaMon     = Convert.ToDouble(request.MetaRevisitaMon),                        
                        RTIPorcentMon       = Convert.ToInt64(request.RTIPorcentMon),
                        MetaRTIMon          = Convert.ToDouble(request.MetaRTIMon),
                        VistoriaPorcentMon  = Convert.ToInt64(request.VistoriaPorcentMon),
                        MetaVistoriaMon     = Convert.ToDouble(request.MetaVistoriaMon),
                        TECPorcentMon       = Convert.ToInt64(request.TECPorcentMon),                              
                        MetaTECMon          = Convert.ToDouble(request.MetaTECMon),
                        SLAPorcentMon       = Convert.ToInt64(request.SLAPorcentMon),
                        MetaSLAMon          = Convert.ToDouble(request.MetaSLAMon)
                    };

                    var hasConfig = await painelProdConfig.selectFirst(x => x.Id == Convert.ToInt64(request.Id));

                    if (hasConfig != null)
                    {
                        config.Id = 1;
                    }

                    await painelProdConfig.SaveOrUpdate(config);

                    retorno.Ok = true;
                    retorno.Mensagem = "Configurações Salvas com Sucesso!";
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = $"Não foi possível salvar as Configurações de Produtividade: Inner {ex.InnerException}";
                return retorno;
            }

            return retorno;

        }

        [HttpGet]
        [Route("api/PainelProdutividadeApi/GetConfiguracoes")]
        public async Task<PostApiRetorno<PainelProdutividadeConfig>> GetConfiguracoes()
        {
            PostApiRetorno<PainelProdutividadeConfig> retorno = new PostApiRetorno<PainelProdutividadeConfig>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var painelProdConfig = new Conexao<PainelProdutividadeConfig>(session))
                {
                    var configuracoes = await painelProdConfig.selectFirst();
                    retorno.IdObjeto = configuracoes;
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = $"Não foi possível obter as Configurações de Produtividade: Inner {ex.InnerException}";
                return retorno;
            }

            return retorno;
        }
    }
}