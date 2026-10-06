using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.ConnectBusiness.Playhub;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Attributes;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.IntegracaoExterna;
using Aniel.Connect.Repository.Playhub;
using Microsoft.AspNet.Identity;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Util;
using Sinapse.Core;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class ClienteContratoApiController : ApiController
    {
        [HttpPost()]
        [Route("api/clienteContratoapi/GetCombo")]
        public async Task<ObjCombo<Int64>> GetCombo(RequestObjComboPai<Int64?, Int64?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    #region [Filtros]
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();

                    StringBuilder sqlFrom = new StringBuilder($@" FROM TB_CLIENTE_CONTRATO A 
                                                                  INNER JOIN TB_CLIENTE_PLANO_PRODUTO B ON B.ID_CLIENTE_CONTRATO = A.ID 
                                                                  LEFT JOIN TB_PLANO C ON (C.ID = B.ID_PLANO)
                                                                  LEFT JOIN TB_PRODUTO D ON (D.ID = B.ID_PRODUTO)
                                                                  LEFT JOIN TB_COMBO E ON (E.ID = B.ID_COMBO)");

                    List<String> condicoes = new List<String>();

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id}");
                    }

                    if (request.idpai.HasValue)
                    {
                        condicoes.Add($"B.ID_CLIENTE = {request.idpai}");
                    }

                    if (request.soativos)
                    {
                        condicoes.Add($"(A.STATUS <> 4 OR A.STATUS IS NULL)");
                    }
                    else
                    {
                        condicoes.Add($"(A.STATUS IN (0, 1, 2, 3, 4, 5) OR A.STATUS IS NULL)");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER(A.NUMERO_CONTRATO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 255)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE A.NUMERO_CONTRATO != '' and ( {String.Join(" AND ", condicoes)} ) ");
                    }
                    #endregion

                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) AS CO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT A.ID, A.NUMERO_CONTRATO, A.DESCRICAO, A.STATUS ");

                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" GROUP BY A.ID, A.NUMERO_CONTRATO, A.DESCRICAO, A.STATUS ");
                        sql.AppendLine(" ORDER BY UPPER(A.NUMERO_CONTRATO) ");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                var desc = Convert.ToString(reader["NUMERO_CONTRATO"]) + " - " + Convert.ToString(reader["DESCRICAO"]);

                                if (reader["STATUS"] != DBNull.Value)
                                {
                                    if (Convert.ToInt64(reader["STATUS"]) == 4)
                                    {
                                        desc += " - Cancelado";
                                    }
                                }

                                if (!String.IsNullOrEmpty(Convert.ToString(reader["NUMERO_CONTRATO"])))
                                {
                                    retorno.results.Add(new ObjComboResult<Int64>
                                    {
                                        Id = Convert.ToInt64(reader["ID"]),
                                        Text = Convert.ToString(reader["ID"]) + " - " + Convert.ToString(reader["DESCRICAO"])
                                    });

                                }
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/clienteapi/GetComboContratoClienteEndereco")]
        public async Task<ObjCombo<long>> GetComboContratoClienteEndereco(RequestObjComboAvo<Int64?, Int64?, Boolean?> request)
        {
            ObjCombo<Int64> retorno = new ObjCombo<Int64>
            {
                results = new List<IObjComboResult<Int64>>()
            };

            using (ISession session = HibernateUtil.GetSession())
            {
                try
                {
                    //Filtros
                    List<String> condicoes = new List<String>();
                    Dictionary<String, Object> parametros = new Dictionary<string, object>();
                    StringBuilder sqlFrom = new StringBuilder($@" FROM TB_CLIENTE_PLANO_PRODUTO A 
                                                                  LEFT JOIN TB_PLANO B ON (B.ID = A.ID_PLANO)
                                                                  LEFT JOIN TB_PRODUTO C ON (C.ID = A.ID_PRODUTO)
                                                                  LEFT JOIN TB_CLIENTE_CONTRATO D ON (D.ID = A.ID_CLIENTE_CONTRATO)
                                                                  LEFT JOIN TB_MODELO_CONTRATO E ON (E.ID = D.ID_MODELO_CONTRATO)
                                                                  LEFT JOIN TB_ENDERECO_CLIENTE F ON(F.ID = A.ID_ENDERECO_INSTALACAO)");

                    if (request.id.HasValue)
                    {
                        condicoes.Add($"A.ID = {request.id} OR D.ID = {request.id}");
                    }

                    if (request.idpai.HasValue)
                    {
                        condicoes.Add($"A.ID_CLIENTE = {request.idpai}");
                    }
                    
                    if (request.soativos)
                    {
                        condicoes.Add($"(A.STATUS <> 4 OR A.STATUS IS NULL)");
                    }
                    else
                    {
                        condicoes.Add($"(A.STATUS IN (0, 1, 2, 3, 4, 5) OR A.STATUS IS NULL)");
                    }

                    if (request != null && !String.IsNullOrEmpty(request.q))
                    {
                        condicoes.Add($"(UPPER(D.NUMERO_CONTRATO) LIKE '%{StringService.Truncate(request.q.ToUpperInvariant(), 255)}%')");
                    }

                    if (condicoes.Count() > 0)
                    {
                        sqlFrom.AppendLine($" WHERE COALESCE(D.NUMERO_CONTRATO,'') <> '' and ( {String.Join(" AND ", condicoes)} ) ");
                    }



                    using (var com = session.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder("SELECT COUNT(A.ID) AS CO ");
                        sql.AppendLine(sqlFrom.ToString());

                        using (var reader = await CommandHelper.ExecuteReader(com, sql, parametros, session.Connection, null))
                        {
                            if (await reader.ReadAsync())
                                retorno.total_count = Convert.ToInt32(reader["CO"]);
                        }
                    }

                    List<String> numeros_Contrato = new List<String>();

                    //Select
                    using (var com = session.Connection.CreateCommand())
                    {

                        StringBuilder sql = new StringBuilder("SELECT DISTINCT D.ID, A.ID_CLIENTE, D.NUMERO_CONTRATO, E.DESCRICAO AS DESCRICAO_MODELCONTR, F.ENDERECO_LOGRADOURO ,F.ENDERECO_NUMERO , F.ENDERECO_BAIRRO, F.ENDERECO_CIDADE ");
                        sql.AppendLine(sqlFrom.ToString());
                        sql.AppendLine(" ORDER BY UPPER(A.ID) ");

                        using (var reader = await CommandHelper.ExecuteReaderPage(com, sql, parametros, session.Connection, null, request.page))
                        {
                            while (await reader.ReadAsync())
                            {
                                if (reader["ID"] != DBNull.Value)
                                {
                                    string logradouro = reader["ENDERECO_LOGRADOURO"] != DBNull.Value ? Convert.ToString(reader["ENDERECO_LOGRADOURO"]) : "-";
                                    string numeroLogradouro = reader["ENDERECO_NUMERO"] != DBNull.Value ? Convert.ToString(reader["ENDERECO_NUMERO"]) : "-";
                                    string cidade = reader["ENDERECO_CIDADE"] != DBNull.Value ? Convert.ToString(reader["ENDERECO_CIDADE"]) : "-";
                                    string bairro = reader["ENDERECO_BAIRRO"] != DBNull.Value ? Convert.ToString(reader["ENDERECO_BAIRRO"]) : "-";

                                    string Descricao = Convert.ToString($"{reader["NUMERO_CONTRATO"]} - {logradouro} - {numeroLogradouro} - {bairro} - {cidade}");

                                    var obj = new ObjComboResult<Int64>
                                    {
                                        Id = Convert.ToInt64(reader["ID"]),
                                        Text = Descricao
                                    };

                                    retorno.results.Add(obj);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            retorno.results.GroupBy(x => x.Text).Select(x => x.FirstOrDefault()).ToList();

            return retorno;
        }

        [HttpPost()]
        [Route("api/clienteContratoapi/AlterarStatusContrato")]
        public async Task<GetApiRetorno<Int64>> AlterarStatusContrato(AlterarStatusContratoViewModel value)
        {
            GetApiRetorno<Int64> retorno = new GetApiRetorno<Int64>();
            Response responseRadius = new Response();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repoPlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
                    using (var repoStatusLog = new Conexao<Cliente_PlanoProduto_StatusLog>(session))
                    using (var repoClienteContratoLog = new Conexao<Cliente_Contrato_StatusLog>(session))
                    using (var repoMotivoCanc = new Conexao<Motivo_Cancelamento_PlanoProduto>(session))
                    using (var repoMotivoSuspensao = new Conexao<Motivo_Suspensao_Contratual>(session))
                    using (var repoClienteContrato = new Conexao<ClienteContrato>(session))
                    using (var repoCobrancaAdicional = new Conexao<CobrancaAdicional>(session))
                    {

                        var listaPlanoProduto = await repoPlanoProduto.select(x => x.ClienteContrato.Id == value.id);
                        var Ids_TrocaPlanos = listaPlanoProduto.Select(x => x.Id).ToList();
                        var listaTmp = await repoPlanoProduto.select(x => x.Id_TrocaPlano.IsIn(Ids_TrocaPlanos));
                        if (listaTmp.Count > 0)
                        {
                            retorno.Ok = false;
                            retorno.Mensagem = "Atenção! 1 e/ou mais, Planos/Produtos programado(s), em alguma venda em aberto para este cliente, para mudança de plano!\r\n \r\nPor favor! Faça o desvinculamento, e depois, tente novamente!";
                            return retorno;
                        }

                        if (listaPlanoProduto.Any(x => x.Status == Status_PlanoProduto.Aguardando_Aprovacao_Venda))
                        {
                            throw new Exception("Não foi possível efetuar o cancelamento do contrato, pois, existe 1 ou mais Planos/Produtos, AGUARDANDO APROVAÇÃO DE VENDA!!");
                        }

                        List<Cliente_PlanoProduto> listaAtualizarPlano = new List<Cliente_PlanoProduto>();
                        List<Cliente_PlanoProduto_StatusLog> listaAtualizaPlanoLog = new List<Cliente_PlanoProduto_StatusLog>();
                        List<Cliente_Contrato_StatusLog> listaAtualizaContratoLog = new List<Cliente_Contrato_StatusLog>();

                        Cliente_PlanoProduto atualizarPlano = new Cliente_PlanoProduto();
                        Cliente_PlanoProduto_StatusLog atualizaPlanoLog = new Cliente_PlanoProduto_StatusLog();

                        var modelo = listaPlanoProduto.Select(x => x.ClienteContrato).First();
                        ClienteContrato clienteContrato = modelo;
                        List<Cliente_Contrato_StatusLog> statusLogContrato = (await repoClienteContratoLog.select(p => p.ClienteContrato.Id == clienteContrato.Id)).ToList();
                        Cliente_Contrato_StatusLog statusAnteriorContLog = statusLogContrato.OrderBy(p => p.DataHoraAcao).LastOrDefault();
                        Status_Contrato? statusAnteriorContrato = statusAnteriorContLog?.NovoStatus;
                        Status_Contrato? statusContratoReativar = statusAnteriorContLog?.AntigoStatus;
                        List<ClienteContrato> listaClienteContratoCancelado = new List<ClienteContrato>();

                        foreach (var obj in listaPlanoProduto.Where(x => x.Status != Status_PlanoProduto.Venda_Cancelada &&
                                                                        x.Status != Status_PlanoProduto.Cancelado_Mudanca_Plano &&
                                                                        x.Status != Status_PlanoProduto.Cancelado && x.ClienteContrato.Id == modelo.Id))
                        {
                            List<Cliente_PlanoProduto_StatusLog> statusLogLista = (await repoStatusLog.select(p => p.PlanoProduto.Id == obj.Id)).ToList();
                            Cliente_PlanoProduto_StatusLog statusAnteriorLog = statusLogLista.OrderBy(p => p.DataHoraAcao).LastOrDefault();

                            if (value.reativar == true) //volta para o status anterior do contrato e também o plano produto vinculado
                            {

                                atualizaPlanoLog = new Cliente_PlanoProduto_StatusLog
                                {
                                    AntigoStatus = obj.Status,
                                    NovoStatus = statusAnteriorLog.AntigoStatus,
                                    PlanoProduto = obj,
                                    ClienteContrato = clienteContrato,
                                    DataHoraAcao = DateTime.Now

                                };

                                obj.Status = statusAnteriorLog?.AntigoStatus;
                                obj.Atualiza_Ola = true;


                                if (obj.Plano.AutenticacaoRadius)
                                {
                                    if (statusAnteriorLog?.AntigoStatus == Status_PlanoProduto.Habilitado)
                                    {
                                        using (var apiRadius = new IntegracaoRadiusApiController())
                                        {
                                            var response = await apiRadius.DesbloquearUsuario(new Sinapse.Integracao.Ativacao.Models.PlanoProdutoRadiusRequest
                                            {
                                                IdClientePlanoProduto = obj.Id
                                            });

                                            if (!responseRadius.result)
                                                responseRadius.message = "Status do plano foi alterado porém não foi possível desbloquear o Login.";
                                        }
                                    }
                                    else
                                    {
                                        using (var apiRadius = new IntegracaoRadiusApiController())
                                        {
                                            var response = await apiRadius.BloquearUsuario(new Sinapse.Integracao.Ativacao.Models.PlanoProdutoRadiusRequest
                                            {
                                                IdClientePlanoProduto = obj.Id
                                            });

                                            if (!responseRadius.result)
                                                responseRadius.message = "Status do plano foi alterado porém não foi possível bloquear o Login.";
                                        }
                                    }
                                }

                                if (statusContratoReativar == null)
                                {
                                    statusContratoReativar = Status_Contrato.Ativo;
                                }

                                clienteContrato.Status_Contrato = statusContratoReativar;
                                statusAnteriorContrato = statusAnteriorContLog.AntigoStatus;

                                obj.ClienteContrato = clienteContrato;
                                atualizarPlano = obj;

                                listaAtualizarPlano.Add(atualizarPlano);
                                listaAtualizaPlanoLog.Add(atualizaPlanoLog);
                                //listaClienteContrato.Add(clienteContrato);
                            }
                            else if ((Status_Contrato)value.status == Status_Contrato.Cancelado) //Contrato cancelado, muda todos os planos para cancelado
                            {
                                Status_PlanoProduto? statusAnterior = statusAnteriorLog?.NovoStatus;

                                atualizaPlanoLog = new Cliente_PlanoProduto_StatusLog
                                {
                                    AntigoStatus = statusAnterior,
                                    NovoStatus = Status_PlanoProduto.Cancelado,
                                    PlanoProduto = obj,
                                    ClienteContrato = clienteContrato,
                                    DataHoraAcao = DateTime.Now
                                };

                                obj.Status = Status_PlanoProduto.Cancelado;
                                obj.Motivo_Cancelamento = await repoMotivoCanc.selectFirst(p => p.Id == value.idMotivo_cancelamento);
                                clienteContrato.Status_Contrato = Status_Contrato.Cancelado;

                                obj.Atualiza_Ola = true;

                                using (var apiRadius = new IntegracaoRadiusApiController())
                                {
                                    var response = await apiRadius.BloquearUsuario(new Sinapse.Integracao.Ativacao.Models.PlanoProdutoRadiusRequest
                                    {
                                        IdClientePlanoProduto = obj.Id
                                    });

                                    if (!responseRadius.result)
                                        responseRadius.message = "Status plano foi alterado porém não foi possível bloquear o Login.";
                                }

                                if (obj.Plano.Tipo_Tecnologia == Tipo_Tecnologia.Servicos_Adicionais &&
                                                !String.IsNullOrEmpty(obj.Plano.ProductId))
                                {
                                    IIntegracaoExternaRepository repoIntegracaoExterna = new IntegracaoExternaRepository(session);
                                    var integracaoExternaBusiness = new IntegracaoExternaBusiness(repoIntegracaoExterna);

                                    IntegracoesExternas dadosIntegracaoPlayhub =
                                        integracaoExternaBusiness.GetIntegracaoExterna(EnumTipoIntegracao.OUTROS, IntegracaoExterna.PLAYHUB);

                                    if (dadosIntegracaoPlayhub != null)
                                    {
                                        if (String.IsNullOrEmpty(dadosIntegracaoPlayhub.ApiKey) ||
                                            String.IsNullOrEmpty(dadosIntegracaoPlayhub.ApiSecret))
                                        {
                                            throw new Exception("Integração com a API do Playhub não configurada corretamente");
                                        }

                                        IPlayhubRepository repoPlayhub = new PlayhubApiRepository(dadosIntegracaoPlayhub.ApiKey, dadosIntegracaoPlayhub.ApiSecret);
                                        var playhubBusiness = new PlayhubBusiness(repoPlayhub);

                                        if (!String.IsNullOrEmpty(obj.Cliente.UsuarioPlayhub))
                                        {
                                            playhubBusiness.RemoverPlanoUsuario(obj.Cliente.UsuarioPlayhub, obj.Plano.ProductId);
                                        }
                                    }
                                }

                                obj.ClienteContrato = clienteContrato;
                                atualizarPlano = obj;

                                listaAtualizarPlano.Add(atualizarPlano);
                                listaAtualizaPlanoLog.Add(atualizaPlanoLog);
                                listaClienteContratoCancelado.Add(clienteContrato);
                            }
                            else if ((Status_Contrato)value.status == Status_Contrato.Suspenso_Empresa) //Contrato suspenso. muda todos os planos para suspenso empresa
                            {
                                Status_PlanoProduto? statusAnterior = statusAnteriorLog?.NovoStatus;

                                atualizaPlanoLog = new Cliente_PlanoProduto_StatusLog
                                {
                                    AntigoStatus = statusAnterior,
                                    NovoStatus = Status_PlanoProduto.Suspenso_Empresa,
                                    PlanoProduto = obj,
                                    ClienteContrato = clienteContrato,
                                    DataHoraAcao = DateTime.Now
                                };

                                if (value.idMotivo_suspensao_contrato > 0)
                                {
                                    obj.Motivo_Suspensao = await repoMotivoSuspensao.selectFirst(p => p.Id == value.idMotivo_suspensao_contrato);
                                }
                                else
                                {
                                    var motivo = await repoMotivoSuspensao.selectFirst(p => p.Motivo_Para == Status_Motivo_Para.Plano_Produto_Contrato);
                                    if (motivo == null)
                                    {
                                        motivo = new Motivo_Suspensao_Contratual()
                                        {
                                            Observacao_Motivo_Suspensao_Plano_Cliente_Outros = string.Concat(value.observacao_motivo_suspensao, " - opção OUTRO"),
                                            Motivo_Para = Status_Motivo_Para.Plano_Produto_Contrato,
                                            DataHoraCriacao = DateTime.Now,
                                            UsuarioCriacao= User.Identity.GetUserId(),
                                            Status = Status_Default.Ativo
                                        };
                                        await repoMotivoSuspensao.SaveOrUpdate(motivo);
                                    }
                                    else
                                    {
                                        motivo.Observacao_Motivo_Suspensao_Plano_Cliente_Outros = string.Concat(value.observacao_motivo_suspensao, " - opção OUTRO");
                                    }
                                    obj.Motivo_Suspensao = motivo;
                                }

                                obj.Status = Status_PlanoProduto.Suspenso_Empresa;
                                clienteContrato.Status_Contrato = Status_Contrato.Suspenso_Empresa;

                                obj.Atualiza_Ola = true;

                                using (var apiRadius = new IntegracaoRadiusApiController())
                                {
                                    var response = await apiRadius.BloquearUsuario(new Sinapse.Integracao.Ativacao.Models.PlanoProdutoRadiusRequest
                                    {
                                        IdClientePlanoProduto = obj.Id
                                    });

                                    if (!responseRadius.result)
                                        responseRadius.message = "Status do plano foi alterado porém não foi possível bloquear o Login.";
                                }
                                obj.ClienteContrato = clienteContrato;
                                atualizarPlano = obj;

                                listaAtualizarPlano.Add(atualizarPlano);
                                listaAtualizaPlanoLog.Add(atualizaPlanoLog);
                                //listaClienteContrato.Add(clienteContrato);
                            }

                            listaAtualizaContratoLog.Add(new Cliente_Contrato_StatusLog
                            {
                                AntigoStatus = statusAnteriorContLog?.NovoStatus,
                                NovoStatus = clienteContrato.Status_Contrato,
                                ClienteContrato = clienteContrato,
                                DataHoraAcao = DateTime.Now
                            });
                        }

                        //await repoClienteContrato.SaveOrUpdate(listaClienteContrato);
                        await repoClienteContratoLog.SaveOrUpdate(listaAtualizaContratoLog);
                        await repoPlanoProduto.SaveOrUpdate(listaAtualizarPlano);
                        await repoStatusLog.SaveOrUpdate(listaAtualizaPlanoLog);

                        foreach (var item in listaClienteContratoCancelado.Distinct().ToList())
                        {
                            var valorMulta = await GetMultaCarencia(item, DateTime.Today, repoStatusLog);
                            if (valorMulta > 0)
                            {
                                var cobrancaMulta = new CobrancaAdicional
                                {
                                    Cliente = item.Cliente,
                                    Motivo = MotivoCobrancaAdicional.Multa,
                                    Valor = valorMulta,
                                    DataCobranca = DateTime.Today,
                                    ClienteContrato = item,
                                    UsuarioCriacao = User.Identity.GetUserId(),
                                };


                                await repoCobrancaAdicional.Save(cobrancaMulta);
                            }
                        }

                        //Se o Radius não conseguir desbloquear/bloquear o login com sucesso, 
                        //permite realizar a mudança de status mas traz notificação do ocorrido.
                        if (!responseRadius.result)
                            retorno.Mensagem = responseRadius.message;

                        retorno.Ok = true;

                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }

        [HttpPost()]
        [Route("api/clienteContratoapi/GravarStatusContrato")]
        public async Task<GetApiRetorno<Int64>> GravarStatusContrato(AlterarStatusContratoViewModel value)
        {
            GetApiRetorno<Int64> retorno = new GetApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repoPlanoProduto = new Conexao<Cliente_PlanoProduto>(session))
                    using (var repoStatusLog = new Conexao<Cliente_PlanoProduto_StatusLog>(session))
                    using (var repoClienteContratoLog = new Conexao<Cliente_Contrato_StatusLog>(session))
                    using (var repoMotivoCanc = new Conexao<Motivo_Cancelamento_PlanoProduto>(session))
                    using (var repoClienteContrato = new Conexao<ClienteContrato>(session))
                    {
                        ClienteContrato contrato = await repoClienteContrato.selectFirst(p => p.Id == value.id);
                        ClienteContrato contratoModelo = contrato;

                        var listaPlanoProduto = await repoPlanoProduto.select(p => p.ClienteContrato.Id == value.id);

                        //Busca Log de Contrato
                        List<Cliente_Contrato_StatusLog> statusLogContrato = (await repoClienteContratoLog.select(p => p.ClienteContrato.Id == contrato.Id)).ToList();
                        Cliente_Contrato_StatusLog statusAnteriorContLog = statusLogContrato.OrderBy(p => p.DataHoraAcao).LastOrDefault();
                        Status_Contrato? statusAnteriorContrato = statusAnteriorContLog?.NovoStatus;

                        //Reativar Contrato
                        if (value.reativar == true)
                        {
                            if (statusAnteriorContrato == null)
                            {
                                contrato.Status_Contrato = Status_Contrato.Sem_Status;
                            }
                            else
                            {
                                contrato.Status_Contrato = statusAnteriorContrato;
                            }
                        }

                        else
                        {

                            //1: Aguardando Ativação
                            if (listaPlanoProduto.Any(l => l.Status == Status_PlanoProduto.Aguardando_Criacao_OS)
                            || listaPlanoProduto.Any(l => l.Status == Status_PlanoProduto.Aguardando_Instalacao)
                            || listaPlanoProduto.Any(l => l.Status == Status_PlanoProduto.Aguardando_Aprovacao_Venda))
                            {
                                contrato.Status_Contrato = Status_Contrato.Aguardando_Ativacao;
                            }

                            //2: Habilitado
                            else if (listaPlanoProduto.All(l => l.Status == Status_PlanoProduto.Habilitado))
                            {
                                contrato.Status_Contrato = Status_Contrato.Ativo;
                            }

                            //3: Suspenso Empresa
                            else if (listaPlanoProduto.All(l => l.Status == Status_PlanoProduto.Suspenso_Empresa))
                            {
                                contrato.Status_Contrato = Status_Contrato.Suspenso_Empresa;
                            }

                            //4: Cancelado
                            else if (listaPlanoProduto.Any(l => l.Status == Status_PlanoProduto.Cancelado)
                                || listaPlanoProduto.Any(l => l.Status == Status_PlanoProduto.Venda_Cancelada)
                                || listaPlanoProduto.Any(l => l.Status == Status_PlanoProduto.Cancelado_Mudanca_Plano))
                            {

                                contrato.Status_Contrato = Status_Contrato.Cancelado;
                            }

                        }

                        //Salva Log do Contrato
                        var atualizaContratoLog = new Cliente_Contrato_StatusLog
                        {
                            AntigoStatus = statusAnteriorContrato,
                            NovoStatus = contrato.Status_Contrato,
                            ClienteContrato = contrato,
                            DataHoraAcao = DateTime.Now
                        };

                        if (contratoModelo.Status_Contrato != contrato.Status_Contrato)
                        {
                            await repoClienteContratoLog.SaveOrUpdate(atualizaContratoLog);
                            await repoClienteContrato.SaveOrUpdate(contrato);
                        }

                        retorno.Ok = true;

                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;
                retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }

        public async Task<Double> GetMultaCarencia(ClienteContrato CliContrato, DateTime dtCancelamento, Conexao<Cliente_PlanoProduto_StatusLog> repoStatusLog)
        {
            // se o plano chegou ate aqui, tem fatura para cobrar

            var avaliarModeloContrato = CliContrato.ModeloContrato;

            Double valCarencia = 0;
            if (avaliarModeloContrato.Tem_Carencia && CliContrato.Status_Contrato == Status_Contrato.Cancelado)
            {
                if (!avaliarModeloContrato.Tem_Pro_Rata)
                    valCarencia = Convert.ToDouble(String.Format("{0:N2}", avaliarModeloContrato.Multa_Carencia));
                else
                {
                    DateTime? dataCalcularCarencia = null;
                    if (CliContrato.DataAceiteContrato.HasValue)
                        dataCalcularCarencia = CliContrato.DataAceiteContrato.Value;
                    else if (CliContrato.DataHoraCriacao.HasValue)
                        dataCalcularCarencia = CliContrato.DataHoraCriacao.Value;
                    else
                        return 0;

                    var dtCarencia = dataCalcularCarencia.Value.AddMonths(Convert.ToInt16(avaliarModeloContrato.Periodo_Carencia));

                    if (dtCarencia.Date > dtCancelamento.Date)
                    {
                        var valorDia = Convert.ToDouble(avaliarModeloContrato.Multa_Carencia / (dtCarencia.Date - dataCalcularCarencia.Value.Date).TotalDays);

                        var diasDiferenca = (dtCarencia.Date - dtCancelamento.Date).TotalDays;

                        valCarencia = Convert.ToDouble(String.Format("{0:N2}", diasDiferenca * valorDia));
                    }
                }

            }

            return valCarencia;
        }

        [HttpPost]       
        [Route("api/clienteContratoapi/DesbloqueioConfianca")]
        public async Task<Response> DesbloqueioConfianca(DesbloqueioConfiancaRequest request)
        {
            var response = new Response();

            try
            {
                //using (ISession sessionRadius = HibernateUtil.GetSessionRadius())
                using (ISession sessionConnect = HibernateUtil.GetSession())
                using (var repoLoginRadius = new Conexao<LoginRadius>(sessionConnect))
                using (var repoClientePlanoProduto = new Conexao<Cliente_PlanoProduto>(sessionConnect))
                using (var repoClienteContrato = new Conexao<ClienteContrato>(sessionConnect))
                {
                    if (request.IdClienteContrato == null || request.IdClienteContrato == 0)
                    {
                        throw new EAnielException("Contrato não encontrado para este cliente");
                    }

                    var cliente_Contrato = await repoClienteContrato.selectFirst(p => p.Id == request.IdClienteContrato);

                    if (cliente_Contrato == null)
                    {
                        throw new EAnielException("Contrato não encontrado para este cliente");
                    }

                    if (cliente_Contrato.UtilizaDesbloqueioConfianca)
                    {
                        if (cliente_Contrato.Status_Contrato != Status_Contrato.Suspenso_Debito)
                        {
                            throw new EAnielException("Os planos deste contrato não se encontram suspensos por falta de pagamento, por este motivo não é possível realizar o desbloqueio em confiança.");
                        }

                        if (cliente_Contrato.Data_Desbloqueio_Confianca.HasValue)
                        {
                            var periodicidadeDesbloqueio = Convert.ToDouble(cliente_Contrato.QtdDiasDesbloqueados);
                            var qtdDiasDesbloqueio = DateTime.Now.Date - cliente_Contrato.Data_Desbloqueio_Confianca.Value;

                            if (qtdDiasDesbloqueio.TotalDays <= periodicidadeDesbloqueio)
                                throw new EAnielException("Já foi efetuado um desbloqueio em confiança recentemente, tente novamente em alguns dias.");
                        }

                        cliente_Contrato.Status_Contrato = Status_Contrato.Ativo;
                        cliente_Contrato.Data_Desbloqueio_Confianca = DateTime.Now.Date;

                        var listaPlanosContrato = (await repoClientePlanoProduto.select(p => p.ClienteContrato.Id == request.IdClienteContrato)).ToList();
                        if (listaPlanosContrato != null && listaPlanosContrato.Count > 0)
                        {
                            foreach (var cliente_PlanoProduto in listaPlanosContrato)
                            {
                                cliente_PlanoProduto.Status = Sinapse.Model.enumerados.Status_PlanoProduto.Habilitado;
                                cliente_PlanoProduto.Data_Desbloqueio_Confianca = DateTime.Now.Date;
                                await repoClientePlanoProduto.SaveOrUpdate(cliente_PlanoProduto);

                                if (cliente_PlanoProduto.Plano.AutenticacaoRadius)
                                {
                                    using (var apiRadius = new IntegracaoRadiusApiController())
                                    {
                                        var responseDesbloqUsuario = await apiRadius.DesbloquearUsuario(new Sinapse.Integracao.Ativacao.Models.PlanoProdutoRadiusRequest
                                        {
                                            IdClientePlanoProduto = cliente_PlanoProduto.Id
                                        });

                                        if (!responseDesbloqUsuario.result)
                                            response.message = "Status do plano foi alterado porém não foi possível desbloquear o Login.";
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new EAnielException("Não é possível realizar o desbloqueio em confiança pois não existem planos vigentes para este contrato.");
                        }

                        await repoClienteContrato.SaveOrUpdate(cliente_Contrato);
                    }
                    else
                    {
                        throw new EAnielException("Não é possível realizar o procedimento pois este plano não utiliza desbloqueio em confiança.");
                    }

                    response.result = true;
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
                response.result = false;
                response.message = ex.Message;
                response.stacktrace = ex is EAnielException ? null : ex.StackTrace;
            }

            return response;
        }

        #region Mobile
        [HttpPost]
        [AllowAnonymous]
        [OlaAuthorize]
        [Route("api/clienteContratoapi/DesbloqueioConfiancaOla")]
        public async Task<Response> DesbloqueioConfiancaOla(DesbloqueioConfiancaRequest request)
        {
            return await DesbloqueioConfianca(request);
        }
        #endregion
    }

    //[HttpGet]
    //[Route("api/ClienteContratoApi/GetPdf")]
    //public async Task<GetApiRetorno<ArquivoClienteContrato>> GetPdf(long idContrato)
    //{
    //    var retorno = new GetApiRetorno<ArquivoClienteContrato>();

    //    try
    //    {
    //        byte[] arquivoContrato = null;
    //        string descricaoContrato = null;

    //        using (ISession session = HibernateUtil.GetSession())
    //        using (var com = session.Connection.CreateCommand())
    //        {
    //            var sql = new StringBuilder();
    //            var param = new Dictionary<string, object>();

    //            sql.AppendLine("SELECT CT.DESCRICAO, CT.ARQUIVO");
    //            sql.AppendLine("FROM TB_CLIENTE_CONTRATO CT");
    //            sql.AppendLine("WHERE CT.ID = @ID_CONTRATO_CLIENTE");

    //            param.Add("@ID_CONTRATO_CLIENTE", idContrato);

    //            using (var reader = await CommandHelper.ExecuteReader(com, sql, param, session.Connection, null))
    //            {
    //                if (reader.Read())
    //                {
    //                    arquivoContrato = (byte[])reader["ARQUIVO"];
    //                    descricaoContrato = Convert.ToString(reader["DESCRICAO"]);
    //                }
    //            }
    //        }

    //        if (arquivoContrato != null)
    //        {
    //            string pastaTmp = $@"{CONSTANTES.CaminhoFisico}\Arquivos\Tmp";

    //            if (!Directory.Exists(pastaTmp))
    //                Directory.CreateDirectory(pastaTmp);

    //            string tmpDocxPath = $@"{pastaTmp}\{Guid.NewGuid()}.docx";
    //            File.WriteAllBytes(tmpDocxPath, arquivoContrato);

    //            string tmpHtmlPath = $@"{pastaTmp}\{Guid.NewGuid()}.html";
    //            ConvertToPdfService.convertDocxToHtml(tmpHtmlPath, tmpDocxPath);

    //            string pdfFileName = $"{Guid.NewGuid()}.pdf";
    //            string pdfPath = $@"{pastaTmp}\{pdfFileName}";
    //            ConvertToPdfService.convertHtmlToPdf(pdfPath, tmpHtmlPath);

    //            File.Delete(tmpHtmlPath);
    //            File.Delete(tmpDocxPath);

    //            retorno.Ok = true;
    //            retorno.Objeto = new ArquivoClienteContrato
    //            {
    //                Descricao = descricaoContrato,
    //                UrlArquivo = $"{CONSTANTES.URLPortal}/Arquivos/Tmp/{pdfFileName}"
    //            };
    //        }
    //        else
    //        {
    //            retorno.Ok = false;
    //            retorno.Mensagem = "Arquivo do contrato não encontrado";
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        retorno.Mensagem = ex.Message;
    //        retorno.InnerException = ex.InnerException != null ? ex.InnerException.Message : String.Empty;
    //    }

    //    return retorno;
    //}
}
