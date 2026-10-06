using Aniel.Connect.ConnectBusiness.Azza;
using Aniel.Connect.ConnectBusiness.MK;
using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.EtapaVenda;
using Aniel.Connect.Repository.IntegracaoExterna;
using Aniel.Connect.Repository.MK.PendenciaEndereco;
using Aniel.Connect.Repository.MK.PendenciaFinanceira;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.PendenciaEnderecoAniel;
using Aniel.Connect.Repository.PendenciaFinanceira;
using Aniel.Connect.Repository.PendenciaFinanceiraAniel;
using Aniel.Connect.Repository.PendenciaFinanceiraIntegracao;
using Aniel.Connect.Repository.Repository.Azza.PendenciaEndereco;
using Aniel.Connect.Repository.Repository.Azza.PendenciaFinanceira;
using Aniel.Connect.Repository.Repository.PendenciaFinanceiraAniel;
using Aniel.Connect.Repository.Repository.T5.ConfigAnielIntegrator;
using Aniel.Connect.Repository.Venda;
using Aniel.Connect.Repository.Voalle.PendenciaFinanceira;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

namespace Aniel.Connect.ConnectBusiness.Connect
{
    public class PendenciaFinanceiraBusiness
    {
        private readonly IPendenciaFinanceiraRepository _repository;
        private readonly IConfiguracoesGeraisRepository _repoConfig;
        //private readonly IPendenciaFinanceiraIntegracaoRepository _integracao;        

        public PendenciaFinanceiraBusiness(IPendenciaFinanceiraRepository repository, IConfiguracoesGeraisRepository repoConfig/*, IPendenciaFinanceiraIntegracaoRepository integracao*/)
        {
            this._repository = repository;
            this._repoConfig = repoConfig;
            //this._integracao = integracao;
        }

        /// <summary>
        ///     Retorna o registo referente à uma consulta de pendência financeira realizada para o CPF informado
        /// </summary>
        /// <param name="cpf">CPF do cliente</param>
        public Sinapse.Model.PendenciaFinanceira GetConsultaPendenciaFinanceiraByCpf(string cpf)
        {
            var pendenciaFinanceira = this._repository.GetConsultaPendenciaFinanceiraByCpf(cpf);

            if (pendenciaFinanceira != null)
            {
                Configuracoes_Gerais config = this._repoConfig.GetConfiguracoesGerais();

                if (config.ConsultaPendenciaFinanceiraTempoValidade.HasValue)
                {
                    DateTime dataMaxValidadeConsulta = pendenciaFinanceira.DataHoraCriacao.Value.Date.AddDays(config.ConsultaPendenciaFinanceiraTempoValidade.Value);
                    bool consultaDentroValidade = DateTime.Now.Date <= dataMaxValidadeConsulta.Date;
                    if (!consultaDentroValidade)
                    {
                        return null;
                    }
                }
                else 
                {
                    
                }
            }

            return pendenciaFinanceira;
        }

        /// <summary>
        ///     Retorna o registo referente à uma consulta de pendência financeira realizada para o CNPJ informado
        /// </summary>
        /// <param name="cnpj">CNPJ do cliente</param>
        public Sinapse.Model.PendenciaFinanceira GetConsultaPendenciaFinanceiraByCnpj(string cnpj)
        {
            var pendenciaFinanceira = this._repository.GetConsultaPendenciaFinanceiraByCnpj(cnpj);

            if (pendenciaFinanceira != null)
            {
                Configuracoes_Gerais config = this._repoConfig.GetConfiguracoesGerais();

                if (config.ConsultaPendenciaFinanceiraTempoValidade.HasValue)
                {
                    DateTime dataMaxValidadeConsulta = pendenciaFinanceira.DataHoraCriacao.Value.Date.AddDays(config.ConsultaPendenciaFinanceiraTempoValidade.Value);
                    bool consultaDentroValidade = DateTime.Now.Date <= dataMaxValidadeConsulta.Date;

                    if (!consultaDentroValidade)
                    {
                        return null;
                    }
                }
            }

            return pendenciaFinanceira;
        }

