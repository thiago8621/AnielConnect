using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
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
using DocumentFormat.OpenXml.Spreadsheet;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNet.Identity;
using MySqlX.XDevAPI;
using NHibernate;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OpenXmlPowerTools;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class VendaApiController : BaseApiController
    {

        [HttpPost]
        [Route("api/VendaApi/Cadastrar")]
        public async Task<PostApiRetorno<Int64>> Cadastrar(PropostaVendaApiRequest request)
        {
            var retorno = new PostApiRetorno<Int64>();

            using (ISession sessionNh = HibernateUtil.GetSession())
            using (ITransaction transaction = sessionNh.BeginTransaction())
            using (var repository = new Conexao<Venda>(sessionNh))
            {
                try
                {
                    if (request == null)
                    {
                        throw new Exception("Os parâmetros da requisição não foram informados");
                    }

                    using (var api = new UsuarioApiController())
                    {
                        if (!(await api.GetUsuarioValido(request.settings)).Ok)
                        {
                            throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                        }
                    }

                    if (!request.IdCliente.HasValue)
                    {
                        throw new Exception("O ID do cliente não foi informado");
                    }

                    if (String.IsNullOrEmpty(request.EnderecosIgualLead) || request.EnderecosIgualLead.ToUpper() == "N")
                    {
                        if (request.EnderecoCliente == null)
                        {
                            throw new Exception("Os dados do endereço do cliente não foram informados");
                        }

                        if (String.IsNullOrEmpty(request.EnderecoCliente.Logradouro))
                        {
                            throw new Exception("O logradouro do endereço do cliente não foi informado");
                        }

                        if (String.IsNullOrEmpty(request.EnderecoCliente.Numero))
                        {
                            throw new Exception("O número do endereço do cliente não foi informado");
                        }

                        if (String.IsNullOrEmpty(request.EnderecoCliente.Bairro))
                        {
                            throw new Exception("O bairro do endereço do cliente não foi informado");
                        }

                        if (String.IsNullOrEmpty(request.EnderecoCliente.Cidade))
                        {
                            throw new Exception("A cidade do endereço do cliente não foi informada");
                        }

                        if (String.IsNullOrEmpty(request.EnderecoCliente.UF))
                        {
                            throw new Exception("A UF do endereço do cliente não foi informada");
                        }

                        if (request.EnderecoInstalacao == null)
                        {
                            throw new Exception("Os dados do endereço de instalação não foram informados");
                        }

                        if (String.IsNullOrEmpty(request.EnderecoInstalacao.Logradouro))
                        {
                            throw new Exception("O logradouro do endereço do instalação não foi informado");
                        }

                        if (String.IsNullOrEmpty(request.EnderecoInstalacao.Numero))
                        {
                            throw new Exception("O número do endereço do instalação não foi informado");
                        }

                        if (String.IsNullOrEmpty(request.EnderecoInstalacao.Bairro))
                        {
                            throw new Exception("O bairro do endereço do instalação não foi informado");
                        }

                        if (String.IsNullOrEmpty(request.EnderecoInstalacao.Cidade))
                        {
                            throw new Exception("A cidade do endereço do instalação não foi informada");
                        }

                        if (String.IsNullOrEmpty(request.EnderecoInstalacao.UF))
                        {
                            throw new Exception("A UF do endereço do instalação não foi informada");
                        }
                    }

                    IVendaRepository repoVenda = new VendaRepository(sessionNh);
                    IFuncionarioRepository repoFuncionario = new FuncionarioRepository(sessionNh);
                    IDepartamentoRepository repoDepto = new DepartamentoRepository(sessionNh);

                    IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(sessionNh);
                    IUsuarioRepository repoUsuario = new UsuarioRepository(sessionNh);

                    IClienteRepository repoCliente = new ClienteRepository(sessionNh);
                    IEnderecoClienteRepository repoEnderecoCliente = new EnderecoClienteRepository(sessionNh);
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
                    IVencimentoRepository repoVencimento = new VencimentoRepository(sessionNh);
                    IFormaPagamentoRepository repoFormaPagamento = new FormaPagamentoRepository(sessionNh);
                    IComboRepository repoCombo = new ComboRepository(sessionNh);
                    ICampanhaRepository repoCampanha = new CampanhaRepository(sessionNh);
                    IPlanoRepository repoPlano = new PlanoRepository(sessionNh);
                    IProdutoRepository repoProduto = new ProdutoRepository(sessionNh);
                    IPool_IpRepository repoPoolIp = new Pool_IpRepository(sessionNh);
                    IModeloContratoRepository repoModeloContrato = new ModeloContratoRepository(sessionNh);
                    IClienteContratoRepository repoClienteContrato = new ClienteContratoRepository(sessionNh);
                    IClientePlanoProdutoRepository repoClientePlanoProduto = new ClientePlanoProdutoRepository(sessionNh);
                    IComboPlanoProdutoRepository repoComboPlanoProdutoRepository = new ComboPlanoProdutoRepository(sessionNh);
                    IWorkflowRepository workflowRepository = new WorkflowRepository(sessionNh);

                    var etapaVendaBusiness = new EtapaVendaBusiness(repoEtapaVenda, repoConfig, repoCliente, request.IdCliente.Value);
                    var clienteBusiness = new ClienteBusiness(repoCliente);
                    var enderecoLogradouroBusiness = new EnderecoLogradouroBusiness(repoEnderecoLogradouro);
                    var configBusiness = new ConfiguracoesGeraisBusiness(repoConfig);

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

                    var venda = new Venda();
                    Cliente cliente = clienteBusiness.GetById(request.IdCliente.Value);

                    if (cliente == null)
                    {
                        throw new Exception("O cliente de ID " + request.IdCliente.Value + " não foi encontrado");
                    }

                    if (String.IsNullOrEmpty(request.CpfCnpj) && String.IsNullOrEmpty(cliente.CpfCnpj))
                    {
                        throw new Exception("O CPF/CNPJ do cliente de ID " + request.IdCliente.Value + " não foi informado");
                    }

                    if (request.EnderecosIgualLead.ToUpper() == "S")
                    {

                        if (cliente.Endereco_Prospecto == null)
                        {
                            throw new Exception("Endereço do prospecto não encontrado.");
                        }
                        if (cliente.Endereco_Prospecto.Cidade == null && cliente.Endereco_Prospecto?.Endereco_Logradouro?.Cidade?.Id == null)
                        {
                            throw new Exception("Endereço do prospecto Cidade não encontrado.");
                        }
                        if (cliente.Endereco_Prospecto.Bairro == null && cliente.Endereco_Prospecto?.Endereco_Logradouro?.Bairro?.Id == null)
                        {
                            throw new Exception("Endereço do prospecto Bairro não encontrado.");
                        }

                    }

                    Configuracoes_Gerais config = configBusiness.GetConfiguracoesGerais();

                    //Não permite venda para clientes com vendas em aberto
                    if (!config.Permite_Venda_Aberta_Cliente)
                    {
                        if (cliente != null)
                        {
                            IList<Venda> vendasExistentesCliente = repository.select(p =>
                            (p.Id_Cliente == cliente.Id) &&
                            p.Status != Status_Venda.Venda_Cancelada &&
                            p.Status != Status_Venda.Venda_Aprovada).Result;

                            if (vendasExistentesCliente.Count > 0)
                            {
                                throw new Exception(@$"Existem {vendasExistentesCliente.Count} vendas em aberto para o mesmo Cliente!");
                            }
                        }
                    }

                    VendaDadosGeraisViewModel vendaDadosGerais =
                        await this.GetDadosGeraisFromRequest(request, cliente, config, enderecoLogradouroBusiness, repoCliente);

                    if (request.IdVendedor.HasValue)
                    {
                        Funcionario funcionario = repoFuncionario.GetById(request.IdVendedor.Value);
                        IdentityUser usuarioVendedor = repoUsuario.GetAll().Where(p => p.Funcionario.Id == funcionario.Id).FirstOrDefault();
                        vendaDadosGerais.Id_Responsavel = usuarioVendedor?.Id;

                        if(string.IsNullOrEmpty(vendaDadosGerais.Id_Setor_Departamento))
                            vendaDadosGerais.Id_Setor_Departamento = Convert.ToString(usuarioVendedor?.Departamento?.Id);
                    }

                    venda = vendaBusiness.BindDadosGeraisVenda(venda, cliente, vendaDadosGerais, null);
                    venda.UsuarioCriacao = request.settings.user;

                    repoCliente.Update(cliente);
                    vendaBusiness.Save(venda);

                    // A gravação das etapas da venda deverá sempre acontecer após a gravação inicial da venda

                    IEnumerable<EtapaVendaViewModel> etapasVenda = etapaVendaBusiness.GetDadosEtapasVenda(venda.Id);

                    foreach (EtapaVendaViewModel etapa in etapasVenda)
                    {
                        EtapaVenda etapaVenda = new EtapaVenda
                        {
                            DataHoraCriacao = DateTime.Now,
                            Id = etapa.Id,
                            Etapa = (Sinapse.Model.enumerados.EnumEtapaVenda)etapa.Etapa,
                            Descricao = etapa.Descricao,
                            MotivoAprovManual = etapa.MotivoAprovacaoManual,
                            // A etapa de dados gerais é concluída durante a primeira gravação da venda
                            Status = etapa.Etapa == (int)Sinapse.Model.enumerados.EnumEtapaVenda.DadosGerais ? Sinapse.Model.enumerados.StatusEtapaVenda.Aprovado : Sinapse.Model.enumerados.StatusEtapaVenda.Pendente,
                            Venda = venda
                        };

                        etapaVendaBusiness.Save(etapaVenda);
                    }

                    retorno.Ok = true;
                    retorno.IdObjeto = venda.Id;
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    CONSTANTES.log4Net.ErrorFormat("message : {0} \r\n InnerException : {1} StackTrace : {2}\r\n",
                                                ex.Message,
                                                ex?.InnerException?.Message,
                                                ex.StackTrace);
                    retorno.Ok = false;
                    retorno.Mensagem = "Erro ao gravar a proposta de venda: " + ex.Message;
                }
            }

            return retorno;
        }

        private async Task<VendaDadosGeraisViewModel> GetDadosGeraisFromRequest(
            PropostaVendaApiRequest request,
            Cliente cliente,
            Configuracoes_Gerais config,
            EnderecoLogradouroBusiness enderecoLogradouroBusiness,
            IClienteRepository repoCliente)
        {
            var vendaDadosGerais = new VendaDadosGeraisViewModel();

            var cpfCnpj = request.CpfCnpj;
            if (String.IsNullOrEmpty(cpfCnpj) == false)
                cpfCnpj = cpfCnpj.Replace(".", "").Replace("-", "").Replace("/", "");

            if (!String.IsNullOrEmpty(cpfCnpj))
            {
                var clienteCadastrado = repoCliente.GetByCpfCnpj(cpfCnpj);
                
                if (clienteCadastrado != null && clienteCadastrado.Id != cliente.Id)                
                    throw new Exception("O CPF/CNPJ informado já está cadastrado para outro cliente.");
                
            }

            vendaDadosGerais.Id_Vendedor = request.IdVendedor;
            vendaDadosGerais.Id_Supervisor = request.IdSupervisor;
            vendaDadosGerais.Id_Setor_Departamento = Convert.ToString(request.IdSetorDepartamento);

            vendaDadosGerais.Id_Cliente = request.IdCliente.Value;
            vendaDadosGerais.CpfCnpj = !String.IsNullOrEmpty(cpfCnpj) ? cpfCnpj : cliente.CpfCnpj;
            vendaDadosGerais.TipoPessoa = request.TipoPessoa.HasValue ? request.TipoPessoa : cliente.Tipo_Pessoa;

            vendaDadosGerais.Representante =
                !String.IsNullOrEmpty(request.Representante) ?
                    request.Representante :
                    cliente.Nome_Resp_Financeiro;

            vendaDadosGerais.Nacionalidade =
                !String.IsNullOrEmpty(request.Nacionalidade) ?
                    request.Nacionalidade :
                    cliente.Nacionalidade;

            vendaDadosGerais.Sexo = request.Sexo.HasValue ? request.Sexo : cliente.Sexo;

            if (!String.IsNullOrEmpty(request.DataNascimento))
            {
                vendaDadosGerais.DataNascimento = request.DataNascimento;
            }
            else if (cliente.Data_Nascimento.HasValue)
            {
                vendaDadosGerais.DataNascimento = cliente.Data_Nascimento.Value.ToString("dd/MM/yyyy");
            }

            vendaDadosGerais.EstadoCivil = request.EstadoCivil.HasValue ? request.EstadoCivil : cliente.Estado_Civil;
            vendaDadosGerais.RG = !String.IsNullOrEmpty(request.RG) ? request.RG : cliente.Rg;
            vendaDadosGerais.Orgao =
                !String.IsNullOrEmpty(request.OrgaoEmissor) ?
                    request.OrgaoEmissor :
                    cliente.Orgao_Emissao_Rg;

            if (!String.IsNullOrEmpty(request.DataEmissao))
            {
                vendaDadosGerais.DataEmissao = request.DataEmissao;
            }
            else if (cliente.Data_Emissao_Rg.HasValue)
            {
                vendaDadosGerais.DataEmissao = cliente.Data_Emissao_Rg.Value.ToString("dd/MM/yyyy");
            }

            vendaDadosGerais.Instrucao = request.Instrucao.HasValue ? request.Instrucao : cliente.Grau_Instrucao;
            vendaDadosGerais.Mae = !String.IsNullOrEmpty(request.Mae) ? request.Mae : cliente.Filiacao_Mae;
            vendaDadosGerais.Pai = !String.IsNullOrEmpty(request.Pai) ? request.Pai : cliente.Filiacao_Pai;

            vendaDadosGerais.Profissao = !String.IsNullOrEmpty(request.Profissao) ? request.Profissao : cliente.Profissao;
            vendaDadosGerais.Email = !String.IsNullOrEmpty(request.Email) ? request.Email : cliente.Email_Principal;

            vendaDadosGerais.TelefoneFixo =
                !String.IsNullOrEmpty(request.TelefoneFixo) ?
                    request.TelefoneFixo :
                    cliente.Telefone_Fixo;

            vendaDadosGerais.TelefoneCelular =
                !String.IsNullOrEmpty(request.TelefoneCelular) ?
                    request.TelefoneCelular :
                    cliente.Telefone_Celular;

            vendaDadosGerais.TelefoneRecado =
                !String.IsNullOrEmpty(request.TelefoneRecado) ?
                    request.TelefoneRecado :
                    cliente.Telefone_Recado;

            vendaDadosGerais.NomeRecado =
                !String.IsNullOrEmpty(request.NomeRecado) ?
                    request.NomeRecado :
                    cliente.Pessoa_Tel_Recado;

            vendaDadosGerais.Id_Categoria_Cliente =
                request.IdCategoriaCliente.HasValue ?
                    request.IdCategoriaCliente :
                    cliente.Categoria_Cliente?.Id;

            vendaDadosGerais.Id_Quem_Indicou =
                request.IdQuemIndicou.HasValue ?
                    request.IdQuemIndicou :
                    cliente.Quem_Indicou?.Id;

            vendaDadosGerais.Id_Meio_Captacao =
                request.IdMeioCaptacao.HasValue ?
                    request.IdMeioCaptacao :
                    cliente.Qual_Meio?.Id;

            vendaDadosGerais.Numero_Contrato =
                !String.IsNullOrEmpty(request.NumeroContrato) ?
                    request.NumeroContrato :
                    cliente.Numero_Contrato;

            vendaDadosGerais.Id_Campanha_Venda = request.IdCampanha.HasValue ? request.IdCampanha : cliente.IdCampanha;
            if (vendaDadosGerais.TipoPessoa == Tipo_Pessoa.Fisica)
            {
                vendaDadosGerais.NomeCompleto =
                    !String.IsNullOrEmpty(request.NomeCompleto) ?
                        request.NomeCompleto :
                        cliente.NomeFantasia;
            }
            else
            {
                vendaDadosGerais.NomeFantasia =
                    !String.IsNullOrEmpty(request.NomeFantasia) ?
                        request.NomeFantasia :
                        cliente.NomeFantasia;
            }

            if (!String.IsNullOrEmpty(request.EnderecosIgualLead) && request.EnderecosIgualLead.ToUpper() == "S")
            {

                if (!config.Utiliza_Cadastro_Endereco)
                {
                    var enderecoLead = new Aniel.Connect.Repository.Model.EnderecoClienteViewModel
                    {
                        Id_Pais = cliente.Endereco_Prospecto?.Pais?.Id,
                        Pais = cliente.Endereco_Prospecto?.Pais?.Nome,

                        Id_UF = cliente.Endereco_Prospecto?.Id_UF,
                        UF = cliente.Endereco_Prospecto?.UF,

                        Id_Cidade = cliente.Endereco_Prospecto?.Id_Cidade,
                        Cidade = cliente.Endereco_Prospecto?.Cidade,

                        Id_Bairro = cliente.Endereco_Prospecto?.Id_Bairro,
                        Bairro = cliente.Endereco_Prospecto?.Bairro,

                        CEP = cliente.Endereco_Prospecto?.Cep,

                        Logradouro = cliente.Endereco_Prospecto?.Logradouro,
                        Numero = cliente.Endereco_Prospecto?.Numero,
                        Complemento = cliente.Endereco_Prospecto?.Complemento,
                        Ponto_Referencia = cliente.Endereco_Prospecto?.Ponto_Referencia,

                        Latitude =
                        (cliente.Endereco_Prospecto?.Latitude).HasValue ?
                            Convert.ToString(cliente.Endereco_Prospecto?.Latitude.Value) :
                            null,
                        Longitude =
                        (cliente.Endereco_Prospecto?.Longitude).HasValue ?
                            Convert.ToString(cliente.Endereco_Prospecto?.Longitude.Value) :
                            null,

                        Cod_Escritorio = cliente.Endereco_Prospecto?.Cod_Escritorio,
                        Cod_Area = cliente.Endereco_Prospecto?.Cod_Area,
                        Cod_Localidade =
                        (cliente.Endereco_Prospecto?.Cod_Localidade).HasValue ?
                            Convert.ToString(cliente.Endereco_Prospecto?.Cod_Localidade.Value) :
                            null,

                        Id_Tipo_Imovel = cliente.Endereco_Prospecto?.Tipo_Imovel?.Cod_Tipo_Imovel
                    };
                    vendaDadosGerais.EnderecoPrincipal = enderecoLead;
                    vendaDadosGerais.EnderecoInstalacao = enderecoLead;

                }
                else
                {
                    var enderecoLead = new Aniel.Connect.Repository.Model.EnderecoClienteViewModel
                    {
                        Id_Pais = cliente.Endereco_Prospecto?.Endereco_Logradouro?.Pais?.Id,
                        Pais = cliente.Endereco_Prospecto?.Endereco_Logradouro?.Pais?.Nome,
                        Id_UF = cliente.Endereco_Prospecto?.Endereco_Logradouro?.UF?.Id,
                        UF = cliente.Endereco_Prospecto?.Endereco_Logradouro?.UF?.Sigla,
                        Id_Cidade = cliente.Endereco_Prospecto?.Endereco_Logradouro?.Cidade?.Id,
                        Cidade = cliente.Endereco_Prospecto?.Endereco_Logradouro?.Cidade?.Descricao,
                        Id_Bairro = cliente.Endereco_Prospecto?.Endereco_Logradouro?.Bairro?.Id,
                        Bairro = cliente.Endereco_Prospecto?.Endereco_Logradouro?.Bairro?.Descricao,
                        CEP = cliente.Endereco_Prospecto?.Endereco_Logradouro?.Cep,
                        Logradouro = cliente.Endereco_Prospecto?.Endereco_Logradouro?.Descricao,
                        Numero = cliente.Endereco_Prospecto?.Numero,
                        Complemento = cliente.Endereco_Prospecto?.Complemento,
                        Ponto_Referencia = cliente.Endereco_Prospecto?.Ponto_Referencia,
                        Latitude =
                        (cliente.Endereco_Prospecto?.Latitude).HasValue ?
                            Convert.ToString(cliente.Endereco_Prospecto?.Latitude.Value) :
                            null,
                        Longitude =
                        (cliente.Endereco_Prospecto?.Longitude).HasValue ?
                            Convert.ToString(cliente.Endereco_Prospecto?.Longitude.Value) :
                            null,

                        Cod_Escritorio = cliente.Endereco_Prospecto?.Endereco_Logradouro?.Cod_Escritorio ,
                        Cod_Area = cliente.Endereco_Prospecto?.Endereco_Logradouro?.Cod_Area,
                        Cod_Localidade = cliente.Endereco_Prospecto?.Endereco_Logradouro?.Cod_Localidade,

                        Id_Tipo_Imovel = cliente.Endereco_Prospecto?.Tipo_Imovel?.Cod_Tipo_Imovel
                    };

                    enderecoLead.Busca_Endereco = Busca_Endereco.Endereco_Logradouro;
                    enderecoLead.Id_Endereco_Logradouro = cliente.Endereco_Prospecto?.Endereco_Logradouro?.Id;
                    vendaDadosGerais.EnderecoPrincipal = enderecoLead;
                    vendaDadosGerais.EnderecoInstalacao = enderecoLead;
                }

                
            }
            else
            {
                vendaDadosGerais.EnderecoPrincipal = new Aniel.Connect.Repository.Model.EnderecoClienteViewModel
                {
                    Pais = request.EnderecoCliente.Pais,
                    UF = request.EnderecoCliente.UF,
                    Cidade = request.EnderecoCliente.Cidade,
                    Bairro = request.EnderecoCliente.Bairro,
                    CEP = request.EnderecoCliente.CEP,

                    Logradouro = request.EnderecoCliente.Logradouro,
                    Numero = request.EnderecoCliente.Numero,
                    Complemento = request.EnderecoCliente.Complemento,
                    Ponto_Referencia = request.EnderecoCliente.PontoReferencia,

                    Latitude = request.EnderecoCliente.Latitude,
                    Longitude = request.EnderecoCliente.Longitude,

                    Cod_Escritorio = request.EnderecoCliente.CodEscritorio,
                    Cod_Area = request.EnderecoCliente.CodArea,
                    Cod_Localidade = request.EnderecoCliente.CodLocalidade,

                    Id_Tipo_Imovel = request.EnderecoCliente.CodTipoImovel
                };

                vendaDadosGerais.EnderecoInstalacao = new Aniel.Connect.Repository.Model.EnderecoClienteViewModel
                {
                    Pais = request.EnderecoInstalacao.Pais,
                    UF = request.EnderecoInstalacao.UF,
                    Cidade = request.EnderecoInstalacao.Cidade,
                    Bairro = request.EnderecoInstalacao.Bairro,
                    CEP = request.EnderecoInstalacao.CEP,

                    Logradouro = request.EnderecoInstalacao.Logradouro,
                    Numero = request.EnderecoInstalacao.Numero,
                    Complemento = request.EnderecoInstalacao.Complemento,
                    Ponto_Referencia = request.EnderecoInstalacao.PontoReferencia,

                    Latitude = request.EnderecoInstalacao.Latitude,
                    Longitude = request.EnderecoInstalacao.Longitude,

                    Cod_Escritorio = request.EnderecoInstalacao.CodEscritorio,
                    Cod_Area = request.EnderecoInstalacao.CodArea,
                    Cod_Localidade = request.EnderecoInstalacao.CodLocalidade,

                    Id_Tipo_Imovel = request.EnderecoInstalacao.CodTipoImovel
                };

                if (config.Utiliza_Cadastro_Endereco)
                {
                    Endereco_Logradouro enderecoLogradouroPrincipal =
                        enderecoLogradouroBusiness.GetByNomes(
                            request.EnderecoCliente.UF,
                            request.EnderecoCliente.Cidade,
                            request.EnderecoCliente.Bairro,
                            request.EnderecoCliente.Logradouro
                        );

                    vendaDadosGerais.EnderecoPrincipal.Busca_Endereco = Busca_Endereco.Endereco_Logradouro;
                    vendaDadosGerais.EnderecoPrincipal.Id_Endereco_Logradouro = enderecoLogradouroPrincipal?.Id;

                    Endereco_Logradouro enderecoLogradouroInstalacao =
                        enderecoLogradouroBusiness.GetByNomes(
                            request.EnderecoInstalacao.UF,
                            request.EnderecoInstalacao.Cidade,
                            request.EnderecoInstalacao.Bairro,
                            request.EnderecoInstalacao.Logradouro
                        );

                    vendaDadosGerais.EnderecoInstalacao.Busca_Endereco = Busca_Endereco.Endereco_Logradouro;
                    vendaDadosGerais.EnderecoInstalacao.Id_Endereco_Logradouro = enderecoLogradouroInstalacao?.Id;
                }
            }

            return vendaDadosGerais;
        }



        [HttpPost]
        [Route("api/VendaApi/GetAll")]
        public async Task<ApiResponseObj<List<VendaListViewModel>>> GetAll(VendaRequest request)
        {
            List<VendaListViewModel> retorno = new List<VendaListViewModel>();

            using (var api = new UsuarioApiController())
            {
                if (!(await api.GetUsuarioValido(request.settings)).Ok)
                {
                    throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                }
            }

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (var com = session.Connection.CreateCommand())
                {

                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sql = new StringBuilder();
                    sql.AppendLine(" SELECT A.ID, B.NOMEFANTASIA, A.TERMO_ADESAO_ASSINADO, A.STATUS_LIBERACAO,A.STATUS AS STATUS_VENDA,A.DATA_VENDA, ");
                    sql.AppendLine(" B.CPF, B.TIPOPESSOA, B.TIPO_CLIENTE, B.TIPOPESSOA, B.TELEFONE_FIXO, B.TELEFONE_CELULAR, B.EMAIL_PRINCIPAL, B.NUMERO_CONTRATO AS NUMERO_CONTRATO, ");
                    sql.AppendLine(" C.NOME AS VENDEDOR, D.ENDERECO_BAIRRO, D.ENDERECO_CIDADE, D.ENDERECO_UF, D.ENDERECO_LOGRADOURO, D.ENDERECO_NUMERO, D.ENDERECO_COMPLEMENTO, ");
                    sql.AppendLine(" A.ID_CLIENTE, MAX(L.DATA_ULTIMO_CONTATO) AS DATA_ULTIMO_CONTATO, I.NOME AS SUPERVISOR, J.DESCRICAO AS MEIO_CAPTACAO, K.NOMEFANTASIA AS QUEM_INDICOU, ");
                    sql.AppendLine(" M.DESCRICAO AS DESC_TIPO_IMOVEL, V.DESCRICAO  AS EQUIPEVENDA,A.TERMO_ADESAO_ASSINADO,O.DESCRICAO AS DEPARTAMENTO,A.STATUS_LIBERACAO,A.DATAHORACRIACAO ");

                    sql.AppendLine(" FROM TB_VENDA A  ");
                    sql.AppendLine(" LEFT OUTER JOIN TB_CLIENTE B ");
                    sql.AppendLine(" ON(A.ID_CLIENTE = B.ID) ");

                    sql.AppendLine(" LEFT OUTER JOIN TB_FUNCIONARIO C ");
                    sql.AppendLine(" ON(A.ID_VENDEDOR = C.ID) ");

                    sql.AppendLine(" LEFT OUTER JOIN TB_ENDERECO_CLIENTE D ");
                    sql.AppendLine(" ON(B.ID_ENDERECO_PRINCIPAL = D.ID) ");

                    sql.AppendLine("LEFT JOIN TB_EQUIPE_VENDA_SUPERVISOR S");
                    sql.AppendLine("ON (S.ID_SUPERVISOR = A.ID)");

                    sql.AppendLine(" LEFT JOIN bom aTB_EQUIPE_VENDA V");
                    sql.AppendLine(" ON (V.ID = S.ID_EQUIPE_VENDA)");

                    sql.AppendLine(" LEFT JOIN TB_VENCIMENTO E ");
                    sql.AppendLine(" ON(B.ID_VENCIMENTO = E.ID) ");

                    sql.AppendLine(" LEFT JOIN TB_FUNCIONARIO I ");
                    sql.AppendLine(" ON(I.ID = A.ID_SUPERVISOR) ");

                    sql.AppendLine(" LEFT JOIN TB_MEIO_CAPTACAO J ");
                    sql.AppendLine(" ON(B.ID_MEIO_CAPTACAO = J.ID) ");

                    sql.AppendLine(" LEFT JOIN TB_CLIENTE K ");
                    sql.AppendLine(" ON(K.ID = B.ID_QUEM_INDICOU) ");

                    sql.AppendLine(" LEFT JOIN TB_VENDA_HISTORICO_CONTATO L");
                    sql.AppendLine(" ON(A.ID = L.ID_VENDA)");

                    sql.AppendLine(" LEFT JOIN TB_TIPO_IMOVEL M ");
                    sql.AppendLine(" ON(M.ID = D.ID_TIPO_IMOVEL) ");

                    sql.AppendLine(" LEFT JOIN TB_CATEGORIA_CLIENTE N ");
                    sql.AppendLine(" ON(N.ID = B.ID_CATEGORIA) ");

                    sql.AppendLine(" LEFT JOIN TB_DEPARTAMENTO O ");
                    sql.AppendLine(" ON(O.ID = A.ID_DEPARTAMENTO) ");


                    StringBuilder sqlWhere = new StringBuilder();

                    #region filtros


                    if (!String.IsNullOrEmpty(request.CpfCnpj))
                    {
                        sqlWhere.AppendLine("AND (A.CPF = @CPF)");
                        parametros.Add("@CPF", Convert.ToString(request.CpfCnpj));
                    }

                    if (!String.IsNullOrEmpty(request.Cidade))
                    {
                        sqlWhere.AppendLine("AND (B.ENDERECO_CIDADE = @ENDERECO_CIDADE)");
                        parametros.Add("@ENDERECO_BAIRRO", Convert.ToString(request.Cidade));
                    }

                    if (!String.IsNullOrEmpty(request.Bairro))
                    {
                        sqlWhere.AppendLine(" AND (UPPER(D.ENDERECO_UF) = @UF) ");
                        parametros.Add("@UF", request.UF);
                    }

                    if (!String.IsNullOrEmpty(request.UF))
                    {
                        sqlWhere.AppendLine(" AND (UPPER(D.ENDERECO_UF) = @UF) ");
                        parametros.Add("@UF", request.UF);
                    }

                    if (!String.IsNullOrEmpty(request.DataVenda))//ok
                    {
                        sqlWhere.AppendLine(" AND (UPPER(A.DATA_VENDA = @DATA_VENDA) ");
                        parametros.Add("@DATA_VENDA", request.DataVenda);
                    }

                    if (request.Data_Criacao != null)
                    {
                        sqlWhere.AppendLine("AND (A.DATAHORACRIACAO = @DATAHORACRIACAO)");
                        parametros.Add("@DATAHORACRIACAO", Convert.ToString(request.Data_Criacao));
                    }

                    #endregion

                    sql.AppendLine(" GROUP BY A.ID, B.NUMERO_CONTRATO , B.NOMEFANTASIA, B.CPF, B.TIPOPESSOA, B.TIPO_CLIENTE, D.ENDERECO_BAIRRO, N.DESCRICAO, N.ID,A.ID_DEPARTAMENTO, ");
                    sql.AppendLine(" D.ID_CIDADE, M.ID , DESC_TIPO_IMOVEL, D.ENDERECO_CIDADE, D.ENDERECO_UF ,B.TIPOPESSOA, B.TELEFONE_FIXO,C.NOME, ");
                    sql.AppendLine(" I.NOME, A.ID_CAMPANHA, J.DESCRICAO, B.ID_MEIO_CAPTACAO ,A.STATUS_AUDITORIA,");
                    sql.AppendLine(" A.STATUS, B.TELEFONE_FIXO, B.TELEFONE_CELULAR, B.EMAIL_PRINCIPAL, D.ENDERECO_LOGRADOURO, ");
                    sql.AppendLine(" D.ENDERECO_NUMERO, D.ENDERECO_COMPLEMENTO, K.NOMEFANTASIA, A.DATAHORACRIACAO, A.DATA_VENDA,");
                    sql.AppendLine(" B.NUMERO_CONTRATO, A.ID_CLIENTE, A.ID_VENDEDOR, A.ID_SUPERVISOR, A.TERMO_ADESAO_ASSINADO, A.USUARIOCRIACAO,O.DESCRICAO,A.STATUS_LIBERACAO,V.DESCRICAO  ");
                    sql.AppendLine(" HAVING A.ID > 1 ");


                    using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                    {
                        while (await reader.ReadAsync())
                        {
                            retorno.Add(new VendaListViewModel
                            {
                                Id = Convert.ToInt64(reader["ID"]),
                                Pendencia = false,
                                Supervisor = Convert.ToString(reader["SUPERVISOR"]),
                                Vendedor = Convert.ToString(reader["VENDEDOR"]),
                                Departamento = Convert.ToString(reader["DEPARTAMENTO"]),
                                Tipo_Cliente = HtmlEnumExtensions.GetEnumDescription<Tipo_Cliente>(Convert.ToString(reader["TIPO_CLIENTE"])),
                                Tipo_Pessoa = HtmlEnumExtensions.GetEnumDescription<Tipo_Pessoa>(Convert.ToString(reader["TIPOPESSOA"])),
                                CpfCnpj = Convert.ToString(reader["CPF"]),
                                Identificacao_Cliente = Convert.ToString(reader["NUMERO_CONTRATO"]),

                                Bairro = Convert.ToString(reader["ENDERECO_BAIRRO"]),
                                Cidade = Convert.ToString(reader["ENDERECO_CIDADE"]),
                                Estado = Convert.ToString(reader["ENDERECO_UF"]),
                                Logradouro = Convert.ToString(reader["ENDERECO_LOGRADOURO"]),
                                Numero = Convert.ToString(reader["ENDERECO_NUMERO"]),
                                Complemento = Convert.ToString(reader["ENDERECO_COMPLEMENTO"]),

                                DiasSemContato = reader["DATA_ULTIMO_CONTATO"] != DBNull.Value ? Convert.ToString(DateTime.Now.Subtract(Convert.ToDateTime(reader["DATA_ULTIMO_CONTATO"])).Days) : null,
                                NomeFantasia = Convert.ToString(reader["NOMEFANTASIA"]),
                                TelefonePrincipal = Convert.ToString(reader["TELEFONE_FIXO"]),
                                TelefoneCelular = Convert.ToString(reader["TELEFONE_CELULAR"]),
                                Email = Convert.ToString(reader["EMAIL_PRINCIPAL"]),
                                Tipo_Imovel = Convert.ToString(reader["DESC_TIPO_IMOVEL"]),
                                Data_Hora_Criacao = Convert.ToString(reader["DATAHORACRIACAO"]),
                                Data_Expiracao = null,
                                Avaliacao_Expirada = null,

                                DataVenda = reader["DATA_VENDA"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_VENDA"]).ToString("dd/MM/yyyy") : null,
                                DataUltimoContato = reader["DATA_ULTIMO_CONTATO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_ULTIMO_CONTATO"]).ToString("dd/MM/yyyy HH:mm") : null,
                                Quem_Indicou = Convert.ToString(reader["QUEM_INDICOU"]),
                                Categoria_Cliente = Convert.ToString(reader["CATEGORIA_CLIENTE"]),
                                EquipeVenda = Convert.ToString(reader["EQUIPEVENDA"]),
                                Status = reader["STATUS_LIBERACAO"] == DBNull.Value || Convert.ToInt64(reader["STATUS_LIBERACAO"]) == 6 ? HtmlEnumExtensions.GetEnumDescription((Status_Venda)reader["STATUS_VENDA"]) : HtmlEnumExtensions.GetEnumDescription((Status_Venda)reader["STATUS_LIBERACAO"]),

                                Id_Cliente = reader["ID_CLIENTE"] != DBNull.Value ? Convert.ToInt64(reader["ID_CLIENTE"]) : 0,

                                Termo_Adesao_Assinado = HtmlEnumExtensions.GetEnumDescription<Sinapse.Model.enumerados.Padrao>(Convert.ToString(reader["TERMO_ADESAO_ASSINADO"] != DBNull.Value ? reader["TERMO_ADESAO_ASSINADO"] : 0)),

                            });
                        }
                    }
                }
                return new ApiResponseObj<List<VendaListViewModel>>
                {
                    ok = true,
                    data = retorno,
                    error = ""
                };

            }
            catch (Exception ex)
            {
                return new ApiResponseObj<List<VendaListViewModel>>
                {
                    ok = false,
                    error = ex.Message
                };
            }
        }
    }
}
