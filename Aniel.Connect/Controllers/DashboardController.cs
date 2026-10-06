using Aniel.Connect.ControllersWebApi;
using Aniel.Connect.Core.Service;
using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Models;
using Aniel.T5.Model.nhibernate;
using Microsoft.AspNet.Identity;
using NHibernate;
using Sinapse.DAO.repository;
using Sinapse.Model.enumerados;
using System;
using System.Text;
using System.Web.Mvc;

namespace Aniel.Connect.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {

        public ActionResult Vendas()
        {
            var Tipo = DashBoardService.GetPaginaInicial(User.Identity.GetUserId()).Result;
            using (var api = new Dashboard_VendasApiController()) 
            {
                if (Tipo == Sinapse.Model.enumerados.DashboardInicialConnect.Servicos)
                {

                    return RedirectToAction("Servicos", "DashBoard");

                }
                else if (Tipo == Sinapse.Model.enumerados.DashboardInicialConnect.ServicosModelo2)
                {
                    return RedirectToAction("ServicosModelo2", "DashBoard");
                }
                else if (Tipo == Sinapse.Model.enumerados.DashboardInicialConnect.AtendimentoCliente)
                {
                    return RedirectToAction("Atendimento", "DashBoard");
                }

                var visualizacao = api.GetFormatoVisualizacao().Result;
                var obj = new Dashboard_Vendas_Models();
                obj.VisualizacaoDashboard = (Enum_VisualizacaoDashboard)visualizacao.IdObjeto;
                return View(obj);
            }

        }

        public ActionResult Servicos()
        {
            var Tipo = DashBoardService.GetPaginaInicial(User.Identity.GetUserId()).Result;
            if (Tipo == Sinapse.Model.enumerados.DashboardInicialConnect.Comercial)
            {
                return RedirectToAction("Vendas", "DashBoard");
            }
            else if (Tipo == Sinapse.Model.enumerados.DashboardInicialConnect.AtendimentoCliente)
            {
                return RedirectToAction("Atendimento", "DashBoard");
            }
            else if (Tipo == Sinapse.Model.enumerados.DashboardInicialConnect.ServicosModelo2)
            {
                return RedirectToAction("ServicosModelo2", "Dashboard");
            }

            var api = new Dashboard_ServicosApiController();

            //var ProdutividadeMes = api.GetProdutividadeMes().Result.Objeto;
            //var ServicosForaPrazo = api.GetPctServicosForaPrazo().Result.Objeto;
            //var CustoMedio = api.GetCustoMedio().Result.Objeto;
            //var MediaAtraso = api.GetMediaAtraso().Result.Objeto;

            Dashboard_Servicos_Models obj = new Dashboard_Servicos_Models();

            //obj.Produtividade_Mes = ProdutividadeMes.Produtividade_Mes;
            //obj.Seta_Prod_Mes = ProdutividadeMes.Seta_Prod_Mes;

            //obj.Percentual_Servico_Fora_Prazo = ServicosForaPrazo.Percentual_Servico_Fora_Prazo;
            //obj.Seta_Pct_Servico_Fora_Prazo = ServicosForaPrazo.Seta_Pct_Servico_Fora_Prazo;

            //obj.Custo_Medio_Servico = CustoMedio.Custo_Medio_Servico;
            //obj.Seta_Custo_Medio_Servico = CustoMedio.Seta_Custo_Medio_Servico;

            //obj.Media_Atraso = MediaAtraso.Media_Atraso;
            //obj.Seta_Media_Atraso = MediaAtraso.Seta_Media_Atraso;

            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var com = sessionT5.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder(@"SELECT VALOR,DESCRICAO FROM TB_TIPO_HABILIDADE_NATUREZA ");


                using (var reader = CommandHelper.ExecuteReader(com, sql, sessionT5.Connection, null).Result)
                {
                    while (reader.ReadAsync().Result)
                    {

                        if (Convert.ToString(reader["VALOR"]) == "0")
                            ViewBag.DescricaoTipoHabilidade0 = Convert.ToString(reader["DESCRICAO"]);
                        if (Convert.ToString(reader["VALOR"]) == "1")
                            ViewBag.DescricaoTipoHabilidade1 = Convert.ToString(reader["DESCRICAO"]);
                        if (Convert.ToString(reader["VALOR"]) == "2")
                            ViewBag.DescricaoTipoHabilidade2 = Convert.ToString(reader["DESCRICAO"]);

                    }
                }


            }


