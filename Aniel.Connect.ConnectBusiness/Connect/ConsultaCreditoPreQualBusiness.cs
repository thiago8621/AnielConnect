using Aniel.Connect.ConnectBusiness.Exceptions.ConsultaCredito;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Cliente;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.ConsultaCredito;
using Aniel.Connect.Repository.CriteriosParamCredito;
using Aniel.Connect.Repository.IntegracaoExterna;
using Aniel.Connect.Repository.ParametrosConsultaCredito;
using Aniel.Connect.Repository.ParametrosConsultaPreQual;
using Aniel.Connect.Repository.Venda;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.SerasaApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aniel.Connect.ConnectBusiness.Connect
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="token">campo token é utilizado como domain para a Age </param>
    public class ConsultaCreditoPreQualBusiness
    {
        private readonly IConsultaCreditoRepository _repository;
        private readonly ICriteriosParamCreditoRepository _repoCriterios;
        private readonly IParametrosConsultaCreditoRepository _repoParamsConsulta;
        private readonly IParametrosConsultaPreQualRepository _repoParamsConsultaPrequal;

        private readonly IConfiguracoesGeraisRepository _repoConfig;
        private readonly IVendaRepository _repoVenda;
        private readonly IIntegracaoExternaRepository _repoIntegracao;
        private readonly IClienteRepository repoCliente;

        private readonly string _usuarioApiSerasa;
        private readonly string _senhaApiSerasa;
        private readonly string _urlApiSerasa;
        private readonly string _token;
        private readonly string _domain;
        private readonly string _header;
        private readonly string _apiKey;
        private readonly string _apiSecret;



        public ConsultaCreditoPreQualBusiness(
            IConsultaCreditoRepository repository,
            ICriteriosParamCreditoRepository repoCriterios,
            IParametrosConsultaCreditoRepository repoParamsConsulta,
            IParametrosConsultaPreQualRepository repoParamsConsultaPrequal,
            IConfiguracoesGeraisRepository repoConfig,
            IIntegracaoExternaRepository repoIntegracao,
            IClienteRepository repoCliente,
            IVendaRepository repoVenda,
            string usuarioApiSerasa,
            string senhaApiSerasa,
            string urlApiSerasa,
            //string token,
            string apiKey,
            string apiSecret,
            string domain,
            string header
            )
        {
            this._repository = repository;
            this._repoCriterios = repoCriterios;
            this._repoParamsConsulta = repoParamsConsulta;
            this._repoParamsConsultaPrequal = repoParamsConsultaPrequal;
            this.repoCliente = repoCliente;
            this._repoConfig = repoConfig;
            this._repoIntegracao = repoIntegracao;
            this._repoVenda = repoVenda;


            if (String.IsNullOrEmpty(usuarioApiSerasa))
            {
                throw new ArgumentNullException("usuarioApiSerasa");
            }

            if (String.IsNullOrEmpty(senhaApiSerasa))
            {
                throw new ArgumentNullException("senhaApiSerasa");
            }
            if (String.IsNullOrEmpty(urlApiSerasa))
            {
                throw new ArgumentNullException("urlApiSerasa");
            }

            //if (String.IsNullOrEmpty(token))
            //{
            //    throw new ArgumentNullException("tokenApiSerasa");
            //}

            if (String.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentNullException("apiKeyApiSerasa");
            }
            if (String.IsNullOrEmpty(domain))
            {
                throw new ArgumentNullException("domainApiSerasa");
            }

            if (String.IsNullOrEmpty(header))
            {
                throw new ArgumentNullException("CorrelationIdApiSerasa");
            }

            this._usuarioApiSerasa = usuarioApiSerasa;
            this._senhaApiSerasa = senhaApiSerasa;
            this._urlApiSerasa = urlApiSerasa;
            //this._token = token;
            this._apiSecret = apiSecret;
            this._apiKey = apiKey;

            this._header = header;
            this._domain = domain;
        }

        public ConsultaCreditoPreQualBusiness(
            IConsultaCreditoRepository repository,
            ICriteriosParamCreditoRepository repoCriterios,
            IParametrosConsultaCreditoRepository repoParamsConsulta,
            IConfiguracoesGeraisRepository repoConfig)
        {
            this._repository = repository;
            this._repoCriterios = repoCriterios;
            this._repoParamsConsulta = repoParamsConsulta;
            this._repoConfig = repoConfig;
        }

        /// <summary>
        ///     Chama a API de consulta de crédito do Serasa PreQual para pessoa física, 
        ///     grava o resultado da consulta em banco e retorna a string da
        ///     consulta do Serasa.
        /// </summary>
        /// <param name="cpf">CPF da pessoa física que será consultada</param>
        /// <param name="usuario">Usuário vinculado ao Vendedor, em caso de Lead, usuário de criação do Lead</param>
        public async Task<ConsultaSerasaDados> ExecutarConsultaCreditoPF(string cpf, IdentityUser usuario, long? idVenda)
        {
            if (String.IsNullOrEmpty(this._usuarioApiSerasa))
            {
                throw new UsuarioApiSerasaNaoInformadoException();
            }

            if (String.IsNullOrEmpty(this._senhaApiSerasa))
            {
                throw new SenhaApiSerasaNaoInformadoException();
            }

            Cliente cliente = repoCliente.GetAll().FirstOrDefault(p => p.CpfCnpj == cpf);
            var paramPreQual = GetTipoIntegracaoPreQual(idVenda, cliente, usuario);
            string retornoConsultaSerasa =
                await new SerasaDadosPreQual().RetornoDadosCpf(
                    cpf,
                    this._usuarioApiSerasa,
                    this._senhaApiSerasa,
                    this._urlApiSerasa,
                    this._apiSecret,
                    this._apiKey,
                    this._domain,
                    this._header,
                    paramPreQual
                );

            if (String.IsNullOrEmpty(retornoConsultaSerasa))
            {
                throw new ConsultaSerasaSemRetornoException();
            }

            var consultaSerasa = new ConsultaSerasaDados();

            consultaSerasa.DataHoraCriacao = DateTime.Now;
            consultaSerasa.consulta = retornoConsultaSerasa;
            consultaSerasa.cpf = cpf;
            consultaSerasa.TipoIntegracao = IntegracaoExternaConsultaCredEnum.PREQUAL;
            consultaSerasa.ParamLoja = paramPreQual;

            this._repository.Save(consultaSerasa);

            return consultaSerasa;
        }

        /// <summary>
        ///     Chama a API de consulta de crédito do Serasa para pessoa jurídica, 
        ///     grava o resultado da consulta em banco e retorna a string da
        ///     consulta do Serasa.
        /// </summary>
        /// <param name="cnpj">CNPJ da pessoa jurídica que será consultada</param>
        public async Task<ConsultaSerasaDados> ExecutarConsultaCreditoPJ(string cnpj, IdentityUser usuario, long? idVenda)
        {
            if (String.IsNullOrEmpty(this._usuarioApiSerasa))
            {
                throw new UsuarioApiSerasaNaoInformadoException();
            }

            if (String.IsNullOrEmpty(this._senhaApiSerasa))
            {
                throw new SenhaApiSerasaNaoInformadoException();
            }

            Cliente cliente = repoCliente.GetAll().FirstOrDefault(p => p.CpfCnpj == cnpj);
            var paramPreQual = GetTipoIntegracaoPreQual(idVenda, cliente, usuario);
            string retornoConsultaSerasa = await new SerasaDadosPreQual().RetornoDadosCnpj(
                cnpj,
                this._usuarioApiSerasa,
                this._senhaApiSerasa,
                this._urlApiSerasa,
                this._apiSecret,
                this._apiKey,
                this._domain,
                this._header,
                paramPreQual
                );

            if (String.IsNullOrEmpty(retornoConsultaSerasa))
            {
                throw new ConsultaSerasaSemRetornoException();
            }

            var consultaSerasa = new ConsultaSerasaDados();

            consultaSerasa.DataHoraCriacao = DateTime.Now;
            consultaSerasa.consulta = retornoConsultaSerasa;
            consultaSerasa.cnpj = cnpj;
            consultaSerasa.TipoIntegracao = IntegracaoExternaConsultaCredEnum.PREQUAL;
            consultaSerasa.ParamLoja = paramPreQual;

            this._repository.Save(consultaSerasa);

            return consultaSerasa;
        }

        public string GetTipoIntegracaoPreQual(long? idVenda, Cliente cliente, IdentityUser usuario)
        {
            string tipo = null;

            if (usuario?.Funcionario?.ParametrosConsultaPreQual != null)
            {
                tipo = _repoParamsConsultaPrequal.GetAll().FirstOrDefault(p => p.Id == usuario.Funcionario.ParametrosConsultaPreQual.Id).Descricao;
            }
            else
            {
                if (cliente != null)
                {
                    var ativos = _repoParamsConsultaPrequal.GetAll().Where(p => p.Status == Status_Default.Ativo).ToList();
                    var venda = idVenda.HasValue ? this._repoVenda.GetById(idVenda.Value) : null;
                    var departamento = idVenda.HasValue ? venda.Departamento?.Cod_Depto : cliente?.Departamento?.Cod_Depto;

                    Func<ParametrosConsultaPreQual, Endereco_Cliente, List<ParametrosConsultaPreQualParamRegiao>> FilterParamPrequalbyEndereco = (parametro, endereco) =>
                    {
                        var regioes = new List<ParametrosConsultaPreQualParamRegiao>();
                        //String endereco.Condominio_Predio.Uf
                        //String endereco.Condominio_Predio.Bairro
                        long? uf = 0;
                        long? bairro = 0;

                        using (var session = HibernateUtil.GetSession())
                        using (var repoUF = new Conexao<Uf>(session))
                        using (var repoBairro = new Conexao<Bairro>(session))
                        {
                            switch (endereco.Busca_Endereco)
                            {
                                case Busca_Endereco.Endereco_Logradouro:
                                    uf = endereco.Endereco_Logradouro.UF.Id;
                                    bairro = endereco.Endereco_Logradouro.Bairro.Id;

                                    regioes.AddRange(parametro.ParamsRegiao.Where(q => q.CodUF == endereco.Endereco_Logradouro.UF.Id && q.CodCidade == endereco.Endereco_Logradouro.Cidade.Id && q.CodBairro == endereco.Endereco_Logradouro.Bairro.Id).ToList());
                                    break;

                                case Busca_Endereco.Condominio_Predio:

                                    uf = repoUF.selectFirst(record => record.Sigla == endereco.Condominio_Predio.Uf.ToUpper()).Result?.Id;
                                    bairro = repoBairro.selectFirst(record => record.Descricao == endereco.Condominio_Predio.Bairro && record.Cidade.Id == endereco.Condominio_Predio.Id_Cidade && record.UF.Id == uf ).Result?.Id;

                                    regioes.AddRange(parametro.ParamsRegiao.Where(q => q.CodUF == uf && q.CodCidade == endereco.Condominio_Predio.Id_Cidade && q.CodBairro == bairro).ToList());
                                    break;

                                default:
                                    uf = repoUF.selectFirst(record => record.Sigla == endereco.UF.ToUpper()).Result.Id;
                                    bairro = repoBairro.selectFirst(record => record.Descricao == endereco.Bairro && record.Cidade.Id == endereco.Id_Cidade && record.UF.Id  == uf).Result?.Id;

                                    regioes.AddRange(parametro.ParamsRegiao.Where(q => q.CodUF == uf && q.CodCidade == endereco?.Id_Cidade && q.CodBairro == bairro).ToList());
                                    break;
                            }
                        }                       
                        return regioes;
                    };
                    List<ParametrosConsultaPreQual> parametrosPreQual = ativos.Select(z =>
                        {
                            var paramTipoPessoa = z.ParamsTipoPessoa
                                .Where(p => p.CodTipoPessoa == (Tipo_Pessoa)Enum.Parse(typeof(Tipo_Pessoa), Convert.ToString(cliente.Tipo_Pessoa))).ToList();
                            
                            var paramRegiao = idVenda.HasValue ? 
                                FilterParamPrequalbyEndereco(z, venda.EnderecoInstalacao) :
                                FilterParamPrequalbyEndereco(z, cliente.Endereco_Prospecto);

                            var paramCategoriaCliente = z.ParamsCategoriaCliente
                                .Where(u => u.CodCategoriaCliente == Convert.ToString(cliente.Categoria_Cliente.Id)).ToList();

                            var paramDepartamento = z.ParamsSetorDepto.Where(u => u.CodDepto == Convert.ToInt64(departamento));

                            var descricao = z.Descricao;

                            return new ParametrosConsultaPreQual
                            {
                                Descricao = descricao,
                                ParamsTipoPessoa = paramTipoPessoa.ToList(),
                                ParamsRegiao = paramRegiao.ToList(),
                                ParamsCategoriaCliente = paramCategoriaCliente.ToList(),
                                ParamsSetorDepto = paramDepartamento,
                                Status = Status_Default.Ativo,
                                order = paramCategoriaCliente.Count() + paramRegiao.Count() + paramTipoPessoa.Count() + paramDepartamento.Count()
                            };
                        }).ToList();


                    var listaOrdenada = parametrosPreQual
                        .OrderByDescending(obj => obj.order)
                        .ToList();

                    bool maiorValor = listaOrdenada.Any(p => p.order > 0);

                    if (maiorValor == false)
                    {
                        tipo = _repoParamsConsultaPrequal.GetAll().FirstOrDefault(p => p.Padrao == true && p.Status == Status_Default.Ativo).Descricao;
                    }
                    else
                    {
                        tipo = listaOrdenada.FirstOrDefault()?.Descricao;
                    }
                }
            }

            if (tipo == null)
            {
                tipo = _repoParamsConsultaPrequal.GetAll().FirstOrDefault(p => p.Padrao == true && p.Status == Status_Default.Ativo).Descricao;
            }

            return tipo;
        }
    }
}
