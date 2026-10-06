using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.IdentityModel.Tokens;
using NHibernate;
using Resources;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerado;
using Sinapse.Model.enumerados;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;


namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class Escala_TrabalhoApiController : BaseApiController
    {
        [HttpPost()]
        [Route("api/Escala_Trabalhoapi/GetAll")]
        public async Task<DataTableResponse<Escala_TrabalhoListaModel>> GetAll(Escala_Trabalho_DataTableRequest request)
        {
            List<Escala_TrabalhoListaModel> retorno = new List<Escala_TrabalhoListaModel>();
            IOrderedEnumerable<Escala_TrabalhoListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession sessionConnect = HibernateUtil.GetSession())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(sessionConnect))
            using (ISession session = HibernateUtil.GetSessionT5())
            using (var repoHorarioConnect = new Conexao<Horario_Connect>(session))
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                var PadraoUtilizaPoint = repoConfig.selectFirst().Result?.UtilizaPoint_RoteirizacaoAgendamento;
                Horario_Connect horarioPadrao = repoHorarioConnect.selectFirst(p => p.Tipo == Aniel.T5.Model.enumerado.Tipo_Hora_Connect.Horario_Servico && p.Padrao == true && p.Status == Status_Default.Ativo).Result;

                if (horarioPadrao == null)
                {
                    throw new Exception("Horário padrão de serviços não definido em configurações");
                }

                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.EQUIPE) CO FROM TB_EQUIPE A")
                        .AppendLine(" WHERE A.EQUIPE_CONNECT = 'S'");

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder()

                     .AppendLine("SELECT B.EQUIPE, A.NOME, A.SITUACAO, A.HORARIO_ATRIBUIDO,A.NUM_FRE,A.SUPERVISOR,B.USUARIO_MONITOR")
                     .AppendLine(" FROM TB_FUNCIONARIO A")

                     .AppendLine(" LEFT JOIN TB_EQUIPE B ")
                     .AppendLine(" ON(A.NUM_FRE = B.NUM_FRE)")

                     .AppendLine(" WHERE B.EQUIPE_CONNECT = 'S'");


                    //Filtrando
                    if (request.Situacao_Equipe != null)
                    {
                        sql.AppendLine("AND A.SITUACAO = @SITUACAO ");
                        parametros.Add("@SITUACAO", request.Situacao_Equipe);
                    }

                    if (request.IdSupervisor != null)
                    {
                        sql.AppendLine("AND B.COD_SUPERV = @IDSUPERVISOR");
                        parametros.Add("@IDSUPERVISOR", request.IdSupervisor);
                    }

                    if (request.IdMonitor != null)
                    {
                        sql.AppendLine("AND B.USUARIO_MONITOR = @IDMONITOR ");
                        parametros.Add("@IDMONITOR", request.IdMonitor);
                    }

                    if (request.IdTecnico != null)
                    {
                        sql.AppendLine("AND B.EQUIPE = @IDTECNICO ");
                        parametros.Add("@IDTECNICO", request.IdTecnico);
                    }




                    if (!String.IsNullOrEmpty(request.search.value))
                    {
                        sql.AppendLine(" AND ((UPPER(A.NOME) LIKE UPPER(@SEARCH))) ");
                        parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                    }

                    //Busca com paginacao

                    var obj = new Escala_TrabalhoListaModel();

                    using (ISession sessionNh = HibernateUtil.GetSessionT5())
                    {

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                try
                                {
                                    var horarioAtribuido = Convert.ToString(reader["HORARIO_ATRIBUIDO"]);
                                    if (String.IsNullOrEmpty(horarioAtribuido))
                                    {
                                        horarioAtribuido = Convert.ToString(PadraoUtilizaPoint.GetValueOrDefault(false) ? (int)HorarioAtribuido.AnielPoint : (int)HorarioAtribuido.AnielConnect);
                                    }

                                    obj = new Escala_TrabalhoListaModel
                                    {

                                        Equipe = Convert.ToString(reader["EQUIPE"]),
                                        Numfre = Convert.ToString(reader["NUM_FRE"]),
                                        Nome = Convert.ToString(reader["NOME"]),
                                        Situacao = HtmlEnumExtensions.GetEnumDescription<Situacao_Escala_Trabalho>(Convert.ToString(reader["SITUACAO"])),
                                        HorarioServicoAtribuido = HtmlEnumExtensions.GetEnumDescription<HorarioAtribuido>(horarioAtribuido),
                                        HorarioServicoAtribuidoValor = horarioAtribuido

                                    };

                                    PreencheDias(sessionNh, obj, horarioPadrao, request.Ano, Convert.ToInt32(request.Mes));

                                    retorno.Add(obj);
                                }
                                catch
                                {

                                }
                            }
                        }

                    }
                }
            }

            //Ordenacao
            if (request.order != null && request.order.Any())
            {
                int sortColumnIndex = request.order[0].column;
                string sortDirection = request.order[0].dir;
                switch (sortColumnIndex)
                {
                    case 1:     // Equipe
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Equipe);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Equipe);
                            break;
                        }
                    case 2:     // Nome
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Nome);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Nome);
                            break;
                        }
                    default:
                        {
                            listaOrdenada = retorno.OrderBy(p => p.Nome);
                            break;
                        }
                }
            }



            //Page e retornando DataTable
            return new DataTableResponse<Escala_TrabalhoListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        [HttpPost()]
        [Route("api/Escala_Trabalhoapi/RelatorioEscalaTrabalho")]
        public async Task<DataTableResponse<EscalaTrabalhoRelatorioModel>> RelatorioEscalaTrabalho(RelatorioEscalaTrabalhoDataTableRequest request)
        {
            DateTime dataInicioPeriodo;
            DateTime dataFimPeriodo;

            List<EscalaTrabalhoRelatorioModel> retorno = new List<EscalaTrabalhoRelatorioModel>();
            List<EscalaTrabalhoRelatorioModel> listaOrdenada = null;
            Int32 recordsTotal = 0;
           
            
            try
            {
            
                if (DateTime.TryParse(request.DataInicioPeriodo, out dataInicioPeriodo) == false || DateTime.TryParse(request.DataFimPeriodo, out dataFimPeriodo) == false)
                {
                throw new Exception("Período é obrigatório");
                }

                using (ISession sessionConnect = HibernateUtil.GetSession())
                using (var repoConfig = new Conexao<Configuracoes_Gerais>(sessionConnect))
                using (ISession sessionNH = HibernateUtil.GetSessionT5())
                using (var repoHorarioEquipe = new Conexao<Horario_Equipe>(sessionNH))
                using (var repoPerfilAgendamento = new Conexao<PerfilAgendamento>(sessionNH))
                using (ISession session = HibernateUtil.GetSessionT5())
                using (var repoHorarioConnect = new Conexao<Horario_Connect>(session))
                {

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    var horarios = (await repoHorarioEquipe.select(p => p.Data >= dataInicioPeriodo && p.Data <= dataFimPeriodo)).ToList();
                    var perfisAgendamento = (await repoPerfilAgendamento.select()).ToList();
                    var PadraoUtilizaPoint = repoConfig.selectFirst().Result?.UtilizaPoint_RoteirizacaoAgendamento;
                    Horario_Connect horarioPadrao = repoHorarioConnect.selectFirst(p => p.Tipo == Aniel.T5.Model.enumerado.Tipo_Hora_Connect.Horario_Servico && p.Padrao == true && p.Status == Status_Default.Ativo).Result;

                    if (horarioPadrao == null)
                    {
                        throw new Exception("Horário padrão de serviços não definido em configurações");
                    }


                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {

                        StringBuilder sql = new StringBuilder()

                         .AppendLine("SELECT B.EQUIPE, A.NOME, A.SITUACAO, A.HORARIO_ATRIBUIDO,A.NUM_FRE,A.SUPERVISOR,B.USUARIO_MONITOR,C.NOME AS NOME_SUPERVISOR," +
                                     "D.DESCRICAO AS PERFIL_AGENDAMENTO, D.ID AS ID_PERFIL_AGENDAMENTO,E.DESCRICAO AS PONTO_PARTIDA")
                         .AppendLine(" FROM TB_FUNCIONARIO A")

                         .AppendLine(" LEFT JOIN TB_EQUIPE B ")
                         .AppendLine(" ON(A.NUM_FRE = B.NUM_FRE)")
                         .AppendLine(" LEFT JOIN TB_FUNCIONARIO C ")
                         .AppendLine(" ON(B.COD_SUPERV = C.NUM_FRE)")
                         .AppendLine(" LEFT JOIN TB_PERFIL_AGENDAMENTO D ")
                         .AppendLine(" ON(B.ID_PERFIL_AGENDAMENTO = D.ID)")
                         .AppendLine("LEFT JOIN TB_PONTO_PARTIDA E")
                         .AppendLine("ON(B.ID_PONTO_PARTIDA = E.ID)")
                         .AppendLine(" WHERE B.EQUIPE_CONNECT = 'S' ")

                         //Filtrando

                         .AppendLine(" AND A.SITUACAO <> 2 AND B.ENCERRAMENTO IS NULL "); // Técnicos demitidos ou com data de encerramento.

                        if (request.Situacao_Equipe != null)
                        {
                            sql.AppendLine("AND A.SITUACAO = @SITUACAO ");
                            parametros.Add("@SITUACAO", request.Situacao_Equipe);
                        }

                        if (request.IdSupervisor != null)
                        {
                            sql.AppendLine("AND B.COD_SUPERV = @IDSUPERVISOR");
                            parametros.Add("@IDSUPERVISOR", request.IdSupervisor);
                        }

                        if (request.IdMonitor != null)
                        {
                            sql.AppendLine("AND B.USUARIO_MONITOR = @IDMONITOR ");
                            parametros.Add("@IDMONITOR", request.IdMonitor);
                        }

                        if (request.IdTecnico != null)
                        {
                            sql.AppendLine("AND B.EQUIPE = @IDTECNICO ");
                            parametros.Add("@IDTECNICO", request.IdTecnico);
                        }




                        if (!String.IsNullOrEmpty(request.search.value))
                        {
                            sql.AppendLine(" AND ((UPPER(A.NOME) LIKE UPPER(@SEARCH))) ");
                            parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                        }

                        //Busca com paginacao
                        using (ISession sessionNh = HibernateUtil.GetSessionT5())
                        {

                            using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                            {
                                while (await reader.ReadAsync())
                                {

                                    var matricula = Convert.ToString(reader["EQUIPE"]);
                                    var tecnico = Convert.ToString(reader["NOME"]);
                                    var supervisor = Convert.ToString(reader["NOME_SUPERVISOR"]);
                                    var perfilAgendamento = Convert.ToString(reader["PERFIL_AGENDAMENTO"]);
                                    var idPerfilAgendamento = reader["ID_PERFIL_AGENDAMENTO"] != DBNull.Value ? Convert.ToInt64(reader["ID_PERFIL_AGENDAMENTO"]) : (long?)null;
                                    var pontoPartida = Convert.ToString(reader["PONTO_PARTIDA"]);
                                    var diaPreenchido = PreencheDiasRelatorio(sessionNh, matricula, tecnico, supervisor, perfilAgendamento, idPerfilAgendamento, horarioPadrao, dataInicioPeriodo, dataFimPeriodo, horarios);
                                    diaPreenchido = diaPreenchido.Select(p => {
                                        p.PontoPartida = pontoPartida;
                                        return p;
                                    }).ToList();

                                    retorno.AddRange(diaPreenchido);

                                }
                            }

                        }
                    }

                    if (String.IsNullOrEmpty(request.CodEscritorio) == false)
                    {
                        retorno = retorno.Select(p =>
                        {

                            var perfilVinculado = perfisAgendamento.Find(y => y.Id == p.IdPerfilRoteirizacao);
                            if (perfilVinculado == null)
                            {
                                return null;
                            }

                            if (perfilVinculado.Areas.Where(y => y.Escritorio == request.CodEscritorio).Count() == 0)
                            {
                                return null;
                            }

                            if (String.IsNullOrEmpty(request.CodArea) == false)
                            {
                                if (perfilVinculado.Areas.Where(y => y.Escritorio == request.CodEscritorio && y.Area == request.CodArea).Count() == 0)
                                {
                                    return null;
                                }

                            }

                            return p;

                        }).Where(p => p != null).ToList();
                    }


                    if (String.IsNullOrEmpty(request.Situacao) == false)
                    {
                        retorno = retorno.Where(p => p.Indisponivel == request.Situacao).ToList();
                    }
                }


                recordsTotal = retorno.Count;
                listaOrdenada = retorno.OrderBy(p => p.Tecnico).Skip(request.start).Take(request.length).ToList();
            

            //Page e retornando DataTable
            return new DataTableResponse<EscalaTrabalhoRelatorioModel>
            {
                draw = request.draw,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

            }
            catch (Exception ex)
            {
          
                return new DataTableResponse<EscalaTrabalhoRelatorioModel>
                {
                    
                    draw = request.draw,
                    recordsFiltered = recordsTotal,
                    data = listaOrdenada,
                    error = ex.Message
                };
            }
        }

    

        private List<EscalaTrabalhoRelatorioModel> PreencheDiasRelatorio(ISession session, string matricula, string tecnico, string supervisor, string perfilAgendamento,
                                                                         long? idPerfilAgendamento, Horario_Connect horarioPadrao, DateTime dataInicioPeriodo, DateTime dataFimPeriodo,
                                                                         List<Horario_Equipe> horarios_equipeBanco)
        {
            List<EscalaTrabalhoRelatorioModel> retorno = new List<EscalaTrabalhoRelatorioModel>();
            using (FeriadoApiController feriadoapi = new FeriadoApiController())
            using (var repositoryHoraEquipe = new Conexao<Horario_Equipe>(session))
            {
                List<Horario_Equipe> horarios = horarios_equipeBanco.Where(p => p.Equipe.Matricula == matricula).OrderByDescending(p => p.Data).ToList();


                if (horarios.Count() == 0)
                {
                    //Preenche o horario padrão

                    for (DateTime j = dataInicioPeriodo; j <= dataFimPeriodo; j = j.AddDays(1))
                    {
                        IEnumerable<int> feriados = feriadoapi.GetDiasFeriadosMesAno(j.Month, j.Year).Result;
                        retorno.Add(
                            PreencheDiaRelatorio(
                                horarioPadrao, 
                                null, 
                                String.Format("{0:D2}", j.Day), 
                                j.Month, 
                                j.Year, 
                                false, 
                                feriados
                            )
                        );
                    }

                }
                else //Caso equipe tenha alguma alteração de horario
                {
                    DateTime primeirodia_do_mes = Convert.ToDateTime(String.Format("01/{0}", DateTime.Now.ToString("MM/yyyy")));
                    Horario_Connect ultimoHorario = horarioPadrao;

                    List<Horario_Equipe> ultimoHorarioMesPassado = horarios.FindAll(p => p.Data < primeirodia_do_mes && p.ApenasNoDia == false).ToList();
                    if (ultimoHorarioMesPassado.Count() > 0)
                        horarioPadrao = ultimoHorarioMesPassado.First().Horario;


                    //Preenche o horario padrão
                    for (DateTime j = dataInicioPeriodo; j <= dataFimPeriodo; j = j.AddDays(1))
                    {
                        IEnumerable<int> feriados = feriadoapi.GetDiasFeriadosMesAno(j.Month, j.Year).Result;
                        Horario_Equipe horarioNoDia = horarios.Find(p => p.Data == j.Date);

                        if (horarioNoDia != null)
                        {
                            if (horarioNoDia.ApenasNoDia)
                            {
                                retorno.Add(
                                    PreencheDiaRelatorio(
                                        horarioNoDia.Horario,
                                        horarioNoDia,
                                        String.Format("{0:D2}", j.Day),
                                        j.Month,
                                        j.Year,
                                        false,
                                        feriados
                                    )
                                );
                            }
                            else
                            {
                                retorno.Add(
                                    PreencheDiaRelatorio(
                                        horarioNoDia.Horario, 
                                        horarioNoDia, 
                                        String.Format("{0:D2}", j.Day), 
                                        j.Month, 
                                        j.Year, 
                                        false,
                                        feriados
                                    )
                                );
                                
                                if (horarioNoDia.Horario != null)
                                {
                                    horarioPadrao = horarioNoDia.Horario;
                                }
                            }
                        }
                        else
                        {
                            retorno.Add(
                                PreencheDiaRelatorio(
                                    horarioPadrao, 
                                    null, 
                                    String.Format("{0:D2}", j.Day), 
                                    j.Month, 
                                    j.Year, 
                                    true,
                                    feriados
                                )
                            );
                        }
                    }
                }
            }

            foreach (var item in retorno)
            {
                item.Matricula = matricula;
                item.Tecnico = tecnico;
                item.Supervisor = supervisor;

                if (item.PerfilRoteirizacao == null)
                {
                    item.PerfilRoteirizacao = perfilAgendamento;
                    item.IdPerfilRoteirizacao = idPerfilAgendamento;
                }
            }

            return retorno;
        }

        private void PreencheDias(ISession session, Escala_TrabalhoListaModel obj, Horario_Connect horarioPadrao, Int32 ano, Int32 mes)
        {
            using (FeriadoApiController feriadoapi = new FeriadoApiController())
            using (var repositoryHoraEquipe = new Conexao<Horario_Equipe>(session))
            {
                List<Horario_Equipe> horarios_equipeBanco = (repositoryHoraEquipe.select(p => p.Equipe.Matricula == obj.Equipe && p.Data.Year == ano && p.Data.Month == mes)).Result.Take(31).OrderByDescending(p => p.Data).ToList();
                IEnumerable<int> feriados = feriadoapi.GetDiasFeriadosMesAno(mes, ano).Result;

                if (horarios_equipeBanco.Count() == 0)
                {
                    //Preenche o horario padrão
                    for (var i = 1; i <= DateTime.DaysInMonth(ano, mes); i++)
                    {
                        PreencheDia(
                            obj,
                            horarioPadrao,
                            null,
                            String.Format("{0:D2}", i),
                            mes,
                            ano,
                            false,
                            feriados
                        );
                    }
                }
                else //Caso equipe tenha alguma alteração de horario
                {
                    DateTime primeirodia_do_mes = Convert.ToDateTime(String.Format("01/{0}", DateTime.Now.ToString("MM/yyyy")));
                    Horario_Connect ultimoHorario = horarioPadrao;

                    List<Horario_Equipe> ultimoHorarioMesPassado = horarios_equipeBanco.FindAll(p => p.Data < primeirodia_do_mes && p.ApenasNoDia == false).ToList();

                    if (ultimoHorarioMesPassado.Count() > 0)
                        horarioPadrao = ultimoHorarioMesPassado.First().Horario;

                    //Preenche o horario padrão
                    for (var i = 1; i <= DateTime.DaysInMonth(ano, mes); i++)
                    {
                        try
                        {
                            DateTime data = new DateTime(ano, mes, i);
                            var horarioNoDia = horarios_equipeBanco.Find(p => p.Data == data);
                            if (horarioNoDia != null)
                            {
                                if (horarioNoDia.ApenasNoDia)
                                {
                                    PreencheDia(
                                        obj,
                                        horarioNoDia.Horario,
                                        horarioNoDia,
                                        String.Format("{0:D2}", i),
                                        mes,
                                        ano,
                                        false,
                                        feriados
                                    );
                                }
                                else
                                {
                                    PreencheDia(
                                        obj,
                                        horarioNoDia.Horario,
                                        horarioNoDia,
                                        String.Format("{0:D2}", i),
                                        mes,
                                        ano,
                                        false,
                                        feriados
                                    );

                                    if (horarioNoDia.Horario != null)
                                    {
                                        horarioPadrao = horarioNoDia.Horario;
                                    }
                                }
                            }
                            else
                            {
                                PreencheDia(
                                    obj,
                                    horarioPadrao,
                                    null,
                                    String.Format("{0:D2}", i),
                                    mes,
                                    ano,
                                    true,
                                    feriados
                                );
                            }
                        }
                        catch
                        {

                        }
                    }
                }
            }
        }

        private void PreencheDia(
            Escala_TrabalhoListaModel obj, 
            Horario_Connect horario, 
            Horario_Equipe horario_Equipe, 
            String dia, 
            int mes, 
            int ano, 
            Boolean horario_vazio, 
            IEnumerable<int> feriados
        )
        {
            using (HorarioApiController horarioApiController = new HorarioApiController())
            {
                obj.GetType().GetProperty("Id_Dia" + dia).SetValue(obj, horario_Equipe == null ? 0 : horario_Equipe.Id);
                var horarioConnect = horario_Equipe == null ? horario : horario_Equipe?.Horario;
                if (feriados.Contains(Convert.ToInt32(dia)) == false)
                {
                    var horariosDia = horarioConnect == null ? (inicio: null, inicio_intervalo: null, termino_intervalo: null, termino: null) : horarioApiController.RetornaHorarioDia(horarioConnect, new DateTime(ano, mes, Convert.ToInt32(dia)), false);
                    var descricaoExibir = horariosDia.inicio == null ? "<span style='color:#ff0000'><h4>I</h4></span>" : horarioConnect.Codigo;

                    obj.GetType().GetProperty("Dia" + dia).SetValue(obj, descricaoExibir);
                }
                else
                {
                    var horariosDia = horarioConnect == null ? (inicio: null, inicio_intervalo: null, termino_intervalo: null, termino: null) : horarioApiController.RetornaHorarioDia(horarioConnect, new DateTime(ano, mes, Convert.ToInt32(dia)), true);
                    var descricaoExibir = horariosDia.inicio == null ? "<span style='color:#ff0000'><h4>F</h4></span>" : horarioConnect.Codigo;
                    obj.GetType().GetProperty("Dia" + dia).SetValue(obj, descricaoExibir);
                }
            }

        }

        private EscalaTrabalhoRelatorioModel PreencheDiaRelatorio(Horario_Connect horario, Horario_Equipe horario_Equipe, String dia, int mes, int ano, Boolean horario_vazio, IEnumerable<int> feriados)
        {
            using (HorarioApiController horarioApiController = new HorarioApiController())
            {
                EscalaTrabalhoRelatorioModel diaRelatorio = new EscalaTrabalhoRelatorioModel();
                diaRelatorio.Data = (new DateTime(ano, mes, Convert.ToInt32(dia))).ToString("dd/MM/yyyy");
                var horarioConnect = horario_Equipe == null ? horario : horario_Equipe?.Horario;
                diaRelatorio.IdPerfilRoteirizacao = horario_Equipe?.PerfilAgendamento?.Id;
                diaRelatorio.PerfilRoteirizacao = horario_Equipe?.PerfilAgendamento?.Descricao;

                if (feriados.Contains(Convert.ToInt32(dia)) == false)
                {
                    var horariosDia = horarioConnect == null ? (inicio: null, inicio_intervalo: null, termino_intervalo: null, termino: null) : horarioApiController.RetornaHorarioDia(horarioConnect, new DateTime(ano, mes, Convert.ToInt32(dia)), false);
                    diaRelatorio.Horario = horarioConnect?.Descricao;
                    if (String.IsNullOrEmpty(horarioConnect?.Descricao))
                    {
                        diaRelatorio.Horario = "";
                    }
                    diaRelatorio.Expediente = horariosDia.inicio == null ? "Indisponível" : String.Concat(horariosDia.inicio.Value.ToString("HH:mm"), " - ", horariosDia.inicio_intervalo.HasValue ? horariosDia.inicio_intervalo.Value.ToString("HH:mm") : null, " / ", horariosDia.termino_intervalo.HasValue ? horariosDia.termino_intervalo.Value.ToString("HH:mm") : null, " - ", horariosDia.termino.Value.ToString("HH:mm"));
                    //diaRelatorio.Indisponivel = horariosDia.inicio == null ? RsCadastros.SIM : RsCadastros.NAO;
                    diaRelatorio.Indisponivel = horariosDia.inicio == null ? RsCadastros.INDISPONIVEL : RsCadastros.DISPONIVEL;
                }
                else
                {
                    var horariosDia = horarioConnect == null ? (inicio: null, inicio_intervalo: null, termino_intervalo: null, termino: null) : horarioApiController.RetornaHorarioDia(horarioConnect, new DateTime(ano, mes, Convert.ToInt32(dia)), true);
                    diaRelatorio.Horario = horarioConnect?.Descricao;
                    //diaRelatorio.Indisponivel = horariosDia.inicio == null ? RsCadastros.SIM : RsCadastros.NAO;
                    diaRelatorio.Indisponivel = horariosDia.inicio == null ? RsCadastros.INDISPONIVEL : RsCadastros.DISPONIVEL;
                    diaRelatorio.Expediente = horariosDia.inicio == null ? "Indisponível" : String.Concat(horariosDia.inicio.Value.ToString("HH:mm"), " - ", horariosDia.inicio_intervalo.Value.ToString("HH:mm"), " / ", horariosDia.termino_intervalo.Value.ToString("HH:mm"), " - ", horariosDia.termino.Value.ToString("HH:mm"));
                }
                return diaRelatorio;
            }

        }


        private Escala_TrabalhoCadastroModel GetViewObj(Horario_Equipe obj)
        {
            return new Escala_TrabalhoCadastroModel
            {
                Id = obj.Id,
                Equipe = obj.Equipe.Matricula,
                Apenas_Dia = !obj.ApenasNoDia,
                Indisponivel = obj.Indisponivel,
                Id_PerfilAgendamento = obj.PerfilAgendamento?.Id,
                Nome = obj.Equipe.Nome,
                Id_Horario = obj.Horario?.Id,
                Ano = (Int16)obj.Data.Year,
                Mes = (Mes)Enum.Parse(typeof(Mes), Convert.ToString(obj.Data.Month)),
                Dia = (Int16)obj.Data.Day,

            };

        }

        private void BindViewToObj(ISession session, Horario_Equipe objBanco, Escala_TrabalhoCadastroModel view, bool alteracaoMassa)
        {
            using (var repoEquipeAniel = new Conexao<EquipeAniel>(session))
            using (var repoHorario_Connect = new Conexao<Horario_Connect>(session))
            using (var repoPerfilAgendamento = new Conexao<PerfilAgendamento>(session))
            {
                objBanco.Id = view.Id.GetValueOrDefault(0);
                objBanco.Equipe = repoEquipeAniel.selectFirst(p => p.Matricula == view.Equipe).Result;
                objBanco.ApenasNoDia = !view.Apenas_Dia;
                objBanco.Indisponivel = view.Indisponivel;
                objBanco.Horario = view.Id_Horario.HasValue ? repoHorario_Connect.selectFirst(p => p.Id == view.Id_Horario.Value).Result : null;

                if (alteracaoMassa)
                {
                    objBanco.Data = Convert.ToDateTime(view.Data_Selecionada);
                }
                else if (view.Ano.HasValue && view.Mes.HasValue && view.Dia.HasValue)
                {
                    objBanco.Data = new DateTime((int)view.Ano, (int)view.Mes, (int)view.Dia);
                }
                else
                {
                    throw new Exception("Data para alteração de escala não informada");
                }

                objBanco.PerfilAgendamento = repoPerfilAgendamento.selectFirst(p => p.Id == view.Id_PerfilAgendamento).Result;
            }
        }

        [HttpPost()]
        [Route("api/Escala_Trabalhoapi/AlterarStatus")]
        public async Task<PostApiRetorno<Int64>> AlterarStatus(AlterarStatusViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        String _ids = $"({String.Join(",", value.ids)})";

                        StringBuilder sql = new StringBuilder($"UPDATE TB_Escala_Trabalho SET STATUS = {value.status} WHERE ID IN {_ids}");

                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, null);

                        retorno.Ok = true;
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Escala_Trabalhoapi/excluirlista")]
        public async Task<PostApiRetorno<Int64>> ExcluirLista(ExcluirListaViewModel value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        String _ids = $"({String.Join(",", value.ids)})";

                        StringBuilder sql = new StringBuilder($"DELETE FROM TB_Escala_Trabalho WHERE ID IN {_ids}");

                        await CommandHelper.ExecuteNonQuery(com, sql, new Dictionary<String, Object>(), session.Connection, null);

                        retorno.Ok = true;
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
            }

            return retorno;
        }


        [HttpGet()]
        [Route("api/Escala_Trabalhoapi/FindById")]
        public async Task<GetApiRetorno<Escala_TrabalhoCadastroModel>> FindById(Int64 Id)
        {
            GetApiRetorno<Escala_TrabalhoCadastroModel> retorno = new GetApiRetorno<Escala_TrabalhoCadastroModel>();

            using (ISession session = HibernateUtil.GetSessionT5())
            using (var repository = new Conexao<Horario_Equipe>(session))
            using (var repositoryFuncionario = new Conexao<FuncionarioAniel>(session))
            {
                try
                {
                    var horario = await repository.selectFirst(p => p.Id == Id);
                    retorno.Objeto = GetViewObj(horario);
                    var funcionario = await repositoryFuncionario.selectFirst(p => p.Num_Fre == horario.Equipe.Num_Fre);
                    retorno.Objeto.HorarioServicoAtribuido = funcionario.HorarioServicoAtribuido;

                    if (funcionario.HorarioServicoAtribuido == null)
                    {
                        using (ISession sessionConnect = HibernateUtil.GetSession())
                        using (var repoConfig = new Conexao<Configuracoes_Gerais>(sessionConnect))
                        {
                            retorno.Objeto.HorarioServicoAtribuido = repoConfig.selectFirst().Result.UtilizaPoint_RoteirizacaoAgendamento ? HorarioAtribuido.AnielPoint : HorarioAtribuido.AnielConnect;
                        }
                    }


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
        [Route("api/Escala_Trabalhoapi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<Escala_TrabalhoCadastroModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Horario_Equipe modelo = null;

                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    using (var repoHorario_Equipe = new Conexao<Horario_Equipe>(session))
                    using (var repoEquipeAniel = new Conexao<EquipeAniel>(session))
                    using (var repoHorario_Connect = new Conexao<Horario_Connect>(session))
                    using (var repoPerfilAgendamento = new Conexao<PerfilAgendamento>(session))
                    using (var repoHistoricoAlterEscala = new Conexao<HistoricoAlterEscala>(session))
                    {
                        Horario_Connect horarioConnect = value.Objeto.Id_Horario.HasValue ? await repoHorario_Connect.selectFirst(p => p.Id == value.Objeto.Id_Horario.Value) : null;
                        Horario_Connect horarioPadrao = await repoHorario_Connect.selectFirst(p => p.Padrao && p.Tipo == Tipo_Hora_Connect.Horario_Servico);

                        bool alteracaoMassa = false;

                        if (String.IsNullOrEmpty(value.Objeto.Tecnicos))
                        {
                            value.Objeto.Tecnicos = value.Objeto.Equipe;
                        }
                        else
                        {
                            alteracaoMassa = true;
                        }

                        foreach (string equipe in value.Objeto.Tecnicos.Split(','))
                        {
                            value.Objeto.Equipe = equipe;

                            if (value.Objeto.Indisponivel && !String.IsNullOrEmpty(value.Objeto.Data_Inicial))
                            {
                                //Limpa as escalas no período informado
                                var horarios = (await repoHorario_Equipe.select(p => p.Equipe.Matricula == equipe &&
                                                                                        p.Data >= Convert.ToDateTime(value.Objeto.Data_Inicial) &&
                                                                                        p.Data <= Convert.ToDateTime(value.Objeto.Data_Final))).ToList();
                                await repoHorario_Equipe.Remove(horarios);

                                EquipeAniel equipeAniel = repoEquipeAniel.selectFirst(p => p.Matricula == equipe).Result;
                                PerfilAgendamento perfil = await repoPerfilAgendamento.selectFirst(p => p.Id == value.Objeto.Id_PerfilAgendamento);
                                List<Horario_Equipe> horariosNovos = new List<Horario_Equipe>();
                               
                                for (DateTime i = Convert.ToDateTime(value.Objeto.Data_Inicial); i <= Convert.ToDateTime(value.Objeto.Data_Final);)
                                {
                                    if (i < DateTime.Today.AddYears(-5))
                                        throw new Exception("Ocorreu um erro de lançamento de escala.");

                                    var horarioAntigo = horarios.Find(p => p.Data.Date == i.Date);
                                    horariosNovos.Add(new Horario_Equipe
                                    {
                                        Data = i,
                                        Equipe = equipeAniel,
                                        Horario = null,
                                        Indisponivel = true,
                                        ApenasNoDia = true,
                                        Status = Status_Default.Ativo,
                                        PerfilAgendamento = perfil,
                                        UsuarioCriacao = value.Usuario
                                    });

                                    if (horarioAntigo?.Horario?.Id != horarioConnect?.Id || horarioAntigo?.Indisponivel != value.Objeto.Indisponivel || horarioAntigo?.PerfilAgendamento?.Id != perfil?.Id)
                                    {
                                        var historico = new HistoricoAlterEscala
                                        {
                                            Horario_Anterior = horarioAntigo?.Horario ?? horarioPadrao,
                                            Horario_Atual = horarioConnect,
                                            Num_Fre = equipeAniel.Num_Fre,
                                            UsuarioCriacao = value.Usuario,
                                            DataHoraCriacao = DateTime.Now,
                                            Data = i,
                                            DataInicio = Convert.ToDateTime(value.Objeto.Data_Inicial),
                                            IndisponivelAtual = value.Objeto.Indisponivel,
                                            IndisponivelAnterior = horarioAntigo?.Indisponivel ?? false,
                                            PerfilAgendamentoAnterior = horarioAntigo?.PerfilAgendamento ?? equipeAniel?.PerfilAgendamento,
                                            PerfilAgendamentoAtual = perfil
                                        };

                                        await repoHistoricoAlterEscala.Save(historico);
                                    }

                                    i = i.AddDays(1);
                                }

                                await repoHorario_Equipe.Save(horariosNovos);
                            }
                            else if (!String.IsNullOrEmpty(value.Objeto.Data_Inicial))
                            {
                                //Limpa as escalas no período informado
                                var horarios = (await repoHorario_Equipe.select(p => p.Equipe.Matricula == equipe &&
                                                                                        p.Data >= Convert.ToDateTime(value.Objeto.Data_Inicial) &&
                                                                                        p.Data <= Convert.ToDateTime(value.Objeto.Data_Final))).ToList();
                                await repoHorario_Equipe.Remove(horarios);

                                EquipeAniel equipeAniel = repoEquipeAniel.selectFirst(p => p.Matricula == equipe).Result;
                                List<Horario_Equipe> horariosNovos = new List<Horario_Equipe>();
                                PerfilAgendamento perfil = await repoPerfilAgendamento.selectFirst(p => p.Id == value.Objeto.Id_PerfilAgendamento);
                                for (DateTime i = Convert.ToDateTime(value.Objeto.Data_Inicial); i <= Convert.ToDateTime(value.Objeto.Data_Final);)
                                {

                                    if (i < DateTime.Today.AddYears(-5))
                                        throw new Exception("Ocorreu um erro de lançamento de escala.");

                                    var horarioAntigo = horarios.Find(p => p.Data.Date == i.Date);
                                    horariosNovos.Add(new Horario_Equipe
                                    {
                                        Data = i,
                                        Equipe = equipeAniel,
                                        Horario = horarioConnect,
                                        Indisponivel = false,
                                        ApenasNoDia = true,
                                        Status = Status_Default.Ativo,
                                        UsuarioCriacao = value.Usuario,
                                        PerfilAgendamento = perfil
                                    });
                                    i = i.AddDays(1);

                                    if (horarioAntigo?.Horario?.Id != horarioConnect?.Id)
                                    {
                                        var historico = new HistoricoAlterEscala
                                        {
                                            Horario_Anterior = horarioAntigo?.Horario ?? horarioPadrao,
                                            Horario_Atual = horarioConnect,
                                            Num_Fre = equipeAniel.Num_Fre,
                                            UsuarioCriacao = value.Usuario,
                                            DataHoraCriacao = DateTime.Now,
                                            Data = i,
                                            DataInicio = Convert.ToDateTime(value.Objeto.Data_Inicial),
                                            IndisponivelAtual = value.Objeto.Indisponivel,
                                            IndisponivelAnterior = horarioAntigo?.Indisponivel ?? false,
                                            PerfilAgendamentoAnterior = horarioAntigo?.PerfilAgendamento ?? equipeAniel?.PerfilAgendamento,
                                            PerfilAgendamentoAtual = perfil
                                        };

                                        await repoHistoricoAlterEscala.Save(historico);
                                    }

                                }

                                await repoHorario_Equipe.Save(horariosNovos);
                            }
                            else
                            {
                                if (value.Objeto.Id != 0)
                                    modelo = (await repoHorario_Equipe.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                                if (value.Objeto.Apenas_Dia)
                                {

                                    //var horarios = (await new Conexao<Horario_Equipe>(session).select(p => p.Equipe.Matricula == value.Objeto.Equipe &&
                                    //                                                   p.Data.Month == (int)value.Objeto.Mes.Value &&
                                    //                                                   p.Data.Day > value.Objeto.Dia &&
                                    //                                                   p.Indisponivel == true
                                    //                                                   )).ToList();
                                    //await repository.Remove(horarios);

                                    //Limpa as escalas no período informado
                                    var horarios = (await repoHorario_Equipe.select(p => p.Equipe.Matricula == equipe &&
                                                                                            p.Data >= Convert.ToDateTime(value.Objeto.Data_Inicial) &&
                                                                                            p.Data <= Convert.ToDateTime(value.Objeto.Data_Final))).ToList();
                                    await repoHorario_Equipe.Remove(horarios);
                                    PerfilAgendamento perfil = await repoPerfilAgendamento.selectFirst(p => p.Id == value.Objeto.Id_PerfilAgendamento);
                                    EquipeAniel equipeAniel = repoEquipeAniel.selectFirst(p => p.Matricula == equipe).Result;
                                    List<Horario_Equipe> horariosNovos = new List<Horario_Equipe>();
                                    for (DateTime i = Convert.ToDateTime(value.Objeto.Data_Inicial); i <= Convert.ToDateTime(value.Objeto.Data_Final);)
                                    {

                                        if (i < DateTime.Today.AddYears(-5))
                                            throw new Exception("Ocorreu um erro de lançamento de escala.");

                                        horariosNovos.Add(new Horario_Equipe
                                        {
                                            Data = i,
                                            Equipe = equipeAniel,
                                            Horario = await repoHorario_Connect.selectFirst(p => p.Id == value.Objeto.Id_Horario.Value),
                                            Indisponivel = false,
                                            ApenasNoDia = false,
                                            Status = Status_Default.Ativo,
                                            UsuarioCriacao = value.Usuario,
                                            PerfilAgendamento = perfil
                                        });
                                        i = i.AddDays(1);
                                    }
                                    await repoHorario_Equipe.Save(horariosNovos);

                                }
                                else if (modelo == null)
                                {
                                    modelo = new Horario_Equipe
                                    {
                                        Status = Status_Default.Ativo,
                                        UsuarioCriacao = value.Usuario
                                    };

                                    BindViewToObj(session, modelo, value.Objeto, alteracaoMassa);

                                    var remover = (await repoHorario_Equipe.select(p => p.Equipe == modelo.Equipe && p.Data == modelo.Data)).FirstOrDefault();

                                    if(remover != null)
                                        await repoHorario_Equipe.Remove(remover);
                                    
                                    if (remover?.Horario?.Id != horarioConnect?.Id && remover?.PerfilAgendamento?.Id != modelo.PerfilAgendamento?.Id && remover?.Indisponivel != value.Objeto.Indisponivel)
                                    {
                                        var historico = new HistoricoAlterEscala
                                        {
                                            Horario_Anterior = remover?.Horario ?? horarioPadrao,
                                            Horario_Atual = horarioConnect,
                                            Num_Fre = modelo.Equipe.Num_Fre,
                                            UsuarioCriacao = value.Usuario,
                                            DataHoraCriacao = DateTime.Now,
                                            Data = modelo.Data,
                                            IndisponivelAnterior = remover?.Indisponivel,
                                            IndisponivelAtual = value.Objeto.Indisponivel,
                                            PerfilAgendamentoAnterior = remover?.PerfilAgendamento ?? modelo?.Equipe?.PerfilAgendamento,
                                            PerfilAgendamentoAtual = modelo.PerfilAgendamento
                                        };

                                        await repoHistoricoAlterEscala.Save(historico);
                                    }

                                    await repoHorario_Equipe.SaveOrUpdate(modelo);
                                }
                                else
                                {
                                    modelo.UsuarioAlteracao = value.Usuario;

                                    PerfilAgendamento perfil = await repoPerfilAgendamento.selectFirst(p => p.Id == value.Objeto.Id_PerfilAgendamento);
                                    var remover = (await repoHorario_Equipe.select(p => p.Equipe == modelo.Equipe && p.Data == modelo.Data)).FirstOrDefault();

                                    if (remover?.Horario?.Id != horarioConnect?.Id || remover?.PerfilAgendamento?.Id != modelo.PerfilAgendamento?.Id || remover?.Indisponivel != value.Objeto.Indisponivel)
                                    {
                                        var historico = new HistoricoAlterEscala
                                        {
                                            Horario_Anterior = remover?.Horario ?? horarioPadrao,
                                            Horario_Atual = horarioConnect,
                                            Num_Fre = modelo.Equipe.Num_Fre,
                                            UsuarioCriacao = value.Usuario,
                                            DataHoraCriacao = DateTime.Now,
                                            Data = modelo.Data,
                                            IndisponivelAnterior = remover?.Indisponivel,
                                            IndisponivelAtual = value.Objeto.Indisponivel,
                                            PerfilAgendamentoAnterior = remover?.PerfilAgendamento ?? modelo?.Equipe?.PerfilAgendamento,
                                            PerfilAgendamentoAtual = perfil
                                        };
                                      
                                        await repoHistoricoAlterEscala.Save(historico);
                                    }

                                    BindViewToObj(session, modelo, value.Objeto, alteracaoMassa);
                                    //await repoHorario_Equipe.Remove(remover);
                                    await repoHorario_Equipe.SaveOrUpdate(modelo);

                                }
                            }
                        }
                        retorno.IdObjeto = 0;
                        retorno.Ok = true;
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

        [Route("api/Escala_Trabalhoapi/LogHistoricoEscalaTrabalho")]
        [HttpPost()]
        public async Task<DataTableResponse<LogHistoricoEscalaTrabalho>> LogHistoricoEscalaTrabalho(LogHistoricoEscalaTrabalho request)
        {
            List<LogHistoricoEscalaTrabalho> retorno = new List<LogHistoricoEscalaTrabalho>();

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {


                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine("SELECT A.NUM_FRE, B.NOME,C.DESCRICAO AS DESCRICAO_ATUAL, D.DESCRICAO AS DESCRICAO_ANTERIOR, A.DATAHORACRIACAO, A.USUARIOCRIACAO, A.DATA, E.DESCRICAO AS PERFIL_ANTERIOR, F.DESCRICAO AS PERFIL_ATUAL,A.INDISPONIVEL_ANTERIOR,A.INDISPONIVEL_ATUAL");
                        sql.AppendLine("FROM TB_HISTORICO_ALTER_ESCALA A ");
                        sql.AppendLine("INNER JOIN TB_FUNCIONARIO B ");
                        sql.AppendLine("ON(B.NUM_FRE = A.NUM_FRE) ");
                        sql.AppendLine("LEFT JOIN TB_HORARIO_CONNECT C");
                        sql.AppendLine("ON(A.ID_HORARIO_ATUAL = C.ID)");
                        sql.AppendLine("LEFT JOIN TB_HORARIO_CONNECT D");
                        sql.AppendLine("ON(A.ID_HORARIO_ANTERIOR = D.ID)");
                        sql.AppendLine("LEFT JOIN TB_PERFIL_AGENDAMENTO E");
                        sql.AppendLine("ON(A.ID_PERFIL_ANTERIOR = E.ID)");
                        sql.AppendLine("LEFT JOIN TB_PERFIL_AGENDAMENTO F");
                        sql.AppendLine("ON(A.ID_PERFIL_ATUAL = F.ID)");
                        #region Filtros

                        var condicoes = new List<string>();

                        if (!String.IsNullOrEmpty(request.Tecnico))
                        {
                            condicoes.Add($"B.NOME LIKE '%{request.Tecnico}%'");
                        }

                        if (!String.IsNullOrEmpty(request.DataInicio) && !String.IsNullOrEmpty(request.DataFim))
                        {
                            DateTime dataInicio = Convert.ToDateTime(request.DataInicio);
                            DateTime dataFim = Convert.ToDateTime(request.DataFim);

                            condicoes.Add(@$"(
                                (A.DATAINICIO <= @DATA_INICIO AND @DATA_FIM <= A.DATA)
                                OR (@DATA_INICIO <= A.DATAINICIO AND A.DATA <= @DATA_FIM)
                                OR (@DATA_INICIO <= A.DATAINICIO AND A.DATAINICIO <= @DATA_FIM)
                                OR (@DATA_INICIO <= A.DATA AND A.DATA <= @DATA_FIM)
                            )");

                            parametros.Add("@DATA_INICIO", dataInicio);
                            parametros.Add("@DATA_FIM", dataFim);
                        }

                        if (condicoes.Count > 0)
                        {
                            sql.AppendLine($"WHERE ({String.Join(" AND ", condicoes)})");
                        }

                        #endregion

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                var log = new LogHistoricoEscalaTrabalho
                                {
                                    Tecnico = reader["NOME"] != DBNull.Value ? Convert.ToString(reader["NOME"]) : "N/D",
                                    UsuarioAlteracao = reader["USUARIOCRIACAO"] != DBNull.Value ? Convert.ToString(reader["USUARIOCRIACAO"]) : "N/D",
                                    DataInicio = reader["DATAHORACRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORACRIACAO"]).ToString("dd/MM/yyyy HH:mm") : "N/D",
                                    DataFim = reader["DATA"] != DBNull.Value ? Convert.ToDateTime(reader["DATA"]).ToString("dd/MM/yyyy HH:mm") : "N/D",
                                    DataHoraAlteracao = reader["DATAHORACRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORACRIACAO"]).ToString("dd/MM/yyyy HH:mm") : "N/D",
                                    HorarioAnterior = reader["DESCRICAO_ANTERIOR"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO_ANTERIOR"]) : "N/D",
                                    HorarioAtual = reader["DESCRICAO_ATUAL"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO_ATUAL"]) : "N/D",
                                    PerfilAnterior = reader["PERFIL_ANTERIOR"] != DBNull.Value ? Convert.ToString(reader["PERFIL_ANTERIOR"]) : "N/D",
                                    PerfilAtual = reader["PERFIL_ATUAL"] != DBNull.Value ? Convert.ToString(reader["PERFIL_ATUAL"]) : "N/D",
                                    IndisponivelAnterior = reader["INDISPONIVEL_ANTERIOR"] != DBNull.Value ? (Convert.ToBoolean(reader["INDISPONIVEL_ANTERIOR"]) ? RsCadastros.SIM : RsCadastros.NAO) : "N/D",
                                    IndisponivelAtual = reader["INDISPONIVEL_ATUAL"] != DBNull.Value ? (Convert.ToBoolean(reader["INDISPONIVEL_ATUAL"]) ? RsCadastros.SIM : RsCadastros.NAO) : "N/D",
                                };

                                retorno.Add(log);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return new DataTableResponse<LogHistoricoEscalaTrabalho>
            {
                data = retorno,
                recordsFiltered = retorno.Count
            };
        }

        [Route("api/Escala_Trabalhoapi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Int64>> Excluir([FromBody] ParamWebApi<Int64> value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    using (var repository = new Conexao<Horario_Equipe>(session))
                    {

                        Horario_Equipe modelo = null;
                        if (value.Objeto != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value.Objeto)).FirstOrDefault();

                            await repository.Remove(modelo);

                            retorno.IdObjeto = modelo.Id;
                            retorno.Ok = true;
                        }
                        else
                        {
                            throw new Exception(Resources.RsBase.NAOENCONTRADO);
                        }
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

        [Route("api/Escala_Trabalhoapi/ExcluirGrid")]
        [HttpGet()]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGrid(Int64 value)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    using (var repository = new Conexao<Horario_Equipe>(session))
                    {

                        Horario_Equipe modelo = null;
                        if (value != 0)
                        {
                            modelo = (await repository.select(p => p.Id == value)).FirstOrDefault();

                            await repository.Remove(modelo);

                            retorno.IdObjeto = modelo.Id;
                            retorno.Ok = true;
                        }
                        else
                        {
                            throw new Exception(Resources.RsBase.NAOENCONTRADO);
                        }
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



        public async Task<GetApiRetorno<HorarioAtribuido?>> BuscarHorarioAtribuido(String numfre)
        {
            GetApiRetorno<HorarioAtribuido?> retorno = new GetApiRetorno<HorarioAtribuido?>();

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    using (var repository = new Conexao<FuncionarioAniel>(session))
                    {

                        FuncionarioAniel funcionario = await repository.selectFirst(p => p.Num_Fre == numfre);
                        if (funcionario != null)
                        {
                            retorno.Objeto = funcionario.HorarioServicoAtribuido;
                        }
                        else
                        {
                            throw new Exception(Resources.RsBase.NAOENCONTRADO);
                        }
                    }
                }

                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        public async Task<PostApiRetorno> GravarHorarioAtribuido(String numfre, HorarioAtribuido? horarioAtribuido)
        {
            PostApiRetorno retorno = new PostApiRetorno();

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    using (var repository = new Conexao<FuncionarioAniel>(session))
                    {

                        FuncionarioAniel funcionario = await repository.selectFirst(p => p.Num_Fre == numfre);
                        if (funcionario != null)
                        {
                            funcionario.HorarioServicoAtribuido = horarioAtribuido;
                            await repository.Update(funcionario);
                        }
                        else
                        {
                            throw new Exception(Resources.RsBase.NAOENCONTRADO);
                        }
                    }
                }

                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }


        [HttpPost()]
        [Route("api/Escala_Trabalhoapi/GetComboTecnicoLog")]
        public async Task<ObjCombo<String>> GetComboTecnicoLog(RequestObjCombo<String> request)
        {
            ObjCombo<String> retorno = new ObjCombo<String>
            {
                results = new List<IObjComboResult<String>>()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        var sql = new StringBuilder();

                        sql.AppendLine("SELECT A.NUM_FRE, A.NOME ");
                        sql.AppendLine("FROM TB_FUNCIONARIO A ");

                        sql.AppendLine("LEFT JOIN TB_EQUIPE B");
                        sql.AppendLine("ON(A.NUM_FRE = B.NUM_FRE)");

                        sql.AppendLine("WHERE B.EQUIPE_CONNECT = 'S'");

                        if (request != null && !String.IsNullOrEmpty(request.q))
                        {
                            sql.AppendLine($"  AND (UPPER(A.NOME) LIKE '%{request.q.ToUpperInvariant()}%') ");
                        }

                        sql.AppendLine("ORDER BY UPPER(A.NOME)");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["NUM_FRE"]),
                                    Text = Convert.ToString(reader["NOME"])
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

        [HttpPost]
        [Route("api/Escala_Trabalhoapi/GetCombo")]
        public async Task<ObjCombo<String>> GetCombo(RequestObjCombo<String> request)
        {
            ObjCombo<String> retorno = new ObjCombo<String>
            {
                results = new List<IObjComboResult<String>>()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros 
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();


                    StringBuilder sqlFrom = new StringBuilder("FROM TB_HORARIO_CONNECT E ");
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(E.ID) AS CO ");
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
                        var sql = new StringBuilder();

                        sql.AppendLine("SELECT E.ID, E.DESCRICAO ");
                        sql.AppendLine(sqlFrom.ToString());

                        sql.AppendLine("WHERE (E.TIPO = 2) ");

                        if (request != null && !String.IsNullOrEmpty(request.q))
                        {
                            sql.AppendLine($"  AND (UPPER(E.DESCRICAO) LIKE '%{request.q.ToUpperInvariant()}%') ");
                        }

                        sql.AppendLine(" ORDER BY UPPER(E.DESCRICAO)");
                  
                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["ID"]),
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
    }
}
