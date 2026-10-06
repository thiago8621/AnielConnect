using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Models.T5;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.T5.DocumentoProducao;
using Aniel.Connect.Repository.T5.ProdutividadeTecnico;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Resources;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;


namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class PlanejamentoCapacidadeRoteirizacaoApiController : BaseApiController
    {
        [HttpPost]
        [Route("api/PlanejamentoCapacidadeRoteirizacaoApiController/GestaoCapacidadeRoteirizacao")]
        public async Task<GestaoCapacidadeRoteirizacaoResponse> GestaoCapacidadeRoteirizacao(GestaoCapacidadeRoteirizacaoRequest request)
        {
            GestaoCapacidadeRoteirizacaoResponse retorno = new GestaoCapacidadeRoteirizacaoResponse();
            try
            {
                using (var roteirizacaoDAO = new RoteirizacaoDAO())
                using (ISession session = HibernateUtil.GetSession())
                using (Conexao<Configuracoes_Gerais> repoConfig = new Conexao<Configuracoes_Gerais>(session))
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repoHorario = new Conexao<Horario_Connect>(sessionT5))
                using (var horarioApi = new HorarioApiController())
                {
                    List<Horario_Connect> horarios_servico = (repoHorario.select(p => p.Tipo == Aniel.T5.Model.enumerado.Tipo_Hora_Connect.Horario_Servico && p.Status == Sinapse.Model.enumerados.Status_Default.Ativo).Result).ToList();
                    var horario_padrao = horarios_servico.Find(p => p.Padrao == true);
                    var horarioDia = horarioApi.RetornaHorarioDia(horario_padrao, Convert.ToDateTime(request.Data), false);
                    var HorasTrabalhadasHorario = horarioApi.MinutosTrabalhadosHorario(horarioDia.inicio, horarioDia.inicio_intervalo, horarioDia.termino_intervalo, horarioDia.termino);
                    retorno.minutosHorarioTrabalhoPadrao = HorasTrabalhadasHorario;

                    Configuracoes_Gerais config = await repoConfig.selectFirst();
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        TimeSpan? Tempo_Execuxao_Padrao = config.Tempo_Execucao_Padrao?.TimeOfDay;
                        List<TecnicoRoteirizar> tecnicos = await roteirizacaoDAO.GetTecnicoPlanejamentoRoteirizacaoSql(config, new RoteirizacaoDiaConf
                        {
                            Data = Convert.ToDateTime(request.Data),
                            horarios_servico = horarios_servico,
                            Alterar_DataHora_SemAgendamentoAniel = true
                        }, request);

                        var statusIncluir = Enum.GetValues(typeof(StatusExecucaoAniel))
                                            .Cast<StatusExecucaoAniel>()
                                            .Where(s => s != StatusExecucaoAniel.Fechada_Improdutiva   // 1
                                                     && s != StatusExecucaoAniel.Fechada_Produtiva      // 2
                                                     && s != StatusExecucaoAniel.Cancelado)             // 9
                                            .Select(s => (int)s)
                                            .ToList();

                        var parametrosIn = statusIncluir.Select(i => $"{i}").ToList();

                        StringBuilder sql = new StringBuilder(@"SELECT A.NUM_OBRA,A.PROJETO,A.CODCT, A.NUM_OBRA_ORIGINAL,A.NUM_DOC,B.DESCRICAO AS ESCRITORIO,A.EQUIPE,
                                                                A.DATA_MAXIMA,A.HORA_MAXIMA,C.DESCRICAO AS AREA,D.LOCALIDADE, A.COD_ESCRITORIO,A.COD_AREA,A.COD_LOCALIDADE,
                                                                L.TIPO_HABILIDADE,K.NOME AS SUPERVISOR,M.USUARIO_MONITOR,M.COD_SUPERV,Q.NOME AS MONITOR,S.DESLOCAMENTO,S.DATAHORA_INICIO,S.DATAHORA_FIM

                                                                FROM TB_DOCUMENTO_PRODUCAO A
                                                                LEFT JOIN TB_ESCRITORIO B ON(A.COD_ESCRITORIO = B.COD_ESCRITORIO)
                                                                LEFT JOIN TB_AREA C ON(A.COD_ESCRITORIO = C.COD_ESCRITORIO AND A.COD_AREA = C.COD_AREA)
                                                                LEFT JOIN TB_LOCALIDADE D ON(A.COD_ESCRITORIO = D.COD_ESCRITORIO AND A.COD_AREA = D.COD_AREA AND A.COD_LOCALIDADE = D.COD_LOCALIDADE)
                                                                LEFT JOIN TB_TIPO_SERVICO_EQUIPE P
                                                                ON(A.COD_TIPO_SERV = P.COD_TIPO_SERV)
                                                                LEFT JOIN TB_NATUREZA_PRODUCAO L
                                                                ON(L.ID = P.COD_NATUREZA) 
                                                                LEFT JOIN TB_EQUIPE M 
                                                                ON(A.EQUIPE = M.EQUIPE) 
                                                                LEFT JOIN TB_FUNCIONARIO K 
                                                                ON(K.NUM_FRE = M.COD_SUPERV)
                                                                LEFT JOIN TB_SENHA Q
                                                                ON(Q.USUARIO = M.USUARIO_MONITOR)
                                                                LEFT JOIN TB_AGENDAMENTO_EQUIPEANIEL S
                                                                ON(A.NUM_OBRA = S.NUM_OBRA AND A.PROJETO = S.PROJETO AND A.CODCT = S.CODCT AND A.NUM_DOC = S.NUM_DOC) ");


                        List<String> condicoes = new List<string>();
                        condicoes.Add($"((A.STATUS_EXECUCAO IN ({string.Join(", ", parametrosIn)})) OR (A.DATA_PROD = @DATA AND A.STATUS_EXECUCAO IN (1,2)))");




                        if (String.IsNullOrEmpty(request.DataInicioDemanda))
                        {
                            condicoes.Add($"((A.DATA_MAXIMA <= @DATA) {(request.BuscarServicosSemAgendamento == "S" ? "OR(A.DATA_MAXIMA IS NULL)" : "")})");
                            parametros.Add("@DATA", request.Data);
                        }
                        else
                        {
                            condicoes.Add($"((A.DATA_MAXIMA BETWEEN @DATA_INICIO_DEMANDA AND @DATA) {(request.BuscarServicosSemAgendamento == "S" ? "OR(A.DATA_MAXIMA IS NULL)" : "")})");
                            parametros.Add("@DATA", request.Data);
                            parametros.Add("@DATA_INICIO_DEMANDA", request.DataInicioDemanda);
                        }



                        if (String.IsNullOrEmpty(request.Contrato) == false)
                        {
                            condicoes.Add("A.CODCT = @CODCT");
                            parametros.Add("@CODCT", request.Contrato);
                        }
                        if (String.IsNullOrEmpty(request.Projeto) == false)
                        {
                            condicoes.Add("A.PROJETO = @PROJETO");
                            parametros.Add("@PROJETO", request.Projeto);
                        }
                        if (String.IsNullOrEmpty(request.Equipe) == false)
                        {
                            condicoes.Add("A.EQUIPE = @EQUIPE");
                            parametros.Add("@EQUIPE", request.Equipe);
                        }
                        if (String.IsNullOrEmpty(request.GrupoArea) == false)
                        {
                            condicoes.Add("A.COD_ESCRITORIO = @COD_ESCRITORIO");
                            parametros.Add("@COD_ESCRITORIO", request.GrupoArea);
                        }
                        if (String.IsNullOrEmpty(request.Area) == false)
                        {
                            condicoes.Add("A.COD_AREA = @AREA");
                            parametros.Add("@AREA", request.Area);
                        }
                        if (String.IsNullOrEmpty(request.NaturezaServico) == false)
                        //if (request.NaturezaServico.Count != 0)
                        {
                            //condicoes.Add("O.ID IN @NATUREZA");
                            //parametros.Add("@NATUREZA", request.Natureza);
                            condicoes.Add($"(O.ID IN ({String.Join(", ", request.NaturezaServico)}))");
                        }
                        if (String.IsNullOrEmpty(request.TipoServico) == false)
                        {
                            condicoes.Add("A.COD_TIPO_SERV = @COD_TIPO_SERV");
                            parametros.Add("@COD_TIPO_SERV", request.TipoServico);
                        }
                        if (String.IsNullOrEmpty(request.Localidade) == false)
                        {
                            condicoes.Add("R.LOCALIDADE = @LOCALIDADE");
                            parametros.Add("@LOCALIDADE", request.Localidade);
                        }


                        if (condicoes.Count > 0)
                        {
                            sql.AppendLine(" WHERE ");
                            sql.AppendLine(String.Join(" AND ", condicoes));
                        }




                        //Busca com paginacao
                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                var tipoHabilidade = reader["TIPO_HABILIDADE"] != DBNull.Value ? (TipoHabilidade?)Enum.Parse(typeof(TipoHabilidade), Convert.ToString(reader["TIPO_HABILIDADE"])) : (TipoHabilidade?)null;
                                if (tipoHabilidade.HasValue == false)
                                    continue;

                                var deslocamento = reader["DESLOCAMENTO"] != DBNull.Value ? Convert.ToInt16(reader["DESLOCAMENTO"]) : config.Tempo_Medio_Deslocamento.Value.TimeOfDay.TotalMinutes;
                                var tempoExecucao = reader["DATAHORA_INICIO"] != DBNull.Value && reader["DATAHORA_FIM"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_FIM"]).Subtract(Convert.ToDateTime(reader["DATAHORA_INICIO"])) : Tempo_Execuxao_Padrao.Value;
                                //tempoExecucao = tempoExecucao.Add(TimeSpan.FromMinutes(deslocamento));
                                retorno.servicos.Add(new DocumentoPlanejamento
                                {
                                    Num_Obra = Convert.ToString(reader["NUM_OBRA"]),
                                    CodCt = Convert.ToString(reader["CODCT"]),
                                    Projeto = Convert.ToString(reader["PROJETO"]),
                                    Num_Doc = reader["NUM_DOC"] != DBNull.Value ? Convert.ToInt32(reader["NUM_DOC"]) : (Int32?)null,
                                    Data_Maxima = reader["DATA_MAXIMA"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_MAXIMA"]) : (DateTime?)null,
                                    Hora_Maxima = reader["HORA_MAXIMA"] != DBNull.Value ? DateTime.Today.Add((TimeSpan)reader["HORA_MAXIMA"]) : (DateTime?)null,
                                    DataMaximaStr = reader["DATA_MAXIMA"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_MAXIMA"]).ToString("dd/MM/yyyy") : null,
                                    HoraMaximaStr = reader["HORA_MAXIMA"] != DBNull.Value ? DateTime.Today.Add((TimeSpan)reader["HORA_MAXIMA"]).ToString("HH:mm") : null,
                                    Equipe = Convert.ToString(reader["EQUIPE"]),
                                    Num_Obra_Original = Convert.ToString(reader["NUM_OBRA_ORIGINAL"]),
                                    Cod_Escritorio = Convert.ToString(reader["COD_ESCRITORIO"]),
                                    Cod_Area = Convert.ToString(reader["COD_AREA"]),
                                    Cod_Localidade = reader["COD_LOCALIDADE"] != DBNull.Value ? Convert.ToInt32(reader["COD_LOCALIDADE"]) : (Int32?)null,
                                    LocalidadeStr = Convert.ToString(reader["LOCALIDADE"]),
                                    Descricao_Grupo_Area = Convert.ToString(reader["ESCRITORIO"]),
                                    Descricao_Area = Convert.ToString(reader["AREA"]),
                                    TempoExecucaoMinutos = tempoExecucao.TotalMinutes,
                                    Tempo_Execucao = tempoExecucao,
                                    TipoHabilidade = reader["TIPO_HABILIDADE"] != DBNull.Value ? (TipoHabilidade?)Enum.Parse(typeof(TipoHabilidade), Convert.ToString(reader["TIPO_HABILIDADE"])) : (TipoHabilidade?)null,
                                    NumFre_Supervisor = Convert.ToString(reader["COD_SUPERV"]),
                                    Usuario_Monitor = Convert.ToString(reader["USUARIO_MONITOR"]),
                                    NomeSupervisor = Convert.ToString(reader["SUPERVISOR"]),
                                    NomeMonitor = Convert.ToString(reader["MONITOR"]),
                                    DeslocamentoMinutos = (short)deslocamento,

                                });
                            }
                        }

                        retorno.tecnicos = tecnicos;


                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                return retorno;
            }

            return retorno;

        }
        [HttpPost]
        [Route("api/PlanejamentoCapacidadeRoteirizacaoApiController/SalvarAjustePerfil")]
        public async Task<PostApiRetorno> SalvarAjustePerfil(SalvarAjustePerfilRequest request)
        {

            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repoHorarioEquipe = new Conexao<Horario_Equipe>(sessionT5))
                using (var repoHorarioConnect = new Conexao<Horario_Connect>(sessionT5))
                using (var repoEquipe = new Conexao<EquipeAniel>(sessionT5))
                using (var repoPerfilAgendamento = new Conexao<PerfilAgendamento>(sessionT5))
                {
                    DateTime data = Convert.ToDateTime(request.Data);
                    List<Horario_Connect> horarios_servico = (repoHorarioConnect.select(p => p.Tipo == Aniel.T5.Model.enumerado.Tipo_Hora_Connect.Horario_Servico && p.Status == Sinapse.Model.enumerados.Status_Default.Ativo).Result).ToList();
                    var horario_padrao = horarios_servico.Find(p => p.Padrao == true);
                    foreach (var item in request.lista)
                    {
                        var equipe = await repoEquipe.selectFirst(p => p.Matricula == item.Equipe);
                        var perfil = await repoPerfilAgendamento.selectFirst(p => p.Id == item.IdPerfilAgendamento);
                        var horario = await repoHorarioEquipe.selectFirst(p => p.Equipe == equipe && p.Data == data);

                        if (horario == null)
                        {
                            horario = new Horario_Equipe
                            {
                                Data = data,
                                Equipe = equipe,
                                Horario = horario_padrao,
                                Indisponivel = false,
                                ApenasNoDia = true,
                                Status = Status_Default.Ativo,
                                UsuarioCriacao = request.Usuario
                            };
                        }

                        horario.PerfilAgendamento = perfil;
                        await repoHorarioEquipe.SaveOrUpdate(horario);

                    }

                    return new PostApiRetorno
                    {
                        Ok = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new PostApiRetorno
                {
                    Ok = false,
                    Mensagem = ex?.Message,
                    InnerException = ex?.InnerException?.Message
                };
            }

        }
        [HttpPost]
        [Route("api/PlanejamentoCapacidadeRoteirizacaoApiController/GestaoOperacional")]
        public async Task<GestaoCapacidadeRoteirizacaoResponse> GestaoOperacional(GestaoCapacidadeRoteirizacaoRequest request)
        {
            GestaoCapacidadeRoteirizacaoResponse retorno = new GestaoCapacidadeRoteirizacaoResponse();
            try

            {
                using (var roteirizacaoDAO = new RoteirizacaoDAO())
                using (ISession session = HibernateUtil.GetSession())
                using (Conexao<Configuracoes_Gerais> conexaoConfig = new Conexao<Configuracoes_Gerais>(session))
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                using (var repoHorario = new Conexao<Horario_Connect>(sessionT5))
                using (var horarioApi = new HorarioApiController())
                {
                    List<Horario_Connect> horarios_servico = (repoHorario.select(p => p.Tipo == Aniel.T5.Model.enumerado.Tipo_Hora_Connect.Horario_Servico && p.Status == Sinapse.Model.enumerados.Status_Default.Ativo).Result).ToList();
                    var horario_padrao = horarios_servico.Find(p => p.Padrao == true);
                    var horarioDia = horarioApi.RetornaHorarioDia(horario_padrao, Convert.ToDateTime(request.Data), false);
                    var HorasTrabalhadasHorario = horarioApi.MinutosTrabalhadosHorario(horarioDia.inicio, horarioDia.inicio_intervalo, horarioDia.termino_intervalo, horarioDia.termino);
                    retorno.minutosHorarioTrabalhoPadrao = HorasTrabalhadasHorario;

                    Configuracoes_Gerais config = await conexaoConfig.selectFirst();
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        TimeSpan? Tempo_Execuxao_Padrao = config.Tempo_Execucao_Padrao?.TimeOfDay;
                        List<TecnicoRoteirizar> tecnicos = await roteirizacaoDAO.GetTecnicoPlanejamentoRoteirizacaoSql(config, new RoteirizacaoDiaConf
                        {
                            Data = Convert.ToDateTime(request.Data),
                            horarios_servico = horarios_servico,
                            Alterar_DataHora_SemAgendamentoAniel = true
                        }, request);

                        IDocumentoProducaoAnielRepository repoDocProd = new DocumentoProducaoAnielRepository(sessionT5Nh, sessionT5);
                        IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(session);
                        var businessProdutividadeTecnico = new PainelGestaoOperacionalAnielBusiness(repoDocProd, repoConfig);
                        var resultadoServicos = businessProdutividadeTecnico.GetServicos(request);
                        retorno.servicos = resultadoServicos.Data.ToList();
                        retorno.tecnicos = tecnicos;
                    }
                }
            }
            catch (Exception)
            {
                //           --      Pro Tip         --
                //  --    Use Shift + F9 para debuggar o erro     --
                return retorno;
            }

            return retorno;

        }

        [HttpPost]
        [Route("api/PlanejamentoCapacidadeRoteirizacaoApiController/GetExtratoAnalitico")]
        public DataTableResponse<RelProdTecnicoExtratoProdutividadeListModel> GetExtratoAnalitico(GestaoCapacidadeRoteirizacaoRequest request)
        {
            using (ISession sessionNh = HibernateUtil.GetSession())
            using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
            using (ISession sessionT5Sql = HibernateUtil.GetSessionT5())
            {
                IDocumentoProducaoAnielRepository repoDocProd = new DocumentoProducaoAnielRepository(sessionT5Nh, sessionT5Sql);
                IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(sessionNh);
                var business = new PainelGestaoOperacionalAnielBusiness(repoDocProd, repoConfig);

                GestaoOperacionalServicosResponse extrato = business.GetServicos(request);
                var dados = extrato.Data.Select(p => new RelProdTecnicoExtratoProdutividadeListModel
                {
                    NumObra = p.Num_Obra,
                    Codct = p.CodCt,
                    Projeto = p.Projeto,
                    NumDoc = Convert.ToInt32(p.Num_Doc),
                    DataHoraAgendamento = String.IsNullOrEmpty(p.HoraMaximaStr) ? p.DataMaximaStr : p.DataMaximaStr + " " + p.HoraMaximaStr,
                    DataHoraBaixa = p.Data_hora_Encerramento.HasValue ? p.Data_hora_Encerramento.Value.ToString("dd/MM/yyyy HH:mm") : null,
                    Supervisor = p.NomeSupervisor,
                    Monitor = p.NomeMonitor,
                    GrupoArea = p.Descricao_Grupo_Area,
                    Area = p.Descricao_Area,
                    Localidade = p.LocalidadeStr,
                    DescricaoEncerramento = p.DescricaoEncerramento,
                    CodigoEncerramento = p.CodEncerramento,
                    Status = HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(p.Status.ToString()),
                    Abertura_OS = p.Data_hora_Criacao.HasValue ? p.Data_hora_Criacao.Value.ToString("dd/MM/yyyy HH:mm") : null,
                    TipoServico = p.Descricao_Tipo_Serv,
                    Equipe = p.Equipe,
                    Nome = p.NomeTecnico,
                    Natureza = p.Descricao_Natureza,
                    SituacaoPrazo = p.DentroPrazo.HasValue ? RsCadastros.DENTRO_PRAZO : RsCadastros.FORA_PRAZO,
                    Status_Auditoria = HtmlEnumExtensions.GetEnumDescription<StatusAuditoria>(p.StatusAuditoria.ToString()),
                    UsuarioBaixa = p.UsuarioBaixa,
                    DataHoraAberturaOriginal = p.DataHoraAberturaOriginal,
                    NumeroCliente = p.OXBDPEE
                }).ToList();

                return new DataTableResponse<RelProdTecnicoExtratoProdutividadeListModel>
                {
                    draw = request.draw,
                    recordsFiltered = extrato.Total,
                    data = dados
                };
            }
        }

    }
}