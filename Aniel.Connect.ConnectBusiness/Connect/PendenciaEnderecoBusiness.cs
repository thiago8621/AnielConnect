using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.PendenciaEndereco;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.ConnectBusiness.Connect
{
    public class PendenciaEnderecoBusiness
    {
        private readonly IPendenciaEnderecoRepository _repository;
        private readonly IConfiguracoesGeraisRepository _repoConfig;

        public PendenciaEnderecoBusiness(IPendenciaEnderecoRepository repository,
                                         IConfiguracoesGeraisRepository repoConfig)
        {
            this._repository = repository;
            this._repoConfig = repoConfig;
        }

        public PendenciaEndereco GetConsultaPendenciaEndereco(string logradouro, string numero)
        {
            var pendenciaEndereco = this._repository.GetConsultaPendenciaEndereco(logradouro, numero);

            if (pendenciaEndereco != null)
            {
                Configuracoes_Gerais config = this._repoConfig.GetConfiguracoesGerais();

                if (config.ConsultaPendenciaEnderecoTempoValidade.HasValue && pendenciaEndereco != null)
                {
                    DateTime dataMaxValidadeConsulta = pendenciaEndereco.DataHoraCriacao.Value.Date.AddDays(config.ConsultaPendenciaEnderecoTempoValidade.Value);
                    bool consultaDentroValidade = DateTime.Now.Date <= dataMaxValidadeConsulta.Date;
                    if (!consultaDentroValidade)
                    {
                        return null;
                    }
                }
            }

            return pendenciaEndereco;
        }

        /// <summary>
        ///     Retorna os dados de uma consulta de pendência de endereço específica.
        ///     
        /// <para>
        ///     Esse método é utilizado para listar os dados da consulta utilizada para avaliar a etapa de alguma venda portanto
        ///     não devemos verificar se a consulta está dentro da data de validade configurada nas configurações da empresa pois, 
        ///     uma vez que a consulta é utilizada para a venda, temos que saber qual foram os dados utilizados para aprovar ou 
        ///     reprovar a etapa, mesmo que a consulta seja mais antiga
        /// </para>
        /// </summary>
        /// <param name="id">ID da consulta</param>
        public IEnumerable<PendenciaEnderecoListModel> GetPendenciasEnderecoById(long id)
        {
            IEnumerable<PendenciaEnderecoListModel> pendencias = new List<PendenciaEnderecoListModel>();
            Configuracoes_Gerais config = this._repoConfig.GetConfiguracoesGerais();

            Sinapse.Model.PendenciaEndereco consultaPendenciaEndereco = null;

            consultaPendenciaEndereco = this._repository.GetConsultaPendenciaEnderecoById(id);

            if (consultaPendenciaEndereco == null)
            {
                return pendencias;
            }

            pendencias =
                consultaPendenciaEndereco
                    .DadosConsulta
                    .Select(p => new PendenciaEnderecoListModel
                    {
                        Descricao = p.Descricao,

                        Cod_Cliente = p.CodCliente,
                        Cod_Contrato = p.CodContrato,
                        CodConexao = p.CodConexao,
                        CodFatura = p.CodFatura,
                        Cliente = p.Cliente,
                        Cpf = p.Cpf,
                        Cnpj = p.Cnpj,

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
                        Cidade = p.Cidade,
                        FlagCEP = p.FlagCEP
                    }).ToList();

            return pendencias;
        }

        public (bool temValidade, IEnumerable<PendenciaEnderecoListModel> pendencias) GetPendenciasEndereco(string logradouro, string numero)
        {
            IEnumerable<PendenciaEnderecoListModel> pendencias = null;
            Configuracoes_Gerais config = this._repoConfig.GetConfiguracoesGerais();

            Sinapse.Model.PendenciaEndereco consultaPendenciaEndereco = null;
            bool consultaDentroValidade = false;

            consultaPendenciaEndereco = this._repository.GetConsultaPendenciaEndereco(logradouro, numero);


            if (config.ConsultaPendenciaEnderecoTempoValidade.HasValue && consultaPendenciaEndereco != null)
            {
                DateTime dataMaxValidadeConsulta = consultaPendenciaEndereco.DataHoraCriacao.Value.Date.AddDays(config.ConsultaPendenciaEnderecoTempoValidade.Value);
                consultaDentroValidade = DateTime.Now.Date <= dataMaxValidadeConsulta.Date;
            }
            if (consultaDentroValidade && consultaPendenciaEndereco != null)
            {
                if (consultaPendenciaEndereco.DadosConsulta.Count() > 0)
                {
                    pendencias =
                consultaPendenciaEndereco
                    .DadosConsulta
                    .Select(p => new PendenciaEnderecoListModel
                    {
                        Descricao = p.Descricao,

                        Cod_Cliente = p.CodCliente,
                        Cod_Contrato = p.CodContrato,
                        CodConexao = p.CodConexao,
                        CodFatura = p.CodFatura,
                        Cliente = p.Cliente,
                        Cpf = p.Cpf,
                        Cnpj = p.Cnpj,

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

        public (bool temValidade, IEnumerable<PendenciaEnderecoListModel> pendencias) GetPendenciasEnderecoMobile(string logradouro, string numero)
        {
            IEnumerable<PendenciaEnderecoListModel> pendencias = null;
            Configuracoes_Gerais config = this._repoConfig.GetConfiguracoesGerais();

            Sinapse.Model.PendenciaEndereco consultaPendenciaEndereco = null;
            bool consultaDentroValidade = false;

            consultaPendenciaEndereco = this._repository.GetConsultaPendenciaEndereco(logradouro, numero);


            if (consultaPendenciaEndereco != null)
            {
                if (consultaPendenciaEndereco.DadosConsulta.Count() > 0)
                {
                    pendencias =
                consultaPendenciaEndereco
                    .DadosConsulta
                    .Select(p => new PendenciaEnderecoListModel
                    {
                        Descricao = p.Descricao,

                        Cod_Cliente = p.CodCliente,
                        Cod_Contrato = p.CodContrato,
                        CodConexao = p.CodConexao,
                        CodFatura = p.CodFatura,
                        Cliente = p.Cliente,
                        Cpf = p.Cpf,
                        Cnpj = p.Cnpj,

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
        ///     Retorna o status que indica se há uma consulta de pendência no endereço válida 
        ///     registrada no sistema para o endereço informado
        /// </summary>
        /// <param name="logradouro">Logradouro do endereço</param>
        /// <param name="numero">Número do endereço</param>
        public StatusEtapaVenda GetStatusConsultaPendenciaEndereco(string logradouro, string numero)
        {
            bool hasConsultaPendenciaEndereco =
                this._repository.GetConsultaPendenciaEndereco(logradouro, numero) != null;
            StatusEtapaVenda status = StatusEtapaVenda.Pendente;

            if (hasConsultaPendenciaEndereco)
            {
                (bool dentroValidade, IEnumerable<PendenciaEnderecoListModel> pendencias) pendenciasEndereco =
                    this.GetPendenciasEndereco(
                        logradouro,
                        numero
                    );
                if (pendenciasEndereco.dentroValidade && pendenciasEndereco.pendencias != null && pendenciasEndereco.pendencias.Count() == 0)
                {
                    status = StatusEtapaVenda.Aprovado;
                }
                else if (pendenciasEndereco.dentroValidade && pendenciasEndereco.pendencias != null && pendenciasEndereco.pendencias.Count() > 0)
                {
                    Configuracoes_Gerais config = this._repoConfig.GetConfiguracoesGerais();

                    if (config.ConsultaPendenciaEnderecoRestringeVenda)
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
        ///     Grava um registro de pendência de endereço vindo do MKSolutions
        /// </summary>
        /// <param name="logradouro">Logradouro do endereço que foi verificado no MKSolutions</param>
        /// <param name="numero">Número do endereço que foi verificado no MKSolutions</param>
        /// <param name="pendencias">Lista de dados da pendência no endereço</param>
        public PendenciaEndereco GravarPendenciaEndereco(string logradouro, string numero, IEnumerable<PendenciaEnderecoListModel> pendencias)
        {
            var consultaPendencia = new PendenciaEndereco
            {
                DataHoraCriacao = DateTime.Now,
                Logradouro = logradouro,
                Numero = numero
            };

            foreach (PendenciaEnderecoListModel pendencia in pendencias)
            {
                consultaPendencia.DadosConsulta.Add(new PendenciaEnderecoDados
                {
                    DataHoraCriacao = DateTime.Now,
                    Consulta = consultaPendencia,

                    Descricao = pendencia.Descricao,

                    CodCliente = pendencia.Cod_Cliente,
                    CodContrato = pendencia.Cod_Contrato,
                    CodConexao = pendencia.CodConexao,
                    CodFatura = pendencia.CodFatura,
                    Cliente = pendencia.Cliente,
                    Cpf = pendencia.Cpf,
                    Cnpj = pendencia.Cnpj,

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
                    Cidade = pendencia.Cidade,
                    FlagCEP = pendencia.FlagCEP
                });
            }

            this._repository.Save(consultaPendencia);

            return consultaPendencia;
        }
    }
}
