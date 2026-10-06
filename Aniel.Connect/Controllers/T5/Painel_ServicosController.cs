using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.ControllersWebApi.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Models;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.T5.DocumentoProducao;
using Aniel.Connect.Repository.T5.SubtipoServico;
using Aniel.Connect.Repository.T5.TipoServico;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using Microsoft.AspNet.Identity;
using NHibernate;
using RestSharp.Extensions;
using Sinapse.DAO.repository;
using Sinapse.Model.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class Painel_ServicosController : BaseController
    {

        public Painel_ServicosController() : base("Painel_Servicos",
                                                   CONSTANTES.URLPortalInterno,
                                                   String.Empty,
                                                   "api/Painel_Servicosapi/FindById?id={0}",
                                                   "api/Painel_Servicosapi/Gravar",
                                                   "api/Painel_Servicosapi/Excluir",
                                                   String.Empty)
        {

        }

        // GET: Timeline_Servicos
        [HttpGet()]
        public ActionResult Index()
        {

            ViewBag.Areas = Areas;

            using (ISession sessionNh = HibernateUtil.GetSessionT5())
            using (var repoConfig = new Conexao<Master>(sessionNh))
            {
                var config = repoConfig.selectFirst().Result;
                ViewBag.UtilizaParalisaOS = config.ConfigParalisaOS;
                ViewBag.AutorizaBaixaOS = config.Autoriza_Baixa_Servico_Backoffice;
            }
            using (ISession session = HibernateUtil.GetSession())
            {
                // Obtém o ID do usuário logado
                string userId = User.Identity.GetUserId();

                using (var repoUser = new Conexao<IdentityUser>(session))
                {
                    // Busca o IdentityUser com as propriedades Bloqueia_Modelo_Visualizacao e Modelo_Unico_Permitido_ID
                    var usuario =  repoUser.selectFirst(p => p.Id == userId).Result;

                    // Passa as restrições para a View
                    ViewBag.BloqueiaModeloVisualizacao = usuario?.Bloqueia_Modelo_Visualizacao ?? false;
                    ViewBag.ModeloUnicoPermitidoId = usuario?.Modelo_Unico_Permitido_ID;
                }

                
            }
            return View(new Painel_ServicosListaFiltroModel
            {
                DataInicial = DateTime.Now.ToString("dd/MM/yyyy"),
                DataFinal = DateTime.Now.ToString("dd/MM/yyyy")
            });
        }

        [HttpGet()]
        public ActionResult LogAlteracaoOS()
        {
            return View();
        }

        [HttpGet()]
        public ActionResult LogOsNaoAtribuida()
        {
            return View();
        }

        [HttpGet()]
        public async Task<ActionResult> Criar_Ordem_Servico(Int64? Id_Atendimento, String Num_Obra,
                                                            Int64? Id_Cliente, Int64? Id_PlanoProduto, Int64? Id_Venda, Int64? Id_PlanoProdutoAt, Int64? Id_PlanoProdutoAnterior)
        {
            ViewBag.Areas = Areas;

            var Model = new Documento_ProducaoCadastroModel
            {
                Id_Atendimento = Id_Atendimento,
                Tipo = "C"
            };

            FillListasView(Model);

            Model.Origem_OS = Origem_Documento_Producao.Connect;

            if (Id_Atendimento.HasValue && Id_Atendimento.Value != 0)
            {
                using (var api = new AtendimentoApiController())
                using (var apiServ = new CamposObrigatoriosDocumentoProducaoApiController())
                {
                    var atendimento = await api.FindById(Id_Atendimento.Value);
                    Int64? _idCliente = atendimento.Objeto.Id_Cliente > 0 ? atendimento.Objeto?.Id_Cliente : Id_Cliente;
                    //var camposObrigatoriosRequisitados = await apiServ.ValidacaoCamposObrigatoriosOS(Id_Atendimento.Value);
                    var cliente = await new ClienteApiController().FindById(_idCliente.Value);

                    //if (!atendimento.Ok || !camposObrigatoriosRequisitados.Ok)
                    //{
                    //        (string Mensagem, string InnerException)ex = !atendimento.Ok ?
                    //                                                     (atendimento.Mensagem, atendimento?.InnerException) : 
                    //                                                     (camposObrigatoriosRequisitados.Mensagem, camposObrigatoriosRequisitados?.InnerException);
                    //    AddModelError(ex.Mensagem, ex.InnerException);
                    //}
                    if (!atendimento.Ok)
                    {
                        AddModelError(atendimento.Mensagem, atendimento.InnerException);
                    }

                    ViewBag.Planos_Produtos = new List<ObjBusca<Int64>>(cliente.Objeto.Planos.Select(p => new ObjBusca<Int64>
                    {
                        Label = p.Descricao,
                        Value = p.Id_Plano_Produto.Value
                    }));

                    using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                    using (ISession session = HibernateUtil.GetSession())
                    {
                        Model.Id_Cliente = _idCliente;
                        Model.Nome_Cliente = atendimento.Objeto.Nome_Cliente;
                        Model.Id_Plano_Produto = Id_PlanoProdutoAt == null ? atendimento.Objeto.Id_Plano_Produto : Id_PlanoProdutoAt;
                        //Model.Id_Plano_Produto = atendimento.Objeto.Id_Plano_Produto;

                        Model.CPF_CNPJ_Cliente = atendimento.Objeto.CPF_CNPJ_Cliente;
                        Model.Data_Cadastro_Cliente = atendimento.Objeto.Data_Cadastro_Cliente;
                        Model.Categoria_Cliente = atendimento.Objeto.Categoria_Cliente;
                        Model.Telefone_Fixo = atendimento.Objeto.Telefone_Fixo;
                        Model.Email_Principal = atendimento.Objeto.Email_Principal;
                        Model.Telefone_Celular = atendimento.Objeto.Telefone_Celular;
                        Model.Identificacao_Cliente = cliente.Objeto.Identificacao_Cliente;


                        var planoatend = cliente.Objeto.Planos.Find(p => p.Id == (Id_PlanoProdutoAt == null ? atendimento.Objeto.Id_Plano_Produto : Id_PlanoProdutoAt));

                        if (cliente.Objeto.Planos != null && cliente.Objeto.Planos.Count > 0 && planoatend != null)
                        {
                            Model.Logradouro = planoatend.Logradouro;
                            Model.CEP = planoatend.CEP;
                            Model.Complemento = planoatend.Complemento;
                            Model.Ponto_Referencia = planoatend.Ponto_Referencia;
                            Model.Cod_Area = planoatend.Cod_Area;
                            Model.Cod_Localidade = planoatend.Cod_Localidade;
                            Model.Localidade = planoatend.Localidade;
                            Model.Tipo_Imovel = planoatend.Id_Tipo_Imovel != null ? new Conexao<Tipo_Imovel>(session).selectFirst(p => p.Cod_Tipo_Imovel == cliente.Objeto.Planos[0].Id_Tipo_Imovel).Result : null;
                            Model.Cod_Escritorio = planoatend.Cod_Escritorio;

                            Model.Id_Tipo_Imovel = Model.Tipo_Imovel?.Cod_Tipo_Imovel;
                            Model.Id_Tipo_Imovel_Manut = Model.Tipo_Imovel?.Cod_Tipo_Imovel;
                            Model.Desc_Tipo_Imovel = Model.Tipo_Imovel?.Descricao;
                        }

                        //Preenche Tipo de Serviço com base no tipo de atendimento + codct da localidade
                        if (atendimento.Objeto.Id_Tipo_Atendimento.HasValue && !String.IsNullOrEmpty(Model.Localidade))
                        {
                            Tipo_Atendimento tipoAtendimento = await new Conexao<Tipo_Atendimento>(session).selectFirst(p => p.Id == atendimento.Objeto.Id_Tipo_Atendimento);

                            LocalidadeAniel localidade = await new Conexao<LocalidadeAniel>(sessionT5).selectFirst(p => p.Cod_Escritorio == Model.Cod_Escritorio &&
                                                                                                                        p.Cod_Area == Model.Cod_Area &&
                                                                                                                        p.Cod_Localidade == Model.Cod_Localidade);
                            if (localidade != null && !String.IsNullOrEmpty(tipoAtendimento?.Desc_Tipo_Servico))
                            {
                                Tipo_Servico_EquipeAniel tipo_serv = await new Conexao<Tipo_Servico_EquipeAniel>(sessionT5).selectFirst(p => p.Descricao == tipoAtendimento.Desc_Tipo_Servico &&
                                                                                                                                             p.CodCt == localidade.CodCt);
                                Model.Id_Tipo_servico = tipo_serv?.Cod_Tipo_Serv;
                                Model.Exige_Pagamento_Instalacao = Convert.ToString(tipo_serv?.Exige_Pagamento_Instalacao);

                            }
                        }
                    }

                    ViewBag.PlanosProdutos = new PlanoProdutoApiController().getByCliente(cliente.Objeto.Id).Result;
                    ViewBag.Id_Plano_Produto = Id_PlanoProdutoAt != null ? Id_PlanoProdutoAt : atendimento.Objeto.Id_Plano_Produto;
                    //ViewBag.Id_Plano_Produto = atendimento.Objeto.Id_Cliente_Plano_Produto;

                }

                ViewBag.CodCtVemAtendimento = new PlanoProdutoApiController().getByCliente(Model.Id_Cliente.Value).Result.FirstOrDefault()?.CodCt;
            }

            if ((Id_Cliente.HasValue && Id_PlanoProduto.HasValue) || (Id_Cliente.HasValue && Id_PlanoProdutoAt.HasValue))
            {

                var cliente_obj = new Cliente();
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                using (ISession sessionNhT5 = HibernateUtil.GetSessionT5())
                using (ISession session = HibernateUtil.GetSession())
                using (var repoCliente = new Conexao<Cliente>(session))
                using (var repoCategoriaCliente = new Conexao<Categoria_Cliente>(session))
                using (var repoTipoImovel = new Conexao<Tipo_Imovel>(session))
                using (var repoPlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
                {
                    var cliente = await new ClienteApiController().FindById(Id_Cliente.Value);

                    cliente_obj = await repoCliente.selectFirst(p => p.Id == Id_Cliente.Value);

                    ViewBag.Planos_Produtos = new List<ObjBusca<Int64>>(cliente.Objeto.Planos.Select(p => new ObjBusca<Int64>
                    {
                        Label = p.Descricao,
                        Value = p.Id_Plano_Produto.Value
                    }));

                    Model.Id_Cliente = Id_Cliente;
                    Model.Nome_Cliente = cliente.Objeto.NomeFantasia;
                    Model.CPF_CNPJ_Cliente = cliente.Objeto.CpfCnpj;
                    Model.Data_Cadastro_Cliente = Convert.ToDateTime((cliente_obj.Data_Criacao_Cliente ?? cliente_obj.DataHoraCriacao)).ToString("dd/MM/yyyy");
                    Model.Categoria_Cliente = (await repoCategoriaCliente.selectFirst(p => p.Id == cliente.Objeto.Id_Categoria_Cliente))?.Descricao;
                    Model.Telefone_Fixo = cliente.Objeto.TelefoneFixo;
                    Model.Email_Principal = cliente.Objeto.Email;
                    Model.Telefone_Celular = cliente.Objeto.Telefone_Celular;
                    Model.Identificacao_Cliente = cliente.Objeto.Identificacao_Cliente;

                    var plano = cliente.Objeto.Planos.Find(p => p.Id == (Id_PlanoProduto != null ? Id_PlanoProduto : Id_PlanoProdutoAt));

                    if (cliente.Objeto.Planos != null && cliente.Objeto.Planos.Count > 0)
                    {
                        Model.Logradouro = plano?.Logradouro;
                        Model.Uf = plano?.UF;
                        Model.Bairro = plano?.Bairro;
                        Model.Cidade = plano?.Cidade;
                        Model.CEP = plano?.CEP;
                        Model.Numero = plano?.Numero;
                        Model.Complemento = plano?.Complemento;
                        Model.Latitude = Convert.ToString(plano.Latitude);
                        Model.Longitude = Convert.ToString(plano.Longitude);
                        Model.Ponto_Referencia = plano?.Ponto_Referencia;
                        Model.Cod_Area = plano?.Cod_Area;
                        Model.Cod_Localidade = plano?.Cod_Localidade;
                        Model.Localidade = plano?.Localidade;
                        Model.Tipo_Imovel = plano?.Id_Tipo_Imovel != null ? repoTipoImovel.selectFirst(p => p.Cod_Tipo_Imovel == plano.Id_Tipo_Imovel).Result : null;
                        Model.Cod_Escritorio = plano?.Cod_Escritorio;
                        Model.Id_Tipo_Imovel = Model.Tipo_Imovel?.Cod_Tipo_Imovel;
                        Model.Id_Tipo_Imovel_Manut = Model.Tipo_Imovel?.Cod_Tipo_Imovel;
                        Model.Desc_Tipo_Imovel = Model.Tipo_Imovel?.Descricao;
                        Model.Desc_Tipo_Imovel = Model.Tipo_Imovel?.Descricao;
                        Model.Pais = Convert.ToString(plano.Id_Pais);
                    }

                    Cliente_PlanoProduto planoProduto = await repoPlanoProduto.selectFirst(p => p.Id == (Id_PlanoProduto != null ? Id_PlanoProduto : Id_PlanoProdutoAt));

                    if (planoProduto?.Plano != null)
                        Model.Id_Plano_Produto = planoProduto.Plano.Id;
                    else if (planoProduto?.Produto != null)
                        Model.Id_Plano_Produto = planoProduto.Produto.Id;
                    else if (planoProduto?.Combo != null)
                        Model.Id_Plano_Produto = planoProduto.Combo.Id;

                    if (planoProduto?.Plano != null)
                    {
                        if (planoProduto.Plano.DefinirTipo_SubTipoOS)
                        {
                            Model.DefinirTipo_SubTipoOS = planoProduto.Plano.DefinirTipo_SubTipoOS;
                            Model.Id_Tipo_ServicoOS = planoProduto.Plano.Id_Tipo_ServicoOS;
                            Model.Id_Subtipo_ServicoOS = planoProduto.Plano.Id_Subtipo_ServicoOS;
                        }
                    }

                    if (planoProduto != null)
                    {

                        var planosProdutos = await new PlanoProdutoApiController().getByCliente(cliente.Objeto.Id);

                        ViewBag.PlanosProdutos = new PlanoProdutoApiController().getByCliente(cliente.Objeto.Id).Result;
                        ViewBag.Id_Plano_Produto = Id_PlanoProdutoAt == null ? Id_PlanoProdutoAt : Model.Id_Plano_Produto;

                        ITipoServicoAnielRepository tipoServicoAnielRepository = new TipoServicoAnielRepository(sessionNhT5, sessionT5);
                        TipoServicoAnielBusiness tipoServicoAnielBusiness = new TipoServicoAnielBusiness(tipoServicoAnielRepository);
                        ISubtipoServicoAnielRepository subtipoServicoAnielRepository = new SubtipoServicoAnielRepository(sessionT5);
                        SubtipoServicoAnielBusiness subtipoServicoAnielBusiness = new SubtipoServicoAnielBusiness(subtipoServicoAnielRepository);

                        var planoDados = new PlanoProdutoApiController().getByCliente(Model.Id_Cliente.Value).Result.
                                                                         Where(p => p.Id_Cliente_PlanoProduto == planoProduto.Id)
                                                                         .FirstOrDefault();

                        if (planoDados != null)
                        {
                            ViewBag.CodCtVemAtendimento = planoDados.CodCt;

                            if (planoDados.IdTipoServico.HasValue && string.IsNullOrEmpty(planoDados.CodCt) == false)
                            {
                                short? tipoServicoPlano = tipoServicoAnielBusiness.GetTipoServicoPlano(planoDados.IdTipoServico.Value, planoDados.CodCt);
                                Model.Id_Tipo_servico = tipoServicoPlano;
                                if (!planoProduto.Plano.DefinirTipo_SubTipoOS)
                                {
                                    Model.Id_Tipo_ServicoOS = tipoServicoPlano;
                                }

                                Model.Tipo_Imovel = plano?.Id_Tipo_Imovel != null ? repoTipoImovel.selectFirst(p => p.Cod_Tipo_Imovel == cliente.Objeto.Planos[0].Id_Tipo_Imovel).Result : null;

                                if (tipoServicoPlano.HasValue && String.IsNullOrEmpty(planoDados.IdSubTipoServico) == false)
                                {
                                    var subtipoServicoPlano = subtipoServicoAnielBusiness.GetSubtipoServicoPlano(planoDados.IdSubTipoServico, Convert.ToInt16(planoDados.IdTipoServico), tipoServicoPlano.Value);
                                    Model.Id_SubTipo_servico = subtipoServicoPlano;
                                }
                            }
                        }
                    }
                }
            }
            Model.Id_Troca_Plano = Id_PlanoProdutoAnterior;
            Model.Id_Venda = Id_Venda;
            if (Id_PlanoProduto != null || Id_PlanoProdutoAt != null)
                Model.Id_Cliente_Plano_Produto = Id_PlanoProduto ?? Id_PlanoProdutoAt;

            //Preenchimento de dados da venda

            using (ISession sessionNh = HibernateUtil.GetSession())
            using (var repositoryConfig = new Conexao<Configuracoes_Gerais>(sessionNh))
            using (var repositoryCliente = new Conexao<Cliente>(sessionNh))
            using (var repositoryEndereco_Cliente = new Conexao<Endereco_Cliente>(sessionNh))
            using (var repositoryTipo_Imovel = new Conexao<Tipo_Imovel>(sessionNh))
            using (var repositoryCliente_PlanoProduto = new Conexao<Cliente_PlanoProduto>(sessionNh))
            {
                var config = await repositoryConfig.selectFirst();
                ViewBag.Utiliza_Cadastro_Endereco = config.Utiliza_Cadastro_Endereco;
                var cliente = await repositoryCliente.selectFirst(x => x.Id == Convert.ToInt64(Id_Cliente));
                if ((config.CriarOsSemPlanoProduto || config.Permite_OS_LEAD)
                    && cliente != null
                    && (Id_PlanoProdutoAt == null || Id_PlanoProdutoAt == 0))
                {
                    Int64? id_endereco = 0;
                    var cliente_plano_produto = new Cliente_PlanoProduto();
                    if (Id_PlanoProdutoAt > 0)
                    {
                        cliente_plano_produto = await repositoryCliente_PlanoProduto.selectFirst(x => x.Id == Id_PlanoProdutoAt);
                        id_endereco = cliente_plano_produto?.Endereco_Instalacao?.Id;
                    }
                    else if (cliente.Tipo_Cliente == Tipo_Cliente.Prospecto)
                    {
                        id_endereco = cliente?.Endereco_Prospecto?.Id;
                    }
                    else
                    {
                        id_endereco = cliente?.Endereco_Principal?.Id;
                    }
                    if (Id_Cliente.HasValue)
                    {
                        if (cliente != null)
                        {
                            if (id_endereco != null)
                            {
                                var endereco_principal = await repositoryEndereco_Cliente.selectFirst(x => x.Id == id_endereco);

                                Model.Logradouro = endereco_principal?.Logradouro;
                                Model.Numero = endereco_principal?.Numero;
                                Model.Uf = endereco_principal?.UF;
                                Model.Bairro = endereco_principal?.Bairro;
                                Model.Cidade = endereco_principal?.Cidade;
                                Model.CEP = endereco_principal?.Cep;
                                Model.Complemento = endereco_principal?.Complemento;
                                Model.Latitude = Convert.ToString(endereco_principal.Latitude);
                                Model.Longitude = Convert.ToString(endereco_principal.Longitude);
                                Model.Ponto_Referencia = endereco_principal?.Ponto_Referencia;
                                Model.Cod_Area = endereco_principal?.Cod_Area;
                                Model.Cod_Localidade = endereco_principal?.Cod_Localidade;
                                Model.Localidade = endereco_principal?.Localidade;
                                Model.Cod_Escritorio = endereco_principal?.Cod_Escritorio;

                                Model.Tipo_Imovel = endereco_principal.Tipo_Imovel != null ? await repositoryTipo_Imovel.selectFirst(p => p.Cod_Tipo_Imovel == endereco_principal.Tipo_Imovel.Id) : null;
                                Model.Id_Tipo_Imovel = Model.Tipo_Imovel?.Cod_Tipo_Imovel;
                                Model.Id_Tipo_Imovel_Manut = Model.Tipo_Imovel?.Cod_Tipo_Imovel;
                                Model.Desc_Tipo_Imovel = Model.Tipo_Imovel?.Descricao;

                                Model.Pais = Convert.ToString(endereco_principal?.Pais?.Id);

                                ViewBag.Id_Plano_Produto = null;
                            }
                        }
                    }
                }
            }

            return View(Model);
        }

        public override bool Validacoes<T>(T pModel)
        {
            List<CamposObrigatoriosDocumento_ProducaoModels> campos = new CamposObrigatoriosDocumentoProducaoApiController().GetCampos().Result;

            Dictionary<String, String> erros = new Dictionary<String, String>();

            Documento_ProducaoCadastroModel _model = pModel as Documento_ProducaoCadastroModel;

            foreach (var campo in campos)
            {
                if (campo.Obrigatorio)
                    switch (campo.Campo)
                    {
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.ADCIONAL_1:
                            if (String.IsNullOrEmpty(_model.Adcional_1) && !String.IsNullOrEmpty(campo.Titulo))
                                erros.Add("Adcional_1", String.Format(Resources.RsBase.REQUIRED, campo.Titulo));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.ADCIONAL_2:
                            if (String.IsNullOrEmpty(_model.Adcional_2) && !String.IsNullOrEmpty(campo.Titulo))
                                erros.Add("Adcional_2", String.Format(Resources.RsBase.REQUIRED, campo.Titulo));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.ADCIONAL_3:
                            if (String.IsNullOrEmpty(_model.Adcional_3) && !String.IsNullOrEmpty(campo.Titulo))
                                erros.Add("Adcional_3", String.Format(Resources.RsBase.REQUIRED, campo.Titulo));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.ADCIONAL_4:
                            if (String.IsNullOrEmpty(_model.Adcional_4) && !String.IsNullOrEmpty(campo.Titulo))
                                erros.Add("Adcional_4", String.Format(Resources.RsBase.REQUIRED, campo.Titulo));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.ADCIONAL_5:
                            if (String.IsNullOrEmpty(_model.Adcional_5) && !String.IsNullOrEmpty(campo.Titulo))
                                erros.Add("Adcional_5", String.Format(Resources.RsBase.REQUIRED, campo.Titulo));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.ADCIONAL_6:
                            if (String.IsNullOrEmpty(_model.Adcional_6) && !String.IsNullOrEmpty(campo.Titulo))
                                erros.Add("Adcional_6", String.Format(Resources.RsBase.REQUIRED, campo.Titulo));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.ADCIONAL_7:
                            if (String.IsNullOrEmpty(_model.Adcional_7) && !String.IsNullOrEmpty(campo.Titulo))
                                erros.Add("Adcional_7", String.Format(Resources.RsBase.REQUIRED, campo.Titulo));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.ADCIONAL_8:
                            if (String.IsNullOrEmpty(_model.Adcional_8) && !String.IsNullOrEmpty(campo.Titulo))
                                erros.Add("Adcional_8", String.Format(Resources.RsBase.REQUIRED, campo.Titulo));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.ADCIONAL_9:
                            if (String.IsNullOrEmpty(_model.Adcional_9) && !String.IsNullOrEmpty(campo.Titulo))
                                erros.Add("Adcional_9", String.Format(Resources.RsBase.REQUIRED, campo.Titulo));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.ADCIONAL_10:
                            if (String.IsNullOrEmpty(_model.Adcional_10) && !String.IsNullOrEmpty(campo.Titulo))
                                erros.Add("Adcional_10", String.Format(Resources.RsBase.REQUIRED, campo.Titulo));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.SEGMENTO:
                            if (String.IsNullOrEmpty(_model.Segmento))
                                erros.Add("Segmento", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.SEGMENTO));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.SUBTIPO_SERVICO:
                            if (String.IsNullOrEmpty(_model.Id_SubTipo_servico))
                                erros.Add("Id_SubTipo_servico", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.SUBTIPO_SERVICO));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.DEPARTAMENTO:
                            if (_model.IdDepartamento.HasValue == false)
                                erros.Add("IdDepartamento", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.DEPARTAMENTO));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.PRIORIDADE:
                            if (_model.Prioridade_Cliente.HasValue == false)
                                erros.Add("Prioridade_Cliente", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.PRIORIDADE));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.POP:
                            if (_model.Id_Pop.HasValue == false && _model.Tipo == "M")
                                erros.Add("Id_Pop", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.POP));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.OLT:
                            if (_model.Id_Olt.HasValue == false && _model.Tipo == "M")
                                erros.Add("Id_Olt", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.OLT));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.ELEMENTO_REDE:
                            if (_model.Id_Elemento_Rede.HasValue == false && _model.Tipo == "M")
                                erros.Add("Id_Elemento_Rede", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.ELEMENTO_REDE));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.ENDERECO_MANUTENCAO:
                            if (string.IsNullOrEmpty(_model.Logradouro) && _model.Tipo == "M")
                                erros.Add("Logradouro", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.LOGRADOURO));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.CEP_MANUTENCAO:
                            if (string.IsNullOrEmpty(_model.CEP) && _model.Tipo == "M")
                                erros.Add("CEP", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.CEP));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.NUMERO_MANUTENCAO:
                            if (string.IsNullOrEmpty(_model.CEP) && _model.Tipo == "M")
                                erros.Add("Numero", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.ELEMENTO_REDE));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.UF_MANUTENCAO:
                            if (string.IsNullOrEmpty(_model.Uf) && _model.Tipo == "M")
                                erros.Add("Uf", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.UF));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.PAIS:
                            if (string.IsNullOrEmpty(_model.Pais) && _model.Tipo == "M")
                                erros.Add("Pais", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.PAIS));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.TIPO_IMOVEL:
                            if (_model.Id_Tipo_Imovel.HasValue == false)
                                erros.Add("Id_Tipo_Imovel", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.TIPO_IMOVEL));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.COMPLEMENTO_MANUTENCAO:
                            if (string.IsNullOrEmpty(_model.Complemento) && _model.Tipo == "M")
                                erros.Add("Complemento", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.COMPLEMENTO));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.PONTO_REFERENCIA_MANUTENCAO:
                            if (string.IsNullOrEmpty(_model.Ponto_Referencia) && _model.Tipo == "M")
                                erros.Add("Ponto_Referencia", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.PONTOREFERENCIA));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.LATITUDE:
                            if (string.IsNullOrEmpty(_model.Latitude))
                                erros.Add("Uf", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.LATITUDE));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.LONGITUDE:
                            if (string.IsNullOrEmpty(_model.Longitude))
                                erros.Add("Longitude", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.LONGITUDE));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.GRUPO_AREA:
                            if (string.IsNullOrEmpty(_model.Cod_Escritorio))
                                erros.Add("Cod_Escritorio", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.GRUPO_AREA));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.AREA:
                            if (string.IsNullOrEmpty(_model.Cod_Area))
                                erros.Add("Cod_Area", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.AREA));
                            break;
                        case Model.enumerado.CamposObrigatoriosDocumentoProducaoEnum.LOCALIDADE:
                            if (_model.Cod_Localidade.HasValue == false && _model.Tipo == "M")
                                erros.Add("Cod_Localidade", String.Format(Resources.RsBase.REQUIRED, Resources.RsCadastros.LOCALIDADE));
                            break;


                    }
            }

            if (erros.Count > 0)
            {
                AddModelError(erros);
            }

            return erros.Count == 0;
        }

        [HttpPost()]
        public async Task<ActionResult> Criar_Ordem_Servico(Documento_ProducaoCadastroModel pModel)
        {
            //SetTokenByUser();
            try
            {
                if (ModelState.IsValid && Validacoes<Documento_ProducaoCadastroModel>(pModel))
                {
                    PostApiRetorno<Documento_ProducaoChaveModel> retorno = await new Documento_ProducaoApiController().Gravar(new ParamWebApi<Documento_ProducaoCadastroModel>
                    {
                        Usuario = User.Identity.GetUserId(),
                        //Token = GetTokenByUser(User.Identity.Name),
                        Objeto = pModel
                    });

                    if (!retorno.Ok)
                    {
                        AddModelError(retorno.Mensagem, retorno.InnerException);
                    }
                    else
                    {
                        retorno.IdObjeto.Num_Obra = retorno.IdObjeto.Num_Obra.Split('/')[0];
                        using (ISession session = HibernateUtil.GetSession())
                        using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                        using (var repositoryTipo_Servico = new Conexao<Tipo_Servico_EquipeAniel>(sessionT5))
                        using (var repositoryNatureza = new Conexao<Natureza_ProducaoAniel>(sessionT5))
                        {
                            Configuracoes_Gerais config = await new Conexao<Configuracoes_Gerais>(session).selectFirst();

                            var tipo_serv = await repositoryTipo_Servico.selectFirst(p => p.Cod_Tipo_Serv == pModel.Id_Tipo_servico);
                            if (tipo_serv != null)
                            {
                                Natureza_ProducaoAniel natureza = null;

                                if (tipo_serv.Cod_Natureza.HasValue)
                                {
                                    natureza = await repositoryNatureza.selectFirst(p => p.Id == tipo_serv.Cod_Natureza.Value);
                                }

                                if (!String.IsNullOrEmpty(Request.Form["agendar"]) || (config.Exige_Agendamento_Servico && natureza?.Nao_Obriga_Agendamento == false))
                                {
                                    return RedirectToAction("Index", "Agendamento", new { CodCt = retorno.IdObjeto.CodCt, Projeto = retorno.IdObjeto.Projeto, Num_Obra = retorno.IdObjeto.Num_Obra, Num_Doc = retorno.IdObjeto.Num_Doc, Pagina = "Criacao_OS" });

                                }
                                else
                                {
                                    return RedirectToAction("Index", "Acompanhamento_Servico", retorno.IdObjeto);

                                }
                            }
                            else
                            {
                                AddModelError(retorno.Mensagem, retorno.InnerException = "Preencha o campo Tipo de Serviço");
                            }


                        }
                    }
                }
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                {
                    IDocumentoProducaoAnielRepository repo = new DocumentoProducaoAnielRepository(sessionT5, sessionT5);
                    var business = new DocumentoProducaoAnielBusiness(repo);
                    await business.CapturaPlanoSemOs(pModel?.Id_Cliente_Plano_Produto);
                }

                FillListasView(pModel);
            }
            catch (Exception ex)
            {
                FillListasView(pModel);

                AddModelError(Resources.RsBase.ERRO_GRAVARDADOSNOSERVIDOR, ex.Message, ex.InnerException?.Message);
            }

            return View(pModel);


        }

        protected override void FillListasView<T>(T pModel)
        {
            ViewBag.Planos_Produtos = new List<ObjBusca<Int64>>();

            List<CamposObrigatoriosDocumento_ProducaoModels> campos = new CamposObrigatoriosDocumentoProducaoApiController().GetCampos().Result;
            var campobrigatorios = new CamposObrigatoriosDocumentoProducaoApiController().GetCampos().Result.ToList<ICamposObrigatoriosModel>();
            ViewBag.CamposObrigatorios = CamposObrigatoriosController.dictCamposObrigatorios(campobrigatorios);
            ViewBag.CamposBloqueados = CamposObrigatoriosController.dictCamposBloqueados(campobrigatorios);
            ViewBag.CamposTitulo = CamposObrigatoriosController.dictCamposTitulo(campos);

            using (ISession session = HibernateUtil.GetSession())
            {
                var conf_gerais = new Conexao<Configuracoes_Gerais>(session).selectFirst().Result;
                ViewBag.RealizaAgendamento = conf_gerais.Exige_Agendamento_Servico;
                ViewBag.RealizaRoteirizacao = conf_gerais.Empresa_Roteirizacao_Servico;
                ViewBag.Utiliza_Cadastro_Endereco = conf_gerais.Utiliza_Cadastro_Endereco;
                ViewBag.CriarOsSemPlanoProduto = conf_gerais.CriarOsSemPlanoProduto;
                ViewBag.Permite_OS_LEAD = conf_gerais.Permite_OS_LEAD;
                ViewBag.Protocolo_OS_Manual = conf_gerais.Formato_Protocolo_OS == Model.enumerado.Formato_Protocolo_OS.Manual;
            }
            ViewBag.GruposArea = new Grupo_AreaAnielApiController().GetList().Result.Objeto;
            ViewBag.Areas = new List<String>();
            ViewBag.Localidades = new List<String>();
            ViewBag.Area311 = TemAcesso("311");
        }

    }
}