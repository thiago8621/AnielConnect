using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.ModelLog.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.Cliente;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.EtapaVenda;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.T5.Checklist;
using Aniel.Connect.Repository.Venda;
using Aniel.T5.Model.nhibernate;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNet.Identity;
using NHibernate;
using NHibernate.Criterion;
using OpenXmlPowerTools;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Windows.Forms;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class TrocaTitularidadeApiController : BaseApiController
    {
        [HttpPost]
        [Route("api/TrocaTitularidadeApi/GetAll")]
        public async Task<DataTableResponse<TrocaTitularidadeResponse>> GetAll(TrocaTitularidadeRequest request)
        {
            List<TrocaTitularidadeResponse> retorno = new List<TrocaTitularidadeResponse>();
            int count = 0;

            using (var session = HibernateUtil.GetSession())
            {
                Dictionary<string, object> parametros = new Dictionary<string, object>();

                try
                {
                    StringBuilder sqlFrom = new StringBuilder();

                    sqlFrom.AppendLine(" FROM TB_TROCA_TITULARIDADE A ");
                    sqlFrom.AppendLine(" LEFT JOIN TB_CLIENTE B ");
                    sqlFrom.AppendLine(" ON (B.ID = A.ID_CLIENTE_TITULAR_ATUAL) ");
                    sqlFrom.AppendLine(" LEFT JOIN TB_CLIENTE C ");
                    sqlFrom.AppendLine(" ON (C.ID = A.ID_CLIENTE_TITULAR_NOVO) ");
                    sqlFrom.AppendLine(" LEFT JOIN TB_ETAPA_VENDA D ");
                    sqlFrom.AppendLine(" ON (D.ID = A.ID_ETAPA_TROCA) ");
                    sqlFrom.AppendLine(" LEFT JOIN TB_CATEGORIA_CLIENTE E ");
                    sqlFrom.AppendLine(" ON (E.ID = B.ID_CATEGORIA) ");
                    sqlFrom.AppendLine(" LEFT JOIN TB_CATEGORIA_CLIENTE F ");
                    sqlFrom.AppendLine(" ON (F.ID = C.ID_CATEGORIA) ");
                    //sqlFrom.AppendLine(" WHERE A.NUMERO_CONTRATO IS NOT NULL AND (D.ID = (SELECT DISTINCT MIN(D2.ID) FROM TB_ETAPA_VENDA D2 WHERE D2.ID_TROCA_TITULARIDADE = A.ID AND D2.STATUS = 0) OR ");
                    //sqlFrom.AppendLine(" D.ID = (SELECT DISTINCT MAX(D3.ID) FROM TB_ETAPA_VENDA D3 WHERE D3.ID_TROCA_TITULARIDADE = A.ID AND D3.STATUS IN (0,1,2))) ");

                    sqlFrom.AppendLine(" WHERE A.NUMERO_CONTRATO IS NOT NULL ");

                    if (request.TipoPeriodo != null && request.TipoPeriodo != TipoPeriodo.Todos)
                    {
                        var dateInicial = Convert.ToDateTime(request.DataInicial).Date.ToString("yyyy-MM-dd 00:00:00");
                        var dateFim = Convert.ToDateTime(request.DataFinal).Date.ToString("yyyy-MM-dd 23:59:59");

                        if (request.TipoPeriodo == TipoPeriodo.DataCriacao)
                        {
                            sqlFrom.AppendLine(" AND A.DATAHORACRIACAO BETWEEN @DATAINICIO AND @DATAFIM ");
                        }
                        else if (request.TipoPeriodo == TipoPeriodo.DataCancelamento)
                        {
                            sqlFrom.AppendLine(" AND A.DATA_CANCELAMENTO BETWEEN @DATAINICIO AND @DATAFIM ");
                        }
                        else if (request.TipoPeriodo == TipoPeriodo.DataAprovacao)
                        {
                            sqlFrom.AppendLine(" AND A.DATA_APROVACAO BETWEEN @DATAINICIO AND @DATAFIM ");
                        }

                        parametros.Add("@DATAINICIO", dateInicial);
                        parametros.Add("@DATAFIM", dateFim);
                    }

                    if (request.StatusTrocaTitularidade != null && request.StatusTrocaTitularidade != StatusTrocaTitularidadeModels.Todos)
                    {
                        var status = request.StatusTrocaTitularidade;
                        sqlFrom.AppendLine(" AND A.STATUS_TROCA = @STATUS_TROCA");
                        parametros.Add("@STATUS_TROCA", (long?)status);
                    }

                    if (request.TipoPessoaAtual != null && request.TipoPessoaAtual != TipoPessoaTitular.Todos)
                    {
                        sqlFrom.AppendLine(" AND B.TIPOPESSOA = @PESSOATUAL ");
                        parametros.Add("@PESSOATUAL", (long?)request.TipoPessoaAtual);
                    }

                    if (request.TipoPessoaNovo != null && request.TipoPessoaNovo != TipoPessoaTitular.Todos)
                    {
                        sqlFrom.AppendLine(" AND C.TIPOPESSOA = @PESSOANOVO ");
                        parametros.Add("@PESSOANOVO", (long?)request.TipoPessoaNovo);
                    }

                    if (request.EtapaAtual != null)
                    {
                        sqlFrom.AppendLine(" AND D.ETAPA = @ETAPA");
                        parametros.Add("@ETAPA", (long?)request.EtapaAtual);
                    }

                    if (!string.IsNullOrEmpty(request.TermoAssinado))
                    {
                        bool termoAssinado = request.TermoAssinado == "1" ? true : false;
                        sqlFrom.AppendLine(" AND A.TERMO_ADESAO_ASSINADO = @TERMOASSINADO ");
                        parametros.Add("@TERMOASSINADO", termoAssinado);
                    }
                    
                    if (!request.PossuiAcesso)
                    {
                        sqlFrom.AppendLine(" AND A.USUARIOCRIACAO = @USER ");
                        parametros.Add("@USER", User.Identity.GetUserId());
                    }

                    if (!string.IsNullOrEmpty(request.ChecklistPreenchido))
                    {
                        bool checklistPreenchido = request.ChecklistPreenchido == "1" ? true : false;
                        sqlFrom.AppendLine(" AND A.CHECKLIST_ASSINADO = @CHECKLISTASS");
                        parametros.Add("@CHECKLISTASS", checklistPreenchido);
                    }

                    if (!string.IsNullOrEmpty(request.search.value))
                    {
                        sqlFrom.AppendLine(" AND (B.CPF = @DOC OR C.CPF = @DOC) ");
                        parametros.Add("@DOC", request.search.value);
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine(" SELECT COUNT(A.ID) AS CO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                count = reader["CO"] != DBNull.Value ? Convert.ToInt32(reader["CO"]) : 0;
                            }
                        }
                    }

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.AppendLine(" SELECT A.ID, A.NUMERO_CONTRATO, A.USUARIOCRIACAO, A.DATAHORACRIACAO, A.TERMO_ADESAO_ASSINADO, A.STATUS_TROCA, ");
                        sql.AppendLine(" A.DATA_APROVACAO, A.USUARIOAPROVACAO, A.DATA_CANCELAMENTO, A.USUARIOCANCELAMENTO, A.CHECKLIST_ASSINADO, ");
                        sql.AppendLine(" B.NOMEFANTASIA AS CLI_ATUAL, B.TIPOPESSOA AS TIPO_PESSOA_ATUAL, B.CPF AS CPF_ATUAL, ");
                        sql.AppendLine(" C.NOMEFANTASIA AS CLI_NOVO, C.TIPOPESSOA AS TIPO_PESSOA_NOVO, ");
                        sql.AppendLine(" C.CPF AS CPF_NOVO, D.ETAPA, D.DESCRICAO, E.DESCRICAO AS CATEGORIA_ATUAL,  ");
                        sql.AppendLine(" F.DESCRICAO AS CATEGORIA_NOVO ");
                        sql.AppendLine(sqlFrom.ToString());

                        sql.AppendLine(" GROUP BY  A.ID, A.NUMERO_CONTRATO, A.USUARIOCRIACAO, A.DATAHORACRIACAO, A.TERMO_ADESAO_ASSINADO, A.STATUS_TROCA, ");
                        sql.AppendLine(" A.DATA_APROVACAO, A.USUARIOAPROVACAO, A.DATA_CANCELAMENTO, A.USUARIOCANCELAMENTO, A.CHECKLIST_ASSINADO, ");
                        sql.AppendLine(" B.NOMEFANTASIA, B.TIPOPESSOA, B.CPF, ");
                        sql.AppendLine(" C.NOMEFANTASIA , C.TIPOPESSOA , ");
                        sql.AppendLine(" C.CPF, D.ETAPA, D.DESCRICAO, E.DESCRICAO, ");
                        sql.AppendLine(" F.DESCRICAO ");

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            while (await reader.ReadAsync())
                            {
                                retorno.Add(new TrocaTitularidadeResponse()
                                {
                                    Id = reader["ID"] != DBNull.Value ? Convert.ToInt64(reader["ID"]) : 0,
                                    Numero = reader["NUMERO_CONTRATO"] != DBNull.Value ? Convert.ToInt64(reader["NUMERO_CONTRATO"]).ToString() : "-",
                                    StatusTroca = reader["STATUS_TROCA"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<StatusTrocaTitularidade>(Convert.ToString(reader["STATUS_TROCA"])) : "-",
                                    TipoPessoaTitularAtual = reader["TIPO_PESSOA_ATUAL"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Tipo_Pessoa>(Convert.ToString(reader["TIPO_PESSOA_ATUAL"])) : "-",
                                    NomeFantasiaTitularAtual = reader["CLI_ATUAL"] != DBNull.Value ? Convert.ToString(reader["CLI_ATUAL"]) : "-",
                                    DocumentoTitularAtual = reader["CPF_ATUAL"] != DBNull.Value ? Convert.ToString(reader["CPF_ATUAL"]) : "-",
                                    TipoPessoaTitularNovo = reader["TIPO_PESSOA_NOVO"] != DBNull.Value ? HtmlEnumExtensions.GetEnumDescription<Tipo_Pessoa>(Convert.ToString(reader["TIPO_PESSOA_NOVO"])) : "-",
                                    DocumentoTitularNovo = reader["CPF_NOVO"] != DBNull.Value ? Convert.ToString(reader["CPF_NOVO"]) : "-",
                                    CategoriaTitularNovo = reader["CATEGORIA_NOVO"] != DBNull.Value ? Convert.ToString(reader["CATEGORIA_NOVO"]) : "-",
                                    UsuarioCriacao = reader["USUARIOCRIACAO"] != DBNull.Value ? Convert.ToString(reader["USUARIOCRIACAO"]) : "-",
                                    DataHoraCriacao = reader["DATAHORACRIACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATAHORACRIACAO"]).ToString("dd/MM/yyyy HH:mm:ss") : "-",
                                    EtapaAtual = reader["DESCRICAO"] != DBNull.Value ? Convert.ToString(reader["DESCRICAO"]) : "-",
                                    NomeFantasiaTitularNovo = reader["CLI_NOVO"] != DBNull.Value ? Convert.ToString(reader["CLI_NOVO"]) : "-",
                                    UsuarioAprovacao = reader["USUARIOAPROVACAO"] != DBNull.Value ? Convert.ToString(reader["USUARIOAPROVACAO"]) : "-",
                                    UsuarioCancelamento = reader["USUARIOCANCELAMENTO"] != DBNull.Value ? Convert.ToString(reader["USUARIOCANCELAMENTO"]) : "-",
                                    DataAprovacao = reader["DATA_APROVACAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_APROVACAO"]).ToString("dd/MM/yyyy HH:mm:ss") : "-",
                                    DataCancelamento = reader["DATA_CANCELAMENTO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_CANCELAMENTO"]).ToString("dd/MM/yyyy HH:mm:ss") : "-",
                                    //EtapaAtual = reader["ID"] != null ? HtmlEnumExtensions.GetEnumDescription<EnumEtapaVenda>(business.VerificaUltimaEtapaTroca(Convert.ToInt64(reader["ID"])).Etapa) : "-",
                                    ChecklistPreenchido = reader["CHECKLIST_ASSINADO"] != DBNull.Value ? (Convert.ToBoolean(reader["CHECKLIST_ASSINADO"]) == true ? "Sim" : "Não") : "-",
                                    TermoTitularNovo = reader["TERMO_ADESAO_ASSINADO"] != DBNull.Value ? (Convert.ToBoolean(reader["TERMO_ADESAO_ASSINADO"]) == true ? "Sim" : "Não") : "Não",
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                return new DataTableResponse<TrocaTitularidadeResponse>
                {
                    draw = request.draw,
                    recordsFiltered = count,
                    data = retorno,
                    error = ""
                };
            }
        }

        [HttpPost]
        [Route("api/TrocaTitularidadeApi/FindById")]
        public async Task<PostApiRetorno<CadastroTrocaTitularidadeModel>> FindById(long? idTrocaTitularidade, long? idClienteAtual, long? numeroContrato)
        {
            var response = new PostApiRetorno<CadastroTrocaTitularidadeModel>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoCliente = new Conexao<Cliente>(session))
                using (var repoTrocaTitularidade = new Conexao<TrocaTitularidade>(session))
                {
                    TrocaTitularidade trocaTitularidade = null;
                    Cliente clienteAtual = null;
                    Cliente clienteNovo = null;

                    if (idTrocaTitularidade.HasValue && idTrocaTitularidade != 0)
                    {
                        trocaTitularidade = await repoTrocaTitularidade.selectFirst(p => p.Id == idTrocaTitularidade.Value);
                        clienteAtual = await repoCliente.selectFirst(p => p.Id == trocaTitularidade.ClienteTitularAtual.Id);
                        clienteNovo = await repoCliente.selectFirst(p => p.Id == trocaTitularidade.ClienteTitularNovo.Id);
                        numeroContrato = trocaTitularidade.NumeroContrato;
                    }
                    else
                    {
                        trocaTitularidade = new TrocaTitularidade()
                        {
                            StatusTrocaTitularidade = StatusTrocaTitularidade.Analise_Cliente,
                            ClienteTitularAtual = await repoCliente.selectFirst(p => p.Id == idClienteAtual),
                            NumeroContrato = numeroContrato.Value
                        };

                        clienteAtual = trocaTitularidade.ClienteTitularAtual;
                        clienteNovo = new Cliente();
                    }

                    response.IdObjeto = await GetViewObj(trocaTitularidade, clienteAtual, clienteNovo, numeroContrato.Value, session);
                    response.Ok = true;
                }
            }
            catch (Exception ex)
            {
                response.IdObjeto = null;
                response.Ok = false;
                response.Mensagem = "Erro ao Obter informações da Troca por Id";
            }

            return response;
        }


        [HttpPost()]
        [Route("api/TrocaTitularidadeApi/Gravar")]
        public async Task<PostApiRetorno<Int64>> Gravar([FromBody] ParamWebApi<CadastroTrocaTitularidadeModel> value)
        {
            var response = new PostApiRetorno<Int64>();

            try
            {
                TrocaTitularidade trocaTitularidade = null;
                Cliente clienteNovo = null;

                using (var session = HibernateUtil.GetSession())
                using (var repoTrocaTitularidade = new Conexao<TrocaTitularidade>(session))
                using (var repoCliente = new Conexao<Cliente>(session))
                {

                    if (value.Objeto.Id != 0)
                        trocaTitularidade = (await repoTrocaTitularidade.select(p => p.Id == value.Objeto.Id)).FirstOrDefault();

                    if (trocaTitularidade == null)
                    {
                        trocaTitularidade = new TrocaTitularidade
                        {
                            ClienteTitularAtual = await BindClienteToBanco(await repoCliente.selectFirst(x => x.Id ==
                                                                           value.Objeto.ClienteAtual.Id_Cliente),
                                                                           value.Objeto.ClienteAtual,
                                                                           value.Usuario),
                            NumeroContrato = value.Objeto.NumeroContrato,
                            UsuarioCriacao = value.Usuario,
                            StatusTrocaTitularidade = StatusTrocaTitularidade.Analise_Cliente
                        };



                        trocaTitularidade.ClienteTitularNovo = await BindClienteToBanco(value.Objeto.ClienteNovo.Id_Cliente != 0 ?
                                                                                        await repoCliente.selectFirst(x => x.Id == value.Objeto.ClienteNovo.Id_Cliente) :
                                                                                        new Cliente { Id = 0 },
                                                                                        value.Objeto.ClienteNovo,
                                                                                        value.Usuario);

                        clienteNovo = trocaTitularidade.ClienteTitularNovo;

                        await repoTrocaTitularidade.SaveOrUpdate(trocaTitularidade);
                    }
                    else
                    {
                        await BindClienteToBanco(await repoCliente.selectFirst(x => x.Id == value.Objeto.ClienteAtual.Id_Cliente),
                                                                               value.Objeto.ClienteAtual,
                                                                               value.Usuario);

                        clienteNovo = await BindClienteToBanco(trocaTitularidade.ClienteTitularNovo,
                                                                value.Objeto.ClienteNovo,
                                                                value.Usuario);

                        trocaTitularidade.UsuarioAlteracao = value.Usuario;
                        trocaTitularidade.ClienteTitularNovo = clienteNovo;

                        await repoTrocaTitularidade.Update(trocaTitularidade);
                    }
               
                    GravarEtapaTrocaTitularidade(session, trocaTitularidade, value.Objeto, value.Usuario);

                    response.Ok = true;
                    response.IdObjeto = trocaTitularidade.Id;
                }

                bool possuiPendeciaEnvioTermo = await VerificaEnvioTermo(response);

                if (!possuiPendeciaEnvioTermo)
                {
                    using (var termApi = new Termo_AdesaoApiController())
                    {
                        await termApi.EnviarTermoTrocaTitulardade(response.IdObjeto);
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return response;
        }



        [HttpPost()]
        [Route("api/TrocaTitularidadeApi/RealizarTrocaTitularidade")]
        public async Task<PostApiRetorno<Boolean>> RealizarTrocaTitularidade(RealizarTrocaTitularidade request)
        {
            PostApiRetorno<Boolean> response = new PostApiRetorno<bool>();

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionLog = HibernateUtil.GetSessionLog())
            using (var repoCliente = new Conexao<Cliente>(session))
            using (var repoCliPlanoLog = new Conexao<Log_ClientePlanoProduto>(sessionLog))
            using (var repoCliContratoLog = new Conexao<Log_ClienteContrato>(sessionLog))
            using (var repoTrocaTitular = new Conexao<TrocaTitularidade>(session))
            using (var repoClienteCtt = new Conexao<ClienteContrato>(session))
            using (var repoClientePlano = new Conexao<Cliente_PlanoProduto>(session))
            using (var repoFormaPagamento = new Conexao<Forma_Pagamento>(session))
            using (var repoFormaPgtoCli = new Conexao<Forma_Pagamento_Cliente>(session))
            using (var repoVencimento = new Conexao<Vencimento>(session))
            using (var repoPais = new Conexao<Pais>(session))
            using (var repoLoginRadius = new Conexao<LoginRadius>(session))
            {
                try 
                { 
                    List<Log_ClienteContrato> logsContrato = new List<Log_ClienteContrato>();
                    List<Log_ClienteContrato> logsClientePlano = new List<Log_ClienteContrato>();
                    List<Forma_Pagamento> lsFormasValidas = new List<Forma_Pagamento>();
                    List<Forma_Pagamento_Cliente> lsNovasFormasCli = new List<Forma_Pagamento_Cliente>();

                    var trocaTitular = await repoTrocaTitular.selectFirst(x => x.Id == request.IdTroca);

                    if (trocaTitular is null)
                        throw new Exception("Troca de Titularidade não encontrada");

                    Cliente titularNovo = await repoCliente.selectFirst(c => c.Id == trocaTitular.ClienteTitularNovo.Id);

                    if (titularNovo is null)
                        throw new Exception("Novo titular não foi encontrado");

                    if (request.LsFormaPagamentoCliModels == null)
                        throw new Exception("Forma de Pagamento não informada para realizar a troca");
                    else
                    {                       
                        foreach (var item in request.LsFormaPagamentoCliModels)
                        {
                            var formaPagmento = await repoFormaPagamento.selectFirst(x => x.Id == item.Id_Forma_Pagamento);

                            if (formaPagmento is null)
                                continue;

                            if (item.Tipo_Forma_Pagamento == Tipo_Forma_Pagamento.Cartao_Credito && item.Id != 0||
                                item.Tipo_Forma_Pagamento == Tipo_Forma_Pagamento.Debito_Conta && item.Id != 0)
                                continue;

                            int valorPadrao = 0;

                            Forma_Pagamento_Cliente formPagCliNew = new Forma_Pagamento_Cliente()
                             {
                                 Cliente = titularNovo,
                                 Forma_Pagamento = formaPagmento,
                                 Banco = formaPagmento?.Banco,
                                 Agencia = item?.Agencia,
                                 Conta_Corrente = item?.Conta_Corrente,
                                 Digito = !string.IsNullOrEmpty(item?.Digito) ? StringService.Truncate(item?.Digito, 1) : string.Empty,
                                 Vencimento = await repoVencimento.selectFirst(x => x.Id == item.Id_Vencimento),
                                 Dia_Vencimento = item?.Dia_Vencimento,
                                 Forma_Envio = item?.Forma_Envio,
                                 Endereco_Cobranca = new Endereco_Cliente()
                                 {
                                     Logradouro = item.LogradouroCobranca,
                                     Numero = item.NumeroCobranca,
                                     Id_Bairro = item?.Id_BairroCobranca,
                                     Bairro = item?.BairroCobranca,
                                     Cep = item?.CEPCobranca,
                                     Cidade = item?.CidadeCobranca,
                                     Complemento = item?.ComplementoCobranca,
                                     Id_Cidade = item?.Id_CidadeCobranca,
                                     UF = item?.UFCobranca,
                                     Id_UF = !string.IsNullOrEmpty(item.UFCobranca) ? (int.TryParse(item.UFCobranca, out valorPadrao) ? Convert.ToInt64(item.UFCobranca) : (long?)null) : (long?)null,
                                     Pais = item?.Id_PaisCobranca != null ? await repoPais.selectFirst(x => x.Id == item.Id_PaisCobranca) : null,                                     
                                     Latitude = !string.IsNullOrEmpty(item?.LatitudeCobranca) ? Convert.ToDouble(item?.LatitudeCobranca) : (double?)null,
                                     Longitude = !string.IsNullOrEmpty(item?.LongitudeCobranca) ? Convert.ToDouble(item?.LongitudeCobranca) : (double?)null,
                                 },
                                 Status = Status_Default.Ativo,
                             };

                             lsNovasFormasCli.Add(formPagCliNew);                         
                        }
                    }

                    if (lsNovasFormasCli.Count > 0)
                        await repoFormaPgtoCli.SaveOrUpdate(lsNovasFormasCli);
                    else
                        throw new Exception("Cliente não tem formas de pagamento válidas");

                    ClienteContrato clienteContrato = await repoClienteCtt.selectFirst(c => c.Id == trocaTitular.NumeroContrato);
               
                    if (clienteContrato is null)
                        throw new Exception("Contrato do Cliente não foi encontrado");

                    var lsCliPlanos = await repoClientePlano.select(p => p.Cliente.Id == trocaTitular.ClienteTitularAtual.Id &&
                                                                   p.ClienteContrato == clienteContrato);
                    if (lsCliPlanos is null)
                        throw new Exception("Planos não encontrados para efetuar a troca de titularidade");

                    var cliContratoCopy = (ClienteContrato)clienteContrato.Clone();

                    clienteContrato.Cliente = titularNovo;
                    clienteContrato.Endereco_Cliente = titularNovo.Endereco_Instalacao ?? lsNovasFormasCli.FirstOrDefault().Endereco_Cobranca;
                    clienteContrato.DataHoraAlteracao = DateTime.Now;

                    var logsCtt = clienteContrato.GeraLog(cliContratoCopy, User.Identity.Name, User.Identity.Name, Sinapse.ModelLog.TipoOperLog.Alteracao);

                    await repoCliContratoLog.SaveOrUpdate(logsCtt);
                    await repoClienteCtt.SaveOrUpdate(clienteContrato);

                    foreach (var plan in lsCliPlanos)
                    {
                        var cliPlanProdCopy = (Cliente_PlanoProduto)plan.Clone();

                        var formPag = lsNovasFormasCli.Count > 1 ? lsNovasFormasCli.FirstOrDefault(x => x.Forma_Pagamento ==
                                                                   plan.Forma_Pagamento_Cliente.Forma_Pagamento) :
                                                                   lsNovasFormasCli.FirstOrDefault();

                        plan.Cliente = titularNovo;
                        plan.ClienteAnteriorTroca = trocaTitular.ClienteTitularAtual;
                        plan.Forma_Pagamento_Cliente = formPag;


                        var lsLoginsRadius = await repoLoginRadius.select(l => l.Cliente.Id == trocaTitular.ClienteTitularAtual.Id &&
                                                                          l.Cliente_PlanoProduto == plan);

                        if (lsLoginsRadius.Count() > 0)
                        {
                            foreach (var login in lsLoginsRadius)
                            {
                                login.Cliente = titularNovo;
                                login.DataHoraAlteracao = DateTime.Now;
                                login.UsuarioAlteracao = User.Identity.Name;
                            }

                            await repoLoginRadius.SaveOrUpdate(lsLoginsRadius);
                        }

                        var logs = plan.GeraLog(cliPlanProdCopy, User.Identity.Name, User.Identity.Name, Sinapse.ModelLog.TipoOperLog.Alteracao);

                        await repoCliPlanoLog.SaveOrUpdate(logs);
                        await repoClientePlano.SaveOrUpdate(plan);
                    }

                    trocaTitular.StatusTrocaTitularidade = StatusTrocaTitularidade.Aprovado;
                    trocaTitular.DataAprovacao = DateTime.Now;
                    trocaTitular.UsuarioAprovacao = User.Identity.GetUserId();
                    trocaTitular.UsuarioAlteracao = User.Identity.GetUserId();

                    EtapaVendaViewModel etapaAtual = request.EtapasTroca.Where(e => e.Etapa == request.EtapaAtualTrocaTitularidade).FirstOrDefault();
                    if (etapaAtual.Id == trocaTitular.Etapa_Atual.Id)
                    {
                        trocaTitular.Etapa_Atual.Status = StatusEtapaVenda.Aprovado;
                    }

                    await repoTrocaTitular.SaveOrUpdate(trocaTitular);
                }
                catch (Exception ex)
                {
                    response.Mensagem = ex.Message;
                    response.Ok = false;
                    return response;
                }

                response.Ok = true;
                response.Mensagem = "Troca de Titularidade realizada com sucesso!";
            }
            return response;
        }


        [HttpPost()]
        [Route("api/TrocaTitularidadeApi/ConfirmarPreenchimentoChecklist")]
        public async Task<PostApiRetorno<Boolean>> ConfirmarPreenchimentoChecklist(Boolean preenchido, long IdTroca)
        {
            PostApiRetorno<Boolean> response = new PostApiRetorno<bool>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoTrocaTitular = new Conexao<TrocaTitularidade>(session))
            {
                try
                {                
                    var trocaTitular = await repoTrocaTitular.selectFirst(x => x.Id == IdTroca);

                    if (trocaTitular is null)
                        throw new Exception("Troca de Titularidade não encontrada");

                    trocaTitular.DataAssinaturaChecklist = DateTime.Now;
                    trocaTitular.ChecklistAssinado = preenchido;
               
                    await repoTrocaTitular.Update(trocaTitular);
                }
                catch (Exception ex)
                {
                    response.Mensagem = ex.Message;
                    response.Ok = false;
                    return response;
                }

                response.Ok = true;
                response.Mensagem = "Alterações gravadas com sucesso!";
            }
            return response;
        }


        [HttpPost()]
        [Route("api/TrocaTitularidadeApi/Cancelar")]
        public async Task<PostApiRetorno<List<CancelarTrocaResponse>>> CancelarTrocaTitularidade(CancelarTroca viewModel)
        {
            var response = new PostApiRetorno<List<CancelarTrocaResponse>>() { IdObjeto = new List<CancelarTrocaResponse>() };

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var trocaRepo = new Conexao<TrocaTitularidade>(session))
                {
                    if (viewModel.ids.Count == 0)
                    {
                        response.Ok = false;
                        response.IdObjeto.Add(new CancelarTrocaResponse
                        {
                            Id = 0,
                            StatusTroca = "-",
                            Motivo = "Informe os IDs que deseja cancelar.",
                            TermoAdesao = "-"
                        });
                        return response;
                    }

                    foreach (var id in viewModel.ids)
                    {
                        if (id == 0)
                            continue;

                        var trocaTitular = await trocaRepo.selectFirst(t => t.Id == id);

                        if (trocaTitular == null)
                        {
                            response.IdObjeto.Add(new CancelarTrocaResponse
                            {
                                Id = id,
                                StatusTroca = trocaTitular.StatusTrocaTitularidade.ToString(),
                                Motivo = $"Não foi encontrado uma " +
                                $"troca cadastrada com o ID informado {id}.",
                                TermoAdesao = trocaTitular.TermoAdesaoAssinado ? "Sim" : "Não"
                            });
                            continue;
                        }

                        if (trocaTitular.StatusTrocaTitularidade == StatusTrocaTitularidade.Cancelado)
                        {
                            response.IdObjeto.Add(new CancelarTrocaResponse
                            {
                                Id = id,
                                StatusTroca = trocaTitular.StatusTrocaTitularidade.ToString(),
                                Motivo = $"Não foi possível cancelar a Troca de Titularidade {id} " +
                                $"pois já está cancelada.",
                                TermoAdesao = trocaTitular.TermoAdesaoAssinado ? "Sim" : "Não"
                            });
                            continue;
                        }

                        if (trocaTitular.StatusTrocaTitularidade == StatusTrocaTitularidade.Aprovado)
                        {
                            response.IdObjeto.Add(new CancelarTrocaResponse
                            {
                                Id = id,
                                StatusTroca = trocaTitular.StatusTrocaTitularidade.ToString(),
                                Motivo = $"Não foi possível cancelar a Troca de Titularidade {id} " +
                                $"pois já está aprovada.",
                                TermoAdesao = trocaTitular.TermoAdesaoAssinado ? "Sim" : "Não"
                            });
                            continue;
                        }

                        if (trocaTitular.TermoAdesaoAssinado)
                        {
                            response.IdObjeto.Add(new CancelarTrocaResponse
                            {
                                Id = id,
                                StatusTroca = trocaTitular.StatusTrocaTitularidade.ToString(),
                                Motivo = $"Não foi possível cancelar a Troca de Titularidade {id} " +
                                $"pois o termo de adesão já está assinado.",
                                TermoAdesao = trocaTitular.TermoAdesaoAssinado ? "Sim" : "Não"
                            });
                            continue;
                        }

                        trocaTitular.StatusTrocaTitularidade = StatusTrocaTitularidade.Cancelado;
                        trocaTitular.DataHoraAlteracao = DateTime.Now;
                        trocaTitular.DataCancelamento = DateTime.Now;
                        trocaTitular.UsuarioAlteracao = User.Identity.GetUserName();
                        trocaTitular.UsuarioCancelamento = User.Identity.GetUserName();

                        await trocaRepo.SaveOrUpdate(trocaTitular);
                    }
                }
            }
            catch (Exception ex)
            {
                response.IdObjeto.Add(new CancelarTrocaResponse
                {
                    Id = 0,
                    StatusTroca = "-",
                    Motivo = $"{ex.Message}",
                    TermoAdesao = "-"
                });
                response.Ok = false;
                response.InnerException = ex.InnerException.Message;
                return response;
            }

            if (response.IdObjeto.Count == 0)
            {
                response.Ok = true;
                response.Mensagem = "Trocas de Titularidade canceladas com sucesso!";
            }

            return response;
        }

        [HttpPost()]
        [Route("api/TrocaTitularidadeApi/AssociarChecklistTroca")]
        public GetApiRetorno AssociarChecklistTroca(AssociaChecklistTrocaRequest request)
        {
            var retorno = new GetApiRetorno();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                using (var trocaRepo = new Conexao<TrocaTitularidade>(session))
                {
                    IChecklistRepository repository = new ChecklistRepository(sessionT5Nh, sessionT5);
                    IEtapaVendaRepository repositoryEtapa = new EtapaVendaRepository(session);
                    IVendaRepository repositoryVenda = new VendaRepository(session);
                    IClienteRepository repositoryCliente = new ClienteRepository(session);

                    TrocaTitularidade troca = trocaRepo.selectFirst(x => x.Id == request.IdTroca).Result;

                    if (troca != null)
                    {
                        Cliente cliente = repositoryCliente.GetById(troca.ClienteTitularNovo.Id);
                        var business = new ChecklistBusiness(repository, repositoryEtapa);
                        business.AssociarChecklistTroca(troca.Id, cliente.Tipo_Pessoa.Value, request.IdChecklist,
                                                        cliente?.Categoria_Cliente?.Id, cliente?.Qual_Meio?.Id,
                                                        cliente?.Endereco_Instalacao?.Id_UF,
                                                        cliente?.Endereco_Instalacao?.Id_Cidade);

                        Quiz checklist = business.GetChecklistTrocaTitularidade(request.IdTroca);
                        if (checklist == null)
                        {
                            throw new Exception("Sem Checklist possível para ser associado a Troca de Titularidade.");
                        }
                    }
                }
                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
            }

            return retorno;
        }


        [HttpGet()]
        [Route("api/TrocaTitularidadeApi/EnviarEmailChecklistTroca")]
        public async Task<PostApiRetorno<long>> EnviarEmailChecklistTroca(long IdTroca)
        {
            var retorno = new PostApiRetorno<long>();

            using (ISession session = HibernateUtil.GetSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {
                    TrocaTitularidade troca = null;
                    Cliente clienteAtual = null;
                    Cliente clienteNovo = null;

                    using (var trocaRepo = new Conexao<TrocaTitularidade>(session))
                    using (var vendaRepo = new Conexao<Venda>(session))
                    using (var repoCliente = new Conexao<Cliente>(session))
                    using (var repoCanal = new Conexao<Canal_Comunicacao>(session))
                    using (var repoConfCanal = new Conexao<Configuracao_Canal_Comunicacao>(session))
                    using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
                    using (var vendaApiController = new VendaApiController())
                    {
                        troca = await trocaRepo.selectFirst(p => p.Id == IdTroca);
                        clienteNovo = await repoCliente.selectFirst(p => p.Id == troca.ClienteTitularNovo.Id);
                        clienteAtual = await repoCliente.selectFirst(p => p.Id == troca.ClienteTitularAtual.Id);

                        var listaPlanosProdutos = clienteAtual.PlanosProdutos.Where(p => p.Cliente.Id == clienteAtual.Id &&
                                                                                    p.ClienteContrato.Id == troca.NumeroContrato);

                        //utilizado  para pegar informacoes da empresa
                        var vendaFirst = await vendaRepo.selectFirst();

                        if (String.IsNullOrEmpty(clienteNovo.Email_Principal))
                            throw new Exception("Cliente não possui e-mail vinculado");

                        Configuracoes_Gerais config = await repoConfig.selectFirst();

                        if (vendaFirst?.Vendedor?.Filial?.Empresa?.CodEmpresa == null)
                            throw new Exception("Não foi possível encontrar os dados da empresa para inserir no e-mail do checklist");

                        Models.InfoEmpresaTermoAdesao infoEmpresa = vendaApiController.GetInfoEmpresa(Convert.ToString(vendaFirst.Vendedor.Filial.Empresa.CodEmpresa));

                        if (infoEmpresa == null)
                            throw new Exception("Não foi possível encontrar os dados da empresa para inserir no e-mail do checklist");

                        EmailUtil emailService = new EmailUtil(CONSTANTES.Email_De, CONSTANTES.Email_Senha, CONSTANTES.Email_Smtp, CONSTANTES.Email_Porta, CONSTANTES.Email_Usuario);
                        String urlChecklistTroca = $@"{CONSTANTES.URLPortal}/pt/TrocaTitularidade/EmailChecklistTroca?idTroca={troca.Id}";
                        //
                        var produtos = "";

                        foreach (var planoProduto in listaPlanosProdutos)
                        {
                            if (planoProduto.Combo != null)
                            {
                                foreach (var planoProdutoCombo in planoProduto.Combo.PlanosProdutos)
                                {
                                    using (var stream_produtos = System.IO.File.OpenText($@"{CONSTANTES.CaminhoFisico}\Arquivos\email_checklist_venda_produtos.html"))
                                    {
                                        String produto = stream_produtos.ReadToEnd();
                                        var dias = planoProduto.Dias_Teste_Gratis != null ? planoProduto.Dias_Teste_Gratis : 0;

                                        produto = produto.Replace("{{DESC}}", planoProdutoCombo.Plano != null ? planoProdutoCombo.Plano.Descricao : planoProdutoCombo.Produto.Descricao)
                                             .Replace("{{VENCIMENTO}}", planoProduto.Forma_Pagamento_Cliente?.Dia_Vencimento != null ? "Dia " + planoProduto.Forma_Pagamento_Cliente?.Dia_Vencimento : "")
                                             .Replace("{{TESTE}}", dias == 1 ? dias + "dia" : dias + " dias")
                                             .Replace("{{VALOR}}", planoProdutoCombo != null ? planoProdutoCombo.Valor.ToString("C", new CultureInfo("pt-BR")) : "R$ 0,00");

                                        produtos += produto;
                                    }
                                }
                            }
                            else
                            {
                                using (var stream_produtos = System.IO.File.OpenText($@"{CONSTANTES.CaminhoFisico}\Arquivos\email_checklist_venda_produtos.html"))
                                {
                                    String produto = stream_produtos.ReadToEnd();
                                    var dias = planoProduto.Dias_Teste_Gratis != null ? planoProduto.Dias_Teste_Gratis : 0;

                                    produto = produto.Replace("{{DESC}}", planoProduto.Plano != null ? planoProduto.Plano.Descricao : planoProduto.Produto.Descricao)
                                         .Replace("{{VENCIMENTO}}", planoProduto.Forma_Pagamento_Cliente?.Dia_Vencimento != null ? "Dia " + planoProduto.Forma_Pagamento_Cliente?.Dia_Vencimento : "")
                                         .Replace("{{TESTE}}", dias == 1 ? dias + "dia" : dias + " dias")
                                         .Replace("{{VALOR}}", planoProduto.Valor_Final != null ? planoProduto.Valor_Final?.ToString("C", new CultureInfo("pt-BR")) : "R$ 0,00");

                                    produtos += produto;
                                }
                            }
                        }

                        using var stream = System.IO.File.OpenText($@"{CONSTANTES.CaminhoFisico}\Arquivos\email_checklist_venda.html");
                        String arquivo_email = stream.ReadToEnd();
                        String arquivo_modificado = arquivo_email.Replace("{{NOME_CLIENTE}}", clienteNovo.NomeFantasia)
                                                                 .Replace("{{URL_CHECKLIST_VENDA}}", urlChecklistTroca)
                                                                 .Replace("{{URL_LOGO}}", config.OLA_URL_Logo)
                                                                 .Replace("{{PRODUTOS}}", produtos);

                        emailService.EnviarMensagem(clienteNovo.Email_Principal, $@"Bem-vindo(a) à {infoEmpresa.Nome}! Segue Checklist de documentos para validação.", arquivo_modificado, CONSTANTES.Email_UsaSsl == "S");
                    }

                    retorno.IdObjeto = troca.Id;
                    retorno.Ok = true;

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    CONSTANTES.log4Net.Error(ex.Message, ex);

                    transaction.Rollback();

                    retorno.IdObjeto = 0;

                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;

                    if (ex.InnerException != null)
                        retorno.InnerException = ex.InnerException.Message;
                }
            }

            return retorno;
        }

        public async Task<bool> VerificaEnvioTermo(PostApiRetorno<long> retorno)
        {
            List<bool> pendencias = new List<bool>();

            using (ISession session = HibernateUtil.GetSession())
            using (var repoEtapa = new Conexao<EtapaVenda>(session))
            using (var repoTrocaTitularidade = new Conexao<TrocaTitularidade>(session))
            {
                IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(session);
                var configBusiness = new ConfiguracoesGeraisBusiness(repoConfig);

                Configuracoes_Gerais config = configBusiness.GetConfiguracoesGerais();

                bool possuiEtapaPendenteOuReprovada =
                    (await repoEtapa.selectFirst(p => p.TrocaTitularidade.Id == retorno.IdObjeto &&
                                                      (p.Status == StatusEtapaVenda.Reprovado || p.Status == StatusEtapaVenda.Pendente) &&
                                                      ((int)p.Etapa < (int)EnumEtapaVenda.TermoAdesao))) != null;

                bool termoJaAssinado = (await repoTrocaTitularidade.selectFirst(p => p.Id == retorno.IdObjeto)).TermoAdesaoAssinado;

                return possuiEtapaPendenteOuReprovada || termoJaAssinado || !config.EnviarTermoAdesaoAuto;
            }
        }


        [HttpPost()]
        [Route("api/TrocaTitularidadeApi/VerificaExisteChecklist")]
        public async Task<PostApiRetorno<Int64>> VerificaExisteChecklist(Int64 IdTroca)
        {
            var response = new PostApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                using (var trocaRepo = new Conexao<TrocaTitularidade>(session))
                {
                    IChecklistRepository repository = new ChecklistRepository(sessionT5Nh, sessionT5);
                    IEtapaVendaRepository repositoryEtapa = new EtapaVendaRepository(session);
                    IVendaRepository repositoryVenda = new VendaRepository(session);
                    IClienteRepository repositoryCliente = new ClienteRepository(session);

                    TrocaTitularidade troca = trocaRepo.selectFirst(x => x.Id == IdTroca).Result;

                    if (troca != null)
                    {
                        Cliente cliente = repositoryCliente.GetById(troca.ClienteTitularNovo.Id);
                        var business = new ChecklistBusiness(repository, repositoryEtapa);

                        Quiz checklist = business.GetChecklistTrocaTitularidade(IdTroca);

                        if (checklist == null)
                        {
                            throw new Exception("Troca de Titularidade não possui Checklist.");
                        }
                    }
                }
                response.Ok = true;
            }
            catch (Exception ex)
            {
                response.Ok = false;
                response.Mensagem = ex.Message;
            }

            return response;
        }

        [HttpPost()]
        [Route("api/TrocaTitularidadeApi/ValidarCriacaoTroca")]
        public async Task<PostApiRetorno<Boolean>> ValidarCriacaoTroca(Int64 IdCliente, Int64 IdContrato)
        {
            PostApiRetorno<Boolean> response = new PostApiRetorno<bool>()
            {
                IdObjeto = true,
                Ok = true,
                Mensagem = "Cliente e Contrato válido para Troca de Titularidade!"
            };

            try
            {
                if (IdCliente == 0)
                    throw new Exception("Cliente não informado para validação!");

                if (IdContrato == 0)
                    throw new Exception("Contrato não informado para validação!");


                using (ISession session = HibernateUtil.GetSession())
                using (var repoCliCtt = new Conexao<ClienteContrato>(session))
                using (var repoCliente = new Conexao<Cliente>(session))
                using (var repoVenda = new Conexao<Venda>(session))
                using (var repoTrocaTitular = new Conexao<TrocaTitularidade>(session))
                using (var repoClientePlano = new Conexao<Cliente_PlanoProduto>(session))
                {
                    var cliente = await repoCliente.selectFirst(c => c.Id == IdCliente);

                    if (cliente is null)
                        throw new Exception("Cliente informado não foi encontrado para validação!");

                    var clienteContrato = await repoCliCtt.selectFirst(ct => ct.Id == IdContrato &&
                                                                       ct.Cliente.Id == IdCliente);

                    if (clienteContrato is null)
                        throw new Exception("Contrato informado não foi encontrado para validação!");

                    if (clienteContrato.Status_Contrato != Status_Contrato.Ativo)
                        throw new Exception("Contrato informado não está ativo!");

                    var planosProduto = await repoClientePlano.select(p => p.Cliente == cliente &&
                                                                      p.ClienteContrato == clienteContrato);

                    if (planosProduto is null)
                        throw new Exception("Não foi encontrado planos para o cliente e contrato informado!");

                    var temPlanoAtivo = planosProduto.Any(x => x.Status.HasValue &&
                                                          x.Status.Value == Status_PlanoProduto.Habilitado);

                    if (!temPlanoAtivo)
                        throw new Exception("Contrato informado não possuí planos ativos");

                    var temTrocaEnderecoPlano = planosProduto.Any(p => p.Endereco_Mudanca_Cliente != null);

                    if (temTrocaEnderecoPlano)
                        throw new Exception("Contrato informado possuí planos que estão realizando troca de Endereço.");

                    var temTrocaPlanoAtivo = planosProduto.Any(p => p.Id_TrocaPlano != null);

                    if (temTrocaPlanoAtivo)
                        throw new Exception("Contrato informado possuí troca de plano ativo.");

                    var temPlanoRealizandoTroca = await repoTrocaTitular.selectFirst(x => x.NumeroContrato == clienteContrato.Id &&
                                                                                     x.ClienteTitularAtual == cliente &&
                                                                                     x.StatusTrocaTitularidade != StatusTrocaTitularidade.Cancelado &&
                                                                                     x.StatusTrocaTitularidade != StatusTrocaTitularidade.Aprovado &&
                                                                                     x.StatusTrocaTitularidade != StatusTrocaTitularidade.Reprovado) != null;

                    if (temPlanoRealizandoTroca)
                        throw new Exception("Cliente e Contrato já possuem uma Troca de Titularidade em andamento.");


                    var temVendaAtiva = await repoVenda.selectFirst(v => v.Id_Cliente == IdCliente &&
                                                                    v.Status != Status_Venda.Venda_Aprovada &&
                                                                    v.Status != Status_Venda.Venda_Cancelada &&
                                                                    v.Status != Status_Venda.Venda_Reprovada) != null;

                    if (temVendaAtiva)
                        throw new Exception("Cliente selecionado possuí venda em aberto!");

                }
            }
            catch (Exception ex)
            {
                response.Ok = false;
                response.IdObjeto = false;
                response.Mensagem = ex.Message;
            }

            return response;
        }


        [HttpPost()]
        [Route("api/TrocaTitularidadeApi/ValidarRestricoesTroca")]
        public async Task<PostApiRetorno<Boolean>> ValidarRestricoesTroca(Int64 IdTroca)
        {
            PostApiRetorno<Boolean> response = new PostApiRetorno<bool>()
            {
                IdObjeto = true,
                Ok = true,
                Mensagem = "Troca de Titularidade válida para aprovação!"
            };

            try
            {              
                using (ISession session = HibernateUtil.GetSession())
                using (var repoTrocaTitular = new Conexao<TrocaTitularidade>(session))
                using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
                using (var repoEtapaVenda = new Conexao<EtapaVenda>(session))
                {
                    var config = await repoConfig.selectFirst();

                    if (config is null) throw new Exception("Configuração de Empresa não encontrada");

                    var trocaTitular = await repoTrocaTitular.selectFirst(x => x.Id == IdTroca);

                    if (trocaTitular is null) throw new Exception("Troca de Titularidade não encontrada para validação");

                    var etapasTroca = await repoEtapaVenda.select(x => x.TrocaTitularidade == trocaTitular);

                    var pendFinanParalisaProp = config.ConsultaPendenciaFinanceiraExibeVenda && (config.ConsultaPendenciaFinanceiraRestringeVenda);
                    var pendEndereParalisaProp  = config.ConsultaPendenciaEnderecoExibeVenda && (config.ConsultaPendenciaEnderecoRestringeVenda);
                    var pendCreditoParalisaProp = config.ConsultaCreditoExibeVenda && (config.ConsultaCreditoRestringeVenda);
                    var exigeTermoAdesao = config.Exige_Termo_Adesao;

                    if (pendFinanParalisaProp)
                    {
                        var etapas = etapasTroca.Where(x => x.Etapa == EnumEtapaVenda.ConsultaPendenciaFinanceira || 
                                                       x.Etapa == EnumEtapaVenda.ConsultaPendenciaFinanceiraNovo)
                                                .ToList();

                        foreach (var etapa in etapas)
                        {
                            if (etapa.Status == StatusEtapaVenda.Reprovado ||
                                etapa.Status == StatusEtapaVenda.Pendente)
                            {
                                throw new Exception("Não foi possível prosseguir com a aprovação da Troca: Etapa Pendências Financeira não está aprovada");
                            }
                        }
                    }
                    
                    if (pendEndereParalisaProp)
                    {
                        var etapas = etapasTroca.Where(x => x.Etapa == EnumEtapaVenda.ConsultaPendenciaEndereco).ToList();

                        foreach (var etapa in etapas)
                        {
                            if (etapa.Status == StatusEtapaVenda.Reprovado ||
                                etapa.Status == StatusEtapaVenda.Pendente)
                            {
                                throw new Exception("Não foi possível prosseguir com a aprovação da Troca: Etapa Pendências de Endereço não está aprovada");
                            }
                        }
                    }
                    
                    if (pendCreditoParalisaProp)
                    {
                        var etapas = etapasTroca.Where(x => x.Etapa == EnumEtapaVenda.ConsultaCredito).ToList();

                        foreach (var etapa in etapas)
                        {
                            if (etapa.Status == StatusEtapaVenda.Reprovado ||
                                etapa.Status == StatusEtapaVenda.Pendente)
                            {
                                throw new Exception("Não foi possível prosseguir com a aprovação da Troca: Etapa Consulta de Crédito não está aprovada");
                            }
                        }
                    }
                    
                    if (exigeTermoAdesao)
                    {
                        var etapas = etapasTroca.Where(x => x.Etapa == EnumEtapaVenda.TermoAdesao).ToList();

                        foreach (var etapa in etapas)
                        {
                            if (etapa.Status == StatusEtapaVenda.Reprovado ||
                                etapa.Status == StatusEtapaVenda.Pendente)
                            {
                                throw new Exception("Não foi possível prosseguir com a aprovação da Troca: Etapa Termo de Adesão não está aprovada");
                            }
                        }
                    }


                }
            }        
            catch (Exception ex)
            {
                response.Ok = true;
                response.IdObjeto = false;
                response.Mensagem = ex.Message;
            }

            return response;
        }


        [HttpPost()]
        [Route("api/TrocaTitularidadeApi/VincularTrocaAtendimento")]
        public async Task<PostApiRetorno<Boolean>> VincularTrocaAtendimento(VincularAtendimentoRequest request)
        {
            var response = new PostApiRetorno<Boolean>()
            {
                IdObjeto = true,
                Mensagem = "Vínculo realizado com sucesso!",
                Ok = true,
            };

            using (ISession session = HibernateUtil.GetSession())
            using (var repoAtendimento = new Conexao<Atendimento>(session))
            using (var repoTrocaTitular = new Conexao<TrocaTitularidade>(session))
            {
                try
                {
                    List<Atendimento> atendimentos = new List<Atendimento>();

                    if (request == null)
                        throw new Exception("Request vazio ou nulo");

                    if (request.IdsAtendimento == null || request.IdsAtendimento.Count == 0)
                        throw new Exception("Nenhum atendimento foi selecionado");

                    if (request.IdTroca == null)
                        throw new Exception("Não foi informado o ID da Troca de Titularidade");

                    TrocaTitularidade troca = await repoTrocaTitular.selectFirst(t => t.Id == request.IdTroca);

                    if (troca == null)
                        throw new Exception("Troca de Titularidade não foi encontrada");

                    atendimentos = (await repoAtendimento.select(a => a.Id.IsIn(request.IdsAtendimento))).ToList();

                    if (atendimentos == null || atendimentos.Count == 0)
                        throw new Exception("Atendimentos não foram encontrados");

                    foreach (var item in atendimentos)
                        item.TrocaTitularidade = troca;

                    await repoAtendimento.Update(atendimentos);
                }
                catch (Exception ex)
                {
                    response.Ok = false;
                    response.Mensagem = ex.Message;
                    response.IdObjeto = false;
                    return response;
                }
            }

            return response;
        }

        private async Task<ClienteCadastroTitularidade> BindClienteToCadastro(Cliente cliente,
                                                                            TrocaTitularidade trocaTitularidade,
                                                                            Configuracoes_Gerais config,
                                                                            ISession session)
        {
            ClienteCadastroTitularidade retorno = new ClienteCadastroTitularidade();

            if (cliente != null && cliente.Id != 0)
            {

                retorno.Id_Cliente = cliente.Id;
                retorno.Identificacao_Cliente = cliente?.Numero_Contrato;
                retorno.Cargo_Responsavel = cliente?.Cargo_Responsavel;
                retorno.CPFResponsavel = cliente?.Cpf_Responsavel;
                retorno.Representante = cliente?.Nome_Resp_Financeiro;
                retorno.Inscricao_Municipal = cliente?.Inscricao_Municipal;
                retorno.Inscricao_Estadual = cliente?.Inscricao_Estadual;
                retorno.Porte = cliente?.Porte;
                retorno.Data_Abertura = cliente?.Data_Abertura_Empresa;
                retorno.NomeFantasia = cliente?.NomeFantasia;
                retorno.Ramo_Atividade = cliente?.Ramo_Atividade;
                retorno.Razao_Social = cliente?.Razao_Social;
                retorno.CpfCnpj = cliente?.CpfCnpj;
                retorno.NomeCompleto = cliente.NomeFantasia;
                retorno.DataNascimento = Convert.ToString(cliente?.Data_Nascimento);
                retorno.Nacionalidade = cliente?.Nacionalidade;
                retorno.Sexo = cliente?.Sexo;
                retorno.EstadoCivil = cliente?.Estado_Civil;
                retorno.RG = cliente?.Rg;
                retorno.DataEmissao = Convert.ToString(cliente?.Data_Emissao_Rg);
                retorno.Orgao = cliente?.Orgao_Emissao_Rg;
                retorno.Instrucao = cliente?.Grau_Instrucao;
                retorno.Profissao = cliente?.Profissao;
                retorno.Pai = cliente?.Filiacao_Pai;
                retorno.Mae = cliente?.Filiacao_Mae;
                retorno.TipoPessoa = cliente?.Tipo_Pessoa;
                retorno.Email = cliente?.Email_Principal;
                retorno.TelefoneFixo = cliente?.Telefone_Fixo;
                retorno.TelefoneCelular = cliente?.Telefone_Celular;
                retorno.TelefoneRecado = cliente?.Telefone_Recado;
                retorno.NomeRecado = cliente?.Pessoa_Tel_Recado;
                retorno.Categoria = cliente?.Categoria_Cliente?.Descricao;
                retorno.Id_Categoria_Cliente = cliente?.Categoria_Cliente?.Id;
                retorno.EnderecoPrincipal = new EnderecoClienteViewModel
                {
                    Id_Pais = cliente.Endereco_Principal?.Pais?.Id,
                    Id_Tipo_Imovel = cliente.Endereco_Principal?.Tipo_Imovel?.Cod_Tipo_Imovel,
                    Id_Bairro = cliente?.Endereco_Principal?.Id_Bairro,
                    Bairro = cliente?.Endereco_Principal?.Bairro,
                    CEP = cliente?.Endereco_Principal?.Cep,
                    Id_Cidade = cliente?.Endereco_Principal?.Id_Cidade,

                    Cidade = cliente?.Endereco_Principal?.Cidade,
                    Complemento = cliente?.Endereco_Principal?.Complemento,
                    Id_Endereco = cliente?.Endereco_Principal != null ? cliente.Endereco_Principal.Id : (Int64)0,
                    Latitude = Convert.ToString(cliente?.Endereco_Principal?.Latitude),
                    Logradouro = cliente?.Endereco_Principal?.Logradouro,
                    Longitude = Convert.ToString(cliente?.Endereco_Principal?.Longitude),
                    Numero = cliente?.Endereco_Principal?.Numero,
                    Ponto_Referencia = cliente?.Endereco_Principal?.Ponto_Referencia,

                    Tipo = Tipo_EnderecoCliente.Principal,
                    UF = cliente?.Endereco_Principal?.UF,
                    Busca_Endereco = cliente?.Endereco_Principal?.Busca_Endereco != null ? cliente.Endereco_Prospecto.Busca_Endereco : null,
                    Id_Endereco_Logradouro = cliente?.Endereco_Principal?.Endereco_Logradouro?.Id,
                    Id_Condominio_Predio = cliente?.Endereco_Principal?.Condominio_Predio?.Id,
                    Id_Bloco = cliente?.Endereco_Principal?.Bloco,
                    Id_Apartamento = cliente?.Endereco_Principal?.Apartamento,
                    Cod_Escritorio = cliente.Endereco_Principal?.Cod_Escritorio,
                    Cod_Area = cliente.Endereco_Principal?.Cod_Area,
                    Cod_Localidade = Convert.ToString(cliente.Endereco_Principal?.Cod_Localidade),

                    Id_Cliente = cliente?.Id
                };
            }
            return retorno;
        }


        private async void GravarEtapaTrocaTitularidade(ISession session, TrocaTitularidade troca, CadastroTrocaTitularidadeModel view, string usuario)
        {
            try
            {
                IEtapaVendaRepository repoEtapaVenda = new EtapaVendaRepository(session);
                IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(session);
                var etapaVendaBusiness = new EtapaVendaBusiness(repoEtapaVenda, repoConfig);
                var sessionLog = HibernateUtil.GetSessionLog();
                using (var repositoryEtapa = new Conexao<EtapaVenda>(session))
                using (var repositoryTroca = new Conexao<TrocaTitularidade>(session))
                using (var repositoryLogs = new Conexao<LogEtapaVendas>(sessionLog))
                {
                    bool existemEtapas = etapaVendaBusiness.ExistemEtapasTroca(troca.Id);

                    EtapaVenda modelo = null;
                    EtapaVenda modeloAnterior = null;

                    if (view.Id == 0 || !existemEtapas)
                    {
                        foreach (EtapaVendaViewModel etapa in view.EtapasTroca)
                        {
                            EtapaVenda etapaVenda = new EtapaVenda
                            {
                                DataHoraCriacao = DateTime.Now,
                                UsuarioCriacao = usuario,

                                Etapa = (Sinapse.Model.enumerados.EnumEtapaVenda)etapa.Etapa,
                                Descricao = etapa.Descricao,

                                // O status deve ser preenchido no input hidden que existe na view
                                Status = (Sinapse.Model.enumerados.StatusEtapaVenda)etapa.Status,
                                TrocaTitularidade = troca,
                            };

                            //Log de mudança de status da primeira etapa
                            if ((Sinapse.Model.enumerados.StatusEtapaVenda)etapa.Status != StatusEtapaVenda.Pendente)
                            {
                                modelo = (EtapaVenda)etapaVenda.Clone();
                                modelo.Status = StatusEtapaVenda.Pendente;
                                //var logs = etapaVenda.GeraLog(modelo, User.Identity.GetUserId(), User.Identity.GetUserName(), Sinapse.ModelLog.TipoOperLog.Alteracao);
                                //await repositoryLogs.Save(logs);
                            }
                            etapaVendaBusiness.Save(etapaVenda);
                        }
                    }
                    else
                    {
                        EtapaVendaViewModel etapaAtual = view.EtapasTroca.Where(e => e.Etapa == view.EtapaAtualVenda).FirstOrDefault();

                        //Log 
                        EtapaVenda etapaVenda = new EtapaVenda
                        {
                            DataHoraCriacao = DateTime.Now,
                            UsuarioCriacao = usuario,
                            Etapa = (Sinapse.Model.enumerados.EnumEtapaVenda)etapaAtual.Etapa,
                            Descricao = etapaAtual.Descricao,
                            Status = (Sinapse.Model.enumerados.StatusEtapaVenda)etapaAtual.Status,
                            TrocaTitularidade = troca
                        };

                        modelo = await repositoryEtapa.selectFirst(p => p.Venda.Id == troca.Id && p.Etapa == (Sinapse.Model.enumerados.EnumEtapaVenda)etapaAtual.Etapa);

                        if (modelo != null)
                        {
                            modelo.Status = StatusEtapaVenda.Pendente;
                            var logs = etapaVenda.GeraLog(modelo, User.Identity.GetUserId(), User.Identity.GetUserName(), Sinapse.ModelLog.TipoOperLog.Alteracao);
                            //var logs = modelo.GeraLog(etapaVenda, User.Identity.GetUserId(), User.Identity.GetUserName(), Sinapse.ModelLog.TipoOperLog.Alteracao);
                            await repositoryLogs.Save(logs);
                        }


                        if (etapaAtual != null)
                        {
                            etapaVendaBusiness.MudarStatusEtapa(null, (Sinapse.Model.enumerados.EnumEtapaVenda)etapaAtual.Etapa, 
                                                                (Sinapse.Model.enumerados.StatusEtapaVenda)etapaAtual.Status, 
                                                                null, User.Identity.GetUserId(), troca.Id);
                        }
                    }

                    troca.Etapa_Atual = etapaVendaBusiness.VerificaUltimaEtapaTroca(troca.Id);
                    await repositoryTroca.Update(troca);
                }
            }
            catch (Exception)
            {
            }
        }

        private async Task<CadastroTrocaTitularidadeModel> GetViewObj(TrocaTitularidade trocaTitularidade,
                                                                  Cliente clienteAtual,
                                                                  Cliente clienteNovo,
                                                                  long numeroContrato,
                                                                  ISession session)
        {
            using (ISession sessionT5Sql = HibernateUtil.GetSessionT5())
            using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
            using (Conexao<Configuracoes_Gerais> repoConfiguracoes_Gerais = new Conexao<Configuracoes_Gerais>(session))
            using (Conexao<LoginRadius> repo_LoginRadius = new Conexao<LoginRadius>(session))
            using (Conexao<Modelo_Contrato> repo_ModeloContrato = new Conexao<Modelo_Contrato>(session))
            using (Conexao<DocTermoAnexo> repoDocAceite = new Conexao<DocTermoAnexo>(session))
            using (Conexao<TrocaTitularidade> repoTroca = new Conexao<TrocaTitularidade>(session))
            using (Conexao<Forma_Pagamento_Cliente> repoFormaPagCli = new Conexao<Forma_Pagamento_Cliente>(session))
            using (Conexao<ClienteContrato> repoCliContrato= new Conexao<ClienteContrato>(session))
            {
                CadastroTrocaTitularidadeModel retorno = new CadastroTrocaTitularidadeModel();
                Configuracoes_Gerais config = (await repoConfiguracoes_Gerais.selectFirst());

                retorno.NumeroContrato = numeroContrato;
                retorno.Id = trocaTitularidade.Id;
                retorno.StatusTrocaTitularidade = trocaTitularidade.StatusTrocaTitularidade;
                retorno.ClienteAtual = await BindClienteToCadastro(clienteAtual, trocaTitularidade, config, session);
                retorno.ClienteNovo = await BindClienteToCadastro(clienteNovo, trocaTitularidade, config, session);
                retorno.ConsultaViabilidadeRestringeVenda = config.ConsultaViabilidadeRestringeVenda;
                retorno.ConsultaPendenciaEnderecoRestringeVenda = config.ConsultaPendenciaEnderecoRestringeVenda;
                retorno.ConsultaPendenciaFinanceiraRestringeVenda = config.ConsultaPendenciaFinanceiraRestringeVenda;
                retorno.ConsultaCreditoRestringeVenda = config.ConsultaCreditoRestringeVenda;


                IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(session);
                IClienteRepository repoClient = new ClienteRepository(session);
                IEtapaVendaRepository repoEtapaVenda = new EtapaVendaRepository(session);

                var etapaVendaBusiness = new EtapaVendaBusiness(repoEtapaVenda, repoConfig);
                IEnumerable<EtapaVendaViewModel> etapasVenda = etapaVendaBusiness.GetDadosEtapasTrocaTitularidade(trocaTitularidade.Id);

                EtapaVendaViewModel ultimaEtapaVenda = etapasVenda.FirstOrDefault(e => (e.Status == (int)StatusEtapaVenda.Reprovado) ||
                                                       e.Status == (int)StatusEtapaVenda.Pendente);

                if (ultimaEtapaVenda == null)
                    ultimaEtapaVenda = etapasVenda.LastOrDefault();

                if (retorno.Id != 0 && ultimaEtapaVenda.Etapa >= (int)Sinapse.Model.enumerados.EnumEtapaVenda.TermoAdesao)
                {
                    if (trocaTitularidade.TermoAdesaoAssinado)
                    {
                        etapaVendaBusiness.MudarStatusEtapa(retorno.Id, Sinapse.Model.enumerados.EnumEtapaVenda.TermoAdesao, Sinapse.Model.enumerados.StatusEtapaVenda.Aprovado, null, null, trocaTitularidade.Id);
                    }
                }

                retorno.EtapasTroca = etapaVendaBusiness.GetDadosEtapasTrocaTitularidade(trocaTitularidade.Id);

                EtapaVendaViewModel ultimaEtapa =
                    retorno.EtapasTroca.FirstOrDefault(e => (e.Status == (int)StatusEtapaVenda.Reprovado || e.Status == (int)Sinapse.Model.enumerados.StatusEtapaVenda.Pendente));

                if (ultimaEtapa == null)
                    ultimaEtapa = retorno.EtapasTroca.LastOrDefault();

                retorno.EtapaAtualVenda = ultimaEtapa.Etapa;
                retorno.StatusDescricao = HtmlEnumExtensions.GetEnumDescription(retorno.StatusTrocaTitularidade);


                Endereco_Cliente enderecoCobrancaCliAtual = null;
                List<Forma_Pagamento_Cliente> formasPagamentoCliAtual = null;

                var cliContrato = await repoCliContrato.selectFirst(x => x.Id == numeroContrato);

                if (cliContrato == null)
                    throw new Exception();

                enderecoCobrancaCliAtual = cliContrato.Endereco_Cliente ?? clienteAtual.Endereco_Principal;
                formasPagamentoCliAtual = clienteAtual.PlanosProdutos.Where(x => x.ClienteContrato == cliContrato).Select(x => x.Forma_Pagamento_Cliente).ToList();

                //if (trocaTitularidade.Id == 0 || trocaTitularidade.Id != 0 && trocaTitularidade.StatusTrocaTitularidade != StatusTrocaTitularidade.Aprovado)
                //{
                //    enderecoCobrancaCliAtual = clienteAtual.PlanosProdutos.Where(x => x.ClienteContrato.Id == numeroContrato).Select(x => x.Cliente.Endereco_Principal).FirstOrDefault();

                //    formasPagamentoCliAtual = clienteAtual.PlanosProdutos.Where(x => x.ClienteContrato.Id == numeroContrato).Select(x => x.Forma_Pagamento_Cliente).ToList();
                //}
                //else
                //{
                //    enderecoCobrancaCliAtual = clienteNovo.PlanosProdutos.Where(x => x.ClienteContrato.Id == trocaTitularidade.NumeroContrato).Select(x => x.Cliente.Endereco_Principal).FirstOrDefault() ??
                //               clienteAtual.PlanosProdutos.Where(x => x.ClienteContrato.Id == trocaTitularidade.NumeroContrato).Select(x => x.Cliente.Endereco_Principal).FirstOrDefault();

                //    formasPagamentoCliAtual = clienteAtual.PlanosProdutos.Where(x => x.ClienteContrato.Id ==
                //                                                                    trocaTitularidade.NumeroContrato)
                //                                                            .Select(x => x.Forma_Pagamento_Cliente).ToList().Count > 0 ?
                //                                  clienteNovo.PlanosProdutos.Where(x => x.ClienteContrato.Id ==
                //                                                                   trocaTitularidade.NumeroContrato)
                //                                                            .Select(x => x.Forma_Pagamento_Cliente).ToList() : new List<Forma_Pagamento_Cliente>();
                //}


                if (enderecoCobrancaCliAtual != null)
                    retorno.EnderecoCobranca = await BindEnderecoCobranca(enderecoCobrancaCliAtual);

                retorno.Formas_Pagamento = formasPagamentoCliAtual?.Where(f => f != null).Count() > 0 ? formasPagamentoCliAtual?.Distinct().Select(f => new Forma_Pagamento_ClienteModels
                {
                    Id = f.Id,
                    Id_Cliente = f.Id,
                    Id_Forma_Pagamento = f.Forma_Pagamento?.Id,
                    Desc_Forma_Pagamento = f.Forma_Pagamento?.Descricao,

                    Forma_Envio = f.Forma_Envio,
                    Desc_Forma_Envio = f.Forma_Envio?.GetDisplayName(),
                    Id_Vencimento = f.Vencimento?.Id,
                    Id_Banco = f?.Banco?.Id,

                    Desc_Banco = !String.IsNullOrEmpty(f?.Banco?.Descricao) ? f?.Banco?.Descricao : "-",
                    Agencia = !String.IsNullOrEmpty(f.Agencia) ? f.Agencia : "-",
                    Conta_Corrente = !String.IsNullOrEmpty(f.Conta_Corrente) ? f.Conta_Corrente : "-",
                    Digito = !String.IsNullOrEmpty(f.Digito) ? f.Digito : "-",
                    Dia_Vencimento = f.Dia_Vencimento,

                    End_Cobranca_Igual_Principal = f.End_Cobranca_Igual_Principal,
                    Id_EnderecoCobranca = f.Endereco_Cobranca?.Id,
                    Id_Tipo_ImovelCobranca = f.Endereco_Cobranca?.Tipo_Imovel?.Id,
                    Id_PaisCobranca = f.Endereco_Cobranca?.Pais?.Id,
                    LatitudeCobranca = Convert.ToString(f.Endereco_Cobranca?.Latitude),

                    LongitudeCobranca = Convert.ToString(f.Endereco_Cobranca?.Longitude),

                    CEPCobranca = f.Endereco_Cobranca?.Cep,
                    LogradouroCobranca = f.Endereco_Cobranca?.Logradouro,
                    NumeroCobranca = f.Endereco_Cobranca?.Numero,
                    BairroCobranca = f.Endereco_Cobranca?.Bairro,
                    Id_BairroCobranca = f.Endereco_Cobranca?.Id_Bairro,
                    CidadeCobranca = f.Endereco_Cobranca?.Cidade,

                    Id_CidadeCobranca = f.Endereco_Cobranca?.Id_Cidade,
                    UFCobranca = f.Endereco_Cobranca?.UF,
                    ComplementoCobranca = f.Endereco_Cobranca?.Complemento,
                    Ponto_ReferenciaCobranca = f.Endereco_Cobranca?.Ponto_Referencia,
                    Status = f.Status,
                    Tipo_Forma_Pagamento = f.Forma_Pagamento?.Tipo_Forma_Pagamento

                }).ToList() : new List<Forma_Pagamento_ClienteModels>();

                return retorno;
            }
        }


        private async Task<EnderecoClienteViewModel> BindEnderecoCobranca(Endereco_Cliente enderecoCli)
        {
            var response = new EnderecoClienteViewModel
            {
                Id = enderecoCli.Id,
                CEP = enderecoCli?.Cep,
                Logradouro = enderecoCli?.Logradouro,
                Numero = enderecoCli?.Numero,
                UF = enderecoCli?.UF,
                Id_UF = enderecoCli?.Id_UF,
                Cidade = enderecoCli?.Cidade,
                Id_Cidade = enderecoCli?.Id_Cidade,
                Id_Pais = enderecoCli?.Pais?.Id,
                Bairro = enderecoCli?.Bairro,
                Id_Bairro = enderecoCli?.Id_Bairro,
                Id_Tipo_Imovel = enderecoCli?.Tipo_Imovel?.Cod_Tipo_Imovel,
                Pais = enderecoCli?.Pais?.Nome,
                Complemento = enderecoCli?.Complemento,
                Latitude = enderecoCli?.Latitude != null ? enderecoCli?.Latitude.ToString() : "",
                Longitude = enderecoCli?.Longitude != null ? enderecoCli?.Latitude.ToString() : ""
            };

            return response;
        }

        private async Task<Cliente> BindClienteToBanco(Cliente cliente,
                                                       ClienteCadastroTitularidade clienteCadastro,
                                                       string usuario)
        {
            Cliente clienteBanco = null;

            using (ISession session = HibernateUtil.GetSession())
            using (var repoCli = new Conexao<Cliente>(session))
            using (var repoConfigGerais = new Conexao<Configuracoes_Gerais>(session))
            using (var repoCategoriaCli = new Conexao<Categoria_Cliente>(session))
            {
                if (clienteCadastro.Id_Cliente != 0)
                {
                    clienteBanco = await repoCli.selectFirst(x => x.Id == cliente.Id);
                }
                else
                {
                    clienteBanco = new Cliente()
                    {
                        Status = Status_Cliente.Ativo,
                        Tipo_Cliente = Tipo_Cliente.Cliente,
                        UsuarioCriacao = usuario,
                    };
                }

                clienteBanco.Tipo_Pessoa = clienteCadastro?.TipoPessoa;
                clienteBanco.CpfCnpj = clienteCadastro?.CpfCnpj;
                clienteBanco.NomeFantasia = clienteCadastro?.NomeFantasia;
                clienteBanco.Data_Nascimento = !string.IsNullOrEmpty(clienteCadastro?.DataNascimento) ? DateTime.Parse(clienteCadastro?.DataNascimento) : (DateTime?)null;
                clienteBanco.Nacionalidade = clienteCadastro?.Nacionalidade;
                clienteBanco.Sexo = clienteCadastro?.Sexo;
                clienteBanco.Estado_Civil = clienteCadastro?.EstadoCivil;
                clienteBanco.Filiacao_Mae = clienteCadastro?.Mae;
                clienteBanco.Filiacao_Pai = clienteCadastro?.Pai;
                clienteBanco.Rg = clienteCadastro?.RG;
                clienteBanco.Data_Emissao_Rg = !string.IsNullOrEmpty(clienteCadastro?.DataEmissao) ? DateTime.Parse(clienteCadastro?.DataEmissao) : (DateTime?)null;
                clienteBanco.Orgao_Emissao_Rg = clienteCadastro?.Orgao;
                clienteBanco.Profissao = clienteCadastro?.Profissao;
                clienteBanco.Grau_Instrucao = clienteCadastro?.Instrucao;
                clienteBanco.Telefone_Fixo = clienteCadastro?.TelefoneFixo;
                clienteBanco.Telefone_Celular = clienteCadastro?.TelefoneCelular;
                clienteBanco.Email_Principal = clienteCadastro?.Email;
                //empresa
                clienteBanco.Razao_Social = clienteCadastro?.Razao_Social;
                clienteBanco.Data_Abertura_Empresa = clienteCadastro?.Data_Abertura;
                clienteBanco.Ramo_Atividade = clienteCadastro?.Ramo_Atividade;
                clienteBanco.Inscricao_Estadual = clienteCadastro?.Inscricao_Estadual;
                clienteBanco.Inscricao_Municipal = clienteCadastro?.Inscricao_Municipal;
                clienteBanco.Cpf_Responsavel = clienteCadastro?.CPFResponsavel;
                clienteBanco.Cargo_Responsavel = clienteCadastro?.Cargo_Responsavel;
                clienteBanco.Categoria_Cliente = clienteCadastro.Id_Categoria_Cliente != null ? await repoCategoriaCli.selectFirst(x => x.Id == clienteCadastro.Id_Categoria_Cliente) : null;
                clienteBanco.Numero_Contrato = (await repoConfigGerais.selectFirst()).Identificacao_Cliente == Identificacao_Cliente.Sequencial &&
                                                string.IsNullOrEmpty(clienteCadastro.Identificacao_Cliente) ? 
                                                $"{Convert.ToInt32(repoCli.Max(p => p.Numero_Contrato).Result) + 1}" : clienteCadastro.Identificacao_Cliente;

                await repoCli.SaveOrUpdate(clienteBanco);
            }

            return clienteBanco;
        }

        private async Task<Endereco_Cliente> BindFormPagEndereco (Forma_Pagamento_Cliente formPagCli, 
                                                             Cliente titularNovo,
                                                             EnderecoClienteViewModel endCli = null)
        {
            return new Endereco_Cliente()
            {
                Cep = formPagCli?.Endereco_Cobranca?.Cep,
                Logradouro = formPagCli?.Endereco_Cobranca?.Logradouro,
                Atendimento = formPagCli?.Endereco_Cobranca?.Atendimento,
                Tipo_Imovel = formPagCli?.Endereco_Cobranca?.Tipo_Imovel,
                Pais = formPagCli?.Endereco_Cobranca?.Pais,
                Numero = formPagCli?.Endereco_Cobranca?.Numero,
                Cod_Conexao = formPagCli?.Endereco_Cobranca?.Cod_Conexao ,
                //Cliente = titularNovo?.NomeFantasia,
                Cliente = titularNovo,
                Id_Bairro = formPagCli?.Endereco_Cobranca?.Id_Bairro,
                Id_Cidade = formPagCli?.Endereco_Cobranca?.Id_Cidade ,
                Bairro = formPagCli?.Endereco_Cobranca?.Bairro,
                Cidade = formPagCli?.Endereco_Cobranca?.Cidade,
                UF = formPagCli?.Endereco_Cobranca?.UF,
                Id_UF = formPagCli?.Endereco_Cobranca?.Id_UF,
                Complemento = formPagCli?.Endereco_Cobranca?.Complemento,
                Busca_Endereco = formPagCli?.Endereco_Cobranca?.Busca_Endereco,
                Condominio_Predio = formPagCli?.Endereco_Cobranca?.Condominio_Predio,
                Endereco_Logradouro = formPagCli?.Endereco_Cobranca?.Endereco_Logradouro,
                BlocoDescri = formPagCli?.Endereco_Cobranca?.BlocoDescri,
                Bloco = formPagCli?.Endereco_Cobranca?.Bloco,
                ApartamentoDescri = formPagCli?.Endereco_Cobranca?.ApartamentoDescri,
                Apartamento = formPagCli?.Endereco_Cobranca?.Apartamento,
                Ponto_Referencia = formPagCli?.Endereco_Cobranca?.Ponto_Referencia,
                Latitude = formPagCli?.Endereco_Cobranca?.Latitude,
                Longitude = formPagCli?.Endereco_Cobranca?.Longitude,
                Cod_Area = formPagCli?.Endereco_Cobranca?.Cod_Area,
                Cod_Escritorio = formPagCli?.Endereco_Cobranca?.Cod_Escritorio,
                Cod_Localidade = formPagCli?.Endereco_Cobranca?.Cod_Localidade,
                Localidade = formPagCli?.Endereco_Cobranca?.Localidade,
            };
        }
    }

    public class CancelarTroca
    {
        public List<int> ids { get; set; } = new List<int>();
    }
    public class CancelarTrocaResponse
    {
        public long Id { get; set; }
        public string StatusTroca { get; set; }
        public string TermoAdesao { get; set; }
        public string Motivo { get; set; }
    }
    public class AssociaChecklistTrocaRequest
    {
        public long IdTroca { get; set; }
        public long IdChecklist { get; set; }
    }
    public class VincularAtendimentoRequest
    {
        public long? IdTroca { get; set; }
        public List<Int64> IdsAtendimento { get; set; }
    }

    public class RealizarTrocaTitularidade
    {
        public long IdTroca { get; set; }
        public List<Forma_Pagamento_ClienteModels> LsFormaPagamentoCliModels { get; set; }
        public int EtapaAtualTrocaTitularidade { get; set; }
        private IEnumerable<EtapaVendaViewModel> _etapasTroca = null; 
        public IEnumerable<EtapaVendaViewModel> EtapasTroca
        {
            get
            {
                if (this._etapasTroca == null)
                    this._etapasTroca = new List<EtapaVendaViewModel>();

                return this._etapasTroca;
            }
            set
            {
                this._etapasTroca = value;
            }
        }
    }
}

