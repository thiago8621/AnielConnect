using Aniel.Connect.ConnectBusiness.T5;
using Aniel.Connect.Core;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.T5.DocumentoProducao;
using Aniel.Connect.Repository.T5.ProdutividadeTecnico;
using NHibernate;
using Sinapse.DAO.repository;
using System.Web.Http;

namespace Aniel.Connect.ControllersWebApi
{
    [Authorize]
    public class RelatorioProdutividadeTecnicoApiController : BaseApiController
    {
        [HttpPost]
        [Route("api/RelatorioProdutividadeTecnicoApi/GetAll")]
        public DataTableResponse<RelatorioProdutividadeTecnicoListModel> GetAll(RelatorioProdutividadeTecnicoDataTableRequest request)
        {
            using (ISession sessionNh = HibernateUtil.GetSession())
            using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
            using (ISession sessionT5Sql = HibernateUtil.GetSessionT5())
            {
                IProdutividadeTecnicoRepository repoProdTecnico = new ProdutividadeTecnicoRepository(sessionT5Sql);
                IDocumentoProducaoAnielRepository repoDocProd = new DocumentoProducaoAnielRepository(sessionT5Nh, sessionT5Sql);
                IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(sessionNh);

                var businessProdutividadeTecnico =
                    new ProdutividadeTecnicoAnielBusiness(
                        repoProdTecnico,
                        repoDocProd,
                        repoConfig);

                RelatorioProdutividadeResponse relatorio = businessProdutividadeTecnico.GetRelatorio(request);

                return new DataTableResponse<RelatorioProdutividadeTecnicoListModel>
                {
                    draw = request.draw,
                    recordsFiltered = relatorio.Total,
                    data = relatorio.Data
                };
            }
        }

        [HttpPost]
        [Route("api/RelatorioProdutividadeTecnicoApi/GetAllPorMonitor")]
        public DataTableResponse<RelatorioProdutividadeTecnicoListModel> GetAllPorMonitor(RelatorioProdutividadeTecnicoDataTableRequest request)
        {
            using (ISession sessionNh = HibernateUtil.GetSession())
            using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
            using (ISession sessionT5Sql = HibernateUtil.GetSessionT5())
            {
                IProdutividadeTecnicoRepository repoProdTecnico = new ProdutividadeTecnicoRepository(sessionT5Sql);
                IDocumentoProducaoAnielRepository repoDocProd = new DocumentoProducaoAnielRepository(sessionT5Nh, sessionT5Sql);
                IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(sessionNh);

                var businessProdutividadeTecnico =
                    new ProdutividadeTecnicoAnielBusiness(
                        repoProdTecnico,
                        repoDocProd,
                        repoConfig);

                RelatorioProdutividadeResponse relatorio = businessProdutividadeTecnico.GetRelatorioPorMonitor(request);

                return new DataTableResponse<RelatorioProdutividadeTecnicoListModel>
                {
                    draw = request.draw,
                    recordsFiltered = relatorio.Total,
                    data = relatorio.Data
                };
            }
        }

        [HttpPost]
        [Route("api/RelatorioProdutividadeTecnicoApi/GetAllPorSupervisor")]
        public DataTableResponse<RelatorioProdutividadeTecnicoListModel> GetAllPorSupervisor(RelatorioProdutividadeTecnicoDataTableRequest request)
        {
            using (ISession sessionNh = HibernateUtil.GetSession())
            using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
            using (ISession sessionT5Sql = HibernateUtil.GetSessionT5())
            {
                IProdutividadeTecnicoRepository repoProdTecnico = new ProdutividadeTecnicoRepository(sessionT5Sql);
                IDocumentoProducaoAnielRepository repoDocProd = new DocumentoProducaoAnielRepository(sessionT5Nh, sessionT5Sql);
                IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(sessionNh);

                var businessProdutividadeTecnico =
                    new ProdutividadeTecnicoAnielBusiness(
                        repoProdTecnico,
                        repoDocProd,
                        repoConfig);

                RelatorioProdutividadeResponse relatorio = businessProdutividadeTecnico.GetRelatorioPorSupervisor(request);

                return new DataTableResponse<RelatorioProdutividadeTecnicoListModel>
                {
                    draw = request.draw,
                    recordsFiltered = relatorio.Total,
                    data = relatorio.Data
                };
            }
        }

