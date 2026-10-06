using Aniel.Connect.ConnectBusiness.T5.Consulta_de_Viabilidade;
using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Cliente;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.ConsultaCredito;
using Aniel.Connect.Repository.ConsultaViabilidade;
using Aniel.Connect.Repository.CriteriosParamCredito;
using Aniel.Connect.Repository.IntegracaoExterna;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.ParametrosConsultaCredito;
using Aniel.Connect.Repository.PendenciaEndereco;
using Aniel.Connect.Repository.PendenciaEnderecoAniel;
using Aniel.Connect.Repository.PendenciaFinanceira;
using Aniel.Connect.Repository.T5.ElementoRede;
using Aniel.Connect.Repository.T5.ElementoRedePorta;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.ConnectBusiness.Connect
{
    public class ClienteBusiness
    {
        private readonly IClienteRepository _repository;

        public ClienteBusiness(IClienteRepository repository)
        {
            this._repository = repository;
        }

        /// <summary>
        ///     Retorna o cliente associado ao ID informado
        /// </summary>
        /// <param name="id">ID do cliente</param>
        public Cliente GetById(long id)
        {
            return this._repository.GetById(id);
        }

        public void Update(Cliente cliente)
        {
            this._repository.Update(cliente);
        }

        public void AtualizarClientesOla(string urlOla, string senhaOla, string cnpj, string uuidProvedorOla, Cliente cliente)
        {
            Response resposta = new BaseController("", "").GetOla(urlOla, $"api/Param/GetToken/{senhaOla}");

            if (resposta.result)
            {
                UsuarioSetRequest usuarios = new UsuarioSetRequest();
                usuarios.tokenOperacao = resposta.message;
                usuarios.usuarios = new List<UsuarioSetModels>();


                usuarios.usuarios.Add(new UsuarioSetModels()
                {
                    idCliente = cliente.Id,
                    cnpjProvedor = cnpj,
                    uuidProvedor = uuidProvedorOla,
                    cpf_cnpj = cliente.CpfCnpj,
                    nome = cliente.NomeFantasia,
                    email = cliente.Email_Principal,
                    telefone = cliente.Telefone_Celular,
                    status = cliente.Status == Status_Cliente.Ativo ? "A" : "I"
                });


                Response clienteResponse = new BaseController("", "").PostOla<UsuarioSetRequest>(urlOla, "api/Provedor/ManterUsuarios", usuarios);

                if (!clienteResponse.result)
                    throw new Exception(clienteResponse.message);
            }
            else
            {
                throw new Exception(resposta.message);
            }
        }

        public void AtualizarIntegradoOla(long IdCliente)
        {
            this._repository.AtualizarIntegradoOla(IdCliente);
        }


        public void GetStatusProspecto(Cliente objBanco)
        {
            using (ISession sessionNh = HibernateUtil.GetSession())
            using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
            {
                IConsultaCreditoRepository repoConsultaCredito = new ConsultaCreditoRepository(sessionNh);
                ICriteriosParamCreditoRepository repoCriterios = new CriteriosParamCreditoRepository(sessionNh);
                IParametrosConsultaCreditoRepository repoParamsConsulta = new ParametrosConsultaCreditoRepository(sessionNh);
                IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(sessionNh);

                var consultaCreditoBusiness =
                    new ConsultaCreditoBusiness(
                        repoConsultaCredito,
                        repoCriterios,
                        repoParamsConsulta,
                        repoConfig
                    );

                IPendenciaEnderecoRepository repoPendenciaEnderecoConnect = new PendenciaEnderecoRepository(sessionNh);
                IPendenciaEnderecoAnielRepository repoPendenciaEnderecoAnielConnect = new PendenciaEnderecoAnielRepository(sessionNh);
                IIntegracaoExternaRepository repoIntegracaoExterna = new IntegracaoExternaRepository(sessionNh);

                var pendenciaEnderecoBusiness = new PendenciaEnderecoBusiness(repoPendenciaEnderecoConnect, repoConfig);

                IPendenciaFinanceiraRepository repoPendenciaFinanceiraConnect = new PendenciaFinanceiraRepository(sessionNh);
                var pendenciaFinanceiraBusiness = new PendenciaFinanceiraBusiness(repoPendenciaFinanceiraConnect, repoConfig);

                IConsultaViabilidadeRepository repoConsultaViabilidade = new ConsultaViabilidadeRepository(sessionT5Nh);
                IElementoRedeAnielRepository repoElemRede = new ElementoRedeAnielRepository(sessionT5Nh);
                IElementoRedePortaAnielRepository repoPorta = new ElementoRedePortaAnielRepository(sessionNh, sessionT5Nh);


                var consultaViabilidadeBusiness =
                    new ConsultaViabilidadeBusiness(
                        repoConsultaViabilidade,
                        repoElemRede,
                        repoPorta,
                        repoConfig
                    );

                var configBusiness = new ConfiguracoesGeraisBusiness(repoConfig);

                Configuracoes_Gerais config = configBusiness.GetConfiguracoesGerais();

                StatusEtapaVenda statusConsultaViabilidade = StatusEtapaVenda.Pendente;
                StatusEtapaVenda statusConsultaPendenciaEndereco = StatusEtapaVenda.Pendente;
                StatusEtapaVenda statusConsultaPendenciaFinanceira = StatusEtapaVenda.Pendente;
                StatusEtapaVenda statusConsultaCredito = StatusEtapaVenda.Pendente;

                if (!String.IsNullOrEmpty(objBanco.CpfCnpj) && config.ConsultaCreditoQualificaLead)
                {
                    if (objBanco.Tipo_Pessoa.Value == Tipo_Pessoa.Fisica)
                    {
                        statusConsultaCredito =
                            consultaCreditoBusiness.GetStatusConsultaCreditoRestricaoByCpf(objBanco.CpfCnpj);
                    }
                    else
                    {
                        statusConsultaCredito =
                            consultaCreditoBusiness.GetStatusConsultaCreditoRestricaoByCnpj(objBanco.CpfCnpj);
                    }
                }

                if (!String.IsNullOrEmpty(objBanco.CpfCnpj) && config.ConsultaPendenciaFinanceiraQualificaLead)
                {
                    if (objBanco.Tipo_Pessoa.Value == Tipo_Pessoa.Fisica)
                    {
                        statusConsultaPendenciaFinanceira =
                            pendenciaFinanceiraBusiness.GetStatusConsultaPendenciaFinanceiraByCpf(objBanco.CpfCnpj);
                    }
                    else
                    {
                        statusConsultaPendenciaFinanceira =
                            pendenciaFinanceiraBusiness.GetStatusConsultaPendenciaFinanceiraByCnpj(objBanco.CpfCnpj);
                    }
                }

                string logradouro;

                if (config.Utiliza_Cadastro_Endereco)
                {
                    logradouro =
                        objBanco.Endereco_Prospecto?.Endereco_Logradouro != null ?
                             objBanco.Endereco_Prospecto.Endereco_Logradouro.Descricao :
                             objBanco.Endereco_Prospecto?.Condominio_Predio?.Descricao;
                }
                else
                {
                    logradouro = objBanco.Endereco_Prospecto.Logradouro;
                }

                if (!String.IsNullOrEmpty(objBanco.CpfCnpj) && config.ConsultaPendenciaEnderecoQualificaLead)
                {
                    statusConsultaPendenciaEndereco =
                    pendenciaEnderecoBusiness.GetStatusConsultaPendenciaEndereco(logradouro, objBanco.Endereco_Prospecto.Numero);

                }

                if (!String.IsNullOrEmpty(objBanco.CpfCnpj) && config.ConsultaViabilidadeQualificaLead)
                {
                    statusConsultaViabilidade =
                        consultaViabilidadeBusiness.GetStatusConsultaViabilidade(
                            (objBanco.Endereco_Prospecto?.Latitude).GetValueOrDefault(0),
                            (objBanco.Endereco_Prospecto?.Longitude).GetValueOrDefault(0)
                        );
                }



                bool restricoesImpedemQualificacaoLead =
                    (config.ConsultaViabilidadeQualificaLead && config.ConsultaViabilidadeExibeLead && statusConsultaViabilidade != StatusEtapaVenda.Aprovado && statusConsultaViabilidade != StatusEtapaVenda.AprovadoManualmente) ||
                    (config.ConsultaPendenciaEnderecoQualificaLead && config.ConsultaPendenciaEnderecoExibeLead && statusConsultaPendenciaEndereco != StatusEtapaVenda.Aprovado && statusConsultaViabilidade != StatusEtapaVenda.AprovadoManualmente) ||
                    (config.ConsultaPendenciaFinanceiraQualificaLead && config.ConsultaPendenciaFinanceiraExibeLead && statusConsultaPendenciaFinanceira != StatusEtapaVenda.Aprovado && statusConsultaViabilidade != StatusEtapaVenda.AprovadoManualmente) ||
                    (config.ConsultaCreditoQualificaLead && config.ConsultaCreditoExibeLead && statusConsultaCredito != StatusEtapaVenda.Aprovado && statusConsultaViabilidade != StatusEtapaVenda.AprovadoManualmente);

                if (!restricoesImpedemQualificacaoLead)
                {
                    objBanco.Status_Prospecto =
                        !String.IsNullOrEmpty(objBanco.CpfCnpj) ?
                            Status_Prospecto.Lead_Qualificado :
                            Status_Prospecto.Lead_Nao_Qualificado;
                }
                else
                {
                    objBanco.Status_Prospecto = Status_Prospecto.Lead_Com_Restricao;
                }
            }

        }
    }
}
