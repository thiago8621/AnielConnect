using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.MK.Crm;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.Model.Mk;
using NHibernate.Hql.Ast;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;

namespace Aniel.Connect.ConnectBusiness.MK
{
    public class MkCrmBusiness
    {
        private readonly IMkCrmRepository _repository;
        private readonly ConstantesIntegracaoMk _constantesMk;

        public MkCrmBusiness(IMkCrmRepository repository, ConstantesIntegracaoMk constantesMk)
        {
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            if (constantesMk == null)
            {
                throw new ArgumentNullException("constantesMk"); 
            }

            this._repository = repository;
            this._constantesMk = constantesMk;
        }

        /// <summary>
        ///     Retorna os códigos e descrições de cada parte do endereço no MK Solutions. 
        ///     Caso alguma parte do endereço não esteja cadastrado no MK, os dados referentes à ela
        ///     serão retornados com valores nulos.
        /// </summary>
        /// <param name="uf">Sigla da UF</param>
        /// <param name="cidade">Nome da cidade</param>
        /// <param name="bairro">Nome do bairro</param>
        /// <param name="logradouro">Logradouro do endereço</param>
        public DadosEstruturaEndereco GetDadosEnderecoMk(string uf, string cidade, string bairro, string logradouro)
        {
            IEnumerable<DadosEstruturaEndereco> enderecosMk = this._repository.GetEnderecos();

            uf = uf.ToUpperInvariant();
            cidade = cidade.ToUpperInvariant();
            bairro = bairro.ToUpperInvariant();
            logradouro = logradouro.ToUpperInvariant();

            DadosEstruturaEndereco dadosEndereco = 
                enderecosMk.Where(e => e.siglaestado?.ToUpperInvariant() == uf &&
                                       e.cidade?.ToUpperInvariant() == cidade &&
                                       e.bairro?.ToUpperInvariant() == bairro &&
                                       e.logradouro?.ToUpperInvariant() == logradouro)
                           .FirstOrDefault();

            if (dadosEndereco != null)
            {
                return dadosEndereco;
            }

            // Caso o endereço exato informado não exista no MK Solutions tentamos encontrar
            // os dados das demais partes do endereço para retorná-los

            DadosEstruturaEndereco enderecoDadosParciais;
            
            enderecoDadosParciais = 
                enderecosMk.Where(e => e.siglaestado?.ToUpperInvariant() == uf &&
                                       e.cidade?.ToUpperInvariant() == cidade &&
                                       e.bairro?.ToUpperInvariant() == bairro)
                           .FirstOrDefault();

            if (enderecoDadosParciais != null)
            {
                dadosEndereco = new DadosEstruturaEndereco
                {
                    cd_uf = enderecoDadosParciais.cd_uf,
                    cd_cidade = enderecoDadosParciais.cd_cidade,
                    cd_bairro = enderecoDadosParciais.cd_bairro,
                    
                    siglaestado = enderecoDadosParciais.siglaestado,
                    cidade = enderecoDadosParciais.cidade,
                    bairro = enderecoDadosParciais.bairro
                };

                return dadosEndereco;
            }
            
            enderecoDadosParciais = 
                enderecosMk.Where(e => e.siglaestado?.ToUpperInvariant() == uf &&
                                       e.cidade?.ToUpperInvariant() == cidade)
                           .FirstOrDefault();
            
            if (enderecoDadosParciais != null)
            {
                dadosEndereco = new DadosEstruturaEndereco
                {
                    cd_uf = enderecoDadosParciais.cd_uf,
                    cd_cidade = enderecoDadosParciais.cd_cidade,

                    siglaestado = enderecoDadosParciais.siglaestado,
                    cidade = enderecoDadosParciais.cidade
                };

                return dadosEndereco;
            }

            enderecoDadosParciais = enderecosMk.Where(e => e.siglaestado?.ToUpperInvariant() == uf).FirstOrDefault();

            if (enderecoDadosParciais != null)
            {
                dadosEndereco = new DadosEstruturaEndereco
                {
                    cd_uf = enderecoDadosParciais.cd_uf,
                    siglaestado = enderecoDadosParciais.siglaestado
                };

                return dadosEndereco;
            }

            return dadosEndereco;
        }