        /// <summary>
        ///     Retorna os dados de uma consulta de pendência financeira específica.
        ///     
        /// <para>
        ///     Esse método é utilizado para listar os dados da consulta utilizada para avaliar a etapa de alguma venda portanto
        ///     não devemos verificar se a consulta está dentro da data de validade configurada nas configurações da empresa pois, 
        ///     uma vez que a consulta é utilizada para a venda, temos que saber qual foram os dados utilizados para aprovar ou 
        ///     reprovar a etapa, mesmo que a consulta seja mais antiga
        /// </para>
        /// </summary>
        /// <param name="id">ID da consulta</param>
        public IEnumerable<PendenciaFinanceiraListModel> GetPendenciasFinanceirasById(long id)
        {
            IEnumerable<PendenciaFinanceiraListModel> pendencias = new List<PendenciaFinanceiraListModel>();

            Sinapse.Model.PendenciaFinanceira consultaPendenciaFinanceira = this._repository.GetConsultaPendenciaFinanceiraById(id);

            if (consultaPendenciaFinanceira == null)
            {
                return pendencias;
            }

            pendencias =
                consultaPendenciaFinanceira
                    .DadosConsulta
                    .ToList()
                    .Select(p => new PendenciaFinanceiraListModel
                    {
                        Descricao = p.Descricao,

                        Cod_Cliente = p.CodCliente,
                        Cod_Contrato = p.CodContrato,
                        CodConexao = p.CodConexao,
                        CodFatura = p.CodFatura,
                        Cliente = p.Cliente,
                        CpfCnpj = !String.IsNullOrEmpty(p.Cpf) ? p.Cpf : p.Cnpj,

                        Data_Vencimento = p.DataVencimento,
                        Valor_Total = p.ValorTotal,

                        Fatura_Suspensa = p.FaturaSuspensa,
                        Motivo_Suspensao = p.MotivoSuspensao,
                        Contrato_Cancelado = p.ContratoCancelado,
                        Conexao_Bloqueada = p.ConexaoBloqueada,
                        Liquidado = p.Liquidado,

                        Logradouro = p.Logradouro,
                        Numero = p.Numero,
                        Bairro = p.Bairro,
                        Cidade = p.Cidade
                    }).ToList();

            return pendencias;
        }

        public (bool temValidade, IEnumerable<PendenciaFinanceiraListModel> pendencias) GetPendenciasFinanceirasByCpf(string cpf)
        {
            IEnumerable<PendenciaFinanceiraListModel> pendencias = null;
            bool consultaDentroValidade = false;

            Configuracoes_Gerais config = this._repoConfig.GetConfiguracoesGerais();
            Sinapse.Model.PendenciaFinanceira consultaPendenciaFinanceira = this._repository.GetConsultaPendenciaFinanceiraByCpf(cpf);           

            if (config.ConsultaPendenciaFinanceiraTempoValidade.HasValue && consultaPendenciaFinanceira != null)
            {
                DateTime dataMaxValidadeConsulta = consultaPendenciaFinanceira.DataHoraCriacao.Value.Date.AddDays(config.ConsultaPendenciaFinanceiraTempoValidade.Value);
                consultaDentroValidade = DateTime.Now.Date <= dataMaxValidadeConsulta.Date;
            }

            if(consultaDentroValidade && consultaPendenciaFinanceira != null ) 
            {
                if (consultaPendenciaFinanceira.DadosConsulta.Count() > 0) 
                {
                    pendencias =
                        consultaPendenciaFinanceira
                            .DadosConsulta
                            .ToList()
                            .Select(p => new PendenciaFinanceiraListModel
                            {
                                Descricao = p.Descricao,

                                    Cod_Cliente = p.CodCliente,
                                Cod_Contrato = p.CodContrato,
                                CodConexao = p.CodConexao,
                                CodFatura = p.CodFatura,
                                Cliente = p.Cliente,
                                CpfCnpj = !String.IsNullOrEmpty(p.Cpf) ? p.Cpf : p.Cnpj,

                                    Data_Vencimento = p.DataVencimento,
                                Valor_Total = p.ValorTotal,

                                    Fatura_Suspensa = p.FaturaSuspensa,
                                Motivo_Suspensao = p.MotivoSuspensao,
                                Contrato_Cancelado = p.ContratoCancelado,
                                Conexao_Bloqueada = p.ConexaoBloqueada,
                                Liquidado = p.Liquidado,

                                    Logradouro = p.Logradouro,
                                Numero = p.Numero,
                                Bairro = p.Bairro,
                                Cidade = p.Cidade
                            }).ToList();
                }
            }
            return (consultaDentroValidade, pendencias);
        }

