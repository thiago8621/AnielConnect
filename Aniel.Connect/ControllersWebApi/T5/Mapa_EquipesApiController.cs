using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.T5.DocumentoProducao;
using Aniel.Connect.Repository.T5.Equipe;
using Aniel.Connect.Repository.T5.TipoServico;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.mongo;
using Aniel.T5.Model.nhibernate;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using NHibernate;
using Resources;
using RestSharp.Validation;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.repository;
using Sinapse.DAO.Repository;
using Sinapse.Integracao.Rastreamento.Cobli.Models;
using Sinapse.Integracao.Rastreamento.Integracoes;
using Sinapse.Integracao.SMS.Interface;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebGrease.Css.Extensions;

namespace Aniel.Connect.ControllersWebApi.T5
{
    [Authorize]
    public class Mapa_EquipesApiController : BaseApiController
    {

        public async Task<GetApiRetorno<Equipe_MapaItemModel>> GetEquipeNoMapa(String Equipe, DateTime? Data)
        {
            GetApiRetorno<Equipe_MapaItemModel> retorno = new GetApiRetorno<Equipe_MapaItemModel>();

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (ISession sessionNH = HibernateUtil.GetSessionT5())
                using (ISession session_connect = HibernateUtil.GetSession())
                using (var repositoryConfig = new Conexao<Configuracoes_Gerais>(session_connect))
                {
                    var configGerais = await repositoryConfig.selectFirst();
                    ITipoServicoAnielRepository repoTipoServico = new TipoServicoAnielRepository(sessionNH,session);
                    IDocumentoProducaoAnielRepository repoDocumento = new DocumentoProducaoAnielRepository(sessionNH, session);
                    IEquipeAnielRepository repoEquipe = new EquipeAnielRepository(session);
                    EquipeAnielBusiness equipeAnielBusiness = new EquipeAnielBusiness(repoEquipe, repoTipoServico, repoDocumento);

                    Ponto_Partida ponto_partida = null;
                    using (var repositoryPonto = new Conexao<EquipeAniel>(session))
                    {
                        var equipe = (await repositoryPonto.selectFirst(p => p.Matricula == Equipe));

                        if (equipe != null)
                            ponto_partida = equipe.Ponto_Partida;
                    }

                    using (var repository = new Conexao<EquipeStatusAniel>(session))
                    using (var repositoryEquipe = new Conexao<EquipeAniel>(session))
                    using (var repositoryHorarioEquipe = new Conexao<Horario_Equipe>(session))
                    using (Agendamento_EquipeApiController agendamentoApi = new Agendamento_EquipeApiController())
                    using (EquipeApiController equipeApi = new EquipeApiController())
                    using (var repositoryIntegracoesExternas = new Conexao<IntegracoesExternas>(session_connect))
                    {

                        var integracoesExternas = (await repositoryIntegracoesExternas.select()).ToList();

                        if (Data.HasValue == false)
                            Data = DateTime.Now;
                        List<Feriado> feriados = (await new Conexao<Feriado>(session_connect).select(p => p.Data == Data && p.Status == Status_Default.Ativo)).ToList();
                        List<Horario_Connect> horarios_Connect = (await new Conexao<Horario_Connect>(session).select(p => p.Tipo == Tipo_Hora_Connect.Horario_Servico)).ToList();
                        List<Horario_Equipe> horarios_equipe = (await repositoryHorarioEquipe.select(p => p.Equipe.Matricula == Equipe && ((p.ApenasNoDia == true && p.Data == Data) || (p.ApenasNoDia == false && p.Data == Data)))).ToList();
                        Horario_Connect horario_ConnectPadrao = horarios_Connect.Find(p => p.Padrao == true);
                        Horario_Connect Horario_Connect = horario_ConnectPadrao;
                        IntegracaoRastreamento integracaoRastreamento = null;

                        if (integracoesExternas.Find(p => p.IntegracaoExterna == Model.enumerado.IntegracaoExterna.COBLI) != null)
                        {
                            var integracaoCobli = integracoesExternas.Find(p => p.IntegracaoExterna == Model.enumerado.IntegracaoExterna.COBLI);
                            integracaoRastreamento = new Cobli(integracaoCobli.TokenAcesso);
                        }


                        if (horarios_equipe.Count > 0)
                            Horario_Connect = horarios_equipe.OrderBy(p => p.Data).LastOrDefault()?.Horario ?? horario_ConnectPadrao;

                        DayOfWeek diaSemana = Data.Value.DayOfWeek;

                        bool feriado = feriados.Find(p => p.Data == Data.Value.Date) != null;



                        var _funcionarioObj = new FuncionarioAniel();

                        using (var com = session.Connection.CreateCommand())
                        {
                            Dictionary<String, Object> parametros = new Dictionary<string, object>();
                            StringBuilder sql = new StringBuilder(" SELECT B.TEL_CELULAR, B.TELEFONE ,B.EMAIL ")
                                                       .AppendLine(" FROM TB_EQUIPE A ")
                                                       .AppendLine(" LEFT JOIN TB_FUNCIONARIO B ")
                                                       .AppendLine(" ON A.NUM_FRE = B.NUM_FRE ")
                                                       .AppendLine(" WHERE A.EQUIPE = @EQUIPE ");

                            parametros.Add("@EQUIPE", Equipe);

                            using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                            {
                                while (await reader.ReadAsync())
                                {

                                    if (reader["TEL_CELULAR"] != DBNull.Value && !String.IsNullOrEmpty(Convert.ToString(reader["TEL_CELULAR"])))
                                    {
                                        _funcionarioObj.Telefone_Celular = Convert.ToString(reader["TEL_CELULAR"]);
                                    }
                                    else if (reader["TELEFONE"] != DBNull.Value && !String.IsNullOrEmpty(Convert.ToString(reader["TELEFONE"])))
                                    {
                                        _funcionarioObj.Telefone_Celular = Convert.ToString(reader["TELEFONE"]);
                                    }
                                    else
                                    {
                                        _funcionarioObj.Telefone_Celular = null;
                                    }

                                    _funcionarioObj.Email = Convert.ToString(reader["EMAIL"]);
                                }
                            }
                        }

                        if (String.IsNullOrEmpty(_funcionarioObj.Telefone_Celular)) _funcionarioObj.Telefone_Celular = Resources.RsBase.NAOCADASTRADO;
                        if (String.IsNullOrEmpty(_funcionarioObj.Email)) _funcionarioObj.Email = Resources.RsBase.NAOCADASTRADO;

                        if (ponto_partida == null)
                        {
                            var lista_sem_ponto_partida = (await repository.select(p => p.Equipe.Matricula == Equipe)).Select(p =>
                            {
                                var horarioEquipeTecnico = horarios_equipe.OrderBy(p => p.Data).LastOrDefault();
                                var tipo = equipeAnielBusiness.GetStatusPersonalizado(DateTime.Now.Date,
                                                                                      horario_ConnectPadrao,
                                                                                      feriado,
                                                                                      horarioEquipeTecnico,
                                                                                      p.Tipo.Value,
                                                                                      horarioEquipeTecnico?.PerfilAgendamento ?? p.Equipe?.PerfilAgendamento,
                                                                                      Equipe,
                                                                                      configGerais.Tempo_Execucao_Padrao.Value.TimeOfDay);

                                return new Equipe_MapaItemModel
                                {
                                    Equipe_Nome = p.Equipe.Nome,
                                    Num_Fre = p.Equipe.Num_Fre,
                                    Atualizado_Em = p.DataHoraCriacao.HasValue ? p.DataHoraCriacao.Value.ToString("dd/MM/yyyy HH:mm") : String.Empty,
                                    Status_App = p.DataHoraAlteracao.HasValue ? p.DataHoraAlteracao.Value.ToString("dd/MM/yyyy HH:mm") : String.Empty,
                                    Bateria = Convert.ToString(p.Bateria) + "%",
                                    Logado = p.Logado != null ? (p.Logado.Value ? "LOGADO" : "DESLOGADO") : String.Empty,
                                    DataHora_Login = p.DataHora_Login != null ? Convert.ToDateTime(p.DataHora_Login).ToString("dd/MM/yyyy HH:mm") : String.Empty,
                                    Dispositivo = p.Dispositivo + "%",
                                    Justificativa_Ausencia = p.Justificativa_Ausencia,
                                    Endereco = String.IsNullOrEmpty(p.Logradouro) ? String.Empty : String.Format("{0} nº {1}, {2} {3}", p.Logradouro, p.Numero, p.Bairro, p.Cidade),
                                    IMEI = p.IMEI,
                                    InternetMovel = p.InternetMovel,
                                    Precisao = String.Format("{0:N2}", p.Precisao),
                                    Tipo = tipo == EquipeStatusTipo.Em_Intervalo ? Resources.RsCadastros.INDISPONIVEL : HtmlEnumExtensions.GetEnumDescription<EquipeStatusTipo>(Convert.ToString((int)tipo)),
                                    Versao_Android = p.Versao_Android,
                                    Versao_App = p.Versao_App,
                                    Telefone = _funcionarioObj.Telefone_Celular,
                                    Email = _funcionarioObj.Email,
                                    Cor_StatusEquipe_Mapa = EquipeApiController.GetCorStatusEquipe(tipo),
                                };
                            }).ToList();

                            if (lista_sem_ponto_partida.Count() > 0)
                                retorno.Objeto = lista_sem_ponto_partida[0];
                            else
                            {
                                EquipeAniel equipe = await repositoryEquipe.selectFirst(p => p.Matricula == Equipe);
                                retorno.Objeto = new Equipe_MapaItemModel
                                {
                                    Bateria = Resources.RsBase.NAOENCONTRADO,
                                    Dispositivo = Resources.RsBase.NAOENCONTRADO,
                                    Endereco = Resources.RsBase.NAOENCONTRADO,
                                    Atualizado_Em = Resources.RsBase.NAOENCONTRADO,
                                    Equipe_Nome = equipe.Nome,
                                    Num_Fre = equipe.Num_Fre,
                                    Telefone = _funcionarioObj.Telefone_Celular,
                                    Email = _funcionarioObj.Email,
                                    Rotas_Planejadas = new List<Equipe_MapaModel_Planejado>(),
                                    Rotas_Realizada = new List<Equipe_MapaModel_Realizada>(),
                                    Rotas_Integracao = new List<Equipe_MapaModel_Realizada>(),
                                    Rotas_Paradas = new List<Equipe_MapaModel_Realizada>()
                                };
                            }

                            throw new Exception(Resources.RsCadastros.EQUIPE_SEM_PONTO_PARTIDA);

                        }
                        var lista = (await repository.select(p => p.Equipe.Matricula == Equipe)).Select(p =>
                        {

                            TrajetoRealizadoResponseModel trajetoRealizado = integracaoRastreamento != null ? integracaoRastreamento.GetTrajeto(Data.Value, Data.Value.Date.AddHours(23), p.Equipe.Nome) : null;
                            List<Equipe_MapaModel_Realizada> dadosIntegracao = trajetoRealizado != null ? trajetoRealizado.Dados.Select(p => new Equipe_MapaModel_Realizada
                            {
                                Latitude = p.Latitude,
                                Longitude = p.Longitude,
                                DataHoraStr = p.DataHoraStr
                            }).ToList() : new List<Equipe_MapaModel_Realizada>();

                            List<Equipe_MapaModel_Realizada> dadosParadas = integracaoRastreamento != null ? trajetoRealizado.Paradas.Select(p => new Equipe_MapaModel_Realizada
                            {
                                Latitude = p.Latitude,
                                Longitude = p.Longitude,
                                DataHoraStr = p.DataHoraStr,
                                DataHoraFimStr = p.DataHoraFimStr,
                                Geofence = p.Geofence
                            }).ToList() : new List<Equipe_MapaModel_Realizada>();

                            var horarioEquipeTecnico = horarios_equipe.OrderBy(p => p.Data).LastOrDefault();
                            var tipo = equipeAnielBusiness.GetStatusPersonalizado(DateTime.Now.Date,
                                                                                  horario_ConnectPadrao,
                                                                                  feriado,
                                                                                  horarioEquipeTecnico,
                                                                                  p.Tipo.Value,
                                                                                  horarioEquipeTecnico?.PerfilAgendamento ?? p.Equipe?.PerfilAgendamento,
                                                                                  Equipe,
                                                                                  configGerais.Tempo_Execucao_Padrao.Value.TimeOfDay);

                            var obj = new Equipe_MapaItemModel
                            {
                                Equipe_Nome = p.Equipe.Nome,
                                Num_Fre = p.Equipe.Num_Fre,
                                Atualizado_Em = p.DataHoraCriacao.HasValue ? p.DataHoraCriacao.Value.ToString("dd/MM/yyyy HH:mm") : String.Empty,
                                Status_App = p.DataHoraAlteracao.HasValue ? p.DataHoraAlteracao.Value.ToString("dd/MM/yyyy HH:mm") : String.Empty,
                                Bateria = Convert.ToString(p.Bateria) + "%",
                                Logado = p.Logado != null ? (p.Logado.Value ? "LOGADO" : "DESLOGADO") : String.Empty,
                                DataHora_Login = p.DataHora_Login != null ? Convert.ToDateTime(p.DataHora_Login).ToString("dd/MM/yyyy HH:mm") : String.Empty,
                                Dispositivo = p.Dispositivo + "%",
                                Justificativa_Ausencia = p.Justificativa_Ausencia,
                                Endereco = String.IsNullOrEmpty(p.Logradouro) ? String.Empty : String.Format("{0} nº {1}, {2} {3}", p.Logradouro, p.Numero, p.Bairro, p.Cidade),
                                IMEI = p.IMEI,
                                InternetMovel = p.InternetMovel,
                                Precisao = String.Format("{0:N2}", p.Precisao),
                                Tipo = tipo == EquipeStatusTipo.Em_Intervalo ? Resources.RsCadastros.INDISPONIVEL : HtmlEnumExtensions.GetEnumDescription<EquipeStatusTipo>(Convert.ToString((int)tipo)),
                                Versao_Android = p.Versao_Android,
                                Versao_App = p.Versao_App,
                                Rotas_Planejadas = GetEquipe_MapaModel_Planejado(ponto_partida, p.Equipe.Matricula, Data).Result,
                                Rotas_Realizada = GetEquipe_MapaModel_Realizado(ponto_partida, p.Equipe.Matricula, Data).Result,
                                Rotas_Integracao = dadosIntegracao,
                                Rotas_Paradas = dadosParadas,
                                Telefone = _funcionarioObj.Telefone_Celular,
                                Email = _funcionarioObj.Email,
                                Cor_StatusEquipe_Mapa = EquipeApiController.GetCorStatusEquipe(tipo),
                                DistanciaTotalIntegrada = trajetoRealizado != null ? trajetoRealizado.DistanciaTotalKm.ToString("0.00", new CultureInfo("en-US")) : null
                            };


                            obj.Tipo = tipo == EquipeStatusTipo.Em_Intervalo ? Resources.RsCadastros.INDISPONIVEL : HtmlEnumExtensions.GetEnumDescription<EquipeStatusTipo>(Convert.ToString((int)tipo));
                            return obj;

                        }).ToList();

                        if (lista.Count() > 0)
                            retorno.Objeto = lista[0];
                        else
                        {

                            var lista2 = (await new Conexao<EquipeAniel>(session).select(p => p.Matricula == Equipe)).Select(p => new Equipe_MapaItemModel
                            {
                                Rotas_Planejadas = GetEquipe_MapaModel_Planejado(ponto_partida, p.Matricula, Data).Result,
                                Rotas_Realizada = GetEquipe_MapaModel_Realizado(ponto_partida, p.Matricula, Data).Result,
                                Telefone = _funcionarioObj.Telefone_Celular,
                                Email = _funcionarioObj.Email,
                                Equipe_Nome = p.Nome,
                                Bateria = Resources.RsBase.NAOENCONTRADO,
                                Dispositivo = Resources.RsBase.NAOENCONTRADO,
                                Endereco = Resources.RsBase.NAOENCONTRADO,
                                Atualizado_Em = Resources.RsBase.NAOENCONTRADO,
                                Equipe = p.Matricula,

                            }).ToList();

                            if (lista2.Count() < 1)
                                retorno.Objeto = new Equipe_MapaItemModel
                                {
                                    Rotas_Planejadas = new List<Equipe_MapaModel_Planejado>(),
                                    Rotas_Realizada = new List<Equipe_MapaModel_Realizada>()
                                };

                            else
                                retorno.Objeto = lista2[0];
                        }

                        retorno.Ok = true;
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }

        public String GetCorStatusEquipe(String Status)
        {
            try
            {
                if (String.IsNullOrEmpty(Status))
                    return "#000000";
                var statusEnum = (EquipeStatusTipo)Enum.Parse(typeof(EquipeStatusTipo), Status);

                switch (statusEnum)
                {
                    case EquipeStatusTipo.Disponivel:
                        return "#38761d";
                    case EquipeStatusTipo.Ausente:
                        return "#fb6400";
                    case EquipeStatusTipo.Em_Atendimento:
                        return "#ed1c24";
                    case EquipeStatusTipo.Sem_Agenda:
                        return "#ff001d";
                    case EquipeStatusTipo.Em_Deslocamento:
                        return "#3d85c6";
                    case EquipeStatusTipo.Em_Intervalo:
                        return "#7f7f7f";

                    default:
                        return "#000000";
                }
            }
            catch
            {
                return "#000000";
            }
        }

        private async Task<List<Equipe_MapaModel_Planejado>> GetEquipe_MapaModel_Planejado(Ponto_Partida Ponto_Partida, String Equipe, DateTime? Data)
        {
            Dictionary<String, Object> parametros = new Dictionary<string, object>();

            List<Equipe_MapaModel_Planejado> retorno = new List<Equipe_MapaModel_Planejado>();

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (ConexaoMongo conexaoMongo = new ConexaoMongo())
            using (var repository = new RepositoryMongo<DocumentoProducaoMongo>(conexaoMongo))
            {
                Boolean Exibe_RDOs = (await new Conexao<Configuracoes_Gerais>(session).selectFirst()).ExibeRDOs;

                //Count
                using (var com = sessionT5.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder(" SELECT B.CODCT,B.PROJETO,B.NUM_DOC,B.NUM_OBRA,B.DATAHORA_FIM_ATEND,B.DIA + B.HORA AS DATAHORA_CRIACAO,B.DATA_INDICADOR + B.HORA_INDICADOR AS DATAHORA_INDICADOR, B.DATAHORA_INI_ATEND, B.DATAHORA_FIM_ATEND, B.STATUS_EXECUCAO, B.NUM_OBRA_ORIGINAL, B.LATITUDE_STR, B.LONGITUDE_STR, B.DATA_MAXIMA, B.HORA_MAXIMA,C.LATITUDE AS LATITUDE_LOCALIDADE,C.LONGITUDE AS LONGITUDE_LOCALIDADE,R.LATITUDE AS CX_LATITUDE, R.LONGITUDE AS CX_LONGITUDE ")
                                               .AppendLine(" FROM TB_DOCUMENTO_PRODUCAO B ")
                                               .AppendLine(" LEFT JOIN TB_LOCALIDADE C ON (B.COD_ESCRITORIO = C.COD_ESCRITORIO AND B.COD_AREA = C.COD_AREA AND B.COD_LOCALIDADE = C.COD_LOCALIDADE) ")
                                               .AppendLine("LEFT JOIN TB_ELEMENTO_REDE R")
                                               .AppendLine("ON(R.ID = B.CAIXA_FTTH) ")
                                               .AppendLine(" WHERE B.EQUIPE = @EQUIPE AND B.DATA_MAXIMA = @DATA ");
                    if (!Exibe_RDOs)
                        sql.AppendLine(" AND COALESCE(B.SIGLA_MODULO, '') <> 'MCO' ");

                    parametros.Add("@EQUIPE", Equipe);
                    if (Data == null)
                    {
                        parametros.Add("@DATA", DateTime.Now);
                    }

                    else
                    {
                        parametros.Add("@DATA", Data.Value.ToString("dd/MM/yyyy"));
                    }

                    if (Ponto_Partida != null)
                    {
                        retorno.Add(new Equipe_MapaModel_Planejado
                        {
                            Ponto_Partida = Ponto_Partida.Descricao,
                            Latitude = Ponto_Partida.Latitude,
                            Longitude = Ponto_Partida.Longitude
                        });
                    }

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        var doc_api = new Documento_ProducaoApiController();

                        while (await reader.ReadAsync())
                        {
                            string codCt = Convert.ToString(reader["CODCT"]);
                            string projeto = Convert.ToString(reader["PROJETO"]);
                            int numDoc = Convert.ToInt32(reader["NUM_DOC"]);
                            string numObra = Convert.ToString(reader["NUM_OBRA"]);

                            var latitudeOS = reader["LATITUDE_STR"] != DBNull.Value && !String.IsNullOrEmpty(Convert.ToString(reader["LATITUDE_STR"])) ? Convert.ToDouble(reader["LATITUDE_STR"].ToString().Replace(".", ",")) : (Double?)null;
                            var longitudeOS = reader["LONGITUDE_STR"] != DBNull.Value && !String.IsNullOrEmpty(Convert.ToString(reader["LONGITUDE_STR"])) ? Convert.ToDouble(reader["LONGITUDE_STR"].ToString().Replace(".", ",")) : (Double?)null;
                            var latitudeLocalidade = reader["LATITUDE_LOCALIDADE"] != DBNull.Value && !String.IsNullOrEmpty(Convert.ToString(reader["LATITUDE_LOCALIDADE"])) ? Convert.ToDouble(reader["LATITUDE_LOCALIDADE"].ToString().Replace(".", ",")) : (Double?)null;
                            var longitudeLocalidade = reader["LONGITUDE_LOCALIDADE"] != DBNull.Value && !String.IsNullOrEmpty(Convert.ToString(reader["LONGITUDE_LOCALIDADE"])) ? Convert.ToDouble(reader["LONGITUDE_LOCALIDADE"].ToString().Replace(".", ",")) : (Double?)null;
                            var latitudeCaixa = reader["CX_LATITUDE"] != DBNull.Value && !String.IsNullOrEmpty(Convert.ToString(reader["CX_LATITUDE"])) ? Convert.ToDouble(reader["CX_LATITUDE"].ToString().Replace(".", ",")) : (Double?)null;
                            var longitudeCaixa = reader["CX_LONGITUDE"] != DBNull.Value && !String.IsNullOrEmpty(Convert.ToString(reader["CX_LONGITUDE"])) ? Convert.ToDouble(reader["CX_LONGITUDE"].ToString().Replace(".", ",")) : (Double?)null;

                            var obj = new Equipe_MapaModel_Planejado
                            {
                                Data_Ini_Atendimento = reader["DATAHORA_INI_ATEND"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_INI_ATEND"]).ToString("dd/MM/yyyy HH:mm") : "-",
                                Data_Fim_Atendimento = reader["DATAHORA_FIM_ATEND"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_FIM_ATEND"]).ToString("dd/MM/yyyy HH:mm") : "-",
                                Num_Obra = Convert.ToString(reader["NUM_OBRA_ORIGINAL"]),
                                Latitude = latitudeOS ?? (latitudeCaixa ?? latitudeLocalidade),
                                Longitude = longitudeOS ?? (longitudeCaixa ?? longitudeLocalidade),
                                StatusExecucaoAniel = (StatusExecucaoAniel)Enum.Parse(typeof(StatusExecucaoAniel), reader["STATUS_EXECUCAO"].ToString()),
                                StatusExecucaoAniel_Str = HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(Convert.ToString(reader["STATUS_EXECUCAO"]))
                            };

                            if (obj.StatusExecucaoAniel == StatusExecucaoAniel.Fechada_Produtiva ||
                               obj.StatusExecucaoAniel == StatusExecucaoAniel.Fechada_Improdutiva ||
                               obj.StatusExecucaoAniel == StatusExecucaoAniel.Atendimento_Finalizado)
                            {
                                var _collection = repository.GetCollection();

                                var mongoDocumento = _collection
                                    .Aggregate()
                                    .Match(p => p.codCt == codCt
                                    && p.numObra == numObra
                                    && p.contrato == numDoc
                                    && p.projeto == projeto)
                                    .ToList().FirstOrDefault();

                                if (mongoDocumento != null)
                                {
                                    StatusDocumentoProducao statusBaixado = mongoDocumento.status.FindAll(p => p.statusExecucao == (int?)StatusExecucaoAniel.Fechada_Produtiva ||
                                                                                                               p.statusExecucao == (int?)StatusExecucaoAniel.Fechada_Improdutiva ||
                                                                                                               p.statusExecucao == (int?)StatusExecucaoAniel.Atendimento_Finalizado).OrderByDescending(p => p.hora)
                                                                                                  .FirstOrDefault();

                                    if (statusBaixado != null && statusBaixado.latitude != 0 && statusBaixado.longitude != 0)
                                    {
                                        var distanciaKM = MapaService.GetMenorDistancia(obj.Latitude, obj.Longitude, statusBaixado.latitude, statusBaixado.longitude) / 1852;
                                        var distanciaMetros = distanciaKM * 1000;
                                        if (distanciaMetros > 1000)
                                        {
                                            obj.LatitudeBaixa = statusBaixado.latitude;
                                            obj.LongitudeBaixa = statusBaixado.longitude;
                                            obj.DataHoraBaixa = statusBaixado.hora.ToString("dd/MM/yyyy HH:mm");
                                            obj.BaixaNoLocal = RsCadastros.BAIXA_FORA_LOCAL_SERVICO;
                                        }
                                        else
                                        {
                                            obj.BaixaNoLocal = RsCadastros.BAIXA_LOCAL_SERVICO;
                                        }
                                    }
                                    else
                                    {
                                        obj.BaixaNoLocal = RsCadastros.LOCALIZACAO_BAIXA_NAO_ENCONTRADA;
                                    }
                                }
                                else
                                {
                                    obj.BaixaNoLocal = RsCadastros.LOCALIZACAO_BAIXA_NAO_ENCONTRADA;
                                }



                            }


                            if (latitudeOS.HasValue)
                            {
                                obj.TipoLocalizacao = "servico";
                            }
                            else if (latitudeCaixa.HasValue)
                            {
                                obj.TipoLocalizacao = "caixa";
                            }
                            else if (latitudeLocalidade.HasValue)
                            {
                                obj.TipoLocalizacao = "localidade";
                            }


                            if (reader["DATAHORA_INDICADOR"] != DBNull.Value)
                            {
                                DateTime? data_criacao = reader["DATAHORA_CRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_CRIACAO"]) : (DateTime?)null;
                                DateTime? data_indicador = reader["DATAHORA_INDICADOR"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_INDICADOR"]) : (DateTime?)null;
                                DateTime? data_fechamento = reader["DATAHORA_FIM_ATEND"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_FIM_ATEND"]) : (DateTime?)null;

                                obj.Sla_Servico = doc_api.GetDescricaoSla(data_indicador, data_criacao, data_fechamento);

                            }
                            else
                                obj.Sla_Servico = "Sem Indicador de Sla";


                            if (reader["DATA_MAXIMA"] != DBNull.Value)
                            {
                                obj.DataHoraStr = Convert.ToDateTime(reader["DATA_MAXIMA"]).ToString("dd/MM/yyyy");
                                if (reader["HORA_MAXIMA"] != DBNull.Value)
                                {
                                    obj.DataHoraStr += ' ' + ((TimeSpan)(reader["HORA_MAXIMA"])).ToString(@"hh\:mm");
                                }
                                obj.DataHora = Convert.ToDateTime(obj.DataHoraStr);
                            }
                            else
                            {
                                obj.DataHoraStr = String.Empty;
                                obj.DataHora = DateTime.MinValue;
                            }

                            retorno.Add(obj);
                        }
                    }

                    //if (Ponto_Partida != null)
                    //{
                    //    retorno.Add(new Equipe_MapaModel_Planejado
                    //    {
                    //        Ponto_Partida = Ponto_Partida.Descricao,
                    //        Latitude = Ponto_Partida.Latitude,
                    //        Longitude = Ponto_Partida.Longitude
                    //    });
                    //}

                    return retorno;
                }
            }
        }

        private async Task<List<Equipe_MapaModel_Realizada>> GetEquipe_MapaModel_Realizado(Ponto_Partida Ponto_Partida, String Equipe, DateTime? Data)
        {
            List<Equipe_MapaModel_Realizada> retorno = new List<Equipe_MapaModel_Realizada>();
            List<Equipe_MapaModel_Realizada> temp = new List<Equipe_MapaModel_Realizada>();
            using (ISession session = HibernateUtil.GetSession())
            using (ConexaoMongo conexaoMongo = new ConexaoMongo())
            using (var repository = new RepositoryMongo<LogLocalizacaoDataMongo>(conexaoMongo)) // using (var repository = new RepositoryMongo<LogLocalizacaoMongo>(conexaoMongo))
            {
                try
                {
                    Boolean Exibe_RDOs = (await new Conexao<Configuracoes_Gerais>(session).selectFirst()).ExibeRDOs;
                    var dateFimDia = Data.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                    LogLocalizacaoDataMongo logLocalizacao = repository.GetQueryable().Where(x => x.Equipe == Equipe &&
                                                                                                  x.DataDocumento >= Data.Value.Date &&
                                                                                                  x.DataDocumento < dateFimDia).SingleOrDefault();

                    if (logLocalizacao != null)
                    {
                        foreach (var item in logLocalizacao.localizacoes)
                        {
                            var Aparelho = item.Aparelho;
                            var modelo_aparelho = Aparelho.Modelo;
                            var datahora = item.DataHora.Value.ToLocalTime();

                            if (!String.IsNullOrEmpty(modelo_aparelho))
                            {
                                temp.Add(new Equipe_MapaModel_Realizada()
                                {
                                    DataHora = datahora,
                                    DataHoraStr = datahora.ToString("dd/MM/yyyy HH:mm"),
                                    Latitude = item.latitude,
                                    Longitude = item.longitude
                                });
                            }
                        }
                    }



                    temp = temp.Where(p => (Convert.ToString(p.Latitude).Contains(',') || Convert.ToString(p.Latitude).Contains('.')) &&
                                               (Convert.ToString(p.Longitude).Contains(',') || Convert.ToString(p.Longitude).Contains('.'))).ToList();

                    if (temp.Count() > 0)
                    {
                        Equipe_MapaModel_Realizada ultimaRota = temp.First();
                        Double tolerancia = 100; //metros
                        var primeiroPonto = temp.First();
                        primeiroPonto.Ponto_Partida = Equipe;
                        retorno.Add(primeiroPonto);

                        foreach (var obj in temp)
                        {
                            if (obj.Latitude.HasValue && obj.Longitude.HasValue && ultimaRota.Latitude.HasValue && ultimaRota.Longitude.HasValue)
                            {

                                var distanciaKM = MapaService.GetMenorDistancia(obj.Latitude, obj.Longitude, ultimaRota.Latitude, ultimaRota.Longitude) / 1852;
                                var distanciaMetros = distanciaKM * 1000;

                                if (!String.IsNullOrEmpty(obj.Num_Obra) ||
                                    distanciaMetros > tolerancia)
                                {
                                    retorno.Add(obj);
                                    ultimaRota = obj;
                                }
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                }
            }
            return retorno;
        }

        private String GetCorServico(Object DataProd)
        {
            if (DataProd != DBNull.Value)
                return "#00FF00";
            //return "#text-success";
            else if (DataProd == DBNull.Value)
                return "#FFFF00";
            //return "#text-warning";
            else
                return "#0055FF";
            //return "#text-primary";
        }

    }


}