        /// <summary>
        ///     Grava os dados do endereço no MK Solutions e retorna os códigos dos registros no MK
        /// </summary>
        /// <param name="uf">Sigla da UF</param>
        /// <param name="cidade">Nome da Cidade</param>
        /// <param name="codigoIBGECidade">Código do IBGE da cidade</param>
        /// <param name="bairro">Nome do bairro</param>
        /// <param name="logradouro">Logradouro</param>
        /// <param name="cep">CEP do endereço</param>
        public DadosEstruturaEndereco GravarDadosEnderecoMk(
            string uf, 
            string cidade, 
            string codigoIBGECidade,
            string bairro, 
            string logradouro, 
            string cep
        )
        {
            //se o endereço já estiver cadastrado com os campos de integração configurados, apenas retornar os respectivos codigos


            //se não
            DadosEstruturaEndereco dadosEnderecoClienteMk = 
                this.GetDadosEnderecoMk(uf, cidade, bairro, logradouro);

            if (dadosEnderecoClienteMk == null)
            {
                throw new Exception("Erro ao buscar os dados do endereço no MK Solutions");
            }

            if (!dadosEnderecoClienteMk.cd_cidade.HasValue)
            {
                InserirCidadeResponse dadosCidade = this.InserirCidade(dadosEnderecoClienteMk.cd_uf.Value, cidade, codigoIBGECidade);

                if (dadosCidade == null)
                {
                    throw new Exception("Erro ao inserir dados da cidade do endereço do cliente no MK Solutions");
                }

                dadosEnderecoClienteMk.cd_cidade = dadosCidade.id;
            }

            if (!dadosEnderecoClienteMk.cd_bairro.HasValue)
            {
                InserirBairroResponse dadosBairro = this.InserirBairro(dadosEnderecoClienteMk.cd_cidade.Value, bairro);

                if (dadosBairro == null)
                {
                    throw new Exception("Erro ao inserir dados do bairro do endereço do cliente no MK Solutions");
                }

                dadosEnderecoClienteMk.cd_bairro = dadosBairro.id;
            }

            if (!dadosEnderecoClienteMk.cd_logradouro.HasValue)
            {
                InserirLogradouroResponse dadosLogradouro =
                    this.InserirLogradouro(
                        logradouro,
                        cep,
                        dadosEnderecoClienteMk.cd_cidade.Value,
                        dadosEnderecoClienteMk.cd_bairro.Value
                    );

                if (dadosLogradouro == null)
                {
                    throw new Exception("Erro ao inserir dados do logradouro do endereço do cliente no MK Solutions");
                }

                dadosEnderecoClienteMk.cd_logradouro = dadosLogradouro.id;
            }

            return dadosEnderecoClienteMk;
        }

        /// <summary>
        ///     Insere uma cidade no MK Solutions
        /// </summary>
        /// <param name="codUf">Código da UF da cidade no MK Solutions</param>
        /// <param name="nomeCidade">Nome da cidade</param>
        /// <param name="codigoIbge">Código do IBGE da cidade</param>
        public InserirCidadeResponse InserirCidade(long codUf, string nomeCidade, string codigoIbge)
        {
            return this._repository.InserirCidade(new InserirCidadeRequest
            {
                nome = nomeCidade,
                ibge = codigoIbge,
                estadoId = codUf
            });
        }

        /// <summary>
        ///     Insere um bairro no MK Solutions
        /// </summary>
        /// <param name="codCidade">Código da cidade no MK Solutions</param>
        /// <param name="nomeBairro">Nome do bairro</param>
        public InserirBairroResponse InserirBairro(long codCidade, string nomeBairro)
        {
            return this._repository.InserirBairro(new InserirBairroRequest
            {
                cidadeId = codCidade,
                nome = nomeBairro
            });
        }

