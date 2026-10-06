using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Core.Service;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Models.T5;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.T5.DocumentoProducao;
using Aniel.Connect.Repository.T5.Equipe;
using Aniel.Connect.Repository.T5.TipoServico;
using Aniel.Connect.Repository.Usuario;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using NHibernate;
using NHibernate.Criterion;
using Resources;
using Sinapse.DAO.repository;
using Sinapse.Model.Identity;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;


namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class Gestao_EquipeApiController : BaseApiController
    {
        [AcceptVerbs("Get", "Post")]
        [Route("api/Gestao_Equipeapi/GeraTimeline")]
        public async Task<GetApiRetorno<string>> GeraTimeline(Int32? TipoFiltro, Int32? TipoVisualizacao, Int32? Prazo,
            Int32? FormatoTimeline, String Supervisor, String Id_Monitor, String Cod_Escritorio, String Cod_Area, String Cod_Localidade, String Localidade,
            Int64? Id_Natureza, String Data, String DataFinal, String Tecnico, String DataHoraParametro, Tipo_Prioridade? Tipo_Prioridade, String TipoServico, String SubTipoServico, String CodDepto, string CodCt, string Projeto, [FromUri] List<Int32?> IdsJornadas)
        {
            DateTime? DataConsulta = String.IsNullOrEmpty(Data) ? DateTime.Now : Convert.ToDateTime(Data);
            DateTime? DataConsultaFim = String.IsNullOrEmpty(DataFinal) ? DateTime.Now : Convert.ToDateTime(DataFinal);
            List<Timeline_ServicosEquipeModels> retorno = new List<Timeline_ServicosEquipeModels>();
            Dictionary<long, PerfilAgendamento> cachePerfilAgendamento = new Dictionary<long, PerfilAgendamento>();

            string erroServicos = null;
            //FormatoTimeline 0: Técnico
            //FormatoTimeline 1: Data

            //FORMATO DE DATA


            if (FormatoTimeline == 1)
            {
                try
                {
                    var RetornoTodosServicos = await GetServicos(Data: DataConsulta.Value.ToString("dd/MM/yyyy"),
                                                          Equipe: null,
                                                          Monitor: Id_Monitor,
                                                          Supervisor: Supervisor,
                                                          TodosTecnicos: true,
                                                          DataHoraParametro: DataHoraParametro,
                                                          Prazo: Prazo,
                                                          TipoVisualizacao: TipoVisualizacao,
                                                          GrupoArea: Cod_Escritorio,
                                                          Area: Cod_Area,
                                                          Localidade: Cod_Localidade,
                                                          TipoServico: TipoServico,
                                                          SubTipoServico: SubTipoServico,
                                                          NaturezaServico: null,
                                                          TipoPrioridade: Tipo_Prioridade,
                                                          DataCriacaoInicio: null,
                                                          DataCriacaoFim: null,
                                                          contrato: CodCt,
                                                          projeto: Projeto);

                    if (RetornoTodosServicos.Ok == false)
                    {
                        erroServicos = RetornoTodosServicos.Mensagem;
                    }
                    ;


                    var TodosServicos = RetornoTodosServicos.Objeto;

                    using (ISession sessionNH = HibernateUtil.GetSessionT5())
                    using (ISession session_connectNH = HibernateUtil.GetSession())
                    using (var repository = new Conexao<Horario_Connect>(sessionNH))
                    using (var repositoryPerfilAgendamento = new Conexao<PerfilAgendamento>(sessionNH))
                    using (var feriado_repository = new Conexao<Feriado>(session_connectNH))
                    using (var repositoryHorarioEquipe = new Conexao<Horario_Equipe>(sessionNH))
                    using (var horarioapi = new HorarioApiController())
                    using (ISession session = HibernateUtil.GetSessionT5())
                    using (var repositoryConfig = new Conexao<Configuracoes_Gerais>(session_connectNH))
                    {
                        var configGerais = await repositoryConfig.selectFirst();
                        ITipoServicoAnielRepository repoTipoServico = new TipoServicoAnielRepository(sessionNH, session);
                        IDocumentoProducaoAnielRepository repoDocumento = new DocumentoProducaoAnielRepository(sessionNH, session);
                        IEquipeAnielRepository repoEquipe = new EquipeAnielRepository(session);
                        EquipeAnielBusiness equipeAnielBusiness = new EquipeAnielBusiness(repoEquipe, repoTipoServico, repoDocumento);
                        var horario_modelo = await repository.selectFirst(p => p.Padrao && p.Tipo == Tipo_Hora_Connect.Horario_Servico);
                        bool feriado = feriado_repository.selectFirst(p => p.Data == DataConsultaFim.Value.Date).Result != null;
                        Dictionary<String, Object> parametros = new Dictionary<string, object>();
                        var horariosEquipes = await repositoryHorarioEquipe.select(p => p.Data == DataConsultaFim.Value);


                        //Select
                        using (var com = session.Connection.CreateCommand())
                        {
                            StringBuilder sql = new StringBuilder(" SELECT DISTINCT A.EQUIPE, A.NOME, C.TIPO, C.JUSTIFICATIVA,C.TIPO,A.ID_PERFIL_AGENDAMENTO,D.NOME AS SUPERV,C.BATERIA, W.DESCRICAO, W.CODIGO  ")
                                .AppendLine(" FROM TB_EQUIPE A ")
                                .AppendLine(" INNER JOIN TB_FUNCIONARIO B ")
                                .AppendLine(" ON(A.NUM_FRE = B.NUM_FRE) ")
                                .AppendLine(" LEFT JOIN TB_EQUIPE_STATUS C ")
                                .AppendLine(" ON(A.EQUIPE = C.EQUIPE) ")
                                .AppendLine(" LEFT JOIN TB_FUNCIONARIO D ")
                                .AppendLine(" ON(D.NUM_FRE = A.COD_SUPERV) ")
                                .AppendLine(" LEFT JOIN TB_HORARIO_EQUIPE_CONNECT Z ")
                                .AppendLine(" ON(Z.EQUIPE = A.EQUIPE AND Z.DATA = @DATA) ")
                                .AppendLine(" LEFT JOIN TB_HORARIO_CONNECT W ")
                                .AppendLine(" ON(W.ID = Z.ID_HORARIO) ");


                            sql.AppendLine(" WHERE (B.SITUACAO = 0 OR B.SITUACAO = 4) AND A.EQUIPE_CONNECT = 'S'");

                            if (IdsJornadas.Count != 0)
                            {
                                sql.AppendLine("AND (CASE");
                                sql.AppendLine("    WHEN COALESCE(B.HORARIO_ATRIBUIDO, 0) = 0 THEN ( "); // Horario do Connect

                                sql.AppendLine("    CASE");
                                sql.AppendLine("     WHEN (");

                                sql.AppendLine("    SELECT FIRST 1 H.ID");
                                sql.AppendLine("    FROM TB_HORARIO_EQUIPE_CONNECT H_EQUIPE");
                                sql.AppendLine("    INNER JOIN TB_HORARIO_CONNECT H");
                                sql.AppendLine("    ON H.ID = H_EQUIPE.ID_HORARIO");
                                sql.AppendLine("    WHERE H_EQUIPE.EQUIPE = A.EQUIPE");
                                sql.AppendLine("    AND H_EQUIPE.DATA BETWEEN @DATA AND @DATAFIM");
                                sql.AppendLine("    ) IS NULL ");
                                sql.AppendLine("    THEN ");
                                sql.AppendLine($"    ( SELECT H1.ID FROM TB_HORARIO_CONNECT H1 WHERE H1.ID IN ({String.Join(",", IdsJornadas)}) AND H1.PADRAO = 1) ");
                                sql.AppendLine("     ELSE ");
                                sql.AppendLine("     ( ");
                                sql.AppendLine("     SELECT FIRST 1 H.ID ");
                                sql.AppendLine("     FROM TB_HORARIO_EQUIPE_CONNECT H_EQUIPE ");
                                sql.AppendLine("     INNER JOIN TB_HORARIO_CONNECT H ");
                                sql.AppendLine("     ON H.ID = H_EQUIPE.ID_HORARIO ");
                                sql.AppendLine("     WHERE H_EQUIPE.EQUIPE = A.EQUIPE ");
                                sql.AppendLine("     AND H_EQUIPE.DATA BETWEEN @DATA AND @DATAFIM ");
                                sql.AppendLine($"    AND H.ID IN ({String.Join(",", IdsJornadas)}) ");
                                sql.AppendLine("     ) ");
                                sql.AppendLine("     END ");
                                sql.AppendLine("      ) ");
                                sql.AppendLine("     WHEN COALESCE(B.HORARIO_ATRIBUIDO, 0) = 1 THEN ");
                                sql.AppendLine("     ( ");
                                sql.AppendLine("     SELECT FIRST 1 ");
                                sql.AppendLine("     COALESCE(JORN_DIA.ID_HORARIO_CONNECT, ");
                                sql.AppendLine("     JORN_FUNC.ID_HORARIO_CONNECT) ");
                                sql.AppendLine("     FROM TB_FUNCIONARIO F ");
                                sql.AppendLine("     LEFT JOIN TB_PT_DIA_FUNCIONARIO DIA ");
                                sql.AppendLine("     ON DIA.DATA BETWEEN @DATA AND @DATAFIM ");
                                sql.AppendLine("     AND DIA.NUM_FRE = F.NUM_FRE ");
                                sql.AppendLine("     LEFT JOIN TB_PT_JORNADA_FUNCIONARIO JORN_DIA ");
                                sql.AppendLine("     ON DIA.ID_JORNADA = JORN_DIA.ID ");
                                sql.AppendLine("     LEFT JOIN TB_PT_JORNADA_FUNCIONARIO JORN_FUNC ");
                                sql.AppendLine("     ON F.ID_JORNADA = JORN_FUNC.ID ");
                                sql.AppendLine("     WHERE F.NUM_FRE = B.NUM_FRE ");
                                sql.AppendLine("     AND COALESCE(JORN_DIA.ID_HORARIO_CONNECT, ");
                                sql.AppendLine($"    JORN_FUNC.ID_HORARIO_CONNECT) IN ({String.Join(",", IdsJornadas)}) ");
                                sql.AppendLine("      ) ");
                                sql.AppendLine("      END ");
                                sql.AppendLine("      ) IS NOT NULL ");

                                // parametros.Add("@LISTA", String.Join(",", IdsJornadas));
                            }

                            parametros.Add("@DATA", DataConsulta.Value.Date);
                            parametros.Add("@DATAFIM", DataConsultaFim.Value.Date);

                            if (!String.IsNullOrEmpty(Supervisor))
                            {
                                sql.AppendLine(" AND A.COD_SUPERV = @SUPERVISOR");
                                parametros.Add("@SUPERVISOR", Supervisor);
                            }

                            if (!String.IsNullOrEmpty(Id_Monitor))
                            {
                                sql.AppendLine(" AND A.USUARIO_MONITOR = UPPER(@ID_MONITOR) ");
                                parametros.Add("@ID_MONITOR", Id_Monitor);
                            }

                            if (!String.IsNullOrEmpty(Tecnico))
                            {
                                sql.AppendLine(" AND (B.NUM_FRE = @NUM_FRE )");
                                parametros.Add("@NUM_FRE", Tecnico);
                            }

                            if (!String.IsNullOrEmpty(CodDepto))
                            {
                                sql.AppendLine(" AND B.COD_DEPTO = @COD_DEPTO ");
                                parametros.Add("@COD_DEPTO", CodDepto);
                            }


                            sql.AppendLine(" ORDER BY UPPER(A.NOME) ");


                            using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                            {
                                while (await reader.ReadAsync())
                                {
                                    var equipe = new Timeline_ServicosEquipeModels
                                    {
                                        Equipe = Convert.ToString(reader["EQUIPE"]),
                                        Nome_Equipe = Convert.ToString(reader["NOME"]),
                                        Detalhe_Equipe = Convert.ToString(reader["TIPO"]) == "1" ? "Técnico Ausente" : String.Empty,
                                        Justificativa = Convert.ToString(reader["JUSTIFICATIVA"])?.Replace("\r\n", @"<br/>"),
                                        Supervisor = Convert.ToString(reader["SUPERV"]),
                                        Bateria = reader["BATERIA"] != DBNull.Value ? Convert.ToString(reader["BATERIA"]) + "%" : null,
                                        JornadaTrabalho = Convert.ToString(reader["CODIGO"]) + " - " + Convert.ToString(reader["DESCRICAO"]),
                                        Servicos = new List<Timeline_ServicosEquipeServicosModels>()
                                    };

                                    var servicos = TodosServicos.FindAll(p => p.Equipe == equipe.Equipe);
                                    EquipeStatusTipo tipo = reader["TIPO"] != DBNull.Value ? (EquipeStatusTipo)Enum.Parse(typeof(EquipeStatusTipo), Convert.ToString(reader["TIPO"])) : EquipeStatusTipo.Sem_Status;
                                    var horarioEquipeTecnico = horariosEquipes.FirstOrDefault(p => p.Equipe.Matricula == equipe.Equipe);
                                    PerfilAgendamento perfilAgendamento = null;
                                    if (horarioEquipeTecnico != null)
                                    {
                                        if (horarioEquipeTecnico?.Indisponivel == true)
                                        {
                                            tipo = EquipeStatusTipo.Em_Intervalo;
                                        }

                                        if (horarioEquipeTecnico?.Indisponivel == true && servicos.Count == 0)
                                        {
                                            continue;
                                        }

                                        if (horarioEquipeTecnico.Horario != null)
                                        {
                                            var horario_dia = horarioapi.RetornaHorarioDia(horarioEquipeTecnico.Horario, DataConsulta.Value.Date, feriado);
                                            if (horario_dia.inicio.HasValue == false && servicos.Count == 0)
                                            {
                                                continue;
                                            }
                                        }

                                    }
                                    else
                                    {



                                        if (horario_modelo != null)
                                        {
                                            var horario_dia = horarioapi.RetornaHorarioDia(horario_modelo, DataConsulta.Value.Date, feriado);
                                            if (horario_dia.inicio.HasValue == false && servicos.Count == 0)
                                            {
                                                continue;
                                            }
                                        }
                                    }

                                    if (horarioEquipeTecnico?.PerfilAgendamento != null)
                                    {
                                        perfilAgendamento = horarioEquipeTecnico.PerfilAgendamento;
                                    }
                                    else
                                    {
                                        if (reader["ID_PERFIL_AGENDAMENTO"] != DBNull.Value)
                                        {
                                            if (cachePerfilAgendamento.TryGetValue(Convert.ToInt64(reader["ID_PERFIL_AGENDAMENTO"]), out perfilAgendamento) == false)
                                            {
                                                perfilAgendamento = await repositoryPerfilAgendamento.selectFirst(p => p.Id == Convert.ToInt64(reader["ID_PERFIL_AGENDAMENTO"]));
                                                if (perfilAgendamento != null)
                                                    cachePerfilAgendamento.Add(perfilAgendamento.Id, perfilAgendamento);
                                            }
                                        }

                                    }

                                    equipe.PerfilAgendamento = perfilAgendamento?.Descricao;


                                    if (!String.IsNullOrEmpty(Cod_Escritorio) ||
                                        !String.IsNullOrEmpty(Cod_Area) ||
                                        !String.IsNullOrEmpty(Localidade))
                                    {

                                        if (perfilAgendamento == null)
                                            continue;

                                        perfilAgendamento.Areas = perfilAgendamento.Areas.Where(p => p.Escritorio == Cod_Escritorio).ToList();

                                        if (!String.IsNullOrEmpty(Cod_Area))
                                        {
                                            perfilAgendamento.Areas = perfilAgendamento.Areas.Where(p => p.Area == Cod_Area || p.Todas_Areas).ToList();
                                        }

                                        if (!String.IsNullOrEmpty(Localidade))
                                        {
                                            perfilAgendamento.Areas = perfilAgendamento.Areas.Where(p => p.Localidade == Localidade || p.Todas_Localidades).ToList();
                                        }

                                        if (perfilAgendamento.Areas.Count == 0)
                                            continue;
                                    }

                                    if (Id_Natureza.HasValue)
                                    {
                                        if (perfilAgendamento == null)
                                            continue;

                                        perfilAgendamento.Naturezas = perfilAgendamento.Naturezas.Where(p => p.Natureza.Id == Id_Natureza).ToList();
                                        if (perfilAgendamento.Naturezas.Count == 0)
                                            continue;
                                    }



                                    tipo = equipeAnielBusiness.GetStatusPersonalizado(DataConsultaFim.Value,
                                                                                      horario_modelo,
                                                                                      feriado,
                                                                                      horarioEquipeTecnico,
                                                                                      tipo,
                                                                                      perfilAgendamento,
                                                                                      equipe.Equipe,
                                                                                      configGerais.Tempo_Execucao_Padrao.Value.TimeOfDay);

                                    equipe.Cor_StatusEquipe = EquipeApiController.GetCorStatusEquipe(tipo);
                                    var fora_agendamento = await GetTempoNaoUsadoAgendamento(DataConsultaFim.Value, horario_modelo, feriado, horarioEquipeTecnico);

                                    //Teste Geissiane
                                    var hora_extra = await GetTempoHoraExtra(DataConsultaFim.Value, horario_modelo, feriado, horarioEquipeTecnico);
                                    fora_agendamento.AddRange(hora_extra);

                                    var ausenciaTecnico = await GetHorarioAusente(DataConsulta.Value, equipe.Equipe, equipe);

                                    foreach (var ausencia in ausenciaTecnico)
                                    {
                                        var periodos = QuebrarPeriodoPorDia(
                                            ausencia.DataHoraIni,
                                            ausencia.DataHoraFim
                                        );

                                        foreach (var periodo in periodos)
                                        {
                                            if (periodo.Inicio.Date == DataConsulta.Value.Date)
                                            {
                                                servicos.Add(new Timeline_ServicosEquipeServicosModels
                                                {
                                                    DataHoraIni = periodo.Inicio,
                                                    DataHoraFim = periodo.Fim,
                                                    TipoOS = ausencia.TipoOS,
                                                    NumDoc = ausencia.NumDoc,
                                                    Num_OS = ausencia.Num_OS,
                                                    HtmlTitle = ausencia.HtmlTitle,
                                                    Equipe = ausencia.Equipe,
                                                    Prioridade_Legenda = ausencia.Prioridade_Legenda,
                                                    Pixels = (periodo.Fim - periodo.Inicio).TotalMinutes + "px"
                                                });
                                            }
                                        }
                                    }

                                    //servicos.AddRange(ausenciaTecnico);

                                    try
                                    {
                                        var equipeTemp = new Timeline_ServicosEquipeModels
                                        {
                                            Equipe = equipe.Equipe,
                                            Detalhe_Equipe = equipe.Detalhe_Equipe,
                                            Justificativa = equipe.Justificativa,
                                            Nome_Equipe = equipe.Nome_Equipe,
                                            Servicos = new List<Timeline_ServicosEquipeServicosModels>()
                                        };

                                        var servicosTemp = new List<Timeline_ServicosEquipeServicosModels>();

                                        List<Timeline_ServicosEquipeServicosModels> temp = new List<Timeline_ServicosEquipeServicosModels>();
                                        foreach (var item in servicos.OrderBy(p => p.DataHoraIni).ToList())
                                        {
                                            var servicoHorarioOcupado = temp.Find(p => HorarioApiController.IgualDatas(item.DataHoraIni, p.DataHoraIni));
                                            if (servicoHorarioOcupado != null)
                                            {
                                                servicos.Remove(item);
                                                servicosTemp.AddRange(fora_agendamento);
                                                servicosTemp = servicosTemp.OrderBy(p => p.DataHoraIni.TimeOfDay).ToList();
                                                equipeTemp.Servicos.AddRange(PreencheTipoOSNenhum(DataConsulta.Value, servicosTemp));
                                                retorno.Add(equipeTemp);
                                            }

                                        }

                                        servicos.AddRange(fora_agendamento);
                                        servicos = servicos.OrderBy(p => p.DataHoraIni.TimeOfDay).ToList();
                                        if (servicos.Count > 0)
                                            equipe.Servicos.AddRange(PreencheTipoOSNenhum(DataConsulta.Value, servicos));
                                    }
                                    catch { }


                                    retorno.Add(equipe);
                                }
                            }


                            return new GetApiRetorno<string>
                            {
                                Ok = String.IsNullOrEmpty(erroServicos) ? true : false,
                                Objeto = JsonConvert.SerializeObject(retorno),
                                Mensagem = erroServicos
                            };
                        }
                    }

                }
                catch (Exception ex)
                {
                    return new GetApiRetorno<string>
                    {
                        Ok = false,
                        Mensagem = ex.Message,
                        InnerException = ex.InnerException?.Message
                    };
                }
            }

            //FORMATO DE TÉCNICO
            else
            {
                var datasLista = await GetListaDatas(DataConsulta, DataConsultaFim);
                var item_ = new List<Timeline_ServicosEquipeServicosModels>();
                foreach (DateTime dataParametro in datasLista)
                {
                    using (ISession sessionNH = HibernateUtil.GetSessionT5())
                    using (ISession session = HibernateUtil.GetSessionT5())
                    using (ISession session_connectNH = HibernateUtil.GetSession())
                    using (var repository = new Conexao<Horario_Connect>(sessionNH))
                    using (var repositoryEquipe = new Conexao<EquipeAniel>(sessionNH))
                    using (var repositoryEquipeStatus = new Conexao<EquipeStatusAniel>(sessionNH))
                    using (var repositoryHorarioEquipe = new Conexao<Horario_Equipe>(sessionNH))
                    using (var repositoryFuncionario = new Conexao<FuncionarioAniel>(sessionNH))
                    using (var feriado_repository = new Conexao<Feriado>(session_connectNH))
                    using (var repocategoria_Clie = new Conexao<Categoria_Cliente>(session_connectNH))
                    using (var repositoryConfig = new Conexao<Configuracoes_Gerais>(session_connectNH))
                    {
                        var configGerais = await repositoryConfig.selectFirst();
                        ITipoServicoAnielRepository repoTipoServico = new TipoServicoAnielRepository(sessionNH, session);
                        IDocumentoProducaoAnielRepository repoDocumento = new DocumentoProducaoAnielRepository(sessionNH, session);
                        IEquipeAnielRepository repoEquipe = new EquipeAnielRepository(session);
                        EquipeAnielBusiness equipeAnielBusiness = new EquipeAnielBusiness(repoEquipe, repoTipoServico, repoDocumento);

                        var horario_modelo = await repository.selectFirst(p => p.Padrao && p.Tipo == Tipo_Hora_Connect.Horario_Servico);
                        bool feriado = feriado_repository.selectFirst(p => p.Data == DataConsultaFim.Value.Date).Result != null;
                        var equipeParametro = dataParametro.ToString("dd/MM/yyyy");
                        equipeParametro = equipeParametro.Replace("/", "");
                        EquipeAniel equipeAniel = await repositoryEquipe.selectFirst(p => p.Matricula == Tecnico);
                        FuncionarioAniel supervisor = equipeAniel != null ? await repositoryFuncionario.selectFirst(p => p.Num_Fre == equipeAniel.Num_Fre) : null;
                        EquipeStatusAniel equipeStatusAniel = await repositoryEquipeStatus.selectFirst(p => p.Equipe.Matricula == Tecnico);
                        Categoria_Cliente categoriaCliente = await repocategoria_Clie.selectFirst(p => p.Tipo_Prioridade == Tipo_Prioridade);
                        var horarioEquipeTecnico = await repositoryHorarioEquipe.selectFirst(p => p.Equipe.Matricula == Tecnico && p.Data == dataParametro);

                        var equipe = new Timeline_ServicosEquipeModels
                        {
                            Equipe = equipeParametro,
                            Nome_Equipe = dataParametro.ToString("dd/MM/yyyy"),
                            Detalhe_Equipe = "",
                            Justificativa = "",
                            Supervisor = supervisor?.Nome,
                            PerfilAgendamento = horarioEquipeTecnico?.PerfilAgendamento?.Descricao ?? equipeAniel?.PerfilAgendamento?.Descricao,
                            Servicos = new List<Timeline_ServicosEquipeServicosModels>()
                        };

                        if (equipeStatusAniel != null)
                        {
                            equipe.Bateria = equipeStatusAniel.Bateria.HasValue ? Convert.ToString(equipeStatusAniel?.Bateria) + "%" : null;
                        }

                        if (horarioEquipeTecnico != null)
                        {
                            if (horarioEquipeTecnico?.Indisponivel == true)
                            {
                                equipeStatusAniel.Tipo = EquipeStatusTipo.Fora_Horario_Trabalho;
                            }

                        }

                        var tipo = equipeStatusAniel?.Tipo ?? EquipeStatusTipo.Sem_Status;
                        var fora_agendamento = await GetTempoNaoUsadoAgendamento(dataParametro, horario_modelo, feriado, horarioEquipeTecnico);
                        //Teste Geissiane
                        var hora_extra = await GetTempoHoraExtra(dataParametro, horario_modelo, feriado, horarioEquipeTecnico);
                        fora_agendamento.AddRange(hora_extra);

                        tipo = equipeAnielBusiness.GetStatusPersonalizado(DataConsultaFim.Value,
                                                                          horario_modelo,
                                                                          feriado,
                                                                          horarioEquipeTecnico,
                                                                          tipo,
                                                                          horarioEquipeTecnico?.PerfilAgendamento ?? equipeAniel?.PerfilAgendamento,
                                                                          equipeAniel.Matricula,
                                                                          configGerais.Tempo_Execucao_Padrao.Value.TimeOfDay);


                        equipe.Cor_StatusEquipe = EquipeApiController.GetCorStatusEquipe(tipo);



                        var RetornoServicos = await GetServicos(dataParametro.ToString("dd/MM/yyyy"), Convert.ToString(Tecnico),
                                                         Monitor: null,
                                                         Supervisor = null,
                                                         TodosTecnicos: false,
                                                         DataHoraParametro: DataHoraParametro,
                                                         Prazo: Prazo,
                                                         TipoVisualizacao: TipoVisualizacao,
                                                         GrupoArea: null,
                                                         Area: null,
                                                         Localidade: null,
                                                         TipoServico: TipoServico,
                                                         SubTipoServico: SubTipoServico,
                                                         NaturezaServico: null,
                                                         TipoPrioridade: Tipo_Prioridade,
                                                         DataCriacaoInicio: null,
                                                         DataCriacaoFim: null,
                                                         contrato: CodCt,
                                                         projeto: Projeto
                                                         );

                        if (RetornoServicos.Ok == false)
                        {
                            erroServicos = RetornoServicos.Mensagem;
                        }
                        ;

                        var servicos = RetornoServicos.Objeto;

                        var equipeTemp = new Timeline_ServicosEquipeModels
                        {
                            Equipe = equipe.Equipe,
                            Detalhe_Equipe = equipe.Detalhe_Equipe,
                            Justificativa = equipe.Justificativa,
                            Nome_Equipe = equipe.Nome_Equipe,
                            Servicos = new List<Timeline_ServicosEquipeServicosModels>()
                        };
                        var servicosTemp = new List<Timeline_ServicosEquipeServicosModels>();

                        var ausenciaTecnico = await GetHorarioAusente(dataParametro, Tecnico, equipe);

                        foreach (var ausencia in ausenciaTecnico)
                        {
                            var periodos = QuebrarPeriodoPorDia(
                                ausencia.DataHoraIni,
                                ausencia.DataHoraFim
                            );

                            foreach (var periodo in periodos)
                            {
                                if (periodo.Inicio.Date == dataParametro)
                                {
                                    servicos.Add(new Timeline_ServicosEquipeServicosModels
                                    {
                                        DataHoraIni = periodo.Inicio,
                                        DataHoraFim = periodo.Fim,
                                        TipoOS = ausencia.TipoOS,
                                        NumDoc = ausencia.NumDoc,
                                        Num_OS = ausencia.Num_OS,
                                        HtmlTitle = ausencia.HtmlTitle,
                                        Equipe = ausencia.Equipe,
                                        Prioridade_Legenda = ausencia.Prioridade_Legenda,
                                        Pixels = (periodo.Fim - periodo.Inicio).TotalMinutes + "px"
                                    });
                                }
                            }
                        }


                        List<Timeline_ServicosEquipeServicosModels> temp = new List<Timeline_ServicosEquipeServicosModels>();
                        foreach (var item in servicos.OrderBy(p => p.DataHoraIni).ToList())
                        {
                            var servicoHorarioOcupado = temp.Find(p => HorarioApiController.IgualDatas(item.DataHoraIni, p.DataHoraIni));
                            if (servicoHorarioOcupado != null)
                            {
                                servicos.Remove(item);
                                servicosTemp.Add(item);
                                servicosTemp.AddRange(fora_agendamento);

                                servicosTemp = servicosTemp.OrderBy(p => p.DataHoraIni.TimeOfDay).ToList();
                                equipeTemp.Servicos.AddRange(PreencheTipoOSNenhum(dataParametro, servicosTemp));
                                retorno.Add(equipeTemp);
                            }

                            //1) Burlar Equipe para exibir em duas datas
                            bool estaEntreDatas = item.DataHoraIni == null ? false : item.DataHoraFim.Date >= DataConsulta && item.DataHoraFim.Date <= DataConsultaFim;
                            if (item.DataHoraFim.Date != item.DataHoraIni.Date && estaEntreDatas)
                            {
                                var temp_ = item;
                                item_.Add(temp_);
                            }
                        }

                        servicos.AddRange(fora_agendamento);
                        servicos = servicos.OrderBy(p => p.DataHoraIni.TimeOfDay).ToList();
                        if (servicos.Count > 0)
                            equipe.Servicos.AddRange(PreencheTipoOSNenhum(dataParametro, servicos));

                        //2) Burlar Equipe para exibir em duas datas
                        if (item_.Count > 0)
                        {
                            var lista = new List<Timeline_ServicosEquipeServicosModels>();

                            foreach (var old in item_)
                            {
                                if (equipe.Equipe == old.DataHoraFim.ToString("dd/MM/yyyy").Replace("/", "")
                                    && equipe.Equipe != old.DataHoraIni.ToString("dd/MM/yyyy").Replace("/", ""))
                                {
                                    if (old.DataHoraIni.Date < old.DataHoraFim.Date)
                                    {
                                        lista.Add(old);
                                    }
                                }
                            }

                            if (lista.Count > 0)
                                equipe.Servicos.AddRange(lista);
                        }

                        retorno.Add(equipe);
                    }
                }

                return new GetApiRetorno<string>
                {
                    Ok = String.IsNullOrEmpty(erroServicos) ? true : false,
                    Objeto = JsonConvert.SerializeObject(retorno),
                    Mensagem = erroServicos
                };
            }
        }


        [HttpPost()]
        [Route("api/Gestao_Equipeapi/ReprocessarPedido")]

        public async Task<OsReprocessadas> ReprocessarPedido(OsReprocessaRequestModel datas)
        {
            var OsProcessadas = new System.Text.StringBuilder();
            var OsNaoProcessadasOuComErro = new System.Text.StringBuilder();
            Int32 Processadas = 0;
            Int32 NaoProcessadas = 0;
            using (ISession session = HibernateUtil.GetSessionT5())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Count
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder(@"SELECT DISTINCT V_OSPROCESSADAS, V_OSCOMERRO, V_NUM_DOC_REP, V_NUM_DOC_ERR FROM SP_REPROCESSAR_REINCIDENCIAS(@DATAINICIAL,@DATAFINAL)");

                    parametros.Add("@DATAINICIAL", datas.DataInicio);
                    parametros.Add("@DATAFINAL", datas.DataFim);

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {

                        while (await reader.ReadAsync())
                        {
                            Processadas += reader["V_OSPROCESSADAS"] != DBNull.Value ? Convert.ToInt32(reader["V_OSPROCESSADAS"]) : 0;
                            NaoProcessadas += Convert.ToInt32(reader["V_OSCOMERRO"]);
                            OsProcessadas.AppendLine(reader["V_NUM_DOC_REP"] == null ? ", " : Convert.ToString(reader["V_NUM_DOC_REP"]) + "");
                            OsNaoProcessadasOuComErro.AppendLine(reader["V_NUM_DOC_ERR"] == null ? ", " : Convert.ToString(reader["V_NUM_DOC_ERR"]) + "");
                        }


                    }
                }

            }
            OsReprocessadas retorno = new OsReprocessadas()
            {
                TotalProcessada = Processadas.ToString(),
                TotalNaoReprocessadaOuComErro = NaoProcessadas.ToString(),
                OsReprocessada = OsProcessadas.ToString(),
                OsNaoReprocessada = OsNaoProcessadasOuComErro.ToString()
            };

            return retorno;

        }

        [AcceptVerbs("Get", "Post")]
        [Route("api/Gestao_Equipeapi/GetTimeLine")]
        public async Task<GetApiRetorno<List<Timeline_ServicosEquipeModels>>> GetTimeLine(Int32? TipoFiltro, Int32? TipoVisualizacao, Int32? Prazo, Int32? FormatoTimeline,
            String Supervisor, String Id_Monitor, String Cod_Escritorio, String Cod_Area, Int32? Cod_Localidade, Int64? Id_Natureza, DateTime? Data, DateTime? DataFinal, String Tecnico, String CodDepto, string CodCt, string Projeto, String JornadaTrabalho)
        {
            List<Timeline_ServicosEquipeModels> retorno = new List<Timeline_ServicosEquipeModels>();
            string erroServicos = null;
            //FormatoTimeline 0: Técnico
            //FormatoTimeline 1: Data
            if (FormatoTimeline == 0)
            {

            }

            //TipoVisualizacao 0: Prazo Execução
            //TipoVisualizacao 1: Status Serviço
            if (TipoVisualizacao == 0)
            {

            }

            //Prazo 0: SLA
            //Prazo 1: Janela de Atendimento
            if (Prazo == 0)
            {
                //Calcular o SLA de acordo com a OS
            }


            try
            {

                var fora_agendamento = await GetTempoNaoUsadoAgendamento(Data.Value);

                using (ISession session = HibernateUtil.GetSessionT5())
                {

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {

                        StringBuilder sql = new StringBuilder(" SELECT A.EQUIPE, A.NOME, C.TIPO, C.JUSTIFICATIVA ")
                            .AppendLine(" FROM TB_EQUIPE A ")
                            .AppendLine(" INNER JOIN TB_FUNCIONARIO B ")
                            .AppendLine(" ON(A.NUM_FRE = B.NUM_FRE) ")

                            .AppendLine(" LEFT JOIN TB_EQUIPE_STATUS C ")
                            .AppendLine(" ON(A.EQUIPE = C.EQUIPE) ")

                            .AppendLine(" WHERE (B.SITUACAO = 0 OR B.SITUACAO = 4) AND A.EQUIPE_CONNECT = 'S' ")
                              .AppendLine(" AND COALESCE((SELECT FIRST 1 E.INDISPONIVEL ")
                                             .AppendLine("  FROM TB_HORARIO_EQUIPE_CONNECT E ")
                                            .AppendLine("  WHERE E.EQUIPE = A.EQUIPE AND E.INDISPONIVEL = 1 ")
                                            .AppendLine("    AND E.DATA BETWEEN @DATA AND @DATAFIM), 0) = 0 ");
                        //.AppendLine("    AND E.DATA = @DATA), 0) = 0 ");

                        parametros.Add("@DATA", Data.Value.Date);
                        parametros.Add("@DATAFIM", Data.Value.Date);

                        if (!String.IsNullOrEmpty(Supervisor))
                        {
                            sql.AppendLine(" AND A.COD_SUPERV = @SUPERVISOR");
                            parametros.Add("@SUPERVISOR", Supervisor);
                        }

                        if (!String.IsNullOrEmpty(Id_Monitor))
                        {
                            sql.AppendLine(" AND A.USUARIO_MONITOR = UPPER(@ID_MONITOR) ");
                            parametros.Add("@ID_MONITOR", Id_Monitor);
                        }

                        if (!String.IsNullOrEmpty(Cod_Escritorio) ||
                            !String.IsNullOrEmpty(Cod_Area) ||
                            Cod_Localidade.HasValue)
                        {
                            sql.AppendLine("   AND A.EQUIPE IN(SELECT C.EQUIPE FROM TB_EQUIPE_LOCALIDADE C ")
                                .AppendLine("                    WHERE C.COD_ESCRITORIO = @COD_ESCRITORIO ");
                            parametros.Add("@COD_ESCRITORIO", Cod_Escritorio);

                            if (!String.IsNullOrEmpty(Cod_Area))
                            {
                                sql.AppendLine(" AND C.COD_AREA = @COD_AREA ");
                                parametros.Add("@COD_AREA", Cod_Area);
                            }

                            if (Cod_Localidade.HasValue)
                            {
                                sql.AppendLine(" AND (C.COD_LOCALIDADE = @COD_LOCALIDADE OR C.TODAS_LOCALIDADES = 1)");
                                parametros.Add("@COD_LOCALIDADE", Cod_Localidade);
                            }

                            sql.AppendLine(") ");
                        }

                        if (!String.IsNullOrEmpty(Tecnico))
                        {
                            sql.AppendLine(" AND (B.NUM_FRE = @NUM_FRE )");
                            parametros.Add("@NUM_FRE", Tecnico);
                        }

                        if (Id_Natureza.HasValue)
                        {
                            sql.AppendLine("   AND A.EQUIPE IN(SELECT D.EQUIPE ")
                            .AppendLine("                        FROM TB_EQUIPE_NATUREZA D ")
                            .AppendLine("                       WHERE D.ID_NATUREZA = @ID_NATUREZA) ");
                            parametros.Add("@ID_NATUREZA", Id_Natureza);
                        }

                        sql.AppendLine(" ORDER BY UPPER(A.NOME) ");
                        //Busca com paginacao
                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                var equipe = new Timeline_ServicosEquipeModels
                                {
                                    Equipe = Convert.ToString(reader["EQUIPE"]),
                                    Nome_Equipe = Convert.ToString(reader["NOME"]),
                                    Detalhe_Equipe = Convert.ToString(reader["TIPO"]) == "1" ? "Técnico Ausente" : String.Empty,
                                    Justificativa = Convert.ToString(reader["JUSTIFICATIVA"])?.Replace("\r\n", @"<br/>"),
                                    Servicos = new List<Timeline_ServicosEquipeServicosModels>()
                                };

                                //if (equipe.Indisponivel)
                                //{
                                //    equipe.Servicos.Add(new Timeline_ServicosEquipeServicosModels
                                //    {
                                //        TipoOS = Timeline_ServicosEquipeServicosEnum.Técnico_Ausente,
                                //        Pixels = (24 * 60).ToString() + "px"
                                //    });
                                //}
                                //else
                                //{
                                try
                                {
                                    var RetornoServicos = await GetServicos(Data.Value.ToString("dd/MM/yyyy"), Convert.ToString(reader["EQUIPE"]),
                                                         Monitor: null,
                                                         Supervisor = null,
                                                         TodosTecnicos: false,
                                                         DataHoraParametro: "",
                                                         Prazo: Prazo,
                                                         TipoVisualizacao: TipoVisualizacao,
                                                         GrupoArea: null,
                                                         Area: null,
                                                         Localidade: null,
                                                         TipoServico: null,
                                                         SubTipoServico: null,
                                                         NaturezaServico: null,
                                                         TipoPrioridade: null,
                                                         DataCriacaoInicio: null,
                                                         DataCriacaoFim: null,
                                                         contrato: CodCt,
                                                         projeto: Projeto);

                                    if (RetornoServicos.Ok == false)
                                    {
                                        erroServicos = RetornoServicos.Mensagem;
                                    }
                                    ;

                                    var servicos = RetornoServicos.Objeto;

                                    var equipeTemp = new Timeline_ServicosEquipeModels
                                    {
                                        Equipe = equipe.Equipe,
                                        Detalhe_Equipe = equipe.Detalhe_Equipe,
                                        Justificativa = equipe.Justificativa,
                                        Nome_Equipe = equipe.Nome_Equipe,
                                        Servicos = new List<Timeline_ServicosEquipeServicosModels>()
                                    };
                                    var servicosTemp = new List<Timeline_ServicosEquipeServicosModels>();
                                    List<Timeline_ServicosEquipeServicosModels> temp = new List<Timeline_ServicosEquipeServicosModels>();
                                    foreach (var item in servicos.OrderBy(p => p.DataHoraIni).ToList())
                                    {
                                        var servicoHorarioOcupado = temp.Find(p => HorarioApiController.IgualDatas(item.DataHoraIni, p.DataHoraIni));

                                        if (servicoHorarioOcupado != null)
                                        {

                                            servicos.Remove(item);

                                            servicosTemp.AddRange(fora_agendamento);
                                            servicosTemp = servicosTemp.OrderBy(p => p.DataHoraIni.TimeOfDay).ToList();
                                            equipeTemp.Servicos.AddRange(PreencheTipoOSNenhum(Data.Value, servicosTemp));
                                            retorno.Add(equipeTemp);
                                        }
                                        else
                                        {
                                            temp.Add(item);
                                        }
                                    }

                                    servicos.AddRange(fora_agendamento);
                                    servicos = servicos.OrderBy(p => p.DataHoraIni.TimeOfDay).ToList();
                                    if (servicos.Count > 0)
                                        equipe.Servicos.AddRange(PreencheTipoOSNenhum(Data.Value, servicos));
                                }
                                catch { }
                                //}

                                retorno.Add(equipe);
                            }
                        }

                        return new GetApiRetorno<List<Timeline_ServicosEquipeModels>>
                        {
                            Ok = String.IsNullOrEmpty(erroServicos) ? true : false,
                            Objeto = retorno,
                            Mensagem = erroServicos
                        };
                    }
                }

            }
            catch (Exception ex)
            {
                return new GetApiRetorno<List<Timeline_ServicosEquipeModels>>
                {
                    Ok = false,
                    Mensagem = ex.Message,
                    InnerException = ex.InnerException?.Message
                };
            }

        }

        [AcceptVerbs("Get", "Post")]
        [Route("api/Gestao_Equipeapi/GetParticipantes")]
        public async Task<List<ServicoUsuario>> GetParticipantes(string codct, string proj, string Num_OS, int numDoc)
        {
            List<ServicoUsuario> tecnicos = new List<ServicoUsuario>();
            List<ServicoUsuario> retorno = new List<ServicoUsuario>();

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                Dictionary<String, Object> parametrosMonSup = new Dictionary<string, object>();



                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder(" SELECT C.NUM_FRE, C.NOME, 0 AS TIPO FROM TB_DOCUMENTO_PRODUCAO A ")
                    .AppendLine("   INNER JOIN TB_EQUIPE B ON B.EQUIPE = A.EQUIPE ")
                    .AppendLine("   INNER JOIN TB_FUNCIONARIO C ON C.NUM_FRE = B.NUM_FRE ")
                    .AppendLine("   WHERE A.NUM_OBRA = @NUM_OS ");
                    if (!String.IsNullOrEmpty(codct) && !String.IsNullOrEmpty(proj))
                    {
                        sql.AppendLine(" AND A.CODCT = @CODCT AND A.PROJETO = @PROJETO AND A.NUM_DOC = @NUMDOC ");
                        parametros.Add("@CODCT", codct);
                        parametros.Add("@PROJETO", proj);                        
                        parametros.Add("@NUMDOC", numDoc);
                    }
                    parametros.Add("@NUM_OS", Num_OS);

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            var equipe = new ServicoUsuario
                            {
                                ativo = true,
                                num_fre = Convert.ToString(reader["NUM_FRE"]),
                                nome = Convert.ToString(reader["NOME"]),
                                //foto_usuario = Convert.ToString(reader["FOTO"]), adicionar na busca
                                tipo_usuario = reader["TIPO"] != DBNull.Value ? (TipoUsuarioChatEnum)Enum.Parse(typeof(TipoUsuarioChatEnum), Convert.ToString(reader["TIPO"])) : TipoUsuarioChatEnum.AGENTE
                            };

                            tecnicos.Add(equipe);
                        }
                    }
                }

                //Select monitores e supervisores
                foreach (var tec in tecnicos)
                {
                    using (var com = session.Connection.CreateCommand())
                    {
                        //Busca os técnicos/equipe vinculados à essa OS
                        StringBuilder sqlMonitor = new StringBuilder("  SELECT A.NUM_FRE, A.NOME,  2 AS TIPO FROM TB_EQUIPE A ")
                        .AppendLine("   WHERE NUM_FRE IN ")
                        .AppendLine("   (SELECT USUARIO_MONITOR FROM TB_EQUIPE WHERE NUM_FRE = @NUM_FRE) ")
                        .AppendLine("   UNION ")
                        .AppendLine("  SELECT A.NUM_FRE, A.NOME,  1 AS TIPO FROM TB_EQUIPE A ")
                        .AppendLine("   WHERE EQUIPE IN ")
                        .AppendLine("   (SELECT COD_SUPERV FROM TB_EQUIPE WHERE NUM_FRE = @NUM_FRE) ");
                        parametrosMonSup.Add("@NUM_FRE", tec.num_fre);

                        using (var readerSup = await CommandHelper.ExecuteReader(com, sqlMonitor, parametrosMonSup, session.Connection, null))
                        {
                            while (await readerSup.ReadAsync())
                            {
                                var equipeMon = new ServicoUsuario
                                {
                                    ativo = true,
                                    num_fre = Convert.ToString(readerSup["NUM_FRE"]),
                                    nome = Convert.ToString(readerSup["NOME"]),
                                    //foto_usuario = Convert.ToString(reader["FOTO"]), adicionar na busca
                                    tipo_usuario = readerSup["TIPO"] != DBNull.Value ? (TipoUsuarioChatEnum)Enum.Parse(typeof(TipoUsuarioChatEnum), Convert.ToString(readerSup["TIPO"])) : TipoUsuarioChatEnum.AGENTE
                                };

                                retorno.Add(equipeMon);
                            }

                            retorno.AddRange(tecnicos);
                        }
                    }
                }
            }
            return retorno;
        }

        [HttpPost]
        [Route("api/Gestao_Equipeapi/GetVerificaTimeline")]
        public async Task<PostApiRetornoVerificaTimeline<string>> GetVerificaTimeline(TimelineParametros request)
        {
            PostApiRetornoVerificaTimeline<string> retorno = new PostApiRetornoVerificaTimeline<string>();

            retorno = new PostApiRetornoVerificaTimeline<string>()
            {
                Mensagens = new List<PostApiMensagem>(),
                Ok = true
            };

            DateTime DataConsulta = Convert.ToDateTime(request.DataInicio);
            DateTime DataConsultaFim = Convert.ToDateTime(request.DataFim);
            using (ISession session_connectNH = HibernateUtil.GetSession())
            using (var repositoryConfig = new Conexao<Configuracoes_Gerais>(session_connectNH))
            {
                var configGerais = await repositoryConfig.selectFirst();

                //1)VALIDA SE DATA É RETROATIVA
                DateTime DataAtual = Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                if (DataConsulta <= DataAtual && DataConsultaFim <= DataAtual)
                {
                    retorno.Ok = false;
                    retorno.Mensagem = "Não é possível agendar uma data e/ou horário retroativo!";

                    return retorno;
                }

                //2) VALIDA HORÁRIO PERMITIDO
                List<AreaModuloModel> areas = new List<AreaModuloModel>();
                using (ISession sessionNH = HibernateUtil.GetSessionT5())

                using (var repositoryHorarioEquipe = new Conexao<Horario_Equipe>(sessionNH))
                using (var repositoryTipo_Servico_EquipeAniel = new Conexao<Tipo_Servico_EquipeAniel>(sessionNH))
                using (var repository = new Conexao<Horario_Connect>(sessionNH))
                using (var repositoryLocalidade = new Conexao<LocalidadeAniel>(sessionNH))
                using (var repositoryDocumentoProducao = new Conexao<Documento_ProducaoAniel>(sessionNH))

                using (var repositoryEquipe = new Conexao<EquipeAniel>(sessionNH))
                using (var feriado_repository = new Conexao<Feriado>(session_connectNH))
                using (var acessoApi = new AcessoApiController())
                {
                    var horario_modelo = await repository.selectFirst(p => p.Padrao && p.Tipo == Tipo_Hora_Connect.Horario_Servico);
                    bool feriado = feriado_repository.selectFirst(p => p.Data == DataConsultaFim.Date).Result != null;
                    var horarioEquipeTecnico = await repositoryHorarioEquipe.selectFirst(p => p.Equipe.Matricula == request.Grupo && p.Data == DataConsulta.Date);
                    List<Timeline_ServicosEquipeServicosModels> tempoNaoUsado = await GetTempoNaoUsadoAgendamento(DataConsulta, horario_modelo, feriado, horarioEquipeTecnico);
                    areas = acessoApi.GetArea_ModuloUsuario(User.Identity.GetUserId()).Result.Objeto;
                    if (areas.Where(p => p.Cod_Area == "188").Count() == 0)
                    {
                        foreach (var horariosNaoPermitidos in tempoNaoUsado)
                        {
                            var start = TimeSpan.Parse(horariosNaoPermitidos.DataHoraIni.ToString("HH:mm:ss"));
                            var end = TimeSpan.Parse(horariosNaoPermitidos.DataHoraFim.ToString("HH:mm:ss"));
                            var dataParametro = TimeSpan.Parse(DataConsulta.ToString("HH:mm:ss"));
                            var dataParametroFim = TimeSpan.Parse(DataConsultaFim.ToString("HH:mm:ss"));

                            if (dataParametro >= start && dataParametro <= end || dataParametroFim >= start && dataParametroFim <= end)//if(dataParametroFim > start || dataParametro >= start)
                            {
                                retorno = new PostApiRetornoVerificaTimeline<string>()
                                {
                                    Ok = false,
                                    Mensagem = "O horário escolhido não está disponível para agendamento!"
                                };
                            }
                        }
                    }


                    //4) VALIDA SE USUÁRIO ESTÁ INDISPONÍVEL
                    if (areas.Where(p => p.Cod_Area == "188").Count() == 0)
                    {
                        List<Timeline_ServicosEquipeServicosModels> tecnicoIndisponivel = await GetHorarioAusente(DataConsulta, request.Grupo, null);
                        foreach (var horarioIndisponivelAus in tecnicoIndisponivel)
                        {
                            var start = TimeSpan.Parse(horarioIndisponivelAus.DataHoraIni.ToString("HH:mm:ss"));
                            var end = TimeSpan.Parse(horarioIndisponivelAus.DataHoraFim.ToString("HH:mm:ss"));
                            var dataParametro = TimeSpan.Parse(DataConsulta.ToString("HH:mm:ss"));
                            var dataParametroFim = TimeSpan.Parse(DataConsultaFim.ToString("HH:mm:ss"));

                            if (dataParametro >= start && dataParametro <= end || dataParametroFim >= start && dataParametroFim <= end)
                            {
                                retorno = new PostApiRetornoVerificaTimeline<string>()
                                {
                                    Ok = false,
                                    Mensagem = "O horário escolhido não está disponível para agendamento pois o técnico se encontra ausente!"
                                };
                            }
                        }
                    }

                    //3) VALIDA PERFIL CAPAZ
                    Documento_ProducaoAniel modelo = await repositoryDocumentoProducao.selectFirst(p => p.CodCt == request.CodCt &&
                                                                                                        p.Projeto == request.Projeto &&
                                                                                                        p.Num_Doc == request.NumDoc &&
                                                                                                        p.Num_Obra == request.Num_OS);
                    if (modelo?.Responsabilidade == ResponsabilidadeOS.TECNICO)
                    {
                        retorno.Ok = false;
                        retorno.Mensagem = $"Ordem de {RsCadastros.SERVICO} {modelo.Num_Obra} se encontra com a responsabilidade {RsCadastros.TECNICO}.";
                        return retorno;
                    }

                    LocalidadeAniel localidade = await repositoryLocalidade.selectFirst(p => p.Cod_Escritorio == modelo.Cod_Escritorio && p.Cod_Area == modelo.Cod_Area && p.Cod_Localidade == modelo.Cod_Localidade);
                    var tipoServ = await repositoryTipo_Servico_EquipeAniel.selectFirst(p => p.Cod_Tipo_Serv == modelo.Cod_Tipo_Serv);

                    var equipe = await repositoryEquipe.selectFirst(p => p.Matricula == request.Grupo);
                    PerfilAgendamento perfilAgendamento = horarioEquipeTecnico?.PerfilAgendamento ?? equipe.PerfilAgendamento;
                    if (perfilAgendamento == null && (configGerais.Criterio_Area || configGerais.Criterio_Habilidade))
                    {
                        retorno.Ok = false;
                        retorno.Mensagens.Add(new PostApiMensagem { Mensagem = "tecnico_nao_apto" });
                    }
                    else
                    {

                        if (configGerais.Criterio_Habilidade && configGerais.Criterio_Area)
                        {
                            if (perfilAgendamento.Naturezas.Where(p => p.Natureza.Id == tipoServ.Cod_Natureza).Count() == 0 ||
                                perfilAgendamento.Areas.Where(p => p.Escritorio == modelo.Cod_Escritorio && p.Area == modelo.Cod_Area && (p.Todas_Localidades || p.Localidade == localidade.LocalidadeStr)).Count() == 0)
                            {
                                retorno.Ok = false;
                                retorno.Mensagens.Add(new PostApiMensagem { Mensagem = "tecnico_nao_apto" });

                            }
                        }
                        else if (configGerais.Criterio_Habilidade)
                        {
                            if (perfilAgendamento.Naturezas.Where(p => p.Natureza.Id == tipoServ.Cod_Natureza).Count() == 0)
                            {
                                retorno.Ok = false;
                                retorno.Mensagens.Add(new PostApiMensagem { Mensagem = "tecnico_nao_apto" });

                            }
                        }
                        else if (configGerais.Criterio_Area)
                        {
                            if (perfilAgendamento.Areas.Where(p => p.Escritorio == modelo.Cod_Escritorio && p.Area == modelo.Cod_Area && (p.Todas_Localidades || p.Localidade == localidade.LocalidadeStr)).Count() == 0)
                            {
                                retorno.Ok = false;
                                retorno.Mensagens.Add(new PostApiMensagem { Mensagem = "tecnico_nao_apto" });
                            }
                        }
                    }
                }

                using (ISession session = HibernateUtil.GetSessionT5())
                using (var repository = new Conexao<Documento_ProducaoAniel>(session))
                using (var repositoryAgendamento = new Conexao<Agendamento_EquipeAniel>(session))
                using (var RoteirizacaoApi = new Painel_RoteirizacaoApiController())
                {

                    Documento_ProducaoAniel modelo = await repository.selectFirst(p => p.CodCt == request.CodCt &&
                                                                                       p.Projeto == request.Projeto &&
                                                                                       p.Num_Doc == request.NumDoc &&
                                                                                       p.Num_Obra == request.Num_OS);

                    Agendamento_EquipeAniel agendamento = await repositoryAgendamento.selectFirst(p => p.CodCt == modelo.CodCt &&
                                                                                                       p.Projeto == modelo.Projeto &&
                                                                                                       p.Num_Doc == modelo.Num_Doc &&
                                                                                                       p.Num_Obra == modelo.Num_Obra);

                    Agendamento_EquipeAniel agendamentoAnterior = agendamento != null ? repositoryAgendamento.select(p => p.Equipe.Matricula == request.Grupo &&
                                                                                                            p.Id != agendamento.Id &&
                                                                                                            p.DataHora_Inicio.Date == Convert.ToDateTime(request.DataInicio).Date &&
                                                                                                            p.DataHora_Inicio < Convert.ToDateTime(request.DataInicio)).Result.OrderByDescending(p => p.DataHora_Inicio).FirstOrDefault() : null;

                    if (agendamentoAnterior == null)
                    {
                        agendamentoAnterior = repositoryAgendamento.select(p => p.Equipe.Matricula == request.Grupo &&
                                                                                p.DataHora_Inicio.Date == Convert.ToDateTime(request.DataInicio).Date &&
                                                                                p.DataHora_Inicio < Convert.ToDateTime(request.DataInicio)).Result.
                                                                    OrderByDescending(p => p.DataHora_Inicio).
                                                                    FirstOrDefault();
                    }

                    DateTime DataFimAntiga = new DateTime(Convert.ToDateTime(request.DataFim).Ticks);
                    if (agendamentoAnterior != null)
                    {
                        var roteirizacaoBase = new RoteirizacaoBusiness();
                        Documento_ProducaoAniel ServicoAnterior = await repository.selectFirst(p => p.Num_Obra == agendamentoAnterior.Num_Obra &&
                                                                                                    p.CodCt == agendamentoAnterior.CodCt &&
                                                                                                    p.Projeto == agendamentoAnterior.Projeto &&
                                                                                                    p.Num_Doc == agendamentoAnterior.Num_Doc);

                        var documentoRoteirizar = new DocumentoRoteirizar
                        {
                            Latitude_Str = modelo.Latitude_Str,
                            Longitude_Str = modelo.Longitude_Str,
                        };



                        double distancia = Math.Ceiling(roteirizacaoBase.GetDistancias(ServicoAnterior.GetLatitude_Str(), ServicoAnterior.GetLongitude_Str(), new List<DocumentoRoteirizar>() { documentoRoteirizar }).Values.First() / 1852);
                        var tempoPadraoDeslocamento = configGerais.Tempo_Medio_Deslocamento.Value.Minute;
                        TimeSpan tempo_deslocamento = (ServicoAnterior?.Latitude_Str != null && ServicoAnterior?.Longitude_Str != null && modelo.Latitude_Str != null && modelo.Longitude_Str != null) ? roteirizacaoBase.GetTempoDeslocamento(configGerais, distancia, ReferenciaDistancia.UltimoServico) : TimeSpan.FromMinutes(tempoPadraoDeslocamento); /*TimeSpan.Zero*/
                        var agendamentoOld = agendamento?.Deslocamento ?? 0;

                        if (agendamento != null)
                            agendamento.Deslocamento = tempo_deslocamento.TotalMinutes;

                        var diffDeslocamento = tempo_deslocamento.TotalMinutes - agendamentoOld;
                        request.DataFim = Convert.ToDateTime(request.DataFim).AddMinutes(diffDeslocamento).ToString("dd/MM/yyyy HH:mm");
                        retorno.DataFim = request.DataFim;
                    }

                    RoteirizacaoApi.HoraRoteirizacao = Convert.ToDateTime(request.DataFim);
                    var resultadoEncaixe = await RoteirizacaoApi.CompactarRotaEncaixe(new CompactarRotaEncaixeModel
                    {
                        Equipe = request.Grupo,
                        DataHoraFimEncaixe = request.DataInicio,
                        DataHoraFimAntigoAgendamento = request.DataInicio,
                        NumObraEncaixe = modelo.Num_Obra,
                        SalvarRota = false,
                    });


                    if (resultadoEncaixe.Ok == false || resultadoEncaixe.QtdOsCompactar > resultadoEncaixe.QtdOsRoteirizada)
                    {
                        retorno.Ok = false;
                        retorno.Mensagens.Add(new PostApiMensagem { Mensagem = "nao será possível montar rota após encaixe" });
                    }
                    else
                    {
                        retorno.DataFim = Convert.ToDateTime(request.DataFim).ToString("dd/MM/yyyy HH:mm");
                    }

                }



                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    Int32 recordsTotal = 0;
                    using (var repository = new Conexao<Documento_ProducaoAniel>(session))
                    using (var repositoryAgendamento = new Conexao<Agendamento_EquipeAniel>(session))
                    {

                        Documento_ProducaoAniel modelo = await repository.selectFirst(p => p.CodCt == request.CodCt &&
                                                                                           p.Projeto == request.Projeto &&
                                                                                           p.Num_Doc == request.NumDoc &&
                                                                                           p.Num_Obra == request.Num_OS);

                        if (request.FormatoTimeline == "0")
                        {
                            request.Grupo = Convert.ToString(modelo.Equipe);
                        }

                        //Filtros
                        Dictionary<String, Object> parametros = new Dictionary<string, object>();
                        StringBuilder sql = new StringBuilder(" FROM TB_AGENDAMENTO_EQUIPEANIEL A ");
                        sql.AppendLine("INNER JOIN TB_DOCUMENTO_PRODUCAO B ON A.NUM_OBRA = B.NUM_OBRA ");
                        sql.AppendLine("WHERE (A.DATAHORA_INICIO BETWEEN @DATA_INICIO AND @DATA_FIM OR A.DATAHORA_FIM BETWEEN @DATA_INICIO AND @DATA_FIM)");
                        sql.AppendLine("AND A.EQUIPE = @EQUIPE AND A.NUM_OBRA <> @NUM_OBRA AND B.STATUS_EXECUCAO NOT IN (1,2,9,8)");
                        parametros.Add("@DATA_INICIO", DataConsulta);
                        parametros.Add("@DATA_FIM", DataConsultaFim);
                        parametros.Add("@EQUIPE", request.Grupo);
                        parametros.Add("@NUM_OBRA", modelo.Num_Obra);

                        string listaOS = "";


                        //Count
                        using (var com = session.Connection.CreateCommand())
                        {
                            StringBuilder sqlFrom = new StringBuilder("SELECT COUNT(A.ID) AS CO  ");
                            sqlFrom.AppendLine(sql.ToString());

                            using (var reader = await CommandHelper.ExecuteReader(com, sqlFrom, parametros, session.Connection, null))
                            {
                                if (await reader.ReadAsync())
                                {
                                    recordsTotal = Convert.ToInt32(reader["CO"]);
                                }
                            }
                        }

                        if (recordsTotal > 0)
                        {
                            //Select
                            using (var com = session.Connection.CreateCommand())
                            {
                                StringBuilder sqlFrom = new StringBuilder();

                                sqlFrom.AppendLine("SELECT A.NUM_OBRA, A.DATAHORA_INICIO, A.DATAHORA_FIM, B.NUM_OBRA_ORIGINAL ");
                                sqlFrom.AppendLine(sql.ToString());

                                using (var reader = await CommandHelper.ExecuteReader(com, sqlFrom, parametros, session.Connection, null))
                                {
                                    while (await reader.ReadAsync())
                                    {
                                        var Num_Doc = Convert.ToString(reader["NUM_OBRA_ORIGINAL"]);
                                        DateTime DataHora_Inicio = Convert.ToDateTime(reader["DATAHORA_INICIO"]);
                                        DateTime DataHora_Fim = Convert.ToDateTime(reader["DATAHORA_FIM"]);

                                        if (DataHora_Inicio.ToString("HH:mm") != DataConsultaFim.ToString("HH:mm"))
                                        {
                                            listaOS += ("<b> " + Num_Doc + ": " + "</b>" + DataHora_Inicio.ToString("dd/MM/yyyy") + " " + DataHora_Inicio.ToString("HH:mm") + " às " + DataHora_Fim.ToString("HH:mm") + "<br/>");
                                        }
                                    }
                                }

                                if (listaOS != "")
                                {
                                    retorno.Ok = false;
                                    retorno.Mensagens.Add(new PostApiMensagem { Mensagem = "mesmo_horario" });
                                    retorno.IdObjeto = listaOS;
                                }

                            }
                        }
                    }
                }
            }

            return retorno;
        }

        [HttpPost]
        [Route("api/Gestao_Equipeapi/GravarDadosGerais")]
        public async Task<PostApiRetorno<String>> GravarDadosGerais(TimelineParametros value)
        {
            PostApiRetorno<String> retorno = new PostApiRetorno<String>();

            var codCt = value.CodCt;
            var projeto = value.Projeto;
            var numDoc = value.NumDoc;
            var numObra = value.Num_OS;

            var grupo = value.Grupo;
            var DataInicio = value.DataInicio;
            var DataFim = value.DataFim;

            try
            {
                Documento_ProducaoAniel modelo = null;

                using (ISession session = HibernateUtil.GetSessionT5())
                using (ISession sessionConnect = HibernateUtil.GetSession())
                {
                    using (var repoConfig = new Conexao<Configuracoes_Gerais>(sessionConnect))
                    using (var repoTipo_Servico_EquipeAniel = new Conexao<Tipo_Servico_EquipeAniel>(session))
                    using (var repoTipo_ImovelAniel = new Conexao<Tipo_ImovelAniel>(session))
                    using (var repoTipoServico = new Conexao<Tipo_Servico_EquipeAniel>(session))
                    using (var repoSubTipoServico = new Conexao<Sub_Tipo_ServicoAniel>(session))
                    using (var repository = new Conexao<Documento_ProducaoAniel>(session))
                    using (var repositoryAgendamento = new Conexao<Agendamento_EquipeAniel>(session))
                    {

                        modelo = await repository.selectFirst(p => p.CodCt == codCt &&
                                                                   p.Projeto == projeto &&
                                                                   p.Num_Doc == numDoc &&
                                                                   p.Num_Obra == numObra);

                        Configuracoes_Gerais config = await repoConfig.selectFirst();
                        Agendamento_EquipeAniel agendamento = await repositoryAgendamento.selectFirst(p => p.CodCt == modelo.CodCt &&
                                                                                                           p.Projeto == modelo.Projeto &&
                                                                                                           p.Num_Doc == modelo.Num_Doc &&
                                                                                                           p.Num_Obra == modelo.Num_Obra);

                        if (agendamento == null)
                        {
                            agendamento = new Agendamento_EquipeAniel
                            {
                                CodCt = modelo.CodCt,
                                Projeto = modelo.Projeto,
                                Num_Obra = modelo.Num_Obra,
                                Num_Doc = modelo.Num_Doc,
                                Cod_Escritorio = modelo.Cod_Escritorio,
                                Cod_Area = modelo.Cod_Area,
                                Cod_Localidade = modelo.Cod_Localidade,
                                DataHora_Inicio = Convert.ToDateTime(DataInicio),
                                DataHora_Fim = Convert.ToDateTime(DataFim),
                                Flexivel = null,
                                Deslocamento = 0,
                                //Log = GerarLog(value.Objeto.Lista_Forca_Trabalho, value.Objeto.Lista_Ja_Agendados, config),
                                Tipo_Servico = modelo.Cod_Tipo_Serv.HasValue ? await repoTipo_Servico_EquipeAniel.selectFirst(p => p.Cod_Tipo_Serv == modelo.Cod_Tipo_Serv) : null,
                                Tipo_Imovel = modelo.Cod_Tipo_Imovel.HasValue ? await repoTipo_ImovelAniel.selectFirst(p => p.Cod_Tipo_Imovel == modelo.Cod_Tipo_Imovel) : null,
                                UsuarioCriacao = User.Identity.GetUserId(),
                                Origem = OrigemAgendamentoOS.Manual,
                                Latitude = modelo.GetLatitude_Str(),
                                Longitude = modelo.GetLongitude_Str(),
                            };
                        }



                        var data_old = modelo.Data_Maxima;
                        var hora_old = modelo.Hora_Maxima;
                        var lat_old = modelo.Latitude_Str;
                        var long_old = modelo.Longitude_Str;
                        var status_old = modelo.Status;
                        var tipo_servico_old = await repoTipoServico.selectFirst(p => p.Cod_Tipo_Serv == modelo.Cod_Tipo_Serv);
                        var tipo_servico = await repoTipoServico.selectFirst(p => p.Cod_Tipo_Serv == modelo.Cod_Tipo_Serv);

                        var sub_tipo_servico_old = await repoSubTipoServico.selectFirst(p => p.Codigo_Sub_Tipo == modelo.Cod_Sub_Tipo_Serv && p.Codigo_Tipo == modelo.Cod_Tipo_Serv);
                        var sub_tipo_servico = await repoSubTipoServico.selectFirst(p => p.Codigo_Sub_Tipo == modelo.Cod_Sub_Tipo_Serv && p.Codigo_Tipo == Convert.ToInt16(modelo.Cod_Tipo_Serv));

                        var equipe_old = await new Conexao<EquipeAniel>(session).selectFirst(p => p.Matricula == modelo.Equipe);

                        //if (value.FormatoTimeline == "0")
                        //{
                        //    grupo = Convert.ToString(equipe_old?.Matricula);
                        //}


                        if (modelo == null)
                        {
                            modelo = new Documento_ProducaoAniel
                            {

                            };
                        }
                        else
                        {
                            DateTime temp;
                            if (DateTime.TryParse(DataInicio, out temp))
                            {
                                //Convert.ToDateTime(String.Format(value.Data, "dd/MM/yyyy") + ' ' + String.Format(value.Hora, "hh:mm"));
                                modelo.Data_Maxima = Convert.ToDateTime(String.Format(DataInicio, "dd/MM/yyyy"));
                                modelo.Hora_Maxima = Convert.ToDateTime(String.Format(DataInicio, "HH:mm"));
                            }

                            if (modelo.Data_Maxima.Value.Date != data_old || modelo.Hora_Maxima.Value.TimeOfDay != hora_old.Value.TimeOfDay)
                            {
                                if (new List<StatusExecucaoAniel> { StatusExecucaoAniel.Fechada_Produtiva,
                                StatusExecucaoAniel.Fechada_Improdutiva,
                                StatusExecucaoAniel.Atendimento_Finalizado}.Contains(modelo.Status.Value))
                                {
                                    throw new Exception($"Não será possível alterar o agendamento do serviço, pois o mesmo se encontra fechado.");
                                }
                            }


                            if (modelo.Data_Maxima != null)
                            {
                                if (modelo.Status == StatusExecucaoAniel.Aberta_Aguard_Agendamento
                                    || modelo.Status == StatusExecucaoAniel.Aberta_Aguard_Responsavel
                                    || modelo.Status == StatusExecucaoAniel.Aberta_Aguard_Atendimento)
                                    modelo.Status = modelo.Equipe != null ? StatusExecucaoAniel.Aberta_Aguard_Atendimento : StatusExecucaoAniel.Aberta_Aguard_Responsavel;

                                if (modelo.Data_Maxima.Value.Date != data_old || modelo.Hora_Maxima.Value.TimeOfDay != hora_old.Value.TimeOfDay)
                                    modelo.Usuario_Agendamento = User.Identity.GetUserId();
                            }


                            if (modelo.Equipe != grupo)
                            {
                                if (!new List<StatusExecucaoAniel> { StatusExecucaoAniel.Aberta_Aguard_Agendamento,
                                StatusExecucaoAniel.Aberta_Aguard_Atendimento,
                                StatusExecucaoAniel.Atendimento_Paralizado,
                                StatusExecucaoAniel.Interromper_Deslocamento,
                                StatusExecucaoAniel.Aberta_Aguard_Responsavel}.Contains(modelo.Status.Value))
                                {
                                    throw new Exception($"Não foi possível alterar equipe devido ao fato do Documento ter sido alterado para o status {HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(modelo.Status.Value)}.");
                                }
                            }

                            status_old = modelo.Status;

                            if (!String.IsNullOrEmpty(grupo))
                            {

                                if (agendamento == null)
                                {
                                    modelo.Equipe = grupo;
                                }
                                else
                                {
                                    List<AreaModuloModel> areas = new List<AreaModuloModel>();
                                    using (var acessoApi = new AcessoApiController())
                                    {
                                        areas = acessoApi.GetArea_ModuloUsuario(User.Identity.GetUserId()).Result.Objeto;
                                        if (areas.Where(p => p.Cod_Area == "188").Count() == 0)
                                        {
                                            var _temHorarioDisponivel = new Conexao<Agendamento_EquipeAniel>(session).selectFirst(p => p.Equipe.Matricula == grupo &&
                                                                                                                                  ((agendamento.DataHora_Inicio >= p.DataHora_Inicio &&
                                                                                                                                  agendamento.DataHora_Inicio <= p.DataHora_Fim) ||
                                                                                                                                  (agendamento.DataHora_Fim <= p.DataHora_Inicio &&
                                                                                                                                  agendamento.DataHora_Fim >= p.DataHora_Fim))
                                                                                                                                  ).Result == null;
                                            if (_temHorarioDisponivel)
                                            {
                                                modelo.Equipe = grupo;
                                                agendamento.Equipe = new Conexao<EquipeAniel>(session).selectFirst(p => p.Matricula == grupo).Result;
                                            }
                                            else
                                                throw new Exception("Técnico nao possui disponibilidade no horário agendado");
                                        }

                                        else
                                        {
                                            modelo.Equipe = grupo;
                                            agendamento.Equipe = new Conexao<EquipeAniel>(session).selectFirst(p => p.Matricula == grupo).Result;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                modelo.Equipe = null;
                                if (agendamento != null)
                                    agendamento.Equipe = null;
                            }

                            Agendamento_EquipeAniel agendamentoAnterior = repositoryAgendamento.select(p => p.Equipe.Matricula == value.Grupo &&
                                                                                                            p.Id != agendamento.Id &&
                                                                                                            p.DataHora_Inicio.Date == Convert.ToDateTime(DataInicio).Date &&
                                                                                                            p.DataHora_Inicio < Convert.ToDateTime(DataInicio)).Result.OrderByDescending(p => p.DataHora_Inicio).FirstOrDefault();
                            DateTime DataFimAntiga = new DateTime(Convert.ToDateTime(DataFim).Ticks);
                            if (agendamentoAnterior != null)
                            {
                                var roteirizacaoBase = new RoteirizacaoBusiness();
                                Documento_ProducaoAniel ServicoAnterior = await repository.selectFirst(p => p.Num_Obra == agendamentoAnterior.Num_Obra &&
                                                                                                            p.CodCt == agendamentoAnterior.CodCt &&
                                                                                                            p.Projeto == agendamentoAnterior.Projeto &&
                                                                                                            p.Num_Doc == agendamentoAnterior.Num_Doc);

                                var documentoRoteirizar = new DocumentoRoteirizar
                                {
                                    Latitude_Str = modelo.Latitude_Str,
                                    Longitude_Str = modelo.Longitude_Str,
                                };

                                double distancia = Math.Ceiling(roteirizacaoBase.GetDistancias(ServicoAnterior.GetLatitude_Str(), ServicoAnterior.GetLongitude_Str(), new List<DocumentoRoteirizar>() { documentoRoteirizar }).Values.First() / 1852);
                                var tempoPadraoDeslocamento = config.Tempo_Medio_Deslocamento.Value.Minute;
                                TimeSpan tempo_deslocamento = (ServicoAnterior?.Latitude_Str != null && ServicoAnterior?.Longitude_Str != null && modelo.Latitude_Str != null && modelo.Longitude_Str != null) ? roteirizacaoBase.GetTempoDeslocamento(config, distancia, ReferenciaDistancia.UltimoServico) : TimeSpan.FromMinutes(tempoPadraoDeslocamento); /*TimeSpan.Zero*/
                                var agendamentoOld = agendamento.Deslocamento;
                                agendamento.Deslocamento = tempo_deslocamento.TotalMinutes;

                                var diffDeslocamento = agendamento.Deslocamento - agendamentoOld;
                                DataFim = Convert.ToDateTime(DataFim).AddMinutes(diffDeslocamento).ToString("dd/MM/yyyy HH:mm");
                            }




                            using (var documento_api = new Documento_ProducaoApiController())
                            {

                                if (data_old != modelo?.Data_Maxima.Value.Date)
                                {
                                    ValidaAlteracaoDocumento(modelo);
                                    await documento_api.InsertLog_Lanc_Servico(session,
                                                                                modelo.CodCt,
                                                                                modelo.Projeto,
                                                                                modelo.Num_Obra,
                                                                                modelo.Num_Doc,
                                                                                String.Format("Agendamento do serviço {0} {1} (Timeline)", modelo?.Data_Maxima.Value.Date.ToString("dd/MM/yyyy"), hora_old == null ? hora_old?.ToString(@"HH:mm") : String.Empty),
                                                                                String.Concat(data_old?.ToString("dd/MM/yyyy"), " ", hora_old?.ToString(@"HH:mm")),
                                                                                User.Identity.GetUserId(),
                                                                                null,
                                                                                null,
                                                                                modelo.Data_Maxima.HasValue ? modelo?.Data_Maxima.Value.Add(modelo.Hora_Maxima.Value.TimeOfDay).ToString() : null, null);
                                    agendamento.DataHora_Inicio = Convert.ToDateTime(DataInicio);
                                    agendamento.DataHora_Fim = Convert.ToDateTime(DataFim);
                                    agendamento.Origem = OrigemAgendamentoOS.Manual;
                                    agendamento.DataHoraAlteracao = DateTime.Now;
                                }
                                else if (hora_old?.TimeOfDay != modelo?.Hora_Maxima?.TimeOfDay)
                                {
                                    ValidaAlteracaoDocumento(modelo);
                                    await documento_api.InsertLog_Lanc_Servico(session,
                                                                                modelo.CodCt,
                                                                                modelo.Projeto,
                                                                                modelo.Num_Obra,
                                                                                modelo.Num_Doc,
                                                                                String.Format("Agendamento do serviço {0} {1} (Timeline)", modelo?.Data_Maxima.Value.Date.ToString("dd/MM/yyyy"), hora_old == null ? hora_old?.ToString(@"HH:mm") : String.Empty),
                                                                                String.Concat(data_old?.ToString("dd/MM/yyyy"), " ", hora_old?.ToString(@"HH:mm")),
                                                                                User.Identity.GetUserId(),
                                                                                null,
                                                                                null,
                                                                                modelo.Hora_Maxima.HasValue ? modelo?.Hora_Maxima.ToString() : null, null);
                                    agendamento.DataHora_Inicio = Convert.ToDateTime(DataInicio);
                                    agendamento.DataHora_Fim = Convert.ToDateTime(DataFim);
                                    agendamento.Origem = OrigemAgendamentoOS.Manual;
                                    agendamento.DataHoraAlteracao = DateTime.Now;
                                }
                            }


                            if (modelo.Data_Maxima.HasValue && modelo.Hora_Maxima.HasValue)
                            {
                                using (var documentoApi = new Documento_ProducaoApiController())
                                {
                                    var tempoExecucao = await documentoApi.GetTempoExecucao(config, modelo.CodCt, modelo.Projeto, modelo.Num_Obra, modelo.Num_Doc);
                                    if (config.Considerar_Agendamento_Periodo)
                                    {
                                        var dataHoraInicio = (modelo?.Data_Maxima.Value.Add(modelo.Hora_Maxima.Value.TimeOfDay)).Value;
                                        modelo.CodTurnoServ = PeriodoApiController.BuscarTurno(session, modelo.Hora_Maxima.Value, tempoExecucao)?.Id;
                                    }
                                }
                            }

                            if (modelo.Equipe != null && (hora_old.HasValue != modelo?.Hora_Maxima.HasValue || hora_old.Value.TimeOfDay != modelo?.Hora_Maxima.Value.TimeOfDay || data_old.Value.Date != modelo.Data_Maxima.Value.Date) && modelo.Equipe != "FALSE")
                            {
                                var equipe = await new Conexao<EquipeAniel>(session).selectFirst(p => p.Matricula == modelo.Equipe);
                                if (!String.IsNullOrEmpty(equipe.Device_Id))
                                {
                                    MobileService.NotificarSmartphone(equipe.Device_Id, String.Empty, "Houve mudanças em sua rotina.", "Atualize a sua lista de atividades", "br.com.sinapse.touch.MSG_RESINC_OS", modelo.CodCt, modelo.Projeto, modelo.Num_Doc.ToString(), modelo.Num_Obra);
                                }
                            }

                            // Envia mensagem para o mobile com alteração da OS
                            if (!String.IsNullOrEmpty(equipe_old?.Device_Id))
                            {
                                if (equipe_old?.Matricula != modelo.Equipe)
                                    MobileService.NotificarSmartphone(equipe_old.Device_Id, String.Empty, "Houve mudanças em sua rotina.", "Atualize a sua lista de atividades", "br.com.sinapse.touch.MSG_RESINC_OS", modelo.CodCt, modelo.Projeto, modelo.Num_Doc.ToString(), modelo.Num_Obra);

                                if (modelo.Equipe != null)
                                {
                                    var equipeAnterior = await new Conexao<EquipeAniel>(session).selectFirst(p => p.Matricula == modelo.Equipe);
                                    if (!String.IsNullOrEmpty(equipeAnterior.Device_Id))
                                    {
                                        MobileService.NotificarSmartphone(equipeAnterior.Device_Id, String.Empty, "Houve mudanças em sua rotina.", "Atualize a sua lista de atividades", "br.com.sinapse.touch.MSG_RESINC_OS", modelo.CodCt, modelo.Projeto, modelo.Num_Doc.ToString(), modelo.Num_Obra);
                                    }
                                }

                            }
                            else
                            {
                                var equipeAnterior = await new Conexao<EquipeAniel>(session).selectFirst(p => p.Matricula == modelo.Equipe);
                                if (!String.IsNullOrEmpty(equipeAnterior?.Device_Id))
                                {
                                    MobileService.NotificarSmartphone(equipeAnterior.Device_Id, String.Empty, "Houve mudanças em sua rotina.", "Atualize a sua lista de atividades", "br.com.sinapse.touch.MSG_RESINC_OS", modelo.CodCt, modelo.Projeto, modelo.Num_Doc.ToString(), modelo.Num_Obra);
                                }
                            }

                            var matricula = equipe_old != null ? equipe_old.Matricula : null;
                            var nome = equipe_old != null ? equipe_old.Nome : null;


                            if (modelo?.Equipe != equipe_old?.Matricula)
                                await new Documento_ProducaoApiController().InsertLog_Lanc_Servico(session, modelo.CodCt, modelo.Projeto, modelo.Num_Obra, modelo.Num_Doc,
                                        modelo.Equipe == null ? "Remoção de Equipe" : "Equipe", $"{matricula} - {nome}",
                                        User.Identity.GetUserId(), null, null, modelo.Equipe, null);

                            using (ITransaction transaction = session.BeginTransaction())
                            {

                                if (modelo.Equipe != null && modelo.Status == StatusExecucaoAniel.Aberta_Aguard_Responsavel)
                                    modelo.Status = StatusExecucaoAniel.Aberta_Aguard_Atendimento;

                                await repository.SaveOrUpdate(modelo);

                                if (agendamento != null)
                                    await repositoryAgendamento.SaveOrUpdate(agendamento);


                                transaction.Commit();
                            }


                            if (agendamento != null)
                            {
                                string SqlAgendamentoPosterior = repositoryAgendamento.QueryableSelect().Where(p => p.Equipe.Matricula == value.Grupo &&
                                                                                                                p.Id != agendamento.Id &&
                                                                                                                p.DataHora_Inicio.Date == agendamento.DataHora_Inicio.Date &&
                                                                                                                p.DataHora_Inicio > agendamento.DataHora_Inicio).ToSql();

                                Agendamento_EquipeAniel agendamentoPosterior = repositoryAgendamento.QueryableSelect().Where(p => p.Equipe.Matricula == value.Grupo &&
                                                                                                               p.Id != agendamento.Id &&
                                                                                                               p.DataHora_Inicio > agendamento.DataHora_Inicio).ToList().OrderByDescending(p => p.DataHora_Inicio).FirstOrDefault(p => p.DataHora_Inicio.Date == agendamento.DataHora_Inicio.Date);



                                if (agendamentoPosterior != null)
                                {
                                    var roteirizacaoBase = new RoteirizacaoBusiness();
                                    Documento_ProducaoAniel ServicoPosterior = await repository.selectFirst(p => p.Num_Obra == agendamentoPosterior.Num_Obra &&
                                                                                                                p.CodCt == agendamentoPosterior.CodCt &&
                                                                                                                p.Projeto == agendamentoPosterior.Projeto &&
                                                                                                                p.Num_Doc == agendamentoPosterior.Num_Doc);

                                    var documentoRoteirizar = new DocumentoRoteirizar
                                    {
                                        Latitude_Str = modelo.Latitude_Str,
                                        Longitude_Str = modelo.Longitude_Str,
                                    };

                                    double distancia = Math.Ceiling(roteirizacaoBase.GetDistancias(ServicoPosterior.GetLatitude_Str(), ServicoPosterior.GetLongitude_Str(), new List<DocumentoRoteirizar>() { documentoRoteirizar }).Values.First() / 1852);
                                    var tempoPadraoDeslocamento = config.Tempo_Medio_Deslocamento.Value.Minute;
                                    TimeSpan tempo_deslocamento = (ServicoPosterior?.Latitude_Str != null && ServicoPosterior?.Longitude_Str != null && modelo.Latitude_Str != null && modelo.Longitude_Str != null) ? roteirizacaoBase.GetTempoDeslocamento(config, distancia, ReferenciaDistancia.UltimoServico) : TimeSpan.FromMinutes(tempoPadraoDeslocamento); /*TimeSpan.Zero*/
                                    var agendamentoPosteriorDeslocamento = agendamentoPosterior.Deslocamento;

                                    //if (tempo_deslocamento.TotalMinutes > agendamentoPosteriorDeslocamento)
                                    //{
                                    using (var documentoApi = new Painel_RoteirizacaoApiController())
                                    {
                                        documentoApi.HoraRoteirizacao = Convert.ToDateTime(DataFim);
                                        var resultadoEncaixe = await documentoApi.CompactarRotaEncaixe(new CompactarRotaEncaixeModel
                                        {
                                            Equipe = grupo,
                                            DataHoraFimEncaixe = DataInicio,
                                            NumObraEncaixe = modelo.Num_Obra,
                                            DataHoraFimAntigoAgendamento = DataInicio,
                                            SalvarRota = true
                                        });
                                    }
                                    //}
                                }
                            }

                        }
                    }
                    retorno.IdObjeto = "";
                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = "";
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        private void ValidaAlteracaoDocumento(Documento_ProducaoAniel modelo)
        {
            if (new List<StatusExecucaoAniel> { StatusExecucaoAniel.Fechada_Produtiva,
                                                                        StatusExecucaoAniel.Fechada_Improdutiva,
                                                                        StatusExecucaoAniel.Atendimento_Finalizado}.Contains(modelo.Status.Value))
            {
                throw new Exception($"Não será possível gravar dados do serviço, pois o mesmo se encontra fechado.");
            }

            if (new List<StatusExecucaoAniel> { StatusExecucaoAniel.Cancelado }.Contains(modelo.Status.Value))
            {
                throw new Exception($"Não será possível gravar dados do serviço, pois o mesmo se encontra cancelado.");
            }

            if (modelo.Responsabilidade == ResponsabilidadeOS.TECNICO)
            {
                throw new Exception($"Não será possível gravar dados do serviço, pois o mesmo se esta na responsabilidade do técnico.");
            }
        }

        [HttpPost()]
        [Route("api/Gestao_Equipeapi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar(String DataInicio, String DataFim, String grupo, String Num_OS)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                Documento_ProducaoAniel modelo = null;

                using (ISession sessionConnect = HibernateUtil.GetSession())
                using (ISession session = HibernateUtil.GetSessionT5())
                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                using (var repoConfiguracoes_Gerais = new Conexao<Configuracoes_Gerais>(sessionConnect))
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    using (var api_Roteirizacao = new Painel_RoteirizacaoApiController())
                    using (var api = new Documento_ProducaoApiController())
                    using (var repositoryAgendamento = new Conexao<AgendamentoCadastroModel>(session))
                    {
                        AgendamentoCadastroModel value = await repositoryAgendamento.selectFirst(p => p.Num_Doc == Convert.ToInt32(Num_OS));

                        Configuracoes_Gerais config = await repoConfiguracoes_Gerais.selectFirst();

                        using (var repository = new Conexao<Documento_ProducaoAniel>(session))
                        {
                            //Busca informações de acordo com o Num_OS
                            modelo = await repository.selectFirst(p => p.Num_Doc == Convert.ToInt32(Num_OS));

                            var status_old = modelo.Status;

                            if (modelo.Status == StatusExecucaoAniel.Fechada_Produtiva ||
                                modelo.Status == StatusExecucaoAniel.Fechada_Improdutiva ||
                                modelo.Status == StatusExecucaoAniel.Atendimento_Finalizado)
                            {
                                throw new Exception("Não foi possível alterar o agendamento do serviço, pois o mesmo se encontra fechado.");
                            }

                            if (modelo == null)
                            {
                                throw new Exception("Documento Produção não encontrado");
                            }


                            await api.InsertLog_Lanc_Servico(session, modelo.CodCt, modelo.Projeto, modelo.Num_Obra, modelo.Num_Doc,
                                                             String.Format("Agendamento do serviço {0} {1}", value.Data, value.Hora),
                                                             modelo.Data_Maxima.HasValue ? modelo.Data_Maxima.Value.ToString("dd/MM/yyyy") : "",
                                                             modelo.Usuario, Convert.ToDateTime(value.Data), null, value.Data + value.Hora, null, transaction);

                            modelo.Status = StatusExecucaoAniel.Aberta_Aguard_Responsavel;
                            modelo.Data_Maxima = Convert.ToDateTime(value.Data);
                            modelo.Hora_Maxima = Convert.ToDateTime(value.Hora);
                            modelo.Usuario_Agendamento = modelo.Usuario;
                            modelo.DiaHora_Agendamento = DateTime.Now;
                            modelo.Equipe = grupo;
                            modelo.Responsabilidade = 0;
                            modelo.Roterizar = "N";

                            if (config.Considerar_Agendamento_Periodo)
                            {
                                modelo.CodTurnoServ = PeriodoApiController.BuscarTurno(session, (modelo.Data_Maxima.Value.Date + modelo.Hora_Maxima.Value.TimeOfDay), Convert.ToDateTime(value.Hora_Final).TimeOfDay - Convert.ToDateTime(value.Hora_Inicio).TimeOfDay)?.Id;
                            }

                            await repository.SaveOrUpdate(modelo);
                            if (status_old != modelo.Status)
                            {
                                await new Documento_ProducaoApiController().InsertLog_Lanc_Servico(session,
                                                                            modelo.CodCt,
                                                                            modelo.Projeto,
                                                                            modelo.Num_Obra,
                                                                            modelo.Num_Doc,
                                                                            "Mudança de Status",
                                                                            HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(status_old.Value),
                                                                            User.Identity.GetUserId(),
                                                                            null,
                                                                            null,
                                                                            HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(modelo.Status.Value),
                                                                            null,
                                                                            transaction
                                                                            );
                            }
                        }

                        using (var repository = new Conexao<Agendamento_EquipeAniel>(session))
                        {
                            List<Agendamento_EquipeAniel> agendamentos = (await repository.select(p => p.CodCt == value.CodCt && p.Projeto == value.Projeto &&
                                                                                                    p.Num_Obra == value.Num_Obra && p.Num_Doc == value.Num_Doc)).ToList();
                            // p.DataHora_Inicio.Date >= Convert.ToDateTime(value.Objeto.Data)


                            await repository.Remove(agendamentos);

                            using (var repoTipo_Servico_EquipeAniel = new Conexao<Tipo_Servico_EquipeAniel>(session))
                            using (var repoTipo_ImovelAniel = new Conexao<Tipo_ImovelAniel>(session))
                            using (var repoTipoServico = new Conexao<Tipo_Servico_EquipeAniel>(session))
                            {
                                var agendamento = new Agendamento_EquipeAniel
                                {
                                    CodCt = modelo.CodCt,
                                    Projeto = modelo.Projeto,
                                    Num_Obra = modelo.Num_Obra,
                                    Num_Doc = modelo.Num_Doc,
                                    Cod_Escritorio = modelo.Cod_Escritorio,
                                    Cod_Area = modelo.Cod_Area,
                                    Cod_Localidade = modelo.Cod_Localidade,
                                    DataHora_Inicio = Convert.ToDateTime(String.Concat(value.Data, ' ', value.Hora_Inicio)),
                                    DataHora_Fim = Convert.ToDateTime(String.Concat(value.Data, ' ', value.Hora_Final)),
                                    Deslocamento = value.Deslocamento,
                                    Equipe = !String.IsNullOrEmpty(modelo.Equipe) ? await new Conexao<EquipeAniel>(session).selectFirst(p => p.Matricula == grupo) : null,
                                    Equipe_Tmp = value.Equipe_Tmp,
                                    Log = GerarLog(value.Lista_Forca_Trabalho, value.Lista_Ja_Agendados, config),
                                    Tipo_Servico = modelo.Cod_Tipo_Serv.HasValue ? await repoTipo_Servico_EquipeAniel.selectFirst(p => p.Cod_Tipo_Serv == modelo.Cod_Tipo_Serv) : null,
                                    Tipo_Imovel = modelo.Cod_Tipo_Imovel.HasValue ? await repoTipo_ImovelAniel.selectFirst(p => p.Cod_Tipo_Imovel == modelo.Cod_Tipo_Imovel) : null,
                                    UsuarioCriacao = modelo.Usuario,
                                    Origem = OrigemAgendamentoOS.Manual,
                                    Latitude = modelo.GetLatitude_Str(),
                                    Longitude = modelo.GetLongitude_Str(),

                                    //Status_Cobranca=modelo.Status_Cobranca
                                };

                                await repository.Save(agendamento);

                                if (agendamento.Equipe != null)
                                {
                                    if (!String.IsNullOrEmpty(agendamento.Equipe.Device_Id))
                                    {
                                        MobileService.NotificarSmartphone(agendamento.Equipe.Device_Id, String.Empty, "Houve mudanças em sua rotina.", "Atualize a sua lista de atividades", "br.com.sinapse.touch.MSG_RESINC_OS", modelo.CodCt, modelo.Projeto, modelo.Num_Doc.ToString(), modelo.Num_Obra);
                                    }
                                }

                            }
                        }



                        transaction.Commit();

                        if (modelo.Data_Maxima.HasValue && value.Roteirizar)
                        {
                            var resultado_roteirizacao = await api_Roteirizacao.Roteirizar(new ParamWebApi<RoteirizarServicosModel>
                            {
                                Objeto = new RoteirizarServicosModel
                                {
                                    ids = new List<string> { String.Join("|", new List<String> {
                                            modelo.CodCt, modelo.Projeto,
                                            modelo.Num_Obra,
                                            modelo.Num_Doc.ToString(),
                                        })
                                    },
                                    data = modelo.Data_Maxima.Value.ToString("dd/MM/yyyy"),
                                    numero_max_atendimento = 1000
                                },
                                Usuario = modelo.Usuario,
                                //Token = modelo.Token
                            });

                            if (resultado_roteirizacao.Ok == false)
                            {
                                throw new Exception($"Não foi possível roteirizar essa Ordem de Serviço.Verifique o log de roteirização para obter detalhes.");
                            }

                        }

                        if (transaction.WasCommitted == false)
                            transaction.Commit();


                    }
                }

                retorno.IdObjeto = 1;
                retorno.Ok = true;
            }

            catch (Exception ex)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }


            return retorno;
        }

        private String GerarLog(List<String> Lista_Forca_Trabalho, List<String> Lista_Ja_Agendados, Configuracoes_Gerais config)
        {
            return $"Força de Trabalho: {String.Join(",", Lista_Forca_Trabalho)}\nJa Agendados:{String.Join(",", Lista_Ja_Agendados)}\n" +
                   $"Exige Agendamento Para Criação dos Serviços ? {config.Exige_Agendamento_Servico}\n" +
                   $"Empresa Utilizará Roteirização de Serviços ? {config.Empresa_Roteirizacao_Servico}\n" +
                   $"Criterio Habilidade:{config.Criterio_Habilidade}\nCriterio Area: {config.Criterio_Area}\nCriterio SLA:{config.Criterio_SLA_Execucao}";
        }

        //public OsReprocessadas ReprocessarPedido()
        //{
        //    //using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
        //    //{
        //    //    while (await reader.ReadAsync())
        //    //    {
        //    //        retorno.Add(new MotivoAusencia_ListModel
        //    //        {

        //    //            Id = Convert.ToInt64(reader["ID"]),
        //    //            Motivo = Convert.ToString(reader["MOTIVO"]),
        //    //            Status = HtmlEnumExtensions.GetEnumDescription<Status_Default>(Convert.ToString(reader["STATUS"])),
        //    //        });
        //    //    }
        //    //}
        //    var retorno = new OsReprocessadas();

        //    return retorno;


        //}

        private List<Timeline_ServicosEquipeServicosModels> PreencheTipoOSNenhum(DateTime Data, List<Timeline_ServicosEquipeServicosModels> servicos)
        {
            List<Timeline_ServicosEquipeServicosModels> retorno = new List<Timeline_ServicosEquipeServicosModels>();

            if (servicos.Count() == 0) return retorno;

            var servicosTmp = servicos.OrderBy(p => p.DataHoraIni.TimeOfDay);

            var terminoAnterior = Convert.ToDateTime(Data.ToString("dd/MM/yyyy 00:00"));

            foreach (var documento in servicos)
            {
                var diferenca = (documento.DataHoraIni.TimeOfDay - terminoAnterior.TimeOfDay).TotalMinutes;

                if (diferenca != 0)
                {
                    retorno.Add(new Timeline_ServicosEquipeServicosModels
                    {
                        TipoOS = Timeline_ServicosEquipeServicosEnum.Nenhum,
                        Pixels = $"{diferenca}px"
                    });
                }
                retorno.Add(documento);

                if (documento.DataHoraFim.Date > documento.DataHoraIni.Date)
                {
                    terminoAnterior = documento.DataHoraFim.Date;
                }
                else
                {
                    terminoAnterior = documento.DataHoraFim;
                }
            }

            return retorno;
        }

        private async Task<List<DateTime>> GetListaDatas(DateTime? DataInicio, DateTime? DataFim)
        {
            List<DateTime> datas = new List<DateTime>();

            DateTime data = DataInicio.Value;
            if (DataInicio == DataFim)
            {
                datas.Add(DataInicio.Value);
            }
            else
            {
                while (data <= DataFim.Value)
                {
                    datas.Add(data);
                    data = data.AddDays(1);
                }
            }

            return datas;
        }

        private async Task<List<Timeline_ServicosEquipeServicosModels>> GetTempoNaoUsadoAgendamento(DateTime data)
        {

            using (ISession session = HibernateUtil.GetSessionT5())
            using (ISession session_connect = HibernateUtil.GetSession())
            using (var repository = new Conexao<Horario_Connect>(session))
            using (var feriado_repository = new Conexao<Feriado>(session_connect))
            using (HorarioApiController horarioApi = new HorarioApiController())
            {
                List<Timeline_ServicosEquipeServicosModels> retorno = new List<Timeline_ServicosEquipeServicosModels>();
                var horario_modelo = await repository.selectFirst(p => p.Padrao && p.Tipo == Tipo_Hora_Connect.Horario_Agendamento);

                if (horario_modelo == null)
                {
                    throw new Exception(RsCadastros.EX_HORARIO_AGENDAMENTO_NAO_DEFINIDO);
                }

                bool feriado = feriado_repository.selectFirst(p => p.Data == data.Date).Result != null;
                var horario = horarioApi.RetornaHorarioDia(horario_modelo, data, feriado);

                if (horario_modelo != null)
                {
                    DateTime InicioDia = data.Date;

                    retorno.Add(new Timeline_ServicosEquipeServicosModels
                    {
                        DataHoraIni = InicioDia,
                        DataHoraFim = horario.inicio.Value,
                        TipoOS = Timeline_ServicosEquipeServicosEnum.Fora_Agendamento,
                        Pixels = Convert.ToString((horario.inicio.Value.TimeOfDay - InicioDia.TimeOfDay).TotalMinutes) + "px"
                    });

                    if (horario.inicio_intervalo.HasValue && horario.termino_intervalo.HasValue)
                    {

                        retorno.Add(new Timeline_ServicosEquipeServicosModels
                        {
                            DataHoraIni = horario.inicio_intervalo.Value,
                            DataHoraFim = horario.termino_intervalo.Value,
                            TipoOS = Timeline_ServicosEquipeServicosEnum.Fora_Agendamento,
                            Pixels = Convert.ToString((horario.termino_intervalo.Value.TimeOfDay - horario.inicio_intervalo.Value.TimeOfDay).TotalMinutes) + "px"
                        });
                    }

                    retorno.Add(new Timeline_ServicosEquipeServicosModels
                    {
                        DataHoraIni = horario.termino.Value,
                        DataHoraFim = Convert.ToDateTime(InicioDia.ToString("dd/MM/yyyy 23:59")),
                        TipoOS = Timeline_ServicosEquipeServicosEnum.Fora_Agendamento,
                        Pixels = Convert.ToString((Convert.ToDateTime(InicioDia.ToString("dd/MM/yyyy 23:59")).TimeOfDay - horario.termino.Value.TimeOfDay).TotalMinutes) + "px"
                    });

                }

                return retorno;
            }

        }

        private async Task<List<Timeline_ServicosEquipeServicosModels>> GetTempoNaoUsadoAgendamento(DateTime data, Horario_Connect horario_modelo, bool feriado, Horario_Equipe horarioEquipeTecnico)
        {
            List<Timeline_ServicosEquipeServicosModels> retorno = new List<Timeline_ServicosEquipeServicosModels>();
            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (ISession session_connect = HibernateUtil.GetSession())
                using (var repository = new Conexao<Horario_Connect>(session))
                using (var repositoryHorarioEquipe = new Conexao<Horario_Equipe>(session))
                using (var feriado_repository = new Conexao<Feriado>(session_connect))
                using (HorarioApiController horarioApi = new HorarioApiController())
                {
                    var horarioTecnico = horarioEquipeTecnico?.Horario;
                    var horario = horarioTecnico != null ? horarioApi.RetornaHorarioDia(horarioTecnico, data, feriado) : horarioApi.RetornaHorarioDia(horario_modelo, data, feriado);

                    if (horarioTecnico == null && horario.inicio == null)
                    {
                        horario_modelo = await repository.selectFirst(p => p.Padrao && p.Tipo == Tipo_Hora_Connect.Horario_Agendamento);
                        horario = horarioApi.RetornaHorarioDia(horario_modelo, data, feriado);
                    }

                    DateTime InicioDia = data.Date;

                    if (horario_modelo != null)
                    {

                        if (horario.inicio.HasValue)
                        {
                            retorno.Add(new Timeline_ServicosEquipeServicosModels
                            {
                                DataHoraIni = InicioDia,
                                DataHoraFim = horario.inicio.Value,
                                TipoOS = Timeline_ServicosEquipeServicosEnum.Fora_Agendamento,
                                Pixels = Convert.ToString((horario.inicio.Value.TimeOfDay - InicioDia.TimeOfDay).TotalMinutes) + "px"
                            });
                        }


                        if (horario.inicio_intervalo.HasValue && horario.termino_intervalo.HasValue)
                        {
                            retorno.Add(new Timeline_ServicosEquipeServicosModels
                            {
                                DataHoraIni = horario.inicio_intervalo.Value,
                                DataHoraFim = horario.termino_intervalo.Value,
                                TipoOS = Timeline_ServicosEquipeServicosEnum.Fora_Agendamento,
                                Pixels = Convert.ToString((horario.termino_intervalo.Value.TimeOfDay - horario.inicio_intervalo.Value.TimeOfDay).TotalMinutes) + "px"
                            });
                        }

                        if (horario.termino.HasValue)
                        {
                            retorno.Add(new Timeline_ServicosEquipeServicosModels
                            {
                                DataHoraIni = horario.termino.Value,
                                DataHoraFim = Convert.ToDateTime(InicioDia.ToString("dd/MM/yyyy 23:59:59")),
                                TipoOS = Timeline_ServicosEquipeServicosEnum.Fora_Agendamento,
                                Pixels = Convert.ToString((Convert.ToDateTime(InicioDia.ToString("dd/MM/yyyy 23:59:59")).TimeOfDay - horario.termino.Value.TimeOfDay).TotalMinutes) + "px"
                            });
                        }
                    }
                    else
                    {
                        retorno.Add(new Timeline_ServicosEquipeServicosModels
                        {
                            DataHoraIni = horario.termino.Value,
                            DataHoraFim = Convert.ToDateTime(InicioDia.ToString("dd/MM/yyyy 23:59:59")),
                            TipoOS = Timeline_ServicosEquipeServicosEnum.Fora_Agendamento,
                            Pixels = Convert.ToString((Convert.ToDateTime(InicioDia.ToString("dd/MM/yyyy 23:59:59")).TimeOfDay - horario.termino.Value.TimeOfDay).TotalMinutes) + "px"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }
            return retorno;
        }


        private async Task<List<Timeline_ServicosEquipeServicosModels>> GetTempoHoraExtra(DateTime data, Horario_Connect horario_modelo, bool feriado, Horario_Equipe horarioEquipeTecnico)
        {

            using (ISession session = HibernateUtil.GetSessionT5())
            using (ISession session_connect = HibernateUtil.GetSession())
            using (var repository = new Conexao<Horario_Connect>(session))
            using (var repositoryHorarioEquipe = new Conexao<Horario_Equipe>(session))
            using (var feriado_repository = new Conexao<Feriado>(session_connect))
            using (HorarioApiController horarioApi = new HorarioApiController())
            {
                List<Timeline_ServicosEquipeServicosModels> retorno = new List<Timeline_ServicosEquipeServicosModels>();
                var horarioTecnico = horarioEquipeTecnico?.Horario;
                var horario = horarioTecnico != null ? horarioApi.RetornaHorarioDia(horarioTecnico, data, feriado) : horarioApi.RetornaHorarioDia(horario_modelo, data, feriado);

                if (horario_modelo != null)
                {
                    var horario_hora_extra = horarioTecnico != null ? horarioApi.RetornaHoraExtraDia(horarioTecnico, data, feriado) : horarioApi.RetornaHoraExtraDia(horario_modelo, data, feriado);

                    if (horario_hora_extra != null)
                    {
                        var hora_inicio_extra = horario.termino.Value.AddMinutes(1);
                        var hora_fim_extra = (hora_inicio_extra).AddHours(horario_hora_extra.Value.Hour);


                        retorno.Add(new Timeline_ServicosEquipeServicosModels
                        {
                            DataHoraIni = hora_inicio_extra,
                            DataHoraFim = hora_fim_extra,
                            TipoOS = Timeline_ServicosEquipeServicosEnum.Hora_Extra,
                            Pixels = Convert.ToString((hora_inicio_extra.TimeOfDay - hora_fim_extra.TimeOfDay).TotalMinutes) + "px"
                        });

                    }
                }

                return retorno;
            }

        }


        public async Task<List<Classe_ListModel>> GetClasseServicos(Timeline_ServicosEquipeServicosModels item, int? TipoVisualizacao, int? Prazo, String DataHoraParametro, Tineline_Documentos documento)
        {

            var classe = "";
            List<Classe_ListModel> retorno = new List<Classe_ListModel>();


            //PRAZO POR SLA
            DateTime? data_prod = documento.DATAHORA_PROD;
            DateTime? data_indicador = documento.DATAHORA_INDICADOR;


            if (data_indicador == null)
            {
                retorno.Add(new Classe_ListModel
                {
                    Classe = "bg-sem-sla-definido",
                    Num_OS = item.Num_OS,
                    TipoClasse = "SLA"
                });
            }
            else
            {
                DateTime dataComparar = DateTime.Now;
                if (!String.IsNullOrEmpty(DataHoraParametro))
                {
                    DateTime DateObject = Convert.ToDateTime(DataHoraParametro);
                    dataComparar = DateObject;

                    if (data_prod >= dataComparar)
                    {
                        data_prod = dataComparar;
                    }

                }


                classe = new Documento_ProducaoApiController().GetDescricaoHorasSla(data_indicador, documento.DATAHORA_CRIACAO, data_prod, dataComparar);

                switch (classe)
                {
                    case "Sem indicador de SLA":
                        retorno.Add(new Classe_ListModel
                        {
                            Classe = "bg-sem-sla-definido",
                            Num_OS = item.Num_OS,
                            TipoClasse = "SLA"
                        });
                        break;

                    case "Acima de 4 horas para vencimento do SLA":
                        retorno.Add(new Classe_ListModel
                        {
                            Classe = "bg-dentro-prazo-sla",
                            Num_OS = item.Num_OS,
                            TipoClasse = "SLA"
                        });
                        break;

                    case "SLA vencerá em menos de 4 horas":
                        retorno.Add(new Classe_ListModel
                        {
                            Classe = "bg-atencao-sla",
                            Num_OS = item.Num_OS,
                            TipoClasse = "SLA"
                        });
                        break;

                    case "Serviço fechado dentro do prazo":
                        retorno.Add(new Classe_ListModel
                        {
                            Classe = "bg-prazo-cumprido-sla",
                            Num_OS = item.Num_OS,
                            TipoClasse = "SLA"
                        });
                        break;
                    case "Serviço fechado fora do prazo":
                        retorno.Add(new Classe_ListModel
                        {
                            Classe = "bg-fech-prazo-venc-sla",
                            Num_OS = item.Num_OS,
                            TipoClasse = "SLA"
                        });
                        break;

                    case "SLA vencerá em menos de 2 horas":
                        retorno.Add(new Classe_ListModel
                        {
                            Classe = "bg-prox-vencimento-sla",
                            Num_OS = item.Num_OS,
                            TipoClasse = "SLA"
                        });
                        break;

                    case "SLA vencerá em menos de 1 hora":
                        retorno.Add(new Classe_ListModel
                        {
                            Classe = "bg-risco-perda-sla",
                            Num_OS = item.Num_OS,
                            TipoClasse = "SLA"
                        });
                        break;

                    case "SLA vencido":
                        retorno.Add(new Classe_ListModel
                        {
                            Classe = "bg-prazo-venc-sla",
                            Num_OS = item.Num_OS,
                            TipoClasse = "SLA"
                        });
                        break;
                    case "Prazo Cumprido":
                        retorno.Add(new Classe_ListModel
                        {
                            Classe = "bg-prazo-cumprido-sla",
                            Num_OS = item.Num_OS,
                            TipoClasse = "SLA"
                        });
                        break;


                }


            }

            //JANELA DE ATENDIMENTO
            DateTime horarioAtual = DateTime.Now;

            if (!String.IsNullOrEmpty(DataHoraParametro))
            {
                DateTime DateObject = Convert.ToDateTime(DataHoraParametro);
                horarioAtual = DateObject;
            }


            //Dentro do Prazo
            if (horarioAtual <= Convert.ToDateTime(item.DataHoraIniAgendamento))
            {
                retorno.Add(new Classe_ListModel
                {
                    Classe = "bg-serv-den-pra-atendimento",
                    Num_OS = item.Num_OS,
                    TipoClasse = "JANELA"
                });
            }
            else if (horarioAtual > documento.DATAHORA_DESLOCAMENTO && documento.STATUS_EXECUCAO == StatusExecucaoAniel.Atendimento_Iniciado_Reiniciado)
            {
                retorno.Add(new Classe_ListModel
                {
                    Classe = "bg-serv-den-pra-atendimento",
                    Num_OS = item.Num_OS,
                    TipoClasse = "JANELA"
                });
            }
            else if (horarioAtual <= Convert.ToDateTime(item.DataHoraFimAgendamento) && documento.STATUS_EXECUCAO == StatusExecucaoAniel.Atendimento_Iniciado_Reiniciado)
            {
                retorno.Add(new Classe_ListModel
                {
                    Classe = "bg-serv-den-pra-atendimento",
                    Num_OS = item.Num_OS,
                    TipoClasse = "JANELA"
                });
            }
            else if (horarioAtual <= Convert.ToDateTime(item.DataHoraFimAgendamento) && documento.STATUS_EXECUCAO == StatusExecucaoAniel.Atendimento_Paralizado)
            {
                retorno.Add(new Classe_ListModel
                {
                    Classe = "bg-serv-den-pra-atendimento",
                    Num_OS = item.Num_OS,
                    TipoClasse = "JANELA"
                });
            }
            else if ((documento.STATUS_EXECUCAO == StatusExecucaoAniel.Fechada_Produtiva && (item.DATAHORA_PROD <= Convert.ToDateTime(item.DataHoraFimAgendamento))) ||
                (documento.STATUS_EXECUCAO == StatusExecucaoAniel.Fechada_Improdutiva && (item.DATAHORA_PROD <= Convert.ToDateTime(item.DataHoraFimAgendamento))))
            {
                retorno.Add(new Classe_ListModel
                {
                    Classe = "bg-serv-den-pra-atendimento",
                    Num_OS = item.Num_OS,
                    TipoClasse = "JANELA"
                });
            }



            //Atrasado
            else if ((horarioAtual > Convert.ToDateTime(item.DataHoraIniAgendamento) && documento.STATUS_EXECUCAO == StatusExecucaoAniel.Aberta_Aguard_Atendimento))
            {
                retorno.Add(new Classe_ListModel
                {
                    Classe = "bg-serv-atrasado-atendimento",
                    Num_OS = item.Num_OS,
                    TipoClasse = "JANELA"
                });
            }
            else if ((documento.DATAHORA_DESLOCAMENTO > Convert.ToDateTime(item.DataHoraIniAgendamento) && documento.STATUS_EXECUCAO == StatusExecucaoAniel.OS_Em_deslocamento))
            {
                retorno.Add(new Classe_ListModel
                {
                    Classe = "bg-serv-atrasado-atendimento",
                    Num_OS = item.Num_OS,
                    TipoClasse = "JANELA"
                });
            }
            else if ((horarioAtual > Convert.ToDateTime(item.DataHoraIniAgendamento) && documento.STATUS_EXECUCAO == StatusExecucaoAniel.OS_Em_deslocamento))
            {
                retorno.Add(new Classe_ListModel
                {
                    Classe = "bg-serv-atrasado-atendimento",
                    Num_OS = item.Num_OS,
                    TipoClasse = "JANELA"
                });
            }
            else if (documento.STATUS_EXECUCAO == StatusExecucaoAniel.Atendimento_Iniciado_Reiniciado && horarioAtual >= Convert.ToDateTime(item.DataHoraIni))
            {
                retorno.Add(new Classe_ListModel
                {
                    Classe = "bg-serv-atrasado-atendimento",
                    Num_OS = item.Num_OS,
                    TipoClasse = "JANELA"
                });
            }
            else if (documento.STATUS_EXECUCAO == StatusExecucaoAniel.Atendimento_Paralizado && horarioAtual >= Convert.ToDateTime(item.DataHoraIni))
            {
                retorno.Add(new Classe_ListModel
                {
                    Classe = "bg-serv-atrasado-atendimento",
                    Num_OS = item.Num_OS,
                    TipoClasse = "JANELA"
                });
            }
            else if (documento.STATUS_EXECUCAO == StatusExecucaoAniel.Atendimento_Iniciado_Reiniciado && horarioAtual >= Convert.ToDateTime(item.DataHoraFimAgendamento))
            {
                retorno.Add(new Classe_ListModel
                {
                    Classe = "bg-serv-atrasado-atendimento",
                    Num_OS = item.Num_OS,
                    TipoClasse = "JANELA"
                });
            }
            else if (documento.STATUS_EXECUCAO == StatusExecucaoAniel.Atendimento_Paralizado && horarioAtual >= Convert.ToDateTime(item.DataHoraFimAgendamento))
            {
                retorno.Add(new Classe_ListModel
                {
                    Classe = "bg-serv-atrasado-atendimento",
                    Num_OS = item.Num_OS,
                    TipoClasse = "JANELA"
                });
            }

            //Fora do Prazo
            else if ((documento.STATUS_EXECUCAO == StatusExecucaoAniel.Fechada_Produtiva && (item.DATAHORA_PROD > Convert.ToDateTime(item.DataHoraFimAgendamento))) ||
                (documento.STATUS_EXECUCAO == StatusExecucaoAniel.Fechada_Improdutiva && (item.DATAHORA_PROD > Convert.ToDateTime(item.DataHoraFimAgendamento))))
            {
                retorno.Add(new Classe_ListModel
                {
                    Classe = "bg-serv-fora-prazo-atendimento",
                    Num_OS = item.Num_OS,
                    TipoClasse = "JANELA"
                });
            }

            //STATUS
            switch (item.TipoOS)
            {
                case Timeline_ServicosEquipeServicosEnum.Serviço_Previsto:
                    retorno.Add(new Classe_ListModel
                    {
                        Classe = "bg-serv-prev",
                        Num_OS = item.Num_OS,
                        TipoClasse = "STATUS"
                    });
                    break;

                case Timeline_ServicosEquipeServicosEnum.Tecnico_em_Deslocamento:
                    retorno.Add(new Classe_ListModel
                    {
                        Classe = "bg-tec-desloc",
                        Num_OS = item.Num_OS,
                        TipoClasse = "STATUS"
                    });
                    break;

                case Timeline_ServicosEquipeServicosEnum.Serviço_Iniciado:
                    retorno.Add(new Classe_ListModel
                    {
                        Classe = "bg-serv-inic",
                        Num_OS = item.Num_OS,
                        TipoClasse = "STATUS"
                    });
                    break;

                case Timeline_ServicosEquipeServicosEnum.Serviço_Paralisado:
                    retorno.Add(new Classe_ListModel
                    {
                        Classe = "bg-serv-paralisado",
                        Num_OS = item.Num_OS,
                        TipoClasse = "STATUS"
                    });
                    break;

                case Timeline_ServicosEquipeServicosEnum.Serviço_Baixado_no_Prazo_Produtivo:
                    retorno.Add(new Classe_ListModel
                    {
                        Classe = "bg-serv-baixo-prod",
                        Num_OS = item.Num_OS,
                        TipoClasse = "STATUS"
                    });
                    break;

                case Timeline_ServicosEquipeServicosEnum.Serviço_Baixado_no_Prazo_Improdutivo:
                    retorno.Add(new Classe_ListModel
                    {
                        Classe = "bg-serv-baixa-improd",
                        Num_OS = item.Num_OS,
                        TipoClasse = "STATUS"
                    });
                    break;

                case Timeline_ServicosEquipeServicosEnum.Servico_Com_Atendimento_Finalizado:
                    retorno.Add(new Classe_ListModel
                    {
                        Classe = "bg-serv-atend_finalizado",
                        Num_OS = item.Num_OS,
                        TipoClasse = "STATUS"
                    });
                    break;

                case Timeline_ServicosEquipeServicosEnum.Serviço_Atrasado:
                    retorno.Add(new Classe_ListModel
                    {
                        Classe = "bg-serv-prev",
                        Num_OS = item.Num_OS,
                        TipoClasse = "STATUS"
                    });
                    break;

                case Timeline_ServicosEquipeServicosEnum.Serviço_Fora_do_Prazo_Produtivo:
                    retorno.Add(new Classe_ListModel
                    {
                        Classe = "bg-serv-baixo-prod",
                        Num_OS = item.Num_OS,
                        TipoClasse = "STATUS"
                    });
                    break;

                case Timeline_ServicosEquipeServicosEnum.Serviço_Fora_do_Prazo_Improdutivo:
                    retorno.Add(new Classe_ListModel
                    {
                        Classe = "bg-serv-baixa-improd",
                        Num_OS = item.Num_OS,
                        TipoClasse = "STATUS"
                    });
                    break;
            }


            //TIPO SERVICO
            retorno.Add(new Classe_ListModel
            {
                Classe = GetCorTipoServico(item.Cod_Tipo_Servico),
                Num_OS = item.Num_OS,
                TipoClasse = "TIPOSERV"
            });


            return retorno;
        }

        public String GetCorTipoServico(string CodtipoServ)
        {
            using (ISession session = HibernateUtil.GetSessionT5())
            using (var repo = new Conexao<LegendaTimelineTipoServicos>(session))
            {
                var cor = !String.IsNullOrEmpty(CodtipoServ) ? repo.selectFirst(p => p.Tipo_Servico_Equipe.Cod_Tipo_Serv == Convert.ToInt64(CodtipoServ)).Result : null;

                if (cor == null)
                {
                    return "#A9A9A9";
                }
                else
                {
                    return cor.Cor;
                }
            }

        }

        [HttpGet]
        [Route("api/Gestao_Equipeapi/GetLegendaTipoServico")]
        public async Task<List<object>> GetLegendaTipoServico()
        {
            var retorno = new List<object>();
            using (ISession session = HibernateUtil.GetSessionT5())
            using (var repo = new Conexao<LegendaTimelineTipoServicos>(session))
            {
                var legendas = await repo.select();

                foreach (var leg in legendas)
                {
                    var lista = new List<object>{ new{
                        cor = leg.Cor,
                        descricao = leg.Tipo_Servico_Equipe?.Descricao
                    }
                    }.ToList();

                    retorno.Add(lista);
                }

                return retorno;
            }
        }



        [HttpGet()]
        [Route("api/Gestao_Equipeapi/GetServicos")]
        public async Task<GetApiRetorno<List<Timeline_ServicosEquipeServicosModels>>> GetServicos(String Data, String Equipe, String Monitor, String Supervisor, bool TodosTecnicos, String DataHoraParametro, Int32? Prazo, Int32? TipoVisualizacao, String GrupoArea, String Area, String Localidade, String TipoServico, String SubTipoServico, String NaturezaServico, Tipo_Prioridade? TipoPrioridade, String DataCriacaoInicio, String DataCriacaoFim, string contrato, string projeto)
        {
            List<Timeline_ServicosEquipeServicosModels> retornoTmp = new List<Timeline_ServicosEquipeServicosModels>();
            DateTime? data = String.IsNullOrEmpty(Data) ? (DateTime?)null : Convert.ToDateTime(Data);
            GetApiRetorno<List<Timeline_ServicosEquipeServicosModels>> retorno = new GetApiRetorno<List<Timeline_ServicosEquipeServicosModels>>();
            retorno.Objeto = new List<Timeline_ServicosEquipeServicosModels>();
            retorno.Ok = true;

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (ISession session_connect = HibernateUtil.GetSession())
                using (var repositoryTipoC = new Conexao<Categoria_Cliente>(session_connect))
                using (var repoconfig = new Conexao<Configuracoes_Gerais>(session_connect))
                {
                    var documentos = (await GetDocumentos(session, session_connect, data, Equipe, Monitor, Supervisor, TodosTecnicos, GrupoArea, Area, Localidade, TipoServico, SubTipoServico, NaturezaServico, TipoPrioridade, DataCriacaoInicio, DataCriacaoFim, contrato, projeto)).OrderBy(p => p.DATAHORA_INICIO);
                    
                    List<long?> lista_ids = documentos.Select(p => p.ID_CATEGORIA_CLIENTE)
                                                     .Where(p => p != null).Distinct()
                                                     .ToList();

                    List<Categoria_Cliente> listaBanco = new List<Categoria_Cliente>();

                    foreach (var item in ListService.GetListaQuebrada(listaBanco, 100))
                    {
                        List<ICriterion> listaCriteria = new List<ICriterion>();
                        listaCriteria.Add(NHibernate.Criterion.Expression.In(nameof(Categoria_Cliente.Id), lista_ids.ToArray()));
                        listaBanco.AddRange(await repositoryTipoC.selectCriteria(listaCriteria.ToArray()));
                    }
                    
                    
                    var config = repoconfig.selectFirst().Result;
                    foreach (var documentoTmp in documentos)
                    {
                        var obj = await CriaDocumentoBasico(documentoTmp);
                        var prioridade = listaBanco.FirstOrDefault(p => p.Id == documentoTmp.ID_CATEGORIA_CLIENTE)?.Tipo_Prioridade;

                        if (prioridade == null)
                        {
                            obj.Prioridade_Cliente = "0";
                        }
                        else if (prioridade == Tipo_Prioridade.Alta)
                        {
                            obj.Prioridade_Cliente = "1";
                        }
                        else if (prioridade == Tipo_Prioridade.Media)
                        {
                            obj.Prioridade_Cliente = "2";
                        }
                        else if (prioridade == Tipo_Prioridade.Baixa)
                        {
                            obj.Prioridade_Cliente = "3";
                        }


                        if (documentoTmp.TIPO_PRIORIDADE == null)
                        {
                            obj.Prioridade_Natureza = "0";
                        }
                        else if (documentoTmp.TIPO_PRIORIDADE.Value == Tipo_PrioridadeT5.Alta)
                        {
                            obj.Prioridade_Natureza = "1";
                        }
                        else if (documentoTmp.TIPO_PRIORIDADE.Value == Tipo_PrioridadeT5.Media)
                        {
                            obj.Prioridade_Natureza = "2";
                        }
                        else if (documentoTmp.TIPO_PRIORIDADE.Value == Tipo_PrioridadeT5.Baixa)
                        {
                            obj.Prioridade_Natureza = "3";
                        }

                        //Prioridade e outras Legenda das configurações da empresa

                        string legenda = null;

                        if (config.ExibirInfoOSTimeline)
                        {
                            switch (config.CampoinfoOStimeline)
                            {
                                case InfoOSTimelineEnum.TIPO_SERVICO:
                                    legenda = documentoTmp.DESC_TIPO;
                                    obj.Prioridade_Legenda = legenda + " " + obj.Prioridade_Cliente + "/" + obj.Prioridade_Natureza;
                                    break;

                                case InfoOSTimelineEnum.SEGMENTO:
                                    legenda = documentoTmp.SEGMENTO;
                                    obj.Prioridade_Legenda = legenda + " " + obj.Prioridade_Cliente + "/" + obj.Prioridade_Natureza;
                                    break;

                                case InfoOSTimelineEnum.CATEGORIA_CLIENTE:
                                    legenda = listaBanco.FirstOrDefault(p => p.Id == documentoTmp.ID_CATEGORIA_CLIENTE)?.Descricao;
                                    obj.Prioridade_Legenda = legenda + " " + obj.Prioridade_Cliente + "/" + obj.Prioridade_Natureza;
                                    break;

                                case InfoOSTimelineEnum.NATUREZA_SERVICO:
                                    // TODO: PREENCHER A NATUREZA SEM FAZER MAIS CONSULTAS AO BANCO, APENAS COM O OBJETO documentoTmp
                                    legenda = string.Empty;
                                    obj.Prioridade_Legenda = legenda + " " + obj.Prioridade_Cliente + "/" + obj.Prioridade_Natureza;
                                    break;
                            }
                        }
                        else
                        {
                            obj.Prioridade_Legenda = obj.Prioridade_Cliente + "/" + obj.Prioridade_Natureza;
                        }




                        if (documentoTmp.DATAHORA_INICIO.HasValue)
                            CalculaTempoTimeLine(documentoTmp.DATAHORA_INICIO.Value, documentoTmp.DATAHORA_FIM.Value, documentoTmp.DESLOCAMENTO, ref obj);


                        obj.TipoOS = Timeline_ServicosEquipeServicosEnum.Serviço_Previsto;

                        // Se não baixado
                        if (!documentoTmp.DATAHORA_PROD.HasValue)
                        {
                            if (documentoTmp.STATUS_EXECUCAO == StatusExecucaoAniel.OS_Em_deslocamento)
                            {
                                obj.TipoOS = Timeline_ServicosEquipeServicosEnum.Tecnico_em_Deslocamento;
                            }
                            else if (documentoTmp.STATUS_EXECUCAO == StatusExecucaoAniel.Atendimento_Iniciado_Reiniciado)
                            {
                                obj.TipoOS = Timeline_ServicosEquipeServicosEnum.Serviço_Iniciado;

                                //Caso seja iniciado, calcula o tempo previsto de acordo com o inicio
                                var datahoraatendimento = documentoTmp.DATAHORA_INI_ATEND ?? documentoTmp.DATAHORA_REINICIO;

                                if (datahoraatendimento.HasValue)
                                    obj.DataHoraIni = datahoraatendimento.Value;

                                if (documentoTmp.DATAHORA_INICIO.HasValue && documentoTmp.DATAHORA_FIM.HasValue && datahoraatendimento.HasValue)
                                    obj.DataHoraFim = await RetornaTempoIniciado(documentoTmp.DATAHORA_INICIO.Value, documentoTmp.DATAHORA_FIM.Value, datahoraatendimento.Value);
                                //else
                                //{
                                //    obj.DataHoraFim = obj.DataHoraIni.Add(TimeSpan.FromMinutes(obj.TempoExecucao));
                                //}

                            }
                            else if (documentoTmp.STATUS_EXECUCAO == StatusExecucaoAniel.Aberta_Aguard_Atendimento ||
                                documentoTmp.STATUS_EXECUCAO == StatusExecucaoAniel.Interromper_Deslocamento)
                            {
                                if (documentoTmp.DATAHORA_INICIO > DateTime.Now)
                                {
                                    obj.TipoOS = Timeline_ServicosEquipeServicosEnum.Serviço_Previsto;
                                }
                                else
                                {
                                    obj.TipoOS = Timeline_ServicosEquipeServicosEnum.Serviço_Atrasado;
                                }
                            }
                            else if (documentoTmp.STATUS_EXECUCAO == StatusExecucaoAniel.Atendimento_Paralizado)
                            {
                                obj.TipoOS = Timeline_ServicosEquipeServicosEnum.Serviço_Paralisado;
                            }
                            else if (documentoTmp.STATUS_EXECUCAO == StatusExecucaoAniel.Atendimento_Finalizado)//(documentoTmp.DATAHORA_INI_ATEND.HasValue && documentoTmp.DATAHORA_FIM_ATEND.HasValue)
                            {
                                obj.TipoOS = Timeline_ServicosEquipeServicosEnum.Servico_Com_Atendimento_Finalizado;
                            }
                            else if (!documentoTmp.DATAHORA_INI_ATEND.HasValue)
                            {
                                if (documentoTmp.DATAHORA_INICIO > DateTime.Now)
                                {
                                    obj.TipoOS = Timeline_ServicosEquipeServicosEnum.Serviço_Previsto;
                                }
                                else
                                {
                                    obj.TipoOS = Timeline_ServicosEquipeServicosEnum.Serviço_Atrasado;
                                }
                            }
                            else if ((documentoTmp.DATAHORA_INI_ATEND.HasValue &&
                                      documentoTmp.STATUS_EXECUCAO == StatusExecucaoAniel.Fechada_Improdutiva &&
                                      documentoTmp.STATUS_EXECUCAO == StatusExecucaoAniel.Fechada_Produtiva &&
                                      documentoTmp.STATUS_EXECUCAO == StatusExecucaoAniel.Atendimento_Finalizado &&
                                      documentoTmp.STATUS_EXECUCAO == StatusExecucaoAniel.Atendimento_Iniciado_Reiniciado &&
                                      documentoTmp.STATUS_EXECUCAO == StatusExecucaoAniel.Reiniciado_Atendimento))
                            {
                                obj.TipoOS = Timeline_ServicosEquipeServicosEnum.Serviço_Iniciado;

                                //Caso seja reiniciado, calcula o tempo previsto
                                obj.DataHoraIni = Convert.ToDateTime(documentoTmp.DATAHORA_INI_ATEND);

                                if (documentoTmp.DATAHORA_INICIO.HasValue && documentoTmp.DATAHORA_FIM.HasValue)
                                    obj.DataHoraFim = await RetornaTempoIniciado(documentoTmp.DATAHORA_INICIO.Value, documentoTmp.DATAHORA_FIM.Value, documentoTmp.DATAHORA_INI_ATEND.Value);
                                else
                                    obj.DataHoraFim = obj.DataHoraIni.Add(TimeSpan.FromMinutes(obj.TempoExecucao));
                            }
                        }
                        else // Se baixado
                        {
                            if (documentoTmp.DATAHORA_FIM.HasValue)
                            {
                                if (documentoTmp.PRODUTIVA)
                                {
                                    if (documentoTmp.DATAHORA_PROD.Value <= documentoTmp.DATAHORA_FIM.Value)
                                    {
                                        obj.TipoOS = Timeline_ServicosEquipeServicosEnum.Serviço_Baixado_no_Prazo_Produtivo;
                                    }
                                    else
                                    {
                                        obj.TipoOS = Timeline_ServicosEquipeServicosEnum.Serviço_Fora_do_Prazo_Produtivo;
                                    }
                                }
                                else //Improdutiva
                                {
                                    if (documentoTmp.DATAHORA_PROD.Value <= documentoTmp.DATAHORA_FIM.Value)
                                    {
                                        obj.TipoOS = Timeline_ServicosEquipeServicosEnum.Serviço_Baixado_no_Prazo_Improdutivo;
                                    }
                                    else
                                    {
                                        obj.TipoOS = Timeline_ServicosEquipeServicosEnum.Serviço_Fora_do_Prazo_Improdutivo;
                                    }
                                }
                            }

                            //Tratamento para Datas de Inicio Atendimento nulas e baixados
                            if (String.IsNullOrEmpty(Convert.ToString(documentoTmp.DATAHORA_INI_ATEND)))
                            {
                                if (String.IsNullOrEmpty(Convert.ToString(documentoTmp.DATAHORA_FIM_ATEND)) == false)
                                {
                                    TimeSpan span = Convert.ToDateTime(documentoTmp.DATAHORA_FIM).Subtract(Convert.ToDateTime(documentoTmp.DATAHORA_INICIO));
                                    DateTime horaInicio = Convert.ToDateTime(documentoTmp.DATAHORA_FIM_ATEND);
                                    horaInicio = horaInicio.AddHours(-span.Hours);
                                    horaInicio = horaInicio.AddMinutes(-span.Minutes);
                                    obj.DataHoraIni = horaInicio;
                                    obj.DataHoraFim = Convert.ToDateTime(documentoTmp.DATAHORA_FIM_ATEND);
                                }
                                else
                                {
                                    TimeSpan span = Convert.ToDateTime(documentoTmp.DATAHORA_FIM).Subtract(Convert.ToDateTime(documentoTmp.DATAHORA_INICIO));
                                    DateTime horaInicio = Convert.ToDateTime(documentoTmp.DATAHORA_PROD);
                                    horaInicio = horaInicio.AddHours(-span.Hours);
                                    horaInicio = horaInicio.AddMinutes(-span.Minutes);
                                    obj.DataHoraIni = horaInicio;
                                    obj.DataHoraFim = Convert.ToDateTime(documentoTmp.DATAHORA_PROD);
                                }
                            }
                            else
                            {
                                //Caso seja produtivo ou improdutivo, pega a hora de atendimento
                                obj.DataHoraIni = Convert.ToDateTime(documentoTmp.DATAHORA_INI_ATEND);
                                obj.DataHoraFim = Convert.ToDateTime(documentoTmp.DATAHORA_FIM_ATEND);
                            }


                        }

                        obj.ClasseCorServico = await GetClasseServicos(obj, TipoVisualizacao, Prazo, DataHoraParametro, documentoTmp);

                        if (data.HasValue)
                        {
                            bool pertenceNaData = obj.DataHoraIni.Date == data.Value.Date;
                            if (pertenceNaData == false)
                                continue;
                        }

                        obj.DataHoraIni_String = obj.DataHoraIni.ToString();
                        obj.DataHoraFim_String = obj.DataHoraFim.ToString();


                        retornoTmp.Add(obj);
                    }

                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException?.Message;
            }

            retorno.Objeto = retornoTmp;
            return retorno;
        }

        private async Task<Timeline_ServicosEquipeServicosModels> CriaDocumentoBasico(Tineline_Documentos documento)
        {
            var hasInfoReincidencia = await InformacoesReincOSAnterior(documento);

            //criar query para execucao de dados para obter o fornecedor anterior, atual, e técnico anterior

            var obj = new Timeline_ServicosEquipeServicosModels
            {
                Num_OS = documento.NUM_OBRA,
                Projeto = documento.PROJETO,
                Equipe = documento.EQUIPE,
                NumDoc = documento.NUM_DOC,
                CodCt = documento.CODCT,
                Tipo_Servico = documento.DESC_TIPO,
                Cod_Tipo_Servico = documento.COD_TIPO_SERV,
                Sub_Tipo_Servico = documento.DESC_SUBTIPO,
                Nome_Cliente = documento.TITULAR,
                Prioridade = documento.TIPO_PRIORIDADE,
                DATAHORA_INI_ATEND = documento.DATAHORA_INI_ATEND,
                DATAHORA_FIM_ATEND = documento.DATAHORA_FIM_ATEND,
                DATAHORA_PROD = documento.DATAHORA_PROD,
                DataHoraIndicador = documento.DATAHORA_INDICADOR,
                Flexivel = documento.FLEXIVEL.GetValueOrDefault(false),
                Escritorio = documento.ESCRITORIO,
                Area = documento.AREA,
                Localidade = documento.LOCALIDADE,
                Responsabilidade = HtmlEnumExtensions.GetEnumDescription(documento.RESPONSABILIDADE),
                DataHoraIniAgendamento = documento.DATAHORA_INICIO?.ToString("dd/MM/yyyy HH:mm"),
                DataHoraFimAgendamento = documento.DATAHORA_FIM?.ToString("dd/MM/yyyy HH:mm"),
                TempoExecucao = documento.TEMPO_EXECUCAO,
                Deslocamento = documento.DESLOCAMENTO,
                Segmento = documento.SEGMENTO,
                FornecedorAnterior = hasInfoReincidencia?.FornecedorAnterior,
                FornecedorAtual = hasInfoReincidencia?.FornecedorAtual,
                TecnicoAnterior = hasInfoReincidencia?.EquipeAnteriorNome,
                IsReincidencia = hasInfoReincidencia?.IsReincidencia,
            };

            DateTime dataComparar = DateTime.Now;
            var descricaoSla = new Documento_ProducaoApiController().GetDescricaoSla(documento.DATAHORA_INDICADOR, documento.DATAHORA_CRIACAO, documento.DATAHORA_PROD, dataComparar);
            obj.DescricaoSla = descricaoSla;

            switch (descricaoSla)
            {
                case "Sem indicador de SLA":
                    obj.ClasseSla = "bg-sem-sla-definido";
                    break;

                case "Dentro do Prazo":
                    obj.ClasseSla = "bg-dentro-prazo-sla";
                    break;

                case "Atenção":
                    obj.ClasseSla = "bg-atencao-sla";
                    break;

                case "Próximo do Vencimento":
                    obj.ClasseSla = "bg-prox-vencimento-sla";
                    break;

                case "Risco de Perda do Prazo":
                    obj.ClasseSla = "bg-risco-perda-sla";
                    break;

                case "Prazo Vencido":
                    obj.ClasseSla = "bg-prazo-venc-sla";
                    break;
                case "Prazo Cumprido":
                    obj.ClasseSla = "bg-prazo-cumprido-sla";
                    break;
            }

            return obj;
        }



        private async Task<InfoReincidenciaOS> InformacoesReincOSAnterior(Tineline_Documentos documento)
        {
            using (ISession session = HibernateUtil.GetSessionT5())
            using (var repoDocProducao = new Conexao<Documento_ProducaoAniel>(session))
            using (var repoTipoServico = new Conexao<Tipo_Servico_EquipeAniel>(session))
            using (var repoNatureza = new Conexao<Natureza_ProducaoAniel>(session))
            using (var repoEquipe = new Conexao<EquipeAniel>(session))
            using (var repoForn = new Conexao<Fornecedor>(session))
            using (var repoReincidencia = new Conexao<ReincidenciaDocProdAniel>(session))
            {
                InfoReincidenciaOS modal = new InfoReincidenciaOS();

                //obtem docTimeline completo
                var documentoSelecionado = await repoDocProducao.selectFirst(os =>
                    os.CodCt == documento.CODCT && os.Projeto == documento.PROJETO &&
                    os.Num_Obra == documento.NUM_OBRA && os.Num_Doc == Convert.ToInt64(documento.NUM_DOC));

                if (documentoSelecionado is null) return modal;

                //verifica a equipe atual responsavel
                var equipeAtual = await repoEquipe.selectFirst(e => e.Matricula == documentoSelecionado.Equipe);

                if (equipeAtual is null) return modal;

                //caso estiver ao tecnico um fornecedor responsavle, ira procurar por ele
                var fornecedorAtual = equipeAtual?.Cod_Fornecedor != null
                    ? await repoForn.selectFirst(f => f.Cod_Fornecedor == equipeAtual.Cod_Fornecedor)
                    : null;

                if (fornecedorAtual != null)
                    modal.FornecedorAtual = fornecedorAtual.RazaoSocial;

                var tipoServico = await repoTipoServico.selectFirst(t =>
                    t.Cod_Tipo_Serv == documentoSelecionado.Cod_Tipo_Serv);

                if (tipoServico is null) return modal;

                //verifica se é uma reincidencia dada a natureza da OS e tipo de Servico
                var codNatureza = await repoNatureza.selectFirst(n => n.Id == tipoServico.Cod_Natureza);

                if (codNatureza is null || codNatureza.GeraReincidencia != "S")
                    return modal;

                var docOsReincidencia = await repoReincidencia.selectFirst(x =>
                    x.CodCt == documentoSelecionado.CodCt &&
                    x.Projeto == documentoSelecionado.Projeto &&
                    x.NumDoc == documentoSelecionado.Num_Doc &&
                    x.NumObra == documentoSelecionado.Num_Obra); //seleciona a OS dado CODCT, PROJETO, NUM_DOC, COD_CLIENTE, E NUM_OBRA seja diferente da selecionada para roteirizar

                if (docOsReincidencia is null)
                    return modal;
                else
                    modal.IsReincidencia = true;

                var docOsAnterior = await repoDocProducao.selectFirst(x =>
                    x.CodCt == docOsReincidencia.CodCtAnterior &&
                    x.Projeto == docOsReincidencia.ProjetoAnterior &&
                    x.Num_Doc == docOsReincidencia.NumDocAnterior &&
                    x.Num_Obra == docOsReincidencia.NumObraAnterior);

                var equipeOsAnterior = await repoEquipe.selectFirst(e =>
                    e.Matricula == docOsAnterior.Equipe); //pegara a OS mais recente criada por ultimo

                if (equipeOsAnterior != null)
                    modal.EquipeAnteriorNome = equipeOsAnterior.Nome;

                var fornecedorAnterior = equipeOsAnterior?.Cod_Fornecedor != null
                    ? await repoForn.selectFirst(f => f.Cod_Fornecedor == equipeOsAnterior.Cod_Fornecedor)
                    : null;

                if (fornecedorAnterior != null)
                    modal.FornecedorAnterior = fornecedorAnterior.RazaoSocial;

                return modal;
            }
        }



        public async Task<DateTime> RetornaTempoIniciado(DateTime DataHoraIni, DateTime DataHoraFim, DateTime DataHoraInicio)
        {

            TimeSpan ts = DataHoraFim - DataHoraIni;

            DateTime retorno = DataHoraInicio.AddHours(ts.Hours);
            retorno = retorno.AddMinutes(ts.Minutes);
            retorno = retorno.AddSeconds(ts.Seconds);

            return retorno;
        }

        private void CalculaTempoTimeLine(DateTime DataHoraIni, DateTime DataHoraFim, Double? deslocamento, ref Timeline_ServicosEquipeServicosModels obj)
        {
            var tempo = DataHoraFim - DataHoraIni;
            obj.DataHoraIni = DataHoraIni;
            obj.DataHoraFim = DataHoraFim;
            obj.HoraIni = DataHoraIni.ToString("HH:mm");
            obj.HoraFim = DataHoraFim.ToString("HH:mm");
            obj.Pixels = tempo.TotalMinutes + "px";
            obj.Deslocamento = deslocamento;
        }


        public async Task<List<Timeline_ServicosEquipeServicosModels>> GetHorarioAusente(DateTime data, string tecnico, Timeline_ServicosEquipeModels equipe)
        {
            List<Timeline_ServicosEquipeServicosModels> retorno = new List<Timeline_ServicosEquipeServicosModels>();
            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (ISession session_connect = HibernateUtil.GetSession())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {

                        StringBuilder sql = new StringBuilder();
                        sql = new StringBuilder(@"SELECT A.DATA_SAIDA, A.HORA_SAIDA, A.DATA_VOLTA, A.HORA_VOLTA, A.NUM_FRE, A.PREVISTO, A.JUSTIFICATICA FROM TB_LOG_AUSENTE A 
                                              LEFT JOIN TB_EQUIPE B ON (A.NUM_FRE = B.NUM_FRE) 
                                              WHERE (A.DATA_SAIDA = @DATA OR A.DATA_VOLTA = @DATA) ");

                        parametros.Add("@DATA", data);

                        if (!String.IsNullOrEmpty(tecnico))
                        {
                            sql.Append(" AND B.EQUIPE = @NUM_FRE");
                            parametros.Add("@NUM_FRE", tecnico);
                        }

                        //Busca com paginacao
                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {

                                var justificativa = reader["JUSTIFICATICA"] != DBNull.Value ? Convert.ToString(reader["JUSTIFICATICA"]) : null;

                                var num_fre = reader["NUM_FRE"] != DBNull.Value ? Convert.ToString(reader["NUM_FRE"]) : null;

                                DateTime? data_saida = reader["DATA_SAIDA"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_SAIDA"]) : (DateTime?)null;
                                TimeSpan? hora_saida = reader["HORA_SAIDA"] != DBNull.Value ? (TimeSpan)reader["HORA_SAIDA"] : (TimeSpan?)null;
                                DateTime? data_volta = reader["DATA_VOLTA"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_VOLTA"]) : (DateTime?)null;
                                TimeSpan? hora_volta = reader["HORA_VOLTA"] != DBNull.Value ? (TimeSpan)reader["HORA_VOLTA"] : (TimeSpan?)null;


                                DateTime? dataHoraSaida = new DateTime();
                                DateTime? dataHoraRetorno = new DateTime();

                                TimeSpan? previsto = reader["PREVISTO"] != DBNull.Value ? (TimeSpan)reader["PREVISTO"] : (TimeSpan?)null;

                                if (data_saida.HasValue && hora_saida.HasValue)
                                {
                                    dataHoraSaida = data_saida.Value.Add(hora_saida.Value);
                                }


                                if (previsto.HasValue && !data_volta.HasValue && !hora_volta.HasValue)
                                {
                                    dataHoraRetorno = dataHoraSaida + previsto.Value;
                                }
                                else if (data_volta.HasValue && hora_volta.HasValue)
                                {
                                    dataHoraRetorno = data_volta.Value.Add(hora_volta.Value);
                                }

                                retorno.Add(new Timeline_ServicosEquipeServicosModels
                                {
                                    DataHoraIni = dataHoraSaida.Value,
                                    DataHoraFim = dataHoraRetorno.Value,
                                    TipoOS = Timeline_ServicosEquipeServicosEnum.Tecnico_Ausente,
                                    NumDoc = null,
                                    Num_OS = null,
                                    HtmlTitle = null,
                                    Equipe = equipe != null ? equipe.Equipe : num_fre,
                                    Prioridade_Legenda = justificativa,
                                    Pixels = ((dataHoraSaida.Value - dataHoraRetorno.Value).TotalMinutes) + "px"
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //                          ----        ProTip      ----
                //      ----    Use Shift+F9 para capturar a exceção caso precise       -----
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }
            return retorno;
        }

        public async Task<List<Painel_Servicos_AgendamentosEquipe>> GetHorarioAusentePainelServicos(DateTime data, string tecnico, Timeline_ServicosEquipeModels equipe)
        {

            using (ISession session = HibernateUtil.GetSessionT5())
            using (ISession session_connect = HibernateUtil.GetSession())
            {
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {
                    List<Painel_Servicos_AgendamentosEquipe> retorno = new List<Painel_Servicos_AgendamentosEquipe>();

                    StringBuilder sql = new StringBuilder();
                    sql = new StringBuilder(@"SELECT A.DATA_SAIDA, A.HORA_SAIDA, A.DATA_VOLTA, A.HORA_VOLTA, A.NUM_FRE, A.PREVISTO, A.JUSTIFICATICA FROM TB_LOG_AUSENTE A  
                                              LEFT JOIN TB_EQUIPE B ON (A.NUM_FRE = B.NUM_FRE) 
                                              WHERE A.DATA_SAIDA = @DATA ");

                    parametros.Add("@DATA", data);

                    if (!String.IsNullOrEmpty(tecnico))
                    {
                        sql.Append(" AND B.EQUIPE = @NUM_FRE");
                        parametros.Add("@NUM_FRE", tecnico);
                    }

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {

                            var justificativa = reader["JUSTIFICATICA"] != DBNull.Value ? Convert.ToString(reader["JUSTIFICATICA"]) : null;

                            var num_fre = reader["NUM_FRE"] != DBNull.Value ? Convert.ToString(reader["NUM_FRE"]) : null;

                            DateTime? data_saida = reader["DATA_SAIDA"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_SAIDA"]) : (DateTime?)null;
                            TimeSpan? hora_saida = reader["HORA_SAIDA"] != DBNull.Value ? (TimeSpan)reader["HORA_SAIDA"] : (TimeSpan?)null;
                            DateTime? data_volta = reader["DATA_VOLTA"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_VOLTA"]) : (DateTime?)null;
                            TimeSpan? hora_volta = reader["HORA_VOLTA"] != DBNull.Value ? (TimeSpan)reader["HORA_VOLTA"] : (TimeSpan?)null;


                            DateTime? dataHoraSaida = new DateTime();
                            DateTime? dataHoraRetorno = new DateTime();

                            TimeSpan? previsto = reader["PREVISTO"] != DBNull.Value ? (TimeSpan)reader["PREVISTO"] : (TimeSpan?)null;

                            if (data_saida.HasValue && hora_saida.HasValue)
                            {
                                dataHoraSaida = data_saida.Value.Add(hora_saida.Value);
                            }


                            if (previsto.HasValue && !data_volta.HasValue && !hora_volta.HasValue)
                            {
                                dataHoraRetorno = dataHoraSaida + previsto.Value;
                            }
                            else if (data_volta.HasValue && hora_volta.HasValue)
                            {
                                dataHoraRetorno = data_volta.Value.Add(hora_volta.Value);
                            }

                            retorno.Add(new Painel_Servicos_AgendamentosEquipe
                            {
                                Data_Inicial = dataHoraSaida,
                                Data_Final = dataHoraRetorno,
                                Equipe = num_fre
                            });
                        }
                    }

                    return retorno;
                }
            }

        }


        public async Task<List<Tineline_Documentos>> GetDocumentos(ISession sessionT5, ISession session, DateTime? Data, String Equipe, String Monitor, String Supervisor, bool TodosTecnicos, String GrupoArea, String Area, String localidade, String TipoServico, String SubTipoServico, String NaturezaServico, Tipo_Prioridade? TipoPrioridade, String DataCriacaoInicio, String DataCriacaoFim, string Contrato, string Projeto)
        {
            List<Tineline_Documentos> retorno = new List<Tineline_Documentos>();

            try
            {

                IUsuarioRepository repoUsuario = new UsuarioRepository(session);
                var usuarioBusiness = new UsuarioBusiness(repoUsuario);

                IdentityUser user = usuarioBusiness.GetById(User.Identity.GetUserId());
                var config = await new Conexao<Configuracoes_Gerais>(session).selectFirst();
                Boolean Exibe_RDOs = config.ExibeRDOs;
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = sessionT5.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();
                    StringBuilder sqlWhereAgendamento = new StringBuilder();
                    StringBuilder sqlWhereDocumentoProducao = new StringBuilder();
                    StringBuilder sqlWhere = new StringBuilder();
                    if (Data.HasValue)
                    {

                        sql = new StringBuilder(@"SELECT A.EQUIPE,L.DESCRICAO AS SEGMENTO,A.NUM_OBRA,A.PROJETO,A.CODCT,A.NUM_DOC, A.COD_TIPO_SERV, A.NUM_OBRA_ORIGINAL, A.DATA_PROD, A.HORA_PROD, A.STATUS_EXECUCAO, A.TITULAR, C.STATUS_EXECUCAO AS STATUS_EXECUCAO_ENCERRAMENTO,A.DATAHORA_DESLOCAMENTO, A.DATAHORA_INI_ATEND, A.DATA_INDICADOR, A.HORA_INDICADOR,A.DIA,A.HORA,
                                                            A.DATAHORA_FIM_ATEND, A.DATAHORA_PAUSOU, A.DATAHORA_REINICIO, D.DESCRICAO AS DESC_TIPO, E.DESCRICAO AS DESC_SUBTIPO,J.ID_CATEGORIA, A.TIPO_PRIORIDADE,
                                                            B.DATAHORA_INICIO, B.DATAHORA_FIM, B.DESLOCAMENTO, A.DATAHORA_INTERROMPER_DESLOC,B.FLEXIVEL,F.DESCRICAO AS DESCRICAO_ESCRITORIO,G.DESCRICAO AS DESCRICAO_AREA,H.LOCALIDADE,COALESCE(J.RAZAO_SOCIAL,A.TITULAR) AS TITULAR,A.RESPONSABILIDADE,
                                                            (SELECT FIRST 1 M.HORAS 
                                                             FROM TB_TIPO_SERVICO_EQUIPE_IMOVEL M 
                                                             WHERE ( (M.ID_TIPO_IMOVEL = A.PARPRIM_PARA OR M.ID_TIPO_IMOVEL IS NULL) AND M.ID_TIPO_SERVICO = A.COD_TIPO_SERV) ORDER BY M.ID_TIPO_IMOVEL DESC) AS TEMPO_EXECUCAO
                                                     FROM TB_AGENDAMENTO_EQUIPEANIEL B
                                                     LEFT JOIN TB_DOCUMENTO_PRODUCAO A 
                                                     ON(A.CODCT = B.CODCT AND A.PROJETO = B.PROJETO AND A.NUM_OBRA = B.NUM_OBRA AND 
                                                         A.NUM_DOC = B.NUM_DOC) 
                                                     LEFT JOIN TB_CODIGO_ENCERRAMENTO C 
                                                     ON(A.COD_ENCERRAMENTO = C.COD_ENCERRAMENTO AND A.CODCT = C.CODCT) 
                                                     LEFT JOIN TB_TIPO_SERVICO_EQUIPE D 
                                                     ON (A.COD_TIPO_SERV = D.COD_TIPO_SERV) 
                                                     LEFT JOIN TB_SUB_TIPO_SERVICO E 
                                                     ON (A.COD_TIPO_SERV = E.COD_TIPO_SERV AND A.COD_SUB_TIPO_SERV = E.COD_SUB_TIPO_SERV) 
                                                    LEFT JOIN TB_ESCRITORIO F 
                                                    ON(A.COD_ESCRITORIO = F.COD_ESCRITORIO) 
                                                    LEFT JOIN TB_FUNCIONARIO FUNC
                                                    ON (FUNC.NUM_FRE = A.EQUIPE)
                                                    LEFT JOIN TB_AREA G 
                                                    ON(G.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = G.COD_ESCRITORIO AND A.COD_AREA = G.COD_AREA) 
                                                    LEFT JOIN TB_LOCALIDADE H 
                                                    ON(H.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = H.COD_ESCRITORIO AND A.COD_AREA = H.COD_AREA AND 
                                                    A.COD_LOCALIDADE = H.COD_LOCALIDADE) 
                                                    LEFT JOIN TB_CLIENTE J 
                                                    ON(A.ID_CLIENTE = J.COD_CLIENTE)    
                                                    LEFT JOIN TB_EQUIPE K
                                                    ON(A.EQUIPE = K.equipe)
                                                    LEFT JOIN TB_SEGMENTO_SERVICO L
                                                    ON(A.SEGMENTO = L.CODIGO) 
                                                     @SQLWHEREAGENDAMENTO

                                                     UNION

                                                     SELECT A.EQUIPE,L.DESCRICAO AS SEGMENTO,A.NUM_OBRA,A.PROJETO,A.CODCT, A.NUM_DOC, A.COD_TIPO_SERV, A.NUM_OBRA_ORIGINAL, A.DATA_PROD, A.HORA_PROD, A.STATUS_EXECUCAO, A.TITULAR, C.STATUS_EXECUCAO AS STATUS_EXECUCAO_ENCERRAMENTO,         A.DATAHORA_DESLOCAMENTO, A.DATAHORA_INI_ATEND, A.DATA_INDICADOR, A.HORA_INDICADOR,A.DIA,A.HORA,
                                                            A.DATAHORA_FIM_ATEND, A.DATAHORA_PAUSOU, A.DATAHORA_REINICIO, D.DESCRICAO AS DESC_TIPO, E.DESCRICAO AS DESC_SUBTIPO,J.ID_CATEGORIA, A.TIPO_PRIORIDADE,
                                                            B.DATAHORA_INICIO, B.DATAHORA_FIM, B.DESLOCAMENTO, A.DATAHORA_INTERROMPER_DESLOC,B.FLEXIVEL,F.DESCRICAO AS DESCRICAO_ESCRITORIO,G.DESCRICAO AS DESCRICAO_AREA,H.LOCALIDADE,COALESCE(J.RAZAO_SOCIAL,A.TITULAR) AS TITULAR,A.RESPONSABILIDADE,
                                                            (SELECT FIRST 1 M.HORAS 
                                                             FROM TB_TIPO_SERVICO_EQUIPE_IMOVEL M 
                                                             WHERE ( (M.ID_TIPO_IMOVEL = A.PARPRIM_PARA OR M.ID_TIPO_IMOVEL IS NULL) AND M.ID_TIPO_SERVICO = A.COD_TIPO_SERV) ORDER BY M.ID_TIPO_IMOVEL DESC) AS TEMPO_EXECUCAO
                                                     FROM  TB_DOCUMENTO_PRODUCAO A
                                                     INNER JOIN TB_AGENDAMENTO_EQUIPEANIEL B
                                                     ON(A.CODCT = B.CODCT AND A.PROJETO = B.PROJETO AND A.NUM_OBRA = B.NUM_OBRA AND 
                                                         A.NUM_DOC = B.NUM_DOC) 
                                                     LEFT JOIN TB_CODIGO_ENCERRAMENTO C 
                                                     ON(A.COD_ENCERRAMENTO = C.COD_ENCERRAMENTO AND A.CODCT = C.CODCT) 
                                                     LEFT JOIN TB_TIPO_SERVICO_EQUIPE D 
                                                     ON (A.COD_TIPO_SERV = D.COD_TIPO_SERV) 
                                                     LEFT JOIN TB_SUB_TIPO_SERVICO E 
                                                     ON (A.COD_TIPO_SERV = E.COD_TIPO_SERV AND A.COD_SUB_TIPO_SERV = E.COD_SUB_TIPO_SERV) 
                                                    LEFT JOIN TB_ESCRITORIO F 
                                                    ON(A.COD_ESCRITORIO = F.COD_ESCRITORIO) 
                                                    LEFT JOIN TB_FUNCIONARIO FUNC
                                                    ON (FUNC.NUM_FRE = A.EQUIPE)
                                                    LEFT JOIN TB_AREA G 
                                                    ON(G.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = G.COD_ESCRITORIO AND A.COD_AREA = G.COD_AREA) 
                                                    LEFT JOIN TB_LOCALIDADE H 
                                                    ON(H.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = H.COD_ESCRITORIO AND A.COD_AREA = H.COD_AREA AND 
                                                    A.COD_LOCALIDADE = H.COD_LOCALIDADE) 
                                                    LEFT JOIN TB_CLIENTE J 
                                                    ON(A.ID_CLIENTE = J.COD_CLIENTE) 
                                                    LEFT JOIN TB_EQUIPE K
                                                    ON(A.EQUIPE = K.equipe)
                                                    LEFT JOIN TB_SEGMENTO_SERVICO L
                                                    ON(A.SEGMENTO = L.CODIGO) 
                                                     @SQLWHEREDOCUMENTO ");
                    }
                    else
                    {

                        sql = new StringBuilder("SELECT A.EQUIPE,K.DESCRICAO AS SEGMENTO,A.NUM_OBRA,A.PROJETO, A.COD_TIPO_SERV, A.CODCT,A.NUM_DOC, A.NUM_OBRA_ORIGINAL, A.DATA_PROD, A.HORA_PROD, A.STATUS_EXECUCAO, A.TITULAR, C.STATUS_EXECUCAO AS STATUS_EXECUCAO_ENCERRAMENTO, ")
                            .AppendLine("        A.DATAHORA_DESLOCAMENTO, A.DATAHORA_INI_ATEND, A.DATA_INDICADOR, A.HORA_INDICADOR,A.DIA,A.HORA,A.TIPO_PRIORIDADE,J.ID_CATEGORIA, ")
                            .AppendLine("        A.DATAHORA_FIM_ATEND, A.DATAHORA_PAUSOU, A.DATAHORA_REINICIO, D.DESCRICAO AS DESC_TIPO, E.DESCRICAO AS DESC_SUBTIPO, ")
                            .AppendLine("        B.DATAHORA_INICIO, B.DATAHORA_FIM, B.DESLOCAMENTO, A.DATAHORA_INTERROMPER_DESLOC,B.FLEXIVEL,F.DESCRICAO AS DESCRICAO_ESCRITORIO,G.DESCRICAO AS DESCRICAO_AREA,H.LOCALIDADE,COALESCE(J.RAZAO_SOCIAL,A.TITULAR) AS TITULAR,A.RESPONSABILIDADE, ")
                            .AppendLine("(SELECT FIRST 1 M.HORAS ")
                            .AppendLine("FROM TB_TIPO_SERVICO_EQUIPE_IMOVEL M ")
                            .AppendLine("WHERE ( (M.ID_TIPO_IMOVEL = A.PARPRIM_PARA OR M.ID_TIPO_IMOVEL IS NULL) AND M.ID_TIPO_SERVICO = A.COD_TIPO_SERV) ORDER BY M.ID_TIPO_IMOVEL DESC) AS TEMPO_EXECUCAO ")
                            .AppendLine(" FROM TB_DOCUMENTO_PRODUCAO A  ")
                            .AppendLine(" LEFT JOIN TB_AGENDAMENTO_EQUIPEANIEL B ")
                            .AppendLine(" ON(A.CODCT = B.CODCT AND A.PROJETO = B.PROJETO AND A.NUM_OBRA = B.NUM_OBRA AND ")
                            .AppendLine("     A.NUM_DOC = B.NUM_DOC) ")
                            .AppendLine(" LEFT JOIN TB_CODIGO_ENCERRAMENTO C ")
                            .AppendLine(" ON(A.COD_ENCERRAMENTO = C.COD_ENCERRAMENTO AND A.CODCT = C.CODCT) ")

                            .AppendLine(" LEFT JOIN TB_TIPO_SERVICO_EQUIPE D ")
                            .AppendLine(" ON (A.COD_TIPO_SERV = D.COD_TIPO_SERV) ")
                            .AppendLine(" LEFT JOIN TB_SUB_TIPO_SERVICO E ")
                            .AppendLine(" ON (A.COD_TIPO_SERV = E.COD_TIPO_SERV AND A.COD_SUB_TIPO_SERV = E.COD_SUB_TIPO_SERV) ")
                            .AppendLine("LEFT JOIN TB_ESCRITORIO F ")
                            .AppendLine("ON(A.COD_ESCRITORIO = F.COD_ESCRITORIO) ")
                            .AppendLine("LEFT JOIN TB_FUNCIONARIO FUNC ")
                            .AppendLine("ON(A.EQUIPE = FUNC.NUM_FRE) ")
                            .AppendLine("LEFT JOIN TB_AREA G ")
                            .AppendLine("ON(G.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = G.COD_ESCRITORIO AND A.COD_AREA = G.COD_AREA) ")
                            .AppendLine("LEFT JOIN TB_LOCALIDADE H ")
                            .AppendLine("ON(H.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = H.COD_ESCRITORIO AND A.COD_AREA = H.COD_AREA AND ")
                            .AppendLine("A.COD_LOCALIDADE = H.COD_LOCALIDADE) ")
                            .AppendLine("LEFT JOIN TB_CLIENTE J ")
                            .AppendLine("ON(A.ID_CLIENTE = J.COD_CLIENTE) ")
                            .AppendLine("LEFT JOIN TB_SEGMENTO_SERVICO K  ")
                            .AppendLine("ON(A.SEGMENTO = K.CODIGO) ")
                            .AppendLine("@SQLWHEREDOCUMENTO");

                    }

                    //.AppendLine(" WHERE A.EQUIPE = @EQUIPE AND A.DATA_MAXIMA BETWEEN @DATA AND @DATAFIM ");  // COALESCE(A.SIGLA_MODULO, '') <> 'MCO'
                    if (TodosTecnicos == true)
                    {
                        if (Data.HasValue)
                        {
                            sqlWhereAgendamento.AppendLine(" WHERE (B.DATAHORA_INICIO BETWEEN @DATA AND @DATAFIM) ");
                            sqlWhereDocumentoProducao.AppendLine(" WHERE (A.DATAHORA_INI_ATEND BETWEEN @DATA AND @DATAFIM) ");

                        }
                        else
                        {
                            sqlWhereAgendamento.AppendLine(" WHERE B.DATAHORA_INICIO IS NULL ");
                        }

                    }
                    else
                    {
                        if (Equipe != null)
                        {
                            sqlWhereAgendamento.AppendLine(" WHERE A.EQUIPE = @EQUIPE ");
                            sqlWhereDocumentoProducao.AppendLine(" WHERE A.EQUIPE = @EQUIPE ");

                            parametros.Add("@EQUIPE", Equipe);
                        }
                        else
                        {
                            sqlWhereAgendamento.AppendLine(" WHERE A.EQUIPE IS NULL ");  // COALESCE(A.SIGLA_MODULO, '') <> 'MCO'
                            sqlWhereDocumentoProducao.AppendLine(" WHERE A.EQUIPE IS NULL ");

                            // nesse caso so devem ser exibidos servicos abertos
                            sqlWhere.AppendLine(" AND (A.STATUS_EXECUCAO <> 1 AND A.STATUS_EXECUCAO <> 2 AND A.STATUS_EXECUCAO <> 8) ");
                        }


                        if (Data.HasValue)
                        {
                            sqlWhereAgendamento.AppendLine(" AND (B.DATAHORA_INICIO BETWEEN @DATA AND @DATAFIM) ");
                            sqlWhereDocumentoProducao.AppendLine(" AND (A.DATAHORA_INI_ATEND BETWEEN @DATA AND @DATAFIM) ");
                        }
                        else
                        {
                            sqlWhere.AppendLine(" AND B.DATAHORA_INICIO IS NULL ");
                        }
                    }

                    sqlWhere.AppendLine(" AND A.STATUS_EXECUCAO <> 9 ");

                    if (!Exibe_RDOs)
                    {
                        sqlWhere.AppendLine(" AND COALESCE(A.SIGLA_MODULO, '') <> 'MCO' ");
                    }

                    if (!String.IsNullOrEmpty(Contrato))
                    {
                        sqlWhere.AppendLine(" AND A.CODCT = @CONTRATO");
                        parametros.Add("@CONTRATO", Contrato);
                    }
                    if (!String.IsNullOrEmpty(Projeto))
                    {
                        sqlWhere.AppendLine(" AND A.PROJETO = @PROJETO");
                        parametros.Add("@PROJETO", Projeto);
                    }
                    if (String.IsNullOrEmpty(GrupoArea) == false)
                    {
                        sqlWhere.AppendLine(" AND A.COD_ESCRITORIO = @GRUPOAREA ");
                        parametros.Add("@GRUPOAREA", GrupoArea);
                    }

                    if (String.IsNullOrEmpty(Area) == false)
                    {
                        sqlWhere.AppendLine(" AND A.COD_AREA = @AREA ");
                        parametros.Add("@AREA", Area);
                    }

                    if (String.IsNullOrEmpty(localidade) == false)
                    {
                        sqlWhere.AppendLine(" AND A.COD_LOCALIDADE = @LOCALIDADE ");
                        parametros.Add("@LOCALIDADE", localidade);
                    }

                    if (!String.IsNullOrEmpty(Monitor) && Monitor != "null")
                    {
                        sqlWhere.AppendLine(" AND K.USUARIO_MONITOR = @ID_MONITOR ");
                        parametros.Add("@ID_MONITOR", Monitor);
                    }

                    if (!String.IsNullOrEmpty(Supervisor) && Supervisor != "null")
                    {
                        sqlWhere.AppendLine(" AND K.COD_SUPERV = @COD_SUPERV ");
                        parametros.Add("@COD_SUPERV", Supervisor);
                    }

                    if (String.IsNullOrEmpty(TipoServico) == false)
                    {
                        sqlWhere.AppendLine($" AND A.COD_TIPO_SERV IN ({TipoServico}) ");
                    }

                    if (String.IsNullOrEmpty(SubTipoServico) == false)
                    {
                        sqlWhere.AppendLine($" AND A.COD_SUB_TIPO_SERV IN ({SubTipoServico}) ");
                    }

                    if (String.IsNullOrEmpty(NaturezaServico) == false)
                    {
                        sqlWhere.AppendLine(" AND D.COD_NATUREZA = @NATUREZA ");
                        parametros.Add("@NATUREZA", NaturezaServico);
                    }
                    if (TipoPrioridade.HasValue)
                    {
                        sqlWhere.AppendLine(" AND A.TIPO_PRIORIDADE = @TIPO_PRIORIDADE ");
                        parametros.Add("@TIPO_PRIORIDADE", (int)TipoPrioridade.Value);
                    }


                    if (String.IsNullOrEmpty(DataCriacaoInicio) == false && String.IsNullOrEmpty(DataCriacaoFim) == false)
                    {
                        sqlWhere.AppendLine(" AND A.DIA BETWEEN @DIA_INICIO AND  @DIA_FIM ");
                        parametros.Add("@DIA_INICIO", DataCriacaoInicio);
                        parametros.Add("@DIA_FIM", DataCriacaoFim);
                    }

                    #region Filtro Perfil Visualização de Serviço

                    if (user.PerfilVisualizacaoServico != null)
                    {
                        if (user.PerfilVisualizacaoServico.ParamsContratoProjeto.Count() > 0)
                        {
                            var filtrosContratoProj = new List<string>();

                            foreach (var paramContratoProj in user.PerfilVisualizacaoServico.ParamsContratoProjeto)
                            {
                                if (!String.IsNullOrEmpty(paramContratoProj.CodCt) && !String.IsNullOrEmpty(paramContratoProj.Projeto))
                                {
                                    filtrosContratoProj.Add($"(A.CODCT = '{paramContratoProj.CodCt}' AND A.PROJETO = '{paramContratoProj.Projeto}')");
                                }
                                else
                                {
                                    filtrosContratoProj.Add($"(A.CODCT = '{paramContratoProj.CodCt}')");
                                }
                            }

                            sqlWhere.Append($" AND ({String.Join(" OR ", filtrosContratoProj)})");
                        }

                        if (user.PerfilVisualizacaoServico.ParamsArea.Count() > 0)
                        {
                            var filtrosArea = new List<string>();

                            foreach (var paramArea in user.PerfilVisualizacaoServico.ParamsArea)
                            {
                                if (!String.IsNullOrEmpty(paramArea.CodEscritorio) &&
                                   !String.IsNullOrEmpty(paramArea.CodArea) &&
                                   paramArea.CodLocalidade.HasValue)
                                {
                                    filtrosArea.Add($"(A.COD_ESCRITORIO = '{paramArea.CodEscritorio}' AND A.COD_AREA = '{paramArea.CodArea}' AND A.COD_LOCALIDADE = {paramArea.CodLocalidade.Value})");
                                }
                                else if (!String.IsNullOrEmpty(paramArea.CodEscritorio) &&
                                         !String.IsNullOrEmpty(paramArea.CodArea))
                                {
                                    filtrosArea.Add($"(A.COD_ESCRITORIO = '{paramArea.CodEscritorio}' AND A.COD_AREA = '{paramArea.CodArea}')");
                                }
                                else
                                {
                                    filtrosArea.Add($"(A.COD_ESCRITORIO = '{paramArea.CodEscritorio}')");
                                }
                            }

                            sqlWhere.Append($" AND ({String.Join(" OR ", filtrosArea)})");
                        }

                        if (user.PerfilVisualizacaoServico.ParamsSetorDepto.Count() > 0)
                        {
                            var filtrosSetor = new List<string>();

                            foreach (var paramSetor in user.PerfilVisualizacaoServico.ParamsSetorDepto)
                            {
                                filtrosSetor.Add($"(A.COD_DEPTO = {paramSetor.CodDepto})");
                            }

                            sqlWhere.Append($" AND ({String.Join(" OR ", filtrosSetor)})");
                        }

                        if (user.PerfilVisualizacaoServico.ParamsSupervisor.Count() > 0)
                        {
                            var filtrosSupervisor = new List<string>();

                            foreach (var paramSupervisor in user.PerfilVisualizacaoServico.ParamsSupervisor)
                            {
                                filtrosSupervisor.Add($"(A.COD_SUPERV = '{paramSupervisor.CodSuperv}')");
                            }

                            sqlWhere.Append($" AND ({String.Join(" OR ", filtrosSupervisor)})");
                        }

                        if (user.PerfilVisualizacaoServico.ParamsMonitor.Count() > 0)
                        {
                            var filtrosMonitor = new List<string>();

                            foreach (var paramMonitor in user.PerfilVisualizacaoServico.ParamsMonitor)
                            {
                                filtrosMonitor.Add($"(A.USUARIO_MONITOR = '{paramMonitor.UsuarioMonitor}')");
                            }

                            sqlWhere.Append($" AND ({String.Join(" OR ", filtrosMonitor)})");
                        }
                    }

                    #endregion

                    if (Data.HasValue)
                    {
                        parametros.Add("@DATA", Data.Value.Date);
                        parametros.Add("@DATAFIM", Data.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59));
                    }

                    sqlWhereAgendamento.AppendLine(sqlWhere.ToString());
                    sqlWhereDocumentoProducao.AppendLine(sqlWhere.ToString());
                    sql.Replace("@SQLWHEREAGENDAMENTO", sqlWhereAgendamento.ToString());
                    sql.Replace("@SQLWHEREDOCUMENTO", sqlWhereDocumentoProducao.ToString());

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            var documento = new Tineline_Documentos
                            {
                                NUM_OBRA = reader["NUM_OBRA"] != DBNull.Value ? Convert.ToString(reader["NUM_OBRA"]) : String.Empty,
                                PROJETO = reader["PROJETO"] != DBNull.Value ? Convert.ToString(reader["PROJETO"]) : String.Empty,
                                EQUIPE = reader["EQUIPE"] != DBNull.Value ? Convert.ToString(reader["EQUIPE"]) : String.Empty,
                                CODCT = reader["CODCT"] != DBNull.Value ? Convert.ToString(reader["CODCT"]) : String.Empty,
                                NUM_DOC = reader["NUM_DOC"] != DBNull.Value ? Convert.ToString(reader["NUM_DOC"]) : String.Empty,
                                NUM_OBRA_ORIGINAL = Convert.ToString(reader["NUM_OBRA_ORIGINAL"]),
                                DATAHORA_CRIACAO = reader["DIA"] != DBNull.Value ? Convert.ToDateTime(reader["DIA"]) : (DateTime?)null,
                                DATAHORA_PROD = reader["DATA_PROD"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_PROD"]) : (DateTime?)null,
                                DATAHORA_INDICADOR = reader["DATA_INDICADOR"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_INDICADOR"]) : (DateTime?)null,
                                STATUS_EXECUCAO = reader["STATUS_EXECUCAO"] != DBNull.Value ? (StatusExecucaoAniel)Enum.Parse(typeof(StatusExecucaoAniel), Convert.ToString(reader["STATUS_EXECUCAO"])) : StatusExecucaoAniel.Aberta_Aguard_Agendamento,
                                PRODUTIVA = reader["STATUS_EXECUCAO_ENCERRAMENTO"] != DBNull.Value ? Convert.ToInt32(reader["STATUS_EXECUCAO_ENCERRAMENTO"]) == 2 : false,
                                DESC_TIPO = reader["DESC_TIPO"] != DBNull.Value ? Convert.ToString(reader["DESC_TIPO"]) : String.Empty,
                                COD_TIPO_SERV = reader["COD_TIPO_SERV"] != DBNull.Value ? Convert.ToString(reader["COD_TIPO_SERV"]) : String.Empty,
                                DESC_SUBTIPO = reader["DESC_SUBTIPO"] != DBNull.Value ? Convert.ToString(reader["DESC_SUBTIPO"]) : String.Empty,
                                TITULAR = reader["TITULAR"] != DBNull.Value ? Convert.ToString(reader["TITULAR"]) : String.Empty,
                                DATAHORA_DESLOCAMENTO = reader["DATAHORA_DESLOCAMENTO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_DESLOCAMENTO"]) : (DateTime?)null,
                                DATAHORA_INTERROMPER_DESLOC = reader["DATAHORA_INTERROMPER_DESLOC"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_INTERROMPER_DESLOC"]) : (DateTime?)null,
                                DATAHORA_INI_ATEND = reader["DATAHORA_INI_ATEND"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_INI_ATEND"]) : (DateTime?)null,
                                DATAHORA_FIM_ATEND = reader["DATAHORA_FIM_ATEND"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_FIM_ATEND"]) : (DateTime?)null,
                                DATAHORA_PAUSOU = reader["DATAHORA_PAUSOU"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_PAUSOU"]) : (DateTime?)null,
                                DATAHORA_REINICIO = reader["DATAHORA_REINICIO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_REINICIO"]) : (DateTime?)null,
                                DATAHORA_INICIO = reader["DATAHORA_INICIO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_INICIO"]) : (DateTime?)null,
                                DATAHORA_FIM = reader["DATAHORA_FIM"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_FIM"]) : (DateTime?)null,
                                DESLOCAMENTO = reader["DESLOCAMENTO"] != DBNull.Value ? Convert.ToDouble(reader["DESLOCAMENTO"]) : 0,
                                FLEXIVEL = reader["FLEXIVEL"] != DBNull.Value ? Convert.ToBoolean(reader["FLEXIVEL"]) : (Boolean?)null,
                                ESCRITORIO = reader["DESCRICAO_ESCRITORIO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO_ESCRITORIO"]) : String.Empty,
                                AREA = reader["DESCRICAO_AREA"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO_AREA"]) : String.Empty,
                                LOCALIDADE = reader["DESCRICAO_AREA"] != DBNull.Value ? Convert.ToString(reader["LOCALIDADE"]) : String.Empty,
                                ID_CATEGORIA_CLIENTE = reader["ID_CATEGORIA"] != DBNull.Value ? Convert.ToInt64(reader["ID_CATEGORIA"]) : (long?)null,
                                TEMPO_EXECUCAO = reader["TEMPO_EXECUCAO"] != DBNull.Value ? Convert.ToDateTime(((TimeSpan)(reader["TEMPO_EXECUCAO"])).ToString(@"hh\:mm")).TimeOfDay.TotalMinutes : (config.Tempo_Execucao_Padrao.HasValue ? config.Tempo_Execucao_Padrao.Value.TimeOfDay.TotalMinutes : 0),
                                RESPONSABILIDADE = reader["RESPONSABILIDADE"] != DBNull.Value ? (ResponsabilidadeOS)Enum.Parse(typeof(ResponsabilidadeOS), Convert.ToString(reader["RESPONSABILIDADE"])) : (ResponsabilidadeOS?)null,
                                TIPO_PRIORIDADE = reader["TIPO_PRIORIDADE"] != DBNull.Value ? (Tipo_PrioridadeT5)Enum.Parse(typeof(Tipo_PrioridadeT5), Convert.ToString(reader["TIPO_PRIORIDADE"])) : Tipo_PrioridadeT5.NaoEncontrado,
                                SEGMENTO = reader["SEGMENTO"] != DBNull.Value ? Convert.ToString(reader["SEGMENTO"]) : String.Empty,
                            };


                            if (!Data.HasValue && reader["TIPO_PRIORIDADE"] != DBNull.Value)
                            {
                                documento.TIPO_PRIORIDADE = (Tipo_PrioridadeT5)Enum.Parse(typeof(Tipo_PrioridadeT5), Convert.ToString(reader["TIPO_PRIORIDADE"]));
                            }

                            if (documento.DATAHORA_PROD.HasValue && reader["HORA_PROD"] != DBNull.Value)
                            {
                                var hora = ((TimeSpan)(reader["HORA_PROD"]));
                                documento.DATAHORA_PROD = documento.DATAHORA_PROD.Value.Add(hora);
                            }

                            if (documento.DATAHORA_CRIACAO.HasValue && reader["HORA"] != DBNull.Value)
                            {
                                var hora = ((TimeSpan)(reader["HORA"]));
                                documento.DATAHORA_CRIACAO = documento.DATAHORA_CRIACAO.Value.Add(hora);
                            }

                            if (documento.DATAHORA_INDICADOR.HasValue && reader["HORA_INDICADOR"] != DBNull.Value)
                            {
                                var hora = ((TimeSpan)(reader["HORA_INDICADOR"]));
                                documento.DATAHORA_INDICADOR = documento.DATAHORA_INDICADOR.Value.Add(hora);
                            }



                            retorno.Add(documento);
                        }
                    }

                    return retorno;
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static List<(DateTime Inicio, DateTime Fim)> QuebrarPeriodoPorDia(
            DateTime inicio,
            DateTime fim
        )
        {
            var resultado = new List<(DateTime Inicio, DateTime Fim)>();

            if (fim <= inicio)
                return resultado;

            var inicioAtual = inicio;

            while (inicioAtual < fim)
            {
                var fimDoDia = inicioAtual.Date.AddDays(1).AddTicks(-1);
                var fimAtual = fim < fimDoDia ? fim : fimDoDia;
                resultado.Add((inicioAtual, fimAtual));
                inicioAtual = fimDoDia.AddSeconds(1);
            }

            return resultado;
        }


    }
}