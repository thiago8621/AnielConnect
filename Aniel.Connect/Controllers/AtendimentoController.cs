using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.DAO.map;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Socket.ChatHub;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using NHibernate;
using Sinapse.DAO;
using Sinapse.DAO.repository;
using Sinapse.Model.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class AtendimentoController : BaseController
    {
        UserManager<IdentityUser> _userManager;

        public AtendimentoController() : base("Atendimento",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/atendimentoapi/FindById?id={0}",
                                                   "api/atendimentoapi/Gravar",
                                                   "api/atendimentoapi/Excluir",
                                                   String.Empty)
        {

        }

        //Painel de Atendimentos
        public ActionResult Index()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        //Acompanhamento de Agentes
        public ActionResult Acompanhamento_Agentes()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        // GET: Atendimento
        public ActionResult Listagem_Atendimento()
        {
            if (Areas.Find(p => p.Cod_Area == "04") == null)
            {
                return RedirectToAction("ErroSemAcesso", "Home");
            }

            ViewBag.Areas = Areas;

            using (ISession session = HibernateUtil.GetSession())
            {
                var usuario = GetUserManager(session).FindById(User.Identity.GetUserId());
                //ViewBag.Departamento_Usuario = usuario.Funcionario?.Departamento?.Id;
                ViewBag.Departamento_Usuario = usuario.Departamento?.Id;
            }

            return View(new AtendimentoFiltroModel());
        }

        public ActionResult Chat_iFrame()
        {
            ViewBag.Areas = Areas;
            return View();
        }

        [HttpGet()]
        public ActionResult Historico_Alteracao(Int64? id)
        {
            ViewBag.Chave = "Id";
            ViewBag.TiposOperacao = new List<SelectListItem>
                    {
                        new SelectListItem{ Value = "1", Text = Resources.RsBase.INSERCAO },
                        new SelectListItem{ Value = "2", Text = Resources.RsBase.ALTERACAO },
                        //new SelectListItem{ Value = "3", Text = Resources.RsBase.EXCLUSAO },
                        new SelectListItem{ Value = "4", Text = Resources.RsBase.BAIXA },
                    };
            return View();
        }

        // POST: Atendimento/Cadastro/5

        [HttpGet()]
        public async Task<ActionResult> Cadastro(Int64? id, Int64? Id_Cliente,
                                                 String Numero_Protocolo, Int64? IdVenda,
                                                 Int64? IdContrato, Int64? IdClientePlanoProduto,
                                                 String IdTrocaTitularidade)
        {
            var model = new AtendimentoCadastroModel();
            var atendimento_api = new AtendimentoApiController();
            using (ISession session = HibernateUtil.GetSession())
            using (var repoConfiguracaoGeral = new Conexao<Configuracoes_Gerais>(session))
            using (var repoContratocliente = new Conexao<ClienteContrato>(session))
            using (var repoClientePlano = new Conexao<Cliente_PlanoProduto>(session))
            using (PlanoProdutoApiController planoProdutoapi = new PlanoProdutoApiController())
            using (AtendimentoApiController atendimentoApi = new AtendimentoApiController())           
            {
                if (IdVenda.HasValue && IdContrato.HasValue && IdClientePlanoProduto.HasValue)
                {
                    model.Id_Cliente_Contrato = Convert.ToString(IdContrato);
                    model.Id_Venda = IdVenda;
                    model.Id_Cliente_Plano_Produto = IdClientePlanoProduto;
                }
                else if (!string.IsNullOrEmpty(IdTrocaTitularidade) && IdContrato.HasValue)
                {
                    model.Id_Cliente_Contrato = Convert.ToString(IdContrato);
                    model.Id_TrocaTitularidade = Convert.ToInt64(IdTrocaTitularidade);
                    model.Id_Cliente_Plano_Produto = (await repoClientePlano.selectFirst(x => x.ClienteContrato.Id == IdContrato)).Id;
                }

                if (!String.IsNullOrEmpty(Numero_Protocolo) && !id.HasValue)
                {
                    id = await atendimentoApi.FindIdByProtocolo(Numero_Protocolo);
                }

                var usuario = GetUserManager(session).FindById(User.Identity.GetUserId());
                ViewBag.Departamento_Usuario = usuario.Departamento?.Id;
                Cliente cliente = Id_Cliente.HasValue ? await new Conexao<Cliente>(session).selectFirst(p => p.Id == Id_Cliente) : null;
                
                FillListasView(model);
                ViewBag.Tipos_Imovel = (await new Tipo_ImovelAnielApiController().GetList()).Objeto;

                if (usuario.Departamento == null)
                {
                    return RedirectToAction("Listagem_Atendimento", "Atendimento", new { SemSetor = "S" });
                }

                if (Id_Cliente.HasValue)
                {
                    model.Id_Cliente = Id_Cliente;
                    model.CPF_CNPJ_Cliente = cliente.CpfCnpj;
                }
                else if (id.HasValue)
                {
                    model = (await atendimentoApi.FindById(id.Value)).Objeto;
                }

                ViewBag.Atendimento_Cria_Cliente = (await repoConfiguracaoGeral.selectFirst())?.Atendimento_Cria_Cliente ?? false;
                ViewBag.CriarAtendimentoSemPlanoProduto = (await repoConfiguracaoGeral.selectFirst())?.CriarAtendimentoSemPlanoProduto ?? false;
                ViewBag.Criacao_Atendimento_DominioEmail = (await repoConfiguracaoGeral.selectFirst())?.Criacao_Atendimento_DominioEmail ?? false;

                var usuarioDepar = GetUserManager(session).FindById(User.Identity.GetUserId());
                ViewBag.Departamento_Usuario = usuarioDepar.Departamento?.Id;
                ViewBag.Id_Usuario = usuarioDepar.Funcionario?.Id;
                ViewBag.HistoricoOs = atendimento_api.HistoricoVincular(new HistoricoVincularResponse_DataTableRequest { NumeroAtendimento = model.Protocolo }).Result.data.Count() > 0;
            }

            if (String.IsNullOrEmpty(model?.Numero) == false)
            {
                ViewBag.ChatAtivo = ChatHub.ChatActive(model.Numero);
                ViewBag.ChatSuporte = ChatHub.GetChatSuporte(model.Numero);
            }
            else
            {
                ViewBag.ChatAtivo = false;
            }
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Chat(String Numero_Prototocolo, Int64? id, Int64? Id_Cliente)
        {


            var model = new AtendimentoCadastroModel();

            List<SelectValue> usuariosResp = new List<SelectValue>();

            using (ISession session = HibernateUtil.GetSession())
            using (PlanoProdutoApiController planoProdutoapi = new PlanoProdutoApiController())
            //using (var usuRepo = new Conexao<IdentityUser>(session))
            {
                var usuario = GetUserManager(session).FindById(User.Identity.GetUserId());

                var cliente = await new Conexao<Cliente>(session).selectFirst(p => p.Id == Id_Cliente);


                FillListasView(model);

                if (usuario.Departamento == null)
                {
                    return RedirectToAction("Listagem_Atendimento", "Atendimento", new { SemSetor = "S" });
                }

                if (Id_Cliente.HasValue)
                {
                    model.Id_Cliente = Id_Cliente;
                    model.CPF_CNPJ_Cliente = cliente.CpfCnpj;
                }
                else if (id.HasValue)
                {
                    model = (await new AtendimentoApiController().FindById(id.Value)).Objeto;
                }

            }

            ViewBag.Responsaveis = usuariosResp;

            using (var api = new AtendimentoApiController())
            {
                return View(await api.DadosAtendimentoChat(Request.Params["Numero_Protocolo"]));
            }


        }


        public ActionResult Painel_Atendimento(String protocolo)
        {
            var model = new AtendimentoCadastroModel();
            model.Protocolo = protocolo;

            using (ISession session = HibernateUtil.GetSession())
            using (var repoConfiguracaoGeral = new Conexao<Configuracoes_Gerais>(session))
            {
                var usuario = GetUserManager(session).FindById(User.Identity.GetUserId());
                ViewBag.Nome_Func = usuario.Nome;
                ViewBag.Criacao_Atendimento_DominioEmail = repoConfiguracaoGeral.selectFirst().Result?.Criacao_Atendimento_DominioEmail ?? false;
            }

            FillListasView();
            ViewBag.Areas = Areas;
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Cadastro(AtendimentoCadastroModel Model)
        {
            ViewBag.MensagemReabrirOsOuCancelarAtd = "";
            FillListasView();
            if (String.IsNullOrEmpty(Model.Numero) == false)
            {
                FillListasView(Model);
                ViewBag.ChatAtivo = ChatHub.ChatActive(Model.Numero);
            }

            try
            {
                if (ValidacoesAtendimento(Model).Count == 0)
                {
                    using (AtendimentoApiController api = new AtendimentoApiController())
                    {
                        PostApiRetornoAtendimento retornoPost = await api.Gravar(new ParamWebApi<AtendimentoCadastroModel>
                        {
                            Usuario = User.Identity.GetUserId(),
                            Objeto = Model
                        });

                        if (Model?.Id == null || Model?.Id == 0)
                        {
                            foreach (var item in Model.Historico_Atendimento)
                            {
                                await GravarAcao(new GravarAcaoAtendimentoModel
                                {
                                    IdAtendimento = retornoPost.IdObjeto,
                                    DescricaoAcao = item.descricaoAcao,
                                    Usuario = User.Identity.GetUserId(),
                                    Arquivos = item.arquivos,
                                    TipoAtividade = item.tipoAtividade
                                });
                            }
                        }

                        if (!retornoPost.Ok)
                        {
                            //if (retornoPost.Mensagens[0].Mensagem == "SIM")
                            if (retornoPost.Mensagem == "S")
                            {
                                //AddModelError(" Reabrir OS ou cancelar atendimento ?", retornoPost.InnerException);

                                retornoPost.Mensagem = null;
                                retornoPost.Mensagens = null;

                                ViewBag.MensagemReabrirOsOuCancelarAtd = "S";
                                return View(Model);
                            }
                            else if (retornoPost.Mensagem == "N")
                            {
                                //AddModelError("Atendimento só poderá ser concluido se a OS vinculada estiver fechada.", retornoPost.InnerException);

                                retornoPost.Mensagem = null;
                                retornoPost.Mensagens = null;

                                ViewBag.MensagemReabrirOsOuCancelarAtd = "N";

                                return View(Model);
                            }
                            else
                            {

                                return View(Model);
                            }
                        }
                        else
                        {

                            if (Model.Criar_Os_Encerrar_Parametro_Controller == true && Model.Id_Plano_Produto != null && Model.Id_Plano_Produto > 0 && Model.Id > 0)
                            {
                                return RedirectToAction("Criar_Ordem_Servico", "Painel_Servicos", new
                                {
                                    Id_Atendimento = Model.Id
                                });
                            }
                            else
                            {
                                FillListasView();
                                return RedirectToAction("Cadastro", "Atendimento", new { Sucesso = "S", Id = retornoPost.IdObjeto });
                            }

                        }
                    }
                }



                return View(Model);
            }
            catch (Exception ex)
            {
                AddModelError(ex.Message, ex.InnerException?.Message);
                return View(Model);
            }


        }


        protected override void FillListasView<T>(T pModel)
        {
            ViewBag.AreasAcesso = Areas;
            ViewBag.SweetAlertCadastrado = Request.Params["primeiro_cadastro"] == "0" ? true : false;


            var campos = new CamposObrigatoriosAtendimentoApiController().GetCampos().Result.ToList<ICamposObrigatoriosModel>();

            ViewBag.CamposObrigatorios = CamposObrigatoriosController.dictCamposObrigatorios(campos);
            ViewBag.CamposBloqueados = CamposObrigatoriosController.dictCamposBloqueados(campos);

            using (ISession session = HibernateUtil.GetSession())
            {
                var usuario = GetUserManager(session).FindById(User.Identity.GetUserId());

                ViewBag.Departamento_Usuario = usuario.Funcionario?.Departamento?.Id;
            }
        }

        private UserManager<IdentityUser> GetUserManager(ISession session)
        {
            if (_userManager == null)
                _userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(session));
            return _userManager;
        }

        protected void FillListasView()
        {

            var campos = new CamposObrigatoriosAtendimentoApiController().GetCampos().Result.ToList<ICamposObrigatoriosModel>();

            var dicionarioObrigatorios = CamposObrigatoriosController.dictCamposObrigatorios(campos);
            var dicionarioBloqueados = CamposObrigatoriosController.dictCamposBloqueados(campos);

            ViewBag.CamposObrigatorios = dicionarioObrigatorios;
            ViewBag.CamposBloqueados = dicionarioBloqueados;

        }

        // GET: Atendimento/Delete/5
        public ActionResult Delete(Int64? id)
        {
            if (!id.HasValue)
                id = 0;

            return base.DeleteBase<AtendimentoCadastroModel, Int64>(id.Value);
        }

        // POST: Atendimento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(AtendimentoCadastroModel pModel)
        {
            return base.DeleteBase<AtendimentoCadastroModel, Int64>(pModel);
        }

        // GET: Historico
        public ActionResult HistoricoAtendimento(Int64 IdCliente)
        {
            return View();
        }


        public async Task<ActionResult> Script_Atendimento(Int64 IdTipoAtendimento, Int64 IdAtendimento, string Sucesso = "N")
        {
            GetApiRetorno<List<Atendimento_Script_AtendimentoModel>> retorno = new GetApiRetorno<List<Atendimento_Script_AtendimentoModel>>();

            using (AtendimentoApiController api = new AtendimentoApiController())
            {
                try
                {

                    retorno = await api.GetViewScriptAtendimento(IdTipoAtendimento, IdAtendimento);


                    var status = (await api.FindById(IdAtendimento)).Objeto?.Status_Atendimento;

                    ViewBag.AtendimentoCanceladoConcluido = status == Status_Atendimento.Concluido || status == Status_Atendimento.Cancelado;
                    ViewBag.Sucesso = Sucesso;

                    if (!retorno.Ok)
                    {
                        AddModelError(retorno.Mensagem, retorno.InnerException);
                        retorno.Objeto = new List<Atendimento_Script_AtendimentoModel>();
                    }
                }
                catch (Exception ex)
                {
                    AddModelError(ex.Message);
                    retorno.Objeto = new List<Atendimento_Script_AtendimentoModel>();
                }

                return View(retorno.Objeto);
            }
        }

        public String GetScript_Atendimento(Int64 IdTipoAtendimento)
        {
            GetApiRetorno<List<Atendimento_Script_AtendimentoModel>> retorno = new GetApiRetorno<List<Atendimento_Script_AtendimentoModel>>();

            using (AtendimentoApiController api = new AtendimentoApiController())
            {
                try
                {

                    retorno = api.GetViewScriptAtendimento(IdTipoAtendimento, null).Result;

                    if (!retorno.Ok)
                    {
                        AddModelError(retorno.Mensagem, retorno.InnerException);
                        retorno.Objeto = new List<Atendimento_Script_AtendimentoModel>();
                    }
                }
                catch (Exception ex)
                {
                    retorno.Objeto = new List<Atendimento_Script_AtendimentoModel>();
                    retorno.Mensagem = ex.Message;
                }

                return JsonConvert.SerializeObject(retorno);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Script_Atendimento(List<Atendimento_Script_AtendimentoModel> pModel)
        {
            PostApiRetorno<Int64> retorno = new PostApiRetorno<Int64>();

            try
            {
                retorno = await new AtendimentoApiController().GravarScriptAtendimento(pModel);

                if (!retorno.Ok)
                {
                    AddModelError(retorno.Mensagem, retorno.InnerException);
                    return View(pModel);
                }
            }
            catch (Exception ex)
            {
                AddModelError(ex.Message);
                return View(pModel);
            }

            return await Script_Atendimento(pModel[0].Id_Tipo_Atendimento, pModel[0].Id_Atendimento.Value);
        }

        public ActionResult ConsultaClienteAtendimento(Int64? Id_Cliente)
        {
            var _retornoTipo_Atendimento = new Tipo_AtendimentoApiController().GetList().Result;

            if (!_retornoTipo_Atendimento.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoTipo_Atendimento.Mensagem, _retornoTipo_Atendimento.InnerException);

            ViewBag.Tipos_Atendimento = _retornoTipo_Atendimento.Objeto;

            return View(new ConsultaClienteAtendimento_DataTableRequest
            {
                Dt_Inicial_Filtro = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy"),
                Dt_Final_Filtro = DateTime.Now.ToString("dd/MM/yyyy"),
                Status = null,
                Id_Cliente = Id_Cliente
            });
        }
        public ActionResult VincularHistoricoVenda(Int64? Id_Cliente)
        {
            var _retornoTipo_Atendimento = new Tipo_AtendimentoApiController().GetList().Result;

            if (!_retornoTipo_Atendimento.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoTipo_Atendimento.Mensagem, _retornoTipo_Atendimento.InnerException);

            ViewBag.Tipos_Atendimento = _retornoTipo_Atendimento.Objeto;

            return View(new ConsultaClienteAtendimento_DataTableRequest
            {
                Dt_Inicial_Filtro = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy"),
                Dt_Final_Filtro = DateTime.Now.ToString("dd/MM/yyyy"),
                Status = null,
                Id_Cliente = Id_Cliente
            });
        }

        public ActionResult HistoricoVenda(Int64? Id_Cliente)
        {
            var _retornoTipo_Atendimento = new Tipo_AtendimentoApiController().GetList().Result;

            if (!_retornoTipo_Atendimento.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoTipo_Atendimento.Mensagem, _retornoTipo_Atendimento.InnerException);

            ViewBag.Tipos_Atendimento = _retornoTipo_Atendimento.Objeto;

            return View(new ConsultaClienteAtendimento_DataTableRequest
            {
                Dt_Inicial_Filtro = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy"),
                Dt_Final_Filtro = DateTime.Now.ToString("dd/MM/yyyy"),
                Status = null,
                Id_Cliente = Id_Cliente
            });
        }

        public ActionResult ConsultaVendaAtendimento(Int64? Id_Cliente)
        {
            var _retornoTipo_Atendimento = new Tipo_AtendimentoApiController().GetList().Result;

            if (!_retornoTipo_Atendimento.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoTipo_Atendimento.Mensagem, _retornoTipo_Atendimento.InnerException);

            ViewBag.Tipos_Atendimento = _retornoTipo_Atendimento.Objeto;

            return View(new ConsultaClienteAtendimento_DataTableRequest
            {
                Dt_Inicial_Filtro = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy"),
                Dt_Final_Filtro = DateTime.Now.ToString("dd/MM/yyyy"),
                Status = null,
                Id_Cliente = Id_Cliente
            });
        }

        public ActionResult Ordens_Servico(Int64? Id_Atendimento)
        {
            return View();
        }

        public ActionResult Historico_Chamadas(Int64? Id_Atendimento)
        {
            return View();
        }




        public ActionResult Acompanhamento_Atendimentos()
        {
            List<AcompanhamentoAtendimentoModel> atendimentos = new List<AcompanhamentoAtendimentoModel>();
            if (Areas.Find(p => p.Cod_Area == "185") == null)
            {
                atendimentos = ChatHub.AcompanhamentoAtendimento(User.Identity.GetUserId(), true);
            }
            else
            {
                atendimentos = ChatHub.AcompanhamentoAtendimento(User.Identity.GetUserId(), false);
            }
            ViewBag.Aguardando = atendimentos.FindAll(p => p.agente == null && p.bot == false);
            ViewBag.EmAtendimento = atendimentos.FindAll(p => p.agente != null && p.bot == false);
            ViewBag.EmAtendimentoBot = atendimentos.FindAll(p => p.agente == null && p.bot == true);
            return View();
        }

        [HttpPost()]
        public async Task<String> GravarAcao(GravarAcaoAtendimentoModel model)
        {
            String _pasta = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Atendimento";
            using (AtendimentoApiController api = new AtendimentoApiController())
            {
                List<ArquivoAcaoModel> temp = new List<ArquivoAcaoModel>();
                for (var i = 0; i < Request.Files.Count; i++)
                {
                    if (!String.IsNullOrEmpty(Request.Files[i].FileName))
                    {
                        foreach (var arquivo in model.Arquivos ?? new List<ArquivoAcaoModel>())
                        {

                            if (arquivo.nome_Arquivo + arquivo.extensao == Request.Files[i].FileName)
                            {
                                temp.Add(new ArquivoAcaoModel
                                {
                                    nome_Arquivo = Path.GetFileNameWithoutExtension(Request.Files[i].FileName),
                                    extensao = Path.GetExtension(Request.Files[i].FileName)
                                });
                            }

                        }
                    }
                }

                model.Arquivos = temp;
                var retorno = await api.GravarAcao(model);


                if (!Directory.Exists(_pasta))
                {
                    Directory.CreateDirectory(_pasta);
                }

                if (model.Arquivos.Count > 0)
                {
                    foreach (var arquivo in retorno.IdObjeto.Arquivos_AtendimentoAcao)
                    {
                        for (var i = 0; i < Request.Files.Count; i++)
                        {
                            if (!String.IsNullOrEmpty(Request.Files[i].FileName))
                            {
                                if (arquivo.Arquivo + arquivo.Extensao == Request.Files[i].FileName)
                                {
                                    byte[] allBytes = new byte[Request.Files[i].InputStream.Length];
                                    int bytesRead = Request.Files[i].InputStream.Read(allBytes, 0, (int)Request.Files[i].InputStream.Length);
                                    String _arquivo = _arquivo = $@"{_pasta}\{arquivo.Id}{arquivo.Extensao}";
                                    System.IO.File.WriteAllBytes($@"{_arquivo}", allBytes);
                                }
                            }
                        }
                    }
                }


                return JsonConvert.SerializeObject(new PostApiRetorno
                {
                    Ok = retorno.Ok,
                    InnerException = retorno.InnerException,
                    Mensagem = retorno.Mensagem,
                    Mensagens = retorno.Mensagens
                });
            }
        }

        public Dictionary<String, String> ValidacoesAtendimento<T>(T pModel)
        {
            List<CamposObrigatoriosAtendimentoModels> campos = new CamposObrigatoriosAtendimentoApiController().GetCampos().Result;

            Dictionary<String, String> erros = new Dictionary<String, String>();

            AtendimentoCadastroModel _model = pModel as AtendimentoCadastroModel;


            foreach (var campo in campos)
            {
                if (campo.Obrigatorio)
                    switch (campo.Campo)
                    {
                        case CamposObrigatoriosAtendimentoEnum.ASSUNTO:
                            {
                                if (String.IsNullOrEmpty(_model.Assunto))
                                {
                                    erros.Add("Assunto", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.ASSUNTO));
                                }

                                break;
                            }
                        case CamposObrigatoriosAtendimentoEnum.CATEGORIA_ATENDIMENTO:
                            {
                                if (!_model.Id_Categoria_Atendimento.HasValue)
                                {
                                    erros.Add("Id_Categoria_Atendimento", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.CATEGORIA_ATENDIMENTO));
                                }
                                break;
                            }
                        case CamposObrigatoriosAtendimentoEnum.ORIGEM:
                            {
                                if (!_model.Id_Origem_Atendimento.HasValue)
                                {
                                    erros.Add("Id_Origem_Atendimento", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.ORIGEM));
                                }
                                break;
                            }
                        case CamposObrigatoriosAtendimentoEnum.PESSOA_QUE_ENTROU_EM_CONTATO:
                            {
                                if (String.IsNullOrEmpty(_model.Pessoa_Contato))
                                {
                                    erros.Add("Pessoa_Contato", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.PESSOA_QUE_ENTROU_EM_CONTATO));
                                }
                                break;
                            }
                        case CamposObrigatoriosAtendimentoEnum.PRIORIDADE:
                            {
                                if (!_model.Prioridade.HasValue)
                                {
                                    erros.Add("Prioridade", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.PRIORIDADE));
                                }
                                break;
                            }
                        case CamposObrigatoriosAtendimentoEnum.SETOR_ENVOLVIDO:
                            {
                                if (!_model.Id_Setor.HasValue)
                                {
                                    erros.Add("Id_Setor", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.SETOR));
                                }
                                break;
                            }
                            //case CamposObrigatoriosAtendimentoEnum.PROBLEMA_RELATADO:
                            //    {
                            //        erros.Add("ProblemaRelatado", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.CPF_RESPONSAVEL_EMPRESA));
                            //        break;
                            //    }

                    }
            }

            if (erros.Count > 0)
            {
                AddModelError(erros);
            }

            return erros;
        }
        [HttpGet]
        public ActionResult ConsultaCredito(string Id_Cliente)
        {
            ViewBag.IdCliente = Id_Cliente;
            return View();

        }
        [HttpPost]
        public ActionResult RetirarNegativacao(string Id_Cliente)
        {
            ViewBag.IdCliente = Id_Cliente;
            return View();

        }

        public ActionResult VincularOs(Int64? Id_Cliente)
        {
            var _retornoTipo_Atendimento = new Tipo_AtendimentoApiController().GetList().Result;

            if (!_retornoTipo_Atendimento.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoTipo_Atendimento.Mensagem, _retornoTipo_Atendimento.InnerException);

            ViewBag.Tipos_Atendimento = _retornoTipo_Atendimento.Objeto;

            return View(new VincularAtendimento_DataTableRequest
            {
                Dt_Inicial_Filtro = DateTime.Now.AddMonths(-1).ToString("dd/MM/yyyy"),
                Dt_Final_Filtro = DateTime.Now.ToString("dd/MM/yyyy"),
                Status = null,
                Id_Cliente = Id_Cliente
            });
    }
        public ActionResult HistoricoVincularOs(String numero)
        {
            var _retornoTipo_Atendimento = new Tipo_AtendimentoApiController().GetList().Result;

            if (!_retornoTipo_Atendimento.Ok)
                AddModelError(Resources.RsBase.ERROAORECUPERARLISTAGEMDOSERVIDOR, _retornoTipo_Atendimento.Mensagem, _retornoTipo_Atendimento.InnerException);

            ViewBag.Tipos_Atendimento = _retornoTipo_Atendimento.Objeto;

            return View(new HistoricoVincularResponse_DataTableRequest
            {
                NumeroAtendimento = numero
            });
        }


    }
}