        /// <summary>
        ///     Insere um logradouro no MK Solutions
        /// </summary>
        /// <param name="logradouro">Logradouro</param>
        /// <param name="cep">CEP do Logradouro</param>
        /// <param name="codCidade">Código da cidade no MK Solutions</param>
        /// <param name="codBairro">Código do bairro no MK Solutions</param>
        public InserirLogradouroResponse InserirLogradouro(string logradouro, string cep, long codCidade, long codBairro)
        {
            return this._repository.InserirLogradouro(new InserirLogradouroRequest
            {
                nome = logradouro,
                cep = cep,
                cidadeId = codCidade,
                bairroId = codBairro
            });
        }

        /// <summary>
        ///     Indica se um cliente está registrado no MK
        /// </summary>
        /// <param name="documento">CPF ou CNPJ do cliente</param>
        public bool ClienteExisteMk(string documento)
        {
            return this._repository.GetDadosClienteByDocumento(documento) != null;
        }

        /// <summary>
        ///     Retorna o código do cliente no MK caso ele esteja registrado por lá
        /// </summary>
        /// <param name="documento">CPF ou CNPJ do cliente</param>
        public string GetCodigoClienteMk(string documento)
        {
            long? codigoPessoa = this._repository.GetDadosClienteByDocumento(documento)?.CodigoPessoa;
            return Convert.ToString(codigoPessoa);
        }

        public GetDadosClienteByDocMkResponse GetClienteMk(string documento)
        {
            return this._repository.GetDadosClienteByDocumento(documento);
        }

        public InserirAtendimentoResponse InserirAtendimento(InserirAtendimentoRequest atendimento)
        {
            return _repository.InserirAtendimento(atendimento);
        }

        /// <summary>
        ///     Insere um novo cliente no MK
        /// </summary>
        /// <param name="cliente">Objeto com os dados do cliente</param>
        /// <param name="dadosEndereco">Dados do endereço</param>
        public InserirClienteResponse InserirCliente(Cliente cliente, DadosEstruturaEndereco dadosEndereco, Venda venda)
        {
            string documento =
                cliente.CpfCnpj?.Replace(".", String.Empty)
                               ?.Replace("-", String.Empty)
                               ?.Replace("/", String.Empty);

            string rg = cliente.Rg?.Replace(".", String.Empty)
                                  ?.Replace("-", String.Empty);


            string email = cliente.Email_Principal;

            if (!string.IsNullOrEmpty(cliente.Email_Aux))
                email = $"{email},{cliente.Email_Aux}";

            if (cliente.Endereco_Principal?.Complemento != null && cliente.Endereco_Principal?.Complemento.Length > 50)
            {
                throw new Exception("Complemento ultrapassou o limite de 50 caracteres (limite do MK)");
            }

            if (String.IsNullOrEmpty(cliente.Categoria_Cliente?.CodigoTipoClienteIntegracao))
            {
                throw new Exception("Categoria de cliente não preencida.");
            }

            var numero = String.IsNullOrEmpty(cliente.Endereco_Principal?.Numero) ? "0" : cliente.Endereco_Principal?.Numero;
            numero = Regex.IsMatch(numero, @"^\d+$") ? numero : "0";

            var dadosCliente = new InserirClienteRequest
            {
                doc = documento.Trim(),
                nome = cliente.NomeFantasia?.Trim(),

                cep = cliente.Endereco_Principal?.Cep?.Trim(),
                cd_uf = dadosEndereco.cd_uf.HasValue ? Convert.ToString(dadosEndereco.cd_uf).Trim() : String.Empty,
                cd_cidade = dadosEndereco.cd_cidade.HasValue ? Convert.ToString(dadosEndereco.cd_cidade).Trim() : String.Empty,
                cd_bairro = dadosEndereco.cd_bairro.HasValue ? Convert.ToString(dadosEndereco.cd_bairro).Trim() : String.Empty,
                cd_logradouro = dadosEndereco.cd_logradouro.HasValue ? Convert.ToString(dadosEndereco.cd_logradouro).Trim() : String.Empty,
                numero = numero?.Trim(),
                complemento = cliente.Endereco_Principal?.Complemento?.Trim(),
                
                cd_empresa = this._constantesMk.CodigoEmpresa?.Trim(),
                email = email?.Trim(),
                nascimento = cliente.Data_Nascimento.HasValue ? cliente.Data_Nascimento.Value.ToString("dd/MM/yyyy").Trim() : String.Empty,
                fone = cliente.Telefone_Celular != null ? new String(cliente.Telefone_Celular.Where(Char.IsDigit).ToArray()).Trim() : null,

                lat = cliente.Endereco_Principal?.Latitude != null ? Convert.ToString(cliente.Endereco_Principal?.Latitude).Trim() : String.Empty,
                lon = cliente.Endereco_Principal?.Longitude != null ? Convert.ToString(cliente.Endereco_Principal?.Longitude).Trim() : String.Empty,

                nome_mae = cliente.Filiacao_Mae?.Trim(),
                rg = rg?.Trim(),
                user_sac = documento?.Trim(),
                senha_sac = documento?.Substring(0, 6).Trim(),

                grupo_atend = cliente.Categoria_Cliente?.CodigoTipoClienteIntegracao?.Trim(),
                tipo_cadastro = cliente.Tipo_Pessoa.Value == Tipo_Pessoa.Fisica ? 1 : 2,
                cliente_com = cliente.Tipo_Pessoa.Value == Tipo_Pessoa.Fisica ? "01" : "02",
                revenda = venda.Vendedor.CodigoVendedorAuxiliar?.Trim(),
            };

            return this._repository.InserirCliente(dadosCliente);
        }

