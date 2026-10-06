using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.nhibernate;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.API_Aniel.Models
{
    public class Sintetico_Saldo_Banco_HorasListaModel
    {
        public String Data { get; set; }
        public String Id_Funcionario { get; set; }
        public String Matricula_Auxiliar { get; set; }
        public String Nome_Funcionario { get; set; }
        public Int64? Id_Departamento { get; set; }
        public String Descricao_Ct_Custo { get; set; }
        public Int64? Id_Empresa { get; set; }
        public String Empresa { get; set; }
        public Int64? Id_Filial { get; set; }
        public String Filial { get; set; }
        public String Id_Gestor { get; set; }
        public String Gestor { get; set; }
        public Int64? Id_Jornada { get; set; }
        public String Cod_Jornada { get; set; }
        public Int64? Id_Centro_Custo { get; set; }
        public Double Horas_Saldo_Credito { get; set; }
        public String Saldo_Credito { get; set; }
        public Double Horas_Saldo_Debito { get; set; }
        public String Saldo_Debito { get; set; }
        public Double Horas_Saldo_Final_Banco { get; set; }
        public String Saldo_Final_Banco { get; set; }
    }

    [Serializable]
    public class Tb_Pt_Fechamento_Periodo_Banco_Hora : BaseModel
    {
        public virtual DateTime? Data_Fechamento_Periodo_Anterior { get; set; }

        public virtual Int64? Duracao_Periodo { get; set; }

        public virtual Enum_Status_Periodo? Status { get; set; }

        public virtual DateTime? Data_Inicio { get; set; }

        public virtual DateTime? Data_Fechamento { get; set; }

        public virtual Int64? Teto_Periodo_Banco_Horas { get; set; }
    }

    public class RelatorioSinteticoRequest : ApiRequest
    {
        /// <summary> Data do Banco de Horas </summary>
        public String Data { get; set; }
        /// <summary> Id da Empresa </summary>
        public Int64? Id_Empresa { get; set; }
        /// <summary> Identificador do Filial </summary>
        public Int64? Id_Filial { get; set; }
        /// <summary> Identificador do Centro de Custo </summary>
        public Int64? Id_Centro_Custo { get; set; }
        /// <summary> Identificador do Departamento </summary>
        public Int64? Id_Departamento { get; set; }
        /// <summary> Identificador do Gestor </summary>
        public String Id_Gestor { get; set; }
        /// <summary> Identificador do Funcionário </summary>
        public String Id_Funcionario { get; set; }
        /// <summary> Identificador da Jornada de Trabalho </summary>
        public Int64? Id_Jornada { get; set; }
        /// <summary> Código da Função </summary>
        public String Cod_Funcao { get; set; }
    }

    public class Horas
    {
        public Horas_Extras Horas_Extras { get; set; }
        public Horas_Extras Horas_Extras_Compensadas { get; set; }

        public Horas_Adic_Noturno Horas_Adic_Noturno { get; set; }
        /// <summary>
        ///     Em jornadas com compensacao mensal, as horas extras em adicional noturno que
        ///     foram compensadas ainda precisam ser pagas e devem ser exibidas no total de
        ///     horas em adicional noturno
        /// </summary>
        public Double Horas_Adic_Noturno_Compensadas { get; set; }

        public Double Horas_Debito { get; set; }
        public Double Horas_Debito_Compensadas { get; set; }

        public Double Horas_Abonadas { get; set; }
        public Double Horas_Sobreaviso { get; set; }

        public Horas()
        {
            Horas_Extras = new Horas_Extras();
            Horas_Extras_Compensadas = new Horas_Extras();
            Horas_Adic_Noturno = new Horas_Adic_Noturno();
        }

        public Horas(Horas objOriginal)
        {
            Horas_Extras = new Horas_Extras(objOriginal.Horas_Extras);
            Horas_Extras_Compensadas = new Horas_Extras(objOriginal.Horas_Extras_Compensadas);

            Horas_Adic_Noturno = new Horas_Adic_Noturno(objOriginal.Horas_Adic_Noturno);
            Horas_Adic_Noturno_Compensadas = objOriginal.Horas_Adic_Noturno_Compensadas;

            Horas_Debito = objOriginal.Horas_Debito;
            Horas_Debito_Compensadas = objOriginal.Horas_Debito_Compensadas;

            Horas_Abonadas = objOriginal.Horas_Abonadas;
            Horas_Sobreaviso = objOriginal.Horas_Sobreaviso;
        }
    }

    public class Horas_Adic_Noturno
    {
        public Horas_Adic_Noturno()
        {
            Horas = new Dictionary<double, double>();
        }

        public Horas_Adic_Noturno(Horas_Adic_Noturno objOriginal)
        {
            He_Vinculada = objOriginal.He_Vinculada;
            Horas = new Dictionary<double, double>(objOriginal.Horas);
        }

        public Int64 He_Vinculada { get; set; }

        /// <summary> Percentual / Horas </summary>
        public Dictionary<double, double> Horas { get; set; }
    }

    public class Horas_Extras
    {
        public Horas_Extras()
        {
            Horas = new Dictionary<double, double>();
        }

        public Horas_Extras(Horas_Extras objOriginal)
        {
            Id = objOriginal.Id;
            Horas = new Dictionary<double, double>(objOriginal.Horas);
        }

        public Int64 Id { get; set; }
        /// <summary> Percentual / Horas </summary>
        public Dictionary<double, double> Horas { get; set; }
    }

    public class JornadaPeriodo
    {
        public Int64 Id_Jornada { get; set; }
        public DateTime Data_Inicio { get; set; }
        public DateTime Data_Fim { get; set; }
    }

    public class HorasBancoParams
    {
        public ISession session { get; set; }
        public DateTime dataFechto { get; set; }
        public String numFre { get; set; }
        public Boolean fechaMovtos { get; set; }

        // campos usados para emitir o ACJEF
        public Int64? idJornada { get; set; }
        public DayOfWeek? diaSemana { get; set; }
    }

    public class HorasDia : Horas
    {
        public DateTime Data { get; set; }

        public HorasDia()
        {
            Horas_Extras = new Horas_Extras();
            Horas_Adic_Noturno = new Horas_Adic_Noturno();
            Horas_Extras_Compensadas = new Horas_Extras();
        }

        public HorasDia(HorasDia objOriginal)
        {
            Data = objOriginal.Data;

            Horas_Extras = new Horas_Extras(objOriginal.Horas_Extras);
            Horas_Extras_Compensadas = new Horas_Extras(objOriginal.Horas_Extras_Compensadas);

            Horas_Adic_Noturno = new Horas_Adic_Noturno(objOriginal.Horas_Adic_Noturno);
            Horas_Adic_Noturno_Compensadas = objOriginal.Horas_Adic_Noturno_Compensadas;

            Horas_Debito = objOriginal.Horas_Debito;
            Horas_Debito_Compensadas = objOriginal.Horas_Debito_Compensadas;

            Horas_Abonadas = objOriginal.Horas_Abonadas;
            Horas_Sobreaviso = objOriginal.Horas_Sobreaviso;
        }
    }

    public class HorasFolha
    {
        public Int64 Id { get; set; }
        public DateTime Data { get; set; }
        public Char Transacao { get; set; }
        public Double Horas { get; set; }
        public Double Percentual { get; set; }
    }

    public class BancoHoras
    {
        public static async Task<Horas> GetHorasExtra(ISession session, String numFre, DateTime dataIni, DateTime dataFim, Boolean banco)
        {
            Horas horas = new Horas();

            List<JornadaPeriodo> jornadasPeriodo = await GetJornadasPeriodo(session, numFre, dataIni, dataFim);

            foreach (var jornada in jornadasPeriodo)
            {
                if (jornada.Id_Jornada != 0)
                {
                    Horas horasExtrasPeriodo = await GetHorasExtraPeriodo(session, numFre, dataIni, dataFim, jornada.Id_Jornada, banco);

                    foreach (KeyValuePair<double, double> horasExtras in horasExtrasPeriodo.Horas_Extras.Horas)
                    {
                        if (!horas.Horas_Extras.Horas.ContainsKey(horasExtras.Key))
                            horas.Horas_Extras.Horas.Add(horasExtras.Key, horasExtras.Value);
                        else
                            horas.Horas_Extras.Horas[horasExtras.Key] += horasExtras.Value;
                    }

                    foreach (KeyValuePair<double, double> horasExtrasCompensadas in horasExtrasPeriodo.Horas_Extras_Compensadas.Horas)
                    {
                        if (!horas.Horas_Extras_Compensadas.Horas.ContainsKey(horasExtrasCompensadas.Key))
                            horas.Horas_Extras_Compensadas.Horas.Add(horasExtrasCompensadas.Key, horasExtrasCompensadas.Value);
                        else
                            horas.Horas_Extras_Compensadas.Horas[horasExtrasCompensadas.Key] += horasExtrasCompensadas.Value;
                    }

                    foreach (KeyValuePair<double, double> heAdicNoturno in horasExtrasPeriodo.Horas_Adic_Noturno.Horas)
                    {
                        if (!horas.Horas_Adic_Noturno.Horas.ContainsKey(heAdicNoturno.Key))
                            horas.Horas_Adic_Noturno.Horas.Add(heAdicNoturno.Key, heAdicNoturno.Value);
                        else
                            horas.Horas_Adic_Noturno.Horas[heAdicNoturno.Key] += heAdicNoturno.Value;
                    }

                    horas.Horas_Adic_Noturno_Compensadas += horasExtrasPeriodo.Horas_Adic_Noturno_Compensadas;

                    horas.Horas_Debito += horasExtrasPeriodo.Horas_Debito;
                    horas.Horas_Debito_Compensadas += horasExtrasPeriodo.Horas_Debito_Compensadas;

                    horas.Horas_Sobreaviso = horasExtrasPeriodo.Horas_Sobreaviso;
                    horas.Horas_Abonadas = horasExtrasPeriodo.Horas_Abonadas;
                }
            }

            return horas;
        }

        public static async Task<List<JornadaPeriodo>> GetJornadasPeriodo(ISession session, String numFre, DateTime dataIni, DateTime dataFim)
        {
            List<JornadaPeriodo> retorno = new List<JornadaPeriodo>();

            using (var com = session.Connection.CreateCommand())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT A.ID_JORNADA, MIN(A.DATA) AS DATA_INICIO, MAX(A.DATA) AS DATA_FIM ");
                sql.AppendLine(" FROM TB_PT_DIA_FUNCIONARIO A ");
                sql.AppendLine(" WHERE A.NUM_FRE = @NUM_FRE ");
                sql.AppendLine("   AND A.DATA BETWEEN @DATA_INI AND @DATA_FIM ");

                sql.AppendLine(" GROUP BY ID_JORNADA ");

                parametros.Add("@NUM_FRE", numFre);
                parametros.Add("@DATA_INI", dataIni);
                parametros.Add("@DATA_FIM", dataFim);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        retorno.Add(new JornadaPeriodo
                        {
                            Id_Jornada = reader["ID_JORNADA"] != DBNull.Value ? Convert.ToInt64(reader["ID_JORNADA"]) : 0,
                            Data_Inicio = Convert.ToDateTime(reader["DATA_INICIO"]),
                            Data_Fim = Convert.ToDateTime(reader["DATA_FIM"])
                        });
                    }
                }
            }

            return retorno;
        }

        public static async Task<Double> GetTotalHorasBancoSql(HorasBancoParams args)
        {
            double horasTotais = 0;

            using (var com = args.session.Connection.CreateCommand())
            {
                Dictionary<String, Object> parametros = new Dictionary<String, Object>();

                StringBuilder sql = new StringBuilder(@" SELECT COALESCE(SUM(B.RHORAS),0) TOTAL
                                                         FROM SP_PT_BD_BANCOHORAS(@NUM_FRE, @DATA, NULL) B");

                parametros.Add("@DATA", args.dataFechto);
                parametros.Add("@NUM_FRE", args.numFre);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, args.session.Connection, null))
                {
                    if (reader.Read())
                    {
                        horasTotais = Convert.ToDouble(reader["TOTAL"]);
                    }
                }
            }
            return horasTotais;
        }

        public static async Task<Horas> GetHorasExtraPeriodo(ISession session, String numFre, DateTime dataIni, DateTime dataFim, Int64 idJornada, Boolean banco)
        {
            using (var repoJornada = new Conexao<Tb_Pt_Jornada>(session))
            {
                Horas horas = new Horas();

                List<HorasDia> listHorasDia = new List<HorasDia>();
                List<HorasFolha> horasFolha = new List<HorasFolha>();

                if (banco)
                {
                    horasFolha = await GetHorasEmBanco(session, numFre, dataIni, dataFim, idJornada);
                }
                else
                {
                    horasFolha = await GetHorasFolha(session, numFre, dataIni, dataFim, idJornada);
                }

                foreach (var itemHoras in horasFolha)
                {
                    HorasDia horasDia = new HorasDia
                    {
                        Data = itemHoras.Data,
                        Horas_Extras = new Horas_Extras(),
                        Horas_Adic_Noturno = new Horas_Adic_Noturno()
                    };

                    if (itemHoras.Transacao == 'D')
                    {
                        horasDia.Horas_Debito += itemHoras.Horas;
                    }
                    else
                    {
                        horasDia.Horas_Extras.Id = itemHoras.Id;
                        horasDia.Horas_Extras.Horas.Add(itemHoras.Percentual, itemHoras.Horas);

                        var horasPerc = await GetHeAdicionalNoturno(session, numFre, dataIni, dataFim, itemHoras.Id);

                        foreach (var itemHENoturnas in horasPerc)
                        {
                            horasDia.Horas_Adic_Noturno.He_Vinculada = itemHoras.Id;
                            horasDia.Horas_Adic_Noturno.Horas.Add(itemHENoturnas.Key, itemHENoturnas.Value);
                        }
                    }

                    listHorasDia.Add(horasDia);
                }

                double debito = listHorasDia.Sum(p => p.Horas_Debito);
                double debitoCompensado = debito;

                #region Descontar debito do credito (levando a faixa em consideracao)

                List<HorasDia> listHorasDiaOriginal = new List<HorasDia>();

                foreach (var horasDia in listHorasDia)
                {
                    listHorasDiaOriginal.Add(new HorasDia(horasDia));
                }

                Tb_Pt_Jornada jornada = await repoJornada.selectFirst(p => p.Id == idJornada);

                if (jornada.Trabalha_Compensacao && jornada.Forma_Compensacao_Horas == Enum_Forma_Compensacao.Mensal)
                {
                    if (jornada.Ordem_Compensacao.GetValueOrDefault(Ordem_Compensacao.Data) == Ordem_Compensacao.Data)
                    {
                        Horas_Extras horasExtrasMaisAntigas = null;

                        foreach (HorasDia horasDia in listHorasDiaOriginal.OrderBy(p => p.Data))
                        {
                            if (jornada.Ordem_Compensacao.GetValueOrDefault(Ordem_Compensacao.Data) == Ordem_Compensacao.Data)
                            {
                                if (horasExtrasMaisAntigas == null)
                                {
                                    horasExtrasMaisAntigas = (from dia in listHorasDia
                                                              where dia.Horas_Extras.Horas.Count > 0
                                                              orderby dia.Data
                                                              select dia.Horas_Extras).FirstOrDefault();

                                    if (horasExtrasMaisAntigas == null)
                                        break;
                                }

                                Horas_Extras horasExtrasAntigasOriginal = new Horas_Extras(horasExtrasMaisAntigas);

                                Horas_Adic_Noturno horasAdicNoturno = null;
                                Horas_Adic_Noturno horasAdicNoturnoCopy = null;

                                foreach (KeyValuePair<double, double> horasAntigas in horasExtrasAntigasOriginal.Horas)
                                {
                                    horasExtrasMaisAntigas.Horas[horasAntigas.Key] = (debitoCompensado > horasAntigas.Value) ? 0 : horasAntigas.Value - debitoCompensado;
                                    debitoCompensado = (horasAntigas.Value > debitoCompensado) ? 0 : debitoCompensado - horasAntigas.Value;

                                    if (debitoCompensado > 0)
                                    {
                                        #region Compensação do Adicional Noturno por Data

                                        horasAdicNoturno = (from dia in listHorasDia
                                                            where dia.Horas_Adic_Noturno.He_Vinculada == horasExtrasMaisAntigas.Id
                                                            select dia.Horas_Adic_Noturno).FirstOrDefault();

                                        if (horasAdicNoturno != null)
                                        {
                                            horasAdicNoturnoCopy = new Horas_Adic_Noturno(horasAdicNoturno);

                                            foreach (KeyValuePair<double, double> adicNoturno in horasAdicNoturnoCopy.Horas)
                                            {
                                                double percAdicNoturno = adicNoturno.Key;
                                                double horasAntigasAdicNoturno = horasAdicNoturno.Horas[percAdicNoturno];

                                                horasAdicNoturno.Horas[percAdicNoturno] = (debitoCompensado > horasAdicNoturno.Horas[percAdicNoturno]) ? 0 : horasAdicNoturno.Horas[percAdicNoturno] - debitoCompensado;
                                                horasDia.Horas_Adic_Noturno_Compensadas += horasAntigasAdicNoturno - horasAdicNoturno.Horas[percAdicNoturno];
                                            }
                                        }

                                        #endregion
                                    }
                                }

                                if (horasExtrasMaisAntigas.Horas.Values.Sum() == 0)
                                {
                                    var diaHoraExtraMaisAntiga = (from dia in listHorasDia
                                                                  where dia.Horas_Extras.Horas.Count > 0
                                                                  orderby dia.Data
                                                                  select dia).First();

                                    diaHoraExtraMaisAntiga.Horas_Extras.Horas.Clear();
                                    horasExtrasMaisAntigas = null;
                                }

                                if (horasAdicNoturno != null && horasAdicNoturno.Horas.Values.Sum() == 0)
                                {
                                    horasAdicNoturno.Horas.Clear();
                                }
                            }

                            if (debitoCompensado <= 0)
                                break;
                        }

                        #region Conversao p/ o retorno

                        var horasExtrasCompensadas = listHorasDia.SelectMany(p => p.Horas_Extras.Horas);

                        foreach (double percentual in horasExtrasCompensadas.Select(p => p.Key).Distinct())
                        {
                            double totalHorasPercentual = horasExtrasCompensadas.Where(p => p.Key == percentual).Sum(p => p.Value);
                            horas.Horas_Extras_Compensadas.Horas.Add(percentual, totalHorasPercentual);
                        }

                        var horasExtrasAdicNoturno = listHorasDia.SelectMany(p => p.Horas_Adic_Noturno.Horas);

                        foreach (double percentual in horasExtrasAdicNoturno.Select(p => p.Key).Distinct())
                        {
                            double totalHorasPercentual = horasExtrasAdicNoturno.Where(p => p.Key == percentual).Sum(p => p.Value);
                            horas.Horas_Adic_Noturno.Horas.Add(percentual, totalHorasPercentual);
                        }

                        #endregion
                    }
                    else
                    {
                        var listHorasExtras = listHorasDia.SelectMany(p => p.Horas_Extras.Horas).OrderBy(p => p.Key);

                        foreach (var perc in listHorasExtras.Select(p => p.Key).Distinct())
                        {
                            double totalHorasPerc = listHorasExtras.Where(p => p.Key == perc).Sum(p => p.Value);

                            if (debitoCompensado > 0)
                            {
                                if (totalHorasPerc > debitoCompensado)
                                {
                                    totalHorasPerc -= debitoCompensado;
                                    debitoCompensado = 0;
                                }
                                else
                                {
                                    debitoCompensado -= totalHorasPerc;
                                    totalHorasPerc = 0;
                                }
                            }

                            horas.Horas_Extras_Compensadas.Horas.Add(perc, totalHorasPerc);

                            if (debitoCompensado > 0)
                            {
                                #region Compensação do Adicional Noturno por Percentual

                                var listHorasAdicNoturno = listHorasDia.SelectMany(p => p.Horas_Adic_Noturno.Horas).OrderBy(p => p.Key);

                                foreach (var percAdicNoturno in listHorasAdicNoturno.Select(p => p.Key).Distinct())
                                {
                                    double horasAdicNoturnoCompensadas = 0;
                                    double totalHorasPercAdicNoturno = listHorasAdicNoturno.Where(p => p.Key == percAdicNoturno).Sum(p => p.Value);

                                    if (totalHorasPercAdicNoturno > debitoCompensado)
                                    {
                                        horasAdicNoturnoCompensadas = debitoCompensado;

                                        totalHorasPercAdicNoturno -= debitoCompensado;
                                        debitoCompensado = 0;
                                    }
                                    else
                                    {
                                        horasAdicNoturnoCompensadas = totalHorasPercAdicNoturno;

                                        debitoCompensado -= totalHorasPercAdicNoturno;
                                        totalHorasPercAdicNoturno = 0;
                                    }

                                    horas.Horas_Adic_Noturno_Compensadas += horasAdicNoturnoCompensadas;

                                    if (totalHorasPercAdicNoturno > 0)
                                        horas.Horas_Adic_Noturno.Horas.Add(percAdicNoturno, totalHorasPercAdicNoturno);

                                    if (debitoCompensado == 0)
                                        break;
                                }

                                #endregion
                            }
                        }
                    }
                }

                #endregion

                var horasExtras = listHorasDiaOriginal.SelectMany(p => p.Horas_Extras.Horas);

                foreach (double percentual in horasExtras.Select(p => p.Key).Distinct())
                {
                    double totalHorasPercentual = horasExtras.Where(p => p.Key == percentual).Sum(p => p.Value);
                    horas.Horas_Extras.Horas.Add(percentual, totalHorasPercentual);
                }

                horas.Horas_Adic_Noturno_Compensadas = listHorasDia.Sum(p => p.Horas_Adic_Noturno_Compensadas);

                horas.Horas_Debito = debito;
                horas.Horas_Debito_Compensadas = debitoCompensado;

                horas.Horas_Abonadas = await GetHorasAbonadas(numFre, dataIni, dataFim);
                horas.Horas_Sobreaviso = await GetHorasSobreaviso(numFre, dataIni, dataFim);

                return horas;
            }
        }

        public static async Task<List<HorasFolha>> GetHorasEmBanco(ISession session, String numFre, DateTime dataIni, DateTime dataFim, Int64? idJornada = null)
        {
            List<HorasFolha> retorno = new List<HorasFolha>();
            Dictionary<string, object> parametros = new Dictionary<string, object>();

            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT A.ID, A.DATA, A.TRANSACAO, (CASE ");
                sql.AppendLine("                                        WHEN C.BASE_CONVERSAO_HORAS = 0 THEN A.HORAS ");
                sql.AppendLine("                                        ELSE A.HORAS_FINAIS ");
                sql.AppendLine("                                    END) AS HORAS, A.PERCENTUAL ");
                sql.AppendLine(" FROM TB_PT_BD_HORAS A ");
                sql.AppendLine(" INNER JOIN TB_PT_DIA_FUNCIONARIO B ");
                sql.AppendLine(" ON(B.ID = A.ID_DIA_FUNCIONARIO) ");
                sql.AppendLine(" INNER JOIN TB_PT_JORNADA_FUNCIONARIO C ");
                sql.AppendLine(" ON(C.ID = B.ID_JORNADA) ");
                sql.AppendLine(" WHERE A.NUM_FRE = @NUM_FRE ");
                sql.AppendLine("   AND CAST(A.DATA AS DATE) BETWEEN @DATA_INI AND @DATA_FIM ");
                sql.AppendLine("   AND A.TIPO_MOVTO = 'B' ");
                sql.AppendLine("   AND A.TIPO_HORA = 'J' ");
                sql.AppendLine("   AND A.ID_DIA_FUNCIONARIO IS NOT NULL");

                if (idJornada.HasValue)
                {
                    sql.AppendLine("   AND B.ID_JORNADA = @ID_JORNADA ");
                    parametros.Add("@ID_JORNADA", idJornada.Value);
                }

                parametros.Add("@NUM_FRE", numFre);
                parametros.Add("@DATA_INI", dataIni);
                parametros.Add("@DATA_FIM", dataFim);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        retorno.Add(new HorasFolha
                        {
                            Id = Convert.ToInt64(reader["ID"]),
                            Data = Convert.ToDateTime(reader["DATA"]),
                            Transacao = Convert.ToChar(reader["TRANSACAO"]),
                            Percentual = Convert.ToDouble(reader["PERCENTUAL"]),
                            Horas = Convert.ToDouble(reader["HORAS"])
                        });
                    }
                }
            }

            return retorno;
        }

        public static async Task<Dictionary<Double, Double>> GetHeAdicionalNoturno(ISession session, String numFre, DateTime dataIni, DateTime dataFim, Int64 idHoras)
        {
            Dictionary<double, double> retorno = new Dictionary<double, double>();
            Dictionary<string, object> parametros = new Dictionary<string, object>();

            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT B.PERCENTUAL, COALESCE(SUM(B.HORAS), 0) AS HORAS ");
                sql.AppendLine(" FROM TB_PT_BD_HORAS B ");
                sql.AppendLine(" WHERE B.HE_VINCULADA = @ID_HORAS ");

                sql.AppendLine(" AND B.NUM_FRE = @NUM_FRE ");
                sql.AppendLine("   AND CAST(B.DATA AS DATE) BETWEEN @DATA_INI AND @DATA_FIM ");
                sql.AppendLine("   AND B.TIPO_MOVTO <> 'A' ");
                sql.AppendLine("   AND B.TIPO_HORA = 'N' ");
                sql.AppendLine("   GROUP BY B.PERCENTUAL, B.HORAS ");

                parametros.Add("@NUM_FRE", numFre);
                parametros.Add("@ID_HORAS", idHoras);
                parametros.Add("@DATA_INI", dataIni);
                parametros.Add("@DATA_FIM", dataFim);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        double percentual = Convert.ToDouble(reader["PERCENTUAL"]);
                        retorno.Add(DateTimeUtil.RemoveSeconds(percentual), Convert.ToDouble(reader["HORAS"]));
                    }
                }
            }

            return retorno;
        }

        public static async Task<List<HorasFolha>> GetHorasFolha(ISession session, String numFre, DateTime dataIni, DateTime dataFim, Int64? idJornada = null)
        {
            List<HorasFolha> retorno = new List<HorasFolha>();
            Dictionary<string, object> parametros = new Dictionary<string, object>();

            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT A.ID, A.DATA, A.TRANSACAO, A.HORAS, A.PERCENTUAL ");
                sql.AppendLine(" FROM TB_PT_BD_HORAS A ");

                sql.AppendLine(" INNER JOIN TB_PT_DIA_FUNCIONARIO B ");
                sql.AppendLine(" ON (B.ID = A.ID_DIA_FUNCIONARIO) ");

                sql.AppendLine(" WHERE A.NUM_FRE = @NUM_FRE ");
                sql.AppendLine("   AND CAST(A.DATA AS DATE) BETWEEN @DATA_INI AND @DATA_FIM ");
                sql.AppendLine("   AND A.TIPO_MOVTO <> 'A' ");
                sql.AppendLine("   AND A.TIPO_HORA = 'J' ");
                sql.AppendLine("   AND A.ORIGEM_MOVTO <> 'F' ");
                sql.AppendLine("   AND A.ID_DIA_FUNCIONARIO IS NOT NULL");

                if (idJornada.HasValue)
                {
                    sql.AppendLine("   AND B.ID_JORNADA = @ID_JORNADA ");
                    parametros.Add("@ID_JORNADA", idJornada.Value);
                }

                parametros.Add("@NUM_FRE", numFre);
                parametros.Add("@DATA_INI", dataIni);
                parametros.Add("@DATA_FIM", dataFim);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        retorno.Add(new HorasFolha
                        {
                            Id = Convert.ToInt64(reader["ID"]),
                            Data = Convert.ToDateTime(reader["DATA"]),
                            Transacao = Convert.ToChar(reader["TRANSACAO"]),
                            Percentual = Convert.ToDouble(reader["PERCENTUAL"]),
                            Horas = DateTimeUtil.RemoveSeconds(Convert.ToDouble(reader["HORAS"]))
                        });
                    }
                }
            }

            return retorno;
        }

        public static async Task<Double> GetHorasAbonadas(String numFre, DateTime dataIni, DateTime dataFim)
        {
            Double retorno = 0;
            Dictionary<string, object> parametros = new Dictionary<string, object>();

            using (ISession session = HibernateUtil.GetSession())
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT SUM(A.HORAS) AS HORAS ");
                sql.AppendLine(" FROM TB_PT_BD_HORAS A ");
                sql.AppendLine(" WHERE A.NUM_FRE = @NUM_FRE ");
                sql.AppendLine("   AND A.DATA BETWEEN @DATA_INI and @DATA_FIM ");
                sql.AppendLine("   AND A.TIPO_MOVTO = 'A' ");

                parametros.Add("@NUM_FRE", numFre);
                parametros.Add("@DATA_INI", dataIni);
                parametros.Add("@DATA_FIM", dataFim);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        if (reader["HORAS"] != DBNull.Value)
                            retorno = Convert.ToDouble(reader["HORAS"]);
                    }
                }
            }

            return DateTimeUtil.RemoveSeconds(retorno);
        }

        public static async Task<Double> GetHorasSobreaviso(String numFre, DateTime dataIni, DateTime dataFim)
        {
            Double retorno = 0;
            Dictionary<string, object> parametros = new Dictionary<string, object>();

            using (ISession session = HibernateUtil.GetSession())
            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();

                sql.AppendLine(" SELECT COALESCE(SUM(D.HORAS), 0) AS HORAS ");
                sql.AppendLine(" FROM TB_PT_BD_HORAS D ");
                sql.AppendLine(" WHERE D.NUM_FRE = @NUM_FRE ");
                sql.AppendLine("   AND CAST(D.DATA AS DATE) BETWEEN @DATA_INI AND @DATA_FIM ");
                sql.AppendLine("   AND D.TIPO_HORA = 'S' ");
                sql.AppendLine("   AND D.TIPO_MOVTO = 'F' ");

                parametros.Add("@NUM_FRE", numFre);
                parametros.Add("@DATA_INI", dataIni);
                parametros.Add("@DATA_FIM", dataFim);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        retorno = Convert.ToDouble(reader["HORAS"]);
                    }
                }
            }

            return DateTimeUtil.RemoveSeconds(retorno);
        }

    }

}