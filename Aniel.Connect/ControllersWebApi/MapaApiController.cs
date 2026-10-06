using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
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
using NHibernate;
using NHibernate.Criterion;
using OpenXmlPowerTools;
using Sinapse.DAO.repository;
using Sinapse.Integracao.Rastreamento.Integracoes;
using Sinapse.Integracao.SMS.Interface;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class MapaApiController : BaseApiController
    {

        [HttpGet()]
        [Route("api/Mapaapi/GetComercial")]
        public async Task<Comercial_MapaModel> GetComercial(Int64 Id_Vendedor, String Data)
        {
            Comercial_MapaModel retorno = new Comercial_MapaModel();
            retorno.PontoVenda_CheckPoint_Mapa = new List<Comercial_Mapa_PontoVenda_CheckPointModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                var _data_ini = Convert.ToDateTime(Convert.ToDateTime(Data).ToString("dd/MM/yyyy 00:00:00"));
                var _data_fim = Convert.ToDateTime(Convert.ToDateTime(Data).ToString("dd/MM/yyyy 23:59:59"));

                string celular = (await new Conexao<Funcionario>(session).selectFirst(p => p.Id == Id_Vendedor)).Tel_Celular;


                string vendedor = (await new Conexao<Funcionario>(session).selectFirst(p => p.Id == Id_Vendedor)).Nome;
                string supervisor = Convert.ToString((await new Conexao<Funcionario>(session).selectFirst(p => p.Id == Id_Vendedor)).Supervisor);
                string num_fre = (await new Conexao<Funcionario>(session).selectFirst(p => p.Id == Id_Vendedor)).Num_Fre;
                string email = (await new Conexao<FuncionarioAniel>(sessionT5).selectFirst(p => p.Num_Fre == num_fre)).Email;
                //string chave = (await new Conexao<EquipeAniel>(sessionT5).selectFirst(p => p.Num_Fre == num_fre)).Matricula;

                //string bateria = Convert.ToString((await new Conexao<EquipeStatusAniel>(sessionT5).selectFirst(p => Convert.ToString(p.Equipe) == equipe)).Bateria);

                // Lista de Check-ins

                var listaCheckIn = await new Conexao<Checkin_Vendedor>(session).select(
                                                                                    p => p.Vendedor.Id == Id_Vendedor
                                                                                      && (p.Atualizado_Em >= _data_ini
                                                                                          && p.Atualizado_Em <= _data_fim)
                                                                                );

                retorno.PontoVenda_CheckPoint_Mapa.AddRange(
                    listaCheckIn.Select(
                        p => new Comercial_Mapa_PontoVenda_CheckPointModel
                        {
                            Ponto_Venda = true,

                            Id_Ponto_Venda = p.Ponto_Venda.Id,
                            Desc_Nome_Venda = p.Ponto_Venda.Nome,

                            Vendedor = vendedor,

                            Latitude = p.Ponto_Venda.Latitude,
                            Longitude = p.Ponto_Venda.Longitude,
                            Endereco = String.Format("{0} - {1}", p.Ponto_Venda.Logradouro, p.Ponto_Venda.Numero),

                            CheckIn = p.DataInicial.HasValue,
                            CheckOut = p.DataFinal.HasValue,

                            DataHora_CheckIn = p.DataInicial,

                            Atualizado_Em = p.Atualizado_Em.HasValue ? p.Atualizado_Em.Value.ToString("dd/MM/yyy HH:mm") : String.Empty,
                            Precisao = String.Format("{0} m", Math.Round(p.Precisao.Value, 2)),
                            Bateria = String.Format("{0}%", p.Bateria),
                            Status_Gps = p.Status_Gps,
                            Dispositivo = p.Dispositivo,
                            Versao_Android = p.Versao_Android,
                            Versao_App = p.Versao_App
                        }
                    ).ToList()
                );

                var listIdsPontoVenda = retorno.PontoVenda_CheckPoint_Mapa.Select(u => u.Id_Ponto_Venda).ToList();

                var listaPontosVenda = await new Conexao<Ponto_Venda_Vendedor>(session).select(
                                                                                            p => p.Vendedor.Id == Id_Vendedor
                                                                                              && (_data_ini >= p.DataInicial
                                                                                                  && _data_ini <= p.DataFinal)
                                                                                              && !p.Ponto_Venda.Id.IsIn(listIdsPontoVenda)
                                                                                        );

                retorno.PontoVenda_CheckPoint_Mapa.AddRange(
                    listaPontosVenda.Select(p => new Comercial_Mapa_PontoVenda_CheckPointModel
                    {
                        Ponto_Venda = true,

                        Id_Ponto_Venda = p.Ponto_Venda.Id,
                        Desc_Nome_Venda = p.Ponto_Venda.Nome,

                        Vendedor = vendedor,

                        Latitude = p.Ponto_Venda.Latitude,
                        Longitude = p.Ponto_Venda.Longitude,
                        Endereco = String.Format("{0} - {1}", p.Ponto_Venda.Logradouro, p.Ponto_Venda.Numero)
                    })
                );

                // Lista de Checkpoints 

                var listaCheckpoints = await new Conexao<Checkpoint_Vendedor>(session).select(
                                                                                        p => p.Vendedor.Id == Id_Vendedor
                                                                                          && (p.Data_Hora >= _data_ini
                                                                                              && p.Data_Hora <= _data_fim)
                                                                                    );

                int count_checkpoint = listaCheckpoints.Count();

                retorno.PontoVenda_CheckPoint_Mapa.AddRange(
                    listaCheckpoints.Select(p => new Comercial_Mapa_PontoVenda_CheckPointModel
                    {
                        Latitude = p.Latitude,
                        Longitude = p.Longitude,

                        Checkpoint = true,
                        Obs_Checkpoint = p.Observacao,

                        DataHora_CheckIn = p.Data_Hora,

                        Tipo = p.Tipo.Descricao,
                        Vendedor = p.Vendedor.Nome,
                        Endereco = p.Endereco,
                        Atualizado_Em = p.Atualizado_Em.HasValue ? p.Atualizado_Em.Value.ToString("dd/MM/yyyy HH:mm") : String.Empty,
                        Precisao = String.Format("{0}m", Math.Round(p.Precisao.Value, 2)),
                        Bateria = String.Format("{0}%", p.Bateria),
                        Status_Gps = p.Status_Gps,
                        Dispositivo = p.Dispositivo,
                        Versao_Android = p.Versao_Android,
                        Versao_App = p.Versao_App
                    })
                );

                int count_checkin = retorno.PontoVenda_CheckPoint_Mapa.Where(p => !p.Ponto_Venda).Count();

                retorno.PontoVenda_CheckPoint_Mapa.ForEach(p => p.Count_CheckIn = count_checkin);
                retorno.PontoVenda_CheckPoint_Mapa = retorno.PontoVenda_CheckPoint_Mapa.OrderBy(p => p.DataHora_CheckIn).ToList();

                retorno.Vendedor = vendedor;
                retorno.Telefone = celular == null ? Resources.RsBase.NAOENCONTRADO : celular;
                retorno.Email = email == null ? Resources.RsBase.NAOENCONTRADO : email;
                retorno.Supervisor = supervisor == null ? Resources.RsBase.NAOENCONTRADO : supervisor;
                retorno.Bateria = Resources.RsBase.NAOENCONTRADO;
                retorno.Atualizado_Em = Resources.RsBase.NAOENCONTRADO;
                //retorno.Bateria = bateria == null ? Resources.RsBase.NAOENCONTRADO : bateria;



            }

            retorno.Data_Busca = Data;

            return retorno;
        }

        private async Task<List<Equipe_MapaModel_Realizada>> GetEquipe_MapaModel_Planejado(/*Ponto_Partida Ponto_Partida,*/ Int64 Id_Vendedor, DateTime? Data)
        {
            Dictionary<String, Object> parametros = new Dictionary<string, object>();

            List<Equipe_MapaModel_Realizada> retorno = new List<Equipe_MapaModel_Realizada>();

            using (ISession session = HibernateUtil.GetSession())
            {
                //Count
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("  SELECT   A.LATITUDE_CHECKIN , A.LONGITUDE_CHECKIN , A.DATA ,  ")
                                               .AppendLine(" A.ENDERECO_CHECKIN,  A.ENDERECO_CHECKOUT  ")
                                               .AppendLine(" FROM TB_CHECKIN_VENDEDOR  A  ")
                                               .AppendLine(" WHERE A.ID_VENDEDOR = @ID_VENDEDOR AND A.DATA = @DATA ");

                    parametros.Add("@ID_VENDEDOR", Id_Vendedor);
                    if (Data == null)
                    {
                        parametros.Add("@DATA", DateTime.Now);
                    }
                    else
                    {
                        var data = Data.Value.ToString().Replace("/", ".");
                        parametros.Add("@DATA", data);
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

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            var obj = new Equipe_MapaModel_Realizada
                            {
                                //Num_Obra = Convert.ToString(reader["NUM_OBRA"]),
                                Latitude = reader["LATITUDE_CHECKIN"] != DBNull.Value ? Convert.ToDouble(reader["LATITUDE_CHECKIN"]) : (Double?)null,
                                Longitude = reader["LONGITUDE_CHECKIN"] != DBNull.Value ? Convert.ToDouble(reader["LONGITUDE_CHECKIN"]) : (Double?)null
                            };

                            //if (reader["DATA_MAXIMA"] != DBNull.Value)
                            //{
                            //    obj.DataHora = Convert.ToDateTime(reader["DATA_MAXIMA"]).ToString("dd/MM/yyyy");

                            //    //if (reader["HORA_MAXIMA"] != DBNull.Value)
                            //    //    obj.DataHora += ' ' + ((TimeSpan)(reader["HORA_MAXIMA"])).ToString(@"hh\:mm"); ;
                            //}
                            //else
                            //    obj.DataHora = String.Empty;

                            retorno.Add(obj);
                        }
                    }



                    return retorno;
                }
            }
        }

        private async Task<List<Equipe_MapaModel_Realizada>> GetEquipe_MapaModel_Realizado(Ponto_Partida Ponto_Partida, String Equipe, DateTime? Data)
        {
            Dictionary<String, Object> parametros = new Dictionary<string, object>();

            List<Equipe_MapaModel_Realizada> retorno = new List<Equipe_MapaModel_Realizada>();
            List<Equipe_MapaModel_Realizada> temp = new List<Equipe_MapaModel_Realizada>();

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                Boolean Exibe_RDOs = (await new Conexao<Configuracoes_Gerais>(session).selectFirst()).ExibeRDOs;

                //Count
                using (var com = sessionT5.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT A.NUM_OBRA, A.LATITUDE, A.LONGITUDE, A.DATAHORA, B.DATA_PROD, A.TIPO_LOCALIZACAO ")
                                              .AppendLine(" FROM TB_LOG_LOCALIZACAO_JORNADA A ")
                                              .AppendLine(" LEFT JOIN TB_DOCUMENTO_PRODUCAO B ")
                                              .AppendLine(" ON(A.CODCT = B.CODCT AND A.PROJETO = B.PROJETO AND ")
                                              .AppendLine("     A.NUM_OBRA = B.NUM_OBRA AND A.CONTRATO = B.NUM_DOC");
                    if (!Exibe_RDOs)
                        sql.AppendLine(" AND COALESCE(B.SIGLA_MODULO, '') <> 'MCO' ");

                    sql.AppendLine(") WHERE  A.EQUIPE = @EQUIPE AND A.DATAHORA BETWEEN @DATA_INI AND @DATA_FIM ");

                    parametros.Add("@EQUIPE", Equipe);
                    if (Data == null)
                    {
                        parametros.Add("@DATA_INI", DateTime.Now.ToString("dd/MM/yyyy 00:00"));
                        parametros.Add("@DATA_FIM", DateTime.Now.ToString("dd/MM/yyyy 23:59"));
                    }
                    else
                    {
                        parametros.Add("@DATA_INI", Data.Value.ToString("dd/MM/yyyy 00:00"));
                        parametros.Add("@DATA_FIM", Data.Value.ToString("dd/MM/yyyy 23:59"));
                    }


                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            temp.Add(new Equipe_MapaModel_Realizada
                            {
                                DataHoraStr = reader["DATAHORA"] != DBNull.Value ? Convert.ToString(reader["DATAHORA"]) : String.Empty,
                                Num_Obra = Convert.ToString(reader["NUM_OBRA"]),
                                Latitude = reader["LATITUDE"] != DBNull.Value ? Convert.ToDouble(reader["LATITUDE"]) : (Double?)null,
                                Longitude = reader["LONGITUDE"] != DBNull.Value ? Convert.ToDouble(reader["LONGITUDE"]) : (Double?)null,
                                Cor = GetCorServico(reader["DATA_PROD"])
                            });
                        };
                    }

                    if (temp.Count() > 0)
                    {
                        Equipe_MapaModel_Realizada ultimaRota = temp.First();
                        Double tolerancia = 20; //metros
                        var primeiroPonto = temp.First();
                        primeiroPonto.Ponto_Partida = Equipe;
                        retorno.Add(primeiroPonto);

                        foreach (var obj in temp)
                        {
                            if (obj.Latitude.HasValue && obj.Longitude.HasValue && ultimaRota.Latitude.HasValue && ultimaRota.Longitude.HasValue)
                            {
                                if (!String.IsNullOrEmpty(obj.Num_Obra) ||
                                    MapaService.GetMenorDistancia(obj.Latitude, obj.Longitude, ultimaRota.Latitude, ultimaRota.Longitude) > tolerancia)
                                {
                                    retorno.Add(obj);
                                    ultimaRota = obj;
                                }
                            }

                        }
                    }
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

        [HttpPost()]
        [Route("api/Mapaapi/GetEquipe")]
        public async Task<List<Equipe_MapaItemModel>> GetEquipes(String Num_Fre_Supervisor, String Id_Monitor, String Equipe, String Cod_Escritorio, String Cod_Area, Int32? Id_Localidade,
            Int64? Id_Naturesa)
        {
            Dictionary<long, PerfilAgendamento> cachePerfilAgendamento = new Dictionary<long, PerfilAgendamento>();
            List<Equipe_MapaItemModel> retorno = new List<Equipe_MapaItemModel>();
            Dictionary<String, Object> parametros = new Dictionary<string, object>();

            Agendamento_EquipeApiController agendamentoApi = new Agendamento_EquipeApiController();

            EquipeApiController equipeApi = new EquipeApiController();

            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (ISession session_connect = HibernateUtil.GetSession())
            using (ISession sessionT5NH = HibernateUtil.GetSessionT5())
            using (ISession session = HibernateUtil.GetSession())

            using (var repositoryHorarioEquipe = new Conexao<Horario_Equipe>(sessionT5NH))
            using (var repositoryIntegracoesExternas = new Conexao<IntegracoesExternas>(session_connect))
            using (var tipoServicoApi = new Tipo_Servico_EquipeApiController())
            using (var acessoApi = new AcessoApiController())
            using (var repositoryConfig = new Conexao<Configuracoes_Gerais>(session_connect))
            using (var repositoryPerfilAgendamento = new Conexao<PerfilAgendamento>(sessionT5))
            {
                var configGerais = await repositoryConfig.selectFirst();
                ITipoServicoAnielRepository repoTipoServicoAniel = new TipoServicoAnielRepository(sessionT5NH,sessionT5);
                IDocumentoProducaoAnielRepository repoDocumento = new DocumentoProducaoAnielRepository(sessionT5NH, sessionT5);
                IEquipeAnielRepository repoEquipe = new EquipeAnielRepository(sessionT5);
                EquipeAnielBusiness equipeAnielBusiness = new EquipeAnielBusiness(repoEquipe, repoTipoServicoAniel, repoDocumento);

                string userId = User.Identity.GetUserId();
                List<AreaModuloModel> areas = acessoApi.GetArea_ModuloUsuario(userId).Result.Objeto;

                var integracoesExternas = (await repositoryIntegracoesExternas.select()).ToList();

                IntegracaoRastreamento integracaoRastreamento = null;

                if (integracoesExternas.Find(p => p.IntegracaoExterna == Model.enumerado.IntegracaoExterna.COBLI) != null)
                {
                    var integracaoCobli = integracoesExternas.Find(p => p.IntegracaoExterna == Model.enumerado.IntegracaoExterna.COBLI);
                    integracaoRastreamento = new Cobli(integracaoCobli.TokenAcesso);
                }


                var lastLocationRastreamento = integracaoRastreamento != null ? integracaoRastreamento.LastLocation() : null;


                List<Feriado> feriados = (await new Conexao<Feriado>(session_connect).select(p => p.Data == DateTime.Now.Date)).ToList();
                List<Horario_Connect> horarios_Connect = (await new Conexao<Horario_Connect>(sessionT5).select(p => p.Tipo == Tipo_Hora_Connect.Horario_Servico)).ToList();
                Horario_Connect horario_ConnectPadrao = horarios_Connect.Find(p => p.Padrao == true);
                DayOfWeek diaSemana = DateTime.Now.DayOfWeek;



                var _dataIni = Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy 00:00"));
                var _dataFin = Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy 23:59"));

                //Count
                using (var com = sessionT5.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT A.LATITUDE, A.LONGITUDE, A.ID,B.ID_PERFIL_AGENDAMENTO ")
                           .AppendLine(" B.NOME, B.EQUIPE AS EQUIPE_ID, A.DATAHORACRIACAO, A.DATAHORAALTERACAO, A.LOGRADOURO, ")
                           .AppendLine(" A.NUMERO, A.BAIRRO, A.CIDADE, A.PRECISAO, A.BATERIA, A.STATUSGPS, ")
                           .AppendLine(" A.DISPOSITIVO, A.JUSTIFICATIVA, A.VERSAO_ANDROID, A.VERSAO_APP, A.IMEI, ")
                           .AppendLine(" A.INTERNET_MOVEL, A.WIFI, A.TIPO, ")
                           .AppendLine("           (SELECT FIRST 1 F.ID ")
                           .AppendLine("           FROM TB_HORARIO_EQUIPE_CONNECT F ")
                           .AppendLine("           LEFT JOIN TB_HORARIO_CONNECT G ")
                           .AppendLine("           ON(F.ID_HORARIO = G.ID) ")
                           .AppendLine("           WHERE F.EQUIPE = A.EQUIPE AND(((F.APENAS_NO_DIA = 1) AND F.DATA = @DATA) OR ")
                           .AppendLine("                                          (F.APENAS_NO_DIA = 0 AND F.DATA <= @DATA)) ")
                           .AppendLine("           ORDER BY F.DATA DESC) AS ID_HORARIO_EQUIPE, C.TEL_CELULAR, C.TELEFONE ")
                    .AppendLine(" FROM TB_EQUIPE_STATUS A ")
                    .AppendLine(" INNER JOIN TB_EQUIPE B ")
                    .AppendLine(" ON(A.EQUIPE = B.EQUIPE) ")
                    .AppendLine(" INNER JOIN TB_FUNCIONARIO C ")
                    .AppendLine(" ON(B.NUM_FRE = C.NUM_FRE) ")
                    .AppendLine(" WHERE B.EQUIPE_CONNECT = 'S' AND C.SITUACAO IN(0, 4)  ") //AND A.DATAHORACRIACAO BETWEEN @DATA_INI AND @DATA_FIM
                    .AppendLine(" AND COALESCE(A.LATITUDE, 0.00) <> 0.00 "); // A.LOGADO = 1

                    parametros.Add("@DATA", DateTime.Now.Date);
                    parametros.Add("@DATA_INI", Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy 00:00")));
                    parametros.Add("@DATA_FIM", Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy 23:59")));

                    // Só visualiza técnicos do qual o usuário é supervisor ou monitor
                    if (!AcessoHelper.TemAcesso(areas, "176"))
                    {
                        IUsuarioRepository repoUsuario = new UsuarioRepository(session);
                        var usuarioBusiness = new UsuarioBusiness(repoUsuario);

                        IdentityUser user = usuarioBusiness.GetById(userId);

                        sql.AppendLine(" AND (B.COD_SUPERV = @COD_SUPERV OR B.USUARIO_MONITOR = @USUARIO_MONITOR) ");

                        parametros.Add("@USUARIO_MONITOR", userId);
                        parametros.Add("@COD_SUPERV", user.Funcionario?.Num_Fre);
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(Num_Fre_Supervisor))
                        {
                            sql.AppendLine(" AND (B.COD_SUPERV = @COD_SUPERV) ");

                            if (!parametros.ContainsKey("@COD_SUPERV"))
                                parametros.Add("@COD_SUPERV", Num_Fre_Supervisor);
                        }

                        if (!String.IsNullOrEmpty(Id_Monitor))
                        {
                            sql.AppendLine(" AND (B.USUARIO_MONITOR = @USUARIO_MONITOR) ");

                            if (!parametros.ContainsKey("@USUARIO_MONITOR"))
                                parametros.Add("@USUARIO_MONITOR", Id_Monitor);
                        }
                    }

                    if (!String.IsNullOrEmpty(Equipe))
                    {
                        sql.AppendLine(" AND (A.EQUIPE = @EQUIPE) ");
                        parametros.Add("@EQUIPE", Equipe);
                    }

                    if (Id_Naturesa.HasValue)
                    {
                        sql.AppendLine(" AND A.EQUIPE IN (SELECT D.EQUIPE FROM TB_EQUIPE_NATUREZA D WHERE A.EQUIPE = D.EQUIPE AND D.ID_NATUREZA = @ID_NATUREZA) ");
                        parametros.Add("@ID_NATUREZA", Id_Naturesa);
                    }

                    if (!String.IsNullOrEmpty(Cod_Escritorio))
                    {
                        sql.AppendLine(" AND A.EQUIPE IN (SELECT E.EQUIPE FROM TB_EQUIPE_LOCALIDADE E WHERE A.EQUIPE = E.EQUIPE AND E.COD_ESCRITORIO = @COD_ESCRITORIO ");

                        if (!String.IsNullOrEmpty(Cod_Area))
                        {
                            sql.AppendLine(" AND E.COD_AREA = @COD_AREA");
                            parametros.Add("@COD_AREA", Cod_Escritorio);
                        }

                        if (Id_Localidade.HasValue)
                        {
                            sql.AppendLine(" AND (E.COD_LOCALIDADE = @COD_LOCALIDADE OR E.TODAS_LOCALIDADES = 1)");
                            parametros.Add("@COD_LOCALIDADE", Id_Localidade);
                        }

                        sql.AppendLine(" )");
                        parametros.Add("@COD_ESCRITORIO", Cod_Escritorio);
                    }

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            String endereco = String.Empty;

                            if ((reader["LOGRADOURO"] != DBNull.Value) && !String.IsNullOrEmpty(Convert.ToString(reader["LOGRADOURO"])))
                            {
                                endereco = $@"{reader["LOGRADOURO"]} Nº:{reader["NUMERO"]} {reader["BAIRRO"]} {reader["CIDADE"]}";
                            }

                            var temp = new Equipe_MapaItemModel();

                            EquipeStatusTipo tipo = reader["TIPO"] != DBNull.Value ? (EquipeStatusTipo)Enum.Parse(typeof(EquipeStatusTipo), Convert.ToString(reader["TIPO"])) : EquipeStatusTipo.Sem_Status;

                            Horario_Connect horario_Equipe = horario_ConnectPadrao;
                            Horario_Equipe horarioEquipeConnect = null;
                            if (reader["ID_HORARIO_EQUIPE"] != DBNull.Value)
                            {
                                horarioEquipeConnect = await repositoryHorarioEquipe.selectFirst(p => p.Id == Convert.ToInt64(reader["ID_HORARIO_EQUIPE"]));

                                if (horarioEquipeConnect?.Indisponivel == true)
                                {
                                    continue;
                                }

                                long? horario_connect = horarioEquipeConnect?.Horario?.Id;
                                var tmp = horarios_Connect.Find(p => p.Id == Convert.ToInt64(horario_connect));

                                if (tmp != null)
                                    horario_Equipe = tmp;
                            }

                            PerfilAgendamento perfilAgendamento = null;
                            if (horarioEquipeConnect?.PerfilAgendamento != null)
                            {
                                perfilAgendamento = horarioEquipeConnect.PerfilAgendamento;
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

                            bool feriado = feriados.Find(p => p.Data == DateTime.Now.Date) != null;
                            AgendamentoDiaModel configDoDia = agendamentoApi.GetConfigDoDia(diaSemana, DateTime.Now.Date, feriado, horario_Equipe);

                            if (configDoDia != null && configDoDia.HoraInicioIntervalo.HasValue)
                            {
                                if (DateTime.Now >= configDoDia.HoraInicioIntervalo && DateTime.Now <= configDoDia.HoraTerminoIntervalo && (tipo != EquipeStatusTipo.Em_Atendimento && tipo != EquipeStatusTipo.Em_Deslocamento))
                                    tipo = EquipeStatusTipo.Em_Intervalo;
                            }


                            var conf_gerais = new Conexao<Configuracoes_Gerais>(session).selectFirst().Result;

                            var tempoPadraoExecucao = conf_gerais.Tempo_Execucao_Padrao;
                            var tempoPadraoDeslocamento = conf_gerais.Tempo_Medio_Deslocamento;

                            var tempoMinimoNaturezaExecucao = tipoServicoApi.TempoServicoMinimo(sessionT5).Result.valor;



                            if (configDoDia != null && (tipo == EquipeStatusTipo.Disponivel || tipo == EquipeStatusTipo.Em_Atendimento))
                            {
                                var horarioApi = new HorarioApiController();

                                DateTime HoraInicioServico = DateTime.Now;
                                DateTime? HoraFim = configDoDia.HoraTerminoIntervalo.HasValue ? configDoDia.HoraTerminoIntervalo.Value : (DateTime?)null;

                                var tempoExecucaoServico = tempoPadraoExecucao.Value.AddMinutes(tempoPadraoDeslocamento.Value.Minute);


                                if (tempoMinimoNaturezaExecucao != null)
                                {
                                    tempoExecucaoServico = Convert.ToDateTime(tempoMinimoNaturezaExecucao).AddMinutes(tempoPadraoDeslocamento.Value.Minute);
                                }

                                if (configDoDia.HoraInicioIntervalo.HasValue && configDoDia.HoraTerminoIntervalo.HasValue
                                   && tempoExecucaoServico >= DateTime.Now)
                                {
                                    HoraInicioServico = configDoDia.HoraTerminoIntervalo.Value;
                                    HoraFim = configDoDia.HoraTerminoIntervalo.Value.AddHours(tempoExecucaoServico.Hour);
                                }

                                bool apto = horarioApi.AptoHorario(configDoDia.HoraInicio, configDoDia.HoraInicioIntervalo,
                                   configDoDia.HoraTerminoIntervalo, configDoDia.HoraTermino, HoraInicioServico, HoraFim);

                                if (apto == false && tempoExecucaoServico >= DateTime.Now)
                                {
                                    tipo = EquipeStatusTipo.Sem_Agenda;
                                }

                                using (ISession session2 = HibernateUtil.GetSessionT5())
                                using (var repoDocumentoProducao = new Conexao<Documento_ProducaoAniel>(session2))
                                using (var repoTipoServico = new Conexao<Tipo_Servico_EquipeAniel>(session2))

                                {
                                    var os = await repoDocumentoProducao.selectFirst(p =>
                                             p.Equipe == (Convert.ToString(reader["EQUIPE_ID"]))
                                             && (p.Status == StatusExecucaoAniel.OS_Em_deslocamento
                                             || p.Status == StatusExecucaoAniel.Reiniciado_Atendimento
                                             || p.Status == StatusExecucaoAniel.Atendimento_Paralizado
                                             || p.Status == StatusExecucaoAniel.Atendimento_Iniciado_Reiniciado)
                                             && p.Data_Maxima == DateTime.Now.Date
                                             );

                                    if (os != null)
                                    {
                                        temp.Numero_Os = os.Numero_Obra_Original;
                                        if (os.Cod_Tipo_Serv.HasValue)
                                        {
                                            temp.TipoServico = repoTipoServico.selectFirst(p => p.Cod_Tipo_Serv == os.Cod_Tipo_Serv).Result.Descricao;
                                        }
                                    }
                                }
                            }

                            tipo = equipeAnielBusiness.GetStatusPersonalizado(DateTime.Now.Date,
                                                                              horario_ConnectPadrao,
                                                                              feriado,
                                                                              horarioEquipeConnect,
                                                                              tipo,
                                                                              perfilAgendamento,
                                                                              Equipe,
                                                                              configGerais.Tempo_Execucao_Padrao.Value.TimeOfDay);

                            temp.Cor_StatusEquipe_Mapa = EquipeApiController.GetCorStatusEquipe(tipo);
                            temp.Latitude = Convert.ToString(reader["Latitude"]).Replace(',', '.');
                            temp.Longitude = Convert.ToString(reader["Longitude"]).Replace(',', '.');
                            temp.OrigemLocalizacao = "Aniel";

                            temp.Id = Convert.ToInt64(reader["Id"]);
                            temp.Equipe = Convert.ToString(reader["NOME"]);

                            if (String.IsNullOrEmpty(temp.Equipe) == false)
                            {
                                temp.Equipe = temp.Equipe.Trim();
                            }

                            temp.Status_Tecnico = HtmlEnumExtensions.GetEnumDescription<EquipeStatusTipo>(tipo);

                            if (lastLocationRastreamento != null && lastLocationRastreamento.Sucesso)
                            {
                                var dadosUltimaLocalizacao = lastLocationRastreamento.Dados.Find(p => p.nome != null && p.nome.ToUpper() == temp.Equipe.ToUpper());
                                if (dadosUltimaLocalizacao != null)
                                {
                                    temp.Latitude = Convert.ToString(dadosUltimaLocalizacao.latitude).Replace(',', '.');
                                    temp.Longitude = Convert.ToString(dadosUltimaLocalizacao.longitude).Replace(',', '.');
                                    temp.OrigemLocalizacao = integracaoRastreamento.Nome;
                                }
                            }

                            if (reader["TEL_CELULAR"] != DBNull.Value && !String.IsNullOrEmpty(Convert.ToString(reader["TEL_CELULAR"])))
                            {
                                temp.Telefone = Convert.ToString(reader["TEL_CELULAR"]);
                            }
                            else if (reader["TELEFONE"] != DBNull.Value && !String.IsNullOrEmpty(Convert.ToString(reader["TELEFONE"])))
                            {
                                temp.Telefone = Convert.ToString(reader["TELEFONE"]);
                            }
                            else
                            {
                                temp.Telefone = "-";
                            }

                            temp.Data = reader["DataHoraCriacao"] != DBNull.Value ? Convert.ToDateTime(reader["DataHoraCriacao"]).ToString("dd/MM/yyyy") : String.Empty;
                            temp.Endereco = endereco;
                            //temp.Atualizado_Em = reader["DataHoraCriacao"] != DBNull.Value ? Convert.ToDateTime(reader["DataHoraCriacao"]).ToString("dd/MM/yyyy HH:mm") : String.Empty;
                            temp.Atualizado_Em = reader["DataHoraAlteracao"] != DBNull.Value ? Convert.ToDateTime(reader["DataHoraAlteracao"]).ToString("dd/MM/yyyy HH:mm") : String.Empty;
                            temp.Precisao = reader["Precisao"] != DBNull.Value ? Convert.ToDouble(reader["Precisao"]).ToString("N2") : String.Empty;
                            temp.Bateria = Convert.ToString(reader["Bateria"]);
                            temp.Status_Gps = Convert.ToString(reader["StatusGPS"]);
                            temp.Dispositivo = Convert.ToString(reader["Dispositivo"]);
                            temp.Justificativa_Ausencia = Convert.ToString(reader["JUSTIFICATIVA"]);
                            temp.Versao_Android = Convert.ToString(reader["Versao_Android"]);
                            temp.Versao_App = Convert.ToString(reader["Versao_App"]);
                            temp.IMEI = Convert.ToString(reader["IMEI"]);
                            temp.InternetMovel = Convert.ToString(reader["INTERNET_MOVEL"]);
                            temp.Wifi = Convert.ToString(reader["WIFI"]) == "1" ? Resources.RsBase.LIGADO : Resources.RsBase.DESLIGADO;

                            if (String.IsNullOrEmpty(temp.Justificativa_Ausencia))
                            {
                                temp.Justificativa_Ausencia = "-";
                            }

                            retorno.Add(temp);
                        };
                    }
                }

                if (retorno.Count == 0)
                {
                    throw new Exception("Não existe movimentação neste dia");
                }

                return retorno;
            }
        }
    }
}
