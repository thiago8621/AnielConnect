using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Models.T5;
using Aniel.Connect.Repository.T5.Cliente;
using Aniel.Connect.Repository.T5.CodigoEncerramento;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNet.Identity;
using NHibernate;
using OpenXmlPowerTools;
using Resources;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerado;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi.T5
{
    [Authorize]
    public class Material_TarefaApiController : BaseApiController
    {


        [HttpPost()]
        [Route("api/Material_TarefaApi/GetEquipamentos")]
        public async Task<DataTableResponse<Material_TarefaListaModel>> GetEquipamentos(Material_Tarefa_DataTableRequest request)
        {
            List<Material_TarefaListaModel> retorno = new List<Material_TarefaListaModel>();
            IOrderedEnumerable<Material_TarefaListaModel> listaOrdenada = null;

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                Boolean Exibe_RDOs = (await new Conexao<Configuracoes_Gerais>(session).selectFirst()).ExibeRDOs;

                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    //Select
                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.CODMAT AS CODIGO, B.DESCRICAO, A.CODCPL ");
                        sql.AppendLine("FROM TB_MATERIAL_TAREFA A ");
                        sql.AppendLine("INNER JOIN TB_MATERIAL B ");
                        sql.AppendLine("ON(A.CODMAT = B.CODMAT AND B.SUBCOD = 'S') ");
                        sql.AppendLine("INNER JOIN TB_DOCUMENTO_PRODUCAO C ");
                        sql.AppendLine("ON(C.CODCT = A.CODCT AND C.PROJETO = A.PROJETO AND ");
                        sql.AppendLine("C.NUM_OBRA = A.NUM_OBRA AND C.NUM_DOC = A.NUM_DOC) ");

                        sql.AppendLine("WHERE  C.ID_CLIENTE = @ID_CLIENTE AND B.CLASSIFICACAO != 'R' AND C.ID_CLIENTE_PLANOprodutoCONNECT = @ID_CLIENTEPLANOproduto "); //

                        if (!Exibe_RDOs)
                            sql.AppendLine(" AND COALESCE(C.SIGLA_MODULO, '') <> 'MCO' ");

                        parametros.Add("@ID_CLIENTE", request.Id_Cliente);
                        parametros.Add("@ID_CLIENTEPLANOproduto", request.Id_ClientePlanoProduto);

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new Material_TarefaListaModel
                                {
                                    Codigo = Convert.ToString(reader["CODIGO"]),
                                    Descricao = Convert.ToString(reader["DESCRICAO"]),
                                    Codigo_Complementar = Convert.ToString(reader["CODCPL"])
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
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
                                listaOrdenada = retorno.OrderBy(p => p.Codigo);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Codigo);
                            break;
                        }
                    default:
                        {
                            listaOrdenada = retorno.OrderBy(p => p.Codigo);
                            break;
                        }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<Material_TarefaListaModel>
            {
                draw = request.draw,
                data = listaOrdenada,
                error = ""
            };

        }

        [HttpPost()]
        [Route("api/Material_TarefaApi/GetMaterialAplicado")]
        public async Task<DataTableResponse<Material_AplicadoListaModel>> GetMaterialAplicado(Material_Aplicado_DataTableRequest request)
        {

            List<Material_AplicadoListaModel> retorno = new List<Material_AplicadoListaModel>();
            IOrderedEnumerable<Material_AplicadoListaModel> listaOrdenada = null;

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                String SIGLA_MODULO = await GetSiglaModulo(session, request.Num_Obra, Convert.ToInt32(request.Num_Doc), request.Projeto, request.CodCt);
                String CHAVE = GetCampoChave(SIGLA_MODULO);

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                parametros.Add("@CODCT", request.CodCt);
                parametros.Add("@PROJETO", request.Projeto);
                parametros.Add("@NUM_OBRA", String.Format("{0}%", request.Num_Obra.Split('/')[0]));
                parametros.Add("@NUM_DOC", request.Num_Doc);


                if (SIGLA_MODULO == "MCO")
                    parametros.Add("@CHAVE", String.Format("{0}", request.Num_Obra));
                else
                    parametros.Add("@CHAVE", String.Format("{0}%", request.Num_Obra.Split('/')[0]));

                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    StringBuilder sql = new StringBuilder($@" SELECT A.CODCT, A.PROJETO, A.NUM_OBRA, A.NUM_DOC, A.TAREFA, A.DATA_APLIC, A.CODMAT, B.DESCRICAO, A.CODCPL, B.UNID, C.NOME, B.VALOR, 
                                                           A.QUANTIDADE, (B.VALOR * A.QUANTIDADE) AS TOTAL, A.QUANT_REMOV, (B.VALOR * A.QUANT_REMOV) AS TOTAL_REMOV
                                                           FROM TB_MATERIAL_TAREFA A 
                                                           INNER JOIN TB_MATERIAL B 
                                                           ON(A.CODMAT = B.CODMAT) 
                                                           INNER JOIN TB_EQUIPE C 
                                                           ON(A.EQUIPE = C.EQUIPE) 
                                                           WHERE A.CODCT = @CODCT AND A.PROJETO = @PROJETO AND 
                                                           A.{CHAVE} LIKE @CHAVE AND A.NUM_DOC = @NUM_DOC ");


                    if (request.Retirada)
                        sql.AppendLine("AND B.CLASSIFICACAO IN('R','B') AND A.QUANT_REMOV > 0");
                    else
                        sql.AppendLine("AND B.CLASSIFICACAO IN('I','B') AND A.QUANTIDADE > 0");

                    if (request.Exibir_Vinculado)
                    {
                        //Lógica de Exibir_Vinculo
                    }

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new Material_AplicadoListaModel
                            {
                                CodCt = Convert.ToString(reader["CODCT"]),
                                Projeto = Convert.ToString(reader["PROJETO"]),
                                Num_Doc = Convert.ToInt32(reader["NUM_DOC"]),
                                Num_Obra = Convert.ToString(reader["NUM_OBRA"]),
                                Tarefa = Convert.ToString(reader["TAREFA"]),
                                Data_Aplic = Convert.ToString(reader["DATA_APLIC"]),
                                Codigo = Convert.ToString(reader["CODMAT"]),
                                Codigo_Complementar = Convert.ToString(reader["CODCPL"]),
                                Descricao = Convert.ToString(reader["DESCRICAO"]),
                                Equipe = Convert.ToString(reader["NOME"]),
                                Quantidade = request.Retirada ? String.Format("{0:N2}", Convert.ToDouble(reader["QUANT_REMOV"] != DBNull.Value ? reader["QUANT_REMOV"] : 0)) :
                                                                String.Format("{0:N2}", Convert.ToDouble(reader["QUANTIDADE"] != DBNull.Value ? reader["QUANTIDADE"] : 0)),
                                Unidade = Convert.ToString(reader["UNID"]),
                                Valor_Unitario = String.Format("{0:N2}", Convert.ToDouble(reader["VALOR"] != DBNull.Value ? reader["VALOR"] : 0)),
                                Valor_Total = request.Retirada ? String.Format("{0:N2}", Convert.ToDouble(reader["TOTAL_REMOV"] != DBNull.Value ? reader["TOTAL_REMOV"] : 0)) :
                                                                 String.Format("{0:N2}", Convert.ToDouble(reader["TOTAL"] != DBNull.Value ? reader["TOTAL"] : 0))
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
                    case 1:     // Descricao
                        {
                            if (sortDirection == "asc")
                                listaOrdenada = retorno.OrderBy(p => p.Codigo);
                            else
                                listaOrdenada = retorno.OrderByDescending(p => p.Codigo);
                            break;
                        }
                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<Material_AplicadoListaModel>
            {
                draw = request.draw,
                recordsFiltered = retorno.Count,
                data = listaOrdenada,
                error = ""
            };

        }

        public async Task<String> GetSiglaModulo(ISession session, String num_Obra, Int32 num_Doc, String projeto, String cod_Ct)
        {

            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                parametros.Add("@CODCT", cod_Ct);
                parametros.Add("@PROJETO", projeto);
                parametros.Add("@NUM_OBRA", String.Format("{0}%", num_Obra.Split('/')[0]));
                parametros.Add("@NUM_DOC", num_Doc);

                sql.AppendLine(" SELECT D.SIGLA_MODULO FROM TB_DOCUMENTO_PRODUCAO D")
                   .AppendLine("WHERE  D.CODCT = @CODCT AND D.PROJETO = @PROJETO AND")
                   .AppendLine("D.NUM_OBRA LIKE @NUM_OBRA AND D.NUM_DOC = @NUM_DOC");

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        return Convert.ToString(reader["SIGLA_MODULO"]);
                    }
                }
            }

            return String.Empty;

        }

        public String GetCampoChave(string sigla_modulo)
        {
            return sigla_modulo == "MCO" ? "TAREFA" : "NUM_OBRA";
        }

        [HttpPost()]
        [Route("api/Material_TarefaApi/RelatorioMateriaisAplicadosRetiradosAnalitico")]
        public async Task<DataTableResponse<RelatorioMateriaisAnaliticosModel>> RelatorioMateriaisAplicadosRetiradosAnalitico(Relatorio_Materiais_Analiticos_DataTableRequest request)
        {
            var retorno = new List<RelatorioMateriaisAnaliticosModel>();
            int recordsTotal = 0;
            string mensagemErro = String.Empty;

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (ISession sessionNh = HibernateUtil.GetSessionT5())
                {
                    // ID do cliente / nome do cliente
                    var clienteCache = new Dictionary<int, string>();
                    // Código do contrato / (Código de encerramento / Descricao)
                    var codEncerramentoCache = new Dictionary<string, Dictionary<string, string>>();

                    IClienteAnielRepository repoCliente = new ClienteAnielRepository(sessionNh);
                    ICodigoEncerramentoAnielRepository repoCodEncerramento = new CodigoEncerramentoAnielRepository(sessionNh);

                    var businessCliente = new ClienteAnielBusiness(repoCliente);
                    var businessCodEncerramento = new CodigoEncerramentoAnielBusiness(repoCodEncerramento);

                    #region SQL From e filtros

                    var sqlFrom = new StringBuilder();
                    var parametros = new Dictionary<string, object>();

                    sqlFrom.AppendLine("FROM TB_MATERIAL_TAREFA A");

                    sqlFrom.AppendLine("INNER JOIN TB_MATERIAL B");
                    sqlFrom.AppendLine("ON(A.CODMAT = B.CODMAT)");

                    sqlFrom.AppendLine("LEFT JOIN TB_EQUIPE C");
                    sqlFrom.AppendLine("ON(A.EQUIPE = C.EQUIPE)");

                    sqlFrom.AppendLine("LEFT JOIN TB_DOCUMENTO_PRODUCAO D");
                    sqlFrom.AppendLine("ON(A.CODCT = D.CODCT AND A.PROJETO = D.PROJETO AND A.NUM_OBRA = D.NUM_OBRA AND A.NUM_DOC = D.NUM_DOC)");

                    sqlFrom.AppendLine("LEFT JOIN TB_TIPO_SERVICO_EQUIPE F");
                    sqlFrom.AppendLine("ON(COALESCE(D.COD_TIPO_SERV,A.COD_TIPO_SERV) = F.COD_TIPO_SERV)");

                    sqlFrom.AppendLine("LEFT JOIN TB_SUB_TIPO_SERVICO G");
                    sqlFrom.AppendLine("ON(F.COD_TIPO_SERV = G.COD_TIPO_SERV AND COALESCE(D.COD_SUB_TIPO_SERV,A.COD_SUB_TIPO_SERV) = G.COD_SUB_TIPO_SERV)");

                    sqlFrom.AppendLine("LEFT JOIN TB_SEGMENTO_SERVICO S");
                    sqlFrom.AppendLine("ON(S.CODIGO = D.SEGMENTO)");

                    sqlFrom.AppendLine("LEFT JOIN TB_CLIENTE R");
                    sqlFrom.AppendLine("ON(R.COD_CLIENTE = A.ID_CLIENTE)");

                    sqlFrom.AppendLine("WHERE B.CLASSIFICACAO IN('R','I','B')");

                    if (!String.IsNullOrEmpty(request.Data_Inicio) && !String.IsNullOrEmpty(request.Data_Fim))
                    {
                        if (request.Tipo_Data == "agendamento")
                            sqlFrom.AppendLine("AND (D.DATA_MAXIMA BETWEEN @DATA_INICIO AND @DATA_FIM)");
                        else if (request.Tipo_Data == "digitacao")
                            sqlFrom.AppendLine("AND (A.DIA BETWEEN @DATA_INICIO AND @DATA_FIM)");
                        else if (request.Tipo_Data == "movimentacao")
                            sqlFrom.AppendLine("AND (A.DATA_APLIC BETWEEN @DATA_INICIO AND @DATA_FIM)");

                        parametros.Add("@DATA_INICIO", Convert.ToDateTime(request.Data_Inicio).ToString("dd/MM/yyyy 00:00"));
                        parametros.Add("@DATA_FIM", Convert.ToDateTime(request.Data_Fim).ToString("dd/MM/yyyy 23:59"));
                    }

                    if (!String.IsNullOrEmpty(request.Contrato))
                    {
                        sqlFrom.AppendLine("AND (A.CODCT = @CODCT)");
                        parametros.Add("@CODCT", request.Contrato);

                        if (!String.IsNullOrEmpty(request.Projeto))
                        {
                            sqlFrom.AppendLine("AND (A.PROJETO = @PROJETO)");
                            parametros.Add("@PROJETO", request.Projeto);
                        }
                    }

                    if (request.Cod_Tipo_Serv.HasValue)
                    {
                        sqlFrom.AppendLine("AND (F.COD_TIPO_SERV = @COD_TIPO_SERV)");
                        parametros.Add("@COD_TIPO_SERV", request.Cod_Tipo_Serv);
                    }

                    if (!String.IsNullOrEmpty(request.Equipe))
                    {
                        sqlFrom.AppendLine("AND (A.EQUIPE = @EQUIPE)");
                        parametros.Add("@EQUIPE", request.Equipe);
                    }

                    if (!String.IsNullOrEmpty(request.Supervisor))
                    {
                        sqlFrom.AppendLine("AND (C.COD_SUPERV = @SUPERVISOR)");
                        parametros.Add("@SUPERVISOR", request.Supervisor);
                    }

                    if (request.Cod_Cliente.HasValue)
                    {
                        sqlFrom.AppendLine("AND (A.ID_CLIENTE = @ID_CLIENTE)");
                        parametros.Add("@ID_CLIENTE", request.Cod_Cliente);
                    }

                    if (!String.IsNullOrEmpty(request.CodMat))
                    {
                        sqlFrom.AppendLine("AND (A.CODMAT = @CODMAT)");
                        parametros.Add("@CODMAT", request.CodMat);
                    }
                    if (request.ClassificacaoMaterial.HasValue)
                    {
                        sqlFrom.AppendLine("AND (B.CLASSIFICACAO = @CLASSIFICACAO_MATERIAL)");
                        parametros.Add("@CLASSIFICACAO_MATERIAL", GetCodigoAniel(request.ClassificacaoMaterial.Value));
                    }

                    if (!String.IsNullOrEmpty(request.FamiliaMaterial))
                    {
                        sqlFrom.AppendLine("AND (B.FAMILIA = @FAMILIA)");
                        parametros.Add("@FAMILIA", request.FamiliaMaterial);
                    }

                    if (!String.IsNullOrEmpty(request.GrupoMaterial))
                    {
                        sqlFrom.AppendLine("AND (B.COD_SUBGRUPO_MAT = @GRUPO)");
                        parametros.Add("@GRUPO", request.GrupoMaterial);
                    }

                    if (request.Id_NaturezaProducao.HasValue)
                    {
                        sqlFrom.AppendLine("AND (F.COD_NATUREZA = @NATUREZA)");
                        parametros.Add("@NATUREZA", request.Id_NaturezaProducao);
                    }

                    #endregion

                    using (var com = session.Connection.CreateCommand())
                    {
                        var sql = new StringBuilder();

                        sql.AppendLine("SELECT COUNT(*) AS QTD_REGISTROS");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (reader.Read())
                            {
                                recordsTotal = Convert.ToInt32(reader["QTD_REGISTROS"]);
                            }
                        }
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {

                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine("SELECT A.CODCT,A.PROJETO, A.NUM_OBRA,A.NUM_DOC, D.DATA_MAXIMA + COALESCE(D.HORA_MAXIMA, CAST('00:00' AS TIME)) AS DATAHORA_AGENDAMENTO,");
                        sql.AppendLine("R.CNPJ,A.CODMAT, B.DESCRICAO AS DESCRICAO_MATERIAL, A.CODCPL, B.UNID,R.CNPJ,COALESCE(R.FANTASIA,R.RAZAO_SOCIAL) AS NOME_CLIENTE,");
                        sql.AppendLine("B.VALOR, A.QUANTIDADE AS QUANTIDADE_APLICADA,");
                        sql.AppendLine("A.QUANT_REMOV AS QUANTIDADE_RETIRADA, (B.VALOR * A.QUANTIDADE) AS TOTAL_APLICADO,");
                        sql.AppendLine("0 AS TOTAL_RETIRADO, A.COD_TIPO_SERV,F.DESCRICAO AS DESCRICAO_TIPO_SERV,");
                        sql.AppendLine("D.STATUS_EXECUCAO,A.USUARIO,A.DIA,A.HORA,D.COD_ENCERRAMENTO,");
                        sql.AppendLine("A.EQUIPE, C.NOME AS NOME_EQUIPE, A.ID_CLIENTE, D.COD_ENCERRAMENTO, S.DESCRICAO AS SEGMENTO, G.DESCRICAO AS DESCRICAO_SUB_TIPO_SERV, G.COD_SUB_TIPO_SERV");
                        sql.AppendLine(sqlFrom.ToString());

                        //Busca com paginacao
                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new RelatorioMateriaisAnaliticosModel
                                {
                                    Contrato_aniel = Convert.ToString(reader["CODCT"]),
                                    Projeto = Convert.ToString(reader["PROJETO"]),
                                    Contrato_cliente = Convert.ToString(reader["NUM_DOC"]),
                                    Cod_equipe = Convert.ToString(reader["EQUIPE"]),
                                    Nome_equipe = Convert.ToString(reader["NOME_EQUIPE"]),
                                    Cpf_cnpj_cliente = Convert.ToString(reader["CNPJ"]),
                                    Id_cliente = Convert.ToString(reader["ID_CLIENTE"]),
                                    Os = Convert.ToString(reader["NUM_OBRA"]).Split('/')[0],
                                    Status_os = reader["STATUS_EXECUCAO"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(Convert.ToString(reader["STATUS_EXECUCAO"])) : String.Empty,
                                    Cod_encerramento = Convert.ToString(reader["COD_ENCERRAMENTO"]),
                                    Data_servico = reader["DATAHORA_AGENDAMENTO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_AGENDAMENTO"]).ToString("dd/MM/yyyy HH:mm") : String.Empty,
                                    Cod_tipo_serv = Convert.ToString(reader["COD_TIPO_SERV"]),
                                    Desc_tipo_serv = Convert.ToString(reader["DESCRICAO_TIPO_SERV"]),
                                    Cod_sub_tipo_serv = Convert.ToString(reader["COD_SUB_TIPO_SERV"]),
                                    desc_sub_tipo_serv = Convert.ToString(reader["DESCRICAO_SUB_TIPO_SERV"]),
                                    Segmento = Convert.ToString(reader["SEGMENTO"]),
                                    Cod_material = Convert.ToString(reader["CODMAT"]),
                                    Nome_material = Convert.ToString(reader["DESCRICAO_MATERIAL"]),
                                    Unidade = Convert.ToString(reader["UNID"]),
                                    Num_serie = Convert.ToString(reader["CODCPL"]),
                                    Qtde_aplicada = Convert.ToString(reader["QUANTIDADE_APLICADA"]),
                                    Qtde_removida = Convert.ToString(reader["QUANTIDADE_RETIRADA"]),
                                    Valor_unitario = String.Format("{0:N2}", Convert.ToDouble(reader["VALOR"] != DBNull.Value ? reader["VALOR"] : 0)),
                                    Total_aplicado = String.Format("{0:N2}", Convert.ToDouble(reader["TOTAL_APLICADO"] != DBNull.Value ? reader["TOTAL_APLICADO"] : 0)),
                                    Total_removido = String.Format("{0:N2}", Convert.ToDouble(reader["TOTAL_RETIRADO"] != DBNull.Value ? reader["TOTAL_RETIRADO"] : 0)),
                                    Hora = reader["HORA"] != DBNull.Value ? ((TimeSpan)(reader["HORA"])).ToString(@"hh\:mm") : String.Empty,
                                    Data = reader["DIA"] != DBNull.Value ? Convert.ToDateTime(reader["DIA"]).ToString("dd/MM/yyyy") : String.Empty,
                                    Usuario = Convert.ToString(reader["USUARIO"]),
                                    Nome_cliente = Convert.ToString(reader["NOME_CLIENTE"])
                                };

                                // Contrato não está presente no cache
                                if (!codEncerramentoCache.ContainsKey(obj.Contrato_aniel))
                                {
                                    var descricaoEncerramentoCache = new Dictionary<string, string>();

                                    descricaoEncerramentoCache.Add(
                                        obj.Cod_encerramento,
                                        businessCodEncerramento.GetDescricao(obj.Cod_encerramento, obj.Contrato_aniel)
                                    );

                                    codEncerramentoCache.Add(obj.Contrato_aniel, descricaoEncerramentoCache);
                                }
                                // Contrato está presente no cache mas não possui a descrição do encerramento
                                else if (!codEncerramentoCache[obj.Contrato_aniel].ContainsKey(obj.Cod_encerramento))
                                {
                                    codEncerramentoCache[obj.Contrato_aniel].Add(
                                        obj.Cod_encerramento,
                                        businessCodEncerramento.GetDescricao(obj.Cod_encerramento, obj.Contrato_aniel)
                                    );
                                }

                                obj.Descricao_encerramento = codEncerramentoCache[obj.Contrato_aniel][obj.Cod_encerramento];

                                retorno.Add(obj);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mensagemErro = ex.Message;
            }
            
            return new DataTableResponse<RelatorioMateriaisAnaliticosModel>
            {
                data = retorno,
                recordsFiltered = recordsTotal,
                draw = request.draw,
                error = mensagemErro
            };
        }

        [HttpPost()]
        [Route("api/Material_TarefaApi/RelatorioSaldoMaterialCliente")]
        public async Task<DataTableResponse<Relatorio_Saldo_ClienteModel>> RelatorioSaldoMaterialCliente(Relatorio_Saldo_Cliente_DataTableRequest request)
        {
            int recordsTotal = 0;
            List<Relatorio_Saldo_ClienteModel> retorno = new List<Relatorio_Saldo_ClienteModel>();
            using (ISession session = HibernateUtil.GetSessionT5())
            {
                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    StringBuilder sql = new StringBuilder();
                    StringBuilder sqlWhere = new StringBuilder();


                    //if (!String.IsNullOrEmpty(request.Plano))
                    //{
                    //    sqlWhere.AppendLine("AND (G.ID = @CODP)");
                    //    parametros.Add("@CODP", request.Plano);
                    //}
                    if (!String.IsNullOrEmpty(request.Tipo_Pessoa))
                    {
                        sqlWhere.AppendLine("AND (C.TIPO = @TIPO)");
                        parametros.Add("@TIPO", request.Tipo_Pessoa);
                    }

                    //if (!String.IsNullOrEmpty(request.Contrato))
                    //{
                    //    sqlWhere.AppendLine("AND B.CODCT = @CODCT)");
                    //    parametros.Add("@CODCT", request.Contrato);
                    //}

                    if (!String.IsNullOrEmpty(request.CpfCliente))
                    {
                        sqlWhere.AppendLine("AND (C.CNPJ = @CPF_CLIENTE)");
                        parametros.Add("@CPF_CLIENTE", request.CpfCliente);
                    }
                    //if (!String.IsNullOrEmpty(request.Projeto))
                    //{
                    //    sqlWhere.AppendLine("AND (F.PROJETO = @PROJETO)");
                    //    parametros.Add("@PROJETO", request.Projeto);
                    //}

                    if (!String.IsNullOrEmpty(request.CodMat))
                    {
                        sqlWhere.AppendLine("AND D.CODMAT = @CODMAT");
                        parametros.Add("@CODMAT", request.CodMat);
                    }

                    if (!String.IsNullOrEmpty(request.FamiliaMaterial))
                    {
                        sqlWhere.AppendLine("AND FAM.FAMILIA = @FAMILIA");
                        parametros.Add("@FAMILIA", request.FamiliaMaterial);
                    }
                 
                    if (request.ExibirApenasSerializados)
                    {
                        sqlWhere.AppendLine("AND A.CODCPL <> '-' ");
                    }

                    if (!String.IsNullOrEmpty(request.GrupoMaterial))
                    {
                        sqlWhere.AppendLine("AND D.COD_SUBGRUPO_MAT = @COD_SUBGRUPO_MAT");
                        parametros.Add("@COD_SUBGRUPO_MAT", request.GrupoMaterial);
                    }
                    //if (!String.IsNullOrEmpty(request.Cod_Tipo_Serv))
                    //{
                    //    sqlWhere.AppendLine("AND H.DESCRICAO = @COD_TIPO_SERV");
                    //    parametros.Add("@COD_TIPO_SERV", request.GrupoMaterial);
                    //}
                    //if (!String.IsNullOrEmpty(request.Natureza_Servico))
                    //{
                    //    sqlWhere.AppendLine("AND H.DESCRICAO = @COD_TIPO_SERV");
                    //    parametros.Add("@COD_TIPO_SERV", request.Natureza_Servico);
                    //}
                    if (!String.IsNullOrEmpty(request.Localidade))
                    {
                        sqlWhere.AppendLine("AND E.COD_LOCALIDADE = @COD_LOCALIDADE");
                        parametros.Add("@COD_LOCALIDADE", request.Localidade);
                    }
                    if (!String.IsNullOrEmpty(request.Area))
                    {
                        sqlWhere.AppendLine("AND E.COD_AREA  = @COD_AREA");
                        parametros.Add("@COD_AREA", request.Area);
                    }
                    if (!String.IsNullOrEmpty(request.GrupoArea))
                    {
                        sqlWhere.AppendLine("AND E.COD_ESCRITORIO = @GRUPO");
                        parametros.Add("@GRUPO", request.GrupoArea);
                    }
                    if (!String.IsNullOrEmpty(request.Empresa))
                    {
                        sqlWhere.AppendLine("AND I.CODEMPRESA = @CODEMPRESA");
                        parametros.Add("@CODEMPRESA", request.Empresa);
                    }
                    if (!String.IsNullOrEmpty(request.Filial))
                    {
                        sqlWhere.AppendLine("AND I.CODFILIAL = @CODFILIAL");
                        parametros.Add("@CODFILIAL", request.Filial);
                    }

                    if (!String.IsNullOrEmpty(Convert.ToString(request.Id_Categoria_Cliente)))
                    {
                        sqlWhere.AppendLine("AND C.ID_CATEGORIA = @pID_CATEGORIA");
                        parametros.Add("@pID_CATEGORIA", Convert.ToString(request.Id_Categoria_Cliente));
                    }

                    var groupby = String.Empty;

                    sql.AppendLine($@" SELECT * FROM
                                     ( 
                                      SELECT C.COD_CLIENTE, MAX(A.NUM_DOC) AS NUM_DOC,C.CNPJ, D.CODMAT,D.DESCRICAO AS MATERIAL,
                                             MAX(A.CODCPL) AS CODCPL,D.UNID,C.UF,C.RAZAO_SOCIAL,
                                             C.MUNICIPIO,C.TIPO,COALESCE(D.VALOR,0)AS VALOR_UNITARIO, 
                                             COALESCE((SELECT COALESCE(SA.RSALDO,0) 
                                                       FROM SP_CALCULA_SALDO_CLIENTE(COALESCE(C.COD_CLIENTE,0),D.CODMAT,A.CODCPL)SA),0) AS SALDO
                                      FROM TB_MATERIAL_TAREFA A
                                      LEFT JOIN TB_CONTRATO B ON (B.CODCT = A.CODCT)
                                      LEFT JOIN TB_CLIENTE C ON (A.ID_CLIENTE  = C.COD_CLIENTE)
                                      LEFT JOIN TB_MATERIAL D ON (D.CODMAT = A.CODMAT)
                                      LEFT JOIN TB_FAMILIA_MATERIAL FAM ON (D.FAMILIA = FAM.FAMILIA)
                                      LEFT JOIN TB_DOCUMENTO_PRODUCAO E ON (A.CODCT = E.CODCT
                                        		AND A.PROJETO  = E.PROJETO AND A.NUM_OBRA  = E.NUM_OBRA

                                        		AND A.NUM_DOC = E.NUM_DOC)
                                      LEFT JOIN TB_PLANO_PRODUTO G ON  (G.ID = E.ID_PLANOPRODUTO)
                                      LEFT JOIN TB_PROJETO F ON (F.CODCT =A.CODCT AND F.PROJETO = A.PROJETO)
                                      LEFT JOIN TB_TIPO_SERVICO_EQUIPE H  ON(E.COD_TIPO_SERV = H.COD_TIPO_SERV)
                                      LEFT JOIN TB_FUNCIONARIO I ON (A.USUARIO =I.NUM_FRE)
                                      LEFT JOIN TB_EMPRESA J ON (J.CODEMPRESA=I.CODEMPRESA)
                                      LEFT JOIN TB_FILIAL L ON (L.CODEMPRESA =I.CODEMPRESA AND L.CODFILIAL = I.CODFILIAL)
                                      WHERE D.CODMAT IS NOT NULL 
                                      AND D.CLASSIFICACAO = 'I'
                                     {sqlWhere.ToString()}
                                      GROUP BY C.COD_CLIENTE,C.CNPJ,D.CODMAT,
                                    	    D.DESCRICAO,A.CODCPL,D.UNID,
                                            C.UF,C.MUNICIPIO,C.RAZAO_SOCIAL,C.TIPO,COALESCE(D.VALOR,0)
                                    ) WHERE SALDO > 0 ");




                    //Remove a vírgula do groupby, se for nulo ele pegará a variável orderby
                    //Esse processo foi feito para poder exportar os campos dessa variável

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            var saldo = Convert.ToInt64(reader["SALDO"]);
                            var obj = new Relatorio_Saldo_ClienteModel
                            {
                                cod_cliente = Convert.ToString(reader["COD_CLIENTE"]),
                                cpf = Convert.ToString(reader["CNPJ"]),
                                nome = Convert.ToString(reader["RAZAO_SOCIAL"]),
                                contrato = Convert.ToString(reader["NUM_DOC"]),
                                codmat = Convert.ToString(reader["CODMAT"]),
                                material = Convert.ToString(reader["MATERIAL"]),
                                codcpl = Convert.ToString(reader["CODCPL"]),
                                unid = Convert.ToString(reader["UNID"]),
                                saldo = Convert.ToString(reader["SALDO"]),
                                valUnit = Convert.ToDouble(reader["VALOR_UNITARIO"]).ToString("C"),
                                valTotal = (Convert.ToDouble(reader["VALOR_UNITARIO"]) * Convert.ToInt32(reader["SALDO"])).ToString("C")
                            };

                     
                            retorno.Add(obj);

                        }


                        recordsTotal = retorno.Count();
                    }

                }
            }

            return new DataTableResponse<Relatorio_Saldo_ClienteModel>
            {
                draw = request.draw,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };
        }

        [HttpPost()]
        [Route("api/Material_TarefaApi/RelatorioSaldoMaterialPOP")]
        public async Task<DataTableResponse<Relatorio_Saldo_POPModel>> RelatorioSaldoMaterialPOP(Relatorio_Saldo_POP_DataTableRequest request)
        {                                                                                           
            int recordsTotal = 0;                                   
            List<Relatorio_Saldo_POPModel> retorno = new List<Relatorio_Saldo_POPModel>();
            using (ISession session = HibernateUtil.GetSessionT5())
            {
                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    StringBuilder sql = new StringBuilder();
                    StringBuilder sqlWhere = new StringBuilder();

                    if (!String.IsNullOrEmpty(request.Empresa))
                    {
                        sqlWhere.AppendLine(" AND (E.CODEMPRESA = @CODEMPRESA)");
                        parametros.Add("@CODEMPRESA", request.Empresa);
                    }

                    if (!String.IsNullOrEmpty(request.Filial))
                    {
                        sqlWhere.AppendLine(" AND (E.CODFILIAL = @CODFILIAL)");
                        parametros.Add("@CODFILIAL", request.Filial);
                    }
                       if (!String.IsNullOrEmpty(request.Pop))
                    {
                        sqlWhere.AppendLine(" AND (B.ID_POP = @ID_POP)");
                        parametros.Add("@ID_POP", request.Pop);
                    }

                    if (!String.IsNullOrEmpty(request.Contrato))
                    {
                        sqlWhere.AppendLine(" AND (B.CODCT = @CODCT)");
                        parametros.Add("@CODCT", request.Contrato);
                    }


                    if (!String.IsNullOrEmpty(request.Projeto))
                    {
                        sqlWhere.AppendLine(" AND (B.PROJETO = @PROJETO)");
                        parametros.Add("@PROJETO", request.Projeto);
                    }
                 

                    if (!String.IsNullOrEmpty(request.CodMat))
                    {
                        sqlWhere.AppendLine(" AND (A.CODMAT = @CODMAT)");
                        parametros.Add("@CODMAT", request.CodMat);
                    }

                    if (!String.IsNullOrEmpty(request.FamiliaMaterial))
                    {
                        sqlWhere.AppendLine(" AND (D.FAMILIA = @FAMILIA)");
                        parametros.Add("@FAMILIA", request.FamiliaMaterial);
                    }
                    if (!String.IsNullOrEmpty(request.GrupoMaterial))
                    {
                        sqlWhere.AppendLine(" AND (D.COD_SUBGRUPO_MAT = @COD_SUBGRUPO_MAT)");
                        parametros.Add("@COD_SUBGRUPO_MAT", request.GrupoMaterial);
                    }             
                    
                    var groupby = String.Empty;

                    sql.AppendLine($@"SELECT
                                        POP, CODMAT,CODCT, PROJETO, MATERIAL, CODCPL, UNID, VALOR_UNITARIO, SALDO,TOTAL_REG,
                                        COALESCE((SALDO*VALOR_UNITARIO),0) AS VALOR_TOTAL
                                        FROM
                                        (SELECT
                                        B.ID_POP || ' - '||B.DESCRICAO_POP AS POP,
                                        A.CODCT,
                                        A.PROJETO, 
                                        A.CODMAT,
                                        C.MATERIAL,
                                        A.CODCPL,
                                        C.UNID,
                                        D.VALOR AS VALOR_UNITARIO,
                                        COALESCE((SELECT COALESCE(SA.RSALDO,0)
                                                   FROM SP_CALCULA_SALDO_POP(COALESCE(B.ID_POP,0),D.CODMAT,A.CODCPL)SA),0) AS SALDO,
                                        COUNT(*) OVER() AS TOTAL_REG                    

                                        FROM TB_MATERIAL_TAREFA A
                                        INNER JOIN TB_INVENTARIO_POP B ON B.CODCT = A.CODCT AND B.PROJETO = A.PROJETO AND B.ID_POP = A.ID_POP AND B.SEQUENCIAL = A.SEQ_INVENTARIO_POP
                                        INNER JOIN TB_INVENTARIO_POP_MAT C ON C.SEQUENCIAL = B.SEQUENCIAL  AND C.CODMAT = A.CODMAT AND C.CODCPL = A.CODCPL
                                        INNER JOIN TB_MATERIAL D ON D.CODMAT = C.CODMAT
                                        LEFT JOIN TB_FUNCIONARIO E ON (A.USUARIO =E.NUM_FRE)
                                        WHERE D.CODMAT IS NOT NULL 
                                        {sqlWhere.ToString()}
                                        )
                                        GROUP BY  2,3,1,4,5,6,7,8,9,10 ");


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, request.length, request.start))
                    {
                        while (await reader.ReadAsync())
                        {
                            var saldo = Convert.ToInt64(reader["SALDO"]);
                            var objpop = new Relatorio_Saldo_POPModel
                            {
                                pop = Convert.ToString(reader["POP"]),
                                contrato = Convert.ToString(reader[ "CODCT"]),
                                projeto = Convert.ToString(reader[ "PROJETO"]),
                                codmat = Convert.ToString(reader["CODMAT"]),
                                material = Convert.ToString(reader["MATERIAL"]),
                                codcpl = Convert.ToString(reader["CODCPL"]),
                                unid = Convert.ToString(reader["UNID"]),                                                  
                                saldo = Convert.ToString(reader["SALDO"]),
                                valUnit = Convert.ToDouble(reader["VALOR_UNITARIO"]).ToString("C"),
                                valTotal = (Convert.ToDouble(reader["VALOR_UNITARIO"]) * Convert.ToInt32(reader["SALDO"])).ToString("C")
                            };

                            recordsTotal = Convert.ToInt32(reader["TOTAL_REG"]);
                            retorno.Add(objpop);
                        }
                    }
                }
            }


            return new DataTableResponse<Relatorio_Saldo_POPModel>
            {
                draw = request.draw,
                recordsFiltered = recordsTotal,
                data = retorno,
                error = ""
            };
        }


        [HttpPost()]
        [Route("api/Material_TarefaApi/GetComboRelatorioSaldoMaterialCliente")]
        public async Task<ObjCombo<Int64>> GetComboRelatorioSaldoMaterialCliente(RequestObjComboPai<Int64?, Int64?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                try
                {
                    //Filtros
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder(" FROM TB_CLIENTE A ");

                    List<String> condicoes = new List<String>();

                    if (request.idpai != null)
                    {
                        if (request.idpai > 1)
                        {
                            condicoes.Add($"A.TIPO = 'J'");
                        }
                        else
                        {
                            condicoes.Add($"A.TIPO = 'F'");
                        }
                    }

                    if (request.soativos)
                    {
                        condicoes.Add($"COALESCE(A.SITUACAO, 0) = 0");
                    }

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }


                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER(A.RAZAO_SOCIAL) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 255)}%')");
                    }

                    condicoes.Add($"EXISTS (SELECT 1 FROM SP_CALCULA_SALDO_CLIENTE(A.COD_CLIENTE, '-', '[ALL]') HAVING SUM(RSALDO) > 0)");

                    sqlFrom.AppendLine($" WHERE A.RAZAO_SOCIAL != '' AND ( {String.Join(" AND ", condicoes)} ) ");



                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.COD_CLIENTE) AS CO ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.COD_CLIENTE, A.RAZAO_SOCIAL ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" ORDER BY UPPER(A.RAZAO_SOCIAL) ");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {

                                if (!String.IsNullOrEmpty(Convert.ToString(reader["RAZAO_SOCIAL"])))
                                {
                                    retorno.results.Add(new ObjComboResult<Int64>
                                    {
                                        Id = Convert.ToInt64(reader["COD_CLIENTE"]),
                                        Text = Convert.ToString(reader["RAZAO_SOCIAL"])
                                    });

                                }
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


        [HttpPost()]
        [Route("api/Material_TarefaApi/RelatorioMateriaisAplicadosRetiradosSintetico")]
        public async Task<DataTableResponse<Relatorio_Materiais_SinteticoModel>> RelatorioMateriaisAplicadosRetiradosSintetico(Relatorio_Materiais_Analiticos_DataTableRequest request)
        {

            List<Relatorio_Materiais_SinteticoModel> retorno = new List<Relatorio_Materiais_SinteticoModel>();
            using (ISession session = HibernateUtil.GetSessionT5())
            {
                //Select
                using (var com = session.Connection.CreateCommand())
                {

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    StringBuilder sql = new StringBuilder();
                    StringBuilder sqlWhere = new StringBuilder();

                    if (!String.IsNullOrEmpty(request.Data_Inicio) && !String.IsNullOrEmpty(request.Data_Fim))
                    {
                        if (request.Tipo_Data == "agendamento")
                            sqlWhere.AppendLine("AND (DATA_MAXIMA BETWEEN @DATA_INICIO AND @DATA_FIM)");
                        else
                            sqlWhere.AppendLine("AND (A.DIA BETWEEN @DATA_INICIO AND @DATA_FIM)");

                        parametros.Add("@DATA_INICIO", Convert.ToDateTime(request.Data_Inicio).ToString("dd/MM/yyyy 00:00"));
                        parametros.Add("@DATA_FIM", Convert.ToDateTime(request.Data_Fim).ToString("dd/MM/yyyy 23:59"));
                    }

                    if (!String.IsNullOrEmpty(request.Contrato))
                    {
                        sqlWhere.AppendLine("AND (D.CODCT = @CODCT)");
                        parametros.Add("@CODCT", request.Contrato);

                        if (!String.IsNullOrEmpty(request.Projeto))
                        {
                            sqlWhere.AppendLine("AND (D.PROJETO = @PROJETO)");
                            parametros.Add("@PROJETO", request.Projeto);
                        }
                    }

                    if (request.Cod_Tipo_Serv.HasValue)
                    {
                        sqlWhere.AppendLine("AND (A.COD_TIPO_SERV = @COD_TIPO_SERV)");
                        parametros.Add("@COD_TIPO_SERV", request.Cod_Tipo_Serv);
                    }

                    if (!String.IsNullOrEmpty(request.Equipe))
                    {
                        sqlWhere.AppendLine("AND (D.EQUIPE = @EQUIPE)");
                        parametros.Add("@EQUIPE", request.Equipe);
                    }

                    if (!String.IsNullOrEmpty(request.Supervisor))
                    {
                        sqlWhere.AppendLine("AND (H.NUM_FRE = @SUPERVISOR)");
                        parametros.Add("@SUPERVISOR", request.Supervisor);
                    }

                    if (request.Cod_Cliente.HasValue)
                    {
                        sqlWhere.AppendLine("AND (A.ID_CLIENTE = @ID_CLIENTE)");
                        parametros.Add("@ID_CLIENTE", request.Cod_Cliente);
                    }

                    if (!String.IsNullOrEmpty(request.CodMat))
                    {
                        sqlWhere.AppendLine("AND (A.CODMAT = @CODMAT)");
                        parametros.Add("@CODMAT", request.CodMat);
                    }
                    if (request.ClassificacaoMaterial.HasValue)
                    {
                        sqlWhere.AppendLine("AND (B.CLASSIFICACAO = @CLASSIFICACAO_MATERIAL)");
                        parametros.Add("@CLASSIFICACAO_MATERIAL", GetCodigoAniel(request.ClassificacaoMaterial.Value));
                    }

                    if (!String.IsNullOrEmpty(request.FamiliaMaterial))
                    {
                        sqlWhere.AppendLine("AND (B.FAMILIA = @FAMILIA)");
                        parametros.Add("@FAMILIA", request.FamiliaMaterial);
                    }

                    if (!String.IsNullOrEmpty(request.GrupoMaterial))
                    {
                        sqlWhere.AppendLine("AND (B.COD_SUBGRUPO_MAT = @GRUPO)");
                        parametros.Add("@GRUPO", request.GrupoMaterial);
                    }

                    if (request.Id_NaturezaProducao.HasValue)
                    {
                        sqlWhere.AppendLine("AND (F.COD_NATUREZA = @NATUREZA)");
                        parametros.Add("@NATUREZA", request.Id_NaturezaProducao);
                    }

                    var groupby = String.Empty;
                    var orderby = String.Empty;
                    switch (request.Agrupamento)
                    {
                        case "contrato_aniel":
                            groupby = ",CONTRATO_ANIEL";
                            break;
                        case "projeto":
                            orderby = "PROJETO";
                            break;
                        case "nome_equipe":
                            orderby = "NOME_EQUIPE";
                            break;
                        case "nome_cliente":
                            groupby = ",RAZAO_SOCIAL";
                            break;
                        case "desc_tipo_serv":
                            groupby = ",DESCRICAO_TIPO_SERV";
                            break;
                        case "mes_ano":
                            groupby = ",MES,ANO";
                            break;
                        case "cidade":
                            groupby = ",CIDADE";
                            break;
                    }

                    sql.AppendLine($@"SELECT CODMAT,DESCRICAO_MATERIAL,UNID,VALOR,PROJETO,NOME_EQUIPE{groupby},
                                       SUM(QUANTIDADE_APLICADA) AS QUANTIDADE_APLICADA,
                                       SUM(QUANTIDADE_RETIRADA) AS QUANTIDADE_RETIRADA,
                                       SUM(TOTAL_APLICADO) AS TOTAL_APLICADO,
                                       SUM(TOTAL_RETIRADO) AS TOTAL_RETIRADO,
                                       SUM(QUANTIDADE_OS_APLICACAO) AS QUANTIDADE_OS_APLICACAO,
                                       SUM(QUANTIDADE_OS_RETIRADA) AS QUANTIDADE_OS_RETIRADA
                                FROM (
                                     select
                                            A.CODMAT, B.DESCRICAO AS DESCRICAO_MATERIAL,B.UNID,B.VALOR,
                                            EXTRACT(MONTH FROM D.DATA_MAXIMA) AS MES,EXTRACT(YEAR FROM D.DATA_MAXIMA) AS ANO,
                                            D.DATA_MAXIMA,D.ENDERECO_TIPO AS CIDADE,F.DESCRICAO AS DESCRICAO_TIPO_SERV,E.RAZAO_SOCIAL,D.CODCT ||' - '|| G.DESCRICAO AS CONTRATO_ANIEL ,D.PROJETO,C.NOME AS NOME_EQUIPE,
                                            A.DIA AS DIA_DIGITACAO,
                                            COALESCE(A.QUANTIDADE, 0) AS QUANTIDADE_APLICADA,
                                            COALESCE(A.QUANT_REMOV, 0) AS QUANTIDADE_RETIRADA,
                                            (COALESCE(A.QUANTIDADE, 0) * B.VALOR) AS TOTAL_APLICADO,
                                            (COALESCE(A.QUANT_REMOV, 0) * B.VALOR) AS TOTAL_RETIRADO,
                                            IIF( COALESCE(A.QUANTIDADE,0) > 0, 1, 0 ) QUANTIDADE_OS_APLICACAO,
                                            IIF( COALESCE(A.QUANT_REMOV,0) > 0, 1, 0 ) QUANTIDADE_OS_RETIRADA
                                     from TB_MATERIAL_TAREFA A
                                     inner join TB_MATERIAL B 
                                     ON(A.CODMAT = B.CODMAT) 
                                     INNER JOIN TB_EQUIPE C 
                                     ON(A.EQUIPE = C.EQUIPE) 
                                     LEFT JOIN TB_FUNCIONARIO H
                                     ON(H.NUM_FRE = C.COD_SUPERV)
                                     LEFT JOIN TB_DOCUMENTO_PRODUCAO D 
                                     ON(A.CODCT = D.CODCT AND A.PROJETO = D.PROJETO AND A.NUM_OBRA = D.NUM_OBRA AND A.NUM_DOC = D.NUM_DOC) 
                                     LEFT JOIN TB_CLIENTE E 
                                     ON(E.COD_CLIENTE = A.ID_CLIENTE) 
                                     LEFT JOIN TB_TIPO_SERVICO_EQUIPE F 
                                     ON(COALESCE(D.COD_TIPO_SERV,A.COD_TIPO_SERV) = F.COD_TIPO_SERV) 
                                     LEFT JOIN TB_CONTRATO G
                                     ON(G.CODCT = A.CODCT)

                                     WHERE B.CLASSIFICACAO IN ('I','B','R') AND (A.QUANTIDADE > 0 OR A.QUANT_REMOV > 0)
                                       {sqlWhere.ToString()}
                                )
                                     GROUP BY CODMAT,DESCRICAO_MATERIAL,UNID,VALOR,PROJETO,NOME_EQUIPE{groupby}");

                    //Remove a vírgula do groupby, se for nulo ele pegará a variável orderby
                    //Esse processo foi feito para poder exportar os campos dessa variável
                    groupby = groupby != "" ? groupby.Substring(1) : orderby;

                    sql.AppendLine($"ORDER BY {groupby}");

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            var obj = new Relatorio_Materiais_SinteticoModel
                            {
                                cod_material = Convert.ToString(reader["CODMAT"]),
                                projeto = Convert.ToString(reader["PROJETO"]),
                                nome_equipe = Convert.ToString(reader["NOME_EQUIPE"]),
                                nome_material = Convert.ToString(reader["DESCRICAO_MATERIAL"]),
                                unidade = Convert.ToString(reader["UNID"]),
                                qtde_aplicada = Convert.ToString(reader["QUANTIDADE_APLICADA"]),
                                qtde_removida = Convert.ToString(reader["QUANTIDADE_RETIRADA"]),
                                valor_unitario = String.Format("{0:N2}", Convert.ToDouble(reader["VALOR"] != DBNull.Value ? reader["VALOR"] : 0)),
                                total_aplicado = String.Format("{0:N2}", Convert.ToDouble(reader["TOTAL_APLICADO"] != DBNull.Value ? reader["TOTAL_APLICADO"] : 0)),
                                total_removido = String.Format("{0:N2}", Convert.ToDouble(reader["TOTAL_RETIRADO"] != DBNull.Value ? reader["TOTAL_RETIRADO"] : 0)),
                                media_aplicada_servico = Convert.ToDouble(reader["QUANTIDADE_OS_APLICACAO"]) > 0 ? (Convert.ToDouble(reader["QUANTIDADE_APLICADA"]) / Convert.ToDouble(reader["QUANTIDADE_OS_APLICACAO"])).ToString() : "0",
                                media_removida_servico = Convert.ToDouble(reader["QUANTIDADE_OS_RETIRADA"]) > 0 ? (Convert.ToDouble(reader["QUANTIDADE_RETIRADA"]) / Convert.ToDouble(reader["QUANTIDADE_OS_RETIRADA"])).ToString() : "0",
                            };

                            switch (request.Agrupamento)
                            {
                                case "contrato_aniel":
                                    obj.contrato_aniel = Convert.ToString(reader["CONTRATO_ANIEL"]);
                                    break;
                                case "projeto":
                                    obj.projeto = Convert.ToString(reader["PROJETO"]);
                                    break;
                                case "nome_equipe":
                                    obj.nome_equipe = Convert.ToString(reader["NOME_EQUIPE"]);
                                    break;
                                case "nome_cliente":
                                    obj.nome_cliente = Convert.ToString(reader["RAZAO_SOCIAL"]);
                                    break;
                                case "desc_tipo_serv":
                                    obj.desc_tipo_serv = Convert.ToString(reader["DESCRICAO_TIPO_SERV"]);
                                    break;
                                case "mes_ano":

                                    var mes = reader["MES"] != DBNull.Value ? Convert.ToInt16(reader["MES"]) : (Int16?)null;
                                    var ano = reader["ANO"] != DBNull.Value ? Convert.ToInt16(reader["ANO"]) : (Int16?)null;

                                    if (mes.HasValue && ano.HasValue)
                                    {
                                        obj.mes_ano = $"{HtmlEnumExtensions.GetEnumDescription<Mes_Simplicado>(mes.ToString())}/{ano}";
                                    }

                                    break;
                                case "cidade":
                                    obj.cidade = Convert.ToString(reader["CIDADE"]);
                                    break;
                            }

                            retorno.Add(obj);

                        }
                    }

                }
            }

            return new DataTableResponse<Relatorio_Materiais_SinteticoModel>
            {
                data = retorno,
                recordsFiltered = retorno.Count
            };
        }

        [HttpPost()]
        [Route("api/Material_TarefaApi/GetCombo")]
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_MATERIAL A ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        //condicoes.Add($"TERMREAL IS NULL");
                    }

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"UPPER(A.CODMAT) = '{request.id.ToUpperInvariant()}'");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.CODMAT) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 12)}%' OR " +
                                      $" UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 55)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes)} ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.CODMAT) AS CO ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.CODMAT, A.DESCRICAO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["CODMAT"]),
                                    Text = $"{Convert.ToString(reader["CODMAT"])} - {Convert.ToString(reader["DESCRICAO"])}"
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

        [HttpPost()]
        [Route("api/Material_TarefaApi/GetComboFamiliaMaterial")]
        public async Task<ObjCombo<String>> GetComboFamiliaMaterial(RequestObjCombo<String> request)
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_FAMILIA_MATERIAL A ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        //condicoes.Add($"TERMREAL IS NULL");
                    }

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"UPPER(A.FAMILIA) = '{request.id.ToUpperInvariant()}'");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.FAMILIA) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 5)}%' OR " +
                                      $" UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 30)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes)} ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.FAMILIA) AS CO ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.FAMILIA, A.DESCRICAO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["FAMILIA"]),
                                    Text = $"{Convert.ToString(reader["FAMILIA"])} - {Convert.ToString(reader["DESCRICAO"])}"
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


        private async Task<Boolean> VerificaOSProtocolada(ISession session, String CodCt, String Projeto, String Num_Obra, Int32 Num_Doc)
        {

            try
            {
                //Filtros
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sql = new StringBuilder(@"SELECT COUNT(A.CODMAT) AS CO 
                                                                FROM TB_MATERIAL_TAREFA A
                                                            WHERE A.CODCT = @CODCT AND A.PROJETO = @PROJETO AND A.NUM_OBRA = @NUM_OBRA AND A.NUM_DOC = @NUM_DOC
                                                            AND COALESCE(A.NUM_PROTOCOLO, '''') <> ''''");

                parametros.Add("@CODCT", CodCt);
                parametros.Add("@PROJETO", Projeto);
                parametros.Add("@NUM_OBRA", Num_Obra);
                parametros.Add("@NUM_DOC", Num_Doc);

                using (var com = session.Connection.CreateCommand())
                {
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        if (await reader.ReadAsync())
                            return Convert.ToInt32(reader["CO"]) > 0;
                        else
                            return false;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task EliminarDoReservatorio(ISession session, String CodCt, String Projeto, String Num_Obra, Int32 Num_Doc, String Tarefa, String Equipe, String CodMat, String CodCpl)
        {

            try
            {
                //Filtros
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sql = new StringBuilder(@"DELETE FROM TB_RESERV_IMPORTACAO A WHERE CODCT=@CODCT AND PROJETO=@PROJETO AND TAREFA=@TAREFA AND NUM_OBRA=@NUM_OBRA AND NUM_DOC=@NUM_DOC AND EQUIPE=@EQUIPE AND CODMAT=@CODMAT AND CODCPL=@CODCPL");

                parametros.Add("@CODCT", CodCt);
                parametros.Add("@PROJETO", Projeto);
                parametros.Add("@NUM_OBRA", Num_Obra);
                parametros.Add("@NUM_DOC", Num_Doc);
                parametros.Add("@TAREFA", Tarefa);
                parametros.Add("@EQUIPE", Equipe);
                parametros.Add("@CODMAT", CodMat);
                parametros.Add("@CODCPL", CodCpl);

                using (var com = session.Connection.CreateCommand())
                {
                    await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task InsereNoReservatorio(ISession session, String CodCt, String Projeto, String Num_Obra, Int32 Num_Doc, String Tarefa, String Equipe, String CodMat, String CodCpl,
            DateTime? Data_Aplic, Double? Quantidade, Double? Quant_Remov, String Usuario, String Unidade_Custo, String Cod_Escritorio, String Cod_Area, Int16? Cod_Tipo_Serv, String Cod_Sub_Tipo_Serv,
            Int32? Cod_Localidade)
        {
            /*
            @USUARIO, @DIA, @HORA, @MOTIVO, @IMPORTAR, @UNIDADE_CUSTO,
                                                                @COD_ESCRITORIO, @COD_AREA, @COD_TIPO_SERV, @COD_SUB_TIPO_SERV, @COD_LOCALIDADE
            */

            try
            {
                //Filtros
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sql = new StringBuilder(@"EXECUTE PROCEDURE SP_IMPORTA_RESERV(@OPERACAO, @CODCT, @PROJETO, @NUM_OBRA, @NUM_DOC, @TAREFA, @CODMAT, @CODCPL,
                                                                @EQUIPE, @DATA_APLIC, @QUANTIDADE, @QUANT_REMOV, @USUARIO, CURRENT_DATE, CURRENT_TIME, @MOTIVO, @IMPORTAR, @UNIDADE_CUSTO,
                                                                @COD_ESCRITORIO, @COD_AREA, @COD_TIPO_SERV, @COD_SUB_TIPO_SERV, @COD_LOCALIDADE)");

                parametros.Add("@OPERACAO", 2);
                parametros.Add("@CODCT", CodCt);
                parametros.Add("@PROJETO", Projeto);
                parametros.Add("@NUM_OBRA", Num_Obra);
                parametros.Add("@NUM_DOC", Num_Doc);
                parametros.Add("@TAREFA", Tarefa);
                parametros.Add("@CODMAT", CodMat);
                parametros.Add("@CODCPL", CodCpl);
                parametros.Add("@EQUIPE", Equipe);

                parametros.Add("@DATA_APLIC", Data_Aplic);
                parametros.Add("@QUANTIDADE", Quantidade);
                parametros.Add("@QUANT_REMOV", Quant_Remov);
                parametros.Add("@USUARIO", Usuario);

                parametros.Add("@MOTIVO", "EQUIPE SEM SALDO NESTE MATERIAL");
                parametros.Add("@IMPORTAR", "N");
                parametros.Add("@UNIDADE_CUSTO", Unidade_Custo);
                parametros.Add("@COD_ESCRITORIO", Cod_Escritorio);
                parametros.Add("@COD_AREA", Cod_Area);
                parametros.Add("@COD_TIPO_SERV", Cod_Tipo_Serv);
                parametros.Add("@COD_SUB_TIPO_SERV", Cod_Sub_Tipo_Serv);
                parametros.Add("@COD_LOCALIDADE", Cod_Localidade);

                using (var com = session.Connection.CreateCommand())
                {
                    await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        [HttpPost()]
        [Route("api/Material_TarefaApi/GetComboGrupoMaterial")]
        public async Task<ObjCombo<String>> GetComboGrupoMaterial(RequestObjCombo<String> request)
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_SUBGRUPO_MATERIAL A ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        //condicoes.Add($"TERMREAL IS NULL");
                    }

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($"UPPER(A.COD_SUBGRUPO_MAT) = '{request.id.ToUpperInvariant()}'");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( UPPER(A.COD_SUBGRUPO_MAT) LIKE '%{request.q.ToUpperInvariant()}%' OR " +
                                      $" UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 30)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes)} ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.COD_SUBGRUPO_MAT) AS CO ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.COD_SUBGRUPO_MAT, A.DESCRICAO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["COD_SUBGRUPO_MAT"]),
                                    Text = $"{Convert.ToString(reader["COD_SUBGRUPO_MAT"])} - {Convert.ToString(reader["DESCRICAO"])}"
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

        [HttpPost()]
        [Route("api/Material_TarefaApi/GetComboCategoriaCliente")]
        public async Task<ObjCombo<String>> GetComboCategoriaCliente(RequestObjCombo<String> request)
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

                    StringBuilder sqlFrom = new StringBuilder("FROM TB_CATEGORIA_CLIENTE_CONNECT A ");

                    List<String> condicoes = new List<String>();

                    if (request.soativos)
                    {
                        //condicoes.Add($"TERMREAL IS NULL");
                    }

                    if (!String.IsNullOrEmpty(request.id))
                    {
                        condicoes.Add($" A.ID = '{request.id.ToUpperInvariant()}'");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"( A.ID LIKE '%{request.q.ToUpperInvariant()}%' OR " +
                                      $" UPPER(A.DESCRICAO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 30)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($"WHERE ( {String.Join(" AND ", condicoes)} ) ");
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) AS CO ");
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
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.DESCRICAO ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine("ORDER BY UPPER(A.DESCRICAO)");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var obj = new ObjComboResult<String>
                                {
                                    Id = Convert.ToString(reader["ID"]),
                                    Text = $"{Convert.ToString(reader["ID"])} - {Convert.ToString(reader["DESCRICAO"])}"
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


        [HttpPost()]
        [Route("api/Material_TarefaApi/Inserir")]
        public async Task<PostApiRetorno<Material_AplicadoListaModel>> Inserir(ParamWebApi<Material_AplicadoListaModel> value)
        {
            PostApiRetorno<Material_AplicadoListaModel> retorno = new PostApiRetorno<Material_AplicadoListaModel>();

            try
            {
                Material_TarefaAniel modelo = null;

                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    using (var repository = new Conexao<Material_TarefaAniel>(session))
                    using (var repositoryDoc = new Conexao<Documento_ProducaoAniel>(session))
                    using (var repositoryContrato = new Conexao<ContratoAniel>(session))
                    using (var repositoryMaterial = new Conexao<MaterialAniel>(session))
                    using (var apiEquipe = new EquipeApiController())
                    using (var apiFechamento = new FechamentoPeriodoApiController())
                    using (var apiDocumento = new Documento_ProducaoApiController())
                    using (var apiSequenciaRMDM = new Sequencia_RMDMApiController())
                    using (var apiMaster = new MasterApiController())
                    using (var apiCodigoComplementar = new Codigo_ComplementarApiController())
                    using (var apiSaldo = new Saldo_EquipeApiController())
                    {
                        var contratoAniel = await repositoryContrato.selectFirst(p => p.CodCt == value.Objeto.CodCt);
                        var documentos = await repositoryDoc.select(p => p.CodCt == value.Objeto.CodCt && p.Projeto == value.Objeto.Projeto && p.Numero_Obra_Original == value.Objeto.Num_Obra && p.Num_Doc == value.Objeto.Num_Doc);                        
                        if (documentos.Count == 0)
                        {
                            documentos = await repositoryDoc.select(p => p.CodCt == value.Objeto.CodCt && p.Projeto == value.Objeto.Projeto && p.Num_Obra == value.Objeto.Num_Obra && p.Num_Doc == value.Objeto.Num_Doc);
                        }
                        var documento = documentos.First(y => y.Seq == documentos.Max(p => p.Seq));

                        if (String.IsNullOrEmpty(value.Objeto.Codigo))
                            throw new Exception(String.Format(RsBase.REQUIRED, RsCadastros.CODIGO));

                        var _codcpl = String.IsNullOrEmpty(value.Objeto.Codigo_Complementar) ? "-" : value.Objeto.Codigo_Complementar;

                        //DAIANE TICKET 28848
                        double _qtdeAplic;
                        if (Convert.ToDouble(value.Objeto.Quantidade) == 0)
                        {
                            if (value.Objeto.Retirada)
                            {
                                throw new Exception(String.Format(RsBase.REQUIRED, RsCadastros.QUANTIDADE_REMOVIDA));
                            }
                            else
                            {
                                throw new Exception(String.Format(RsBase.REQUIRED, RsCadastros.QUANTIDADE_APLICADA));
                            }
                        }
                        else
                        {
                             _qtdeAplic = Convert.ToDouble(value.Objeto.Quantidade);
                        }

                        modelo = (await repository.selectFirst(p => p.CodCt == documento.CodCt && p.Projeto == documento.Projeto && p.Num_Obra == documento.Num_Obra && p.Num_Doc == documento.Num_Doc &&
                                                                    p.CodMat == value.Objeto.Codigo && p.CodCpl == _codcpl));

                        if (modelo == null)
                        {
                            var material = await repositoryMaterial.selectFirst(p => p.CodMat == value.Objeto.Codigo);

                            var data_Aplic = documento.Data_Prod ?? documento.Data_Maxima ?? DateTime.Now;

                            if (material.SubCod == "S" && _codcpl == "-")
                                throw new Exception(String.Format(RsBase.REQUIRED, RsCadastros.CODIGO_COMPLEMENTAR));

                            if (String.IsNullOrEmpty(documento.Equipe))
                                throw new Exception(String.Format(RsBase.REQUIRED, RsCadastros.EQUIPE));

                            if (await apiEquipe.Afastado(session, documento.Equipe, null))
                                throw new Exception(String.Format(RsCadastros.EQUIPE_ENCERRADA, documento.Equipe));

                            await apiEquipe.VerifFechtoEquipe(session, contratoAniel.TipoSaldoAplic, documento.CodCt, documento.Projeto, documento.Equipe, data_Aplic, true);

                            //if (documento.Roterizar == "S")
                            //    throw new Exception(RsCadastros.TANOMOBILE);

                            var periodo = await apiFechamento.PeriodoFechado(session, documento.CodCt, documento.Projeto, DateTime.Now);
                            if (periodo.Item1 != null)
                                throw new Exception(String.Format(RsCadastros.CONTRATO_PROJETO_PERIODO_FECHADO, documento.CodCt, documento.Projeto, periodo.Item1.Value.ToString("dd/MM/yyyy"), periodo.Item2.Value.ToString("dd/MM/yyyy")));

                            if (await VerificaOSProtocolada(session, documento.CodCt, documento.Projeto, documento.Num_Obra, documento.Num_Doc))
                                throw new Exception(RsCadastros.EX_OS_PROTOCOLADA);

                            if (contratoAniel.Bloquear_Aplic_Remov_OS_Aberta == "S")
                            {
                                if ((new List<Int32> { 0, 3, 4, 6 }).Exists(p => p == Convert.ToInt32(documento.Status.Value)))
                                    throw new Exception(RsCadastros.EX_STATUS_OS_NAO_PERMITE_APLICACAO);
                            }

                            if (contratoAniel.Bloquear_Terminais_OS_Improd == "S")
                            {
                                if (documento.Status.Value == StatusExecucaoAniel.Fechada_Improdutiva && material.SubCod == "S")
                                    throw new Exception(RsCadastros.EX_STATUS_OS_NAO_PERMITE_APLICACAO_IMPROD);
                            }

                            Boolean enviaReservatorio = false;

                            //Material Retirado
                            if (value.Objeto.Retirada)
                            {
                                if (documento.Devolvida == "S")
                                    throw new Exception(RsCadastros.EX_OS_QUITADA);
                                if (material.SubCod_Exclusivo == "S")
                                {
                                    await CriticaComplementarPendente(session, material.CodMat, _codcpl, contratoAniel);
                                }
                                if (contratoAniel.Fluxo_Mat == 2)
                                {
                                    bool devolvido = await VerificarDevolucaoComplementar(contratoAniel.CodCt, value.Objeto.Projeto, material.CodMat, _codcpl, session);
                                    if (devolvido == false)
                                    {
                                        throw new Exception(RsCadastros.CRITICA_VOLTA_ALMOXARIFADO);
                                    }

                                }

                                await apiCodigoComplementar.IncluirCodCpl(session, documento.CodCt, documento.Projeto, material.CodMat, _codcpl, "");
                            }
                            else //Material Aplicado
                            {
                                await EliminarDoReservatorio(session, documento.CodCt, documento.Projeto, documento.Num_Obra, documento.Num_Doc, documento.Tarefa, documento.Equipe, material.CodMat, _codcpl);

                                if (material.SubCod == "S")
                                    await apiSequenciaRMDM.CriticaCplProgramadoDM(session, documento.CodCt, documento.Equipe, material.CodMat, _codcpl);

                                Double progDM = await apiSaldo.CalcularProgramacaoRmDm(session, contratoAniel.TipoSaldoAplic, "D", documento.CodCt, documento.Projeto, documento.Equipe, material.CodMat, _codcpl);

                                SaldoEquipeModel saldoEquipe = await apiSaldo.LocalizaSaldoEquipe(session, contratoAniel.TipoSaldoAplic, documento.CodCt, documento.Projeto, documento.Equipe, material.CodMat, _codcpl);
                                Double _saldoEquipe = saldoEquipe.SALDO.Value - progDM;

                                var criticaSaldoData = await apiMaster.GetConfig_Doc(session, "ALMOXARIFADO", "CRITICA_SALDO_EQUIPE_DATA", "N");
                                if (_saldoEquipe >= _qtdeAplic && criticaSaldoData == "S")
                                {
                                    saldoEquipe = await apiSaldo.LocalizaSaldoEquipePer(session, contratoAniel.TipoSaldoAplic, documento.CodCt, documento.Projeto, documento.Equipe, new DateTime(1900, 1, 1), data_Aplic, material.CodMat, _codcpl);
                                    _saldoEquipe = saldoEquipe.SALDO.Value - progDM;
                                }
                                else                                
                                    criticaSaldoData = "N";

                                var GERAR_TI_AUTO_APLIC = await apiMaster.GetConfig_Doc(session, "ALMOXARIFADO", "GERAR_TI_AUTO_APLIC", "N");
                                if (_saldoEquipe < _qtdeAplic)
                                {
                                    var qtdfaltando = _qtdeAplic - _saldoEquipe;

                                    if (material.Gerar_Saldo_Auto == "S")
                                    {
                                        modelo = new Material_TarefaAniel
                                        {
                                            CodCt = documento.CodCt,
                                            Projeto = documento.Projeto,
                                            Num_Obra = documento.Num_Obra,
                                            Num_Doc = documento.Num_Doc,
                                            Data_Aplic = data_Aplic,
                                            Cod_Tipo_Serv = documento.Cod_Tipo_Serv,
                                            Cod_Sub_Tipo_Serv = documento.Cod_Sub_Tipo_Serv ?? "-",
                                            Equipe = documento.Equipe,
                                            Tarefa = "-",
                                            CodMat = value.Objeto.Codigo,
                                            CodCpl = _codcpl,
                                            Quant_Catal = 0,
                                            Quantidade = value.Objeto.Retirada ? 0 : _qtdeAplic,
                                            Quant_Remov = value.Objeto.Retirada ? _qtdeAplic : 0,
                                            Dia = DateTime.Now,
                                            Hora = DateTime.Now,
                                            Usuario = value.Usuario,
                                            Id_Cliente = documento.Cliente?.Cod_Cliente
                                        };

                                        await GerarMovimentacaoLivre(documento, modelo, value.Usuario, qtdfaltando, documento.Data_Maxima.Value, documento.Equipe, session);
                                        //gerar um movimento livre aumentando o saldo da equipe
                                        _saldoEquipe += qtdfaltando;
                                    }
                                    else if (material.ENVIA_RESERV == "S")
                                    {
                                        enviaReservatorio = true;
                                        _saldoEquipe += qtdfaltando;
                                    }
                                    else if (GERAR_TI_AUTO_APLIC.Equals("S") && !string.IsNullOrEmpty(value.Objeto.Codigo_Complementar))
                                    {

                                        var saldoComplementar = await apiSaldo.LocalizaSaldoEquipeSaldo(session, contratoAniel.TipoSaldoAplic, documento.CodCt, documento.Projeto, "-", material.CodMat, _codcpl, qtdfaltando);
                                        if (saldoComplementar.EQUIPE != value.Objeto.Equipe && saldoComplementar.SALDO >= qtdfaltando)
                                        {

                                            if (value.Objeto.Permitir_TI_Automatica)
                                            {
                                                await GerarMovimentoTransferenciaSaldo(documento, material.CodMat, _codcpl, value.Usuario, documento.Equipe, saldoComplementar.EQUIPE, qtdfaltando, session);
                                                _saldoEquipe += qtdfaltando;
                                            }
                                            else
                                            {
                                                String mensagem = criticaSaldoData == "S" ? String.Format(RsCadastros.EX_SALDO_APLICAR_INSUFICIENTE_PER,
                                                                                                   documento.Equipe,
                                                                                                   data_Aplic.ToString("dd/MM/yyyy"),
                                                                                                   material.CodMat,
                                                                                                   material.Descricao,
                                                                                                   _codcpl,
                                                                                                   saldoEquipe.SALDO.Value.ToString("N2"),
                                                                                                   saldoEquipe.TOTAPLIC.Value.ToString("N2"),
                                                                                                   saldoEquipe.TOTREMOV.Value.ToString("N2"),
                                                                                                   progDM.ToString("N2"),
                                                                                                   _qtdeAplic.ToString("N2")).Replace("\n", "<br/>") :
                                                                                     String.Format(RsCadastros.EX_SALDO_APLICAR_INSUFICIENTE,
                                                                                                   documento.Equipe,
                                                                                                   material.CodMat,
                                                                                                   material.Descricao,
                                                                                                   _codcpl,
                                                                                                   saldoEquipe.SALDO.Value.ToString("N2"),
                                                                                                   saldoEquipe.TOTAPLIC.Value.ToString("N2"),
                                                                                                   saldoEquipe.TOTREMOV.Value.ToString("N2"),
                                                                                                   progDM.ToString("N2"),
                                                                                                   _qtdeAplic.ToString("N2")).Replace("\n", "<br/>");





                                                throw new Exception(mensagem + $@"<br/><br/> O material {material.Descricao} consta no técnico {saldoComplementar.EQUIPE}. Você gostaria de dar continuidade na transferência deste material para o técnico {documento.Equipe} ? <GERAR_TI_AUTO_APLIC> ");

                                            }

                                        }


                                    }

                                    if (_saldoEquipe < _qtdeAplic)
                                    {
                                        String mensagem = criticaSaldoData == "S" ? String.Format(RsCadastros.EX_SALDO_APLICAR_INSUFICIENTE_PER,
                                                                                                   documento.Equipe,
                                                                                                   data_Aplic.ToString("dd/MM/yyyy"),
                                                                                                   material.CodMat,
                                                                                                   material.Descricao,
                                                                                                   _codcpl,
                                                                                                   saldoEquipe.SALDO.Value.ToString("N2"),
                                                                                                   saldoEquipe.TOTAPLIC.Value.ToString("N2"),
                                                                                                   saldoEquipe.TOTREMOV.Value.ToString("N2"),
                                                                                                   progDM.ToString("N2"),
                                                                                                   _qtdeAplic.ToString("N2")).Replace("\n", "<br/>") :
                                                                                     String.Format(RsCadastros.EX_SALDO_APLICAR_INSUFICIENTE,
                                                                                                   documento.Equipe,
                                                                                                   material.CodMat,
                                                                                                   material.Descricao,
                                                                                                   _codcpl,
                                                                                                   saldoEquipe.SALDO.Value.ToString("N2"),
                                                                                                   saldoEquipe.TOTAPLIC.Value.ToString("N2"),
                                                                                                   saldoEquipe.TOTREMOV.Value.ToString("N2"),
                                                                                                   progDM.ToString("N2"),
                                                                                                   _qtdeAplic.ToString("N2")).Replace("\n", "<br/>");

                                        throw new Exception(mensagem);
                                    }
                                }
                            }

                            if (enviaReservatorio)
                            {
                                await InsereNoReservatorio(session, documento.CodCt, documento.Projeto, documento.Num_Obra, documento.Num_Doc, documento.Tarefa, documento.Equipe,
                                        material.CodMat, _codcpl, data_Aplic, _qtdeAplic, 0, value.Usuario, documento.Unidade_Custo, documento.Cod_Escritorio, documento.Cod_Area, documento.Cod_Tipo_Serv,
                                        documento.Cod_Sub_Tipo_Serv, documento.Cod_Localidade);

                                retorno.Ok = true;
                                retorno.Mensagem = RsCadastros.MATERIAL_ENVIADO_RESEVATORIO;
                            }
                            else
                            {
                                modelo = new Material_TarefaAniel
                                {
                                    CodCt = documento.CodCt,
                                    Projeto = documento.Projeto,
                                    Num_Obra = documento.Num_Obra,
                                    Num_Doc = documento.Num_Doc,
                                    Data_Aplic = data_Aplic,
                                    Cod_Tipo_Serv = documento.Cod_Tipo_Serv,
                                    Cod_Sub_Tipo_Serv = documento.Cod_Sub_Tipo_Serv ?? "-",
                                    Equipe = documento.Equipe,
                                    Tarefa = "-",
                                    CodMat = value.Objeto.Codigo,
                                    CodCpl = _codcpl,
                                    Quant_Catal = 0,
                                    Quantidade = value.Objeto.Retirada ? 0 : _qtdeAplic,
                                    Quant_Remov = value.Objeto.Retirada ? _qtdeAplic : 0,
                                    Dia = DateTime.Now,
                                    Hora = DateTime.Now,
                                    Usuario = value.Usuario,
                                    Id_Cliente = documento.Cliente?.Cod_Cliente
                                };

                                var valUnit = material.Valor ?? 0;

                                retorno.IdObjeto = new Material_AplicadoListaModel
                                {
                                    CodCt = modelo.CodCt,
                                    Projeto = modelo.Projeto,
                                    Num_Obra = modelo.Num_Obra,
                                    Num_Doc = modelo.Num_Doc,
                                    Codigo = modelo.CodMat,
                                    Tarefa = modelo.Tarefa,
                                    Descricao = material.Descricao,
                                    Codigo_Complementar = modelo.CodCpl,
                                    Unidade = material.Unid,
                                    Equipe = $"{modelo.Equipe} - {await apiEquipe.GetNome(session, modelo.Equipe)}",
                                    Valor_Unitario = valUnit.ToString("N2"),
                                    Quantidade = _qtdeAplic.ToString("N2"),
                                    Valor_Total = (valUnit * _qtdeAplic).ToString("N2")
                                };


                                await repository.SaveOrUpdate(modelo);

                                await apiDocumento.InsertLog_Lanc_Servico(session, documento.CodCt, documento.Projeto, documento.Num_Obra, documento.Num_Doc, "Incluiu Material", null, value.Usuario, documento.Data_Maxima, documento.Cod_Tipo_Serv, modelo.CodMat + " - " + material.Descricao + " - " + "Compl.: " + modelo.CodCpl, null);

                                retorno.Ok = true;

                            }
                        }
                        else
                        {
                            throw new Exception(RsCadastros.MATERIAL_JA_LANCADO);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = null;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;

                if (retorno.Mensagem.Contains("<GERAR_TI_AUTO_APLIC>"))
                {
                    retorno.Mensagem = retorno.Mensagem.Replace("<GERAR_TI_AUTO_APLIC>", "");
                    retorno.InnerException = @"GERAR_TI_AUTO_APLIC";
                }
            }

            return retorno;
        }

        public async Task CriticaComplementarPendente(ISession session, String CodMat, String CodCpl, ContratoAniel contrato)
        {
            using (var apiComplementar = new Codigo_ComplementarApiController())
            {
                var local = await apiComplementar.GetLocalizacao(session, CodMat, CodCpl, null);

                if (local.Status == Codigo_ComplementarLocalizacaoEnum.Fisico || local.Status == Codigo_ComplementarLocalizacaoEnum.Volante)
                {
                    if (local.Status == Codigo_ComplementarLocalizacaoEnum.Fisico && local.Classificacao == "I")
                        throw new Exception("O Complementar está com o saldo no Estoque");
                    else if (local.Status == Codigo_ComplementarLocalizacaoEnum.Volante)
                         throw new Exception("O Complementar está no saldo Volante");
                    //else if (local.Status == Codigo_ComplementarLocalizacaoEnum.Fisico && local.Classificacao == "12")
                }
                //else if (local.Status == Codigo_ComplementarLocalizacaoEnum.Aplicado)
                //{
                //    if (contrato.Fluxo_Mat == 2 && local.Classificacao == "R")
                //    {
                //        throw new Exception(RsCadastros.CRITICA_VOLTA_ALMOXARIFADO);
                //    }

                //}

            }
        }

        [Route("api/Material_TarefaApi/Excluir")]
        [HttpPost()]
        public async Task<DeleteApiRetorno<Material_TarefaChave>> Excluir(ParamWebApi<Material_TarefaChave> value)
        {
            DeleteApiRetorno<Material_TarefaChave> retorno = new DeleteApiRetorno<Material_TarefaChave>();

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    using (var repository = new Conexao<Material_TarefaAniel>(session))
                    using (var repositoryDoc = new Conexao<Documento_ProducaoAniel>(session))
                    using (var repositoryContrato = new Conexao<ContratoAniel>(session))
                    using (var repositoryMaterial = new Conexao<MaterialAniel>(session))
                    using (var apiMaster = new MasterApiController())
                    using (var apiDocumento = new Documento_ProducaoApiController())
                    using (var apiEquipe = new EquipeApiController())
                    using (var apiFechamento = new FechamentoPeriodoApiController())
                    using (var apiSaldo = new Saldo_EquipeApiController())
                    {

                        var contratoAniel = await repositoryContrato.selectFirst(p => p.CodCt == value.Objeto.CodCt);
                        var documento = await repositoryDoc.selectFirst(p => p.CodCt == value.Objeto.CodCt && p.Projeto == value.Objeto.Projeto && p.Num_Obra == value.Objeto.Num_Obra && p.Num_Doc == value.Objeto.Num_Doc);

                        Material_TarefaAniel modelo = null;
                        if (value.Objeto?.CodMat != String.Empty)
                        {
                            //var codcpl = String.IsNullOrEmpty(value.Objeto.CodCpl) ? "-" : value.Objeto.CodCpl;
                            var codcpl = value.Objeto.CodCpl;

                            if (value.Objeto.Tarefa == null)
                                value.Objeto.Tarefa = "";

                            modelo = (await repository.selectFirst(p => p.CodCt == value.Objeto.CodCt && p.Projeto == value.Objeto.Projeto && p.Num_Obra == value.Objeto.Num_Obra && p.Num_Doc == value.Objeto.Num_Doc &&
                                                                        p.Tarefa == value.Objeto.Tarefa && p.CodMat == value.Objeto.CodMat && p.CodCpl == codcpl));

                            var material = await repositoryMaterial.selectFirst(p => p.CodMat == value.Objeto.CodMat);

                            //Criticas
                            if (documento.Roterizar == "S")
                                throw new Exception(RsCadastros.TANOMOBILE);

                            if (await apiEquipe.Afastado(session, modelo.Equipe, null))
                                throw new Exception(String.Format(RsCadastros.EQUIPE_ENCERRADA, documento.Equipe));

                            await apiEquipe.VerifFechtoEquipe(session, contratoAniel.TipoSaldoAplic, documento.CodCt, documento.Projeto, modelo.Equipe, modelo.Data_Aplic, true);

                            var periodo = await apiFechamento.PeriodoFechado(session, documento.CodCt, documento.Projeto, DateTime.Now);
                            if (periodo.Item1 != null)
                                throw new Exception(String.Format(RsCadastros.CONTRATO_PROJETO_PERIODO_FECHADO, documento.CodCt, documento.Projeto, periodo.Item1.Value.ToString("dd/MM/yyyy"), periodo.Item2.Value.ToString("dd/MM/yyyy")));

                            if (await VerificaOSProtocolada(session, documento.CodCt, documento.Projeto, documento.Num_Obra, documento.Num_Doc))
                                throw new Exception(RsCadastros.EX_OS_PROTOCOLADA);

                            if (material.CodMat[0].ToString() == "R" && material.Classificacao == "R")
                                value.Objeto.Retirada = true;

                            if (value.Objeto.Retirada)
                            {
                                var criticaSaldoData = await apiMaster.GetConfig_Doc(session, "ALMOXARIFADO", "CRITICA_SALDO_EQUIPE_DATA", "N");

                                SaldoEquipeModel saldoEquipe = null;
                                if (criticaSaldoData == "S")
                                {
                                    saldoEquipe = await apiSaldo.LocalizaSaldoEquipePer(session, contratoAniel.TipoSaldoAplic, documento.CodCt, documento.Projeto, documento.Equipe, new DateTime(1900, 1, 1), modelo.Data_Aplic.Value, modelo.CodMat, modelo.CodCpl);
                                }
                                else
                                {
                                    saldoEquipe = await apiSaldo.LocalizaSaldoEquipe(session, contratoAniel.TipoSaldoAplic, documento.CodCt, documento.Projeto, documento.Equipe, modelo.CodMat, modelo.CodCpl);
                                }

                                if (modelo.Quant_Remov > saldoEquipe.SALDO)
                                {
                                    throw new Exception(RsCadastros.EX_SALDO_RETIRADA_EQUIPE_NEGATIVO);
                                }
                            }
                            else
                            {
                                await DeletarMovimentoLivreEstoque(modelo.CodCt, modelo.Projeto, modelo.CodMat, modelo.CodCpl, modelo.Num_Obra, modelo.Data_Aplic, session);
                            }

                            if (modelo.ID_DEVOLUCAO_OS_MAT.HasValue)
                            {
                                throw new Exception(RsCadastros.EX_MATERIAL_DEVOLUCAO_OS);
                            }


                            await repository.Remove(modelo);



                            await apiDocumento.InsertLog_Lanc_Servico(session, documento.CodCt, documento.Projeto, documento.Num_Obra, documento.Num_Doc, "Excluiu Material", null, value.Usuario, documento.Data_Maxima, documento.Cod_Tipo_Serv, modelo.CodMat + " - " + material.Descricao + " - " + "Compl.: " + modelo.CodCpl, null);

                            await InsereLogExcMatAplic(session, documento.CodCt, documento.Projeto, documento.Num_Obra, documento.Num_Doc, modelo.Cod_Tipo_Serv, documento.Data_Maxima, modelo.Data_Aplic, modelo.CodMat, modelo.CodCpl, modelo.Quantidade, modelo.Quant_Remov, 0, User.Identity.GetUserId());

                            retorno.IdObjeto = value.Objeto;
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
                retorno.IdObjeto = null;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        public async Task<Boolean> InsereLogExcMatAplic(ISession session, String CodCt, String Projeto, String Num_Os, Int32 Num_Doc,
            Int16? CodTipoServ, DateTime? Data_Maxima, DateTime? Data_Aplic, String CodMat, String CodCpl, Double? Quant, Double? QuantRemov, Int32 Recurso, String Usuario)
        {
            Dictionary<String, Object> parametros = new Dictionary<string, object>();

            //Select
            using (var com = session.Connection.CreateCommand())
            {

                StringBuilder sql = new StringBuilder(@"INSERT INTO TB_LOG_EXCLUSAO_APLICACAO_MAT (CODCT,PROJETO,NUM_OBRA,
                                                                    NUM_DOC, DATA_MAXIMA, DATA_APLIC, COD_TIPO_SERV, CODMAT, CODCPL, QUANTIDADE, QUANTIDADE_REMOV, RECURSO, USUARIO, DIA, HORA)
                                                               VALUES(@CODCT, @PROJETO, @NUM_OBRA,
                                                                    @NUM_DOC, @DATA_MAXIMA, @DATA_APLIC, @COD_TIPO_SERV, @CODMAT, @CODCPL, @QUANTIDADE, @QUANTIDADE_REMOV, @RECURSO, @USUARIO, CURRENT_DATE, CURRENT_TIME)");

                parametros.Add("@CODCT", CodCt);
                parametros.Add("@PROJETO", Projeto);
                parametros.Add("@NUM_OBRA", Num_Os);
                parametros.Add("@NUM_DOC", Num_Doc);

                parametros.Add("@DATA_MAXIMA", Data_Maxima);
                parametros.Add("@DATA_APLIC", Data_Aplic);
                parametros.Add("@COD_TIPO_SERV", CodTipoServ);
                parametros.Add("@CODMAT", CodMat);
                parametros.Add("@CODCPL", CodCpl);
                parametros.Add("@QUANTIDADE", Quant);
                parametros.Add("@QUANTIDADE_REMOV", QuantRemov);
                parametros.Add("@RECURSO", Recurso);
                parametros.Add("@USUARIO", Usuario);


                await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);

                return true;

            }

        }

        public String GetCodigoAniel(ClassificacaoMaterial classificacao)
        {
            switch (classificacao)
            {
                case ClassificacaoMaterial.Instalacao:
                    return "I";
                case ClassificacaoMaterial.Defeito:
                    return "D";
                case ClassificacaoMaterial.Def_Estoque:
                    return "E";
                case ClassificacaoMaterial.Retirada:
                    return "R";
                case ClassificacaoMaterial.Reaproveitado:
                    return "B";
                case ClassificacaoMaterial.Sucata:
                    return "S";
            }

            return null;

        }

        /// <summary>
        /// Verifica se o material com código complementar está com uma equipe diferente do parametro
        /// </summary>
        /// <param name="codigoCpl"></param>
        /// <param name="numFre"></param>
        /// <returns></returns>
        async Task<bool> VerificaCodigoCplEquipe(string codigoCpl, string numFre)
        {
            if (string.IsNullOrEmpty(codigoCpl) || string.IsNullOrEmpty(numFre))
            {
                return false;
            }

            using (ISession session = HibernateUtil.GetSession())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                parametros.Add("codcpl", codigoCpl);

                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("select A.codcpl,C.EQUIPE from TB_CODIGO_COMPLEMENTAR A");
                    sql.AppendLine("INNER JOIN TB_PROJETO_EQUIPE B");
                    sql.AppendLine("ON(A.projeto = B.projeto)");
                    sql.AppendLine("INNER JOIN TB_EQUIPE C");
                    sql.AppendLine("ON(B.equipe = C.equipe)");
                    sql.AppendLine("WHERE A.codcpl = @codcpl");

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null)) //request.length, request.start
                    {
                        while (await reader.ReadAsync())
                        {
                            string numFreSql = reader["EQUIPE"].ToString();

                            if (numFreSql.Equals(numFre))
                            {
                                return false; //equipe é a mesma do parâmetro, não está com outra equipe
                            }

                        }
                    }
                }
            }

            return true; //está com outra equipe
        }



        private async Task GerarMovimentacaoLivre(Documento_ProducaoAniel documentoProd, Material_TarefaAniel produto, string usuario, Double saldo, DateTime dataAgendamento, string equipe, ISession session)
        {
            //STRING CODCT = CONFIGURATIONMANAGER.APPSETTINGS["CONTRATOPADRAO"].TOSTRING();
            //STRING PROJETO = CONFIGURATIONMANAGER.APPSETTINGS["PROJETOPADRAO"].TOSTRING();

            double? entradaOuSaida = (produto.Quantidade - saldo) == 0 ? produto.Quantidade : (produto.Quantidade - saldo);

            StringBuilder SQL = new StringBuilder();
            Dictionary<String, Object> PARAMETROS = new Dictionary<string, object>();

            SQL.Append("INSERT INTO TB_ESTOQUE ");
            SQL.Append("(CODCT, PROJETO, SEQTRANSF, SEQRMDM, SEQLIVRE, DATA, CODMAT, CODCPL, ENTRADA, ");
            SQL.Append(" SAIDA, EQUIPE, OBSERVACAO, TRANSFERENCIA, CTORIGEM, PROJORIGEM, ");
            SQL.Append(" CTDESTINO, PROJDESTINO,COD_CCUSTO, USUARIO, DIA, HORA) ");
            SQL.Append("VALUES ");
            SQL.Append(" (@PCODCT, @PPROJETO, @PSEQTRANSF, @PSEQRMDM, @PSEQLIVRE, @PDATA, @PCODMAT, @PCODCPL, ");
            SQL.Append("  @PENTRADA, @PSAIDA, @PEQUIPE, @POBSERVACAO, @PTRANSFERENCIA, @PCTORIGEM, @PPROJORIGEM, ");
            SQL.Append("  @PCTDESTINO, @PPROJDESTINO, @PCOD_CCUSTO, @PUSUARIO, CURRENT_DATE, CURRENT_TIME)");

            //ENTRADA DE MATERIAL
            PARAMETROS.Add("@PCODCT", documentoProd.CodCt);
            PARAMETROS.Add("@PPROJETO", documentoProd.Projeto);
            PARAMETROS.Add("@PSEQTRANSF", "-");
            PARAMETROS.Add("@PSEQRMDM", "-");
            PARAMETROS.Add("@PSEQLIVRE", documentoProd.Num_Obra);
            //PARAMETROS.Add("@PDATA",DATETIME.NOW.TOSHORTDATESTRING().REPLACE('/','.'));
            PARAMETROS.Add("@PDATA", dataAgendamento.ToShortDateString().Replace("/", "."));
            PARAMETROS.Add("@PCODMAT", produto.CodMat);

            try { PARAMETROS.Add("@PCODCPL", produto.CodCpl.ToUpper()); }
            catch { PARAMETROS.Add("@PCODCPL", "-"); }

            PARAMETROS.Add("@PENTRADA", entradaOuSaida);
            PARAMETROS.Add("@PSAIDA", "0");
            PARAMETROS.Add("@PEQUIPE", "-");
            PARAMETROS.Add("@POBSERVACAO", "ENT AUTO REF. OS: " + documentoProd.Num_Obra);
            PARAMETROS.Add("@PTRANSFERENCIA", "N");
            PARAMETROS.Add("@PCTORIGEM", "-");
            PARAMETROS.Add("@PPROJORIGEM", "-");
            PARAMETROS.Add("@PCTDESTINO", "-");
            PARAMETROS.Add("@PPROJDESTINO", "-");

            String CCUSTOPADRAO = await CentroCustoPadrao(documentoProd.CodCt, documentoProd.Projeto, equipe, session);
            PARAMETROS.Add("@PCOD_CCUSTO", CCUSTOPADRAO);

            PARAMETROS.Add("@PUSUARIO", usuario);

            using (var com = session.Connection.CreateCommand())
            {
                await CommandHelper.ExecuteNonQuery(com, SQL, PARAMETROS, session.Connection, null);
            }

            //GERAR RM
            Dictionary<String, Object> PARAMETROSRM = new Dictionary<string, object>();
            PARAMETROSRM.Add("@PCODCT", documentoProd.CodCt);
            PARAMETROSRM.Add("@PPROJETO", documentoProd.Projeto);
            PARAMETROSRM.Add("@PSEQTRANSF", "-");
            PARAMETROSRM.Add("@PSEQRMDM", "-");
            PARAMETROSRM.Add("@PSEQLIVRE", documentoProd.Num_Obra);
            PARAMETROSRM.Add("@PDATA", dataAgendamento.ToShortDateString().Replace("/", "."));
            //PARAMETROSRM.Add("@PDATA", DATETIME.NOW.TOSHORTDATESTRING().REPLACE('/', '.'));
            PARAMETROSRM.Add("@PCODMAT", produto.CodMat);

            try { PARAMETROSRM.Add("@PCODCPL", produto.CodCpl.ToUpper()); }
            catch { PARAMETROSRM.Add("@PCODCPL", "-"); }

            PARAMETROSRM.Add("@PENTRADA", "0");
            PARAMETROSRM.Add("@PSAIDA", entradaOuSaida);
            PARAMETROSRM.Add("@PEQUIPE", equipe);
            PARAMETROSRM.Add("@POBSERVACAO", "RM AUTO REF. OS: " + documentoProd.Num_Obra);
            PARAMETROSRM.Add("@PTRANSFERENCIA", "N");
            PARAMETROSRM.Add("@PCTORIGEM", "-");
            PARAMETROSRM.Add("@PPROJORIGEM", "-");
            PARAMETROSRM.Add("@PCTDESTINO", "-");
            PARAMETROSRM.Add("@PPROJDESTINO", "-");
            PARAMETROSRM.Add("@PCOD_CCUSTO", CCUSTOPADRAO);

            PARAMETROSRM.Add("@PUSUARIO", usuario);
            using (var com = session.Connection.CreateCommand())
            {
                await CommandHelper.ExecuteNonQuery(com, SQL, PARAMETROSRM, session.Connection, com.Transaction);
            }
        }


        private async Task GerarMovimentoTransferenciaSaldo(Documento_ProducaoAniel documentoProd, string codmat, string codcpl, string usuario, string equipe, String equipeRemover, double qtdeComplementar, ISession session)
        {
            //STRING CODCT = CONFIGURATIONMANAGER.APPSETTINGS["CONTRATOPADRAO"].TOSTRING();
            //STRING PROJETO = CONFIGURATIONMANAGER.APPSETTINGS["PROJETOPADRAO"].TOSTRING();

            StringBuilder SQL = new StringBuilder();
            Dictionary<String, Object> PARAMETROS = new Dictionary<string, object>();
            Dictionary<String, Object> PARAMETROS2 = new Dictionary<string, object>();


            SQL.AppendLine("INSERT INTO TB_ESTOQUE");
            SQL.AppendLine("  (CODCT, PROJETO, SEQTRANSF, SEQRMDM, SEQLIVRE, DATA, CODMAT, CODCPL, ENTRADA,");
            SQL.AppendLine("   SAIDA, EQUIPE, OBSERVACAO, TRANSFERENCIA, CTORIGEM, PROJORIGEM,");
            SQL.AppendLine("   CTDESTINO, PROJDESTINO,COD_CCUSTO, USUARIO, DIA, HORA, SEQ_INSERT)");
            SQL.AppendLine("VALUES(@pCODCT, @pPROJETO, @pSEQTRANSF, @pSEQRMDM, @pSEQLIVRE, @pDATA, @pCODMAT,");
            SQL.AppendLine("       @pCODCPL, @pENTRADA, @pSAIDA, @pEQUIPE, @pOBSERVACAO, @pTRANSFERENCIA,");
            SQL.AppendLine("       @pCTORIGEM, @pPROJORIGEM, @pCTDESTINO, @pPROJDESTINO, @pCOD_CCUSTO, @pUSUARIO,");
            SQL.AppendLine("       CURRENT_DATE, CURRENT_TIME, @pSEQ_INSERT)");

            //    With QryTemp Do
            //    Begin
            //{ *gravando a devolução no projeto origem * }
            PARAMETROS.Add("@pCODCT", documentoProd.CodCt);
            PARAMETROS.Add("@pPROJETO", documentoProd.Projeto);
            PARAMETROS.Add("@pSEQTRANSF", '-');
            PARAMETROS.Add("@pSEQRMDM", '-');
            PARAMETROS.Add("@pSEQLIVRE", documentoProd.Num_Obra);
            PARAMETROS.Add("@pDATA", documentoProd.Data_Prod ?? documentoProd.Data_Maxima ?? DateTime.Now);
            PARAMETROS.Add("@pCODMAT", codmat);
            PARAMETROS.Add("@pCODCPL", codcpl);
            PARAMETROS.Add("@pENTRADA", qtdeComplementar);
            PARAMETROS.Add("@pSAIDA", 0);
            PARAMETROS.Add("@pEQUIPE", equipeRemover);
            PARAMETROS.Add("@pOBSERVACAO", "DM REF. TI P/OS:' " + documentoProd.Num_Obra);
            PARAMETROS.Add("@pTRANSFERENCIA", "S");
            PARAMETROS.Add("@pCTDESTINO", "-");
            PARAMETROS.Add("@pPROJDESTINO", "-");
            PARAMETROS.Add("@pCTORIGEM", "-");
            PARAMETROS.Add("@pPROJORIGEM", "-");
            PARAMETROS.Add("@pCOD_CCUSTO", await CentroCustoPadrao(documentoProd.CodCt, documentoProd.Projeto, equipe, session));
            PARAMETROS.Add("@pUSUARIO", usuario);
            PARAMETROS.Add("@pSEQ_INSERT", 0);

            using (var com = session.Connection.CreateCommand())
            {
                await CommandHelper.ExecuteNonQuery(com, SQL, PARAMETROS, session.Connection, com.Transaction);
            }

            //{ *gravando a requisição no projeto destino * }

            PARAMETROS2.Add("@pCODCT", documentoProd.CodCt);
            PARAMETROS2.Add("@pPROJETO", documentoProd.Projeto);
            PARAMETROS2.Add("@pSEQTRANSF", '-');
            PARAMETROS2.Add("@pSEQRMDM", '-');
            PARAMETROS2.Add("@pSEQLIVRE", documentoProd.Num_Obra);
            PARAMETROS2.Add("@pDATA", documentoProd.Data_Prod ?? documentoProd.Data_Maxima ?? DateTime.Now);
            PARAMETROS2.Add("@pCODMAT", codmat);
            PARAMETROS2.Add("@pCODCPL", codcpl);
            PARAMETROS2.Add("@pENTRADA", 0);
            PARAMETROS2.Add("@pSAIDA", qtdeComplementar);
            PARAMETROS2.Add("@pEQUIPE", equipe);
            PARAMETROS2.Add("@pOBSERVACAO", "RM REF. TI P/OS:' " + documentoProd.Num_Obra);
            PARAMETROS2.Add("@pTRANSFERENCIA", "S");
            PARAMETROS2.Add("@pCTDESTINO", "-");
            PARAMETROS2.Add("@pPROJDESTINO", "-");
            PARAMETROS2.Add("@pCTORIGEM", "-");
            PARAMETROS2.Add("@pPROJORIGEM", "-");
            PARAMETROS2.Add("@pCOD_CCUSTO", await CentroCustoPadrao(documentoProd.CodCt, documentoProd.Projeto, equipe, session));
            PARAMETROS2.Add("@pUSUARIO", usuario);
            PARAMETROS2.Add("@pSEQ_INSERT", 1);


            using (var com = session.Connection.CreateCommand())
            {
                await CommandHelper.ExecuteNonQuery(com, SQL, PARAMETROS2, session.Connection, com.Transaction);
            }
        }


        private async Task<String> CentroCustoPadrao(String codCt, String projeto, String equipe, ISession session)
        {
            String retorno;

            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            using (var com = session.Connection.CreateCommand())
            {

                sql.Append("SELECT rCOD_CCUSTO FROM SP_BUSCAR_CENTRO_CUSTO_PADRAO(@pCODCT,@pPROJETO,@pEQUIPE)");

                parametros.Add("@pCODCT", codCt);
                parametros.Add("@pPROJETO", projeto);
                parametros.Add("@pEQUIPE", equipe);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))//request.length, request.start
                {
                    if (await reader.ReadAsync())
                        retorno = reader["rCOD_CCUSTO"].ToString();
                    else
                        retorno = String.Empty;

                }
            }

            return retorno;
        }

        private async Task<Boolean> VerificarDevolucaoComplementar(String codCt, String projeto, String CodMat, String CodCpl, ISession session)
        {
            Boolean retorno = false;

            StringBuilder sql = new StringBuilder();
            Dictionary<string, string> parametros = new Dictionary<string, string>();

            using (var com = session.Connection.CreateCommand())
            {

                sql.Append(@"SELECT B.BAIXADA, A.CANCELADO  FROM TB_MOVIMENTO_RMDM  A
                            INNER JOIN TB_SEQUENCIA_RMDM B ON (A.SEQRMDM = B.SEQRMDM AND A.CODCT = B.CODCT AND A.PROJETO = B.PROJETO)
                            WHERE B.CODCT = @CODCT AND B.PROJETO = @PROJETO
                            AND B.TIPO_MOVIMENTO = 'D' AND B.BAIXADA = 'S' AND B.CANCELADO <> 'S'
                            AND A.CODMAT = @CODMAT AND A.CODCPL = @CODCPL");

                parametros.Add("@CODCT", codCt);
                parametros.Add("@PROJETO", projeto);
                parametros.Add("@CODMAT", CodMat);
                parametros.Add("@CODCPL", CodCpl);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (reader.ReadAsync().Result)
                        retorno = true;
                }
            }

            return retorno;
        }

        private async Task<Boolean> DeletarMovimentoLivreEstoque(String codCt, String projeto, String CodMat, String CodCpl, String Num_Obra, DateTime? data, ISession session)
        {
            Boolean retorno = false;

            StringBuilder sql = new StringBuilder();
            Dictionary<string, object> parametros = new Dictionary<string, object>();



            sql.Append(@"DELETE FROM TB_ESTOQUE WHERE CODCT = @CODCT AND PROJETO = @PROJETO AND CODMAT = @CODMAT AND CODCPL = @CODCPL AND SEQLIVRE = @SEQLIVRE AND DATA = @DATA_APLIC AND TRANSFERENCIA = 'S'");

            parametros.Add("@CODCT", codCt);
            parametros.Add("@PROJETO", projeto);
            parametros.Add("@CODMAT", CodMat);
            parametros.Add("@CODCPL", CodCpl);
            parametros.Add("@SEQLIVRE", Num_Obra);
            parametros.Add("@DATA_APLIC", data);

            using (var com = session.Connection.CreateCommand())
            {
                await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, com.Transaction);
            }


            return retorno;
        }

    }
}