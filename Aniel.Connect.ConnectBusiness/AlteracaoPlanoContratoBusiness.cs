using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.ConnectBusiness.T5.Consulta_de_Viabilidade;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.ModelLog.nhibernate;
using Aniel.Connect.Repository.AlteracaoPlanoContrato;
using Aniel.Connect.Repository.Bairro;
using Aniel.Connect.Repository.Banco;
using Aniel.Connect.Repository.Campanha;
using Aniel.Connect.Repository.CategoriaCliente;
using Aniel.Connect.Repository.Cidade;
using Aniel.Connect.Repository.Cliente;
using Aniel.Connect.Repository.ClienteContrato;
using Aniel.Connect.Repository.ClientePlanoProduto;
using Aniel.Connect.Repository.Combo;
using Aniel.Connect.Repository.CondominioPredio;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.ConsultaCredito;
using Aniel.Connect.Repository.ConsultaViabilidade;
using Aniel.Connect.Repository.CriteriosParamCredito;
using Aniel.Connect.Repository.Departamento;
using Aniel.Connect.Repository.EnderecoCliente;
using Aniel.Connect.Repository.EnderecoLogradouro;
using Aniel.Connect.Repository.EtapaVenda;
using Aniel.Connect.Repository.FormaPagamento;
using Aniel.Connect.Repository.Funcionario;
using Aniel.Connect.Repository.MeioCaptacao;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.ModeloContrato;
using Aniel.Connect.Repository.Pais;
using Aniel.Connect.Repository.ParametrosConsultaCredito;
using Aniel.Connect.Repository.PendenciaEndereco;
using Aniel.Connect.Repository.PendenciaEnderecoAniel;
using Aniel.Connect.Repository.PendenciaFinanceira;
using Aniel.Connect.Repository.Plano;
using Aniel.Connect.Repository.Pool_Ip;
using Aniel.Connect.Repository.Produto;
using Aniel.Connect.Repository.Repository.ClienteContrato;
using Aniel.Connect.Repository.Repository.ComboPlanoProduto;
using Aniel.Connect.Repository.Repository.Pool_Ip;
using Aniel.Connect.Repository.T5.Checklist;
using Aniel.Connect.Repository.T5.ElementoRede;
using Aniel.Connect.Repository.T5.ElementoRedePorta;
using Aniel.Connect.Repository.TipoImovel;
using Aniel.Connect.Repository.Uf;
using Aniel.Connect.Repository.Usuario;
using Aniel.Connect.Repository.Vencimento;
using Aniel.Connect.Repository.Venda;
using Aniel.Connect.Repository.Workflow;
using NHibernate;
using NHibernate.Util;
using Sinapse.DAO;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Aniel.Connect.ConnectBusiness
{
    public class AlteracaoPlanoContratoBusiness
    {
        private IAlteracaoPlanoContratoRepository _repository;
        public AlteracaoPlanoContratoBusiness(IAlteracaoPlanoContratoRepository repo)
        {
            this._repository = repo;
        }
        /// <summary>
        /// Método Business do GetAll responsável por métodos como Ordenação, Filtro e Consulta.
        /// </summary>
        /// <returns></returns>
        public async Task<AlteracaoPlanoContratoListResponseModel> GetAll(AlteracaoPlanoContratoDataTableRequestModel request)
        {
            #region [GetAll]

            var parametros = new Dictionary<string, object>();
            List<string> condicoes = new List<string>();
            var parametrosOrdenacao = new Dictionary<string, string>();
            var ordenacao = request.order.ToList();
            try
            {

                if (!string.IsNullOrEmpty(request.search.value))
                {
                    condicoes.Add(" ((B.CPF LIKE @SEARCHCPF) OR UPPER(B.NOMEFANTASIA) LIKE UPPER(@SEARCH)) ");
                    parametros.Add("@SEARCH", $"%{request.search.value.ToUpper()}%");
                    parametros.Add("@SEARCHCPF", StringService.Truncate(String.Format("%{0}%", request.search.value.ToUpper().Replace(".", String.Empty).Replace("-", String.Empty).Replace("/", String.Empty)), 18));
                }
                if (request.IdContrato.HasValue)
                {
                    condicoes.Add(" G.ID_CLIENTE_CONTRATO = @ID_CONTRATO ");
                    parametros.Add("@ID_CONTRATO", request.IdContrato.Value);
                }
                if (request.IdVendedor.HasValue)
                {
                    condicoes.Add(" A.ID_VENDEDOR = @ID_VENDEDOR ");
                    parametros.Add("@ID_VENDEDOR", request.IdVendedor.Value);
                }
                if (request.IdSupervisor.HasValue)
                {
                    condicoes.Add(" A.ID_SUPERVISOR = @ID_SUPERV ");
                    parametros.Add("@ID_SUPERV", request.IdSupervisor.Value);
                }
                if (request.StatusVenda.HasValue)
                {
                    condicoes.Add(" A.STATUS = @STATUS ");
                    parametros.Add("@STATUS", Convert.ToInt32(request.StatusVenda.Value));
                }

                if (request.TermoAdesaoAssinado.HasValue)
                {
                    condicoes.Add(" A.TERMO_ADESAO_ASSINADO = @ASSINADO ");
                    parametros.Add("@ASSINADO", Convert.ToInt16(request.TermoAdesaoAssinado.Value));
                }

                if (!string.IsNullOrEmpty(request.DataInicial) && !string.IsNullOrEmpty(request.DataFinal))
                {
                    string dateInicial = Convert.ToDateTime(request.DataInicial).Date.ToString("dd/MM/yyyy 00:00:00");
                    string dateFim = Convert.ToDateTime(request.DataFinal).Date.ToString("dd/MM/yyyy 23:59:59");

                    condicoes.Add(" A.DATAHORACRIACAO BETWEEN @DATAINICIAL AND @DATAFIM ");
                    parametros.Add("@DATAINICIAL", dateInicial);
                    parametros.Add("@DATAFIM", dateFim);
                }


                //Condição Obrigatória: 
                condicoes.Add(" A.MUDANCA_PLANO = 1 ");
                //condicoes.Add($"B.STATUS = {Convert.ToInt32(Status_PlanoProduto.Cancelado_Mudanca_Plano)}");
                if (ordenacao != null && ordenacao.Any())
                {
                    int sortColumnIndex = request.order[0].column;
                    string coluna = request.columns[sortColumnIndex].data;
                    string sortDirection = request.order[0].dir;
                    switch (coluna)
                    {
                        //case "id_Vendedor":
                        //    {
                        //        parametrosOrdenacao.Add("A.id_Vendedor", sortDirection);
                        //        break;
                        //    }
                        //case "data_Assinatura":
                        //    {
                        //        parametrosOrdenacao.Add("A.Data", sortDirection);
                        //        break;
                        //    }
                        //case "data_HoraCriacao":
                        //    {
                        //        parametrosOrdenacao.Add("A.Data", sortDirection);
                        //        break;
                        //    }
                        //case "data_HoraAprovacao":
                        //    {
                        //        parametrosOrdenacao.Add("A.Data", sortDirection);
                        //        break;
                        //    }
                        //case "data_HoraCancelamento":
                        //    {
                        //        parametrosOrdenacao.Add("A.Data", sortDirection);
                        //        break;
                        //    }
                    }
                }

                return new AlteracaoPlanoContratoListResponseModel
                {
                    RecordsFiltered = await this._repository.Count(parametros, condicoes),
                    Records = await this._repository.GetAll(parametros, condicoes, parametrosOrdenacao, request.length, request.start)
                };
            }
            catch (Exception ex) { throw ex; }
            #endregion
        }
        public async Task<bool> GetValidacaoCriacaoOS(long planoAnterior, long plano)
        {
            #region [GetValidacaoOS]
            bool materiaisCorrespondentes = false;
            bool criaOS = true;
            using (var session = HibernateUtil.GetSession())
            using (var repo = new Conexao<Plano>(session))
            {
                var recordPlano = await repo.selectFirst(p => p.Id == plano);
                if (recordPlano != null && recordPlano.OS_Mudanca_Plano)
                {
                    var listagemMateriaisPlanoAntigo = await this._repository.GetMateriaisPlanoOs(planoAnterior);
                    var listagemMateriais = await this._repository.GetMateriaisPlanoOs(plano);
                    if (listagemMateriais.Count() == listagemMateriaisPlanoAntigo.Count())
                    {
                        materiaisCorrespondentes = listagemMateriais.All(novo =>
                                                   listagemMateriaisPlanoAntigo.Any(antigo =>
                                                   antigo.Codigo == novo.Codigo));
                    }
                }
            }
            if (materiaisCorrespondentes)
            {
                criaOS = false;
            }

            return criaOS;
            #endregion
        }
        public async Task<long> Gravar(AlteracaoPlanoContratoCadastroModel view, string user, string username)
        {
            #region [Gravar]
            Venda obj = await this._repository.BindVenda(view, user);
            var modeloAnterior = (Venda)obj.Clone();

            using (ISession session = HibernateUtil.GetSession())
            {
                IClienteRepository repositoryCliente = new ClienteRepository(session);
                IVendaRepository repositoryVenda = new VendaRepository(session);
                IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(session);
                IEtapaVendaRepository repoEtapaVenda = new EtapaVendaRepository(session);
                IFuncionarioRepository repoFuncionario = new FuncionarioRepository(session);
                IDepartamentoRepository repoDepto = new DepartamentoRepository(session);
                IUsuarioRepository repoUsuario = new UsuarioRepository(session);
                IClienteRepository repoCliente = new ClienteRepository(session);
                IEnderecoClienteRepository repoEnderecoCliente = new EnderecoClienteRepository(session);
                ICategoriaClienteRepository repoCategoriaCliente = new CategoriaClienteRepository(session);
                IMeioCaptacaoRepository repoMeioCaptacao = new MeioCaptacaoRepository(session);
                IPaisRepository repoPais = new PaisRepository(session);
                IUfRepository repoUf = new UfRepository(session);
                ICidadeRepository repoCidade = new CidadeRepository(session);
                IBairroRepository repoBairro = new BairroRepository(session);
                ITipoImovelRepository repoTipoImovel = new TipoImovelRepository(session);
                IEnderecoLogradouroRepository repoEnderecoLogradouro = new EnderecoLogradouroRepository(session);
                ICondominioPredioRepository repoCondominioPredio = new CondominioPredioRepository(session);
                IBancoRepository repoBanco = new BancoRepository(session);
                IVencimentoRepository repoVencimento = new VencimentoRepository(session);
                IFormaPagamentoRepository repoFormaPagamento = new FormaPagamentoRepository(session);
                IComboRepository repoCombo = new ComboRepository(session);
                ICampanhaRepository repoCampanha = new CampanhaRepository(session);
                IPlanoRepository repoPlano = new PlanoRepository(session);
                IProdutoRepository repoProduto = new ProdutoRepository(session);
                IPool_IpRepository repoPoolIP = new Pool_IpRepository(session);
                IModeloContratoRepository repoModeloContrato = new ModeloContratoRepository(session);
                IClienteContratoRepository repoClienteContrato = new ClienteContratoRepository(session);
                IClientePlanoProdutoRepository repoClientePlanoProduto = new ClientePlanoProdutoRepository(session);
                IComboPlanoProdutoRepository comboPlanoProdutoRepository = new ComboPlanoProdutoRepository(session);
                IWorkflowRepository _workflowRepositor = new WorkflowRepository(session);

                var etapaVendaBusiness = new EtapaVendaBusiness(repoEtapaVenda, repoConfig);
                ClienteBusiness businessCliente = new ClienteBusiness(repositoryCliente);

                VendaBusiness businessVenda = new VendaBusiness(repositoryVenda, repoFuncionario, repoDepto, repoConfig, repoUsuario, repoCliente, repoEnderecoCliente, repoCategoriaCliente,
                                                                repoMeioCaptacao, repoPais, repoUf, repoCidade, repoBairro, repoTipoImovel, repoEnderecoLogradouro, repoCondominioPredio,
                                                                repoBanco, repoVencimento, repoFormaPagamento, repoCombo, repoCampanha, repoPlano, repoProduto, repoPoolIP,
                                                                repoModeloContrato, repoClienteContrato, repoClientePlanoProduto, comboPlanoProdutoRepository,
                                                                _workflowRepositor);
                if (obj.Id != 0)
                {
                    await this._repository.Update(obj);
                }
                else
                {
                    await this._repository.Save(obj);
                }

                Model.nhibernate.Cliente client = businessCliente.GetById(obj.Id_Cliente.Value);
                Cliente_PlanoProduto modelo = (Cliente_PlanoProduto)businessVenda.GetPlanosProdutosVenda(obj.Id).FirstOrDefault()?.Clone();
                var plano_descontinuado_view = repoClientePlanoProduto.GetById(Convert.ToInt64(view.IdPlanoAnterior));
                if (plano_descontinuado_view != null)
                {
                    plano_descontinuado_view.Id_TrocaPlano = view.IdPlanoAnterior;
                    view.PlanoProduto = GetPlanoProdutoListModel(client, plano_descontinuado_view);
                    view.PlanoProduto.Id_PlanoMudanca = plano_descontinuado_view.Id_TrocaPlano;
                    view.PlanoProduto.Id_Plano_Produto = view.IdPlanoProduto;
                }
                Cliente_PlanoProduto planoAlteracao = businessVenda.BindPlanoProduto(
                    client,
                    obj,
                    view.PlanoProduto);
                await this._repository.BindEtapasVenda(view, obj, user, planoAlteracao);
                await this._repository.Update(obj);





                if (modelo != null) { planoAlteracao.Id = modelo.Id; }

                var isPagPlanProdCartao = planoAlteracao.Forma_Pagamento_Cliente != null && planoAlteracao.Forma_Pagamento_Cliente.Forma_Pagamento.Tipo_Forma_Pagamento == Tipo_Forma_Pagamento.Cartao_Credito &&
                                               planoAlteracao.Forma_Pagamento_Cliente.Forma_Pagamento.Gerar_Boleto_Automatico;

                if (isPagPlanProdCartao)
                {
                    if (planoAlteracao.Forma_Pagamento_Cliente.StatusValidoCartao == null)
                        planoAlteracao.Forma_Pagamento_Cliente.StatusValidoCartao = StatusValidacaoCartao.Aguardando_Validacao;

                    etapaVendaBusiness.AdcEtapaCartao(user, obj);
                }

                if (planoAlteracao.Plano?.Tipo_Tecnologia == Tipo_Tecnologia.Internet)
                {

                    if (!String.IsNullOrEmpty(view.PlanoProduto.Login_Radius) && !String.IsNullOrEmpty(view.PlanoProduto.Senha_Radius))
                    {
                        planoAlteracao.Login_Radius = view.PlanoProduto.Login_Radius.ToLowerInvariant();
                        planoAlteracao.Senha_Radius = view.PlanoProduto.Senha_Radius.ToLowerInvariant();
                    }
                }

                businessCliente.Update(client);
                IEnumerable<LogAlteracaoPlanoContrato> logs = new List<LogAlteracaoPlanoContrato>();
                if (modelo != null)
                {
                    if (obj?.Vendedor?.Id != view.Id_Vendedor)
                        obj.Vendedor = view.Id_Vendedor.HasValue ? repoFuncionario.GetById(view.Id_Vendedor.Value) : null;

                    if (obj?.Departamento?.Id != view.Id_Departamento)
                        obj.Departamento = view.Id_Departamento.HasValue ? repoDepto.GetById(view.Id_Departamento.Value) : obj.Vendedor?.Departamento;

                    if (obj?.Supervisor?.Id != view.Id_Supervisor)
                        obj.Supervisor = view.Id_Supervisor.HasValue ? repoFuncionario.GetById(view.Id_Supervisor.Value) : null;

                    logs = obj.GeraLogAlteracao(modeloAnterior, user, username, Sinapse.ModelLog.TipoOperLog.Alteracao);
                    await this._repository.SavePlanoProduto(planoAlteracao);
                    await this._repository.SaveLog(logs.ToList());
                }
                else
                {
                    logs = obj.GeraLogAlteracao(modeloAnterior, user, username, Sinapse.ModelLog.TipoOperLog.Insercao);
                    await this._repository.SavePlanoProduto(planoAlteracao);
                    await this._repository.SaveLog(logs.ToList());
                }

                await this._repository.Update(obj);

                return obj.Id;
            }
            #endregion
        }

        public async Task<List<IObjComboResult<long>>> GetComboContrato(RequestObjComboAvo<long?, long?, long?> request)
        {
            #region [GetComboContrato]

            var parametros = new Dictionary<string, object>();
            List<string> condicoes = new List<string>();
            var sql = new StringBuilder();
            var sqlFrom = new StringBuilder();
            var sqlWhere = new StringBuilder();


            #region [Filtros]

            if (request.soativos)
            {
                condicoes.Add("C.STATUS NOT IN (5,7) AND A.STATUS = 2");
            }
            if (request.id.HasValue)
            {
                condicoes.Add(" A.ID = @ID_CONTRATO");
                parametros.Add("@ID_CONTRATO", request.id);
            }
            if (request.idpai.HasValue)
            {
                condicoes.Add(" C.ID = @ID_PLANO");
                parametros.Add("@ID_PLANO", request.idpai);
            }
            if (request.idavo.HasValue)
            {
                condicoes.Add(" B.ID = @ID_CLIENTE");
                parametros.Add("@ID_CLIENTE", request.idavo);
            }
            #endregion
            return (await this._repository.GetComboContrato(condicoes, parametros, request.page)).ToList();
            #endregion

        }
        public async Task<long?> FindById(long? IdCliente, long? IdClienteContrato, long? IdClientePlano, long? IdVenda)
        {
            #region [FindById]            
            var condicoes = new List<string>();
            var parametros = new Dictionary<string, object>();
            if (IdCliente.HasValue)
            {
                condicoes.Add("A.ID_CLIENTE = @CLIENTE");
                parametros.Add("@CLIENTE", IdCliente);
            }
            if (IdClientePlano.HasValue)
            {
                condicoes.Add("E.ID = @PLANO");
                parametros.Add("@PLANO", IdClientePlano);
            }
            if (IdClienteContrato.HasValue)
            {
                condicoes.Add("A.ID_CLIENTE_CONTRATO = @CONTRATO");
                parametros.Add("@CONTRATO", IdClienteContrato);
            }

            return await this._repository.GetVenda(IdVenda, condicoes, parametros);
            #endregion
        }
        public async Task<long> GetPlanoByContrato(long cliente, long contrato)
        {
            return await this._repository.GetPlanoByContrato(cliente, contrato);
        }

        public async Task<GetApiRetorno<VendaDadosGeraisViewModel>> DuplicarVendaByContrato(long cliente, long contrato, string UserId)
        {
            #region [DuplicarVendaByContrato]
            var retorno = new GetApiRetorno<VendaDadosGeraisViewModel>();
            using (var session = HibernateUtil.GetSession())

            using (var repoCliente = new Conexao<Cliente>(session))
            using (var repoContrato = new Conexao<ClienteContrato>(session))
            {
                var bancocliente = await repoCliente.selectFirst(p => p.Id == cliente);
                var bancocontrato = await repoContrato.selectFirst(p => p.Id == contrato);

                retorno.Objeto = await GetViewObj(bancocliente, bancocontrato, null);
            }

            return retorno;
            #endregion
        }
        public async Task<GetApiRetorno<VendaDadosGeraisViewModel>> DuplicarVendaByContratoPlano(long cliente, long contrato, long plano, string UserId, long? vendaId = null)
        {
            #region [DuplicarVendaByContratoPlano]
            var retorno = new GetApiRetorno<VendaDadosGeraisViewModel>();
            using (var session = HibernateUtil.GetSession())

            using (var repoCliente = new Conexao<Cliente>(session))
            using (var repoContrato = new Conexao<ClienteContrato>(session))
            using (var repoPlano = new Conexao<Plano>(session))
            using (var repoVenda = new Conexao<Venda>(session))
            {
                var bancocliente = await repoCliente.selectFirst(p => p.Id == cliente);
                var bancocontrato = await repoContrato.selectFirst(p => p.Id == contrato);
                var bancoplano = await repoPlano.selectFirst(p => p.Id == plano);
                var bancovenda = await repoVenda.selectFirst(x => x.Id == vendaId);

                retorno.Objeto = await GetViewObj(bancocliente, bancocontrato, bancoplano, bancovenda);
            }

            return retorno;
            #endregion
        }
        public async Task<GetApiRetorno<VendaDadosGeraisViewModel>> GetVendaByPlano(long cliente, long plano)
        {
            #region [GetVendaByPlano]
            var retorno = new GetApiRetorno<VendaDadosGeraisViewModel>();
            long? IdClientePlanoProduto = await this._repository.GetClientePlanoProdutoByPlano(cliente, plano);

            if (!IdClientePlanoProduto.HasValue)
            {
                await this._repository.GerarVendaByClientePlano(cliente, plano);
            }
            else
            {
                retorno = await GetVendaByClientePlanoProduto(IdClientePlanoProduto.Value);
            }
            return retorno;
            #endregion
        }
        public async Task<GetApiRetorno<VendaDadosGeraisViewModel>> GetVendaByClientePlanoProduto(long idClientePlanoProduto)
        {
            #region [GetVendaByClientePlanoProduto]

            var retorno = new GetApiRetorno<VendaDadosGeraisViewModel>();
            using (ISession session = HibernateUtil.GetSession())
            using (var repoClientePlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
            using (var repoVenda = new Conexao<Venda>(session))
            {
                var bancoClientePlanoProduto = await repoClientePlanoProduto.selectFirst(p => p.Id == idClientePlanoProduto);
                var bancoVenda = await repoVenda.selectFirst(p => p.Id == bancoClientePlanoProduto.Id_Venda);
                if (bancoVenda != null)
                {
                    retorno.Objeto = await GetViewObjByVenda(bancoVenda, bancoClientePlanoProduto.Cliente, null, null, idClientePlanoProduto, session);
                }
            }
            return retorno;
            #endregion
        }
        public async Task<VendaDadosGeraisViewModel> GetViewObj(Cliente cliente, ClienteContrato contrato, Plano plano, Venda venda = null)
        {
            #region [GetViewObj]

            var retorno = new VendaDadosGeraisViewModel();
            using (var session = HibernateUtil.GetSession())
            using (Conexao<Configuracoes_Gerais> repoConfiguracoes_Gerais = new Conexao<Configuracoes_Gerais>(session))
            using (Conexao<IdentityUser> repoUsuario = new Conexao<IdentityUser>(session))
            {
                var config = await repoConfiguracoes_Gerais.selectFirst();
                IEnumerable<string> nome = cliente.NomeFantasia != null ? cliente.NomeFantasia.Split(' ') : null;
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
                retorno.Tipo_Cliente = cliente.Tipo_Cliente.Value;

                retorno.Id_Cliente = cliente.Id;
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
                retorno.Qual_Meio = cliente?.Outro_Meio;
                retorno.TipoPessoa = cliente?.Tipo_Pessoa;

                retorno.Email = cliente?.Email_Principal;
                retorno.TelefoneFixo = cliente?.Telefone_Fixo;
                retorno.TelefoneCelular = cliente?.Telefone_Celular;
                retorno.TelefoneRecado = cliente?.Telefone_Recado;
                retorno.NomeRecado = cliente?.Pessoa_Tel_Recado;
                //retorno.Desc_Banco = cliente?.Banco?.Descricao;

                retorno.Quem_Indicou = cliente?.Quem_Indicou?.NomeFantasia;

                retorno.Id_Categoria_Cliente = cliente?.Categoria_Cliente?.Id;
                retorno.DescCategoriaCliente = cliente?.Categoria_Cliente?.Descricao;
                retorno.Id_Meio_Captacao = cliente?.Qual_Meio?.Id;
                retorno.Meio_Captacao = cliente?.Qual_Meio?.Descricao;
                retorno.Id_Quem_Indicou = cliente?.Quem_Indicou?.Id;
                retorno.Quem_Indicou = Convert.ToString(cliente?.Quem_Indicou?.Id);

                var formasPagamentoCliente = cliente.Formas_Pagamento_Cliente.ToList();

                retorno.Formas_Pagamento = formasPagamentoCliente.Select(f => new Forma_Pagamento_ClienteModels
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

                }).ToList();

                IEnumerable<Cliente_PlanoProduto> listaPlanosProdutos = contrato != null && plano != null ? cliente?.PlanosProdutos.
                                                                Where(p => p.ClienteContrato == contrato && p.Plano == plano) : cliente?.PlanosProdutos.
                                                                Where(p => p.ClienteContrato == contrato);

                var cliente_plano_produto = plano != null ? cliente?.PlanosProdutos.Where(p => p.ClienteContrato.Id == contrato.Id && p.Plano.Id == plano.Id).FirstOrDefault() :
                     cliente?.PlanosProdutos.Where(p => p.ClienteContrato.Id == contrato.Id).FirstOrDefault();
                DateTime dataFimCarencia = cliente_plano_produto.Data_Habilitado != null ? Convert.ToDateTime(cliente_plano_produto.Data_Habilitado) : new DateTime();
                int? periodoCarencia = contrato.PeriodoCarencia != null ? Convert.ToInt32(contrato.PeriodoCarencia) : (int?)null;
                DateTime fimCarencia = periodoCarencia.HasValue ? dataFimCarencia.AddMonths(periodoCarencia.Value) : dataFimCarencia;
                retorno.Encerra_Carencia = fimCarencia.ToString(System.Globalization.CultureInfo.CreateSpecificCulture("pt-BR"));

                if (cliente.Tipo_Cliente == Tipo_Cliente.Prospecto &&
                        cliente?.Endereco_Principal?.Logradouro == null &&
                        cliente?.Endereco_Principal?.Endereco_Logradouro == null)
                {
                    retorno.End_CadastroCondominioPrincipal = cliente.End_CadastroCondominioProspecto;
                    retorno.EnderecoPrincipal = new EnderecoClienteViewModel
                    {
                        Id_Pais = cliente.Endereco_Prospecto?.Pais?.Id,
                        Pais = cliente.Endereco_Prospecto?.Pais?.Nome,
                        Id_Tipo_Imovel = cliente.Endereco_Prospecto?.Tipo_Imovel?.Cod_Tipo_Imovel,
                        Desc_Tipo_Imovel = cliente?.Endereco_Prospecto?.Tipo_Imovel?.Descricao,
                        Id_Bairro = cliente?.Endereco_Prospecto?.Id_Bairro,
                        Bairro = cliente?.Endereco_Prospecto?.Bairro,
                        CEP = cliente?.Endereco_Prospecto?.Cep,
                        Id_Cidade = cliente?.Endereco_Prospecto?.Id_Cidade,

                        Cidade = cliente?.Endereco_Prospecto?.Cidade,
                        Complemento = cliente?.Endereco_Prospecto?.Complemento,
                        Id_Endereco = cliente?.Endereco_Prospecto != null ? cliente.Endereco_Prospecto.Id : (Int64)0,
                        Latitude = Convert.ToString(cliente?.Endereco_Prospecto?.Latitude),
                        Logradouro = cliente?.Endereco_Prospecto?.Logradouro,
                        Longitude = Convert.ToString(cliente?.Endereco_Prospecto?.Longitude),
                        Numero = cliente?.Endereco_Prospecto?.Numero,
                        Ponto_Referencia = cliente?.Endereco_Prospecto?.Ponto_Referencia,

                        Tipo = Tipo_EnderecoCliente.Principal,
                        UF = cliente?.Endereco_Prospecto?.UF,
                        Busca_Endereco = cliente?.Endereco_Prospecto?.Busca_Endereco != null ? cliente.Endereco_Prospecto.Busca_Endereco : null,
                        Id_Endereco_Logradouro = cliente?.Endereco_Prospecto?.Endereco_Logradouro?.Id,
                        Id_Condominio_Predio = cliente?.Endereco_Prospecto?.Condominio_Predio?.Id,
                        Id_Bloco = cliente?.Endereco_Prospecto?.Bloco,
                        Id_Apartamento = cliente?.Endereco_Prospecto?.Apartamento,

                        Cod_Escritorio = cliente?.Endereco_Prospecto?.Cod_Escritorio,
                        Cod_Area = cliente?.Endereco_Prospecto?.Cod_Area,
                        Cod_Localidade = Convert.ToString(cliente?.Endereco_Prospecto?.Cod_Localidade),

                        Id_Cliente = cliente?.Id,

                    };
                }
                else
                {
                    retorno.EnderecoPrincipal = new EnderecoClienteViewModel
                    {
                        Id_Pais = cliente?.Endereco_Principal?.Pais?.Id,
                        Id_Tipo_Imovel = cliente?.Endereco_Principal?.Tipo_Imovel?.Cod_Tipo_Imovel,
                        Pais = cliente?.Endereco_Principal?.Pais?.Nome,
                        Desc_Tipo_Imovel = cliente?.Endereco_Principal?.Tipo_Imovel?.Descricao,
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
                        Busca_Endereco = cliente?.Endereco_Principal?.Busca_Endereco != null ? cliente?.Endereco_Principal?.Busca_Endereco : null,
                        Id_Endereco_Logradouro = cliente?.Endereco_Principal?.Endereco_Logradouro?.Id,
                        Id_Condominio_Predio = cliente?.Endereco_Principal?.Condominio_Predio?.Id,
                        Id_Bloco = cliente?.Endereco_Principal?.Bloco,
                        Id_Apartamento = cliente?.Endereco_Principal?.Apartamento,
                        Cod_Escritorio = cliente?.Endereco_Principal?.Cod_Escritorio,
                        Cod_Area = cliente?.Endereco_Principal?.Cod_Area,
                        Cod_Localidade = Convert.ToString(cliente?.Endereco_Principal?.Cod_Localidade),

                        Id_Cliente = cliente?.Id
                    };
                }

                retorno.EnderecoProspecto = new EnderecoClienteViewModel
                {
                    Id_Pais = cliente.Endereco_Prospecto?.Pais?.Id,
                    Pais = cliente.Endereco_Prospecto?.Pais?.Nome,
                    Id_Tipo_Imovel = cliente.Endereco_Prospecto?.Tipo_Imovel?.Cod_Tipo_Imovel,
                    Desc_Tipo_Imovel = cliente.Endereco_Prospecto?.Tipo_Imovel?.Descricao,
                    Id_Bairro = cliente?.Endereco_Prospecto?.Id_Bairro,
                    Bairro = cliente?.Endereco_Prospecto?.Bairro,
                    CEP = cliente?.Endereco_Prospecto?.Cep,
                    Id_Cidade = cliente?.Endereco_Prospecto?.Id_Cidade,

                    Cidade = cliente?.Endereco_Prospecto?.Cidade,
                    Complemento = cliente?.Endereco_Prospecto?.Complemento,
                    Id_Endereco = cliente?.Endereco_Prospecto != null ? cliente.Endereco_Prospecto.Id : (Int64)0,
                    Latitude = Convert.ToString(cliente?.Endereco_Prospecto?.Latitude),
                    Logradouro = cliente?.Endereco_Prospecto?.Logradouro,
                    Longitude = Convert.ToString(cliente?.Endereco_Prospecto?.Longitude),
                    Numero = cliente?.Endereco_Prospecto?.Numero,
                    Ponto_Referencia = cliente?.Endereco_Prospecto?.Ponto_Referencia,

                    Tipo = Tipo_EnderecoCliente.Principal,
                    UF = cliente?.Endereco_Prospecto?.UF,
                    Busca_Endereco = cliente?.Endereco_Prospecto?.Busca_Endereco != null ? cliente.Endereco_Prospecto.Busca_Endereco : null,
                    Id_Endereco_Logradouro = cliente?.Endereco_Prospecto?.Endereco_Logradouro?.Id,
                    Id_Condominio_Predio = cliente?.Endereco_Prospecto?.Condominio_Predio?.Id,
                    Id_Bloco = cliente?.Endereco_Prospecto?.Bloco,
                    Id_Apartamento = cliente?.Endereco_Prospecto?.Apartamento,
                    Cod_Escritorio = cliente.Endereco_Prospecto?.Cod_Escritorio,
                    Cod_Area = cliente.Endereco_Prospecto?.Cod_Area,
                    Cod_Localidade = Convert.ToString(cliente.Endereco_Prospecto?.Cod_Localidade),

                    Id_Cliente = cliente?.Id
                };
                if (plano != null)
                {
                    var enderecoPlano = listaPlanosProdutos.FirstOrDefault().Endereco_Instalacao;
                    retorno.EnderecoInstalacao = new EnderecoClienteViewModel
                    {
                        Tipo = Tipo_EnderecoCliente.Instalacao,
                        Id_Pais = enderecoPlano?.Pais?.Id,
                        Pais = enderecoPlano?.Pais?.Nome,
                        Id_Tipo_Imovel = enderecoPlano?.Tipo_Imovel?.Cod_Tipo_Imovel,
                        Desc_Tipo_Imovel = enderecoPlano?.Tipo_Imovel?.Descricao,
                        Id_Bairro = enderecoPlano?.Id_Bairro,
                        Bairro = enderecoPlano?.Bairro,
                        CEP = enderecoPlano?.Cep,
                        Id_Cidade = enderecoPlano?.Id_Cidade,
                        Cidade = enderecoPlano?.Cidade,
                        Complemento = enderecoPlano?.Complemento,
                        Id_Endereco = enderecoPlano != null ? enderecoPlano.Id : (Int64)0,
                        Latitude = Convert.ToString(enderecoPlano?.Latitude),
                        Logradouro = enderecoPlano?.Logradouro,
                        Longitude = Convert.ToString((enderecoPlano?.Longitude)),
                        Numero = enderecoPlano?.Numero,
                        Ponto_Referencia = enderecoPlano?.Ponto_Referencia,
                        UF = enderecoPlano?.UF,
                        Busca_Endereco = enderecoPlano?.Busca_Endereco,
                        Id_Endereco_Logradouro = enderecoPlano?.Endereco_Logradouro?.Id,
                        Id_Condominio_Predio = enderecoPlano?.Condominio_Predio?.Id,
                        Id_Bloco = enderecoPlano?.Bloco,
                        Id_Apartamento = enderecoPlano?.Apartamento,
                        Cod_Escritorio = enderecoPlano?.Cod_Escritorio,
                        Cod_Area = enderecoPlano?.Cod_Area,
                        Cod_Localidade = Convert.ToString((enderecoPlano?.Cod_Localidade)),
                        Id_Cliente = cliente.Id
                    };
                }
                else
                {
                    retorno.EnderecoInstalacao = new EnderecoClienteViewModel
                    {
                        Tipo = Tipo_EnderecoCliente.Instalacao,

                        Cod_Escritorio_Prospect = retorno.EnderecoProspecto.Cod_Escritorio,
                        Cod_Area_Prospect = retorno.EnderecoProspecto.Cod_Area,
                        Cod_Localidade_Prospect = retorno.EnderecoProspecto.Cod_Localidade,
                    };
                }
                retorno.Exige_Termo_Adesao = config.Exige_Termo_Adesao;
                retorno.Exige_Termo_Contrato = config.Exige_Termo_Contrato;
                retorno.Utiliza_Envio_Automatico_Contrato = config.Utiliza_Envio_Automatico_Contrato;
                retorno.Exige_Foto_Termo = config.Exige_Foto_Termo;
                retorno.Permite_Work_Sem_Termo = config.Permite_Work_Sem_Termo;
                retorno.Mudanca_Plano = true;
                retorno.Planos = new List<PlanoProdutoListModel>();

                foreach (Cliente_PlanoProduto planoProduto in listaPlanosProdutos)
                {

                    var planoProdutoModel = this.GetPlanoProdutoListModel(cliente, planoProduto);
                    retorno.Planos.Add(planoProdutoModel);
                }
            }

            return retorno;
            #endregion
        }
        #region [GetViewObjByVenda]
        private async Task<VendaDadosGeraisViewModel> GetViewObjByVenda(Venda venda, Cliente cliente, long? contrato, long? plano, long? clientePlanoProduto, ISession session)
        {
            using (ISession sessionT5Sql = HibernateUtil.GetSessionT5())
            using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())

            using (Conexao<Documento_ProducaoAniel> repoDocProd = new Conexao<Documento_ProducaoAniel>(sessionT5Nh))
            using (Conexao<Configuracoes_Gerais> repoConfiguracoes_Gerais = new Conexao<Configuracoes_Gerais>(session))
            using (Conexao<Atividade_StatusVenda> repo_AtividadeStatus = new Conexao<Atividade_StatusVenda>(session))
            using (Conexao<Workflow_VendasAtividade> repo_Atividade = new Conexao<Workflow_VendasAtividade>(session))
            using (Conexao<Venda_Ultimo_Contato> repo_VendaContato = new Conexao<Venda_Ultimo_Contato>(session))
            using (Conexao<LoginRadius> repo_LoginRadius = new Conexao<LoginRadius>(session))
            using (Conexao<Modelo_Contrato> repo_ModeloContrato = new Conexao<Modelo_Contrato>(session))
            using (Conexao<Uf> repo_Uf = new Conexao<Uf>(session))
            using (Conexao<DocTermoAnexo> repoDocAceite = new Conexao<DocTermoAnexo>(session))
            using (Conexao<ClienteContrato> repoClienteContrato = new Conexao<ClienteContrato>(session))
            using (Conexao<Plano> repoPlano = new Conexao<Plano>(session))
            {
                VendaDadosGeraisViewModel retorno = new VendaDadosGeraisViewModel();
                Configuracoes_Gerais config = (await repoConfiguracoes_Gerais.selectFirst());

                //retorno.Id = venda.Id;

                retorno.Id_Vendedor = venda.Vendedor != null ? venda.Vendedor.Id : (Int64?)null;
                retorno.Id_Supervisor = venda.Supervisor != null ? venda.Supervisor.Id : (Int64?)null;
                retorno.Identificacao_Cliente = cliente?.Numero_Contrato;
                retorno.Tipo_Cliente = cliente.Tipo_Cliente.Value;
                retorno.Id_Venda = venda.Id;
                retorno.Nome_Vendedor = venda?.Vendedor?.Nome;

                retorno.End_CadastroCondominioPrincipal = venda.End_CadastroCondominioPrincipal;
                retorno.End_CadastroCondominioInstalacao = venda.End_CadastroCondominioInstalacao;
                retorno.End_CadastroCondominioProspecto = cliente.End_CadastroCondominioProspecto;
                retorno.End_CadastroCondominioCobranca = venda.End_CadastroCondominioCobranca;

                retorno.Nome_Supervisor = venda?.Supervisor?.Nome;
                retorno.DataVenda = venda?.Data_Venda?.ToString("dd/MM/yyyy");
                retorno.DataCriacao = venda?.DataHoraCriacao?.ToString("dd/MM/yyyy");
                retorno.DataEncerramento = venda?.Data_Venda?.ToString("dd/MM/yyyy");
                retorno.Justificativa_Avaliacao_Comercial = venda?.Motivo_Desistencia_Venda != null ? (venda?.Motivo_Desistencia_Venda?.Descricao ?? venda?.Motivo_Desistencia_Venda?.Descricao_Outro) : venda?.Justificativa_Avaliacao_Comercial;
                retorno.Id_Departamento = venda.Departamento?.Id;
                retorno.Status_Liberacao = venda.Status_Liberacao;

                if (venda.Workflow_VendasAtividade != null)
                {
                    retorno.Workflow_VendasAtividade = venda.Workflow_VendasAtividade;
                    retorno.Workflow_Vendas = venda.Workflow_Vendas;

                    retorno.Status = venda.Workflow_VendasAtividade.Ativ_StatusVenda.StatusPadrao.Value != venda.Status ? venda.Status : venda.Workflow_VendasAtividade.Ativ_StatusVenda.StatusPadrao.Value;
                    if (venda.Workflow_VendasAtividade.Ativ_NAtivAprovada.HasValue)
                    {
                        Workflow_VendasAtividade workflowAtivAprovacao = await repo_Atividade.selectFirst(p => p.Id == venda.Workflow_VendasAtividade.Ativ_NAtivAprovada);

                        retorno.AprovarEtapaWorkflowAprovaVenda =
                            workflowAtivAprovacao?.Ativ_StatusVenda?.StatusPadrao != null &&
                            workflowAtivAprovacao?.Ativ_StatusVenda?.StatusPadrao.Value == Status_Venda.Venda_Aprovada;
                    }

                    if (venda.Workflow_VendasAtividade.Ativ_NAtivReprovada.HasValue)
                    {
                        Workflow_VendasAtividade workflowAtivReprovacao = await repo_Atividade.selectFirst(p => p.Id == venda.Workflow_VendasAtividade.Ativ_NAtivReprovada);

                        retorno.ReprovarEtapaWorkflowCancelaVenda =
                            workflowAtivReprovacao?.Ativ_StatusVenda?.StatusPadrao != null &&
                            workflowAtivReprovacao?.Ativ_StatusVenda?.StatusPadrao.Value == Status_Venda.Venda_Cancelada;
                    }
                }
                else
                {
                    retorno.Status = venda.Status;
                };

                retorno.Descricao_WF = venda.Workflow_Vendas?.Descricao;
                retorno.Id_Workflow_Vendas = venda.Workflow_Vendas?.Id;

                if (retorno.Id_Workflow_Vendas != null)
                {
                    retorno.StatusVenda = venda.Workflow_VendasAtividade?.Ativ_StatusVenda.Descricao;
                }

                else
                {
                    retorno.StatusVenda = HtmlEnumExtensions.GetEnumDescription<Status_Venda>(Status_Venda.Analise_Cliente);
                }

                retorno.StatusAtividadeAprovada = venda.Workflow_VendasAtividade?.Ativ_NAtivAprovada != null ? repo_Atividade.selectFirst(p => p.Id == (long)venda.Workflow_VendasAtividade.Ativ_NAtivAprovada).Result?.Ativ_StatusVenda?.StatusPadrao : (Status_Venda?)null;
                retorno.StatusAtividadeReprovada = venda.Workflow_VendasAtividade?.Ativ_NAtivReprovada != null ? repo_Atividade.selectFirst(p => p.Id == (long)venda.Workflow_VendasAtividade.Ativ_NAtivReprovada).Result?.Ativ_StatusVenda?.StatusPadrao : (Status_Venda?)null;
                retorno.Prazo_Atividade = venda.Workflow_VendasAtividade?.Ativ_Prazo_Minutos;
                retorno.Decorrido_Atividade = venda.Data_Alteracao_Status.HasValue ? (DateTime.Now - venda.Data_Alteracao_Status.Value).ToString(@"hh\:mm") : (venda.DataHoraCriacao.HasValue ? (DateTime.Now - venda.DataHoraCriacao.Value).ToString(@"hh\:mm") : null);
                retorno.Decorrido_Workflow_Vendas = venda.DataHoraCriacao.HasValue ? (DateTime.Now - venda.DataHoraCriacao.Value).ToString(@"hh\:mm") : null;
                retorno.Prazo_Workflow_Vendas = venda.Workflow_Vendas?.Prazo;
                retorno.Atividade_Decisoria = venda.Workflow_VendasAtividade?.Ativ_Decisao;
                retorno.InformacoesComplementares = venda.InformacoesComplementares;


                IEnumerable<string> nome = cliente.NomeFantasia != null ? cliente.NomeFantasia.Split(' ') : null;
                string utimoNome = nome != null ? nome.Last() : null;

                retorno.IdAtendimento = venda.IdAtendimento;
                retorno.StatusAuditoria = venda.StatusAuditoria;

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
                //retorno.Forma_Envio = cliente?.Forma_Envio;

                //retorno.Ultimo_Contato = venda?.Data_Ultimo_Contato;
                retorno.Ultimo_Contato = repo_VendaContato.selectFirst(p => p.Id_Venda == venda.Id).Result?.Data_Ultimo_Contato;

                if (retorno.Ultimo_Contato != null)
                {
                    int tempo = (DateTime.Now - retorno.Ultimo_Contato).Value.Days;
                    retorno.Tempo_Ultimo_Contato = tempo == 0 ? "Hoje" : String.Concat(tempo.ToString(), " ", "Dias");
                }

                retorno.Id_Cliente = cliente.Id;
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
                retorno.Qual_Meio = cliente?.Outro_Meio;
                retorno.TipoPessoa = cliente?.Tipo_Pessoa;

                retorno.End_Instalacao_Igual_Principal = venda.Status == Status_Venda.Analise_Cliente && config.EnderecoInstalacaoMesmoPrincipal ? true : cliente.End_Instalacao_Igual_Principal;

                retorno.Email = cliente?.Email_Principal;
                retorno.TelefoneFixo = cliente?.Telefone_Fixo;
                retorno.TelefoneCelular = cliente?.Telefone_Celular;
                retorno.TelefoneRecado = cliente?.Telefone_Recado;
                retorno.NomeRecado = cliente?.Pessoa_Tel_Recado;
                //retorno.Desc_Banco = cliente?.Banco?.Descricao;

                retorno.Quem_Indicou = cliente?.Quem_Indicou?.NomeFantasia;

                retorno.Id_Categoria_Cliente = cliente?.Categoria_Cliente?.Id;
                retorno.Id_Meio_Captacao = cliente?.Qual_Meio?.Id;
                retorno.Meio_Captacao = cliente?.Qual_Meio?.Descricao;
                retorno.Id_Quem_Indicou = cliente?.Quem_Indicou?.Id;
                retorno.Quem_Indicou = Convert.ToString(cliente?.Quem_Indicou?.Id);

                if (cliente.Tipo_Cliente == Tipo_Cliente.Prospecto &&
                    cliente?.Endereco_Principal?.Logradouro == null &&
                    cliente?.Endereco_Principal?.Endereco_Logradouro == null)
                {
                    retorno.End_CadastroCondominioPrincipal = cliente.End_CadastroCondominioProspecto;
                    retorno.EnderecoPrincipal = new EnderecoClienteViewModel
                    {
                        Id_Pais = cliente.Endereco_Prospecto?.Pais?.Id,
                        Id_Tipo_Imovel = cliente.Endereco_Prospecto?.Tipo_Imovel?.Cod_Tipo_Imovel,
                        Id_Bairro = cliente?.Endereco_Prospecto?.Id_Bairro,
                        Bairro = cliente?.Endereco_Prospecto?.Bairro,
                        CEP = cliente?.Endereco_Prospecto?.Cep,
                        Id_Cidade = cliente?.Endereco_Prospecto?.Id_Cidade,

                        Cidade = cliente?.Endereco_Prospecto?.Cidade,
                        Complemento = cliente?.Endereco_Prospecto?.Complemento,
                        Id_Endereco = cliente?.Endereco_Prospecto != null ? cliente.Endereco_Prospecto.Id : (Int64)0,
                        Latitude = Convert.ToString(cliente?.Endereco_Prospecto?.Latitude),
                        Logradouro = cliente?.Endereco_Prospecto?.Logradouro,
                        Longitude = Convert.ToString(cliente?.Endereco_Prospecto?.Longitude),
                        Numero = cliente?.Endereco_Prospecto?.Numero,
                        Ponto_Referencia = cliente?.Endereco_Prospecto?.Ponto_Referencia,

                        Tipo = Tipo_EnderecoCliente.Principal,
                        UF = cliente?.Endereco_Prospecto?.UF,
                        Busca_Endereco = cliente?.Endereco_Prospecto?.Busca_Endereco != null ? cliente.Endereco_Prospecto.Busca_Endereco : null,
                        Id_Endereco_Logradouro = cliente?.Endereco_Prospecto?.Endereco_Logradouro?.Id,
                        Id_Condominio_Predio = cliente?.Endereco_Prospecto?.Condominio_Predio?.Id,
                        Id_Bloco = cliente?.Endereco_Prospecto?.Bloco,
                        Id_Apartamento = cliente?.Endereco_Prospecto?.Apartamento,

                        Cod_Escritorio = cliente?.Endereco_Prospecto?.Cod_Escritorio,
                        Cod_Area = cliente?.Endereco_Prospecto?.Cod_Area,
                        Cod_Localidade = Convert.ToString(cliente?.Endereco_Prospecto?.Cod_Localidade),

                        Id_Cliente = cliente?.Id
                    };
                }
                else
                {
                    retorno.EnderecoPrincipal = new EnderecoClienteViewModel
                    {
                        Id_Pais = cliente?.Endereco_Principal?.Pais?.Id,
                        Id_Tipo_Imovel = cliente?.Endereco_Principal?.Tipo_Imovel?.Cod_Tipo_Imovel,
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
                        Busca_Endereco = cliente?.Endereco_Principal?.Busca_Endereco != null ? cliente?.Endereco_Principal?.Busca_Endereco : null,
                        Id_Endereco_Logradouro = cliente?.Endereco_Principal?.Endereco_Logradouro?.Id,
                        Id_Condominio_Predio = cliente?.Endereco_Principal?.Condominio_Predio?.Id,
                        Id_Bloco = cliente?.Endereco_Principal?.Bloco,
                        Id_Apartamento = cliente?.Endereco_Principal?.Apartamento,
                        Cod_Escritorio = cliente?.Endereco_Principal?.Cod_Escritorio,
                        Cod_Area = cliente?.Endereco_Principal?.Cod_Area,
                        Cod_Localidade = Convert.ToString(cliente?.Endereco_Principal?.Cod_Localidade),

                        Id_Cliente = cliente?.Id
                    };

                }

                retorno.EnderecoProspecto = new EnderecoClienteViewModel
                {
                    Id_Pais = cliente.Endereco_Prospecto?.Pais?.Id,
                    Id_Tipo_Imovel = cliente.Endereco_Prospecto?.Tipo_Imovel?.Cod_Tipo_Imovel,
                    Id_Bairro = cliente?.Endereco_Prospecto?.Id_Bairro,
                    Bairro = cliente?.Endereco_Prospecto?.Bairro,
                    CEP = cliente?.Endereco_Prospecto?.Cep,
                    Id_Cidade = cliente?.Endereco_Prospecto?.Id_Cidade,

                    Cidade = cliente?.Endereco_Prospecto?.Cidade,
                    Complemento = cliente?.Endereco_Prospecto?.Complemento,
                    Id_Endereco = cliente?.Endereco_Prospecto != null ? cliente.Endereco_Prospecto.Id : (Int64)0,
                    Latitude = Convert.ToString(cliente?.Endereco_Prospecto?.Latitude),
                    Logradouro = cliente?.Endereco_Prospecto?.Logradouro,
                    Longitude = Convert.ToString(cliente?.Endereco_Prospecto?.Longitude),
                    Numero = cliente?.Endereco_Prospecto?.Numero,
                    Ponto_Referencia = cliente?.Endereco_Prospecto?.Ponto_Referencia,

                    Tipo = Tipo_EnderecoCliente.Principal,
                    UF = cliente?.Endereco_Prospecto?.UF,
                    Busca_Endereco = cliente?.Endereco_Prospecto?.Busca_Endereco != null ? cliente.Endereco_Prospecto.Busca_Endereco : null,
                    Id_Endereco_Logradouro = cliente?.Endereco_Prospecto?.Endereco_Logradouro?.Id,
                    Id_Condominio_Predio = cliente?.Endereco_Prospecto?.Condominio_Predio?.Id,
                    Id_Bloco = cliente?.Endereco_Prospecto?.Bloco,
                    Id_Apartamento = cliente?.Endereco_Prospecto?.Apartamento,
                    Cod_Escritorio = cliente.Endereco_Prospecto?.Cod_Escritorio,
                    Cod_Area = cliente.Endereco_Prospecto?.Cod_Area,
                    Cod_Localidade = Convert.ToString(cliente.Endereco_Prospecto?.Cod_Localidade),

                    Id_Cliente = cliente?.Id
                };

                if (venda.Id == 0)
                {
                    retorno.EnderecoInstalacao = new EnderecoClienteViewModel
                    {
                        Tipo = Tipo_EnderecoCliente.Instalacao,

                        Cod_Escritorio_Prospect = retorno.EnderecoProspecto.Cod_Escritorio,
                        Cod_Area_Prospect = retorno.EnderecoProspecto.Cod_Area,
                        Cod_Localidade_Prospect = retorno.EnderecoProspecto.Cod_Localidade,
                    };
                }
                else
                {
                    retorno.EnderecoInstalacao = new EnderecoClienteViewModel
                    {
                        Id_Pais = venda.EnderecoInstalacao?.Pais?.Id,
                        Id_Tipo_Imovel = venda.EnderecoInstalacao?.Tipo_Imovel?.Cod_Tipo_Imovel,
                        Id_Bairro = venda.EnderecoInstalacao?.Id_Bairro,
                        Bairro = venda.EnderecoInstalacao?.Bairro,
                        CEP = venda.EnderecoInstalacao?.Cep,
                        Id_Cidade = venda.EnderecoInstalacao?.Id_Cidade,

                        Cidade = venda.EnderecoInstalacao?.Cidade,
                        Complemento = venda.EnderecoInstalacao?.Complemento,
                        Id_Endereco = venda.EnderecoInstalacao != null ? venda.EnderecoInstalacao.Id : (Int64)0,
                        Latitude = Convert.ToString(venda.EnderecoInstalacao?.Latitude),
                        Logradouro = venda.EnderecoInstalacao?.Logradouro,
                        Longitude = Convert.ToString(venda.EnderecoInstalacao?.Longitude),
                        Numero = venda.EnderecoInstalacao?.Numero,
                        Ponto_Referencia = venda.EnderecoInstalacao?.Ponto_Referencia,

                        Tipo = Tipo_EnderecoCliente.Instalacao,
                        UF = venda.EnderecoInstalacao?.UF,
                        Busca_Endereco = venda.EnderecoInstalacao?.Busca_Endereco,
                        Id_Endereco_Logradouro = venda.EnderecoInstalacao?.Endereco_Logradouro?.Id,
                        Id_Condominio_Predio = venda.EnderecoInstalacao?.Condominio_Predio?.Id,
                        Id_Bloco = venda.EnderecoInstalacao?.Bloco,
                        Id_Apartamento = venda.EnderecoInstalacao?.Apartamento,

                        Cod_Escritorio = venda.EnderecoInstalacao?.Cod_Escritorio,
                        Cod_Area = venda.EnderecoInstalacao?.Cod_Area,
                        Cod_Localidade = Convert.ToString(venda.EnderecoInstalacao?.Cod_Localidade),

                        Id_Cliente = venda?.Id_Cliente
                    };
                }

                var formasPagamentoCliente = cliente.Formas_Pagamento_Cliente.ToList();

                retorno.Formas_Pagamento = formasPagamentoCliente.Select(f => new Forma_Pagamento_ClienteModels
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

                }).ToList();

                //Descontos da Venda Específica
                IEnumerable<String> ContratosEspecificosVenda = cliente?.PlanosProdutos.Where(p => p.Id_Venda == retorno.Id).Select(p => p.ClienteContrato?.NumeroContrato).ToList();
                IEnumerable<Cliente_Desconto> DescontosEspecificosVenda = cliente?.Descontos.Where(p => ContratosEspecificosVenda.Contains(p.Numero_ContratoVinculado)).ToList();

                retorno.Descontos = (cliente?.Descontos.Count() == 0) ? new List<DescontoListaModel>() :

                DescontosEspecificosVenda.Select(p => new DescontoListaModel
                {
                    Id = p.Id,
                    Id_Desconto = p.Desconto != null ? p.Desconto.Id : 0,
                    Descricao = p.Desconto?.Descricao,
                    Valor_Desconto = Convert.ToString(p?.Valor_Desconto),
                    Percentual_Desconto = Convert.ToString(p.Desconto?.Percentual_Desconto),
                    Tipo_Desconto = p.Desconto?.Tipo_Desconto?.Descricao,
                    Numero_ContratoVinculado = p.Numero_ContratoVinculado,
                    Formato = !String.IsNullOrEmpty(Convert.ToString(p.Desconto?.Tipo_Desconto?.Formato)) ? Convert.ToString(Convert.ToInt64(p.Desconto?.Tipo_Desconto?.Formato)) : String.Empty
                }).ToList();

                ClienteContrato registroContrato = await repoClienteContrato.selectFirst(p => p.Id == contrato);
                Plano registroPlano = await repoPlano.selectFirst(p => p.Id == plano);
                IEnumerable<Cliente_PlanoProduto> listaPlanosProdutos = registroContrato != null || registroPlano != null ? cliente?.PlanosProdutos.
                                                                            Where(p => p.Id_Venda.HasValue && p.Id_Venda.Value == venda.Id &&
                                                                                  p.ClienteContrato == registroContrato &&
                                                                                  p.Plano == registroPlano) :
                                                                                  cliente?.PlanosProdutos.Where(p => p.Id == clientePlanoProduto).ToList();

                //Pega Planos e produtos diferentes de cancelado
                retorno.Planos = new List<PlanoProdutoListModel>();

                foreach (Cliente_PlanoProduto planoProduto in listaPlanosProdutos)
                {
                    var planoProdutoModel = this.GetPlanoProdutoListModelByVenda(planoProduto, venda);
                    retorno.Planos.Add(planoProdutoModel);
                }

                retorno.Termo_Adesao_Assinado = venda.Termo_Adesao_Assinado;
                retorno.Exige_Termo_Adesao = config.Exige_Termo_Adesao;
                retorno.Utiliza_Envio_Automatico_Contrato = config.Utiliza_Envio_Automatico_Contrato;
                retorno.Exige_Termo_Contrato = config.Exige_Termo_Contrato;
                retorno.Exige_Foto_Termo = config.Exige_Foto_Termo;
                retorno.Permite_Work_Sem_Termo = config.Permite_Work_Sem_Termo;
                retorno.Mudanca_Plano = venda.Mudanca_Plano;

                #region Consulta de Crédito

                retorno.Consulta_Viabilidade_Realizada = venda.Consulta_Viabilidade_Realizada;

                #endregion

                IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(session);
                IClienteRepository repoClient = new ClienteRepository(session);
                IEtapaVendaRepository repoEtapaVenda = new EtapaVendaRepository(session);

                var etapaVendaBusiness = new EtapaVendaBusiness(repoEtapaVenda, repoConfig, repoClient, cliente.Id);

                IEnumerable<EtapaVendaViewModel> etapasVenda = etapaVendaBusiness.GetDadosEtapasVenda(venda.Id);

                // Variável com a última etapa da venda para saber se devemos verificar se o checklist e o termo de adesão estão preenchidos.
                // O preenchimento dos dados das etapas da venda são preenchidos depois
                EtapaVendaViewModel ultimaEtapaVenda =
                    etapasVenda.FirstOrDefault(e => (e.Status == (int)StatusEtapaVenda.Reprovado && e.Etapa != 7) || e.Status == (int)Sinapse.Model.enumerados.StatusEtapaVenda.Pendente);

                // Caso todas as etapas tenham sido aprovadas a etapa atual será a última etapa da venda
                if (ultimaEtapaVenda == null)
                {
                    ultimaEtapaVenda = etapasVenda.LastOrDefault();
                }

                #region Checklist da venda

                if (venda.Id != 0)
                {
                    IChecklistRepository repoChecklist = new ChecklistRepository(sessionT5Nh, sessionT5Sql);
                    var checklistBusiness = new ChecklistBusiness(repoChecklist, repoEtapaVenda);

                    if (!checklistBusiness.VendaHasChecklist(retorno.Id))
                    {
                        // Cria a checklist somente caso a etapa do checklist seja a etapa atual e ainda não tenha nenhum checklist
                        // associado a venda
                        if (ultimaEtapaVenda.Etapa == (int)Sinapse.Model.enumerados.EnumEtapaVenda.Checklist)
                        {
                            Tipo_Pessoa tipoPessoa = cliente.Tipo_Pessoa.Value;
                            long? idCategoriaCliente = cliente.Categoria_Cliente?.Id;
                            long? idMeioCaptacao = cliente.Qual_Meio?.Id;

                            long? idUf = null;
                            long? idCidade = null;

                            if (config.Utiliza_Cadastro_Endereco)
                            {
                                idUf = venda.EnderecoInstalacao?.Endereco_Logradouro?.UF?.Id;
                                idCidade = venda.EnderecoInstalacao?.Endereco_Logradouro?.Cidade?.Id;
                            }
                            else
                            {
                                string siglaUf = venda.EnderecoInstalacao?.UF;

                                if (!String.IsNullOrEmpty(siglaUf))
                                {
                                    idUf = repo_Uf.selectFirst(u => u.Sigla == siglaUf).Result?.Id;
                                }

                                idCidade = venda.EnderecoInstalacao?.Id_Cidade;
                            }

                            checklistBusiness.AssociarChecklistVenda(
                                retorno.Id,
                                tipoPessoa,
                                idCategoriaCliente,
                                idMeioCaptacao,
                                idUf,
                                idCidade
                            );

                            // Envia a checklist por e-mail para o cliente
                            if (config.EnviarCheckListAuto)
                            {
                                await EnviarEmailChecklist(retorno.Id);
                            }
                        }
                    }
                    else if (ultimaEtapaVenda.Etapa >= (int)Sinapse.Model.enumerados.EnumEtapaVenda.Checklist)
                    {
                        if (checklistBusiness.ChecklistPreenchido(retorno.Id))
                        {
                            etapaVendaBusiness.MudarStatusEtapa(retorno.Id, Sinapse.Model.enumerados.EnumEtapaVenda.Checklist, Sinapse.Model.enumerados.StatusEtapaVenda.Aprovado);
                        }
                        else
                        {
                            etapaVendaBusiness.MudarStatusEtapa(retorno.Id, Sinapse.Model.enumerados.EnumEtapaVenda.Checklist, Sinapse.Model.enumerados.StatusEtapaVenda.Reprovado);
                        }
                    }
                }

                #endregion

                #region Termo de Adesão

                if (retorno.Id != 0 && ultimaEtapaVenda.Etapa >= (int)Sinapse.Model.enumerados.EnumEtapaVenda.TermoAdesao)
                {
                    if (venda.Termo_Adesao_Assinado)
                    {
                        etapaVendaBusiness.MudarStatusEtapa(retorno.Id, Sinapse.Model.enumerados.EnumEtapaVenda.TermoAdesao, Sinapse.Model.enumerados.StatusEtapaVenda.Aprovado);
                    }
                }

                #endregion

                // O preenchimento das etapas da venda devem sempre estar após a verificação do checklist e do termo de adesão
                // pois essas duas verificações podem alterar os status das etapas da venda.
                #region Etapa da Venda

                retorno.EtapasVenda = etapaVendaBusiness.GetDadosEtapasVenda(venda.Id);

                EtapaVendaViewModel ultimaEtapa =
                    retorno.EtapasVenda.FirstOrDefault(e => (e.Status == (int)StatusEtapaVenda.Reprovado && e.Etapa != 7) || e.Status == (int)Sinapse.Model.enumerados.StatusEtapaVenda.Pendente);

                // Caso todas as etapas tenham sido aprovadas a etapa atual será a última etapa da venda
                if (ultimaEtapa == null)
                {
                    ultimaEtapa = retorno.EtapasVenda.LastOrDefault();
                }

                retorno.EtapaAtualVenda = ultimaEtapa.Etapa;

                retorno.EtapasVendasConcluidas =
                    retorno.EtapasVenda != null &&
                    retorno.EtapasVenda.Where(t => t.Etapa < 7 && t.Status == (int)Sinapse.Model.enumerados.StatusEtapaVenda.Pendente).Count() == 0;

                #endregion

                #region Status das consultas de restrição

                IConsultaCreditoRepository repoConsultaCredito = new ConsultaCreditoRepository(session);
                ICriteriosParamCreditoRepository repoCriterios = new CriteriosParamCreditoRepository(session);
                IParametrosConsultaCreditoRepository repoParamsConsulta = new ParametrosConsultaCreditoRepository(session);

                var consultaCreditoBusiness =
                new ConsultaCreditoBusiness(
                        repoConsultaCredito,
                        repoCriterios,
                        repoParamsConsulta,
                        repoConfig
                );

                IPendenciaEnderecoRepository repoPendenciaEnderecoConnect = new PendenciaEnderecoRepository(session);
                IPendenciaEnderecoAnielRepository repoPendenciaEnderecoAnielConnect = new PendenciaEnderecoAnielRepository(session);

                var pendenciaEnderecoBusiness = new PendenciaEnderecoBusiness(repoPendenciaEnderecoConnect, repoConfig);

                IPendenciaFinanceiraRepository repoPendenciaFinanceiraConnect = new PendenciaFinanceiraRepository(session);
                var pendenciaFinanceiraBusiness = new PendenciaFinanceiraBusiness(repoPendenciaFinanceiraConnect, repoConfig);

                IConsultaViabilidadeRepository repoConsultaViabilidade = new ConsultaViabilidadeRepository(sessionT5Nh);
                IElementoRedeAnielRepository repoElemRede = new ElementoRedeAnielRepository(sessionT5Nh);
                IElementoRedePortaAnielRepository repoPorta = new ElementoRedePortaAnielRepository(session, sessionT5Nh);

                var consultaViabilidadeBusiness =
                    new ConsultaViabilidadeBusiness(
                        repoConsultaViabilidade,
                        repoElemRede,
                        repoPorta,
                        repoConfig
                    );
                if (retorno.EtapasVenda.Any(e => e.Etapa == (int)EnumEtapaVenda.ConsultaCredito))
                {
                    if (retorno.TipoPessoa.Value == Tipo_Pessoa.Fisica)
                    {
                        retorno.StatusConsultaCredito =
                            consultaCreditoBusiness.GetStatusConsultaCreditoRestricaoByCpf(cliente.CpfCnpj);

                        retorno.StatusConsultaPendenciaFinanceira =
                            pendenciaFinanceiraBusiness.GetStatusConsultaPendenciaFinanceiraByCpf(cliente.CpfCnpj);
                    }
                    else
                    {
                        retorno.StatusConsultaCredito =
                            consultaCreditoBusiness.GetStatusConsultaCreditoRestricaoByCnpj(cliente.CpfCnpj);

                        retorno.StatusConsultaPendenciaFinanceira =
                            pendenciaFinanceiraBusiness.GetStatusConsultaPendenciaFinanceiraByCnpj(cliente.CpfCnpj);
                    }
                }

                if (retorno.EtapasVenda.Any(e => e.Etapa == (int)EnumEtapaVenda.ConsultaPendenciaFinanceira))
                {
                    // Caso a consulta de pendência financeira não retorne pendência alguma o método GetStatusConsultaPendenciaFinanceiraByCpf() e
                    // GetStatusConsultaPendenciaFinanceiraByCnpj() retornam incorretamente que a consulta ainda está pendente pois não encontrou
                    // nenhum registro em banco.

                    // Para driblar esse caso, verificamos se a etapa da consulta de pendência financeira na venda
                    // já não está mais pendente e passamos a utilizar o status da etapa
                    int statusEtapaConsultaPendenciaFinanceira = retorno.EtapasVenda.First(p => p.Etapa == (int)EnumEtapaVenda.ConsultaPendenciaFinanceira).Status;

                    if (retorno.StatusConsultaPendenciaFinanceira == StatusEtapaVenda.Pendente &&
                        statusEtapaConsultaPendenciaFinanceira != (int)StatusEtapaVenda.Pendente)
                    {
                        retorno.StatusConsultaPendenciaFinanceira = (StatusEtapaVenda)statusEtapaConsultaPendenciaFinanceira;
                    }
                }

                string logradouro;

                if (cliente.Endereco_Principal != null)
                {
                    if (config.Utiliza_Cadastro_Endereco)
                    {
                        logradouro =
                            cliente.Endereco_Principal?.Endereco_Logradouro != null ?
                                 cliente.Endereco_Principal.Endereco_Logradouro.Descricao :
                                 cliente.Endereco_Principal?.Condominio_Predio?.Descricao;
                    }
                    else
                    {
                        logradouro = cliente.Endereco_Principal?.Logradouro;
                    }
                }
                else
                {
                    if (config.Utiliza_Cadastro_Endereco)
                    {
                        logradouro =
                            cliente.Endereco_Prospecto?.Endereco_Logradouro != null ?
                                 cliente.Endereco_Prospecto.Endereco_Logradouro.Descricao :
                                 cliente.Endereco_Prospecto?.Condominio_Predio?.Descricao;
                    }
                    else
                    {
                        logradouro = cliente.Endereco_Prospecto?.Logradouro;
                    }
                }

                retorno.StatusConsultaPendenciaEndereco =
                    pendenciaEnderecoBusiness.GetStatusConsultaPendenciaEndereco(logradouro, retorno.EnderecoProspecto.Numero);

                if (retorno.EtapasVenda.Any(e => e.Etapa == (int)EnumEtapaVenda.ConsultaPendenciaEndereco))
                {
                    // Caso a consulta de pendência de endereço não retorne pendência alguma o método GetStatusConsultaPendenciaEndereco() retorna
                    // incorretamente que a consulta ainda está pendente pois não encontrou nenhum registro em banco.

                    // Para driblar esse caso, verificamos se a etapa da consulta de pendência de endereço na venda
                    // já não está mais pendente e passamos a utilizar o status da etapa
                    int statusEtapaConsultaPendenciaEndereco = retorno.EtapasVenda.First(p => p.Etapa == (int)EnumEtapaVenda.ConsultaPendenciaEndereco).Status;

                    if (retorno.StatusConsultaPendenciaEndereco == StatusEtapaVenda.Pendente &&
                        statusEtapaConsultaPendenciaEndereco != (int)StatusEtapaVenda.Pendente)
                    {
                        retorno.StatusConsultaPendenciaEndereco = (StatusEtapaVenda)statusEtapaConsultaPendenciaEndereco;
                    }
                }

                if (!String.IsNullOrEmpty(venda.MotivoAprovManualViabilidade))
                {
                    retorno.StatusConsultaViabilidade = StatusEtapaVenda.Aprovado;
                }
                else
                {
                    retorno.StatusConsultaViabilidade =
                        consultaViabilidadeBusiness.GetStatusConsultaViabilidade(
                            (venda.EnderecoInstalacao?.Latitude).GetValueOrDefault(0),
                            (venda.EnderecoInstalacao?.Longitude).GetValueOrDefault(0)
                        );
                }


                retorno.ConsultaViabilidadeRestringeVenda = config.ConsultaViabilidadeRestringeVenda;
                retorno.ConsultaPendenciaEnderecoRestringeVenda = config.ConsultaPendenciaEnderecoRestringeVenda;
                retorno.ConsultaPendenciaFinanceiraRestringeVenda = config.ConsultaPendenciaFinanceiraRestringeVenda;
                retorno.ConsultaCreditoRestringeVenda = config.ConsultaCreditoRestringeVenda;

                retorno.ConsultaViabilidadeVendedorVisualiza = config.ConsultaViabilidadeVendedorVisualiza;
                retorno.ConsultaPendenciaEnderecoVendedorVisualiza = config.ConsultaPendenciaEnderecoVendedorVisualiza;
                retorno.ConsultaPendenciaFinanceiraVendedorVisualiza = config.ConsultaPendenciaFinanceiraVendedorVisualiza;
                retorno.ConsultaCreditoVendedorVisualiza = config.ConsultaCreditoVendedorVisualiza;

                retorno.TermosAceite = (await repoDocAceite.select(p => p.Venda.Id == venda.Id)).ToList();

                #endregion

                return retorno;
            }
        }
        #endregion

        public PlanoProdutoListModel GetPlanoProdutoListModel(Cliente cliente, Cliente_PlanoProduto planoProduto)
        {
            #region [GetPlanoProdutoListModel]
            PlanoProdutoListModel planoProdutoModel = new PlanoProdutoListModel();

            planoProdutoModel.Id_Modelo_Contrato = planoProduto?.ClienteContrato?.ModeloContrato?.Id;
            planoProdutoModel.Id_Contrato = planoProduto?.ClienteContrato?.Id;
            planoProdutoModel.Descricao_Contrato = planoProduto?.ClienteContrato?.Descricao;
            planoProdutoModel.Numero_Contrato = planoProduto?.ClienteContrato?.NumeroContrato ?? Convert.ToString(planoProduto?.ClienteContrato?.Id);

            //planoProdutoModel.Id_Venda = venda?.Id;
            planoProdutoModel.Id_Vendedor = (long?)null;
            planoProdutoModel.Id_Supervisor = (long?)null;

            planoProdutoModel.Status_Venda = Status_Venda.Analise_Cliente;

            //planoProdutoModel.Desc_Status_Venda = venda != null ? HtmlEnumExtensions.GetEnumDescription<Status_Venda>(venda.Status) : null;
            planoProdutoModel.Nome_Vendedor = String.Empty;
            planoProdutoModel.Nome_Supervisor = String.Empty;

            planoProdutoModel.Id = planoProduto.Id;

            #region [Regra para multa de rescisão]

            var obj_modelo_contrato = planoProduto?.ClienteContrato?.ModeloContrato;
            Double periodo_fidelidade = obj_modelo_contrato.Tem_Carencia ? Convert.ToDouble(obj_modelo_contrato.Periodo_Carencia) : 0;
            Double multa_fidelidade = obj_modelo_contrato.MultaFidelidade ? Convert.ToDouble(obj_modelo_contrato.Multa_Carencia) : 0;

            DateTime dataFimCarencia = planoProduto.Data_Habilitado != null ? Convert.ToDateTime(planoProduto.Data_Habilitado) : new DateTime();
            int? periodoCarencia = planoProduto?.ClienteContrato?.PeriodoCarencia > 0 ? Convert.ToInt32(planoProduto?.ClienteContrato?.PeriodoCarencia) : (int?)null;
            DateTime fimCarencia = periodoCarencia.HasValue ? dataFimCarencia.AddMonths(periodoCarencia.Value) : dataFimCarencia;

            int anos = fimCarencia.Year - DateTime.Now.Year;
            int meses = fimCarencia.Month - DateTime.Now.Month;

            int totalMeses = anos * 12 + meses;

            // Se o dia final for menor que o dia inicial, desconta um mês
            if (fimCarencia.Day < DateTime.Now.Day)
            {
                totalMeses--;
            }

            var tempo_restante = totalMeses;

            String multa_pro_rata = obj_modelo_contrato.Tem_Pro_Rata ? String.Format("{0:N2}", Math.Round((multa_fidelidade / periodo_fidelidade) * tempo_restante)) : String.Format("{0:N2}", multa_fidelidade);

            planoProdutoModel.Multa_Carencia_Reais = multa_pro_rata;

            #endregion

            planoProdutoModel.ValorPlanoProduto = Convert.ToString(planoProduto?.Valor_Real);
            planoProdutoModel.ValorInstalacao_Final = Convert.ToString(planoProduto?.Valor_Instalacao_Final);
            planoProdutoModel.ValorPlanoProduto_Final = Convert.ToString(planoProduto?.Valor_Final);

            planoProdutoModel.ValorInstalacao = Convert.ToString(planoProduto?.Valor_Instalacao_Real);

            planoProdutoModel.ValorTotal = Convert.ToString(planoProduto?.Valor_Final_Total);
            planoProdutoModel.Qtde_Dias = planoProduto?.Dias_Teste_Gratis == null ? 0 : planoProduto.Dias_Teste_Gratis;
            planoProdutoModel.Meses_Carencia = planoProduto?.Meses_Carencia;

            planoProdutoModel.Status_PlanoProduto = planoProduto?.Status;
            planoProdutoModel.Status_PlanoProduto_Str = planoProduto.Status != null ? HtmlEnumExtensions.GetEnumDescription(planoProduto.Status.Value) : String.Empty;

            planoProdutoModel.Numero_Parcelas = planoProduto?.Parcelas_Instalacao == null ? 0 : planoProduto.Parcelas_Instalacao;
            planoProdutoModel.End_Instalacao_Igual_Principal = cliente.End_Instalacao_Igual_Principal;
            //planoProdutoModel.End_Instalacao_Igual_Prospecto = cliente.End_Instalacao_Igual_Prospecto;

            planoProdutoModel.Teste_Gratis = planoProduto?.Dias_Teste_Gratis > 0;
            planoProdutoModel.Periodo_Carencia = planoProduto?.Periodo_Carencia == null ? false : planoProduto.Periodo_Carencia;
            planoProdutoModel.Desc_Periodo_Carencia = planoProduto?.Periodo_Carencia == null ? "Não" : "Sim";
            planoProdutoModel.Login_Radius = planoProduto?.Login_Radius;
            planoProdutoModel.Senha_Radius = planoProduto?.Senha_Radius;
            planoProdutoModel.IdIpPool = planoProduto.Pool_Ip?.Id;
            planoProdutoModel.Id_Endereco = planoProduto.Endereco_Instalacao != null ? planoProduto.Endereco_Instalacao.Id : 0;

            planoProdutoModel.Id_Bairro = planoProduto?.Endereco_Instalacao?.Id_Bairro;
            planoProdutoModel.Bairro = planoProduto?.Endereco_Instalacao?.Bairro;
            planoProdutoModel.CEP = planoProduto?.Endereco_Instalacao?.Cep;
            planoProdutoModel.Id_Cidade = planoProduto?.Endereco_Instalacao?.Id_Cidade;
            planoProdutoModel.Cidade = planoProduto?.Endereco_Instalacao?.Cidade;
            planoProdutoModel.Ponto_Referencia = planoProduto?.Endereco_Instalacao?.Ponto_Referencia;

            planoProdutoModel.Complemento = planoProduto?.Endereco_Instalacao?.Complemento;
            planoProdutoModel.Logradouro = planoProduto?.Endereco_Instalacao?.Logradouro;
            planoProdutoModel.Numero = planoProduto?.Endereco_Instalacao?.Numero;
            planoProdutoModel.UF = planoProduto?.Endereco_Instalacao?.UF;

            planoProdutoModel.Latitude = Convert.ToString(planoProduto.Endereco_Instalacao?.Latitude);
            planoProdutoModel.Longitude = Convert.ToString(planoProduto.Endereco_Instalacao?.Longitude);
            planoProdutoModel.Id_Pais = planoProduto?.Endereco_Instalacao?.Pais?.Id;
            planoProdutoModel.Id_Tipo_Imovel = planoProduto?.Endereco_Instalacao?.Tipo_Imovel?.Cod_Tipo_Imovel;

            planoProdutoModel.Cod_Escritorio = planoProduto?.Cod_Escritorio;
            planoProdutoModel.Cod_Area = planoProduto?.Cod_Area;
            planoProdutoModel.Cod_Localidade = planoProduto?.Cod_Localidade;
            planoProdutoModel.Localidade = planoProduto?.Localidade;
            planoProdutoModel.Localidade_Anatel = planoProduto?.Localidade_Anatel;

            planoProdutoModel.Id_Forma_De_Pagamento = planoProduto?.Forma_Pagamento_Cliente?.Forma_Pagamento?.Id;
            planoProdutoModel.Id_Forma_Pagamento_Cliente = planoProduto?.Forma_Pagamento_Cliente?.Id;

            planoProdutoModel.Desc_Forma_Pagto = !String.IsNullOrEmpty(planoProduto?.Forma_Pagamento_Cliente?.Forma_Pagamento?.Descricao) ? planoProduto.Forma_Pagamento_Cliente?.Forma_Pagamento?.Descricao : "Sem Forma de Pagamento";

            planoProdutoModel.Id_Troca_Plano = planoProduto?.Id_TrocaPlano != null ? planoProduto.Id_TrocaPlano : (long?)null;
            planoProdutoModel.FlagTrocaPlano = planoProduto.FlagTrocaPlano;
            planoProdutoModel.CobrarInstalacaoEntrada = planoProduto?.CobrarInstalacaoEntrada != null ? planoProduto.CobrarInstalacaoEntrada : false;

            if (planoProduto.Plano != null)
            {
                planoProdutoModel.Tipo_Tecnologia = planoProduto?.Plano?.Tipo_Tecnologia;
                planoProdutoModel.Id_Plano_Produto = planoProduto?.Plano.Id;
                planoProdutoModel.Descricao = planoProduto?.Plano.Descricao;
                planoProdutoModel.Tipo = PlanoProdutoEnum.Plano;

                if (planoProduto.Plano.StatusInicial == Status_PlanoProduto.Aguardando_Criacao_OS || planoProduto.Plano.OS_Mudanca_Plano)
                {
                    planoProdutoModel.NecessitaCriarOS = Resources.RsBase.SIM;
                }
                else
                {
                    planoProdutoModel.NecessitaCriarOS = Resources.RsBase.NAO;
                }
            }
            else if (planoProduto.Produto != null)
            {
                planoProdutoModel.Id_Plano_Produto = planoProduto?.Produto?.Id;
                planoProdutoModel.Descricao = planoProduto?.Produto?.Descricao;
                planoProdutoModel.Tipo = PlanoProdutoEnum.Produto;

                if (planoProduto.Produto.Status_Inicial == Status_PlanoProduto.Aguardando_Criacao_OS || planoProduto.Plano.OS_Mudanca_Plano)
                {
                    planoProdutoModel.NecessitaCriarOS = Resources.RsBase.SIM;
                }
                else
                {
                    planoProdutoModel.NecessitaCriarOS = Resources.RsBase.NAO;
                }
            }
            else if (planoProduto.Combo != null)
            {
                planoProdutoModel.Id_Plano_Produto = planoProduto?.Combo?.Id;
                planoProdutoModel.Descricao = planoProduto?.Combo?.Descricao;
                planoProdutoModel.Tipo = null;

                if (planoProduto.Combo.PlanosProdutos.Any(x => x?.Plano?.Tipo_Tecnologia ==
                                                                Tipo_Tecnologia.Internet))
                {
                    planoProdutoModel.Tipo_Tecnologia = Tipo_Tecnologia.Internet;
                }

                // TODO: Descobrir como informar se a venda de um combo necessita de OS criada
                planoProdutoModel.NecessitaCriarOS = Resources.RsBase.SIM;
            }
            return planoProdutoModel;
            #endregion
        }
        public PlanoProdutoListModel GetPlanoProdutoListModelByVenda(Cliente_PlanoProduto planoProduto, Venda venda)
        {
            #region [GetPlanoProdutoListModelByVenda]
            PlanoProdutoListModel planoProdutoModel = new PlanoProdutoListModel();

            planoProdutoModel.Id_Modelo_Contrato = planoProduto?.ClienteContrato?.ModeloContrato?.Id;
            planoProdutoModel.Id_Contrato = planoProduto?.ClienteContrato?.Id;
            planoProdutoModel.Descricao_Contrato = planoProduto?.ClienteContrato?.Descricao;
            planoProdutoModel.Numero_Contrato = planoProduto?.ClienteContrato?.NumeroContrato ?? Convert.ToString(planoProduto?.ClienteContrato?.Id);

            planoProdutoModel.Id_Venda = venda?.Id;
            planoProdutoModel.Id_Vendedor = venda?.Vendedor?.Id;
            planoProdutoModel.Id_Supervisor = venda?.Supervisor?.Id;

            planoProdutoModel.Status_Venda = venda?.Status;

            planoProdutoModel.Desc_Status_Venda = venda != null ? HtmlEnumExtensions.GetEnumDescription<Status_Venda>(venda.Status) : null;
            planoProdutoModel.Nome_Vendedor = venda?.Vendedor?.Nome;
            planoProdutoModel.Nome_Supervisor = venda?.Supervisor?.Nome;

            planoProdutoModel.Id = planoProduto == null ? 0 : planoProduto.Id;

            planoProdutoModel.ValorPlanoProduto = Convert.ToString(planoProduto?.Valor_Real);
            planoProdutoModel.ValorInstalacao_Final = Convert.ToString(planoProduto?.Valor_Instalacao_Final);
            planoProdutoModel.ValorPlanoProduto_Final = Convert.ToString(planoProduto?.Valor_Final);

            planoProdutoModel.ValorInstalacao = Convert.ToString(planoProduto?.Valor_Instalacao_Real);

            #region [Regra para multa de rescisão]

            var obj_modelo_contrato = planoProduto?.ClienteContrato?.ModeloContrato;
            Double periodo_fidelidade = obj_modelo_contrato != null && obj_modelo_contrato.Tem_Carencia ? Convert.ToDouble(obj_modelo_contrato.Periodo_Carencia) : 0;
            Double multa_fidelidade = obj_modelo_contrato != null && obj_modelo_contrato.MultaFidelidade ? Convert.ToDouble(obj_modelo_contrato.Multa_Carencia) : 0;

            DateTime dataFimCarencia = planoProduto?.Data_Habilitado != null ? Convert.ToDateTime(planoProduto.Data_Habilitado) : new DateTime();
            int? periodoCarencia = planoProduto?.ClienteContrato?.PeriodoCarencia > 0 ? Convert.ToInt32(planoProduto?.ClienteContrato?.PeriodoCarencia) : (int?)null;
            DateTime fimCarencia = periodoCarencia.HasValue ? dataFimCarencia.AddMonths(periodoCarencia.Value) : dataFimCarencia;

            int anos = fimCarencia.Year - DateTime.Now.Year;
            int meses = fimCarencia.Month - DateTime.Now.Month;

            int totalMeses = anos * 12 + meses;

            // Se o dia final for menor que o dia inicial, desconta um mês
            if (fimCarencia.Day < DateTime.Now.Day)
            {
                totalMeses--;
            }

            var tempo_restante = totalMeses;

            String multa_pro_rata = obj_modelo_contrato != null && obj_modelo_contrato.Tem_Pro_Rata ? String.Format("{0:N2}", Math.Round((multa_fidelidade / periodo_fidelidade) * tempo_restante)) : String.Format("{0:N2}", multa_fidelidade);

            planoProdutoModel.Multa_Carencia_Reais = multa_pro_rata;

            #endregion

            planoProdutoModel.ValorTotal = Convert.ToString(planoProduto?.Valor_Final_Total);
            planoProdutoModel.Qtde_Dias = planoProduto == null ? 0 : planoProduto?.Dias_Teste_Gratis;
            planoProdutoModel.Meses_Carencia = planoProduto?.Meses_Carencia;

            planoProdutoModel.Status_PlanoProduto = planoProduto?.Status;
            planoProdutoModel.Status_PlanoProduto_Str = planoProduto != null ? HtmlEnumExtensions.GetEnumDescription(planoProduto.Status.Value) : String.Empty;

            planoProdutoModel.Numero_Parcelas = planoProduto == null ? 0 : planoProduto.Parcelas_Instalacao;
            planoProdutoModel.End_Instalacao_Igual_Principal = venda.End_Instalacao_Igual_Principal;
            planoProdutoModel.End_Instalacao_Igual_Prospecto = venda.End_Instalacao_Igual_Prospecto;

            if (planoProduto != null)
            {
                planoProdutoModel.Teste_Gratis = planoProduto.Dias_Teste_Gratis > 0;
                planoProdutoModel.Periodo_Carencia = planoProduto.Periodo_Carencia;
                planoProdutoModel.Desc_Periodo_Carencia = !planoProduto.Periodo_Carencia ? "Não" : "Sim";

                planoProdutoModel.Login_Radius = planoProduto.Login_Radius;
                planoProdutoModel.Senha_Radius = planoProduto.Senha_Radius;
                planoProdutoModel.IdIpPool = planoProduto.Pool_Ip?.Id;
                planoProdutoModel.Id_Endereco = planoProduto.Endereco_Instalacao != null ? planoProduto.Endereco_Instalacao.Id : 0;

                planoProdutoModel.Id_Bairro = planoProduto.Endereco_Instalacao?.Id_Bairro;
                planoProdutoModel.Bairro = planoProduto.Endereco_Instalacao?.Bairro;
                planoProdutoModel.CEP = planoProduto.Endereco_Instalacao?.Cep;
                planoProdutoModel.Id_Cidade = planoProduto.Endereco_Instalacao?.Id_Cidade;
                planoProdutoModel.Cidade = planoProduto.Endereco_Instalacao?.Cidade;
                planoProdutoModel.Ponto_Referencia = planoProduto.Endereco_Instalacao?.Ponto_Referencia;

                planoProdutoModel.Complemento = planoProduto.Endereco_Instalacao?.Complemento;
                planoProdutoModel.Logradouro = planoProduto.Endereco_Instalacao?.Logradouro;
                planoProdutoModel.Numero = planoProduto.Endereco_Instalacao?.Numero;
                planoProdutoModel.UF = planoProduto.Endereco_Instalacao?.UF;

                planoProdutoModel.Latitude = Convert.ToString(planoProduto.Endereco_Instalacao?.Latitude);
                planoProdutoModel.Longitude = Convert.ToString(planoProduto.Endereco_Instalacao?.Longitude);
                planoProdutoModel.Id_Pais = planoProduto.Endereco_Instalacao?.Pais?.Id;
                planoProdutoModel.Id_Tipo_Imovel = planoProduto.Endereco_Instalacao?.Tipo_Imovel?.Cod_Tipo_Imovel;

                planoProdutoModel.Cod_Escritorio = planoProduto.Cod_Escritorio;
                planoProdutoModel.Cod_Area = planoProduto.Cod_Area;
                planoProdutoModel.Cod_Localidade = planoProduto.Cod_Localidade;
                planoProdutoModel.Localidade = planoProduto.Localidade;
                planoProdutoModel.Localidade_Anatel = planoProduto.Localidade_Anatel;

                planoProdutoModel.Id_Forma_De_Pagamento = planoProduto.Forma_Pagamento_Cliente?.Forma_Pagamento?.Id;
                planoProdutoModel.Id_Forma_Pagamento_Cliente = planoProduto.Forma_Pagamento_Cliente?.Id;

                planoProdutoModel.Desc_Forma_Pagto = !String.IsNullOrEmpty(planoProduto.Forma_Pagamento_Cliente?.Forma_Pagamento?.Descricao) ? planoProduto.Forma_Pagamento_Cliente?.Forma_Pagamento?.Descricao : "Sem Forma de Pagamento";

                planoProdutoModel.Id_Troca_Plano = planoProduto.Id_TrocaPlano != null ? planoProduto.Id_TrocaPlano : (long?)null;
                planoProdutoModel.FlagTrocaPlano = planoProduto.FlagTrocaPlano;
                planoProdutoModel.CobrarInstalacaoEntrada = planoProduto.CobrarInstalacaoEntrada != null ? planoProduto.CobrarInstalacaoEntrada : false;

                if (planoProduto.Plano != null)
                {
                    planoProdutoModel.Tipo_Tecnologia = planoProduto?.Plano?.Tipo_Tecnologia;
                    planoProdutoModel.Id_Plano_Produto = planoProduto.Plano.Id;
                    planoProdutoModel.Descricao = planoProduto.Plano.Descricao;
                    planoProdutoModel.Tipo = PlanoProdutoEnum.Plano;

                    if (planoProduto.Plano.StatusInicial == Status_PlanoProduto.Aguardando_Criacao_OS)
                    {
                        planoProdutoModel.NecessitaCriarOS = Resources.RsBase.SIM;
                    }
                    else
                    {
                        planoProdutoModel.NecessitaCriarOS = Resources.RsBase.NAO;
                    }
                }
                else if (planoProduto.Produto != null)
                {
                    planoProdutoModel.Id_Plano_Produto = planoProduto.Produto.Id;
                    planoProdutoModel.Descricao = planoProduto.Produto.Descricao;
                    planoProdutoModel.Tipo = PlanoProdutoEnum.Produto;

                    if (planoProduto.Produto.Status_Inicial == Status_PlanoProduto.Aguardando_Criacao_OS)
                    {
                        planoProdutoModel.NecessitaCriarOS = Resources.RsBase.SIM;
                    }
                    else
                    {
                        planoProdutoModel.NecessitaCriarOS = Resources.RsBase.NAO;
                    }
                }
                else if (planoProduto.Combo != null)
                {
                    planoProdutoModel.Id_Plano_Produto = planoProduto.Combo.Id;
                    planoProdutoModel.Descricao = planoProduto.Combo.Descricao;
                    planoProdutoModel.Tipo = null;

                    if (planoProduto.Combo.PlanosProdutos.Any(x => x?.Plano?.Tipo_Tecnologia ==
                                                                    Tipo_Tecnologia.Internet))
                    {
                        planoProdutoModel.Tipo_Tecnologia = Tipo_Tecnologia.Internet;
                    }

                    // TODO: Descobrir como informar se a venda de um combo necessita de OS criada
                    planoProdutoModel.NecessitaCriarOS = Resources.RsBase.SIM;
                }
            }
            return planoProdutoModel;
            #endregion
        }

        public async Task<PostApiRetorno<long>> EnviarEmailChecklist(long IdVenda)
        {
            #region [EnviarEmailCheclist]
            var retorno = new PostApiRetorno<long>();

            using (ISession session = HibernateUtil.GetSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                try
                {
                    Venda venda = null;
                    Cliente cliente = null;

                    using (var repoVenda = new Conexao<Venda>(session))
                    using (var repoCliente = new Conexao<Cliente>(session))
                    using (var repoCanal = new Conexao<Canal_Comunicacao>(session))
                    using (var repoConfCanal = new Conexao<Configuracao_Canal_Comunicacao>(session))
                    using (var repoConfig = new Conexao<Configuracoes_Gerais>(session))
                    {
                        venda = await repoVenda.selectFirst(p => p.Id == IdVenda);
                        cliente = await repoCliente.selectFirst(p => p.Id == venda.Id_Cliente.Value);
                        var listaPlanosProdutos = cliente.PlanosProdutos.Where(p => p.Id_Venda.HasValue && p.Id_Venda.Value == venda.Id);

                        if (String.IsNullOrEmpty(cliente.Email_Principal))
                        {
                            throw new Exception("Cliente não possui e-mail vinculado");
                        }

                        Configuracoes_Gerais config = await repoConfig.selectFirst();

                        if (venda?.Vendedor?.Filial?.Empresa?.CodEmpresa == null)
                        {
                            throw new Exception("Não foi possível encontrar os dados da empresa para inserir no e-mail do checklist");
                        }

                        InfoEmpresaTermoAdesao infoEmpresa = GetInfoEmpresa(Convert.ToString(venda.Vendedor.Filial.Empresa.CodEmpresa));

                        if (infoEmpresa == null)
                        {
                            throw new Exception("Não foi possível encontrar os dados da empresa para inserir no e-mail do checklist");
                        }

                        EmailUtil emailService = new EmailUtil(CONSTANTES.Email_De, CONSTANTES.Email_Senha, CONSTANTES.Email_Smtp, CONSTANTES.Email_Porta, CONSTANTES.Email_Usuario);
                        String urlChecklistVenda = $@"{CONSTANTES.URLPortal}/pt/Venda/EmailChecklist?Id_Venda={venda.Id}&Email=S";
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
                                             .Replace("{{VALOR}}", planoProduto.Valor_Final != null ? planoProduto.Valor_Final?.ToString("C", new CultureInfo("pt-BR")) : "R$ 0,00");

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

                        using (System.IO.StreamReader stream = System.IO.File.OpenText($@"{CONSTANTES.CaminhoFisico}\Arquivos\email_checklist_venda.html"))
                        {
                            String arquivo_email = stream.ReadToEnd();
                            String arquivo_modificado = arquivo_email.Replace("{{NOME_CLIENTE}}", cliente.NomeFantasia)
                                                                     .Replace("{{URL_CHECKLIST_VENDA}}", urlChecklistVenda)
                                                                     .Replace("{{URL_LOGO}}", config.OLA_URL_Logo)
                                                                     .Replace("{{PRODUTOS}}", produtos);

                            emailService.EnviarMensagem(cliente.Email_Principal, $@"Bem-vindo(a) à {infoEmpresa.Nome}! Segue Checklist de documentos para validação.", arquivo_modificado, CONSTANTES.Email_UsaSsl == "S");
                        }
                    }

                    retorno.IdObjeto = venda.Id;
                    retorno.Ok = true;

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    retorno.IdObjeto = 0;

                    retorno.Ok = false;
                    retorno.Mensagem = ex.Message;

                    if (ex.InnerException != null)
                        retorno.InnerException = ex.InnerException.Message;
                }
            }

            return retorno;
            #endregion
        }
        private InfoEmpresaTermoAdesao GetInfoEmpresa(String codEmpresa)
        {
            #region [GetInfoEmpresa]
            InfoEmpresaTermoAdesao retorno = null;

            StringBuilder sql = new StringBuilder();
            sql.AppendLine("SELECT RAZAO_SOCIAL, EMPRESA, CNPJ, ENDERECO, BAIRRO, CIDADE, UF, LOGOTIPO ");
            sql.AppendLine("FROM TB_EMPRESA");
            sql.AppendLine("WHERE CODEMPRESA = :pCOD_EMPRESA");

            using (ISession session = HibernateUtil.GetSessionT5())
            using (Query query = new Query(session))
            {
                query.setParametro("pCOD_EMPRESA", codEmpresa);
                var values = query.Values(sql.ToString()).Result;

                if (values.Count > 0)
                    retorno = new InfoEmpresaTermoAdesao
                    {
                        Nome = Convert.ToString(values[0]["EMPRESA"]),
                        RazaoSocial = Convert.ToString(values[0]["RAZAO_SOCIAL"]),
                        Cnpj = Convert.ToString(values[0]["CNPJ"]),
                        Endereco = Convert.ToString(values[0]["ENDERECO"]),
                        Bairro = Convert.ToString(values[0]["BAIRRO"]),
                        Cidade = Convert.ToString(values[0]["CIDADE"]),
                        Estado = Convert.ToString(values[0]["UF"]),
                        Logo = (byte[])values[0]["LOGOTIPO"]
                    };
            }

            return retorno;
            #endregion
        }
    }
}