        [HttpPost]
        [Route("api/RelatorioProdutividadeTecnicoApi/GetAllPorTecnico")]
        public DataTableResponse<RelatorioProdutividadeTecnicoListModel> GetAllPorTecnico(RelatorioProdutividadeTecnicoDataTableRequest request)
        {
            using (ISession sessionNh = HibernateUtil.GetSession())
            using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
            using (ISession sessionT5Sql = HibernateUtil.GetSessionT5())
            {
                IProdutividadeTecnicoRepository repoProdTecnico = new ProdutividadeTecnicoRepository(sessionT5Sql);
                IDocumentoProducaoAnielRepository repoDocProd = new DocumentoProducaoAnielRepository(sessionT5Nh, sessionT5Sql);
                IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(sessionNh);

                var businessProdutividadeTecnico =
                    new ProdutividadeTecnicoAnielBusiness(
                        repoProdTecnico,
                        repoDocProd,
                        repoConfig);

                RelatorioProdutividadeResponse relatorio = businessProdutividadeTecnico.GetRelatorioPorTecnico(request);

                return new DataTableResponse<RelatorioProdutividadeTecnicoListModel>
                {
                    draw = request.draw,
                    recordsFiltered = relatorio.Total,
                    data = relatorio.Data
                };
            }
        }

        [HttpPost]
        [Route("api/RelatorioProdutividadeTecnicoApi/GetExtratoProdutividade")]
        public DataTableResponse<RelProdTecnicoExtratoProdutividadeListModel> GetExtratoProdutividade(RelatorioProdutividadeTecnicoDataTableRequest request)
        {
            using (ISession sessionNh = HibernateUtil.GetSession())
            using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
            using (ISession sessionT5Sql = HibernateUtil.GetSessionT5())
            {
                IProdutividadeTecnicoRepository repoProdTecnico = new ProdutividadeTecnicoRepository(sessionT5Sql);
                IDocumentoProducaoAnielRepository repoDocProd = new DocumentoProducaoAnielRepository(sessionT5Nh, sessionT5Sql);
                IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(sessionNh);

                var businessProdutividadeTecnico =
                    new ProdutividadeTecnicoAnielBusiness(
                        repoProdTecnico,
                        repoDocProd,
                        repoConfig);

                RelProdTecnicoExtratoProdutividadeResponse extrato =
                    businessProdutividadeTecnico.GetExtratoProdutividade(request);

                return new DataTableResponse<RelProdTecnicoExtratoProdutividadeListModel>
                {
                    draw = request.draw,
                    recordsFiltered = extrato.Total,
                    data = extrato.Data
                };
            }
        }

        [HttpPost]
        [Route("api/RelatorioProdutividadeTecnicoApi/GetExtratoPrazo")]
        public DataTableResponse<RelProdTecnicoExtratoPrazoListModel> GetExtratoPrazo(RelatorioProdutividadeTecnicoDataTableRequest request)
        {
            using (ISession sessionNh = HibernateUtil.GetSession())
            using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
            using (ISession sessionT5Sql = HibernateUtil.GetSessionT5())
            {
                IProdutividadeTecnicoRepository repoProdTecnico = new ProdutividadeTecnicoRepository(sessionT5Sql);
                IDocumentoProducaoAnielRepository repoDocProd = new DocumentoProducaoAnielRepository(sessionT5Nh, sessionT5Sql);
                IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(sessionNh);

                var businessProdutividadeTecnico =
                    new ProdutividadeTecnicoAnielBusiness(
                        repoProdTecnico,
                        repoDocProd,
                        repoConfig);

                RelProdTecnicoExtratoPrazoResponse extrato =
                    businessProdutividadeTecnico.GetExtratoPrazo(request);

                return new DataTableResponse<RelProdTecnicoExtratoPrazoListModel>
                {
                    draw = request.draw,
                    recordsFiltered = extrato.Total,
                    data = extrato.Data
                };
            }
        }

        [HttpPost]
        [Route("api/RelatorioProdutividadeTecnicoApi/GetExtratoReincidencia")]
        public DataTableResponse<RelProdTecnicoExtratoReincidenciaListModel> GetExtratoReincidencia(RelatorioProdutividadeTecnicoDataTableRequest request)
        {
            using (ISession sessionNh = HibernateUtil.GetSession())
            using (ISession sessionT5Nh = HibernateUtil.GetSessionT5())
            using (ISession sessionT5Sql = HibernateUtil.GetSessionT5())
            {
                IProdutividadeTecnicoRepository repoProdTecnico = new ProdutividadeTecnicoRepository(sessionT5Sql);
                IDocumentoProducaoAnielRepository repoDocProd = new DocumentoProducaoAnielRepository(sessionT5Nh, sessionT5Sql);
                IConfiguracoesGeraisRepository repoConfig = new ConfiguracoesGeraisRepository(sessionNh);

                var businessProdutividadeTecnico =
                    new ProdutividadeTecnicoAnielBusiness(
                        repoProdTecnico,
                        repoDocProd,
                        repoConfig);

                RelProdTecnicoExtratoReincidenciaResponse extrato =
                    businessProdutividadeTecnico.GetExtratoReincidencia(request);

                return new DataTableResponse<RelProdTecnicoExtratoReincidenciaListModel>
                {
                    draw = request.draw,
                    recordsFiltered = extrato.Total,
                    data = extrato.Data
                };
            }
        }
    }
}