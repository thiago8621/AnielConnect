using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.ModelLog.nhibernate;
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
using Aniel.Connect.Repository.Departamento;
using Aniel.Connect.Repository.EnderecoCliente;
using Aniel.Connect.Repository.EnderecoLogradouro;
using Aniel.Connect.Repository.FormaPagamento;
using Aniel.Connect.Repository.Funcionario;
using Aniel.Connect.Repository.MeioCaptacao;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.ModeloContrato;
using Aniel.Connect.Repository.Pais;
using Aniel.Connect.Repository.Produto;
using Aniel.Connect.Repository.Repository.ComboPlanoProduto;
using Aniel.Connect.Repository.Repository.Pool_Ip;
using Aniel.Connect.Repository.TipoImovel;
using Aniel.Connect.Repository.Uf;
using Aniel.Connect.Repository.Usuario;
using Aniel.Connect.Repository.Vencimento;
using Aniel.Connect.Repository.Venda;
using NHibernate;
using Sinapse.DAO.repository;
using Aniel.Connect.Repository.Workflow;
using Aniel.Connect.Repository.Workflow;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.ConnectBusiness.Connect
{
    public class VendaBusiness
    {
        private readonly IVendaRepository _repository;
        private readonly IFuncionarioRepository _repoFuncionario;
        private readonly IDepartamentoRepository _repoDepto;

        private readonly IConfiguracoesGeraisRepository _repoConfig;
        private readonly IUsuarioRepository _repoUsuario;

        private readonly IClienteRepository _repoCliente;
        private readonly IEnderecoClienteRepository _repoEnderecoCliente;
        private readonly ICategoriaClienteRepository _repoCategoriaCliente;
        private readonly IMeioCaptacaoRepository _repoMeioCaptacao;

        private readonly IPaisRepository _repoPais;
        private readonly IUfRepository _repoUf;
        private readonly ICidadeRepository _repoCidade;
        private readonly IBairroRepository _repoBairro;

        private readonly ITipoImovelRepository _repoTipoImovel;

        private readonly IEnderecoLogradouroRepository _repoEnderecoLogradouro;
        private readonly ICondominioPredioRepository _repoCondominioPredio;
        private readonly IBancoRepository _repoBanco;
        private readonly IVencimentoRepository _repoVencimento;
        private readonly IFormaPagamentoRepository _repoFormaPagamento;

        private readonly ICampanhaRepository _repoCampanha;
        private readonly IComboRepository _repoCombo;
        private readonly IPlanoRepository _repoPlano;
        private readonly IProdutoRepository _repoProduto;
        private readonly IPool_IpRepository _repoPoolIP;
        private readonly IModeloContratoRepository _repoModeloContrato;
        private readonly IClienteContratoRepository _repoClienteContrato;
        private readonly IClientePlanoProdutoRepository _repoClientePlanoProduto;
        private readonly IComboPlanoProdutoRepository _comboPlanoProdutoRepository;
        private readonly IWorkflowRepository _workflowRepository;

        public VendaBusiness(IVendaRepository repository)
        {
            this._repository = repository;
        }

        public VendaBusiness(IVendaRepository repository, IEnderecoClienteRepository repoEnderecoCliente)
        {
            this._repository = repository;
            this._repoEnderecoCliente = repoEnderecoCliente;
        }

        public VendaBusiness(
            IVendaRepository repository,
            IFuncionarioRepository repoFuncionario,
            IDepartamentoRepository repoDepto,

            IConfiguracoesGeraisRepository repoConfig,
            IUsuarioRepository repoUsuario,

            IClienteRepository repoCliente,
            IEnderecoClienteRepository repoEnderecoCliente,
            ICategoriaClienteRepository repoCategoriaCliente,
            IMeioCaptacaoRepository repoMeioCaptacao,

            IPaisRepository repoPais,
            IUfRepository repoUf,
            ICidadeRepository repoCidade,
            IBairroRepository repoBairro,

            ITipoImovelRepository repoTipoImovel,

            IEnderecoLogradouroRepository repoEnderecoLogradouro,
            ICondominioPredioRepository repoCondominioPredio,
            IBancoRepository repoBanco,
            IVencimentoRepository repoVencimento,
            IFormaPagamentoRepository repoFormaPagamento,

            IComboRepository repoCombo,
            ICampanhaRepository repoCampanha,
            IPlanoRepository repoPlano,
            IProdutoRepository repoProduto,
            IPool_IpRepository repoPoolIP,
            IModeloContratoRepository repoModeloContrato,
            IClienteContratoRepository repoClienteContrato,
            IClientePlanoProdutoRepository repoClientePlanoProduto,
            IComboPlanoProdutoRepository comboPlanoProdutoRepository,
            IWorkflowRepository _workflowRepository)
        {
            this._repository = repository;
            this._repoFuncionario = repoFuncionario;
            this._repoDepto = repoDepto;

            this._repoConfig = repoConfig;
            this._repoUsuario = repoUsuario;

            this._repoCliente = repoCliente;
            this._repoEnderecoCliente = repoEnderecoCliente;
            this._repoCategoriaCliente = repoCategoriaCliente;
            this._repoMeioCaptacao = repoMeioCaptacao;

            this._repoPais = repoPais;
            this._repoUf = repoUf;
            this._repoCidade = repoCidade;
            this._repoBairro = repoBairro;

            this._repoTipoImovel = repoTipoImovel;

            this._repoEnderecoLogradouro = repoEnderecoLogradouro;
            this._repoCondominioPredio = repoCondominioPredio;
            this._repoBanco = repoBanco;
            this._repoVencimento = repoVencimento;
            this._repoFormaPagamento = repoFormaPagamento;
            this._repoCombo = repoCombo;
            this._repoCampanha = repoCampanha;
            this._repoPlano = repoPlano;
            this._repoProduto = repoProduto;
            this._repoPoolIP = repoPoolIP;
            this._repoModeloContrato = repoModeloContrato;
            this._repoClienteContrato = repoClienteContrato;
            this._repoClientePlanoProduto = repoClientePlanoProduto;
            this._comboPlanoProdutoRepository = comboPlanoProdutoRepository;
            this._workflowRepository = _workflowRepository;
        }

        private Cliente GravarDadosCliente(Cliente cliente, VendaDadosGeraisViewModel view, string usuario)
        {
            IdentityUser usuarioLogado = this._repoUsuario.GetById(usuario);

            if (view.Id_Cliente == 0)
            {
                cliente.UsuarioAlteracao = usuario;
            }

            cliente.Status = Status_Cliente.Ativo;
            cliente.Nome_Resp_Financeiro = view.Representante;
            cliente.CpfCnpj = view.CpfCnpj;
            cliente.Cpf_Responsavel = view.CPFResponsavel;
            cliente.Nacionalidade = view.Nacionalidade;
            cliente.Sexo = view.Sexo;
            cliente.Data_Nascimento = !string.IsNullOrEmpty(view.DataNascimento)
                                      ? ParseDataComMensagem(view.DataNascimento)
                                      : (DateTime?)null;

            cliente.Data_Abertura_Empresa = !string.IsNullOrEmpty(view.Data_Abertura?.ToString())
                                            ? ParseDataComMensagem(view.Data_Abertura?.ToString("dd/MM/yyyy"))
                                            : (DateTime?)null;

            cliente.Estado_Civil = view.EstadoCivil;
            cliente.Rg = view.RG;
            cliente.Orgao_Emissao_Rg = view.Orgao;
            cliente.Data_Emissao_Rg = !string.IsNullOrEmpty(view.DataEmissao)
                                      ? ParseDataComMensagem(view.DataEmissao)
                                      : (DateTime?)null;

            cliente.Grau_Instrucao = view.Instrucao;
            cliente.Filiacao_Mae = view.Mae;
            cliente.Filiacao_Pai = view.Pai;
            cliente.Profissao = view.Profissao;

            cliente.Email_Principal = view.Email;
            cliente.Telefone_Fixo = view.TelefoneFixo;
            cliente.Telefone_Celular = view.TelefoneCelular;
            cliente.Telefone_Recado = view.TelefoneRecado;
            cliente.Pessoa_Tel_Recado = view.NomeRecado;

            cliente.Vendedor = usuarioLogado?.Funcionario;
            cliente.Supervisor = usuarioLogado?.Funcionario?.Supervisor;

            cliente.Categoria_Cliente =
                view.Id_Categoria_Cliente.HasValue ?
                    this._repoCategoriaCliente.GetById(view.Id_Categoria_Cliente.Value) :
                    null;

            cliente.Quem_Indicou =
                view.Id_Quem_Indicou.HasValue ?
                    this._repoCliente.GetById(view.Id_Quem_Indicou.Value) :
                    null;

            cliente.Dt_Ultimo_Contato = view.Ultimo_Contato;
            cliente.Tipo_Pessoa = view.TipoPessoa != null ? view.TipoPessoa : Tipo_Pessoa.Fisica;

            cliente.Qual_Meio =
                view.Id_Meio_Captacao.HasValue ?
                    this._repoMeioCaptacao.GetById(view.Id_Meio_Captacao.Value) :
                    null;

            if (view.Id == 0 && (cliente.Tipo_Cliente == Tipo_Cliente.Prospecto))
            {
                cliente.Status_Prospecto = Status_Prospecto.Em_Negociacao;
            }

            //long numeroContratoMax = this._repoCliente.GetMaxNumeroContrato() + 1;
            //if (!String.IsNullOrEmpty(view.Numero_Contrato))
            //{
            //    if (config.Identificacao_Cliente.Value == Identificacao_Cliente.Numero_Contrato)
            //    {
            //        cliente.Numero_Contrato = view.Numero_Contrato;
            //    }
            //    else if (view.Id == 0 || String.IsNullOrEmpty(cliente.Numero_Contrato))
            //    {
            //        cliente.Numero_Contrato = Convert.ToString(numeroContratoMax);
            //    }
            //    else
            //    {
            //        cliente.Numero_Contrato = view.Numero_Contrato;
            //    }
            //}
            //else if (cliente.Id == 0 || String.IsNullOrEmpty(cliente.Numero_Contrato))
            //{
            //    cliente.Numero_Contrato = Convert.ToString(numeroContratoMax);
            //}

            cliente.NomeFantasia = cliente.Tipo_Pessoa == Tipo_Pessoa.Fisica ? view.NomeCompleto : view.NomeFantasia;
            cliente.Outro_Meio = cliente.Qual_Meio?.Id != 0 ? Convert.ToString(cliente.Qual_Meio?.Id) : view.Qual_Meio;

            cliente.Inscricao_Estadual = view.Inscricao_Estadual?.Trim();
            cliente.Inscricao_Municipal = view.Inscricao_Municipal?.Trim();

            if (cliente.Id == 0)
            {
                cliente.Endereco_Principal.Id = 0;
            }

            this._repoCliente.Update(cliente);

            return cliente;
        }

        /// <summary>
        ///     Retorna informações de forma de pagamento do cliente 
        /// </summary>
        /// <param name="cliente">Objeto com as informações da forma de pagamento do cliente</param>
        /// <param name="formaPgtoView">Objeto com as informações da forma de pagamento que será gravada</param>
        /// <param name="vendaView">Objeto com as informações da venda para extrair o endereço de cobrança</param>
        public Forma_Pagamento_Cliente BindFormaPagamentoCliente(
            Cliente cliente,
            Forma_Pagamento_ClienteModels formaPgtoView,
            VendaDadosGeraisViewModel vendaView
        )
        {
            var formaPagamento = new Forma_Pagamento_Cliente();

            formaPagamento.DataHoraCriacao = DateTime.Now;
            formaPagamento.Cliente = cliente;
            formaPagamento.Cliente.Id = cliente.Id;
            formaPagamento.Agencia = formaPgtoView.Agencia;

            formaPagamento.Banco = formaPgtoView.Id_Banco.HasValue ? _repoBanco.GetBancoById(formaPgtoView.Id_Banco.Value) : null;
            formaPagamento.Conta_Corrente = formaPgtoView.Conta_Corrente;
            formaPagamento.Dia_Vencimento = formaPgtoView.Dia_Vencimento;
            formaPagamento.Digito = formaPgtoView.Digito;

            formaPagamento.Forma_Envio = formaPgtoView.Forma_Envio;

            formaPagamento.Forma_Pagamento = formaPgtoView.Id_Forma_Pagamento.HasValue
                ? _repoFormaPagamento.GetFormaPagamentoById(formaPgtoView.Id_Forma_Pagamento.Value) : null;


            formaPagamento.Vencimento = formaPgtoView.Id_Vencimento.HasValue ? _repoVencimento.GetByIdVencimento(formaPgtoView.Id_Vencimento.Value) : null;
            formaPagamento.Status = formaPgtoView.Status.HasValue ? formaPgtoView.Status.Value : Status_Default.Ativo;
            formaPagamento.Origem_Mobile = formaPgtoView.Origem_Mobile;
            formaPagamento.Forma_Envio = formaPgtoView.Forma_Envio;

            formaPagamento.End_Cobranca_Igual_Principal = formaPgtoView.End_Cobranca_Igual_Principal;

            formaPagamento.Endereco_Cobranca = this.BindEnderecoCobranca(vendaView);

            return formaPagamento;
        }

        /// <summary>
        /// Retorna informaçoes de venda 
        /// </summary>
        /// <param name="cliente">Objeto cliente venda</param>
        /// <param name="vendaView">Objeto de dados da view venda</param>
        /// <returns></returns>
        public async Task<Forma_Pagamento_Cliente> BindFormaPagamentoVenda(
            Cliente cliente,
            VendaDadosGeraisViewModel vendaView
        )
        {
            using (ISession session = HibernateUtil.GetSession())
            using (Conexao<Forma_Pagamento_Cliente> repoFormaPagamentoCliente = new Conexao<Forma_Pagamento_Cliente>(session))
            {

                var formaPagamento = repoFormaPagamentoCliente.selectFirst(p => p.Id == vendaView.Id_Forma_Pagamento).Result;
                if (vendaView.Id_Forma_Pagamento == null)
                {
                    formaPagamento = new Forma_Pagamento_Cliente();
                }

                formaPagamento.DataHoraCriacao = DateTime.Now;
                formaPagamento.Cliente = cliente;
                formaPagamento.Cliente.Id = cliente.Id;
                formaPagamento.Agencia = vendaView.Agencia;

                formaPagamento.Banco = vendaView.Id_Banco.HasValue ? _repoBanco.GetBancoById(vendaView.Id_Banco.Value) : null;
                formaPagamento.Conta_Corrente = vendaView.Conta_Corrente;
                formaPagamento.Digito = vendaView.Digito;

                formaPagamento.Forma_Envio = vendaView.Forma_Envio;

                var formaPagamentoSelecionada = vendaView.Id_Forma_Pagamento.HasValue
                    && vendaView.Id_Forma_Pagamento == vendaView.Formas_Pagamento.Where(p => p.Selecionado).FirstOrDefault()?.Id
                    ? vendaView.Id_Forma_Pagamento.Value : vendaView.Formas_Pagamento.Where(p => p.Selecionado).FirstOrDefault()?.Id;

                formaPagamento.Forma_Pagamento = formaPagamentoSelecionada.HasValue
                                               ? repoFormaPagamentoCliente.selectFirst(p => p.Id == formaPagamentoSelecionada.Value).Result.Forma_Pagamento : null;

                formaPagamento.Vencimento = vendaView.Id_Vencimento.HasValue ? _repoVencimento.GetByIdVencimento(vendaView.Id_Vencimento.Value) : null;

                formaPagamento.End_Cobranca_Igual_Principal = vendaView.End_Cobranca_Igual_Principal;

                formaPagamento.Endereco_Cobranca = this.BindEnderecoCobranca(vendaView);

                await repoFormaPagamentoCliente.SaveOrUpdate(formaPagamento);

                return formaPagamento;
            }
        }


        /// <summary>
        /// Retorna informações de plano produto preenchidos para a proposta de venda
        /// </summary>
        /// <param name="venda">Objeto com as informações dos dados gerais que serão gravadas</param>
        /// <param name="cliente">Objeto com as informações do cliente do plano produto</param>
        /// <param name="modeloContrato">Objeto modelo de contrato</param>
        /// <param name="planoProduto">lista de plano produto</param>
        /// <returns></returns>
        public Cliente_PlanoProduto BindPlanoProduto(Cliente cliente, Venda venda, PlanoProdutoListModel planoProduto, Boolean planoUtilizaVendaAgrupada = false)
        {
            Configuracoes_Gerais config = null;

            Modelo_Contrato modelo = planoProduto.Id_Modelo_Contrato.HasValue ?
                                         _repoModeloContrato.GetById(planoProduto.Id_Modelo_Contrato.Value) :
                                         _repoClienteContrato.GetById(Convert.ToInt64(planoProduto.Numero_Contrato)).ModeloContrato;


            Plano plano = null;
            switch (planoProduto.Tipo)
            {
                case PlanoProdutoEnum.Plano:
                    if (!planoProduto.Id_Combo.HasValue)
                        plano = _repoPlano.GetById(Convert.ToInt64(planoProduto.Id_Plano_Produto));
                    break;
                case PlanoProdutoEnum.Produto:
                    plano = null;
                    break;
                default:
                    if (planoProduto.Id_Combo.HasValue)
                        plano = _comboPlanoProdutoRepository.GetById(Convert.ToInt64(planoProduto.Id_Combo))?.Plano;
                    break;
            }
            var clientePlanoProduto = new Cliente_PlanoProduto();
            if (planoProduto.Id > 0 && planoUtilizaVendaAgrupada)
            {
                clientePlanoProduto = _repoClientePlanoProduto.GetById(planoProduto.Id);
                clientePlanoProduto.DataHoraAlteracao = DateTime.Now;
                clientePlanoProduto.Endereco_Instalacao = venda.EnderecoInstalacao;
                clientePlanoProduto.Cod_Escritorio = planoProduto.Cod_Escritorio;
                clientePlanoProduto.Cod_Area = planoProduto.Cod_Area;
                clientePlanoProduto.Localidade = planoProduto.Localidade;
                clientePlanoProduto.Cod_Localidade = planoProduto.Cod_Localidade;
                clientePlanoProduto.Dias_Teste_Gratis = planoProduto.Qtde_Dias;
                clientePlanoProduto.Parcelas_Instalacao = planoProduto.Numero_Parcelas;
                clientePlanoProduto.Periodo_Carencia = planoProduto.Periodo_Carencia;
                clientePlanoProduto.Meses_Carencia = planoProduto.Meses_Carencia;
                clientePlanoProduto.Campanha = planoProduto.Id_Campanha.HasValue ? _repoCampanha.GetById(planoProduto.Id_Campanha.Value) : null;
                clientePlanoProduto.Plano = plano;
                clientePlanoProduto.Produto = (!planoProduto.Id_Combo.HasValue && planoProduto.Tipo == PlanoProdutoEnum.Produto) ? _repoProduto.GetById((long)planoProduto.Id_Plano_Produto) : null;
                clientePlanoProduto.Combo = planoProduto.Id_Combo.HasValue ? _repoCombo.GetById((long)planoProduto.Id_Combo) : null;
                clientePlanoProduto.Multa_Carencia_Reais = modelo?.Multa_Carencia ?? 0;
                clientePlanoProduto.CancelaContratoFimCarencia = modelo?.Cancela_Contrato_Automatico_Final_Carencia ?? false;
                clientePlanoProduto.Tem_Reajuste = modelo?.Tem_Reajuste ?? false;
                clientePlanoProduto.Frequencia_Reajuste = modelo?.Frequencia_Reajuste ?? null;
                clientePlanoProduto.Renovar_CarenciaAuto = modelo?.Renova_Carencia_Automaticamente ?? null;
                clientePlanoProduto.Data_Habilitado = DateTime.Now;
                clientePlanoProduto.End_Instalacao_Igual_Principal = venda.End_Instalacao_Igual_Principal;
                clientePlanoProduto.Login_Radius = planoProduto.Login_Radius;
                clientePlanoProduto.Senha_Radius = planoProduto.Senha_Radius;
                clientePlanoProduto.Pool_Ip = planoProduto.IdIpPool.HasValue ? _repoPoolIP.GetById((long)planoProduto.IdIpPool) : null;
                clientePlanoProduto.Valor_Final = !String.IsNullOrEmpty(planoProduto.ValorPlanoProduto_Final) ? Convert.ToDouble(planoProduto.ValorPlanoProduto_Final) : 0;
                clientePlanoProduto.Valor_Final_Total = !String.IsNullOrEmpty(planoProduto.ValorTotal) ? Convert.ToDouble(planoProduto.ValorTotal) : 0;
                clientePlanoProduto.Valor_Real = !String.IsNullOrEmpty(planoProduto.ValorPlanoProduto) ? Convert.ToDouble(planoProduto.ValorPlanoProduto) : 0;
                clientePlanoProduto.Valor_Instalacao_Final = !String.IsNullOrEmpty(planoProduto.ValorInstalacao_Final) ? Convert.ToDouble(planoProduto.ValorInstalacao_Final) : 0;
                clientePlanoProduto.Valor_Instalacao_Real = !String.IsNullOrEmpty(planoProduto.ValorInstalacao) ? Convert.ToDouble(planoProduto.ValorInstalacao) : 0;
                clientePlanoProduto.Cliente = cliente;
                clientePlanoProduto.Id_Venda = venda.Id;
                clientePlanoProduto.Id_TrocaPlano = planoProduto?.Id_PlanoMudanca > 0 ? planoProduto?.Id_PlanoMudanca : planoProduto?.Id_MudancaPlano;
                clientePlanoProduto.JsonTelefonia = planoProduto.JsonTelefonia;
                clientePlanoProduto.Status = Status_PlanoProduto.Aguardando_Aprovacao_Venda;
                clientePlanoProduto.BaixadoInstalacao = false;
                clientePlanoProduto.ContaReceberInstalacao = false;
                clientePlanoProduto.UtilizaVendaAgroup = planoProduto.UtilizaVendaAgroup;
                clientePlanoProduto.isPlanoAdicional = planoProduto.isPlanoAdicional;
                clientePlanoProduto.possuiPlanosAdicionais = planoProduto.possuiPlanosAdicionais;
                clientePlanoProduto.Id_Plano_Atrelado = planoProduto.Id_Plano_Atrelado;
                clientePlanoProduto.FlagTrocaPlano = planoProduto.FlagTrocaPlano;
                clientePlanoProduto.Quantidade_Produto_Variavel = planoProduto?.Quantidade > 0 ? planoProduto?.Quantidade : 0;
                clientePlanoProduto.Valor_Total_Plano_Variavel = !String.IsNullOrEmpty(planoProduto.Valor_Total_Plano) ? Convert.ToDouble(planoProduto.Valor_Total_Plano) : 0;
                clientePlanoProduto.Parcela_Entrada = planoProduto?.Parcela > 0 ? Convert.ToInt64(planoProduto.Parcela) : (Int64?)null;
                clientePlanoProduto.Valor_Entrada = !String.IsNullOrEmpty(planoProduto.Valor_Entrada) ? Convert.ToDouble(planoProduto.Valor_Entrada) : 0;
            }
            else
            {
                clientePlanoProduto = new Cliente_PlanoProduto
                {
                    Endereco_Instalacao = venda.EnderecoInstalacao,

                    Cod_Escritorio = planoProduto.Cod_Escritorio,
                    Cod_Area = planoProduto.Cod_Area,
                    Localidade = planoProduto.Localidade,

                    Cod_Localidade = planoProduto.Cod_Localidade,
                    Dias_Teste_Gratis = planoProduto.Qtde_Dias,
                    Parcelas_Instalacao = planoProduto.Numero_Parcelas,

                    Periodo_Carencia = planoProduto.Periodo_Carencia,
                    Meses_Carencia = planoProduto.Meses_Carencia,

                    Campanha = planoProduto.Id_Campanha.HasValue ?
                         _repoCampanha.GetById(planoProduto.Id_Campanha.Value) :
                          null,

                    Plano = plano,

                    Produto = (!planoProduto.Id_Combo.HasValue && planoProduto.Tipo == PlanoProdutoEnum.Produto) ?
                                  _repoProduto.GetById((long)planoProduto.Id_Plano_Produto) :
                                  null,

                    Combo = planoProduto.Id_Combo.HasValue ?
                           _repoCombo.GetById((long)planoProduto.Id_Combo) :
                            null,

                    Multa_Carencia_Reais = modelo?.Multa_Carencia ?? 0,
                    CancelaContratoFimCarencia = modelo?.Cancela_Contrato_Automatico_Final_Carencia ?? false,

                    Tem_Reajuste = modelo?.Tem_Reajuste ?? false,

                    Frequencia_Reajuste = modelo?.Frequencia_Reajuste ?? null,
                    Renovar_CarenciaAuto = modelo?.Renova_Carencia_Automaticamente ?? null,

                    Data_Habilitado = DateTime.Now,
                    End_Instalacao_Igual_Principal = venda.End_Instalacao_Igual_Principal,

                    Login_Radius = planoProduto.Login_Radius,
                    Senha_Radius = planoProduto.Senha_Radius,

                    Pool_Ip = planoProduto.IdIpPool.HasValue ?
                        _repoPoolIP.GetById((long)planoProduto.IdIpPool) : null,

                    Valor_Final = !String.IsNullOrEmpty(planoProduto.ValorPlanoProduto_Final) ?
                                  Convert.ToDouble(planoProduto.ValorPlanoProduto_Final) : 0,

                    Valor_Final_Total = !String.IsNullOrEmpty(planoProduto.ValorTotal) ?
                                        Convert.ToDouble(planoProduto.ValorTotal) : 0,

                    Valor_Real = !String.IsNullOrEmpty(planoProduto.ValorPlanoProduto) ?
                            Convert.ToDouble(planoProduto.ValorPlanoProduto) : 0,

                    Valor_Instalacao_Final = !String.IsNullOrEmpty(planoProduto.ValorInstalacao_Final) ?
                            Convert.ToDouble(planoProduto.ValorInstalacao_Final) : 0,

                    Valor_Instalacao_Real = !String.IsNullOrEmpty(planoProduto.ValorInstalacao) ?
                            Convert.ToDouble(planoProduto.ValorInstalacao) : 0,

                    Cliente = cliente,
                    Id_Venda = venda.Id,

                    Id_TrocaPlano = planoProduto?.Id_PlanoMudanca > 0 ? planoProduto?.Id_PlanoMudanca : planoProduto?.Id_MudancaPlano,
                    JsonTelefonia = planoProduto.JsonTelefonia,
                    Status = Status_PlanoProduto.Aguardando_Aprovacao_Venda,
                    BaixadoInstalacao = false,
                    ContaReceberInstalacao = false,
                    UtilizaVendaAgroup = planoProduto.UtilizaVendaAgroup,
                    isPlanoAdicional = planoProduto.isPlanoAdicional,
                    possuiPlanosAdicionais = planoProduto.possuiPlanosAdicionais,
                    Id_Plano_Atrelado = planoProduto.Id_Plano_Atrelado,
                    FlagTrocaPlano = planoProduto.FlagTrocaPlano,
                    Quantidade_Produto_Variavel = planoProduto?.Quantidade > 0 ? planoProduto?.Quantidade : 0,
                    Valor_Total_Plano_Variavel = !String.IsNullOrEmpty(planoProduto.Valor_Total_Plano) ? Convert.ToDouble(planoProduto.Valor_Total_Plano) : 0,
                    Parcela_Entrada = planoProduto?.Parcela > 0 ? Convert.ToInt64(planoProduto.Parcela) : (Int64?)null,
                    Valor_Entrada = !String.IsNullOrEmpty(planoProduto.Valor_Entrada) ? Convert.ToDouble(planoProduto.Valor_Entrada) : 0,
                    FlagVendaFormatoAvulso = planoProduto.FlagVendaFormatoAvulso
                };
            }

            clientePlanoProduto.Telefonia = !String.IsNullOrEmpty(clientePlanoProduto?.JsonTelefonia) ? true : false;

            this._repoCliente.Update(cliente);

            if (modelo != null)
            {
                // Preenchimento do número do contrato
                if (!String.IsNullOrEmpty(planoProduto.Numero_Contrato) && !planoProduto.Numero_Contrato.Contains("Plano_TMP"))
                {
                    var objCC = this._repoClienteContrato.GetAll().Where(p => p.Id == Convert.ToInt64(planoProduto.Numero_Contrato)).FirstOrDefault();

                    if (objCC != null)
                        clientePlanoProduto.ClienteContrato = objCC;
                }
                else if (!String.IsNullOrEmpty(planoProduto.Numero_Contrato) && planoProduto.Numero_Contrato.Contains("Plano_TMP"))
                {
                    var objClienteContrato = _repoClientePlanoProduto.GetAll().OrderBy(x => x.ClienteContrato.Id)
                        .Where(x => x.Id_Venda == venda.Id && x.ClienteContrato.ModeloContrato.Id == planoProduto.Id_Modelo_Contrato).Select(x => x.ClienteContrato).FirstOrDefault();

                    if (objClienteContrato != null)
                    {
                        clientePlanoProduto.ClienteContrato = objClienteContrato;
                    }
                    else
                    {
                        clientePlanoProduto.ClienteContrato = modelo != null ? ClienteContrato.FromModelo(modelo, clientePlanoProduto.Cliente) : null;


                        _repoClienteContrato.Save(clientePlanoProduto.ClienteContrato);

                        clientePlanoProduto.ClienteContrato.NumeroContrato = Convert.ToString(clientePlanoProduto.ClienteContrato.Id);

                        _repoClienteContrato.Save(clientePlanoProduto.ClienteContrato);

                    }
                }
            }
            if (clientePlanoProduto.ClienteContrato == null)
            {
                throw new Exception("Plano inserido não apresenta Número de Contrato");
            }

            clientePlanoProduto.Forma_Pagamento_Cliente =
                cliente.Formas_Pagamento_Cliente
                       .Where(f => f.Id == planoProduto.Id_Forma_Pagamento_Cliente)
                       .FirstOrDefault();

            if (venda.Status == Status_Venda.Venda_Aprovada)
            {
                clientePlanoProduto.Atualiza_Ola = true;
            }

            return clientePlanoProduto;
        }

        private Endereco_Cliente BindEnderecoPrincipal(VendaDadosGeraisViewModel view, Configuracoes_Gerais configs)
        {
            Pais pais =
                view.EnderecoPrincipal.Id_Pais.HasValue ?
                    this._repoPais.GetById(view.EnderecoPrincipal.Id_Pais.Value) :
                    null;

            long? idUf = view.EnderecoPrincipal.Id_UF;
            long? idCidade = view.EnderecoPrincipal.Id_Cidade;
            long? idBairro = view.EnderecoPrincipal.Id_Bairro;

            if (pais == null)
            {
                pais = this._repoPais.GetByName(view.EnderecoPrincipal.Pais);
            }

            if (!idUf.HasValue)
            {
                Uf uf = this._repoUf.GetBySigla(view.EnderecoPrincipal.UF, view.EnderecoPrincipal.Pais);
                idUf = uf?.Id;
            }

            if (!idCidade.HasValue)
            {
                Cidade cidade =
                    this._repoCidade.GetByName(
                        view.EnderecoPrincipal.Cidade,
                        view.EnderecoPrincipal.UF,
                        view.EnderecoPrincipal.Pais
                    );

                idCidade = cidade?.Id;
            }

            if (!idBairro.HasValue)
            {
                Bairro bairro =
                    this._repoBairro.GetByName(
                        view.EnderecoPrincipal.Bairro,
                        view.EnderecoPrincipal.Cidade,
                        view.EnderecoPrincipal.UF,
                        view.EnderecoPrincipal.Pais
                    );

                idBairro = bairro?.Id;
            }

            if (configs.Utiliza_Cadastro_Endereco)
            {
                if (view.EnderecoPrincipal?.Id_Endereco_Logradouro == null && view.EnderecoPrincipal?.Id_Condominio_Predio == null)
                {
                    throw new Exception("Endereço/Logradouro ou Condomínio/Prédio deve ser preenchido corretamente em Dados Gerais");
                }
            }

            return new Endereco_Cliente
            {
                Pais = pais,

                Id_UF = idUf,
                UF = view.EnderecoPrincipal?.UF,

                Id_Cidade = idCidade,
                Cidade = view.EnderecoPrincipal?.Cidade,

                Id_Bairro = idBairro,
                Bairro = view.EnderecoPrincipal?.Bairro,

                Latitude =
                    !String.IsNullOrEmpty(view.EnderecoPrincipal?.Latitude) ?
                        Convert.ToDouble(view.EnderecoPrincipal?.Latitude?.Replace('.', ',')) :
                        0,
                Longitude =
                    !String.IsNullOrEmpty(view.EnderecoPrincipal?.Longitude) ?
                        Convert.ToDouble(view.EnderecoPrincipal?.Longitude?.Replace('.', ',')) :
                        0,

                Logradouro = view.EnderecoPrincipal?.Logradouro,
                Numero = view.EnderecoPrincipal?.Numero,
                Cep = view.EnderecoPrincipal?.CEP,
                Complemento = view.EnderecoPrincipal?.Complemento,
                Ponto_Referencia = view.EnderecoPrincipal?.Ponto_Referencia,

                Busca_Endereco =
                    view.EnderecoPrincipal.Busca_Endereco.HasValue ?
                        view.EnderecoPrincipal.Busca_Endereco.Value :
                        (Busca_Endereco?)null,

                Condominio_Predio =
                    (view.EnderecoPrincipal?.Busca_Endereco == Busca_Endereco.Condominio_Predio &&
                     view.EnderecoPrincipal.Id_Condominio_Predio.HasValue) ?
                        this._repoCondominioPredio.GetById(view.EnderecoPrincipal.Id_Condominio_Predio.Value) :
                        null,

                Bloco = (view.EnderecoPrincipal?.Busca_Endereco == Busca_Endereco.Condominio_Predio
                      && view.EnderecoPrincipal.Id_Bloco.HasValue) ? view.EnderecoPrincipal.Id_Bloco : (Int64?)null,

                Apartamento = (view.EnderecoPrincipal?.Busca_Endereco == Busca_Endereco.Condominio_Predio
                      && view.EnderecoPrincipal.Id_Apartamento.HasValue) ? view.EnderecoPrincipal.Id_Apartamento : (Int64?)null,

                Endereco_Logradouro =
                    (view.EnderecoPrincipal?.Busca_Endereco == Busca_Endereco.Endereco_Logradouro &&
                     view.EnderecoPrincipal.Id_Endereco_Logradouro.HasValue) ?
                        this._repoEnderecoLogradouro.GetById(view.EnderecoPrincipal.Id_Endereco_Logradouro.Value) :
                        null,

                Tipo_Imovel =
                    view.EnderecoPrincipal.Id_Tipo_Imovel.HasValue ?
                        this._repoTipoImovel.GetByCodTipoImovel(view.EnderecoPrincipal.Id_Tipo_Imovel.Value) :
                        null,

                Cod_Escritorio = view.EnderecoPrincipal?.Cod_Escritorio,
                Cod_Area = view.EnderecoPrincipal?.Cod_Area,
                Cod_Localidade =
                    !String.IsNullOrEmpty(view.EnderecoPrincipal?.Cod_Localidade) ?
                        Convert.ToInt32(view.EnderecoPrincipal?.Cod_Localidade) :
                        (int?)null,
            };
        }

        private Endereco_Cliente BindEnderecoInstalacao(VendaDadosGeraisViewModel view, bool enderecoInstalacaoMesmoPrincipal, Configuracoes_Gerais configs)
        {
            var enderecoViewPreencher = enderecoInstalacaoMesmoPrincipal ? view.EnderecoPrincipal : view.EnderecoInstalacao;

            Pais pais =
                enderecoViewPreencher.Id_Pais.HasValue ?
                    this._repoPais.GetById(enderecoViewPreencher.Id_Pais.Value) :
                    null;

            long? idUf = enderecoViewPreencher.Id_UF;
            long? idCidade = enderecoViewPreencher.Id_Cidade;
            long? idBairro = enderecoViewPreencher.Id_Bairro;

            if (pais == null)
            {
                pais = this._repoPais.GetByName(enderecoViewPreencher.Pais);
            }

            if (!idUf.HasValue)
            {
                Uf uf = this._repoUf.GetBySigla(enderecoViewPreencher.UF, enderecoViewPreencher.Pais);
                idUf = uf?.Id;
            }

            if (!idCidade.HasValue)
            {
                Cidade cidade =
                    this._repoCidade.GetByName(
                        enderecoViewPreencher.Cidade,
                        enderecoViewPreencher.UF,
                        enderecoViewPreencher.Pais
                    );

                idCidade = cidade?.Id;
            }

            if (!idBairro.HasValue)
            {
                Bairro bairro =
                    this._repoBairro.GetByName(
                        enderecoViewPreencher.Bairro,
                        enderecoViewPreencher.Cidade,
                        enderecoViewPreencher.UF,
                        enderecoViewPreencher.Pais
                    );

                idBairro = bairro?.Id;
            }

            if (configs.Utiliza_Cadastro_Endereco)
            {
                if (enderecoViewPreencher?.Id_Endereco_Logradouro == null && enderecoViewPreencher?.Id_Condominio_Predio == null)
                {
                    throw new Exception("Endereço/Logradouro ou Condomínio/Prédio de Instalação deve ser preenchido corretamente em Dados Gerais");
                }
            }

            return new Endereco_Cliente
            {
                Pais = pais,

                Id_UF = idUf,
                UF = enderecoViewPreencher?.UF,

                Id_Cidade = idCidade,
                Cidade = enderecoViewPreencher?.Cidade,

                Id_Bairro = idBairro,
                Bairro = enderecoViewPreencher?.Bairro,

                Latitude =
                !String.IsNullOrEmpty(enderecoViewPreencher?.Latitude) ?
                Convert.ToDouble(enderecoViewPreencher?.Latitude?.Replace('.', ',')) :
                (double?)null,
                Longitude =
                !String.IsNullOrEmpty(enderecoViewPreencher?.Longitude) ?
                Convert.ToDouble(enderecoViewPreencher?.Longitude?.Replace('.', ',')) :
                (double?)null,

                Logradouro = enderecoViewPreencher?.Logradouro,
                Numero = enderecoViewPreencher?.Numero,
                Cep = enderecoViewPreencher?.CEP,
                Complemento = enderecoViewPreencher?.Complemento,
                Ponto_Referencia = enderecoViewPreencher?.Ponto_Referencia,

                Busca_Endereco =
                enderecoViewPreencher.Busca_Endereco.HasValue ?
                enderecoViewPreencher.Busca_Endereco.Value :
                (Busca_Endereco?)null,

                Condominio_Predio =
                (enderecoViewPreencher?.Busca_Endereco == Busca_Endereco.Condominio_Predio &&
                enderecoViewPreencher.Id_Condominio_Predio.HasValue) ?
                this._repoCondominioPredio.GetById(enderecoViewPreencher.Id_Condominio_Predio.Value) :
                null,

                Bloco = (enderecoViewPreencher?.Busca_Endereco == Busca_Endereco.Condominio_Predio
                      && enderecoViewPreencher.Id_Bloco.HasValue) ? enderecoViewPreencher.Id_Bloco : (Int64?)null,

                Apartamento = (enderecoViewPreencher?.Busca_Endereco == Busca_Endereco.Condominio_Predio
                      && enderecoViewPreencher.Id_Apartamento.HasValue) ? enderecoViewPreencher.Id_Apartamento : (Int64?)null,

                Endereco_Logradouro =
                (enderecoViewPreencher?.Busca_Endereco == Busca_Endereco.Endereco_Logradouro &&
                enderecoViewPreencher.Id_Endereco_Logradouro.HasValue) ?
                this._repoEnderecoLogradouro.GetById(enderecoViewPreencher.Id_Endereco_Logradouro.Value) :
                null,

                Tipo_Imovel =
                enderecoViewPreencher.Id_Tipo_Imovel.HasValue ?
                this._repoTipoImovel.GetByCodTipoImovel(enderecoViewPreencher.Id_Tipo_Imovel.Value) :
                null,

                Cod_Escritorio = enderecoViewPreencher?.Cod_Escritorio,
                Cod_Area = enderecoViewPreencher?.Cod_Area,
                Cod_Localidade =
                !String.IsNullOrEmpty(enderecoViewPreencher.Cod_Localidade) ?
                Convert.ToInt32(enderecoViewPreencher?.Cod_Localidade) :
                (int?)null,
            };
        }

        private Endereco_Cliente BindEnderecoCobranca(VendaDadosGeraisViewModel view)
        {
            Pais pais =
                view.EnderecoCobranca.Id_Pais.HasValue ?
                    this._repoPais.GetById(view.EnderecoCobranca.Id_Pais.Value) :
                    null;

            long? idUf = view.EnderecoCobranca.Id_UF;
            long? idCidade = view.EnderecoCobranca.Id_Cidade;
            long? idBairro = view.EnderecoCobranca.Id_Bairro;

            if (pais == null)
            {
                pais = this._repoPais.GetByName(view.EnderecoCobranca.Pais);
            }

            if (!idUf.HasValue)
            {
                Uf uf = this._repoUf.GetBySigla(view.EnderecoCobranca.UF, view.EnderecoCobranca.Pais);
                idUf = uf?.Id;
            }

            if (!idCidade.HasValue)
            {
                Cidade cidade =
                    this._repoCidade.GetByName(
                        view.EnderecoCobranca.Cidade,
                        view.EnderecoCobranca.UF,
                        view.EnderecoCobranca.Pais
                    );

                idCidade = cidade?.Id;
            }

            if (!idBairro.HasValue)
            {
                Bairro bairro =
                    this._repoBairro.GetByName(
                        view.EnderecoCobranca.Bairro,
                        view.EnderecoCobranca.Cidade,
                        view.EnderecoCobranca.UF,
                        view.EnderecoCobranca.Pais
                    );

                idBairro = bairro?.Id;
            }

            return new Endereco_Cliente
            {
                Pais = pais,

                Id_UF = idUf,
                UF = view.EnderecoCobranca?.UF,

                Id_Cidade = idCidade,
                Cidade = view.EnderecoCobranca?.Cidade,

                Id_Bairro = idBairro,
                Bairro = view.EnderecoCobranca?.Bairro,

                Latitude =
                    !String.IsNullOrEmpty(view.EnderecoCobranca?.Latitude) ?
                        Convert.ToDouble(view.EnderecoCobranca?.Latitude?.Replace('.', ',')) :
                        (double?)null,
                Longitude =
                    !String.IsNullOrEmpty(view.EnderecoCobranca?.Longitude) ?
                        Convert.ToDouble(view.EnderecoCobranca?.Longitude?.Replace('.', ',')) :
                        (double?)null,

                Logradouro = view.EnderecoCobranca?.Logradouro,
                Numero = view.EnderecoCobranca?.Numero,
                Cep = view.EnderecoCobranca?.CEP,
                Complemento = view.EnderecoCobranca?.Complemento,
                Ponto_Referencia = view.EnderecoCobranca?.Ponto_Referencia,

                Busca_Endereco =
                    view.EnderecoCobranca.Busca_Endereco.HasValue ?
                        view.EnderecoCobranca.Busca_Endereco.Value :
                        (Busca_Endereco?)null,

                Condominio_Predio =
                    (view.EnderecoCobranca?.Busca_Endereco == Busca_Endereco.Condominio_Predio &&
                     view.EnderecoCobranca.Id_Condominio_Predio.HasValue) ?
                        this._repoCondominioPredio.GetById(view.EnderecoCobranca.Id_Condominio_Predio.Value) :
                        null,
                Bloco = (view.EnderecoCobranca?.Busca_Endereco == Busca_Endereco.Condominio_Predio
                      && view.EnderecoCobranca.Id_Bloco.HasValue) ? view.EnderecoCobranca.Id_Bloco : (Int64?)null,

                Apartamento = (view.EnderecoCobranca?.Busca_Endereco == Busca_Endereco.Condominio_Predio
                      && view.EnderecoCobranca.Id_Apartamento.HasValue) ? view.EnderecoCobranca.Id_Apartamento : (Int64?)null,
                Endereco_Logradouro =
                    (view.EnderecoCobranca?.Busca_Endereco == Busca_Endereco.Endereco_Logradouro &&
                     view.EnderecoCobranca.Id_Endereco_Logradouro.HasValue) ?
                        this._repoEnderecoLogradouro.GetById(view.EnderecoCobranca.Id_Endereco_Logradouro.Value) :
                        null,

                Tipo_Imovel =
                    view.EnderecoCobranca.Id_Tipo_Imovel.HasValue ?
                        this._repoTipoImovel.GetByCodTipoImovel(view.EnderecoCobranca.Id_Tipo_Imovel.Value) :
                        null,

                Cod_Escritorio = view.EnderecoCobranca?.Cod_Escritorio,
                Cod_Area = view.EnderecoCobranca?.Cod_Area,
                Cod_Localidade =
                    !String.IsNullOrEmpty(view.EnderecoCobranca?.Cod_Localidade) ?
                        Convert.ToInt32(view.EnderecoCobranca?.Cod_Localidade) :
                        (int?)null,
            };
        }

        /// <summary>
        ///     Retorna um objeto com os dados da venda preenchido com as informações dos dados gerais
        /// </summary>
        /// <param name="venda">Objeto com as informações da venda</param>
        /// <param name="cliente">Objeto com as informações do cliente da venda</param>
        /// <param name="view">Objeto com as informações dos dados gerais que serão gravadas</param>
        /// <param name="usuario">Usuário responsável pela gravação dos dados gerais</param>
        public Venda BindDadosGeraisVenda(Venda venda, Cliente cliente, VendaDadosGeraisViewModel view, string usuario)
        {
            venda.Id_Cliente = view.Id_Cliente;
            venda.StatusVenda = view.StatusVenda;


            if (venda?.Vendedor?.Id != view.Id_Vendedor)
                venda.Vendedor = view.Id_Vendedor.HasValue ? this._repoFuncionario.GetById(view.Id_Vendedor.Value) : null;

            if (venda?.Departamento?.Id != view.Id_Departamento || venda?.Id == 0)
                venda.Departamento = view.Id_Departamento.HasValue ? this._repoDepto.GetById(view.Id_Departamento.Value) : venda.Vendedor?.Departamento;

            if (venda?.Supervisor?.Id != view.Id_Supervisor || venda?.Id == 0)
                venda.Supervisor = view.Id_Supervisor.HasValue ? this._repoFuncionario.GetById(view.Id_Supervisor.Value) : venda.Vendedor?.Supervisor;

            if (venda?.Campanha?.Id != view.Id_Campanha_Venda || venda?.Id == 0)
                venda.Campanha = view.Id_Campanha_Venda.HasValue ? this._repoCampanha.GetById(view.Id_Campanha_Venda.Value) : null;

            venda.Justificativa_Avaliacao_Comercial = view.Justificativa_Avaliacao_Comercial;
            venda.Consulta_Viabilidade_Realizada = view.Consulta_Viabilidade_Realizada;
            venda.End_Instalacao_Igual_Principal = view.End_Instalacao_Igual_Principal;
            venda.End_Instalacao_Igual_Prospecto = view.End_Instalacao_Igual_Prospecto;
            venda.Id_Responsavel = view.Workflow_VendasAtividade?.Responsavel == null ? this._repoUsuario.GetById(view.Id_Responsavel): view.Workflow_VendasAtividade?.Responsavel;
            venda.Id_Setor_Departamento = view.Workflow_VendasAtividade?.SetorResponsavel == null ? this._repoDepto.GetById(Convert.ToInt64(view.Id_Setor_Departamento)) : view.Workflow_VendasAtividade?.SetorResponsavel;


            venda.End_CadastroCondominioPrincipal = view.End_CadastroCondominioPrincipal;
            venda.End_CadastroCondominioInstalacao = view.End_CadastroCondominioInstalacao;
            venda.End_CadastroCondominioCobranca = view.End_CadastroCondominioCobranca;

            if (view.Id == 0)
            {
                venda.Status = Status_Venda.Analise_Cliente;
            }
            else
            {
                venda.Status = view.Status;
            }

            if (!String.IsNullOrEmpty(view.DataEncerramento))
            {
                venda.Data_Venda = DateTime.ParseExact(view.DataEncerramento, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            else
            {
                venda.Data_Venda = null;
            }

            cliente = this.GravarDadosCliente(cliente, view, usuario);

            Configuracoes_Gerais config = this._repoConfig.GetConfiguracoesGerais();
            cliente.Endereco_Principal = this.BindEnderecoPrincipal(view, config);
            bool enderecoInstalacaoMesmoPrincipal = venda.Status == Status_Venda.Analise_Cliente && config.EnderecoInstalacaoMesmoPrincipal;

            if (venda.Id != 0)
            {
                double? latitudeView =
                    !String.IsNullOrEmpty(view.EnderecoInstalacao?.Latitude) ?
                        Convert.ToDouble(view.EnderecoInstalacao?.Latitude?.Replace('.', ',')) :
                        (double?)null;

                double? longitudeView =
                    !String.IsNullOrEmpty(view.EnderecoInstalacao?.Longitude) ?
                        Convert.ToDouble(view.EnderecoInstalacao?.Longitude?.Replace('.', ',')) :
                        (double?)null;

                // Determina se os dados do endereço de instalação vindos da view são os mesmos
                // do endereço de instalação gravado no banco
                bool enderecoInstalacaoIgualView =
                    venda.EnderecoInstalacao?.Pais?.Id == view.EnderecoInstalacao?.Id_Pais &&
                    venda.EnderecoInstalacao?.UF == view.EnderecoInstalacao?.UF &&
                    venda.EnderecoInstalacao?.Id_Cidade == view.EnderecoInstalacao?.Id_Cidade &&
                    venda.EnderecoInstalacao?.Cep == view.EnderecoInstalacao?.CEP &&
                    venda.EnderecoInstalacao?.Logradouro == view.EnderecoInstalacao?.Logradouro &&
                    venda.EnderecoInstalacao?.Numero == view.EnderecoInstalacao?.Numero &&
                    venda.EnderecoInstalacao?.Cod_Escritorio == view.EnderecoInstalacao?.Cod_Escritorio &&
                    venda.EnderecoInstalacao?.Cod_Area == view.EnderecoInstalacao?.Cod_Area &&
                    venda.EnderecoInstalacao?.Cod_Localidade.GetValueOrDefault(0) == Convert.ToInt64(view.EnderecoInstalacao?.Cod_Localidade) &&
                    venda.EnderecoInstalacao?.Latitude == latitudeView &&
                    venda.EnderecoInstalacao?.Longitude == longitudeView;


                if (enderecoInstalacaoMesmoPrincipal)
                {
                    venda.EnderecoInstalacao = this.BindEnderecoInstalacao(view, enderecoInstalacaoMesmoPrincipal, config);
                }
                if (!enderecoInstalacaoIgualView)
                {
                    venda.EnderecoInstalacao = this.BindEnderecoInstalacao(view, enderecoInstalacaoMesmoPrincipal, config);
                }
            }
            else
            {
                venda.EnderecoInstalacao = this.BindEnderecoInstalacao(view, enderecoInstalacaoMesmoPrincipal, config);
            }

            if (venda.EnderecoInstalacao.Endereco_Logradouro != null)
            {
                venda.EnderecoInstalacao.Endereco_Logradouro.Cod_Area = venda.EnderecoInstalacao?.Cod_Area;
                venda.EnderecoInstalacao.Endereco_Logradouro.Cod_Localidade = Convert.ToString(venda.EnderecoInstalacao?.Cod_Localidade);
                venda.EnderecoInstalacao.Endereco_Logradouro.Cod_Escritorio = venda.EnderecoInstalacao?.Cod_Escritorio;
            }

            // define workflow e atividade
            if (view.IdWorkFLowEscolhido.HasValue)
            {
                venda.Workflow_Vendas = _workflowRepository.GetById(view.IdWorkFLowEscolhido.Value);
            }
            else
            {
                VerificaWorkflowRequestModel requestVerificar = new VerificaWorkflowRequestModel
                {
                    UF = cliente.Endereco_Principal.UF,
                    TipoPessoa = cliente.Tipo_Pessoa.HasValue ? cliente.Tipo_Pessoa.ToString() : null,
                    Categoria = cliente.Categoria_Cliente != null ? cliente.Categoria_Cliente.Id.ToString() : null,
                    Captacao = cliente.Qual_Meio != null ? cliente.Qual_Meio.Id.ToString() : null,
                    Cidade = cliente.Endereco_Principal != null ? cliente.Endereco_Principal.Id_Cidade.ToString() : null,
                };
                venda.Workflow_Vendas = FindWorkFlow(requestVerificar).Result.FirstOrDefault();
            }

            if (venda.Workflow_Vendas != null && venda.Workflow_VendasAtividade == null)
            {
                venda.Workflow_VendasAtividade = venda.Workflow_Vendas.Atividades.Where(p => p.Ativ_Status == Status_Default.Ativo).FirstOrDefault();

                if (venda.Workflow_VendasAtividade != null)
                {
                    venda.Status = _repository.GetAllStatusVenda().Where(p => p.Id == Convert.ToInt64(venda.Workflow_VendasAtividade.Ativ_StatusVenda.Id)).FirstOrDefault().StatusPadrao.Value;
                    venda.StatusVenda = venda.Workflow_VendasAtividade.Ativ_StatusVenda?.Descricao;
                }
            }

            return venda;
        }

        /// <summary>
        ///     Retorna a venda associado ao ID informado
        /// </summary>
        /// <param name="id">ID da venda</param>      
        public Venda GetVendaById(long id)
        {
            return this._repository.GetById(id);
        }

        /// <summary>
        ///     Retorna os planos e produtos associados à venda informada
        /// </summary>
        /// <param name="idVenda">ID da venda</param>
        public IEnumerable<Cliente_PlanoProduto> GetPlanosProdutosVenda(long idVenda)
        {
            return this._repository.GetPlanosProdutosVenda(idVenda)?.ToList();
        }

        /// <summary>
        ///     Retorna os planos e produtos associados à venda informada
        /// </summary>
        /// <param name="idVenda">ID da venda</param>
        public async Task<IEnumerable<ClienteContratoModel>> GetPlanosProdutosVendaSQL(long idVenda)
        {
            return (await this._repository.GetPlanosProdutosVendaSQL(idVenda))?.ToList();
        }

        public void GravarIdConsultaRestricao(Venda venda, EnumEtapaVenda consultaRestricao, long idConsultaRestricao)
        {
            switch (consultaRestricao)
            {
                case EnumEtapaVenda.ConsultaViabilidade:

                    venda.IdConsultaViabilidade = idConsultaRestricao;

                    break;
                case EnumEtapaVenda.ConsultaPendenciaEndereco:

                    venda.IdConsultaPendenciaEndereco = idConsultaRestricao;

                    break;
                case EnumEtapaVenda.ConsultaPendenciaFinanceira:

                    venda.IdConsultaPendenciaFinanceira = idConsultaRestricao;

                    break;
                case EnumEtapaVenda.ConsultaCredito:

                    venda.IdConsultaCredito = idConsultaRestricao;

                    break;
            }
            ;

            this._repository.Update(venda);
        }

        /// <summary>
        ///     Grava o motivo de aprovação manual para a consulta de viabiliade na venda informada
        /// </summary>
        /// <param name="idVenda">ID da Venda</param>
        /// <param name="motivo">Motivo para aprovação</param>
        public void GravarMotivoAprovacaoManualViabilidade(long idVenda, string motivo, String Idmotivo, String user)
        {
            if (String.IsNullOrEmpty(motivo))
            {
                throw new Exception("Motivo para aprovação não informado");
            }

            Venda venda = this._repository.GetById(idVenda);

            if (venda == null)
            {
                throw new Exception("Venda não encontrada");
            }

            var modeloAnterior = (Venda)venda.Clone();

            venda.MotivoAprovManualViabilidade = motivo;
            venda.MotivoAprovManualESC = Idmotivo;


            this._repository.Update(venda);



            using (var sessionLog = HibernateUtil.GetSessionLog())
            using (var session = HibernateUtil.GetSession())
            using (var repoLogVendas = new Conexao<LogVendas>(sessionLog))
            using (var reporMotivoAprovacao = new Conexao<Motivo_Aprovacao_Resticao>(session))
            {
                var motivoDes = reporMotivoAprovacao.selectFirst(p => p.Id == Convert.ToInt64(Idmotivo)).Result;

                if (motivoDes == null)
                {
                    throw new Exception("Motivo não encontrada");
                }

                venda.MotivoAprovManualESC = motivoDes.Descricao;

                var logs = venda.GeraLog(modeloAnterior, user, user, Sinapse.ModelLog.TipoOperLog.Alteracao);

                foreach (var log in logs)
                    repoLogVendas.Save(log);
            }
        }

        /// <summary>
        /// Atualiza uma venda no sistema
        /// </summary>
        /// <param name="venda">Objeto com as informações da venda</param>
        public void Update(Venda venda)
        {
            this._repoEnderecoCliente.Update(venda.EnderecoInstalacao);
            this._repository.Update(venda);
        }

        /// <summary>
        ///     Grava uma venda nova no sistema
        /// </summary>
        /// <param name="venda">Objeto com as informações da venda</param>
        public void Save(Venda venda)
        {
            // Precisamos gravar o endereço de instalação da venda antes de gravar todo o objeto da venda.
            // Caso contrário nos deparamos com um erro do NHibernate
            if (venda?.EnderecoInstalacao != null)
            {
                this._repoEnderecoCliente.Save(venda.EnderecoInstalacao);
            }
            this._repository.Save(venda);
        }

        /// <summary>
        ///     Grava plano produto no sistema
        /// </summary>
        /// <param name="planoproduto">Objeto com as informações da plano produto</param>
        public void SavePlanoProduto(Cliente_PlanoProduto planoproduto)
        {
            this._repoClientePlanoProduto.Save(planoproduto);
        }

        public async Task ReprovacaoAutomaticaWorkflow(long venda, long atividade, string user, string name)
        {
            using (var session = HibernateUtil.GetSession())
            {
                var commands = new List<StringBuilder>();
                List<AtividadeVendaRequestModel> atividades = (await this._repository.GetAtividadesReprovacaoAutomatica(venda)).ToList();
                List<Atividade_StatusVenda> statusCadastrados = this._repository.GetAtivStatusVenda().ToList();


                if (statusCadastrados.Any(p => p.StatusPadrao == Status_Venda.Venda_Cancelada))
                {
                    var atvById = atividades.Find(p => p.Id == atividade);
                    var atividadeAtual = atividades.Find(p => p.Descricao.Trim() == Convert.ToString(atvById.Descricao).Trim());
                    var caminhoCurto = new List<AtividadeVendaRequestModel>();
                    bool caminhoFormado = false;
                    while (!caminhoFormado)
                    {
                        var atvPrincipal = atividades.Find(p => p.Descricao.Trim() == Convert.ToString(atividadeAtual.Descricao).Trim());

                        if (statusCadastrados.FirstOrDefault(p => p.Id == atvPrincipal.Status && p.StatusPadrao != Status_Venda.Venda_Cancelada) != null)
                        {
                            caminhoCurto.Add(atvPrincipal);
                            atividadeAtual = atividades.Find(p => p.Descricao.Trim() == Convert.ToString(atvPrincipal.AtividadeReprovada.Value).Trim());
                        }
                        else
                        {
                            caminhoFormado = true;
                        }
                    }
                    var passosReprovacao = caminhoCurto.Select(p => FuncReprovaWorkflow(p, statusCadastrados, venda).Result)
                                                       .ToList();
                    commands.AddRange(passosReprovacao.Where(p => !p.ok).Select(p => p.sql).ToList());
                    if (commands.Count > 0)
                    {
                        var repAutomatico = new ReprovacaoAutomaticaComandos
                        {
                            ComandosSql = commands,
                            Venda = venda,
                            Atividades = new Queue<long>(caminhoCurto.Where(p => !FuncReprovaWorkflow(p, statusCadastrados, venda).Result.ok).Select(p => p.Id.Value).ToList()),
                            Usuario = user,
                            Nome = name,
                        };
                        await this._repository.ReprovaAtividadesWorkflowVendaAutomatico(repAutomatico);
                    }
                }
                else
                {
                    throw new Exception("Não há status padrão para cancelamento de venda!");
                }
            }
        }

        private DateTime? ParseDataComMensagem(string data)
        {
            try
            {
                return DateTime.ParseExact(data, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                throw new FormatException("Preencha a data no formato dd/MM/yyyy.");
            }
        }
        public async Task<(bool ok, StringBuilder sql)> FuncReprovaWorkflow(AtividadeVendaRequestModel request, IEnumerable<Atividade_StatusVenda> statusCadastrados, long venda)
        {
            bool ok = false;
            if (statusCadastrados.Select(p => p.Id == request.Status && p.StatusPadrao == Status_Venda.Venda_Cancelada).Count() == 1)
            {
                ok = true;
                return (ok, null);
            }
            else
            {
                var record = await this._repository.GetItemReprovacaoAutomaticaByAtividade(venda, request.AtividadeReprovada.Value);
                var sql = new StringBuilder();
                sql.AppendLine($"UPDATE TB_VENDA SET ID_WORKFLOW_VENDAS_ATIVIDADE = {record.Id} Where ID = {venda}");
                return (ok, sql);
            }
        }

        public async Task<List<Workflow_Vendas>> FindWorkFlow(VerificaWorkflowRequestModel dadosVerificar)
        {
            return await _workflowRepository.FindWorkFlow(dadosVerificar);
        }


        #region Mobile
        public void GravarIdConsultaRestricaoMobile(Venda venda, Sinapse.Model.enumerados.EnumEtapaVenda consultaRestricao, long? idConsultaRestricao)
        {
            switch (consultaRestricao)
            {
                case Sinapse.Model.enumerados.EnumEtapaVenda.ConsultaViabilidade:
                    var idConsultaAntiga = venda?.IdConsultaViabilidade;
                    if (idConsultaAntiga != idConsultaRestricao)
                    {
                        venda.MotivoAprovManualViabilidade = null;
                    }
                    venda.IdConsultaViabilidade = idConsultaRestricao;
                    break;
                case Sinapse.Model.enumerados.EnumEtapaVenda.ConsultaPendenciaEndereco:
                    venda.IdConsultaPendenciaEndereco = idConsultaRestricao;
                    break;
                case Sinapse.Model.enumerados.EnumEtapaVenda.ConsultaPendenciaFinanceira:
                    venda.IdConsultaPendenciaFinanceira = idConsultaRestricao;
                    break;
                case Sinapse.Model.enumerados.EnumEtapaVenda.ConsultaCredito:
                    venda.IdConsultaCredito = idConsultaRestricao;
                    break;
            }
            ;

            this._repository.Update(venda);
        }
        #endregion
    }
}
