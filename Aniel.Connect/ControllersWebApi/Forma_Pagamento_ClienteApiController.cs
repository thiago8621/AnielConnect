using Aniel.Connect.Core;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.ModelLog.nhibernate;
using Microsoft.AspNet.Identity;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class Forma_Pagamento_ClienteApiController : BaseApiController
    {
        [HttpGet]
        [Route("api/Forma_Pagamento_Clienteapi/ExcluirGrid")]
        public async Task<DeleteApiRetorno<Int64>> ExcluirGrid(Int64 value, Int64? idVenda)
        {
            DeleteApiRetorno<Int64> retorno = new DeleteApiRetorno<Int64>();
            try
            {
                using (ISession session = HibernateUtil.GetSession())
                using (ISession sessionLog = HibernateUtil.GetSessionLog())
                {
                    using (var repository = new Conexao<Forma_Pagamento_Cliente>(session))
                    using (var repositoryVenda = new Conexao<Venda>(session))
                    using (var repoClientPlanPrd = new Conexao<Cliente_PlanoProduto>(session))
                    using (var repositoryLogs = new Conexao<LogVendas>(sessionLog))
                    using (var repositoryLogFormaPagamentoCliente = new Conexao<LogFormaPagamentoCliente>(sessionLog))
                    {
                        if (value != 0)
                        {
                            Venda modeloAnterior = null;
                            Venda venda = null;
                            Forma_Pagamento_Cliente modelo = (await repository.select(p => p.Id == value)).FirstOrDefault();

                            if (idVenda.HasValue)
                            {
                                venda = await repositoryVenda.selectFirst(x => x.Id == idVenda);
                                modeloAnterior = (Venda)venda.Clone();
                            }

                            if (modelo != null)
                            {
                                var hasPlanProd = await repoClientPlanPrd.selectFirst(c =>
                                    c.Cliente.Id == modelo.Cliente.Id &&
                                    c.Forma_Pagamento_Cliente.Id == modelo.Id);

                                var formaPagamentoVenda = (await repositoryVenda.select(v => v.FormaPagamentoCliente.Id == modelo.Id)).ToList();

                                if (hasPlanProd != null)
                                {
                                    retorno.Ok = false;
                                    retorno.Mensagem = "Não foi possível excluir a forma de pagamento, pois está atrelada a um Plano/Produto do Cliente.";
                                }
                                else if (formaPagamentoVenda != null && formaPagamentoVenda.Any(v => v.Id != idVenda))
                                {
                                    retorno.Ok = false;
                                    retorno.Mensagem = $"Não foi possível excluir a forma de pagamento, pois está atrelada a venda {formaPagamentoVenda.FirstOrDefault(p => p.Id != idVenda)?.Id}";
                                }
                                else
                                {
                                    venda.FormaPagamentoCliente = null;
                                    await repositoryVenda.Update(venda);

                                    await repository.Remove(modelo);
                                    retorno.IdObjeto = modelo.Id;
                                    retorno.Ok = true;

                                    if (venda != null)
                                    {
                                        if (venda.Formas_Pagamento == null)
                                            venda.Formas_Pagamento = new List<Forma_Pagamento_ClienteModel>();

                                        venda.Formas_Pagamento.Add(new Forma_Pagamento_ClienteModel
                                        {
                                            Id = value,
                                            Id_Forma_Pagamento = modelo.Forma_Pagamento.Id,
                                            Desc_Forma_Pagamento = modelo.Forma_Pagamento.Descricao,
                                        });

                                        var logs = venda.GeraLog(modeloAnterior, User.Identity.GetUserId(), User.Identity.GetUserName(), Sinapse.ModelLog.TipoOperLog.Exclusao);
                                        await repositoryLogs.Save(logs);
                                        await repositoryLogFormaPagamentoCliente.SaveOrUpdate(GetObjLog(modelo));
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new Exception(Resources.RsBase.NAOENCONTRADO);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.IdObjeto = 0;
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }

        private LogFormaPagamentoCliente GetObjLog(Forma_Pagamento_Cliente obj)
        {
            return new LogFormaPagamentoCliente
            {
                Id_Forma_Pagamento_Cliente = obj.Id,
                Id_Cliente = obj.Cliente?.Id,
                Id_Forma_Pagamento = obj.Forma_Pagamento?.Id,
                Id_Banco = obj.Banco?.Id,
                Conta_Corrente = obj.Conta_Corrente,
                Agencia = obj.Agencia,
                Digito = obj.Digito,
                Id_Vencimento = obj.Vencimento?.Id,
                Dia_Vencimento = obj.Dia_Vencimento,
                Forma_Envio = (int?)obj.Forma_Envio,
                Id_Endereco_Cobranca = obj.Endereco_Cobranca?.Id,
                End_Cobranca_Igual_Principal = obj.End_Cobranca_Igual_Principal,
                Status = (int)obj.Status,
                Origem_Mobile = obj.Origem_Mobile,
                Alterado = obj.Alterado,
                StatusValidoCartao = (int?)obj.StatusValidoCartao,

                Token_Cartao = obj.Token_Cartao,
                DataHoraCriacao = DateTime.Now
            };
        }

        [HttpGet]
        [Route("api/Forma_Pagamento_Clienteapi/GetListById")]
        public async Task<GetApiRetorno<List<Forma_Pagamento_Cliente>>> GetListById(Int64 id_cliente)
        {
            GetApiRetorno<List<Forma_Pagamento_Cliente>> retorno = new GetApiRetorno<List<Forma_Pagamento_Cliente>>();

            try
            {
                using (ISession session = HibernateUtil.GetSession())
                {
                    using (var repository = new Conexao<Forma_Pagamento_Cliente>(session))
                    {
                        var listaFormaPagamentoCliente = (await repository.select(f => f.Cliente.Id == id_cliente)).ToList();

                        if (listaFormaPagamentoCliente.Count > 0)
                            retorno.Objeto = listaFormaPagamentoCliente;
                        else
                            retorno.Objeto = new List<Forma_Pagamento_Cliente>();

                        retorno.Ok = true;
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Mensagem = ex.Message;
                if (ex.InnerException != null) retorno.InnerException = ex.InnerException.Message;
            }

            return retorno;
        }
    }
}