        public (bool temValidade,IEnumerable<PendenciaFinanceiraListModel> pendencias )GetPendenciasFinanceirasByCnpj(string cnpj)
        {
            IEnumerable<PendenciaFinanceiraListModel> pendencias = new List<PendenciaFinanceiraListModel>();
            bool consultaDentroValidade = false;
            Configuracoes_Gerais config = this._repoConfig.GetConfiguracoesGerais();
            Sinapse.Model.PendenciaFinanceira consultaPendenciaFinanceira = this._repository.GetConsultaPendenciaFinanceiraByCnpj(cnpj);

            if (config.ConsultaPendenciaFinanceiraTempoValidade.HasValue && consultaPendenciaFinanceira != null)
            {
                DateTime dataMaxValidadeConsulta = consultaPendenciaFinanceira.DataHoraCriacao.Value.Date.AddDays(config.ConsultaPendenciaFinanceiraTempoValidade.Value);
                consultaDentroValidade = DateTime.Now.Date <= dataMaxValidadeConsulta.Date;
            }

            if (consultaDentroValidade && consultaPendenciaFinanceira != null)
            {
                if (consultaPendenciaFinanceira.DadosConsulta.Count() > 0)
                {
                    pendencias =
                        consultaPendenciaFinanceira
                            .DadosConsulta
                            .ToList()
                            .Select(p => new PendenciaFinanceiraListModel
                            {
                                Descricao = p.Descricao,

                                Cod_Cliente = p.CodCliente,
                                Cod_Contrato = p.CodContrato,
                                CodConexao = p.CodConexao,
                                CodFatura = p.CodFatura,
                                Cliente = p.Cliente,
                                CpfCnpj = !String.IsNullOrEmpty(p.Cpf) ? p.Cpf : p.Cnpj,

                                Data_Vencimento = p.DataVencimento,
                                Valor_Total = p.ValorTotal,

                                Fatura_Suspensa = p.FaturaSuspensa,
                                Motivo_Suspensao = p.MotivoSuspensao,
                                Contrato_Cancelado = p.ContratoCancelado,
                                Conexao_Bloqueada = p.ConexaoBloqueada,
                                Liquidado = p.Liquidado,

                                Logradouro = p.Logradouro,
                                Numero = p.Numero,
                                Bairro = p.Bairro,
                                Cidade = p.Cidade
                            }).ToList();
                }
            }

            return (consultaDentroValidade, pendencias);
        }

        #region Mobile;

        public (bool temValidade, IEnumerable<PendenciaFinanceiraListModel> pendencias) GetPendenciasFinanceirasByCpfMobile(string cpf)
        {
            IEnumerable<PendenciaFinanceiraListModel> pendencias = null;
            bool consultaDentroValidade = false;

            Configuracoes_Gerais config = this._repoConfig.GetConfiguracoesGerais();
            Sinapse.Model.PendenciaFinanceira consultaPendenciaFinanceira = this._repository.GetConsultaPendenciaFinanceiraByCpf(cpf);

           

            if (consultaPendenciaFinanceira != null)
            {
                if (consultaPendenciaFinanceira.DadosConsulta.Count() > 0)
                {
                    pendencias =
                        consultaPendenciaFinanceira
                            .DadosConsulta
                            .ToList()
                            .Select(p => new PendenciaFinanceiraListModel
                            {
                                Descricao = p.Descricao,

                                Cod_Cliente = p.CodCliente,
                                Cod_Contrato = p.CodContrato,
                                CodConexao = p.CodConexao,
                                CodFatura = p.CodFatura,
                                Cliente = p.Cliente,
                                CpfCnpj = !String.IsNullOrEmpty(p.Cpf) ? p.Cpf : p.Cnpj,

                                Data_Vencimento = p.DataVencimento,
                                Valor_Total = p.ValorTotal,

                                Fatura_Suspensa = p.FaturaSuspensa,
                                Motivo_Suspensao = p.MotivoSuspensao,
                                Contrato_Cancelado = p.ContratoCancelado,
                                Conexao_Bloqueada = p.ConexaoBloqueada,
                                Liquidado = p.Liquidado,

                                Logradouro = p.Logradouro,
                                Numero = p.Numero,
                                Bairro = p.Bairro,
                                Cidade = p.Cidade
                            }).ToList();
                }
            }
            return (consultaDentroValidade, pendencias);
        }


