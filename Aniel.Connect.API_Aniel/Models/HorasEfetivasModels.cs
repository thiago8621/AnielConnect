using Aniel.T5.Model.enumerado;
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

namespace Aniel.Connect.API_Aniel.Models
{
    public class Horas_EfetivasListModel
    {
        public String Num_Fre { get; set; }
        public String Matricula_Auxiliar { get; set; }
        public String Nome { get; set; }
        public String Empresa { get; set; }
        public String Filial { get; set; }
        public String Centro_Custo { get; set; }
        public String Gestor { get; set; }

        public String Horas_Previstas { get; set; }
        public String Horas_Trabalhadas { get; set; }
        public String Horas_Extras_Totais { get; set; }
        public String Horas_Debito_Totais { get; set; }
        public String Horas_Abonadas { get; set; }
        public String Horas_Sobreaviso { get; set; }
        public String Horas_Adic_Noturno { get; set; }

        /// <summary> Percentual / Horas </summary>
        public Dictionary<double, double> Horas_Extras { get; set; }
        public String Horas_Extras_Adic_Noturno { get; set; }

        public String perc_1 { get; set; }
        public String perc_2 { get; set; }
        public String perc_3 { get; set; }
        public String perc_4 { get; set; }
        public String perc_5 { get; set; }
        public String perc_6 { get; set; }
        public String perc_7 { get; set; }
        public String perc_8 { get; set; }
        public String perc_9 { get; set; }
        public String perc_10 { get; set; }

        public String Atrasos { get; set; }
        public String Faltas { get; set; }
    }

    public class HorasEfetivasRequest : ApiRequest
    {
        /// <summary> Data inicial do período de Busca </summary>
        public String Data_Inicio { get; set; }
        /// <summary>  Data Final do período de Busca </summary>
        public String Data_Fim { get; set; }
        /// <summary> Num_fre do Funcionário </summary>
        public String Num_Fre { get; set; }
        /// <summary> Id da Empresa </summary>
        public Int64? Id_Empresa { get; set; }
        /// <summary> Identificador do Filial </summary>
        public Int64? Id_Filial { get; set; }
        /// <summary> Identificador do Departamento </summary>
        public Int64? Id_Departamento { get; set; }
        /// <summary> Num_Fre do Gestor </summary>
        public String Num_Fre_Gestor { get; set; }
        /// <summary> Identificador da Jornada </summary>
        public Int64? Id_Jornada { get; set; }
        /// <summary> Identificador do Centro de Custo </summary>
        public Int64? Id_Centro_Custo { get; set; }
        /// <summary> Código da Função </summary>
        public String Cod_Funcao { get; set; }
    }

    public class GetHorasTrabalhadasRetorno
    {
        public Double horasTrabalhadas { get; set; }
        public String horasTrabalhadasStr { get; set; }
        public Boolean temInconsistencia { get; set; }
        public List<Int64> listIdDiaFunc { get; set; }
    }

    public class GetHorasTrabalhadasModel
    {
        public DateTime Data_Inicio { get; set; }
        public DateTime Data_Fim { get; set; }

        public Int64? Id_Empresa { get; set; }
        public Int64? Id_Filial { get; set; }

        public Boolean VisualizarInconsistentes { get; set; }
        public Boolean Totalizador { get; set; }
        public Boolean RubricaDSR { get; set; }

        public Int64 Id_Dia_Funcionario { get; set; }
        public Int64 Id_Jornada { get; set; }
        public String Num_Fre { get; set; }
        public Int16 Ciclos { get; set; }
        public DateTime Data { get; set; }
        public Boolean Tem_Intervalo { get; set; }
        public List<Marcacao> Marcacoes { get; set; }
        public Boolean Tem_Inconsistencia { get; set; }
        public Boolean? Alterado { get; set; }
        public Int32? Status_Ajuste { get; set; }
        public Int64? Id_Evento_Dia { get; set; }
        public DayOfWeek? Dia_Semana { get; set; }
        public Boolean Permite_Marcar { get; set; }
        public Boolean? Considera_Intervalo_HE_Atraso { get; set; }
    }
    public class Marcacao
    {
        public Int64 Id { get; set; }
        public String NumFre { get; set; }
        public String Tipo_Marcacao { get; set; }
        public Int32 Ciclo { get; set; }
        public DateTime DataHora { get; set; }
        public Boolean Intervalo { get; set; }
    }

    public class Feriado_FolhaRequest
    {
        public DateTime Data { get; set; }
        public String Num_Fre { get; set; }
    }

    public class Feriado_FolhaModel
    {
        public String Titulo_Feriado { get; set; }
        public Boolean? E_Feriado { get; set; }
    }


    public class HorasIntervalo
    {
        public TimeSpan EntradaIntervalo { get; set; }
        public TimeSpan SaidaIntervalo { get; set; }
        public TimeSpan DuracaoIntervalo { get; set; }
    }

