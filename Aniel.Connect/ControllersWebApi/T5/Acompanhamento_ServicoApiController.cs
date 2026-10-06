using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Attributes;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Core.Service;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.ModelLog.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Models.T5;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.EnderecoCliente;
using Aniel.Connect.Repository.EtapaVenda;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.T5.DocumentoProducao;
using Aniel.Connect.Repository.Venda;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.mongo;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Data;
using Microsoft.AspNet.Identity;
using MongoDB.Driver;
using NHibernate;
using NHibernate.Criterion;
using OpenXmlPowerTools;
using Resources;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.repository;
using Sinapse.DAO.Repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Padrao = Sinapse.Model.enumerados.Padrao;

using Microsoft.Ajax.Utilities;
using DocumentFormat.OpenXml.VariantTypes;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class Acompanhamento_ServicoApiController : BaseApiController
    {

        [HttpPost()]
        [Route("api/Acompanhamento_ServicoApi/AtualizaVinculoManutencao")]
        public async void AtualizaVinculoManutencao(AlterarVinculoManutencao request)
        {

            try
            {
                VincularOsModel vinculros = new VincularOsModel();
                vinculros.ids = new List<string>();
                vinculros.ids.Add(request.CodCt + "|" + request.Projeto + "|" + request.Num_Obra + "|" + request.numDoc + "|");
                vinculros.Projeto_Manut = request.Projeto_Manut;
                List<Painel_ServicosListaModel> retorno = new List<Painel_ServicosListaModel>();
                Int32 recordsTotal = 0;
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                {
                    IDocumentoProducaoAnielRepository repo = new DocumentoProducaoAnielRepository(sessionT5, sessionT5);

                    new DocumentoProducaoAnielBusiness(repo).AtualizaVinculoManutencaoOSs(vinculros);

                }

            }
            catch (Exception e)
            {
            }

        }

        [HttpPost()]
        [Route("api/Acompanhamento_ServicoApi/HistoricoAtendimento")]
        public async Task<DataTableResponse<VincularAtendimentoResponse>> HistoricoAtendimento(VincularAtendimento_DataTableRequest request)
        {
            List<VincularAtendimentoResponse> retorno = new List<VincularAtendimentoResponse>();

            Int32 recordsTotal = 0;

            using (ISession session = HibernateUtil.GetSession())
            using (Documento_ProducaoApiController documentoApi = new Documento_ProducaoApiController())
            {

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                StringBuilder sqlFrom = new StringBuilder();


                sqlFrom.AppendLine("FROM TB_ATENDIMENTO A ");
                sqlFrom.AppendLine("LEFT JOIN TB_TIPO_ATENDIMENTO B ON B.ID = A.ID_TIPO_ATENDIMENTO ");
                sqlFrom.AppendLine(" LEFT JOIN TB_CLIENTE C ON C.ID =A.ID_CLIENTE");
                sqlFrom.AppendLine(" LEFT JOIN ASPNETUSERS D ON (A.ID_RESPONSAVEL = D.ID)");
                sqlFrom.AppendLine(" LEFT JOIN TB_MOTIVO_STATUS_ATENDIMENTO E ON A.ID_MOTIVO_STATUS = E.ID");

                sqlFrom.AppendLine(" WHERE A.STATUS_ATENDIMENTO <> 3 AND (A.DATAHORACRIACAO BETWEEN @DATA_INICIAL AND @DATA_FINAL) ");


                try
                {
                    DateTime dataInicial = Convert.ToDateTime(request.DataInicial);
                    DateTime dataFinal = Convert.ToDateTime(request.DataFinal).AddHours(23).AddMinutes(59).AddSeconds(59);

                    if ((dataFinal - dataInicial).TotalDays > 30)
                    {
                        throw new Exception("Período maior que 30 dias");
                    }

                    sqlFrom.AppendLine("  ");
                    parametros.Add("@DATA_INICIAL", dataInicial);
                    parametros.Add("@DATA_FINAL", dataFinal);

                }
                catch
                {
                    throw new Exception("Periodo Inválido");
                }

                if (request.Id_Tipo_Atendimento.HasValue)
                {
                    sqlFrom.AppendLine("AND B.ID = @ID_TIPO_ATENDIMENTO");
                    parametros.Add("@ID_TIPO_ATENDIMENTO", request.Id_Tipo_Atendimento);
                }

                if (!string.IsNullOrEmpty(Convert.ToString(request.Status)))
                {
                    sqlFrom.AppendLine("AND A.STATUS_ATENDIMENTO = @STATUS_ATENDIMENTO");
                    parametros.Add("@STATUS_ATENDIMENTO", request.Status);
                }
                if (!string.IsNullOrEmpty(Convert.ToString(request.Responsavel)))
                {
                    sqlFrom.AppendLine("AND A.ID_RESPONSAVEL = @ID_RESPONSAVEL");
                    parametros.Add("@ID_RESPONSAVEL", request.Responsavel);
                }
                if (!string.IsNullOrEmpty(Convert.ToString(request.Id_Cliente)))
                {
                    sqlFrom.AppendLine("AND A.ID_CLIENTE = @ID_CLIENTE");
                    parametros.Add("@ID_CLIENTE", request.Id_Cliente);
                }
                if (!String.IsNullOrEmpty(request.search.value))
                {
                    sqlFrom.AppendLine(" AND ((UPPER(A.NUMERO) LIKE UPPER(@SEARCH)) OR (UPPER(A.NUMERO) LIKE UPPER(@SEARCH))) ");
                    parametros.Add("@SEARCH", String.Format("%{0}%", request.search.value));
                }

                //Select
                using (var com = session.Connection.CreateCommand())
                {
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine(@"SELECT A.NUMERO,B.NOME AS TIPO_ATENDIMENTO,A.ASSUNTO,C.NOMEFANTASIA,A.DATAHORACRIACAO,A.DATAINDICADOR_ETAPAFLUXO,D.USERNAME,E.DESCRICAO AS MOTIVO_STATUS,A.STATUS_ATENDIMENTO ");
                    sql.AppendLine(sqlFrom.ToString());

                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            DateTime? dataAbertura = reader["DATAHORACRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORACRIACAO"]) : (DateTime?)null;
                            DateTime? dataSla = reader["DATAINDICADOR_ETAPAFLUXO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAINDICADOR_ETAPAFLUXO"]) : (DateTime?)null;

                            var desc_user_name = reader["USERNAME"] != DBNull.Value ? Convert.ToString(reader["USERNAME"]) : String.Empty;
                            var protocolo_numero = reader["NUMERO"] != DBNull.Value ? Convert.ToString(reader["NUMERO"]) : String.Empty;
                            retorno.Add(new VincularAtendimentoResponse
                            {
                                protocolo = protocolo_numero,
                                numero = protocolo_numero,
                                tipoAtendimento = reader["TIPO_ATENDIMENTO"] != DBNull.Value ? Convert.ToString(reader["TIPO_ATENDIMENTO"]) : String.Empty,
                                assunto = reader["ASSUNTO"] != DBNull.Value ? Convert.ToString(reader["ASSUNTO"]) : String.Empty,
                                cliente = reader["NOMEFANTASIA"] != DBNull.Value ? Convert.ToString(reader["NOMEFANTASIA"]) : String.Empty,
                                dataHoraCriacao = dataAbertura.HasValue ? dataAbertura.Value.ToString("dd/MM/yyyy HH:mm:ss") : String.Empty,
                                dataSla = dataSla.HasValue ? dataSla.Value.ToString("dd/MM/yyyy HH:mm:ss") : String.Empty,
                                responsavel = desc_user_name,
                                status = reader["STATUS_ATENDIMENTO"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Status_Atendimento>(Convert.ToString(reader["STATUS_ATENDIMENTO"])) : String.Empty,
                                motivoStatus = reader["MOTIVO_STATUS"] != DBNull.Value ? Convert.ToString(reader["MOTIVO_STATUS"]) : String.Empty,

                            });
                        }
                    }

                }
            }

            //Page e retornando DataTable
            return new DataTableResponse<VincularAtendimentoResponse>
            {
                draw = request.draw,
                //recordsTotal = recordsTotal,
                recordsFiltered = retorno.Count(),
                data = retorno,
                error = ""
            };

        }

        [HttpPost]
        [Route("api/Acompanhamento_ServicoApi/VincularAtendimento")]
        public async Task<PostApiRetorno<Int64>> VincularAtendimento([FromBody] AcompanhamentoListaModel request)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<VincularAtendimento>(session))
            {
                try
                {
                    VincularAtendimento modelo = null;

                    if (request != null)
                    {
                        modelo = (await repository.select(p => p.Numero_Atendimento == request.IdAtendimento && p.Projeto == request.Projeto && p.CodCT == request.CodCt && p.Num_Obra == request.Num_OS && p.Num_Doc == Convert.ToString(request.Num_Doc))).FirstOrDefault();

                        if (modelo == null)
                        {
                            modelo = new VincularAtendimento
                            {
                                Numero_Atendimento = request.IdAtendimento,
                                CodCT = request.CodCt,
                                Projeto = request.Projeto,
                                Num_Obra = request.Num_OS,
                                Num_Doc = Convert.ToString(request.Num_Doc),
                                UsuarioCriacao = User.Identity.GetUserId(),
                                DataHoraCriacao = DateTime.Now,
                            };
                        }
                        else
                        {
                            throw new Exception("Atendimento já está vinculado a essa ordem de serviço");
                        }

                        await repository.SaveOrUpdate(modelo);


                    }

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null)
                        retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;
        }

        [HttpPost]
        [Route("api/Acompanhamento_ServicoApi/HistoricoVincular")]
        public async Task<DataTableResponse<HistoricoVincularAtendimentoResponse>> HistoricoVincular([FromBody] HistoricoVincularOSResponse_DataTableRequest request)
        {
            List<HistoricoVincularAtendimentoResponse> retorno = new List<HistoricoVincularAtendimentoResponse>();
            IOrderedEnumerable<HistoricoVincularAtendimentoResponse> listaOrdenada = null;
            var response = new DataTableResponse<HistoricoVincularAtendimentoResponse>
            {
                draw = request.draw,
                data = new List<HistoricoVincularAtendimentoResponse>()
            };

            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<VincularAtendimento>(session))
            using (var repositoryCliente = new Conexao<Cliente>(session))
            using (var repositoryTipoAtendimento = new Conexao<Tipo_Atendimento>(session))
            using (var repositoryMotivoStatus = new Conexao<Motivo_Status_Atendimento>(session))
            using (var repositoryAtendimento = new Conexao<Atendimento>(session))
            {
                try
                {
                    var oss = await repository.select(p => p.CodCT == request.CodCt && p.Projeto == request.Projeto && p.Num_Doc == Convert.ToString(request.Num_Doc) && p.Num_Obra == request.Num_Obra);

                    foreach (var os in oss)
                    {
                        var atendimentos = await repositoryAtendimento.select(p => p.Numero == os.Numero_Atendimento);

                        foreach (var atendimento in atendimentos)
                        {
                            var tipoAtendimento = await repositoryTipoAtendimento.selectFirst(p => p.Id == atendimento.Tipo_Atendimento.Id);
                            var motivostatus = atendimento.Motivo_Status?.Id != null ? await repositoryMotivoStatus.selectFirst(p => p.Id == atendimento.Motivo_Status.Id) : null;
                            var cliente = await repositoryCliente.selectFirst(p => p.Id == atendimento.Cliente.Id);

                            retorno.Add(new HistoricoVincularAtendimentoResponse
                            {
                                protocolo = atendimento.Numero,
                                tipoAtendimento = tipoAtendimento.Nome,
                                assunto = atendimento.Assunto,
                                cliente = cliente?.NomeFantasia ?? "N/A",
                                dataHoraCriacao = atendimento.DataHoraCriacao?.ToString("dd/MM/yyyy") ?? "N/A",
                                dataSla = atendimento.DataIndicador_EtapaFluxo?.ToString("dd/MM/yyyy") ?? "N/A",
                                responsavel = Convert.ToString(atendimento.Responsavel.Nome),
                                status = HtmlEnumExtensions.GetEnumDescription<Status_Atendimento>(Convert.ToString(atendimento.Status_Atendimento)),
                                motivoStatus = atendimento.Motivo_Status?.Descricao ?? "N/A"
                            });
                        }
                    }

                    response.recordsFiltered = retorno.Count;
                    response.data = retorno;
                }
                catch (Exception ex)
                {
                    response.error = "Erro interno: " + ex.Message + (ex.InnerException?.Message ?? "");
                }
            }

            return response;
        }

        [HttpPost]
        [Route("api/Acompanhamento_ServicoApi/DesvincularOS")]
        public async Task<PostApiRetorno<Int64>> DesvincularOS([FromBody] AcompanhamentoListaModel request)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repository = new Conexao<VincularAtendimento>(session))
            {
                try
                {
                    VincularAtendimento modelo = null;

                    if (request != null)
                    {
                        modelo = (await repository.select(p => p.Numero_Atendimento == request.IdAtendimento && p.Projeto == request.Projeto && p.CodCT == request.CodCt && p.Num_Obra == request.Num_OS && p.Num_Doc == Convert.ToString(request.Num_Doc))).FirstOrDefault();

                        if (modelo != null)
                        {
                            await repository.Remove(modelo);
                            retorno.Ok = true;
                            retorno.Mensagem = "OS desvinculada com sucesso.";
                        }
                        else
                        {

                            throw new Exception("Vínculo não encontrado.");

                        }

                    }

                    retorno.Ok = true;
                }
                catch (Exception ex)
                {
                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null)
                        retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;
        }



        [HttpGet()]
        [Route("api/Acompanhamento_ServicoApi/GetDadosGerais")]
        public async Task<GetApiRetorno<DadosGerais_CadastroModel>> GetDadosGerais_Cliente(String CodCt, String Projeto, String Num_Obra, Int32? Num_Doc)
        {
            GetApiRetorno<DadosGerais_CadastroModel> retorno = new GetApiRetorno<DadosGerais_CadastroModel>();
            DadosGerais_CadastroModel _obj = new DadosGerais_CadastroModel();
            Tipo_Servico_EquipeAniel tipo_servico_equipe = new Tipo_Servico_EquipeAniel();


            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionNh = HibernateUtil.GetSession())
            using (ISession sessionNhTmp = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
            using (Conexao<Plano> repoPlano = new Conexao<Plano>(session))
            using (Conexao<Cliente_PlanoProduto> repoClientePlano = new Conexao<Cliente_PlanoProduto>(session))
            using (Conexao<Tecnologia> repoTecnologia = new Conexao<Tecnologia>(session))
            using (Conexao<Atendimento> repoAtendimento = new Conexao<Atendimento>(sessionNhTmp))
            using (Conexao<Endereco_Cliente> repoEndereco_Cliente = new Conexao<Endereco_Cliente>(session))
            using (Conexao<Categoria_Cliente> repoCategoria = new Conexao<Categoria_Cliente>(session))
            using (Conexao<Cliente_PlanoProduto> repoCliente_PlanoProduto = new Conexao<Cliente_PlanoProduto>(sessionNhTmp))
            using (Conexao<Cliente> repoCliente = new Conexao<Cliente>(sessionNhTmp))
            {
                Boolean Exibe_RDOs = (await new Conexao<Configuracoes_Gerais>(session).selectFirst()).ExibeRDOs;

                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                try
                {
                    using (var com = sessionT5.Connection.CreateCommand())
                    {

                        StringBuilder sql = new StringBuilder("SELECT A.TIPO, A.CODCT, A.COD_DEPTO, A.COD_DEPTO, A.PROJETO, A.NUM_OBRA,A.NUM_DOC, A.STATUS_EXECUCAO, A.STATUS_AUDITORIA, A.DATA_MAXIMA AS DATA_MAXIMA, A.HORA_MAXIMA AS HORA_MAXIMA,A.RESPONSABILIDADE, ")
                        .AppendLine("  A.DATA_MAXIMA + A.HORA_MAXIMA AS DATAHORA_MAXIMA, A.DATA_INDICADOR + A.HORA_INDICADOR AS DATAHORA_INDICADOR,A.NOME_INSTALACAO,A.CPF_INSTALACAO,A.TELEFONE_INSTALACAO,A.UTILIZA_END_COND, A.END_CONDPRED_SELECIONADO, A.BUSCAR_ENDPOR_ID, ")
                        .AppendLine("  A.NUM_OBRA_ORIGINAL, A.OBSERVACAO, A.ID_ENDERECO_CLIENTE_ANTIGO, A.ENDERECO, A.ENDERECO_CPL AS CEP,COALESCE(A.ENDERECO_TIPO,A.ENDERECO_CIDADE) AS CIDADE,A.ENDERECO_BAIRRO,A.ENDERECO_NUMERO, ")
                        .AppendLine("  A.ENDERECO_COMPL AS COMPLEMENTO, A.SEGMENTO, A.OBSERVACAO_TESTE, G.DESCRICAO AS NATUREZA, ")
                        .AppendLine("  K.COD_TIPO_SERV AS COD_TIPO_SERVICO, K.DESCRICAO AS TIPO_SERVICO, L.COD_SUB_TIPO_SERV AS COD_SUBTIPO_SERVICO, L.DESCRICAO AS SUBTIPO_SERVICO, A.ORIGEM_DOCUMENTO, ")
                        .AppendLine("  A.DATA_ABERTURA + A.HORA_ABERTURA AS DATAHORA_ABERTURA, A.STATUS_EXECUCAO AS STATUS_OS, ")
                        .AppendLine("  COALESCE(B.TELEFONE,A.TAREFA) AS TELEFONE, B.DATA_CADASTRO, COALESCE(B.TELEFONE2,A.TEL_CIENTE) AS TELEFONE2,")
                        .AppendLine("  COALESCE(B.EMAIL, A.EMAIL_ASSINANTE) AS EMAIL, COALESCE(B.RAZAO_SOCIAL, A.TITULAR) AS RAZAO_SOCIAL, COALESCE(B.CNPJ, A.CPF_CLIENTE) AS CNPJ, A.ID_CLIENTE,")
                        .AppendLine("  C.DESCRICAO AS CATEGORIA_CLIENTE, B.ID_CATEGORIA AS COD_CAT_CLIENTE, D.DESCRICAO AS AREA, E.LOCALIDADE, A.DIA + A.HORA AS DATAHORA_CADASTRO, ")
                        .AppendLine("  H.DESCRICAO AS TIPO_IMOVEL, COALESCE(H.COD_TIPO_IMOVEL, '1') COD_TIPO_IMOVEL, A.ID_PLANOPRODUTO, J.COD_ESCRITORIO AS COD_GRUPO_AREA, A.ID_CLIENTE_PLANOPRODUTOCONNECT,  J.DESCRICAO AS GRUPO_AREA, ")
                        .AppendLine("  A.LATITUDE_STR, A.LONGITUDE_STR, A.OBSERVACAO_AUX,A.COD_TURNO_SERV,M.DESCRICAO AS DESC_TURNO, N.DESCRICAO AS DEPARTAMENTO,")
                        .AppendLine("  A.DATA_PROD + HORA_PROD AS DATAHORA_PROD, A.ADICIONAL_1, A.ADICIONAL_2, A.ADICIONAL_3, A.ADICIONAL_4, ")
                        .AppendLine("  A.ADICIONAL_5, A.ADICIONAL_6, A.ADICIONAL_7, A.ADICIONAL_8, A.ADICIONAL_9, A.ADICIONAL_10, A.TIPO, A.ID_POP, A.ID_OLT, A.ID_ELEMENTO_REDE, A.ENDERECO_UF, A.ENDERECO_PAIS,")
                        .AppendLine("  A.OBSERVACAO_PAUSA,A.OBSERVACAO_OITV,A.ID_ATENDIMENTO_CONNECT,K.EXIGE_PAGAMENTO_INSTALACAO,A.COD_ESCRITORIO,A.COD_AREA,A.COD_LOCALIDADE,A.TIPO_PRIORIDADE, O.DESCRICAO AS DESCRICAO_CONTRATO, A.ID_VENDA,A.PROJETO_MANUT,A.CODCT_MANUT,A.NumObra_Manut,A.numDoc_Manut, A.BLOCO, A.APARTAMENTO, ")
                        .AppendLine("  A.ID_CLIENTE_PLANOCONNECT_ANT, A.DESC_PLANO_ANT, A.TIPO_MANUTENCAO ")

                        .AppendLine("FROM TB_DOCUMENTO_PRODUCAO A ")

                        .AppendLine("LEFT JOIN TB_CLIENTE B ") //ID CLIENTE
                        .AppendLine("ON(B.COD_CLIENTE = A.ID_CLIENTE) ")

                        .AppendLine("LEFT JOIN TB_CATEGORIA_CLIENTE_CONNECT C ")
                        .AppendLine("ON(B.ID_CATEGORIA = C.ID) ")

                        .AppendLine("LEFT JOIN TB_AREA D ")
                        .AppendLine("ON(A.COD_ESCRITORIO = D.COD_ESCRITORIO AND D.COD_AREA = A.COD_AREA) ")

                        .AppendLine("LEFT JOIN TB_LOCALIDADE E ")
                        .AppendLine("ON(A.COD_ESCRITORIO = E.COD_ESCRITORIO AND A.COD_AREA = E.COD_AREA AND E.COD_LOCALIDADE = A.COD_LOCALIDADE) ")

                        .AppendLine("LEFT JOIN TB_EQUIPE_NATUREZA F ")
                        .AppendLine("ON(F.EQUIPE = A.EQUIPE) ")

                        .AppendLine("LEFT JOIN TB_TIPO_SERVICO_EQUIPE K ")
                        .AppendLine("ON(A.COD_TIPO_SERV = K.COD_TIPO_SERV) ")

                        .AppendLine("LEFT JOIN TB_NATUREZA_PRODUCAO G ")
                        .AppendLine("ON(K.COD_NATUREZA = G.ID) ")

                        .AppendLine("LEFT JOIN TB_TIPO_IMOVEL H ")
                        .AppendLine("ON(H.COD_TIPO_IMOVEL = A.PARPRIM_PARA)")

                        .AppendLine("LEFT JOIN TB_PLANO_PRODUTO I ")
                        .AppendLine("ON(A.ID_PLANOPRODUTO = I.ID) ")

                        .AppendLine("LEFT JOIN TB_ESCRITORIO J ")
                        .AppendLine("ON(A.COD_ESCRITORIO = J.COD_ESCRITORIO) ")

                        .AppendLine("LEFT JOIN TB_SUB_TIPO_SERVICO L ")
                        .AppendLine("ON(A.COD_SUB_TIPO_SERV = L.COD_SUB_TIPO_SERV AND A.COD_TIPO_SERV = L.COD_TIPO_SERV) ")

                        .AppendLine("LEFT JOIN TB_TURNO_SERVICO M ")
                        .AppendLine("ON(A.COD_TURNO_SERV = M.ID) ")

                        .AppendLine("LEFT JOIN TB_FOLHA_DEPTO N ")
                        .AppendLine("ON(A.COD_DEPTO = N.COD_DEPTO) ")

                        .AppendLine("LEFT JOIN TB_CONTRATO O ")
                        .AppendLine("ON(A.CODCT = O.CODCT) ")

                        .AppendLine(" WHERE A.CODCT = @CODCT ")
                        .AppendLine(" AND A.PROJETO = @PROJETO ")
                        .AppendLine(" AND A.NUM_OBRA = @NUM_OBRA ")
                        .AppendLine(" AND A.NUM_DOC = @NUM_DOC ");

                        if (!Exibe_RDOs)
                            sql.AppendLine(" AND COALESCE(A.SIGLA_MODULO, '') <> 'MCO' ");

                        parametros.Add("@CODCT", CodCt);
                        parametros.Add("@PROJETO", Projeto);
                        parametros.Add("@NUM_OBRA", Num_Obra);
                        parametros.Add("@NUM_DOC", Num_Doc);
                        Int64 _IdAtendimento = 0;
                        Int64 _IdClientePlanoProduto = 0;
                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, sessionT5.Connection, null, 10, 0))
                        {

                            while (await reader.ReadAsync())
                            {

                                _obj.CodCt_Manut = reader["CodCt_Manut"] != DBNull.Value ? Convert.ToString(reader["CodCt_Manut"]) : "-";
                                _obj.Num_Obra_Manut = reader["NumObra_Manut"] != DBNull.Value ? Convert.ToString(reader["NumObra_Manut"]) : "-";
                                _obj.numDoc_Manut = reader["numDoc_Manut"] != DBNull.Value ? Convert.ToInt32(reader["numDoc_Manut"]) : 0;
                                _obj.Projeto_Manut = reader["Projeto_Manut"] != DBNull.Value ? Convert.ToString(reader["Projeto_Manut"]) : "-";
                                _obj.Id_Cliente = reader["ID_CLIENTE"] != DBNull.Value ? Convert.ToInt64(reader["ID_CLIENTE"]) : 0;
                                _obj.Id_Venda = reader["ID_VENDA"] != DBNull.Value ? Convert.ToInt64(reader["ID_VENDA"]) : (long?)null;
                                _obj.Id_Troca_Plano = reader["ID_CLIENTE_PLANOCONNECT_ANT"] != DBNull.Value ? Convert.ToInt64(reader["ID_CLIENTE_PLANOCONNECT_ANT"]) : (long?)null;
                                _obj.Descricao_Plano_Anterior = Convert.ToString(reader["DESC_PLANO_ANT"]);
                                _obj.Nome_Cliente = reader["RAZAO_SOCIAL"] != DBNull.Value ? Convert.ToString(reader["RAZAO_SOCIAL"]) : "-";
                                _obj.Cpf_Cnpj = reader["CNPJ"] != DBNull.Value ? Convert.ToString(reader["CNPJ"]) : "-";
                                _obj.Identificacao_Cliente = reader["NUM_DOC"] != DBNull.Value ? Convert.ToString(reader["NUM_DOC"]) : "-";
                                _obj.Categoria_Cliente = reader["CATEGORIA_CLIENTE"] != DBNull.Value ? Convert.ToString(reader["CATEGORIA_CLIENTE"]) : "-";
                                _obj.Observacao_Aux = reader["OBSERVACAO_AUX"] != DBNull.Value ? Convert.ToString(reader["OBSERVACAO_AUX"]) : "-";
                                _obj.InformacoesComplementares = reader["OBSERVACAO_OITV"] != DBNull.Value ? Convert.ToString(reader["OBSERVACAO_OITV"]) : String.Empty;
                                _obj.Tipo = reader["Tipo"] != DBNull.Value ? Convert.ToString(reader["Tipo"]) : "-";
                                var categoria_Cliente = Convert.ToString(reader["COD_CAT_CLIENTE"]);
                                var prioridade_cliente = categoria_Cliente != "" ? repoCategoria.selectFirst(p => p.Id == Convert.ToInt16(categoria_Cliente)).Result : null;
                                if (prioridade_cliente != null)
                                {
                                    _obj.Prioridade_Cliente = prioridade_cliente.Tipo_Prioridade != null ?
                                                                (Tipo_PrioridadeT5)Enum.Parse(typeof(Tipo_PrioridadeT5), Convert.ToString(prioridade_cliente.Tipo_Prioridade)) : (Tipo_PrioridadeT5?)null;
                                }
                                else
                                {
                                    _obj.Prioridade_Cliente = null;
                                }
                                _obj.Observacao_Pausa = reader["OBSERVACAO_PAUSA"] != DBNull.Value ? Convert.ToString(reader["OBSERVACAO_PAUSA"]) : "-";

                                _obj.Tempo_Cliente = reader["DATA_CADASTRO"] != DBNull.Value ? (DateTime.Now - Convert.ToDateTime(reader["DATA_CADASTRO"])).Days.ToString() + " Dias" : String.Empty;
                                _obj.Telefone = reader["TELEFONE"] != DBNull.Value ? Convert.ToString(reader["TELEFONE"]) : "-";
                                _obj.Telefone_Celular = reader["TELEFONE2"] != DBNull.Value ? Convert.ToString(reader["TELEFONE2"]) : "-";
                                _obj.Email = reader["EMAIL"] != DBNull.Value ? Convert.ToString(reader["EMAIL"]) : "-";

                                _obj.Id_PlanoProduto = reader["ID_CLIENTE_PLANOPRODUTOCONNECT"] != DBNull.Value ? Convert.ToInt64(reader["ID_CLIENTE_PLANOPRODUTOCONNECT"]) : (Int64?)null;

                                _obj.Latitude_Str = Convert.ToString(reader["LATITUDE_STR"]).Replace(',', '.');
                                _obj.Longitude_Str = Convert.ToString(reader["LONGITUDE_STR"]).Replace(',', '.');

                                _obj.Origem_Ordem_Servico = reader["ORIGEM_DOCUMENTO"] != DBNull.Value ?
                                                            (Origem_Documento_Producao)Enum.Parse(typeof(Origem_Documento_Producao), Convert.ToString(reader["ORIGEM_DOCUMENTO"])) : Origem_Documento_Producao.Connect;

                                _obj.Id_Origem_Ordem_Servico = Convert.ToString(reader["ORIGEM_DOCUMENTO"]);


                                _obj.Id_Tipo_Servico = reader["COD_TIPO_SERVICO"] != DBNull.Value ? Convert.ToInt32(reader["COD_TIPO_SERVICO"]) : (Int32?)null;
                                _obj.TipoServico = reader["TIPO_SERVICO"] != DBNull.Value ? Convert.ToString(reader["TIPO_SERVICO"]) : "";
                                _obj.Id_Subtipo_Servico = Convert.ToString(reader["COD_SUBTIPO_SERVICO"]);
                                _obj.Subtipo_Servico = Convert.ToString(reader["SUBTIPO_SERVICO"]);

                                if (_obj.Tipo == "M")
                                {
                                    var desc = "";
                                    if(reader["TIPO_MANUTENCAO"] != DBNull.Value)
                                        switch (Convert.ToInt32(reader["TIPO_MANUTENCAO"]))
                                        {
                                            case 0:
                                                desc = "Corretiva";
                                                break;
                                            case 1:
                                                desc = "Preventiva";
                                                break;
                                        }
                                    _obj.DescTipoManutencao = desc;
                                }

                                #region [Endereço Mudança]
                                _IdClientePlanoProduto = reader["ID_CLIENTE_PLANOPRODUTOCONNECT"] != DBNull.Value ? Convert.ToInt64(reader["ID_CLIENTE_PLANOPRODUTOCONNECT"]) : 0;
                                _IdAtendimento = reader["ID_ATENDIMENTO_CONNECT"] != DBNull.Value ? Convert.ToInt64(reader["ID_ATENDIMENTO_CONNECT"]) : 0;

                                var atendimento = await repoAtendimento.selectFirst(x => x.Id == _IdAtendimento);
                                var flagEnderecoMudanca_Atendimento = atendimento?.Endereco_Mudanca != null;
                                var flagEnderecoMudanca_ClientePlanoProduto = await repoCliente_PlanoProduto.selectFirst(x => x.Id == _IdClientePlanoProduto && x.Endereco_Mudanca_Cliente != null) != null;

                                if ((_IdAtendimento > 0 && flagEnderecoMudanca_Atendimento) || (_IdClientePlanoProduto > 0 && flagEnderecoMudanca_ClientePlanoProduto))
                                {
                                    var _idGenerecio = _IdAtendimento > 0 ? _IdAtendimento : _IdClientePlanoProduto;
                                    Dictionary<String, Object> _parametros = new Dictionary<string, object>() { { "@ID", _idGenerecio } };

                                    var _queryEnderecoCliente = new StringBuilder();
                                    _queryEnderecoCliente.AppendLine("SELECT COALESCE(B.ENDERECO_CEP, C.CEP) ENDERECO_CEP, COALESCE(B.ENDERECO_LOGRADOURO, C.DESCRICAO) ENDERECO_LOGRADOURO, B.ENDERECO_NUMERO,");
                                    _queryEnderecoCliente.AppendLine("COALESCE(B.ENDERECO_UF, F.SIGLA) ENDERECO_UF, COALESCE(B.ENDERECO_CIDADE, G.DESCRICAO) ENDERECO_CIDADE, COALESCE(B.ENDERECO_BAIRRO, H.DESCRICAO) ENDERECO_BAIRRO,");
                                    _queryEnderecoCliente.AppendLine("COALESCE(E.NOME, G.PAIS) ENDERECO_PAIS, COALESCE(B.ENDERECO_COMPLEMENTO, '') ENDERECO_COMPLEMENTO, COALESCE(B.ID_TIPO_IMOVEL, D.COD_TIPO_IMOVEL) COD_TIPO_IMOVEL,");
                                    _queryEnderecoCliente.AppendLine("COALESCE(B.ENDERECO_LONGITUDE, '') ENDERECO_LONGITUDE, COALESCE(B.ENDERECO_LATITUDE, '') ENDERECO_LATITUDE, COALESCE(B.ENDERECO_PONTO_REFERENCIA, '') ENDERECO_PONTO_REFERENCIA,");
                                    _queryEnderecoCliente.AppendLine("COALESCE(B.COD_AREA, C.COD_AREA) COD_AREA, COALESCE(B.COD_EESCRITORIO, C.COD_ESCRITORIO) COD_ESCRITORIO, COALESCE(B.COD_LOCALIDADE, C.COD_LOCALIDADE) COD_LOCALIDADE,");
                                    _queryEnderecoCliente.AppendLine("COALESCE(B.BLOCO, '') BLOCO, COALESCE(B.APARTAMENTO, '') APARTAMENTO, COALESCE(B.ID_CONDOMINIO_PREDIO, 0) ID_CONDOMINIO_PREDIO, COALESCE(B.BUSCA_ENDERECO, 0) BUSCA_ENDERECO,");
                                    _queryEnderecoCliente.AppendLine("COALESCE(B.LOCALIDADE, '') LOCALIDADE");

                                    if (_IdAtendimento > 0 && flagEnderecoMudanca_Atendimento)
                                    {
                                        _queryEnderecoCliente.AppendLine("FROM TB_ATENDIMENTO A");
                                        _queryEnderecoCliente.AppendLine("INNER JOIN TB_ENDERECO_CLIENTE B ON A.ID_ENDERECO_MUDANCA = B.ID");
                                    }
                                    else
                                    {
                                        _queryEnderecoCliente.AppendLine("FROM TB_CLIENTE_PLANO_PRODUTO A");
                                        _queryEnderecoCliente.AppendLine("INNER JOIN TB_ENDERECO_CLIENTE B ON A.ID_ENDERECO_MUDANCA_CLIENTE = B.ID");
                                    }

                                    _queryEnderecoCliente.AppendLine("LEFT JOIN TB_ENDERECO_LOGRADOURO C ON B.ID_ENDERECO_LOGRADOURO = C.ID");
                                    _queryEnderecoCliente.AppendLine("LEFT JOIN TB_TIPO_IMOVEL D ON B.ID_TIPO_IMOVEL = D.ID");
                                    _queryEnderecoCliente.AppendLine("LEFT JOIN TB_PAIS E ON(B.ID_PAIS = E.ID)");
                                    _queryEnderecoCliente.AppendLine("LEFT JOIN TB_UF F ON(C.ID_UF = F.ID)");
                                    _queryEnderecoCliente.AppendLine("LEFT JOIN TB_CIDADE G ON(C.ID_CIDADE = G.ID)");
                                    _queryEnderecoCliente.AppendLine("LEFT JOIN TB_BAIRRO H ON(C.ID_BAIRRO = H.ID)");
                                    _queryEnderecoCliente.AppendLine("WHERE A.ID = @ID");

                                    using (var readerEnd = await CommandHelper.ExecuteReaderFirst(session.Connection.CreateCommand(), _queryEnderecoCliente, _parametros, session.Connection, null, 10, 0))
                                    {
                                        while (await readerEnd.ReadAsync())
                                        {
                                            _obj.CodCt = Convert.ToString(reader["CODCT"]);
                                            _obj.Projeto = Convert.ToString(reader["PROJETO"]);
                                            _obj.CodCt_Antigo = _obj.CodCt;
                                            _obj.Projeto_Antigo = _obj.Projeto;
                                            using (var repoContrato = new Conexao<ContratoAniel>(sessionT5Nh))
                                            {
                                                _obj.DescricaoContrato = repoContrato.selectFirst(x => x.CodCt == _obj.CodCt).Result?.Descricao;
                                            }
                                            _obj.DescricaoContrato_Antigo = _obj.DescricaoContrato;
                                            _obj.Cep = Convert.ToString(readerEnd["ENDERECO_CEP"]);
                                            _obj.Logradouro = Convert.ToString(readerEnd["ENDERECO_LOGRADOURO"]);
                                            _obj.Numero = Convert.ToString(readerEnd["ENDERECO_NUMERO"]);
                                            _obj.Uf = Convert.ToString(readerEnd["ENDERECO_UF"]);
                                            _obj.Cidade = Convert.ToString(readerEnd["ENDERECO_CIDADE"]);
                                            _obj.Bairro = Convert.ToString(readerEnd["ENDERECO_BAIRRO"]);
                                            _obj.Pais = Convert.ToString(readerEnd["ENDERECO_PAIS"]);
                                            _obj.Complemento = Convert.ToString(readerEnd["ENDERECO_COMPLEMENTO"]);
                                            _obj.Cod_Tipo_Imovel = readerEnd["COD_TIPO_IMOVEL"] != DBNull.Value ? Convert.ToInt32(readerEnd["COD_TIPO_IMOVEL"]) : (int?)null;
                                            _obj.Tipo_Imovel = readerEnd["COD_TIPO_IMOVEL"] != DBNull.Value ? Convert.ToString(readerEnd["COD_TIPO_IMOVEL"]) : string.Empty;
                                            _obj.Latitude_Str = Convert.ToString(readerEnd["ENDERECO_LATITUDE"]).Replace(',', '.');
                                            _obj.Longitude_Str = Convert.ToString(readerEnd["ENDERECO_LONGITUDE"]).Replace(',', '.');
                                            _obj.Ponto_Referencia = Convert.ToString(readerEnd["ENDERECO_PONTO_REFERENCIA"]);
                                            _obj.Cod_Area = Convert.ToString(readerEnd["COD_AREA"]);
                                            _obj.Localidade = Convert.ToString(readerEnd["LOCALIDADE"]);
                                            _obj.Cod_Escritorio = Convert.ToString(readerEnd["COD_ESCRITORIO"]);
                                            _obj.Cod_Localidade = Convert.ToString(readerEnd["COD_LOCALIDADE"]);

                                            _obj.Bloco = Convert.ToString(readerEnd["BLOCO"]);
                                            _obj.Apartamento = Convert.ToString(readerEnd["APARTAMENTO"]);
                                            _obj.UtilizaEndeCondo = Convert.ToInt64(readerEnd["ID_CONDOMINIO_PREDIO"]) > 0 ? true : false;
                                            _obj.IdCondominioPredioEndereco = Convert.ToInt64(readerEnd["ID_CONDOMINIO_PREDIO"]);
                                            _obj.BuscarEnderecoPorId = Convert.ToInt32(readerEnd["BUSCA_ENDERECO"]);
                                        }
                                    }

                                    if (string.IsNullOrEmpty(_obj.Cep))
                                    {
                                        _obj.Cep = Convert.ToString(reader["CEP"]);
                                        _obj.Logradouro = Convert.ToString(reader["ENDERECO"]);
                                        _obj.Numero = Convert.ToString(reader["ENDERECO_NUMERO"]);
                                        _obj.Uf = Convert.ToString(reader["ENDERECO_UF"]);
                                        _obj.Cidade = Convert.ToString(reader["CIDADE"]);
                                        _obj.Bairro = Convert.ToString(reader["ENDERECO_BAIRRO"]);
                                        _obj.Pais = Convert.ToString(reader["ENDERECO_PAIS"]);
                                        _obj.Complemento = Convert.ToString(reader["COMPLEMENTO"]);
                                        _obj.Cod_Tipo_Imovel = reader["COD_TIPO_IMOVEL"] != DBNull.Value ? Convert.ToInt32(reader["COD_TIPO_IMOVEL"]) : (Int32?)null;
                                        _obj.Tipo_Imovel = Convert.ToString(reader["COD_TIPO_IMOVEL"]);
                                        _obj.Latitude_Str = Convert.ToString(reader["LATITUDE_STR"]).Replace(',', '.');
                                        _obj.Longitude_Str = Convert.ToString(reader["LONGITUDE_STR"]).Replace(',', '.');
                                        _obj.Ponto_Referencia = Convert.ToString(reader["OBSERVACAO"]);
                                        _obj.Grupo_Area = Convert.ToString(reader["GRUPO_AREA"]);
                                        _obj.Cod_Area = Convert.ToString(reader["COD_AREA"]);
                                        _obj.Localidade = Convert.ToString(reader["LOCALIDADE"]);
                                        _obj.Area = Convert.ToString(reader["AREA"]);
                                        _obj.Cod_Escritorio = Convert.ToString(reader["COD_ESCRITORIO"]);
                                        _obj.Cod_Localidade = Convert.ToString(reader["COD_LOCALIDADE"]);
                                        _obj.CodCt = Convert.ToString(reader["CODCT"]);
                                        _obj.Projeto = Convert.ToString(reader["PROJETO"]);
                                        _obj.Bloco = reader["BLOCO"] != DBNull.Value ? Convert.ToString(reader["BLOCO"]) : String.Empty;
                                        _obj.Apartamento = reader["APARTAMENTO"] != DBNull.Value ? Convert.ToString(reader["APARTAMENTO"]) : String.Empty;
                                        _obj.UtilizaEndeCondo = reader["UTILIZA_END_COND"] != DBNull.Value ? Convert.ToBoolean(reader["UTILIZA_END_COND"]) : false;
                                        _obj.IdCondominioPredioEndereco = reader["END_CONDPRED_SELECIONADO"] != DBNull.Value ? Convert.ToInt64(reader["END_CONDPRED_SELECIONADO"]) : (Int64?)null;
                                        _obj.BuscarEnderecoPorId = reader["BUSCAR_ENDPOR_ID"] != DBNull.Value ? Convert.ToInt32(reader["BUSCAR_ENDPOR_ID"]) : 0;
                                        _obj.DescricaoContrato = Convert.ToString(reader["DESCRICAO_CONTRATO"]);
                                    }
                                    else
                                    {
                                        if (_IdClientePlanoProduto > 0 && flagEnderecoMudanca_ClientePlanoProduto)
                                        {
                                            _queryEnderecoCliente = new StringBuilder();
                                            _queryEnderecoCliente.AppendLine("SELECT COALESCE(B.ENDERECO_CEP, C.CEP) ENDERECO_CEP, COALESCE(B.ENDERECO_LOGRADOURO, C.DESCRICAO) ENDERECO_LOGRADOURO, B.ENDERECO_NUMERO,");
                                            _queryEnderecoCliente.AppendLine("COALESCE(B.ENDERECO_UF, F.SIGLA) ENDERECO_UF, COALESCE(B.ENDERECO_CIDADE, G.DESCRICAO) ENDERECO_CIDADE, COALESCE(B.ENDERECO_BAIRRO, H.DESCRICAO) ENDERECO_BAIRRO,");
                                            _queryEnderecoCliente.AppendLine("COALESCE(E.NOME, G.PAIS) ENDERECO_PAIS, COALESCE(B.ENDERECO_COMPLEMENTO, '') ENDERECO_COMPLEMENTO, COALESCE(B.ID_TIPO_IMOVEL, D.COD_TIPO_IMOVEL) COD_TIPO_IMOVEL,");
                                            _queryEnderecoCliente.AppendLine("COALESCE(B.ENDERECO_LONGITUDE, '') ENDERECO_LONGITUDE, COALESCE(B.ENDERECO_LATITUDE, '') ENDERECO_LATITUDE, COALESCE(B.ENDERECO_PONTO_REFERENCIA, '') ENDERECO_PONTO_REFERENCIA,");
                                            _queryEnderecoCliente.AppendLine("COALESCE(B.COD_AREA, C.COD_AREA) COD_AREA, COALESCE(B.COD_EESCRITORIO, C.COD_ESCRITORIO) COD_ESCRITORIO, COALESCE(B.COD_LOCALIDADE, C.COD_LOCALIDADE) COD_LOCALIDADE,");
                                            _queryEnderecoCliente.AppendLine("COALESCE(B.BLOCO, '') BLOCO, COALESCE(B.APARTAMENTO, '') APARTAMENTO, COALESCE(B.ID_CONDOMINIO_PREDIO, 0) ID_CONDOMINIO_PREDIO, COALESCE(B.BUSCA_ENDERECO, 0) BUSCA_ENDERECO,");
                                            _queryEnderecoCliente.AppendLine("COALESCE(B.LOCALIDADE, '') LOCALIDADE");
                                            _queryEnderecoCliente.AppendLine("FROM TB_CLIENTE_PLANO_PRODUTO A");
                                            _queryEnderecoCliente.AppendLine("INNER JOIN TB_ENDERECO_CLIENTE B ON A.ID_ENDERECO_INSTALACAO = B.ID");
                                            _queryEnderecoCliente.AppendLine("LEFT JOIN TB_ENDERECO_LOGRADOURO C ON B.ID_ENDERECO_LOGRADOURO = C.ID");
                                            _queryEnderecoCliente.AppendLine("LEFT JOIN TB_TIPO_IMOVEL D ON B.ID_TIPO_IMOVEL = D.ID");
                                            _queryEnderecoCliente.AppendLine("LEFT JOIN TB_PAIS E ON(B.ID_PAIS = E.ID)");
                                            _queryEnderecoCliente.AppendLine("LEFT JOIN TB_UF F ON(C.ID_UF = F.ID)");
                                            _queryEnderecoCliente.AppendLine("LEFT JOIN TB_CIDADE G ON(C.ID_CIDADE = G.ID)");
                                            _queryEnderecoCliente.AppendLine("LEFT JOIN TB_BAIRRO H ON(C.ID_BAIRRO = H.ID)");
                                            _queryEnderecoCliente.AppendLine("WHERE A.ID = @ID");
                                            _parametros = new Dictionary<string, object>() { { "@ID", _IdClientePlanoProduto } };

                                            using (var readerEndTmp = await CommandHelper.ExecuteReaderFirst(sessionNh.Connection.CreateCommand(), _queryEnderecoCliente, _parametros, sessionNh.Connection, null, 10, 0))
                                            {
                                                while (await readerEndTmp.ReadAsync())
                                                {
                                                    _obj.Cep_Antigo = Convert.ToString(readerEndTmp["ENDERECO_CEP"]);
                                                    _obj.Logradouro_Antigo = Convert.ToString(readerEndTmp["ENDERECO_LOGRADOURO"]);
                                                    _obj.Numero_Antigo = Convert.ToString(readerEndTmp["ENDERECO_NUMERO"]);
                                                    _obj.Uf_Antigo = Convert.ToString(readerEndTmp["ENDERECO_UF"]);
                                                    _obj.Cidade_Antigo = Convert.ToString(readerEndTmp["ENDERECO_CIDADE"]);
                                                    _obj.Bairro_Antigo = Convert.ToString(readerEndTmp["ENDERECO_BAIRRO"]);
                                                    _obj.Pais_Antigo = Convert.ToString(readerEndTmp["ENDERECO_PAIS"]);
                                                    _obj.Complemento_Antigo = Convert.ToString(readerEndTmp["ENDERECO_COMPLEMENTO"]);
                                                    _obj.Cod_Tipo_Imovel_Antigo = Convert.ToInt32(readerEndTmp["COD_TIPO_IMOVEL"]) > 0 ? Convert.ToInt32(readerEndTmp["COD_TIPO_IMOVEL"]) : 1;
                                                    _obj.Tipo_Imovel_Antigo = Convert.ToString(readerEndTmp["COD_TIPO_IMOVEL"]);
                                                    _obj.Latitude_Str_Antigo = Convert.ToString(readerEndTmp["ENDERECO_LATITUDE"]).Replace(',', '.');
                                                    _obj.Longitude_Str_Antigo = Convert.ToString(readerEndTmp["ENDERECO_LONGITUDE"]).Replace(',', '.');
                                                    _obj.Ponto_Referencia_Antigo = Convert.ToString(readerEndTmp["ENDERECO_PONTO_REFERENCIA"]);
                                                    _obj.Cod_Area_Antigo = Convert.ToString(readerEndTmp["COD_AREA"]);
                                                    _obj.Cod_Escritorio_Antigo = Convert.ToString(readerEndTmp["COD_ESCRITORIO"]);
                                                    _obj.Cod_Localidade_Antigo = Convert.ToString(readerEndTmp["COD_LOCALIDADE"]);
                                                    _obj.Bloco_Antigo = Convert.ToString(readerEndTmp["BLOCO"]);
                                                    _obj.Apartamento_Antigo = Convert.ToString(readerEndTmp["APARTAMENTO"]);
                                                    _obj.IdCondominioPredioEndereco_Antigo = Convert.ToInt64(readerEndTmp["ID_CONDOMINIO_PREDIO"]);
                                                    _obj.BuscarEnderecoPorId_Antigo = Convert.ToInt32(readerEndTmp["BUSCA_ENDERECO"]);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            var id_endereco_cliente = reader["ID_ENDERECO_CLIENTE_ANTIGO"] != DBNull.Value ? Convert.ToInt64(reader["ID_ENDERECO_CLIENTE_ANTIGO"]) : 0;
                                            if (id_endereco_cliente > 0)
                                            {
                                                var obj_endereco_cliente = await repoEndereco_Cliente.selectFirst(x => x.Id == id_endereco_cliente);

                                                _obj.Cep_Antigo = obj_endereco_cliente?.Cep;
                                                _obj.Logradouro_Antigo = obj_endereco_cliente?.Logradouro;
                                                _obj.Numero_Antigo = obj_endereco_cliente?.Numero;
                                                _obj.Uf_Antigo = obj_endereco_cliente?.UF;
                                                _obj.Cidade_Antigo = obj_endereco_cliente?.Cidade;
                                                _obj.Bairro_Antigo = obj_endereco_cliente?.Bairro;
                                                _obj.Pais_Antigo = obj_endereco_cliente?.Pais?.Nome;
                                                _obj.Complemento_Antigo = obj_endereco_cliente?.Complemento;
                                                _obj.CodCt_Antigo = Convert.ToString(reader["CODCT"]);
                                                _obj.Projeto_Antigo = Convert.ToString(reader["PROJETO"]);
                                                _obj.DescricaoContrato_Antigo = Convert.ToString(reader["DESCRICAO_CONTRATO"]);

                                                _obj.Tipo_Imovel_Antigo = Convert.ToString(obj_endereco_cliente?.Tipo_Imovel?.Cod_Tipo_Imovel);
                                                _obj.Cod_Tipo_Imovel_Antigo = obj_endereco_cliente?.Tipo_Imovel?.Cod_Tipo_Imovel;
                                                _obj.Latitude_Str_Antigo = Convert.ToString(obj_endereco_cliente?.Latitude);
                                                _obj.Longitude_Str_Antigo = Convert.ToString(obj_endereco_cliente?.Longitude);
                                                _obj.Ponto_Referencia_Antigo = obj_endereco_cliente?.Ponto_Referencia;
                                                _obj.Cod_Area_Antigo = obj_endereco_cliente?.Cod_Area;
                                                _obj.Cod_Escritorio_Antigo = obj_endereco_cliente?.Cod_Escritorio;
                                                _obj.Cod_Localidade_Antigo = Convert.ToString(obj_endereco_cliente?.Cod_Localidade);
                                                _obj.Bloco_Antigo = Convert.ToString(obj_endereco_cliente?.Bloco);
                                                _obj.Apartamento_Antigo = Convert.ToString(obj_endereco_cliente?.Apartamento);
                                                _obj.IdCondominioPredioEndereco_Antigo = obj_endereco_cliente?.Condominio_Predio?.Id;
                                                _obj.BuscarEnderecoPorId_Antigo = obj_endereco_cliente?.Busca_Endereco != null ? Convert.ToInt32(obj_endereco_cliente?.Busca_Endereco) : (int?)null;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    _obj.Cep = Convert.ToString(reader["CEP"]);
                                    _obj.Logradouro = Convert.ToString(reader["ENDERECO"]);
                                    _obj.Numero = Convert.ToString(reader["ENDERECO_NUMERO"]);
                                    _obj.Uf = Convert.ToString(reader["ENDERECO_UF"]);
                                    _obj.Cidade = Convert.ToString(reader["CIDADE"]);
                                    _obj.Bairro = Convert.ToString(reader["ENDERECO_BAIRRO"]);
                                    _obj.Pais = Convert.ToString(reader["ENDERECO_PAIS"]);
                                    _obj.Complemento = Convert.ToString(reader["COMPLEMENTO"]);
                                    _obj.Cod_Tipo_Imovel = reader["COD_TIPO_IMOVEL"] != DBNull.Value ? Convert.ToInt32(reader["COD_TIPO_IMOVEL"]) : (Int32?)null;
                                    _obj.Tipo_Imovel = Convert.ToString(reader["COD_TIPO_IMOVEL"]);
                                    _obj.Latitude_Str = Convert.ToString(reader["LATITUDE_STR"]).Replace(',', '.');
                                    _obj.Longitude_Str = Convert.ToString(reader["LONGITUDE_STR"]).Replace(',', '.');
                                    _obj.Ponto_Referencia = Convert.ToString(reader["OBSERVACAO"]);
                                    _obj.Grupo_Area = Convert.ToString(reader["GRUPO_AREA"]);
                                    _obj.Cod_Area = Convert.ToString(reader["COD_AREA"]);
                                    _obj.Localidade = Convert.ToString(reader["LOCALIDADE"]);
                                    _obj.Area = Convert.ToString(reader["AREA"]);
                                    _obj.Cod_Escritorio = Convert.ToString(reader["COD_ESCRITORIO"]);
                                    _obj.Cod_Localidade = Convert.ToString(reader["COD_LOCALIDADE"]);
                                    _obj.CodCt = Convert.ToString(reader["CODCT"]);
                                    _obj.Projeto = Convert.ToString(reader["PROJETO"]);
                                    _obj.Bloco = reader["BLOCO"] != DBNull.Value ? Convert.ToString(reader["BLOCO"]) : String.Empty;
                                    _obj.Apartamento = reader["APARTAMENTO"] != DBNull.Value ? Convert.ToString(reader["APARTAMENTO"]) : String.Empty;
                                    _obj.UtilizaEndeCondo = reader["UTILIZA_END_COND"] != DBNull.Value ? Convert.ToBoolean(reader["UTILIZA_END_COND"]) : false;
                                    _obj.IdCondominioPredioEndereco = reader["END_CONDPRED_SELECIONADO"] != DBNull.Value ? Convert.ToInt64(reader["END_CONDPRED_SELECIONADO"]) : (Int64?)null;
                                    _obj.BuscarEnderecoPorId = reader["BUSCAR_ENDPOR_ID"] != DBNull.Value ? Convert.ToInt32(reader["BUSCAR_ENDPOR_ID"]) : 0;
                                    _obj.DescricaoContrato = Convert.ToString(reader["DESCRICAO_CONTRATO"]);
                                }

                                #endregion

                                _obj.Segmento = Convert.ToString(reader["SEGMENTO"]);
                                _obj.Obs_Criacao_Servico = Convert.ToString(reader["OBSERVACAO_TESTE"]);
                                _obj.Natureza = Convert.ToString(reader["NATUREZA"]);


                                _obj.Num_Doc = Convert.ToString(reader["NUM_DOC"]);
                                _obj.Num_Obra = Convert.ToString(reader["NUM_OBRA"]);
                                _obj.Num_Obra_Original = Convert.ToString(reader["NUM_OBRA_ORIGINAL"]);

                                _obj.Status_Execucao = reader["STATUS_EXECUCAO"] != DBNull.Value ?
                                                (StatusExecucaoAniel?)Enum.Parse(typeof(StatusExecucaoAniel), Convert.ToString(reader["STATUS_EXECUCAO"])) : null;

                                _obj.Status_Auditoria = reader["STATUS_AUDITORIA"] != DBNull.Value ?
                                                (Status_AuditoriaAniel?)Enum.Parse(typeof(Status_AuditoriaAniel), Convert.ToString(reader["STATUS_AUDITORIA"])) : null;

                                _obj.ResponsabilidadeOS = reader["RESPONSABILIDADE"] != DBNull.Value ?
                                                (ResponsabilidadeOS?)Enum.Parse(typeof(ResponsabilidadeOS), Convert.ToString(reader["RESPONSABILIDADE"])) : null;


                                _obj.Status_OS = reader["STATUS_EXECUCAO"] != DBNull.Value ?
                                                HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(_obj.Status_Execucao.Value) : String.Empty;

                                var _temp_agendamento = (reader["DATAHORA_MAXIMA"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_MAXIMA"]).ToString("dd/MM/yyyy") : "Sem Agendamento");

                                _obj.Data = reader["DATA_MAXIMA"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_MAXIMA"]).ToString("dd/MM/yyyy") : "Sem Agendamento";

                                _obj.Hora = reader["HORA_MAXIMA"] != DBNull.Value ? Convert.ToDateTime(reader["HORA_MAXIMA"].ToString()).ToString("HH:mm") : String.Empty;

                                _obj.Desc_Turno = Convert.ToString(reader["DESC_TURNO"]);
                                _obj.Id_Turno = reader["COD_TURNO_SERV"] != DBNull.Value ? Convert.ToInt32(reader["COD_TURNO_SERV"]) : (int?)null;

                                var depto = Convert.ToString(reader["COD_DEPTO"]);

                                _obj.Departamento = Convert.ToString(reader["DEPARTAMENTO"]);
                                _obj.IdDepartamento = depto;
                                _obj.DescricaoContrato = Convert.ToString(reader["DESCRICAO_CONTRATO"]);


                                if (reader["DATAHORA_INDICADOR"] != DBNull.Value)
                                {
                                    DateTime? DataHora_Indicador = reader["DATAHORA_INDICADOR"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_INDICADOR"]) : (DateTime?)null;
                                    DateTime? DataHora_Criacao = reader["DATAHORA_CADASTRO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORA_CADASTRO"]) : (DateTime?)null;
                                    DateTime? DataHora_Prod = null;

                                    if (reader["DATAHORA_PROD"] != DBNull.Value)
                                        DataHora_Prod = Convert.ToDateTime(reader["DATAHORA_PROD"]);

                                    _obj.Hora_Indicador = DataHora_Indicador.Value.ToString("HH:mm");
                                    _obj.Data_Indicador = DataHora_Indicador.Value.ToString("dd/MM/yyyy");

                                    _obj.SLA_Atual = $"{DataHora_Indicador.Value.ToString("dd/MM/yyyy HH:mm")}  ({new Documento_ProducaoApiController().GetDescricaoSla(DataHora_Indicador, DataHora_Criacao, DataHora_Prod)})";
                                }
                                else
                                    _obj.SLA_Atual = "Sem Indicador de Sla";

                                _obj.Adcional_1 = Convert.ToString(reader["ADICIONAL_1"]);
                                _obj.Adcional_2 = Convert.ToString(reader["ADICIONAL_2"]);
                                _obj.Adcional_3 = Convert.ToString(reader["ADICIONAL_3"]);
                                _obj.Adcional_4 = Convert.ToString(reader["ADICIONAL_4"]);
                                _obj.Adcional_5 = Convert.ToString(reader["ADICIONAL_5"]);
                                _obj.Adcional_6 = Convert.ToString(reader["ADICIONAL_6"]);
                                _obj.Adcional_7 = Convert.ToString(reader["ADICIONAL_7"]);
                                _obj.Adcional_8 = Convert.ToString(reader["ADICIONAL_8"]);
                                _obj.Adcional_9 = Convert.ToString(reader["ADICIONAL_9"]);
                                _obj.Adcional_10 = Convert.ToString(reader["ADICIONAL_10"]);
                                _obj.NomeInstalacao = Convert.ToString(reader["NOME_INSTALACAO"]);
                                _obj.CpfInstalacao = Convert.ToString(reader["CPF_INSTALACAO"]);
                                _obj.TelefoneInstalacao = Convert.ToString(reader["TELEFONE_INSTALACAO"]);
                                _obj.Numero_Atendimento = atendimento?.Protocolo;
                                _obj.Exige_Pagamento_Instalacao = Convert.ToString(reader["EXIGE_PAGAMENTO_INSTALACAO"]);
                                _obj.Tipo_Prioridade = reader["TIPO_PRIORIDADE"] != DBNull.Value ?
                                                            (Tipo_PrioridadeT5)Enum.Parse(typeof(Tipo_PrioridadeT5), Convert.ToString(reader["TIPO_PRIORIDADE"])) : (Tipo_PrioridadeT5?)null;
                                //_obj.Exige_Pagamento_Instalacao = (string)(reader["EXIGE_PAGAMENTO_INSTALACAO"] != DBNull.Value ? Enum.Parse(typeof(Padrao), Convert.ToString(reader["EXIGE_PAGAMENTO_INSTALACAO"])) : null);
                                _obj.Tipo = reader["TIPO"] != DBNull.Value ? Convert.ToString(reader["TIPO"]) : "C";
                                _obj.Id_Pop = reader["ID_POP"] != DBNull.Value ? Convert.ToInt64(reader["ID_POP"]) : (Int64?)null;
                                _obj.Id_Olt = reader["ID_OLT"] != DBNull.Value ? Convert.ToInt64(reader["ID_OLT"]) : (Int64?)null;
                                _obj.Id_Elemento_Rede = reader["ID_ELEMENTO_REDE"] != DBNull.Value ? Convert.ToInt64(reader["ID_ELEMENTO_REDE"]) : (Int64?)null;
                                _obj.CodCt = Convert.ToString(reader["CODCT"]);
                                _obj.Projeto = Convert.ToString(reader["PROJETO"]);
                                _obj.Bloco = reader["BLOCO"] != DBNull.Value ? Convert.ToString(reader["BLOCO"]) : String.Empty;
                                _obj.Apartamento = reader["APARTAMENTO"] != DBNull.Value ? Convert.ToString(reader["APARTAMENTO"]) : String.Empty;
                                _obj.UtilizaEndeCondo = reader["UTILIZA_END_COND"] != DBNull.Value ? Convert.ToBoolean(reader["UTILIZA_END_COND"]) : false;
                                _obj.IdCondominioPredioEndereco = reader["END_CONDPRED_SELECIONADO"] != DBNull.Value ? Convert.ToInt64(reader["END_CONDPRED_SELECIONADO"]) : (Int64?)null;
                                _obj.BuscarEnderecoPorId = reader["BUSCAR_ENDPOR_ID"] != DBNull.Value ? Convert.ToInt32(reader["BUSCAR_ENDPOR_ID"]) : 0;
                            }
                            var planoCliente = _obj.Id_PlanoProduto != null ? repoClientePlano.selectFirst(p => p.Id == _obj.Id_PlanoProduto).Result : null;
                            _obj.Plano_Produto = planoCliente?.Plano?.Descricao;
                            _obj.Nome_Tecnologia = planoCliente?.Plano?.Tecnologia?.Descricao;
                            if (_IdAtendimento > 0)
                            {
                                var objAtendimento = await repoAtendimento.selectFirst(x => x.Id == _IdAtendimento);
                                _obj.Nome_Tecnologia = objAtendimento?.PlanoProduto?.Plano?.Id > 0 ? repoPlano.selectFirst(x => x.Id == objAtendimento.PlanoProduto.Plano.Id).Result?.Tecnologia?.Descricao : string.Empty;
                            }
                            else if (_IdClientePlanoProduto > 0)
                            {
                                var objClientePlanoProduto = await repoCliente_PlanoProduto.selectFirst(x => x.Id == _IdClientePlanoProduto);
                                _obj.Nome_Tecnologia = objClientePlanoProduto?.Plano?.Id > 0 ? repoPlano.selectFirst(x => x.Id == objClientePlanoProduto.Plano.Id).Result?.Tecnologia?.Descricao : string.Empty;
                            }

                            _obj.Endereco_Mudanca = (await repoAtendimento.selectFirst(x => x.Id == _IdAtendimento && x.Endereco_Mudanca != null)) != null ||
                                (await repoCliente_PlanoProduto.selectFirst(x => x.Id == _IdClientePlanoProduto && x.Endereco_Mudanca_Cliente != null)) != null;

                            var objPlanoCliente = _obj.Id_PlanoProduto != null ? repoPlano.selectFirst(p => p.Id == _obj.Id_PlanoProduto).Result?.Tecnologia?.Descricao : null;
                            _obj.Nome_Tecnologia = objPlanoCliente;
                        }
                    }

                    retorno.Objeto = _obj;
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

        public async Task<bool> GetDesbloqueioDetalhes(String Codct, String Projeto, String NumObra, int? NumDoc)
        {
            var retorno = false;

            using (ISession session = HibernateUtil.GetSessionT5())
            using (var repository = new Conexao<Documento_ProducaoAniel>(session))
            {
                var num_obra_original = getOsReaberta(NumObra, Codct, Projeto, NumDoc).Result;

                var obj = await repository.selectFirst(p => p.CodCt == Codct && p.Projeto == Projeto && p.Num_Obra == num_obra_original && p.Num_Doc == NumDoc);
                List<StatusExecucaoAniel> valoresBloqueioDetalhes = new List<StatusExecucaoAniel>
                                            {
                                                StatusExecucaoAniel.Fechada_Improdutiva,
                                                StatusExecucaoAniel.Fechada_Produtiva,
                                                StatusExecucaoAniel.Cancelado,
                                                StatusExecucaoAniel.Atendimento_Finalizado
                                            };


                retorno = obj?.Status != null ? valoresBloqueioDetalhes.Contains(obj.Status.Value) :
                                       false;
            }

            return retorno;
        }

        [HttpPost]
        [Route("api/Acompanhamento_ServicoApi/GetChecklist")]
        public async Task<DataTableResponse<ChecklistListaModel>> GetCheckList(CheckListRequest request)
        {
            List<ChecklistListaModel> retorno = new List<ChecklistListaModel>();

            var acompanhamento_api = new Acompanhamento_ServicoApiController();
            var num_obra = acompanhamento_api.getOsReaberta(request.NumObra, request.Codct, request.Projeto, Convert.ToInt32(request.NumDoc)).Result;
            var num_obra_original = num_obra.Split('/')[0];

            bool validaRDO = false;
            string tarefa = null;

            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
            {
                var repository = new DocumentoProducaoAnielRepository(sessionT5Nh, sessionT5);
                var docBusiness = new DocumentoProducaoAnielBusiness(repository);
                List<Documento_ProducaoAniel> listaos = repository.GetAll().
                           Where(a => a.CodCt == request.Codct &&
                                      a.Projeto == request.Projeto &&
                                      a.Numero_Obra_Original == num_obra_original &&
                                      a.Num_Doc == Convert.ToInt32(request.NumDoc)).ToList();

                validaRDO = docBusiness.ValidaTipoDocumentoRDO(listaos.FirstOrDefault());
                if (validaRDO)
                {
                    num_obra = repository.GetManutencao_OT_OR(request.Codct, request.Projeto, num_obra, Convert.ToInt32(request.NumDoc));

                    tarefa = listaos.FirstOrDefault().Tarefa;
                }

                GetApiRetorno<List<QuizAnswer>> lista = new GetApiRetorno<List<QuizAnswer>>();

                foreach (var os in listaos)
                {
                    if (validaRDO)
                        lista = await new QuizApiController().GetQuizAnswer(request.Codct, request.Projeto, os.Num_Obra, tarefa, Convert.ToInt32(request.NumDoc));
                    else
                        lista = await new QuizApiController().GetQuizAnswer(request.Codct, request.Projeto, os.Num_Obra, String.Empty, Convert.ToInt32(request.NumDoc));


                    foreach (var item in lista.Objeto)
                    {
                        ChecklistListaModel novo = new ChecklistListaModel();
                        var criacao = item.diaInicio;

                        if (criacao.HasValue && item.horaInicio.HasValue)
                            criacao = criacao.Value.Add(item.horaInicio.Value.TimeOfDay);

                        novo.Id = item.idA.ToString();
                        novo.Titulo = item.titulo;
                        novo.Tipo = item.tituloQuiz;
                        novo.Perguntas = item.respostas.Count.ToString();
                        novo.Respondidas = item.respostas.Where(x => x.resposta != null || x.multi.Count > 0).Count().ToString();
                        novo.UsuarioCriacao = item.usuario;
                        novo.UsuarioUltimaAlteracao = "";
                        novo.DataHoraCriacao = criacao.HasValue ? criacao?.ToString("dd/MM/yyyy HH:mm") : String.Empty;
                        novo.DataHoraUltimaAlteracao = "";

                        retorno.Add(novo);
                    }
                }

            }




            return new DataTableResponse<ChecklistListaModel> { data = retorno.OrderBy(x => x.Titulo), recordsFiltered = retorno.Count };

        }

        [HttpPost]
        [Route("api/Acompanhamento_ServicoApi/ExcluirChecklist")]
        public async Task<PostApiRetorno> ExcluirChecklist(int id, string numDoc)
        {
            Dictionary<String, Object> parametros = new Dictionary<string, object>();
            string idCapa = "";

            try
            {
                if (!string.IsNullOrEmpty(numDoc))
                {
                    using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        StringBuilder sqlBusca = new StringBuilder();
                        parametros.Add("@id", id);
                        parametros.Add("@numdoc", numDoc);

                        #region buscaIdResposta

                        sqlBusca.AppendLine(" SELECT ID_ANSWER_CAPA FROM TB_QUIZ_ANSWER_DOCPROD WHERE ID = @id");

                        using (var reader = await CommandHelper.ExecuteReader(com, sqlBusca, parametros, sessionT5.Connection, null))
                        {
                            if (await reader.ReadAsync())
                            {
                                idCapa = reader["ID_ANSWER_CAPA"].ToString();
                            }
                        }

                        parametros.Add("@idRes", idCapa);

                        #endregion


                        StringBuilder sqlDeleteProd = new StringBuilder();

                        sqlDeleteProd.AppendLine(" DELETE FROM TB_QUIZ_ANSWER_DOCPROD WHERE ID = @id ");

                        await CommandHelper.ExecuteNonQuery(com, sqlDeleteProd, parametros, sessionT5.Connection, null);

                        if (!string.IsNullOrEmpty(idCapa))
                        {
                            StringBuilder sqlDeleteCapa = new StringBuilder();

                            sqlDeleteCapa.AppendLine(" DELETE FROM TB_QUIZ_ANSWER_CAPA WHERE ID = @idRes ");

                            await CommandHelper.ExecuteNonQuery(com, sqlDeleteCapa, parametros, sessionT5.Connection, null);
                        }
                    }
                }

                return new PostApiRetorno { Ok = true };
            }
            catch (Exception e)
            {
                return new PostApiRetorno { Ok = false, Mensagem = e.Message, InnerException = e.InnerException.Message };
            }
        }

        [HttpGet()]
        [Route("api/Acompanhamento_ServicoApi/GetTecnico")]
        public async Task<GetApiRetorno<TecnicoCadastroModel>> GetTecnico(String CodCt, String Projeto, String Num_Obra, Int32? Num_Doc)
        {
            GetApiRetorno<TecnicoCadastroModel> retorno = new GetApiRetorno<TecnicoCadastroModel>();
            TecnicoCadastroModel _obj = new TecnicoCadastroModel();

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                IDocumentoProducaoAnielRepository repoDocProd = new DocumentoProducaoAnielRepository(sessionT5, sessionT5);
                var documentoBusiness = new DocumentoProducaoAnielBusiness(repoDocProd);
                string numObraOriginal = null;
                Boolean Exibe_RDOs;

                using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
                {
                    var config = (await repoConfig.selectFirst());
                    Exibe_RDOs = config != null ? config.ExibeRDOs : false;
                }
                Dictionary<String, Object> parametros = new Dictionary<string, object>();

                try
                {
                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();

                        // P/ Fazer:
                        // Qualidade do Serviço

                        sql.AppendLine("SELECT D.PROJETO,D.NUM_DOC,D.CODCT, D.ID_VENDA, D.COD_AREA,D.COD_LOCALIDADE,D.COD_ESCRITORIO,D.COD_TIPO_SERV, D.NUM_OBRA,D.NUM_OBRA_ORIGINAL, D.STATUS_EXECUCAO,")
                            .AppendLine("      A.NUM_FRE, F.NUM_FRE_AUX,F.NUM_FRE AS NUM_FRE_FUNC, F.NOME, F.TEL_CELULAR, F.TELEFONE, F.EMAIL, B.DESCRICAO AS CENTRO_CUSTO,D.STATUS_COBRANCA, ");
                        sql.AppendLine("       D.MODULO_BAIXA,C.NOME AS SUPERVISOR, E.STATUS_EXECUCAO, D.HORA_PROD + D.DATA_PROD AS DATAHORA_PROD, D.COD_ENCERRAMENTO,D.RESPONSABILIDADE, ");
                        sql.AppendLine("       E.DESCRICAO AS DESC_ENCERRAMENTO, D.RX, D.TX, G.QUALIDADE, D.DATA_MAXIMA, D.HORA_MAXIMA, "); //F.FOTO
                        //--- Devido ao uso da api de Criação de OS pela Desktop, foi sugerido passar também o campo relacionado a caixa_ftth e porta_ftth da tb_documento_producao
                        sql.AppendLine("       Z.ID, Coalesce(Z.DESCRICAO, D.CAIXA_FTTH) AS DESCRICAO_ELEMENTO, Z.CONTRATO, Z.PROJETO, Y.ID, Y.ID_CAIXA, Coalesce(Y.PORTA, D.PORTA_FTTH) AS PORTA, H.LOCALIDADE ");
                        sql.AppendLine("FROM TB_DOCUMENTO_PRODUCAO D ");
                        sql.AppendLine("LEFT JOIN TB_EQUIPE A ");
                        sql.AppendLine("  ON(D.EQUIPE = A.EQUIPE) ");
                        sql.AppendLine("LEFT JOIN TB_FUNCIONARIO F ");
                        sql.AppendLine("  ON(F.NUM_FRE = A.NUM_FRE) ");
                        sql.AppendLine("LEFT JOIN TB_CENTRO_CUSTO B ");
                        sql.AppendLine("  ON(F.COD_CCUSTO = B.COD_CCUSTO) ");
                        sql.AppendLine("LEFT JOIN TB_FUNCIONARIO C ");
                        sql.AppendLine("  ON(A.COD_SUPERV = C.NUM_FRE) ");
                        sql.AppendLine("LEFT JOIN TB_CODIGO_ENCERRAMENTO E ");
                        sql.AppendLine("  ON(E.CODCT = D.CODCT AND E.COD_ENCERRAMENTO = D.COD_ENCERRAMENTO) ");
                        sql.AppendLine("LEFT JOIN TB_ELEMENTO_REDE Z ");
                        sql.AppendLine("ON(CAST(Z.ID AS VARCHAR(255)) = CAST(D.CAIXA_FTTH AS VARCHAR(255)))");
                        sql.AppendLine("LEFT JOIN TB_ELEMENTO_REDE_PORTA Y ");
                        sql.AppendLine("ON(CAST(Y.ID_CAIXA AS VARCHAR(255)) = CAST(Z.ID AS VARCHAR(255)) AND CAST(D.PORTA_FTTH AS VARCHAR(255)) = CAST(Y.PORTA AS VARCHAR(255)))");
                        sql.AppendLine("LEFT JOIN TB_CLASSIFICACAO_QUALIDADE G ");
                        sql.AppendLine("  ON(G.ID = D.COD_QUALIDADE) ");
                        sql.AppendLine("LEFT JOIN TB_LOCALIDADE H ");
                        sql.AppendLine("  ON(H.COD_ESCRITORIO = D.COD_ESCRITORIO AND H.COD_AREA = D.COD_AREA AND H.COD_LOCALIDADE = D.COD_LOCALIDADE) ");
                        sql.AppendLine("WHERE D.CODCT = @CODCT ");
                        sql.AppendLine("  AND D.PROJETO = @PROJETO ");
                        sql.AppendLine("  AND D.NUM_OBRA = @NUM_OBRA ");
                        sql.AppendLine("  AND D.NUM_DOC = @NUM_DOC ");

                        if (!Exibe_RDOs)
                            sql.AppendLine(" AND COALESCE(D.SIGLA_MODULO, '') <> 'MCO' ");

                        parametros.Add("@CODCT", CodCt);
                        parametros.Add("@PROJETO", Projeto);
                        parametros.Add("@NUM_OBRA", Num_Obra);
                        parametros.Add("@NUM_DOC", Num_Doc);

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, sessionT5.Connection, null, 10, 0))
                        {
                            if (await reader.ReadAsync())
                            {
                                numObraOriginal = Convert.ToString(reader["NUM_OBRA_ORIGINAL"]);
                                String matricula = Convert.ToString(reader["NUM_FRE"]);
                                String nome = Convert.ToString(reader["NOME"]);

                                String cod_encerramento = Convert.ToString(reader["COD_ENCERRAMENTO"]);
                                String desc_encerramento = Convert.ToString(reader["DESC_ENCERRAMENTO"]);
                                _obj.Matricula = Convert.ToString(reader["NUM_FRE_FUNC"]);
                                _obj.Matricula_Id = matricula;
                                _obj.Matricula_Nome = String.Concat(matricula, " - ", nome);
                                _obj.Cod_Area = Convert.ToString(reader["COD_AREA"]);
                                _obj.Cod_Escritorio = Convert.ToString(reader["COD_ESCRITORIO"]);
                                _obj.Cod_Localidade = Convert.ToString(reader["COD_LOCALIDADE"]);
                                _obj.Cod_Tipo_Serv = Convert.ToString(reader["COD_TIPO_SERV"]);


                                _obj.Id = Convert.ToString(reader["NUM_OBRA"]);
                                _obj.Projeto = Convert.ToString(reader["PROJETO"]);
                                _obj.Num_Doc = Convert.ToInt32(reader["NUM_DOC"]);
                                _obj.CodCt = Convert.ToString(reader["CODCT"]);


                                if (reader["TEL_CELULAR"] != DBNull.Value && !String.IsNullOrEmpty(Convert.ToString(reader["TEL_CELULAR"])))
                                {
                                    _obj.Telefone = Convert.ToString(reader["TEL_CELULAR"]);
                                }
                                else if (reader["TELEFONE"] != DBNull.Value && !String.IsNullOrEmpty(Convert.ToString(reader["TELEFONE"])))
                                {
                                    _obj.Telefone = Convert.ToString(reader["TELEFONE"]);
                                }
                                else
                                {
                                    _obj.Telefone = "-";
                                }

                                _obj.Email = Convert.ToString(reader["EMAIL"]);

                                _obj.Matricula_Aux = Convert.ToString(reader["NUM_FRE_AUX"]);
                                _obj.LocalidadeStr = Convert.ToString(reader["LOCALIDADE"]);
                                _obj.Centro_Custo = Convert.ToString(reader["CENTRO_CUSTO"]);
                                _obj.Supervisor = Convert.ToString(reader["SUPERVISOR"]);

                                _obj.Qualidade_Servico = Convert.ToString(reader["QUALIDADE"]);
                                _obj.Tipo_Baixa = Convert.ToString(reader["MODULO_BAIXA"]);
                                //if (reader["DATA_PROD"] != DBNull.Value)
                                //{
                                //    _obj.Data_Hora_Encerramento = Convert.ToDateTime(reader["DATA_PROD"]).ToString("dd/MM/yyyy");
                                //    if (reader["HORA_PROD"] != DBNull.Value)
                                //        _obj.Data_Hora_Encerramento += ' ' + ((TimeSpan)(reader["HORA_PROD"])).ToString(@"hh\:mm"); ;
                                //}

                                if (reader["DATAHORA_PROD"] != DBNull.Value)
                                {
                                    _obj.Data_Hora_Encerramento = Convert.ToDateTime(reader["DATAHORA_PROD"]).ToString("dd/MM/yyyy HH:mm");
                                }


                                _obj.Cod_Desc_Encerramento = String.Concat(cod_encerramento, " - ", desc_encerramento);
                                _obj.PortaFTTH = Convert.ToString(reader["PORTA"]);
                                _obj.CaixaFTTH = Convert.ToString(reader["DESCRICAO_ELEMENTO"]);

                                _obj.Sinal_RX = String.Concat(Convert.ToString(reader["RX"]), "%");
                                _obj.Sinal_TX = String.Concat(Convert.ToString(reader["TX"]), "%");

                                _obj.Custo_Servico = (await GetCustoServico(_obj.CodCt, _obj.Projeto, _obj.Id, _obj.Num_Doc)).Objeto;
                                _obj.Id_Atendimento = 0;
                                _obj.Status_Execucao = reader["STATUS_EXECUCAO"] != DBNull.Value ? (StatusExecucaoAniel?)Enum.Parse(typeof(StatusExecucaoAniel), Convert.ToString(reader["STATUS_EXECUCAO"])) : null;
                                _obj.Status_Cobranca = reader["STATUS_COBRANCA"] != DBNull.Value ? (Status_CobrancaAniel?)Enum.Parse(typeof(Status_CobrancaAniel), Convert.ToString(reader["STATUS_COBRANCA"])) : null;
                                //_obj.Foto = reader["FOTO"] != DBNull.Value ? Convert.ToBase64String((byte[])reader["FOTO"]) : String.Empty;
                                _obj.ResponsabilidadeOS = reader["RESPONSABILIDADE"] != DBNull.Value ? (ResponsabilidadeOS?)Enum.Parse(typeof(ResponsabilidadeOS), Convert.ToString(reader["RESPONSABILIDADE"])) : null;
                                _obj.Data = reader["DATA_MAXIMA"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_MAXIMA"]).ToString("dd/MM/yyyy") : String.Empty;
                                _obj.Hora = reader["HORA_MAXIMA"] != DBNull.Value ? ((TimeSpan)(reader["HORA_MAXIMA"])).ToString(@"hh\:mm") : String.Empty;

                                _obj.CaixaFTTH = Convert.ToString(reader["DESCRICAO_ELEMENTO"]);
                                _obj.PortaFTTH = Convert.ToString(reader["PORTA"]);

                            }
                        }
                        retorno.Objeto = _obj;
                        retorno.Ok = true;
                        string ultimaObservacao = documentoBusiness.ListarObservacoesBaixa(CodCt, Projeto, Num_Doc.Value, numObraOriginal).
                                                                     FindAll(p => string.IsNullOrEmpty(p) == false).
                                                                     LastOrDefault();

                        _obj.Observacao_Tecnico = ultimaObservacao;
                    }

                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;
        }

        [HttpGet()]
        [Route("api/Acompanhamento_ServicoApi/GetCustoServico")]
        public async Task<GetApiRetorno<String>> GetCustoServico(String CodCt, String Projeto, String Num_Obra, Int32 Num_Doc)
        {


            GetApiRetorno<String> retorno = new GetApiRetorno<String>();

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var material_api = new Material_TarefaApiController())
            {
                Boolean Exibe_RDOs = (await new Conexao<Configuracoes_Gerais>(session).selectFirst()).ExibeRDOs;

                Dictionary<String, Object> parametros = new Dictionary<string, object>();
                String Num_ObraTmp = Num_Obra.Split('\\')[0];
                Num_ObraTmp = Num_Obra.Split('/')[0];

                String SIGLA_MODULO = await material_api.GetSiglaModulo(sessionT5, Num_ObraTmp, Num_Doc, Projeto, CodCt);
                String CHAVE = material_api.GetCampoChave(SIGLA_MODULO);
                parametros.Add("@CODCT", CodCt);
                parametros.Add("@PROJETO", Projeto);
                parametros.Add("@NUM_DOC", Num_Doc);

                if (SIGLA_MODULO == "MCO")
                    parametros.Add("@CHAVE", String.Format("{0}", Num_ObraTmp));
                else
                    parametros.Add("@CHAVE", String.Format("{0}%", Num_ObraTmp));

                try
                {
                    using (var com = sessionT5.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();



                        sql.AppendLine(" SELECT SUM(TOTAL) AS TOTAL FROM ")
                           .AppendLine(" ( ")
                           .AppendLine("     SELECT COALESCE(SUM(A.QUANT_PROD * B.QUANTIDADE * C.RVALOR_FAT), 0) AS TOTAL ")

                           .AppendLine("     FROM TB_CUSTOS_TAREFA A ")

                           .AppendLine("     LEFT JOIN TB_CADERNO_ENCARGOS B ")
                           .AppendLine("     ON(A.UNIDADE_CUSTO = B.UNIDADE_CUSTO AND B.CODIGO = A.CODIGO) ")
                           .AppendLine("     LEFT JOIN SP_BUSCAR_VALOR_SERVICO(A.CODCT, A.PROJETO, NULL, A.UNIDADE_CUSTO, A.COD_PRECO) C ")
                           .AppendLine("     ON(C.RCLASSE_MO = B.CLASSE_MO) ")
                           .AppendLine("     INNER JOIN TB_DOCUMENTO_PRODUCAO D ")
                           .AppendLine($"     ON(A.CODCT = D.CODCT AND A.PROJETO = D.PROJETO AND A.{CHAVE} = D.NUM_OBRA AND A.NUM_DOC = D.NUM_DOC) ") // COALESCE(D.SIGLA_MODULO, '') <> 'MCO'

                           .AppendLine("     WHERE A.CODCT = @CODCT ") //
                           .AppendLine("         AND A.PROJETO = @PROJETO ")
                           .AppendLine($"        AND A.{CHAVE} LIKE @CHAVE ")
                           .AppendLine("         AND A.NUM_DOC = @NUM_DOC ");


                        if (!Exibe_RDOs)
                            sql.AppendLine(" AND COALESCE(D.SIGLA_MODULO, '') <> 'MCO' ");

                        sql.AppendLine("     UNION ALL")


                       .AppendLine("     SELECT COALESCE((A.QUANTIDADE * B.VALOR), 0) AS TOTAL ")

                       .AppendLine("     FROM TB_MATERIAL_TAREFA A ")

                       .AppendLine("     INNER JOIN TB_MATERIAL B ")
                       .AppendLine("     ON(A.CODMAT = B.CODMAT) ")
                       .AppendLine("     INNER JOIN TB_DOCUMENTO_PRODUCAO C ")
                       .AppendLine($"     ON(A.CODCT = C.CODCT AND A.PROJETO = C.PROJETO AND A.{CHAVE} = C.NUM_OBRA AND A.NUM_DOC = C.NUM_DOC) ")

                       .AppendLine("     WHERE   A.CODCT = @CODCT ") //
                       .AppendLine("         AND A.PROJETO = @PROJETO ")
                       .AppendLine($"        AND A.{CHAVE} LIKE @CHAVE ")
                       .AppendLine("         AND A.NUM_DOC = @NUM_DOC ");

                        if (!Exibe_RDOs)
                            sql.AppendLine(" AND COALESCE(C.SIGLA_MODULO, '') <> 'MCO' ");

                        sql.AppendLine(" ) ");



                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, sessionT5.Connection, null, 10, 0))
                        {
                            if (await reader.ReadAsync())
                            {
                                try
                                {
                                    retorno.Objeto = Convert.ToDouble(reader["TOTAL"]).ToString("N2");
                                }
                                catch
                                {
                                    retorno.Objeto = "0.00";
                                }
                            }
                        }
                        retorno.Ok = true;
                    }
                }
                catch (Exception ex)
                {
                    retorno.Mensagem = ex.Message;
                    if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
                }
            }
            return retorno;
        }

        [HttpGet()]
        [Route("api/Acompanhamento_Servicoapi/FotoAjax")]
        public async Task<GetApiRetorno<String>> FotoAjax(String Num_fre)
        {
            GetApiRetorno<String> retorno = new GetApiRetorno<String>();
            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    using (var com = session.Connection.CreateCommand())
                    {
                        //String _ids = $"({String.Join(",", value.ids)})";

                        StringBuilder sql = new StringBuilder();

                        sql.AppendLine($"SELECT FOTO FROM TB_FUNCIONARIO WHERE NUM_FRE=@NUM_FRE");
                        parametros.Add("@NUM_FRE", Num_fre);

                        using (var reader = await CommandHelper.ExecuteReaderFirst(com, sql, parametros, session.Connection, null, 10, 0))
                        {
                            if (await reader.ReadAsync())
                                retorno.Objeto = reader["FOTO"] != DBNull.Value ? Convert.ToBase64String((byte[])reader["FOTO"]) : String.Empty;
                        }

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
        [Route("api/Acompanhamento_ServicoApi/GravarTecnico")]
        public async Task<PostApiRetorno<String>> GravarTecnico(TecnicoCadastroModel value, Boolean? tem_acesso139)
        {
            PostApiRetorno<String> retorno = new PostApiRetorno<String>();

            Documento_ProducaoAniel documento_producao_old = new Documento_ProducaoAniel();
            EquipeAniel equipe_old = new EquipeAniel();
            StatusExecucaoAniel? status_old;

            try
            {
                Documento_ProducaoAniel modelo = null;

                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    var repositoryAgendamento = new Conexao<Agendamento_EquipeAniel>(session);
                    var agendamento = repositoryAgendamento.selectFirst(p => p.CodCt == value.CodCt &&
                                                                        p.Projeto == value.Projeto &&
                                                                        p.Num_Doc == value.Num_Doc &&
                                                                        p.Num_Obra == value.Id).Result;

                    using (var repository = new Conexao<Documento_ProducaoAniel>(session))
                    {
                        if (!String.IsNullOrEmpty(value.Id))
                        {
                            modelo = (await repository.select(p => p.CodCt == value.CodCt && p.Projeto == value.Projeto &&
                                                                   p.Num_Doc == value.Num_Doc && p.Num_Obra == value.Id)).FirstOrDefault();

                            documento_producao_old = modelo;
                            equipe_old = documento_producao_old.Equipe != null ? await new Conexao<EquipeAniel>(session).selectFirst(p => p.Matricula == documento_producao_old.Equipe) : null;
                        }


                        if (modelo == null)
                        {
                            modelo = new Documento_ProducaoAniel
                            {

                            };

                            documento_producao_old = await repository.selectFirst(p => p.CodCt == value.CodCt &&
                                                                                p.Projeto == value.Projeto &&
                                                                                p.Num_Obra == value.Id &&
                                                                                p.Num_Doc == value.Num_Doc);

                            if (String.IsNullOrEmpty(value.Matricula_Id) == false && modelo.Data_Maxima.HasValue == false)
                            {
                                throw new Exception($"Não foi possível atribuir técnico devido ao fato do Documento estar sem Data de Agendamento.");
                            }

                            equipe_old = await new Conexao<EquipeAniel>(session).selectFirst(p => p.Matricula == documento_producao_old.Equipe);
                            status_old = documento_producao_old.Status;
                        }
                        else
                        {

                            if (String.IsNullOrEmpty(value.Matricula_Id) == false && modelo.Data_Maxima.HasValue == false)
                            {
                                throw new Exception($"Não foi possível alterar técnico devido ao fato do Documento estar sem Data de Agendamento.");
                            }

                            if (documento_producao_old.Equipe != value.Matricula_Id)
                            {
                                if (!new List<StatusExecucaoAniel> { StatusExecucaoAniel.Aberta_Aguard_Agendamento,
                                StatusExecucaoAniel.Aberta_Aguard_Atendimento,
                                StatusExecucaoAniel.Atendimento_Paralizado,
                                StatusExecucaoAniel.Aberta_Aguard_Responsavel}.Contains(documento_producao_old.Status.Value))
                                {
                                    throw new Exception($"Não foi possível alterar equipe devido ao fato do Documento ter sido alterado para o status {HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(documento_producao_old.Status.Value)}.");
                                }

                                if (documento_producao_old.Responsabilidade == ResponsabilidadeOS.TECNICO)
                                {
                                    throw new Exception($"Não foi possível alterar equipe devido ao fato do Documento estar na responsabilidade do técnico.");
                                }

                            }

                            status_old = documento_producao_old.Status;

                            if (!String.IsNullOrEmpty(value.Matricula_Id))
                            {

                                if (agendamento == null)
                                {
                                    modelo.Equipe = value.Matricula_Id;
                                }
                                else
                                {
                                    if (!tem_acesso139.Value)
                                    {
                                        var _temHorarioDisponivel = new Conexao<Agendamento_EquipeAniel>(session).selectFirst(p => p.Equipe.Matricula == value.Matricula_Id &&
                                                                                                                              ((agendamento.DataHora_Inicio >= p.DataHora_Inicio &&
                                                                                                                              agendamento.DataHora_Inicio <= p.DataHora_Fim) ||
                                                                                                                              (agendamento.DataHora_Fim <= p.DataHora_Inicio &&
                                                                                                                              agendamento.DataHora_Fim >= p.DataHora_Fim))
                                                                                                                              ).Result == null;
                                        if (_temHorarioDisponivel)
                                        {
                                            modelo.Equipe = value.Matricula_Id;
                                            agendamento.Equipe = new Conexao<EquipeAniel>(session).selectFirst(p => p.Matricula == value.Matricula_Id).Result;
                                        }
                                        else
                                            throw new Exception("Técnico nao possui disponibilidade no horário agendado");
                                    }

                                    else
                                    {
                                        modelo.Equipe = value.Matricula_Id;
                                        agendamento.Equipe = new Conexao<EquipeAniel>(session).selectFirst(p => p.Matricula == value.Matricula_Id).Result;
                                    }
                                }
                            }
                            else
                            {
                                modelo.Equipe = null;
                                if (agendamento != null)
                                    agendamento.Equipe = null;
                            }
                        }

                        // Envia mensagem para o mobile com alteração da OS
                        if (!String.IsNullOrEmpty(equipe_old?.Device_Id))
                        {
                            if (equipe_old?.Matricula != modelo.Equipe)
                                MobileService.NotificarSmartphone(equipe_old.Device_Id, String.Empty, "Houve mudanças em sua rotina.", "Atualize a sua lista de atividades", "br.com.sinapse.touch.MSG_RESINC_OS", value.CodCt, value.Projeto, value.Num_Doc.ToString(), value.Id);

                            if (modelo.Equipe != null)
                            {
                                var equipeAnterior = await new Conexao<EquipeAniel>(session).selectFirst(p => p.Matricula == modelo.Equipe);
                                if (!String.IsNullOrEmpty(equipeAnterior.Device_Id))
                                {
                                    MobileService.NotificarSmartphone(equipeAnterior.Device_Id, String.Empty, "Houve mudanças em sua rotina.", "Atualize a sua lista de atividades", "br.com.sinapse.touch.MSG_RESINC_OS", value.CodCt, value.Projeto, value.Num_Doc.ToString(), value.Id);
                                }
                            }

                        }

                        else
                        {
                            var equipeAnterior = await new Conexao<EquipeAniel>(session).selectFirst(p => p.Matricula == modelo.Equipe);
                            if (!String.IsNullOrEmpty(equipeAnterior?.Device_Id))
                            {
                                MobileService.NotificarSmartphone(equipeAnterior.Device_Id, String.Empty, "Houve mudanças em sua rotina.", "Atualize a sua lista de atividades", "br.com.sinapse.touch.MSG_RESINC_OS", value.CodCt, value.Projeto, value.Num_Doc.ToString(), value.Id);
                            }
                        }

                        var matricula = equipe_old != null ? equipe_old.Matricula : null;
                        var nome = equipe_old != null ? equipe_old.Nome : null;
                        var nome_new = modelo.Equipe != null ? await new Conexao<EquipeAniel>(session).selectFirst(p => p.Matricula == modelo.Equipe) : null;

                        if (modelo?.Equipe != equipe_old?.Matricula)
                            await new Documento_ProducaoApiController().InsertLog_Lanc_Servico(session, modelo.CodCt, modelo.Projeto, modelo.Num_Obra, modelo.Num_Doc,
                                    modelo.Equipe == null ? "Remoção de Equipe" : "Equipe", $"{matricula} - {nome}",
                                    value.Usuario, null, null, $"{modelo.Equipe} - {nome_new?.Nome}", null);


                        using (ITransaction transaction = session.BeginTransaction())
                        {
                            if (modelo.Status == StatusExecucaoAniel.Aberta_Aguard_Responsavel ||
                                modelo.Status == StatusExecucaoAniel.Aberta_Aguard_Atendimento ||
                                modelo.Status == StatusExecucaoAniel.Aberta_Aguard_Agendamento ||
                                modelo.Status == StatusExecucaoAniel.Atendimento_Paralizado)
                            {
                                if (modelo.Equipe != null && modelo.Data_Maxima.HasValue)
                                    modelo.Status = StatusExecucaoAniel.Aberta_Aguard_Atendimento;
                                else if (modelo.Equipe == null && modelo.Data_Maxima.HasValue)
                                    modelo.Status = StatusExecucaoAniel.Aberta_Aguard_Responsavel;
                                else if (modelo.Equipe == null && modelo.Data_Maxima.HasValue == false)
                                    modelo.Status = StatusExecucaoAniel.Aberta_Aguard_Agendamento;
                            }


                            await repository.SaveOrUpdate(modelo);

                            if (agendamento != null)
                                await repositoryAgendamento.SaveOrUpdate(agendamento);

                            transaction.Commit();
                        }

                        if (modelo.Status != status_old)
                        {

                            await new Documento_ProducaoApiController().InsertLog_Lanc_Servico(session,
                                                                                    modelo.CodCt,
                                                                                    modelo.Projeto,
                                                                                    modelo.Num_Obra,
                                                                                    modelo.Num_Doc,
                                                                                    "Mudança de Status",
                                                                                    HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(status_old.Value),
                                                                                    value.Usuario,
                                                                                    null,
                                                                                    null,
                                                                                    HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(modelo.Status.Value), null);
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


        [HttpPost()]
        [Route("api/Acompanhamento_ServicoApi/GravarDadosGerais")]
        public async Task<PostApiRetorno<String>> GravarDadosGerais(DadosGerais_CadastroModel value)
        {
            PostApiRetorno<String> retorno = new PostApiRetorno<String>();
            try
            {
                Documento_ProducaoAniel modelo = null;
                var coordenadasGeoParametros = new Dictionary<String, object>();

                using (ISession session = HibernateUtil.GetSessionT5())
                using (ISession sessionConnect = HibernateUtil.GetSession())
                {
                    using (var repositoryConfig = new Conexao<Configuracoes_Gerais>(sessionConnect))
                    using (var repositoryClienteContrato = new Conexao<ClienteContrato>(sessionConnect))
                    using (var repoTipoImovelConnect = new Conexao<Tipo_Imovel>(sessionConnect))
                    using (var repositoryHorario = new Conexao<Horario_Connect>(session))
                    using (var repositoryAgendamento = new Conexao<Agendamento_EquipeAniel>(session))
                    using (var repoTipoServico = new Conexao<Tipo_Servico_EquipeAniel>(session))
                    using (var repoTipoImovel = new Conexao<Tipo_ImovelAniel>(session))
                    using (var repoEquipe = new Conexao<EquipeAniel>(session))
                    using (var documentoApi = new Documento_ProducaoApiController())
                    using (var agendamentoEquipeApi = new Agendamento_EquipeApiController(1))
                    using (var repoSubTipoServico = new Conexao<Sub_Tipo_ServicoAniel>(session))
                    using (var repository = new Conexao<Documento_ProducaoAniel>(session))
                    using (var repositoryEscritorioAniel = new Conexao<EscritorioAniel>(session))
                    using (var repositoryAreaAniel = new Conexao<AreaAniel>(session))
                    using (var repositoryLocalidadeAniel = new Conexao<LocalidadeAniel>(session))
                    using (var repositoryTurno = new Conexao<Turno_ServicoAniel>(session))
                    using (var repositoryLogLanServico = new Conexao<Log_Lanc_Servico>(session))
                    {
                        IDocumentoProducaoAnielRepository repoDocProd = new DocumentoProducaoAnielRepository(session, session);
                        var documentoBusiness = new DocumentoProducaoAnielBusiness(repoDocProd);
                        //if (!String.IsNullOrEmpty(value.Id))
                        modelo = (await repository.select(p => p.CodCt == value.CodCt && p.Projeto == value.Projeto &&
                                                               p.Num_Doc == Convert.ToInt32(value.Num_Doc) && p.Num_Obra == value.Num_Obra)).FirstOrDefault();
                        Configuracoes_Gerais config = await repositoryConfig.selectFirst();
                        var data_old = modelo.Data_Maxima;
                        var hora_old = modelo.Hora_Maxima;
                        var lat_old = modelo.Latitude_Str;
                        var long_old = modelo.Longitude_Str;
                        var tipoPrioridade_old = modelo.Tipo_Prioridade;
                        var horaIndicador_old = modelo.Hora_Indicador;
                        var dataIndicador_old = modelo.Data_Indicador;
                        var prioridade_cliente = modelo.Tipo_Prioridade == null ? null : modelo.Tipo_Prioridade;
                        var status_old = modelo.Status;
                        string CodEscritorioOld = modelo.Cod_Escritorio == String.Empty ? null : modelo.Cod_Escritorio;
                        string CodAreaOld = modelo.Cod_Area == String.Empty ? null : modelo.Cod_Area;
                        int? CodLocalidadeOld = modelo.Cod_Localidade == 0 ? null : modelo.Cod_Localidade;
                        var tipo_servico_old = await repoTipoServico.selectFirst(p => p.Cod_Tipo_Serv == modelo.Cod_Tipo_Serv);
                        var tipo_servico = await repoTipoServico.selectFirst(p => p.Cod_Tipo_Serv == Convert.ToInt16(value.Tipo_Servico));
                        var tipo_imovel_old = modelo.Cod_Tipo_Imovel;

                        var detalhesOs_old = modelo.Observacao_Aux;
                        if (detalhesOs_old?.Length > 900) throw new Exception("A descrição dos detalhes da OS não pode exceder 900 caracteres");

                        DateTime minimoFireBird = new DateTime(1753, 01, 01);

                        //-------------         Parametros geograficos lat/log          -------------//
                        coordenadasGeoParametros.Add("LatitudeOld", modelo.Latitude_Str);
                        coordenadasGeoParametros.Add("LongitudeOld", modelo.Longitude_Str);


                        var sub_tipo_servico_old = await repoSubTipoServico.selectFirst(p => p.Codigo_Sub_Tipo == modelo.Cod_Sub_Tipo_Serv && p.Codigo_Tipo == modelo.Cod_Tipo_Serv);
                        var sub_tipo_servico = await repoSubTipoServico.selectFirst(p => p.Codigo_Sub_Tipo == value.Subtipo_Servico && p.Codigo_Tipo == Convert.ToInt16(value.Tipo_Servico));

                        if (modelo == null)
                        {
                            modelo = new Documento_ProducaoAniel
                            {

                            };
                        }
                        else
                        {
                            DateTime temp;
                            if (DateTime.TryParse(value.Data, out temp))
                            {
                                //Convert.ToDateTime(String.Format(value.Data, "dd/MM/yyyy") + ' ' + String.Format(value.Hora, "hh:mm"));
                                modelo.Data_Maxima = Convert.ToDateTime(String.Format(value.Data, "dd/MM/yyyy"));

                                if (!String.IsNullOrEmpty(value.Hora))
                                    modelo.Hora_Maxima = Convert.ToDateTime(String.Format(value.Hora, "hh:mm"));
                                else
                                    modelo.Hora_Maxima = null;
                            }
                            else if (value.Data == null)
                            {
                                modelo.Data_Maxima = null;
                                modelo.Hora_Maxima = null;
                            }

                            modelo.Hora_Indicador = string.IsNullOrEmpty(value.Hora_Indicador) == false ? Convert.ToDateTime(String.Format(value.Hora_Indicador, "dd/MM/yyyy")) : (DateTime?)null;
                            modelo.Data_Indicador = string.IsNullOrEmpty(value.Data_Indicador) == false ? Convert.ToDateTime(String.Format(value.Data_Indicador, "hh:mm")) : (DateTime?)null;

                            string hora_max = modelo.Hora_Maxima.HasValue ? modelo.Hora_Maxima.Value.ToString("HH:mm:ss") : null;
                            string hora_ant = hora_old.HasValue ? hora_old.Value.ToString("HH:mm:ss") : null;

                            //if (modelo.Data_Maxima != data_old || modelo.Hora_Maxima != hora_old)
                            if (modelo.Data_Maxima != data_old || hora_max != hora_ant)
                            {
                                if (modelo.Status.Value == StatusExecucaoAniel.Fechada_Produtiva ||
                                    modelo.Status.Value == StatusExecucaoAniel.Fechada_Improdutiva ||
                                    modelo.Status.Value == StatusExecucaoAniel.Atendimento_Finalizado)
                                {
                                    throw new Exception($"Não será possível alterar o agendamento do serviço, pois o mesmo se encontra fechado.");
                                }
                            }


                            if (modelo.Data_Maxima != null)
                            {
                                if (modelo.Status == StatusExecucaoAniel.Aberta_Aguard_Agendamento
                                    || modelo.Status == StatusExecucaoAniel.Aberta_Aguard_Responsavel
                                    || modelo.Status == StatusExecucaoAniel.Aberta_Aguard_Atendimento
                                    || modelo.Status == StatusExecucaoAniel.Atendimento_Paralizado)
                                    modelo.Status = modelo.Equipe != null ? StatusExecucaoAniel.Aberta_Aguard_Atendimento : StatusExecucaoAniel.Aberta_Aguard_Responsavel;

                                if (modelo.Data_Maxima != data_old || hora_old?.TimeOfDay != modelo?.Hora_Maxima?.TimeOfDay)
                                    modelo.Usuario_Agendamento = User.Identity.GetUserId();
                            }

                            modelo.Cod_Tipo_Serv = Convert.ToInt16(value.Tipo_Servico);
                            modelo.Cod_Sub_Tipo_Serv = value.Subtipo_Servico;
                            modelo.Segmento = value.Segmento;
                            modelo.Origem_Documento = value.Origem_Ordem_Servico;
                            modelo.Observacao_Teste = value.Obs_Criacao_Servico;
                            modelo.NomeInstalacao = value.NomeInstalacao;
                            modelo.CpfInstalacao = StringService.Truncate(value.CpfInstalacao, 50);
                            modelo.TelefoneInstalacao = StringService.Truncate(value.TelefoneInstalacao, 50);
                            modelo.Cod_Escritorio = value.Cod_Escritorio;
                            modelo.Cod_Area = value.Cod_Area;
                            modelo.Cod_Localidade = Convert.ToInt32(value.Cod_Localidade);
                            modelo.Tipo_Prioridade = value.Tipo_Prioridade;
                            modelo.Cod_Tipo_Imovel = Convert.ToInt32(value.Tipo_Imovel);
                            modelo.Observacao_Aux = StringService.Truncate(value.Observacao_Aux, 900);

                            //removido logica para apenas M para todos
                            modelo.Tipo = value.Tipo;
                            modelo.Cep = value.Cep;
                            modelo.Logradouro = StringService.Truncate(value.Logradouro, 100);
                            modelo.Bairro = value.Bairro;

                            if (int.TryParse(value.Cidade, out int result))
                            {
                                var buscaCidade = await new Conexao<Cidade>(sessionConnect).selectFirst(p => p.Id == Convert.ToDouble(value.Cidade));
                                modelo.Cidade = StringService.Truncate(buscaCidade?.Descricao, 60);
                            }
                            else
                            {
                                modelo.Cidade = StringService.Truncate(value.Cidade, 60);
                            }

                            if (int.TryParse(value.Uf, out int result1))
                            {
                                var buscaUf = await new Conexao<Uf>(sessionConnect).selectFirst(p => p.Id == Convert.ToDouble(value.Uf));
                                modelo.UF = buscaUf?.Sigla;
                            }
                            else
                            {
                                modelo.UF = value.Uf;
                            }

                            modelo.Pais = value.Pais;
                            modelo.Complemento = StringService.Truncate(value.Complemento, 100);
                            modelo.Numero = value.Numero;
                            modelo.Observacao = StringService.Truncate(value.Ponto_Referencia, 100);
                            modelo.Id_Pop = value.Id_Pop;
                            modelo.Id_Olt = value.Id_Olt;
                            modelo.Id_Elemento_Rede = value.Id_Elemento_Rede;


                            modelo.CodDepto = !String.IsNullOrEmpty(value.IdDepartamento) ? Convert.ToInt64(value.IdDepartamento) : (long?)null;
                            modelo.Longitude_Str = value.Longitude_Str;
                            modelo.Latitude_Str = value.Latitude_Str;
                            modelo.Bloco = value.Bloco;
                            modelo.Apartamento = value.Apartamento;
                            modelo.BuscarEnderecoPorId = value.BuscarEnderecoPorId;

                            if (!config.Utiliza_Cadastro_Endereco)
                                modelo.UtilizaEndCond = value.UtilizaEndeCondo;

                            modelo.IdCondominioPredioEndereco = value.IdCondominioPredioEndereco;

                            //var clienteContrato = await repositoryClienteContrato.selectFirst(p => p.Id == modelo.Id_Cliente_ContratoConnect);


                            //if (value.Tipo_Imovel != null)
                            //{
                            //    Tipo_Imovel imovel = await repoTipoImovelConnect.selectFirst(p => p.Cod_Tipo_Imovel == Convert.ToInt32(value.Tipo_Imovel));
                            //    clienteContrato.Endereco_Cliente.Tipo_Imovel = imovel;
                            //}
                            //modelo.Hora_Indicador = value.HoraIndicador;
                            //modelo.Data_Indicador = value.DataIndicador;

                            int? codNatureza = null;

                            if (modelo.Cod_Tipo_Serv.HasValue)
                                codNatureza = (await repoTipoServico.selectFirst(p => p.Cod_Tipo_Serv == modelo.Cod_Tipo_Serv.Value)).Cod_Natureza;


                            var tempoExecucao = (TimeSpan?)null;
                            tempoExecucao = documentoBusiness.GetTempoExecucao(config, modelo.Cod_Tipo_Imovel, modelo.Cod_Tipo_Serv);

                            if (modelo.Data_Maxima.HasValue &&
                                modelo.CodTurnoServ != value.Id_Turno &&
                                config.Considerar_Agendamento_Periodo)
                            {
                                if (modelo.Status.Value == StatusExecucaoAniel.Fechada_Produtiva ||
                                    modelo.Status.Value == StatusExecucaoAniel.Fechada_Improdutiva ||
                                    modelo.Status.Value == StatusExecucaoAniel.Atendimento_Finalizado)
                                {
                                    throw new Exception($"Não será possível alterar o agendamento do serviço, pois o mesmo se encontra fechado.");
                                }

                                LocalidadeAniel localidadeAniel = await repositoryLocalidadeAniel.selectFirst(p => p.Cod_Localidade == modelo.Cod_Localidade && p.Cod_Area == modelo.Cod_Area && p.Cod_Escritorio == modelo.Cod_Escritorio);
                                Turno_ServicoAniel turno = await repositoryTurno.selectFirst(p => p.Id == value.Id_Turno);
                                var resultadoAgendamento = await agendamentoEquipeApi.GetDatasDisponiveis(modelo.Cod_Tipo_Serv, codNatureza, modelo.Cod_Escritorio, modelo.Cod_Area, modelo.Cod_Localidade, localidadeAniel?.LocalidadeStr, tempoExecucao.Value.TotalMinutes, modelo.Data_Maxima);
                                if (resultadoAgendamento.Ok)
                                {
                                    var turnosPossiveis = resultadoAgendamento.Objeto.FindAll(p => p.Data == modelo.Data_Maxima.Value.ToString("dd/MM/yyyy")).FirstOrDefault()?.HorasTurno;

                                    var agendamentoTurno = turnosPossiveis != null ? turnosPossiveis.FirstOrDefault(p => p.Display == turno.Descricao) : null;
                                    if (agendamentoTurno != null)
                                    {
                                        modelo.Hora_Maxima = agendamentoTurno.Hora_Inicio;
                                    }
                                    else
                                    {
                                        modelo.Hora_Maxima = turno.Hora_Ini;
                                    }
                                }
                                else
                                {
                                    modelo.Hora_Maxima = turno.Hora_Ini;
                                }

                                modelo.CodTurnoServ = value.Id_Turno;

                            }

                            if (modelo.Data_Maxima.HasValue && modelo.Hora_Maxima.HasValue)
                            {
                                var dataHoraInicio = (modelo?.Data_Maxima.Value.Add(modelo.Hora_Maxima.Value.TimeOfDay)).Value;
                                modelo.CodTurnoServ = PeriodoApiController.BuscarTurno(session, modelo.Hora_Maxima.Value, tempoExecucao)?.Id;

                            }
                            else
                            {
                                modelo.CodTurnoServ = null;
                            }

                            using (var documento_api = new Documento_ProducaoApiController())
                            {
                                string observacaoAuxComparacao = modelo.Observacao_Aux != null ? modelo.Observacao_Aux.Replace("\r\n", "").Replace("\n", "") : null;
                                string detalhesOsOldComparacao = detalhesOs_old != null ? detalhesOs_old.Replace("\r\n", "").Replace("\n", "") : null;

                                coordenadasGeoParametros.Add("Latitude", modelo.Latitude_Str);
                                coordenadasGeoParametros.Add("Longitude", modelo.Longitude_Str);

                                if (coordenadasGeoParametros["Latitude"] != coordenadasGeoParametros["LatitudeOld"] || coordenadasGeoParametros["Longitude"] != coordenadasGeoParametros["LongitudeOld"] || !String.IsNullOrEmpty(value.ErroApiMaps))
                                {

                                    ValidaAlteracaoDocumento(modelo, "LatitudeLongitude");
                                    await documento_api.InsertLog_Lanc_Servico(session,
                                                                                modelo.CodCt,
                                                                                modelo.Projeto,
                                                                                modelo.Num_Obra,
                                                                                modelo.Num_Doc,
                                                                                !String.IsNullOrEmpty(value.ErroApiMaps) ? StringService.Truncate($"Falha no preenchimento do endereço da OS devido a erro do serviço do google. {value.ErroApiMaps}", 200) : "Alteração das coordenadas de Latitude/Longitude",
                                                                                $"{coordenadasGeoParametros["LatitudeOld"]} - {coordenadasGeoParametros["LongitudeOld"]}",
                                                                                value.Usuario,
                                                                                null,
                                                                                null,
                                                                                $"{coordenadasGeoParametros["Latitude"]} - {coordenadasGeoParametros["Longitude"]}",
                                                                                null);
                                }

                                if (observacaoAuxComparacao != detalhesOsOldComparacao)
                                {
                                    ValidaAlteracaoDocumento(modelo);
                                    await documento_api.InsertLog_Lanc_Servico(session,
                                                                                modelo.CodCt,
                                                                                modelo.Projeto,
                                                                                modelo.Num_Obra,
                                                                                modelo.Num_Doc,
                                                                                "Alteração na Observação/Detalhes da OS",
                                                                                null,
                                                                                value.Usuario,
                                                                                null,
                                                                                null,
                                                                                null,
                                                                                detalhesOs_old);
                                }
                                if (modelo.Cod_Tipo_Imovel != tipo_imovel_old)
                                {
                                    Tipo_ImovelAniel imovelNovo = await repoTipoImovel.selectFirst(p => p.Cod_Tipo_Imovel == Convert.ToInt32(value.Tipo_Imovel));
                                    Tipo_ImovelAniel imovelAntigo = await repoTipoImovel.selectFirst(p => p.Cod_Tipo_Imovel == Convert.ToInt32(tipo_imovel_old));

                                    ValidaAlteracaoDocumento(modelo);
                                    await documento_api.InsertLog_Lanc_Servico(session,
                                                                                modelo.CodCt,
                                                                                modelo.Projeto,
                                                                                modelo.Num_Obra,
                                                                                modelo.Num_Doc,
                                                                                "Mudança de Tipo Imóvel",
                                                                                Convert.ToString(imovelAntigo?.Descricao),
                                                                                value.Usuario,
                                                                                null,
                                                                                null,
                                                                                Convert.ToString(imovelNovo?.Descricao), null);
                                }
                                if (modelo.Data_Indicador != dataIndicador_old)
                                {

                                    var Data_Indicador = Convert.ToDateTime(String.Format(value.Data_Indicador, "dd/MM/yyyy"));
                                    modelo.AlterarIndicador = true;

                                    ValidaAlteracaoDocumento(modelo);
                                    await documento_api.InsertLog_Lanc_Servico(session,
                                                                                modelo.CodCt,
                                                                                modelo.Projeto,
                                                                                modelo.Num_Obra,
                                                                                modelo.Num_Doc,
                                                                                "Mudança de Data Indicador",
                                                                                dataIndicador_old?.Date == minimoFireBird.Date ? " - " : Convert.ToString(dataIndicador_old),
                                                                                value.Usuario,
                                                                                null,
                                                                                null,
                                                                                dataIndicador_old?.Date == minimoFireBird.Date ? " - " : Convert.ToString(Data_Indicador), null);
                                }

                                var novaHoraIndicador = modelo.Hora_Indicador.HasValue ? modelo.Hora_Indicador.Value.ToString("HH:mm") : null;
                                var oldHoraIndicador = horaIndicador_old.HasValue ? horaIndicador_old.Value.ToString("HH:mm") : null;
                                if (novaHoraIndicador != oldHoraIndicador)
                                {
                                    modelo.AlterarIndicador = true;

                                    ValidaAlteracaoDocumento(modelo);
                                    await documento_api.InsertLog_Lanc_Servico(session,
                                                                                modelo.CodCt,
                                                                                modelo.Projeto,
                                                                                modelo.Num_Obra,
                                                                                modelo.Num_Doc,
                                                                                "Mudança de Hora Indicador",
                                                                                dataIndicador_old?.Date == minimoFireBird.Date &&
                                                                                horaIndicador_old?.TimeOfDay == minimoFireBird.TimeOfDay ? " - " : String.Concat(modelo.Data_Indicador?.ToString("dd/MM/yyyy"), " ", oldHoraIndicador),
                                                                                value.Usuario,
                                                                                null,
                                                                                null,
                                                                                Convert.ToString(String.Concat(modelo.Data_Indicador?.ToString("dd/MM/yyyy"), " ", novaHoraIndicador)), null);
                                }

                                if (status_old != modelo.Status)
                                {
                                    ValidaAlteracaoDocumento(modelo);
                                    await documento_api.InsertLog_Lanc_Servico(session,
                                                                                modelo.CodCt,
                                                                                modelo.Projeto,
                                                                                modelo.Num_Obra,
                                                                                modelo.Num_Doc,
                                                                                "Mudança de Status",
                                                                                HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(status_old.Value),
                                                                                value.Usuario,
                                                                                null,
                                                                                null,
                                                                                HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(modelo.Status.Value), null);
                                }
                                if (lat_old != modelo?.Latitude_Str || long_old != modelo?.Longitude_Str)
                                {
                                    ValidaAlteracaoDocumento(modelo, "LatitudeLongitude");
                                    List<String> lista_latlng = new List<string>();
                                    lista_latlng.Add(lat_old);
                                    lista_latlng.Add(long_old);

                                    await documento_api.InsertLog_Lanc_Servico(session,
                                                                                modelo.CodCt,
                                                                                modelo.Projeto,
                                                                                modelo.Num_Obra,
                                                                                modelo.Num_Doc,
                                                                                "Latitude/Longitude",
                                                                                String.Join(",", lista_latlng),
                                                                                value.Usuario,
                                                                                null,
                                                                                null,
                                                                                $"{modelo.Latitude_Str},{modelo.Longitude_Str}", null);


                                    if (modelo.Id_Cliente_ContratoConnect != null)
                                    {
                                        var clienteContrato = await repositoryClienteContrato.selectFirst(p => p.Id == modelo.Id_Cliente_ContratoConnect);



                                        if (clienteContrato?.Endereco_Cliente != null)
                                        {

                                            double latitudeDouble;
                                            double longitudeDouble;
                                            if (Double.TryParse(modelo.Latitude_Str, out latitudeDouble))
                                            {
                                                clienteContrato.Endereco_Cliente.Latitude = latitudeDouble;
                                            }
                                            else
                                            {
                                                clienteContrato.Endereco_Cliente.Latitude = null;
                                            }


                                            if (Double.TryParse(modelo.Longitude_Str, out longitudeDouble))
                                            {
                                                clienteContrato.Endereco_Cliente.Longitude = longitudeDouble;
                                            }
                                            else
                                            {
                                                clienteContrato.Endereco_Cliente.Longitude = null;
                                            }


                                            await repositoryClienteContrato.SaveOrUpdate(clienteContrato);
                                        }
                                    }

                                }

                                if ((tipo_servico_old != null) && tipo_servico_old?.Descricao != tipo_servico?.Descricao)
                                {
                                    ValidaAlteracaoDocumento(modelo);
                                    await documento_api.InsertLog_Lanc_Servico(session,
                                                                                modelo.CodCt,
                                                                                modelo.Projeto,
                                                                                modelo.Num_Obra,
                                                                                modelo.Num_Doc,
                                                                                "Tipo de Serviço",
                                                                                tipo_servico_old?.Descricao,
                                                                                value.Usuario,
                                                                                null,
                                                                                null,
                                                                                tipo_servico?.Descricao, null);
                                }


                                if ((sub_tipo_servico != null) && (sub_tipo_servico_old?.Descricao != sub_tipo_servico?.Descricao))
                                {
                                    ValidaAlteracaoDocumento(modelo);
                                    await documento_api.InsertLog_Lanc_Servico(session,
                                                                                modelo.CodCt,
                                                                                modelo.Projeto,
                                                                                modelo.Num_Obra,
                                                                                modelo.Num_Doc,
                                                                                "Subtipo de Serviço",
                                                                                sub_tipo_servico_old?.Descricao,
                                                                                value.Usuario,
                                                                                null,
                                                                                null,
                                                                                sub_tipo_servico?.Descricao, null);
                                }

                                string textoReagendamento = !String.IsNullOrEmpty(value.Codigo_Motivo_Reagendamento) ? "Reagendamento do serviço {0} {1} (Manual)" : "Agendamento do serviço {0} {1} (Manual)";
                                if (data_old != modelo?.Data_Maxima)
                                {
                                    ValidaAlteracaoDocumento(modelo);
                                    String data_old_string = null;
                                    if (data_old != null)
                                    {
                                        if (hora_old != null)
                                        {
                                            data_old_string = String.Concat(data_old?.ToString("dd/MM/yyyy"), " ", hora_old?.ToString(@"HH:mm"));
                                        }
                                        else
                                        {
                                            data_old_string = data_old?.ToString("dd/MM/yyyy");
                                        }

                                    }
                                    String dataHoraMaxima = null;

                                    if (modelo.Data_Maxima != null)
                                    {
                                        if (modelo.Hora_Maxima != null)
                                        {
                                            dataHoraMaxima = modelo?.Data_Maxima.Value.Add(modelo.Hora_Maxima.Value.TimeOfDay).ToString();
                                        }
                                        else
                                        {
                                            dataHoraMaxima = modelo?.Data_Maxima.Value.ToString();
                                        }
                                    }


                                    if (!String.IsNullOrEmpty(value.Codigo_Motivo_Reagendamento))
                                    {
                                        var CodMotivoReagendamento = value.Codigo_Motivo_Reagendamento != "0" ? Convert.ToInt64(value.Codigo_Motivo_Reagendamento) : 0;
                                        var ObsMotivoReagendamento = !String.IsNullOrEmpty(value.Observacao_Motivo_Reagendamento) ? value.Observacao_Motivo_Reagendamento : null;

                                        await documento_api.InsertLog_Lanc_ServicoReagendamento(session,
                                                                                modelo.CodCt,
                                                                                modelo.Projeto,
                                                                                modelo.Num_Obra,
                                                                                modelo.Num_Doc,
                                                                                String.Format(textoReagendamento, value.Data, value.Hora),
                                                                                data_old_string,
                                                                                value.Usuario,
                                                                                null,
                                                                                null,
                                                                                dataHoraMaxima,
                                                                                null,
                                                                                CodMotivoReagendamento,
                                                                                ObsMotivoReagendamento);

                                    }
                                    else
                                    {
                                        await documento_api.InsertLog_Lanc_Servico(session,
                                                                                    modelo.CodCt,
                                                                                    modelo.Projeto,
                                                                                    modelo.Num_Obra,
                                                                                    modelo.Num_Doc,
                                                                                    String.Format(textoReagendamento, value.Data, value.Hora),
                                                                                    data_old_string,
                                                                                    value.Usuario,
                                                                                    null,
                                                                                    null,
                                                                                    dataHoraMaxima,
                                                                                    null);
                                    }
                                }
                                else if (hora_old?.TimeOfDay != modelo?.Hora_Maxima?.TimeOfDay)
                                {
                                    ValidaAlteracaoDocumento(modelo);
                                    await documento_api.InsertLog_Lanc_Servico(session,
                                                                                modelo.CodCt,
                                                                                modelo.Projeto,
                                                                                modelo.Num_Obra,
                                                                                modelo.Num_Doc,
                                                                                String.Format(textoReagendamento, value.Data, value.Hora),
                                                                                String.Concat(data_old?.ToString("dd/MM/yyyy"), " ", hora_old?.ToString(@"HH:mm")),
                                                                                value.Usuario,
                                                                                null,
                                                                                null,
                                                                                modelo.Hora_Maxima.HasValue ? modelo?.Hora_Maxima.ToString() : null, null);

                                    if (!String.IsNullOrEmpty(value.Codigo_Motivo_Reagendamento))
                                    {
                                        var CodMotivoReagendamento = value.Codigo_Motivo_Reagendamento != "0" ? Convert.ToInt64(value.Codigo_Motivo_Reagendamento) : 0;
                                        var ObsMotivoReagendamento = !String.IsNullOrEmpty(value.Observacao_Motivo_Reagendamento) ? value.Observacao_Motivo_Reagendamento : null;
                                        await documento_api.InsertLog_Lanc_ServicoReagendamento(session,
                                                                                modelo.CodCt,
                                                                                modelo.Projeto,
                                                                                modelo.Num_Obra,
                                                                                modelo.Num_Doc,
                                                                                String.Format(textoReagendamento, value.Data, value.Hora),
                                                                                String.Concat(data_old?.ToString("dd/MM/yyyy"), " ", hora_old?.ToString(@"HH:mm")),
                                                                                value.Usuario,
                                                                                null,
                                                                                null,
                                                                                modelo.Hora_Maxima.HasValue ? modelo?.Hora_Maxima.ToString() : null,
                                                                                null,
                                                                                CodMotivoReagendamento,
                                                                                ObsMotivoReagendamento);

                                    }
                                    else
                                    {
                                        await documento_api.InsertLog_Lanc_Servico(session,
                                                                                modelo.CodCt,
                                                                                modelo.Projeto,
                                                                                modelo.Num_Obra,
                                                                                modelo.Num_Doc,
                                                                                String.Format(textoReagendamento, value.Data, value.Hora),
                                                                                String.Concat(data_old?.ToString("dd/MM/yyyy"), " ", hora_old?.ToString(@"HH:mm")),
                                                                                value.Usuario,
                                                                                null,
                                                                                null,
                                                                                modelo.Hora_Maxima.HasValue ? modelo?.Hora_Maxima.ToString() : null, null);

                                    }
                                }
                                if (CodEscritorioOld != modelo.Cod_Escritorio)
                                {
                                    ValidaAlteracaoDocumento(modelo);
                                    EscritorioAniel escritorioAniel = await repositoryEscritorioAniel.selectFirst(p => p.Cod_Escritorio == modelo.Cod_Escritorio);
                                    EscritorioAniel escritorioAnielOld = await repositoryEscritorioAniel.selectFirst(p => p.Cod_Escritorio == CodEscritorioOld);
                                    AreaAniel areaAniel = await repositoryAreaAniel.selectFirst(p => p.Cod_Area == modelo.Cod_Area && p.Cod_Escritorio == modelo.Cod_Escritorio);
                                    AreaAniel areaAnielOld = await repositoryAreaAniel.selectFirst(p => p.Cod_Area == CodAreaOld && p.Cod_Escritorio == CodEscritorioOld);
                                    LocalidadeAniel localidadeAniel = await repositoryLocalidadeAniel.selectFirst(p => p.Cod_Localidade == modelo.Cod_Localidade && p.Cod_Area == modelo.Cod_Area && p.Cod_Escritorio == modelo.Cod_Escritorio);
                                    LocalidadeAniel localidadeAnielOld = await repositoryLocalidadeAniel.selectFirst(p => p.Cod_Localidade == Convert.ToInt32(CodLocalidadeOld) && p.Cod_Area == CodAreaOld && p.Cod_Escritorio == CodEscritorioOld);

                                    if (modelo.Id_Cliente_ContratoConnect != null)
                                    {
                                        var clienteContrato = await repositoryClienteContrato.selectFirst(p => p.Id == modelo.Id_Cliente_ContratoConnect);
                                        if (clienteContrato?.Endereco_Cliente != null)
                                        {
                                            clienteContrato.Endereco_Cliente.Cod_Escritorio = escritorioAniel.Cod_Escritorio;
                                            clienteContrato.Endereco_Cliente.Cod_Area = areaAniel.Cod_Area;
                                            clienteContrato.Endereco_Cliente.Cod_Localidade = localidadeAniel.Cod_Localidade;
                                            clienteContrato.Endereco_Cliente.Localidade = localidadeAniel.LocalidadeStr;
                                            await repositoryClienteContrato.SaveOrUpdate(clienteContrato);
                                        }
                                    }

                                    await documento_api.InsertLog_Lanc_Servico(session,
                                                                                modelo.CodCt,
                                                                                modelo.Projeto,
                                                                                modelo.Num_Obra,
                                                                                modelo.Num_Doc,
                                                                                RsCadastros.GRUPO_AREA,
                                                                                escritorioAnielOld?.Descricao,
                                                                                value.Usuario,
                                                                                null,
                                                                                null,
                                                                                escritorioAniel?.Descricao,
                                                                                null);

                                    await documento_api.InsertLog_Lanc_Servico(session,
                                                                                modelo.CodCt,
                                                                                modelo.Projeto,
                                                                                modelo.Num_Obra,
                                                                                modelo.Num_Doc,
                                                                                RsCadastros.AREA,
                                                                                areaAnielOld?.Descricao,
                                                                                value.Usuario,
                                                                                null,
                                                                                null,
                                                                                areaAniel?.Descricao,
                                                                                null);

                                    await documento_api.InsertLog_Lanc_Servico(session,
                                                                                modelo.CodCt,
                                                                                modelo.Projeto,
                                                                                modelo.Num_Obra,
                                                                                modelo.Num_Doc,
                                                                                RsCadastros.LOCALIDADE,
                                                                                localidadeAnielOld?.LocalidadeStr,
                                                                                value.Usuario,
                                                                                null,
                                                                                null,
                                                                                localidadeAniel?.LocalidadeStr,
                                                                                null);
                                }
                                else if (CodAreaOld != modelo.Cod_Area)
                                {
                                    ValidaAlteracaoDocumento(modelo);
                                    AreaAniel areaAniel = await repositoryAreaAniel.selectFirst(p => p.Cod_Area == modelo.Cod_Area && p.Cod_Escritorio == modelo.Cod_Escritorio);
                                    AreaAniel areaAnielOld = await repositoryAreaAniel.selectFirst(p => p.Cod_Area == CodAreaOld && p.Cod_Escritorio == CodEscritorioOld);
                                    LocalidadeAniel localidadeAniel = await repositoryLocalidadeAniel.selectFirst(p => p.Cod_Localidade == modelo.Cod_Localidade && p.Cod_Area == modelo.Cod_Area && p.Cod_Escritorio == modelo.Cod_Escritorio);
                                    LocalidadeAniel localidadeAnielOld = await repositoryLocalidadeAniel.selectFirst(p => p.Cod_Localidade == Convert.ToInt32(CodLocalidadeOld) && p.Cod_Area == CodAreaOld && p.Cod_Escritorio == CodEscritorioOld);



                                    if (modelo.Id_Cliente_ContratoConnect != null)
                                    {
                                        var clienteContrato = await repositoryClienteContrato.selectFirst(p => p.Id == modelo.Id_Cliente_ContratoConnect);

                                        if (clienteContrato?.Endereco_Cliente != null)
                                        {
                                            clienteContrato.Endereco_Cliente.Cod_Escritorio = areaAniel.Cod_Escritorio;
                                            clienteContrato.Endereco_Cliente.Cod_Area = areaAniel.Cod_Area;
                                            clienteContrato.Endereco_Cliente.Cod_Localidade = localidadeAniel.Cod_Localidade;
                                            clienteContrato.Endereco_Cliente.Localidade = localidadeAniel.LocalidadeStr;

                                            await repositoryClienteContrato.SaveOrUpdate(clienteContrato);
                                        }
                                    }

                                    await documento_api.InsertLog_Lanc_Servico(session,
                                                                                modelo.CodCt,
                                                                                modelo.Projeto,
                                                                                modelo.Num_Obra,
                                                                                modelo.Num_Doc,
                                                                                RsCadastros.AREA,
                                                                                areaAnielOld?.Descricao,
                                                                                value.Usuario,
                                                                                null,
                                                                                null,
                                                                                areaAniel?.Descricao,
                                                                                null);

                                    await documento_api.InsertLog_Lanc_Servico(session,
                                                                                modelo.CodCt,
                                                                                modelo.Projeto,
                                                                                modelo.Num_Obra,
                                                                                modelo.Num_Doc,
                                                                                RsCadastros.LOCALIDADE,
                                                                                localidadeAnielOld?.LocalidadeStr,
                                                                                value.Usuario,
                                                                                null,
                                                                                null,
                                                                                localidadeAniel?.LocalidadeStr,
                                                                                null);
                                }
                                else if (Convert.ToInt32(CodLocalidadeOld) != modelo.Cod_Localidade)
                                {
                                    LocalidadeAniel localidadeAniel = await repositoryLocalidadeAniel.selectFirst(p => p.Cod_Localidade == modelo.Cod_Localidade && p.Cod_Area == modelo.Cod_Area && p.Cod_Escritorio == modelo.Cod_Escritorio);
                                    LocalidadeAniel localidadeAnielOld = await repositoryLocalidadeAniel.selectFirst(p => p.Cod_Localidade == Convert.ToInt32(CodLocalidadeOld) && p.Cod_Area == CodAreaOld && p.Cod_Escritorio == CodEscritorioOld);


                                    if (modelo.Id_Cliente_ContratoConnect != null)
                                    {
                                        var clienteContrato = await repositoryClienteContrato.selectFirst(p => p.Id == modelo.Id_Cliente_ContratoConnect);
                                        if (clienteContrato?.Endereco_Cliente != null)
                                        {
                                            clienteContrato.Endereco_Cliente.Cod_Escritorio = localidadeAniel.Cod_Escritorio;
                                            clienteContrato.Endereco_Cliente.Cod_Area = localidadeAniel.Cod_Area;
                                            clienteContrato.Endereco_Cliente.Cod_Localidade = localidadeAniel.Cod_Localidade;
                                            clienteContrato.Endereco_Cliente.Localidade = localidadeAniel.LocalidadeStr;
                                            await repositoryClienteContrato.SaveOrUpdate(clienteContrato);
                                        }
                                    }

                                    ValidaAlteracaoDocumento(modelo);
                                    await documento_api.InsertLog_Lanc_Servico(session,
                                                                                modelo.CodCt,
                                                                                modelo.Projeto,
                                                                                modelo.Num_Obra,
                                                                                modelo.Num_Doc,
                                                                                RsCadastros.LOCALIDADE,
                                                                                localidadeAnielOld?.LocalidadeStr,
                                                                                value.Usuario,
                                                                                null,
                                                                                null,
                                                                                localidadeAniel?.LocalidadeStr,
                                                                                null);
                                }
                                if (tipoPrioridade_old != modelo?.Tipo_Prioridade)
                                {
                                    ValidaAlteracaoDocumento(modelo);
                                    await documento_api.InsertLog_Lanc_Servico(session,
                                                                                modelo.CodCt,
                                                                                modelo.Projeto,
                                                                                modelo.Num_Obra,
                                                                                modelo.Num_Doc,
                                                                                RsCadastros.TIPO_PRIORIDADE,
                                                                                HtmlEnumExtensions.GetEnumDescription(tipoPrioridade_old),
                                                                                value.Usuario,
                                                                                null,
                                                                                null,
                                                                                HtmlEnumExtensions.GetEnumDescription(modelo?.Tipo_Prioridade),
                                                                                null);
                                }
                            }




                            Agendamento_EquipeAniel agendamento = await repositoryAgendamento.selectFirst(p => p.CodCt == modelo.CodCt && p.Projeto == modelo.Projeto && p.Num_Doc == modelo.Num_Doc && p.Num_Obra == modelo.Num_Obra);
                            if (agendamento == null && modelo.Data_Maxima.HasValue && modelo.Hora_Maxima.HasValue)
                            {

                                var dataHoraInicio = (modelo?.Data_Maxima.Value.Add(modelo.Hora_Maxima.Value.TimeOfDay)).Value;

                                agendamento = new Agendamento_EquipeAniel
                                {
                                    CodCt = modelo.CodCt,
                                    Projeto = modelo.Projeto,
                                    Num_Obra = modelo.Num_Obra,
                                    Num_Doc = modelo.Num_Doc,
                                    Cod_Escritorio = modelo.Cod_Escritorio,
                                    Cod_Area = modelo.Cod_Area,
                                    Cod_Localidade = modelo.Cod_Localidade,
                                    DataHora_Inicio = dataHoraInicio,
                                    DataHora_Fim = dataHoraInicio.Add(tempoExecucao.Value),
                                    Flexivel = null,
                                    Deslocamento = 0,
                                    //Log = GerarLog(value.Objeto.Lista_Forca_Trabalho, value.Objeto.Lista_Ja_Agendados, config),
                                    Tipo_Servico = modelo.Cod_Tipo_Serv.HasValue ? await repoTipoServico.selectFirst(p => p.Cod_Tipo_Serv == modelo.Cod_Tipo_Serv) : null,
                                    Tipo_Imovel = modelo.Cod_Tipo_Imovel.HasValue ? await repoTipoImovel.selectFirst(p => p.Cod_Tipo_Imovel == modelo.Cod_Tipo_Imovel) : null,
                                    UsuarioCriacao = User.Identity.GetUserId(),
                                    Origem = OrigemAgendamentoOS.Manual,
                                    Latitude = modelo.GetLatitude_Str(),
                                    Longitude = modelo.GetLongitude_Str(),




                                };

                                if (modelo.Equipe != null)
                                {
                                    agendamento.Equipe = await repoEquipe.selectFirst(p => p.Matricula == modelo.Equipe);
                                }
                                else
                                {
                                    IList<Horario_Connect> horariosServico =
                                        await repositoryHorario.select(p => p.Tipo == Tipo_Hora_Connect.Horario_Servico &&
                                                                            p.Status == Sinapse.Model.enumerados.Status_Default.Ativo);



                                    var dadosServico = new DadosServicoAgendamento
                                    {
                                        Cod_Area = modelo.Cod_Area,
                                        Cod_Escritorio = modelo.Cod_Escritorio,
                                        Cod_Localidade = modelo.Cod_Localidade,
                                        Cod_Natureza = codNatureza,
                                        LocalidadeStr = value.Localidade,
                                        TempoExecucaoMinutos = DateTime.Today.AddMinutes(tempoExecucao.Value.TotalMinutes)
                                    };

                                    var agendamentoHorasDisponiveis =
                                        await agendamentoEquipeApi.GetAgendamentoHorasDisponiveis(
                                            config,
                                            horariosServico.ToList(),
                                            dataHoraInicio,
                                            tempoExecucao.Value,
                                            dadosServico
                                        );

                                    agendamento.Equipe_Tmp = agendamentoHorasDisponiveis.Equipe_Tmp;
                                }

                                await repositoryAgendamento.SaveOrUpdate(agendamento);
                            }
                            else if (modelo.Data_Maxima.HasValue == false || modelo.Hora_Maxima.HasValue == false)
                            {
                                if (agendamento != null)
                                    await repositoryAgendamento.Remove(agendamento);

                                modelo.Equipe = null;
                                modelo.Agendamento = null;


                                if (modelo.Status == StatusExecucaoAniel.Aberta_Aguard_Agendamento
                                     || modelo.Status == StatusExecucaoAniel.Aberta_Aguard_Responsavel
                                     || modelo.Status == StatusExecucaoAniel.Aberta_Aguard_Atendimento
                                     || modelo.Status == StatusExecucaoAniel.Atendimento_Paralizado)
                                    modelo.Status = StatusExecucaoAniel.Aberta_Aguard_Agendamento;
                            }
                            else if ((data_old != modelo.Data_Maxima || hora_old?.TimeOfDay != modelo.Hora_Maxima?.TimeOfDay) && agendamento != null)
                            {

                                var dataHoraInicio = (modelo?.Data_Maxima.Value.Add(modelo.Hora_Maxima.Value.TimeOfDay)).Value;
                                agendamento.DataHora_Inicio = (modelo?.Data_Maxima.Value.Add(modelo.Hora_Maxima.Value.TimeOfDay)).Value;
                                agendamento.DataHora_Fim = dataHoraInicio.Add(tempoExecucao.Value);
                                if (modelo.Equipe != null)
                                {
                                    agendamento.Equipe = await repoEquipe.selectFirst(p => p.Matricula == modelo.Equipe);
                                }
                                else
                                {
                                    IList<Horario_Connect> horariosServico =
                                        await repositoryHorario.select(p => p.Tipo == Tipo_Hora_Connect.Horario_Servico &&
                                                                            p.Status == Sinapse.Model.enumerados.Status_Default.Ativo);


                                    if (modelo.Cod_Tipo_Serv.HasValue)
                                        codNatureza = (await repoTipoServico.selectFirst(p => p.Cod_Tipo_Serv == modelo.Cod_Tipo_Serv.Value)).Cod_Natureza;

                                    var dadosServico = new DadosServicoAgendamento
                                    {
                                        Cod_Area = modelo.Cod_Area,
                                        Cod_Escritorio = modelo.Cod_Escritorio,
                                        Cod_Localidade = modelo.Cod_Localidade,
                                        Cod_Natureza = codNatureza,
                                        LocalidadeStr = value.Localidade,
                                        TempoExecucaoMinutos = DateTime.Today.AddMinutes(tempoExecucao.Value.TotalMinutes)
                                    };

                                    var agendamentoHorasDisponiveis =
                                        await agendamentoEquipeApi.GetAgendamentoHorasDisponiveis(
                                            config,
                                            horariosServico.ToList(),
                                            dataHoraInicio,
                                            tempoExecucao.Value,
                                            dadosServico
                                        );

                                    agendamento.Equipe_Tmp = agendamentoHorasDisponiveis.Equipe_Tmp;
                                }

                                agendamento.Origem = OrigemAgendamentoOS.Manual;
                                agendamento.DataHoraAlteracao = DateTime.Now;
                                await repositoryAgendamento.SaveOrUpdate(agendamento);

                            }
                            else if ((tipo_imovel_old != modelo.Cod_Tipo_Imovel || tipo_servico_old?.Cod_Tipo_Serv != modelo.Cod_Tipo_Serv) && agendamento != null)
                            {

                                var dataHoraInicio = (modelo?.Data_Maxima.Value.Add(modelo.Hora_Maxima.Value.TimeOfDay)).Value;
                                agendamento.DataHora_Inicio = (modelo?.Data_Maxima.Value.Add(modelo.Hora_Maxima.Value.TimeOfDay)).Value;
                                agendamento.DataHora_Fim = dataHoraInicio.Add(tempoExecucao.Value);
                                if (modelo.Equipe != null)
                                {
                                    agendamento.Equipe = await repoEquipe.selectFirst(p => p.Matricula == modelo.Equipe);
                                }
                                else
                                {
                                    IList<Horario_Connect> horariosServico =
                                        await repositoryHorario.select(p => p.Tipo == Tipo_Hora_Connect.Horario_Servico &&
                                                                            p.Status == Sinapse.Model.enumerados.Status_Default.Ativo);


                                    if (modelo.Cod_Tipo_Serv.HasValue)
                                        codNatureza = (await repoTipoServico.selectFirst(p => p.Cod_Tipo_Serv == modelo.Cod_Tipo_Serv.Value)).Cod_Natureza;

                                    var dadosServico = new DadosServicoAgendamento
                                    {
                                        Cod_Area = modelo.Cod_Area,
                                        Cod_Escritorio = modelo.Cod_Escritorio,
                                        Cod_Localidade = modelo.Cod_Localidade,
                                        Cod_Natureza = codNatureza,
                                        LocalidadeStr = value.Localidade,
                                        TempoExecucaoMinutos = DateTime.Today.AddMinutes(tempoExecucao.Value.TotalMinutes)
                                    };

                                    var agendamentoHorasDisponiveis =
                                        await agendamentoEquipeApi.GetAgendamentoHorasDisponiveis(
                                            config,
                                            horariosServico.ToList(),
                                            dataHoraInicio,
                                            tempoExecucao.Value,
                                            dadosServico
                                        );

                                    agendamento.Equipe_Tmp = agendamentoHorasDisponiveis.Equipe_Tmp;
                                }

                                agendamento.DataHoraAlteracao = DateTime.Now;
                                await repositoryAgendamento.SaveOrUpdate(agendamento);

                            }

                            modelo.Adicional_1 = value.Adcional_1;
                            modelo.Adicional_2 = value.Adcional_2;
                            modelo.Adicional_3 = value.Adcional_3;
                            modelo.Adicional_4 = value.Adcional_4;
                            modelo.Adicional_5 = value.Adcional_5;
                            modelo.Adicional_6 = value.Adcional_6;
                            modelo.Adicional_7 = value.Adcional_7;
                            modelo.Adicional_8 = value.Adcional_8;
                            modelo.Adicional_9 = value.Adcional_9;
                            modelo.Adicional_10 = value.Adcional_10;

                            if (!String.IsNullOrEmpty(value.Codigo_Motivo_Reagendamento))
                            {
                                modelo.CodMotivoReagendamento = value.Codigo_Motivo_Reagendamento != "0" ? Convert.ToInt64(value.Codigo_Motivo_Reagendamento) : 0;
                            }

                            modelo.ObsMotivoReagendamento = !String.IsNullOrEmpty(value.Observacao_Motivo_Reagendamento) ? value.Observacao_Motivo_Reagendamento : null;

                            await repository.SaveOrUpdate(modelo);

                            if (modelo.Equipe != null && (hora_old != modelo?.Hora_Maxima || data_old != modelo.Data_Maxima) && modelo.Equipe != "FALSE")
                            {
                                var equipe = await new Conexao<EquipeAniel>(session).selectFirst(p => p.Matricula == modelo.Equipe);
                                if (!String.IsNullOrEmpty(equipe?.Device_Id))
                                {
                                    MobileService.NotificarSmartphone(equipe.Device_Id, String.Empty, "Houve mudanças em sua rotina.", "Atualize a sua lista de atividades", "br.com.sinapse.touch.MSG_RESINC_OS", modelo.CodCt, modelo.Projeto, modelo.Num_Doc.ToString(), modelo.Num_Obra);
                                }
                            }

                        }

                        //await new Documento_ProducaoApiController().InsertLog_Lanc_Servico(modelo.CodCt, modelo.Projeto, modelo.Num_Obra, modelo.Num_Doc,
                        //        modelo.Equipe == null ? "Remoção de Equipe" : String.Format("Alteração para Equipe {0}", modelo.Equipe), null,
                        //        value.Usuario, null, null);


                        //using (ITransaction transaction = session.BeginTransaction())
                        //{
                        //    await repository.SaveOrUpdate(modelo);
                        //    if (agendamento != null)
                        //        await repositoryAgendamento.SaveOrUpdate(agendamento);
                        //    transaction.Commit();
                        //}
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

        private void ValidaAlteracaoDocumento(Documento_ProducaoAniel modelo, String campoAlterado = null)
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

            if (modelo.Responsabilidade == ResponsabilidadeOS.TECNICO && campoAlterado != "LatitudeLongitude")
            {
                throw new Exception($"Não será possível gravar dados do serviço, pois o mesmo se esta na responsabilidade do técnico.");
            }
        }


        [HttpGet()]
        [Route("api/Acompanhamento_ServicoApi/FindById_Historico")]
        public async Task<GetApiRetorno<Historico_ServicoCadastroModel>> FindById_Historico(String CodCt, String Projeto, String Num_Obra, Int32? Num_Doc)
        {
            //CodCt = "01";
            //Projeto = "ANDERSON";
            //Num_Obra = "20190529_121349";
            //Num_Doc = 0;
            GetApiRetorno<Historico_ServicoCadastroModel> retorno = new GetApiRetorno<Historico_ServicoCadastroModel>();


            try
            {
                using (ISession session_connect = HibernateUtil.GetSession())
                using (ConexaoMongo conexaoMongo = new ConexaoMongo())
                using (var repository = new RepositoryMongo<DocumentoProducaoMongo>(conexaoMongo))
                using (ISession session = HibernateUtil.GetSessionT5())
                using (ISession sessionSQL = HibernateUtil.GetSessionT5())
                using (var repoLogLanc = new Conexao<Log_Lanc_Servico>(session))
                using (var documentoApi = new Documento_ProducaoApiController())
                {
                    var num_obra_original = Num_Obra.Split('/')[0];
                    List<Documento_ProducaoAniel> documentos = new Conexao<Documento_ProducaoAniel>(session).select(p => p.CodCt == CodCt && p.Projeto == Projeto &&
                                                                                                             p.Numero_Obra_Original == num_obra_original && p.Num_Doc == Num_Doc).
                                                                                                             Result.ToList();



                    List<Log_Lanc_Servico> logs_Lanc = new List<Log_Lanc_Servico>(); //await repoLogLanc.select(p => p.CodCt == CodCt && p.Projeto == Projeto && p.Num_Obra.IsLike(String.Concat('%', num_obra_original, '%')) && p.Contrato == Num_Doc);

                    using (var com = sessionSQL.Connection.CreateCommand())
                    {
                        foreach (var item in documentos)
                        {
                            Dictionary<String, Object> parametros = new Dictionary<string, object>();
                            StringBuilder sql = new StringBuilder();

                            sql.AppendLine(@"SELECT CODCT, DIA, HORA, NUM_DOC, NUM_OBRA, PROJETO, USUARIO, ID, VALOR_ANT, ALTERACAO, DATA_MAXIMA, COD_TIPO_SERV,MOTIVO_EXCLUSAO,VALOR_ATUALIZADO,OBS_STATUS_COBRANCA, COD_MOTIVO_REAGENDAMENTO, OBSERVACAO_MOTIVO_REAGENDAMENTO
                                         FROM TB_LOG_LANC_SERVICO");

                            sql.AppendLine("WHERE CODCT = @CODCT AND PROJETO = @PROJETO AND NUM_OBRA = @NUM_OBRA AND NUM_DOC = @NUM_DOC");

                            parametros.Add("@CODCT", CodCt);
                            parametros.Add("@PROJETO", Projeto);
                            parametros.Add("@NUM_OBRA", $"{item.Num_Obra}");
                            parametros.Add("@NUM_DOC", Num_Doc);


                            //Busca com paginacao
                            using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionSQL.Connection, null))
                            {
                                while (await reader.ReadAsync())
                                {
                                    logs_Lanc.Add(new Log_Lanc_Servico
                                    {
                                        Id = Convert.ToInt32(reader["ID"]),
                                        Alteracao = Convert.ToString(reader["ALTERACAO"]),
                                        Num_Obra = Convert.ToString(reader["NUM_OBRA"]),
                                        Dia = Convert.ToDateTime(reader["DIA"]),
                                        Hora = reader["HORA"] != DBNull.Value ? new DateTime().Add((TimeSpan)(reader["HORA"])) : (DateTime?)null,
                                        CodCt = Convert.ToString(reader["CODCT"]),
                                        Projeto = Convert.ToString(reader["PROJETO"]),
                                        Usuario = Convert.ToString(reader["USUARIO"]),
                                        Motivo_Exclusao = Convert.ToString(reader["MOTIVO_EXCLUSAO"]),
                                        Valor_Ant = Convert.ToString(reader["VALOR_ANT"]),
                                        Data_Maxima = reader["DATA_MAXIMA"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_MAXIMA"]) : (DateTime?)null,
                                        Cod_Tipo_Serv = reader["COD_TIPO_SERV"] != DBNull.Value ? Convert.ToInt16(reader["Cod_Tipo_Serv"]) : (Int16?)null,
                                        Contrato = reader["NUM_DOC"] != DBNull.Value ? Convert.ToInt32(reader["NUM_DOC"]) : 0,
                                        Valor_Atualizado = Convert.ToString(reader["VALOR_ATUALIZADO"]),
                                        Obs_Cobranca_Status = Convert.ToString(reader["OBS_STATUS_COBRANCA"]),
                                        CodMotivoReagendamento = reader["COD_MOTIVO_REAGENDAMENTO"] != DBNull.Value ? Convert.ToInt64(reader["COD_MOTIVO_REAGENDAMENTO"]) : 0,
                                        ObsMotivoReagendamento = Convert.ToString(reader["OBSERVACAO_MOTIVO_REAGENDAMENTO"])
                                    });
                                }

                            }
                        }
                    }


                    List<StatusDocumentoMongoModel> logs = new List<StatusDocumentoMongoModel>();

                    foreach (var documento in documentos)
                    {
                        var _collection = repository.GetCollection();

                        var mongoDocumentos = _collection
                            .Aggregate()
                            .Match(p => p.codCt == documento.CodCt
                            && p.numObra == documento.Num_Obra
                            && p.contrato == documento.Num_Doc
                            && p.projeto == documento.Projeto)
                            .ToList();

                        foreach (var mongoDocumento in mongoDocumentos)
                        {
                            logs.AddRange(mongoDocumento.status.Select(p => new StatusDocumentoMongoModel()
                            {
                                numObra = documento.Num_Obra,
                                projeto = documento.Projeto,
                                codCt = documento.CodCt,
                                contrato = documento.Num_Doc,
                                equipe = p.equipe,
                                nome = p.nome,
                                latitude = p.latitude,
                                longitude = p.longitude,
                                data = p.data,
                                hora = p.hora,
                                dataHoraServidor = p.dataHoraServidor,
                                statusExecucao = p.statusExecucao,
                                justificativaPausaMongo = String.IsNullOrEmpty(p.justificativaPausaMongo) ? documento.ObservacaoPausa : p.justificativaPausaMongo,
                                justificativaReprova = p.justificativaReprova,
                                justificativaAprova = p.justificativaAprova,
                                indexStatus = mongoDocumento.status.IndexOf(p).ToString()
                            }).ToList());
                        }
                    }


                    TimeSpan TotalDeslocamento = new TimeSpan(0, 0, 0);
                    TimeSpan TotalAtendimento = new TimeSpan(0, 0, 0);
                    TimeSpan TotalParalisacoes = new TimeSpan(0, 0, 0);
                    TimeSpan TotalTempoAtenidmento = new TimeSpan(0, 0, 0);
                    Double CustoTotal = 0;




                    Dashboard_ServicosApiController controleDash = new Dashboard_ServicosApiController();
                    Documento_ProducaoApiController controleDocProd = new Documento_ProducaoApiController();
                    var documentofirst = documentos.FirstOrDefault();
                    var data_encerramento = documentofirst.Data_Prod.HasValue ? documentofirst.Data_Prod.Value.Add(documentofirst.Hora_Prod.Value.TimeOfDay) : (DateTime?)null;

                    var _deslocamentotemp = TimeSpan.FromTicks(documentos.Sum(p => controleDocProd.GetTempoDeslocamento(p.CodCt, p.Projeto, p.Num_Obra, p.Num_Doc, p.Data_Prod.HasValue ? p.Data_Prod.Value.Add(p.Hora_Prod.Value.TimeOfDay) : (DateTime?)null).Ticks));
                    TotalDeslocamento = Convert.ToDateTime(_deslocamentotemp.ToString(@"hh\:mm")).TimeOfDay;
                    var _paralisacaotemp = TimeSpan.FromTicks(documentos.Sum(p => controleDocProd.GetTempoParalizacao(p.CodCt, p.Projeto, p.Num_Obra, p.Num_Doc, p.Data_Prod.HasValue ? p.Data_Prod.Value.Add(p.Hora_Prod.Value.TimeOfDay) : (DateTime?)null).Value.Ticks));

                    TotalParalisacoes = Convert.ToDateTime(_paralisacaotemp.ToString(@"hh\:mm")).TimeOfDay;
                    TotalTempoAtenidmento = TimeSpan.FromTicks(documentos.Sum(p => controleDocProd.GetTempoAtendimento(p.CodCt, p.Projeto, p.Num_Obra, p.Num_Doc, p.Data_Prod.HasValue ? p.Data_Prod.Value.Add(p.Hora_Prod.Value.TimeOfDay) : (DateTime?)null).Ticks));
                    CustoTotal += Convert.ToDouble((await GetCustoServico(CodCt, Projeto, Num_Obra, Num_Doc.GetValueOrDefault(0))).Objeto);


                    List<Historico_ServicoListaModel> Historico_Servico = new List<Historico_ServicoListaModel>();

                    foreach (Documento_ProducaoAniel documento in documentos)
                    {


                        if (logs_Lanc.Where(p => (p.Alteracao.ToUpper() == "Criação do Serviço".ToUpper() || p.Alteracao.ToUpper() == "Criação O.S".ToUpper()) && p.Num_Obra == documento.Num_Obra).Count() == 0)
                        {
                            logs_Lanc.Add(new Log_Lanc_Servico
                            {

                                CodCt = documento.CodCt,
                                Projeto = documento.Projeto,
                                Num_Obra = documento.Num_Obra,
                                Alteracao = "Criação do Serviço",
                                Contrato = documento.Num_Doc,
                                Valor_Ant = "-",
                                Usuario = "-",
                                Dia = documento.Dia.Value,
                                Hora = documento.Hora.Value
                            });
                        }
                        else if (TotalTempoAtenidmento == TimeSpan.Zero &&
                                 documento.DataHora_Ini_Atend.HasValue &&
                                 documento.DataHora_Fim_Atend.HasValue)
                        {
                            TotalTempoAtenidmento += (Convert.ToDateTime((documento.DataHora_Fim_Atend.Value.ToString(@"hh\:mm")))) - (Convert.ToDateTime((documento.DataHora_Ini_Atend.Value.ToString(@"hh\:mm"))));
                        }
                        else if (TotalTempoAtenidmento == TimeSpan.Zero &&
                                 documento.DataHora_Ini_Atend.HasValue &&
                                 documento.Data_Prod.HasValue &&
                                 documento.Hora_Prod.HasValue)
                        {
                            TotalTempoAtenidmento += (Convert.ToDateTime((documento.Data_Prod.Value.Add(documento.Hora_Prod.Value.TimeOfDay).ToString(@"hh\:mm")))) - (Convert.ToDateTime((documento.DataHora_Ini_Atend.Value.ToString(@"hh\:mm"))));
                        }

                        Historico_Servico = logs.Select(p => new Historico_ServicoListaModel
                        {
                            Usuario = p.equipe,
                            DataHora = p.hora.ToString("dd/MM/yyyy HH:mm:ss"),
                            Latitude = Convert.ToString(p.latitude),
                            Longitude = Convert.ToString(p.longitude),
                            Status_Documento = documentos.Find(x => p.numObra == x.Num_Obra).Status,
                            NumObra = p.numObra,
                            TipoMudanca = "Mudança de Status",
                            Valor_Atual = HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>((StatusExecucaoAniel)p.statusExecucao.Value),
                            //Valor_Alterado = ,
                            Status_OS = HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>((StatusExecucaoAniel)p.statusExecucao.Value),
                            DataHora_Servidor = p.dataHoraServidor.HasValue ? p.dataHoraServidor.Value.ToString("dd/MM/yyyy HH:mm:ss") : String.Empty,

                            IndexStatus = p.indexStatus

                        }).ToList(); // para evitar erro

                        Historico_Servico = Historico_Servico.Concat(logs_Lanc.Select(p =>
                        {
                            var status = documentos.Find(x => p.Num_Obra == x.Num_Obra)?.Status;

                            return new Historico_ServicoListaModel
                            {
                                Id = p.Id,
                                Usuario = !String.IsNullOrEmpty(p.Usuario) ? p.Usuario : new Conexao<EquipeAniel>(session).
                                selectFirst(y => y.Matricula == documentos.Find(x => p.Num_Obra == x.Num_Obra).Equipe).Result?.Nome,
                                Latitude = "-",
                                Longitude = "-",
                                Status_Documento = documentos.Find(x => p.Num_Obra == x.Num_Obra)?.Status,
                                NumObra = p.Num_Obra,
                                TipoMudanca = p.Alteracao,
                                Valor_Alterado = String.IsNullOrEmpty(p.Valor_Ant) ? "-" : p.Valor_Ant,
                                Valor_Atual = p.Valor_Atualizado,
                                Status_OS = HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(status.GetValueOrDefault(StatusExecucaoAniel.Aberta_Aguard_Atendimento)),
                                DataHora = p.Dia.Add(p.Hora.Value.TimeOfDay).ToString("dd/MM/yyyy HH:mm:ss"),
                                Obs_Cobranca_Status = p.Obs_Cobranca_Status,
                                CodMotivoReagendamento = p.CodMotivoReagendamento,
                                ObsMotivoReagendamento = p.ObsMotivoReagendamento
                            };

                        })).ToList()
                        .OrderByDescending(p => !String.IsNullOrEmpty(p.DataHora) ? Convert.ToDateTime(p.DataHora) : new DateTime()) // para evitar erro
                        .ThenByDescending(p => p.Id) //desempatar a data com ID para não ficar embaralhado
                        .ToList();

                        Historico_Servico = Historico_Servico.Select((p, index) =>
                        {
                            if (p.TipoMudanca == "Mudança de Status" && p.Valor_Alterado == null)
                            {
                                if ((index + 1) < Historico_Servico.Count)
                                {
                                    if (Historico_Servico[index + 1].TipoMudanca == "Mudança de Status")
                                    {
                                        p.Valor_Alterado = Historico_Servico[index + 1].Valor_Atual ?? HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(StatusExecucaoAniel.Aberta_Aguard_Atendimento);
                                    }
                                    else if (p.Valor_Atual == HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(StatusExecucaoAniel.OS_Em_deslocamento))
                                        p.Valor_Alterado = HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(StatusExecucaoAniel.Aberta_Aguard_Atendimento);
                                    else
                                        p.Valor_Alterado = "-";
                                }
                                else
                                    p.Valor_Alterado = HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(StatusExecucaoAniel.Aberta_Aguard_Atendimento);


                            }
                            return p;
                        }).ToList();
                    }


                    retorno.Objeto = new Historico_ServicoCadastroModel
                    {
                        CodCt = CodCt,
                        Projeto = Projeto,
                        Num_Obra = Num_Obra,
                        Numero_Doc = Num_Doc,
                        Historico_Servico = Historico_Servico,
                        TempoRealDeslocamento = TotalDeslocamento.ToString(@"hh\:mm"),
                        TempoAtendimento = (TotalTempoAtenidmento - TotalParalisacoes).Duration().ToString(@"hh\:mm"),
                        TempoTotalParalisacoes = TotalParalisacoes.ToString(@"hh\:mm"),
                        TempoTotalAtendimento = TotalTempoAtenidmento.ToString(@"hh\:mm"),
                        CustoTotalServico = CustoTotal.ToString("N2")
                    };
                }
                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException += ex.InnerException.Message;
            }

            return retorno;
        }

        private string GetValorAtual(Documento_ProducaoAniel documento, string tipoMudanca)
        {

            string retorno = null;
            if (tipoMudanca == "Cancelamento")
            {
                retorno = "-";
            }

            else if (tipoMudanca == "Latitude/Longitude")
            {
                retorno = $"{documento.GetLatitude_Str()},{documento.GetLongitude_Str()}";
            }

            else if (tipoMudanca == "Baixou OS" || tipoMudanca == "Reabriu OS" || tipoMudanca == "Corrigiu OS")
            {
                retorno = "-";
            }
            else if (tipoMudanca == "Agendamento")
            {
                if (documento.Data_Maxima.HasValue)
                {
                    if (documento.Hora_Maxima.HasValue)
                        retorno = documento.Data_Maxima.Value.Add(documento.Hora.Value.TimeOfDay).ToString("dd/MM/yyyy HH:mm");
                    else
                        retorno = documento.Data_Maxima.Value.ToString("dd/MM/yyyy HH:mm");
                }
                else
                    retorno = "-";
            }
            else if (tipoMudanca == "Criação do Serviço")
            {
                retorno = "-";
            }
            else if (tipoMudanca == "Incluiu Material" || tipoMudanca == "Incluiu Produção")
            {
                retorno = "-";
            }
            else if (tipoMudanca == "Código de Encerramento")
            {
                retorno = documento.Cod_Encerramento;
            }
            else if (tipoMudanca == "Tipo de Serviço")
            {
                retorno = documento.Cod_Tipo_Serv.ToString();
            }
            else if (tipoMudanca == "Subtipo de Serviço")
            {
                retorno = documento.Cod_Sub_Tipo_Serv.ToString();
            }
            else if (tipoMudanca.Contains("Equipe"))
            {
                retorno = documento.Equipe;
            }

            if (String.IsNullOrEmpty(retorno))
                retorno = "-";

            return retorno;
        }

        [HttpGet()]
        [Route("api/Acompanhamento_ServicoApi/GetObservacaoTestePorSeq")]
        public async Task<GetApiRetorno<String>> GetObservacaoTestePorSeq(String CodCt, String Projeto, String Num_Obra, Int32? Num_Doc)
        {
            GetApiRetorno<String> retorno = new GetApiRetorno<string>();
            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var com = sessionT5.Connection.CreateCommand())
                {


                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine("SELECT B.COD_ENCERRAMENTO,B.DESCRICAO,COALESCE(A.OBSERVACAO_TESTE,'') as OBSERVACAO_TESTE");
                    sql.AppendLine("FROM TB_DOCUMENTO_PRODUCAO A ");
                    sql.AppendLine("LEFT JOIN TB_CODIGO_ENCERRAMENTO B ON(A.COD_ENCERRAMENTO = B.COD_ENCERRAMENTO AND A.CODCT = B.CODCT)");
                    sql.AppendLine("WHERE A.CODCT = @CODCT AND A.PROJETO = @PROJETO AND A.NUM_OBRA = @NUM_OBRA AND A.NUM_DOC = @NUM_DOC");

                    parametros.Add("@CODCT", CodCt);
                    parametros.Add("@PROJETO", Projeto);
                    parametros.Add("@NUM_OBRA", Num_Obra);
                    parametros.Add("@NUM_DOC", Num_Doc);


                    //Busca com paginacao
                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            retorno.Objeto = Convert.ToString(reader["COD_ENCERRAMENTO"]) != null ? Convert.ToString(reader["COD_ENCERRAMENTO"]) + " - " + Convert.ToString(reader["DESCRICAO"]) + "\n\n" + Convert.ToString(reader["OBSERVACAO_TESTE"]) : null;
                        }

                    }
                }

                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.Ok = false;
                retorno.InnerException = ex.InnerException?.Message ?? String.Empty;
            }

            return retorno;
        }

        [HttpGet()]
        [Route("api/Acompanhamento_ServicoApi/GetObjMotivoReagendamento")]
        public async Task<GetApiRetorno<MotivoReagendamentosAniel>> GetObjMotivoReagendamento(String cod_motivo)
        {
            GetApiRetorno<MotivoReagendamentosAniel> retorno = new GetApiRetorno<MotivoReagendamentosAniel>();

            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var _repoMotivo = new Conexao<MotivoReagendamentosAniel>(sessionT5))
                {
                    if (cod_motivo != "0")
                    {
                        var objMot = await _repoMotivo.selectFirst(x => x.Codigo == Convert.ToInt64(cod_motivo));
                        retorno.Objeto = objMot;
                    }
                }
                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.Ok = false;
                retorno.InnerException = ex.InnerException?.Message ?? String.Empty;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Acompanhamento_ServicoApi/AlterarObservacaoTecnico")]
        public async Task<PostApiRetorno> AlterarObservacaoTecnico(AlterarObservacaoTecnico request)
        {

            PostApiRetorno retorno = new PostApiRetorno();
            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var com = sessionT5.Connection.CreateCommand())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine($"UPDATE TB_DOCUMENTO_PRODUCAO SET OBSERVACAO_TESTE = @OBSERVACAO_TESTE WHERE CODCT = @CODCT AND PROJETO = @PROJETO AND NUM_OBRA = @NUM_OBRA AND NUM_DOC = @NUM_DOC ");

                    parametros.Add("@CODCT", request.codCt);
                    parametros.Add("@PROJETO", request.projeto);
                    parametros.Add("@NUM_OBRA", request.num_obra);
                    parametros.Add("@NUM_DOC", request.num_doc);
                    parametros.Add("@OBSERVACAO_TESTE", request.observacao_teste);

                    await CommandHelper.ExecuteNonQuery(com, sql, parametros, sessionT5.Connection, null);

                }

                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.Ok = false;
                retorno.InnerException = ex.InnerException?.Message ?? String.Empty;
            }

            return retorno;
        }


        [HttpGet()]
        [Route("api/Acompanhamento_ServicoApi/GetObservacaoPausa")]
        public async Task<GetApiRetorno<String>> GetObservacaoPausaPorSeq(String CodCt, String Projeto, String Num_Obra, Int32? Num_Doc, int index)
        {
            GetApiRetorno<String> retorno = new GetApiRetorno<string>
            {
                Objeto = String.Empty
            };

            try
            {
                using ISession session = HibernateUtil.GetSessionT5();
                using ConexaoMongo conexaoMongo = new ConexaoMongo();
                using var repository = new RepositoryMongo<DocumentoProducaoMongo>(conexaoMongo);


                var collection = repository.GetCollection();
                var documentoProducao = await collection.Aggregate()
                                                .Match(p => p.codCt == CodCt
                                                            && p.numObra == Num_Obra
                                                            && p.contrato == Num_Doc
                                                            && p.projeto == Projeto
                                                            )
                                                .FirstOrDefaultAsync();

                if (documentoProducao == null)
                {
                    throw new Exception("Documento não encontrado");
                }

                var statusDocumento = documentoProducao.status;

                if (statusDocumento == null)
                {
                    throw new Exception("Não existem Justificativas");
                }

                Documento_ProducaoApiController api = new Documento_ProducaoApiController();
                var MotivoParalizacaoDocumentoProducao = await api.GetMotivoParalizacaoDocumentoProducao(new MotivoParalizacaoRequest
                {
                    CodCt = CodCt,
                    Contrato = Num_Doc.Value,
                    Projeto = Projeto,
                    NumObra = Num_Obra
                });

                retorno.Objeto = String.IsNullOrEmpty(statusDocumento[index].justificativaPausaMongo) ? MotivoParalizacaoDocumentoProducao : statusDocumento[index].justificativaPausaMongo;

                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.Ok = false;
                retorno.InnerException = ex.InnerException?.Message ?? String.Empty;
            }

            return retorno;
        }




        [HttpPost()]
        [Route("api/Acompanhamento_ServicoApi/AlterarObservacaoPausa")]
        public async Task<PostApiRetorno> AlterarObservacaoPausa(AlterarObservacaoPausa request)
        {

            PostApiRetorno retorno = new PostApiRetorno();
            try
            {
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var com = sessionT5.Connection.CreateCommand())
                {
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    StringBuilder sql = new StringBuilder();

                    sql.AppendLine($"UPDATE TB_DOCUMENTO_PRODUCAO SET OBSERVACAO_PAUSA = @OBSERVACAO_PAUSA WHERE CODCT = @CODCT AND PROJETO = @PROJETO AND NUM_OBRA = @NUM_OBRA AND NUM_DOC = @NUM_DOC ");

                    parametros.Add("@CODCT", request.codct);
                    parametros.Add("@PROJETO", request.projeto);
                    parametros.Add("@NUM_OBRA", request.num_obra);
                    parametros.Add("@NUM_DOC", request.num_doc);
                    parametros.Add("@OBSERVACAO_PAUSA", request.observacao_pausa);

                    await CommandHelper.ExecuteNonQuery(com, sql, parametros, sessionT5.Connection, null);

                }

                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.Ok = false;
                retorno.InnerException = ex.InnerException?.Message ?? String.Empty;
            }

            return retorno;
        }


        [HttpGet()]
        [Route("api/Acompanhamento_Servicoapi/FindById")]
        public async Task<GetApiRetorno<AcompanhamentoListaModel>> FindById(String Codct, String Projeto, String Num_Obra, Int32? Num_Doc)
        {
            GetApiRetorno<AcompanhamentoListaModel> retorno = new GetApiRetorno<AcompanhamentoListaModel>();

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var repoAtendimento = new Conexao<Atendimento>(session))
            using (var repoElemento_Rede_Porta = new Conexao<Elemento_Rede_Porta>(sessionT5))
            using (var repoElemento_Agendamento_EquipeAniel = new Conexao<Agendamento_EquipeAniel>(sessionT5))
            {
                try
                {
                    var obj = await new Conexao<Documento_ProducaoAniel>(sessionT5).selectFirst(p => p.CodCt == Codct && p.Projeto == Projeto && p.Num_Obra == Num_Obra && p.Num_Doc == Num_Doc);
                    retorno.Objeto = new AcompanhamentoListaModel();
                    retorno.Objeto.Tipo = obj.Tipo;
                    retorno.Objeto.CodCt = obj.CodCt;
                    retorno.Objeto.Projeto = obj.Projeto;
                    retorno.Objeto.Observacao_Aux = !String.IsNullOrEmpty(obj.Observacao_Aux) ? obj.Observacao_Aux : "-";
                    retorno.Objeto.Num_Doc = obj.Num_Doc;
                    retorno.Objeto.TipoManutencaoOs = obj.TipoManutencaoOs;
                    retorno.Objeto.Num_OS = obj.Num_Obra;
                    retorno.Objeto.Equipe = obj.Equipe;
                    retorno.Objeto.Unidade_Custo = obj.Unidade_Custo;
                    retorno.Objeto.Num_OS_Original = obj.Numero_Obra_Original;
                    retorno.Objeto.CodCt = obj.CodCt;
                    retorno.Objeto.Status_OS = obj.Status != null ? HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(obj.Status.Value) : null;
                    retorno.Objeto.Status_Execucao = obj.Status;
                    retorno.Objeto.Status_Cobranca = obj.Status_Cobranca;
                    retorno.Objeto.Tipo_Servico_Equipe = obj.TipoServico?.Exige_Pagamento_Instalacao == "S" ? Padrao.Sim : Padrao.Nao;
                    retorno.Objeto.CodNatureza = obj.TipoServico?.Cod_Natureza;
                    retorno.Objeto.Data_Prod = obj.Data_Maxima.HasValue ? obj.Data_Maxima.Value.ToString("dd/MM/yyyy") : null;
                    retorno.Objeto.TipoServico = GetTipoServico(Codct, Projeto, Num_Obra, sessionT5).Result;
                    retorno.Objeto.Status_Auditoria = obj.Status_Auditoria;
                    retorno.Objeto.CodTipoServ = obj.Cod_Tipo_Serv;
                    retorno.Objeto.Endereco_Mudanca = obj.Id_Atendimento.HasValue ? ((await repoAtendimento.selectFirst(x => x.Endereco_Mudanca != null && x.Id == (long)obj.Id_Atendimento)) != null) : false;
                    retorno.Objeto.Id_Caixa_FTTH = obj.CaixaFTTH;
                    var obj_rede_porta = !String.IsNullOrEmpty(obj.PortaFTTH) && !String.IsNullOrEmpty(obj.CaixaFTTH) ? await repoElemento_Rede_Porta.selectFirst(x => x.Porta == Convert.ToInt32(obj.PortaFTTH) && x.Caixa.Id == Convert.ToInt64(obj.CaixaFTTH)) : null;
                    retorno.Objeto.Id_Porta_FTTH = obj_rede_porta != null ? Convert.ToString(obj_rede_porta.Id) : String.Empty;

                    using (var periodoapi = new PeriodoApiController())
                    {
                        retorno.Objeto.Periodo = obj.CodTurnoServ.HasValue ? periodoapi.FindById(obj.CodTurnoServ.Value).Result.Objeto?.Descricao : "-";
                    }

                    var _temp_agendamento = (obj.Data_Maxima != null ? obj.Data_Maxima.Value.ToString("dd/MM/yyyy") : Resources.RsCadastros.SEM_AGENDAMENTO);
                    if (obj.Data_Maxima != null && obj.Hora_Maxima != null) _temp_agendamento += ' ' + (obj.Hora_Maxima.Value).ToString("HH:mm");
                    retorno.Objeto.Agendamento = _temp_agendamento;

                    string tituloTexto = "";
                        
                    if(obj.Data_Maxima.HasValue)
                        tituloTexto = "Alerta de Manutenção de Rede - Período Previsto de Execução:";

                    if(obj.Status == StatusExecucaoAniel.Fechada_Produtiva || obj.Status == StatusExecucaoAniel.Fechada_Improdutiva)
                        tituloTexto = "Alerta de Manutenção de Rede - Período de Execução:";

                    if (obj.TipoManutencaoOs == TipoManutencaoOs.Corretiva)
                    {
                        var dth_inicio = obj.Dia.HasValue ? obj.Dia.Value.ToString("dd/MM/yyyy") : String.Empty;
                        if (obj.Dia.HasValue && obj.Hora.HasValue) dth_inicio += ' ' + (obj.Hora.Value).ToString("HH:mm");

                        var dth_fim = (obj.Data_Indicador != null ? obj.Data_Indicador.Value.ToString("dd/MM/yyyy") : "Sem Indicador de Sla");
                        if (obj.Data_Indicador.HasValue && obj.Hora_Indicador.HasValue) dth_fim += ' ' + (obj.Hora_Indicador.Value).ToString("HH:mm");

                        retorno.Objeto.DataHora_TipoManutencaoCorretiva = $"{tituloTexto} {dth_inicio} - {dth_fim}";
                    }
                    else
                    {
                        retorno.Objeto.DataHora_TipoManutencaoPreventiva = Resources.RsCadastros.SEM_AGENDAMENTO;
                        var agendamento = await repoElemento_Agendamento_EquipeAniel.selectFirst(x => x.Num_Obra == obj.Num_Obra);
                        if (agendamento != null)
                        {
                            var dth_inicio = agendamento.DataHora_Inicio.ToString("dd/MM/yyyy HH:mm");
                            var dth_fim = agendamento.DataHora_Fim.ToString("dd/MM/yyyy HH:mm");
                            retorno.Objeto.DataHora_TipoManutencaoPreventiva = $"{tituloTexto} {dth_inicio} - {dth_fim}";
                        }
                    }

                    var _temp_indicador = (obj.Data_Indicador != null ? obj.Data_Indicador.Value.ToString("dd/MM/yyyy") : "Sem Indicador de Sla");
                    if (obj.Data_Indicador.HasValue && obj.Hora_Indicador.HasValue)
                        _temp_indicador += ' ' + (obj.Hora_Indicador.Value).ToString("HH:mm");

                    if (obj.Data_Indicador.HasValue && obj.Hora_Indicador.HasValue)
                    {
                        DateTime? DataHora_Indicador = obj.Data_Indicador.Value.AddHours(obj.Hora_Indicador.Value.Hour).AddMinutes(obj.Hora_Indicador.Value.Minute).AddSeconds(obj.Hora_Indicador.Value.Second);
                        DateTime? DataHora_Criacao = obj.Dia.Value.AddHours(obj.Hora.Value.Hour).AddMinutes(obj.Hora.Value.Minute).AddSeconds(obj.Hora.Value.Second);
                        DateTime? DataHora_Prod = null;
                        if (obj.Data_Prod.HasValue && obj.Hora_Prod.HasValue)
                            DataHora_Prod = obj.Data_Prod.Value.AddHours(obj.Hora_Prod.Value.Hour).AddMinutes(obj.Hora_Prod.Value.Minute);
                        else if (obj.Data_Prod.HasValue)
                            DataHora_Prod = obj.Data_Prod.Value;

                        retorno.Objeto.SLA_Atual = $"{DataHora_Indicador.Value.ToString("dd/MM/yyyy HH:mm")}  ({new Documento_ProducaoApiController().GetDescricaoSla(DataHora_Indicador, DataHora_Criacao, DataHora_Prod)})";
                    }
                    else
                        retorno.Objeto.SLA_Atual = _temp_indicador;

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

        [HttpGet()]
        [Route("api/Acompanhamento_Servicoapi/AlterarStatus")]
        public async Task<PostApiRetorno<Int64>> AlterarStatus(String CodCt, String Projeto, String Num_Obra, String Usuario, Int32 numDoc = 0, Int32 CodMotivo = 0, String MotivoCancelamento = null)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            var num_os = await getOsReaberta(Num_Obra, CodCt, Projeto, numDoc);

            if (String.IsNullOrEmpty(num_os))
            {
                retorno.Ok = false;
                retorno.Mensagem = RsCadastros.SERVICO_NAO_ENCONTRADO;
                return retorno;
            }

            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            {
                using (ITransaction transaction = sessionT5.BeginTransaction())
                {
                    using (ISession session2 = HibernateUtil.GetSessionT5())
                    using (ISession session = HibernateUtil.GetSession())
                    {
                        try
                        {

                            var os = await new Conexao<Documento_ProducaoAniel>(session2).selectFirst(p => p.CodCt == CodCt && p.Projeto == p.Projeto && p.Num_Doc == numDoc && p.Num_Obra == num_os);
                            if (os.Responsabilidade == ResponsabilidadeOS.TECNICO)
                            {
                                throw new Exception("Não é possível cancelar a ordem de serviço pois ela esta com a responsabilidade do técnico.");
                            }
                            Documento_ProducaoApiController doc_api = new Documento_ProducaoApiController();
                            await doc_api.InsertLog_Lanc_Servico
                                (sessionT5, CodCt, Projeto, Num_Obra, numDoc, String.Concat("Cancelamento"), HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(os.Status.Value), Usuario, os.Data_Maxima, os.Cod_Tipo_Serv, HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(StatusExecucaoAniel.Cancelado), MotivoCancelamento, transaction);

                            var atendimento = await new Conexao<Atendimento>(session).selectFirst(x => x.Id == os.Id_Atendimento);
                            if (atendimento != null)
                            {
                                if (atendimento.Endereco_Mudanca != null)
                                {
                                    atendimento.Endereco_Mudanca = null;
                                    atendimento.Ultima_Acao.Add(new Atendimento_Acao()
                                    {
                                        Atendimento = atendimento,
                                        HTML = "Alteração de endereço de instalação abortada! Ordem de Serviço cancelada.",
                                        Responsavel = atendimento.Responsavel,
                                        Status_Atendimento = atendimento.Status_Atendimento,
                                        UsuarioCriacao = User.Identity.GetUserId(),
                                    });

                                    await new Conexao<Atendimento>(session).SaveOrUpdate(atendimento);
                                }
                            }

                            //DbTransaction  dbTransaction = new DbTransaction();
                            Dictionary<String, Object> parametros = new Dictionary<string, object>();
                            using (var com = sessionT5.Connection.CreateCommand())
                            {
                                StringBuilder sql = new StringBuilder();
                                // /* , DATA_HORA_FECHAMENTO = '{DateTime.Now.ToString().Replace("/", ".")}' */
                                sql.AppendLine($"UPDATE TB_DOCUMENTO_PRODUCAO SET STATUS_EXECUCAO=9,RESPONSABILIDADE=1,COD_MOTIVO_CANCEL=@COD_MOTIVO_CANCEL, MOTIVOCANCELAMENTO=@MOTIVOCANCELAMENTO WHERE CODCT = @CODCT AND PROJETO = @PROJETO AND NUM_OBRA = @NUM_OBRA AND NUM_DOC = @NUM_DOC ");

                                parametros.Add("@CODCT", CodCt);
                                parametros.Add("@PROJETO", Projeto);
                                parametros.Add("@NUM_OBRA", num_os);
                                parametros.Add("@NUM_DOC", numDoc);
                                parametros.Add("@COD_MOTIVO_CANCEL", CodMotivo);
                                parametros.Add("@MOTIVOCANCELAMENTO", MotivoCancelamento);

                                transaction.Enlist(com);

                                await CommandHelper.ExecuteNonQuery(com, sql, parametros, sessionT5.Connection, com.Transaction);
                            }

                            using (var com = sessionT5.Connection.CreateCommand())
                            {
                                StringBuilder sql = new StringBuilder();
                                transaction.Enlist(com);
                                sql.AppendLine($"DELETE FROM TB_AGENDAMENTO_EQUIPEANIEL WHERE CODCT = @CODCT AND PROJETO = @PROJETO AND NUM_OBRA = @NUM_OBRA AND NUM_DOC = @NUM_DOC ");
                                await CommandHelper.ExecuteNonQuery(com, sql, parametros, sessionT5.Connection, com.Transaction);
                            }


                            transaction.Commit();
                            retorno.Ok = true;
                        }
                        catch (Exception ex)
                        {
                            retorno.Mensagem = ex.Message;
                            retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                        }
                    }
                }

            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Acompanhamento_Servicoapi/AdicionarInformacaoComplementar")]
        public async Task<PostApiRetorno<Int64>> AdicionarInformacaoComplementar(AdicionarInformacaoComplementarModel model)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            var num_os = await getOsReaberta(model.Num_Obra, model.CodCt, model.Projeto, model.numDoc);
            retorno.Ok = true;

            if (String.IsNullOrEmpty(num_os))
            {
                retorno.Ok = false;
                retorno.Mensagem = RsCadastros.SERVICO_NAO_ENCONTRADO;
                return retorno;
            }

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (var repo = new Conexao<Documento_ProducaoAniel>(session))
                {
                    Documento_ProducaoAniel doc = await repo.selectFirst(p => p.CodCt == model.CodCt && p.Projeto == model.Projeto && p.Num_Doc == model.numDoc && p.Num_Obra == num_os);

                    if (doc != null)
                    {
                        if (String.IsNullOrEmpty(doc.InformacoesComplementares))
                        {
                            doc.InformacoesComplementares = model.Informacao;
                        }
                        else
                        {
                            doc.InformacoesComplementares += $"\n{model.Informacao}";
                        }

                        await repo.Update(doc);
                        retorno.Mensagem = doc.InformacoesComplementares;

                    }
                    else
                    {
                        retorno.Ok = false;
                        retorno.Mensagem = RsCadastros.SERVICO_NAO_ENCONTRADO;
                        return retorno;
                    }


                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                retorno.Ok = false;
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/Acompanhamento_Servicoapi/VerificarCriterioAgendamento")]
        public async Task<PostApiRetorno<Int64>> VerificarCriterioAgendamento(AdicionarInformacaoComplementarModel model)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();
            var num_os = await getOsReaberta(model.Num_Obra, model.CodCt, model.Projeto, model.numDoc);
            retorno.Ok = true;

            if (String.IsNullOrEmpty(num_os))
            {
                retorno.Ok = false;
                retorno.Mensagem = RsCadastros.SERVICO_NAO_ENCONTRADO;
                return retorno;
            }

            try
            {
                using (ISession session = HibernateUtil.GetSessionT5())
                using (ISession sessionConnect = HibernateUtil.GetSession())
                using (var repoConfig = new Conexao<Configuracoes_Gerais>(sessionConnect))
                using (var repo = new Conexao<Documento_ProducaoAniel>(session))
                using (var repoTipoServico = new Conexao<Tipo_Servico_EquipeAniel>(session))
                {
                    Documento_ProducaoAniel doc = await repo.selectFirst(p => p.CodCt == model.CodCt && p.Projeto == model.Projeto && p.Num_Doc == model.numDoc && p.Num_Obra == num_os);
                    var config = await repoConfig.selectFirst();

                    if (doc != null)
                    {
                        if (config.Criterio_Area)
                        {
                            if (String.IsNullOrEmpty(doc.Cod_Escritorio) || String.IsNullOrEmpty(doc.Cod_Area) || doc.Cod_Localidade.HasValue == false)
                            {
                                throw new Exception("Serviço sem localidade vinculada.");
                            }
                        }

                        if (config.Criterio_Habilidade)
                        {
                            if (doc.Cod_Tipo_Serv == null)
                            {
                                throw new Exception("Serviço sem natureza vinculada.");
                            }
                            else
                            {
                                var tipoServico = await repoTipoServico.selectFirst(p => p.Cod_Tipo_Serv == doc.Cod_Tipo_Serv);
                                if (tipoServico?.Cod_Natureza == null)
                                {
                                    throw new Exception("Serviço sem natureza vinculada.");
                                }
                            }
                        }

                        if (config.Exige_LatLong_Agendamento)
                        {
                            if(String.IsNullOrEmpty(doc.Latitude_Str) || String.IsNullOrEmpty(doc.Longitude_Str))
                            {
                                throw new Exception("Latitude/Longitude não vinculada.");
                            }
                        }

                    }
                    else
                    {
                        retorno.Ok = false;
                        retorno.Mensagem = RsCadastros.SERVICO_NAO_ENCONTRADO;
                        return retorno;
                    }



                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                retorno.Ok = false;
            }

            return retorno;
        }

        public async Task<String> getOsReaberta(String num_obra_original, String CodCt, String Projeto, Int32? numDoc)
        {
            num_obra_original = num_obra_original.Split('/')[0];

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                var lista_os = await new Conexao<Documento_ProducaoAniel>(session).select(p => p.CodCt == CodCt && p.Projeto == Projeto && p.Num_Doc == numDoc && p.Numero_Obra_Original == num_obra_original);
                return lista_os.OrderByDescending(p => p.Seq).FirstOrDefault()?.Num_Obra;
            }
        }

        public async Task<Documento_ProducaoAniel> getOsCompletaReaberta(String num_obra_original, String CodCt, String Projeto, Int32? numDoc)
        {
            num_obra_original = num_obra_original.Split('/')[0];

            using (ISession session = HibernateUtil.GetSessionT5())
            {
                var lista_os = await new Conexao<Documento_ProducaoAniel>(session).select(p => p.CodCt == CodCt && p.Projeto == Projeto && p.Num_Doc == numDoc && p.Numero_Obra_Original == num_obra_original);
                return lista_os.OrderByDescending(p => p.Seq).FirstOrDefault();
            }
        }

        async Task<string> GetTipoServico(String Codct, String Projeto, String Num_Obra, ISession session)
        {
            string ret = "";
            Dictionary<string, object> parametros = new Dictionary<string, object>();
            parametros.Add("codct", Codct);
            parametros.Add("numObra", Num_Obra);
            parametros.Add("proj", Projeto);

            using (var com = session.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder(@"SELECT B.DESCRICAO AS TIPO FROM TB_DOCUMENTO_PRODUCAO A
                                                        LEFT JOIN TB_TIPO_SERVICO_EQUIPE B
                                                        ON(A.COD_TIPO_SERV = B.COD_TIPO_SERV)");
                sql.AppendLine("WHERE A.codct = @codct AND A.num_obra = @numObra AND A.projeto = @proj");

                using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    while (await reader.ReadAsync())
                    {
                        ret = reader["TIPO"] != DBNull.Value ? reader["TIPO"].ToString() : "";
                    }
                }
            }

            return ret;
        }

        #region Mobile
        [AllowAnonymous]
        [OlaAuthorize]
        [HttpPost()]
        [Route("api/Acompanhamento_ServicoApi/CancelarVisita")]
        public async Task<PostApiRetorno<String>> CancelarVisita(CancelamentoVisita value)
        {
            PostApiRetorno<String> retorno = new PostApiRetorno<String>();

            Documento_ProducaoAniel documento_producao_old = new Documento_ProducaoAniel();
            EquipeAniel equipe_old = new EquipeAniel();
            StatusExecucaoAniel? status_old;

            try
            {
                Documento_ProducaoAniel modelo = null;

                using (ISession session = HibernateUtil.GetSessionT5())
                {
                    var repositoryAgendamento = new Conexao<Agendamento_EquipeAniel>(session);
                    var agendamento = repositoryAgendamento.selectFirst(p => p.CodCt == value.CodCt &&
                                                                        p.Projeto == value.Projeto &&
                                                                        p.Num_Doc == value.Num_Doc &&
                                                                        p.Num_Obra == value.Num_Obra).Result;

                    using (var repository = new Conexao<Documento_ProducaoAniel>(session))
                    {
                        if (!String.IsNullOrEmpty(value.Num_Obra))
                        {
                            modelo = (await repository.select(p => p.CodCt == value.CodCt && p.Projeto == value.Projeto &&
                                                                   p.Num_Doc == value.Num_Doc && p.Num_Obra == value.Num_Obra)).FirstOrDefault();

                        }

                        status_old = modelo.Status;


                        if (modelo.Status == StatusExecucaoAniel.Aberta_Aguard_Responsavel ||
                            modelo.Status == StatusExecucaoAniel.Aberta_Aguard_Atendimento ||
                            modelo.Status == StatusExecucaoAniel.Aberta_Aguard_Agendamento ||
                            modelo.Status == StatusExecucaoAniel.Atendimento_Paralizado)
                        {

                            modelo.Data_Maxima = null;
                            modelo.Hora_Maxima = null;
                            modelo.Agendamento = null;

                            modelo.Status = StatusExecucaoAniel.Cancelado;


                            await new Documento_ProducaoApiController().InsertLog_Lanc_Servico(session, modelo.CodCt, modelo.Projeto, modelo.Num_Obra, modelo.Num_Doc,
                                   "Cancelamento de Visita", $"{value.Descricao_Cancelamento}",
                                    value.Usuario, null, null, modelo.Equipe, null);


                            using (ITransaction transaction = session.BeginTransaction())
                            {
                                try
                                {
                                    await repository.SaveOrUpdate(modelo);

                                    transaction.Commit();

                                    if (agendamento != null)
                                    {
                                        Dictionary<String, Object> parametros = new Dictionary<string, object>();
                                        using (var com = session.Connection.CreateCommand())
                                        {
                                            StringBuilder sql = new StringBuilder($"DELETE FROM TB_AGENDAMENTO_EQUIPEANIEL WHERE ID = @ID");
                                            parametros.Add("@ID", agendamento.Id);

                                            var result = await CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
                                            retorno.IdObjeto = Convert.ToString(result);
                                        }
                                        //await repositoryAgendamento.Remove(agendamento);

                                        //transaction.Commit();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    transaction.Rollback();
                                    throw ex;
                                }
                            }


                            var equipe = await new Conexao<EquipeAniel>(session).selectFirst(p => p.Matricula == modelo.Equipe);
                            // Envia mensagem para o mobile com alteração da OS
                            if (!String.IsNullOrEmpty(equipe?.Device_Id))
                            {
                                MobileService.NotificarSmartphone(equipe.Device_Id, String.Empty, "Houve mudanças em sua rotina.", "Atualize a sua lista de atividades", "br.com.sinapse.touch.MSG_RESINC_OS", value.CodCt, value.Projeto, value.Num_Doc.ToString(), value.Num_Obra);
                            }

                            if (modelo.Status != status_old)
                            {

                                await new Documento_ProducaoApiController().InsertLog_Lanc_Servico(session,
                                                                                        modelo.CodCt,
                                                                                        modelo.Projeto,
                                                                                        modelo.Num_Obra,
                                                                                        modelo.Num_Doc,
                                                                                        "Mudança de Status",
                                                                                        HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(status_old.Value),
                                                                                        value.Usuario,
                                                                                        null,
                                                                                        null,
                                                                                        HtmlEnumExtensions.GetEnumDescription<StatusExecucaoAniel>(modelo.Status.Value), null);
                            }
                            retorno.IdObjeto = "";
                            retorno.Ok = true;
                        }
                        else if (modelo.Status == StatusExecucaoAniel.Cancelado)
                        {
                            retorno.IdObjeto = "";
                            retorno.Ok = true;
                            retorno.Mensagem = "Atendimento já se encontra cancelado.";
                        }
                        else
                        {
                            retorno.IdObjeto = "";
                            retorno.Ok = true;
                            retorno.Mensagem = "Atendimento não pode ser cancelado pois o técnico já está atuando no mesmo.";
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = "";
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }
        #endregion
    }
}