        public (bool temValidade, IEnumerable<PendenciaFinanceiraListModel> pendencias) GetPendenciasFinanceirasByCnpjMobile(string cnpj)
        {
            IEnumerable<PendenciaFinanceiraListModel> pendencias = new List<PendenciaFinanceiraListModel>();
            bool consultaDentroValidade = false;
            Configuracoes_Gerais config = this._repoConfig.GetConfiguracoesGerais();
            Sinapse.Model.PendenciaFinanceira consultaPendenciaFinanceira = this._repository.GetConsultaPendenciaFinanceiraByCnpj(cnpj);

           

            if (consultaPendenciaFinanceira != null)
            {
                if (consultaPendenciaFinanceira.DadosConsulta.Count() > 0)
                {
                    pendencias =
                        consultaPendenciaFinanceira
                            .DadosConsulta
                            .ToList()
                            .Select(p => new PendenciaFinanceiraListModel
                            {
                                Descricao = p.Descricao,

                                Cod_Cliente = p.CodCliente,
                                Cod_Contrato = p.CodContrato,
                                CodConexao = p.CodConexao,
                                CodFatura = p.CodFatura,
                                Cliente = p.Cliente,
                                CpfCnpj = !String.IsNullOrEmpty(p.Cpf) ? p.Cpf : p.Cnpj,

                                Data_Vencimento = p.DataVencimento,
                                Valor_Total = p.ValorTotal,

                                Fatura_Suspensa = p.FaturaSuspensa,
                                Motivo_Suspensao = p.MotivoSuspensao,
                                Contrato_Cancelado = p.ContratoCancelado,
                                Conexao_Bloqueada = p.ConexaoBloqueada,
                                Liquidado = p.Liquidado,

                                Logradouro = p.Logradouro,
                                Numero = p.Numero,
                                Bairro = p.Bairro,
                                Cidade = p.Cidade
                            }).ToList();
                }
            }

            return (consultaDentroValidade, pendencias);
        }


        #endregion

        /// <summary>
        ///     Retorna o status que indica se há uma consulta de pendência financeira válida 
        ///     registrada no sistema para o CPF informado
        /// </summary>
        /// <param name="cpf">CPF da consulta de pendência</param>
        public Sinapse.Model.enumerados.StatusEtapaVenda GetStatusConsultaPendenciaFinanceiraByCpf(string cpf)
        {
            bool hasConsultaPendenciaFinanceira =
                this._repository.GetConsultaPendenciaFinanceiraByCpf(cpf) != null;
            var status = StatusEtapaVenda.Pendente;

            if (hasConsultaPendenciaFinanceira)
            {
                (bool dentroValidade, IEnumerable<PendenciaFinanceiraListModel> pendencias) pendenciasFinanceira = 
                    this.GetPendenciasFinanceirasByCpf(cpf);

                if (pendenciasFinanceira.dentroValidade && pendenciasFinanceira.pendencias != null && pendenciasFinanceira.pendencias.Count() == 0)
                {
                    status = StatusEtapaVenda.Aprovado;
                }
                else if (pendenciasFinanceira.dentroValidade && pendenciasFinanceira.pendencias != null && pendenciasFinanceira.pendencias.Count() > 0)
                {

                    Configuracoes_Gerais config = this._repoConfig.GetConfiguracoesGerais();

                    if (config.ConsultaPendenciaFinanceiraRestringeVenda)
                        status = StatusEtapaVenda.Reprovado;
                    else
                        status = StatusEtapaVenda.AguardandoAprovacaoGestor;

                }
                else 
                {
                    status = StatusEtapaVenda.Pendente;
                }
            }
            return status;
        }
        
