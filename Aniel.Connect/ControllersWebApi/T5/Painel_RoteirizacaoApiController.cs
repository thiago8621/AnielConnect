using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Core.Service;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.ModelLog.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Models.T5;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.Usuario;
using Aniel.Connect.Tasks;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using Microsoft.AspNet.Identity;
using NHibernate;
using Resources;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.Identity;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi.T5
{

    [Authorize]
    public class Painel_RoteirizacaoApiController : BaseApiController
    {
        public DateTime? HoraRoteirizacao { get; set; }
        [HttpPost()]
        [Route("api/Painel_RoteirizacaoApi/GetAll")]
        public async Task<DataTableResponse<Painel_RoteirizacaoListaModel>> GetAll(Painel_Roteirizacao_DataTableRequest request)
        {

            List<Painel_RoteirizacaoListaModel> retorno = new List<Painel_RoteirizacaoListaModel>();
            IOrderedEnumerable<Painel_RoteirizacaoListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                IUsuarioRepository repoUsuario = new UsuarioRepository(session);
                var usuarioBusiness = new UsuarioBusiness(repoUsuario);

                IdentityUser user = usuarioBusiness.GetById(User.Identity.GetUserId());
                Boolean Exibe_RDOs = (await new Conexao<Configuracoes_Gerais>(session).selectFirst()).ExibeRDOs;

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                List<String> condicoes = new List<String>();
                //From e Parametros
                StringBuilder sqlFrom = new StringBuilder();
                sqlFrom.AppendLine("FROM TB_DOCUMENTO_PRODUCAO A ");
                sqlFrom.AppendLine("LEFT JOIN TB_EQUIPE B ");
                sqlFrom.AppendLine("on(A.EQUIPE = B.EQUIPE) ");
                sqlFrom.AppendLine("LEFT JOIN TB_ESCRITORIO C ");
                sqlFrom.AppendLine("on(A.COD_ESCRITORIO = C.COD_ESCRITORIO) ");
                sqlFrom.AppendLine("LEFT JOIN TB_AREA D ");
                sqlFrom.AppendLine("on(D.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = D.COD_ESCRITORIO AND A.COD_AREA = D.COD_AREA) ");
                sqlFrom.AppendLine("LEFT JOIN TB_LOCALIDADE E ");
                sqlFrom.AppendLine("on(E.COD_CLIENTE = 1 AND A.COD_ESCRITORIO = E.COD_ESCRITORIO AND A.COD_AREA = E.COD_AREA AND ");
                sqlFrom.AppendLine("A.COD_LOCALIDADE = E.COD_LOCALIDADE) ");
                sqlFrom.AppendLine(" LEFT JOIN TB_CLIENTE F ");
                sqlFrom.AppendLine(" ON(A.ID_CLIENTE = F.COD_CLIENTE) "); // ID CLIENTE
                sqlFrom.AppendLine(" LEFT JOIN TB_CATEGORIA_CLIENTE_CONNECT G ");
                sqlFrom.AppendLine("  ON(F.ID_CATEGORIA = G.ID) ");
                sqlFrom.AppendLine("LEFT JOIN TB_TIPO_SERVICO_EQUIPE K ");
                sqlFrom.AppendLine("ON(A.COD_TIPO_SERV = K.COD_TIPO_SERV) ");
                sqlFrom.AppendLine("LEFT JOIN TB_TIPO_IMOVEL J ");
                sqlFrom.AppendLine("ON(A.PARPRIM_PARA = J.COD_TIPO_IMOVEL) ");
                if (!String.IsNullOrEmpty(request.IdFilial))
                {
                    sqlFrom.AppendLine("LEFT JOIN TB_CONTRATO L ");
                    sqlFrom.AppendLine("ON(A.CODCT = L.CODCT) ");
                }

                sqlFrom.AppendLine("WHERE A.CODCT IS NOT NULL ");

                if (!Exibe_RDOs)
                    sqlFrom.AppendLine(" AND COALESCE(A.SIGLA_MODULO, '') <> 'MCO' ");

                //EVITAR DUPLICIDADE DE NUM_OBRA_ORIGINAL
                sqlFrom.AppendLine("AND COALESCE(A.SEQ, 1) = (SELECT FIRST 1 COALESCE(MAX(M.SEQ), 1)");
                sqlFrom.AppendLine("FROM TB_DOCUMENTO_PRODUCAO M");
                sqlFrom.AppendLine("WHERE A.CODCT = M.CODCT AND A.PROJETO = M.PROJETO");
                sqlFrom.AppendLine("AND A.NUM_OBRA_ORIGINAL = M.NUM_OBRA_ORIGINAL");
                sqlFrom.AppendLine("AND A.NUM_DOC = M.NUM_DOC)");

                if (String.IsNullOrEmpty(request.Numero_Ordem_Servico) && String.IsNullOrEmpty(request.IdFilial) &&
                    String.IsNullOrEmpty(request.IdSupervisor) && String.IsNullOrEmpty(request.Matricula_Id) &&
                    String.IsNullOrEmpty(request.CodCt) && !request.Servico_Sem_Agendamento && !request.Servico_Sem_Roteirizacao && !request.Servico_Agendado &&
                    !request.Servico_Paralisado && !request.Servico_Nao_Executado && !request.Servico_Roteirizados
                   )
                {
                    sqlFrom.AppendLine("AND (A.DATA_MAXIMA < @DATA_MAXIMA) AND (A.STATUS_EXECUCAO <> 9)");
                    parametros.Add("@DATA_MAXIMA", new DateTime(1900, 01, 01));
                }
                else if (!String.IsNullOrEmpty(request.Numero_Ordem_Servico))
                {
                    String[] obras = request.Numero_Ordem_Servico.Split('\n');
                    var obraComAspas = from o in obras select $"'{o.Trim()}'";
                    sqlFrom.AppendLine("AND (A.NUM_OBRA_ORIGINAL IN (" + String.Join(",", obraComAspas) + "))");
                }
                else
                {
                    sqlFrom.AppendLine("AND (A.DATA_PROD IS NULL) AND A.STATUS_EXECUCAO != 9");
                    ////Filtrando
                    //Filtro por Datas

                    List<String> condicoesOr = new List<String>();
                    if (request.Servico_Sem_Agendamento)
                    {
                        condicoesOr.Add("((A.DATA_MAXIMA IS NULL AND A.STATUS_EXECUCAO = 4) AND A.DIA BETWEEN @SERVICO_SEM_AGENDAMENTO_INI AND @SERVICO_SEM_AGENDAMENTO_FIM)");
                        parametros.Add("@SERVICO_SEM_AGENDAMENTO_INI", Convert.ToDateTime(request.NaoAgendadosIni));
                        parametros.Add("@SERVICO_SEM_AGENDAMENTO_Fim", Convert.ToDateTime(request.NaoAgendadosFim));
                    }

                    if (request.Servico_Sem_Roteirizacao)
                    {
                        condicoesOr.Add("(A.DATA_MAXIMA = @SERVICO_SEM_ROTEIRIZACAO AND A.EQUIPE IS NULL)");
                        parametros.Add("@SERVICO_SEM_ROTEIRIZACAO", Convert.ToDateTime(request.DataNaoRoteirizados));
                    }

                    if (request.Servico_Agendado)
                    {
                        condicoesOr.Add("(A.DATA_MAXIMA = @SERVICO_AGENDADO) AND ((A.STATUS_EXECUCAO = 13 OR COALESCE(A.EQUIPE, '') <> '') OR (A.STATUS_EXECUCAO = 0 AND A.EQUIPE IS NULL))");
                        parametros.Add("@SERVICO_AGENDADO", Convert.ToDateTime(request.DataAgendados));
                    }

                    if (request.Servico_Paralisado)
                    {
                        condicoesOr.Add("(A.DATAHORA_PAUSOU IS NOT NULL AND A.DATAHORA_REINICIO IS NULL)");
                        condicoesOr.Add("(A.DATAHORA_PAUSOU BETWEEN @DATAHORA_PAUSOUINI AND @DATAHORA_PAUSOUFIM)");
                        parametros.Add("@DATAHORA_PAUSOUINI", Convert.ToDateTime($"{request.Data_ParalisadoIni} 00:00"));
                        parametros.Add("@DATAHORA_PAUSOUFIM", Convert.ToDateTime($"{request.Data_ParalisadoFim} 23:59"));
                    }

                    if (request.Servico_Nao_Paralisado)
                    {
                        condicoes.Add("(A.DATAHORA_PAUSOU IS NULL)");
                    }

                    if (request.Servico_Nao_Executado)
                    {
                        condicoesOr.Add("(A.DATA_PROD IS NULL AND DATA_MAXIMA BETWEEN @DATA_NAO_EXECUTADOINI AND @DATA_NAO_EXECUTADOFIM)");
                        parametros.Add("@DATA_NAO_EXECUTADOINI", request.Data_Nao_ExecutadoIni);
                        parametros.Add("@DATA_NAO_EXECUTADOFIM", request.Data_Nao_ExecutadoFim);
                    }

                    if (request.Servico_Roteirizados)
                    {
                        condicoesOr.Add("(A.DATA_MAXIMA BETWEEN @DATA_ROTEIRIZACAOINI AND @DATA_ROTEIRIZACAOFIM AND A.EQUIPE IS NOT NULL)");
                        parametros.Add("@DATA_ROTEIRIZACAOINI", request.Data_RoteirizadosIni);
                        parametros.Add("@DATA_ROTEIRIZACAOFIM", request.Data_RoteirizadosFim);
                    }

                    if (condicoesOr.Count > 0)
                    {
                        condicoes.Add($"( {String.Join(" OR ", condicoesOr)} ) ");
                    }
                }

                if (!String.IsNullOrEmpty(request.IdFilial))
                {
                    Filial filial = await new Conexao<Filial>(session).selectFirst(p => p.Id == Convert.ToInt64(request.IdFilial));
                    condicoes.Add(" L.CODEMPRESA = @CODEMPRESA AND L.CODFILIAL = @CODFILIAL");
                    parametros.Add("@CODEMPRESA", filial.Empresa.CodEmpresa);
                    parametros.Add("@CODFILIAL", filial.CodFilial);
                }

                if (!String.IsNullOrEmpty(request.IdSupervisor))
                {
                    condicoes.Add(" B.COD_SUPERV = @COD_SUPERV");
                    parametros.Add("@COD_SUPERV", request.IdSupervisor);
                }

                if (!String.IsNullOrEmpty(request.Matricula_Id))
                {
                    condicoes.Add(" B.EQUIPE = @EQUIPE");
                    parametros.Add("@EQUIPE", request.Matricula_Id);
                }

                if (!String.IsNullOrEmpty(request.CodCt))
                {
                    condicoes.Add(" A.CODCT = @CODCT");
                    parametros.Add("@CODCT", request.CodCt);
                }

                if (!String.IsNullOrEmpty(request.Projeto))
                {
                    condicoes.Add(" A.PROJETO = @PROJETO");
                    parametros.Add("@PROJETO", request.Projeto);
                }

                if (!String.IsNullOrEmpty(request.GrupoArea))
                {
                    condicoes.Add("(A.COD_ESCRITORIO = @GRUPOAREA)");
                    parametros.Add("@GRUPOAREA", request.GrupoArea);
                }

                if (!String.IsNullOrEmpty(request.Area))
                {
                    condicoes.Add("(A.COD_AREA = @AREA)");
                    parametros.Add("@AREA", request.Area);
                }

                if (request.Localidade != 0)
                {
                    condicoes.Add("(A.COD_LOCALIDADE = @LOCALIDADE)");
                    parametros.Add("@LOCALIDADE", request.Localidade);
                }

                if (request.Id_NaturezaServico.Count != 0)
                {
                    condicoes.Add($"(K.COD_NATUREZA IN ({String.Join(", ", request.Id_NaturezaServico)}))");
                }

                if (request.Id_CategoriaCliente.HasValue)
                {
                    condicoes.Add("(F.ID_CATEGORIA = @CATEGORIA_CLIENTE)");
                    parametros.Add("@CATEGORIA_CLIENTE", request.Id_CategoriaCliente);

                }

                if (!String.IsNullOrEmpty(request.TipoPrioridade))
                {
                    sqlFrom.AppendLine(" AND A.TIPO_PRIORIDADE = @TIPOPRIORIDADE");
                    parametros.Add("@TIPOPRIORIDADE", request.TipoPrioridade);
                }


                if (!String.IsNullOrEmpty(request.search.value))
                {
                    sqlFrom.AppendLine("AND ((UPPER(A.NUM_OBRA_ORIGINAL) LIKE UPPER(@SEARCH)))");
                    parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                }

                if (condicoes.Count() > 0)
                {
                    sqlFrom.AppendLine($" AND ( {String.Join(" AND ", condicoes)} ) ");
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

                        sqlFrom.Append($" AND ({String.Join(" OR ", filtrosContratoProj)})");
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

                        sqlFrom.Append($" AND ({String.Join(" OR ", filtrosArea)})");
                    }

                    if (user.PerfilVisualizacaoServico.ParamsSetorDepto.Count() > 0)
                    {
                        var filtrosSetor = new List<string>();

                        foreach (var paramSetor in user.PerfilVisualizacaoServico.ParamsSetorDepto)
                        {
                            filtrosSetor.Add($"(A.COD_DEPTO = {paramSetor.CodDepto})");
                        }

                        sqlFrom.Append($" AND ({String.Join(" OR ", filtrosSetor)})");
                    }

                    if (user.PerfilVisualizacaoServico.ParamsSupervisor.Count() > 0)
                    {
                        var filtrosSupervisor = new List<string>();

                        foreach (var paramSupervisor in user.PerfilVisualizacaoServico.ParamsSupervisor)
                        {
                            filtrosSupervisor.Add($"(A.COD_SUPERV = '{paramSupervisor.CodSuperv}')");
                        }

                        sqlFrom.Append($" AND ({String.Join(" OR ", filtrosSupervisor)})");
                    }

                    if (user.PerfilVisualizacaoServico.ParamsMonitor.Count() > 0)
                    {
                        var filtrosMonitor = new List<string>();

                        foreach (var paramMonitor in user.PerfilVisualizacaoServico.ParamsMonitor)
                        {
                            filtrosMonitor.Add($"(A.USUARIO_MONITOR = '{paramMonitor.UsuarioMonitor}')");
                        }

                        sqlFrom.Append($" AND ({String.Join(" OR ", filtrosMonitor)})");
                    }
                }

                #endregion

                //Count
                using (var com = sessionT5.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.NUM_OBRA) CO ");
                    sql.AppendLine(sqlFrom.ToString());

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                //Select
                using (var com = sessionT5.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT A.CODCT, A.PROJETO, A.NUM_OBRA, A.NUM_OBRA_ORIGINAL ,A.NUM_DOC, COALESCE(F.CNPJ, A.CPF_CLIENTE) AS CNPJ, COALESCE(F.RAZAO_SOCIAL, A.TITULAR) RAZAO_SOCIAL, ");
                    sql.AppendLine("A.DIA + A.HORA AS DATA_ABERTURA, A.STATUS_EXECUCAO AS STATUS, ");
                    sql.AppendLine("A.DATA_MAXIMA AS DATA_AGENDAMENTO, A.HORA_MAXIMA AS HORA_AGENDAMENTO,");
                    sql.AppendLine("B.NOME AS NOME_EQUIPE, C.DESCRICAO AS DESCRICAO_ESCRITORIO, ");
                    sql.AppendLine("D.DESCRICAO AS DESCRICAO_AREA, A.COD_LOCALIDADE, E.LOCALIDADE, ");
                    sql.AppendLine("G.DESCRICAO AS CATEGORIA_CLIENTE,F.RAZAO_SOCIAL AS NOME_CLIENTE, J.DESCRICAO AS TIPO_IMOVEL, K.DESCRICAO AS TIPO_SERV,(A.DATA_INDICADOR + A.HORA_INDICADOR) AS SLA ");

                    sql.AppendLine(sqlFrom.ToString());

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, sessionT5.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Painel_RoteirizacaoListaModel
                            {
                                Cnpj = Convert.ToString(reader["CNPJ"]) == null ? null : Convert.ToString(reader["CNPJ"]).Replace(".", "").Replace("-", ""),
                                Razao_Social = Convert.ToString(reader["RAZAO_SOCIAL"]),
                                CodCt = Convert.ToString(reader["CODCT"]),
                                Projeto = Convert.ToString(reader["PROJETO"]),
                                Num_Obra_Original = Convert.ToString(reader["NUM_OBRA_ORIGINAL"]),
                                Num_Obra = Convert.ToString(reader["NUM_OBRA"]),
                                Num_Doc = Convert.ToInt32(reader["NUM_DOC"]),
                                Grupo_Area = Convert.ToString(reader["DESCRICAO_ESCRITORIO"]),
                                Area = Convert.ToString(reader["DESCRICAO_AREA"]),
                                Cod_Localidade = reader["COD_LOCALIDADE"] != DBNull.Value ? Convert.ToInt32(reader["COD_LOCALIDADE"]) : (Int32?)null,
                                Localidade = Convert.ToString(reader["LOCALIDADE"]),
                                Data_Criacao = Convert.ToString(reader["DATA_ABERTURA"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_ABERTURA"]).ToString("dd/MM/yyyy HH:mm") : String.Empty),
                                SLA = reader["SLA"] != DBNull.Value ? Convert.ToDateTime(reader["SLA"]).ToString("dd/MM/yyyy HH:mm") : String.Empty,
                                Data_Agendamento = Convert.ToString(reader["DATA_AGENDAMENTO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_AGENDAMENTO"]).ToString("dd/MM/yyyy") : String.Empty),
                                Cliente = Convert.ToString(reader["NOME_CLIENTE"]),
                                Categoria_Cliente = Convert.ToString(reader["CATEGORIA_CLIENTE"]),
                                Tipo_Imovel = Convert.ToString(reader["TIPO_IMOVEL"]),
                                Nome_Equipe = Convert.ToString(reader["NOME_EQUIPE"]),
                                Status = reader["STATUS"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(Convert.ToString(reader["STATUS"])) : String.Empty,
                                Tipo_Servico = Convert.ToString(reader["TIPO_SERV"]),
                                Hora_Agendamento = Convert.ToString(reader["HORA_AGENDAMENTO"] != DBNull.Value ? Convert.ToDateTime(reader["HORA_AGENDAMENTO"].ToString()).ToString("HH:mm") : String.Empty),
                            });
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
                    case 1:     // numero_servico
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Num_Obra);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Num_Obra);
                            break;
                        }
                    case 2:     // cpf
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Cnpj);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Cnpj);
                            break;
                        }
                    case 3:     // cliente
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Cliente);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Cliente);
                            break;
                        }
                    case 4:     // grupo_Area
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Grupo_Area);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Grupo_Area);
                            break;
                        }
                    case 5:     // area
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Area);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Area);
                            break;
                        }
                    case 6:     // localidade
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Localidade);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Localidade);
                            break;
                        }
                    case 7:     // tipo_Servico
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Tipo_Servico);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Tipo_Servico);
                            break;
                        }
                    case 8:     // data_Criacao
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => String.IsNullOrEmpty(p.Data_Criacao) ? (DateTime?)null : Convert.ToDateTime(p.Data_Criacao));
                            else
                                listaOrdenada = retorno.OrderByDescending(p => String.IsNullOrEmpty(p.Data_Criacao) ? (DateTime?)null : Convert.ToDateTime(p.Data_Criacao));
                            break;
                        }
                    case 9:     // sla
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => String.IsNullOrEmpty(p.SLA) ? (DateTime?)null : Convert.ToDateTime(p.SLA));
                            else
                                listaOrdenada = retorno.OrderByDescending(p => String.IsNullOrEmpty(p.SLA) ? (DateTime?)null : Convert.ToDateTime(p.SLA));
                            break;
                        }
                    case 10:     // data_Agendamento
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => String.IsNullOrEmpty(p.Data_Agendamento) ? (DateTime?)null : Convert.ToDateTime(p.Data_Agendamento));
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Data_Agendamento);
                            break;
                        }
                    case 11:     // hora_Agendamento
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => String.IsNullOrEmpty(p.Hora_Agendamento) ? (TimeSpan?)null : Convert.ToDateTime(p.Hora_Agendamento).TimeOfDay);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => String.IsNullOrEmpty(p.Hora_Agendamento) ? (TimeSpan?)null : Convert.ToDateTime(p.Hora_Agendamento).TimeOfDay);
                            break;
                        }
                    case 12:     // cliente
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Cliente);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Cliente);
                            break;
                        }
                    case 13:     // categoria_Cliente
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Categoria_Cliente);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Categoria_Cliente);
                            break;
                        }
                    case 14:     // tipo_Imovel
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Tipo_Imovel);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Tipo_Imovel);
                            break;
                        }
                    case 15:     // nome_Equipe
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Nome_Equipe);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Nome_Equipe);
                            break;
                        }
                    case 16:     // status
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Status);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Status);
                            break;
                        }
                    default:
                        {
                            listaOrdenada = retorno.OrderBy(p => p.Num_Obra);
                            break;
                        }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<Painel_RoteirizacaoListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };
        }

        [HttpPost()]
        [Route("api/Painel_RoteirizacaoApi/GetAllRotas")]
        public async Task<DataTableResponse<Painel_RotasListaModel>> GetAllRotas(Painel_Rotas_DataTableRequest request)
        {

            List<Painel_RotasListaModel> retorno = new List<Painel_RotasListaModel>();
            IOrderedEnumerable<Painel_RotasListaModel> listaOrdenada = null;
            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                Boolean Exibe_RDOs = (await new Conexao<Configuracoes_Gerais>(session).selectFirst()).ExibeRDOs;

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                //From e Parametros
                StringBuilder sqlFrom = new StringBuilder();
                sqlFrom.AppendLine("FROM TB_AGENDAMENTO_EQUIPEANIEL A ");
                sqlFrom.AppendLine("INNER JOIN TB_EQUIPE B ");
                sqlFrom.AppendLine("on(A.EQUIPE = B.EQUIPE) ");
                sqlFrom.AppendLine("LEFT JOIN TB_FUNCIONARIO J ");
                sqlFrom.AppendLine("on(B.NUM_FRE = J.NUM_FRE) ");
                sqlFrom.AppendLine("LEFT JOIN TB_DOCUMENTO_PRODUCAO C ");
                sqlFrom.AppendLine("on(A.NUM_OBRA = C.NUM_OBRA AND C.PROJETO = A.PROJETO AND A.NUM_DOC = C.NUM_DOC AND A.CODCT = C.CODCT");

                if (!Exibe_RDOs)
                    sqlFrom.AppendLine(" AND COALESCE(C.SIGLA_MODULO, '') <> 'MCO' ");

                sqlFrom.AppendLine(")");
                sqlFrom.AppendLine("LEFT JOIN TB_TIPO_SERVICO_EQUIPE D ");
                sqlFrom.AppendLine("on(C.COD_TIPO_SERV = D.COD_TIPO_SERV) ");
                sqlFrom.AppendLine("LEFT JOIN TB_CLIENTE E");
                sqlFrom.AppendLine("on(C.ID_CLIENTE = E.COD_CLIENTE)");
                sqlFrom.AppendLine("LEFT JOIN TB_ESCRITORIO F");
                sqlFrom.AppendLine("on(C.COD_ESCRITORIO = F.COD_ESCRITORIO)");
                sqlFrom.AppendLine("LEFT JOIN TB_AREA G");
                sqlFrom.AppendLine("on(G.COD_CLIENTE = 1 AND C.COD_ESCRITORIO = G.COD_ESCRITORIO AND C.COD_AREA = G.COD_AREA)");
                sqlFrom.AppendLine("LEFT JOIN TB_LOCALIDADE H");
                sqlFrom.AppendLine("on(H.COD_CLIENTE = 1 AND C.COD_ESCRITORIO = H.COD_ESCRITORIO AND C.COD_AREA = H.COD_AREA AND");
                sqlFrom.AppendLine("C.COD_LOCALIDADE = H.COD_LOCALIDADE)");


                ////Filtrando
                //Filtro por Datas
                List<String> condicoes = new List<String>();


                if (!String.IsNullOrEmpty(request.Equipe))
                {
                    condicoes.Add("A.EQUIPE = " + request.Equipe);
                }

                if (!String.IsNullOrEmpty(request.Supervisor))
                {
                    condicoes.Add("J.NUM_FRE = " + request.Supervisor);
                }

                if (!String.IsNullOrEmpty(request.Tipo_Servico))
                {
                    condicoes.Add("C.COD_TIPO_SERV = " + request.Tipo_Servico);
                }

                if (!String.IsNullOrEmpty(request.Grupo_Area))
                {
                    condicoes.Add("C.COD_ESCRITORIO = " + request.Grupo_Area);
                }
                if (!String.IsNullOrEmpty(request.Area))
                {
                    condicoes.Add("C.COD_AREA =" + request.Area);
                }

                if (!String.IsNullOrEmpty(request.Localidade))
                {
                    condicoes.Add("(A.COD_LOCALIDADE = " + request.Localidade);
                }

                if (!String.IsNullOrEmpty(request.Data_Agendamento))
                {
                    condicoes.Add("A.DATAHORA_INICIO BETWEEN @DATA_AGENDAMENTO_INICIO AND @DATA_AGENDAMENTO_FIM");
                    parametros.Add("@DATA_AGENDAMENTO_INICIO", Convert.ToDateTime(request.Data_Agendamento).ToString("dd/MM/yyyy 00:00"));
                    parametros.Add("@DATA_AGENDAMENTO_FIM", Convert.ToDateTime(request.Data_Agendamento).ToString("dd/MM/yyyy 23:59"));
                }

                //Filtrando
                if (!String.IsNullOrEmpty(request.search.value))
                {
                    condicoes.Add(" ((A.NUM_OBRA_ORIGINAL = @SEARCH))");
                    parametros.Add("@SEARCH", request.search.value);
                }

                if (condicoes.Count() > 0)
                {
                    sqlFrom.AppendLine(" WHERE  "); //
                    sqlFrom.AppendLine($" ( {String.Join(" AND ", condicoes)} ) ");
                }
                //else
                //{
                //    sqlFrom.AppendLine($" A.DIA = @DATA ");
                //    parametros.Add("@DATA", new DateTime(1900, 1, 1));
                //}



                //Count
                using (var com = sessionT5.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder("SELECT COUNT(A.NUM_OBRA) CO ");
                    sql.AppendLine(sqlFrom.ToString());

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            recordsTotal = Convert.ToInt32(reader["CO"]);
                        }
                    }
                }

                //Select
                using (var com = sessionT5.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT A.NUM_OBRA,C.NUM_OBRA_ORIGINAL,B.EQUIPE,B.NOME AS NOME_EQUIPE,A.DATAHORA_INICIO,A.DATAHORA_FIM,D.DESCRICAO AS TIPO_SERV,E.RAZAO_SOCIAL,");
                    sql.AppendLine("F.DESCRICAO AS DESCRICAO_ESCRITORIO, ");
                    sql.AppendLine("G.DESCRICAO AS DESCRICAO_AREA, H.LOCALIDADE ");

                    sql.AppendLine(sqlFrom.ToString());

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, sessionT5.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            var obj = new Painel_RotasListaModel
                            {
                                Num_Obra = Convert.ToString(reader["NUM_OBRA"]),
                                Num_Obra_Original = Convert.ToString(reader["NUM_OBRA_ORIGINAL"]),
                                Nome_Equipe = Convert.ToString(reader["NOME_EQUIPE"]),
                                Id_Equipe = Convert.ToString(reader["EQUIPE"]),
                                Data_Agendamento = Convert.ToString(reader["DATAHORA_INICIO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_INICIO"]).ToString("dd/MM/yyyy") : String.Empty),
                                Hora_Inicio_Agendamento = Convert.ToString(reader["DATAHORA_INICIO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_INICIO"].ToString()).ToString("HH:mm") : String.Empty),
                                Hora_Fim_Agendamento = Convert.ToString(reader["DATAHORA_FIM"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_FIM"].ToString()).ToString("HH:mm") : String.Empty),
                                Tipo_Servico = Convert.ToString(reader["TIPO_SERV"]),
                                Cliente = Convert.ToString(reader["RAZAO_SOCIAL"]),
                                Grupo_Area = Convert.ToString(reader["DESCRICAO_ESCRITORIO"]),
                                Area = Convert.ToString(reader["DESCRICAO_AREA"]),
                                Localidade = Convert.ToString(reader["LOCALIDADE"])
                            };

                            retorno.Add(obj);
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
                    case 1:     // Nome
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Num_Obra);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Num_Obra);
                            break;
                        }
                    default:
                        {
                            listaOrdenada = retorno.OrderBy(p => p.Num_Obra);
                            break;
                        }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<Painel_RotasListaModel>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = recordsTotal,
                data = listaOrdenada,
                error = ""
            };

        }

        [HttpPost()]
        [Route("api/Painel_RoteirizacaoApi/Roteirizar")]
        public async Task<PostApiRetorno<Int64>> Roteirizar(ParamWebApi<RoteirizarServicosModel> value)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (RoteirizacaoDAO dao = new RoteirizacaoDAO())
            using (var repoReincidencia = new Conexao<ReincidenciaDocProdAniel>(sessionT5))
            using (var repoDocProducao = new Conexao<Documento_ProducaoAniel>(sessionT5))
            using (var repoEquipe = new Conexao<EquipeAniel>(sessionT5))
            using (var repoTipoServico = new Conexao<Tipo_Servico_EquipeAniel>(sessionT5))
            using (var repoNatureza = new Conexao<Natureza_ProducaoAniel>(sessionT5))
            {
                try
                {
                    var configuracoes = await GetConfiguracoesRoteirizacao(session, sessionT5, value.Objeto);
                    List<DocumentoRoteirizar> documentos = await GetDocumentosRoteirizacao(value.Objeto, configuracoes.configGerais, dao);
                    (List<DocumentoRoteirizar> documentosRoteirizados, List<DocumentoRoteirizar> documentosNaoRoteirizados) resultado = new ValueTuple<List<DocumentoRoteirizar>, List<DocumentoRoteirizar>>();

                    var tecnicosRoteirizacao = new List<TecnicoRoteirizar>();

                    var validacao = ValidaDocumentos(documentos, configuracoes.diaConf, configuracoes.configGerais, dao);

                    #region roteirizacaoPorReincidenciaOs
                    if (validacao.documentosValidos.Any())
                    {

                        //foreach doc valido, verifico se há reincidencia da OS. caso não tiver, segue para os proximos
                        foreach (var document in validacao.documentosValidos)
                    {
                            var docReincidencia = await repoReincidencia.selectFirst(x =>
                                x.CodCt == document.CodCt && 
                                x.Projeto == document.Projeto &&
                                x.NumDoc == document.Num_Doc &&
                                x.NumObra == document.Num_Obra); //seleciona a OS dado CODCT, PROJETO, NUM_DOC, COD_CLIENTE, E NUM_OBRA seja diferente da selecionada para roteirizar, isso pois na TB_REINCIDENCIA, só é gravado após a finalização da OS.

                            if (docReincidencia is null) continue;

                            document.IsDocReincidencia = true;

                            var docOsAnterior = await repoDocProducao.selectFirst(d =>
                                d.Num_Obra == docReincidencia.NumObraAnterior &&
                                d.CodCt == docReincidencia.CodCtAnterior &&
                                d.Num_Obra == docReincidencia.NumObraAnterior &&
                                d.Projeto == docReincidencia.ProjetoAnterior);

                            var equipeOsOriginal = await repoEquipe.selectFirst(e =>
                                e.Matricula == docOsAnterior.Equipe);

                            if (equipeOsOriginal is null) continue;

                            document.CodFornecedorAnterior = equipeOsOriginal.Cod_Fornecedor;
                            document.Equipe_Anterior = equipeOsOriginal.Matricula;
                        }
                    }
                    #endregion

                    tecnicosRoteirizacao.AddRange(await RoteirizarFiltrarTecnicos(value, dao, configuracoes, validacao));

                    var filaTecnicos = new Queue<TecnicoRoteirizar>(tecnicosRoteirizacao.Select(p => (TecnicoRoteirizar)p.Clone()).ToList());
                    IRoterizar roteirizar = GetEstrategiaRoteirizacao(configuracoes.configGerais, configuracoes.diaConf, configuracoes.turnos, filaTecnicos, false, false);

                    var executarRoteirizacao = roteirizar.Run(validacao.documentosValidos.Select(p => (DocumentoRoteirizar)p.Clone()).ToList()); //executa a roteizacao padrao

                    if (resultado.documentosRoteirizados == null && resultado.documentosNaoRoteirizados == null)
                    {
                        resultado.documentosRoteirizados = new List<DocumentoRoteirizar>();
                        resultado.documentosNaoRoteirizados = new List<DocumentoRoteirizar>();
                    }

                    resultado.documentosRoteirizados.AddRange(executarRoteirizacao.documentosRoteirizados);
                    resultado.documentosNaoRoteirizados.AddRange(executarRoteirizacao.documentosNaoRoteirizados);
                    resultado.documentosNaoRoteirizados.AddRange(validacao.documentosNaoValidos);

                    await dao.GravarRoteirizacao(validacao.documentosValidos, resultado.documentosRoteirizados, resultado.documentosNaoRoteirizados, configuracoes.configGerais, value.Usuario, configuracoes.diaConf, tecnicosRoteirizacao, value.Objeto.simulacao);

                    if (value.Objeto.simulacao == false)
                    {
                        await Pos_Roteirizacao(sessionT5, resultado.documentosRoteirizados, resultado.documentosNaoRoteirizados,string.IsNullOrEmpty(value.Usuario) ? User.Identity.GetUserId() : value.Usuario);
                    }

                    var notificacao = new NotificacoesApiController().EnviaNotificacoesServico(23,null, null, null, null, Convert.ToString(resultado.documentosRoteirizados.Count()));

                    if (resultado.documentosNaoRoteirizados.Any())
                    {
                        retorno.Ok = false;
                        retorno.Mensagem = String.Format("{0} Serviços Roteirizados / {1} Serviços Não Roteirizados",
                                                        resultado.documentosRoteirizados.Count(), resultado.documentosNaoRoteirizados.Count());
                    }
                    else
                        retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;
                    retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                }
            }

            return retorno;
        }

        private async Task<List<TecnicoRoteirizar>> RoteirizarFiltrarTecnicos(ParamWebApi<RoteirizarServicosModel> value, RoteirizacaoDAO dao,
            (RoteirizacaoDiaConf diaConf, Configuracoes_Gerais configGerais, List<Turno_ServicoAniel> turnos) configuracoes,
            (List<DocumentoRoteirizar> documentosValidos, List<DocumentoRoteirizar> documentosNaoValidos, List<long> ids_agendamento_ignorar) validacao, String equipe = null)
        {
            List<TecnicoRoteirizar> tecnicos = await dao.GetTecnicoRoteirizacaoSql(configuracoes.configGerais, configuracoes.diaConf, equipe);

            if (value.Objeto.EquipesConfiguradas == true)
            {
                var equipes = (await dao.GetUsuarioTecnicoRoteirizacao(value.Usuario)).Select(p => p.Equipe);
                if (equipes.Count() > 0)
                    tecnicos = tecnicos.FindAll(p => equipes.Contains(p.Equipe));
            }

            tecnicos = tecnicos.Select(p =>
            {
                p.JaAgendados = p.JaAgendados.Where(y => validacao.ids_agendamento_ignorar.Contains(y.Id) == false).ToList();
                return p;
            }).ToList();


            tecnicos = tecnicos.OrderBy(p =>
                Guid.NewGuid()).Select((tecnico, index) =>
            {
                tecnico.Posicao_Fila = index + 1;
                return tecnico;
            }).ToList();

            return tecnicos;
        }

        private async Task<List<DocumentoRoteirizar>> GetDocumentosRoteirizacao(RoteirizarServicosModel request, Configuracoes_Gerais config, RoteirizacaoDAO dao)
        {

            List<DocumentoRoteirizar> documentos = new List<DocumentoRoteirizar>();

            if (request.ids.Count > 0)
            {
                List<DocumentoProducao_GetDocumento> chaves_documento = request.ids.Select(chave =>
                {

                    String[] _chaves = chave.Split('|');
                    String CodCt = _chaves[0];
                    String Projeto = _chaves[1];
                    String Num_Obra = _chaves[2];
                    Int32 Num_Doc = Convert.ToInt32(_chaves[3]);

                    return new DocumentoProducao_GetDocumento
                    {
                        CodCt = CodCt,
                        Num_Doc = Num_Doc,
                        Num_Obra = Num_Obra,
                        Projeto = Projeto
                    };

                }).ToList();

                foreach (var listaQuebrada in ListService.GetListaQuebrada(chaves_documento, 200))
                {
                    documentos.AddRange(await dao.GetDocumentoRoteirizacaoSql(listaQuebrada.Value, null, null, config));
                }
            }
            else if (String.IsNullOrEmpty(request.Equipe) == false)
            {
                documentos.AddRange(await dao.GetDocumentoRoteirizacaoSql(new List<DocumentoProducao_GetDocumento>(), request.Equipe, request.data, config));
            }

            return documentos;
        }

        private (List<DocumentoRoteirizar> documentosValidos, List<DocumentoRoteirizar> documentosAgendadoCliente, List<Int64> ids_agendamento_ignorar) ValidaAgendamento(List<DocumentoRoteirizar> documentos, RoteirizacaoDiaConf DiaConf, Configuracoes_Gerais config, RoteirizacaoDAO dao)
        {
            List<DocumentoRoteirizar> documentosAgendadoCliente = new List<DocumentoRoteirizar>();
            List<Int64> ids_agendamento_remover = new List<Int64>();
            List<Int64> ids_agendamento_ignorar = new List<Int64>();
            List<DocumentoRoteirizar> documentos_removerEquipe = new List<DocumentoRoteirizar>();

            var ids_agendamento = documentos.Select(p => p.Id_Agendamento).Where(p => p.HasValue).ToList();
            List<Agendamento_EquipeAniel> agendamentos = dao.GetAgendamentos(ids_agendamento).Result;


            if (DiaConf.Alterar_DataHora_SemAgendamentoAniel)
            {
                documentos = documentos.Select(documento => ValidaAgendamentoAniel(documento, agendamentos, ids_agendamento_ignorar, ids_agendamento_remover,
                documentos_removerEquipe, DiaConf, documentosAgendadoCliente, config)).Where(p => p != null).ToList();
            }
            else
            {
                documentos = documentos.Select(documento => ValidaDocumentoDataHoraMaxima(documento, documentosAgendadoCliente,
                                                            ids_agendamento_ignorar,
                                                            ids_agendamento_remover, documentos_removerEquipe, DiaConf)).Where(p => p != null).ToList();
            }

            if (DiaConf.GravarDados)
            {
                dao.ApagarAgendamento(ids_agendamento_remover).Wait();
                dao.Documento_RemoverEquipe(documentos_removerEquipe).Wait();
            }

            return (documentos, documentosAgendadoCliente, ids_agendamento_ignorar);
        }

        private DocumentoRoteirizar ValidaAgendamentoAniel(DocumentoRoteirizar documento, List<Agendamento_EquipeAniel> agendamentos,
                                                            List<Int64> ids_agendamento_ignorar,
                                                            List<Int64> ids_agendamento_remover, List<DocumentoRoteirizar> documentos_removerEquipe,
                                                            RoteirizacaoDiaConf DiaConf, List<DocumentoRoteirizar> documentosAgendadoCliente, Configuracoes_Gerais config)
        {
            var dataHoraDoc = (DateTime?)null;

            if (documento.Data_Maxima.HasValue)
            {
                dataHoraDoc = documento.Data_Maxima.Value;
                if (documento.Hora_Maxima.HasValue)
                    dataHoraDoc = dataHoraDoc?.AddHours(documento.Hora_Maxima.Value.Hour).AddMinutes(documento.Hora_Maxima.Value.Minute);
            }

            Int64? agendamento = documento.Id_Agendamento;
            var AgendamentoNh = agendamentos.Find(p => p.Id == agendamento);
            if (AgendamentoNh != null)
            {

                documento.OrigemAgendamento = AgendamentoNh.Origem.HasValue ? AgendamentoNh.Origem.ToString() : null;

                if (!dataHoraDoc.HasValue)
                {
                    dataHoraDoc = AgendamentoNh.DataHora_Inicio;
                }

                if (dataHoraDoc < this.HoraRoteirizacao || (AgendamentoNh.Origem == OrigemAgendamentoOS.Roteirizacao || AgendamentoNh.Origem == OrigemAgendamentoOS.Integracao))
                {
                    documento.Equipe = null;
                    ids_agendamento_ignorar.Add(agendamento.Value);
                    documentos_removerEquipe.Add(documento);
                    documento.Id_Agendamento = null;

                }
                else if (((AgendamentoNh.Origem == OrigemAgendamentoOS.Agendamento) || (AgendamentoNh.Origem == OrigemAgendamentoOS.Mobile) || AgendamentoNh.Origem == OrigemAgendamentoOS.Manual) && config.Exige_Agendamento_Servico)
                {
                    // se esta tentando roteirizar para um dia diferente do que foi agendado
                    if (DiaConf.Data.Date != documento.Data_Maxima)
                    {
                        if (dataHoraDoc >= this.HoraRoteirizacao)
                        {
                            documento.Log = $"{documento.Num_Obra} - Documento agendado pelo cliente. Fazer o reagendamento com o cliente para prosseguir com a roteirização.Para realizar o reagendamento, por favor conduza-se ao Acompanhamento de Serviço";
                            documentosAgendadoCliente.Add(documento);
                            return null;
                        }
                        else
                        {
                            ids_agendamento_ignorar.Add(agendamento.Value);
                            documento.Id_Agendamento = null;
                            documentos_removerEquipe.Add(documento);

                        }
                    }
                    else
                    {

                        //documento.Id_Agendamento = null;
                        documento.Id_Agendamento = agendamento;
                        documentos_removerEquipe.Add(documento);
                        ids_agendamento_ignorar.Add(agendamento.Value);
                        documento.Data_hora_ini_agendamento = AgendamentoNh.DataHora_Inicio;
                        documento.Data_hora_fim_agendamento = AgendamentoNh.DataHora_Fim;
                        documento.DeslocamentoMinutos = Convert.ToInt16(AgendamentoNh.Deslocamento);
                    }

                }
                else
                {
                    ids_agendamento_ignorar.Add(agendamento.Value);
                    documentos_removerEquipe.Add(documento);
                    documento.Id_Agendamento = null;
                }

            }

            return documento;
        }

        private DocumentoRoteirizar ValidaDocumentoDataHoraMaxima(DocumentoRoteirizar documento, List<DocumentoRoteirizar> documentosAgendadoCliente,
                                                                  List<Int64> ids_agendamento_ignorar,
                                                                  List<Int64> ids_agendamento_remover, List<DocumentoRoteirizar> documentos_removerEquipe, RoteirizacaoDiaConf DiaConf)
        {

            Int64? agendamento = documento.Id_Agendamento;

            var dataHoraDoc = (DateTime?)null;

            if (documento.Data_Maxima.HasValue)
            {
                dataHoraDoc = documento.Data_Maxima.Value;
                if (documento.Hora_Maxima.HasValue)
                    dataHoraDoc = dataHoraDoc?.AddHours(documento.Hora_Maxima.Value.Hour).AddMinutes(documento.Hora_Maxima.Value.Minute);
            }

            if (dataHoraDoc.HasValue)
            {
                if (dataHoraDoc < this.HoraRoteirizacao)
                {
                    documento.Equipe = null;
                    if (agendamento.HasValue)
                    {
                        ids_agendamento_ignorar.Add(agendamento.Value);
                    }
                    documentos_removerEquipe.Add(documento);
                    documento.Id_Agendamento = null;
                }
                else if (DiaConf.Data.Date != documento.Data_Maxima)
                {
                    if (dataHoraDoc >= this.HoraRoteirizacao)
                    {
                        documento.Log = $"{documento.Num_Obra} - Documento agendado pelo cliente. Fazer o reagendamento com o cliente para prosseguir com a roteirização.Para realizar o reagendamento, por favor conduza-se ao Acompanhamento de Serviço";
                        documentosAgendadoCliente.Add(documento);
                        return null;
                    }
                }
                else
                {
                    if (agendamento.HasValue)
                    {
                        ids_agendamento_ignorar.Add(agendamento.Value);
                    }
                    //documentos_removerEquipe.Add(documento);
                    documento.Data_hora_ini_agendamento = dataHoraDoc;
                    documento.Data_hora_fim_agendamento = dataHoraDoc?.Add(documento.Tempo_Execucao.Value);
                    documento.DeslocamentoMinutos = 0;
                }

            }

            return documento;
        }

        private (List<DocumentoRoteirizar> documentosValidos, List<DocumentoRoteirizar> documentosNaoValidos) ValidaCriterios(List<DocumentoRoteirizar> documentos, Configuracoes_Gerais config)
        {

            List<DocumentoRoteirizar> documentosNaoValidos = new List<DocumentoRoteirizar>();

            documentos = documentos.Select(documento =>
            {
                //if (documento.Data_Prod.HasValue)
                //    throw new Exception($"Serviço {documento.Num_Obra} já encerrado.");
                //if (!documento.Cod_Tipo_Imovel.HasValue)
                //    throw new Exception(String.Format(RsCadastros.DADOS_NAO_INFORMADOS_NA_OS, RsCadastros.TIPO_IMOVEL, documento.Num_Obra));
                if (documento.StatusOld == StatusExecucaoAniel.Cancelado)
                {
                    documento.Log = (String.Format(RsCadastros.SERVICO_CANCELADO, documento.Num_Obra));
                    documentosNaoValidos.Add(documento);
                    return null;
                }

                if (documento.StatusOld == StatusExecucaoAniel.Fechada_Produtiva)
                {
                    documento.Log = (String.Format(RsCadastros.SERVICO_FECHADO_PRODUTIVO, documento.Num_Obra));
                    documentosNaoValidos.Add(documento);
                    return null;
                }

                if (documento.StatusOld == StatusExecucaoAniel.Fechada_Improdutiva)
                {
                    documento.Log = (String.Format(RsCadastros.SERVICO_FECHADO_IMPRODUTIVO, documento.Num_Obra));
                    documentosNaoValidos.Add(documento);
                    return null;
                }

                if (documento.StatusOld == StatusExecucaoAniel.Atendimento_Finalizado)
                {
                    documento.Log = (String.Format(RsCadastros.SERVICO_ATENDIMENTO_FINALIZADO, documento.Num_Obra));
                    documentosNaoValidos.Add(documento);
                    return null;
                }

                if (documento.StatusOld == StatusExecucaoAniel.Atendimento_Iniciado_Reiniciado ||
                    documento.StatusOld == StatusExecucaoAniel.Reiniciado_Atendimento)
                {
                    documento.Log = (String.Format(RsCadastros.SERVICOS_EM_EXECUCAO, documento.Num_Obra));
                    documentosNaoValidos.Add(documento);
                    return null;
                }

                if (documento.StatusOld == StatusExecucaoAniel.OS_Em_deslocamento)
                {
                    documento.Log = (String.Format(RsCadastros.OS_EM_DESLOCAMENTO, documento.Num_Obra));
                    documentosNaoValidos.Add(documento);
                    return null;
                }

                if (documento.StatusOld == StatusExecucaoAniel.Atendimento_Finalizado)
                {
                    documento.Log = (String.Format(RsCadastros.SERVICO_ATENDIMENTO_FINALIZADO, documento.Num_Obra));
                    documentosNaoValidos.Add(documento);
                    return null;
                }

                if (!documento.Cod_Tipo_Serv.HasValue && config.Criterio_Habilidade)
                {
                    documento.Log = (String.Format(RsCadastros.DADOS_NAO_INFORMADOS_NA_OS, RsCadastros.TIPO_SERVICO, documento.Num_Obra));
                    documentosNaoValidos.Add(documento);
                    return null;
                }
                else if (String.IsNullOrEmpty(documento.Grupo_Area) && config.Criterio_Area)
                {
                    documento.Log = (String.Format(RsCadastros.DADOS_NAO_INFORMADOS_NA_OS, RsCadastros.GRUPO_AREA, documento.Num_Obra));
                    documentosNaoValidos.Add(documento);
                    return null;
                }
                else if (String.IsNullOrEmpty(documento.Area) && config.Criterio_Area)
                {
                    documento.Log = (String.Format(RsCadastros.DADOS_NAO_INFORMADOS_NA_OS, RsCadastros.AREA, documento.Num_Obra));
                    documentosNaoValidos.Add(documento);
                    return null;
                }
                else if (!documento.Cod_Localidade.HasValue && config.Criterio_Area)
                {
                    documento.Log = (String.Format(RsCadastros.DADOS_NAO_INFORMADOS_NA_OS, RsCadastros.LOCALIDADE, documento.Num_Obra));
                    documentosNaoValidos.Add(documento);
                    return null;
                }

                return documento;
            }).Where(p => p != null).ToList();

            return (documentos, documentosNaoValidos);

        }

        [HttpGet()]
        [Route("api/Painel_RoteirizacaoApi/GerarKmRota")]
        public async Task<EquipeQuilometragemAniel> GetKmRota(String equipe, String data)
        {
            using (var ISession = HibernateUtil.GetSessionT5())
            using (var equipeQuilometragemRepo = new Conexao<EquipeQuilometragemAniel>(ISession))
            {
                EquipeQuilometragemAniel equipeQuilometragemAniel = await equipeQuilometragemRepo.selectFirst(p => p.Equipe.Matricula == equipe && p.Data == Convert.ToDateTime(data));
                return equipeQuilometragemAniel;
            }
        }

        [HttpPost()]
        [Route("api/Painel_RoteirizacaoApi/RelatorioKm")]
        public async Task<DataTableResponse<RelatorioKmModel>> RelatorioKm(RelatorioKmDataTableRequest request)
        {
            using (var ISession = HibernateUtil.GetSessionT5())
            using (var documentoRepo = new Conexao<Documento_ProducaoAniel>(ISession))
            using (var equipeRepo = new Conexao<EquipeAniel>(ISession))
            using (var funcionarioRepo = new Conexao<FuncionarioAniel>(ISession))
            using (var localidadeRepo = new Conexao<LocalidadeAniel>(ISession))
            using (var horarioEquipeRepo = new Conexao<Horario_Equipe>(ISession))
            {
                List<RelatorioKmModel> retorno = new List<RelatorioKmModel>();
                IList<EquipeAniel> equipesAniel = await equipeRepo.select(p => p.Connect == "S");

                if (String.IsNullOrEmpty(request.Equipe) == false)
                {
                    equipesAniel = equipesAniel.Where(p => p.Matricula == request.Equipe).ToList();
                }

                if (request.Supervisor.Count() > 0)
                {
                    equipesAniel = equipesAniel.Where(p => request.Supervisor.Contains(p.CodSuperv)).ToList();
                }

                if (String.IsNullOrEmpty(request.Monitor) == false)
                {
                    equipesAniel = equipesAniel.Where(p => p.Monitor == request.Monitor).ToList();
                }

                var data = Convert.ToDateTime(request.Data);
                foreach (var item in equipesAniel)
                {

                    var funcionario = (await funcionarioRepo.selectFirst(p => p.Num_Fre == item.Num_Fre));
                    if (funcionario.Situacao != SituacaoFuncionarioAniel.Demitido)
                    {
                        var escala = await horarioEquipeRepo.selectFirst(p => p.Equipe.Matricula == item.Matricula && p.Data == data);
                        var disponibilidade = escala != null ? !escala.Indisponivel : true;

                        if (disponibilidade)
                        {
                            var supervisor = (await funcionarioRepo.selectFirst(p => p.Num_Fre == item.CodSuperv));
                            var equipeQuilometragem = await GetKmRota(item.Matricula, request.Data) ?? (new EquipeQuilometragemTask()).PreencherQuilometragem(item.Matricula, Convert.ToDateTime(request.Data), false)?.Objeto;

                            if (request.ApenasTecnicosComOs && equipeQuilometragem?.Quantidade_OS == 0)
                            {
                                continue;
                            }

                            var perfilAgendamento = escala?.PerfilAgendamento ?? item.PerfilAgendamento;
                            if(String.IsNullOrEmpty(request.Natureza) == false)
                            {
                                if (perfilAgendamento.Naturezas.FirstOrDefault(p => Convert.ToString(p.Natureza?.Id) == request.Natureza) == null)
                                {
                                    continue;
                                }
                            }

                            if (String.IsNullOrEmpty(request.GrupoArea) == false)
                            {
                                if (perfilAgendamento.Areas.FirstOrDefault(p => Convert.ToString(p.Escritorio) == request.GrupoArea) == null)
                                {
                                    continue;
                                }
                            }

                            if (String.IsNullOrEmpty(request.GrupoArea) == false && String.IsNullOrEmpty(request.Area) == false)
                            {
                                if (perfilAgendamento.Areas.FirstOrDefault(p => Convert.ToString(p.Escritorio) == request.GrupoArea && Convert.ToString(p.Area) == request.Area) == null)
                                {
                                    continue;
                                }
                            }

                            retorno.Add(new RelatorioKmModel
                            {
                                Matricula = item.Matricula,
                                Nome = item.Nome,
                                Supervisor = $"{supervisor?.Num_Fre} - {supervisor?.Nome}",
                                DistanciaPlanejada = equipeQuilometragem?.DistanciaPlanejada ?? 0,
                                DistanciaRealizada = equipeQuilometragem?.DistanciaRealizada ?? 0,
                                Quantidade_OS = equipeQuilometragem.Quantidade_OS,
                                PerfilAgendamento = escala?.PerfilAgendamento?.Descricao ?? item.PerfilAgendamento?.Descricao,
                            });
                        }
                    }
                }

                return new DataTableResponse<RelatorioKmModel>
                {
                    draw = request.draw,
                    //recordsTotal = recordsTotal,
                    recordsFiltered = retorno.Count(),
                    data = retorno.OrderBy(p => p.Nome).ToList(),
                    error = ""
                };
            }
        }

        [HttpGet()]
        [Route("api/Painel_RoteirizacaoApi/GerarEquipeQuilometragem")]
        public async Task<DataTableResponse<RelatorioKmModel>> RelatorioKm(string dataInicio, string dataFim)
        {
            using (var ISession = HibernateUtil.GetSessionT5())
            using (var documentoRepo = new Conexao<Documento_ProducaoAniel>(ISession))
            using (var equipeRepo = new Conexao<EquipeAniel>(ISession))
            using (var localidadeRepo = new Conexao<LocalidadeAniel>(ISession))
            {
                List<RelatorioKmModel> retorno = new List<RelatorioKmModel>();
                IList<EquipeAniel> equipesAniel = await equipeRepo.select(p => p.Connect == "S");
                foreach (var item in equipesAniel)
                {
                    DateTime dateInicio = Convert.ToDateTime(dataInicio);
                    DateTime dateFim = Convert.ToDateTime(dataFim);
                    foreach (DateTime day in EachDay(dateInicio, dateFim))
                    {
                        var equipeQuilometragem = await GetKmRota(item.Matricula, day.ToString("dd/MM/yyyy")) ?? (new EquipeQuilometragemTask()).PreencherQuilometragem(item.Matricula, day,true)?.Objeto;
                    }
                }

                return new DataTableResponse<RelatorioKmModel>
                {
                    draw = 0,
                    //recordsTotal = recordsTotal,
                    recordsFiltered = retorno.Count(),
                    data = retorno.OrderBy(p => p.Nome).ToList(),
                    error = ""
                };
            }
        }

        public IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        private (List<DocumentoRoteirizar> documentosValidos, List<DocumentoRoteirizar> documentosNaoValidos, List<Int64> ids_agendamento_ignorar) ValidaDocumentos(List<DocumentoRoteirizar> documentos, RoteirizacaoDiaConf DiaConf, Configuracoes_Gerais config, RoteirizacaoDAO dao)
        {
            List<DocumentoRoteirizar> documentosNaoValidos = new List<DocumentoRoteirizar>();
            List<DocumentoRoteirizar> documentosValidos = new List<DocumentoRoteirizar>(documentos);

            var resultado_criterio = ValidaCriterios(documentosValidos, config);
            documentosValidos = resultado_criterio.documentosValidos;
            documentosNaoValidos = resultado_criterio.documentosNaoValidos;

            var resultado_agendamento = ValidaAgendamento(documentosValidos, DiaConf, config, dao);
            documentosValidos = resultado_agendamento.documentosValidos;
            documentosNaoValidos = documentosNaoValidos.Concat(resultado_agendamento.documentosAgendadoCliente).ToList();


            return (documentosValidos, documentosNaoValidos, resultado_agendamento.ids_agendamento_ignorar);
        }

        private async Task<(RoteirizacaoDiaConf diaConf, Configuracoes_Gerais configGerais, List<Turno_ServicoAniel> turnos)> GetConfiguracoesRoteirizacao(ISession session, ISession sessionT5, RoteirizarServicosModel request)
        {
            using (var repoFeriado = new Conexao<Feriado>(session))
            using (var repoConfiguracoes = new Conexao<Configuracoes_Gerais>(session))
            using (var repoHorario = new Conexao<Horario_Connect>(sessionT5))
            using (var repoTurno = new Conexao<Turno_ServicoAniel>(sessionT5))
            {
                Horario_Connect horario_Connect = await repoHorario.selectFirst(p => p.Status == Sinapse.Model.enumerados.Status_Default.Ativo && p.Padrao == true && p.Tipo == Aniel.T5.Model.enumerado.Tipo_Hora_Connect.Horario_Agendamento);
                Configuracoes_Gerais config = await repoConfiguracoes.selectFirst();
                List<Horario_Connect> horarios_servico = (repoHorario.select(p => p.Tipo == Aniel.T5.Model.enumerado.Tipo_Hora_Connect.Horario_Servico && p.Status == Sinapse.Model.enumerados.Status_Default.Ativo).Result).ToList();
                DateTime? data_roteirizar = null;
                List<Turno_ServicoAniel> turnos = (await repoTurno.select()).ToList();

                try
                {
                    data_roteirizar = Convert.ToDateTime(request.data);
                    if (data_roteirizar == null)
                        throw new Exception("Data informada para roteirização é invalida");
                }
                catch
                {
                    throw new Exception("Data informada para roteirização é invalida");
                }

                if (horario_Connect == null)
                {
                    throw new Exception("Horario de Agendamento Padrão não definido no sistema ou Inativado");
                }
                if (horarios_servico.Find(p => p.Padrao == true) == null)
                {
                    throw new Exception("Horario de Serviço Padrão não definido no sistema");
                }
                if (!config.Tempo_Execucao_Padrao.HasValue || config.Tempo_Execucao_Padrao.Value.TimeOfDay == new TimeSpan(0))
                {
                    throw new Exception("Tempo de Execução não definido no sistema");
                }
                if (!config.Tempo_Medio_Deslocamento.HasValue)
                {
                    throw new Exception("Tempo Medio de Deslocamento não definido no sistema");
                }

                Feriado feriado = await repoFeriado.selectFirst(p => p.Data == data_roteirizar);

                return (new RoteirizacaoDiaConf
                {
                    Data = data_roteirizar.Value,
                    feriado = feriado != null,
                    NumeroMaxAtendimentoTecnico = request.numero_max_atendimento.GetValueOrDefault(100),
                    Alterar_DataHora_SemAgendamentoAniel = request.alterar_DataHora_SemAgendamentoAniel.GetValueOrDefault(true),
                    horarios_servico = horarios_servico,
                    GravarDados = request.simulacao ? false : true,
                    Encaixe = request.encaixe,
                    EncaixeAutomatico = request.encaixeAutomatico
                }, config, turnos);

            }

        }

        public IRoterizar GetEstrategiaRoteirizacao(Configuracoes_Gerais config, RoteirizacaoDiaConf DiaConf, List<Turno_ServicoAniel> turnos, Queue<TecnicoRoteirizar> tecnicos, bool ignorarCriteriosHabilidadeArea, bool manterOrdemRota)
        {

            var listaAusencia = new Gestao_EquipeApiController().GetHorarioAusentePainelServicos(Convert.ToDateTime(DiaConf.Data).Date, null, null).Result.ToList();

            
            if (DiaConf.Encaixe)
            {
                return new RoteirizarEncaixe(config, DiaConf, turnos, tecnicos, this.HoraRoteirizacao ?? DateTime.Now, ignorarCriteriosHabilidadeArea, manterOrdemRota, listaAusencia);
        }
            else
            {
                return new RoteirizarDistribuirEquipe(config, DiaConf, turnos, tecnicos, this.HoraRoteirizacao ?? DateTime.Now, ignorarCriteriosHabilidadeArea, manterOrdemRota, listaAusencia);
            }

            

        }

        public async Task Pos_Roteirizacao(ISession session,
                                           List<DocumentoRoteirizar> documentoRoteirizados,
                                           List<DocumentoRoteirizar> documentoNaoRoteirizados,
                                           string usuario)
        {
            List<TecnicoRoteirizar> tecnicos = new List<TecnicoRoteirizar>();
            using (var repoEquipe = new Conexao<EquipeAniel>(session))
            {
                foreach (var documento in documentoRoteirizados)
                {
                    try
                    {
                        var dataHoraDoc = documento.Data_Maxima;
                        if (documento.Hora_Maxima.HasValue)
                            dataHoraDoc = dataHoraDoc.Value.Add(documento.Hora_Maxima.Value.TimeOfDay);

                        await new Documento_ProducaoApiController().InsertLog_Lanc_Servico(session, documento.CodCt, documento.Projeto,
                                                                                               documento.Num_Obra, documento.Num_Doc.Value,
                                                                                               String.Format("Agendamento do serviço {0} (Roteirização)", documento.Data_hora_ini_agendamento?.Date.ToString("dd/MM/yyyy")),
                                                                                               (dataHoraDoc.HasValue ? dataHoraDoc.Value.ToString("dd/MM/yyyy HH:mm") : null),
                                                                                               usuario,
                                                                                           documento.Data_hora_ini_agendamento.Value.AddMinutes(documento.DeslocamentoMinutos), null, documento.Data_hora_ini_agendamento.Value.AddMinutes(documento.DeslocamentoMinutos).ToString(), null);

                        if (documento.StatusOld != StatusExecucaoAniel.Aberta_Aguard_Atendimento)
                        {
                            await new Documento_ProducaoApiController().InsertLog_Lanc_Servico(session,
                                                                        documento.CodCt,
                                                                        documento.Projeto,
                                                                        documento.Num_Obra,
                                                                        documento.Num_Doc.GetValueOrDefault(0),
                                                                        "Mudança de Status",
                                                                        HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(documento.StatusOld.Value),
                                                                        usuario,
                                                                        null,
                                                                        null,
                                                                        HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(StatusExecucaoAniel.Aberta_Aguard_Atendimento),
                                                                        null
                                                                        );
                        }

                        if (documento.Equipe != documento.Equipe_Anterior)
                        {
                            if (!String.IsNullOrEmpty(documento.Tecnico_Anterior.Device_Id))
                            {
                                tecnicos.Add(documento.Tecnico_Anterior);
                            }
                            if (!String.IsNullOrEmpty(documento.Tecnico.Device_Id))
                            {
                                tecnicos.Add(documento.Tecnico_Anterior);
                            }

                            EquipeAniel equipe = !String.IsNullOrEmpty(documento.Equipe) ? await repoEquipe.selectFirst(p => p.Matricula == documento.Equipe) : null;
                            await new Documento_ProducaoApiController().InsertLog_Lanc_Servico(session, documento.CodCt, documento.Projeto, documento.Num_Obra, documento.Num_Doc.GetValueOrDefault(0),
                                        "Equipe", documento.Equipe_Anterior, usuario, null, null, $"{documento.Equipe} - {equipe?.Nome}", null);

                        }
                    }
                    catch (Exception ex)
                    {
                        CONSTANTES.log4Net.Error(ex.Message, ex);
                    }
                }

                foreach (var documento in documentoNaoRoteirizados)
                {
                    try
                    {
                        var dataHoraDoc = documento.Data_Maxima;
                        if (documento.Hora_Maxima.HasValue)
                            dataHoraDoc = dataHoraDoc.Value.Add(documento.Hora_Maxima.Value.TimeOfDay);

                        await new Documento_ProducaoApiController().InsertLog_Lanc_Servico(session, documento.CodCt, documento.Projeto,
                                                                                               documento.Num_Obra, documento.Num_Doc.Value,
                                                                                               $"Roteirização Sem Sucesso {DateTime.Now.ToString("dd/MM/yyyy HH:mm")}",
                                                                                               null,
                                                                                               User.Identity.GetUserId(),
                                                                                               null, null, null, null);



                        if (documento.Equipe != documento.Equipe_Anterior)
                        {
                            if (!String.IsNullOrEmpty(documento.Tecnico_Anterior?.Device_Id))
                            {
                                tecnicos.Add(documento.Tecnico_Anterior);
                            }
                            if (!String.IsNullOrEmpty(documento.Tecnico?.Device_Id))
                            {
                                tecnicos.Add(documento.Tecnico_Anterior);
                            }

                            if (documento.Tecnico != null)
                            {
                                await new Documento_ProducaoApiController().InsertLog_Lanc_Servico(session, documento.CodCt, documento.Projeto, documento.Num_Obra, documento.Num_Doc.GetValueOrDefault(0),
                                            "Equipe", $"{documento.Equipe} - {documento?.Tecnico?.Nome}", User.Identity.GetUserId(), null, null, documento.Equipe_Anterior, null);
                            }
                            else
                            {
                                await new Documento_ProducaoApiController().InsertLog_Lanc_Servico(session,
                                                                                           documento.CodCt,
                                                                                           documento.Projeto,
                                                                                           documento.Num_Obra,
                                                                                           documento.Num_Doc.GetValueOrDefault(0),
                                                                                           "Remoção de Equipe",
                                                                                           documento.Tecnico_Anterior == null ? String.Empty : $"{documento.Equipe_Anterior} - {documento.Tecnico_Anterior?.Nome}",
                                                                                           User.Identity.GetUserId(),
                                                                                           null,
                                                                                           null,
                                                                                           String.Empty,
                                                                                           null);

                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        CONSTANTES.log4Net.Error(ex.Message, ex);
                    }
                }

                var lista_device_id = tecnicos.Select(p => p.Device_Id).Distinct().ToList();

                foreach (var item in lista_device_id)
                {
                    MobileService.NotificarSmartphone(item, String.Empty, "Houve mudanças em sua rotina.",
                                                        "Atualize a sua lista de atividades", "br.com.sinapse.touch.MSG_RESINC_OS",
                                                        "", "", "", "");
                }
            }
        }

        [HttpGet()]
        [Route("api/Painel_RoteirizacaoApi/CompactarRota")]
        public async Task CompactarRota(CompactarRotaModel request)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            using (var repoDocumentoProducao = new Conexao<Documento_ProducaoAniel>(sessionT5))
            using (var repoAgendamento = new Conexao<Agendamento_EquipeAniel>(sessionT5))
            using (var repoHorario = new Conexao<Horario_Connect>(sessionT5))
            using (var repoTurnos = new Conexao<Turno_ServicoAniel>(sessionT5))
            using (RoteirizacaoDAO dao = new RoteirizacaoDAO())
            {
                Configuracoes_Gerais config = await repoConfig.selectFirst();
                List<Turno_ServicoAniel> turnos = (await repoTurnos.select()).ToList();
                Documento_ProducaoAniel documento = await repoDocumentoProducao.selectFirst(p => p.CodCt == request.CodCt && p.Projeto == request.Projeto && p.Num_Obra == request.NumObra && p.Num_Doc == request.NumDoc);
                Agendamento_EquipeAniel agendamento = await repoAgendamento.selectFirst(p => p.CodCt == request.CodCt && p.Projeto == request.Projeto && p.Num_Obra == request.NumObra && p.Num_Doc == request.NumDoc);

                if (documento.Data_Prod.Value.Add(documento.Hora_Prod.Value.TimeOfDay) < agendamento.DataHora_Fim)
                {

                    List<DocumentoRoteirizar> documentos = GetDocumentosRoteirizacao(new RoteirizarServicosModel
                    {
                        data = documento.Data_Maxima.Value.ToString("dd/MM/yyyy"),
                        Equipe = documento.Equipe

                    }, config, dao).Result.Where(p => p.StatusOld != StatusExecucaoAniel.Fechada_Produtiva &&
                                                      p.StatusOld != StatusExecucaoAniel.Fechada_Improdutiva &&
                                                      p.Hora_Maxima.HasValue &&
                                                      p.Hora_Maxima.Value.TimeOfDay > documento.Hora_Prod.Value.TimeOfDay).ToList();

                    var roteirizacaoConf = new RoteirizacaoDiaConf
                    {
                        horarios_servico = repoHorario.select(p => p.Tipo == Aniel.T5.Model.enumerado.Tipo_Hora_Connect.Horario_Servico && p.Status == Sinapse.Model.enumerados.Status_Default.Ativo).Result.ToList(),
                        Alterar_DataHora_SemAgendamentoAniel = true,
                        GravarDados = false,
                        NumeroMaxAtendimentoTecnico = 100,
                        Data = Convert.ToDateTime(documento.Data_Maxima),

                    };

                    var validacao = ValidaDocumentos(documentos, roteirizacaoConf, config, dao);

                    List<TecnicoRoteirizar> tecnicos = await dao.GetTecnicoRoteirizacaoSql(config, new RoteirizacaoDiaConf
                    {
                        horarios_servico = repoHorario.select(p => p.Tipo == Aniel.T5.Model.enumerado.Tipo_Hora_Connect.Horario_Servico && p.Status == Sinapse.Model.enumerados.Status_Default.Ativo).Result.ToList(),
                        Alterar_DataHora_SemAgendamentoAniel = true,
                    }, documento.Equipe);

                    tecnicos = tecnicos.Where(p => p.Equipe == documento.Equipe).ToList();

                    tecnicos = tecnicos.Select(p =>
                    {
                        p.JaAgendados = p.JaAgendados.Where(y => validacao.ids_agendamento_ignorar.Contains(y.Id) == false).ToList();
                        return p;
                    }).ToList();

                    roteirizacaoConf.GravarDados = request.SalvarRota;
                    IRoterizar roteirizar = GetEstrategiaRoteirizacao(config, roteirizacaoConf, turnos, new Queue<TecnicoRoteirizar>(tecnicos), true, true);
                    var resultado = roteirizar.Run(new List<DocumentoRoteirizar>(validacao.documentosValidos));

                    if (request.SalvarRota)
                    {
                        await dao.GravarRoteirizacao(validacao.documentosValidos, resultado.documentosRoteirizados, new List<DocumentoRoteirizar>(), config, documento.Equipe, roteirizacaoConf, tecnicos);
                        await Pos_Roteirizacao(sessionT5, resultado.documentosRoteirizados, resultado.documentosNaoRoteirizados, User.Identity.GetUserId());
                    }
                }
            }
        }

        [HttpGet()]
        [Route("api/Painel_RoteirizacaoApi/CompactarRotaEncaixe")]
        public async Task<CompactarRotaRetorno> CompactarRotaEncaixe(CompactarRotaEncaixeModel request)
        {
            var retorno = new CompactarRotaRetorno();
            retorno.Ok = true;
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
                using (var repoDocumentoProducao = new Conexao<Documento_ProducaoAniel>(sessionT5))
                using (var repoAgendamento = new Conexao<Agendamento_EquipeAniel>(sessionT5))
                using (var repoHorario = new Conexao<Horario_Connect>(sessionT5))
                using (var repoTurnos = new Conexao<Turno_ServicoAniel>(sessionT5))
                using (RoteirizacaoDAO dao = new RoteirizacaoDAO())
                {
                    Configuracoes_Gerais config = await repoConfig.selectFirst();
                    List<Turno_ServicoAniel> turnos = (await repoTurnos.select()).ToList();

                    request.DataHoraFimEncaixe = Convert.ToDateTime(request.DataHoraFimEncaixe).ToString("dd/MM/yyyy HH:mm");
                    request.DataHoraFimAntigoAgendamento = Convert.ToDateTime(request.DataHoraFimAntigoAgendamento).ToString("dd/MM/yyyy HH:mm");

                    List<DocumentoRoteirizar> documentos = GetDocumentosRoteirizacao(new RoteirizarServicosModel
                    {
                        data = Convert.ToDateTime(request.DataHoraFimAntigoAgendamento).ToString("dd/MM/yyyy"),
                        Equipe = request.Equipe

                    }, config, dao).Result.Where(p => p.StatusOld != StatusExecucaoAniel.Fechada_Produtiva &&
                                                      p.StatusOld != StatusExecucaoAniel.Fechada_Improdutiva &&
                                                      p.Num_Obra != request.NumObraEncaixe &&
                                                      p.Hora_Maxima.HasValue &&
                                                      p.Hora_Maxima.Value.TimeOfDay >= Convert.ToDateTime(request.DataHoraFimAntigoAgendamento).TimeOfDay).
                                                      OrderBy(p => p.Hora_Maxima).ToList();

                    if (config.Exige_Agendamento_Servico == true && config.Considerar_Agendamento_Periodo == false)
                        documentos = documentos.TakeWhile(p => p.OrigemAgendamento != "0").ToList();


                    var documentosOriginais = documentos.Select(p => (DocumentoRoteirizar)p.Clone()).ToList();
                    foreach (var item in documentosOriginais)
                    {
                        var agendamentoOriginal = await repoAgendamento.selectFirst(p => p.CodCt == item.CodCt &&
                                                         p.Projeto == item.Projeto &&
                                                         p.Num_Doc == item.Num_Doc &&
                                                         p.Num_Obra == item.Num_Obra);

                        item.Data_hora_ini_agendamento = agendamentoOriginal.DataHora_Inicio;

                    }

                    var roteirizacaoConf = new RoteirizacaoDiaConf
                    {
                        horarios_servico = repoHorario.select(p => p.Tipo == Aniel.T5.Model.enumerado.Tipo_Hora_Connect.Horario_Servico && p.Status == Sinapse.Model.enumerados.Status_Default.Ativo).Result.ToList(),
                        Alterar_DataHora_SemAgendamentoAniel = true,
                        GravarDados = false,
                        NumeroMaxAtendimentoTecnico = 100,
                        Data = Convert.ToDateTime(request.DataHoraFimEncaixe).Date,

                    };

                    var validacao = ValidaDocumentos(documentos, roteirizacaoConf, config, dao);


                    List<TecnicoRoteirizar> tecnicos = await dao.GetTecnicoRoteirizacaoSql(config, new RoteirizacaoDiaConf
                    {
                        horarios_servico = repoHorario.select(p => p.Tipo == Aniel.T5.Model.enumerado.Tipo_Hora_Connect.Horario_Servico && p.Status == Sinapse.Model.enumerados.Status_Default.Ativo).Result.ToList(),
                        Alterar_DataHora_SemAgendamentoAniel = true,
                    }, request.Equipe);

                    tecnicos = tecnicos.Where(p => p.Equipe == request.Equipe).ToList();

                    tecnicos = tecnicos.Select(p =>
                    {
                        p.JaAgendados = p.JaAgendados.Where(y => validacao.ids_agendamento_ignorar.Contains(y.Id) == false).ToList();
                        return p;
                    }).ToList();

                    roteirizacaoConf.GravarDados = request.SalvarRota;
                    IRoterizar roteirizar = GetEstrategiaRoteirizacao(config, roteirizacaoConf, turnos, new Queue<TecnicoRoteirizar>(tecnicos), true, true);
                    var resultado = roteirizar.Run(new List<DocumentoRoteirizar>(validacao.documentosValidos));

                    if (request.SalvarRota && validacao.documentosValidos.Count == resultado.documentosRoteirizados.Count)
                    {
                        resultado.documentosRoteirizados = resultado.documentosRoteirizados.Where(p => p.Data_hora_ini_agendamento >= documentosOriginais.Find(y => p.Num_Obra == y.Num_Obra).Data_hora_ini_agendamento).ToList();
                        await dao.GravarRoteirizacao(validacao.documentosValidos, resultado.documentosRoteirizados, new List<DocumentoRoteirizar>(), config, request.Equipe, roteirizacaoConf, tecnicos);
                        await Pos_Roteirizacao(sessionT5, resultado.documentosRoteirizados, resultado.documentosNaoRoteirizados,User.Identity.GetUserId());
                    }

                    retorno.QtdOsCompactar = validacao.documentosValidos.Count;
                    retorno.QtdOsRoteirizada = resultado.documentosRoteirizados.Count;

                }
            }
            catch
            {
                retorno.Ok = false;
            }

            return retorno;
        }
        
        [HttpGet()]
        [Route("api/Painel_RoteirizacaoApi/DebugLog")]
        public async Task<CompactarRotaRetorno> DebugLog(long IdLog)
        {
            var retorno = new CompactarRotaRetorno();
            retorno.Ok = true;
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                using (var repoLog = new Conexao<Log_RoteirizacaoAniel>(sessionLog))
                using (var repoTurno = new Conexao<Turno_ServicoAniel>(sessionT5))
                {

                    Log_RoteirizacaoAniel log = await repoLog.selectFirst(p => p.Id == IdLog);
                    var turnos = (await repoTurno.select()).ToList();

                    var config = JsonService.DeserializeObject<Configuracoes_Gerais>(log.ConfiguracoesGerais);
                    var roteirizacaoConf = JsonService.DeserializeObject<RoteirizacaoDiaConf>(log.ConfiguracoesRoteirizacao);
                    var tecnicos = JsonService.DeserializeObject<List<TecnicoRoteirizar>>(log.TecnicosRoteirizacao);
                    var documentosValidos = JsonService.DeserializeObject<List<DocumentoRoteirizar>>(log.DocumentosValidos);
                    this.HoraRoteirizacao = log.DataHoraCriacao;

                    IRoterizar roteirizar = GetEstrategiaRoteirizacao(config, roteirizacaoConf, turnos, new Queue<TecnicoRoteirizar>(tecnicos), false, false);
                    var resultado = roteirizar.Run(documentosValidos);
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
            }

            return retorno;
        }

        [HttpGet()]
        [Route("api/Painel_RoteirizacaoApi/ExecutarProgramacaoRoteirizacao")]
        public async Task<CompactarRotaRetorno> ExecutarProgramacaoRoteirizacao(long request)
        {
            var retorno = new CompactarRotaRetorno();
            retorno.Ok = true;
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repoProgramacaoRoteirizacao = new Conexao<ProgramacaoRoteirizacaoAutomatica>(sessionT5))
                using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
                using (var repoDocumentoProducao = new Conexao<Documento_ProducaoAniel>(sessionT5))
                using (var repoAgendamento = new Conexao<Agendamento_EquipeAniel>(sessionT5))
                using (var repoHorario = new Conexao<Horario_Connect>(sessionT5))
                using (var repoTurnos = new Conexao<Turno_ServicoAniel>(sessionT5))
                using (RoteirizacaoDAO dao = new RoteirizacaoDAO())
                {

                    ProgramacaoRoteirizacaoAutomatica programacaoRoteirizacaoAutomatica = await repoProgramacaoRoteirizacao.selectFirst(p => p.Id == request);
                    var configuracoes = await GetConfiguracoesRoteirizacao(session, sessionT5, new RoteirizarServicosModel
                    {
                        simulacao = false,
                        data = DateTime.Now.ToString("dd/MM/yyyy"),
                        alterar_DataHora_SemAgendamentoAniel = programacaoRoteirizacaoAutomatica.AlterarDataHoraSemAgendamentoAniel,
                        numero_max_atendimento = programacaoRoteirizacaoAutomatica.NumeroMaximoAtendimento,

                    });

                    var documentos = await dao.GetDocumentoRoteirizacaoProgramada(programacaoRoteirizacaoAutomatica);
                    var validacao = ValidaDocumentos(documentos, configuracoes.diaConf, configuracoes.configGerais, dao);
                    List<TecnicoRoteirizar> tecnicos = await dao.GetTecnicoRoteirizacaoSql(configuracoes.configGerais, configuracoes.diaConf, null);

                    tecnicos = tecnicos.Select(p =>
                    {
                        p.JaAgendados = p.JaAgendados.Where(y => validacao.ids_agendamento_ignorar.Contains(y.Id) == false).ToList();
                        return p;
                    }).ToList();

                    tecnicos = tecnicos.OrderBy(p => Guid.NewGuid()).Select((tecnico, index) =>
                    {
                        tecnico.Posicao_Fila = index + 1;
                        return tecnico;

                    }).ToList();

                    var filaTecnicos = new Queue<TecnicoRoteirizar>(tecnicos.Select(p => (TecnicoRoteirizar)p.Clone()).ToList());
                    IRoterizar roteirizar = GetEstrategiaRoteirizacao(configuracoes.configGerais, configuracoes.diaConf, configuracoes.turnos, filaTecnicos, false, false);
                    var resultado = roteirizar.Run(validacao.documentosValidos.Select(p => (DocumentoRoteirizar)p.Clone()).ToList());
                    resultado.documentosNaoRoteirizados = resultado.documentosNaoRoteirizados.Concat(validacao.documentosNaoValidos).ToList();

                    await dao.GravarRoteirizacao(validacao.documentosValidos, resultado.documentosRoteirizados, resultado.documentosNaoRoteirizados, configuracoes.configGerais, "ADMIN", configuracoes.diaConf, tecnicos, false);
                    await Pos_Roteirizacao(sessionT5, resultado.documentosRoteirizados, resultado.documentosNaoRoteirizados,User.Identity.GetUserId());
                    retorno.QtdOsCompactar = validacao.documentosValidos.Count;
                    retorno.QtdOsRoteirizada = resultado.documentosRoteirizados.Count;
                }
            }
            catch
            {
                retorno.Ok = false;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Painel_RoteirizacaoApi/SalvarUsuarioTecnicoRoteirizacao")]
        public async Task<PostApiRetorno> SalvarUsuarioTecnicoRoteirizacao(SalvarUsuarioTecnicoRoteirizacaoRequestModel request)
        {
            var retorno = new PostApiRetorno();
            retorno.Ok = true;
            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repoUsuarioTecnicoRoteirizacao = new Conexao<UsuarioTecnicoRoteirizacao>(sessionT5))
                {
                    var registrosAntigos = await repoUsuarioTecnicoRoteirizacao.select(p => p.Usuario == request.Usuario);

                    foreach (var item in registrosAntigos)
                    {
                        await repoUsuarioTecnicoRoteirizacao.Remove(item);
                    }

                    foreach (var item in request.Equipes)
                    {
                        var tecnicoRoteirizacao = new UsuarioTecnicoRoteirizacao
                        {
                            Equipe = item,
                            Usuario = request.Usuario
                        };

                        await repoUsuarioTecnicoRoteirizacao.Save(tecnicoRoteirizacao);
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException?.Message;
            }

            return retorno;
        }
    }
}