        /// <summary>
        ///     Insere um contrato para o cliente no MK Solutions
        /// </summary>
        /// <param name="venda">Objeto com os dados da venda feita para o cliente pelo Connect</param>
        /// <param name="plano">Objeto com os plano vendido para o cliente pelo Connect</param>
        /// <param name="tipoPessoaCliente">Tipo do cliente (pessoa física ou jurídica)</param>
        /// <param name="diaVencimentoFatura">Dia de vencimento da fatura</param>
        /// <param name="codigoCliente">Código do cliente no MK Solutions</param>
        public InserirContratoResponse InserirContrato(
            Venda venda,
            Plano plano,
            Tipo_Pessoa tipoPessoaCliente,
            string valorPlano,
            string diaVencimentoFatura,
            string codigoCliente
        )
        {
            bool pessoaFisica = tipoPessoaCliente == Tipo_Pessoa.Fisica;
            
            if (this._constantesMk.CodigosRegraVencimento == null || this._constantesMk.CodigosRegraVencimento.Count() == 0)
            {
                throw new Exception("Códigos de regra de vencimento do MK não configurados");
            }

            string codigoRegraVencimento =
                this._constantesMk
                    .CodigosRegraVencimento
                    .Where(c => c.DiaVencimento == diaVencimentoFatura)
                    .FirstOrDefault()?.Codigo;

            if (String.IsNullOrEmpty(codigoRegraVencimento))
            {
                throw new Exception("Código de regra de vencimento do MK não encontrado");
            }

            if (String.IsNullOrEmpty(plano?.CodigoTipoPlanoIntegracao))
            {
                if (!String.IsNullOrEmpty(plano?.Descricao))
                    throw new Exception("Código do Tipo do Plano para Integração não informado no plano " + plano.Descricao);
                else
                    throw new Exception("Código do Tipo do Plano para Integração não informado");
            }

            if (String.IsNullOrEmpty(plano?.CodigoIntegracao))
            {
                if (!String.IsNullOrEmpty(plano?.Descricao))
                    throw new Exception("Código do Plano para Integração não informado no plano " + plano.Descricao);
                else
                    throw new Exception("Código do Plano para Integração não informado");
            }

            var dadosContrato = new InserirContratoRequest
            {
                CodigoCliente = codigoCliente,
                CodigoTipoPlano = plano.CodigoTipoPlanoIntegracao,
                CodigoPlanoAcesso = plano.CodigoIntegracao,
                CodigoRegraVencimento = codigoRegraVencimento,
                CodigoSLA = pessoaFisica ? this._constantesMk.SlaResidencial : this._constantesMk.SlaCorporativo,
                IgnorarRenovacaoAutomatica = this._constantesMk.IgnorarRenovacaoAutomatica,
                ValorSugeridoRenovacao = (String.IsNullOrEmpty(valorPlano) || valorPlano == Convert.ToString(Double.NaN)) ? Convert.ToString(0) : valorPlano,
                CodigoRegraBloqueio = this._constantesMk.CodigoRegraBloqueio,
                CodigoFormaPagamento = this._constantesMk.CodigoFormaPagamento,
                CodigoProfilePagamento = this._constantesMk.CodigoProfilePagamento,
                CodigoMetodoFaturamento = this._constantesMk.CodigoMetodoFaturamento,
                CodigoPlanoContas = this._constantesMk.CodigoPlanoContas,
                ConsultorCRM = venda.Vendedor.CodigoVendedorIntegracao,
                AguardaAtivacao = this._constantesMk.AguardaAtivacao,
                cd_empresa = this._constantesMk.CodigoEmpresa
            };

            return this._repository.InserirContrato(dadosContrato);
        }

