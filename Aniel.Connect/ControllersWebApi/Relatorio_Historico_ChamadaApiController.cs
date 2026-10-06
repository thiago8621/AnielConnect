using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Integracoes.Saperx.Models;
using Aniel.Connect.Integracoes.Saperx.Services;
using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.ModelLog.nhibernate;
using Aniel.Connect.Models;
using NHibernate;
using NHibernate.Criterion;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class Relatorio_Historico_ChamadaApiController : BaseApiController
    {
        [HttpPost()]
        [Route("api/Relatorio_Historico_ChamadaApiController/Relatorio_Historico_Chamada")]
        public async Task<DataTableResponse<Relatorio_Historico_ChamadaModel>> Relatorio_Historico_Chamada(Relatorio_Historico_ChamadaRequest request)
        {
            try
            {
                using (var session = HibernateUtil.GetSession())
                using (var repository = new Conexao<Did>(session))
                using (var repositoryCircuito = new Conexao<CircuitoTelefonia>(session))
                using (var repositoryIntegracoesExternas = new Conexao<IntegracoesExternas>(session))
                using (var sessionLog = HibernateUtil.GetSessionLog())
                {

                    List<Relatorio_Historico_ChamadaModel> retorno = new List<Relatorio_Historico_ChamadaModel>();
                    var integracao = await repositoryIntegracoesExternas.selectFirst(p => p.IntegracaoExterna == IntegracaoExterna.SAPERX);
                    if (integracao != null)
                    {
                        BillingIntegracaoSaperxService billingIntegracaoSaperxService = new BillingIntegracaoSaperxService();
                        var meses = MonthsBetween(Convert.ToDateTime(request.dataini), Convert.ToDateTime(request.datafim));
                        List<Did> dids = new List<Did>();
                        if (request.idClientePlanoProduto.HasValue)
                        {
                            var circuitos = await repositoryCircuito.select(p => p.Clientes_PlanosProdutos.Id == request.idClientePlanoProduto);
                            dids.AddRange(circuitos.SelectMany(p => p.Did));
                        }
                        else
                        {
                            dids.Add(await repository.selectFirst(p => p.Id == Convert.ToInt64(request.id_did)));
                        }

                        foreach (var item in meses)
                        {
                            foreach (var did in dids)
                            {
                                var response = billingIntegracaoSaperxService.Billing(new BillingRequest { month_bill = item.Year + item.Month, id_sip = did.CodigoAuxiliar });
                                retorno.AddRange(response.Select(p => new Relatorio_Historico_ChamadaModel
                                {
                                    cidade = p.city,
                                    custoLigacao = p.sip_cost,
                                    didDestino = p.cdr.destiny,
                                    didOrigem = p.cdr.source,
                                    duracao = string.Format("{0:N2}", Convert.ToDouble(Convert.ToDouble(p.duration) / 60)),
                                    horaInicio = p.time_start.ToString("dd/MM/yyyy HH:mm"),
                                    horaFim = p.time_end.ToString("dd/MM/yyyy HH:mm"),
                                    numeroCircuito = p.sip.sip_number,
                                    data = p.time_start.Date.ToString("dd/MM/yyyy"),
                                    sentido = p.cdr.way != "S" ? "Entrada" : "Saída"
                                }).ToList());
                            }
                        }

                    }

                    if (String.IsNullOrEmpty(request.id_didDestino) == false)
                    {
                        retorno = retorno.Where(p => p.didDestino == request.id_didDestino.Replace("(", "").Replace(")", "").Replace("-", "")).ToList();
                    }
                    if (String.IsNullOrEmpty(request.id_didOrigem) == false)
                    {
                        retorno = retorno.Where(p => p.didOrigem == request.id_didOrigem.Replace("(", "").Replace(")", "").Replace("-", "")).ToList();
                    }



                    return new DataTableResponse<Relatorio_Historico_ChamadaModel>
                    {
                        data = retorno
                    };
                }
            }
            catch (Exception ex)
            {
                return new DataTableResponse<Relatorio_Historico_ChamadaModel>
                {

                };
            }
        }

        public static IEnumerable<(string Month, int Year)> MonthsBetween(
        DateTime startDate,
        DateTime endDate)
        {
            DateTime iterator;
            DateTime limit;

            if (endDate > startDate)
            {
                iterator = new DateTime(startDate.Year, startDate.Month, 1);
                limit = endDate;
            }
            else
            {
                iterator = new DateTime(endDate.Year, endDate.Month, 1);
                limit = startDate;
            }

            var dateTimeFormat = CultureInfo.CurrentCulture.DateTimeFormat;
            while (iterator <= limit)
            {
                yield return (
                    iterator.ToString("MM"),
                    iterator.Year
                );

                iterator = iterator.AddMonths(1);
            }
        }

    }
}