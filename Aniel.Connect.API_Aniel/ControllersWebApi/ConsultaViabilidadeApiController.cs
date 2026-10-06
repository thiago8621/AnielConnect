using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.ConnectBusiness.T5.Consulta_de_Viabilidade;
using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Cidade;
using Aniel.Connect.Repository.Cliente;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.ConsultaViabilidade;
using Aniel.Connect.Repository.EnderecoCliente;
using Aniel.Connect.Repository.EnderecoLogradouro;
using Aniel.Connect.Repository.EtapaVenda;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.MotivoInviabilidade;
using Aniel.Connect.Repository.Repository.T5.ElementoRedeEnderecos;
using Aniel.Connect.Repository.T5.AreaCoberturaMongo;
using Aniel.Connect.Repository.T5.ElementoRede;
using Aniel.Connect.Repository.T5.ElementoRedeEnderecos;
using Aniel.Connect.Repository.T5.ElementoRedePorta;
using Aniel.Connect.Repository.Venda;
using Microsoft.AspNet.Identity;
using NHibernate;
using Sinapse.DAO.MongoDB;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class ConsultaViabilidadeApiController : BaseApiController
    {
        [HttpPost]
        [Route("api/Consulta_ViabilidadeApi/ConsultaViabilidadeVendas")]
        public async Task<GetApiRetorno<ConsultaViabilidadeResponse>> ConsultaViabilidadeVendas(Consulta_ViabilidadeRequestModel request)
        {
            var retorno = new GetApiRetorno<ConsultaViabilidadeResponse>();
            retorno.Objeto = new ConsultaViabilidadeResponse();

            var motivos = new List<MotivoInviabilidadeConsulta>();

            try
            {
                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Invalidos");
                    }
                }
                if (request.Venda == null && request.Cliente == null) 
                {
                    throw new Exception("Viabilidade precisa estar atrelada ou à Venda ou ao Cliente");
                }
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                using (var sessionMongo = new ConexaoMongoGeography())
                using (Conexao<Venda_Ultimo_Contato> repoVenda_Ultimo_Contato = new Conexao<Venda_Ultimo_Contato>(sessionNh))
                {
                    IElementoRedeAnielRepository repoElemRede = new ElementoRedeAnielRepository(sessionT5Nh);
                    IElementoRedePortaAnielRepository repoPorta = new ElementoRedePortaAnielRepository(sessionNh, sessionT5Nh);
                    IConsultaViabilidadeRepository repoConsultaViabilidade = new ConsultaViabilidadeRepository(sessionT5Nh);
                    IElementoRedeEnderecosAnielRepository repositoryElementoRedeEnderecos = new ElementoRedeEnderecosAnielRepository(sessionT5Nh);
                    
                    IClienteRepository repoCliente = new ClienteRepository(sessionNh);
                    IVendaRepository repoVenda = new VendaRepository(sessionNh);
                    ICidadeRepository repositoryCidade = new CidadeRepository(sessionNh);
                    IEtapaVendaRepository repoEtapaVenda = new EtapaVendaRepository(sessionNh);
                    IMotivoInviabilidadeRepository repoMotivo = new MotivoInviabilidadeRepository(sessionNh);
                    IEnderecoClienteRepository repoEnderecoCliente = new EnderecoClienteRepository(sessionNh);
                    IConfiguracoesGeraisRepository repositoryConfig = new ConfiguracoesGeraisRepository(sessionNh);                    
                    IEnderecoLogradouroRepository repositoryEndLogradouro = new EnderecoLogradouroRepository(sessionNh);

                    IAreaCoberturaMongoRepository repoAreaCobertura = new AreaCoberturaMongoRepository(sessionMongo);
                    
                    var business = new VendaBusiness(repoVenda, repoEnderecoCliente);
                    var clienteBusiness = new ClienteBusiness(repoCliente);
                    var consultaViabilidadeBusiness =
                        new ConsultaViabilidadeBusiness(
                            repoConsultaViabilidade,
                            repoElemRede,
                            repoPorta,
                            repositoryConfig,
                            repoMotivo,
                            repoAreaCobertura
                        );

                    var config = repositoryConfig.GetConfiguracoesGerais();
                    var venda = business.GetVendaById(request.Venda ?? 0);
                    var cliente = clienteBusiness.GetById(request.Cliente ?? 0);
                    var raio = (config.Disponibilidade) / 1000.0;

                    List<Consulta_ViabilidadeCaixa> caixasConsulta = new List<Consulta_ViabilidadeCaixa>();
                    //Consulta por Cadastro de Endereço
                    Endereco_Cliente enderecoUtilizado = request.Venda != null ? venda.EnderecoInstalacao : 
                                                         cliente.Tipo_Cliente == Tipo_Cliente.Prospecto ? cliente.Endereco_Prospecto : 
                                                         cliente.Endereco_Principal;
                    
                    var latitude = request.Latitude != 0 ? request.Latitude : enderecoUtilizado?.Latitude;
                    var longitude = request.Longitude != 0 ? request.Longitude : enderecoUtilizado?.Longitude;

                    if(!latitude.HasValue  || !longitude.HasValue)
                    {
                        throw new Exception("Latitude e Longitude são obrigatórios para consulta de viabilidade na requisição ou no endereço do cliente/venda");
                    }

                    var caixasCadastroEndereco = consultaViabilidadeBusiness.ExecutarEtapaConsultaViabilidade(
                                                                        new ConsultaViabilidadeCadastroEndereco(latitude,
                                                                        longitude,
                                                                        cliente?.PlanosProdutos?.FirstOrDefault()?.Id,
                                                                        config.Utiliza_Cadastro_Endereco, enderecoUtilizado?.Id,
                                                                        enderecoUtilizado?.Id, enderecoUtilizado?.Numero, enderecoUtilizado?.Id_Cidade,
                                                                        enderecoUtilizado?.Logradouro, enderecoUtilizado?.Bloco, enderecoUtilizado?.Apartamento, repositoryCidade,
                                                                        repositoryElementoRedeEnderecos, repositoryEndLogradouro));
                    //Consulta por Campo Dentro do Raio
                    var caixasDentroRaio = consultaViabilidadeBusiness.ExecutarEtapaConsultaViabilidade(
                                                                         new ConsultaViabilidadeDentroRaio(latitude,longitude, cliente?.PlanosProdutos.FirstOrDefault()?.Id, raio, repoElemRede));
                    //Consulta por Área de Cobertura
                    var consultaCobertura = consultaViabilidadeBusiness.ExecutarEtapaConsultaViabilidade(
                                                                        new ConsultaViabilidadeAreaCobertura(latitude,
                                                                        longitude, cliente?.PlanosProdutos.FirstOrDefault()?.Id, repoAreaCobertura));

                    caixasConsulta.AddRange(caixasCadastroEndereco.caixasEncontradas.ToList());
                    caixasConsulta.AddRange(caixasDentroRaio.caixasEncontradas.ToList());

                    if ((!caixasCadastroEndereco.aprovacao) || (!caixasDentroRaio.aprovacao) || !consultaCobertura.aprovacao)
                    {
                        motivos =
                            consultaViabilidadeBusiness
                                .VerificarMotivosInviabilidade(
                                    caixasConsulta,
                                    latitude.Value,
                                    longitude.Value,
                                    consultaCobertura.aprovacao
                                );
                    }

                    ConsultaViabilidade consultaViabilidade =
                        consultaViabilidadeBusiness
                            .GravarConsultaViabilidade(
                                latitude.Value,
                                longitude.Value,
                                caixasConsulta,
                                motivos,
                                config
                            );

                    if (consultaViabilidade == null)
                    {
                        throw new Exception("Erro ao gravar os dados da consulta de viabilidade");
                    }

                    StatusConsultaViabilidade possuiDisponibilidade = consultaViabilidade.Status;
                    if (request.Venda.HasValue)
                    {                                               
                       
                        if (venda == null)
                        {
                            throw new Exception("Erro ao gravar os dados da consulta de viabilidade,Esta Sem Etapa de Venda");
                        }
                        if (request.Latitude != 0 && request.Longitude != 0) 
                        {
                            venda.EnderecoInstalacao.Latitude = request.Latitude;
                            venda.EnderecoInstalacao.Longitude = request.Longitude;
                        }

                        Venda_Ultimo_Contato historico = new Venda_Ultimo_Contato
                        {
                            DataHoraCriacao = DateTime.Now,
                            Acao = "Verificar Disponibilidade",
                            Data_Ultimo_Contato = DateTime.Now,
                            Id_Venda = request.Venda.Value,
                            Venda = venda,
                            UsuarioCriacao = User.Identity.GetUserId(),
                            Observacao = possuiDisponibilidade != StatusConsultaViabilidade.Reprovado ? "Disponível em sua região" : "Indisponível em sua região"
                        };

                        business.Update(venda);
                        IEtapaVendaRepository repoEtapa = new EtapaVendaRepository(sessionNh);
                        var businessEtapa = new EtapaVendaBusiness(repoEtapa, repositoryConfig);

                        StatusEtapaVenda statusEtapa;

                        if (consultaViabilidade.Status == StatusConsultaViabilidade.Reprovado)
                        {
                            if (config.ConsultaViabilidadeRestringeVenda)
                            {
                                statusEtapa = StatusEtapaVenda.Reprovado;
                            }
                            else
                            {
                                statusEtapa = StatusEtapaVenda.AguardandoAprovacaoGestor;
                            }
                        }
                        else if (consultaViabilidade.Status == StatusConsultaViabilidade.Aprovado_Ressalva)
                        {
                            statusEtapa = StatusEtapaVenda.AguardandoAprovacaoGestor;
                        }
                        else
                        {
                            statusEtapa = StatusEtapaVenda.Aprovado;
                        }

                        businessEtapa.MudarStatusEtapa(request.Venda, Sinapse.Model.enumerados.EnumEtapaVenda.ConsultaViabilidade, statusEtapa);
                        business.GravarIdConsultaRestricao(venda, EnumEtapaVenda.ConsultaViabilidade, consultaViabilidade.Id);
                        await repoVenda_Ultimo_Contato.Save(historico);
                    }
                    var listaMotivos = consultaViabilidade.Etapas.SelectMany(p => p.Motivos).ToList();
                    retorno.Objeto.IdConsultaViabilidade = consultaViabilidade.Id;
                    retorno.Objeto.PossuiDisponibilidade = possuiDisponibilidade;
                    retorno.Objeto.Motivos = listaMotivos.Select(p => new MotivoInviabilidadeConsulta
                    {
                        Id = p.Id,
                        Motivo = p.Motivo,
                    }).ToList();
                    retorno.Objeto.Passos = consultaViabilidadeBusiness.PassosConsultaViabilidade(listaMotivos);
                    retorno.Objeto.Caixas = caixasConsulta.ToList();
                    retorno.Objeto.Latitude = request.Latitude != 0 ? request.Latitude : enderecoUtilizado.Latitude;
                    retorno.Objeto.Longitude = request.Longitude != 0 ? request.Longitude:enderecoUtilizado.Longitude;
                }
                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;

                CONSTANTES.log4Net.ErrorFormat("Message: {0} \r\n InnerException : {1} StackTrace : {2}",
                                ex.Message,
                                ex?.InnerException?.Message,
                                ex.StackTrace);
            }

            return retorno;
        }

        [HttpPost]
        [Route("api/Consulta_ViabilidadeApi/ConsultaViabilidadeLatLong")]
        public async Task<GetApiRetorno<ConsultaViabilidadeResponse>> ConsultaViabilidadePorLatLong(Consulta_ViabilidadeRequestModel request)
        {
            var retorno = new GetApiRetorno<ConsultaViabilidadeResponse>
            {
                Objeto = new ConsultaViabilidadeResponse()
            };

            try
            {
                using (var api = new UsuarioApiController())
                {
                    if (!(await api.GetUsuarioValido(request.settings)).Ok)
                        throw new Exception("Usuário e/ou Senha e/ou Token inválidos");
                }

                if (request.Latitude == 0 || request.Longitude == 0)
                    throw new Exception("Latitude e Longitude são obrigatórios");

                using (ISession sessionNh = HibernateUtil.GetSession())
                using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
                using (var sessionMongo = new ConexaoMongoGeography())
                {
                    IElementoRedeAnielRepository repoElemRede = new ElementoRedeAnielRepository(sessionT5Nh);
                    IElementoRedePortaAnielRepository repoPorta = new ElementoRedePortaAnielRepository(sessionNh, sessionT5Nh);
                    IConsultaViabilidadeRepository repoConsultaViabilidade = new ConsultaViabilidadeRepository(sessionT5Nh);
                    IMotivoInviabilidadeRepository repoMotivo = new MotivoInviabilidadeRepository(sessionNh);
                    IConfiguracoesGeraisRepository repositoryConfig = new ConfiguracoesGeraisRepository(sessionNh);
                    IAreaCoberturaMongoRepository repoAreaCobertura = new AreaCoberturaMongoRepository(sessionMongo);

                    var consultaViabilidadeBusiness =
                        new ConsultaViabilidadeBusiness(
                            repoConsultaViabilidade,
                            repoElemRede,
                            repoPorta,
                            repositoryConfig,
                            repoMotivo,
                            repoAreaCobertura
                        );

                    var config = repositoryConfig.GetConfiguracoesGerais();
                    var raio = config.Disponibilidade / 1000.0;

                    var caixasConsulta = new List<Consulta_ViabilidadeCaixa>();

                    // 🔹 Consulta dentro do raio
                    var caixasDentroRaio =
                        consultaViabilidadeBusiness.ExecutarEtapaConsultaViabilidade(
                            new ConsultaViabilidadeDentroRaio(
                                request.Latitude,
                                request.Longitude,
                                null,
                                raio,
                                repoElemRede
                            )
                        );

                    // 🔹 Consulta área de cobertura
                    var consultaCobertura =
                        consultaViabilidadeBusiness.ExecutarEtapaConsultaViabilidade(
                            new ConsultaViabilidadeAreaCobertura(
                                request.Latitude,
                                request.Longitude,
                                null,
                                repoAreaCobertura
                            )
                        );

                    caixasConsulta.AddRange(caixasDentroRaio.caixasEncontradas);

                    var motivos = new List<MotivoInviabilidadeConsulta>();

                    if (!caixasDentroRaio.aprovacao || !consultaCobertura.aprovacao)
                    {
                        motivos = consultaViabilidadeBusiness.VerificarMotivosInviabilidade(
                            caixasConsulta,
                            request.Latitude,
                            request.Longitude,
                            consultaCobertura.aprovacao
                        );
                    }

                    var consultaViabilidade =
                        consultaViabilidadeBusiness.GravarConsultaViabilidade(
                            request.Latitude,
                            request.Longitude,
                            caixasConsulta,
                            motivos,
                            config
                        );

                    if (consultaViabilidade == null)
                        throw new Exception("Erro ao gravar consulta de viabilidade");

                    var listaMotivos = consultaViabilidade.Etapas.SelectMany(e => e.Motivos).ToList();

                    retorno.Objeto.IdConsultaViabilidade = consultaViabilidade.Id;
                    retorno.Objeto.PossuiDisponibilidade = consultaViabilidade.Status;
                    retorno.Objeto.Motivos = listaMotivos.Select(m => new MotivoInviabilidadeConsulta
                    {
                        Id = m.Id,
                        Motivo = m.Motivo
                    }).ToList();

                    retorno.Objeto.Passos =
                        consultaViabilidadeBusiness.PassosConsultaViabilidade(listaMotivos);

                    retorno.Objeto.Caixas = caixasConsulta;
                    retorno.Objeto.Latitude = request.Latitude;
                    retorno.Objeto.Longitude = request.Longitude;

                    retorno.Ok = true;
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException?.Message;

                CONSTANTES.log4Net.ErrorFormat(
                    "Message: {0} InnerException : {1} StackTrace : {2}",
                    ex.Message,
                    ex.InnerException?.Message,
                    ex.StackTrace
                );
            }

            return retorno;
        }
    }
}