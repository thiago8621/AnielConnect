using Aniel.Connect.API_Aniel.Models;
using Aniel.Connect.ConnectBusiness.Azza;
using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.ConnectBusiness.MK;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Cidade;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.EtapaVenda;
using Aniel.Connect.Repository.IntegracaoExterna;
using Aniel.Connect.Repository.MK.PendenciaEndereco;
using Aniel.Connect.Repository.MK.PendenciaFinanceira;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.PendenciaEndereco;
using Aniel.Connect.Repository.PendenciaEnderecoAniel;
using Aniel.Connect.Repository.PendenciaFinanceiraAniel;
using Aniel.Connect.Repository.PendenciaFinanceiraIntegracao;
using Aniel.Connect.Repository.Repository.Azza.PendenciaEndereco;
using Aniel.Connect.Repository.Repository.Azza.PendenciaFinanceira;
using Aniel.Connect.Repository.Repository.PendenciaFinanceiraAniel;
using Aniel.Connect.Repository.Repository.T5.ConfigAnielIntegrator;
using Aniel.Connect.Repository.Venda;
using MySqlX.XDevAPI;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    public class ConsultaPendenciaEnderecoApiController : BaseApiController
    {
        [HttpPost]
        [Route("api/ConsultaPendenciaEnderecoApi/GetFaturaByEndereco")]
        public async Task<GetApiRetorno<IEnumerable<ConsultaPendenciasListModel>>> GetFaturaByEndereco(PendenciaEnderecoRequest request)
        {
            PendenciaEndereco pendenciaEndereco = null;
            var retorno = new GetApiRetorno<IEnumerable<ConsultaPendenciasListModel>>
            {
                Objeto = new List<ConsultaPendenciasListModel>()
            };

            try
            {
                using (var usuarioApi = new UsuarioApiController())
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionT5 = HibernateUtil.GetSessionT5())
                {
                    if (request == null)
                    {
                        throw new Exception("Parâmetros da requisição não informados");
                    }

                    if (request.settings != null && !usuarioApi.GetUsuarioValido(request.settings).Result.Ok)
                    {
                        throw new Exception("Usuário e/ou Senha e/ou Token Inválidos");
                    }



                    IPendenciaEnderecoRepository repoPendenciaEndereco = new PendenciaEnderecoRepository(session);
                    IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(session);
                    IIntegracaoExternaRepository repository = new IntegracaoExternaRepository(session);
                    IPendenciaEnderecoAnielRepository repoPendenciaEnderecoAnielConnect = new PendenciaEnderecoAnielRepository(session);
                    IPendenciaFinanceiraAnielRepository repoPendenciaFinanceiraAnielConnect = new PendenciaFinanceiraAnielRepository(session);
                    var pendenciaEnderecoBusiness = new PendenciaEnderecoBusiness(repoPendenciaEndereco, repoConfig);
                    var consultaPendenciasConnectBusiness = new ConsultaPendenciasConnectBusiness(repoPendenciaEnderecoAnielConnect, repoPendenciaFinanceiraAnielConnect);
                    var business = new IntegracaoExternaBusiness(repository);

                    IPendenciaEnderecoRepository repoPendenciaEnderecoConnect = new PendenciaEnderecoRepository(session);
                    IIntegracaoExternaRepository repoIntegracaoExterna = new IntegracaoExternaRepository(session);
                    var pendenciaEnderecoConnectBusiness =
                        new PendenciaEnderecoBusiness(repoPendenciaEnderecoConnect, repoConfig);

                    if (repository.UtilizaMkConsultaPendenciaEndereco())
                    {
                        if (String.IsNullOrEmpty(CONSTANTES.MKConnectionString))
                        {
                            throw new Exception("Banco do MK Solutions não está configurado");
                        }

                        if (String.IsNullOrEmpty(request.Logradouro))
                        {
                            throw new Exception("Logradouro não informado para a consulta de endereço");
                        }

                        if (String.IsNullOrEmpty(request.Numero))
                        {
                            throw new Exception("Número não informado para a consulta de endereço");
                        }

                        using (var connPg = PostgreSqlUtil.GetConnection(CONSTANTES.MKConnectionString))
                        {
                            IPendenciaEnderecoMkRepository repoPendenciaEnderecoMk = new PendenciaEnderecoMkRepository(connPg);
                            IPendenciaFinanceiraIntegracaoRepository repoPendenciaFinanceiraMk = new PendenciaFinanceiraMkRepository(connPg);
                            var mkBusiness = new MKBusiness(repoPendenciaEnderecoMk, repoPendenciaFinanceiraMk, null);

                            IEnumerable<PendenciaEnderecoListModel> pendenciasMk =
                                mkBusiness.GetPendenciasEndereco(request.Logradouro, request.Numero, request.Cidade);

                            // Gravando a execução da consulta e os dados das pendências no Connect
                            pendenciaEndereco = pendenciaEnderecoConnectBusiness
                                .GravarPendenciaEndereco(request.Logradouro, request.Numero, pendenciasMk);

                            retorno.Objeto = pendenciasMk.Select(p => new ConsultaPendenciasListModel
                            {
                                CodFatura = p.CodFatura,
                                Cod_Contrato = p.Cod_Contrato,
                                Data_Vencimento = p.Data_Vencimento,
                                Valor_Total = p.Valor_Total
                            });



                            retorno.Ok = true;
                        }
                    }
                    else if (business.UtilizaIXCConsultaPendenciaEndereco())
                    {
                        if (String.IsNullOrEmpty(request.Logradouro))
                        {
                            throw new Exception("Logradouro não informado para a consulta de endereço");
                        }

                        if (String.IsNullOrEmpty(request.Numero))
                        {
                            throw new Exception("Número não informado para a consulta de endereço");
                        }

                        IConfigAnielIntegratorRepository _repoConfigAnielIntegrator = new ConfigAnielIntegratorRepository(sessionT5);
                        var objIntegrator = await _repoConfigAnielIntegrator.ObterConfigAnielIntegrator();
                        if (objIntegrator != null)
                        {
                            IPendenciaFinanceiraIntegracaoRepository repoPendenciaFinanceiraIXC = new PendenciaFinanceiraIXCRepository(objIntegrator.URL, objIntegrator.Token);
                            ICidadeRepository cidadeRepository = new CidadeRepository(session);
                            IPendenciaEnderecoIntegracaoRepository repoPendenciaEnderecoIXC = new PendenciaEnderecoIntegracaoRepository(objIntegrator.URL, objIntegrator.Token, cidadeRepository);

                            var azzaBusiness = new IXCBusiness(repoPendenciaFinanceiraIXC, null, repoPendenciaEnderecoIXC);

                            IEnumerable<PendenciaEnderecoListModel> pendenciasAzza = await azzaBusiness.GetPendenciasEndereco(request.Logradouro, request.Numero, request.Cidade, request.CEP);

                            pendenciaEndereco =
                                    pendenciaEnderecoConnectBusiness.GravarPendenciaEndereco(request.Logradouro, request.Numero, pendenciasAzza);
                            retorno.Objeto = pendenciasAzza.Select(p => new ConsultaPendenciasListModel
                            {
                                CodFatura = p.CodFatura,
                                Cod_Contrato = p.Cod_Contrato,
                                Data_Vencimento = p.Data_Vencimento,
                                Valor_Total = p.Valor_Total
                            }).ToList();
                        }
                        else
                        {
                            throw new Exception("Integração do IXC não está devidamente configurada");
                        }




                    }
                    else if (business.UtilizaAlcansConsultaPendenciaFinanceira())
                    {
                        using (var integracoesExternas = new Conexao<IntegracoesExternas>(session))
                        using (VendaApiController vendaApiController = new VendaApiController())
                        {
                            if (request.IdVenda == null)
                            {
                                throw new Exception("Logradouro não informado para a consulta de endereço");
                            }

                            var configIntegracao = await integracoesExternas.selectFirst(x => x.TipoIntegracao == Model.enumerado.EnumTipoIntegracao.CONSULTA_PENDENCIA_FINANCEIRA && x.IntegracaoExterna == Model.enumerado.IntegracaoExterna.ALCANS);

                            if (configIntegracao != null || !string.IsNullOrEmpty(configIntegracao.Usuario) || !string.IsNullOrEmpty(configIntegracao.Senha) || !string.IsNullOrEmpty(configIntegracao.UrlApi))
                            {
                                IPendenciaEnderecoMkRepository repoPendenciaFinanceiraAlcans = new PendenciaEnderecoMkAlcansRepository(configIntegracao.Usuario, configIntegracao.Senha, configIntegracao.UrlApi);

                                var enderecoConnect = await GetEnderencoByVenda(request.IdVenda.Value);

                                if (enderecoConnect == null || string.IsNullOrEmpty(enderecoConnect.CodUf) || string.IsNullOrEmpty(enderecoConnect.CodCidade) || string.IsNullOrEmpty(enderecoConnect.CodBairro)
                                    || string.IsNullOrEmpty(enderecoConnect.CodRua))
                                    throw new Exception(Resources.RsCadastros.MENSAGEM_ENDERECO_SEM_CODAUX);

                                IEnumerable<PendenciaEnderecoListModel> pendenciasAlcans = repoPendenciaFinanceiraAlcans.GetPendenciasEnderecoAlcans(
                                    "", "", "", enderecoConnect.CodRua, request.Numero);

                                pendenciaEndereco = pendenciaEnderecoConnectBusiness.GravarPendenciaEndereco(request.Logradouro, request.Numero, pendenciasAlcans);

                                retorno.Objeto = pendenciasAlcans.Select(p => new ConsultaPendenciasListModel
                                {
                                    CodFatura = p.CodFatura,
                                    Cod_Contrato = p.Cod_Contrato,
                                    Data_Vencimento = p.Data_Vencimento,
                                    Valor_Total = p.Valor_Total
                                }).ToList();
                            }
                            else
                                throw new Exception("Middleware da Alcans não configurado");
                        }
                    }
                    else
                    {
                        IEnumerable<PendenciaEnderecoListModel> pendenciasConnect =
                           consultaPendenciasConnectBusiness.GetPendenciasEndereco(request.Logradouro, request.Numero);

                        pendenciaEndereco = pendenciaEnderecoBusiness.GravarPendenciaEndereco(request.Logradouro, request.Numero, pendenciasConnect);

                        retorno.Objeto = pendenciasConnect.Select(p => new ConsultaPendenciasListModel
                        {
                            CodFatura = p.CodFatura,
                            Cod_Contrato = p.Cod_Contrato,
                            Data_Vencimento = p.Data_Vencimento,
                            Valor_Total = p.Valor_Total
                        }).ToList();
                    }



                    if (request.IdVenda != null && pendenciaEndereco?.Id != null)
                    {

                        IVendaRepository repoVenda = new VendaRepository(session);
                        var vendaBusiness = new VendaBusiness(repoVenda);
                        Venda venda = vendaBusiness.GetVendaById(request.IdVenda.Value);
                        vendaBusiness.GravarIdConsultaRestricao(venda, EnumEtapaVenda.ConsultaPendenciaEndereco, pendenciaEndereco.Id);

                        IEtapaVendaRepository repoEtapa = new EtapaVendaRepository(session);
                        var businessEtapa = new EtapaVendaBusiness(repoEtapa, repoConfig);
                        bool possuiPendencia = retorno.Objeto.Any();
                        if (possuiPendencia)
                        {
                            var config = repoConfig.GetConfiguracoesGerais();
                            if (config.ConsultaPendenciaEnderecoRestringeVenda)
                            {
                                businessEtapa.MudarStatusEtapa(request.IdVenda.Value, Sinapse.Model.enumerados.EnumEtapaVenda.ConsultaPendenciaEndereco, Sinapse.Model.enumerados.StatusEtapaVenda.Reprovado);
                            }
                            else
                            {
                                businessEtapa.MudarStatusEtapa(request.IdVenda.Value, Sinapse.Model.enumerados.EnumEtapaVenda.ConsultaPendenciaEndereco, Sinapse.Model.enumerados.StatusEtapaVenda.AguardandoAprovacaoGestor);
                            }
                        }
                        else
                        {
                            businessEtapa.MudarStatusEtapa(request.IdVenda.Value, Sinapse.Model.enumerados.EnumEtapaVenda.ConsultaPendenciaEndereco, Sinapse.Model.enumerados.StatusEtapaVenda.Aprovado);
                        }

                    }

                    retorno.Ok = true;
                }
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

        private async Task<EnderecoVendaIntegracao> GetEnderencoByVenda(long idVenda)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(@"SELECT C.COD_AUX COD_CIDADE, D.COD_AUXILIAR COD_UF, E.COD_AUX COD_BAIRRO, F.COD_AUXILIAR COD_RUA, B.ENDERECO_NUMERO
                    FROM TB_VENDA A
                    INNER JOIN TB_ENDERECO_CLIENTE B
                    ON (B.ID = A.ID_ENDERECO_INSTALACAO)
                    INNER JOIN TB_CIDADE C
                    ON (C.ID = B.ID_CIDADE)
                    INNER JOIN TB_UF D
                    ON(D.SIGLA = B.ENDERECO_UF)
                    INNER JOIN TB_ENDERECO_LOGRADOURO F
                    ON(F.ID = B.ID_ENDERECO_LOGRADOURO )
                    INNER JOIN TB_BAIRRO E
                    ON(E.ID = F.ID_BAIRRO )
                    WHERE A.ID = @idVenda
                    AND C.COD_AUX IS NOT NULL AND D.COD_AUXILIAR IS NOT NULL AND E.COD_AUX IS NOT NULL AND F.COD_AUXILIAR IS NOT NULL");

            Dictionary<string, object> parametros = new Dictionary<string, object> { { "idVenda", idVenda } };

            using (ISession session = HibernateUtil.GetSession())
            using (DbCommand com = session.Connection.CreateCommand())
            {
                using (DbDataReader reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                {
                    if (await reader.ReadAsync())
                    {
                        return new EnderecoVendaIntegracao
                        {
                            CodCidade = Convert.ToString(reader["COD_CIDADE"]),
                            CodUf = Convert.ToString(reader["COD_UF"]),
                            CodBairro = Convert.ToString(reader["COD_BAIRRO"]),
                            CodRua = Convert.ToString(reader["COD_RUA"]),
                            Numero = Convert.ToString(reader["ENDERECO_NUMERO"]),
                        };
                    }
                }

                return null;
            }
        }

        public class EnderecoVendaIntegracao
        {
            public string CodCidade { get; set; }
            public string CodUf { get; set; }
            public string CodBairro { get; set; }
            public string CodRua { get; set; }
            public string Numero { get; set; }
        }
    }
}
