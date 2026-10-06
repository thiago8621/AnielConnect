using Aniel.Connect.API_Aniel.Models.IManager;
using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using NHibernate;
using Sinapse.Core;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

public enum EStatusPortaAniel { Disponivel = 1, Reservada = 2, Vinculada = 3 }

namespace Aniel.Connect.API_Aniel.ControllersWebApi
{
    /// <summary>
    /// Endpoints de Integração com o IManager.
    /// </summary>
    public class IntegracaoIManagerApiController : BaseApiController
    {
        /// <summary>
        /// Endpoint para vincular/desvincular um contrato ou assinatura na CTO via Webhook iManager.
        /// </summary>
        [HttpPost()]
        [Route("api/IntegracaoIManagerApi/AtualizarCaixaPorta")]
        public async Task<PostApiRetorno> AtualizarCaixaPorta(ObjApiRequest<WebHookCTORequest> request)
        {
            var retorno = new PostApiRetorno();                      

            try
            {
                using (UsuarioApiController userApi = new UsuarioApiController())
                {
                    if (!(await userApi.GetUsuarioValido(request.settings)).Ok)
                    {
                        throw new UnauthorizedAccessException("Usuário e/ou Senha e/ou Token Inválidos");
                    }
                }
                
                if (request.Obj == null || request.Obj.contrato_assinatura == null || string.IsNullOrEmpty(request.Obj.contrato_assinatura.condicao))
                {
                    throw new ArgumentException("Parâmetros do Webhook inválidos ou ausentes.");
                }

                if (request.Obj.contrato_assinatura?.idassinatura.HasValue == false && request.Obj.contrato_assinatura?.idcontrato.HasValue == false)
                    throw new ArgumentException("Obrigatório informar um dos seguintes parâmetros: idassinatura ou idcontrato.");                

                if (!request.Obj.idportacto.HasValue)
                 throw new ArgumentException("Parâmetro idportacto não informado.");
                

                long? idPortaAniel = null;

                using (var session = HibernateUtil.GetSession())
                using (var sessionT5 = HibernateUtil.GetSessionT5())
                {
                    StringBuilder sql = new StringBuilder();
                    Dictionary<string, object> parametros = new Dictionary<string, object>();
                    sql.AppendLine("SELECT FIRST 1 ID FROM TB_ELEMENTO_REDE_PORTA WHERE COD_AUXILIAR = @COD_AUXILIAR");

                    parametros.Add("@COD_AUXILIAR", Convert.ToString(request.Obj.idportacto.Value));

                    using (var comm = sessionT5.Connection.CreateCommand())
                    using (var reader = await CommandHelper.ExecuteReader(comm, sql, parametros, sessionT5.Connection, null))
                    {
                        if(await reader.ReadAsync())
                        {
                            idPortaAniel = reader["ID"] != DBNull.Value ? Convert.ToInt64(reader["ID"]) : (long?)null;                            
                        }
                    }

                    if(!idPortaAniel.HasValue)
                        throw new EAnielException($"Porta não encontrada no Aniel. Id Porta IManager informada no WebHook: {request.Obj.idportacto}");

                    string condicao = request.Obj.contrato_assinatura.condicao.ToUpperInvariant();

                    sql.Clear();
                    parametros.Clear();

                    //Ocupação de Caixa/Porta
                    if (condicao == "RESERVADA" || condicao == "VINCULADA")
                    {
                        using(var repoLogin = new Conexao<LoginRadius>(session))
                        {
                            var login = await repoLogin.selectFirst(p => p.Login == request.Obj.contrato_assinatura.idcontrato.ToString() || p.CodigoAuxiliar == request.Obj.contrato_assinatura.idcontrato.ToString());
                            
                            //Cria o login se não existir no Aniel somente quando o WebHook enviar idContrato.
                            if (login == null && condicao == "VINCULADA")
                            {
                                using (var repoCliente = new Conexao<Cliente>(session))
                                using (var repoClientePlano = new Conexao<Cliente_PlanoProduto>(session))
                                using (var repoClienteContrato = new Conexao<ClienteContrato>(session))
                                using (var repoPlano = new Conexao<Plano>(session))
                                {
                                    #region Cliente
                                    string cpfcnpjTratado = request.Obj.contrato_assinatura.cpfcnpj.Replace("/", string.Empty).Replace("-", string.Empty).Replace(".", string.Empty);
                                    var clienteConnect = await repoCliente.selectFirst(p => p.CpfCnpj == cpfcnpjTratado);
                                   
                                    if(clienteConnect == null)
                                    {
                                        clienteConnect = new Cliente();
                                        clienteConnect.NomeFantasia = request.Obj.contrato_assinatura.nomecliente;
                                        clienteConnect.CpfCnpj = cpfcnpjTratado;
                                        clienteConnect.Status = Status_Cliente.Ativo;
                                        clienteConnect.Tipo_Cliente = Model.enumerado.Tipo_Cliente.Cliente;
                                        clienteConnect.Tipo_Pessoa = !string.IsNullOrEmpty(cpfcnpjTratado) ? cpfcnpjTratado?.Length > 11 ? Tipo_Pessoa.Juridica : Tipo_Pessoa.Fisica : (Tipo_Pessoa?)null;
                                        clienteConnect.UsuarioCriacao = "WEBHOOK IMANAGER";
                                        clienteConnect.DataHoraCriacao = DateTime.Now;

                                        await repoCliente.SaveOrUpdate(clienteConnect);
                                    }
                                    #endregion

                                    #region Cliente Contrato
                                    string contratoStr = request.Obj.contrato_assinatura.idcontrato.ToString();
                                    var clienteContrato = await repoClienteContrato.selectFirst(p => p.Cliente.Id == clienteConnect.Id && p.NumeroContrato == contratoStr);
                                    if (clienteContrato == null)
                                    {
                                        clienteContrato = new ClienteContrato
                                        {
                                            Cliente = clienteConnect,
                                            NumeroContrato = contratoStr,
                                            Descricao = contratoStr,
                                            UsuarioCriacao = "WEBHOOK IMANAGER",
                                            DataHoraCriacao = DateTime.Now
                                        };

                                        await repoClienteContrato.SaveOrUpdate(clienteContrato);
                                    }
                                    #endregion

                                    #region Cliente Plano/Produto
                                    var plano = await repoPlano.selectFirst(p => p.Descricao == "PLANO INTEGRACAO IMANAGER");
                                    if(plano == null)
                                    {
                                        plano = new Plano()
                                        {
                                            Descricao = "PLANO INTEGRACAO IMANAGER",                                            
                                            Tipo_Tecnologia = Model.enumerado.Tipo_Tecnologia.Internet,                                            
                                            SituacaoPlano = Model.enumerado.Situacao_plano.Ativo,
                                        };

                                        await repoPlano.SaveOrUpdate(plano);
                                    }

                                    var clientePlanoProduto = await repoClientePlano.selectFirst(p => p.Cliente.Id == clienteConnect.Id &&
                                                                      p.Plano == plano && p.Plano != null &&
                                                                      (p.Login_Radius == contratoStr || p.ClienteContrato.Id == clienteContrato.Id));

                                    if(clientePlanoProduto == null)
                                    {
                                        clientePlanoProduto = new Cliente_PlanoProduto();

                                        clientePlanoProduto.Cliente = clienteConnect;
                                        clientePlanoProduto.Plano = plano;
                                        clientePlanoProduto.Login_Radius = contratoStr; //Login será criado baseado no contrato abaixo
                                        clientePlanoProduto.ClienteContrato = clienteContrato;
                                        clientePlanoProduto.Status = Status_PlanoProduto.Habilitado;
                                        clientePlanoProduto.UsuarioCriacao = "WEBHOOK IMANAGER";
                                        clientePlanoProduto.DataHoraCriacao = DateTime.Now;

                                        await repoClientePlano.SaveOrUpdate(clientePlanoProduto);
                                    }

                                    #endregion

                                    #region Login Radius
                                    login = new LoginRadius();
                                    login.Status = Sinapse.Model.enumerados.Status_Default.Ativo;
                                    login.UsuarioCriacao = "WEBHOOK IMANAGER";
                                    login.DataHoraCriacao = DateTime.Now;
                                    login.Cliente = clienteConnect;
                                    login.Cliente_PlanoProduto = clientePlanoProduto;
                                    login.Login = request.Obj.contrato_assinatura.idcontrato.HasValue ? Convert.ToString(request.Obj.contrato_assinatura.idcontrato) : null;
                                    login.CodigoAuxiliar = request.Obj.contrato_assinatura.idcontrato.HasValue ? Convert.ToString(request.Obj.contrato_assinatura.idcontrato) : null;


                                    await repoLogin.SaveOrUpdate(login);
                                    #endregion
                                }                               
                            }     
                           
                            if(condicao == "VINCULADA")
                            {
                                //Checa se o login já está reservando/ocupando outra porta
                                long? idPortaAnterior = null;

                                sql.AppendLine("SELECT FIRST 1 ID FROM TB_ELEMENTO_REDE_PORTA WHERE ID_LOGIN = @ID_LOGIN");
                                parametros.Add("@ID_LOGIN", login.Id);

                                using (var comm = sessionT5.Connection.CreateCommand())
                                using (var reader = await CommandHelper.ExecuteReader(comm, sql, parametros, sessionT5.Connection, null))
                                {
                                    if (await reader.ReadAsync())
                                    {
                                        idPortaAnterior = reader["ID"] != DBNull.Value ? Convert.ToInt64(reader["ID"]) : (long?)null;
                                    }
                                }

                                //Limpa o vínculo do login com a porta anterior
                                if (idPortaAnterior.HasValue)
                                {
                                    var sucessoLimpeza = await this.LimparPorta(sessionT5, idPortaAnterior.Value);

                                    if (!sucessoLimpeza)
                                        throw new EAnielException($"Falha ao desocupar a porta Id Aniel {idPortaAnterior} que já estava vinculada anteriormente a este login. Nenhum registro atualizado.");
                                }
                            }                                                        

                            //Realiza ocupação da Porta
                            var situacaoPorta = condicao == "RESERVADA" ? Situacao_Porta.Reservado : Situacao_Porta.Ocupado;
                            var sucesso = await this.OcuparPorta(sessionT5, idPortaAniel.Value, situacaoPorta, login);

                            if(!sucesso)
                                throw new EAnielException($"Falha ao atualizar a porta, nenhum registro atualizado. Id Porta Aniel {idPortaAniel} / Id IManager {request.Obj.idportacto}");

                            retorno.Mensagem = $"{(situacaoPorta == Situacao_Porta.Ocupado ? "Ocupação" : "Reserva")} da porta realizada com sucesso.";
                        }
                    }
                    //Limpeza de Caixa/Porta
                    else if(condicao == "DESOCUPADA")
                    {
                        var sucesso = await this.LimparPorta(sessionT5, idPortaAniel.Value);

                        if(!sucesso)
                            throw new EAnielException($"Falha ao desocupar a porta, Nenhum registro atualizado. Id Porta Aniel {idPortaAniel} / Id IManager {request.Obj.idportacto}");

                        retorno.Mensagem = "Desocupação da porta realizada com sucesso.";
                    }
                    else
                    {
                        throw new NotSupportedException($"Condição '{condicao}' informada para a porta não suportada.");
                    }                        
                }

                retorno.Ok = true;
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net?.Error($"Erro ao processar Webhook IManager: {ex.Message}", ex);
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;                
                retorno.InnerException = ex.InnerException?.Message;
            }
            
            return retorno;
        }

