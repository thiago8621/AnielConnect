using Aniel.Connect.ConnectBusiness.T5.Consulta_de_Viabilidade.Interface;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Models;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.ConsultaViabilidade;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.MotivoInviabilidade;
using Aniel.Connect.Repository.T5.AreaCoberturaMongo;
using Aniel.Connect.Repository.T5.ElementoRede;
using Aniel.Connect.Repository.T5.ElementoRedePorta;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.ConnectBusiness.T5.Consulta_de_Viabilidade
{
    public class ConsultaViabilidadeBusiness
    {
        private readonly IConsultaViabilidadeRepository _repository;

        private readonly IElementoRedeAnielRepository _repoElemRede;
        private readonly IElementoRedePortaAnielRepository _repoPorta;

        private readonly IConfiguracoesGeraisRepository _repoConfig;

        //Específico para verificação do motivo de inviabilidade
        private readonly IMotivoInviabilidadeRepository _repoMotivo;
        private readonly IAreaCoberturaMongoRepository _repoAreaCobertura;

        public ConsultaViabilidadeBusiness(
            IConsultaViabilidadeRepository repository,
            IElementoRedeAnielRepository repoElemRede,
            IElementoRedePortaAnielRepository repoPorta,
            IConfiguracoesGeraisRepository repoConfig
        )
        {
            this._repository = repository;
            this._repoElemRede = repoElemRede;

            this._repoPorta = repoPorta;
            this._repoConfig = repoConfig;
        }

        public ConsultaViabilidadeBusiness(
            IConsultaViabilidadeRepository repository,
            IElementoRedeAnielRepository repoElemRede,
            IElementoRedePortaAnielRepository repoPorta,
            IConfiguracoesGeraisRepository repoConfig,
            IMotivoInviabilidadeRepository repoMotivoInviabilidade,
            IAreaCoberturaMongoRepository repoAreaCobertura
        )
        {
            this._repository = repository;
            this._repoElemRede = repoElemRede;

            this._repoPorta = repoPorta;
            this._repoConfig = repoConfig;

            this._repoMotivo = repoMotivoInviabilidade;
            this._repoAreaCobertura = repoAreaCobertura;
        }

        public ConsultaViabilidade GetConsultaViabilidadeById(long id)
        {
            return this._repository.GetById(id);
        }

        /// <summary>
        ///     Retorna uma lista das caixas disponíveis para determinado local, de acordo com a consulta de viabilidade
        /// </summary>
        /// <param name="lat">Latitude do local em que foi verificado a viabilidade</param>
        /// <param name="lon">Longitude do local em que foi verificado a viabilidade</param>
        public (bool temValidade, IEnumerable<Consulta_ViabilidadeCaixa>caixas) GetCaixasByLatLong(double lat, double lon)
        {
            IEnumerable<Consulta_ViabilidadeCaixa> caixas = new List<Consulta_ViabilidadeCaixa>();
            var retornoCaixasConsulta = new List<ConsultaViabilidadeDados>();
            bool consultaDentroValidade = false;

            Configuracoes_Gerais config = this._repoConfig.GetConfiguracoesGerais();
            ConsultaViabilidade consultaViabilidade = this._repository.GetConsultaViabilidadeByLatLong(lat, lon);          

            if (config.ConsultaViabilidadeTempoValidade.HasValue && consultaViabilidade != null)
            {
                DateTime dataMaxValidadeConsulta = consultaViabilidade.DataHoraCriacao.Value.Date.AddDays(config.ConsultaViabilidadeTempoValidade.Value);
                consultaDentroValidade = DateTime.Now.Date <= dataMaxValidadeConsulta.Date;
            }
            if (consultaViabilidade != null) 
            {
                if(consultaDentroValidade == false && config.ConsultaViabilidadeTempoValidade.HasValue)
                {
                    return (consultaDentroValidade, caixas);
                }

                if (consultaViabilidade.Etapas.Count() == 0)
                {
                    retornoCaixasConsulta.AddRange(consultaViabilidade.DadosConsulta);
                }
                else 
                {
                    foreach (var etapa in consultaViabilidade.Etapas)
                    {
                        retornoCaixasConsulta.AddRange(etapa.DadosConsulta);
                    }
                }
                if (retornoCaixasConsulta.Count > 0) 
                {
                    caixas =
                    retornoCaixasConsulta.Select(c => new Consulta_ViabilidadeCaixa
                    {
                        Id = c.Caixa.Id,
                        Descricao = c.Caixa.Descricao,
                        Latitude = c.Caixa.Latitude,
                        Longitude = c.Caixa.Longitude,
                        // Esse ToList() é necessário para evitar erro de serialização ao retornar esse objeto pro uma API
                        PortasDisponiveis = c.PortasDisponiveis.Where(p => p.Porta != null)
                                                                    .Select(p => p.Porta.Porta)
                                                                    .ToList(),
                        reservada = c.PortaReservada,
                        DistanciaKm = c.Distancia,
                        Capacidade = c.Caixa.Capacidade,
                        path = c.Caixa.Tipo?.Icone?.Caminho,
                        cor = c.Caixa.Tipo?.Cor
                    }).ToList();
                }               
            }
            return (consultaDentroValidade, caixas);
        }

        public IEnumerable<MotivoInviabilidadeConsulta> GetMotivosInviabilidadeByLatLong(double lat, double lon)
        {
            IEnumerable<MotivoInviabilidadeConsulta> motivos = new List<MotivoInviabilidadeConsulta>();

            Configuracoes_Gerais config = this._repoConfig.GetConfiguracoesGerais();
            ConsultaViabilidade consultaViabilidade = this._repository.GetConsultaViabilidadeByLatLong(lat, lon);

            if (consultaViabilidade == null)
            {
                return motivos;
            }

            if (config.ConsultaViabilidadeTempoValidade.HasValue)
            {
                DateTime dataMaxValidadeConsulta = consultaViabilidade.DataHoraCriacao.Value.Date.AddDays(config.ConsultaViabilidadeTempoValidade.Value);
                bool consultaDentroValidade = DateTime.Now.Date <= dataMaxValidadeConsulta.Date;

                if (!consultaDentroValidade)
                {
                    return motivos;
                }
            }

            motivos =
                consultaViabilidade
                    .Motivos
                    .Select(m => new MotivoInviabilidadeConsulta
                    {
                        Id = m.Id,
                        Motivo = m.Motivo,

                    }).ToList();

            return motivos;
        }

        /// <summary>
        ///     Retorna o status que indica se há uma consulta de viabilidade válida 
        ///     registrada no sistema para as coordenadas informadas
        /// </summary>
        /// <param name="lat">Latitude do local da consulta de viablidade</param>
        /// <param name="lon">Longitude do local da consulta de viablidade</param>
        public Sinapse.Model.enumerados.StatusEtapaVenda GetStatusConsultaViabilidade(double lat, double lon)
        {
            bool hasConsultaViabilidade = this._repository.GetConsultaViabilidadeByLatLong(lat, lon) != null;
            var status = StatusEtapaVenda.Pendente;
            if (hasConsultaViabilidade)
            {
                (bool dentroValidade, IEnumerable<Consulta_ViabilidadeCaixa> caixas) viabilidade = this.GetCaixasByLatLong(lat, lon);

                if (viabilidade.dentroValidade && viabilidade.caixas.Count() > 0)
                {
                    status = StatusEtapaVenda.Aprovado;
                }
                else if (viabilidade.dentroValidade && viabilidade.caixas.Count() == 0)
                {
                    Configuracoes_Gerais config = this._repoConfig.GetConfiguracoesGerais();

                    if (config.ConsultaViabilidadeRestringeVenda)
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

        public ConsultaViabilidade GetConsultaViabilidadeByLatLong(double lat, double lon)
        {
            ConsultaViabilidade consultaViabilidade = this._repository.GetConsultaViabilidadeByLatLong(lat, lon);

            if (consultaViabilidade != null)
            {
                Configuracoes_Gerais config = this._repoConfig.GetConfiguracoesGerais();

                if (config.ConsultaViabilidadeTempoValidade.HasValue)
                {
                    DateTime dataMaxValidadeConsulta = consultaViabilidade.DataHoraCriacao.Value.Date.AddDays(config.ConsultaViabilidadeTempoValidade.Value);
                    bool consultaDentroValidade = DateTime.Now.Date <= dataMaxValidadeConsulta.Date;
                    if (!consultaDentroValidade)
                    {
                        return consultaViabilidade;
                    }
                }
            }

            return consultaViabilidade;
        }

        /// <summary>
        ///     Registra os dados de uma consulta de viabilidade no banco do T5 e retorna os dados gravados.
        /// </summary>
        /// <param name="lat">Latitude do local em que foi verificado a viabilidade</param>
        /// <param name="lon">Longitude do local em que foi verificado a viabilidade</param>
        /// <param name="caixas">
        ///     Dados das caixas encontradas para a latitude e longitue informada na consulta de viabilidade
        /// </param>
        /// <param> Motivos para reprovação da consulta de viabilidade</param>
        public ConsultaViabilidade GravarConsultaViabilidade(
            double lat,
            double lon,
            IEnumerable<Consulta_ViabilidadeCaixa> caixas,
            IEnumerable<MotivoInviabilidadeConsulta> motivos,
            Configuracoes_Gerais config
        )
        {
            var listagemEtapas = new List<ConsultaViabilidadeEtapa>();
            var consultaViabilidade = new ConsultaViabilidade
            {
                DataHoraCriacao = DateTime.Now,
                Latitude = Convert.ToDouble(string.Format("{0:F7}", lat)),
                Longitude = Convert.ToDouble(string.Format("{0:F7}", lon))
            };

            foreach (EtapasValidacaoConsultaViabilidadeEnum enumEtapa in Enum.GetValues(typeof(EtapasValidacaoConsultaViabilidadeEnum)))
            {
                var agrupamentoCaixa = caixas.Where(p => p.EtapaValidacao == enumEtapa).ToList();
                var etapa = new ConsultaViabilidadeEtapa
                {
                    Consulta = consultaViabilidade,
                    DataHoraCriacao = DateTime.Now,
                    TipoEtapa = enumEtapa,
                };

                foreach (var motivo in motivos.Where(p => p.EtapaValidacao == enumEtapa))
                {
                    //TODO gravação de motivos de Inviabilidade
                    etapa.Motivos.Add(new ConsultaViabilidadeMotivo
                    {
                        Consulta = consultaViabilidade,
                        IdMotivo = motivo.Id.Value,
                        Motivo = motivo.Motivo,
                        Etapa = etapa,
                        DataHoraCriacao = DateTime.Now,
                    });
                }
                foreach (Consulta_ViabilidadeCaixa caixaConsulta in agrupamentoCaixa.OrderBy(p => p.DistanciaKm).ToList())
                {
                    Elemento_Rede caixa = this._repoElemRede.GetById(caixaConsulta.Id.Value);

                    var dadosConsultaViabilidade = new ConsultaViabilidadeDados
                    {
                        DataHoraCriacao = DateTime.Now,
                        Consulta = consultaViabilidade,
                        Caixa = caixa,

                        Distancia = caixaConsulta.DistanciaKm.GetValueOrDefault(0),
                        PortaReservada = caixaConsulta.reservada,
                    };

                    foreach (int porta in caixaConsulta.PortasDisponiveis)
                    {
                        Elemento_Rede_Porta portaDisponivel = this._repoPorta.GetByNumeroPorta(caixaConsulta.Id.Value,porta);

                        dadosConsultaViabilidade
                            .PortasDisponiveis
                            .Add(new ConsultaViabilidadePortaDisponivel
                            {
                                DataHoraCriacao = DateTime.Now,
                                Consulta = dadosConsultaViabilidade,
                                Porta = portaDisponivel
                            });
                    }

                    etapa.DadosConsulta.Add(dadosConsultaViabilidade);
                }
                //Adição das etapas na listagem para gravação posterior
                listagemEtapas.Add(etapa);
            }

            foreach (var etapa in listagemEtapas)
            {
                consultaViabilidade.Etapas.Add(etapa);
            }
            consultaViabilidade.Status = ValidaStatusConsultaViabilidade(consultaViabilidade, config);

            this._repository.Save(consultaViabilidade);
            return consultaViabilidade;
        }

        private StatusConsultaViabilidade ValidaStatusConsultaViabilidade(ConsultaViabilidade consulta, Configuracoes_Gerais config)
        {
            var status = StatusConsultaViabilidade.Aprovado;
            var etapas = consulta.Etapas;
            var reprovacao = false;
            var ressalva = false;

            List<bool> definicoes = new List<bool> { config.Requisito_Dentro_Raio, config.Requisito_Vinculo_Endereco, config.Requisito_Dentro_Area_Cobertura };

            var etapasEnum = Enum.GetValues(typeof(EtapasValidacaoConsultaViabilidadeEnum))
                                 .Cast<EtapasValidacaoConsultaViabilidadeEnum>();

            Dictionary<EtapasValidacaoConsultaViabilidadeEnum, bool> dicionario = etapasEnum.Zip(definicoes, (vEnum, vDef) => new { etapa = vEnum, requisitado = vDef })
                                       .ToDictionary(p => p.etapa, p => p.requisitado);


            foreach (var item in consulta.Etapas)
            {
                try
                {
                    item.GerouStatus = dicionario[item.TipoEtapa];
                }
                catch
                {
                    item.GerouStatus = false;
                }
            }

            //Se não definimos meio de viabilizar a consulta, ela é automaticamente reprovada.
            if (dicionario.All(p => p.Value == false))
            {
                reprovacao = true;
            }
            else
            {
                //Se todas as etapas cujo valor da configuracao seja verdadeira tiverem 1/mais motivos de reprovação, ele está reprovado
                reprovacao = dicionario.Any(p => p.Value == true && consulta.Etapas.Where(etapa => etapa.TipoEtapa == p.Key).All(etapa => etapa.Motivos.Count > 0));
                //Porém se alguma configuração for atendida, ele não é reprovado, apenas sendo aprovado com ressalva
                ressalva = dicionario.Any(p => p.Value == true && consulta.Etapas.Where(etapa => etapa.TipoEtapa == p.Key).Any(etapa => etapa.Motivos.Count > 0));
            }

            if (reprovacao)
            {
                status = StatusConsultaViabilidade.Reprovado;
            }
            else if (ressalva)
            {
                status = StatusConsultaViabilidade.Aprovado_Ressalva;
            }


            return status;
        }

        public List<MotivoInviabilidadeConsulta> VerificarMotivosInviabilidade(IEnumerable<Consulta_ViabilidadeCaixa> caixasConsultadas, double latitude, double longitude, bool aprovadoCobertura)
        {
            var retorno = new List<MotivoInviabilidadeConsulta>();
            var motivos = new List<(EtapasValidacaoConsultaViabilidadeEnum etapa, Motivo_Inviabilidade objeto)>();

            foreach (EtapasValidacaoConsultaViabilidadeEnum etapa in Enum.GetValues(typeof(EtapasValidacaoConsultaViabilidadeEnum)))
            {
                var agrupamento = caixasConsultadas.Where(p => p.EtapaValidacao == etapa).ToList();
                switch (etapa)
                {

                    case EtapasValidacaoConsultaViabilidadeEnum.CadastroEndereco:
                        if (agrupamento.Count() == 0)
                        {
                            motivos.Add((etapa, this._repoMotivo.GetById(2)));
                        }
                        else if (agrupamento.All(p => p.PortasDisponiveis.Count() == 0))
                        {
                            motivos.Add((etapa, this._repoMotivo.GetById(1)));
                        }
                        break;
                    case EtapasValidacaoConsultaViabilidadeEnum.DistanciaRaio:

                        if (agrupamento.Count() == 0)
                        {
                            motivos.Add((etapa, this._repoMotivo.GetById(2)));
                        }
                        else if (agrupamento.All(p => p.PortasDisponiveis.Count() == 0))
                        {
                            motivos.Add((etapa, this._repoMotivo.GetById(1)));
                        }
                        break;
                }
            }
            if (!aprovadoCobertura)
            {
                // Cliente fora da área de cobertura
                motivos.Add((EtapasValidacaoConsultaViabilidadeEnum.AreaCobertura, this._repoMotivo.GetById(3)));
            }
            foreach (var motivo in motivos)
            {
                retorno.Add(new MotivoInviabilidadeConsulta
                {
                    Id = motivo.objeto.Id,
                    Motivo = motivo.objeto.Motivo,
                    EtapaValidacao = motivo.etapa
                });
            }

            return retorno;
        }
        public List<String> PassosConsultaViabilidade(IEnumerable<ConsultaViabilidadeMotivo> motivos)
        {
            var retorno = new List<String>();
            try
            {
                Configuracoes_Gerais config = this._repoConfig.GetConfiguracoesGerais();
                List<Motivo_Inviabilidade> motivosInviabilidade = this._repoMotivo.GetAll().ToList();

                //Cadastro de Endereço                
                retorno.Add($@"Passo 1: Possui Caixa com porta disponível vinculada ao endereço? {(motivos.Where(p => p.Etapa.TipoEtapa == EtapasValidacaoConsultaViabilidadeEnum.CadastroEndereco)
                            .Count() > 0 ? "Não" : "Sim")}");

                //Distancia no Raio                
                retorno.Add($@"Passo 2: Possui caixa com porta disponível dentro de {config.Disponibilidade} metros? {(motivos.Where(p => p.Etapa.TipoEtapa == EtapasValidacaoConsultaViabilidadeEnum.DistanciaRaio)
                            .Count() > 0 ? "Não" : "Sim")}");

                //Área de Cobertura               
                retorno.Add($@"Passo 3: O endereço de instalação do Cliente está dentro de uma área de cobertura? {(motivos.Where(p => p.Etapa.TipoEtapa == EtapasValidacaoConsultaViabilidadeEnum.AreaCobertura)
                            .Count() > 0 ? "Não" : "Sim")}");

            }
            catch (Exception)
            {
            }
            return retorno;
        }
        public (List<Consulta_ViabilidadeCaixa> caixasEncontradas, bool aprovacao) ExecutarEtapaConsultaViabilidade(IEtapaViabilidade tipoConsulta)
        {
            var retorno = new List<Consulta_ViabilidadeCaixa>();
            var aprovado = false;

            try
            {
                var retornoConsulta = tipoConsulta.ExecutarConsulta();
                if (retornoConsulta.Ok)
                {
                    foreach (var caixa in retornoConsulta.Objeto)
                    {
                        var portas = this._repoPorta.GetAll().Where(p =>
                            p.Caixa.Id == caixa.Id
                        ).ToList();

                        var portasDisponiveis = portas.Where(p => p.Status == Sinapse.Model.enumerados.Situacao_Porta.Disponivel);

                        Elemento_Rede_Porta portaReservadaPlano = tipoConsulta.ClientePlanoProduto.HasValue ? portas.FirstOrDefault(p => p.Cliente_Plano_Prod == tipoConsulta.ClientePlanoProduto) : null;

                        if (portasDisponiveis.Count() > 0)
                        {

                            double distanciaCaixa = MapaService.GetMenorDistancia(Convert.ToDouble(tipoConsulta.Latitude),
                                                                   Convert.ToDouble(tipoConsulta.Longitude),
                                                                   caixa.Latitude,
                                                                   caixa.Longitude) / 1852;

                            bool portaReservada = portaReservadaPlano != null;

                            portasDisponiveis = portasDisponiveis.Where(p => p.Cliente_Plano_Prod == null);
                            IEnumerable<int?> idsPortasDisponiveis = portasDisponiveis.Select(p => p.Porta).ToList();

                            var obj = new Consulta_ViabilidadeCaixa
                            {
                                Id = caixa.Id,
                                Descricao = caixa.Descricao,
                                Latitude = caixa.Latitude,
                                Longitude = caixa.Longitude,
                                PortasDisponiveis = idsPortasDisponiveis,
                                reservada = portaReservada,
                                DistanciaKm = distanciaCaixa,
                                Capacidade = caixa.Capacidade,
                                path = caixa.Tipo?.Icone?.Caminho,
                                cor = caixa.Tipo?.Cor,
                                EtapaValidacao = tipoConsulta.Etapa
                            };

                            retorno.Add(obj);
                        }
                    }
                    if (tipoConsulta.Etapa == EtapasValidacaoConsultaViabilidadeEnum.AreaCobertura && retornoConsulta.Ok)
                    {
                        aprovado = true;
                    }
                    else if (retorno.Count > 0)
                    {
                        aprovado = true;
                    }
                }
            }
            catch (Exception)
            {
                retorno = new List<Consulta_ViabilidadeCaixa>();
            }

            return (retorno, aprovado);
        }


        #region Mobile
        public ConsultaViabilidade GetConsultaViabilidadeByLatLongVenda(double lat, double lon)
        {
            ConsultaViabilidade consultaViabilidade = this._repository.GetConsultaViabilidadeByLatLong(lat, lon);

            if (consultaViabilidade != null)
            {
                Configuracoes_Gerais config = this._repoConfig.GetConfiguracoesGerais();

                if (config.ConsultaViabilidadeTempoValidade == null)
                {
                    return consultaViabilidade;

                }
                else
                {

                    if (config.ConsultaViabilidadeTempoValidade.HasValue)
                    {
                        DateTime dataMaxValidadeConsulta = consultaViabilidade.DataHoraCriacao.Value.Date.AddDays(config.ConsultaViabilidadeTempoValidade.Value);
                        bool consultaDentroValidade = DateTime.Now.Date <= dataMaxValidadeConsulta.Date;

                        if (!consultaDentroValidade)
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;

                    }
                }


            }

            return consultaViabilidade;
        }
        #endregion
    }
}
