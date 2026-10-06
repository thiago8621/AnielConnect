using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
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
using Aniel.Connect.Repository.EtapaVenda;
using Aniel.Connect.Repository.FormaPagamento;
using Aniel.Connect.Repository.Funcionario;
using Aniel.Connect.Repository.MeioCaptacao;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.ModeloContrato;
using Aniel.Connect.Repository.Pais;
using Aniel.Connect.Repository.Plano;
using Aniel.Connect.Repository.Pool_Ip;
using Aniel.Connect.Repository.Produto;
using Aniel.Connect.Repository.Repository.ClienteContrato;
using Aniel.Connect.Repository.Repository.ComboPlanoProduto;
using Aniel.Connect.Repository.Repository.Pool_Ip;
using Aniel.Connect.Repository.TipoImovel;
using Aniel.Connect.Repository.Uf;
using Aniel.Connect.Repository.Usuario;
using Aniel.Connect.Repository.Vencimento;
using Aniel.Connect.Repository.Venda;
using Aniel.Connect.Repository.Workflow;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Office2010.Excel;
using MySqlX.XDevAPI;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class VincularPlanoProdutoApiController : ApiController
    {
        /// <summary>
        /// Vincula Plano Produto na proposta de venda
        /// </summary>
        /// <param name="request">Requisição dos dados</param>
        /// <returns></returns>            

        [HttpPost]
        [Route("api/VendaApi/VincularPlanoProdutoVenda")]
        public async Task<PostApiRetorno> VincularPlanoProdutoComboLote(VincularPlanoProdutoComboLoteRequest request)
        {
            var retorno = new PostApiRetorno();
          

            using (ISession sessionNh = HibernateUtil.GetSession())
            using (ITransaction transaction = sessionNh.BeginTransaction())
            {
                IClienteRepository repoCliente = new ClienteRepository(sessionNh);
                IEnderecoClienteRepository repoEnderecoCliente = new EnderecoClienteRepository(sessionNh);
                IVencimentoRepository repoVencimento = new VencimentoRepository(sessionNh);
                IFormaPagamentoRepository repoFormaPagamento = new FormaPagamentoRepository(sessionNh);
                IComboRepository repoCombo = new ComboRepository(sessionNh);
                ICampanhaRepository repoCampanha = new CampanhaRepository(sessionNh);
                IPlanoRepository repoPlano = new PlanoRepository(sessionNh);
                IProdutoRepository repoProduto = new ProdutoRepository(sessionNh);
                IPool_IpRepository repoPoolIp = new Pool_IpRepository(sessionNh); IVendaRepository repoVenda = new VendaRepository(sessionNh);
                IFuncionarioRepository repoFuncionario = new FuncionarioRepository(sessionNh);
                IDepartamentoRepository repoDepto = new DepartamentoRepository(sessionNh);

                IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(sessionNh);
                IUsuarioRepository repoUsuario = new UsuarioRepository(sessionNh);
                ICategoriaClienteRepository repoCategoriaCliente = new CategoriaClienteRepository(sessionNh);
                IMeioCaptacaoRepository repoMeioCaptacao = new MeioCaptacaoRepository(sessionNh);

                IPaisRepository repoPais = new PaisRepository(sessionNh);
                IUfRepository repoUf = new UfRepository(sessionNh);
                ICidadeRepository repoCidade = new CidadeRepository(sessionNh);
                IBairroRepository repoBairro = new BairroRepository(sessionNh);

                ITipoImovelRepository repoTipoImovel = new TipoImovelRepository(sessionNh);
                IEnderecoLogradouroRepository repoEnderecoLogradouro = new EnderecoLogradouroRepository(sessionNh);
                ICondominioPredioRepository repoCondominioPredio = new CondominioPredioRepository(sessionNh);
                IEtapaVendaRepository repoEtapaVenda = new EtapaVendaRepository(sessionNh);
                IBancoRepository repoBanco = new BancoRepository(sessionNh);
                IClienteContratoRepository repoClienteContrato = new ClienteContratoRepository(sessionNh);

                IModeloContratoRepository repoModeloContrato = new ModeloContratoRepository(sessionNh);
                IClientePlanoProdutoRepository repoClientePlanoProduto = new ClientePlanoProdutoRepository(sessionNh);
                IComboPlanoProdutoRepository repoComboPlanoProdutoRepository = new ComboPlanoProdutoRepository(sessionNh);
                IWorkflowRepository workflowRepository = new WorkflowRepository(sessionNh);

                var etapaVendaBusiness = new EtapaVendaBusiness(repoEtapaVenda, repoConfig);
                var clienteBusiness = new ClienteBusiness(repoCliente);
                var campanhaBusiness = new CampanhaBusiness(repoCampanha);
                var formasPagamentoBusiness = new FormaPagamentoBusiness(repoFormaPagamento);

                var vendaBusiness =
                    new VendaBusiness(
                        repoVenda,
                        repoFuncionario,
                        repoDepto,

                        repoConfig,
                        repoUsuario,

                        repoCliente,
                        repoEnderecoCliente,
                        repoCategoriaCliente,
                        repoMeioCaptacao,

                        repoPais,
                        repoUf,
                        repoCidade,
                        repoBairro,

                        repoTipoImovel,

                        repoEnderecoLogradouro,
                        repoCondominioPredio,

                        repoBanco,
                        repoVencimento,
                        repoFormaPagamento,
                        repoCombo,
                        repoCampanha,
                        repoPlano,
                        repoProduto,
                        repoPoolIp,
                        repoModeloContrato,
                        repoClienteContrato,
                        repoClientePlanoProduto,
                        repoComboPlanoProdutoRepository,
                        workflowRepository
                );

                try
                {
                    if (request == null) throw new Exception("Os parâmetros da requisição não foram informados");
                    if (request.IdVenda == 0) throw new Exception("O ID da venda não foi informado");
                    if (request.IdFormaPagamento == 0) throw new Exception("O ID de forma de pagamento não foi informado");
                    if (request.Itens == null || request.Itens.Count == 0) throw new Exception("Nenhum plano/combo foi informado");

                    using (var api = new UsuarioApiController())
                        if (!(await api.GetUsuarioValido(request.settings)).Ok)
                            throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");


                    var venda = vendaBusiness.GetVendaById(request.IdVenda);
                    var planoDescricao = "";
                    if (venda == null) throw new Exception($"A venda de ID {request.IdVenda} não foi encontrada");
                    if (!venda.Id_Cliente.HasValue) throw new Exception("O cliente não foi encontrado");

                    var cliente = clienteBusiness.GetById(venda.Id_Cliente.Value);
                    if (cliente == null) throw new Exception("O cliente de ID não foi encontrado");

                    var formaPagamento = formasPagamentoBusiness.GetById(request.IdFormaPagamento);

                    var formaPagamentoCliente = cliente.Formas_Pagamento_Cliente
                        .FirstOrDefault(p => p.Forma_Pagamento.Id == formaPagamento.Id);

                    if (formaPagamentoCliente == null)
                    {                        
                        var formaPagamentoClienteModels = GetFormaPagamentoFromRequest(request, cliente, formaPagamento);
                        var vendaDadosGerais = new VendaDadosGeraisViewModel { Id = venda.Id };

                        if (request.End_Cobranca_Igual_Principal)
                        {
                            vendaDadosGerais.EnderecoCobranca = new EnderecoClienteViewModel
                            {
                                Id_Pais = cliente?.Endereco_Principal?.Pais?.Id,
                                Id_UF = cliente?.Endereco_Principal?.Id_UF,
                                Id_Cidade = cliente?.Endereco_Principal?.Id_Cidade,
                                Id_Bairro = cliente?.Endereco_Principal?.Id_Bairro,
                                Id_Condominio_Predio = cliente?.Endereco_Principal?.Condominio_Predio?.Id,
                                Id_Bloco = cliente?.Endereco_Principal?.Bloco,
                                Id_Apartamento = cliente?.Endereco_Principal?.Apartamento,
                                Id_Endereco_Logradouro = cliente?.Endereco_Principal?.Endereco_Logradouro?.Id,
                                Id_Tipo_Imovel = Convert.ToInt32(cliente?.Endereco_Principal?.Tipo_Imovel?.Id),
                                Pais = cliente?.Endereco_Principal?.Pais?.Nome,
                                UF = cliente?.Endereco_Principal?.UF,
                                Cidade = cliente?.Endereco_Principal?.Cidade,
                                Bairro = cliente?.Endereco_Principal?.Bairro,
                                Logradouro = cliente?.Endereco_Principal?.Logradouro,
                                Numero = cliente?.Endereco_Principal?.Numero,
                                CEP = cliente?.Endereco_Principal?.Cep,
                                Complemento = cliente?.Endereco_Principal?.Complemento,
                                Ponto_Referencia = cliente?.Endereco_Principal?.Ponto_Referencia,
                                Latitude = cliente?.Endereco_Principal?.Latitude?.ToString(),
                                Longitude = cliente?.Endereco_Principal?.Longitude?.ToString(),
                                Cod_Escritorio = cliente?.Endereco_Principal?.Cod_Escritorio,
                                Cod_Area = cliente?.Endereco_Principal?.Cod_Area,
                                Cod_Localidade = cliente?.Endereco_Principal?.Cod_Localidade?.ToString(),
                                Busca_Endereco = cliente?.Endereco_Principal?.Busca_Endereco
                            };
                        }

                        formaPagamentoCliente = vendaBusiness.BindFormaPagamentoCliente(cliente, formaPagamentoClienteModels, vendaDadosGerais);
                        cliente.Formas_Pagamento_Cliente.Add(formaPagamentoCliente);
                        clienteBusiness.Update(cliente);
                    }

                    foreach (var item in request.Itens)
                    {
                        bool itensDuplicados = request.Itens.GroupBy(i => new { i.IdPlano, i.IdProduto, i.IdCombo })
                            .Any(g => g.Count() > 1);
                        if (itensDuplicados)
                            throw new Exception("Não é possível adicionar Plano, Produto ou Combo duplicados na mesma venda.");

                        if (item.IdCombo.HasValue && (item.IdPlano.HasValue || item.IdProduto.HasValue))
                            throw new Exception("Em cada item, informar apenas ID plano/produto OU ID combo");

                        if (!item.IdCombo.HasValue && !item.IdPlano.HasValue && !item.IdProduto.HasValue)
                            throw new Exception("Em cada item, informe IdCombo ou IdPlano/IdProduto");

                        Cliente_PlanoProduto planoProdutoJaVinculado = null;
                        using (var clientePlanoProdutoRepo = new Conexao<Cliente_PlanoProduto>(sessionNh))
                        {
                            planoProdutoJaVinculado = await clientePlanoProdutoRepo.selectFirst(p =>
                                p.Id_Venda == request.IdVenda &&
                                p.Cliente.Id == cliente.Id &&
                                (
                                    p.Combo.Id == item.IdCombo ||
                                    p.Plano.Id == item.IdPlano ||
                                    p.Produto.Id == item.IdProduto
                                )
                            );
                        }
                        if (planoProdutoJaVinculado != null)
                            throw new Exception($"Um plano/produto ou combo que você está tentando vincular já está vinculado a venda");

                        var planoProdutoVM = GetPlanoProdutoFromItem(request, item, repoCombo, repoPlano, formaPagamentoCliente);

                        var clientePlanoProduto = vendaBusiness.BindPlanoProduto(cliente, venda, planoProdutoVM);
                        vendaBusiness.SavePlanoProduto(clientePlanoProduto);
                    }

                    if (request.Itens.Any())
                    {
                        IEtapaVendaRepository repoEtapa = new EtapaVendaRepository(sessionNh);
                        var business = new EtapaVendaBusiness(repoEtapa, repoConfig);
                        business.MudarStatusEtapa(request.IdVenda, Sinapse.Model.enumerados.EnumEtapaVenda.PlanoProduto, Sinapse.Model.enumerados.StatusEtapaVenda.Aprovado);
                        business.MudarStatusEtapa(request.IdVenda, Sinapse.Model.enumerados.EnumEtapaVenda.Financeiro, Sinapse.Model.enumerados.StatusEtapaVenda.Aprovado);
                    }

                    retorno.Ok = true;
                    retorno.Mensagem = "Planos/Combos vinculados com sucesso na Venda :"+venda.Id;
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    retorno.Ok = false;
                    retorno.Mensagem = "Erro ao vincular planos/combos na proposta de venda: " + ex.Message;
                }

                return retorno;
            }
        }

         private PlanoProdutoListModel GetPlanoProdutoFromItem(
                 VincularPlanoProdutoComboLoteRequest request,
                 VinculoPlanoProdutoComboItemRequest item,
                 IComboRepository repoCombo,
                 IPlanoRepository repoPlano,
                 Forma_Pagamento_Cliente forma_Pagamento_Cliente)
        {
            var planoProduto = new PlanoProdutoListModel();

            planoProduto.Id_Venda = request.IdVenda;
            planoProduto.Id_Campanha = item.IdCampanha; 
            planoProduto.Descricao = item.DescricaoPlanoProduto;
            planoProduto.Detalhes = item.Detalhes;

            planoProduto.Id_Forma_Pagamento_Cliente = forma_Pagamento_Cliente?.Id;
            planoProduto.Id_Forma_De_Pagamento = request.IdFormaPagamento;
            planoProduto.Desc_Forma_Pagto = request.Desc_Forma_Pagto; 

            if (item.IdCombo.HasValue && (item.IdPlano.HasValue || item.IdProduto.HasValue))
                throw new Exception("Em cada item, informe apenas IdCombo OU IdPlano/IdProduto.");

            if (item.IdCombo.HasValue)
            {
                planoProduto.Id_Combo = item.IdCombo;

                var resultCombo = repoCombo.GetById(planoProduto.Id_Combo.Value);

                planoProduto.Id_Modelo_Contrato = resultCombo.ModeloContrato != null ? resultCombo.ModeloContrato.Id : 0;

                var valor = resultCombo.PlanosProdutos.Sum(p => p.Valor);
                var valorInst = resultCombo.PlanosProdutos.Sum(p => p.ValorInstalacao);

                planoProduto.ValorPlanoProduto = Convert.ToString(valor);
                planoProduto.ValorTotal = Convert.ToString(valor);
                planoProduto.ValorPlanoProduto_Final = Convert.ToString(valor);

                planoProduto.ValorInstalacao = Convert.ToString(valorInst);
                planoProduto.ValorInstalacao_Final = Convert.ToString(valorInst);

                planoProduto.Status_PlanoProduto =
                    resultCombo.PlanosProdutos
                        .FirstOrDefault(p => p.Plano.Tipo_Tecnologia == Tipo_Tecnologia.Internet)?
                        .Plano?.StatusInicial;
            }
            else
            {
                var idPlanoProduto = item.IdPlano ?? item.IdProduto;
                if (!idPlanoProduto.HasValue)
                    throw new Exception("Em cada item, informe IdCombo ou IdPlano/IdProduto.");

                planoProduto.Tipo = PlanoProdutoEnum.Plano;
                planoProduto.Id_Plano_Produto = idPlanoProduto;

                var resultPlano = repoPlano.GetById(planoProduto.Id_Plano_Produto.Value);

                planoProduto.Id_Modelo_Contrato = resultPlano.Modelo_Contrato != null ? resultPlano.Modelo_Contrato.Id : 0;

                planoProduto.ValorPlanoProduto = Convert.ToString(resultPlano.ValorPlano);
                planoProduto.ValorTotal = Convert.ToString(resultPlano.ValorPlano);
                planoProduto.ValorPlanoProduto_Final = Convert.ToString(resultPlano.ValorPlano);

                planoProduto.Status_PlanoProduto = resultPlano.StatusInicial;

                planoProduto.ValorInstalacao = Convert.ToString(resultPlano.ValorInstalacao);
                planoProduto.ValorInstalacao_Final = Convert.ToString(resultPlano.ValorInstalacao);
            }

            if (!string.IsNullOrEmpty(item.NecessitaCriarOS))
            {
                planoProduto.NecessitaCriarOS = item.NecessitaCriarOS;
                planoProduto.PossuiOSCriada = item.PossuiOSCriada;
                planoProduto.NumOS = item.NumOS;
            }

            if (!string.IsNullOrEmpty(item.ValorPlanoProduto))
            {
                planoProduto.ValorPlanoProduto = item.ValorPlanoProduto;
                planoProduto.ValorTotal = item.ValorTotal;
                planoProduto.ValorPlanoProduto_Final = item.ValorPlanoProduto_Final;
            }

            if (item.StatusPlanoProduto.HasValue)
                planoProduto.Status_PlanoProduto = item.StatusPlanoProduto;

            if (item.Teste_Gratis)
            {
                planoProduto.Periodo_Carencia = item.Periodo_Carencia;
                planoProduto.Meses_Carencia = item.MesesCarencia;
            }

            if (item.Id_Modelo_Contrato != 0)
                planoProduto.Id_Modelo_Contrato = item.Id_Modelo_Contrato;

            planoProduto.Numero_Contrato = "Plano_TMP";
            return planoProduto;
        }

        private Forma_Pagamento_ClienteModels GetFormaPagamentoFromRequest(
         VincularPlanoProdutoComboLoteRequest request,
         Cliente cliente,
         Forma_Pagamento forma_Pagamento)
        {
            try
            {
                var formaPagamento = new Forma_Pagamento_ClienteModels();

                formaPagamento.Id_Cliente = cliente.Id;
                formaPagamento.Id_Forma_Pagamento = request.IdFormaPagamento;

                if (!int.TryParse(request.Dia_Vencimento, out int diaVencimento) || diaVencimento < 1 || diaVencimento > 31)
                {
                    throw new Exception("Dia de vencimento inválido. Informe um número entre 1 e 31.");
                }

                formaPagamento.Dia_Vencimento = Convert.ToString(diaVencimento);
                formaPagamento.Desc_Banco = request.Banco;
                formaPagamento.Agencia = request.Agencia;
                formaPagamento.Conta_Corrente = request.Conta_Corrente;
                formaPagamento.Digito = request.Digito;

                formaPagamento.Forma_Envio = request.Forma_Envio;
                formaPagamento.Desc_Forma_Envio = request.Forma_Envio.HasValue
                    ? Convert.ToString(request.Forma_Envio)
                    : null;

                formaPagamento.Desc_Forma_Pagamento = forma_Pagamento.Descricao;
                formaPagamento.End_Cobranca_Igual_Principal = request.End_Cobranca_Igual_Principal;

                EnderecoRequestModel enderecoRequest = request.Endereco_Cobranca;

                if (request.End_Cobranca_Igual_Principal)
                {
                    enderecoRequest = new EnderecoRequestModel
                    {
                        Pais = cliente.Endereco_Principal.Pais,
                        UF = cliente.Endereco_Principal?.UF,
                        Cidade = cliente.Endereco_Principal?.Cidade,
                        Bairro = cliente.Endereco_Principal?.Bairro,
                        Cep = cliente.Endereco_Principal?.Cep,
                        Logradouro = cliente.Endereco_Principal?.Logradouro,
                        Numero = cliente.Endereco_Principal?.Numero,
                        Complemento = cliente.Endereco_Principal?.Complemento,
                        Ponto_Referencia = cliente.Endereco_Principal?.Ponto_Referencia,
                        Cod_Escritorio = cliente.Endereco_Principal?.Cod_Escritorio,
                        Cod_Area = cliente.Endereco_Principal?.Cod_Area,
                        Cod_Localidade = Convert.ToString(cliente.Endereco_Principal.Cod_Localidade),
                        Cod_TipoImovel = Convert.ToInt32(cliente.Endereco_Principal.Tipo_Imovel.Id)
                    };
                }

                formaPagamento.LogradouroCobranca = enderecoRequest.Logradouro;
                formaPagamento.BairroCobranca = enderecoRequest.Bairro;
                formaPagamento.CEPCobranca = enderecoRequest.Cep;
                formaPagamento.CidadeCobranca = enderecoRequest.Cidade;
                formaPagamento.ComplementoCobranca = enderecoRequest.Complemento;
                formaPagamento.UFCobranca = enderecoRequest.UF;

                return formaPagamento;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao processar os dados de forma de pagamento: " + ex.Message);
            }
        }
    }
}

