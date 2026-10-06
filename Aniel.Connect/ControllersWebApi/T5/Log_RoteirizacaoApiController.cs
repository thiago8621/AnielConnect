using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.ModelLog;
using Aniel.Connect.ModelLog.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi.T5
{
    [Authorize]
    public class Log_RoteirizacaoApiController : BaseApiController
    {
        [HttpPost()]
        [Route("api/Log_RoteirizacaoApi/GetAll")]
        public async Task<DataTableResponse<Log_RoteirizacaoListaModel>> GetAll(Log_Roteirizacao_DataTableRequest request)
        {
            List<Log_RoteirizacaoListaModel> retorno = new List<Log_RoteirizacaoListaModel>();
            IOrderedEnumerable<Log_RoteirizacaoListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSessionLog())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.ID,A.DATAHORACRIACAO, A.STATUS, A.USUARIOCRIACAO,A.SIMULACAO ");
                    sql.AppendLine(" FROM TB_LOG_ROTEIRIZACAO A ");
                    sql.AppendLine(" WHERE A.ID > 0 ");

                    if (request.Filtro)
                    {
                        sql.AppendLine(" AND A.DATAHORACRIACAO BETWEEN @DATAINI AND @DATAFIM ");

                        parametros.Add("@DATAINI", Convert.ToDateTime($"{request.DataHoraCriacao} 00:00"));
                        parametros.Add("@DATAFIM", Convert.ToDateTime($"{request.DataHoraCriacao} 23:59"));

                        if (!String.IsNullOrEmpty(request.Usuario))
                        {
                            sql.AppendLine(" AND A.USUARIOCRIACAO = @USUARIO");
                            parametros.Add("@USUARIO", request.Usuario);
                        }
                    }


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            string simulacao = reader["SIMULACAO"] == DBNull.Value ? Resources.RsCadastros.NAO : (Convert.ToBoolean(reader["SIMULACAO"]) ? Resources.RsCadastros.SIM : Resources.RsCadastros.NAO);
                            retorno.Add(new Log_RoteirizacaoListaModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Data_Log = Convert.ToString(reader["DATAHORACRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORACRIACAO"]).ToString("dd/MM/yyyy HH:mm") : String.Empty),
                                Status = HtmlEnumExtensions.GetEnumDescription<StatusRoteirizacao>(Convert.ToString(reader["STATUS"])),
                                Usuario = Convert.ToString(reader["USUARIOCRIACAO"]),
                                Simulacao = simulacao
                            });
                        }
                    }

                }
            }

            //Ordenacao
            listaOrdenada = retorno.OrderBy(p => p.Data_Log);

            recordsTotal = listaOrdenada.Count();

            //Page e retornando DataTable
            return new DataTableResponse<Log_RoteirizacaoListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        [HttpPost()]
        public async Task<GetApiRetorno<IEnumerable<ObjBusca<Int32>>>> GetList()
        {
            GetApiRetorno<IEnumerable<ObjBusca<Int32>>> retorno = new GetApiRetorno<IEnumerable<ObjBusca<Int32>>>();

            using (ISession session = HibernateUtil.GetSessionLog())
            {
                try
                {
                    var lista = await new Conexao<Log_RoteirizacaoAniel>(session).select();

                    retorno.Objeto = lista.Select(p => new ObjBusca<Int32>
                    {
                        Value = Convert.ToInt32(p.Id),
                        Label = Convert.ToString(p.DataHoraFinalizacao)
                    });

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

        public async Task<GetApiRetorno<Equipe_MapaLogItemModel>> GetEquipeNoMapa(String Equipe, DateTime? Data, long IdLog)
        {
            GetApiRetorno<Equipe_MapaLogItemModel> retorno = new GetApiRetorno<Equipe_MapaLogItemModel>();

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (ISession session_connect = HibernateUtil.GetSessionT5())
                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                {

                    using (var repositoryPonto = new Conexao<EquipeAniel>(session))
                    using (var repository = new Conexao<EquipeStatusAniel>(session))
                    using (var repositoryEquipe = new Conexao<EquipeAniel>(session))
                    using (var repositoryLog = new Conexao<Log_RoteirizacaoAniel>(sessionLog))
                    using (var repositoryLogItem = new Conexao<Log_RoteirizacaoItemAniel>(sessionLog))
                    using (var repositoryHorarioEquipe = new Conexao<Horario_Equipe>(session))
                    using (Agendamento_EquipeApiController agendamentoApi = new Agendamento_EquipeApiController())
                    using (EquipeApiController equipeApi = new EquipeApiController())
                    {
                        if (Data.HasValue == false)
                            Data = DateTime.Now;


                        var equipe = (await repositoryPonto.selectFirst(p => p.Matricula == Equipe));
                        var logs = await repositoryLogItem.select(p => p.Log.Id == IdLog && p.Equipe == equipe.Matricula && p.Motivo == "Roteirizado com Sucesso");
                        var log = await repositoryLog.selectFirst(p => p.Id == IdLog);
                        var tecnicosRoteirizacao = Sinapse.Util.JsonService.DeserializeObject<List<TecnicoRoteirizar>>(log.TecnicosRoteirizacao);
                        DayOfWeek diaSemana = Data.Value.DayOfWeek;


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

                        var tecnicoRoteirizacao = tecnicosRoteirizacao.Find(p => p.Equipe == Equipe);
                        if (String.IsNullOrEmpty(tecnicoRoteirizacao?.PontoPartida))
                        {
                            var lista_sem_ponto_partida = (await repository.select(p => p.Equipe.Matricula == Equipe)).Select(p =>
                            {
                                return new Equipe_MapaLogItemModel
                                {
                                    Equipe_Nome = p.Equipe.Nome,
                                    Log = log.UsuarioCriacao + " " + log.DataHoraCriacao.Value.ToString("dd/MM/yyyy HH:mm"),
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
                                    Versao_Android = p.Versao_Android,
                                    Versao_App = p.Versao_App,
                                    Telefone = _funcionarioObj.Telefone_Celular,
                                    Email = _funcionarioObj.Email,

                                };
                            }).ToList();

                            if (lista_sem_ponto_partida.Count() > 0)
                                retorno.Objeto = lista_sem_ponto_partida[0];
                            else
                            {
                                retorno.Objeto = new Equipe_MapaLogItemModel
                                {
                                    Log = log.UsuarioCriacao + " " + log.DataHoraCriacao.Value.ToString("dd/MM/yyyy HH:mm"),
                                    Bateria = Resources.RsBase.NAOENCONTRADO,
                                    Dispositivo = Resources.RsBase.NAOENCONTRADO,
                                    Endereco = Resources.RsBase.NAOENCONTRADO,
                                    Atualizado_Em = Resources.RsBase.NAOENCONTRADO,
                                    Equipe_Nome = equipe.Nome,
                                    Num_Fre = equipe.Num_Fre,
                                    Telefone = _funcionarioObj.Telefone_Celular,
                                    Email = _funcionarioObj.Email,
                                    Rotas_Planejadas = new List<Equipe_MapaModel_Planejado>(),
                                    Rotas_Realizada = new List<Equipe_MapaModel_Realizada>()
                                };
                            }

                            throw new Exception(Resources.RsCadastros.EQUIPE_SEM_PONTO_PARTIDA);

                        }
                        var lista = (await repository.select(p => p.Equipe.Matricula == Equipe)).Select(p =>
                        {
                            var logFirst = logs.FirstOrDefault();
                            var obj = new Equipe_MapaLogItemModel
                            {
                                Equipe_Nome = p.Equipe.Nome,
                                Num_Fre = p.Equipe.Num_Fre,
                                Log = log.UsuarioCriacao + " " + log.DataHoraCriacao.Value.ToString("dd/MM/yyyy HH:mm"),
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
                                Versao_Android = p.Versao_Android,
                                Versao_App = p.Versao_App,
                                Rotas_Planejadas = GetEquipe_MapaModel_Planejado(new Ponto_Partida
                                {
                                    Descricao = tecnicoRoteirizacao.PontoPartida,
                                    Latitude = tecnicoRoteirizacao.Latitude,
                                    Longitude = tecnicoRoteirizacao.Longitude,
                                },
                                p.Equipe.Matricula,
                                Data, logs.ToList()).Result,
                                Rotas_Realizada = new List<Equipe_MapaModel_Realizada>(),
                                Telefone = _funcionarioObj.Telefone_Celular,
                                Email = _funcionarioObj.Email,

                            };

                            return obj;

                        }).ToList();

                        if (lista.Count() > 0)
                            retorno.Objeto = lista[0];
                        else
                        {
                            var logFirst = logs.FirstOrDefault();
                            var lista2 = (await new Conexao<EquipeAniel>(session).select(p => p.Matricula == Equipe)).Select(p => new Equipe_MapaLogItemModel
                            {
                                Rotas_Planejadas = GetEquipe_MapaModel_Planejado(new Ponto_Partida
                                {
                                    Descricao = tecnicoRoteirizacao.PontoPartida,
                                    Latitude = tecnicoRoteirizacao.Latitude,
                                    Longitude = tecnicoRoteirizacao.Longitude,
                                },
                                p.Matricula,
                                Data, logs.ToList()).Result,
                                Rotas_Realizada = new List<Equipe_MapaModel_Realizada>(),
                                Log = log.UsuarioCriacao + " " + log.DataHoraCriacao.Value.ToString("dd/MM/yyyy HH:mm"),
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
                                retorno.Objeto = new Equipe_MapaLogItemModel
                                {
                                    Rotas_Planejadas = new List<Equipe_MapaModel_Planejado>(),
                                    Log = log.UsuarioCriacao + " " + log.DataHoraCriacao.Value.ToString("dd/MM/yyyy HH:mm"),
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

        private async Task<List<Equipe_MapaModel_Planejado>> GetEquipe_MapaModel_Planejado(Ponto_Partida Ponto_Partida, String Equipe, DateTime? Data, List<Log_RoteirizacaoItemAniel> logs)
        {


            List<Equipe_MapaModel_Planejado> retorno = new List<Equipe_MapaModel_Planejado>();

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                Boolean Exibe_RDOs = (await new Conexao<Configuracoes_Gerais>(session).selectFirst()).ExibeRDOs;

                //Count
                using (var com = sessionT5.Connection.CreateCommand())
                {


                    if (Ponto_Partida != null)
                    {
                        retorno.Add(new Equipe_MapaModel_Planejado
                        {
                            Ponto_Partida = Ponto_Partida.Descricao,
                            Latitude = Ponto_Partida.Latitude,
                            Longitude = Ponto_Partida.Longitude
                        });
                    }


                    var doc_api = new Documento_ProducaoApiController();

                    foreach (var log in logs)
                    {
                        double? latitudeOS = String.IsNullOrEmpty(log.LatitudeOS) ? (double?)null : Convert.ToDouble(log.LatitudeOS.Replace(',', '.'), CultureInfo.InvariantCulture);
                        double? longitudeOS = String.IsNullOrEmpty(log.LongitudeOS) ? (double?)null : Convert.ToDouble(log.LongitudeOS.Replace(',', '.'), CultureInfo.InvariantCulture);
                        double? latitudeLocalidade = String.IsNullOrEmpty(log.LatitudeLocalidade) ? (double?)null : Convert.ToDouble(log.LatitudeLocalidade.Replace(',', '.'), CultureInfo.InvariantCulture);
                        double? longitudeLocalidade = String.IsNullOrEmpty(log.LongitudeLocalidade) ? (double?)null : Convert.ToDouble(log.LongitudeLocalidade.Replace(',', '.'), CultureInfo.InvariantCulture);
                        double? latitudeCaixa = String.IsNullOrEmpty(log.LatitudeCaixa) ? (double?)null : Convert.ToDouble(log.LatitudeCaixa.Replace(',', '.'), CultureInfo.InvariantCulture);
                        double? longitudeCaixa = String.IsNullOrEmpty(log.LongitudeCaixa) ? (double?)null : Convert.ToDouble(log.LongitudeCaixa.Replace(',', '.'), CultureInfo.InvariantCulture);

                        var obj = new Equipe_MapaModel_Planejado
                        {
                            Num_Obra = log.Num_Obra,
                            Latitude = latitudeOS ?? (latitudeCaixa ?? latitudeLocalidade),
                            Longitude = longitudeOS ?? (longitudeCaixa ?? longitudeLocalidade),
                            StatusExecucaoAniel = (StatusExecucaoAniel)StatusExecucaoAniel.Aberta_Aguard_Atendimento,
                            StatusExecucaoAniel_Str = HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(StatusExecucaoAniel.Aberta_Aguard_Atendimento)
                        };


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



                        obj.Sla_Servico = log.SLA;

                        if (log.Data_hora_ini_agendamento.HasValue)
                        {
                            obj.DataHoraStr = log.Data_hora_ini_agendamento.Value.ToString("dd/MM/yyyy HH:mm");
                            obj.DataHora = log.Data_hora_ini_agendamento.Value;
                        }

                        retorno.Add(obj);
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
    }
}