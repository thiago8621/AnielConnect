using Aniel.Connect.ConnectBusiness.Connect;
using Aniel.Connect.Core;
using Aniel.Connect.Core.Helpers;
using Aniel.Connect.Integracoes.SerasaExperian.Models;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.ConsultaCredito;
using Aniel.Connect.Repository.CriteriosParamCredito;
using Aniel.Connect.Repository.ParametrosConsultaCredito;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Util;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class ConsultaCreditoApiController : BaseApiController
    {
        [HttpGet]
        [Route("api/ConsultaCreditoApi/ExisteConsultaCredito")]
        public GetApiRetorno<ExisteConsultaCreditoResponse> ExisteConsultaCredito(string cpf = null, string cnpj = null)
        {
            var retorno = new GetApiRetorno<ExisteConsultaCreditoResponse>
            {
                Objeto = new ExisteConsultaCreditoResponse()
            };

            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                {
                    if (String.IsNullOrEmpty(cpf) && String.IsNullOrEmpty(cnpj))
                    {
                        throw new Exception("CPF/CNPJ não informado");
                    }

                    IConsultaCreditoRepository repoConsulta = new ConsultaCreditoRepository(sessionNh);
                    ICriteriosParamCreditoRepository repoCriterio = new CriteriosParamCreditoRepository(sessionNh);
                    IParametrosConsultaCreditoRepository repoParams = new ParametrosConsultaCreditoRepository(sessionNh);
                    IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(sessionNh);

                    var consultaCreditoBusiness =
                            new ConsultaCreditoBusiness(
                                repoConsulta,
                                repoCriterio,
                                repoParams,
                                repoConfig
                            );

                    if (!String.IsNullOrEmpty(cpf))
                    {
                        ConsultaSerasaDados consultaCredito = consultaCreditoBusiness.GetConsultaCreditoByCpf(cpf);

                        retorno.Objeto.ExisteConsulta = consultaCredito != null;
                        retorno.Objeto.IdConsultaCredito = consultaCredito?.Id;

                        if (retorno.Objeto.ExisteConsulta)
                            switch (consultaCredito.TipoIntegracao)
                            {
                                case IntegracaoExternaConsultaCredEnum.SERASA:
                                    retorno.Objeto.StatusConsulta = HtmlEnumExtensions.GetEnumDescription(consultaCreditoBusiness.GetStatusConsultaCreditoCpf(cpf));
                                    break;

                                case IntegracaoExternaConsultaCredEnum.PREQUAL:
                                    retorno.Objeto.StatusConsulta = HtmlEnumExtensions.GetEnumDescription(consultaCreditoBusiness.GetStatusConsultaCreditoCpfByConsultaPreQual(consultaCredito.Id, cpf));
                                    break;
                                case IntegracaoExternaConsultaCredEnum.SERASA_EXPERIAN:
                                    retorno.Objeto.StatusConsulta = HtmlEnumExtensions.GetEnumDescription(consultaCreditoBusiness.SerasaStatusRetorno(consultaCredito.consulta, consultaCredito.Id, cpf, true));
                                    break;
                            }
                    }
                    else
                    {
                        ConsultaSerasaDados consultaCredito = consultaCreditoBusiness.GetConsultaCreditoByCnpj(cnpj);
                        
                        retorno.Objeto.ExisteConsulta = consultaCredito != null;
                        retorno.Objeto.IdConsultaCredito = consultaCredito?.Id;
                        
                        if (retorno.Objeto.ExisteConsulta)
                            switch (consultaCredito.TipoIntegracao)
                            {
                                case IntegracaoExternaConsultaCredEnum.SERASA:
                                    retorno.Objeto.StatusConsulta = HtmlEnumExtensions.GetEnumDescription(consultaCreditoBusiness.GetStatusConsultaCreditoCnpj(cnpj));
                                    break;
                                case IntegracaoExternaConsultaCredEnum.PREQUAL:
                                    retorno.Objeto.StatusConsulta = HtmlEnumExtensions.GetEnumDescription(consultaCreditoBusiness.GetStatusConsultaCreditoCnpjByConsultaPreQual(consultaCredito.Id, cnpj));
                                    break;
                                case IntegracaoExternaConsultaCredEnum.SERASA_EXPERIAN:
                                    retorno.Objeto.StatusConsulta = HtmlEnumExtensions.GetEnumDescription(consultaCreditoBusiness.SerasaStatusRetorno(consultaCredito.consulta, consultaCredito.Id, cnpj, false));
                                    break;
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
            }

            return retorno;
        }

        
        private async Task<GetApiRetorno> PreencheScoreVendaNula()
        {
            var retorno = new GetApiRetorno();

            try
            {
                using (ISession sessionNh = HibernateUtil.GetSession())
                using (var repo = new Conexao<Venda>(sessionNh))
                {
                    var vendas = await repo.select(p => p.ScoreConsultaCredito == null && p.IdConsultaCredito != null);
                    IConsultaCreditoRepository repoConsulta = new ConsultaCreditoRepository(sessionNh);
                    foreach (var venda in vendas)
                    {
                        var consultaSerasa = repoConsulta.GetById(venda.IdConsultaCredito.Value).consulta;
                        var serasaResponse = JsonService.DeserializeObject<ConsultaSerasaResponse>(consultaSerasa);
                        if (serasaResponse.reports != null && serasaResponse.reports.FirstOrDefault()?.score != null)
                        {
                            venda.ScoreConsultaCredito = serasaResponse.reports.FirstOrDefault()?.score?.score;
                            await repo.Update(venda);
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
            }

            return retorno;
        }
    }
}
