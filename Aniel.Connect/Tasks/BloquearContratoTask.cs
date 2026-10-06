using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.ConnectBusiness.Financeiro;
using Aniel.Connect.ConnectBusiness.Playhub;
using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Feriado;
using Aniel.Connect.Repository.Financeiro.CobrancaFaturamento;
using Aniel.Connect.Repository.IntegracaoExterna;
using Aniel.Connect.Repository.Playhub;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Integracao.Ativacao.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aniel.Connect.Tasks
{
    public class BloquearContratoTask : BackgroundTask
    {
        public async Task<GetApiRetorno> BloquearContrato()
        {
            var retorno = new GetApiRetorno();

            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (ISession sessionSql = HibernateUtil.GetSession())
                {
                    ICobrancaFaturamentoRepository repoCobrancaFaturamento = new CobrancaFaturamentoRepository(sessionNh);
                    var cobrancaFaturamentoBusiness = new CobrancaFaturamentoBusiness(repoCobrancaFaturamento);

                    IIntegracaoExternaRepository repoIntegracaoExterna = new IntegracaoExternaRepository(sessionNh);
                    var integracaoExternaBusiness = new IntegracaoExternaBusiness(repoIntegracaoExterna);

                    IntegracoesExternas dadosIntegracaoPlayhub =
                        integracaoExternaBusiness.GetIntegracaoExterna(EnumTipoIntegracao.OUTROS, IntegracaoExterna.PLAYHUB);

                    PlayhubBusiness playhubBusiness = null;
                    bool usaIntegracaoPlayhub = dadosIntegracaoPlayhub != null;

                    if (usaIntegracaoPlayhub)
                    {
                        if (String.IsNullOrEmpty(dadosIntegracaoPlayhub.ApiKey) ||
                            String.IsNullOrEmpty(dadosIntegracaoPlayhub.ApiSecret))
                        {
                            throw new Exception("Integração com a API do Playhub não configurada corretamente");
                        }

                        IPlayhubRepository repoPlayhub =
                        new PlayhubApiRepository(
                            dadosIntegracaoPlayhub.ApiKey,
                            dadosIntegracaoPlayhub.ApiSecret
                        );

                        playhubBusiness = new PlayhubBusiness(repoPlayhub);
                    }

                    IEnumerable<CobrancaFaturamento> cobrancasVencidas = cobrancaFaturamentoBusiness.GetCobrancasVencidas();

                    foreach (CobrancaFaturamento cobranca in cobrancasVencidas)
                    {
                        bool bloqueioProcessado = false;

                        foreach (FaturamentoClientePlanos planosFaturamento in cobranca.FaturamentoCliente.Planos)
                        {
                            if (planosFaturamento.PlanoProduto != null &&
                                planosFaturamento.PlanoProduto.ClienteContrato?.ModeloContrato != null)
                            {
                                Modelo_Contrato modeloContrato = planosFaturamento.PlanoProduto.ClienteContrato?.ModeloContrato;

                                if ((DateTime.Now.Date - cobranca.vencimento.Date).Days > modeloContrato.BloquearNumeroDias)
                                {
                                    if (modeloContrato.ImpedirBloqueio_Automatico)
                                    {
                                        bool impedirBloqueioDia = false;

                                        IFeriadoRepository repoFeriado = new FeriadoRepository(sessionNh, sessionSql);
                                        var feriadoBusiness = new FeriadoBusiness(repoFeriado);

                                        bool isFeriado =
                                            feriadoBusiness
                                                .IsFeriado(
                                                    DateTime.Now,
                                                    planosFaturamento.PlanoProduto?.Endereco_Instalacao?.UF,
                                                    planosFaturamento.PlanoProduto?.Endereco_Instalacao?.Cidade
                                                );

                                        if (isFeriado)
                                        {
                                            impedirBloqueioDia = modeloContrato.ImpedirBloqueio_Feriados;
                                        }
                                        else
                                        {
                                            switch (DateTime.Now.DayOfWeek)
                                            {
                                                case DayOfWeek.Sunday:
                                                impedirBloqueioDia = modeloContrato.ImpedirBloqueio_Domingo;
                                                break;
                                                case DayOfWeek.Monday:
                                                impedirBloqueioDia = modeloContrato.ImpedirBloqueio_Segunda;
                                                break;
                                                case DayOfWeek.Tuesday:
                                                impedirBloqueioDia = modeloContrato.ImpedirBloqueio_Terca;
                                                break;
                                                case DayOfWeek.Wednesday:
                                                impedirBloqueioDia = modeloContrato.ImpedirBloqueio_Quarta;
                                                break;
                                                case DayOfWeek.Thursday:
                                                impedirBloqueioDia = modeloContrato.ImpedirBloqueio_Quinta;
                                                break;
                                                case DayOfWeek.Friday:
                                                impedirBloqueioDia = modeloContrato.ImpedirBloqueio_Sexta;
                                                break;
                                                case DayOfWeek.Saturday:
                                                impedirBloqueioDia = modeloContrato.ImpedirBloqueio_Sabado;
                                                break;
                                            }
                                        }

                                        // Caso as configurações do contrato impeçam o bloqueio no dia atual,
                                        // pulamos esse contrato e continuamos para o próximo
                                        if (impedirBloqueioDia)
                                        {
                                            continue;
                                        }
                                    }

                                    using (var apiRadius = new IntegracaoRadiusApiController())
                                    {
                                        await apiRadius.BloquearUsuario(new PlanoProdutoRadiusRequest
                                        {
                                            IdClientePlanoProduto = planosFaturamento.PlanoProduto.Id
                                        });
                                    }

                                    if (usaIntegracaoPlayhub)
                                    {
                                        if (planosFaturamento.PlanoProduto.Plano != null &&
                                            planosFaturamento.PlanoProduto.Plano.Tipo_Tecnologia == Tipo_Tecnologia.Servicos_Adicionais &&
                                            !String.IsNullOrEmpty(planosFaturamento.PlanoProduto.Plano.ProductId))
                                        {
                                            if (!String.IsNullOrEmpty(planosFaturamento.PlanoProduto.Cliente.UsuarioPlayhub))
                                            {
                                                playhubBusiness.RemoverPlanoUsuario(
                                                    planosFaturamento.PlanoProduto.Cliente.UsuarioPlayhub,
                                                    planosFaturamento.PlanoProduto.Plano.ProductId
                                                );
                                            }
                                        }
                                        else if (planosFaturamento.PlanoProduto.Combo != null)
                                        {
                                            // Caso haja um combo, cada plano configurado com o código do playhub dentro do combo
                                            // é removido do usuário no Playhub
                                            IEnumerable<ComboPlanoProduto> planosCombo =
                                                planosFaturamento
                                                    .PlanoProduto
                                                    .Combo
                                                    .PlanosProdutos
                                                    .Where(c => c.Plano != null &&
                                                                c.Plano.Tipo_Tecnologia == Tipo_Tecnologia.Servicos_Adicionais &&
                                                                !String.IsNullOrEmpty(c.Plano.ProductId));

                                            foreach (ComboPlanoProduto planoProdutoCombo in planosCombo)
                                            {
                                                playhubBusiness.AdicionarPlanoUsuario(
                                                    planosFaturamento.PlanoProduto.Cliente.UsuarioPlayhub,
                                                    planoProdutoCombo.Plano.ProductId
                                                );
                                            }
                                        }
                                    }

                                    if (!bloqueioProcessado)
                                        bloqueioProcessado = true;
                                }
                            }
                        }

                        if (bloqueioProcessado)
                        {
                            cobranca.Status = Cobranca_StatusPagamento.Cancelado;
                            cobrancaFaturamentoBusiness.Update(cobranca);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                retorno.Ok = false;
                retorno.Mensagem = ex.Message;

                if (ex.InnerException != null)
                    retorno.InnerException = ex.InnerException.Message;

                CONSTANTES.log4Net.Error(ex.Message, ex);
            }

            return retorno;
        }
    }
}
