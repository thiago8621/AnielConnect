using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Models;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.ModelLog.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Models.HubDesenvolvedor;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.DashBoardVendas;
using Aniel.Connect.Repository.EtapaVenda;
using Aniel.Connect.Repository.IntegracaoExterna;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.T5.Checklist;
using Aniel.T5.Model.nhibernate;
using Microsoft.AspNet.Identity;
using NHibernate;
using NHibernate.Criterion;
using OpenXmlPowerTools;
using Resources;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.ModelLog;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Image = System.Drawing.Image;
using StatusEtapaVenda = Sinapse.Model.enumerados.StatusEtapaVenda;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class VendaController : BaseController
    {
        public VendaController() : base("Cliente",
                                            CONSTANTES.URLPortalInterno,
                                            String.Empty,
                                            "api/VendaApi/FindById?id={0}",
                                            "api/VendaApi/Gravar",
                                            "api/VendaApi/Excluir",
                                            String.Empty)
        {

        }

        // GET: Cliente
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            ViewBag.Cancelamento = false;
            //Configuração Validação de telefone
            bool configuracaoBloqueioValidaTelefone = false;
            using (ISession session = HibernateUtil.GetSession())
            using (var repo = new Conexao<Configuracoes_Gerais>(session))
            {
                configuracaoBloqueioValidaTelefone = repo.selectFirst().Result.BloquearVendaSemContatoValidado;
            }
            ViewBag.BloquearVendaSemContatoValidado = configuracaoBloqueioValidaTelefone;
            if (Request.Params["CancelamentoVenda"] == "S")
            {
                ViewBag.Cancelamento = true;
            }
            else
            {
                ViewBag.Cancelamento = false;
            }

            bool origemDashBoard = !string.IsNullOrEmpty(Request.QueryString["dataFiltrada"]);
            if (origemDashBoard)
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (var repoFuncionario = new Conexao<Funcionario>(session))
                {
                    var repository = new DashBoardVendasRepository(session, sessionT5);
                    var business = new DashboardVendasBusiness(repository);
                    GetApiRetorno<PainelVendasFiltrosDashBoard> retorno = null;
                    
                    retorno = business.GetFiltrosPainelVendas(Request.QueryString["dataFiltrada"], 
                                                              Request.QueryString["filtroDashBoard"],
                                                              Request.QueryString["formato"], 
                                                              User.Identity.GetUserId());


                    ViewBag.DashBoard = retorno?.Objeto;
                }
            }
            return View();
        }


        // GET: Dashboard Vendas
        public ActionResult Dashboard_Vendas()
        {
            ViewBag.Areas = Areas;

            return View();
        }

        public ActionResult Dashboard_Vendas_Itens(Int64? IdFuncionario)
        {
            return View(new Dashboard_Vendas_ItemListModel
            {
                Id_Funcionario = IdFuncionario

            });
        }
        public ActionResult AdicionarLogin(Int64? Id_Venda)
        {
            return View(new VendaDadosGeraisViewModel
            {
                Id_Venda = (long)Id_Venda

            });
        }

        // 
        public ActionResult CadastroClientePasso1()
        {
            ViewBag.HabilitaCadastro = false;

            return View(new VendaPasso1ViewModel
            {
                TipoPessoa = Tipo_Pessoa.Fisica
            });
        }

        [HttpPost]
        public ActionResult CadastroClientePasso1(VendaPasso1ViewModel valor)
        {
            ViewBag.HabilitaCadastro = false;


            VendaPasso1ViewModel retorno = valor;

            if (Request.Form["validar"] != null)
            {
                try
                {
                    if (valor.TipoPessoa == Tipo_Pessoa.Fisica)
                    {
                        String _cpf = valor.CPF.Replace(".", String.Empty).Replace("-", String.Empty);

                        String _url = String.Format(CONSTANTES.URLHUBDESENVOLVEDOR_CPF, _cpf, valor.DataNascimento);
                        ResponseHub<ResponseCpf> resultadoHUB = GetByUrl<ResponseHub<ResponseCpf>>(_url);

                        retorno.retornoCPF = resultadoHUB;

                        if (retorno.retornoCPF.result == null)
                        {
                            String _mensage = retorno.retornoCPF.message;
                            retorno.retornoCPF = null;
                            ViewBag.HabilitaCadastro = true;
                            throw new Exception(_mensage);
                        }
                    }

                    if (valor.TipoPessoa == Tipo_Pessoa.Juridica)
                    {
                        String _cnpj = valor.CNPJ.Replace(".", String.Empty).Replace("-", String.Empty).Replace("/", String.Empty);

                        String _url = String.Format(CONSTANTES.URLHUBDESENVOLVEDOR_CNPJ, _cnpj);
                        ResponseHub<ResponseCnpj> resultadoHUB = GetByUrl<ResponseHub<ResponseCnpj>>(_url);

                        retorno.retornoCNPJ = resultadoHUB;

                        if (retorno.retornoCNPJ.result == null)
                        {
                            String _mensage = retorno.retornoCNPJ.message;
                            retorno.retornoCNPJ = null;
                            ViewBag.HabilitaCadastro = true;
                            throw new Exception(_mensage);
                        }
                    }
                }
                catch (Exception ex)
                {
                    AddModelError(ex.Message, String.Empty, String.Empty);
                }

                return View(retorno);
            }
            else if (Request.Form["cadastrar"] != null)
            {
                return RedirectToAction("ClienteDadosGerais", new
                {
                    tipo_pessoa = valor.TipoPessoa,
                    cpfcnpj = valor.TipoPessoa == Tipo_Pessoa.Fisica ? valor.CPF : valor.CNPJ,
                    datanascimento = valor.TipoPessoa == Tipo_Pessoa.Fisica ? valor.DataNascimento : null
                });
            }

            return RedirectToAction("Index", "Cliente");
        }

        [HttpGet()]
        public async Task<ActionResult> ClienteDadosGerais(Int64? Id_Venda, Int64? Id_Cliente, Tipo_Pessoa? tipo_pessoa, String cpfcnpj, String datanascimento, Int64? IdAtendimento)
        {
            //SetTokenByUser();
            ViewBag.Venda = Id_Venda;
            bool inconsistencia = Request.Params["InconsistenciaStatus"] == "S";

            if (inconsistencia)
                AddModelError("Página precisou ser recarregada pelo fato do status ser alterado por outro usuario.");

            using (ISession session = HibernateUtil.GetSession())
            using (ISession sessionLog = HibernateUtil.GetSessionLog())
            using (var repoLog = new Conexao<LogVendas>(sessionLog))
            using (var repoIntegracao = new Conexao<IntegracoesExternas>(session))
            {
                var usuario = new Conexao<IdentityUser>(session).selectFirst(p => p.Id == User.Identity.GetUserId()).Result;
                var _retornoModelos_Contrato = new Modelo_ContratoApiController().GetList().Result;
                var integracaoConfigurada = new IntegracaoExternaBusiness(new IntegracaoExternaRepository(session)).GetIntegracoesExternas(EnumTipoIntegracao.COMERCIAL);
                ViewBag.Modelos_Contrato = _retornoModelos_Contrato.Objeto;
                ViewBag.IntegracaoConfigurada = integracaoConfigurada;
                ViewBag.Departamento = usuario.Departamento?.Id;

                if (Id_Venda.HasValue || Id_Cliente.HasValue)
                {
                    var retorno = await new VendaApiController().FindById(Id_Venda, Id_Cliente);

                    
                    if (!retorno.Ok)
                    {
                        AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, retorno.Mensagem, retorno.InnerException);
                    }

                    if (!Id_Venda.HasValue)
                    {
                        retorno.Objeto.Id_Vendedor = usuario?.Funcionario != null ? (usuario.Funcionario.Vendedor ? usuario.Funcionario.Id : (Int64?)null) : (Int64?)null;
                        retorno.Objeto.Nome_Vendedor = usuario?.Funcionario != null ? (usuario.Funcionario.Vendedor ? usuario.Funcionario.Nome : String.Empty) : String.Empty;
                        retorno.Objeto.Id_Supervisor = usuario?.Funcionario?.Supervisor?.Id;
                        retorno.Objeto.Nome_Supervisor = usuario?.Funcionario?.Supervisor?.Nome;
                    }

                    using (var configApi = new Configuracoes_GeraisApiController())
                    {
                        var config = (await configApi.FindById(1)).Objeto;

                        ViewBag.HabilitarConsultaCredito = config.ConsultaCreditoExibeVenda;
                        ViewBag.HabilitarConsultaPendenciaEndereco = config.ConsultaPendenciaEnderecoExibeVenda;
                        ViewBag.HabilitarConsultaPendenciaFinanceira = config.ConsultaPendenciaFinanceiraExibeVenda;
                        ViewBag.HabilitarConsultaPendenciaViabilidade = config.ConsultaViabilidadeExibeVenda;
                        ViewBag.CriarAtendimento = config.Gerar_Atendimento_Apos_Venda;
                        ViewBag.End_Insta_Mesmo_Princ = config.EnderecoInstalacaoMesmoPrincipal;
                    }

                    using (var param = new Conexao<ParametrosConsultaCredito>(session))
                    {
                        ViewBag.PontosTotaisRestricao = param.select(p => p.PontosPrimeiraFaixaCriterio + p.PontosSegundaFaixaCriterio + p.PontosTerceiraFaixaCriterio >= 0);
                    }

                    FillListasView(retorno.Objeto);

                    if (IdAtendimento.HasValue)
                    {
                        retorno.Objeto.IdAtendimento = IdAtendimento;
                    }

                    TempData["Usuario_Vendedor"] = false;

                    if (usuario?.Funcionario != null && usuario.Funcionario.Vendedor)
                    {
                        TempData["Usuario_Vendedor"] = retorno?.Objeto?.Id_Vendedor == usuario.Funcionario.Id;
                    }

                    return View(retorno.Objeto);
                }
                else
                {
                    var obj = new VendaDadosGeraisViewModel();

                    FillListasView(obj);

                    return View(obj);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ClienteDadosGerais(VendaDadosGeraisViewModel valor)
        {
            var dadosConfigGerais = new Configuracoes_GeraisApiController().FindById(1);
            if (dadosConfigGerais.Result.Objeto.Identificacao_Cliente.Value == Identificacao_Cliente.Numero_Contrato)
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    List<ICriterion> listaCriteria = new List<ICriterion>();
                    var temp_identificacao_cliente = valor.Identificacao_Cliente ?? String.Empty;

                    listaCriteria.Add(Expression.Eq(nameof(Cliente.Numero_Contrato), temp_identificacao_cliente).IgnoreCase());
                    if (valor.Id != 0)
                        listaCriteria.Add(Expression.Not(Restrictions.Eq(nameof(Cliente.Id), valor.Id)));

                    var cliente = new Conexao<Cliente>(session).selectCriteria(listaCriteria.ToArray()).Result.FirstOrDefault();

                    if (cliente != null)
                    {
                        AddModelError(RsCadastros.IDENTIFICACAO_CLIENTE_JA_EXISTE);
                    }

                }
            }

            var temp_id = valor.Id;

            valor.End_Insta_Mesmo_Princ = dadosConfigGerais.Result.Objeto.EnderecoInstalacaoMesmoPrincipal;
            ViewBag.End_Insta_Mesmo_Princ = valor.End_Insta_Mesmo_Princ;

            //Caso a configuração Bloquear alteração do endereço de instalação nas vendas esteja ativa
            //as validações ocorrerão somente no endereço principal, pois o endereço de instalação
            //será uma réplica do endereço principal.
            if (valor.End_Insta_Mesmo_Princ)
            {
                var keysToRemove = ModelState.Keys
                    .Where(k => k.StartsWith("EnderecoInstalacao."))
                    .ToList();

                foreach (var key in keysToRemove)
                {
                    ModelState.Remove(key);
                }
            }

            try
            {
                if (ModelState.IsValid && Validacoes(valor))
                {
                    //PostApiRetorno<Int64> retorno = PostByUrl<VendaDadosGeraisViewModel, Int64>(valor, UrlPortal, UrlPost);
                    ParamWebApi<VendaDadosGeraisViewModel> modelWebPai = new ParamWebApi<VendaDadosGeraisViewModel>
                    {
                        Usuario = User.Identity.GetUserId(),
                        Objeto = valor
                    };

                    PostApiRetorno<Int64> retorno = await new VendaApiController().Gravar(modelWebPai);

                    if (retorno.Ok) //Edição
                    {
                        using (ISession session = HibernateUtil.GetSession())
                        using (var repoVenda = new Conexao<Venda>(session))
                        {
                            Venda venda = repoVenda.selectFirst(p => p.Id == retorno.IdObjeto).Result;
                            var Reserva_automatica = new Conexao<Configuracoes_Gerais>(session).selectFirst().Result.Reserva_Porta_Automatica;
                            var planosProdutos = new Conexao<Cliente_PlanoProduto>(session).select(p => p.Id_Venda == retorno.IdObjeto).Result;

                            IEtapaVendaRepository repoEtapaVenda = new EtapaVendaRepository(session);
                            IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(session);

                            var etapaVendaBusiness = new EtapaVendaBusiness(repoEtapaVenda, repoConfig);

                            bool vendaConcluida = !etapaVendaBusiness.GetDadosEtapasVenda(venda.Id)
                                                                     .Any(e => e.Status != (int)StatusEtapaVenda.Aprovado && e.Status != (int)StatusEtapaVenda.AprovadoManualmente);

                            if (Reserva_automatica == true)
                            {

                                foreach (var item in planosProdutos)
                                {
                                    var ReservadeCaixa = new Mapa_Analise_ComercialApiController().ReservarCaixa(valor.Id_Caixa, valor.Id_Porta, item.Id);
                                }

                                return RedirectToAction("ClienteDadosGerais", new { Id_Venda = retorno.IdObjeto, Sucesso = "S", VendaConcluida = valor.VendaConcluida, Id_Cliente = venda?.Id_Cliente });
                            }
                            else if (vendaConcluida)
                            {
                                return RedirectToAction("ClienteDadosGerais", new { Id_Venda = retorno.IdObjeto, Sucesso = "S", VendaConcluida = "S", Id_Cliente = planosProdutos.FirstOrDefault()?.Cliente?.Id });
                            }
                            else
                            {
                                return RedirectToAction("ClienteDadosGerais", new { Id_Venda = retorno.IdObjeto, Sucesso = "S", VendaConcluida = valor.VendaConcluida, Id_Cliente = planosProdutos.FirstOrDefault()?.Cliente?.Id });
                            }
                        }
                    }
                    else if (retorno.Mensagem == "Inconsistencia de Status")
                    {
                        return RedirectToAction("ClienteDadosGerais", new { Id_Venda = modelWebPai.Objeto.Id, InconsistenciaStatus = "S" });
                    }
                    else if (retorno.Mensagem.Contains("Endereço/Logradouro ou Condomínio/Prédio deve ser preenchido"))
                    {
                        AddModelError(retorno.Mensagem, retorno.InnerException);

                        var etapaAtual = valor.EtapaAtualVenda;

                        foreach (var etapaAlterarStatus in valor.EtapasVenda)
                        {
                            if (etapaAlterarStatus.Etapa == etapaAtual)
                            {
                                etapaAlterarStatus.Status = 0;
                            }
                        }                    
                    }
                    else
                    {
                        AddModelError(retorno.Mensagem, retorno.InnerException);
                    }
                }


                FillListasView(valor);
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                FillListasView(valor);
                AddModelError(Resources.RsBase.ERRO_GRAVARDADOSNOSERVIDOR, ex.Message, ex.InnerException?.Message);
            }

            return View(valor);
        }

        public override bool Validacoes<T>(T pModel)
        {
            List<CamposObrigatoriosVendaModels> campos = new CamposObrigatoriosVendaApiController().GetCampos().Result;

            Dictionary<String, String> erros = new Dictionary<String, String>();

            VendaDadosGeraisViewModel _model = pModel as VendaDadosGeraisViewModel;

            if (_model.TipoPessoa == Tipo_Pessoa.Juridica)
            {
                foreach (var campo in campos)
                {
                    if (campo.Obrigatorio)
                        switch (campo.Campo)
                        {
                            case Model.enumerado.CamposObrigatoriosVendaEnum.CPF_RESPONSAVEL_EMPRESA:
                                if (String.IsNullOrEmpty(_model.CPFResponsavel))
                                    erros.Add("CPFResponsavel", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.CPF_RESPONSAVEL_EMPRESA));
                                break;
                            case Model.enumerado.CamposObrigatoriosVendaEnum.CARGO_RESPONSAVEL_EMPRESA:
                                if (String.IsNullOrEmpty(_model.Cargo_Responsavel))
                                    erros.Add("Cargo_Responsavel", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.CARGO_RESPONSAVEL_EMPRESA));
                                break;
                            case Model.enumerado.CamposObrigatoriosVendaEnum.CATEGORIA_CLIENTE:
                                if (!_model.Id_Categoria_Cliente.HasValue)
                                    erros.Add("Id_Categoria_Cliente", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.CATEGORIA_CLIENTE));
                                break;
                            case Model.enumerado.CamposObrigatoriosVendaEnum.TELEFONE_CELULAR:
                                if (String.IsNullOrEmpty(_model.TelefoneCelular))
                                    erros.Add("TelefoneCelular", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.CELULAR));
                                break;
                            case Model.enumerado.CamposObrigatoriosVendaEnum.INSCRICAO_ESTADUAL:
                                if (String.IsNullOrEmpty(_model.Inscricao_Estadual))
                                    erros.Add("Inscricao_Estadual", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.INSCRICAO_ESTADUAL));
                                break;
                            case Model.enumerado.CamposObrigatoriosVendaEnum.INSCRICAO_MUNICIPAL:
                                if (String.IsNullOrEmpty(_model.Inscricao_Municipal))
                                    erros.Add("Inscricao_Municipal", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.INSCRICAO_MUNICIPAL));
                                break;
                            case Model.enumerado.CamposObrigatoriosVendaEnum.PORTE:
                                if (!_model.Porte.HasValue)
                                    erros.Add("Porte", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.PORTE));
                                break;
                            case Model.enumerado.CamposObrigatoriosVendaEnum.REPRESENTANTE:
                                if (String.IsNullOrEmpty(_model.Representante))
                                    erros.Add("Representante", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.REPRESENTANTE));
                                break;
                            case Model.enumerado.CamposObrigatoriosVendaEnum.TELEFONE1:
                                if (String.IsNullOrEmpty(_model.TelefoneFixo))
                                    erros.Add("TelefoneFixo", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.TELEFONEFIXO));
                                break;
                            case Model.enumerado.CamposObrigatoriosVendaEnum.TELEFONERECADO:
                                if (String.IsNullOrEmpty(_model.TelefoneRecado))
                                    erros.Add("TelefoneRecado", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.TELEFONERECADO));
                                break;
                            case Model.enumerado.CamposObrigatoriosVendaEnum.NOMERECADO:
                                if (String.IsNullOrEmpty(_model.NomeRecado))
                                    erros.Add("NomeRecado", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.NOMERECADO));
                                break;
                            case Model.enumerado.CamposObrigatoriosVendaEnum.DATA_ABERTURA:
                                if (!_model.Data_Abertura.HasValue)
                                    erros.Add("Data_Abertura", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.DATA_ABERTURA));
                                break;

                            case Model.enumerado.CamposObrigatoriosVendaEnum.PAIS:
                                if (_model.EnderecoPrincipal != null)
                                {
                                    if (!_model.EnderecoPrincipal.Id_Pais.HasValue)
                                        erros.Add("Pais", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.PAIS + " - " + Resources.RsCadastros.DADOSGERAIS));
                                }
                                break;
                            case Model.enumerado.CamposObrigatoriosVendaEnum.CEP:
                                if (_model.EnderecoPrincipal != null)
                                {
                                    if (String.IsNullOrEmpty(_model.EnderecoPrincipal.CEP))
                                        erros.Add("EnderecoPrincipal.CEP", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.CEP + " - " + Resources.RsCadastros.DADOSGERAIS));
                                }
                                /*if (_model.EnderecoCobranca != null)
                                {
                                    if (String.IsNullOrEmpty(_model.EnderecoCobranca.CEP) && !_model.End_Cobranca_Igual_Principal)
                                        erros.Add("", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.CEP + " - " + Resources.RsCadastros.INSTALACAO));
                                }*/
                                break;
                            case Model.enumerado.CamposObrigatoriosVendaEnum.COMPLEMENTO:
                                if (_model.EnderecoPrincipal != null)
                                {
                                    if (String.IsNullOrEmpty(_model.EnderecoPrincipal.Complemento))
                                        erros.Add("Complemento", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.COMPLEMENTO + " - " + Resources.RsCadastros.DADOSGERAIS));
                                }
                                /*if (_model.EnderecoCobranca != null)
                                {
                                    if (String.IsNullOrEmpty(_model.EnderecoCobranca.Complemento) && !_model.End_Cobranca_Igual_Principal)
                                        erros.Add("Complemento", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.COMPLEMENTO + " - " + Resources.RsCadastros.INSTALACAO));
                                }*/
                                break;
                            case Model.enumerado.CamposObrigatoriosVendaEnum.PONTO_REFERENCIA:
                                if (_model.EnderecoPrincipal != null)
                                {
                                    if (String.IsNullOrEmpty(_model.EnderecoPrincipal.Ponto_Referencia))
                                        erros.Add("Ponto_Referencia", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.PONTO_REFERENCIA + " - " + Resources.RsCadastros.DADOSGERAIS));
                                }
                                /*if (_model.EnderecoCobranca != null)
                                {
                                    if (String.IsNullOrEmpty(_model.EnderecoCobranca.Ponto_Referencia) && !_model.End_Cobranca_Igual_Principal)
                                        erros.Add("Ponto_Referencia", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.PONTO_REFERENCIA + " - " + Resources.RsCadastros.INSTALACAO));
                                }*/
                                break;
                            case Model.enumerado.CamposObrigatoriosVendaEnum.TIPO_IMOVEL:
                                if (_model.EnderecoPrincipal != null)
                                {
                                    if (!_model.EnderecoPrincipal.Id_Tipo_Imovel.HasValue)
                                        erros.Add("EnderecoPrincipal.Id_Tipo_Imovel", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.TIPO_IMOVEL + " - " + Resources.RsCadastros.DADOSGERAIS));
                                }
                                /*if (_model.EnderecoCobranca != null)
                                {
                                    if (!_model.EnderecoPrincipal.Id_Tipo_Imovel.HasValue && !_model.End_Cobranca_Igual_Principal)
                                        erros.Add("", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.TIPO_IMOVEL + " - " + Resources.RsCadastros.INSTALACAO));
                                }*/
                                break;


                        }
                }
            }
            else
            {
                foreach (var campo in campos)
                {
                    if (campo.Obrigatorio)
                        switch (campo.Campo)
                        {
                            case Model.enumerado.CamposObrigatoriosVendaEnum.CATEGORIA_CLIENTE:
                                if (!_model.Id_Categoria_Cliente.HasValue)
                                    erros.Add("Id_Categoria_Cliente", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.CATEGORIA_CLIENTE));
                                break;
                            case Model.enumerado.CamposObrigatoriosVendaEnum.TELEFONE_CELULAR:
                                if (String.IsNullOrEmpty(_model.TelefoneCelular))
                                    erros.Add("TelefoneCelular", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.CELULAR));
                                break;
                            case Model.enumerado.CamposObrigatoriosVendaEnum.MEIO_CAPTACAO:
                                if (!_model.Id_Meio_Captacao.HasValue)
                                    erros.Add("Id_Meio_Captacao", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.COMO_FICOU_SABENDO));
                                break;
                            case Model.enumerado.CamposObrigatoriosVendaEnum.EMAIL_PRINCIPAL:
                                if (String.IsNullOrEmpty(_model.Email))
                                    erros.Add("Email", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.EMAIL));
                                break;
                            case Model.enumerado.CamposObrigatoriosVendaEnum.ESTADO_CIVIL:
                                if (!_model.EstadoCivil.HasValue)
                                    erros.Add("EstadoCivil", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.ESTADO_CIVIL));
                                break;
                            case Model.enumerado.CamposObrigatoriosVendaEnum.NACIONALIDADE:
                                if (String.IsNullOrEmpty(_model.Nacionalidade))
                                    erros.Add("Nacionalidade", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.NACIONALIDADE));
                                break;
                            case Model.enumerado.CamposObrigatoriosVendaEnum.NOME_MAE:
                                if (String.IsNullOrEmpty(_model.Mae))
                                    erros.Add("Mae", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.MAE));
                                break;
                            case Model.enumerado.CamposObrigatoriosVendaEnum.NOME_PAI:
                                if (String.IsNullOrEmpty(_model.Pai))
                                    erros.Add("Pai", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.PAI));
                                break;
                            case Model.enumerado.CamposObrigatoriosVendaEnum.RG:
                                if (String.IsNullOrEmpty(_model.RG))
                                    erros.Add("RG", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.RG));
                                break;
                            case Model.enumerado.CamposObrigatoriosVendaEnum.SEXO:
                                if (!_model.Sexo.HasValue)
                                    erros.Add("Sexo", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.SEXO));
                                break;
                            case Model.enumerado.CamposObrigatoriosVendaEnum.TELEFONE1:
                                if (String.IsNullOrEmpty(_model.TelefoneFixo))
                                    erros.Add("TelefoneFixo", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.TELEFONEFIXO));
                                break;
                            case Model.enumerado.CamposObrigatoriosVendaEnum.TELEFONERECADO:
                                if (String.IsNullOrEmpty(_model.TelefoneRecado))
                                    erros.Add("TelefoneRecado", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.TELEFONERECADO));
                                break;
                            case Model.enumerado.CamposObrigatoriosVendaEnum.NOMERECADO:
                                if (String.IsNullOrEmpty(_model.NomeRecado))
                                    erros.Add("NomeRecado", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.NOMERECADO));
                                break;
                            case Model.enumerado.CamposObrigatoriosVendaEnum.DATA_NASCIMENTO:
                                if (String.IsNullOrEmpty(_model.DataNascimento))
                                    erros.Add("DataNascimento", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.DATANASCIMENTO));
                                break;
                            case Model.enumerado.CamposObrigatoriosVendaEnum.DATA_EMISSAO:
                                if (String.IsNullOrEmpty(_model.DataEmissao))
                                    erros.Add("DataEmissao", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.DATAEMISSAO));
                                break;
                            case Model.enumerado.CamposObrigatoriosVendaEnum.ORGAO:
                                if (String.IsNullOrEmpty(_model.Orgao))
                                    erros.Add("Orgao", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.ORGAO));
                                break;
                            case Model.enumerado.CamposObrigatoriosVendaEnum.CEP:
                                if (_model.EnderecoPrincipal != null)
                                {
                                    if (String.IsNullOrEmpty(_model.EnderecoPrincipal.CEP))
                                        erros.Add("EnderecoPrincipal.CEP", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.CEP + " - " + Resources.RsCadastros.DADOSGERAIS));
                                }
                                /*if (_model.EnderecoCobranca != null)
                                {
                                    if (String.IsNullOrEmpty(_model.EnderecoCobranca.CEP) && !_model.End_Cobranca_Igual_Principal)
                                        erros.Add("", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.CEP + " - " + Resources.RsCadastros.INSTALACAO));
                                }*/
                                break;
                            case Model.enumerado.CamposObrigatoriosVendaEnum.PAIS:
                                if (_model.EnderecoPrincipal != null)
                                {
                                    if (!_model.EnderecoPrincipal.Id_Pais.HasValue)
                                        erros.Add("EnderecoPrincipal.Id_Pais", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.PAIS + " - " + Resources.RsCadastros.DADOSGERAIS));
                                }
                                if (_model.EnderecoInstalacao != null)
                                {
                                    if (!_model.EnderecoInstalacao.Id_Pais.HasValue)
                                        erros.Add("EnderecoInstalacao.Id_Pais", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.PAIS + " - " + Resources.RsCadastros.INSTALACAO));
                                }
                                break;
                            case Model.enumerado.CamposObrigatoriosVendaEnum.COMPLEMENTO:
                                if (_model.EnderecoPrincipal != null)
                                {
                                    if (String.IsNullOrEmpty(_model.EnderecoPrincipal.Complemento))
                                        erros.Add("Complemento", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.COMPLEMENTO + " - " + Resources.RsCadastros.DADOSGERAIS));
                                }
                                /*if (_model.EnderecoCobranca != null)
                                {
                                    if (String.IsNullOrEmpty(_model.EnderecoCobranca.Complemento) && !_model.End_Cobranca_Igual_Principal)
                                        erros.Add("Complemento", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.COMPLEMENTO + " " + Resources.RsCadastros.INSTALACAO));
                                }*/
                                break;
                            case Model.enumerado.CamposObrigatoriosVendaEnum.PONTO_REFERENCIA:
                                if (_model.EnderecoPrincipal != null)
                                {
                                    if (String.IsNullOrEmpty(_model.EnderecoPrincipal.Ponto_Referencia))
                                        erros.Add("Ponto_Referencia", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.PONTO_REFERENCIA + " - " + Resources.RsCadastros.DADOSGERAIS));
                                }
                                /*if (_model.EnderecoCobranca != null && !_model.End_Cobranca_Igual_Principal)
                                {
                                    if (String.IsNullOrEmpty(_model.EnderecoCobranca.Ponto_Referencia))
                                        erros.Add("Ponto_Referencia", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.PONTO_REFERENCIA + " - " + Resources.RsCadastros.INSTALACAO));
                                }*/
                                break;
                            case Model.enumerado.CamposObrigatoriosVendaEnum.TIPO_IMOVEL:
                                if (_model.EnderecoPrincipal != null)
                                {
                                    if (!_model.EnderecoPrincipal.Id_Tipo_Imovel.HasValue)
                                        erros.Add("EnderecoPrincipal.Id_Tipo_Imovel", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.TIPO_IMOVEL + " - " + Resources.RsCadastros.DADOSGERAIS));
                                }
                                /*if (_model.EnderecoCobranca != null)
                                {
                                    if (!_model.EnderecoPrincipal.Id_Tipo_Imovel.HasValue && !_model.End_Cobranca_Igual_Principal)
                                        erros.Add("", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.TIPO_IMOVEL + " - " + Resources.RsCadastros.INSTALACAO));
                                }*/
                                break;

                        }
                }
            }

            if (!String.IsNullOrEmpty(_model.DataNascimento))
            {

                if (DateTimeUtil.GetIdade(Convert.ToDateTime(_model.DataNascimento)) < 18)
                {
                    erros.Add("", String.Format(Resources.RsBase.REQUIRED, RsCadastros.NAO_PERMITIDO_18));
                }
            }

            if (erros.Count > 0)
            {
                AddModelError(erros);
            }

            return erros.Count == 0;
        }

        protected override void FillListasView<T>(T pModel)
        {

            var confGerais = new Configuracoes_GeraisApiController().FindById(1).Result.Objeto;

            var campos = new CamposObrigatoriosVendaApiController().GetCampos().Result.ToList<ICamposObrigatoriosModel>();
            ViewBag.CamposObrigatorios = CamposObrigatoriosController.dictCamposObrigatorios(campos);
            ViewBag.CamposBloqueados = CamposObrigatoriosController.dictCamposBloqueados(campos);

            List<CamposObrigatoriosClienteModels> camposObrigatoriosCliente = new CamposObrigatoriosClienteApiController().GetCampos().Result;
            ViewBag.CamposObrigatoriosCliente = camposObrigatoriosCliente;
            ViewBag.Gerar_Atendimento_Venda = confGerais.Gerar_Atendimento_Apos_Venda;

            ViewBag.Areas = Areas;
            ViewBag.Identificacao_Cliente = Convert.ToInt32(confGerais.Identificacao_Cliente);
            ViewBag.Utiliza_Cadastro_Endereco = Convert.ToBoolean(confGerais.Utiliza_Cadastro_Endereco);

            ViewBag.ConsultaViabilidade = confGerais.ConsultaViabilidadeExibeVenda;
            ViewBag.ConsultaCredito = confGerais.ConsultaCreditoExibeVenda;
            ViewBag.ConsultaPendenciaEndereco = confGerais.ConsultaPendenciaEnderecoExibeVenda;
            ViewBag.ConsultaPendenciaFinanceira = confGerais.ConsultaPendenciaFinanceiraExibeVenda;
            // ViewBag.ResultConsulta = new ParametrosConsultaCreditoApiController().GetListBlocosConsultaSerasa().Result.Objeto;


            ViewBag.Campanhas = new List<CampanhaListaModel>();
            ViewBag.Descontos = new List<DescontoListaModel>();
            ViewBag.Desconto_Meio_Captacao = new List<Desconto_Meio_Captacao>();
            ViewBag.ComboTmp = new List<ComboObj>();
            ViewBag.Meios = new List<Meio_CaptacaoCadastroModel>();
            ViewBag.Categorias = new List<Categoria_Cliente>();
            ViewBag.FormasDePagamento = new List<Forma_PagamentoCadastroModel>();
            ViewBag.Banco = new List<BancoCadastroModel>();

            ViewBag.GruposArea = new Grupo_AreaAnielApiController().GetList().Result.Objeto;
            ViewBag.Tipos_Imovel = new Tipo_ImovelAnielApiController().GetList().Result.Objeto;



            ViewBag.Localidades = new List<String>();

            ViewBag.Paises = new PaisApiController().GetList().Result.Objeto;


            var _retornoDescontos = new DescontoApiController().GetList().Result;
            var _retornoMeios = new Meio_CaptacaoApiController().GetList().Result;
            var _retornoDescontoMeioCaptacao = new Desconto_Meio_CaptacaoApiController().GetList().Result;



            if (!_retornoMeios.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoMeios.Mensagem, _retornoMeios.InnerException);




            if (!_retornoDescontoMeioCaptacao.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoDescontoMeioCaptacao.Mensagem, _retornoDescontoMeioCaptacao.InnerException);



            ViewBag.Descontos = _retornoDescontos.Objeto;

            ViewBag.Meios = _retornoMeios.Objeto;

            ViewBag.Desconto_Meio_Captacao = _retornoDescontoMeioCaptacao.Objeto;
            ViewBag.Num_Contrato_Auto = new Configuracoes_GeraisApiController().FindById(1).Result.Objeto.Num_Contrato_Gerado_Auto;


            using (var session = HibernateUtil.GetSession())
            using (var repo = new Conexao<Configuracoes_Gerais>(session))
            {
                //Dados das Configurações Gerais sobre os descontos
                ViewBag.Descontos_Tipo_Diferente = repo.selectFirst().Result.Acumular_Descontos_Diferentes;
                ViewBag.Acumula_Descontos_Mesmo_Tipo = repo.selectFirst().Result.Acumular_Descontos_Mesmo_Tipo;
                ViewBag.Ofertar_Plano_Produto_Cobertura = new Conexao<Configuracoes_Gerais>(session).selectFirst().Result.Ofertar_Plano_Produto_Cobertura;
                var integracaoConfigurada = new IntegracaoExternaBusiness(new IntegracaoExternaRepository(session)).GetIntegracoesExternas(EnumTipoIntegracao.COMERCIAL);
                ViewBag.IntegracaoConfigurada = integracaoConfigurada;
            }
        }

        private void GetListaAnexarArquivo(Int64 Id_Cliente)
        {
            GetApiRetorno<List<ClienteAnexoModelView>> retornoLista = base.GetAll<ClienteAnexoModelView>($"api/Cliente/GetAnexos?id_cliente={Id_Cliente}");

            if (!retornoLista.Ok)
            {
                AddModelError(retornoLista.Mensagem);
                ViewBag.Lista = new List<ClienteAnexoModelView>();
            }
            else
            {
                ViewBag.Lista = retornoLista.Objeto;
            }

        }

        public async Task<ActionResult> ListaPlanos(Int64? Id_Venda, Int64? Id_Cliente)
        {

            GetApiRetorno<List<Planos_HabilitadosModel>> retorno = new GetApiRetorno<List<Planos_HabilitadosModel>>();
            GetApiRetorno<List<Planos_HabilitadosModel>> retorno_cliente = new GetApiRetorno<List<Planos_HabilitadosModel>>();

            using (var apiPlanoProduto = new PlanoProdutoApiController())
            using (var apiCliente = new ClienteApiController())
            {
                if (Id_Venda.HasValue)
                {
                    retorno_cliente = await apiPlanoProduto.GetListValorVenda(Id_Venda.Value);

                    retorno.Objeto = retorno_cliente.Objeto;

                }
                else if (Id_Cliente.HasValue)
                {
                    retorno = await apiCliente.GetListPlanosProdutos(Id_Cliente.Value);
                }
            }

            return View(retorno.Objeto);
        }

        public async Task<ActionResult> ListaComboPlano(Int64? Id_Venda, Int64? Id_Cliente)
        {
            GetApiRetorno<List<Planos_HabilitadosModel>> retorno = new GetApiRetorno<List<Planos_HabilitadosModel>>();
            GetApiRetorno<List<Planos_HabilitadosModel>> retorno_cliente = new GetApiRetorno<List<Planos_HabilitadosModel>>();

            using (var apiPlanoProduto = new PlanoProdutoApiController())
            using (var apiCliente = new ClienteApiController())
            {
                if (Id_Venda.HasValue)
                {
               
                    retorno_cliente = await apiPlanoProduto.GetListComboPlano(Id_Venda.Value);

                    retorno.Objeto = retorno_cliente.Objeto;
                }
                else if (Id_Cliente.HasValue)
                {
                    retorno = await apiCliente.GetListPlanosProdutos(Id_Cliente.Value);
                }
            }

            // Reaproveita a mesma View
            return View("ListaPlanos", retorno.Objeto);
        }

        public async Task<ActionResult> UltimosContatos(Int64 Id_Venda)
        {
            var retorno = await new VendaApiController().GetHistoricoContato(Id_Venda);

            if (!retorno.Ok)
            {
                AddModelError(retorno.Mensagem, retorno.InnerException);
            }

            return View(retorno.Objeto);
        }

        public async Task<ActionResult> Historico_UltimosContatos_AcaoStatus(Int64 Id_Venda)
        {
            var retorno = await new VendaApiController().Historico_UltimosContatos_AcaoStatus(Id_Venda);


            if (!retorno.Ok)
            {
                AddModelError(retorno.Mensagem, retorno.InnerException);
            }

            return View(retorno.Objeto);

            //return View();
        }

        public ActionResult AnexarArquivo(Int64 Id_Cliente)
        {
            GetListaAnexarArquivo(Id_Cliente);

            return View(new ClienteAnexoModelView
            {
                Id_Cliente = Id_Cliente
            });
        }

        [HttpPost]
        public ActionResult AnexarArquivo(ClienteAnexoModelView Model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (Request.Form["gravarfoto"] != null)
                    {
                        return AnexarArquivoAux(Model, true);
                    }
                    else
                    {
                        return AnexarArquivoAux(Model, false);
                    }
                }
                else
                {
                    return View(Model);
                }
            }
            catch (Exception ex)
            {
                if (ViewBag.Lista == null) GetListaAnexarArquivo(Model.Id_Cliente);

                AddModelError(ex.Message, ex.InnerException != null ? ex.InnerException.Message : String.Empty);
                return View(Model);
            }
        }

        public ActionResult AnexarArquivoAux(ClienteAnexoModelView Model, Boolean WebCam)
        {
            try
            {
                if (WebCam)
                {
                    Model.Nome_Arquivo = "Camera.jpg";
                    Model.Extensao_Arquivo = "jpg";
                }
                else if (Request.Files.Count > 0)
                {
                    Model.Nome_Arquivo = Request.Files[0].FileName;
                    Model.Extensao_Arquivo = Model.Nome_Arquivo.Split('.').Last();
                }

                PostApiRetorno<Int64> retornoPost = base.PostByUrl<ClienteAnexoModelView, Int64>(Model, CONSTANTES.URLPortalInterno, "api/Venda/GravarAnexo");

                GetListaAnexarArquivo(Model.Id_Cliente);

                if (!retornoPost.Ok)
                {
                    AddModelError(retornoPost.Mensagem, retornoPost.InnerException);

                    return View(Model);
                }
                else
                {
                    String _pasta = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Cliente\{Model.Id_Cliente}";
                    String _arquivo = $@"{_pasta}\{retornoPost.IdObjeto}.{Model.Extensao_Arquivo}";

                    if (!Directory.Exists(_pasta))
                    {
                        Directory.CreateDirectory(_pasta);
                    }

                    if (WebCam)
                    {
                        Image imagem1 = ImageService.Base64ToImage(Model.FotoWebCam.Replace("data:image/png;base64,", String.Empty));
                        imagem1.Save(_arquivo);
                    }
                    else if (Request.Files.Count > 0)
                    {
                        Request.Files[0].SaveAs(_arquivo);
                    }

                    return RedirectToAction("AnexarArquivo", new { Id_Cliente = Model.Id_Cliente });
                }
            }
            catch (Exception ex)
            {
                AddModelError(ex.Message, ex.InnerException?.Message);
                return View(Model);
            }
        }


        public void CarregaListasEndereco()
        {
            ViewBag.ComboTmp = new List<ComboObj>();
        }


        public async Task<ActionResult> AlterarEndereco(Int64 IdEndereco, Int64 IdCliente, Int64 IdClientePlanoProduto)
        {
            GetApiRetorno<AlterarEnderecoClienteViewModel> retorno = await new ClienteApiController().FindByIdEndereco(IdEndereco, IdCliente, IdClientePlanoProduto);

            CarregaListasEndereco();

            if (!retorno.Ok)
            {
                AddModelError(retorno.Mensagem, retorno.InnerException);
            }

            return View(retorno.Objeto);
            //return View();
        }

        public async Task<ActionResult> AlterarLoginRadius(Int64 IdPlanoProduto)//
        {
            try
            {
                GetApiRetorno<AlterarLoginRadiusViewModel> retorno = await new PlanoProdutoApiController().FindLoginByPlano(IdPlanoProduto);

                //CarregaListasEndereco();

                if (!retorno.Ok)
                {
                    AddModelError(retorno.Mensagem, retorno.InnerException);
                }

                return View(retorno.Objeto); //retorno.Objeto
            }
            catch (Exception ex)
            {
                AddModelError(ex.Message, ex.InnerException?.Message);
                return View(IdPlanoProduto);
            }
        }

        [HttpPost]
        public ActionResult AlterarLoginRadius(AlterarLoginRadiusViewModel valor)
        {

            try
            {
                PostApiRetorno<Int64> retornoPost = base.PostByUrl<AlterarLoginRadiusViewModel, Int64>(valor, CONSTANTES.URLPortalInterno, "api/PlanoProdutoapi/GravarLogin");

                if (!retornoPost.Ok)
                {
                    AddModelError(retornoPost.Mensagem, retornoPost.InnerException);

                    return View(valor);

                }
                else
                {
                    return View(valor);
                    //View();
                }
            }
            catch (Exception ex)
            {
                AddModelError(ex.Message, ex.InnerException?.Message);
                return View(valor);
            }
        }

        [HttpPost]
        public ActionResult AlterarEndereco(AlterarEnderecoClienteViewModel valor)
        {

            PostApiRetorno<Int64> retornoPost = base.PostByUrl<EnderecoClienteViewModel, Int64>(valor.EnderecoInstalacao, CONSTANTES.URLPortalInterno, "api/Venda/GravarEndereco");

            CarregaListasEndereco();

            if (!retornoPost.Ok)
            {
                AddModelError(retornoPost.Mensagem, retornoPost.InnerException);

                return View(valor);

            }
            else
            {
                return View(valor);
                //View();
            }

        }


        [HttpGet()]
        public ActionResult Mapa_Equipes(string Equipe, DateTime? Data, String Nome_Equipe, String Pagina)
        {
            if (String.IsNullOrEmpty(Equipe))
            {
                return View(new Equipe_MapaModel());
            }

            var retorno = new Mapa_EquipesApiController().GetEquipeNoMapa(Equipe, Data).Result;

            if (!retorno.Ok)
            {
                AddModelError(retorno.Mensagem, retorno.InnerException);
            }


            retorno.Objeto.Data_Busca = Data != null ? Convert.ToDateTime(Data).ToString("dd/MM/yyyy") : String.Empty;

            return View(retorno.Objeto);


        }

        public ActionResult ConsultaVendaHistorico(Int64? Id_Cliente)
        {
            var _retornoTipo_Atendimento = new Tipo_AtendimentoApiController().GetList().Result;

            if (!_retornoTipo_Atendimento.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoTipo_Atendimento.Mensagem, _retornoTipo_Atendimento.InnerException);

            ViewBag.Tipos_Atendimento = _retornoTipo_Atendimento.Objeto;
            var id = Convert.ToInt32(Request.QueryString["Id_Venda"].ToString());
            return View(new ConsultaVendaHistorico_DataTableRequest
            {
                Dt_Inicial_Filtro = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy"),
                Dt_Final_Filtro = DateTime.Now.ToString("dd/MM/yyyy"),
                Status = null,
                Id_Venda = id,
                Id_Log = id,
            });

        }

        public async Task<ActionResult> Checklist(long Id)
        {
            ViewBag.Areas = this.Areas;
            ViewBag.IdVenda = Id;

            using (var api = new ClienteApiController())
            {
                var cliente = await api.FindById(Id);

                if (cliente != null && cliente.Ok)
                {
                    ViewBag.MeioCaptacao = cliente?.Objeto?.Id_Meio_Captacao;
                    ViewBag.Uf = cliente?.Objeto?.EnderecoInstalacao?.Id_UF;
                    ViewBag.Cidade = cliente?.Objeto?.EnderecoInstalacao?.Id_Cidade;
                    ViewBag.TipoPessoa = cliente?.Objeto?.TipoPessoa;
                }
            }

            return View();
        }

        [AllowAnonymous]
        public async Task<ActionResult> VisualizarChecklist(long Venda)
        {
            ViewBag.Areas = Areas;

            using (var apiQuiz = new QuizApiController())
            using (ISession sessionNh = HibernateUtil.GetSession())
            using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
            using (ISession sessionT5Sql = HibernateUtil.GetSessionT5())
            using (ISession sessionFotoSql = HibernateUtil.GetSessionFoto())
            using (var repoDocumentoProducao = new Conexao<Documento_ProducaoAniel>(sessionT5Nh))
            {
                var model = new GetApiRetorno<List<QuizAnswer>>
                {
                    Objeto = new List<QuizAnswer>()
                };

                GetApiRetorno<List<QuizAnswer>> resultado = await apiQuiz.GetQuizAnswerVenda(Venda);

                model.Objeto = resultado.Objeto;

                return View(model.Objeto);
            }
        }

        [AllowAnonymous]
        public async Task<ActionResult> EmailChecklist(String id_venda)
        {
            var model = new EmailChecklistModel();
            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                using (ISession sessionT5Sql = HibernateUtil.GetSessionT5())
                using (var repoVenda = new Conexao<Venda>(sessionNh))
                {
                    var IdVenda = Criptografia_AES256.doDecryptAES(id_venda);

                    IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(sessionNh);
                    var configBusiness = new ConfiguracoesGeraisBusiness(repoConfig);

                    IChecklistRepository repoChecklist = new ChecklistRepository(sessionT5Nh, sessionT5Sql);
                    IEtapaVendaRepository repoEtapaVenda = new EtapaVendaRepository(sessionNh);

                    var checklistBusiness = new ChecklistBusiness(repoChecklist, repoEtapaVenda);

                    Configuracoes_Gerais config = configBusiness.GetConfiguracoesGerais();
                    Quiz checklist = checklistBusiness.GetChecklistVenda(Convert.ToInt64(IdVenda));

                    model.IdVenda = Convert.ToInt64(IdVenda);

                    model.NomeEmpresa = config.Nome_Cliente_Sinapse;
                    model.Cnpj = config.CNPJ;
                    model.LogoClienteBase64 = config.OLA_URL_Logo;

                    model.MensagemConclusao = checklist?.MensagemConcluido ?? CONSTANTES.MensagemPadraoConclusaoChecklist;
                    model.ChecklistPreenchido = checklistBusiness.ChecklistPreenchido(Convert.ToInt64(IdVenda));

                    var venda = await repoVenda.selectFirst(i => i.Id == Convert.ToInt64(IdVenda));
                    model.Status = venda.Status;
                }
            }
            catch (Exception)
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (var repoConfig = new Conexao<Configuracoes_Gerais>(sessionNh))
                {
                    var config = await repoConfig.selectFirst();
                    return RedirectToAction("FormularioConcluido", "Termo_Adesao", new TermoAdesaoFormularioConcluidoRequestModel { IsAssinado = true, MensagemErro = $@"Não existe um checklist disponível para preenchimento, entre em contato com a {config.Nome_Cliente_Sinapse}" });
                }
            }
            return View(model);
        }

        public ActionResult ConsultaVendasOrdemServico(Int64 Id_Venda)
        {
            return View(new ConsultaVendasOrdemServicoListModel
            {
                IdVenda = Id_Venda,
                DataInicio = DateTime.Now.Date,
                DataFim = DateTime.Now.Date
            });
        }

        [HttpPost]
        public async Task<ActionResult> SaveAnexosContrato(AnexosContratoAceiteRequestModel request)
        {
            var retorno = new GetApiRetorno();
            using (var session = HibernateUtil.GetSession())
            using (var sessionLog = HibernateUtil.GetSessionLog())
            using (var transaction = sessionLog.BeginTransaction())
            using (var repository = new Conexao<AnexoContratoAceiteModel>(session))
            using (var repositoryVenda = new Conexao<Venda>(session))
            {
                try
                {
                    List<AnexoContratoAceiteModel> anexosNovos = new List<AnexoContratoAceiteModel>();
                    var venda = await repositoryVenda.selectFirst(p => p.Id == request.IdVenda);
                    var anexosGravados = (await repository.select(p => p.IdVenda == request.IdVenda)).ToList();
                    string _pasta = $@"{CONSTANTES.CaminhoFisico}\Arquivos\AnexosContrato\{request.IdVenda}";
                    if (!Directory.Exists(_pasta))
                    {
                        Directory.CreateDirectory($"{_pasta}");
                    }

                    for (var i = 0; i < Request.Files.Count; i++)
                    {
                        string fileName = Request.Files[i].FileName;
                        if (!string.IsNullOrEmpty(Request.Files[i].FileName))
                        {
                            if (!anexosGravados.Any(anexo => $"{anexo.NomeArquivo}.{anexo.Extensao}" == Request.Files[i].FileName))
                            {
                                byte[] allBytes = new byte[Request.Files[i].InputStream.Length];
                                int bytesRead = Request.Files[i].InputStream.Read(allBytes, 0, (int)Request.Files[i].InputStream.Length);
                                String _arquivo = $@"{_pasta}\{Request.Files[i].FileName}";

                                System.IO.File.WriteAllBytes($@"{_arquivo}", allBytes);

                                var nomenclaturaArquivo = Request.Files[i].FileName.Split('.');
                                anexosNovos.Add(new AnexoContratoAceiteModel
                                {
                                    Descricao = i == 0 ? request.Descricao : $"{request.Descricao}({i})",
                                    NomeArquivo = nomenclaturaArquivo[0],
                                    Extensao = nomenclaturaArquivo[1],
                                    ExigeAceite = request.ExigeAceite,
                                    IdVenda = request.IdVenda,
                                    Anexo = allBytes,
                                    Status = StatusAnexoContrato.Pendente
                                });
                            }
                        }
                    }
                    if (anexosNovos.Count > 0)
                    {
                        await repository.Save(anexosNovos);
                    }

                    var sql = new StringBuilder();

                    sql.AppendLine("INSERT INTO TB_VENDAS (ID, USUARIOCRIACAO, NOMEUSUARIOCRIACAO, DATAHORACRIACAO, TIPOOPERACAO, IDLOG, CAMPO_VALORANTES, CAMPO_VALORDEPOIS, STATUSVENDA, CAMPO)")
                       .AppendLine($"VALUES (GEN_ID(GN_VENDAS, 1), @USER, @USERNAME, @DATA_ATUAL, @OPERACAO,@VENDA, '', @ANEXO, @STATUS,'Inclusão de Anexo de Contrato')");

                    foreach (var anexo in anexosNovos)
                    {
                        var parametros = new Dictionary<string, object>();
                        parametros.Add("@USER", User.Identity.GetUserId());
                        parametros.Add("@USERNAME", User.Identity.GetUserName());
                        parametros.Add("@DATA_ATUAL", DateTime.Now);
                        parametros.Add("@OPERACAO", TipoOperLog.Alteracao);
                        parametros.Add("@VENDA", venda.Id);
                        parametros.Add("@STATUS", venda.Id);
                        parametros.Add("@ANEXO", anexo.Id);

                        using (var comm = sessionLog.Connection.CreateCommand())
                        {
                            transaction.Enlist(comm);
                            await CommandHelper.ExecuteNonQuery(comm, sql, parametros, sessionLog.Connection, comm.Transaction);
                        }
                    }

                    transaction.Commit();
                    retorno.Ok = true;
                }

                catch (Exception ex)
                {
                    transaction.Rollback();
                    retorno.Ok = false;
                    retorno.Mensagem = $"{ex.Message}{" - " + ex.InnerException?.Message}";
                    CONSTANTES.log4Net.Error(ex.Message, ex);
                }
            }
            return RedirectToAction("ClienteDadosGerais", "Venda", new { IdVenda = request.IdVenda });
        }
    }
}