        public InserirContratoResponse InserirContratoCombo(
            Venda venda,
            Combo combo,
            string idDesconto,
            Forma_Pagamento formaPagamento,
            Tipo_Pessoa tipoPessoaCliente,
            string valorPlano,
            string diaVencimentoFatura,
            string codigoCliente
        )
        {
            bool pessoaFisica = tipoPessoaCliente == Tipo_Pessoa.Fisica;

            if (this._constantesMk.CodigosRegraVencimento == null || this._constantesMk.CodigosRegraVencimento.Count() == 0)
            {
                throw new Exception("Códigos de regra de vencimento do MK não configurados");
            }

            if (formaPagamento != null && string.IsNullOrEmpty(formaPagamento.CodigoIntegracao))
            {
                throw new Exception($"A forma de pagamento {formaPagamento.Descricao} não possui codigo de integração informado");
            }

            string codigoRegraVencimento =
                this._constantesMk
                    .CodigosRegraVencimento
                    .Where(c => c.DiaVencimento == diaVencimentoFatura)
                    .FirstOrDefault()?.Codigo;

            if (String.IsNullOrEmpty(codigoRegraVencimento))
            {
                throw new Exception("Código de regra de vencimento do MK não encontrado");
            }

            if (String.IsNullOrEmpty(combo?.CodigoTipoComboIntegracao))
            {
                if (!String.IsNullOrEmpty(combo?.Descricao))
                    throw new Exception("Código do Tipo do Plano para Integração não informado no plano " + combo.Descricao);
                else
                    throw new Exception("Código do Tipo do Plano para Integração não informado");
            }

            if (String.IsNullOrEmpty(combo?.CodigoIntegracao))
            {
                if (!String.IsNullOrEmpty(combo?.Descricao))
                    throw new Exception("Código do Plano para Integração não informado no plano " + combo.Descricao);
                else
                    throw new Exception("Código do Plano para Integração não informado");
            }

            var dadosContrato = new InserirContratoRequest
            {
                CodigoCliente = codigoCliente,
                CodigoTipoPlano = combo.CodigoTipoComboIntegracao,
                CodigoPlanoAcesso = combo.CodigoIntegracao,
                CodigoRegraVencimento = codigoRegraVencimento,
                CodigoSLA = pessoaFisica ? this._constantesMk.SlaResidencial : this._constantesMk.SlaCorporativo,
                IgnorarRenovacaoAutomatica = this._constantesMk.IgnorarRenovacaoAutomatica,
                ValorSugeridoRenovacao = (String.IsNullOrEmpty(valorPlano) || valorPlano == Convert.ToString(Double.NaN)) ? Convert.ToString(0) : valorPlano,
                CodigoRegraBloqueio = this._constantesMk.CodigoRegraBloqueio,
                CodigoFormaPagamento = this._constantesMk.CodigoFormaPagamento,
                CodigoProfilePagamento = formaPagamento.CodigoIntegracao,
                CodigoMetodoFaturamento = Convert.ToDouble(valorPlano) > 0 ? this._constantesMk.CodigoMetodoFaturamento : _constantesMk.CodigoMetodoFaturamentoCortesia,
                CodigoPlanoContas = this._constantesMk.CodigoPlanoContas,
                ConsultorCRM = venda.Vendedor.CodigoVendedorIntegracao,
                AguardaAtivacao = this._constantesMk.AguardaAtivacao,
                CodigoDescontoAplicado = idDesconto,
                cd_empresa = this._constantesMk.CodigoEmpresa
            };

            return this._repository.InserirContrato(dadosContrato);
        }

