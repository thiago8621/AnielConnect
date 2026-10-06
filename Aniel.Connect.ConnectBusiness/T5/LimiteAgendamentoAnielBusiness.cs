using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.T5.LimiteAgendamento;
using Aniel.Connect.Repository.T5.Natureza;
using Aniel.T5.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.ConnectBusiness.T5
{
    public class LimiteAgendamentoAnielBusiness
    {
        private readonly ILimiteAgendamentoAnielRepository _repoLimAgend;
        private readonly INaturezaAnielRepository _repoNatureza;

        public LimiteAgendamentoAnielBusiness(ILimiteAgendamentoAnielRepository repoLimAgend, INaturezaAnielRepository repoNatureza)
        {
            this._repoLimAgend = repoLimAgend;
            this._repoNatureza = repoNatureza;
        }

        public LimiteAgendamentoListResponse GetAll(LimiteAgendamentoDataTableRequest filtros)
        {
            return this._repoLimAgend.GetListagem(filtros);
        }

        /// <summary>
        ///     Retorna todas as naturezas com percentuais em branco para serem configuradas no 
        ///     cadastro de limite de agendamento para um novo período
        /// </summary>
        public IEnumerable<LimiteAgendamentoPercentualNatureza> GetNaturezas()
        {
            IQueryable<Natureza_ProducaoAniel> naturezas =
                this._repoNatureza
                    .GetAll();

            return naturezas.Select(n => new LimiteAgendamentoPercentualNatureza
            {
                IdNatureza = n.Id,
                Natureza = n.Descricao,
                Percentual = String.Empty
            }).ToArray();
        }

        /// <summary>
        ///     Retorna todas as naturezas com os seus percentuais registrados na área informada para uma determinada data
        ///     para a utilização na edição de uma data no cadastro de limite de agendamento.
        /// </summary>
        /// <param name="codEscritorio">Código do grupo área</param>
        /// <param name="codArea">Código da área</param>
        /// <param name="data">Data para qual o limite será consultado</param>
        public IEnumerable<LimiteAgendamentoPercentualNatureza> GetPercentuaisNaturezas(
            string codEscritorio,
            string codArea,
            DateTime data
        )
        {
            var percentuais = new List<LimiteAgendamentoPercentualNatureza>();

            // Dicionário com o ID da natureza como chave e a descrição como valor
            Dictionary<int, string> naturezas =
                this._repoNatureza
                    .GetAll()
                    .ToDictionary(n => n.Id, n => n.Descricao);

            foreach (KeyValuePair<int, string> natureza in naturezas)
            {
                var percNatureza = new LimiteAgendamentoPercentualNatureza();

                percNatureza.IdNatureza = natureza.Key;
                percNatureza.Natureza = natureza.Value;

                int? perc =
                    this._repoLimAgend
                        .GetPercNaturezaByAreaData(
                            codEscritorio,
                            codArea,
                            percNatureza.IdNatureza,
                            data
                        )?.Percentual;

                percNatureza.Percentual = perc.HasValue ? String.Format("{0}%", perc.Value) : String.Empty;

                percentuais.Add(percNatureza);
            }

            return percentuais;
        }

        /// <summary>
        ///     Retorna os percentuais configurados para uma natureza em uma área na data específicada
        /// </summary>
        /// <param name="codEscritorio">Grupo Área</param>
        /// <param name="codArea">Área</param>
        /// <param name="idNatureza">Natureza</param>
        /// <param name="data">Data do percentual</param>
        public LimiteAgendamentoAniel GetPercNaturezaByAreaData(
            string codEscritorio,
            string codArea,
            int idNatureza,
            DateTime data
        )
        {
            return this._repoLimAgend.GetPercNaturezaByAreaData(codEscritorio, codArea, idNatureza, data);
        }

        /// <summary>
        ///     Retorna os percentuais configurados para uma natureza em uma área na data específicada
        /// </summary>
        /// <param name="codEscritorio">Grupo Área</param>
        /// <param name="codArea">Área</param>
        /// <param name="idNatureza">Natureza</param>
        public List<LimiteAgendamentoAniel> GetPercNaturezaByArea(
            string codEscritorio,
            string codArea,
            int idNatureza
        )
        {
            return this._repoLimAgend.GetPercNaturezaByArea(codEscritorio, codArea, idNatureza);
        }

        /// <summary>
        ///     Grava as configurações do limite de agendamento
        /// </summary>
        /// <param name="view">Objeto com as informações vindas do cadastro de limite de agendamento</param>
        /// <param name="usuario">Usuário responsável pela gravação</param>
        public void Gravar(LimiteAgendamentoModel view, string usuario)
        {
            string grupoArea = view.CodEscritorio;
            string area = view.CodArea;

            DateTime? data = !String.IsNullOrEmpty(view.Data) ? Convert.ToDateTime(view.Data) : (DateTime?)null;

            DateTime? dataInicio = !String.IsNullOrEmpty(view.DataInicio) ? Convert.ToDateTime(view.DataInicio) : (DateTime?)null;
            DateTime? dataFim = !String.IsNullOrEmpty(view.DataFim) ? Convert.ToDateTime(view.DataFim) : (DateTime?)null;

            foreach (LimiteAgendamentoPercentualNatureza natureza in view.PercentualNatureza)
            {
                if (!String.IsNullOrEmpty(natureza.Percentual))
                {
                    // Caso o percentual tenha sido preenchido significa que o usuário alterou a configuração para
                    // a natureza atual e devemos inserir essa configuração no banco

                    if (dataInicio.HasValue && dataFim.HasValue)
                    {
                        // Caso os campos DataInicio e DataFim estejam preenchido estamos lidando
                        // com um lançamento de novos percentuais para o período informado

                        IEnumerable<LimiteAgendamentoAniel> limitesAntigos =
                            this._repoLimAgend.GetAll()
                                              .Where(p => p.CodEscritorio == grupoArea &&
                                                          p.CodArea == area &&
                                                          p.IdNatureza == natureza.IdNatureza &&
                                                          (p.DataInicio <= dataFim.Value && dataInicio.Value <= p.DataFim))
                                              .ToArray();

                        int perc = Convert.ToInt32(natureza.Percentual.Replace("%", String.Empty));
                        bool percAlterado = limitesAntigos.Any(l => l.Percentual != perc);

                        if (percAlterado)
                        {
                            // Se existe alguma configuração anterior dentro do novo período informado
                            // que define um percentual diferente para essa determinada natureza é
                            // preciso remanejar os limites antigos para poder encaixar o novo período

                            foreach (LimiteAgendamentoAniel limiteAntigo in limitesAntigos)
                            {
                                if (dataInicio.Value <= limiteAntigo.DataInicio && limiteAntigo.DataFim <= dataFim.Value)
                                {
                                    // Caso o limite antigo esteja contido dentro do novo período configurado
                                    // devemos apagar o limite antigo, pois a nova configuração irá sobrescrever o antigo
                                    // limite configurado para o período
                                    this._repoLimAgend.Remove(limiteAntigo);
                                }
                                else if (dataInicio.Value <= limiteAntigo.DataInicio && limiteAntigo.DataInicio <= dataFim.Value)
                                {
                                    // Caso o limite antigo comece dentro e termine após o novo período configurado 
                                    // devemos atualizar o registro de limite antigo para iniciar o seu período
                                    // um dia após da data do fim do novo período que será inserido

                                    limiteAntigo.DataHoraAlteracao = DateTime.Now;
                                    limiteAntigo.UsuarioAlteracao = usuario;

                                    limiteAntigo.DataInicio = dataFim.Value.AddDays(1);
                                    this._repoLimAgend.Update(limiteAntigo);
                                }
                                else if (dataInicio.Value <= limiteAntigo.DataFim && limiteAntigo.DataFim <= dataFim.Value)
                                {
                                    // Caso o limite antigo comece antes mas termine dentro do novo período configurado
                                    // devemos atualizar o registro de limite antigo limitando o seu período até
                                    // um dia antes da data de início do novo período

                                    limiteAntigo.DataHoraAlteracao = DateTime.Now;
                                    limiteAntigo.UsuarioAlteracao = usuario;

                                    limiteAntigo.DataFim = dataInicio.Value.AddDays(-1);
                                    this._repoLimAgend.Update(limiteAntigo);
                                }
                                else if (limiteAntigo.DataInicio <= dataInicio.Value && dataFim.Value <= limiteAntigo.DataFim)
                                {
                                    // Caso o novo período configurado esteja contido dentro do limite antigo devemos
                                    // dividir o limite antigo em um limite que vai da data inicial do limite antigo até um
                                    // dia antes do novo limite e outra parte que vai de um dia após o fim do novo limite até
                                    // a data final do limite antigo

                                    DateTime dataInicioLimAntigo = limiteAntigo.DataInicio;
                                    DateTime dataFimLimAntigo = limiteAntigo.DataFim;

                                    LimiteAgendamentoAniel limiteAntigoAnterior = limiteAntigo;

                                    limiteAntigoAnterior.DataHoraAlteracao = DateTime.Now;
                                    limiteAntigoAnterior.UsuarioAlteracao = usuario;

                                    limiteAntigoAnterior.DataInicio = dataInicioLimAntigo;
                                    limiteAntigoAnterior.DataFim = dataInicio.Value.AddDays(-1);

                                    this._repoLimAgend.Update(limiteAntigoAnterior);

                                    var limiteAntigoPosterior = LimiteAgendamentoAniel.CreateFrom(limiteAntigo);

                                    limiteAntigoPosterior.DataHoraCriacao = DateTime.Now;
                                    limiteAntigoPosterior.UsuarioCriacao = usuario;

                                    limiteAntigoPosterior.DataInicio = dataFim.Value.AddDays(1);
                                    limiteAntigoPosterior.DataFim = dataFimLimAntigo;

                                    this._repoLimAgend.Save(limiteAntigoPosterior);
                                }
                            }

                            var limite = new LimiteAgendamentoAniel
                            {
                                DataHoraCriacao = DateTime.Now,
                                UsuarioCriacao = usuario,

                                CodEscritorio = grupoArea,
                                CodArea = area,
                                IdNatureza = natureza.IdNatureza,

                                Percentual = perc,

                                DataInicio = dataInicio.Value,
                                DataFim = dataFim.Value
                            };

                            this._repoLimAgend.Save(limite);
                        }
                        else if (limitesAntigos.Count() > 0)
                        {
                            // Caso existam configurações antigas para a natureza para datas dentro do novo período
                            // e essas configurações tenham o mesmo percentual definido na nova configuração devemos
                            // verificar se existem datas dentro do novo período configurado que não são cobertas
                            // pelas antigas configurações de limite

                            DateTime primeiraDataLimitesAntigos =
                                limitesAntigos
                                    .OrderBy(l => l.DataInicio)
                                    .Select(l => l.DataInicio)
                                    .First();

                            if (primeiraDataLimitesAntigos > dataInicio.Value)
                            {
                                var limite = new LimiteAgendamentoAniel
                                {
                                    DataHoraCriacao = DateTime.Now,
                                    UsuarioCriacao = usuario,

                                    CodEscritorio = grupoArea,
                                    CodArea = area,
                                    IdNatureza = natureza.IdNatureza,

                                    Percentual = perc,

                                    DataInicio = dataInicio.Value,
                                    DataFim = primeiraDataLimitesAntigos.AddDays(-1)
                                };

                                this._repoLimAgend.Save(limite);
                            }

                            DateTime ultimaDataLimitesAntigos =
                                limitesAntigos
                                    .OrderByDescending(l => l.DataFim)
                                    .Select(l => l.DataFim)
                                    .First();

                            if (ultimaDataLimitesAntigos < dataFim.Value)
                            {
                                var limite = new LimiteAgendamentoAniel
                                {
                                    DataHoraCriacao = DateTime.Now,
                                    UsuarioCriacao = usuario,

                                    CodEscritorio = grupoArea,
                                    CodArea = area,
                                    IdNatureza = natureza.IdNatureza,

                                    Percentual = perc,

                                    DataInicio = ultimaDataLimitesAntigos.AddDays(1),
                                    DataFim = dataFim.Value
                                };

                                this._repoLimAgend.Save(limite);
                            }
                        }
                        else
                        {
                            // Caso não haja nenhuma configuração antiga podemos simplesmente gravar a nova configuração

                            var limite = new LimiteAgendamentoAniel
                            {
                                DataHoraCriacao = DateTime.Now,
                                UsuarioCriacao = usuario,

                                CodEscritorio = grupoArea,
                                CodArea = area,
                                IdNatureza = natureza.IdNatureza,

                                Percentual = perc,

                                DataInicio = dataInicio.Value,
                                DataFim = dataFim.Value
                            };

                            this._repoLimAgend.Save(limite);
                        }
                    }
                    else if (data.HasValue)
                    {
                        // Caso o campo Data esteja preenchido estamos lidando com uma atualização dos percentuais
                        // das naturezas para uma data específica

                        IEnumerable<LimiteAgendamentoAniel> limitesAntigos =
                            this._repoLimAgend.GetAll()
                                              .Where(p => p.CodEscritorio == grupoArea &&
                                                          p.CodArea == area &&
                                                          p.IdNatureza == natureza.IdNatureza &&
                                                          (p.DataInicio <= data.Value && data.Value <= p.DataFim))
                                              .ToArray();

                        int perc = Convert.ToInt32(natureza.Percentual.Replace("%", String.Empty));
                        bool percAlterado = limitesAntigos.Any(l => l.Percentual != perc);

                        if (percAlterado)
                        {
                            // Precisamos remanejar os limites das configurações antigas
                            // para poder encaixar a nova configuração para a data

                            foreach (LimiteAgendamentoAniel limiteAntigo in limitesAntigos)
                            {
                                limiteAntigo.DataHoraAlteracao = DateTime.Now;
                                limiteAntigo.UsuarioAlteracao = usuario;

                                if (limiteAntigo.DataInicio == data.Value)
                                {
                                    // Se a data em que vamos alterar o percentual é a mesma data do início do período
                                    // configurado anteriormente devemos atualizar a configuração anterior para começar
                                    // um dia depois
                                    limiteAntigo.DataInicio = limiteAntigo.DataInicio.AddDays(1);
                                    this._repoLimAgend.Update(limiteAntigo);
                                }
                                else if (limiteAntigo.DataFim == data.Value)
                                {
                                    // Se a data em que vamos alterar o percentual é a mesma data do final do período
                                    // configurado anteriormente devemos atualizar a configuração anterior para terminar
                                    // um dia antes
                                    limiteAntigo.DataFim = limiteAntigo.DataFim.AddDays(-1);
                                    this._repoLimAgend.Update(limiteAntigo);
                                }
                                else
                                {
                                    // Se a data em que vamos alterar o percentual não for igual a data inicial e nem igual
                                    // a data final então essa data corta o período da configuração antiga ao meio.

                                    // Devemos dividir a antiga configuração em um período que vai da data inicial do
                                    // período original até um dia antes da data que será alterada e outro período
                                    // que vai de um dia após a data que será alterada até a data final do período original.

                                    DateTime dataInicioLimAntigo = limiteAntigo.DataInicio;
                                    DateTime dataFimLimAntigo = limiteAntigo.DataFim;

                                    LimiteAgendamentoAniel limiteAntigoAnterior = limiteAntigo;

                                    limiteAntigoAnterior.DataHoraAlteracao = DateTime.Now;
                                    limiteAntigoAnterior.UsuarioAlteracao = usuario;

                                    limiteAntigoAnterior.DataInicio = dataInicioLimAntigo;
                                    limiteAntigoAnterior.DataFim = data.Value.AddDays(-1);

                                    this._repoLimAgend.Update(limiteAntigoAnterior);

                                    var limiteAntigoPosterior = LimiteAgendamentoAniel.CreateFrom(limiteAntigo);

                                    limiteAntigoPosterior.DataHoraCriacao = DateTime.Now;
                                    limiteAntigoPosterior.UsuarioCriacao = usuario;

                                    limiteAntigoPosterior.DataInicio = data.Value.AddDays(1);
                                    limiteAntigoPosterior.DataFim = dataFimLimAntigo;

                                    this._repoLimAgend.Save(limiteAntigoPosterior);
                                }
                            }

                            var limite = new LimiteAgendamentoAniel
                            {
                                DataHoraCriacao = DateTime.Now,
                                UsuarioCriacao = usuario,

                                CodEscritorio = grupoArea,
                                CodArea = area,
                                IdNatureza = natureza.IdNatureza,

                                Percentual = perc,

                                DataInicio = data.Value,
                                DataFim = data.Value
                            };

                            this._repoLimAgend.Save(limite);
                        }
                    }
                }
            }
        }
    }
}