            using (ISession session = HibernateUtil.GetSession())
            using (var repoConfig = new Conexao<ConfiguracoesDashboardServicos>(session))
            {
                var config = repoConfig.selectFirst().Result;
                obj.MetaBacklogReparo = Convert.ToString(config?.MetaBacklogReparo);
                obj.MetaBancoInstalacao = Convert.ToString(config?.MetaBancoInstalacao);
                obj.MetaEficienciaMesInstalacao = Convert.ToString(config?.MetaEficienciaMesInstalacao);
                obj.MetaEficienciaMesReparo = Convert.ToString(config?.MetaEficienciaMesReparo);
                obj.MetaEficienciaDiaInstalacao = Convert.ToString(config?.MetaEficienciaDiaInstalacao);
                obj.MetaEficienciaDiaReparo = Convert.ToString(config?.MetaEficienciaDiaReparo);
                obj.MetaEficienciaDiaMultiskill = Convert.ToString(config?.MetaEficienciaDiaOutros);
                obj.MetaEficienciaDiaTotal = Convert.ToString(config?.MetaEficienciaDiaTotal);
                obj.MetaReclamacaoInicial = Convert.ToString(config?.MetaReclamacaoInicial);
                obj.MetaRepetidas = Convert.ToString(config?.MetaRepetidas);
                obj.MetaAtendimentoJanelaInstalacao = Convert.ToString(config?.MetaAtendimentoJanelaInstalacao);
                obj.MetaAtendimentoJanelaReparo = Convert.ToString(config?.MetaAtendimentoJanelaReparo);
                obj.MetaAtendimentoJanelaMultiskill = Convert.ToString(config?.MetaAtendimentoJanelaMultiskill);
                obj.MetaCustoMedioInstalacao = Convert.ToString(config?.MetaCustoMedioInstalacao);
                obj.MetaCustoMedioReparo = Convert.ToString(config?.MetaCustoMedioReparo);
            }