        /// <summary>
        ///     Insere uma conexão no MK Solutions
        /// </summary>
        /// <param name="venda">Objeto com os dados da venda feita pelo Connect</param>
        /// <param name="plano">Objeto com os dados do plano vendido pelo Connect</param>
        /// <param name="enderecoMk">Objeto com os dados do endereço no MK Solutions</param>
        /// <param name="codigoCliente">Código do cliente no MK Solutions</param>
        /// <param name="codigoContrato">Código do contrato no MK Solutions</param>
        public InserirConexaoResponse InserirConexao(Cliente cliente,
            Venda venda,
            Plano plano,
            DadosEstruturaEndereco enderecoMk,
            string codigoCliente,
            string codigoContrato)
        {
            bool enderecoValido =
                enderecoMk?.cd_uf != null &&
                enderecoMk?.cd_cidade != null &&
                enderecoMk?.cd_bairro != null &&
                enderecoMk?.cd_logradouro != null &&
                !String.IsNullOrEmpty(venda?.EnderecoInstalacao?.Numero) &&
                !String.IsNullOrEmpty(venda?.EnderecoInstalacao?.Cep);

            if (this._constantesMk.EnderecoCadastro != "true" && !enderecoValido)
                throw new Exception("Os dados do endereço não estão completos");

            if (venda.EnderecoInstalacao.Complemento != null && venda.EnderecoInstalacao.Complemento.Length > 50)
            {
                throw new Exception("Complemento ultrapassou o limite de 50 caracteres (limite do MK)");
            }

            string password = cliente.Tipo_Pessoa == Tipo_Pessoa.Fisica ? cliente.Data_Nascimento.Value.ToString("ddMMYYYY") : cliente.Data_Criacao_Cliente.Value.ToString("ddMMYYYY");

            var dadosConexao = new InserirConexaoRequest
            {
                CodigoCliente = codigoCliente,
                CodigoPontoAcesso = this._constantesMk.CodigoPontoAcesso,
                CodigoContrato = codigoContrato,
                CodigoServidor = this._constantesMk.CodigoServidor,
                CodigoPlanoAcesso = this._constantesMk.UtilizaCodigoPlanoIntegracaoAuxiliar == "S" ? plano?.CodigoIntegracaoAuxiliar : plano?.CodigoIntegracao,
                CodigoTipoIP = this._constantesMk.CodigoTipoIP,
                Tecnologia = this._constantesMk.Tecnologia,
                AutoDesbloqueio = this._constantesMk.AutoDesbloqueio,
                MacAddress = this._constantesMk.MacAddress,
                EnderecoCadastro = this._constantesMk.EnderecoCadastro,
                TipoConexao = plano.ValorPlano > 0 ? "1" : "2",//1 = cobrança ou 2 = cortesia

                PoolMKCentral = this._constantesMk.PoolMKCentral,

                Username = $"{codigoContrato}{cliente.CpfCnpj}",
                Password = password,

                CodigoEstado = Convert.ToString(enderecoMk.cd_uf.Value),
                CodigoCidade = Convert.ToString(enderecoMk.cd_cidade.Value),
                CodigoBairro = Convert.ToString(enderecoMk.cd_bairro.Value),
                CodigoLogradouro = Convert.ToString(enderecoMk.cd_logradouro.Value),
                EnderecoNumero = venda.EnderecoInstalacao.Numero,
                Complemento = venda.EnderecoInstalacao.Complemento ?? "",
                CEP = venda.EnderecoInstalacao.Cep,
                DataConnection = this._constantesMk.DataConnection,
                TpAutenticacao = this._constantesMk.TpAutenticacao
            };

            return this._repository.InserirConexao(dadosConexao);
        }

