using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.T5.LimiteAgendamento;
using Aniel.Connect.Repository.T5.Natureza;
using Microsoft.AspNet.Identity;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{

    [Authorize]
    public class LimiteAgendamentoController : BaseController
    {

        public LimiteAgendamentoController() : base("LimiteAgendamento",
                                                    CONSTANTES.URLPortalInterno)
        {

        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Cadastro(string codEscritorio, string codArea, string data)
        {
            using (ISession sessionT5Sql = HibernateUtil.GetSessionT5())
            using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
            {
                ILimiteAgendamentoAnielRepository repoLimAgend = new LimiteAgendamentoRepository(sessionT5Sql, sessionT5Nh);
                INaturezaAnielRepository repoNatureza = new NaturezaAnielRepository(sessionT5Nh);

                var businessLimAgend = new LimiteAgendamentoAnielBusiness(repoLimAgend, repoNatureza);

                if (String.IsNullOrEmpty(codEscritorio) && String.IsNullOrEmpty(codArea) && String.IsNullOrEmpty(data))
                {
                    IEnumerable<LimiteAgendamentoPercentualNatureza> percNatureza = businessLimAgend.GetNaturezas();

                    return View(new LimiteAgendamentoModel
                    {
                        PercentualNatureza = percNatureza
                    });
                }
                else
                {
                    DateTime dataCadastro = Convert.ToDateTime(data);

                    IEnumerable<LimiteAgendamentoPercentualNatureza> percNatureza =
                        businessLimAgend.GetPercentuaisNaturezas(
                            codEscritorio, 
                            codArea,
                            dataCadastro
                        );

                    return View(new LimiteAgendamentoModel
                    {
                        CodEscritorio = codEscritorio,
                        CodArea = codArea,
                        Data = data,
                        PercentualNatureza = percNatureza
                    });
                } 
            }
        }

        [HttpPost]
        public ActionResult Cadastro(LimiteAgendamentoModel model)
        {
            using (ISession sessionT5Sql = HibernateUtil.GetSessionT5())
            using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
            using (ITransaction transNh = sessionT5Nh.BeginTransaction())
            using (ITransaction transSql = sessionT5Sql.BeginTransaction())
            {
                try
                {
                    ILimiteAgendamentoAnielRepository repoLimAgend = new LimiteAgendamentoRepository(sessionT5Sql, sessionT5Nh);
                    INaturezaAnielRepository repoNatureza = new NaturezaAnielRepository(sessionT5Nh);

                    var businessLimAgend = new LimiteAgendamentoAnielBusiness(repoLimAgend, repoNatureza);

                    businessLimAgend.Gravar(model, this.User.Identity.GetUserId());

                    transNh.Commit();
                    transSql.Commit();
                }
                catch (Exception ex)
                {
                    transNh.Rollback();
                    transSql.Rollback();

                    AddModelError(ex.Message, ex.InnerException?.Message);
                    return View(model);
                }

                return RedirectToAction("Index");
            }
        }
    }
}