using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.ConnectBusiness.MK;
using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.Controllers;
using Aniel.Connect.ControllersWebApi.Ativacao;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Models.T5;
using Aniel.Connect.Repository.IntegracaoExterna;
using Aniel.Connect.Repository.MK;
using Aniel.Connect.Repository.MK.PendenciaEndereco;
using Aniel.Connect.Repository.MK.PendenciaFinanceira;
using Aniel.Connect.Repository.Model.Hubsoft;
using Aniel.Connect.Repository.Model.HubSoft;
using Aniel.Connect.Repository.Model.HubSoft.MovimentoCliente;
using Aniel.Connect.Repository.PendenciaFinanceiraIntegracao;
using Aniel.Connect.Repository.Repository.HubSoft;
using Aniel.Connect.Repository.Repository.MK;
using Aniel.Connect.Repository.Repository.T5.InventarioCliente;
using Aniel.Connect.Repository.T5.Cliente;
using Aniel.Connect.Repository.T5.DocumentoProducao;
using Aniel.Connect.Repository.Voalle;
using FirebirdSql.Data.FirebirdClient;
using Google.Protobuf.WellKnownTypes;
using log4net;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using MySqlX.XDevAPI;
using Newtonsoft.Json;
using NHibernate;
using Renci.SshNet;
using Sinapse.Core;
using Sinapse.DAO.repository;
using Sinapse.Integracao.Ativacao.Models;
using Sinapse.Model;
//using Aniel.Connect.Models;
using Sinapse.Model.enumerados;
using Sinapse.Util.RemoteConnection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI.WebControls;
using Xceed.Words.NET;
using static Aniel.Connect.Repository.Model.HubSoft.AtivarServico.AtivarServicoHubsoftModel;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class AutorizarONUApiController : BaseApiController
    {
        [AllowAnonymous]
        [HttpPost]
        [Route("api/AutorizarONUApiController/AutorizarONU")]
        public async Task<PostApiRetorno> AutorizarONU(Aniel.Connect.Models.AutorizarONURequest request)
        {
            PostApiRetorno retorno = new PostApiRetorno();
            retorno.Mensagens = new List<PostApiMensagem>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repoONU = new Conexao<ONU>(session))
                using (var repoPlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
                using (var apiRadius = new IntegracaoRadiusApiController())
                {
                    //var taskList = new List<Task>();
                    foreach (var view in request.ListONU)
                    {
                        //var taskONU = Task.Run(async () =>
                        //{
                        //using (var transaction = session.BeginTransaction())
                        //{
                        var onu = await repoONU.selectFirst(p => p.Mac == view.Mac_Serial || p.Serial == view.Mac_Serial);
                        try
                        {
                            

                            if (onu == null)
                            {
                                onu = new ONU
                                {
                                    Mac = view.Mac_Serial,
                                    Serial = view.Mac_Serial
                                };
                            }
                            else
                            {
                                //Permite o técnico do Touch forçar desautorização e/ou remoção de registro de ONU desatualizado no Connect
                                if(view.Origem_Mobile && view.Forcar_Remocao_ONU_Existente)
                                {
                                    var requestDesautorizar = new AtivaEquipamento();
                                    requestDesautorizar.Origem_Mobile = true;
                                    requestDesautorizar.IdONU = onu.Id;
                                    requestDesautorizar.OntId = onu.OntID ?? 0;
                                    requestDesautorizar.IdOLT = onu.OLT.Id;
                                    requestDesautorizar.NumSlot = onu.Slot.NumSlot ?? 0;
                                    requestDesautorizar.NumPon = onu.PON.Num_PON ?? 0;
                                    requestDesautorizar.Serial = view.Mac_Serial;
                                    requestDesautorizar.Serial_Convertido = view.Mac_Serial_Convertido;
                                    requestDesautorizar.IdsOLts = new List<long> { onu.OLT.Id };

                                    var responseDesautorizar = await new IntegracaoAtivacaoApiController().DesautorizarONU(requestDesautorizar);
                                    if (!responseDesautorizar.result && !responseDesautorizar.message.ToUpperInvariant().Contains("SUCESSO"))
                                        throw new AutorizarONUException(responseDesautorizar.message);


                                    
                                    session.Clear();
                                    onu = new ONU
                                    {
                                        Mac = view.Mac_Serial,
                                        Serial = view.Mac_Serial
                                    };
                                }
                                else
                                {
                                    //CUIDADO AO ALTERAR CRÍTICA, O TOUCH REALIZA PROCESSOS BASEADOS NESTA MENSAGEM
                                    throw new AutorizarONUException($"Já existe uma ONU de serial {onu.Serial} provisionada no sistema, na OLT {onu.OLT.Descricao}, slot {onu.Slot.NumSlot} e pon {onu.PON.Num_PON}.");
                                }                                    
                            }

                            if(view.Id_Cliente_Plano_Produto > 0)
                            {
                                await new LoginRadiusApiController().ValidarTrocaPlanoLogin(view.Id_Cliente_Plano_Produto);
                            }

                            onu = await PreencherCamposONU(onu, view);
                            await repoONU.SaveOrUpdate(onu);

                            var clientePlanoProduto = await repoPlanoProduto.selectFirst(p => p.Id == onu.Cliente_PlanoProduto.Id);
                            onu.Cliente_PlanoProduto = clientePlanoProduto;

                            using (var apiAtivacao = new IntegracaoAtivacaoApiController())
                            {
                                string scriptONU = null;
                                string scriptOLT = null;

                                string scriptOLTBase64 = null;

                                switch (onu.Cliente_PlanoProduto.Plano?.Tipo_Tecnologia)
                                {
                                    case Model.enumerado.Tipo_Tecnologia.Internet:
                                        if (onu.PerfilAtivacao.ScriptOLT != null)
                                            scriptOLTBase64 = Encoding.UTF8.GetString(onu.PerfilAtivacao.ScriptOLT);
                                        else
                                            throw new Exception($"Não há Script de Internet para a ONU de serial {view.Mac_Serial}");
                                        break;

                                    case Model.enumerado.Tipo_Tecnologia.Telefonia_Fixo:
                                        if (onu.PerfilAtivacao.ScriptOLTVoz != null)
                                            scriptOLTBase64 = Encoding.UTF8.GetString(onu.PerfilAtivacao.ScriptOLTVoz);
                                        else
                                            throw new Exception($"Não há Script de Voz para a ONU de serial {view.Mac_Serial}");
                                        break;

                                    case null:
                                        throw new Exception($"Tipo Tecnologia não cadastrado para a ONU {view.Mac_Serial}");

                                    default:
                                        throw new Exception($"Tipo Tecnologia não implementado para a ONU {view.Mac_Serial}");

                                }                               

                                scriptOLT = Encoding.UTF8.GetString(Convert.FromBase64String(scriptOLTBase64));

                                var listaValidacoesScriptOLT = ValidarVariaveisScript(onu, scriptOLT);
                                if (listaValidacoesScriptOLT.Count > 0)
                                    throw new EAnielException(string.Join("\r \n", listaValidacoesScriptOLT));

                                scriptOLT = SubstituirVariaveis(onu, scriptOLT);

                                if (onu.PerfilAtivacao.ScriptONU != null)
                                {
                                    var scriptONUBase64 = Encoding.UTF8.GetString(onu.PerfilAtivacao.ScriptONU);

                                    scriptONU = Encoding.UTF8.GetString(Convert.FromBase64String(scriptONUBase64));

                                    var listaValidacoesScriptONU = ValidarVariaveisScript(onu, scriptONU);
                                    if (listaValidacoesScriptONU.Count > 0)
                                        throw new EAnielException(string.Join("\r \n", listaValidacoesScriptONU));

                                    scriptONU = SubstituirVariaveis(onu, scriptONU);
                                }

                                var responseAtivarEquipamento = await apiAtivacao.AtivarEquipamento(new Models.AtivaEquipamento
                                {
                                    IdOLT = onu.OLT.Id,
                                    IdONU = onu.Id,
                                    Detalhe = view.Detalhe,
                                    Usuario = view.Usuario,
                                    ScriptOLT = scriptOLT,
                                    ScriptONU = scriptONU
                                });


                                if (!responseAtivarEquipamento.result)
                                {
                                    throw new Exception(responseAtivarEquipamento.message);
                                }


                                if(!onu.OntID.HasValue && responseAtivarEquipamento.valor.HasValue)
                                {
                                    onu.OntID = responseAtivarEquipamento.valor;
                                }

                                

                                if (onu?.Cliente_PlanoProduto?.Cliente != null && onu?.Cliente_PlanoProduto?.Plano?.Id != null)
                                using (var webHook = new WebHookApiController())
                                {
                                    await webHook.ExecutaAtivacaoPlano(User.Identity.GetUserId(), onu.Cliente_PlanoProduto.Cliente.Id, onu.Cliente_PlanoProduto.Plano.Id, DateTime.Now);
                                }
                               

                            }

                            if (onu.Cliente_PlanoProduto.Plano.AutenticacaoRadius)
                            {
                                string usuario = string.IsNullOrEmpty(view.Usuario) ? string.Empty : view.Usuario;
                                string detalhe = string.IsNullOrEmpty(view.Detalhe) ? $"OPERAÇÃO REALIZADA SEM O.S -> SERIAL: {onu?.Serial}" : view.Detalhe;

                                var responseDesbloqueio = await apiRadius.DesbloquearUsuario(new PlanoProdutoRadiusRequest
                                {
                                    IdClientePlanoProduto = onu.Cliente_PlanoProduto.Id,

                                },
                                this.EnviarLog,
                                usuario,
                                detalhe);

                                if (!responseDesbloqueio.result)
                                {
                                    retorno.Mensagens.Add(new PostApiMensagem
                                    {
                                        Tipo = PostApiMensagemType.Info,
                                        Mensagem = $"Serial {view.Mac_Serial}: {responseDesbloqueio.message}"
                                    });
                                }
                            }
                            else
                            {
                                retorno.Mensagens.Add(new PostApiMensagem
                                {
                                    Tipo = PostApiMensagemType.Info,
                                    Mensagem = $"Login não habilitado pois o Plano não usa autenticação Radius"
                                });
                            }

                            onu.Cliente_PlanoProduto.Status = Sinapse.Model.enumerados.Status_PlanoProduto.Habilitado;
                            await repoPlanoProduto.Update(onu.Cliente_PlanoProduto);

                            #region Integrações Externas de Provisionamento
                            try
                            {
                                IIntegracaoExternaRepository repoIntegracaoExterna = new IntegracaoExternaRepository(session);
                                var integracaoExternaBusiness = new IntegracaoExternaBusiness(repoIntegracaoExterna);
                                IntegracoesExternas externas =
                                integracaoExternaBusiness.GetIntegracaoExterna(EnumTipoIntegracao.PROVISIONAMENTO);

                                if (externas != null)
                                {
                                    switch (externas.IntegracaoExterna)
                                    {
                                        case IntegracaoExterna.VOALLE:
                                            var onuAtualizada = onu.OntID.HasValue ? onu : await repoONU.selectFirst(p => p.Mac == view.Mac_Serial || p.Serial == view.Mac_Serial);
                                            IVoalleRepository repoVoalle = new VoalleApiRepository(externas);

                                            string usuario = string.IsNullOrEmpty(view.Usuario) ? string.Empty : view.Usuario;
                                            string detalhe = string.IsNullOrEmpty(view.Detalhe) ? $"OPERAÇÃO REALIZADA SEM O.S -> SERIAL: {onu?.Serial}" : view.Detalhe;

                                            var voalleBusiness = new ConnectBusiness.MK.VoalleBusiness(null, repoVoalle);
                                            var retornoVoalle = voalleBusiness.UpdateConnection(onu, usuario, detalhe, this.EnviarLog);

                                            if(!retornoVoalle.success || !retornoVoalle.response)
                                            {
                                                throw new EAnielException($"Erro na atualização da conexão no Voalle. Retorno do Voalle: {Sinapse.Util.JsonService.SerializeObject(retornoVoalle)}");
                                            }
                                            break;
                                        case IntegracaoExterna.MK_SOLUTION:

                                            
                                            IMKProvisionamentoRepository repoMK = new MKProvisionamentoApiRepository(externas);
                                            var mkBusiness = new ConnectBusiness.MK.MKBusiness(null, null, repoMK);

                                            //Coletando dados do id da caixa e número da porta que vierem da requisição
                                            var caixa = request.ListONU
                                            .Where(p => p.Mac_Serial == onu.Mac)
                                            .Select(p => p.IdCaixa)
                                            .FirstOrDefault();

                                            var porta = request.ListONU
                                           .Where(p => p.Mac_Serial == onu.Mac) 
                                           .Select(p => p.NumPorta)
                                           .FirstOrDefault();

                                            //Se id da caixa ou número da porta vierem como 0 ou nulo da requisição
                                            //será realizada uma tentativa de encontrar a porta pelo login radius na TB_ELEMENTO_REDE_PORTA.
                                            //Encontrando, a aplicação prosseguirá. Senão, será retornada uma exceção.
                                            if (caixa == 0 || porta == null || porta == 0)
                                            {
                                                var repoElementoRedePorta = new Conexao<Elemento_Rede_Porta>(sessionT5);
                                                var numeroPorta = await repoElementoRedePorta.selectFirst(p => p.Login == onu.LoginRadius.Login);
                                                if(numeroPorta == null)
                                                {
                                                    throw new EAnielException("Não foi encontrada uma porta ou caixa para este login.");
                                                }
                                                caixa = numeroPorta.Caixa.Id;
                                                porta = numeroPorta.Porta;
                                            }
                                            var repoElementoRede = new Conexao<Elemento_Rede>(sessionT5);
                                            var descricaoCaixa = await repoElementoRede.selectFirst(p => p.Id == caixa);
                                            if(descricaoCaixa == null)
                                            {
                                                throw new EAnielException("Caixa não encontrada.");
                                            }
                                            var retornoVinculoConexaoMK = await mkBusiness.VinculaConexao(descricaoCaixa.Descricao, porta, onu.LoginRadius.Login, onu.Mac);


                                            if (retornoVinculoConexaoMK.success == false)
                                            {
                                                throw new EAnielException($"Erro no vínculo de login no MK. Retorno da API: {Sinapse.Util.JsonService.SerializeObject(retornoVinculoConexaoMK)}");
                                            }

                                          break; 
                                                                                     
                                        case IntegracaoExterna.HUBSOFT:
                                            var erros = new List<string>();
                                            IntegracoesExternas dadosIntegracaoHubsoft = integracaoExternaBusiness.GetIntegracaoExterna(EnumTipoIntegracao.PROVISIONAMENTO, IntegracaoExterna.HUBSOFT);

                                            if (String.IsNullOrEmpty(dadosIntegracaoHubsoft.UrlApi) ||
                                                 String.IsNullOrEmpty(dadosIntegracaoHubsoft.Usuario) ||
                                                 String.IsNullOrEmpty(dadosIntegracaoHubsoft.Senha) ||
                                                 String.IsNullOrEmpty(dadosIntegracaoHubsoft.ApiSecret) ||
                                                 String.IsNullOrEmpty(dadosIntegracaoHubsoft.TokenAcesso))
                                            {
                                                string message = "Integração com a API do HubSoft não foi configurada corretamente, confira os acessos.";
                                                throw new EAnielException(message);
                                            }

                                            var loginHubsoft = new LoginHubsoftModel
                                            {
                                                client_id = dadosIntegracaoHubsoft.TokenAcesso,
                                                client_secret = dadosIntegracaoHubsoft.ApiSecret,
                                                username = dadosIntegracaoHubsoft.Usuario,
                                                password = dadosIntegracaoHubsoft.Senha
                                            };

                                            var onuAtualizadaHubsoft = onu.OntID.HasValue ? onu : await repoONU.selectFirst(p => p.Mac == view.Mac_Serial || p.Serial == view.Mac_Serial);

                                            var dadosBodyRequest = new ConfigurarAutenticacaoHubsoft
                                            {
                                                id_cliente_servico = onuAtualizadaHubsoft?.LoginRadius.CodigoAuxiliar,
                                                id_interface_conexao = onuAtualizadaHubsoft?.PON?.CodigoAuxiliar,
                                                phy_addr = onuAtualizadaHubsoft?.Serial ?? onuAtualizadaHubsoft?.Mac
                                                
                                            };

                                            var requestLimpezaMac = new LimpezaMacHubsoft
                                            {
                                                id_cliente_servico = onuAtualizadaHubsoft?.LoginRadius.CodigoAuxiliar
                                            };

                                            string usuarioHubsoft = string.IsNullOrEmpty(view.Usuario) ? string.Empty : view.Usuario;
                                            string detalheHubsoft = string.IsNullOrEmpty(view.Detalhe) ? $"OPERAÇÃO REALIZADA SEM O.S -> SERIAL: {onu?.Serial}" : view.Detalhe;

                                            var limpezaMac = ProvisionamentoHubsoft.LimpezaMac(dadosIntegracaoHubsoft.UrlApi, requestLimpezaMac,
                                                loginHubsoft, usuarioHubsoft, detalheHubsoft, this.EnviarLog);


                                            #region Movimento material cliente HubSoft
                                            if (CONSTANTES.AtivaMovimentacaoMat)
                                            {
                                                if (view.Origem_Mobile == true || !String.IsNullOrEmpty(view.Num_Os))
                                                {
                                                    IDocumentoProducaoAnielRepository docProdRepo = new DocumentoProducaoAnielRepository(sessionT5, sessionT5);
                                                    string usuarioLogin = "";

                                                    if (view.Origem_Mobile == false)
                                                    {
                                                        var (codmat, codcpl) = await GetCodMatByCodComplementar(sessionT5, onu.Serial, onu.SerialConvertido);
                                                        view.CodMat = codmat;
                                                        view.CodCpl = codcpl;

                                                        var dadosOS = await docProdRepo.GetDadosOSByNumOs(sessionT5, view.Num_Os, detalheHubsoft, usuarioHubsoft, this.EnviarLog);

                                                        view.NumObra = dadosOS.num_obra;
                                                        view.CodCt = dadosOS.codct;
                                                        view.Projeto = dadosOS.projeto;
                                                        usuarioLogin = User.Identity.GetUserId();
                                                        usuarioHubsoft = User.Identity.GetUserId();
                                                        detalheHubsoft = view.Num_Os;

                                                    }

                                                    
                                                    if (view.Origem_Mobile == true)
                                                    {
                                                        usuarioLogin = usuarioHubsoft;                                                                                                       
                                                    }

                                                    bool saldoCliente = false;

                                                    //Verifica se a ONU consta no saldo do cliente
                                                    if (view.IdCliente == null)
                                                    {
                                                        var cpfCnpj = !string.IsNullOrWhiteSpace(view.NumCpfCnpj)
                                                            ? view.NumCpfCnpj
                                                            : onu?.Cliente_PlanoProduto?.Cliente.CpfCnpj;

                                                        IClienteAnielRepository clienteAnielRepository = new ClienteAnielRepository(sessionT5);
                                                        ClienteAnielBusiness clienteAnielBusiness = new ClienteAnielBusiness(clienteAnielRepository);
                                                        var clienteAniel = clienteAnielBusiness.GetClientebyCPFCNPJ(cpfCnpj);
                                                        view.IdCliente = clienteAniel.Cod_Cliente;
                                                    }

                                                    var getSaldo = await getSaldoVincCliente(sessionT5, (long)view.IdCliente);
                                                    var verificaSerial = getSaldo.Any(p => p.cpl == view.CodCpl && p.CODMAT == view.CodMat);
                                                    saldoCliente = verificaSerial;

                                                    //Se não constar no inventário do cliente, entra neste bloco.
                                                    //Caso contrário, o material não é movimentado
                                                    if (!saldoCliente)
                                                    {
                                                        var osAniel = await docProdRepo.GetDadosOSByNumObra(sessionT5, view.NumObra, detalheHubsoft, usuarioHubsoft, this.EnviarLog);

                                                        if (osAniel == null)
                                                        {
                                                            throw new Exception("O.S não encontrada na base de dados do Aniel.");
                                                        }

                                                        string idHubsoft = osAniel.adicional_1;

                                                        if (string.IsNullOrEmpty(idHubsoft))
                                                        {
                                                            idHubsoft = osAniel.id_hubsoft;

                                                            if (string.IsNullOrEmpty(idHubsoft))
                                                            {
                                                                throw new Exception("ID da O.S está vazio.");
                                                            }

                                                        }

                                                        docProdRepo.AdicionarEquipamento(sessionT5, detalheHubsoft, usuarioHubsoft, view.CodCpl, view.CodMat, osAniel.equipe, view.NumObra, osAniel);

                                                        var osHubsoft = OrdemServicoHubSoftDao.GetOrdemServicoById(dadosIntegracaoHubsoft.UrlApi, Convert.ToInt32(idHubsoft), loginHubsoft, detalheHubsoft, usuarioHubsoft, this.EnviarLog);

                                                        if (osHubsoft != null)
                                                        {
                                                            bool provisionamento = true;
                                                            var materialAplicado = docProdRepo.GetAplicacaoNaoIntegrados(sessionT5, view.NumObra, view.CodCpl, view.CodMat, view.Projeto, provisionamento, usuarioHubsoft, detalheHubsoft, this.EnviarLog);

                                                            if (!String.IsNullOrEmpty(materialAplicado?.Codcpl))
                                                            {
                                                                if (string.IsNullOrEmpty(materialAplicado.DepositoTecnico))
                                                                {
                                                                    erros.Add($"Local de estoque do técnico {materialAplicado.Equipe} não informado");
                                                                }

                                                                string envio = "";
                                                                string retornoMovimento = null;
                                                                var estoqueHubSoft = EstoqueHubSoftDao.GetEstoque(dadosIntegracaoHubsoft.UrlApi, materialAplicado.DepositoTecnico, loginHubsoft, detalheHubsoft, usuarioHubsoft, this.EnviarLog);

                                                                if (estoqueHubSoft != null)
                                                                {
                                                                    if (osHubsoft.id_pop != null)
                                                                    {
                                                                        var model = docProdRepo.BindMateriaisAplicacaoPop(materialAplicado, loginHubsoft, dadosIntegracaoHubsoft, detalheHubsoft, usuarioHubsoft, this.EnviarLog);
                                                                        if (model != null)
                                                                        {
                                                                            model.id_pop = osHubsoft.id_pop;
                                                                            envio = JsonConvert.SerializeObject(model);
                                                                            retornoMovimento = EstoqueHubSoftDao.MovimentoSaidaPop(model, loginHubsoft, dadosIntegracaoHubsoft.UrlApi, detalheHubsoft, usuarioHubsoft, this.EnviarLog);
                                                                            if (retornoMovimento.ToUpperInvariant().Contains("ERRO".ToUpperInvariant()))
                                                                            {
                                                                                erros.Add(retornoMovimento);
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            erros.Add($"Não foi possível encontrar o patrimônio de serial {view.CodCpl}");
                                                                        }

                                                                    }

                                                                    if (osHubsoft.id_cliente_servico != 0)
                                                                    {
                                                                        var model = docProdRepo.BindMateriaisAplicacao(materialAplicado, osHubsoft, loginHubsoft, dadosIntegracaoHubsoft, detalheHubsoft, usuarioHubsoft, this.EnviarLog);
                                                                        if (model != null)
                                                                        {
                                                                            model.id_cliente_servico = osHubsoft.id_cliente_servico;
                                                                            envio = JsonConvert.SerializeObject(model);
                                                                            retornoMovimento = EstoqueHubSoftDao.MovimentoSaidaClienteServico(model, loginHubsoft, dadosIntegracaoHubsoft.UrlApi, detalheHubsoft, usuarioHubsoft, this.EnviarLog);
                                                                            if (retornoMovimento.ToUpperInvariant().Contains("ERRO".ToUpperInvariant()))
                                                                            {
                                                                                erros.Add(retornoMovimento);
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            erros.Add($"Não foi possível encontrar o patrimônio de serial {view.CodCpl}");
                                                                        }
                                                                    }

                                                                    if (erros.Count() == 0)
                                                                    {

                                                                        Int64 idConsumoProtheus = docProdRepo.GetIDConsumoProtheus(sessionT5, view.NumObra, materialAplicado.Codcpl, materialAplicado.Codmat, view.Projeto, view.CodCt, usuarioHubsoft, detalheHubsoft, this.EnviarLog);

                                                                        if (idConsumoProtheus != 0)
                                                                        {
                                                                            docProdRepo.MarcarImportados(sessionT5, idConsumoProtheus, envio, retornoMovimento);
                                                                        }

                                                                        await docProdRepo.MarcarIntegrado(sessionT5, view.CodCt, view.Projeto, view.NumObra, materialAplicado.Codmat, materialAplicado.Codcpl, provisionamento, detalheHubsoft, usuarioHubsoft, this.EnviarLog);
                                                                    }
                                                                    else
                                                                    {
                                                                        Int64 idConsumoProtheus = docProdRepo.GetIDConsumoProtheus(sessionT5, view.NumObra, materialAplicado.Codcpl, materialAplicado.Codmat, view.Projeto, view.CodCt, usuarioHubsoft, detalheHubsoft, this.EnviarLog);

                                                                        if (idConsumoProtheus != 0)
                                                                        {
                                                                            docProdRepo.MarcarExcecao(sessionT5, idConsumoProtheus, envio, (string.Join(" | ", erros)));
                                                                        }
                                                                    }

                                                                }



                                                            }


                                                        }
                                                    }
                                                    


                                                }
                                            }

                                            #endregion

                                            string id_cliente_servico = onuAtualizadaHubsoft?.LoginRadius.CodigoAuxiliar;



                                            var dadosBodyReqHabilitarContrato = new HabilitarContratoHubsoft
                                            {
                                                motivo_habilitacao = "HABILITANDO SERVIÇO API",
                                                migrar_atendimentos = true,

                                            };
                                            


                                            var habilitarServico = ProvisionamentoHubsoft.HabilitarServico(dadosIntegracaoHubsoft.UrlApi,
                                            loginHubsoft, dadosBodyReqHabilitarContrato, id_cliente_servico, usuarioHubsoft, detalheHubsoft, this.EnviarLog);

                                            if (habilitarServico.status != "success" &&
                                            !habilitarServico.msg.ToUpperInvariant().Contains("JÁ SE ENCONTRA HABILITADO!"))
                                            {
                                                erros.Add($"Erro habilitar serviço: {Sinapse.Util.JsonService.SerializeObject(habilitarServico)}");
                                            }

                                            var provisionamentoHubsoft = ProvisionamentoHubsoft.ConfigurarAutenticacao(dadosIntegracaoHubsoft.UrlApi, dadosBodyRequest, 
                                                loginHubsoft, usuarioHubsoft, detalheHubsoft, this.EnviarLog);


                                            if (provisionamentoHubsoft.status != "success")
                                            {
                                                erros.Add($"Erro configurar auth: {Sinapse.Util.JsonService.SerializeObject(provisionamentoHubsoft)}");
                                            }

                                            if(!String.IsNullOrEmpty(onu.LoginRadius.Senha_WIFI) && !String.IsNullOrEmpty(onu.LoginRadius.SSID_WIFI))
                                            {
                                                var atualizaCredencialBody = new AtualizaCredencialWifi
                                                {
                                                    id_cliente_servico = id_cliente_servico,
                                                    descricao = "ACESSO ROTEADOR",
                                                    usuario = onu.LoginRadius.SSID_WIFI,
                                                    senha = onu.LoginRadius.Senha_WIFI,

                                                };

                                                var atualizaCredencialWifi = ProvisionamentoHubsoft.AtualizaCredenciaisWifi(dadosIntegracaoHubsoft.UrlApi, 
                                                loginHubsoft, atualizaCredencialBody, usuarioHubsoft, detalheHubsoft, this.EnviarLog);

                                                if (atualizaCredencialWifi.status != "success")
                                                {
                                                    erros.Add($"Erro ao atualizar credencial Wifi: {Sinapse.Util.JsonService.SerializeObject(provisionamentoHubsoft)}");
                                                }
                                            }
                                          
                                            if (erros.Count > 0)
                                            {
                                                throw new EAnielException(string.Join(" | ", erros));
                                            }

                                            break;


                                        default:
                                            break;
                                    }
                                }
                            }                            
                            catch(Exception ex)
                            {
                                CONSTANTES.log4Net.Error(ex.Message, ex);
                                //CUIDADO AO ALTERAR MENSAGEM DE RETORNO, PODE AFETAR O TOUCH. MANTER "sucesso" NA MENSAGEM
                                retorno.Mensagens.Add(new PostApiMensagem
                                {
                                    Tipo = PostApiMensagemType.Info,
                                    Mensagem = $"ONU de serial {view.Mac_Serial} provisionada com sucesso, porém algo no processo da integração externa falhou."
                                                        
                                });
                            }
                            
                            #endregion                           

                            if(!retorno.Mensagens.Exists(p => p.Tipo == PostApiMensagemType.Info && p.Mensagem.ToUpperInvariant().Contains("SUCESSO")))
                            {
                                //CUIDADO AO ALTERAR MENSAGEM DE RETORNO, PODE AFETAR O TOUCH. MANTER "sucesso" NA MENSAGEM
                                retorno.Mensagens.Add(new PostApiMensagem
                                {
                                    Tipo = PostApiMensagemType.Info,
                                    Mensagem = $"ONU de serial {view.Mac_Serial} foi autorizada com sucesso"
                                });
                            }

                            if(onu.OLT?.Config_PPPOE_Manual == true)
                            {
                                retorno.Mensagens.Add(new PostApiMensagem
                                {
                                    Tipo = PostApiMensagemType.Info,
                                    Mensagem = $"PPPOE deve ser configurado manualmente para a ONU de serial {view.Mac_Serial}"
                                });
                            }

                                if (view.Origem_Mobile == false)
                                {
                                    var getResultadoCodmat = await GetCodMatByCodComplementar(sessionT5, onu.Serial, onu.SerialConvertido);
                                    var codmat = getResultadoCodmat.codmat;
                                    if (!String.IsNullOrEmpty(codmat))
                                    {

                                        using (var transactionT5 = sessionT5.BeginTransaction())
                                        {
                                            try
                                            {
                                                IInventarioClienteAnielRepository inventarioClienteAnielRepository = new InventarioClienteAnielRepository(sessionT5, transactionT5);
                                                InventarioClienteAnielBusiness inventarioClienteAnielBusiness = new InventarioClienteAnielBusiness(inventarioClienteAnielRepository);
                                                IClienteAnielRepository clienteAnielRepository = new ClienteAnielRepository(sessionT5);
                                                ClienteAnielBusiness clienteAnielBusiness = new ClienteAnielBusiness(clienteAnielRepository);

                                       
                                        
                                                    var clienteAniel = clienteAnielBusiness.GetClientebyCPFCNPJ(onu?.Cliente_PlanoProduto?.Cliente.CpfCnpj);


                                                    var consultaExisteInventario = await inventarioClienteAnielBusiness.ExisteInventarioPendenteAvaliacao(clienteAniel.CPF, codmat.Trim(), onu.Serial);
                                                    if (consultaExisteInventario == false)
                                                    {
                                                        await inventarioClienteAnielBusiness.GerarInventarioCliente(clienteAniel, User.Identity.GetUserId(), codmat.Trim(), onu.Serial);
                                                        await transactionT5.CommitAsync();

                                                    }

                                               
                                                                                                                                      
                                            }
                                            catch (Exception ex)
                                            {
                                                await transactionT5.RollbackAsync();
                                                CONSTANTES.log4Net.Error(ex.Message, ex);

                                            }
                                        }

                                    }

                                }                                                                                         
                            //transaction.Commit();
                        }
                        catch(AutorizarONUException exAutorizar)
                        {
                            CONSTANTES.log4Net.Error(exAutorizar.Message, exAutorizar);
                            retorno.Mensagens.Add(new PostApiMensagem
                            {
                                Tipo = PostApiMensagemType.Erro,
                                Mensagem = exAutorizar.Message
                            });
                        }
                        catch (Exception ex)
                        {
                            CONSTANTES.log4Net.Error(ex.Message, ex);
                            //transaction.Rollback();
                            if (onu.Id != 0)
                            {
                                var onu_remover = await repoONU.selectFirst(o => o.Id == onu.Id);

                                if (onu_remover != null)
                                    await repoONU.Remove(onu_remover);
                            }

                            retorno.Mensagens.Add(new PostApiMensagem
                            {
                                Tipo = PostApiMensagemType.Erro,
                                Mensagem = ex.Message
                            });
                        }
                        //}
                        //});

                        //taskList.Add(taskONU);
                    }

                    retorno.Ok = true;
                    //Task.WaitAll(taskList.ToArray());
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("api/AutorizarONUApiController/TesteProvisionamentoVoalle")]
        public async Task<PostApiRetorno> TesteProvisionamentoVoalle(string Mac_Serial)
        {
            PostApiRetorno retorno = new PostApiRetorno();
            try
            {
                using(var session = HibernateUtil.GetSession())
                using (var repoONU = new Conexao<ONU>(session))
                {
                    IIntegracaoExternaRepository repoIntegracaoExterna = new IntegracaoExternaRepository(session);
                    var integracaoExternaBusiness = new IntegracaoExternaBusiness(repoIntegracaoExterna);
                    IntegracoesExternas externas =
                    integracaoExternaBusiness.GetIntegracaoExterna(EnumTipoIntegracao.PROVISIONAMENTO);

                    if (externas != null)
                    {
                        switch (externas.IntegracaoExterna)
                        {
                            case IntegracaoExterna.VOALLE:
                                var onu = await repoONU.selectFirst(p => p.Mac == Mac_Serial || p.Serial == Mac_Serial);
                                IVoalleRepository repoVoalle = new VoalleApiRepository(externas);

                                var voalleBusiness = new ConnectBusiness.MK.VoalleBusiness(null, repoVoalle);
                                var retornoVoalle = voalleBusiness.UpdateConnection(onu);

                                if (!retornoVoalle.success || !retornoVoalle.response)
                                {
                                    throw new EAnielException($"Erro na atualização da conexão no Voalle. Retorno do Voalle: {Sinapse.Util.JsonService.SerializeObject(retornoVoalle)}");
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);                
                retorno.Mensagens.Add(new PostApiMensagem
                {
                    Tipo = PostApiMensagemType.Erro,
                    Mensagem = ex.Message
                });
            }

            retorno.Mensagens.Add(new PostApiMensagem
            {
                Tipo = PostApiMensagemType.Erro,
                Mensagem = $"ONU de serial {Mac_Serial} foi atualizada com sucesso no Voalle"
            });
            return retorno;
        }

        

        //Para testar este método, primeiro importe os dados de uma ONU
        //via Listagem ONU > Importar ONUs
        [AllowAnonymous]
        [HttpPost]
        [Route("api/AutorizarONUApiController/TesteProvisionamentoHubsoft")]
        public async Task<PostApiRetorno> TesteProvisionamentoHubsoft(Aniel.Connect.Models.AutorizarONURequest request)
        {
            PostApiRetorno retorno = new PostApiRetorno();
            string serial = "";
            try
            {
                using (var session = HibernateUtil.GetSession())
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                using (var repoONU = new Conexao<ONU>(session))
                {
                    foreach(var view in request.ListONU)
                    {
                        IIntegracaoExternaRepository repoIntegracaoExterna = new IntegracaoExternaRepository(session);
                        var integracaoExternaBusiness = new IntegracaoExternaBusiness(repoIntegracaoExterna);
                        IntegracoesExternas externas =
                        integracaoExternaBusiness.GetIntegracaoExterna(EnumTipoIntegracao.PROVISIONAMENTO);

                        if (externas != null)
                        {
                            switch (externas.IntegracaoExterna)
                            {
                                case IntegracaoExterna.HUBSOFT:
                                    var erros = new List<string>();
                                    var onu = await repoONU.selectFirst(p => p.Mac == view.Mac_Serial || p.Serial == view.Mac_Serial);
                                    serial = onu.Serial;
                                    IntegracoesExternas dadosIntegracaoHubsoft = integracaoExternaBusiness.GetIntegracaoExterna(EnumTipoIntegracao.PROVISIONAMENTO, IntegracaoExterna.HUBSOFT);

                                    if (String.IsNullOrEmpty(dadosIntegracaoHubsoft.UrlApi) ||
                                         String.IsNullOrEmpty(dadosIntegracaoHubsoft.Usuario) ||
                                         String.IsNullOrEmpty(dadosIntegracaoHubsoft.Senha) ||
                                         String.IsNullOrEmpty(dadosIntegracaoHubsoft.ApiSecret) ||
                                         String.IsNullOrEmpty(dadosIntegracaoHubsoft.TokenAcesso))
                                    {
                                        string message = "Integração com a API do HubSoft não foi configurada corretamente, confira os acessos.";
                                        throw new EAnielException(message);
                                    }

                                    var loginHubsoft = new LoginHubsoftModel
                                    {
                                        client_id = dadosIntegracaoHubsoft.TokenAcesso,
                                        client_secret = dadosIntegracaoHubsoft.ApiSecret,
                                        username = dadosIntegracaoHubsoft.Usuario,
                                        password = dadosIntegracaoHubsoft.Senha
                                    };

                                    var onuAtualizadaHubsoft = onu.OntID.HasValue ? onu : await repoONU.selectFirst(p => p.Mac == view.Mac_Serial || p.Serial == view.Mac_Serial);

                                    var dadosBodyRequest = new ConfigurarAutenticacaoHubsoft
                                    {
                                        id_cliente_servico = onuAtualizadaHubsoft?.LoginRadius.CodigoAuxiliar,
                                        id_interface_conexao = onuAtualizadaHubsoft?.PON?.CodigoAuxiliar,
                                        phy_addr = onuAtualizadaHubsoft?.Serial ?? onuAtualizadaHubsoft?.Mac

                                    };

                                    var dadosBodyReqHabilitarContrato = new HabilitarContratoHubsoft
                                    {
                                        motivo_habilitacao = "HABILITANDO SERVIÇO API",
                                        migrar_atendimentos = true

                                    };

                                    var requestLimpezaMac = new LimpezaMacHubsoft
                                    {
                                        id_cliente_servico = onuAtualizadaHubsoft?.LoginRadius.CodigoAuxiliar
                                    };

                                   
                                    string usuarioHubsoft = string.IsNullOrEmpty(view.Usuario) ? string.Empty : view.Usuario;
                                    string detalheHubsoft = string.IsNullOrEmpty(view.Detalhe) ? $"OPERAÇÃO REALIZADA SEM O.S -> SERIAL: {onu?.Serial}" : view.Detalhe;

                                    var limpezaMac = ProvisionamentoHubsoft.LimpezaMac(dadosIntegracaoHubsoft.UrlApi, requestLimpezaMac,
                                   loginHubsoft, usuarioHubsoft, detalheHubsoft, this.EnviarLog);

                                    #region Movimento material cliente HubSoft
                                    if (CONSTANTES.AtivaMovimentacaoMat)
                                    {
                                        if (view.Origem_Mobile == true || !String.IsNullOrEmpty(view.Num_Os))
                                        {
                                            IDocumentoProducaoAnielRepository docProdRepo = new DocumentoProducaoAnielRepository(sessionT5, sessionT5);
                                            string usuarioLogin = "";

                                            if (view.Origem_Mobile == false)
                                            {
                                                var (codmat, codcpl) = await GetCodMatByCodComplementar(sessionT5, onu.Serial, onu.SerialConvertido);
                                                view.CodMat = codmat;
                                                view.CodCpl = codcpl;

                                                var dadosOS = await docProdRepo.GetDadosOSByNumOs(sessionT5, view.Num_Os, detalheHubsoft, usuarioHubsoft, this.EnviarLog);

                                                view.NumObra = dadosOS.num_obra;
                                                view.CodCt = dadosOS.codct;
                                                view.Projeto = dadosOS.projeto;
                                                usuarioLogin = User.Identity.GetUserId();
                                                usuarioHubsoft = User.Identity.GetUserId();
                                                detalheHubsoft = view.Num_Os;


                                            }

                                            
                                            if (view.Origem_Mobile == true)
                                            {
                                                usuarioLogin = usuarioHubsoft;                                             
                                            }

                                            bool saldoCliente = false;

                                            //Verifica se a ONU consta no saldo do cliente
                                            if (view.IdCliente == null)
                                            {
                                                var cpfCnpj = !string.IsNullOrWhiteSpace(view.NumCpfCnpj)
                                                    ? view.NumCpfCnpj
                                                    : onu?.Cliente_PlanoProduto?.Cliente.CpfCnpj;

                                                IClienteAnielRepository clienteAnielRepository = new ClienteAnielRepository(sessionT5);
                                                ClienteAnielBusiness clienteAnielBusiness = new ClienteAnielBusiness(clienteAnielRepository);
                                                var clienteAniel = clienteAnielBusiness.GetClientebyCPFCNPJ(cpfCnpj);
                                                view.IdCliente = clienteAniel.Cod_Cliente;
                                            }

                                            var getSaldo = await getSaldoVincCliente(sessionT5, (long)view.IdCliente);
                                            var verificaSerial = getSaldo.Any(p => p.cpl == view.CodCpl && p.CODMAT == view.CodMat);
                                            saldoCliente = verificaSerial;

                                            if (!saldoCliente)
                                            {
                                                var osAniel = await docProdRepo.GetDadosOSByNumObra(sessionT5, view.NumObra, detalheHubsoft, usuarioHubsoft, this.EnviarLog);

                                                if (osAniel == null)
                                                {
                                                    throw new Exception("O.S não encontrada na base de dados do Aniel.");
                                                }

                                                string idHubsoft = osAniel.adicional_1;

                                                if (string.IsNullOrEmpty(idHubsoft))
                                                {
                                                    idHubsoft = osAniel.id_hubsoft;

                                                    if (string.IsNullOrEmpty(idHubsoft))
                                                    {
                                                        throw new Exception("ID da O.S está vazio.");
                                                    }

                                                }

                                                docProdRepo.AdicionarEquipamento(sessionT5, detalheHubsoft, usuarioHubsoft, view.CodCpl, view.CodMat, osAniel.equipe, view.NumObra, osAniel);

                                                var osHubsoft = OrdemServicoHubSoftDao.GetOrdemServicoById(dadosIntegracaoHubsoft.UrlApi, Convert.ToInt32(idHubsoft), loginHubsoft, detalheHubsoft, usuarioHubsoft, this.EnviarLog);

                                                if (osHubsoft != null)
                                                {
                                                    bool provisionamento = true;
                                                    var materialAplicado = docProdRepo.GetAplicacaoNaoIntegrados(sessionT5, view.NumObra, view.CodCpl, view.CodMat, view.Projeto, provisionamento, usuarioHubsoft, detalheHubsoft, this.EnviarLog);

                                                    if (!String.IsNullOrEmpty(materialAplicado?.Codcpl))
                                                    {
                                                        if (string.IsNullOrEmpty(materialAplicado.DepositoTecnico))
                                                        {
                                                            erros.Add($"Local de estoque do técnico {materialAplicado.Equipe} não informado");
                                                        }

                                                        string envio = "";
                                                        string retornoMovimento = null;
                                                        var estoqueHubSoft = EstoqueHubSoftDao.GetEstoque(dadosIntegracaoHubsoft.UrlApi, materialAplicado.DepositoTecnico, loginHubsoft, detalheHubsoft, usuarioHubsoft, this.EnviarLog);

                                                        if (estoqueHubSoft != null)
                                                        {
                                                            if (osHubsoft.id_pop != null)
                                                            {
                                                                var model = docProdRepo.BindMateriaisAplicacaoPop(materialAplicado, loginHubsoft, dadosIntegracaoHubsoft, detalheHubsoft, usuarioHubsoft, this.EnviarLog);
                                                                if (model != null)
                                                                {
                                                                    model.id_pop = osHubsoft.id_pop;
                                                                    envio = JsonConvert.SerializeObject(model);
                                                                    retornoMovimento = EstoqueHubSoftDao.MovimentoSaidaPop(model, loginHubsoft, dadosIntegracaoHubsoft.UrlApi, detalheHubsoft, usuarioHubsoft, this.EnviarLog);
                                                                    if (retornoMovimento.ToUpperInvariant().Contains("ERRO".ToUpperInvariant()))
                                                                    {
                                                                        erros.Add(retornoMovimento);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    erros.Add($"Não foi possível encontrar o patrimônio de serial {view.CodCpl}");
                                                                }

                                                            }

                                                            if (osHubsoft.id_cliente_servico != 0)
                                                            {
                                                                var model = docProdRepo.BindMateriaisAplicacao(materialAplicado, osHubsoft, loginHubsoft, dadosIntegracaoHubsoft, detalheHubsoft, usuarioHubsoft, this.EnviarLog);
                                                                if (model != null)
                                                                {
                                                                    model.id_cliente_servico = osHubsoft.id_cliente_servico;
                                                                    envio = JsonConvert.SerializeObject(model);
                                                                    retornoMovimento = EstoqueHubSoftDao.MovimentoSaidaClienteServico(model, loginHubsoft, dadosIntegracaoHubsoft.UrlApi, detalheHubsoft, usuarioHubsoft, this.EnviarLog);
                                                                    if (retornoMovimento.ToUpperInvariant().Contains("ERRO".ToUpperInvariant()))
                                                                    {
                                                                        erros.Add(retornoMovimento);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    erros.Add($"Não foi possível encontrar o patrimônio de serial {view.CodCpl}");
                                                                }
                                                            }

                                                            if (erros.Count() == 0)
                                                            {

                                                                Int64 idConsumoProtheus = docProdRepo.GetIDConsumoProtheus(sessionT5, view.NumObra, materialAplicado.Codcpl, materialAplicado.Codmat, view.Projeto, view.CodCt, usuarioHubsoft, detalheHubsoft, this.EnviarLog);

                                                                if(idConsumoProtheus != 0)
                                                                {
                                                                    docProdRepo.MarcarImportados(sessionT5, idConsumoProtheus, envio, retornoMovimento);
                                                                }

                                                                await docProdRepo.MarcarIntegrado(sessionT5, view.CodCt, view.Projeto, view.NumObra, materialAplicado.Codmat, materialAplicado.Codcpl, provisionamento, detalheHubsoft, usuarioHubsoft, this.EnviarLog);
                                                            }
                                                            else
                                                            {
                                                                Int64 idConsumoProtheus = docProdRepo.GetIDConsumoProtheus(sessionT5, view.NumObra, materialAplicado.Codcpl, materialAplicado.Codmat, view.Projeto, view.CodCt, usuarioHubsoft, detalheHubsoft, this.EnviarLog);

                                                                if (idConsumoProtheus != 0)
                                                                {
                                                                    docProdRepo.MarcarExcecao(sessionT5, idConsumoProtheus, envio, (string.Join(" | ", erros)));
                                                                }
                                                            }

                                                        }



                                                    }


                                                }
                                            }
                                                                             
                                        }
                                    }
                                    #endregion
                                   
                                    string id_cliente_servico = onuAtualizadaHubsoft?.LoginRadius.CodigoAuxiliar;
                                    var habilitarServico = ProvisionamentoHubsoft.HabilitarServico(dadosIntegracaoHubsoft.UrlApi,
                                    loginHubsoft, dadosBodyReqHabilitarContrato, id_cliente_servico, usuarioHubsoft, detalheHubsoft, this.EnviarLog);

                                    if (habilitarServico.status != "success" &&
                                    !habilitarServico.msg.ToUpperInvariant().Contains("JÁ SE ENCONTRA HABILITADO!"))
                                    {
                                        erros.Add($"Erro habilitar serviço: {Sinapse.Util.JsonService.SerializeObject(habilitarServico)}");
                                    }


                                    var provisionamentoHubsoft = ProvisionamentoHubsoft.ConfigurarAutenticacao(dadosIntegracaoHubsoft.UrlApi, dadosBodyRequest,
                                        loginHubsoft, usuarioHubsoft, detalheHubsoft, this.EnviarLog);


                                    if (provisionamentoHubsoft.status != "success")
                                    {
                                        erros.Add($"Erro configurar auth: {Sinapse.Util.JsonService.SerializeObject(provisionamentoHubsoft)}");
                                    }

                                    if (erros.Count > 0)
                                    {
                                        throw new EAnielException(string.Join(" | ", erros));
                                    }

                                    retorno.Mensagens.Add(new PostApiMensagem
                                    {
                                        Tipo = PostApiMensagemType.Erro,
                                        Mensagem = $"ONU de serial {serial} foi atualizada com sucesso no Hubsoft"
                                    });

                                    break;


                                default:
                                    break;
                            }
                        }
                    }
               
                }

            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.Mensagens.Add(new PostApiMensagem
                {
                    Tipo = PostApiMensagemType.Erro,
                    Mensagem = ex.Message
                });
            }

          
            return retorno;
        }

        #region Preencher Campos da ONU

        private async Task<ONU> PreencherCamposONU(ONU onu, AutorizarONUListModel view)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var repoPerfilAtivacao = new Conexao<PerfilAtivacao>(session))
            using (var repoPlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
            using (var repoLogin = new Conexao<Model.nhibernate.LoginRadius>(session))
            using (var repoOLT = new Conexao<OLT>(session))
            using (var repoSlot = new Conexao<Slot>(session))
            using (var repoPON = new Conexao<PON>(session))
            using (var repoONU = new Conexao<ONU>(session))
            {
                //var idPlanoProduto = await GetIdClientePlanoProduto(sessionT5, view.Mac_Serial.ToUpperInvariant());
                //var idPerfilAtivacao = await GetIdPerfilAtivacao(sessionT5, view.Mac_Serial.ToUpperInvariant());
                long idPerfilAtivacao = 0;
                long idPlanoProduto = 0;

               
                if (view.Pode_Autorizar_Sem_OS || view.Origem_Mobile)
                {
                    idPlanoProduto = view.Id_Cliente_Plano_Produto;                    
                }
                else
                {
                    throw new Exception($"A ONU de serial {view.Mac_Serial} não possui uma OS associada");
                }

                if (view.Id_Perfil_Ativacao.HasValue && idPerfilAtivacao == 0)
                {
                    idPerfilAtivacao = view.Id_Perfil_Ativacao.Value;
                }

                if (idPerfilAtivacao == 0)
                {
                    throw new Exception($"A ONU de serial {view.Mac_Serial} não possui perfil de ativação vinculado");
                }

                onu.Cliente_PlanoProduto = await repoPlanoProduto.selectFirst(p => p.Id == idPlanoProduto);

                onu.PerfilAtivacao = await repoPerfilAtivacao.selectFirst(p => p.Id == idPerfilAtivacao);

                onu.ScriptOLT = onu.PerfilAtivacao.ScriptOLT;
                onu.ScriptONU = onu.PerfilAtivacao.ScriptONU;

                onu.IpGerencia = onu.PerfilAtivacao.IpGerencia;

                onu.PortaTelnet = onu.PerfilAtivacao.PortaTelnet;
                onu.PortaHttp = onu.PerfilAtivacao.PortaHttp;
                onu.SenorId = onu.PerfilAtivacao.SenorId;
                onu.GemPort = onu.PerfilAtivacao.GemPort;
                onu.ServicePort = onu.PerfilAtivacao.ServicePort;

                onu.Login = onu.PerfilAtivacao.Login;
                onu.Senha = onu.PerfilAtivacao.Senha;
                onu.Descricao = view.Desc_ONU;

                //Regra criada para procurar um determinado login caso
                //existam vários logins radius vinculados ao mesmo plano produto.                

                if(view.Origem_Mobile == true && !String.IsNullOrEmpty(view.PPPOE))
                {   

                    onu.LoginRadius = await repoLogin.selectFirst(p => p.Login == view.PPPOE);

                    if(onu.LoginRadius == null)
                    {
                        onu.LoginRadius = await repoLogin.selectFirst(p => p.Cliente_PlanoProduto.Id == idPlanoProduto);
                    }
                }

                else if (view.Origem_Mobile == false && view.Id_Login_Radius != null)
                {
                    onu.LoginRadius = await repoLogin.selectFirst(p => p.Id == view.Id_Login_Radius);
                }

                else
                {
                    onu.LoginRadius = await repoLogin.selectFirst(p => p.Cliente_PlanoProduto.Id == idPlanoProduto);
                }


                if (onu.LoginRadius == null)
                {
                    throw new Exception($"O plano associado à ONU de serial {view.Mac_Serial} não possui login registrado");
                }

                if(view.Origem_Mobile == true)
                {
                    var loginRadiusAtualizado = await GetPPPOEAtualizadoIntegracao(session, sessionT5, repoLogin, onu.LoginRadius, view.Detalhe, view.NumObra, view.Usuario);

                    if (loginRadiusAtualizado.result && loginRadiusAtualizado.valor != null)
                        onu.LoginRadius = loginRadiusAtualizado.valor;
                }
                
                var onuExistente = await repoONU.selectFirst(p => p.Cliente_PlanoProduto.Id == idPlanoProduto);

                if ( onuExistente != null)
                {
                    LogIntegracaoAtivacao($"O plano produto {idPlanoProduto} já está vinculado à ONU de ID: {onuExistente.Id}, Serial: {onuExistente.Serial} e login: {onuExistente.LoginRadius}");
                    throw new Exception($"O login {onu.LoginRadius.Login} já está vinculado a outra ONU");
                }

                if (onu.Cliente_PlanoProduto.Status != Status_PlanoProduto.Aguardando_Criacao_OS && 
                    onu.Cliente_PlanoProduto.Status != Status_PlanoProduto.Aguardando_Instalacao && 
                    onu.Cliente_PlanoProduto.Status != Status_PlanoProduto.Habilitado) 
                {
                    throw new Exception($"Status do plano vinculado ao login não permite o provisionamento. Status: {(onu.Cliente_PlanoProduto.Status.HasValue ? Core.Helpers.HtmlEnumExtensions.GetEnumDescription<Status_PlanoProduto>(onu.Cliente_PlanoProduto.Status.Value) : "Sem Status")}");
                }

                onu.OLT = await repoOLT.selectFirst(p => p.Id == view.Id_OLT);

                if (onu.OLT == null)
                {
                    throw new Exception($"A ONU de serial {view.Mac_Serial} não possui OLT cadastrada");
                }

                onu.Slot = await repoSlot.selectFirst(p => p.OLT == onu.OLT &&
                                                           p.NumSlot != null &&
                                                           p.NumSlot.Value == Convert.ToInt64(view.Slot));

                if (onu.Slot == null)
                {
                    throw new Exception($"A ONU de serial {view.Mac_Serial} não possui Slot cadastrada");
                }

                onu.PON = await repoPON.selectFirst(p => p.OLT == onu.OLT &&
                                                         p.Slot == onu.Slot &&
                                                         p.Num_PON != null &&
                                                         p.Num_PON.Value == Convert.ToInt64(view.PON));

                if (onu.PON == null)
                {
                    throw new Exception($"A ONU de serial {view.Mac_Serial} não possui PON cadastrada");
                }

                if(onu.OLT.ConverteOlt == Status_Default.Ativo)
                {                   
                    onu.SerialConvertido = !String.IsNullOrEmpty(view.Mac_Serial_Convertido) ? view.Mac_Serial_Convertido : await ConverterSerial(sessionT5, onu.Serial);

                    if (string.Equals(onu.SerialConvertido, onu.Serial, StringComparison.OrdinalIgnoreCase))
                    {
                        onu.SerialConvertido = await ConverteONUVendorHex(session, onu.Serial);
                    }
                }

                //Prioridade de atribuição da VLAN Uplink:
                //Vlan do Cliente Plano/Produto > Vlan da Pon > Vlan da OLT
                onu.VLAN_Uplink = onu.Cliente_PlanoProduto?.VLAN_Uplink.HasValue == true ? onu.Cliente_PlanoProduto?.VLAN_Uplink :
                    onu.PON.VLAN_Uplink.HasValue == true ? onu.PON.VLAN_Uplink : onu.OLT.VLAN_Uplink;

                onu.VLAN_PPPOE = onu.PON.VLAN_PPPOE;
                onu.VLAN_DHCP = onu.PON.VLAN_DHCP;
                onu.VLAN_IPTV = onu.PON.VLAN_IPTV;
                onu.VLAN_VOIP = onu.PON.VLAN_VOIP;
                onu.VLAN_TR69 = onu.PON.VLAN_TR69;
                onu.VLAN_Outros = onu.PON.VLAN_Outros;
                onu.VLAN_Profile = onu.PON.VLAN_Profile;

                //onu.LoginRadius.EnderecoIPV4 = onu.IpGerencia;

                //onu.Wireless_SSID = view.Wireless_SSID;
                //onu.Senha_WPA2 = view.Senha_WPA2;
               
                if(view.Origem_Mobile == false)
                {
                    onu.LoginRadius.SSID_WIFI = view.Wireless_SSID;
                    onu.LoginRadius.Senha_WIFI = view.Senha_WPA2;
                    if(onu.PerfilAtivacao.ObrigaWifi == true)
                    {
                        if(String.IsNullOrEmpty(view.Wireless_SSID))
                        {
                            throw new Exception("Perfil de ativação com Obriga Wifi ativo. Campo SSID WIFI é obrigatório.");
                        }

                        if (String.IsNullOrEmpty(view.Senha_WPA2))
                        {
                            throw new Exception("Perfil de ativação com Obriga Wifi ativo. Campo Senha WIFI é obrigatório.");
                        }

                        if (!String.IsNullOrEmpty(view.Senha_WPA2))
                        {
                            if (view.Senha_WPA2.Length < 8)
                            {
                                throw new Exception("Campo Senha WIFI deve ter pelo menos 8 caracteres.");
                            }

                            if (view.Senha_WPA2.Contains(" "))
                            {
                                throw new Exception("Campo Senha WIFI não pode conter espaços.");
                            }
                        }

                    }
                }
                else
                {
                    onu.LoginRadius.SSID_WIFI = onu.LoginRadius.SSID_WIFI;
                    onu.LoginRadius.Senha_WIFI = onu.LoginRadius.Senha_WIFI;
                }
               

                onu.LoginRadius.POP = onu.OLT.POP;

               // onu.LoginRadius.OLT = onu.OLT;
                //onu.LoginRadius.Slot = onu.Slot;
                //onu.LoginRadius.PON = onu.PON;

                onu.LoginRadius.MAC = onu.Mac;
                onu.LoginRadius.Serial = onu.Serial;
                onu.Tipo_Identificacao = onu.OLT.Tipo_Identificacao;
            }

            return onu;
        }

        private async Task<long> GetIdPerfilAtivacao(ISession sessionT5, string macSerial)
        {
            long idPerfilAtivacao = 0;

            using (var com = sessionT5.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> param = new Dictionary<string, object>();

                sql.AppendLine("SELECT FIRST 1 PERFIL.ID_PERFIL_ATIVACAO AS ID_PERFIL_ATIVACAO ");
                sql.AppendLine("FROM TB_MATERIAL_TAREFA MAT_TRF ");

                sql.AppendLine("INNER JOIN TB_MATERIAL MAT ");
                sql.AppendLine("ON(MAT.CODMAT = MAT_TRF.CODMAT) ");

                sql.AppendLine("INNER JOIN TB_CONFIG_PERFIL_ATIVACAO PERFIL ");
                sql.AppendLine("ON(MAT_TRF.CODMAT = PERFIL.CODMAT) ");

                sql.AppendLine("WHERE MAT_TRF.CODCPL = @MAC_SERIAL ");
                sql.AppendLine("ORDER BY MAT_TRF.DATA_APLIC DESC ");

                param.Add("@MAC_SERIAL", macSerial);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, param, sessionT5.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        idPerfilAtivacao = Convert.ToInt64(reader["ID_PERFIL_ATIVACAO"]);
                    }
                }
            }

            return idPerfilAtivacao;
        }

        private async Task<long> GetIdClientePlanoProduto(ISession sessionT5, string macSerial)
        {
            long idClientePlanoProduto = 0;

            using (var com = sessionT5.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder();
                Dictionary<string, object> param = new Dictionary<string, object>();

                sql.AppendLine("SELECT FIRST 1 DOC_PROD.ID_CLIENTE_PLANOPRODUTOCONNECT ");
                sql.AppendLine("FROM TB_MATERIAL_TAREFA MAT_TRF ");

                sql.AppendLine("INNER JOIN TB_DOCUMENTO_PRODUCAO DOC_PROD ");
                sql.AppendLine("ON(DOC_PROD.NUM_DOC = MAT_TRF.NUM_DOC AND ");
                sql.AppendLine("    DOC_PROD.CODCT = MAT_TRF.CODCT AND ");
                sql.AppendLine("    DOC_PROD.PROJETO = MAT_TRF.PROJETO AND ");
                sql.AppendLine("    DOC_PROD.NUM_OBRA = MAT_TRF.NUM_OBRA) ");

                sql.AppendLine("WHERE MAT_TRF.CODCPL = @MAC_SERIAL ");
                sql.AppendLine("ORDER BY MAT_TRF.DATA_APLIC DESC");

                param.Add("@MAC_SERIAL", macSerial);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, param, sessionT5.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        var value = reader["ID_CLIENTE_PLANOPRODUTOCONNECT"];
                        if (value != DBNull.Value)
                        {
                            idClientePlanoProduto = Convert.ToInt64(value);
                        }
                    }
                }
            }

            return idClientePlanoProduto;
        }

        #endregion

        private string SubstituirVariaveis(ONU onu, string script)
        {
            var var_interface = String.Empty;
           

            switch (onu.OLT.Marca_Modelo)
            {
                case Marca_Modelo_OLT.Huawei:
                    var_interface = String.Format($"{onu.OLT.Subrack}/{onu.Slot.NumSlot}/{onu.PON.Num_PON}");                  
                    break;
                case Marca_Modelo_OLT.Fiberhome_A_B:
                    var_interface = String.Format($"{onu.OLT.Subrack}-{onu.Slot.NumSlot}-{onu.PON.Num_PON}");                   
                    break;
                case Marca_Modelo_OLT.Fiberhome_A1:
                    var_interface = String.Format($"{onu.OLT.Subrack}-{onu.Slot.NumSlot}-{onu.PON.Num_PON}");
                    break;
                case Marca_Modelo_OLT.Datacom:
                    var_interface = $"1/{onu.Slot.NumSlot}/{onu.PON.Num_PON}";
                    break;
                case Marca_Modelo_OLT.ZTE:
                    var_interface = $"1/{onu.Slot.NumSlot}/{onu.PON.Num_PON}";
                    break;
                case Marca_Modelo_OLT.Nokia:
                    var_interface = $"1/1/{onu.Slot.NumSlot}/{onu.PON.Num_PON}/{onu.OntID}";
                    break;
                default:
                    break;
            }
            return script.Replace("<CONTRATO_CLIENTE>", Convert.ToString(onu.Cliente_PlanoProduto.Cliente.Numero_Contrato)).
                          Replace("<GABINETE>", onu.OLT.Gabinete).
                          Replace("<GEM_PORT>", Convert.ToString(onu.GemPort)).

                          Replace("<IP_GERENCIA>", onu.IpGerencia).
                          Replace("<LOGIN_ONU_CLIENTE>", onu.Login).

                          Replace("<NOME>", onu.Cliente_PlanoProduto.Cliente.CpfCnpj).

                          Replace("<OLT_DOWNLOAD>", onu.Cliente_PlanoProduto.Plano.VelocidadeDownload).
                          Replace("<OLT_UPLOAD>", onu.Cliente_PlanoProduto.Plano.VelocidadeUpload).
                          Replace("<OLT_IP>", onu.OLT.IP).
                          Replace("<OLT_ID>", onu.OLT.NumOLT).

                          Replace("<ONU_MAC>", onu.OLT.ConverteOlt == Status_Default.Ativo ? onu.SerialConvertido : onu.Serial).
                          Replace("<ONU_TIPO>", onu.Descricao).

                          Replace("<PLANO_CLIENTE>", onu.Cliente_PlanoProduto.Plano.Descricao).

                          Replace("<PON_ID>", Convert.ToString(onu.PON.Num_PON)).

                          Replace("<PORTA_TELNET_ONU>", Convert.ToString(onu.PortaTelnet)).

                          Replace("<PPPOE_LOGIN>", onu.LoginRadius.Login).
                          Replace("<PPPOE_SENHA>", onu.LoginRadius.SenhaPPPOE).

                          Replace("<ROUTER_LOGIN>", onu.LoginRadius.UsuarioRoteador).
                          Replace("<ROUTER_SENHA>", onu.LoginRadius.SenhaRoteador).
                          Replace("<ROUTER_SSID>", onu.LoginRadius.SSID_WIFI).
                          Replace("<ROUTER_SENHA_WIFI>", onu.LoginRadius.Senha_WIFI).

                          Replace("<SENHA_ONU>", onu.Senha).
                          Replace("<SENOR_ID>", Convert.ToString(onu.SenorId)).
                          Replace("<SERVICE_PORT>", Convert.ToString(onu.ServicePort)).
                          Replace("<SLOT_ID>", Convert.ToString(onu.Slot.NumSlot)).
                          Replace("<VLAN_UPLINK>", Convert.ToString(onu.VLAN_Uplink)).
                          Replace("<VLAN_LOGIN>", Convert.ToString(onu.LoginRadius.VLAN)).
                          Replace("<VLAN_PROFILE>", Convert.ToString(onu.VLAN_Profile)).

                          Replace("<LINE_PROFILE>", Convert.ToString(onu.PerfilAtivacao.LineProfile)).
                          //Replace("<LINE_PROFILE>", Convert.ToString(onu.Slot.LineProfile ?? 0)).
                          //Replace("<INTERFACE>", Convert.ToString(onu.PON.Interface)).
                          Replace("<INTERFACE>", var_interface).
                          Replace("<SUBRACK>", onu.OLT.Subrack).
                          Replace("<PORTA_HTTP_ONU>", Convert.ToString(onu.PerfilAtivacao.PortaHttp));
        }

        /// <summary>
        ///     Retorna lista de críticas sobre dados pendentes para execução do script.
        /// </summary>
        private List<string> ValidarVariaveisScript(ONU onu, string script)
        {
            List<string> listaCamposOLT = new List<string>();
            List<string> listaCamposSlot = new List<string>();
            List<string> listaCamposPON = new List<string>();            
            List<string> listaCamposPerfilAtivacao = new List<string>();
            List<string> listaCamposLogin = new List<string>();
            List<string> listaCamposPlano = new List<string>();
            List<string> listaCamposOutro = new List<string>();
            List<string> listaRetorno = new List<string>();

            string titulo = "Os seguintes campos são necessários para a execução do script de ativação e estão pendentes de cadastro!";

            #region Campos OLT           
            if (script.Contains("<GABINETE>") && String.IsNullOrEmpty(onu.OLT.Gabinete))
                listaCamposOLT.Add("Gabinete");

            if (script.Contains("<OLT_IP>") && String.IsNullOrEmpty(onu.OLT.IP))
                listaCamposOLT.Add("Endereço IP da OLT");

            if (script.Contains("<OLT_ID>") && String.IsNullOrEmpty(onu.OLT.NumOLT))
                listaCamposOLT.Add("Número/Id da OLT na Interface");

            if (script.Contains("<SUBRACK>") && String.IsNullOrEmpty(onu.OLT.Subrack))
                listaCamposOLT.Add("Subrack");            

            if (listaCamposOLT.Count > 0)
            {
                listaRetorno.Add("Cadastro de OLT:");
                listaRetorno.AddRange(listaCamposOLT);
                listaRetorno.Add("\n");
            }
            #endregion

            #region Campos Slot            
            if (script.Contains("<SLOT_ID>") && !onu.Slot.NumSlot.HasValue)
                listaCamposSlot.Add("Número/Id do Slot na OLT");

            if (listaCamposSlot.Count > 0)
            {
                listaRetorno.Add("Cadastro de Slot:");
                listaRetorno.AddRange(listaCamposSlot);
                listaRetorno.Add("\n");
            }
            #endregion

            #region Campos PON
            if (script.Contains("<PON_ID>") && !onu.PON.Num_PON.HasValue)
                listaCamposPON.Add("Número/Id da PON no Slot");            

            if (script.Contains("<VLAN_PROFILE>") && String.IsNullOrEmpty(onu.VLAN_Profile))
                listaCamposPON.Add("Vlan Profile");

            if (listaCamposPON.Count > 0)
            {
                listaRetorno.Add("Cadastro de PON:");
                listaRetorno.AddRange(listaCamposPON);
                listaRetorno.Add("\n");
            }
            #endregion

            #region Campos Plano Produto
            if (script.Contains("<OLT_DOWNLOAD>") && String.IsNullOrEmpty(onu.Cliente_PlanoProduto.Plano.VelocidadeDownload))
                listaCamposPlano.Add("Velocidade de Download");

            if (script.Contains("<OLT_UPLOAD>") && String.IsNullOrEmpty(onu.Cliente_PlanoProduto.Plano.VelocidadeUpload))
                listaCamposPlano.Add("Velocidade de Upload");

            if (listaCamposPlano.Count > 0)
            {
                listaRetorno.Add("Cadastro de Plano/Produto:");
                listaRetorno.AddRange(listaCamposPlano);
                listaRetorno.Add("\n");
            }
            #endregion

            #region Campos Login Radius
            if (script.Contains("<PPPOE_LOGIN>") && String.IsNullOrEmpty(onu.LoginRadius.Login))
                listaCamposLogin.Add("Login PPPOE");

            if (script.Contains("<PPPOE_SENHA>") && String.IsNullOrEmpty(onu.LoginRadius.SenhaPPPOE))
                listaCamposLogin.Add("Senha PPPOE");

            if (script.Contains("<ROUTER_LOGIN>") && String.IsNullOrEmpty(onu.LoginRadius.UsuarioRoteador))
                listaCamposLogin.Add("Login do Roteador");

            if (script.Contains("<ROUTER_SENHA>") && String.IsNullOrEmpty(onu.LoginRadius.SenhaRoteador))
                listaCamposLogin.Add("Senha do Roteador");

            if (script.Contains("<ROUTER_SSID>") && String.IsNullOrEmpty(onu.LoginRadius.SSID_WIFI))
                listaCamposLogin.Add("SSID Wi-Fi");

            if (script.Contains("<ROUTER_SENHA_WIFI>") && String.IsNullOrEmpty(onu.LoginRadius.Senha_WIFI))
                listaCamposLogin.Add("Senha do Wi-Fi");

            if (script.Contains("<VLAN_LOGIN>") && String.IsNullOrEmpty(onu.LoginRadius.VLAN))
                listaCamposLogin.Add("Vlan Login");

            if (listaCamposLogin.Count > 0)
            {
                listaRetorno.Add("Cadastro de Login:");
                listaRetorno.AddRange(listaCamposLogin);
                listaRetorno.Add("\n");
            }
            #endregion

            #region Campos Perfil Ativacao
            if (script.Contains("<LINE_PROFILE>") && String.IsNullOrEmpty(onu.PerfilAtivacao.LineProfile))
                listaCamposPerfilAtivacao.Add("Line Profile");

            if (script.Contains("<PORTA_HTTP_ONU>") && !onu.PerfilAtivacao.PortaHttp.HasValue)
                listaCamposPerfilAtivacao.Add("Porta HTTP para acesso remoto");

            if (script.Contains("<GEM_PORT>") && !onu.GemPort.HasValue)
                listaCamposPerfilAtivacao.Add("Gem Port");

            if (script.Contains("<SENOR_ID>") && !onu.SenorId.HasValue)
                listaCamposPerfilAtivacao.Add("Senor Id");

            if (script.Contains("<SERVICE_PORT>") && !onu.ServicePort.HasValue)
                listaCamposPerfilAtivacao.Add("Service Port");

            if (script.Contains("<PORTA_TELNET_ONU>") && !onu.PortaTelnet.HasValue)
                listaCamposPerfilAtivacao.Add("Porta Telnet");

            if (listaCamposPerfilAtivacao.Count > 0)
            {
                listaRetorno.Add("Cadastro de Perfil de Ativação:");
                listaRetorno.Add("\n");
                listaRetorno.AddRange(listaCamposPerfilAtivacao);                
            }
            #endregion

            #region Campos Outros
            if (script.Contains("<VLAN_UPLINK>") && !onu.VLAN_Uplink.HasValue)
                listaCamposOutro.Add("Vlan Uplink");

            if (listaCamposOutro.Count > 0)
            {
                listaRetorno.Add("Necessário cadastrar ao menos em um dos seguintes recursos - OLT, PON ou Configurações de Conexão por Cliente/Plano:");
                listaRetorno.Add("\n");
                listaRetorno.AddRange(listaCamposOutro);
            }
            #endregion

            if (listaRetorno.Count > 0)
                listaRetorno.Insert(0, titulo);

            return listaRetorno;
        }

        #region Conversão de Serial
        /// <summary>
        ///     Consulta se o material/serial possui conversão e em caso positivo, realiza a conversão fazendo o DE PARA.
        /// </summary>       
        public async Task<String> ConverterSerial(ISession sessionT5, String serial)
        {
            String serialConvertido = null;

            StringBuilder sql = new StringBuilder();
            Dictionary<String, object> param = new Dictionary<string, object>();

            List<ConversaoSerial> lista = new List<ConversaoSerial>();

            using (var com = sessionT5.Connection.CreateCommand())
            {               
                sql.AppendLine(" SELECT A.* FROM TB_CONVERSAO_MODELO_ATIVACAO A ");
                sql.AppendLine(" INNER JOIN TB_MATERIAL B ON (A.CODMAT = B.CODMAT) ");
                sql.AppendLine(" INNER JOIN TB_MATERIAL_TAREFA C ON (B.CODMAT = C.CODMAT) ");
                sql.AppendLine(" WHERE UPPER(C.CODCPL) = UPPER(@SERIAL) AND A.MAC_DE IS NOT NULL AND A.MAC_PARA IS NOT NULL ");

                param.Add("@SERIAL", serial);

                using (var reader = await CommandHelper.ExecuteReader(com, sql, param, sessionT5.Connection, null))
                {
                    
                    while (await reader.ReadAsync())
                    {
                        lista.Add(new ConversaoSerial()
                        {
                            codMat = reader["CODMAT"].ToString(),
                            mac_de = reader["MAC_DE"].ToString(),
                            mac_para = reader["MAC_PARA"].ToString()
                        });
                    }
                }                
            }

            if (lista.Count > 0)
            {
                foreach (var item in lista)
                {                   
                    var tam = item.mac_de.Length;
                    if (item.mac_de == serial.Substring(0, tam))
                    {
                        serialConvertido = serial.Replace(item.mac_de, item.mac_para);
                        break;
                    }                                                        
                }
            }

            if (String.IsNullOrEmpty(serialConvertido))
                serialConvertido = serial;

            return serialConvertido;
        }        
        #endregion
    
    /// <summary>
        /// Retorna o Script de OLT que seria utilizado no provisionamento ao consumir o endpoint AutorizarONU. Endpoint para fins de teste.        
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        [Route("api/AutorizarONUApiController/GerarScriptOLT")]
        public async Task<GetApiRetorno<string>> GerarScriptOLT(Aniel.Connect.Models.AutorizarONURequest request)
        {
            GetApiRetorno<string> retorno = new GetApiRetorno<string>();
            string scriptOLT = null;
 
            try 
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repoONU = new Conexao<ONU>(session))
                using (var repoPlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
                {
                    foreach (var view in request.ListONU)
                    {
                        var onu = await repoONU.selectFirst(p => p.Mac == view.Mac_Serial || p.Serial == view.Mac_Serial);
                        if (onu == null)
                        {
                            onu = new ONU
                            {
                                Mac = view.Mac_Serial,
                                Serial = view.Mac_Serial
                            };
                        }
 
                        onu = await PreencherCamposONU(onu, view);
                        var clientePlanoProduto = await repoPlanoProduto.selectFirst(p => p.Id == onu.Cliente_PlanoProduto.Id);
                        onu.Cliente_PlanoProduto = clientePlanoProduto;
 
                        string scriptOLTBase64 = null;
 
                        switch (onu.Cliente_PlanoProduto.Plano?.Tipo_Tecnologia)
                        {
                            case Model.enumerado.Tipo_Tecnologia.Internet:
                                if (onu.PerfilAtivacao.ScriptOLT != null)
                                    scriptOLTBase64 = Encoding.UTF8.GetString(onu.PerfilAtivacao.ScriptOLT);
                                else
                                    throw new Exception($"Não há Script de Internet para a ONU de serial {view.Mac_Serial}");
                                break;
 
                            case Model.enumerado.Tipo_Tecnologia.Telefonia_Fixo:
                                if (onu.PerfilAtivacao.ScriptOLTVoz != null)
                                    scriptOLTBase64 = Encoding.UTF8.GetString(onu.PerfilAtivacao.ScriptOLTVoz);
                                else
                                    throw new Exception($"Não há Script de Voz para a ONU de serial {view.Mac_Serial}");
                                break;
 
                            case null:
                                throw new Exception($"Tipo Tecnologia não cadastrado para a ONU {view.Mac_Serial}");
 
                            default:
                                throw new Exception($"Tipo Tecnologia não implementado para a ONU {view.Mac_Serial}");
 
                        }
 
                        scriptOLT = Encoding.UTF8.GetString(Convert.FromBase64String(scriptOLTBase64));
 
                        var listaValidacoesScriptOLT = ValidarVariaveisScript(onu, scriptOLT);
                        if (listaValidacoesScriptOLT.Count > 0)
                            throw new EAnielException(string.Join("\r \n", listaValidacoesScriptOLT));
 
                        scriptOLT = SubstituirVariaveis(onu, scriptOLT);
 
                        retorno.Objeto = scriptOLT;
                        retorno.Ok = true;
                    }
                }
            }
            catch(Exception ex)
            {
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex?.InnerException?.Message;
            }
            return retorno;
        }

        private void LogIntegracaoAtivacao(string msg)
        {
            LogManager.GetLogger("IntegracaoOLT").Info($"[Processo Ativação - {msg}");
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("api/AutorizarONUApiController/GetObrigaWifiByPerfilAtivacaoId")]
        public async Task<bool> GetObrigaWifi(Int64? idPerfilAtivacao)
        {
            bool retorno = false;

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var repoPerfilAtivacao = new Conexao<PerfilAtivacao>(session))
                {
                    var getPerfilAtivacao = await repoPerfilAtivacao.selectFirst(p => p.Id == idPerfilAtivacao);
                    if (getPerfilAtivacao == null)
                    {
                        CONSTANTES.log4Net.Error("Nenhum perfil de ativação encontrado com o ID informado.");
                        throw new Exception("Nenhum perfil de ativação encontrado com o ID informado.");
                    }

                    if (getPerfilAtivacao.ObrigaWifi == true)
                    {
                        retorno = true;
                    }
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex);
                throw new Exception("Algo de errado ocorreu durante a consulta", ex);
            }

            return retorno;
        }

        //Método que seleciona código do material na TB_CODIGO_COMPLEMENTAR
        //procurando pelo código complementar (serial/serial convertido).
        public async Task<(string codmat, string codcpl)> GetCodMatByCodComplementar(ISession sessionT5, string serial, string serialConvertido)
        {
            string codmat = "";
            string codcpl = "";

            using (var com = sessionT5.Connection.CreateCommand())
            {


                    StringBuilder sql = new StringBuilder();
                    Dictionary<string, string> param = new Dictionary<string, string>();

                    sql.AppendLine("SELECT FIRST 1 CODMAT, CODCPL ");
                    sql.AppendLine("FROM TB_CODIGO_COMPLEMENTAR ");
                    
                    if(String.IsNullOrEmpty(serialConvertido))
                    {
                       sql.AppendLine("WHERE UPPER(CODCPL) LIKE @MAC_SERIAL OR UPPER(LOCALIZACAO_REAL) LIKE @MAC_SERIAL");
                    }
                    else
                    {
                        sql.AppendLine("WHERE UPPER(CODCPL) LIKE @MAC_SERIAL OR UPPER(CODCPL) LIKE @MAC_SERIALCONVERTIDO ");
                        sql.AppendLine(" OR UPPER(LOCALIZACAO_REAL) LIKE @MAC_SERIAL OR UPPER (LOCALIZACAO_REAL) LIKE @MAC_SERIALCONVERTIDO");
                   
                        param.Add("@MAC_SERIALCONVERTIDO", $"%{serialConvertido.ToUpper()}%");

                    }


                    param.Add("@MAC_SERIAL", $"%{serial.ToUpper()}%");
                    

                    using (var reader = await CommandHelper.ExecuteReader(com, sql, param, sessionT5.Connection, null))
                    {
                        if (await reader.ReadAsync())
                        {
                            if (reader["CODMAT"] != DBNull.Value)
                            {
                                codmat = reader["CODMAT"].ToString();
                            }

                            if (reader["CODCPL"] != DBNull.Value)
                            {
                                codcpl = reader["CODCPL"].ToString();
                            }
                        }
                    }                                  
            }

            return (codmat, codcpl);
        }

      
        //Método responsável pela gravação de logs na TB_LOG_COMUNICACAO
        public Task<long> EnviarLog(string json, String usuario, String codOrdemServico, long? id = null)
        {
            Task<long> retorno = Task.FromResult(0L);

            try
            {
                using (ISession session = HibernateUtil.GetSessionLog())
                {
                    StringBuilder sql = new StringBuilder();
                    Dictionary<string, object> parametros = new Dictionary<string, object>();

                    using (var com = session.Connection.CreateCommand())
                    {
                        if (id == null || id == 0)
                        {
                            sql.AppendLine("INSERT INTO TB_LOG_COMUNICACAO(SERVICO, DETALHE, DATA_HORA, USUARIO, XML_REQUEST, TIPO) ");
                            sql.AppendLine("VALUES (@SERVICO, @DETALHE, CURRENT_TIMESTAMP, @USUARIO, @REQUEST, @TIPO)");
                            sql.AppendLine("RETURNING ID");

                            parametros.Add("@REQUEST", json);
                            if (codOrdemServico.Contains("OPERAÇÃO REALIZADA SEM O.S"))
                            {
                                parametros.Add("@SERVICO", "ATIVAÇÃO DE SINAL PELO CONNECT");
                            }
                            else
                            {
                                parametros.Add("@SERVICO", "ATIVAÇÃO DE SINAL COM TOUCH");
                            }
                            parametros.Add("@DETALHE", codOrdemServico);
                            parametros.Add("@USUARIO", usuario);
                            parametros.Add("@TIPO", 5);

                            retorno = CommandHelper.ExecuteNonQueryReturning(com, sql, parametros, session.Connection, null);
                        }
                        else
                        {
                            sql.AppendLine("UPDATE TB_LOG_COMUNICACAO ");
                            sql.AppendLine("SET XML_RESPONSE = @RESPONSE ");
                            sql.AppendLine("WHERE ID = @ID ");

                            parametros.Add("@RESPONSE", json);
                            parametros.Add("@ID", id);

                            var executaComando = CommandHelper.ExecuteNonQuery(com, sql, parametros, session.Connection, null);
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                LogManager.GetLogger(this.GetType().Assembly.GetName().Name).Error(String.Format("ERROR:{0}{1}STACKTRACE:{2}{3}OBJETO:{4}{5}{6}",
                   ex.Message, Environment.NewLine, ex.StackTrace, Environment.NewLine, "", Environment.NewLine, Environment.NewLine));
            }

            return retorno;
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("api/AutorizarONUApiController/AutorizarRadio")]
        public async Task<PostApiRetorno> AutorizarRadio(Aniel.Connect.Models.AutorizarRadioRequest request)
        {
            PostApiRetorno retorno = new PostApiRetorno();
            retorno.Mensagens = new List<PostApiMensagem>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                {
                    IIntegracaoExternaRepository repoIntegracaoExterna = new IntegracaoExternaRepository(session);
                    var integracaoExternaBusiness = new IntegracaoExternaBusiness(repoIntegracaoExterna);
                    IntegracoesExternas externas =
                    integracaoExternaBusiness.GetIntegracaoExterna(EnumTipoIntegracao.PROVISIONAMENTO);

                    if (externas != null)
                    {
                        switch (externas.IntegracaoExterna)
                        {
                            case IntegracaoExterna.HUBSOFT:

                                IntegracoesExternas dadosIntegracaoHubsoft = integracaoExternaBusiness.GetIntegracaoExterna(EnumTipoIntegracao.PROVISIONAMENTO, IntegracaoExterna.HUBSOFT);

                                if (String.IsNullOrEmpty(dadosIntegracaoHubsoft.UrlApi) ||
                                     String.IsNullOrEmpty(dadosIntegracaoHubsoft.Usuario) ||
                                     String.IsNullOrEmpty(dadosIntegracaoHubsoft.Senha) ||
                                     String.IsNullOrEmpty(dadosIntegracaoHubsoft.ApiSecret) ||
                                     String.IsNullOrEmpty(dadosIntegracaoHubsoft.TokenAcesso))
                                {
                                    string message = "Integração com a API do HubSoft não foi configurada corretamente, confira os acessos.";
                                    throw new EAnielException(message);
                                }

                                var loginHubsoft = new LoginHubsoftModel
                                {
                                    client_id = dadosIntegracaoHubsoft.TokenAcesso,
                                    client_secret = dadosIntegracaoHubsoft.ApiSecret,
                                    username = dadosIntegracaoHubsoft.Usuario,
                                    password = dadosIntegracaoHubsoft.Senha
                                };

                                var dadosBodyRequest = new ConfigurarAutenticacaoHubsoft
                                {
                                    id_cliente_servico = request.Id_Cliente_Servico,
                                    id_interface_conexao = request.Id_Interface_Conexao

                                };

                                var provisionamentoHubsoft = ProvisionamentoHubsoft.ConfigurarAutenticacao(dadosIntegracaoHubsoft.UrlApi, dadosBodyRequest,
                                               loginHubsoft, request.Usuario, request.Num_OS, this.EnviarLog);

                                if (provisionamentoHubsoft.status != "success")
                                {
                                    throw new EAnielException("Erro durante o processo de configuração dos parâmetros de autenticação do cliente.",
                                     new Exception ($"Retorno do Hubsoft: {Sinapse.Util.JsonService.SerializeObject(provisionamentoHubsoft)}"));
                                }
                                else
                                {
                                    retorno.Mensagens.Add(new PostApiMensagem
                                    {
                                        Tipo = PostApiMensagemType.Info,
                                        Mensagem = $"A interface rádio {request.Descricao} foi autorizada com sucesso!"
                                    });

                                    retorno.Ok = true;
                                }

                                    break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        throw new Exception("Nenhuma integração externa do tipo 'Provisionamento' foi encontrada.");
                    }
                }

            }
            catch(Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                retorno.Mensagens.Add(new PostApiMensagem
                {
                    Tipo = PostApiMensagemType.Erro,
                    Mensagem = ex.Message
                });
            }

            return retorno;
        }

        public async Task<string> ConverteONUVendorHex(ISession session, string serial)
        {

            if (string.IsNullOrEmpty(serial))
                return serial;

            string serialConvertido = serial;

            string serialVendor = serial.Substring(0, 4);
            string serialHex = serial.Substring(0, 8);

            try
            {   
                using (session = HibernateUtil.GetSession())
                {
                    StringBuilder sql = new StringBuilder();
                    Dictionary<String, object> param = new Dictionary<string, object>();

                    using (var com = session.Connection.CreateCommand())
                    {
                        sql.AppendLine(" SELECT FIRST 1 VENDOR, HEXADECIMAL FROM TB_ONU_VENDOR_HEX");
                        sql.AppendLine(" WHERE UPPER(VENDOR) = UPPER(@SERIAL_VENDOR) OR UPPER(HEXADECIMAL) = UPPER(@SERIAL_HEX) ");

                        param.Add("@SERIAL_VENDOR", serialVendor);
                        param.Add("@SERIAL_HEX", serialHex);

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, param, session.Connection, null))
                        {

                            if(await reader.ReadAsync())
                            {
                                string vendor = reader["VENDOR"].ToString();
                                string hexadecimal = reader["HEXADECIMAL"].ToString();

                                // Se o serial começa com o vendor -> Converte para Hexadecimal
                                if (serial.StartsWith(vendor, StringComparison.OrdinalIgnoreCase))
                                {
                                    serialConvertido = hexadecimal + serial.Substring(vendor.Length);

                                }
                                // Se o serial começa com hexadecimal -> converte para o vendor
                                else if (serial.StartsWith(hexadecimal, StringComparison.OrdinalIgnoreCase))
                                {
                                    serialConvertido = vendor + serial.Substring(hexadecimal.Length);
                                    
                                }

                                serialConvertido = serialConvertido.ToUpperInvariant();

                            }
                        }
                    }                
                }
               
            }
            catch(Exception ex)
            {
                CONSTANTES.log4Net.Error(ex);
            }

            return serialConvertido;
        }

        /// <summary>
        /// Retorna os materiais que se encontram no saldo do Cliente. 
        /// </summary>
        public async Task<IEnumerable<SaldoVincCliente>> getSaldoVincCliente(ISession sessionT5, long idCliente)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, object> parametros = new Dictionary<string, object>();
            List<SaldoVincCliente> lista = new List<SaldoVincCliente>();

            using (var com = sessionT5.Connection.CreateCommand())
            {
                sql.Append("SELECT C.CODMAT, C.DESCRICAO, C.SUBCOD_EXCLUSIVO, A.RSALDO AS SALDO , A.RCODCPL AS RCODCPL, C.ATIVACAO_SINAL, C.TIPO_EQUIPAMENTO, C.UNID ");
                sql.Append("FROM SP_CALCULA_SALDO_CLIENTE(@ID_CLIENTE, '-', '[ALL]') A ");
                sql.Append("INNER JOIN TB_MATERIAL C ON (A.RCODMAT = C.CODMAT) ");

                parametros.Add("@ID_CLIENTE", idCliente);

                using (var _reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                {
                    while (_reader.Read())
                    {
                        var item = new SaldoVincCliente
                        {
                            cpl = _reader["RCODCPL"].ToString(),
                            CODMAT = _reader["CODMAT"].ToString(),
                            CODMAT_RET = await getConvertCodmat(sessionT5, _reader["CODMAT"].ToString(), true),
                            CODMAT_REAP = await getConvertCodmat(sessionT5,_reader["CODMAT"].ToString(), false),
                            descricao = _reader["DESCRICAO"].ToString(),
                            saldo = _reader["SALDO"].ToString(),
                            unid = _reader["UNID"] != DBNull.Value ? _reader["UNID"].ToString() : null,
                            subCodExclusivo = _reader["SUBCOD_EXCLUSIVO"] != DBNull.Value ? _reader["SUBCOD_EXCLUSIVO"].ToString() == "S" : false,
                            ativacaoSinal = _reader["ATIVACAO_SINAL"] != DBNull.Value ? _reader["ATIVACAO_SINAL"].ToString() == "S" : false,
                            tipoEquipamento = _reader["TIPO_EQUIPAMENTO"] != DBNull.Value ? (Tipo_Equipamento?)Convert.ToInt32(_reader["TIPO_EQUIPAMENTO"]) : null
                        };
                        lista.Add(item);
                    }
                }
            }
           
            return lista;

        }

        public async Task<String> getConvertCodmat(ISession sessionT5, String codmat, Boolean isRetirada)
        {
            String novoCodmat = null;
            StringBuilder sql = new StringBuilder();
            Dictionary<string, object> parametros = new Dictionary<string, object>();

            using (var com = sessionT5.Connection.CreateCommand())
            {
                if (isRetirada)
                {
                    sql.Append("SELECT CODMAT_RET ");
                }
                else
                {
                    sql.Append("SELECT CODMAT_REAP ");
                }

                sql.Append("FROM TB_MATERIAL WHERE CODMAT = @pCODMAT ");
                parametros.Add("@pCODMAT", codmat);

                using (var _reader = await CommandHelper.ExecuteReader(com, sql, parametros, sessionT5.Connection, null))
                {
                    while (_reader.Read())
                    {
                        if (isRetirada)
                        {
                            novoCodmat = _reader["CODMAT_RET"] != DBNull.Value ? _reader["CODMAT_RET"].ToString() : null;
                        }
                        else
                        {
                            novoCodmat = _reader["CODMAT_REAP"] != DBNull.Value ? _reader["CODMAT_REAP"].ToString() : null;
                        }
                    }
                }
            }
           
            return novoCodmat;
        }


        /// <summary>
        /// Coleta os dados atualizados do login RADIUS no sistema integrador externo e os atualiza
        /// nas tabelas TB_LOGIN_RADIUS e TB_DOCUMENTO_PRODUCAO.
        /// </summary>
        public async Task<Response<LoginRadius>> GetPPPOEAtualizadoIntegracao(ISession session, ISession sessionT5, Conexao<LoginRadius> repoLogin, LoginRadius loginRadius, string numObraOriginal, string numObra, string usuario)
        {
            Response<LoginRadius> retorno = new Response<LoginRadius>();
            var idLog = await EnviarLog($"Integração HubSoft [GetPPPOEAtualizadoIntegracao] -> ID Login: {loginRadius.Id}, PPPOE anterior: {loginRadius.Login}, Senha anterior: {loginRadius.SenhaPPPOE}", usuario, numObraOriginal, null);
            try
            {
                IIntegracaoExternaRepository repoIntegracaoExterna = new IntegracaoExternaRepository(session);
                var integracaoExternaBusiness = new IntegracaoExternaBusiness(repoIntegracaoExterna);
                IntegracoesExternas externas =
                integracaoExternaBusiness.GetIntegracaoExterna(EnumTipoIntegracao.PROVISIONAMENTO);

                if (externas != null)
                {
                    switch (externas.IntegracaoExterna)
                    {
                        case IntegracaoExterna.HUBSOFT:

                            IntegracoesExternas dadosIntegracaoHubsoft = integracaoExternaBusiness.GetIntegracaoExterna(EnumTipoIntegracao.PROVISIONAMENTO, IntegracaoExterna.HUBSOFT);

                            if (String.IsNullOrEmpty(dadosIntegracaoHubsoft.UrlApi) ||
                                 String.IsNullOrEmpty(dadosIntegracaoHubsoft.Usuario) ||
                                 String.IsNullOrEmpty(dadosIntegracaoHubsoft.Senha) ||
                                 String.IsNullOrEmpty(dadosIntegracaoHubsoft.ApiSecret) ||
                                 String.IsNullOrEmpty(dadosIntegracaoHubsoft.TokenAcesso))
                            {
                                string message = "Integração com a API do HubSoft não foi configurada corretamente, confira os acessos.";
                                throw new EAnielException(message);
                            }

                            var loginHubsoft = new LoginHubsoftModel
                            {
                                client_id = dadosIntegracaoHubsoft.TokenAcesso,
                                client_secret = dadosIntegracaoHubsoft.ApiSecret,
                                username = dadosIntegracaoHubsoft.Usuario,
                                password = dadosIntegracaoHubsoft.Senha
                            };

                            var loginRadiusHubsoft = OrdemServicoHubSoftDao.GetLoginRadiusByOS(dadosIntegracaoHubsoft.UrlApi, numObraOriginal, loginHubsoft,
                                this.EnviarLog, numObraOriginal, usuario);

                            if(loginRadiusHubsoft != null)
                            {
                                if(loginRadiusHubsoft.autenticacao_servico != null && !String.IsNullOrEmpty(loginRadiusHubsoft?.autenticacao_servico?.login)
                                    && !String.IsNullOrEmpty(loginRadiusHubsoft?.autenticacao_servico?.password))
                                {
                                    if (!string.Equals(loginRadius.Login, loginRadiusHubsoft.autenticacao_servico.login)
                                    || !string.Equals(loginRadius.SenhaPPPOE, loginRadiusHubsoft.autenticacao_servico.password))
                                    {
                                        
                                        LoginRadius loginComparacao = await repoLogin.selectFirst(p => p.Login == loginRadiusHubsoft.autenticacao_servico.login);

                                        if(loginComparacao == null)
                                        {
                                            loginRadius.Login = loginRadiusHubsoft.autenticacao_servico.login;
                                            loginRadius.SenhaPPPOE = loginRadiusHubsoft.autenticacao_servico.password;

                                            await repoLogin.SaveOrUpdate(loginRadius);

                                            IDocumentoProducaoAnielRepository docProdRepo = new DocumentoProducaoAnielRepository(sessionT5, sessionT5);
                                            docProdRepo.AtualizaPPPOEDocumentoProducao(sessionT5, loginRadius, numObra, usuario, numObraOriginal, this.EnviarLog);
                                        }
                                        else
                                        {
                                            throw new Exception($"Já existe um cliente com este PPPOE: {loginRadius.Login}");
                                        }
                                        
                                    }
                                }
                            }

                            
                            retorno.result = true;
                            retorno.valor = loginRadius;


                            await EnviarLog($"PPPOE atualizado: {loginRadius.Login}, Senha PPPOE atualizada: {loginRadius.SenhaPPPOE}", usuario, numObraOriginal, idLog);


                            break;

                        default:
                            break;
                    }

                }
            }
            catch (Exception ex)
            {
                retorno.result = false;
                retorno.message = ex.Message;
                await EnviarLog($"{ex.Message}", usuario, numObra, idLog);

            }


            return retorno;
        }

    }

}
