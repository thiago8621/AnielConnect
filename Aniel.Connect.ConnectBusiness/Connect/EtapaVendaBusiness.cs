using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Cliente;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.EtapaVenda;
using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aniel.Connect.ConnectBusiness.Connect
{
    public class EtapaVendaBusiness
    {
        private readonly IEtapaVendaRepository _repository;
        private readonly IConfiguracoesGeraisRepository _repoConfig;
        private readonly IClienteRepository _repoCliente;
        private readonly long _cliente;

        public EtapaVendaBusiness(IEtapaVendaRepository repository, IConfiguracoesGeraisRepository repoConfig)
        {
            this._repository = repository;
            this._repoConfig = repoConfig;
        }

        public EtapaVendaBusiness(IEtapaVendaRepository repository, IConfiguracoesGeraisRepository repoConfig, IClienteRepository repoCliente, long idCliente)
        {
            this._repository = repository;
            this._repoConfig = repoConfig;
            this._repoCliente = repoCliente;
            this._cliente = idCliente;
        }

        /// <summary>
        ///     Retorna uma lista com todas as etapas de venda habilitadas, incluindo 
        ///     as que devem ser habilitadas na tela de configurações da empresa
        /// </summary>
        /// <param name="idVenda">ID da venda</param>
        public IEnumerable<EnumEtapaVenda> GetEtapasVendaHabilitadas(long idVenda)
        {
            var etapasEncontradas = idVenda != 0 ? this._repository.GetByVenda(idVenda) : new List<EtapaVenda>();

            // Caso a venda seja nova pegamos as etapas das configurações da empresa
            if (idVenda == 0 || etapasEncontradas.Count() == 0)
            {
                // A lista é inicializada com as etapas que não podem ser desabilitadas
                // nas configurações da empresa
                var etapas = new List<EnumEtapaVenda>
                {
                    EnumEtapaVenda.DadosGerais,
                    EnumEtapaVenda.Financeiro,
                    EnumEtapaVenda.PlanoProduto,
                    
                    //EnumEtapaVenda.Resumo,
                    EnumEtapaVenda.TermoAdesao,
                };

                //Configuração para aquisição da etapa de checklist                
                Cliente clienteEncontrado = new Cliente();
                if (this._cliente > 0)
                {
                    clienteEncontrado = this._repoCliente.GetById(this._cliente);

                    var quizHabilitados = ValidarEtapaChecklist(clienteEncontrado);
                    if (quizHabilitados.Count() > 0)
                    {
                        etapas.Add(EnumEtapaVenda.Checklist);
                    }
                }

                Configuracoes_Gerais config = this._repoConfig.GetConfiguracoesGerais();

                if (config.ConsultaViabilidadeExibeVenda)
                {
                    etapas.Add(EnumEtapaVenda.ConsultaViabilidade);
                }

                if (config.ConsultaPendenciaFinanceiraExibeVenda)
                {
                    etapas.Add(EnumEtapaVenda.ConsultaPendenciaFinanceira);
                }

                if (config.ConsultaPendenciaEnderecoExibeVenda)
                {
                    etapas.Add(EnumEtapaVenda.ConsultaPendenciaEndereco);
                }

                if (config.ConsultaCreditoExibeVenda)
                {
                    etapas.Add(EnumEtapaVenda.ConsultaCredito);
                }

                //etapas.Add(EnumEtapaVenda.CartaoCredito);

                return etapas.OrderBy(etapa => etapa);
            }
            // Caso a venda já tenha sido gravada pegamos as etapas da TB_ETAPA_VENDA
            else
            {
                return etapasEncontradas.OrderBy(e => e.Etapa)
                                        .Select(e => e.Etapa);
            }
        }

        /// <summary>
        ///     Retorna uma lista com todas as etapas de venda habilitadas, incluindo 
        ///     as que devem ser habilitadas na tela de configurações da empresa
        /// </summary>
        /// <param name="idVenda">ID da venda</param>
        public IEnumerable<EnumEtapaVenda> GetEtapasTrocasHabilitadas(long idTroca)
        {
            var etapasEncontradas = idTroca != 0 ? this._repository.GetByTrocaTitularidade(idTroca) : new List<EtapaVenda>();

            // Caso a venda seja nova pegamos as etapas das configurações da empresa
            if (idTroca == 0 || etapasEncontradas.Count() == 0)
            {
                // A lista é inicializada com as etapas que não podem ser desabilitadas
                // nas configurações da empresa
                var etapas = new List<EnumEtapaVenda>
                {
                    EnumEtapaVenda.DadosGerais,
                    EnumEtapaVenda.TermoAdesao,
                };

                //Configuração para aquisição da etapa de checklist                
                Cliente clienteEncontrado = new Cliente();
                if (this._cliente > 0)
                {
                    clienteEncontrado = this._repoCliente.GetById(this._cliente);

                    var quizHabilitados = ValidarEtapaChecklist(clienteEncontrado);
                    if (quizHabilitados.Count() > 0)
                    {
                        etapas.Add(EnumEtapaVenda.Checklist);
                    }
                }

                Configuracoes_Gerais config = this._repoConfig.GetConfiguracoesGerais();

                if (config.ConsultaViabilidadeExibeVenda)
                {
                    etapas.Add(EnumEtapaVenda.ConsultaViabilidade);
                }

                if (config.ConsultaPendenciaFinanceiraExibeVenda)
                {
                    etapas.Add(EnumEtapaVenda.ConsultaPendenciaFinanceira);
                }

                if (config.ConsultaPendenciaEnderecoExibeVenda)
                {
                    etapas.Add(EnumEtapaVenda.ConsultaPendenciaEndereco);
                }

                if (config.ConsultaCreditoExibeVenda)
                {
                    etapas.Add(EnumEtapaVenda.ConsultaCredito);
                }

                //etapas.Add(EnumEtapaVenda.CartaoCredito);

                return etapas.OrderBy(etapa => etapa);
            }
            // Caso a venda já tenha sido gravada pegamos as etapas da TB_ETAPA_VENDA
            else
            {
                return etapasEncontradas.OrderBy(e => e.Etapa)
                                        .Select(e => e.Etapa);
            }
        }



        /// <summary>
        ///  Validação para determinar se a etapa de Checklist vai ser exibida.
        /// </summary>
        /// <param name="cliente">  Objeto do Cliente cujos parametros serão verificados. </param>
        /// <param name="parametrosObrigatorios">   Configuração que determina se todos os parametros serão usados ou não. </param>
        /// <returns></returns>
        private IEnumerable<Quiz> ValidarEtapaChecklist(Cliente cliente)
        {
            var retorno = new List<Quiz>();
            try
            {
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                using (var repoCheck = new Conexao<Quiz>(sessionT5))
                {
                    IQueryable<Quiz> checklists = repoCheck.QueryableSelect().Where(p => p.Tipo == TipoChecklist.Comercial && p.Ativo == "S");

                    long? idQualMeio = cliente?.Qual_Meio?.Id;
                    long? idCategoria = cliente?.Categoria_Cliente?.Id;
                    Tipo_Pessoa? tipoPessoa = cliente?.Tipo_Pessoa;

                    //Parametros são validados por eliminação (caso quaisquer um dos parametros não seja nulo e diferente do valor estabelecido pelo cliente
                    //o checklist será inviabilizado)

                    //ideia 1 -- pontuação (igual Workflow)
                    //var checklistCandidatos = checklists.ToList();
                    //var listagemPontuacaoChecklist = new List<(long pontuacao, long maiorSoma, long idChecklist)>();

                    //foreach (var checklist in checklistCandidatos) 
                    //{
                    //    long maiorSoma = checklist.IdMeioCaptacao != null && checklist.IdCategoriaCliente != null && checklist.TipoPessoa != null ? 3 :
                    //                     checklist.IdCategoriaCliente != null && checklist.TipoPessoa != null ? 2:
                    //                     checklist.TipoPessoa != null ? 1 : 0;

                    //    (long pontuacao, long maiorSoma, long idChecklist) valoresFiltro = (0, maiorSoma, checklist.Id);

                    //    if (checklist.IdMeioCaptacao != null && checklist.IdMeioCaptacao == idQualMeio) 
                    //    {
                    //        valoresFiltro.pontuacao++;
                    //    }

                    //    if (checklist.IdCategoriaCliente != null && checklist.IdCategoriaCliente == idCategoria)
                    //    {
                    //        valoresFiltro.pontuacao++;
                    //    }

                    //    if (checklist.TipoPessoa != null && checklist.TipoPessoa == tipoPessoa)
                    //    {
                    //        valoresFiltro.pontuacao++;
                    //    }

                    //    listagemPontuacaoChecklist.Add(valoresFiltro);
                    //}
                    //var maioresPontuadores = listagemPontuacaoChecklist.Where(p => p.pontuacao == p.maiorSoma)
                    //    //.ToList()
                    //    .Select(p => p.idChecklist).ToList();

                    //checklistCandidatos = checklistCandidatos.Where(p => p.Id.IsIn(maioresPontuadores)).ToList();                   

                    //retorno.AddRange(checklistCandidatos);

                    //ideia 2 -- Agrupamento

                    var checklistsValidados = checklists.Where(p =>
                                                              (p.TipoPessoa == null || p.TipoPessoa == tipoPessoa) &&
                                                              (p.IdMeioCaptacao == null || p.IdMeioCaptacao == idQualMeio) &&
                                                              (p.IdCategoriaCliente == null || p.IdCategoriaCliente == idCategoria)
                                                              ).ToList();

                    bool hasObjPreenchidos = checklistsValidados.Any(p => p.IdCategoriaCliente != null || p.IdMeioCaptacao != null || p.TipoPessoa != null);
                    if (hasObjPreenchidos)
                    {
                        checklistsValidados.RemoveAll(r => r.IdCategoriaCliente == null && r.IdMeioCaptacao == null && r.TipoPessoa == null);
                    }
                    retorno.AddRange(checklistsValidados);
                }

            }
            catch (Exception)
            {
                retorno = new List<Quiz>();
            }

            return retorno;
        }

        /// <summary>
        ///     Retorna uma lista com as etapas da venda e seus dados para ser utilizada na view
        /// </summary>
        /// <param name="idVenda">ID da venda</param>
        public IEnumerable<EtapaVendaViewModel> GetDadosEtapasVenda(long idVenda)
        {
            var etapasVenda = new List<EtapaVendaViewModel>();
            var etapasEncontradas = idVenda != 0 ? this._repository.GetByVenda(idVenda) : new List<EtapaVenda>();

            if (idVenda == 0 || etapasEncontradas.Count() == 0)
            {
                IEnumerable<EnumEtapaVenda> etapasHabilitadas = this.GetEtapasVendaHabilitadas(idVenda);

                foreach (EnumEtapaVenda etapa in etapasHabilitadas)
                {
                    switch (etapa)
                    {
                        case EnumEtapaVenda.DadosGerais:
                            {
                                etapasVenda.Add(new EtapaVendaViewModel
                                {
                                    Etapa = (int)EnumEtapaVenda.DadosGerais,
                                    Descricao = "Dados Gerais",
                                    Status = (int)Sinapse.Model.enumerados.StatusEtapaVenda.Pendente
                                });

                                break;
                            }
                        case EnumEtapaVenda.ConsultaViabilidade:
                            {
                                etapasVenda.Add(new EtapaVendaViewModel
                                {
                                    Etapa = (int)EnumEtapaVenda.ConsultaViabilidade,
                                    Descricao = "Consulta de Viabilidade",
                                    Status = (int)StatusEtapaVenda.Pendente
                                });

                                break;
                            }
                        case EnumEtapaVenda.ConsultaPendenciaFinanceira:
                            {
                                etapasVenda.Add(new EtapaVendaViewModel
                                {
                                    Etapa = (int)EnumEtapaVenda.ConsultaPendenciaFinanceira,
                                    Descricao = "Consulta de Pendência Financeira",
                                    Status = (int)StatusEtapaVenda.Pendente
                                });

                                break;
                            }
                        case EnumEtapaVenda.ConsultaPendenciaEndereco:
                            {
                                etapasVenda.Add(new EtapaVendaViewModel
                                {
                                    Etapa = (int)EnumEtapaVenda.ConsultaPendenciaEndereco,
                                    Descricao = "Consulta de Pendência no Endereço",
                                    Status = (int)StatusEtapaVenda.Pendente
                                });

                                break;
                            }
                        case EnumEtapaVenda.ConsultaCredito:
                            {
                                etapasVenda.Add(new EtapaVendaViewModel
                                {
                                    Etapa = (int)EnumEtapaVenda.ConsultaCredito,
                                    Descricao = "Consulta de Crédito",
                                    Status = (int)StatusEtapaVenda.Pendente
                                });

                                break;
                            }
                        case EnumEtapaVenda.PlanoProduto:
                            {
                                etapasVenda.Add(new EtapaVendaViewModel
                                {
                                    Etapa = (int)EnumEtapaVenda.PlanoProduto,
                                    Descricao = "Planos/Produtos",
                                    Status = (int)StatusEtapaVenda.Pendente
                                });

                                break;
                            }
                        case EnumEtapaVenda.Financeiro:
                            {
                                etapasVenda.Add(new EtapaVendaViewModel
                                {
                                    Etapa = (int)EnumEtapaVenda.Financeiro,
                                    Descricao = "Financeiro",
                                    Status = (int)StatusEtapaVenda.Pendente
                                });

                                break;
                            }
                        case EnumEtapaVenda.Checklist:
                            {
                                etapasVenda.Add(new EtapaVendaViewModel
                                {
                                    Etapa = (int)EnumEtapaVenda.Checklist,
                                    Descricao = "Checklist",
                                    Status = (int)StatusEtapaVenda.Pendente
                                });

                                break;
                            }
                        //case EnumEtapaVenda.Resumo:
                        //{
                        //    etapasVenda.Add(new EtapaVendaViewModel
                        //    {
                        //        Etapa = (int)EnumEtapaVenda.Resumo,
                        //        Descricao = "Resumo",
                        //        Status = (int)StatusEtapaVenda.Pendente
                        //    });

                        //    break;
                        //}
                        case EnumEtapaVenda.TermoAdesao:
                            {
                                etapasVenda.Add(new EtapaVendaViewModel
                                {
                                    Etapa = (int)EnumEtapaVenda.TermoAdesao,
                                    Descricao = "Termo de Adesão",
                                    Status = (int)StatusEtapaVenda.Pendente
                                });

                                break;
                            }
                    }
                }
            }
            else
            {
                etapasVenda = etapasEncontradas
                        .Select(e => new EtapaVendaViewModel
                        {
                            Id = Convert.ToInt32(e.Id),
                            Etapa = (int)e.Etapa,
                            Descricao = e.Descricao,
                            Status = (int)e.Status,
                            MotivoAprovacaoManual = e.MotivoAprovManual
                        })
                        .OrderBy(e => e.Etapa)
                        .ToList();
            }

            return etapasVenda;
        }

        /// <summary>
        ///     Retorna uma lista com as etapas da venda e seus dados para ser utilizada na view
        /// </summary>
        /// <param name="idVenda">ID da venda</param>
        public IEnumerable<EtapaVendaViewModel> GetDadosEtapasTrocaTitularidade(long idTrocaTitularidade)
        {
            var etapasVenda = new List<EtapaVendaViewModel>();
            var etapasEncontradas = idTrocaTitularidade != 0 ? this._repository.GetByTrocaTitularidade(idTrocaTitularidade) : new List<EtapaVenda>();

            if (idTrocaTitularidade == 0 || etapasEncontradas.Count() == 0)
            {
                IEnumerable<EnumEtapaVenda> etapasHabilitadas = this.GetEtapasTrocasHabilitadas(idTrocaTitularidade);
                var consultaFinanceira = false;
                foreach (EnumEtapaVenda etapa in etapasHabilitadas)
                {
                    switch (etapa)
                    {
                        case EnumEtapaVenda.DadosGerais:
                            {
                                etapasVenda.Add(new EtapaVendaViewModel
                                {
                                    Etapa = (int)EnumEtapaVenda.DadosGerais,
                                    Descricao = "Dados Gerais",
                                    Status = (int)Sinapse.Model.enumerados.StatusEtapaVenda.Pendente
                                });

                                break;
                            }
                        case EnumEtapaVenda.ConsultaPendenciaFinanceira:
                            {
                                etapasVenda.Add(new EtapaVendaViewModel
                                {
                                    Etapa = (int)EnumEtapaVenda.ConsultaPendenciaFinanceira,
                                    Descricao = "Consulta de Pendência Financeira - Titular Atual",
                                    Status = (int)StatusEtapaVenda.Pendente
                                });

                                consultaFinanceira = true;
                                break;
                            }                 
                        case EnumEtapaVenda.ConsultaPendenciaEndereco:
                            {
                                etapasVenda.Add(new EtapaVendaViewModel
                                {
                                    Etapa = (int)EnumEtapaVenda.ConsultaPendenciaEndereco,
                                    Descricao = "Pendencia Endereço – Titular Atual",
                                    Status = (int)StatusEtapaVenda.Pendente
                                });

                                if (consultaFinanceira)
                                {
                                    etapasVenda.Add(new EtapaVendaViewModel
                                    {
                                        Etapa = (int)EnumEtapaVenda.ConsultaPendenciaFinanceiraNovo,
                                        Descricao = "Consulta de Pendência Financeira - Titular Novo",
                                        Status = (int)StatusEtapaVenda.Pendente
                                    });
                                }
                                break;
                       }

                        case EnumEtapaVenda.ConsultaCredito:
                            {
                                etapasVenda.Add(new EtapaVendaViewModel
                                {
                                    Etapa = (int)EnumEtapaVenda.ConsultaCredito,
                                    Descricao = "Consulta de Crédito - Titular Novo",
                                    Status = (int)StatusEtapaVenda.Pendente
                                });

                                break;
                            }
                        case EnumEtapaVenda.Checklist:
                            {
                                etapasVenda.Add(new EtapaVendaViewModel
                                {
                                    Etapa = (int)EnumEtapaVenda.Checklist,
                                    Descricao = "Checklist - Titular Novo",
                                    Status = (int)StatusEtapaVenda.Pendente
                                });

                                break;
                            }
                        case EnumEtapaVenda.TermoAdesao:
                            {
                                etapasVenda.Add(new EtapaVendaViewModel
                                {
                                    Etapa = (int)EnumEtapaVenda.TermoAdesao,
                                    Descricao = "Termo de Adesão - Titular Novo",
                                    Status = (int)StatusEtapaVenda.Pendente
                                });

                                break;
                            }
                    }
                }
            }
            else
            {
                etapasVenda = etapasEncontradas
                        .Select(e => new EtapaVendaViewModel
                        {
                            Id = Convert.ToInt32(e.Id),
                            Etapa = (int)e.Etapa,
                            Descricao = e.Descricao,
                            Status = (int)e.Status,
                            MotivoAprovacaoManual = e.MotivoAprovManual
                        })
                        .OrderBy(e => e.Id)
                        .ToList();
            }

            return etapasVenda;
        }

        /// <summary>
        ///     Adiciona a Etapa da Venda para o Cartão de Crédito
        /// </summary>
        /// <param name="idVenda">ID da venda</param>
        public void AdcEtapaCartao(string usuario, Venda venda)
        {
            var etapasEncontradas = this._repository.GetByVenda(venda.Id);

            etapasEncontradas.Select(e => 
                                    new EtapaVendaViewModel{Id = Convert.ToInt32(e.Id),
                                    Etapa = (int)e.Etapa,
                                    Descricao = e.Descricao,
                                    Status = (int)e.Status,
                                    MotivoAprovacaoManual = e.MotivoAprovManual})
                                    .OrderBy(e => e.Etapa)
                                    .ToList();

            if (!etapasEncontradas.Any(x => x.Etapa == EnumEtapaVenda.CartaoCredito)) {
                EtapaVenda etapaVenda = new EtapaVenda
                {
                    DataHoraCriacao = DateTime.Now,
                    UsuarioCriacao = usuario,

                    Etapa = EnumEtapaVenda.CartaoCredito,
                    Descricao = "Dados do Cartão",

                    Status = StatusEtapaVenda.Pendente,
                    Venda = venda,
                };

                this._repository.Save(etapaVenda);
            }         
        }

        public async Task RmvEtapaCartao(Venda venda)
        {
            var etapasEncontradas = this._repository.GetByVenda(venda.Id);

            etapasEncontradas.Select(e =>
                                    new EtapaVendaViewModel
                                    {
                                        Id = Convert.ToInt32(e.Id),
                                        Etapa = (int)e.Etapa,
                                        Descricao = e.Descricao,
                                        Status = (int)e.Status,
                                        MotivoAprovacaoManual = e.MotivoAprovManual
                                    })
                                    .OrderBy(e => e.Etapa)
                                    .ToList();

            if (etapasEncontradas.Any(x => x.Etapa == EnumEtapaVenda.CartaoCredito))
            {
                EtapaVenda etapaPendente = etapasEncontradas.Where(x => x.Etapa != EnumEtapaVenda.CartaoCredito &&
                                                            x.Status == StatusEtapaVenda.Pendente)
                                                            .ToList()
                                                            .OrderBy(x => x.Etapa)
                                                            .LastOrDefault();
                EtapaVenda lastEtapaAprovada = null;


                if (etapaPendente != null)
                    await this._repository.UpdateEtapaAtualVenda(etapaPendente, venda.Id);
                else
                {
                    lastEtapaAprovada = etapasEncontradas.Where(x => x.Etapa != EnumEtapaVenda.CartaoCredito &&
                                                            x.Status == StatusEtapaVenda.Aprovado)
                                                            .ToList()
                                                            .OrderBy(x => x.Etapa)
                                                            .LastOrDefault();

                    await this._repository.UpdateEtapaAtualVenda(lastEtapaAprovada, venda.Id);
                }

                var etapasCartao = etapasEncontradas.Where(x => x.Etapa == EnumEtapaVenda.CartaoCredito).ToList();

                await this._repository.RemoverEtapasVenda(etapasCartao);
            }
        }

        /// <summary>
        ///     Altera o status de uma etapa de uma venda específica
        /// </summary>
        /// <param name="idVenda">Id da venda que será atualizada</param>
        /// <param name="etapa">Etapa da venda que terá o status alterado</param>
        /// <param name="status">Novo status da etapa informada</param>
        public void MudarStatusEtapa(long? idVenda, EnumEtapaVenda etapa, 
                                     StatusEtapaVenda status, 
                                     DateTime? dateHoraAlteracao = null, 
                                     string usuario = null, 
                                     long? idTrocaTitular = null)
        {

            EtapaVenda etapaVenda = this._repository.GetAll().Where(e => e.Venda.Id == idVenda && e.Etapa == etapa && idTrocaTitular == null ||
                                                                    e.TrocaTitularidade != null && e.TrocaTitularidade.Id == idTrocaTitular && e.Etapa == etapa)
                    .FirstOrDefault();
           
            if (etapaVenda == null)
            {
                throw new Exception("Etapa não encontrada na venda");
            }
          
            etapaVenda.Status = status;

            if(dateHoraAlteracao.HasValue)
                etapaVenda.DataHoraAlteracao = dateHoraAlteracao;

            if(!String.IsNullOrEmpty(usuario))
                etapaVenda.UsuarioAlteracao = usuario;

            this._repository.Update(etapaVenda);
        }

        public void MudarStatusEtapaManual(long? idVenda, EnumEtapaVenda etapa, Sinapse.Model.enumerados.StatusEtapaVenda status,String motivo, long? idTrocaTitular = null)
        {
            EtapaVenda etapaVenda =
                this._repository
                    .GetAll()
                    .Where(e => e.Venda.Id == idVenda &&
                                e.Etapa == etapa || 
                                e.TrocaTitularidade.Id == idTrocaTitular &&
                                e.Etapa == etapa)
                    .FirstOrDefault();
            if (motivo == null)
            {
                throw new Exception("Motivo de Aprovação Manual não encontrada na venda");
            }

            if (etapaVenda == null)
            {
                throw new Exception("Etapa não encontrada na venda");
            }
            etapaVenda.MotivoAprovManual = motivo;
            etapaVenda.Status = status;

            this._repository.Update(etapaVenda);
        }

        public void Save(EtapaVenda etapa)
        {
            this._repository.Save(etapa);
        }

        /// <summary>
        /// Verifica se as etapas da venda já foram gravadas
        /// </summary>
        /// <param name="idVenda">ID da venda</param>
        public bool ExistemEtapas(long idVenda)
        {
            IEnumerable<EtapaVenda> etapasEncontradas = this._repository.GetByVenda(idVenda);
            return etapasEncontradas != null && etapasEncontradas.Count() > 0;
        }

        /// <summary>
        /// Verifica se as etapas da troca de Titularidade já foram gravadas
        /// </summary>
        /// <param name="idVenda">ID da venda</param>
        public bool ExistemEtapasTroca(long idTrocaTitularidade)
        {
            IEnumerable<EtapaVenda> etapasEncontradas = this._repository.GetByTrocaTitularidade(idTrocaTitularidade);
            return etapasEncontradas != null && etapasEncontradas.Count() > 0;
        }

        /// <summary>
        ///     ValidaRestricaoEtapa
        /// </summary>
        /// <param name="idVenda">ID da Venda</param>

        public bool EtapaRestringeVenda(Venda venda, bool exigeTermoAssinado)
        {
            var etapasEncontradas = venda.Id != 0 ? this._repository.GetByVenda(venda.Id) : new List<EtapaVenda>();

            if (etapasEncontradas.Any(p => p.Status != StatusEtapaVenda.Aprovado && p.Status != StatusEtapaVenda.AprovadoManualmente)) 
            {
                var todasEtapasPendentes = etapasEncontradas.Where(x => x.Status == StatusEtapaVenda.Pendente).ToList();

                if (todasEtapasPendentes.Count() == 1 && todasEtapasPendentes.All(x => x.Etapa == EnumEtapaVenda.TermoAdesao))
                    return exigeTermoAssinado ? true : false;
                else
                return true;
            }
            return false;
        }

        public EtapaVenda VerificaUltimaEtapa(long idVenda)
        {
            var etapasVenda = this._repository.GetByVenda(idVenda);

            EtapaVenda ultimaEtapaPendente = etapasVenda.FirstOrDefault(e => e.Status == StatusEtapaVenda.Reprovado || e.Status == StatusEtapaVenda.Pendente);
            var descEtapa = ultimaEtapaPendente ?? etapasVenda.LastOrDefault();

            return descEtapa;

        }
        
        public EtapaVenda VerificaUltimaEtapaTroca(long troca)
        {
            var etapasTroca = this._repository.GetByTrocaTitularidade(troca);

            EtapaVenda ultimaEtapaPendente = etapasTroca.FirstOrDefault(e => e.Status == StatusEtapaVenda.Reprovado || e.Status == StatusEtapaVenda.Pendente);
            var descEtapa = ultimaEtapaPendente ?? etapasTroca.LastOrDefault();

            return descEtapa;
    }
        
        public GetApiRetorno AdcEtapaOS(Venda venda, string usuario) 
        {
            var retorno = new GetApiRetorno();
            try 
            {
                retorno.Ok = true;
                if (this._repository.ConsultaGeraEtapaOS(venda.Id).Result) 
                {
                    EtapaVenda etapaOS = new EtapaVenda
                    {
                        DataHoraCriacao = DateTime.Now,
                        UsuarioCriacao = usuario,
                        Etapa = EnumEtapaVenda.OrdemServico,

                        Descricao = HtmlEnumExtensions.GetEnumDescription(EnumEtapaVenda.OrdemServico),
                        Status = StatusEtapaVenda.Pendente,
                        Venda = venda,
                    };
                    this._repository.Save(etapaOS);                    
}
                retorno.Ok = true;
            }
            catch (Exception ex) 
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
            }
            return retorno;
        }
    }
}