        public InserirConexaoResponse InserirConexaoCombo(
            Venda venda,
            Combo combo,
            Cliente cliente,
            DadosEstruturaEndereco enderecoMk,
            string codigoCliente,
            string codigoContrato)
        {
            bool enderecoValido =
                enderecoMk?.cd_uf != null &&
                enderecoMk?.cd_cidade != null &&
                enderecoMk?.cd_bairro != null &&
                enderecoMk?.cd_logradouro != null &&
                !String.IsNullOrEmpty(venda?.EnderecoInstalacao?.Numero) &&
                !String.IsNullOrEmpty(venda?.EnderecoInstalacao?.Cep);

            if (this._constantesMk.EnderecoCadastro != "true" && !enderecoValido)
            {
                throw new Exception("Os dados do endereço não estão completos");
            }

            if (venda.EnderecoInstalacao.Complemento != null && venda.EnderecoInstalacao.Complemento.Length > 50)
            {
                throw new Exception("Complemento ultrapassou o limite de 50 caracteres (limite do MK)");
            }

            string username = codigoContrato;
            string password = null;

            if (cliente.Tipo_Pessoa == Tipo_Pessoa.Fisica)
            {

                if (!cliente.Data_Nascimento.HasValue)
                {
                    throw new Exception("Data de nascimento do cliente é obrigatória para geração da senha de acesso ao MK Solutions");
                }

                password = cliente.Data_Nascimento.Value.ToString("ddMMyyyy");
            }
            else
            {
                if (!cliente.Data_Abertura_Empresa.HasValue)
                {
                    throw new Exception("Data de abertura da empresa é obrigatória para geração da senha de acesso ao MK Solutions");
                }

                password = cliente.Data_Abertura_Empresa.Value.ToString("ddMMyyyy");
            }


            var dadosConexao = new InserirConexaoRequest
            {
                CodigoCliente = codigoCliente,
                CodigoPontoAcesso = this._constantesMk.CodigoPontoAcesso,
                CodigoContrato = codigoContrato,
                CodigoServidor = this._constantesMk.CodigoServidor,
                CodigoPlanoAcesso = combo.PlanosProdutos.Where(x => x.Plano.Tipo_Tecnologia == Model.enumerado.Tipo_Tecnologia.Internet).FirstOrDefault().Plano.CodigoIntegracao,
                CodigoTipoIP = this._constantesMk.CodigoTipoIP,
                Tecnologia = this._constantesMk.Tecnologia,
                AutoDesbloqueio = this._constantesMk.AutoDesbloqueio,
                MacAddress = this._constantesMk.MacAddress,
                EnderecoCadastro = this._constantesMk.EnderecoCadastro,
                TipoConexao = this._constantesMk.TipoConexao,

                PoolMKCentral = this._constantesMk.PoolMKCentral,

                Username = username,
                Password = password,

                //alterar: utilizar codigo auxiliar do cadastro de endereço quando 

                CodigoEstado = Convert.ToString(enderecoMk.cd_uf.Value),
                CodigoCidade = Convert.ToString(enderecoMk.cd_cidade.Value),
                CodigoBairro = Convert.ToString(enderecoMk.cd_bairro.Value),
                CodigoLogradouro = Convert.ToString(enderecoMk.cd_logradouro.Value),


                EnderecoNumero = venda.EnderecoInstalacao.Numero,
                Complemento = venda.EnderecoInstalacao.Complemento ?? "",
                CEP = venda.EnderecoInstalacao.Cep,
                DataConnection = this._constantesMk.DataConnection,
                TpAutenticacao = this._constantesMk.TpAutenticacao
            };

            return this._repository.InserirConexao(dadosConexao);
        }

        public InserirOrdemServicoResponse InserirOrdemServico(CriarOrdemServicoRequest request)
        {
            request.CodigoTecnico = _constantesMk.CodigoTecnico;
            request.CodigoGrupoServico = _constantesMk.CodigoGrupoServico;
            return _repository.InserirOrdemServico(request);
    }
}
}
