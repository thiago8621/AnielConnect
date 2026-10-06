using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Models;
using Aniel.Connect.Model;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Models.HubDesenvolvedor;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.T5.ElementoRedePorta;
using DocumentFormat.OpenXml.Wordprocessing;
using FluentNHibernate.Testing.Values;
using NHibernate;
using NHibernate.Criterion;
using Resources;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Image = System.Drawing.Image;
using System.Windows.Forms;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class ClienteController : BaseController
    {
        public ClienteController() : base("Cliente",
                                            CONSTANTES.URLPortalInterno,
                                            String.Empty,
                                            "api/Clienteapi/FindById?id={0}",
                                            "api/clienteapi/Gravar",
                                            "api/Clienteapi/Excluir",
                                            String.Empty)
        {

        }

        // GET: Cliente
        public ActionResult Index()
        {
            if (Areas.Find(p => p.Cod_Area == "22") == null)
            {
                return RedirectToAction("ErroSemAcesso", "Home");
            }

            ViewBag.Areas = Areas;
            return View();
        }

        // 
        public ActionResult CadastroClientePasso1()
        {
            ViewBag.HabilitaCadastro = false;

            return View(new ClientePasso1ViewModel
            {
                TipoPessoa = Tipo_Pessoa.Fisica
            });
        }

        [HttpGet()]
        public ActionResult Importacao()
        {
            ExcelServiceLog<Cliente_Importacao> modelo = new ExcelServiceLog<Cliente_Importacao>
            {
                Items = new List<Cliente_Importacao>(),
                Log = new List<ExcelServiceLogItem>()
            };

            return View(modelo);
        }

        [HttpPost]
        public async Task<ActionResult> Importacao(String Arquivo)
        {
            ExcelServiceLog<Cliente_FisicaImportacao> modeloFisico = new ExcelServiceLog<Cliente_FisicaImportacao>
            {
                Items = new List<Cliente_FisicaImportacao>(),
                Log = new List<ExcelServiceLogItem>()
            };

            ExcelServiceLog<Cliente_JuridicaImportacao> modeloJuridico = new ExcelServiceLog<Cliente_JuridicaImportacao>
            {
                Items = new List<Cliente_JuridicaImportacao>(),
                Log = new List<ExcelServiceLogItem>()
            };

            ExcelServiceLog<Cliente_Importacao> modelo = new ExcelServiceLog<Cliente_Importacao>
            {
                Items = new List<Cliente_Importacao>(),
                Log = new List<ExcelServiceLogItem>()
            };

            try
            {
                if (!(Request.Files.Count > 0 && !String.IsNullOrEmpty(Request.Files[0].FileName)))
                {
                    AddModelError("Informe Planilha a ser Importada");
                    return View();
                }

                if (ModelState.IsValid)
                {
                    if (Request.Files.Count > 0 && !String.IsNullOrEmpty(Request.Files[0].FileName))
                    {

                        DataTable dt = ExcelService.GetDataTableFromSpreadsheet(Request.Files[0].InputStream, false, ExcelService.GetSheetName(typeof(Cliente_FisicaImportacao)));
                        modeloFisico = ExcelService.CastDataTableImportacao<Cliente_FisicaImportacao>(dt);

                        DataTable dtJuridico = ExcelService.GetDataTableFromSpreadsheet(Request.Files[0].InputStream, false, ExcelService.GetSheetName(typeof(Cliente_JuridicaImportacao)));
                        modeloJuridico = ExcelService.CastDataTableImportacao<Cliente_JuridicaImportacao>(dtJuridico);

                        using (ClienteApiController cliente_api = new ClienteApiController())
                        {
                            if (dt.Rows.Count == 0 && dtJuridico.Rows.Count == 0)
                            {
                                throw new Exception("A planilha carregada está vazia!");
                            }

                            if (dt.Rows.Count > 0)
                            {
                                foreach (var item in modeloFisico.Items)
                                {
                                    try
                                    {
                                        modeloFisico.Log.AddRange(await cliente_api.ImportacaoModeloFisico(item));
                                    }
                                    catch (Exception ex)
                                    {
                                        modeloFisico.Log.Add(new ExcelServiceLogItem
                                        {
                                            Chave = item.Numero_Contrato,
                                            Erro = $"Erro ao processar com Identificação de Cliente: {item.Numero_Contrato}"
                                        });
                                        CONSTANTES.log4Net.Error(ex.Message, ex);
                                    }
                                }
                            }

                            if (dtJuridico.Rows.Count > 0)
                            {
                                foreach (var item in modeloJuridico.Items)
                                {
                                    try
                                    {
                                        modeloJuridico.Log.AddRange(await cliente_api.ImportacaoModeloJuridico(item));
                                    }
                                    catch (Exception ex)
                                    {
                                        modeloFisico.Log.Add(new ExcelServiceLogItem
                                        {
                                            Chave = item.Numero_Contrato,
                                            Erro = $"Erro ao processar com Identificação de Cliente: {item.Numero_Contrato}"
                                        });
                                        CONSTANTES.log4Net.Error(ex.Message, ex);
                                    }
                                }
                            }
                        }
                    }

                    modelo.Items.AddRange(modeloFisico.Items);
                    modelo.Log.AddRange(modeloFisico.Log);
                    modelo.Items.AddRange(modeloJuridico.Items);
                    modelo.Log.AddRange(modeloJuridico.Log);

                    ViewBag.Sucesso = modelo.Log.Count == 0 ? "S" : "N";
                    return View(modelo);

                }
                else
                {
                    return View(modelo);
                }

            }
            catch (Exception ex)
            {
                AddModelError(ex.Message, ex.InnerException?.Message);
                return View(modelo);
            }
        }

        [HttpPost]
        public ActionResult CadastroClientePasso1(ClientePasso1ViewModel valor)
        {
            ViewBag.HabilitaCadastro = false;
            ClientePasso1ViewModel retorno = valor;

            if (Request.Form["validar"] != null)
            {
                try
                {
                    if (DateTimeUtil.GetIdade(Convert.ToDateTime(valor.DataNascimento)) < 18)
                    {
                        throw new Exception(RsCadastros.NAO_PERMITIDO_18);
                    }

                    if (valor.TipoPessoa == Tipo_Pessoa.Fisica)
                    {
                        String _cpf = valor.CPF.Replace(".", String.Empty).Replace("-", String.Empty);

                        if (new ClienteApiController().Existecpf(_cpf).Result > 0)
                            throw new Exception(String.Format("O CPF {0} já se encontra cadastrado", _cpf));

                        String _url = String.Format(CONSTANTES.URLHUBDESENVOLVEDOR_CPF, _cpf, valor.DataNascimento);
                        ResponseHub<ResponseCpf> resultadoHUB = GetByUrl<ResponseHub<ResponseCpf>>(_url);

                        retorno.retornoCPF = resultadoHUB;

                        if (retorno.retornoCPF.result == null)
                        {
                            Boolean acesso137 = TemAcesso("137");
                            String _mensage = retorno.retornoCPF.message;
                            retorno.retornoCPF = null;
                            ViewBag.HabilitaCadastro = acesso137;

                            if (_mensage == "Token Inválido ou sem saldo para a consulta.")
                            {
                                if (acesso137)
                                {
                                    throw new Exception(RsCadastros.SEM_ACESSO_BASE_RECEITA);
                                }
                                else
                                {
                                    throw new Exception(RsCadastros.USUARIO_SEM_ACESSO137);
                                }
                            }
                            else
                            {
                                throw new Exception(_mensage);
                            }
                        }
                    }
                    else if (valor.TipoPessoa == Tipo_Pessoa.Juridica)
                    {
                        String _cnpj = valor.CNPJ.Replace(".", String.Empty).Replace("-", String.Empty).Replace("/", String.Empty);
                        if (new ClienteApiController().Existecpf(_cnpj).Result > 0)
                            throw new Exception(String.Format("O CNPJ {0} já se encontra cadastrado", _cnpj));

                        String _url = String.Format(CONSTANTES.URLHUBDESENVOLVEDOR_CNPJ, _cnpj);
                        ResponseHub<ResponseCnpj> resultadoHUB = GetByUrl<ResponseHub<ResponseCnpj>>(_url);

                        retorno.retornoCNPJ = resultadoHUB;

                        if (retorno.retornoCNPJ.result == null)
                        {
                            String _mensage = retorno.retornoCNPJ.message;
                            retorno.retornoCNPJ = null;
                            ViewBag.HabilitaCadastro = true;

                            if (_mensage == "Token Inválido ou sem saldo para a consulta.")
                            {
                                throw new Exception(RsCadastros.SEM_ACESSO_BASE_RECEITA);
                            }
                            else
                            {
                                throw new Exception(_mensage);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                    if (ex.Message == RsCadastros.NAO_PERMITIDO_18)
                    {
                        ViewBag.HabilitaCadastro = false;
                    }

                    if (ex.InnerException != null)
                    {
                        if (ex.InnerException.GetType() == typeof(System.Net.Http.HttpRequestException))
                        {
                            Boolean acesso137 = TemAcesso("137");
                            ViewBag.HabilitaCadastro = acesso137;
                            AddModelError(RsCadastros.ERRO_COMUNICACAO_VALIDACAO_CPF, String.Empty, String.Empty);
                        }
                    }


                    else
                    {
                        AddModelError(ex.Message, String.Empty, String.Empty);
                    }
                }

                return View(retorno);
            }
            else if (Request.Form["cadastrar"] != null)
            {
                return RedirectToAction("ClienteDadosGerais", new
                {
                    tipo_pessoa = valor.TipoPessoa,
                    cpfcnpj = valor.TipoPessoa == Tipo_Pessoa.Fisica ? valor.CPF : valor.CNPJ,
                    datanascimento = valor.TipoPessoa == Tipo_Pessoa.Fisica ? valor.DataNascimento : valor.TipoPessoa == Tipo_Pessoa.Fisica ? valor.DataNascimento : null
                });
            }

            return View();
        }

        public async Task<ActionResult> ClienteDadosGerais(Int64? Id, Tipo_Pessoa? tipo_pessoa, String cpfcnpj,String contrato, String datanascimento, string abaAtiva = null)
        {
            //ViewResult retorno = (ViewResult)base.CadastroBase<ClienteDadosGeraisViewModel, Int64?>(Id);
            var retorno = await new ClienteApiController().FindById(Convert.ToInt64(Id));
            ClienteDadosGeraisViewModel _model = null;
            if (!Id.HasValue)
            {
                //ClienteDadosGeraisViewModel _model = ((ClienteDadosGeraisViewModel)retorno.Model);
                _model = retorno.Objeto;
                if (_model == null)
                    _model = new ClienteDadosGeraisViewModel();

                _model.TipoPessoa = tipo_pessoa ?? Tipo_Pessoa.Fisica;
                _model.CpfCnpj = cpfcnpj;
                _model.DataNascimento = datanascimento;

            }

            using (ISession session = HibernateUtil.GetSession())
            using (Conexao<Configuracoes_Gerais> conf_gerais = new Conexao<Configuracoes_Gerais>(session))
            {
                Configuracoes_Gerais conf = conf_gerais.selectFirst().Result;
                ViewBag.Criacao_Atendimento_DominioEmail = conf.Criacao_Atendimento_DominioEmail;
            }

            ViewBag.AbaAtiva = abaAtiva;
            ViewBag.NumeroContrato = contrato;
            FillListasView(retorno.Objeto ?? _model);

            ViewBag.Areas = Areas;
            return View(retorno.Objeto ?? _model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ClienteDadosGerais(ClienteDadosGeraisViewModel valor)
        {
            var config = (await new Configuracoes_GeraisApiController().FindById(1))?.Objeto;

            ViewBag.Criacao_Atendimento_DominioEmail = config.Criacao_Atendimento_DominioEmail;

            if (config.Identificacao_Cliente.Value == Identificacao_Cliente.Numero_Contrato)
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    List<ICriterion> listaCriteria = new List<ICriterion>();
                    var temp_identificacao_cliente = valor.Identificacao_Cliente == null ? String.Empty : valor.Identificacao_Cliente;

                    listaCriteria.Add(NHibernate.Criterion.Expression.Eq(nameof(Cliente.Numero_Contrato), temp_identificacao_cliente).IgnoreCase());
                    if (valor.Id != 0)
                        listaCriteria.Add(NHibernate.Criterion.Expression.Not(Restrictions.Eq(nameof(Cliente.Id), valor.Id)));

                    var cliente = new Conexao<Cliente>(session).selectCriteria(listaCriteria.ToArray()).Result.FirstOrDefault();

                    if (cliente != null)
                    {
                        AddModelError(RsCadastros.IDENTIFICACAO_CLIENTE_JA_EXISTE);
                    }
                }
            }


            var temp_id = valor.Id;

            try
            {
                if (ModelState.IsValid && Validacoes(valor))
                {
                    PostApiRetorno<Int64> retorno = PostByUrl<ClienteDadosGeraisViewModel, Int64>(valor, UrlPortal, UrlPost);

                    if (retorno.Ok) //Edição
                    {
                        Boolean tempPlanoAguardOS = valor.Planos.Find(p => p.Status == Status_PlanoProduto.Aguardando_Criacao_OS) != null;
                        String criaAtenidmento = "";
                        if (temp_id == 0)
                        {
                            if (tempPlanoAguardOS)
                                criaAtenidmento = "R"; //Redirect
                            else
                                criaAtenidmento = "S"; //Pergunta
                        }
                        return RedirectToAction("ClienteDadosGerais", new { Id = retorno.IdObjeto, Sucesso = "S", CriaAtenidmento = criaAtenidmento });

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
                FillListasView(valor);

                AddModelError(Resources.RsBase.ERRO_GRAVARDADOSNOSERVIDOR, ex.Message, ex.InnerException?.Message);
            }



            return View(valor);

        }

        protected override void FillListasView<T>(T pModel)
        {
            ViewBag.AreasAcesso = Areas;

            var campos = new CamposObrigatoriosClienteApiController().GetCampos().Result.ToList<ICamposObrigatoriosModel>();
            ViewBag.CamposObrigatorios = CamposObrigatoriosController.dictCamposObrigatorios(campos);
            ViewBag.CamposBloqueados = CamposObrigatoriosController.dictCamposBloqueados(campos);

            ViewBag.Utiliza_Cadastro_Endereco = Convert.ToBoolean(new Configuracoes_GeraisApiController().FindById(1).Result.Objeto.Utiliza_Cadastro_Endereco);
            ViewBag.ComboTmp = new List<ComboObj>();
            ViewBag.GruposArea = new Grupo_AreaAnielApiController().GetList().Result.Objeto;
            ViewBag.Areas = new List<String>();
            ViewBag.Localidades = new List<String>();
            ViewBag.Paises = new PaisApiController().GetList().Result.Objeto;
            ViewBag.Tipos_Imovel = new Tipo_ImovelAnielApiController().GetList().Result.Objeto;

            //Dados das Configurações Gerais sobre os descontos
            using (var session = HibernateUtil.GetSession())
            using (var repo = new Conexao<Configuracoes_Gerais>(session))
            {
                var conf_gerais = repo.selectFirst().Result;
                ViewBag.Descontos_Tipo_Diferente = conf_gerais.Acumular_Descontos_Diferentes;
                ViewBag.Acumula_Descontos_Mesmo_Tipo = conf_gerais.Acumular_Descontos_Mesmo_Tipo;
                ViewBag.Identificacao_Cliente = Convert.ToInt32(conf_gerais.Identificacao_Cliente);
            }


        }

        private void GetListaAnexarArquivo(Int64 Id_Cliente)
        {
            GetApiRetorno<List<ClienteAnexoModelView>> retornoLista = base.GetAll<ClienteAnexoModelView>($"api/clienteapi/GetAnexos?id_cliente={Id_Cliente}");

            if (!retornoLista.Ok)
            {
                AddModelError(retornoLista.Mensagem);
                ViewBag.Lista = new List<ClienteAnexoModelView>();
            }
            else
            {
                ViewBag.Lista = retornoLista.Objeto;
                ViewBag.Areas = Areas;
            }
        }
        [HttpGet()]
        public async Task<ActionResult> ListaLoginRadius(Int64 Id_Cliente)
        {
            var retorno = await new PlanoProdutoApiController().GetListLogin(Id_Cliente);

            if (!retorno.Ok)
            {
                AddModelError(retorno.Mensagem, retorno.InnerException);
            }

            return View(retorno.Objeto);
        }

        [HttpPost()]
        public ActionResult ListaHistoricoConsumo(String Login_Radius)
        {
            //var retorno = await new RadiusApiController().HistoricoConsumo(Login_Radius);
            //TODO Criar consulta do histórico do consumo e exibir
            var retorno = new HistoricoConsumoRadiusModel();
            return View(retorno);
            //return View();
        }

        /// <summary>
        /// MetodoGet para views de AnexarArquivo paraVenda e CliProspecto
        /// </summary>
        /// <param name="Id_Cliente"></param>
        /// <param name="Id_Venda"></param>
        /// <returns></returns>
        public ActionResult AnexarArquivo(Int64 Id_Cliente, Int64? Id_Venda = null)
        {

            GetListaAnexarArquivo(Id_Cliente);
            if (Areas.Find(p => p.Cod_Area == "263") == null || Areas.Find(p => p.Cod_Area == "264") == null)
            {
                return RedirectToAction("ErroSemAcesso", "Home");
            }

            ViewBag.Areas = Areas;

            return View(new ClienteAnexoModelView
            {

                Id_Cliente = Id_Cliente,
                Id_Venda = Id_Venda
            });


        }

        /// <summary>
        /// Metodo utilizado para a área de Venda e CliProspecto
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AnexarArquivo(List<ClienteAnexoModelView> Model)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    foreach (var anexo in Model)
                    {
                        if (anexo.Id == 0)
                        {
                            if (anexo.FotoWebCam != null)
                            {
                                AnexarArquivoAux(anexo, true);
                            }
                            else
                            {
                                AnexarArquivoAux(anexo, false);
                            }
                        }
                    }

                    return RedirectToAction(
                        "AnexarArquivo",
                        "Cliente",
                        new
                        {
                            Id_Cliente = Model.FirstOrDefault().Id_Cliente,
                            Pagina = Request.Params["Pagina"],
                            Id_Venda = Request.Params["Id_Venda"]
                        }
                    );
                }
                else
                {
                    return AnexarArquivo(Model.FirstOrDefault().Id_Cliente);
                }
            }
            catch (Exception ex)
            {
                if (ViewBag.Lista == null)
                    GetListaAnexarArquivo(Model.FirstOrDefault().Id_Cliente);

                AddModelError(ex.Message, ex.InnerException != null ? ex.InnerException.Message : String.Empty);
                return AnexarArquivo(Model.FirstOrDefault().Id_Cliente);
            }
        }



        /// <summary>
        /// Metodo Utilizado para Visualizar todos os Anexos referentes ao Cliente salvos na TB_CLIENTE_ANEXOS
        /// </summary>
        /// <param name="Id_Cliente"></param>
        /// <param name="Id_Venda"></param>
        /// <returns></returns>
        public ActionResult VisualizarAnexosCliente(Int64 Id_Cliente, Int64? Id_Venda = null)
        {

            GetListaAnexarArquivo(Id_Cliente);
            if (Areas.Find(p => p.Cod_Area == "263") == null)
            {
                return RedirectToAction("ErroSemAcesso", "Home");
            }

            ViewBag.Areas = Areas;

            return View(new ClienteAnexoModelView
            {

                Id_Cliente = Id_Cliente,
                Id_Venda = Id_Venda
            });
        }


        /// <summary>
        /// Metodo utilizado para Obter as Composicoes para os Planos do Cliente
        /// </summary>
        /// <param name="Id_Cliente"></param>
        /// <returns></returns>
        public async Task<ActionResult> GetComposicaoPlanCliente(Int64 Id_Cliente)
        {
            List<ComposicaoNotaCliModels> nfCompCliente = new List<ComposicaoNotaCliModels>();

            using (var planProdApi = new PlanoProdutoApiController())
            {
                var compsCliente = await planProdApi.ObterConfigNotaFiscalCliente(Id_Cliente);

                if (compsCliente != null)
                    nfCompCliente.AddRange(compsCliente);
            }

            return View(new ComposicaoClienteModels { IdCliente = Id_Cliente, ComposicaoNotasCliente = nfCompCliente });
        }


        /// <summary>
        /// Metodo Utilizado para salvar todos os Anexos referentes ao Cliente salvos na TB_CLIENTE_ANEXOS
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult VisualizarAnexosCliente(List<ClienteAnexoModelView> Model)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    foreach (var anexo in Model)
                    {
                        if (anexo.Id == 0)
                        {
                            if (anexo.FotoWebCam != null)
                            {
                                AnexarArquivoAux(anexo, true);
                            }
                            else
                            {
                                AnexarArquivoAux(anexo, false);
                            }
                        }
                    }

                    return RedirectToAction(
                        "VisualizarAnexosCliente",
                        "Cliente",
                        new
                        {
                            Id_Cliente = Model.FirstOrDefault().Id_Cliente,
                            Pagina = Request.Params["Pagina"],
                            Id_Venda = Request.Params["Id_Venda"]
                        }
                    );
                }
                else
                {
                    return VisualizarAnexosCliente(Model.FirstOrDefault().Id_Cliente);
                }
            }
            catch (Exception ex)
            {
                if (ViewBag.Lista == null)
                    GetListaAnexarArquivo(Model.FirstOrDefault().Id_Cliente);

                AddModelError(ex.Message, ex.InnerException != null ? ex.InnerException.Message : String.Empty);
                return VisualizarAnexosCliente(Model.FirstOrDefault().Id_Cliente);
            }
        }


        [AllowAnonymous]
        public async Task<ActionResult> FormularioCliente(Int64? Id)
        {
            using (var api = new ClienteApiController())
            using (ISession session = HibernateUtil.GetSession())
            using (var repoCliente = new Conexao<Cliente>(session))
            using (var repoCliContrato = new Conexao<ClienteContrato>(session))
            {
                var clienteContrato = await repoCliContrato.selectFirst(c => c.Id == Id);

                if (clienteContrato is null)
                    throw new Exception("Não foi encontrato o Contrato correspondente ao id informado.");

                ViewBag.StatusContrato = clienteContrato.StatusAceiteContrato;

                if (clienteContrato.StatusAceiteContrato == Status_Aceite_Contrato.Aprovado)
                {
                    var cliente = await repoCliente.selectFirst(p => p.Id == clienteContrato.Cliente.Id);
                    return RedirectToAction("FormularioClienteConcluido", new { Cliente = cliente.NomeFantasia, IsAssinado = clienteContrato.StatusAceiteContrato == Status_Aceite_Contrato.Aprovado, MensagemErro = String.Empty });
                }
                else if (clienteContrato.StatusAceiteContrato == Status_Aceite_Contrato.Recusado)
                {
                    return RedirectToAction("FormularioClienteConcluido", new ContratoFormularioCliModelConcluidoRequestModel { IsAssinado = false, IdContrato = clienteContrato.Id, MensagemErro = "Seu aceite ao contrato foi recusado. A empresa será notificada sobre a recusa" });
                }

                var retorno = await api.EncontrarContratoId(clienteContrato.Id);

                if (!retorno.Ok)
                {
                    throw new Exception(retorno.Mensagem);
                }

                return View(retorno.Objeto);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> FormularioCliente(ContratoFormularioClienteModel Model)
        {

            var retorno = await new ClienteApiController().GravarFormulario(new ParamWebApi<ContratoFormularioClienteModel>
            {
                Usuario = null,
                Objeto = Model
            });

            if (!retorno.Ok)
            {
                AddModelError(retorno.Mensagem);
            }

            if (retorno.Mensagem == RsCadastros.ACEITE_CONTRATO_RECUSADO)
            {
                return RedirectToAction("FormularioClienteConcluido", new ContratoFormularioCliModelConcluidoRequestModel { Cliente = Model.Cliente, IsAssinado = retorno.IdObjeto.IsAssinado, IdContrato = Model.IdContrato, MensagemErro = "Seu aceite ao contrato foi recusado. A empresa será notificada sobre a recusa" });
            }

            return RedirectToAction("FormularioClienteConcluido", new ContratoFormularioCliModelConcluidoRequestModel { Cliente = Model.Cliente, IsAssinado = retorno.IdObjeto.IsAssinado, IdContrato = Model.IdContrato, MensagemErro = retorno.Mensagem });
        }

        [AllowAnonymous]
        public async Task<ActionResult> FormularioClienteConcluido(ContratoFormularioCliModelConcluidoRequestModel Model)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
            using (var repoTermoAdesao = new Conexao<TermoAdesao>(session))
            {
                var config = await repoConfig.selectFirst();
                String mensagem = "";

                if (!String.IsNullOrEmpty(Model.MensagemErro))
                {
                    mensagem = Model.MensagemErro;
                }
                else if (Model.IsAssinado && String.IsNullOrEmpty(Model.MensagemErro))
                {
                    mensagem = $"Prezado(a) {Model.Cliente}, sua resposta foi gravada com sucesso!";
                }

                return View(new ContratoFormularioCliConcluidoModel
                {
                    Cliente = Model.Cliente,
                    UrlLogo = config.OLA_URL_Logo,
                    Empresa = config.Nome_Cliente_Sinapse,
                    Mensagem = mensagem,
                    Sucesso = Model.IsAssinado,
                    IdContrato = Model.IdContrato
                });
            }
        }


        public void AnexarArquivoAux(ClienteAnexoModelView Model, Boolean WebCam)
        {

            try
            {
                if (Model.Extensao_Arquivo.Contains('.'))
                    Model.Extensao_Arquivo = Model.Extensao_Arquivo.Replace(".", "");

                PostApiRetorno<Int64> retornoPost = base.PostByUrl<ClienteAnexoModelView, Int64>(Model, CONSTANTES.URLPortalInterno, "api/clienteapi/GravarAnexo");

                GetListaAnexarArquivo(Model.Id_Cliente);

                if (!retornoPost.Ok)
                {
                    AddModelError(retornoPost.Mensagem, retornoPost.InnerException);
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
                        Request.Files["Arquivo"].SaveAs(_arquivo);
                    }
                }
            }
            catch (Exception ex)
            {
                AddModelError(ex.Message, ex.InnerException?.Message);
            }
        }

        public void CarregaListasEndereco()
        {
            ViewBag.ComboTmp = new List<ComboObj>();
        }


        public async Task<ActionResult> AlterarEndereco(Int64? IdEndereco, String atendimento, Int64 IdCliente, Int64 IdClientePlanoProduto)
        {
            //GetApiRetorno<AlterarEnderecoClienteViewModel> retorno = await new ClienteApiController().FindByIdEndereco(IdEndereco.Value);
            GetApiRetorno<AlterarEnderecoClienteViewModel> retorno;

            if (!string.IsNullOrEmpty(atendimento))
            {

                retorno = await new ClienteApiController().FindByIdEndereco_ComAtendimento(IdEndereco.Value, atendimento);
            }
            else
            {
                retorno = await new ClienteApiController().FindByIdEndereco(IdEndereco.Value, IdCliente, IdClientePlanoProduto);
            }
            var campos = new CamposObrigatoriosClienteApiController().GetCampos().Result.ToList<ICamposObrigatoriosModel>();
            ViewBag.CamposObrigatorios = CamposObrigatoriosController.dictCamposObrigatorios(campos);
            ViewBag.Utiliza_Cadastro_Endereco = Convert.ToBoolean(new Configuracoes_GeraisApiController().FindById(1).Result.Objeto.Utiliza_Cadastro_Endereco);
            ViewBag.GruposArea = new Grupo_AreaAnielApiController().GetList().Result.Objeto;
            ViewBag.Areas = new List<String>();
            ViewBag.AreasAcesso = Areas;
            ViewBag.Localidades = new List<String>();
            ViewBag.Tipos_Imovel = new Tipo_ImovelAnielApiController().GetList().Result.Objeto;
            ViewBag.Paises = new PaisApiController().GetList().Result.Objeto;

            CarregaListasEndereco();

            if (!retorno.Ok)
            {
                AddModelError(retorno.Mensagem, retorno.InnerException);
            }

            return View(retorno.Objeto);
            //return View();
        }


        public async Task<ActionResult> AlterarDetalhesPlanoProduto(Int64? IdPlano_Produto, Int64? Id_porta, Int64? Id_caixa)
        {
            var obj = await new ClienteApiController().FindByIdClientePlanoProduto(IdPlano_Produto.Value);
            return View(obj.Objeto);
        }

        [HttpPost]
        public async Task<ActionResult> AlterarDetalhesPlanoProduto(PlanoProdutoListModel valor)
        {
            var retornoPost = await new PlanoProdutoApiController().AlterarDetalhesPlanoProduto(valor);

            if (!retornoPost.Ok)
            {
                AddModelError(retornoPost.Mensagem, retornoPost.InnerException);
                return View(valor);
            }
            else
            {
                return View(valor);
            }
        }

        public async Task<ActionResult> AlterarLoginRadius(Int64 IdPlanoProduto)
        {
            try
            {
                AlterarLoginRadiusViewModel model = null;
                var retorno = await new PlanoProdutoApiController().FindLoginByPlano(IdPlanoProduto);

                if (retorno != null)
                {
                    if (!retorno.Ok)
                        AddModelError(retorno.Mensagem, retorno.InnerException);

                    model = retorno.Objeto;
                }
                else
                {
                    model = new AlterarLoginRadiusViewModel
                    {
                        IdPlanoProduto = IdPlanoProduto
                    };
                }

                return View(retorno.Objeto);
            }
            catch (Exception ex)
            {
                AddModelError(ex.Message, ex.InnerException?.Message);
                return View();
            }
        }

        [HttpPost]
        public ActionResult AlterarLoginRadius(AlterarLoginRadiusViewModel valor)
        {
            using (var api = new PlanoProdutoApiController())
            {
                try
                {
                    var param = new ParamWebApi<AlterarLoginRadiusViewModel>
                    {
                        Usuario = valor.Usuario,
                        Objeto = valor
                    };

                    var retornoPost = api.GravarLogin(param).Result;

                    if (!retornoPost.Ok)
                    {
                        AddModelError(retornoPost.Mensagem, retornoPost.InnerException);
                        return View(valor);
                    }
                    else
                    {
                        return View(valor);
                    }
                }
                catch (Exception ex)
                {
                    AddModelError(ex.Message, ex.InnerException?.Message);
                    return View(valor);
                }
            }


        }

        public async Task<ActionResult> AlterarPontoAcesso(Int64 IdPlanoProduto)//
        {
            try
            {
                GetApiRetorno<AlterarPontoAcesso> retorno = await new PlanoProdutoApiController().FindByIdPontoAcesso(IdPlanoProduto);

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
        public ActionResult AlterarPontoAcesso(AlterarPontoAcesso valor)
        {

            try
            {
                PostApiRetorno<Int64> retornoPost = base.PostByUrl<AlterarPontoAcesso, Int64>(valor, CONSTANTES.URLPortalInterno, "api/PlanoProdutoapi/GravarPontoAcesso");

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

        private void CreateViewBagsAlterarEndereco()
        {
            using (var tipoImovelApi = new Tipo_ImovelAnielApiController())
            using (var paisApi = new PaisApiController())
            using (var configApi = new Configuracoes_GeraisApiController())
            using (var grupoAreaApi = new Grupo_AreaAnielApiController())
            {
                ViewBag.Tipos_Imovel = tipoImovelApi.GetList().Result.Objeto;
                ViewBag.Paises = paisApi.GetList().Result.Objeto;
                ViewBag.Utiliza_Cadastro_Endereco = Convert.ToBoolean(configApi.FindById(1).Result.Objeto.Utiliza_Cadastro_Endereco);
                ViewBag.GruposArea = grupoAreaApi.GetList().Result.Objeto;
                ViewBag.Areas = new List<String>();
                ViewBag.Localidades = new List<String>();
            }
        }

        /*public async Task<ActionResult> AlterarEndereco(long? IdEndereco, String atendimento)
        {
            GetApiRetorno<AlterarEnderecoClienteViewModel> retorno = null;

            if (!string.IsNullOrEmpty(atendimento))
            {

                retorno = await new ClienteApiController().FindByIdEndereco_ComAtendimento(IdEndereco.Value, atendimento);
            }
            else
            {
                retorno = await new ClienteApiController().FindByIdEndereco(IdEndereco.Value);

            }

            CreateViewBagsAlterarEndereco();
            CarregaListasEndereco();

            if (retorno != null)
            {
                if (!retorno.Ok)
                {
                    AddModelError(retorno.Mensagem, retorno.InnerException);
                }

                return View(retorno.Objeto);
            }
            else
            {
                return View(new AlterarEnderecoClienteViewModel());
            }
        }*/

        [HttpPost]
        public async Task<ActionResult> AlterarEndereco(AlterarEnderecoClienteViewModel valor)
        {
            var retornoPost = new PostApiRetorno<long>();

            var paramwebapi = new ParamWebApi<EnderecoClienteViewModel>();
            paramwebapi.Objeto = valor.EnderecoInstalacao;

            using (var session = HibernateUtil.GetSession())
            using (var sessionT5 = HibernateUtil.GetSessionT5())
            using (var repoAtendimento = new Conexao<Atendimento>(session))
            using (var apiAtendimento = new AtendimentoApiController())
            using (var apiCliente = new ClienteApiController())
            {
                IElementoRedePortaAnielRepository repository = new ElementoRedePortaAnielRepository(session, sessionT5);
                IConfiguracoesGeraisRepository repositoryconfig = new ConfiguracoesGeraisRepository(session);

                var business = new ElementoRedePortaAnielBusiness(repository);
                var configbusiness = new ConfiguracoesGeraisBusiness(repositoryconfig);

                var config = configbusiness.GetConfiguracoesGerais();
                if (valor.Id_Atendimento != null)
                {
                    var atendimento = await repoAtendimento.selectFirst(p => p.Id == valor.Id_Atendimento);
                    retornoPost = await apiAtendimento.GravarEnderecoAtendimento(paramwebapi, valor.Id_Atendimento);
                }
                else
                {
                    paramwebapi.Objeto.Id_ClientePlanoProduto = valor.Id_ClientePlanoProduto;
                    retornoPost = await apiCliente.GravarEndereco(paramwebapi);
                }

                await business.AlterarReservaByAlteracaoEndereco(valor?.Id_Atendimento,
                                                                 valor.Id_ClientePlanoProduto);
            }


            CreateViewBagsAlterarEndereco();
            CarregaListasEndereco();

            if (retornoPost != null && !retornoPost.Ok)
            {
                AddModelError(retornoPost.Mensagem, retornoPost.InnerException);

                return View(valor);
            }
            else
            {
                return View(valor);
            }
        }

        public async Task<ActionResult> DetalhesPlanoProduto(Int64 Id)
        {
            var obj = await new ClienteApiController().FindByIdClientePlanoProduto(Id);
            return View(obj.Objeto);
        }

        [HttpGet()]
        public ActionResult Equipamentos(Int32 Id_Cliente, Int32 Id_ClientePlanoProduto)
        {
            return View();
        }

        public override bool Validacoes<T>(T pModel)
        {
            List<CamposObrigatoriosClienteModels> campos = new CamposObrigatoriosClienteApiController().GetCampos().Result;

            Dictionary<String, String> erros = new Dictionary<String, String>();

            ClienteDadosGeraisViewModel _model = pModel as ClienteDadosGeraisViewModel;

            if (_model.TipoPessoa == Tipo_Pessoa.Juridica)
            {
                foreach (var campo in campos)
                {
                    if (campo.Obrigatorio)
                        switch (campo.Campo)
                        {
                            case Model.enumerado.CamposObrigatoriosClienteEnum.CPF_RESPONSAVEL_EMPRESA:
                                if (String.IsNullOrEmpty(_model.CPFResponsavel))
                                    erros.Add("CPFResponsavel", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.CPF_RESPONSAVEL_EMPRESA));
                                break;
                            case Model.enumerado.CamposObrigatoriosClienteEnum.CARGO_RESPONSAVEL_EMPRESA:
                                if (String.IsNullOrEmpty(_model.Cargo_Responsavel))
                                    erros.Add("Cargo_Responsavel", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.CARGO_RESPONSAVEL_EMPRESA));
                                break;
                            case Model.enumerado.CamposObrigatoriosClienteEnum.CATEGORIA_CLIENTE:
                                if (!_model.Id_Categoria_Cliente.HasValue)
                                    erros.Add("Id_Categoria_Cliente", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.CATEGORIA_CLIENTE));
                                break;
                            case Model.enumerado.CamposObrigatoriosClienteEnum.TELEFONE_CELULAR:
                                if (String.IsNullOrEmpty(_model.Telefone_Celular))
                                    erros.Add("Telefone_Celular", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.CELULAR));
                                break;
                            case Model.enumerado.CamposObrigatoriosClienteEnum.INSCRICAO_ESTADUAL:
                                if (String.IsNullOrEmpty(_model.Inscricao_Estadual))
                                    erros.Add("Inscricao_Estadual", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.INSCRICAO_ESTADUAL));
                                break;
                            case Model.enumerado.CamposObrigatoriosClienteEnum.INSCRICAO_MUNICIPAL:
                                if (String.IsNullOrEmpty(_model.Inscricao_Municipal))
                                    erros.Add("Inscricao_Municipal", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.INSCRICAO_MUNICIPAL));
                                break;
                            case Model.enumerado.CamposObrigatoriosClienteEnum.PORTE:
                                if (!_model.Porte.HasValue)
                                    erros.Add("Porte", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.PORTE));
                                break;
                            case Model.enumerado.CamposObrigatoriosClienteEnum.REPRESENTANTE:
                                if (String.IsNullOrEmpty(_model.Representante))
                                    erros.Add("Representante", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.REPRESENTANTE));
                                break;
                            case Model.enumerado.CamposObrigatoriosClienteEnum.TELEFONE1:
                                if (String.IsNullOrEmpty(_model.TelefoneFixo))
                                    erros.Add("TelefoneFixo", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.TELEFONEFIXO));
                                break;
                            case Model.enumerado.CamposObrigatoriosClienteEnum.TELEFONERECADO:
                                if (String.IsNullOrEmpty(_model.TelefoneRecado))
                                    erros.Add("TelefoneRecado", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.TELEFONERECADO));
                                break;
                            case Model.enumerado.CamposObrigatoriosClienteEnum.NOMERECADO:
                                if (String.IsNullOrEmpty(_model.NomeRecado))
                                    erros.Add("NomeRecado", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.NOMERECADO));
                                break;
                            case Model.enumerado.CamposObrigatoriosClienteEnum.DATA_ABERTURA:
                                if (!_model.Data_Abertura.HasValue)
                                    erros.Add("Data_Abertura", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.DATA_ABERTURA));
                                break;

                            case Model.enumerado.CamposObrigatoriosClienteEnum.PAIS:
                                if (_model.EnderecoPrincipal != null)
                                {
                                    if (!_model.EnderecoPrincipal.Id_Pais.HasValue)
                                        erros.Add("EnderecoPrincipal.Id_Pais", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.PAIS + " - " + Resources.RsCadastros.DADOSGERAIS));
                                }
                                /*if (_model.EnderecoCobranca != null && !_model.End_Cobranca_Igual_Principal)
                                {
                                    if (!_model.EnderecoCobranca.Id_Pais.HasValue && !_model.End_Cobranca_Igual_Principal)
                                        erros.Add("", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.PAIS + " - " + Resources.RsCadastros.INSTALACAO));
                                }*/
                                break;
                            case Model.enumerado.CamposObrigatoriosClienteEnum.CEP:
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
                            case Model.enumerado.CamposObrigatoriosClienteEnum.COMPLEMENTO:
                                if (_model.EnderecoPrincipal != null)
                                {
                                    if (String.IsNullOrEmpty(_model.EnderecoPrincipal.Complemento))
                                        erros.Add("EnderecoPrincipal.Complemento", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.COMPLEMENTO + " - " + Resources.RsCadastros.DADOSGERAIS));
                                }
                                /*if (_model.EnderecoCobranca != null)
                                {
                                    if (String.IsNullOrEmpty(_model.EnderecoCobranca.Complemento) && !_model.End_Cobranca_Igual_Principal)
                                        erros.Add("", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.COMPLEMENTO + " - " + Resources.RsCadastros.INSTALACAO));
                                }*/
                                break;
                            case Model.enumerado.CamposObrigatoriosClienteEnum.PONTO_REFERENCIA:
                                if (_model.EnderecoPrincipal != null)
                                {
                                    if (String.IsNullOrEmpty(_model.EnderecoPrincipal.Ponto_Referencia))
                                        erros.Add("EnderecoPrincipal.Ponto_Referencia", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.PONTO_REFERENCIA + " - " + Resources.RsCadastros.DADOSGERAIS));
                                }
                                /*if (_model.EnderecoCobranca!= null)
                                {
                                    if (String.IsNullOrEmpty(_model.EnderecoCobranca.Ponto_Referencia) && !_model.End_Cobranca_Igual_Principal)
                                        erros.Add("", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.PONTO_REFERENCIA + " - " + Resources.RsCadastros.INSTALACAO));
                                }*/
                                break;
                            case Model.enumerado.CamposObrigatoriosClienteEnum.TIPO_IMOVEL:
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
                            case Model.enumerado.CamposObrigatoriosClienteEnum.LOGRADOURO:
                                if (_model.EnderecoPrincipal != null)
                                {
                                    if (String.IsNullOrEmpty(_model.EnderecoPrincipal.Logradouro))
                                        erros.Add("", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.LOGRADOURO + " - " + Resources.RsCadastros.DADOSGERAIS));
                                }
                                break;
                            case Model.enumerado.CamposObrigatoriosClienteEnum.NUMERO:
                                if (_model.EnderecoPrincipal != null)
                                {
                                    if (String.IsNullOrEmpty(_model.EnderecoPrincipal.Numero))
                                        erros.Add("", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.NUMERO + " - " + Resources.RsCadastros.DADOSGERAIS));
                                }
                                break;
                            case Model.enumerado.CamposObrigatoriosClienteEnum.UF:
                                if (_model.EnderecoPrincipal != null)
                                {
                                    if (String.IsNullOrEmpty(_model.EnderecoPrincipal.UF))
                                        erros.Add("", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.UF + " - " + Resources.RsCadastros.DADOSGERAIS));
                                }
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
                            case Model.enumerado.CamposObrigatoriosClienteEnum.CATEGORIA_CLIENTE:
                                if (!_model.Id_Categoria_Cliente.HasValue)
                                    erros.Add("Id_Categoria_Cliente", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.CATEGORIA_CLIENTE));
                                break;
                            case Model.enumerado.CamposObrigatoriosClienteEnum.TELEFONE_CELULAR:
                                if (String.IsNullOrEmpty(_model.Telefone_Celular))
                                    erros.Add("Telefone_Celular", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.CELULAR));
                                break;
                            case Model.enumerado.CamposObrigatoriosClienteEnum.MEIO_CAPTACAO:
                                if (!_model.Id_Meio_Captacao.HasValue)
                                    erros.Add("Id_Meio_Captacao", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.COMO_FICOU_SABENDO));
                                break;
                            case Model.enumerado.CamposObrigatoriosClienteEnum.EMAIL_PRINCIPAL:
                                if (String.IsNullOrEmpty(_model.Email))
                                    erros.Add("Email", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.EMAIL));
                                break;
                            case Model.enumerado.CamposObrigatoriosClienteEnum.ESTADO_CIVIL:
                                if (!_model.EstadoCivil.HasValue)
                                    erros.Add("EstadoCivil", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.ESTADO_CIVIL));
                                break;
                            case Model.enumerado.CamposObrigatoriosClienteEnum.NACIONALIDADE:
                                if (String.IsNullOrEmpty(_model.Nacionalidade))
                                    erros.Add("Nacionalidade", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.NACIONALIDADE));
                                break;
                            case Model.enumerado.CamposObrigatoriosClienteEnum.NOME_MAE:
                                if (String.IsNullOrEmpty(_model.Mae))
                                    erros.Add("Mae", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.MAE));
                                break;
                            case Model.enumerado.CamposObrigatoriosClienteEnum.NOME_PAI:
                                if (String.IsNullOrEmpty(_model.Pai))
                                    erros.Add("Pai", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.PAI));
                                break;
                            case Model.enumerado.CamposObrigatoriosClienteEnum.RG:
                                if (String.IsNullOrEmpty(_model.RG))
                                    erros.Add("RG", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.RG));
                                break;
                            case Model.enumerado.CamposObrigatoriosClienteEnum.SEXO:
                                if (!_model.Sexo.HasValue)
                                    erros.Add("Sexo", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.SEXO));
                                break;
                            case Model.enumerado.CamposObrigatoriosClienteEnum.TELEFONE1:
                                if (String.IsNullOrEmpty(_model.TelefoneFixo))
                                    erros.Add("TelefoneFixo", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.TELEFONEFIXO));
                                break;
                            case Model.enumerado.CamposObrigatoriosClienteEnum.TELEFONERECADO:
                                if (String.IsNullOrEmpty(_model.TelefoneRecado))
                                    erros.Add("TelefoneRecado", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.TELEFONERECADO));
                                break;
                            case Model.enumerado.CamposObrigatoriosClienteEnum.NOMERECADO:
                                if (String.IsNullOrEmpty(_model.NomeRecado))
                                    erros.Add("NomeRecado", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.NOMERECADO));
                                break;
                            case Model.enumerado.CamposObrigatoriosClienteEnum.DATA_NASCIMENTO:
                                if (String.IsNullOrEmpty(_model.DataNascimento))
                                    erros.Add("DataNascimento", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.DATANASCIMENTO));
                                break;
                            case Model.enumerado.CamposObrigatoriosClienteEnum.DATA_EMISSAO:
                                if (String.IsNullOrEmpty(_model.DataEmissao))
                                    erros.Add("DataEmissao", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.DATAEMISSAO));
                                break;
                            case Model.enumerado.CamposObrigatoriosClienteEnum.ORGAO:
                                if (String.IsNullOrEmpty(_model.Orgao))
                                    erros.Add("Orgao", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.ORGAO));
                                break;

                            case Model.enumerado.CamposObrigatoriosClienteEnum.PAIS:
                                if (_model.EnderecoPrincipal != null)
                                {
                                    if (!_model.EnderecoPrincipal.Id_Pais.HasValue)
                                        erros.Add("EnderecoPrincipal.Id_Pais", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.PAIS + " - " + Resources.RsCadastros.DADOSGERAIS));
                                }
                                /*if (_model.EnderecoCobranca != null && !_model.End_Cobranca_Igual_Principal)
                                {
                                    if (!_model.EnderecoCobranca.Id_Pais.HasValue && !_model.End_Cobranca_Igual_Principal)
                                        erros.Add("", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.PAIS + " - " + Resources.RsCadastros.INSTALACAO));
                                }*/
                                break;
                            case Model.enumerado.CamposObrigatoriosClienteEnum.CEP:
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
                            case Model.enumerado.CamposObrigatoriosClienteEnum.COMPLEMENTO:
                                if (_model.EnderecoPrincipal != null)
                                {
                                    if (String.IsNullOrEmpty(_model.EnderecoPrincipal.Complemento))
                                        erros.Add("EnderecoPrincipal.Complemento", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.COMPLEMENTO + " - " + Resources.RsCadastros.DADOSGERAIS));
                                }
                                /*if (_model.EnderecoCobranca != null)
                                {
                                    if (String.IsNullOrEmpty(_model.EnderecoCobranca.Complemento) && !_model.End_Cobranca_Igual_Principal)
                                        erros.Add("", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.COMPLEMENTO + " - " + Resources.RsCadastros.INSTALACAO));
                                }*/
                                break;
                            case Model.enumerado.CamposObrigatoriosClienteEnum.PONTO_REFERENCIA:
                                if (_model.EnderecoPrincipal != null)
                                {
                                    if (String.IsNullOrEmpty(_model.EnderecoPrincipal.Ponto_Referencia))
                                        //erros.Add("", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.PONTO_REFERENCIA + " - " + Resources.RsCadastros.DADOSGERAIS));
                                        erros.Add("EnderecoPrincipal.Ponto_Referencia", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.PONTO_REFERENCIA + " - " + Resources.RsCadastros.DADOSGERAIS));
                                }
                                /*if (_model.EnderecoCobranca != null)
                                {
                                    if (String.IsNullOrEmpty(_model.EnderecoCobranca.Ponto_Referencia) && !_model.End_Cobranca_Igual_Principal)
                                        erros.Add("", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.PONTO_REFERENCIA + " - " + Resources.RsCadastros.INSTALACAO));
                                }*/
                                break;
                            case Model.enumerado.CamposObrigatoriosClienteEnum.TIPO_IMOVEL:
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
                            case Model.enumerado.CamposObrigatoriosClienteEnum.LOGRADOURO:
                                if (_model.EnderecoPrincipal != null)
                                {
                                    if (String.IsNullOrEmpty(_model.EnderecoPrincipal.Logradouro))
                                        erros.Add("", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.LOGRADOURO + " - " + Resources.RsCadastros.DADOSGERAIS));
                                }
                                break;
                            case Model.enumerado.CamposObrigatoriosClienteEnum.NUMERO:
                                if (_model.EnderecoPrincipal != null)
                                {
                                    if (String.IsNullOrEmpty(_model.EnderecoPrincipal.Numero))
                                        erros.Add("", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.NUMERO + " - " + Resources.RsCadastros.DADOSGERAIS));
                                }
                                break;
                            case Model.enumerado.CamposObrigatoriosClienteEnum.UF:
                                if (_model.EnderecoPrincipal != null)
                                {
                                    if (String.IsNullOrEmpty(_model.EnderecoPrincipal.UF))
                                        erros.Add("", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.UF + " - " + Resources.RsCadastros.DADOSGERAIS));
                                }
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

        public ActionResult PesquisaIndex(Int64? Id_Cliente)
        {
            ViewBag.Id_Cliente = Id_Cliente ?? 0;

            return View();
        }


        public ActionResult Historico_NotaFiscal(Int32 Id_Cliente)
        {
            var retorno = new ClienteApiController().FindById(Id_Cliente).Result;

            var modelo = new Historico_Nota_FiscalModel() { Id_Cliente = retorno.Objeto.Id };

            if (!retorno.Ok)
            {
                AddModelError(retorno.Mensagem, retorno.InnerException);
            }


            return View();
        }

        public ActionResult ListaContatos(Int64 Id_Cliente)
        {
            return View();
        }

        public ActionResult DetalhesDescontoCliente(Int64? IdClienteDesconto)
        {
            ViewBag.ClienteDesconto = IdClienteDesconto;
            return View();
        }
        public ActionResult EquipamentosCliente(string Cpf_Cnpj)
        {
            ViewBag.Cpf_Cnpj = Cpf_Cnpj;
            return View();

        }
        public ActionResult ConsultaCredito(string Id_Cliente)
        {
            ViewBag.IdCliente = Id_Cliente;
            return View();

        }
    }
}