    public class HorasEfetivas
    {
        public static async Task<Double> HorasPrevistasDia(ISession session, DateTime dia, String numFre)
        {
            using (var com = session.Connection.CreateCommand())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                StringBuilder sql = new StringBuilder();

                #region SQL

                sql.AppendLine(" SELECT (CASE ");
                sql.AppendLine("             WHEN NOT EXISTS (SELECT ID FROM TB_PT_DIA_FUNCIONARIO ");
                sql.AppendLine("                              WHERE DATA = @DATA ");
                sql.AppendLine("                                AND NUM_FRE = @NUM_FRE) THEN ");
                sql.AppendLine("                ((CASE WHEN B.TIPO_JORNADA = 2 THEN ");
                sql.AppendLine("                             CASE WHEN D.ID > 0 THEN B.DURACAO_FERIADO ");
                sql.AppendLine("                                  WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 0 THEN B.DURACAO_DOMINGO ");
                sql.AppendLine("                                  WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 1 THEN B.DURACAO_SEGUNDA ");
                sql.AppendLine("                                  WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 2 THEN B.DURACAO_TERCA ");
                sql.AppendLine("                                  WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 3 THEN B.DURACAO_QUARTA ");
                sql.AppendLine("                                  WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 4 THEN B.DURACAO_QUINTA ");
                sql.AppendLine("                                  WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 5 THEN B.DURACAO_SEXTA ");
                sql.AppendLine("                                  WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 6 THEN B.DURACAO_SABADO ");
                sql.AppendLine("                             END ");
                sql.AppendLine("                          ELSE ");
                sql.AppendLine("                             CASE WHEN D.ID > 0 THEN CAST(((CASE ");
                sql.AppendLine("                                                                WHEN B.TERMINO_FERIADO < B.INICIO_FERIADO THEN DATEADD(DAY, 1, B.TERMINO_FERIADO) ");
                sql.AppendLine("                                                                ELSE B.TERMINO_FERIADO ");
                sql.AppendLine("                                                            END) - B.INICIO_FERIADO) AS DOUBLE PRECISION) * 24 ");
                sql.AppendLine("                                  WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 0 THEN CAST(((CASE ");
                sql.AppendLine("                                                                                                     WHEN B.TERMINO_DOMINGO < B.INICIO_DOMINGO THEN DATEADD(DAY, 1, B.TERMINO_DOMINGO) ");
                sql.AppendLine("                                                                                                     ELSE B.TERMINO_DOMINGO ");
                sql.AppendLine("                                                                                                 END) - B.INICIO_DOMINGO) AS DOUBLE PRECISION) * 24 ");
                sql.AppendLine("                                  WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 1 THEN CAST(((CASE ");
                sql.AppendLine("                                                                                                     WHEN B.TERMINO_SEGUNDA < B.INICIO_SEGUNDA THEN DATEADD(DAY, 1, B.TERMINO_SEGUNDA) ");
                sql.AppendLine("                                                                                                     ELSE B.TERMINO_SEGUNDA ");
                sql.AppendLine("                                                                                                 END) - B.INICIO_SEGUNDA) AS DOUBLE PRECISION) * 24 ");
                sql.AppendLine("                                  WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 2 THEN CAST(((CASE ");
                sql.AppendLine("                                                                                                     WHEN B.TERMINO_TERCA < B.INICIO_TERCA THEN DATEADD(DAY, 1, B.TERMINO_TERCA) ");
                sql.AppendLine("                                                                                                     ELSE B.TERMINO_TERCA ");
                sql.AppendLine("                                                                                                 END) - B.INICIO_TERCA) AS DOUBLE PRECISION) * 24 ");
                sql.AppendLine("                                  WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 3 THEN CAST(((CASE ");
                sql.AppendLine("                                                                                                     WHEN B.TERMINO_QUARTA < B.INICIO_QUARTA THEN DATEADD(DAY, 1, B.TERMINO_QUARTA) ");
                sql.AppendLine("                                                                                                     ELSE B.TERMINO_QUARTA ");
                sql.AppendLine("                                                                                                 END) - B.INICIO_QUARTA) AS DOUBLE PRECISION) * 24 ");
                sql.AppendLine("                                  WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 4 THEN CAST(((CASE ");
                sql.AppendLine("                                                                                                     WHEN B.TERMINO_QUINTA < B.INICIO_QUINTA THEN DATEADD(DAY, 1, B.TERMINO_QUINTA) ");
                sql.AppendLine("                                                                                                     ELSE B.TERMINO_QUINTA ");
                sql.AppendLine("                                                                                                 END) - B.INICIO_QUINTA) AS DOUBLE PRECISION) * 24 ");
                sql.AppendLine("                                  WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 5 THEN CAST(((CASE ");
                sql.AppendLine("                                                                                                     WHEN B.TERMINO_SEXTA < B.INICIO_SEXTA THEN DATEADD(DAY, 1, B.TERMINO_SEXTA) ");
                sql.AppendLine("                                                                                                     ELSE B.TERMINO_SEXTA ");
                sql.AppendLine("                                                                                                 END) - B.INICIO_SEXTA) AS DOUBLE PRECISION) * 24 ");
                sql.AppendLine("                                  WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 6 THEN CAST(((CASE ");
                sql.AppendLine("                                                                                                     WHEN B.TERMINO_SABADO < B.INICIO_SABADO THEN DATEADD(DAY, 1, B.TERMINO_SABADO) ");
                sql.AppendLine("                                                                                                     ELSE B.TERMINO_SABADO ");
                sql.AppendLine("                                                                                                 END) - B.INICIO_SABADO) AS DOUBLE PRECISION) * 24 ");
                sql.AppendLine("                             END ");
                sql.AppendLine("                     END) - ");
                sql.AppendLine("                 (CASE WHEN B.INTERVALO = 1 THEN ");
                sql.AppendLine("                          CASE WHEN B.TIPO_INTERVALO = 2 THEN ");
                sql.AppendLine("                               CASE WHEN B.TIPO_JORNADA = 1 THEN ");
                sql.AppendLine("                                   CASE WHEN D.ID > 0 THEN B.DURACAO_INTERVALO_FERIADO ");
                sql.AppendLine("                                       WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 0 THEN B.DURACAO_INTERVALO_DOMINGO ");
                sql.AppendLine("                                       WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 1 THEN B.DURACAO_INTERVALO_SEGUNDA ");
                sql.AppendLine("                                       WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 2 THEN B.DURACAO_INTERVALO_TERCA ");
                sql.AppendLine("                                       WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 3 THEN B.DURACAO_INTERVALO_QUARTA ");
                sql.AppendLine("                                       WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 4 THEN B.DURACAO_INTERVALO_QUINTA ");
                sql.AppendLine("                                       WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 5 THEN B.DURACAO_INTERVALO_SEXTA ");
                sql.AppendLine("                                       WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 6 THEN B.DURACAO_INTERVALO_SABADO ");
                sql.AppendLine("                                   END ");
                sql.AppendLine("                                   ELSE 0 ");
                sql.AppendLine("                               END ");
                sql.AppendLine("                               ELSE ");
                sql.AppendLine("                                   CASE WHEN D.ID > 0 THEN CAST(((CASE ");
                sql.AppendLine("                                                                      WHEN B.TERMINO_INTERVALO_FERIADO < B.INICIO_INTERVALO_FERIADO THEN DATEADD(DAY, 1, B.TERMINO_INTERVALO_FERIADO) ");
                sql.AppendLine("                                                                      ELSE B.TERMINO_INTERVALO_FERIADO ");
                sql.AppendLine("                                                                  END) -B.INICIO_INTERVALO_FERIADO) AS DOUBLE PRECISION) * 24 ");
                sql.AppendLine("                                        WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 0 THEN CAST(((CASE ");
                sql.AppendLine("                                                                                                           WHEN B.TERMINO_INTERVALO_DOMINGO < B.INICIO_INTERVALO_DOMINGO THEN DATEADD(DAY, 1, B.TERMINO_INTERVALO_DOMINGO) ");
                sql.AppendLine("                                                                                                           ELSE B.TERMINO_INTERVALO_DOMINGO ");
                sql.AppendLine("                                                                                                      END) - B.INICIO_INTERVALO_DOMINGO) AS DOUBLE PRECISION) * 24 ");
                sql.AppendLine("                                        WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 1 THEN CAST(((CASE ");
                sql.AppendLine("                                                                                                           WHEN B.TERMINO_INTERVALO_SEGUNDA < B.INICIO_INTERVALO_SEGUNDA THEN DATEADD(DAY, 1, B.TERMINO_INTERVALO_SEGUNDA) ");
                sql.AppendLine("                                                                                                           ELSE B.TERMINO_INTERVALO_SEGUNDA ");
                sql.AppendLine("                                                                                                       END) - B.INICIO_INTERVALO_SEGUNDA) AS DOUBLE PRECISION) * 24 ");
                sql.AppendLine("                                        WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 2 THEN CAST(((CASE ");
                sql.AppendLine("                                                                                                           WHEN B.TERMINO_INTERVALO_TERCA < B.INICIO_INTERVALO_TERCA THEN DATEADD(DAY, 1, B.TERMINO_INTERVALO_TERCA) ");
                sql.AppendLine("                                                                                                           ELSE B.TERMINO_INTERVALO_TERCA ");
                sql.AppendLine("                                                                                                       END) - B.INICIO_INTERVALO_TERCA) AS DOUBLE PRECISION) * 24 ");
                sql.AppendLine("                                        WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 3 THEN CAST(((CASE ");
                sql.AppendLine("                                                                                                           WHEN B.TERMINO_INTERVALO_QUARTA < B.INICIO_INTERVALO_QUARTA THEN DATEADD(DAY, 1, B.TERMINO_INTERVALO_QUARTA) ");
                sql.AppendLine("                                                                                                           ELSE B.TERMINO_INTERVALO_QUARTA ");
                sql.AppendLine("                                                                                                       END) - B.INICIO_INTERVALO_QUARTA) AS DOUBLE PRECISION) * 24 ");
                sql.AppendLine("                                        WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 4 THEN CAST(((CASE ");
                sql.AppendLine("                                                                                                           WHEN B.TERMINO_INTERVALO_QUINTA < B.INICIO_INTERVALO_QUINTA THEN DATEADD(DAY, 1, B.TERMINO_INTERVALO_QUINTA) ");
                sql.AppendLine("                                                                                                           ELSE B.TERMINO_INTERVALO_QUINTA ");
                sql.AppendLine("                                                                                                       END) - B.INICIO_INTERVALO_QUINTA) AS DOUBLE PRECISION) * 24 ");
                sql.AppendLine("                                        WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 5 THEN CAST(((CASE ");
                sql.AppendLine("                                                                                                           WHEN B.TERMINO_INTERVALO_SEXTA < B.INICIO_INTERVALO_SEXTA THEN DATEADD(DAY, 1, B.TERMINO_INTERVALO_SEXTA) ");
                sql.AppendLine("                                                                                                           ELSE B.TERMINO_INTERVALO_SEXTA ");
                sql.AppendLine("                                                                                                       END) - B.INICIO_INTERVALO_SEXTA) AS DOUBLE PRECISION) * 24 ");
                sql.AppendLine("                                        WHEN EXTRACT(WEEKDAY FROM CAST(@DATA AS DATE)) = 6 THEN CAST(((CASE ");
                sql.AppendLine("                                                                                                           WHEN B.TERMINO_INTERVALO_SABADO < B.INICIO_INTERVALO_SABADO THEN DATEADD(DAY, 1, B.TERMINO_INTERVALO_SABADO) ");
                sql.AppendLine("                                                                                                           ELSE B.TERMINO_INTERVALO_SABADO ");
                sql.AppendLine("                                                                                                       END) - B.INICIO_INTERVALO_SABADO) AS DOUBLE PRECISION) * 24 ");
                sql.AppendLine("                                   END ");
                sql.AppendLine("                           END ");
                sql.AppendLine("                        ELSE 0 ");
                sql.AppendLine("                     END)) ");
                sql.AppendLine("             ELSE 0 ");
                sql.AppendLine("         END) AS HORAS_PREVISTAS ");

                sql.AppendLine(" FROM TB_FUNCIONARIO A ");

                sql.AppendLine(" INNER JOIN TB_PT_JORNADA_FUNCIONARIO B ");
                sql.AppendLine(" ON (B.ID = A.ID_JORNADA) ");

                sql.AppendLine(" LEFT JOIN TB_PT_CIDADE C ON(C.ID = A.ID_MUNICIPIO_TRABALHO) ");
                sql.AppendLine(" LEFT JOIN TB_PT_FERIADO D ON(EXTRACT(MONTH FROM CAST(@DATA AS DATE)) = D.MES ");
                sql.AppendLine("                              AND EXTRACT(DAY FROM CAST(@DATA AS DATE)) = D.DIA AND D.STATUS = 1 ");
                sql.AppendLine("                              AND (CASE ");
                sql.AppendLine("                                       WHEN D.ABRANGENCIA = 2 THEN D.ID_MUNICIPIO = C.ID ");
                sql.AppendLine("                                       WHEN D.ABRANGENCIA = 3 THEN D.UF = C.UF ");
                sql.AppendLine("                                       ELSE TRUE ");
                sql.AppendLine("                                   END)) ");

                sql.AppendLine(" WHERE A.NUM_FRE = @NUM_FRE ");

                #endregion

                parametros.Add("@NUM_FRE", numFre);
                parametros.Add("@DATA", dia);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                        return Convert.ToDouble(reader["HORAS_PREVISTAS"]);
                    else
                        return 0;
                }
            }
        }

        public static async Task<Int64> CiclosZero(String numFre, DateTime data_ini, DateTime data_fim)
        {
            Int64 CiclosZero = 0;

            using (ISession session = HibernateUtil.GetSession())
            using (var com = session.Connection.CreateCommand())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();
                StringBuilder sql = new StringBuilder(@"SELECT COUNT(A.CICLOS) AS CICLOSZERO FROM TB_PT_DIA_FUNCIONARIO A
                                                        LEFT JOIN TB_PT_CLASSIFICACAO_ESCALA B ON (B.ID = A.ID_EVENTO)
                                                        WHERE B.PERMITIR_MARCAR = 1 AND ( A.DATA BETWEEN @DATAINI AND @DATAFIM) AND A.CICLOS = 0 AND A.NUM_FRE = @NUM_FRE");



                parametros.Add("@NUM_FRE", numFre);
                parametros.Add("@DATAINI", data_ini);
                parametros.Add("@DATAFIM", data_fim);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        CiclosZero = Convert.ToInt64(reader["CICLOSZERO"]);
                    }
                }
            }

            return CiclosZero;
        }

        public static async Task<GetHorasTrabalhadasRetorno> GetHorasTrabalhadas(ISession session, GetHorasTrabalhadasModel model)
        {
            double horasTrabalhadas = 0;
            List<GetHorasTrabalhadasModel> fechamento = new List<GetHorasTrabalhadasModel>();
            List<Int64> listaIdDiaFuncionario = new List<Int64>();

            using (var repositoryDiaFuncionario = new Conexao<Tb_Pt_Dia_Funcionario>(session))
            using (var com = session.Connection.CreateCommand())
            {
                try
                {
                    Dictionary<string, object> parametros = new Dictionary<string, object>();

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.ID, B.ID AS ID_DIA_FUNCIONARIO, B.NUM_FRE, A.TIPO_MARCACAO, A.CICLO, ");
                    sql.AppendLine("        A.DATAHORA, A.INTERVALO, B.CICLOS, CAST(B.DATA AS DATE) AS DATA, B.ALTERADO, ");
                    sql.AppendLine("        M.INTERVALO AS TEM_INTERVALO, B.ID_JORNADA AS ID_JORNADA, ");
                    sql.AppendLine("        J.CODEMPRESA AS EMPRESA, H.CODFILIAL AS FILIAL, B.ID_EVENTO, ");
                    sql.AppendLine("        M.CONSIDERA_INTERVALO_HE_ATRASO, I.PERMITIR_MARCAR, LIST(L.STATUS) AS STATUS_AJUSTE ");

                    sql.AppendLine(" FROM TB_PT_DIA_FUNCIONARIO B ");

                    sql.AppendLine(" LEFT JOIN TB_PT_MARCACAO A ");
                    sql.AppendLine(" ON(B.ID = A.ID_DIA_FUNCIONARIO) ");

                    sql.AppendLine(" LEFT JOIN TB_PT_AJUSTE_PONTO L ");
                    sql.AppendLine(" ON(L.ID_DIA_FUNCIONARIO = B.ID) ");

                    sql.AppendLine(" LEFT JOIN TB_FUNCIONARIO C ");
                    sql.AppendLine(" ON(C.NUM_FRE = A.NUM_FRE) ");

                    sql.AppendLine(" LEFT JOIN TB_PT_JORNADA_FUNCIONARIO M ");
                    sql.AppendLine(" ON(M.ID = B.ID_JORNADA) ");

                    sql.AppendLine(" LEFT JOIN TB_EMPRESA J ");
                    sql.AppendLine(" ON(J.CODEMPRESA = C.CODEMPRESA) ");

                    sql.AppendLine(" LEFT JOIN TB_FILIAL H ");
                    sql.AppendLine(" ON(H.CODEMPRESA = C.CODEMPRESA AND H.CODFILIAL = C.CODFILIAL) ");

                    sql.AppendLine(" LEFT JOIN TB_PT_CLASSIFICACAO_ESCALA I ");
                    sql.AppendLine(" ON(B.ID_EVENTO = I.ID) ");

                    sql.AppendLine(" WHERE B.DATA BETWEEN @DATAINICIO AND @DATAFIM ");
                    sql.AppendLine("   AND B.NUM_FRE = @NUM_FRE ");

                    sql.AppendLine(" GROUP BY A.ID, B.ID, B.NUM_FRE, A.TIPO_MARCACAO, A.CICLO, ");
                    sql.AppendLine("          A.DATAHORA, A.INTERVALO, B.CICLOS, CAST(B.DATA AS DATE), B.ALTERADO, M.INTERVALO, B.ID_JORNADA, ");
                    sql.AppendLine("          J.CODEMPRESA, H.CODFILIAL, B.ID_EVENTO, M.CONSIDERA_INTERVALO_HE_ATRASO, I.PERMITIR_MARCAR ");

                    parametros.Add("@NUM_FRE", model.Num_Fre);
                    parametros.Add("@DATAINICIO", model.Data_Inicio);
                    parametros.Add("@DATAFIM", model.Data_Fim);

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            GetHorasTrabalhadasModel dia = null;

                            if (reader["ID_DIA_FUNCIONARIO"] != DBNull.Value)
                                dia = fechamento.Find(p => p.Id_Dia_Funcionario == Convert.ToInt64(reader["ID_DIA_FUNCIONARIO"]));

                            if (dia != null)
                            {
                                dia.Marcacoes.Add(new Marcacao
                                {
                                    Id = reader["ID"] != DBNull.Value ? Convert.ToInt64(reader["ID"]) : 0,
                                    NumFre = Convert.ToString(reader["NUM_FRE"]),
                                    Tipo_Marcacao = reader["TIPO_MARCACAO"] != DBNull.Value ? Convert.ToString(reader["TIPO_MARCACAO"]) : "",
                                    Ciclo = reader["CICLO"] != DBNull.Value ? Convert.ToInt16(reader["CICLO"]) : 0,
                                    DataHora = Convert.ToDateTime(reader["DATAHORA"]),
                                    Intervalo = Convert.ToBoolean(reader["INTERVALO"])
                                });
                            }
                            else
                            {
                                GetHorasTrabalhadasModel fechaTemp = new GetHorasTrabalhadasModel();

                                if (reader["ID_DIA_FUNCIONARIO"] != DBNull.Value)
                                    fechaTemp.Id_Dia_Funcionario = Convert.ToInt64(reader["ID_DIA_FUNCIONARIO"]);

                                fechaTemp.Num_Fre = Convert.ToString(reader["NUM_FRE"]);
                                fechaTemp.Data = Convert.ToDateTime(reader["DATA"]);
                                fechaTemp.Ciclos = reader["CICLOS"] != DBNull.Value ? Convert.ToInt16(reader["CICLOS"]) : (short)0;
                                fechaTemp.Id_Evento_Dia = reader["ID_EVENTO"] != DBNull.Value ? Convert.ToInt64(reader["ID_EVENTO"]) : 0;
                                fechaTemp.Permite_Marcar = reader["PERMITIR_MARCAR"] != DBNull.Value ? Convert.ToBoolean(reader["PERMITIR_MARCAR"]) : false;

                                if (reader["ID_JORNADA"] == DBNull.Value)
                                {
                                    fechaTemp.Tem_Intervalo = false;
                                }
                                else
                                {
                                    fechaTemp.Id_Jornada = Convert.ToInt64(reader["ID_JORNADA"]);
                                    fechaTemp.Tem_Intervalo = reader["TEM_INTERVALO"] != DBNull.Value ? Convert.ToBoolean(Convert.ToInt16(reader["TEM_INTERVALO"])) : false;
                                }

                                fechaTemp.Alterado = reader["ALTERADO"] != DBNull.Value ? Convert.ToBoolean(reader["ALTERADO"]) : (bool?)null;

                                fechaTemp.Status_Ajuste = reader["STATUS_AJUSTE"] != DBNull.Value ?
                                                                (Convert.ToString(reader["STATUS_AJUSTE"]).Split(',').Contains("0") ? 0 : (Int32?)null)
                                                                : null;

                                if (model.Id_Empresa.HasValue)
                                    fechaTemp.Id_Empresa = Convert.ToInt64(reader["EMPRESA"]);

                                if (model.Id_Filial.HasValue)
                                    fechaTemp.Id_Filial = Convert.ToInt64(reader["FILIAL"]);

                                fechaTemp.Marcacoes = new List<Marcacao>();

                                if (reader["ID"] != DBNull.Value)
                                {
                                    fechaTemp.Marcacoes.Add(new Marcacao
                                    {
                                        Id = reader["ID"] != DBNull.Value ? Convert.ToInt64(reader["ID"]) : 0,
                                        NumFre = Convert.ToString(reader["NUM_FRE"]),
                                        Tipo_Marcacao = reader["TIPO_MARCACAO"] != DBNull.Value ? Convert.ToString(reader["TIPO_MARCACAO"]) : "",
                                        Ciclo = reader["CICLO"] != DBNull.Value ? Convert.ToInt16(reader["CICLO"]) : 0,
                                        DataHora = Convert.ToDateTime(reader["DATAHORA"]),
                                        Intervalo = Convert.ToBoolean(reader["INTERVALO"])
                                    });
                                }

                                fechaTemp.Considera_Intervalo_HE_Atraso =
                                    reader["CONSIDERA_INTERVALO_HE_ATRASO"] != DBNull.Value ?
                                        Convert.ToBoolean(reader["CONSIDERA_INTERVALO_HE_ATRASO"]) :
                                        false;

                                fechamento.Add(fechaTemp);
                            }
                        }
                    }

                    bool temInconsistencia = false;

                    if (model.Dia_Semana.HasValue)
                        fechamento = fechamento.Where(p => p.Data.DayOfWeek == model.Dia_Semana.Value).ToList();

                    #region Calculo horas

                    foreach (var diaFechamento in fechamento)
                    {
                        int ultimoCiclo = diaFechamento.Ciclos;

                        if (ultimoCiclo >= 1)
                        {
                            var listCiclos = diaFechamento.Marcacoes.Select(p => p.Ciclo).Distinct();

                            var diaInconsistente = await TemInconsistencia(
                                session,
                                new List<GetHorasTrabalhadasModel> { diaFechamento },
                                diaFechamento.Data,
                                diaFechamento.Data,
                                model.Num_Fre
                            );

                            if (!diaInconsistente)
                            {
                                horasTrabalhadas += await GetHorasDia(session, diaFechamento);
                            }
                            else
                            {
                                diaFechamento.Tem_Inconsistencia = true;
                                temInconsistencia = true;
                            }
                        }
                    }

                    #endregion

                    bool negativo = horasTrabalhadas < 0;

                    var ciclosZerado = await CiclosZero(model.Num_Fre, model.Data_Inicio, model.Data_Fim);

                    if (ciclosZerado > 0)
                        temInconsistencia = true;

                    if (horasTrabalhadas == 0)
                        return new GetHorasTrabalhadasRetorno
                        {
                            horasTrabalhadas = horasTrabalhadas,
                            horasTrabalhadasStr = "00:00",
                            temInconsistencia = fechamento.Where(p => p.Permite_Marcar).Count() > 0 ? true : false,
                            listIdDiaFunc = new List<Int64>()
                        };
                    else if (negativo)
                        return new GetHorasTrabalhadasRetorno
                        {
                            horasTrabalhadas = horasTrabalhadas,
                            horasTrabalhadasStr = "00:00",
                            temInconsistencia = true,
                            listIdDiaFunc = new List<Int64>()
                        };
                    else
                        return new GetHorasTrabalhadasRetorno
                        {
                            horasTrabalhadas = horasTrabalhadas,
                            horasTrabalhadasStr = StringService.HoraParaString(horasTrabalhadas),
                            temInconsistencia = temInconsistencia,
                            listIdDiaFunc = listaIdDiaFuncionario
                        };
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static async Task<HorasIntervalo> GetHorasIntervaloNoDia(ISession session, String numFre, Int64 idJornada, DateTime dia)
        {
            HorasIntervalo retorno = new HorasIntervalo();

            using (var com = session.Connection.CreateCommand())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sql = new StringBuilder(@"SELECT (CASE 
                            WHEN C.TIPO_INTERVALO = 1 THEN 
                            (CASE 
                                WHEN E.ID > 0 THEN CAST(C.INICIO_INTERVALO_FERIADO AS TIME) 
                                WHEN EXTRACT(WEEKDAY FROM A.DATA) = 0 THEN CAST(C.INICIO_INTERVALO_DOMINGO AS TIME) 
                                WHEN EXTRACT(WEEKDAY FROM A.DATA) = 1 THEN CAST(C.INICIO_INTERVALO_SEGUNDA AS TIME) 
                                WHEN EXTRACT(WEEKDAY FROM A.DATA) = 2 THEN CAST(C.INICIO_INTERVALO_TERCA AS TIME) 
                                WHEN EXTRACT(WEEKDAY FROM A.DATA) = 3 THEN CAST(C.INICIO_INTERVALO_QUARTA AS TIME) 
                                WHEN EXTRACT(WEEKDAY FROM A.DATA) = 4 THEN CAST(C.INICIO_INTERVALO_QUINTA AS TIME) 
                                WHEN EXTRACT(WEEKDAY FROM A.DATA) = 5 THEN CAST(C.INICIO_INTERVALO_SEXTA AS TIME) 
                                WHEN EXTRACT(WEEKDAY FROM A.DATA) = 6 THEN CAST(C.INICIO_INTERVALO_SABADO AS TIME) 
                            END) 
                        END) AS INICIO_INTERVALO, 
                    (CASE 
                            WHEN C.TIPO_INTERVALO = 1 THEN 
                            (CASE 
                                WHEN E.ID > 0 THEN CAST(C.TERMINO_INTERVALO_FERIADO AS TIME) 
                                WHEN EXTRACT(WEEKDAY FROM A.DATA) = 0 THEN CAST(C.TERMINO_INTERVALO_DOMINGO AS TIME) 
                                WHEN EXTRACT(WEEKDAY FROM A.DATA) = 1 THEN CAST(C.TERMINO_INTERVALO_SEGUNDA AS TIME) 
                                WHEN EXTRACT(WEEKDAY FROM A.DATA) = 2 THEN CAST(C.TERMINO_INTERVALO_TERCA AS TIME) 
                                WHEN EXTRACT(WEEKDAY FROM A.DATA) = 3 THEN CAST(C.TERMINO_INTERVALO_QUARTA AS TIME) 
                                WHEN EXTRACT(WEEKDAY FROM A.DATA) = 4 THEN CAST(C.TERMINO_INTERVALO_QUINTA AS TIME) 
                                WHEN EXTRACT(WEEKDAY FROM A.DATA) = 5 THEN CAST(C.TERMINO_INTERVALO_SEXTA AS TIME) 
                                WHEN EXTRACT(WEEKDAY FROM A.DATA) = 6 THEN CAST(C.TERMINO_INTERVALO_SABADO AS TIME) 
                            END) 
                        END) AS TERMINO_INTERVALO, 
                    (CASE 
                            WHEN C.TIPO_INTERVALO = 2 THEN 
                            (CASE 
                                WHEN E.ID > 0 THEN C.DURACAO_INTERVALO_FERIADO 
                                WHEN EXTRACT(WEEKDAY FROM A.DATA) = 0 THEN C.DURACAO_INTERVALO_DOMINGO 
                                WHEN EXTRACT(WEEKDAY FROM A.DATA) = 1 THEN C.DURACAO_INTERVALO_SEGUNDA 
                                WHEN EXTRACT(WEEKDAY FROM A.DATA) = 2 THEN C.DURACAO_INTERVALO_TERCA 
                                WHEN EXTRACT(WEEKDAY FROM A.DATA) = 3 THEN C.DURACAO_INTERVALO_QUARTA 
                                WHEN EXTRACT(WEEKDAY FROM A.DATA) = 4 THEN C.DURACAO_INTERVALO_QUINTA 
                                WHEN EXTRACT(WEEKDAY FROM A.DATA) = 5 THEN C.DURACAO_INTERVALO_SEXTA 
                                WHEN EXTRACT(WEEKDAY FROM A.DATA) = 6 THEN C.DURACAO_INTERVALO_SABADO 
                            END) 
                        END) AS DURACAO_INTERVALO 

                FROM TB_PT_DIA_FUNCIONARIO A 

                INNER JOIN TB_FUNCIONARIO B 
                ON(A.NUM_FRE = B.NUM_FRE) 

                LEFT JOIN TB_PT_JORNADA_FUNCIONARIO C 
                ON(A.ID_JORNADA = C.ID) 

                LEFT JOIN TB_PT_CIDADE D 
                ON(D.ID = B.ID_MUNICIPIO_TRABALHO) 

                LEFT JOIN TB_PT_FERIADO E ON(E.STATUS = 1 AND E.DIA = EXTRACT(DAY FROM A.DATA) AND E.MES = EXTRACT(MONTH FROM A.DATA)
                AND((E.ABRANGENCIA = 2 AND E.ID_MUNICIPIO = D.ID) OR
                    (E.ABRANGENCIA = 3 AND E.UF = D.UF) OR
                    (E.ABRANGENCIA NOT IN(2, 3)))
                    )

                WHERE A.DATA = @DATA 
                AND A.ID_JORNADA = @ID_JORNADA 
                AND A.NUM_FRE = @NUM_FRE ");

                parametros.Add("@DATA", dia);
                parametros.Add("@ID_JORNADA", idJornada);
                parametros.Add("@NUM_FRE", numFre);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        retorno.EntradaIntervalo = reader["INICIO_INTERVALO"] != DBNull.Value ? TimeSpan.Parse(Convert.ToString(reader["INICIO_INTERVALO"])) : TimeSpan.Zero;
                        retorno.SaidaIntervalo = reader["TERMINO_INTERVALO"] != DBNull.Value ? TimeSpan.Parse(Convert.ToString(reader["TERMINO_INTERVALO"])) : TimeSpan.Zero;
                        retorno.DuracaoIntervalo = reader["DURACAO_INTERVALO"] != DBNull.Value ? TimeSpan.FromHours(Convert.ToDouble(reader["DURACAO_INTERVALO"])) : TimeSpan.Zero;
                    }
                }
            }

            return retorno;
        }


        public static async Task<Double> GetHorasDia(ISession session,  GetHorasTrabalhadasModel diaFechamento)
        {
            double horasTrabalhadas = 0;
            List<Marcacao> marcacoesFuncionario = diaFechamento.Marcacoes.OrderBy(p => p.Ciclo).ThenBy(p => p.Tipo_Marcacao).ToList();

            bool intervaloMarcado = false;
            double duracaoIntervaloMarcado = 0;

            HorasIntervalo horasIntervalo = await GetHorasIntervaloNoDia(session, diaFechamento.Num_Fre, diaFechamento.Id_Jornada, diaFechamento.Data);

            for (int i = 1; i < marcacoesFuncionario.Count(); i++)
            {
                bool horarioIntervalo = marcacoesFuncionario[i].Intervalo && marcacoesFuncionario[i - 1].Intervalo;
                bool intervaloCiclos = marcacoesFuncionario[i].Ciclo != marcacoesFuncionario[i - 1].Ciclo;

                if (diaFechamento.Considera_Intervalo_HE_Atraso.HasValue)
                {
                    if (marcacoesFuncionario[i].Intervalo && !diaFechamento.Considera_Intervalo_HE_Atraso.Value)
                    {
                        if (!intervaloMarcado)
                        {
                            if (horasIntervalo.EntradaIntervalo != TimeSpan.Zero)
                                marcacoesFuncionario[i].DataHora = marcacoesFuncionario[i].DataHora.Date.Add(horasIntervalo.EntradaIntervalo);
                        }
                        else
                        {
                            if (horasIntervalo.SaidaIntervalo != TimeSpan.Zero)
                                marcacoesFuncionario[i].DataHora = marcacoesFuncionario[i].DataHora.Date.Add(horasIntervalo.SaidaIntervalo);
                        }
                    }
                }

                if (!horarioIntervalo && !intervaloCiclos)
                {
                    horasTrabalhadas += (marcacoesFuncionario[i].DataHora - marcacoesFuncionario[i - 1].DataHora).TotalHours;
                }

                if (horarioIntervalo)
                {
                    duracaoIntervaloMarcado = (marcacoesFuncionario[i].DataHora - marcacoesFuncionario[i - 1].DataHora).TotalHours;
                }

                if (marcacoesFuncionario[i].Intervalo)
                {
                    intervaloMarcado = true;
                }
            }

            if (diaFechamento.Considera_Intervalo_HE_Atraso.HasValue)
            {
                if (!diaFechamento.Considera_Intervalo_HE_Atraso.Value)
                {
                    using (var repoJornada = new Conexao<Tb_Pt_Jornada>(session))
                    {
                        Tb_Pt_Jornada jornada = await repoJornada.selectFirst(jor => jor.Id == diaFechamento.Id_Jornada);

                        double debitoDia = await GetDebitoDia(diaFechamento.Num_Fre, diaFechamento.Data);

                        if (duracaoIntervaloMarcado == 0 &&
                            duracaoIntervaloMarcado != horasIntervalo.DuracaoIntervalo.TotalHours &&
                            jornada.Tipo_Jornada == Tipo_Jornada.Horario_Flexivel &&
                            jornada.Tipo_Intervalo == Tipo_Intervalo_Jornada.Intervalo_Flexivel &&
                            debitoDia >= horasIntervalo.DuracaoIntervalo.TotalHours)
                        {
                            double creditoDia = await GetCreditoDia(diaFechamento.Num_Fre, diaFechamento.Data);
                            double horasPrevistas = await GetHorasPrevistas(jornada, diaFechamento.Data, diaFechamento.Num_Fre);

                            horasTrabalhadas = horasPrevistas - debitoDia + creditoDia;
                        }
                        else
                        {
                            horasTrabalhadas -= await CompensarHorasIntervalo(
                                                            diaFechamento,
                                                            marcacoesFuncionario,
                                                            jornada,
                                                            horasIntervalo,
                                                            duracaoIntervaloMarcado
                                                        );
                        }
                    }
                }
            }

            return DateTimeUtil.RemoveSeconds(horasTrabalhadas);
        }

        public static DateTime GetDataPrevistaIntervalo(DateTime dataMarcacaoInicial, TimeSpan horaIntervalo)
        {
            DateTime dataPrevista = dataMarcacaoInicial;
            TimeSpan horaMarcacaoInicial = DateTimeUtil.GetTimeSpanFromDate(dataMarcacaoInicial);

            if (horaMarcacaoInicial > horaIntervalo)
            {
                // Data de intervalo para as jornadas dobradas

                TimeSpan horasAteIntervalo = horaIntervalo.Add(TimeSpan.FromDays(1)) - horaMarcacaoInicial;
                dataPrevista = dataPrevista.AddDays(1).Add(horasAteIntervalo);

                return dataPrevista;
            }
            else
            {
                dataPrevista = dataPrevista.Date.Add(horaIntervalo);
                return dataPrevista;
            }
        }

        public static  async Task<Double> CompensarHorasIntervalo(
            GetHorasTrabalhadasModel diaFechamento,
            List<Marcacao> marcacoesFuncionario,
            Tb_Pt_Jornada jornada,
            HorasIntervalo horasIntervaloPrevisto,
            Double duracaoIntervaloMarcado
        )
        {
            double horasCompensar = 0;
            double debitoDia = await GetDebitoDia(diaFechamento.Num_Fre, diaFechamento.Data);

            if (horasIntervaloPrevisto.EntradaIntervalo != TimeSpan.Zero && horasIntervaloPrevisto.SaidaIntervalo != TimeSpan.Zero)
            {
                DateTime dataPrevistaIntervalo = GetDataPrevistaIntervalo(marcacoesFuncionario[0].DataHora, horasIntervaloPrevisto.SaidaIntervalo);
                double intervaloPrevisto = (horasIntervaloPrevisto.SaidaIntervalo - horasIntervaloPrevisto.EntradaIntervalo).TotalHours;

                if (diaFechamento.Marcacoes.Where(p => p.DataHora < dataPrevistaIntervalo).Any() &&
                    diaFechamento.Marcacoes.Where(p => p.DataHora > dataPrevistaIntervalo).Any())
                {
                    if (duracaoIntervaloMarcado != intervaloPrevisto)
                    {
                        horasCompensar = intervaloPrevisto - duracaoIntervaloMarcado;
                    }
                }
            }
            else if (horasIntervaloPrevisto.DuracaoIntervalo != TimeSpan.Zero)
            {
                if (duracaoIntervaloMarcado != horasIntervaloPrevisto.DuracaoIntervalo.TotalHours)
                {
                    if (jornada.Tipo_Jornada == Tipo_Jornada.Horarios_Fixos)
                    {
                        if (duracaoIntervaloMarcado != 0)
                        {
                            horasCompensar = horasIntervaloPrevisto.DuracaoIntervalo.TotalHours - duracaoIntervaloMarcado;
                        }
                        else
                        {
                            if (debitoDia < horasIntervaloPrevisto.DuracaoIntervalo.TotalHours)
                            {
                                horasCompensar = horasIntervaloPrevisto.DuracaoIntervalo.TotalHours - duracaoIntervaloMarcado;
                            }
                        }
                    }
                    else
                    {
                        if (duracaoIntervaloMarcado != 0)
                        {
                            horasCompensar = horasIntervaloPrevisto.DuracaoIntervalo.TotalHours - duracaoIntervaloMarcado;
                        }
                        else
                        {
                            if (debitoDia < horasIntervaloPrevisto.DuracaoIntervalo.TotalHours)
                            {
                                horasCompensar = horasIntervaloPrevisto.DuracaoIntervalo.TotalHours - duracaoIntervaloMarcado;
                            }
                        }
                    }
                }
            }

            return horasCompensar;
        }

        public static async Task<Boolean> TemInconsistencia(
        ISession session,
        List<GetHorasTrabalhadasModel> listDiaFunc,
        DateTime Data_Inicio,
        DateTime Data_Fim,
        String Num_Fre)
        {
            using (var repoJornada = new Conexao<Tb_Pt_Jornada>(session))
            {
                Boolean tem_inconsistencia = false;
                foreach (var diaFunc in listDiaFunc)
                {
                    int ultimoCiclo = diaFunc.Ciclos;

                    if (ultimoCiclo >= 1)
                    {
                        var listCiclos = diaFunc.Marcacoes.Select(p => p.Ciclo).Distinct();

                        List<Marcacao> marcacoesFuncionario = diaFunc.Marcacoes.Where(marc => marc.NumFre == diaFunc.Num_Fre && (
                        marc.DataHora.Date >= Data_Inicio.Date && marc.DataHora.Date <= Data_Fim.Date)).ToList();


                        if (diaFunc.Status_Ajuste == (Int32)Enum_Status_Solicitacao_Ajuste.AGUARDANDO_AVALIACAO)
                        {
                            tem_inconsistencia = true;
                        }
                        else
                        {
                            for (int j = 1; j <= diaFunc.Ciclos; j++)
                            {
                                var qtdMarcacoesCiclo = diaFunc.Marcacoes.Where(p => p.Ciclo == j).Count();

                                var tipoMarcacaoEntrada = diaFunc.Marcacoes.Where(p => p.Tipo_Marcacao == "E" && p.Ciclo == j).Count();
                                var tipoMarcacaoSaida = diaFunc.Marcacoes.Where(p => p.Tipo_Marcacao == "S" && p.Ciclo == j).Count();

                                if (qtdMarcacoesCiclo != 2)
                                {
                                    tem_inconsistencia = true;
                                    break;
                                }
                                if (tipoMarcacaoEntrada >= 2)
                                {
                                    tem_inconsistencia = true;
                                }
                                if (tipoMarcacaoSaida >= 2)
                                {
                                    tem_inconsistencia = true;
                                }
                            }
                        }

                        marcacoesFuncionario = marcacoesFuncionario.OrderBy(p => p.Tipo_Marcacao).OrderBy(p => p.Ciclo).ToList();

                        for (var i = 1; i < marcacoesFuncionario.Count(); i++)
                        {
                            var horaAtual = marcacoesFuncionario[i].DataHora;
                            var horaAnterior = marcacoesFuncionario[i - 1].DataHora;

                            if (horaAtual < horaAnterior)
                            {
                                tem_inconsistencia = true;
                            }

                        }
                        var qtdMarcacoes = diaFunc.Marcacoes.Count();
                        if (qtdMarcacoes % 2 != 0)
                        {
                            tem_inconsistencia = true;
                        }

                        if (diaFunc.Ciclos == 0 && marcacoesFuncionario.Count() <= 0)
                        {
                            tem_inconsistencia = true;
                        }
                    }
                    else
                    {
                        bool feriado = (await GetFeriado(new Feriado_FolhaRequest
                        {
                            Data = diaFunc.Data,
                            Num_Fre = diaFunc.Num_Fre
                        })).Where(p => p.E_Feriado.Value).Count() > 0;

                        var jornada = await repoJornada.selectFirst(p => p.Id == diaFunc.Id_Jornada);

                        if (feriado && jornada.Opcao_Trabalha_Feriado)
                        {
                            tem_inconsistencia = true;
                        }
                    }
                }

                return tem_inconsistencia;
            }
        }

        public static async Task<List<Feriado_FolhaModel>> GetFeriado(Feriado_FolhaRequest feriado)
        {
            List<Feriado_FolhaModel> retorno = new List<Feriado_FolhaModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoFeriado = new Conexao<Tb_Pt_Feriado>(session))
            using (var repoFuncionario = new Conexao<Tb_Funcionario>(session))
            {
                var funcionario = await repoFuncionario.selectFirst(p => p.NUM_FRE == feriado.Num_Fre);
                var tem_feriado = await repoFeriado.select(p => p.Mes == feriado.Data.Month && p.Dia == feriado.Data.Day && p.Status == Status_Default.Ativo);

                if (tem_feriado.Count != 0)
                {
                    foreach (var feriadoResult in tem_feriado)
                    {
                        Feriado_FolhaModel objRetorno = new Feriado_FolhaModel();

                        switch (feriadoResult.Abrangencia)
                        {
                            case Enum_Abrangencia.Internacional:
                                objRetorno.E_Feriado = true;
                                objRetorno.Titulo_Feriado = feriadoResult.Descricao;
                                break;
                            case Enum_Abrangencia.Nacional:
                                objRetorno.E_Feriado = true;
                                objRetorno.Titulo_Feriado = feriadoResult.Descricao;
                                break;
                            case Enum_Abrangencia.Estadual:
                                if (feriadoResult.UF == funcionario.UF_TRABALHO)
                                {
                                    objRetorno.E_Feriado = true;
                                    objRetorno.Titulo_Feriado = feriadoResult.Descricao;
                                }
                                else
                                {
                                    objRetorno.E_Feriado = false;
                                    objRetorno.Titulo_Feriado = "";
                                }
                                break;
                            case Enum_Abrangencia.Municipal:
                                if (feriadoResult.UF == funcionario.UF_TRABALHO && feriadoResult.Id_Municipio == funcionario.ID_MUNICIPIO_TRABALHO)
                                {
                                    objRetorno.E_Feriado = true;
                                    objRetorno.Titulo_Feriado = feriadoResult.Descricao;
                                }
                                else
                                {
                                    objRetorno.E_Feriado = false;
                                    objRetorno.Titulo_Feriado = "";
                                }
                                break;

                        }
                        retorno.Add(objRetorno);
                    }
                }
            }
            return retorno;
        }

        public static async Task<Double> GetHorasPrevistas(Tb_Pt_Jornada jornada, DateTime data, String numFre)
        {

            double horasPrevistas = 0;

            bool feriado = (await GetFeriado(new Feriado_FolhaRequest
            {
                Data = data,
                Num_Fre = numFre
            })).Where(p => p.E_Feriado.Value).Count() > 0;

            if (feriado)
            {
                if (jornada.Opcao_Trabalha_Feriado)
                {
                    if (jornada.Tipo_Jornada == Tipo_Jornada.Horarios_Fixos)
                    {
                        if (jornada.Intevalo_Almoco)
                        {
                            if (jornada.Tipo_Intervalo == Tipo_Intervalo_Jornada.Intervalo_Fixo)
                            {
                                horasPrevistas = GetHorasPrevistas(
                                                        jornada.Inicio_Feriado.Value,
                                                        jornada.Termino_Feriado.Value,
                                                        jornada.Inicio_Intervalo_Feriado.Value,
                                                        jornada.Termino_Intervalo_Feriado.Value
                                                    ).TotalHours;
                            }
                            else
                            {
                                horasPrevistas = GetHorasPrevistas(
                                                        jornada.Inicio_Feriado.Value,
                                                        jornada.Termino_Feriado.Value,
                                                        jornada.Duracao_Intervalo_Feriado.Value
                                                    ).TotalHours;
                            }
                        }
                        else
                        {
                            horasPrevistas = GetHorasPrevistas(jornada.Inicio_Feriado.Value, jornada.Termino_Feriado.Value).TotalHours;
                        }
                    }
                    else
                    {
                        horasPrevistas = jornada.Duracao_Feriado.Value;
                    }
                }
            }
            else
            {
                switch (data.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        if (jornada.TrabalhaNoDia(data.DayOfWeek))
                        {
                            if (jornada.Tipo_Jornada == Tipo_Jornada.Horarios_Fixos)
                            {
                                if (jornada.Intevalo_Almoco)
                                {
                                    if (jornada.Tipo_Intervalo == Tipo_Intervalo_Jornada.Intervalo_Fixo)
                                    {
                                        horasPrevistas = GetHorasPrevistas(
                                                            jornada.Inicio_Domingo.Value,
                                                            jornada.Termino_Domingo.Value,
                                                            jornada.Inicio_Intervalo_Domingo.Value,
                                                            jornada.Termino_Intervalo_Domingo.Value
                                                        ).TotalHours;
                                    }
                                    else
                                    {
                                        horasPrevistas = GetHorasPrevistas(
                                                            jornada.Inicio_Domingo.Value,
                                                            jornada.Termino_Domingo.Value,
                                                            jornada.Duracao_Intervalo_Domingo.Value
                                                        ).TotalHours;
                                    }
                                }
                                else
                                {
                                    horasPrevistas = GetHorasPrevistas(jornada.Inicio_Domingo.Value, jornada.Termino_Domingo.Value).TotalHours;
                                }
                            }
                            else
                            {
                                horasPrevistas = jornada.Duracao_Domingo.Value;
                            }
                        }
                        break;
                    case DayOfWeek.Monday:
                        if (jornada.TrabalhaNoDia(data.DayOfWeek))
                        {
                            if (jornada.Tipo_Jornada == Tipo_Jornada.Horarios_Fixos)
                            {
                                if (jornada.Intevalo_Almoco)
                                {
                                    if (jornada.Tipo_Intervalo == Tipo_Intervalo_Jornada.Intervalo_Fixo)
                                    {
                                        horasPrevistas = GetHorasPrevistas(
                                                            jornada.Inicio_Segunda.Value,
                                                            jornada.Termino_Segunda.Value,
                                                            jornada.Inicio_Intervalo_Segunda.Value,
                                                            jornada.Termino_Intervalo_Segunda.Value
                                                        ).TotalHours;
                                    }
                                    else
                                    {
                                        horasPrevistas = GetHorasPrevistas(
                                                            jornada.Inicio_Segunda.Value,
                                                            jornada.Termino_Segunda.Value,
                                                            jornada.Duracao_Intervalo_Segunda.Value
                                                        ).TotalHours;
                                    }
                                }
                                else
                                {
                                    horasPrevistas = GetHorasPrevistas(jornada.Inicio_Segunda.Value, jornada.Termino_Segunda.Value).TotalHours;
                                }
                            }
                            else
                            {
                                horasPrevistas = jornada.Duracao_Segunda.Value;
                            }
                        }
                        break;
                    case DayOfWeek.Tuesday:
                        if (jornada.TrabalhaNoDia(data.DayOfWeek))
                        {
                            if (jornada.Tipo_Jornada == Tipo_Jornada.Horarios_Fixos)
                            {
                                if (jornada.Intevalo_Almoco)
                                {
                                    if (jornada.Tipo_Intervalo == Tipo_Intervalo_Jornada.Intervalo_Fixo)
                                    {
                                        horasPrevistas = GetHorasPrevistas(
                                                            jornada.Inicio_Terca.Value,
                                                            jornada.Termino_Terca.Value,
                                                            jornada.Inicio_Intervalo_Terca.Value,
                                                            jornada.Termino_Intervalo_Terca.Value
                                                        ).TotalHours;
                                    }
                                    else
                                    {
                                        horasPrevistas = GetHorasPrevistas(
                                                            jornada.Inicio_Terca.Value,
                                                            jornada.Termino_Terca.Value,
                                                            jornada.Duracao_Intervalo_Terca.Value
                                                        ).TotalHours;
                                    }
                                }
                                else
                                {
                                    horasPrevistas = GetHorasPrevistas(jornada.Inicio_Terca.Value, jornada.Termino_Terca.Value).TotalHours;
                                }
                            }
                            else
                            {
                                horasPrevistas = jornada.Duracao_Terca.Value;
                            }
                        }
                        break;
                    case DayOfWeek.Wednesday:
                        if (jornada.TrabalhaNoDia(data.DayOfWeek))
                        {
                            if (jornada.Tipo_Jornada == Tipo_Jornada.Horarios_Fixos)
                            {
                                if (jornada.Intevalo_Almoco)
                                {
                                    if (jornada.Tipo_Intervalo == Tipo_Intervalo_Jornada.Intervalo_Fixo)
                                    {
                                        horasPrevistas = GetHorasPrevistas(
                                                            jornada.Inicio_Quarta.Value,
                                                            jornada.Termino_Quarta.Value,
                                                            jornada.Inicio_Intervalo_Quarta.Value,
                                                            jornada.Termino_Intervalo_Quarta.Value
                                                        ).TotalHours;
                                    }
                                    else
                                    {
                                        horasPrevistas = GetHorasPrevistas(
                                                            jornada.Inicio_Quarta.Value,
                                                            jornada.Termino_Quarta.Value,
                                                            jornada.Duracao_Intervalo_Quarta.Value
                                                        ).TotalHours;
                                    }
                                }
                                else
                                {
                                    horasPrevistas = GetHorasPrevistas(jornada.Inicio_Quarta.Value, jornada.Termino_Quarta.Value).TotalHours;
                                }
                            }
                            else
                            {
                                horasPrevistas = jornada.Duracao_Quarta.Value;
                            }
                        }
                        break;
                    case DayOfWeek.Thursday:
                        if (jornada.TrabalhaNoDia(data.DayOfWeek))
                        {
                            if (jornada.Tipo_Jornada == Tipo_Jornada.Horarios_Fixos)
                            {
                                if (jornada.Intevalo_Almoco)
                                {
                                    if (jornada.Tipo_Intervalo == Tipo_Intervalo_Jornada.Intervalo_Fixo)
                                    {
                                        horasPrevistas = GetHorasPrevistas(
                                                            jornada.Inicio_Quinta.Value,
                                                            jornada.Termino_Quinta.Value,
                                                            jornada.Inicio_Intervalo_Quinta.Value,
                                                            jornada.Termino_Intervalo_Quinta.Value
                                                        ).TotalHours;
                                    }
                                    else
                                    {
                                        horasPrevistas = GetHorasPrevistas(
                                                            jornada.Inicio_Quinta.Value,
                                                            jornada.Termino_Quinta.Value,
                                                            jornada.Duracao_Intervalo_Quinta.Value
                                                        ).TotalHours;
                                    }
                                }
                                else
                                {
                                    horasPrevistas = GetHorasPrevistas(jornada.Inicio_Quinta.Value, jornada.Termino_Quinta.Value).TotalHours;
                                }
                            }
                            else
                            {
                                horasPrevistas = jornada.Duracao_Quinta.Value;
                            }
                        }
                        break;
                    case DayOfWeek.Friday:
                        if (jornada.TrabalhaNoDia(data.DayOfWeek))
                        {
                            if (jornada.Tipo_Jornada == Tipo_Jornada.Horarios_Fixos)
                            {
                                if (jornada.Intevalo_Almoco)
                                {
                                    if (jornada.Tipo_Intervalo == Tipo_Intervalo_Jornada.Intervalo_Fixo)
                                    {
                                        horasPrevistas = GetHorasPrevistas(
                                                            jornada.Inicio_Sexta.Value,
                                                            jornada.Termino_Sexta.Value,
                                                            jornada.Inicio_Intervalo_Sexta.Value,
                                                            jornada.Termino_Intervalo_Sexta.Value
                                                        ).TotalHours;
                                    }
                                    else
                                    {
                                        horasPrevistas = GetHorasPrevistas(
                                                            jornada.Inicio_Sexta.Value,
                                                            jornada.Termino_Sexta.Value,
                                                            jornada.Duracao_Intervalo_Sexta.Value
                                                        ).TotalHours;
                                    }
                                }
                                else
                                {
                                    horasPrevistas = GetHorasPrevistas(jornada.Inicio_Sexta.Value, jornada.Termino_Sexta.Value).TotalHours;
                                }
                            }
                            else
                            {
                                horasPrevistas = jornada.Duracao_Sexta.Value;
                            }
                        }
                        break;
                    case DayOfWeek.Saturday:
                        if (jornada.TrabalhaNoDia(data.DayOfWeek))
                        {
                            if (jornada.Tipo_Jornada == Tipo_Jornada.Horarios_Fixos)
                            {
                                if (jornada.Intevalo_Almoco)
                                {
                                    if (jornada.Tipo_Intervalo == Tipo_Intervalo_Jornada.Intervalo_Fixo)
                                    {
                                        horasPrevistas = GetHorasPrevistas(
                                                            jornada.Inicio_Sabado.Value,
                                                            jornada.Termino_Sabado.Value,
                                                            jornada.Inicio_Intervalo_Sabado.Value,
                                                            jornada.Termino_Intervalo_Sabado.Value
                                                        ).TotalHours;
                                    }
                                    else
                                    {
                                        horasPrevistas = GetHorasPrevistas(
                                                            jornada.Inicio_Sabado.Value,
                                                            jornada.Termino_Sabado.Value,
                                                            jornada.Duracao_Intervalo_Sabado.Value
                                                        ).TotalHours;
                                    }
                                }
                                else
                                {
                                    horasPrevistas = GetHorasPrevistas(jornada.Inicio_Sabado.Value, jornada.Termino_Sabado.Value).TotalHours;
                                }
                            }
                            else
                            {
                                horasPrevistas = jornada.Duracao_Sabado.Value;
                            }
                        }
                        break;
                }
            }

            return horasPrevistas;

        }

        public static TimeSpan GetHorasPrevistas(DateTime inicio, DateTime termino, DateTime inicioIntervalo, DateTime terminoIntervalo)
        {
            if (termino < inicio)
                termino = termino.AddDays(1);

            if (terminoIntervalo < inicioIntervalo)
                terminoIntervalo = terminoIntervalo.AddDays(1);

            TimeSpan retorno = (termino - inicio) - (terminoIntervalo - inicioIntervalo);

            return retorno;
        }

        public static TimeSpan GetHorasPrevistas(DateTime inicio, DateTime termino, double duracaoIntervalo)
        {
            if (termino < inicio)
                termino = termino.AddDays(1);

            TimeSpan retorno = (termino - inicio) - TimeSpan.FromHours(duracaoIntervalo);

            return retorno;
        }

        public static TimeSpan GetHorasPrevistas(DateTime inicio, DateTime termino)
        {
            if (termino < inicio)
                termino = termino.AddDays(1);

            return termino - inicio;
        }

        public static async Task<Double> GetCreditoDia(String numFre, DateTime data)
        {
            double credito = 0;

            using (ISession session = HibernateUtil.GetSession())
            using (var com = session.Connection.CreateCommand())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();
                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT COALESCE(SUM(A.HORAS), 0) AS HORAS ");
                sql.AppendLine(" FROM TB_PT_BD_HORAS A ");
                sql.AppendLine(" WHERE A.NUM_FRE = @NUM_FRE ");
                sql.AppendLine("   AND A.DATA = @DATA ");
                sql.AppendLine("   AND A.TRANSACAO = 'C' ");
                sql.AppendLine("   AND A.TIPO_HORA = 'J' ");

                parametros.Add("@NUM_FRE", numFre);
                parametros.Add("@DATA", data);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        credito = Convert.ToDouble(reader["HORAS"]);
                    }
                }
            }

            return credito;
        }

        public static async Task<Double> GetDebitoDia(String numFre, DateTime data)
        {
            double debito = 0;

            using (ISession session = HibernateUtil.GetSession())
            using (var com = session.Connection.CreateCommand())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();
                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT COALESCE(SUM(A.HORAS), 0) AS HORAS ");
                sql.AppendLine(" FROM TB_PT_BD_HORAS A ");
                sql.AppendLine(" WHERE A.NUM_FRE = @NUM_FRE ");
                sql.AppendLine("   AND A.DATA = @DATA ");
                sql.AppendLine("   AND A.TRANSACAO = 'D' ");

                parametros.Add("@NUM_FRE", numFre);
                parametros.Add("@DATA", data.Date);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        debito = Convert.ToDouble(reader["HORAS"]);
                    }
                }
            }

            return debito;
        }

    }

}