        /// <summary>
        ///     Retorna o status que indica se há uma consulta de pendência financeira válida 
        ///     registrada no sistema para o CNPJ informado
        /// </summary>
        /// <param name="cnpj">CNPJ da consulta de pendência</param>
        public Sinapse.Model.enumerados.StatusEtapaVenda GetStatusConsultaPendenciaFinanceiraByCnpj(string cnpj)
        {
            bool hasConsultaPendenciaFinanceira =
                this._repository.GetConsultaPendenciaFinanceiraByCnpj(cnpj) != null;
            var status = StatusEtapaVenda.Pendente;

            if (hasConsultaPendenciaFinanceira)
            {
                (bool dentroValidade, IEnumerable<PendenciaFinanceiraListModel> pendencias) pendenciasFinanceira = 
                    this.GetPendenciasFinanceirasByCnpj(cnpj);

                if (pendenciasFinanceira.dentroValidade && pendenciasFinanceira.pendencias != null && pendenciasFinanceira.pendencias.Count() == 0)
                {
                    status = StatusEtapaVenda.Aprovado;
                }
                else if (pendenciasFinanceira.dentroValidade && pendenciasFinanceira.pendencias != null && pendenciasFinanceira.pendencias.Count() > 0)
                {
                    Configuracoes_Gerais config = this._repoConfig.GetConfiguracoesGerais();

                    if (config.ConsultaPendenciaFinanceiraRestringeVenda)
                        status = StatusEtapaVenda.Reprovado;
                    else
                        status = StatusEtapaVenda.AguardandoAprovacaoGestor;
                }
                else
                {
                    status = StatusEtapaVenda.Pendente;
                }
            }

            return status;
        }

        /// <summary>
        ///     Grava um registro de consulta de pendência financeira vindo do MKSolutions ou Connect
        /// </summary>
        /// <param name="documento">CPF ou CNPJ do cliente</param>
        /// <param name="tipoPessoa">Tipo de pessoa do cliente (pessoa física ou jurídica)</param>
        /// <param name="pendencias">Lista de dados das pendências financeiras do cliente</param>
        public Sinapse.Model.PendenciaFinanceira GravarPendenciaFinanceira(string documento, Tipo_Pessoa tipoPessoa, IEnumerable<PendenciaFinanceiraListModel> pendencias)
        {
            var consultaPendenciaFinanceira = new Sinapse.Model.PendenciaFinanceira();

            consultaPendenciaFinanceira.DataHoraCriacao = DateTime.Now;

            documento = documento.Replace(".", String.Empty)
                                 .Replace("-", String.Empty)
                                 .Replace("/", String.Empty);

            if (tipoPessoa == Tipo_Pessoa.Fisica)
            {
                consultaPendenciaFinanceira.Cpf = documento;
            }
            else
            {
                consultaPendenciaFinanceira.Cnpj = documento;
            }

            foreach (PendenciaFinanceiraListModel pendencia in pendencias)
            {
                consultaPendenciaFinanceira.DadosConsulta.Add(new Sinapse.Model.PendenciaFinanceiraDados
                {
                    DataHoraCriacao = DateTime.Now,
                    Consulta = consultaPendenciaFinanceira,

                    Descricao = pendencia.Descricao,

                    CodCliente = pendencia.Cod_Cliente,
                    CodContrato = pendencia.Cod_Contrato,
                    CodConexao = pendencia.CodConexao,
                    CodFatura = pendencia.CodFatura,
                    Cliente = pendencia.Cliente,
                    Cpf = consultaPendenciaFinanceira.Cpf,
                    Cnpj = consultaPendenciaFinanceira.Cnpj,

                    DataVencimento = pendencia.Data_Vencimento,
                    ValorTotal = pendencia.Valor_Total,

                    FaturaSuspensa = pendencia.Fatura_Suspensa,
                    MotivoSuspensao = pendencia.Motivo_Suspensao,
                    ContratoCancelado = pendencia.Contrato_Cancelado,
                    ConexaoBloqueada = pendencia.Conexao_Bloqueada,
                    Liquidado = pendencia.Liquidado,

                    Logradouro = pendencia.Logradouro,
                    Numero = pendencia.Numero,
                    Bairro = pendencia.Bairro,
                    Cidade = pendencia.Cidade
                });
            }

            this._repository.Save(consultaPendenciaFinanceira);

            return consultaPendenciaFinanceira;
        }

    }
}
