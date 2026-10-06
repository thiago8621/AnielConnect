using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.Core;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class BancoHorasApiController : BaseApiController
    {
        /// <summary>
        /// Relatório Sintético Saldo Banco de Horas
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/BancoHoras/RelatorioSinteticoSaldoBancoHoras")]
        public async Task<ApiResponseObj<List<Sintetico_Saldo_Banco_HorasListaModel>>> RelatorioSinteticoSaldoBancoHoras(RelatorioSinteticoRequest request)
        {
            List<Sintetico_Saldo_Banco_HorasListaModel> retorno = new List<Sintetico_Saldo_Banco_HorasListaModel>();

            Dictionary<string, object> parametros = new Dictionary<string, object>();
            DateTime? inicioBanco = null;

            try
            {
                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                var data = Convert.ToDateTime(request.Data);
                parametros.Add("@data", data.Date);

                using (ISession session = HibernateUtil.GetSessionT5())
                using (ISession sessionHE = HibernateUtil.GetSessionT5())
                using (ISession sessionNH = HibernateUtil.GetSessionT5())
                using (var repoFechaBanco = new Conexao<Tb_Pt_Fechamento_Periodo_Banco_Hora>(sessionNH))
                using (var com = session.Connection.CreateCommand())
                {

                    inicioBanco = (await repoFechaBanco.select()).Min(p => p.Data_Inicio);

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.NUM_FRE, A.GESTOR_PONTO, A.NOME, A.NUM_FRE_AUX AS MATRICULA_AUXILIAR,A.COD_CCUSTO,");
                    sql.AppendLine(" B.ID AS ID_EMPRESA, B.EMPRESA AS EMPRESA, C.NOME AS FILIAL, C.CODFILIAL,");
                    sql.AppendLine(" C.CODEMPRESA, D.ID AS CENTRO,D.DESCRICAO AS DESC_CENTROCUSTO, I.NOME AS NOME_GESTOR, A.ID_JORNADA, ");
                    sql.AppendLine(" A.COD_DEPTO AS DEPARTAMENTO, G.DATA AS DIA_FUNCIONARIO, H.SIGLA AS JORNADA ");

                    sql.AppendLine(" FROM TB_PT_BD_HORAS G ");

                    sql.AppendLine(" INNER JOIN TB_FUNCIONARIO A ");
                    sql.AppendLine(" ON(G.NUM_FRE = A.NUM_FRE) ");

                    sql.AppendLine(" LEFT JOIN TB_CENTRO_CUSTO D  ");
                    sql.AppendLine(" ON(D.COD_CCUSTO = A.COD_CCUSTO) ");

                    sql.AppendLine(" LEFT JOIN TB_EMPRESA B ");
                    sql.AppendLine(" ON(B.CODEMPRESA = A.CODEMPRESA) ");

                    sql.AppendLine(" LEFT JOIN TB_FILIAL C ");
                    sql.AppendLine(" ON(C.CODEMPRESA = A.CODEMPRESA AND C.CODFILIAL = A.CODFILIAL) ");

                    sql.AppendLine(" LEFT JOIN TB_FOLHA_DEPTO F ");
                    sql.AppendLine(" ON(F.COD_DEPTO = A.COD_DEPTO) ");

                    sql.AppendLine(" LEFT JOIN TB_PT_JORNADA_FUNCIONARIO H ");
                    sql.AppendLine(" ON(H.ID = A.ID_JORNADA) ");

                    sql.AppendLine(" LEFT JOIN TB_FUNCIONARIO I ");
                    sql.AppendLine(" ON(I.NUM_FRE = A.GESTOR_PONTO) ");

                    sql.AppendLine(" WHERE G.DATA  =  @DATA ");

                    if (request.Id_Empresa.HasValue)
                    {
                        sql.AppendLine("   AND A.CODEMPRESA = @EMPRESA ");
                        parametros.Add("@EMPRESA", request.Id_Empresa);
                    }
                    if (request.Id_Filial.HasValue)
                    {
                        sql.AppendLine("   AND A.CODFILIAL = @FILIAL ");
                        parametros.Add("@FILIAL", request.Id_Filial);
                    }
                    if (request.Id_Departamento.HasValue)
                    {
                        sql.AppendLine("   AND A.COD_DEPTO = @DEPARTAMENTO ");
                        parametros.Add("@DEPARTAMENTO", request.Id_Departamento);
                    }
                    if (request.Id_Jornada.HasValue)
                    {
                        sql.AppendLine("   AND A.ID_JORNADA = @JORNADA ");
                        parametros.Add("@JORNADA", request.Id_Jornada);
                    }
                    if (request.Id_Centro_Custo.HasValue)
                    {
                        sql.AppendLine("   AND D.COD_CCUSTO = @CENTRO ");
                        parametros.Add("@CENTRO", request.Id_Centro_Custo);
                    }
                    if (!String.IsNullOrEmpty(request.Id_Gestor))
                    {
                        sql.AppendLine("   AND A.GESTOR_PONTO = @ID_GESTOR ");
                        parametros.Add("@ID_GESTOR", request.Id_Gestor);
                    }
                    if (!String.IsNullOrEmpty(request.Id_Funcionario))
                    {
                        sql.AppendLine("   AND A.NUM_FRE = @NUM_FRE ");
                        parametros.Add("@NUM_FRE", request.Id_Funcionario);
                    }

                    if (!String.IsNullOrEmpty(request.Cod_Funcao))
                    {
                        sql.AppendLine("   AND A.COD_FUNCAO = @COD_FUNCAO ");
                        parametros.Add("@COD_FUNCAO", request.Cod_Funcao);
                    }

                    sql.AppendLine(" GROUP BY G.DATA, A.NUM_FRE, A.NOME, A.NUM_FRE_AUX, A.COD_CCUSTO, B.ID, B.EMPRESA, C.NOME,C.CODFILIAL, ");
                    sql.AppendLine("          A.COD_CCUSTO, I.NOME, A.ID_JORNADA, A.GESTOR_PONTO, ");
                    sql.AppendLine("          A.COD_DEPTO, H.SIGLA, ");
                    sql.AppendLine("          C.CODEMPRESA, D.ID, D.DESCRICAO ");

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            Sintetico_Saldo_Banco_HorasListaModel objRetorno = new Sintetico_Saldo_Banco_HorasListaModel();

                            objRetorno.Nome_Funcionario = Convert.ToString(reader["NOME"]);
                            objRetorno.Id_Funcionario = Convert.ToString(reader["NUM_FRE"]);
                            objRetorno.Matricula_Auxiliar = !String.IsNullOrEmpty(reader["MATRICULA_AUXILIAR"].ToString()) ? Convert.ToString(reader["MATRICULA_AUXILIAR"]) : "";
                            objRetorno.Id_Empresa = reader["CODEMPRESA"] != DBNull.Value ? Convert.ToInt64(reader["CODEMPRESA"]) : (long?)null;
                            objRetorno.Empresa = !String.IsNullOrEmpty(reader["EMPRESA"].ToString()) ? Convert.ToString(reader["EMPRESA"]) : "";
                            objRetorno.Id_Filial = reader["CODFILIAL"] != DBNull.Value ? Convert.ToInt64(reader["CODFILIAL"]) : (long?)null;
                            objRetorno.Filial = !String.IsNullOrEmpty(reader["FILIAL"].ToString()) ? Convert.ToString(reader["FILIAL"]) : "";
                            objRetorno.Id_Centro_Custo = reader["COD_CCUSTO"] != DBNull.Value ? Convert.ToInt64(reader["COD_CCUSTO"]) : (long?)null;
                            objRetorno.Gestor = reader["NOME_GESTOR"] != DBNull.Value ? Convert.ToString(reader["NOME_GESTOR"]) : "";
                            objRetorno.Cod_Jornada = !String.IsNullOrEmpty(reader["JORNADA"].ToString()) ? Convert.ToString(reader["JORNADA"]) : "";
                            objRetorno.Descricao_Ct_Custo = !String.IsNullOrEmpty(reader["DESC_CENTROCUSTO"].ToString()) ? Convert.ToString(reader["DESC_CENTROCUSTO"]) : "";
                            objRetorno.Id_Departamento = reader["DEPARTAMENTO"] != DBNull.Value ? Convert.ToInt64(reader["DEPARTAMENTO"]) : (long?)null;
                            objRetorno.Id_Jornada = reader["ID_JORNADA"] != DBNull.Value ? Convert.ToInt64(reader["ID_JORNADA"]) : (long?)null;
                            objRetorno.Id_Gestor = reader["GESTOR_PONTO"] != DBNull.Value ? Convert.ToString(reader["GESTOR_PONTO"]) : "";
                            objRetorno.Data = reader["DIA_FUNCIONARIO"] != DBNull.Value ? Convert.ToString(reader["DIA_FUNCIONARIO"]) : "";

                            if (inicioBanco.HasValue)
                            {
                                var horasPeriodo = await BancoHoras.GetHorasExtra(sessionHE, objRetorno.Id_Funcionario, inicioBanco.Value, data, true);

                                objRetorno.Horas_Saldo_Credito = horasPeriodo.Horas_Extras.Horas.Sum(p => p.Value);
                                objRetorno.Horas_Saldo_Debito = horasPeriodo.Horas_Debito;

                                objRetorno.Saldo_Credito = StringService.HoraParaString(objRetorno.Horas_Saldo_Credito) ?? "00:00";
                                objRetorno.Saldo_Debito = StringService.HoraParaString(objRetorno.Horas_Saldo_Debito) ?? "00:00";
                            }
                            else
                            {
                                objRetorno.Horas_Saldo_Credito = 0;
                                objRetorno.Horas_Saldo_Debito = 0;

                                objRetorno.Saldo_Credito = "00:00";
                                objRetorno.Saldo_Debito = "00:00";
                            }

                            double saldo_banco = await BancoHoras.GetTotalHorasBancoSql(new HorasBancoParams
                            {
                                session = sessionHE,
                                dataFechto = data,
                                numFre = objRetorno.Id_Funcionario
                            });

                            objRetorno.Horas_Saldo_Final_Banco = DateTimeUtil.RemoveSeconds(saldo_banco);
                            objRetorno.Saldo_Final_Banco = StringService.HoraParaString(objRetorno.Horas_Saldo_Final_Banco) ?? "00:00";

                            if (objRetorno.Horas_Saldo_Final_Banco != 0)
                            {
                                retorno.Add(objRetorno);
                            }
                        }
                    }
                }

                return new ApiResponseObj<List<Sintetico_Saldo_Banco_HorasListaModel>>
                {
                    data = retorno,
                    ok = true
                };
            }
            catch (Exception e)
            {
                return new ApiResponseObj<List<Sintetico_Saldo_Banco_HorasListaModel>>
                {
                    error = e.Message,
                    ok = false
                };
            }
        }

        /// <summary>
        /// Relatório Sintético de Horas Efetivas
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/BancoHoras/RelatorioSinteticoHorasEfetivas")]
        public async Task<ApiResponseObj<List<Horas_EfetivasListModel>>> RelatorioSinteticoHorasEfetivas(HorasEfetivasRequest request)
        {
            List<Horas_EfetivasListModel> retorno = new List<Horas_EfetivasListModel>();

            try
            {
                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }

                DateTime data_inicio = DateTime.Parse(request.Data_Inicio);
                DateTime data_fim = DateTime.Parse(request.Data_Fim);

                using (ISession session = HibernateUtil.GetSessionT5())
                using (ISession sessionHorasDia = HibernateUtil.GetSessionT5())
                using (var com = session.Connection.CreateCommand())
                {
                    Dictionary<string, object> parametros = new Dictionary<string, object>();

                    #region SQL
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT NUM_FRE, NUM_FRE_AUX, NOME, EMPRESA, FILIAL, CENTRO_CUSTO, GESTOR, ");
                    sql.AppendLine("  COALESCE(SUM(HORAS_PREVISTAS), 0) AS HORAS_PREVISTAS, ");
                    sql.AppendLine(" COALESCE(SUM(HR_ADIC_NOTURNO), 0) AS HR_ADIC_NOTURNO, ");
                    sql.AppendLine("  COALESCE(SUM(ATRASOS), 0) AS ATRASOS, COALESCE(SUM(FALTAS), 0) AS FALTAS ");
                    sql.AppendLine(" FROM( ");
                    sql.AppendLine("  SELECT A.NUM_FRE, B.NOME, B.NUM_FRE_AUX, D.EMPRESA, C.NOME AS FILIAL, ");
                    sql.AppendLine("  (B.COD_CCUSTO || ' - ' || E.DESCRICAO) AS CENTRO_CUSTO, ");
                    sql.AppendLine("  J.DESCRICAO, J.UF, F.NOME AS GESTOR, ");
                    sql.AppendLine("  ((CASE WHEN G.TIPO_JORNADA = 2 THEN ");

                    sql.AppendLine("   CASE ");
                    sql.AppendLine("   WHEN EXTRACT(WEEKDAY FROM A.DATA) = 0 THEN CAST(((CASE ");
                    sql.AppendLine("  WHEN G.TERMINO_DOMINGO < G.INICIO_DOMINGO THEN DATEADD(DAY, 1, G.TERMINO_DOMINGO) ");
                    sql.AppendLine("  ELSE G.TERMINO_DOMINGO ");
                    sql.AppendLine(" END) - G.INICIO_DOMINGO) AS DOUBLE PRECISION) *24 ");
                    sql.AppendLine(" WHEN EXTRACT(WEEKDAY FROM A.DATA) = 1 THEN CAST(((CASE ");
                    sql.AppendLine("     WHEN G.TERMINO_SEGUNDA<G.INICIO_SEGUNDA THEN DATEADD(DAY, 1, G.TERMINO_SEGUNDA)  ");
                    sql.AppendLine("    ELSE G.TERMINO_SEGUNDA ");
                    sql.AppendLine(" END) -G.INICIO_SEGUNDA) AS DOUBLE PRECISION) *24 ");
                    sql.AppendLine("  WHEN EXTRACT(WEEKDAY FROM A.DATA) = 2 THEN CAST(((CASE ");
                    sql.AppendLine("  WHEN G.TERMINO_TERCA<G.INICIO_TERCA THEN DATEADD(DAY, 1, G.TERMINO_TERCA)  ");
                    sql.AppendLine("  ELSE G.TERMINO_TERCA ");
                    sql.AppendLine("   END) -G.INICIO_TERCA) AS DOUBLE PRECISION) *24 ");
                    sql.AppendLine("  WHEN EXTRACT(WEEKDAY FROM A.DATA) = 3 THEN CAST(((CASE ");
                    sql.AppendLine("     WHEN G.TERMINO_QUARTA<G.INICIO_QUARTA THEN DATEADD(DAY, 1, G.TERMINO_QUARTA)  ");
                    sql.AppendLine("     ELSE G.TERMINO_QUARTA ");
                    sql.AppendLine("  END) -G.INICIO_QUARTA) AS DOUBLE PRECISION) *24 ");
                    sql.AppendLine(" WHEN EXTRACT(WEEKDAY FROM A.DATA) = 4 THEN CAST(((CASE ");
                    sql.AppendLine("  WHEN G.TERMINO_QUINTA<G.INICIO_QUINTA THEN DATEADD(DAY, 1, G.TERMINO_QUINTA)  ");
                    sql.AppendLine("  ELSE G.TERMINO_QUINTA ");
                    sql.AppendLine("  END) -G.INICIO_QUINTA) AS DOUBLE PRECISION) *24 ");
                    sql.AppendLine(" WHEN EXTRACT(WEEKDAY FROM A.DATA) = 5 THEN CAST(((CASE ");
                    sql.AppendLine("  WHEN G.TERMINO_SEXTA<G.INICIO_SEXTA THEN DATEADD(DAY, 1, G.TERMINO_SEXTA)  ");
                    sql.AppendLine("  ELSE G.TERMINO_SEXTA ");
                    sql.AppendLine(" END) -G.INICIO_SEXTA) AS DOUBLE PRECISION) *24 ");
                    sql.AppendLine("  WHEN EXTRACT(WEEKDAY FROM A.DATA) = 6 THEN CAST(((CASE ");
                    sql.AppendLine("   WHEN G.TERMINO_SABADO<G.INICIO_SABADO THEN DATEADD(DAY, 1, G.TERMINO_SABADO)  ");
                    sql.AppendLine("  ELSE G.TERMINO_SABADO ");
                    sql.AppendLine("  END) -G.INICIO_SABADO) AS DOUBLE PRECISION) *24 ");
                    sql.AppendLine("  END ");
                    sql.AppendLine("  END) - ");
                    sql.AppendLine(" (CASE WHEN G.INTERVALO = 1 THEN ");
                    sql.AppendLine(" CASE WHEN G.TIPO_INTERVALO = 2 THEN ");
                    sql.AppendLine("  CASE WHEN G.TIPO_JORNADA = 1 THEN 0 ");
                    sql.AppendLine(" END ");
                    sql.AppendLine(" END ");
                    sql.AppendLine(" ELSE 0 END)) AS HORAS_PREVISTAS, ");
                    sql.AppendLine(" (SELECT SUM(COALESCE(Z.HORAS, 0)) FROM TB_PT_BD_HORAS Z ");
                    sql.AppendLine("  WHERE Z.ID_DIA_FUNCIONARIO = A.ID AND Z.TRANSACAO = 'C' ");
                    sql.AppendLine("  AND Z.TIPO_HORA = 'N' AND Z.HE_VINCULADA IS NULL) AS HR_ADIC_NOTURNO, ");
                    sql.AppendLine(" (SELECT SUM(COALESCE(Z.HORAS, 0)) ");
                    sql.AppendLine(" FROM TB_PT_BD_HORAS Z ");
                    sql.AppendLine(" INNER JOIN TB_PT_DIA_FUNCIONARIO Y ");
                    sql.AppendLine("  ON(Y.ID = Z.ID_DIA_FUNCIONARIO) ");
                    sql.AppendLine(" WHERE Z.ID_DIA_FUNCIONARIO = A.ID ");
                    sql.AppendLine("  AND Z.TRANSACAO = 'D' ");
                    sql.AppendLine(" AND Z.TIPO_MOVTO <> 'A' ");
                    sql.AppendLine("  AND Z.TIPO_HORA = 'J' ");
                    sql.AppendLine("  AND EXISTS(SELECT FIRST 1 X.ID ");
                    sql.AppendLine("  FROM TB_PT_MARCACAO X ");
                    sql.AppendLine("  WHERE X.ID_DIA_FUNCIONARIO = Y.ID)) AS ATRASOS, ");
                    sql.AppendLine(" (SELECT SUM(COALESCE(Z.HORAS, 0)) ");
                    sql.AppendLine(" FROM TB_PT_BD_HORAS Z ");
                    sql.AppendLine(" INNER JOIN TB_PT_DIA_FUNCIONARIO Y ");
                    sql.AppendLine(" ON(Y.ID = Z.ID_DIA_FUNCIONARIO) ");
                    sql.AppendLine("  WHERE Z.ID_DIA_FUNCIONARIO = A.ID ");
                    sql.AppendLine("  AND Z.TRANSACAO = 'D' ");
                    sql.AppendLine("  AND Z.TIPO_MOVTO <> 'A' ");
                    sql.AppendLine(" AND Z.TIPO_HORA = 'J' ");
                    sql.AppendLine(" AND NOT EXISTS(SELECT FIRST 1 X.ID ");
                    sql.AppendLine(" FROM TB_PT_MARCACAO X ");
                    sql.AppendLine(" WHERE X.ID_DIA_FUNCIONARIO = Y.ID)) AS FALTAS ");
                    sql.AppendLine(" FROM TB_PT_DIA_FUNCIONARIO A ");
                    sql.AppendLine(" INNER JOIN TB_FUNCIONARIO B ON(A.NUM_FRE = B.NUM_FRE) ");
                    sql.AppendLine(" LEFT JOIN TB_FILIAL C ON(B.CODEMPRESA = C.CODEMPRESA AND B.CODFILIAL = C.CODFILIAL) ");
                    sql.AppendLine(" LEFT JOIN TB_EMPRESA D ON(B.CODEMPRESA = D.CODEMPRESA) ");
                    sql.AppendLine(" LEFT JOIN TB_CENTRO_CUSTO E ON(B.COD_CCUSTO = E.COD_CCUSTO) ");
                    sql.AppendLine(" LEFT JOIN TB_FUNCIONARIO F ON(B.GESTOR_PONTO = F.NUM_FRE) ");
                    sql.AppendLine(" LEFT JOIN TB_PT_JORNADA_FUNCIONARIO G ON(A.ID_JORNADA = G.ID) ");
                    sql.AppendLine(" LEFT JOIN TB_PT_CIDADE J ON(J.ID = B.ID_MUNICIPIO_TRABALHO) ");
                    sql.AppendLine(" LEFT JOIN TB_PT_CLASSIFICACAO_ESCALA I ON(A.ID_EVENTO = I.ID) ");

                    sql.AppendLine("     WHERE CAST(A.DATA AS DATE) BETWEEN @DATA_INI AND @DATA_FIM ");

                    if (request.Id_Empresa.HasValue)
                    {
                        sql.AppendLine(" AND B.CODEMPRESA = @ID_EMPRESA ");
                        parametros.Add("@ID_EMPRESA", request.Id_Empresa.Value);

                        if (request.Id_Filial.HasValue)
                        {
                            sql.AppendLine(" AND B.CODFILIAL = @ID_FILIAL ");
                            parametros.Add("@ID_FILIAL", request.Id_Filial.Value);
                        }
                    }

                    if (request.Id_Departamento.HasValue)
                    {
                        sql.AppendLine(" AND B.COD_DEPTO = @ID_DEPARTAMENTO ");
                        parametros.Add("@ID_DEPARTAMENTO", request.Id_Departamento.Value);
                    }

                    if (!String.IsNullOrEmpty(request.Num_Fre_Gestor))
                    {
                        sql.AppendLine(" AND B.GESTOR_PONTO = @NUM_FRE_GESTOR ");
                        parametros.Add("@NUM_FRE_GESTOR", request.Num_Fre_Gestor);
                    }

                    if (!String.IsNullOrEmpty(request.Num_Fre))
                    {
                        sql.AppendLine(" AND B.NUM_FRE = @NUM_FRE ");
                        parametros.Add("@NUM_FRE", request.Num_Fre);
                    }

                    if (request.Id_Jornada.HasValue)
                    {
                        sql.AppendLine(" AND A.ID_JORNADA = @ID_JORNADA ");
                        parametros.Add("@ID_JORNADA", request.Id_Jornada);
                    }

                    if (request.Id_Centro_Custo.HasValue)
                    {
                        sql.AppendLine(" AND B.COD_CCUSTO = @ID_CENTRO_CUSTO ");
                        parametros.Add("@ID_CENTRO_CUSTO", request.Id_Centro_Custo.Value);
                    }

                    if (!String.IsNullOrEmpty(request.Cod_Funcao))
                    {
                        sql.AppendLine(" AND B.COD_FUNCAO = @COD_FUNCAO ");
                        parametros.Add("@COD_FUNCAO", request.Cod_Funcao);
                    }

                    sql.AppendLine(" ) ");
                    sql.AppendLine(" GROUP BY NUM_FRE, NUM_FRE_AUX, NOME, EMPRESA, FILIAL, CENTRO_CUSTO, GESTOR ");


                    parametros.Add("@DATA_INI", Convert.ToDateTime(data_inicio.ToString("dd/MM/yyyy 00:00")));
                    parametros.Add("@DATA_FIM", Convert.ToDateTime(data_fim.ToString("dd/MM/yyyy 23:59")));

                    #endregion

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            var horasEfetivas = new Horas_EfetivasListModel
                            {
                                Num_Fre = Convert.ToString(reader["NUM_FRE"]),
                                Matricula_Auxiliar = Convert.ToString(reader["NUM_FRE_AUX"]),

                                Nome = Convert.ToString(reader["NOME"]),
                                Empresa = Convert.ToString(reader["EMPRESA"]),
                                Filial = Convert.ToString(reader["FILIAL"]),

                                Centro_Custo = reader["CENTRO_CUSTO"] != DBNull.Value ? Convert.ToString(reader["CENTRO_CUSTO"]) : String.Empty,
                                Gestor = reader["GESTOR"] != DBNull.Value ? Convert.ToString(reader["GESTOR"]) : String.Empty
                            };

                            var horas = await BancoHoras.GetHorasExtra(sessionHorasDia, horasEfetivas.Num_Fre, data_inicio, data_fim, false);

                            horasEfetivas.Horas_Sobreaviso = StringService.HoraParaString(horas.Horas_Sobreaviso) ?? "00:00";

                            double adicional_noturno = reader["HR_ADIC_NOTURNO"] != DBNull.Value ? Convert.ToDouble(reader["HR_ADIC_NOTURNO"]) : 0;
                            horasEfetivas.Horas_Adic_Noturno = StringService.HoraParaString(adicional_noturno) ?? "00:00";

                            horasEfetivas.Horas_Extras_Adic_Noturno = StringService.HoraParaString(horas.Horas_Adic_Noturno.Horas.Sum(p => p.Value)) ?? "00:00";

                            horasEfetivas.Horas_Abonadas = StringService.HoraParaString(horas.Horas_Abonadas) ?? "00:00";

                            double horas_previstas = reader["HORAS_PREVISTAS"] != DBNull.Value ? Convert.ToDouble(reader["HORAS_PREVISTAS"]) : 0;

                            if (data_fim >= DateTime.Now)
                                horas_previstas += await HorasEfetivas.HorasPrevistasDia(sessionHorasDia, data_fim, horasEfetivas.Num_Fre);

                            horasEfetivas.Horas_Previstas = StringService.HoraParaString(horas_previstas) ?? "00:00";

                            double atrasos = reader["ATRASOS"] != DBNull.Value ? Convert.ToDouble(reader["ATRASOS"]) : 0;
                            horasEfetivas.Atrasos = StringService.HoraParaString(atrasos) ?? "00:00";

                            double faltas = reader["FALTAS"] != DBNull.Value ? Convert.ToDouble(reader["FALTAS"]) : 0;
                            horasEfetivas.Faltas = StringService.HoraParaString(faltas) ?? "00:00";

                            horasEfetivas.Horas_Trabalhadas = (HorasEfetivas.GetHorasTrabalhadas(sessionHorasDia, new GetHorasTrabalhadasModel
                            {
                                Data_Inicio = data_inicio,
                                Data_Fim = data_fim,

                                Num_Fre = horasEfetivas.Num_Fre,
                                Tem_Intervalo = false,
                                VisualizarInconsistentes = false,
                                Totalizador = true
                            }).Result).horasTrabalhadasStr;

                            retorno.Add(horasEfetivas);
                        }
                    }
                }

                return new ApiResponseObj<List<Horas_EfetivasListModel>> { data = retorno, ok = true };

            }
            catch (Exception e)
            {
                return new ApiResponseObj<List<Horas_EfetivasListModel>> { error = e.Message, ok = false };
            }
        }
    }
}