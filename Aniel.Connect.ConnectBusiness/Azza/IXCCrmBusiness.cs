using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Cidade;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.Repository.Azza.Crm;
using Aniel.Connect.Repository.Repository.Azza.Model;
using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Integracao.IntegracoesERP.Ixc.Models;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Aniel.Connect.ConnectBusiness.Azza
{
    public class IXCCrmBusiness
    {
        private readonly IIXCCrmRepository _repository;
        private readonly ICidadeRepository _repositoryCidade;
        private readonly IConfiguracoesGeraisRepository _repositoryConfig;
        private bool _cpfCnpjSemCaracteres;

        public IXCCrmBusiness(IIXCCrmRepository repository, ICidadeRepository repositoryCidade, IConfiguracoesGeraisRepository repositoryConfig, bool cpfCnpjSemCaracteres)
        {
            _repository = repository;
            _repositoryCidade = repositoryCidade;
            _repositoryConfig = repositoryConfig;
            _cpfCnpjSemCaracteres = cpfCnpjSemCaracteres;
        }

        public ResponsePostIXC InserirCliente(ClienteIXCResponse dadosCliente)
        {
            return _repository.InserirCliente(dadosCliente);
        }

        public ResponsePostIXC AtualizarCliente(ClienteIXCResponse dadosCliente)
        {
            return _repository.AtualizarCliente(dadosCliente);
        }

        public ResponsePostIXC InserirAtendimento(AtendimentoIXCResponse dadosAtendimento)
        {
            return _repository.InserirAtendimento(dadosAtendimento);
        }

        public ResponsePostIXC InserirContrato(ClienteContratoIXCResponse dadosClienteContrato)
        {
            return _repository.InserirContrato(dadosClienteContrato);
        }

        public ResponsePostIXC InserirLogin(LoginIXCResponse dadosLogin)
        {
            return _repository.InserirLogin(dadosLogin);
        }

        public ResponseIXC<ClienteIXCResponse> FindByCliente(string cpfCnpj)
        {
            return _repository.FindByCliente(cpfCnpj);
        }

        public ResponseIXC<AtendimentoIXCResponse> FindAtendimentoByContratoCliente(string id_clientecontrato)
        {
            return _repository.FindAtendimentoByContratoCliente(id_clientecontrato);
        }

        public ResponseIXC<LoginIXCResponse> FindByLogin_Contrato(string idContrato)
        {
            return _repository.FindByLogin_Contrato(idContrato);
        }

        public ResponseIXC<RadGruposIXCResponse> FindByRadGrupo_Contrato(string idContrato)
        {
            return _repository.FindByRadGrupo_Contrato(idContrato);
        }

        public ResponseIXC<VdContratosProdutosIXCResponse> FindByVdContratosProdutos(string idContrato)
        {
            return _repository.FindByVdContratosProdutos(idContrato);
        }

        public ResponseIXC<ClienteContratoIXCResponse> FindByIdContratoCliente(string id_contrato_cliente)
        {
            return _repository.FindByIdContratoCliente(id_contrato_cliente);
        }

        #region [GetViewObj - ClienteAzzaResponse]
        public ClienteIXCResponse GetViewObjClienteIXCResponse(Cliente cliente, string IdFilial, Venda venda, ISession sessiont5, ConstantesIntegracaoIXC constantes)
        {
            var config = this._repositoryConfig.GetConfiguracoesGerais();
            var idCidade = config.Utiliza_Cadastro_Endereco ? cliente?.Endereco_Principal?.Endereco_Logradouro.Cidade.Id : cliente?.Endereco_Principal?.Id_Cidade;
            var ibge = idCidade.HasValue ? _repositoryCidade.GetById(idCidade.ToString()).Codigo_IBGE : null;
            var idCidadeIxc = _repository.RequestCidade(ibge).registros.FirstOrDefault()?.id;
            var tipoClienteIxc = cliente?.Categoria_Cliente?.CodigoTipoClienteIntegracao != null ? _repository.FindByTipoCliente(cliente.Categoria_Cliente.CodigoTipoClienteIntegracao).registros.FirstOrDefault() : null;

            string tipo_pessoa = string.Empty, sexo = string.Empty, estado_civil = string.Empty;
            var numeroCepIXC = IXCCrmBusiness.ValidacaoNumeroCep(cliente.Endereco_Principal);
            string numero = numeroCepIXC.Numero;
            string cep = numeroCepIXC.Cep;

            if (this._cpfCnpjSemCaracteres) 
            {
                var caracteres = new List<string>() { "/", "-", "." };
                foreach (var item in caracteres)
                {
                    cliente.CpfCnpj = cliente.CpfCnpj.Replace(item, string.Empty);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(cliente.CpfCnpj))
                {
                    var valor = cliente.CpfCnpj;
                    
                    valor = valor.Replace(".", "")
                                 .Replace("-", "")
                                 .Replace("/", "");

                    if (valor.Length == 11) 
                    {
                        cliente.CpfCnpj = Convert.ToUInt64(valor).ToString(@"000\.000\.000\-00");
                    }
                    else if (valor.Length == 14)
                    {
                        cliente.CpfCnpj = Convert.ToUInt64(valor).ToString(@"00\.000\.000\/0000\-00");
                    }
                }
            }

            var isCondominio = venda.EnderecoInstalacao.Condominio_Predio != null;
            var isCadastroEndereco = venda.EnderecoInstalacao.Endereco_Logradouro != null;

            switch (cliente.Tipo_Pessoa)
            {
                case Tipo_Pessoa.Fisica:
                    tipo_pessoa = "F";
                    break;
                case Tipo_Pessoa.Juridica:
                    tipo_pessoa = "J";
                    break;
            }

            switch (cliente.Sexo)
            {
                case Sexo.Masculino:
                    sexo = "M";
                    break;
                case Sexo.Feminino:
                    sexo = "F";
                    break;
            }

            switch (cliente.Estado_Civil)
            {
                case Estado_Civil.Solteiro:
                    estado_civil = "S";
                    break;
                case Estado_Civil.Casado:
                    estado_civil = "C";
                    break;
                case Estado_Civil.Divorciado:
                    estado_civil = "D";
                    break;
                case Estado_Civil.Viuvo:
                    estado_civil = "V";
                    break;
                case Estado_Civil.UniaoEstavel:
                    estado_civil = "U";
                    break;
            }

            string id_responsavel = string.Empty;
            using (var _repoFuncionario = new Conexao<FuncionarioAniel>(sessiont5))
            {
                id_responsavel = _repoFuncionario.selectFirst(x => x.Num_Fre == venda.Vendedor.Num_Fre).Result.Matricula_Aux;
            }

            string razaoSocial = !String.IsNullOrEmpty(cliente.Razao_Social) ? cliente.Razao_Social.ToUpper() : cliente.NomeFantasia.ToUpper();
            return new ClienteIXCResponse()
            {
                ativo = constantes.ClienteAtivo,
                id_tipo_cliente = tipoClienteIxc?.id,
                tipo_cliente_scm = tipoClienteIxc?.tipo_cliente_scm,
                pais = cliente?.Endereco_Principal.Pais != null ? cliente.Endereco_Principal.Pais.Nome : "Brasil",
                tipo_pessoa = tipo_pessoa,
                razao = StringService.GetFormatStringSingleWhiteSpace(razaoSocial),
                fantasia = !String.IsNullOrEmpty(cliente.Razao_Social) ? cliente.Razao_Social.ToUpper() : cliente.NomeFantasia.ToUpper(),
                cnpj_cpf = !string.IsNullOrEmpty(cliente.CpfCnpj) ? cliente.CpfCnpj : "",
                ie_identidade = !string.IsNullOrEmpty(cliente.Inscricao_Estadual) ? cliente.Inscricao_Estadual : cliente.Rg,
                contribuinte_icms = constantes.ContribuinteICMS,
                rg_orgao_emissor = !string.IsNullOrEmpty(cliente.Orgao_Emissao_Rg) ? cliente?.Orgao_Emissao_Rg : "",
                nacionalidade = !string.IsNullOrEmpty(cliente.Nacionalidade) ? cliente.Nacionalidade : "",
                cidade_naturalidade = null,
                estado_nascimento = cliente?.Endereco_Principal != null ? cliente?.Endereco_Principal?.UF : "",
                data_nascimento = !string.IsNullOrEmpty(Convert.ToString(cliente?.Data_Nascimento)) ? Convert.ToString(cliente?.Data_Nascimento) : "",
                sexo = sexo,
                profissao = !string.IsNullOrEmpty(cliente.Profissao) ? cliente?.Profissao : "",
                estado_civil = estado_civil,
                inscricao_municipal = !string.IsNullOrEmpty(cliente.Inscricao_Municipal) ? cliente?.Inscricao_Municipal : "",
                isuf = "",
                tipo_assinante = !string.IsNullOrEmpty(tipoClienteIxc?.tipo_assinante) ? tipoClienteIxc.tipo_assinante : "3",
                filial_id = IdFilial,
                filtra_filial = constantes.FiltraFilial,
                idx = constantes.Idx,
                data_cadastro = !string.IsNullOrEmpty(Convert.ToString(cliente?.DataHoraCriacao)) ? Convert.ToString(cliente?.DataHoraCriacao) : "",
                ativo_serasa = "",
                convert_cliente_forn = constantes.ConvertClienteForn,
                atualizar_cadastro_galaxPay = constantes.AtualizarGalaxPay,
                grau_satisfacao = "",
                id_condominio = "",
                bloco = "",
                apartamento = "",
                cep = cep,
                cif = cliente?.Endereco_Principal != null ? Convert.ToString(cliente?.Endereco_Principal?.Id) : "",
                endereco = cliente?.Endereco_Principal != null ? cliente?.Endereco_Principal?.Logradouro : "",
                numero = numero,
                complemento = cliente?.Endereco_Principal != null ? cliente?.Endereco_Principal?.Complemento : "",
                bairro = config.Utiliza_Cadastro_Endereco && isCondominio ? cliente.Endereco_Principal.Condominio_Predio.Bairro :
                         config.Utiliza_Cadastro_Endereco && isCadastroEndereco ? cliente.Endereco_Principal.Endereco_Logradouro.Bairro.Descricao :
                         cliente.Endereco_Principal != null ? cliente.Endereco_Principal.Bairro : "",//bairro
                cidade = idCidadeIxc,
                referencia = cliente?.Endereco_Principal != null ? cliente?.Endereco_Principal?.Ponto_Referencia : "",
                uf = cliente?.Endereco_Principal != null ? cliente?.Endereco_Principal?.Endereco_Logradouro?.UF?.Descricao : "",
                moradia = constantes.Moradia,
                tipo_localidade = constantes.TipoLocalidadeCliente,
                latitude = cliente?.Endereco_Principal != null ? Convert.ToString(cliente?.Endereco_Principal?.Latitude) : "",
                longitude = cliente?.Endereco_Principal != null ? Convert.ToString(cliente?.Endereco_Principal?.Longitude) : "",
                cep_cob = cep,
                endereco_cob = cliente?.Endereco_Principal != null ? cliente?.Endereco_Principal?.Logradouro : "",
                numero_cob = cliente?.Endereco_Principal != null ? cliente?.Endereco_Principal?.Numero.Trim() : "",
                bairro_cob = cliente?.Endereco_Principal != null ? cliente?.Endereco_Principal?.Bairro : "",
                cidade_cob = idCidadeIxc,
                complemento_cob = "",
                referencia_cob = "",
                uf_cob = constantes.UfCobranca,
                fone = !String.IsNullOrEmpty(cliente.Telefone_Fixo) ? cliente.Telefone_Fixo : cliente.Telefone_Celular,
                telefone_comercial = !String.IsNullOrEmpty(cliente.Telefone_Recado) ? cliente.Telefone_Recado : cliente.Telefone_Celular,
                ramal = "",
                id_operadora_celular = "",
                telefone_celular = !String.IsNullOrEmpty(cliente.Telefone_Celular) ? cliente.Telefone_Celular : cliente.Telefone_Fixo,
                whatsapp = !String.IsNullOrEmpty(cliente.Telefone_Celular) ? cliente.Telefone_Celular : cliente.Telefone_Fixo,
                email = !string.IsNullOrEmpty(cliente?.Email_Principal) ? cliente?.Email_Principal : "",
                email_opa = !string.IsNullOrEmpty(cliente?.Email_Auxiliar) ? cliente?.Email_Auxiliar : "",
                contato = "",
                website = "",
                skype = "",
                facebook = "",
                hotsite_email = cliente.CpfCnpj, //login cpf completo (somente numeros)
                senha = cliente.CpfCnpj.Substring(0, 5), //senha 5 primeiros digitos cpf (somente numeros)
                acesso_automatico_central = constantes.AcessoAutomaticoCentral,
                alterar_senha_primeiro_acesso = constantes.AlterarSenhaPrimeiroAcesso,
                senha_hotsite_md5 = constantes.SenhaHotsiteMD5,
                hotsite_acesso = constantes.HotsiteAcesso,
                crm = constantes.clienteCRM,
                id_candato_tipo = "",
                id_campanha = "",
                id_concorrente = constantes.IdConcorrente,
                id_perfil = constantes.IdPerfil,
                responsavel = id_responsavel,//responsavel
                indicado_por = "",
                cadastrado_via_viabilidade = constantes.CadastradoViaViabilidade,
                status_prospeccao = constantes.StatusProspeccao,
                crm_data_novo = "",
                crm_data_sondagem = "",
                crm_data_apresentando = "",
                crm_data_negociando = "",
                crm_data_vencemos = "",
                crm_data_perdemos = "",
                crm_data_abortamos = "",
                crm_data_sem_porta_disponivel = "",
                crm_data_sem_viabilidade = "",
                pipe_id_organizacao = constantes.PipeIdOrganizacao,
                foto_cartao = "",
                participa_cobranca = constantes.ParticipaCobranca,
                num_dias_cob = "",
                participa_pre_cobranca = constantes.ParticipaPreCobranca,
                cob_envia_email = constantes.CobrancaEnviaEmail,
                cob_envia_sms = constantes.CobrancaEnviaSMS,
                id_conta = "",
                cond_pagamento = "",
                id_vendedor = "",//vendedor
                tabela_preco = "",
                deb_automatico = "",
                deb_agencia = "",
                deb_conta = "",
                codigo_operacao = "",
                tipo_pessoa_titular_conta = constantes.TipoPessoaTitular,
                cnpj_cpf_titular_conta = "",
                ultima_atualizacao = DateTime.Now.ToString("yyyy-MM-dd"),
                nome_pai = "",
                cpf_pai = "",
                identidade_pai = "",
                nascimento_pai = "",
                nome_mae = "",
                cpf_mae = "",
                identidade_mae = "",
                nascimento_mae = "",
                quantidade_dependentes = "",
                nome_conjuge = "",
                fone_conjuge = "",
                cpf_conjuge = "",
                rg_conjuge = "",
                data_nascimento_conjuge = "",
                nome_contador = "",
                telefone_contador = "",
                orgao_publico = constantes.OrgaoPublico,
                im = "",
                nome_representante_1 = "",
                cpf_representante_1 = "",
                identidade_representante_1 = "",
                nome_representante_2 = "",
                cpf_representante_2 = "",
                identidade_representante_2 = "",
                emp_empresa = "",
                emp_cnpj = "",
                emp_cep = "",
                emp_endereco = "",
                emp_cidade = "",
                emp_fone = "",
                emp_cargo = "",
                emp_remuneracao = "",
                emp_data_admissao = "",
                iss_classificacao_padrao = constantes.ISS,
                pis_retem = constantes.PISRetem,
                cofins_retem = constantes.CofinsRetem,
                csll_retem = constantes.CSLLRetem,
                irrf_retem = constantes.IRRFRetem,
                desconto_irrf_valor_inferior = constantes.DescontoIRRF,
                cli_desconta_iss_retido_total = constantes.DescontaISS,
                dica_imposto_retido_cliente = "",
                ref_com_empresa1 = "",
                ref_com_fone1 = "",
                ref_com_empresa2 = "",
                ref_com_fone2 = "",
                ref_pes_nome1 = "",
                ref_pes_fone1 = "",
                ref_pes_nome2 = "",
                ref_pes_fone2 = "",
                obs = constantes.obs,
                alerta = ""
            };
        }
        #endregion

        #region [GetViewObj - AtendimentoAzzaResponse]
        public AtendimentoIXCResponse GetViewObjAtendimentoIXCResponse(string idCliente, string idContrato, string idLogin,
            Cliente_PlanoProduto cliente_PlanoProduto, string idFilial, ConstantesIntegracaoIXC constantes)
        {
            AssuntoIXCResponse assuntoIxc = null;
            SetorIXCResponse setorIxc = null;
            if (!string.IsNullOrEmpty(cliente_PlanoProduto.Plano.CodigoIntegracaoAuxiliar))
            {
                assuntoIxc = _repository.FindByAssunto(cliente_PlanoProduto.Plano.CodigoIntegracaoAuxiliar).registros[0];
            }

            if (assuntoIxc != null)
            {
                var consultaSetor = _repository.FindBySetor(assuntoIxc.id_processo);
                setorIxc = consultaSetor.registros != null ? consultaSetor.registros.FirstOrDefault() : null;
            }

            return new AtendimentoIXCResponse()
            {
                tipo = constantes.TipoAtendimento,
                id_cliente = idCliente,
                id_login = idLogin,
                id_contrato = idContrato,
                id_filial = string.IsNullOrEmpty(idFilial) ? constantes.idFilial : idFilial,
                id_assunto = cliente_PlanoProduto.Plano.CodigoIntegracaoAuxiliar,//obrigatorio -> su_oss_assunto GET
                titulo = assuntoIxc?.assunto,//obrigatorio -> su_oss_assunto (cliente_PlanoProduto.Plano.CodigoIntegracaoAuxiliar) (assunto) GET
                origem_endereco = constantes.OrigemEndereco,
                origem_endereco_estrutura = constantes.OrigemEnderecoEstrutura,
                id_wfl_processo = assuntoIxc?.id_processo,//obrigatorio -> su_oss_assunto GET (id_processo)
                id_ticket_setor = setorIxc?.id_depto,//obrigatorio -> wfl_processo (id_processo) <- id_depto
                data_criacao = DateTime.Now.ToString("dd/MM/yyyy"),
                prioridade = constantes.Pioridade,
                melhor_horario_reserva = constantes.MelhorHorarioReserva,
                id_ticket_origem = constantes.IdTicketOrigem,
                menssagem = constantes.obs,
                interacao_pendente = constantes.InteracaoPendente,
                su_status = constantes.SuStatus,
                status = constantes.StatusAtendimento,
                finalizar_atendimento = constantes.FinalizarAtendimento,
                origem_cadastro = constantes.OrigemCadastro,
                ultima_atualizacao = DateTime.Now.ToString("dd/MM/yyyy"),
                cliente_telefone_comercial = cliente_PlanoProduto.Cliente.Telefone_Fixo,
                cliente_telefone_celular = cliente_PlanoProduto.Cliente.Telefone_Celular,
                cliente_whatsapp = cliente_PlanoProduto.Cliente.Telefone_Celular,
                cliente_email = cliente_PlanoProduto.Cliente.Email_Principal,
                atualizar_cliente = constantes.AtualizarCliente,
                atualizar_login = constantes.AtualizarLogin
            };
        }
        #endregion

        #region [GetViewObj - ClienteContratoAzzaResponse]
        public ClienteContratoIXCResponse GetViewObjClienteContratoAzzaResponse(
            string IdCliente, Venda venda, Cliente_PlanoProduto cliente_PlanoProduto, string idFilial,string id_motivo_inclusao, ISession sessiont5, ConstantesIntegracaoIXC constantesIntegracao)
        {
            string id_responsavel = string.Empty;
            var config = this._repositoryConfig.GetConfiguracoesGerais();
            using (var _repoFuncionario = new Conexao<FuncionarioAniel>(sessiont5))
            {
                id_responsavel = _repoFuncionario.selectFirst(x => x.Num_Fre == venda.Vendedor.Num_Fre).Result.Matricula_Aux;
            }

            var numeroCepIXC = IXCCrmBusiness.ValidacaoNumeroCep(venda.EnderecoInstalacao);
            string numero = numeroCepIXC.Numero;
            string cep = numeroCepIXC.Cep;

            var isCondominio = venda.EnderecoInstalacao.Condominio_Predio != null;
            var isCadastroEndereco = venda.EnderecoInstalacao.Endereco_Logradouro != null;
            var idCidade = venda?.EnderecoInstalacao?.Id_Cidade;
            var ibge = idCidade.HasValue ? _repositoryCidade.GetById(idCidade.ToString()).Codigo_IBGE : null;
            var idCidadeIxc = _repository.RequestCidade(ibge).registros.FirstOrDefault()?.id;

            return new ClienteContratoIXCResponse()
            {
                tipo = constantesIntegracao.tipo,
                id_cliente = IdCliente,
                id_vd_contrato = cliente_PlanoProduto?.Plano != null ? cliente_PlanoProduto?.Plano?.CodigoIntegracao : "",
                descricao_aux_plano_venda = cliente_PlanoProduto?.Plano != null ? cliente_PlanoProduto?.Plano?.Descricao : "",//plano
                contrato = cliente_PlanoProduto?.Plano != null ? cliente_PlanoProduto?.Plano?.Descricao : "",//plano
                id_tipo_contrato = cliente_PlanoProduto?.Forma_Pagamento_Cliente != null ? cliente_PlanoProduto?.Forma_Pagamento_Cliente?.Forma_Pagamento?.CodigoIntegracao : "",//idpag
                id_modelo = cliente_PlanoProduto?.ClienteContrato != null ? cliente_PlanoProduto?.ClienteContrato?.ModeloContrato?.CodigoIntegracao : "",
                assinatura_digital = constantesIntegracao.assinatura_digital,
                integracao_assinatura_digital = constantesIntegracao.integracao_assinatura_digital,
                liberacao_bloqueio_manual = constantesIntegracao.liberacao_bloqueio_manual,
                id_filial = string.IsNullOrEmpty(idFilial) ? "1" : idFilial,//
                indicacao_contrato_id = "",
                data_assinatura = "",
                isentar_contrato = constantesIntegracao.isentar_contrato,
                data_ativacao = "",
                data = DateTime.Now.ToString("dd/MM/yyyy"),//venda.Data_Venda.ToString(),
                data_renovacao = "",
                pago_ate_data = "",
                status = constantesIntegracao.status,
                status_internet = constantesIntegracao.status_internet,
                status_velocidade = constantesIntegracao.status_velocidade,
                tipo_produtos_plano = constantesIntegracao.tipo_produtos_plano,
                motivo_inclusao = constantesIntegracao.motivo_inclusao,
                id_indexador_reajuste = "",
                url_assinatura_digital = "",
                token_assinatura_digital = "",
                id_tipo_documento = constantesIntegracao.id_tipo_documento,
                id_carteira_cobranca = constantesIntegracao.id_carteira_cobranca,
                id_vendedor = venda.Vendedor != null ? venda.Vendedor.CodigoVendedorIntegracao : "",//venda.Vendedor.CodigoVendedorIntegracao,vendedor
                moeda = "",
                comissao = "",
                cc_previsao = constantesIntegracao.cc_previsao,
                tipo_cobranca = constantesIntegracao.tipo_cobranca,//idpag
                renovacao_automatica = constantesIntegracao.renovacao_automatica,
                gerar_finan_assin_digital_contrato = constantesIntegracao.gerar_finan_assin_digital_contrato,
                base_geracao_tipo_doc = constantesIntegracao.base_geracao_tipo_doc,
                id_contrato_principal = "",
                num_parcelas_atraso = "",
                nf_info_adicionais = "",
                credit_card_recorrente_bandeira_cartao = "",
                credit_card_recorrente_token = "",
                ids_contratos_recorrencia = "",
                tipo_doc_opc = "",
                tipo_doc_opc2 = "",
                tipo_doc_opc3 = "",
                tipo_doc_opc4 = "",
                id_tipo_doc_ativ = constantesIntegracao.id_tipo_doc_ativ,
                id_produto_ativ = cliente_PlanoProduto.Produto != null ? Convert.ToString(cliente_PlanoProduto.Produto.Id) : Convert.ToString(cliente_PlanoProduto.Plano.Id),//idproduto
                taxa_instalacao = constantesIntegracao.taxa_instalacao,
                id_cond_pag_ativ = "",
                id_responsavel = id_responsavel,//responsavel
                id_vendedor_ativ = venda.Vendedor != null ? venda.Vendedor.CodigoVendedorIntegracao : "",//vendedor
                ativacao_numero_parcelas = "",
                ativacao_vencimentos = "",
                btn_nf_ativacao = "",
                ativacao_valor_parcela = "",
                fidelidade = constantesIntegracao.fidelidade,
                data_expiracao = "",
                dica_data_expiracao = "",
                desconto_fidelidade = constantesIntegracao.desconto_fidelidade,
                id_instalador = "",
                taxa_improdutiva = "",
                venc_personalizado = "",
                com_entrada = "",
                dia_fixo_vencimento = "",
                tipo_condicao_pag = "",
                bloqueio_automatico = constantesIntegracao.bloqueio_automatico,
                nao_bloquear_ate = "",
                aviso_atraso = constantesIntegracao.aviso_atraso,
                nao_avisar_ate = "",
                desbloqueio_confianca = constantesIntegracao.desbloqueio_confianca,
                desbloqueio_confianca_ativo = constantesIntegracao.desbloqueio_confianca_ativo,
                restricao_auto_desbloqueio = constantesIntegracao.restricao_auto_desbloqueio,
                motivo_restricao_auto_desbloq = "",
                obs = constantesIntegracao.obs,
                nao_susp_parc_ate = "",
                liberacao_suspensao_parcial = constantesIntegracao.liberacao_suspensao_parcial,
                utilizando_auto_libera_susp_parc = constantesIntegracao.utilizando_auto_libera_susp_parc,
                restricao_auto_libera_susp_parcial = constantesIntegracao.restricao_auto_libera_susp_parcial,
                motivo_restri_auto_libera_parc = "",
                contrato_suspenso = constantesIntegracao.contrato_suspenso,
                data_inicial_suspensao = "",
                data_final_suspensao = "",
                data_acesso_desativado = "",
                data_cancelamento = "",
                id_responsavel_cancelamento = "",
                motivo_cancelamento = "",
                motivo_adicional = "",
                concorrente_mot_adicional = "",
                dica_motivo_adicional_cliente_contrato = "",
                obs_cancelamento = "",
                data_negativacao = "",
                id_responsavel_negativacao = "",
                protocolo_negativacao = "",
                id_motivo_negativacao = "",
                id_motivo_inclusao = id_motivo_inclusao,
                obs_negativacao = "",
                data_desistencia = "",
                id_responsavel_desistencia = "",
                motivo_desistencia = "",
                obs_desistencia = "",
                obs_contrato = constantesIntegracao.obs_contrato,
                alerta_contrato = "",
                imp_realizado = constantesIntegracao.imp_realizado,
                imp_inicial = "",
                imp_carteira = constantesIntegracao.imp_carteira,
                imp_importacao = constantesIntegracao.imp_importacao,
                imp_treinamento = constantesIntegracao.imp_treinamento,
                imp_rede = constantesIntegracao.imp_rede,
                imp_bkp = constantesIntegracao.imp_bkp,
                imp_obs = "",
                imp_final = "",
                imp_status = constantesIntegracao.imp_status,
                imp_motivo = "",
                dt_ult_bloq_auto = "",
                dt_ult_bloq_manual = "",
                dt_ult_finan_atraso = "",
                dt_ult_des_bloq_conf = "",
                dt_ult_liberacao_susp_parc = "",
                dt_ult_ativacao = "",
                dt_utl_negativacao = "",
                dt_ult_desiste = "",
                data_cadastro_sistema = "",
                ultima_atualizacao = DateTime.Now.ToString("yyyy-MM-dd"),
                data_retomada_contrato = "NULL",
                endereco_padrao_cliente = constantesIntegracao.endereco_padrao_cliente,
                endereco_padrao_alert = "",
                id_condominio = "",
                condominio_novo = "",
                bloco = "",
                bloco_novo = "",
                apartamento = "",
                apartamento_novo = "",
                cep = cep,//cep
                cep_novo = "",
                endereco = venda.EnderecoInstalacao != null ? venda.EnderecoInstalacao.Logradouro : "",//rua
                endereco_novo = "",
                numero = numero,//numero
                numero_novo = "",
                bairro = config.Utiliza_Cadastro_Endereco && isCondominio ? venda.EnderecoInstalacao.Condominio_Predio.Bairro :
                         config.Utiliza_Cadastro_Endereco && isCadastroEndereco ? venda.EnderecoInstalacao.Endereco_Logradouro.Bairro.Descricao :
                         venda.EnderecoInstalacao != null ? venda.EnderecoInstalacao.Bairro : "",//bairro
                bairro_novo = "",
                cidade = !String.IsNullOrEmpty(idCidadeIxc) ? idCidadeIxc : "", //idcidade
                cidade_novo = "",
                complemento = venda.EnderecoInstalacao != null ? venda.EnderecoInstalacao.Complemento : "",//complemento
                complemento_novo = "",
                referencia = "",
                referencia_novo = "",
                latitude = venda.EnderecoInstalacao != null ? Convert.ToString(venda.EnderecoInstalacao.Latitude) : "",//lat
                latitude_novo = "",
                longitude = venda.EnderecoInstalacao != null ? Convert.ToString(venda.EnderecoInstalacao.Longitude) : "",//lon
                longitude_novo = "",
                tipo_localidade = constantesIntegracao.tipo_localidade,
                avalista_1 = "",
                avalista_2 = "",
                testemunha_assinatura_digital = "",
                document_photo = constantesIntegracao.document_photo,
                selfie_photo = constantesIntegracao.selfie_photo
            };
        }
        #endregion

        #region [GetViewObj - LoginAzzaResponse]
        public LoginIXCResponse GetViewObjLoginIXCResponse(string idCliente, Venda venda,
            Cliente_PlanoProduto cliente_PlanoProduto, string idContrato, string idGrupo, string idFilial, ConstantesIntegracaoIXC constantes)
        {
            var numeroCepIXC = IXCCrmBusiness.ValidacaoNumeroCep(venda.EnderecoInstalacao);
            string numero = numeroCepIXC.Numero;
            string cep = numeroCepIXC.Cep;

            var idCidade = venda?.EnderecoInstalacao?.Id_Cidade;
            var ibge = idCidade.HasValue ? _repositoryCidade.GetById(idCidade.ToString()).Codigo_IBGE : null;
            var idCidadeIxc = _repository.RequestCidade(ibge).registros.FirstOrDefault()?.id;

            return new LoginIXCResponse()
            {
                autenticacao = constantes.autenticacaoLogin,
                tipo_conexao_mapa = constantes.tipoConexaoMapa,//enum
                id_integracao = "",
                lte_id = "",
                id_cliente = idCliente,
                id_contrato = idContrato,
                id_filial = string.IsNullOrEmpty(idFilial) ? constantes.idFilial : idFilial,
                contrato_plano_venda_ = "",
                id_grupo = idGrupo,//pllogin
                login = idContrato,//login
                agent_circuit_id = "",
                senha_md5 = constantes.senhaMD5,
                senha = cliente_PlanoProduto.Cliente.CpfCnpj.Substring(0, 5),//senha
                usuario_router1 = "",
                senha_router1 = cliente_PlanoProduto.Cliente.CpfCnpj.Substring(0, 5),//senahar1
                senha_router2 = "",
                ssid_router_wifi = "",
                senha_rede_sem_fio = "",
                ssid_router_wifi_5ghz = "",
                senha_rede_sem_fio_5ghz = "",
                ping_traceroute = "",
                login_simultaneo = constantes.LoginSimultaneo,
                ativo = constantes.Ativo,
                online = constantes.Online,
                ultima_atualizacao = DateTime.Now.ToString("dd/MM/yyyy"),
                ip = "",
                ip_aviso = "",
                auto_preencher_ip = constantes.AutoPreencherIP,
                fixar_ip = constantes.FixarIP,
                relacionar_ip_ao_login = constantes.RelacionarIPLogin,
                framed_pd_ipv6 = "",
                framed_autopreencher_ipv6 = constantes.FramedAutopreencherIPV6,
                framed_fixar_ipv6 = constantes.FramedFixarIPV6,
                framed_relacionar_ipv6_ao_login = constantes.FrameRelacionarIPV6Login,
                pd_ipv6 = "",
                auto_preencher_ipv6 = constantes.AutoPreencherIPV6,
                fixar_ipv6 = constantes.FixarIPV6,
                relacionar_ipv6_ao_login = constantes.RelacionarIPV6Login,
                mac = "",
                autenticacao_por_mac = constantes.AutenticacaoPorMac,
                usuario_wpa2aes = "",
                senha_wpa2aes = "",
                autenticacao_wpa = "",
                id_porta_transmissor = "",
                auto_preencher_mac = constantes.AutoPreencherMac,
                relacionar_mac_ao_login = constantes.RelacionarMacLog,
                relacionar_concentrador_ao_login = "",
                pool_radius = "",
                id_radgrupos_pools = "",
                id_rad_dns = "",
                id_concentrador = "",//idconcentrador
                ip_concentrador = "",//ipconcentrados
                _interface = "",
                vlan = "",
                vlan_ip_rede = "",
                gw_vlan = "",
                service_tag_vlan = constantes.ServiceTagVLan,
                mtu = constantes.MTU,
                concentrador = "",//nomeconcentrador
                conexao = "",
                tipo_conexao = "",
                porta_http_nas = "",
                acct_session_id = "",
                tipo_vinculo_plano = constantes.TipoVinculoPlano,
                cliente_tem_a_senha = constantes.ClienteTemSenha,
                autenticacao_wps = constantes.AutenticacaoWPS,
                autenticacao_mac = constantes.AutenticacaoMac,
                tipo_acesso = constantes.TipoAcesso,
                porta_http = "",
                porta_router2 = "",
                ip_aux = "",
                porta_aux = "",
                ultima_conexao_inicial = "",
                ultima_conexao_final = "",
                motivo_desconexao = "",
                count_desconexao = "",
                tempo_conexao = "",
                tempo_conectado = "",
                download_atual = "",
                upload_atual = "",
                franquia_maximo = "",
                franquia_consumo = "",
                franquia_consumo_up = "",
                franquia_atingida = constantes.FranquiaAtingida,
                onu_compartilhada = "",
                id_df_projeto = "",//ctoIDprojeto
                id_transmissor = "",//ctoIDtransmissor
                modelo_tranmissor = "",
                interface_transmissao = "",
                interface_transmissao_fibra = "",
                id_caixa_ftth = "",//ctoID
                ftth_porta = "",//ctoPortadisponivel
                tronco = "",
                splitter = "",
                onu_mac = "",
                sinal_ultimo_atendimento = "",
                id_hardware = "",
                tipo_equipamento = "",
                metragem_interna = "",
                metragem_externa = "",
                id_reserva_rede_neutra = "",
                endereco_padrao_cliente = constantes.endereco_padrao_cliente,
                ponta = "",
                id_condominio = "",
                condominio_novo = "",
                bloco = "",
                bloco_novo = "",
                apartamento = "",
                apartamento_novo = "",
                cep = cep,//cep
                cep_novo = "",
                endereco = venda.EnderecoInstalacao.Logradouro,//rua
                endereco_novo = "",
                numero = numero,//numero
                numero_novo = "",
                bairro = venda.EnderecoInstalacao.Bairro,//bairro
                bairro_novo = "",
                cidade = idCidadeIxc,//idcidade
                cidade_novo = "",
                referencia = "",
                referencia_novo = "",
                complemento = "",
                complemento_novo = "",
                latitude = venda.EnderecoInstalacao.Latitude.ToString(),//lat
                latitude_novo = "",
                longitude = venda.EnderecoInstalacao.Longitude.ToString(),//lon
                longitude_novo = "",
                obs = constantes.obs
            };
        }

        #endregion

        /// <summary>
        /// Método para validação de endereço para incluir na requisição para o IXC
        /// </summary>
        /// <param name="endereco"></param>
        /// <returns></returns>
        private static EnderecoValidadoIXC ValidacaoNumeroCep(Endereco_Cliente endereco)
        {
            var retorno = new EnderecoValidadoIXC();
            if (endereco != null)
            {
                var numero = (Regex.Replace(endereco.Numero, @"^[0 ]+", "")).Trim();

                var caracteres = new List<string>() { "-", "." };
                var cep = endereco.Cep;
                foreach (var item in caracteres)
                {
                    cep = cep.Replace(item, string.Empty);
                }
                cep = Convert.ToUInt64(cep).ToString(@"00000\-000");



                retorno.Numero = (!String.IsNullOrEmpty(endereco?.Numero) && endereco?.Numero.Trim() != "0") ? endereco?.Numero.Trim() : "SN";
                retorno.Cep = cep;
            }

            return retorno;
        }
    }

}