        private async Task<bool> LimparPorta(ISession sessionT5, long idPortaAniel)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, object> parametros = new Dictionary<string, object>();

            sql.AppendLine("UPDATE TB_ELEMENTO_REDE_PORTA SET CLIENTE_PLANO_PRODUTO = NULL, ID_LOGIN = NULL, LOGIN = NULL, STATUS = 1, DATAHORAALTERACAO = CURRENT_TIMESTAMP WHERE ID = @ID_PORTA");
            parametros.Add("@ID_PORTA", idPortaAniel);

            var retorno = false;

            try
            {
                using (var comm = sessionT5.Connection.CreateCommand())
                {
                    var linhasAfetadas = 0;
                    linhasAfetadas = await CommandHelper.ExecuteNonQuery(comm, sql, parametros, sessionT5.Connection, null);

                    retorno = linhasAfetadas > 0;
                }
            }
            catch(Exception ex)
            {                
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }
            
            return retorno;
        }

        private async Task<bool> OcuparPorta(ISession sessionT5, long idPortaAniel, Situacao_Porta status, LoginRadius login)
        {
            StringBuilder sql = new StringBuilder();
            Dictionary<string, object> parametros = new Dictionary<string, object>();

            var retorno = false;

            //Se for reserva só trocamos o status da porta para Reservado.
            if(status == Situacao_Porta.Reservado)
            {
                sql.AppendLine("UPDATE TB_ELEMENTO_REDE_PORTA SET CLIENTE_PLANO_PRODUTO = NULL, ID_LOGIN = NULL, LOGIN = NULL, STATUS = @STATUS, DATAHORAALTERACAO = CURRENT_TIMESTAMP WHERE ID = @ID_PORTA");
            }

            //Se for ocupação segue o fluxo normal de vincular login com a CTO e status Ocupado.
            if(status == Situacao_Porta.Ocupado)
            {
                sql.AppendLine("UPDATE TB_ELEMENTO_REDE_PORTA SET CLIENTE_PLANO_PRODUTO = @ID_PLANO_PRODUTO, ID_LOGIN = @ID_LOGIN, LOGIN = @LOGIN, STATUS = @STATUS, DATAHORAALTERACAO = CURRENT_TIMESTAMP WHERE ID = @ID_PORTA");

                parametros.Add("@ID_PLANO_PRODUTO", login.Cliente_PlanoProduto?.Id);
                parametros.Add("@ID_LOGIN", login.Id);
                parametros.Add("@LOGIN", login.CodigoAuxiliar ?? login.Login);
            }
            
            parametros.Add("@ID_PORTA", idPortaAniel);            
            parametros.Add("@STATUS", (int)status);           
            
            try
            {
                using (var comm = sessionT5.Connection.CreateCommand())
                {
                    var linhasAfetadas = 0;
                    linhasAfetadas = await CommandHelper.ExecuteNonQuery(comm, sql, parametros, sessionT5.Connection, null);

                    retorno = linhasAfetadas > 0;
                }
            }
            catch (Exception ex)
            {
                CONSTANTES.log4Net.Error(ex.Message, ex);
            }
            return retorno;
        }
    }
}