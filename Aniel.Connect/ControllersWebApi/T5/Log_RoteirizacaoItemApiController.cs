using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Resources;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi.T5
{
    [Authorize]
    public class Log_RoteirizacaoItemApiController : BaseApiController
    {
        [HttpPost()]
        [Route("api/Log_RoteirizacaoItemApi/GetAll")]
        public async Task<DataTableResponse<Log_RoteirizacaoItemListaModel>> GetAll(Log_Roteirizacao_Item_DataTableRequest request)
        {
            List<Log_RoteirizacaoItemListaModel> retorno = new List<Log_RoteirizacaoItemListaModel>();
            IOrderedEnumerable<Log_RoteirizacaoItemListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSessionLog())
            using (ISession sessionConnect = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var repoTipoServico = new Conexao<Tipo_Servico_EquipeAniel>(sessionT5))
            using (var repoNatureza = new Conexao<Natureza_ProducaoAniel>(sessionT5))
            using (var repoDocProducao = new Conexao<Documento_ProducaoAniel>(sessionT5))
            using (var repoReincidencia = new Conexao<ReincidenciaDocProdAniel>(sessionT5))
            using (var repoEquipe = new Conexao<EquipeAniel>(sessionT5))
            using (var repoFornecedor = new Conexao<Fornecedor>(sessionT5))
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sqlFrom = new StringBuilder("FROM TB_LOG_ROTEIRIZACAO_ITEM A ");
                sqlFrom.AppendLine(" INNER JOIN TB_LOG_ROTEIRIZACAO B ");
                sqlFrom.AppendLine(" ON (A.ID_LOG = B.ID)");
                sqlFrom.AppendLine(" WHERE A.ID_LOG = @ID_LOG");


                if (!String.IsNullOrEmpty(request.search.value))
                {
                    sqlFrom.AppendLine("AND ((UPPER(A.NUM_OBRA_ORIGINAL) LIKE UPPER(@SEARCH)))");
                    parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                }

                if (request.Somente_LogRoteirizacao.HasValue)
                {
                    if (request.Somente_LogRoteirizacao == Somente_LogRoteirizacao.ROTEIRIZADAS)
                        sqlFrom.AppendLine("AND A.EQUIPE IS NOT NULL");
                    else if (request.Somente_LogRoteirizacao == Somente_LogRoteirizacao.NAO_ROTEIRIZADAS)
                        sqlFrom.AppendLine("AND A.EQUIPE IS NULL");

                }




                parametros.Add("@ID_LOG", request.Id);

                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(ID) CO FROM TB_LOG_ROTEIRIZACAO_ITEM"); //WHERE STATUS = 1  

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
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(" SELECT A.ID, A.CODCT, A.PROJETO, A.NUM_OBRA, A.NUM_DOC,A.MOTIVO, A.ID_LOG,A.POSICAO_FILA, ");
                    sql.AppendLine(" A.COD_TIPO_SERV, A.DESC_TIPO_SERV, A.CLIENTE, A.ID_CLIENTE, A.EQUIPE, A.NOME_EQUIPE, A.DATA_HORA_INI_AGENDAMENTO, ");
                    sql.AppendLine(" A.DATA_HORA_FIM_AGENDAMENTO, A.COD_ESCRITORIO, A.COD_AREA, A.COD_LOCALIDADE, A.DESC_ESCRITORIO, A.DESC_AREA, A.LOCALIDADE, A.DESLOCAMENTO,");
                    sql.AppendLine(" A.SLA, A.DATA_VENCIMENTO,A.NOME_SUPERVISOR,A.NOME_MONITOR, A.POSSUILATLONG, A.LATITUDE_OS, A.LATITUDE_CAIXA, A.LONGITUDE_OS, A.LONGITUDE_CAIXA, ");
                    sql.AppendLine(" A.LATITUDE_LOCALIDADE,A.LONGITUDE_LOCALIDADE,A.DESC_NATUREZA,A.ORDEM_DISTRIBUIDA,A.NIVEL_RODADA_ROTEIRIZADA,A.AGRUPAMENTO_RODADA_ROTEIRIZADA,A.LATITUDE_PONTO_PARTIDA,A.LONGITUDE_PONTO_PARTIDA,A.PONTO_PARTIDA,A.ORIGEM_AGENDAMENTO ");
                    sql.AppendLine(sqlFrom.ToString());

                    //Hora de filtrar
                    if (!String.IsNullOrEmpty(request.Contrato_filtro))
                    {
                        sql.AppendLine(" AND A.CODCT = @CODCT ");
                        parametros.Add("@CODCT", request.Contrato_filtro);
                    }
                    if (!String.IsNullOrEmpty(request.Projeto_filtro))
                    {
                        sql.AppendLine(" AND A.PROJETO = @PROJETO ");
                        parametros.Add("@PROJETO", request.Projeto_filtro);
                    }
                    if (!String.IsNullOrEmpty(request.Localidade_filtro))
                    {
                        sql.AppendLine(" AND A.LOCALIDADE = @LOCALIDADE ");
                        parametros.Add("@LOCALIDADE", request.Localidade_filtro);
                    }
                    if (!String.IsNullOrEmpty(request.Grupo_area_filtro))
                    {
                        sql.AppendLine(" AND (A.COD_ESCRITORIO = @DESC_ESCRITORIO) ");
                        parametros.Add("@DESC_ESCRITORIO", request.Grupo_area_filtro);
                    }
                    if (!String.IsNullOrEmpty(request.Area_filtro))
                    {
                        sql.AppendLine(" AND A.COD_AREA = @DESC_AREA ");
                        parametros.Add("@DESC_AREA", request.Area_filtro);
                    }

                    if (!String.IsNullOrEmpty(request.Numfre_Supervisor))
                    {
                        sql.AppendLine(" AND A.NUMFRE_SUPERVISOR = @NUMFRE_SUPERVISOR ");
                        parametros.Add("@NUMFRE_SUPERVISOR", request.Numfre_Supervisor);
                    }

                    if (!String.IsNullOrEmpty(request.Usuario_Monitor))
                    {
                        sql.AppendLine(" AND A.USUARIO_MONITOR = @USUARIO_MONITOR ");
                        parametros.Add("@USUARIO_MONITOR", request.Usuario_Monitor);
                    }

                    if (!String.IsNullOrEmpty(request.Sla_filtro))
                    {
                        sql.AppendLine(" AND A.SLA = @SLA ");
                        parametros.Add("@SLA", request.Sla_filtro);
                    }
                    if (request.Vencimentoate_filtro == true)
                    {

                        if (!String.IsNullOrEmpty(request.DataFinal) && !String.IsNullOrEmpty(request.DataInicial))
                        {

                            sql.AppendLine(" AND A.DATA_VENCIMENTO BETWEEN @DATA_INICIO AND @DATA_FIM");
                            parametros.Add("@DATA_INICIO", request.DataInicial);
                            parametros.Add("@DATA_FIM", request.DataFinal);
                        }
                    }





                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            var numObra = Convert.ToString(reader["NUM_OBRA"]);
                            var obj = new Log_RoteirizacaoItemListaModel
                            {
                                Id_Log = Convert.ToString(reader["ID"]),
                                Codct = Convert.ToString(reader["CODCT"]),
                                Projeto = Convert.ToString(reader["PROJETO"]),
                                Num_Obra = numObra.Split('/')[0],
                                Num_Doc = Convert.ToString(reader["NUM_DOC"]),
                                Numfre_Supervisor = Convert.ToString(reader["NOME_SUPERVISOR"]),
                                Usuario_Monitor = Convert.ToString(reader["NOME_MONITOR"]),
                                Motivo = Convert.ToString(reader["MOTIVO"]),
                                Cod_tipo_serv = reader["COD_TIPO_SERV"] != DBNull.Value ? Convert.ToInt16(reader["COD_TIPO_SERV"]) : (Int16?)null,
                                Desc_tipo_serv = Convert.ToString(reader["DESC_TIPO_SERV"]),
                                Cliente = Convert.ToString(reader["CLIENTE"]),
                                Id_cliente = reader["ID_CLIENTE"] != DBNull.Value ? Convert.ToInt64(reader["ID_CLIENTE"]) : (Int64?)null,
                                Equipe = Convert.ToString(reader["EQUIPE"]),
                                Nome_equipe = Convert.ToString(reader["NOME_EQUIPE"]),
                                Data_hora_ini_agendamento = reader["DATA_HORA_INI_AGENDAMENTO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_HORA_INI_AGENDAMENTO"]).ToString("dd/MM/yyyy HH:mm") : String.Empty,
                                Data_hora_fim_agendamento = reader["DATA_HORA_FIM_AGENDAMENTO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_HORA_FIM_AGENDAMENTO"]).ToString("dd/MM/yyyy HH:mm") : String.Empty,
                                Cod_escritorio = Convert.ToString(reader["COD_ESCRITORIO"]),
                                Cod_area = Convert.ToString(reader["COD_AREA"]),
                                Cod_localidade = reader["COD_LOCALIDADE"] != DBNull.Value ? Convert.ToInt32(reader["COD_LOCALIDADE"]) : (Int32?)null,
                                Desc_escritorio = Convert.ToString(reader["DESC_ESCRITORIO"]),
                                Desc_area = Convert.ToString(reader["DESC_AREA"]),
                                Localidade = Convert.ToString(reader["LOCALIDADE"]),
                                PosicaoFila = Convert.ToString(reader["POSICAO_FILA"]),
                                SLA = Convert.ToString(reader["SLA"]),
                                Data_Hora_Vencimento_OS = Convert.ToString(reader["DATA_VENCIMENTO"]),
                                PossuiLatLong = reader["POSSUILATLONG"] == DBNull.Value ? "N/D" : (Convert.ToBoolean(reader["POSSUILATLONG"]) ? RsCadastros.SIM : RsCadastros.NAO),
                                LatitudeOS = Convert.ToString(reader["LATITUDE_OS"]),
                                LongitudeOS = Convert.ToString(reader["LONGITUDE_OS"]),
                                LatitudeCaixa = reader["LATITUDE_CAIXA"] != DBNull.Value ? Convert.ToString(reader["LATITUDE_CAIXA"]).Replace(",", ".") : null,
                                LongitudeCaixa = reader["LONGITUDE_CAIXA"] != DBNull.Value ? Convert.ToString(reader["LONGITUDE_CAIXA"]).Replace(",", ".") : null,
                                LatitudeLocalidade = reader["LATITUDE_LOCALIDADE"] != DBNull.Value ? Convert.ToString(reader["LATITUDE_LOCALIDADE"]).Replace(",", ".") : null,
                                LongitudeLocalidade = reader["LONGITUDE_LOCALIDADE"] != DBNull.Value ? Convert.ToString(reader["LONGITUDE_LOCALIDADE"]).Replace(",", ".") : null,
                                DescNatureza = Convert.ToString(reader["DESC_NATUREZA"]),
                                OrdemDistribuida = Convert.ToString(reader["ORDEM_DISTRIBUIDA"]),
                                NivelRodadaRoteirizada = Convert.ToString(reader["NIVEL_RODADA_ROTEIRIZADA"]),
                                AgrupamentoRodadaRoteirizada = Convert.ToString(reader["AGRUPAMENTO_RODADA_ROTEIRIZADA"]),
                                PontoPartida = Convert.ToString(reader["PONTO_PARTIDA"]),
                                LatitudePontoPartida = reader["LATITUDE_PONTO_PARTIDA"] != DBNull.Value ? Convert.ToString(reader["LATITUDE_PONTO_PARTIDA"]).Replace(",", ".") : null,
                                LongitudePontoPartida = reader["LONGITUDE_PONTO_PARTIDA"] != DBNull.Value ? Convert.ToString(reader["LONGITUDE_PONTO_PARTIDA"]).Replace(",", ".") : null,
                                OrigemAgendamento = Convert.ToString(reader["ORIGEM_AGENDAMENTO"])
                            };



                            var deslocamento = reader["DESLOCAMENTO"] != DBNull.Value ? Convert.ToDouble(reader["DESLOCAMENTO"]) : 0;
                            if (!String.IsNullOrEmpty(obj.Data_hora_ini_agendamento))
                            {
                                if (deslocamento > 0) obj.Data_hora_ini_agendamento = Convert.ToDateTime(obj.Data_hora_ini_agendamento).AddMinutes(deslocamento).ToString("dd/MM/yyyy HH:mm");
                            }


                            var documentoSelecionado = await repoDocProducao.selectFirst(os =>
                                os.CodCt == obj.Codct && os.Projeto == obj.Projeto &&
                                os.Num_Obra == numObra && os.Num_Doc == Convert.ToInt64(obj.Num_Doc));

                            var equipe = await repoEquipe.selectFirst(e => e.Matricula == obj.Equipe); //obtem fornecedor atual do tecnico responsavel pela OS

                            if (equipe?.Cod_Fornecedor != null)
                            {
                                Fornecedor fornecedor = equipe.Cod_Fornecedor != null ? await repoFornecedor.selectFirst(f => f.Cod_Fornecedor == equipe.Cod_Fornecedor) : null;

                                if (fornecedor != null)
                                    obj.FornecedorAtual = fornecedor.RazaoSocial;
                            }

                            if (documentoSelecionado != null)
                            {
                                var hasReincidencia = await repoReincidencia.selectFirst(os =>
                                    os.CodCt == documentoSelecionado.CodCt && os.Projeto == documentoSelecionado.Projeto &&
                                    os.NumObra == documentoSelecionado.Num_Obra && os.NumDoc == Convert.ToInt64(documentoSelecionado.Num_Doc));

                                if (hasReincidencia != null)
                                {
                                    obj.UtilizaOsReincidencia = true;

                                    var documentoReincidencia = await repoDocProducao.selectFirst(os =>
                                        os.CodCt == hasReincidencia.CodCtAnterior && os.Projeto == hasReincidencia.ProjetoAnterior &&
                                        os.Num_Obra == hasReincidencia.NumObraAnterior && os.Num_Doc == Convert.ToInt64(hasReincidencia.NumDocAnterior));

                                    var equipeOsOriginal = await repoEquipe.selectFirst(e =>
                                        e.Matricula == documentoReincidencia.Equipe);

                                    if (equipeOsOriginal?.Cod_Fornecedor != null)
                                    {
                                        Fornecedor fornecedor = equipeOsOriginal?.Cod_Fornecedor != null ? await repoFornecedor.selectFirst(f => f.Cod_Fornecedor == equipeOsOriginal.Cod_Fornecedor) : null;

                                        if (fornecedor != null)
                                            obj.FornecedorAnterior = fornecedor.RazaoSocial;
                                    }

                                    if (!string.IsNullOrEmpty(equipeOsOriginal.Nome))
                                        obj.TecnicoAnterior = equipeOsOriginal.Nome;
                                }
                            }

                            retorno.Add(obj);
                        }

                    }
                }

                #region //Ordenacao
                if (request.order != null && request.order.Any())
                {
                    int sortColumnIndex = request.order[0].column;
                    var coluna = request.columns[sortColumnIndex].data;
                    string sortDirection = request.order[0].dir;

                    switch (coluna)
                    {
                        case "codct":
                            {
                                if (sortDirection == "asc")
                                    listaOrdenada = retorno.OrderBy(p => p.Codct);
                                else
                                    listaOrdenada = retorno.OrderByDescending(p => p.Codct);
                                break;
                            }

                        case "projeto":
                            {
                                if (sortDirection == "asc")
                                    listaOrdenada = retorno.OrderBy(p => p.Projeto);
                                else
                                    listaOrdenada = retorno.OrderByDescending(p => p.Projeto);
                                break;
                            }

                        case "num_Obra":
                            {
                                if (sortDirection == "asc")
                                    listaOrdenada = retorno.OrderBy(p => p.Num_Obra);
                                else
                                    listaOrdenada = retorno.OrderByDescending(p => p.Num_Obra);
                                break;
                            }

                        case "num_Doc":
                            {
                                if (sortDirection == "asc")
                                    listaOrdenada = retorno.OrderBy(p => p.Num_Doc);
                                else
                                    listaOrdenada = retorno.OrderByDescending(p => p.Num_Doc);
                                break;
                            }

                        case "motivo":
                            {
                                if (sortDirection == "asc")
                                    listaOrdenada = retorno.OrderBy(p => p.Motivo);
                                else
                                    listaOrdenada = retorno.OrderByDescending(p => p.Motivo);
                                break;
                            }

                        case "desc_tipo_serv":
                            {
                                if (sortDirection == "asc")
                                    listaOrdenada = retorno.OrderBy(p => p.Desc_tipo_serv);
                                else
                                    listaOrdenada = retorno.OrderByDescending(p => p.Desc_tipo_serv);
                                break;
                            }

                        case "descNatureza":
                            {
                                if (sortDirection == "asc")
                                    listaOrdenada = retorno.OrderBy(p => p.DescNatureza);
                                else
                                    listaOrdenada = retorno.OrderByDescending(p => p.DescNatureza);
                                break;
                            }

                        case "cliente":
                            {
                                if (sortDirection == "asc")
                                    listaOrdenada = retorno.OrderBy(p => p.Cliente);
                                else
                                    listaOrdenada = retorno.OrderByDescending(p => p.Cliente);
                                break;
                            }

                        case "nome_equipe":
                            {
                                if (sortDirection == "asc")
                                    listaOrdenada = retorno.OrderBy(p => p.Nome_equipe);
                                else
                                    listaOrdenada = retorno.OrderByDescending(p => p.Nome_equipe);
                                break;
                            }

                        case "posicaoFila":
                            {
                                if (sortDirection == "asc")
                                    listaOrdenada = retorno.OrderBy(p => p.PosicaoFila);
                                else
                                    listaOrdenada = retorno.OrderByDescending(p => p.PosicaoFila);
                                break;
                            }

                        case "data_hora_ini_agendamento":
                            {
                                if (sortDirection == "asc")
                                    listaOrdenada = retorno.OrderBy(p => p.Data_hora_ini_agendamento);
                                else
                                    listaOrdenada = retorno.OrderByDescending(p => p.Data_hora_ini_agendamento);
                                break;
                            }

                        case "desc_escritorio":
                            {
                                if (sortDirection == "asc")
                                    listaOrdenada = retorno.OrderBy(p => p.Desc_escritorio);
                                else
                                    listaOrdenada = retorno.OrderByDescending(p => p.Desc_escritorio);
                                break;
                            }

                        case "desc_area":
                            {
                                if (sortDirection == "asc")
                                    listaOrdenada = retorno.OrderBy(p => p.Desc_area);
                                else
                                    listaOrdenada = retorno.OrderByDescending(p => p.Desc_area);
                                break;
                            }

                        case "localidade":
                            {
                                if (sortDirection == "asc")
                                    listaOrdenada = retorno.OrderBy(p => p.Localidade);
                                else
                                    listaOrdenada = retorno.OrderByDescending(p => p.Localidade);
                                break;
                            }

                        case "numfre_Supervisor":
                            {
                                if (sortDirection == "asc")
                                    listaOrdenada = retorno.OrderBy(p => p.Numfre_Supervisor);
                                else
                                    listaOrdenada = retorno.OrderByDescending(p => p.Numfre_Supervisor);
                                break;
                            }

                        case "usuario_Monitor":
                            {
                                if (sortDirection == "asc")
                                    listaOrdenada = retorno.OrderBy(p => p.Usuario_Monitor);
                                else
                                    listaOrdenada = retorno.OrderByDescending(p => p.Usuario_Monitor);
                                break;
                            }

                        case "possuiLatLong":
                            {
                                if (sortDirection == "asc")
                                    listaOrdenada = retorno.OrderBy(p => p.PossuiLatLong);
                                else
                                    listaOrdenada = retorno.OrderByDescending(p => p.PossuiLatLong);
                                break;
                            }

                        case "latitudeOS":
                            {
                                if (sortDirection == "asc")
                                    listaOrdenada = retorno.OrderBy(p => p.LatitudeOS);
                                else
                                    listaOrdenada = retorno.OrderByDescending(p => p.LatitudeOS);
                                break;
                            }

                        case "longitudeOS":
                            {
                                if (sortDirection == "asc")
                                    listaOrdenada = retorno.OrderBy(p => p.LongitudeOS);
                                else
                                    listaOrdenada = retorno.OrderByDescending(p => p.LongitudeOS);
                                break;
                            }

                        case "latitudeCaixa":
                            {
                                if (sortDirection == "asc")
                                    listaOrdenada = retorno.OrderBy(p => p.LatitudeCaixa);
                                else
                                    listaOrdenada = retorno.OrderByDescending(p => p.LatitudeCaixa);
                                break;
                            }

                        case "longitudeCaixa":
                            {
                                if (sortDirection == "asc")
                                    listaOrdenada = retorno.OrderBy(p => p.LongitudeCaixa);
                                else
                                    listaOrdenada = retorno.OrderByDescending(p => p.LongitudeCaixa);
                                break;
                            }

                        case "latitudeLocalidade":
                            {
                                if (sortDirection == "asc")
                                    listaOrdenada = retorno.OrderBy(p => p.LatitudeLocalidade);
                                else
                                    listaOrdenada = retorno.OrderByDescending(p => p.LatitudeLocalidade);
                                break;
                            }

                        case "longitudeLocalidade":
                            {
                                if (sortDirection == "asc")
                                    listaOrdenada = retorno.OrderBy(p => p.LongitudeLocalidade);
                                else
                                    listaOrdenada = retorno.OrderByDescending(p => p.LongitudeLocalidade);
                                break;
                            }

                        case "pontoPartida":
                            {
                                if (sortDirection == "asc")
                                    listaOrdenada = retorno.OrderBy(p => p.PontoPartida);
                                else
                                    listaOrdenada = retorno.OrderByDescending(p => p.PontoPartida);
                                break;
                            }

                        case "latitudePontoPartida":
                            {
                                if (sortDirection == "asc")
                                    listaOrdenada = retorno.OrderBy(p => p.LatitudePontoPartida);
                                else
                                    listaOrdenada = retorno.OrderByDescending(p => p.LatitudePontoPartida);
                                break;
                            }

                        case "longitudePontoPartida":
                            {
                                if (sortDirection == "asc")
                                    listaOrdenada = retorno.OrderBy(p => p.LongitudePontoPartida);
                                else
                                    listaOrdenada = retorno.OrderByDescending(p => p.LongitudePontoPartida);
                                break;
                            }

                    }
                }
                #endregion

                recordsTotal = listaOrdenada.Count();

                //Page e retornando DataTable
                return new DataTableResponse<Log_RoteirizacaoItemListaModel>
                {
                    draw = request.draw,
                    //recordsTotal = recordsTotal,
                    recordsFiltered = recordsTotal,
                    data = listaOrdenada,
                    error = ""
                };

            }

        }
    }
}