            obj.Chat = true;
            obj.Novidades_Sinapse = true;
            return View(obj);
        }

        [HttpPost()]
        public ActionResult Servicos(Dashboard_Servicos_Models model)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var repoConfig = new Conexao<ConfiguracoesDashboardServicos>(session))
            {
                var config = repoConfig.selectFirst().Result;

                if(config == null)
                {
                    config = new ConfiguracoesDashboardServicos();
                }

                config.MetaBacklogReparo = Convert.ToDouble(model?.MetaBacklogReparo);
                config.MetaBancoInstalacao = Convert.ToDouble(model?.MetaBancoInstalacao);
                config.MetaEficienciaMesInstalacao = Convert.ToDouble(model?.MetaEficienciaMesInstalacao);
                config.MetaEficienciaMesReparo = Convert.ToDouble(model?.MetaEficienciaMesReparo);
                config.MetaEficienciaDiaInstalacao = Convert.ToDouble(model?.MetaEficienciaDiaInstalacao);
                config.MetaEficienciaDiaReparo = Convert.ToDouble(model?.MetaEficienciaDiaReparo);
                config.MetaEficienciaDiaOutros = Convert.ToDouble(model?.MetaEficienciaDiaMultiskill);
                config.MetaEficienciaDiaTotal = Convert.ToDouble(model?.MetaEficienciaDiaMultiskill);
                config.MetaReclamacaoInicial = Convert.ToDouble(model?.MetaReclamacaoInicial);
                config.MetaRepetidas = Convert.ToDouble(model?.MetaRepetidas);
                config.MetaAtendimentoJanelaInstalacao = Convert.ToDouble(model?.MetaAtendimentoJanelaInstalacao);
                config.MetaAtendimentoJanelaReparo = Convert.ToDouble(model?.MetaAtendimentoJanelaReparo);
                config.MetaAtendimentoJanelaMultiskill = Convert.ToDouble(model?.MetaAtendimentoJanelaMultiskill);
                config.MetaCustoMedioInstalacao = Convert.ToDouble(model?.MetaCustoMedioInstalacao);
                config.MetaCustoMedioReparo = Convert.ToDouble(model?.MetaCustoMedioReparo);
                repoConfig.SaveOrUpdate(config).Wait();
                return RedirectToAction("Servicos", "Dashboard");
            }
        }

        public ActionResult Atendimento()
        {
            var Tipo = DashBoardService.GetPaginaInicial(User.Identity.GetUserId()).Result;
            if (Tipo == Sinapse.Model.enumerados.DashboardInicialConnect.Comercial)
            {
                return RedirectToAction("Vendas", "DashBoard");
            }
            else if (Tipo == Sinapse.Model.enumerados.DashboardInicialConnect.Servicos)
            {
                return RedirectToAction("Servicos", "DashBoard");
            }
            else if (Tipo == Sinapse.Model.enumerados.DashboardInicialConnect.ServicosModelo2)
            {
                return RedirectToAction("ServicosModelo2", "DashBoard");
            }

            var obj = new Dashboard_AtendimentoApiController().GetSLAGraphic().Result.Objeto;

            return View(obj);
        }

        public ActionResult ServicosModelo2()
        {
            var Tipo = DashBoardService.GetPaginaInicial(User.Identity.GetUserId()).Result;
            if (Tipo == Sinapse.Model.enumerados.DashboardInicialConnect.Comercial)
            {
                return RedirectToAction("Vendas", "DashBoard");
            }
            else if (Tipo == Sinapse.Model.enumerados.DashboardInicialConnect.AtendimentoCliente)
            {
                return RedirectToAction("Atendimento", "DashBoard");
            }
            else if (Tipo == Sinapse.Model.enumerados.DashboardInicialConnect.Servicos)
            {
                return RedirectToAction("Servicos", "Dashboard");
            }

            var api = new Dashboard_ServicosApiController();
            Dashboard_Servicos_Models obj = new Dashboard_Servicos_Models();

            using (ISession sessionT5 = HibernateUtil.GetSessionT5())
            using (var com = sessionT5.Connection.CreateCommand())
            {
                StringBuilder sql = new StringBuilder(@"SELECT VALOR,DESCRICAO FROM TB_TIPO_HABILIDADE_NATUREZA ");


                using (var reader = CommandHelper.ExecuteReader(com, sql, sessionT5.Connection, null).Result)
                {
                    while (reader.ReadAsync().Result)
                    {

                        if (Convert.ToString(reader["VALOR"]) == "0")
                            ViewBag.DescricaoTipoHabilidade0 = Convert.ToString(reader["DESCRICAO"]);
                        if (Convert.ToString(reader["VALOR"]) == "1")
                            ViewBag.DescricaoTipoHabilidade1 = Convert.ToString(reader["DESCRICAO"]);
                        if (Convert.ToString(reader["VALOR"]) == "2")
                            ViewBag.DescricaoTipoHabilidade2 = Convert.ToString(reader["DESCRICAO"]);

                    }
                }


            }


            using (ISession session = HibernateUtil.GetSession())
            using (var repoConfig = new Conexao<ConfiguracoesDashboardServicosModelo2>(session))
            using (var repoPeriodo = new Conexao<Horario_Connect>(session))
            {
                ViewBag.Periodo = repoPeriodo.select().Result.Count;
                var config = repoConfig.selectFirst().Result;
                obj.MetaBacklogReparo = Convert.ToString(config?.MetaBacklogReparo);
                obj.MetaBancoInstalacao = Convert.ToString(config?.MetaBancoInstalacao);
                obj.MetaEficienciaMesInstalacao = Convert.ToString(config?.MetaEficienciaMesInstalacao);
                obj.MetaEficienciaMesReparo = Convert.ToString(config?.MetaEficienciaMesReparo);
                obj.MetaEficienciaDiaInstalacao = Convert.ToString(config?.MetaEficienciaDiaInstalacao);
                obj.MetaEficienciaDiaReparo = Convert.ToString(config?.MetaEficienciaDiaReparo);
                obj.MetaEficienciaDiaMultiskill = Convert.ToString(config?.MetaEficienciaDiaOutros);
                obj.MetaEficienciaDiaTotal = Convert.ToString(config?.MetaEficienciaDiaTotal);
                obj.MetaReclamacaoInicial = Convert.ToString(config?.MetaReclamacaoInicial);
                obj.MetaRepetidas = Convert.ToString(config?.MetaRepetidas);
                obj.MetaAtendimentoJanelaInstalacao = Convert.ToString(config?.MetaAtendimentoJanelaInstalacao);
                obj.MetaAtendimentoJanelaReparo = Convert.ToString(config?.MetaAtendimentoJanelaReparo);
                obj.MetaAtendimentoJanelaMultiskill = Convert.ToString(config?.MetaAtendimentoJanelaMultiskill);
                obj.MetaCustoMedioInstalacao = Convert.ToString(config?.MetaCustoMedioInstalacao);
                obj.MetaCustoMedioReparo = Convert.ToString(config?.MetaCustoMedioReparo);
            }

            obj.Chat = true;
            obj.Novidades_Sinapse = true;
            return View(obj);
        }

        [HttpPost()]
        public ActionResult ServicosModelo2(Dashboard_Servicos_Models model)
        {
            using (ISession session = HibernateUtil.GetSession())
            using (var repoConfig = new Conexao<ConfiguracoesDashboardServicos>(session))
            {
                var config = repoConfig.selectFirst().Result;

                if (config == null)
                {
                    config = new ConfiguracoesDashboardServicos();
                }

                config.MetaBacklogReparo = Convert.ToDouble(model?.MetaBacklogReparo);
                config.MetaBancoInstalacao = Convert.ToDouble(model?.MetaBancoInstalacao);
                config.MetaEficienciaMesInstalacao = Convert.ToDouble(model?.MetaEficienciaMesInstalacao);
                config.MetaEficienciaMesReparo = Convert.ToDouble(model?.MetaEficienciaMesReparo);
                config.MetaEficienciaDiaInstalacao = Convert.ToDouble(model?.MetaEficienciaDiaInstalacao);
                config.MetaEficienciaDiaReparo = Convert.ToDouble(model?.MetaEficienciaDiaReparo);
                config.MetaEficienciaDiaOutros = Convert.ToDouble(model?.MetaEficienciaDiaMultiskill);
                config.MetaEficienciaDiaTotal = Convert.ToDouble(model?.MetaEficienciaDiaMultiskill);
                config.MetaReclamacaoInicial = Convert.ToDouble(model?.MetaReclamacaoInicial);
                config.MetaRepetidas = Convert.ToDouble(model?.MetaRepetidas);
                config.MetaAtendimentoJanelaInstalacao = Convert.ToDouble(model?.MetaAtendimentoJanelaInstalacao);
                config.MetaAtendimentoJanelaReparo = Convert.ToDouble(model?.MetaAtendimentoJanelaReparo);
                config.MetaAtendimentoJanelaMultiskill = Convert.ToDouble(model?.MetaAtendimentoJanelaMultiskill);
                config.MetaCustoMedioInstalacao = Convert.ToDouble(model?.MetaCustoMedioInstalacao);
                config.MetaCustoMedioReparo = Convert.ToDouble(model?.MetaCustoMedioReparo);
                repoConfig.SaveOrUpdate(config).Wait();
                return RedirectToAction("ServicosModelo2", "Dashboard");
            }
        }


    }
}