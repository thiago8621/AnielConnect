using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.T5.DocumentoProducao;
using Aniel.Connect.Repository.T5.ProdutividadeTecnico;

namespace Aniel.Connect.ConnectBusiness.T5
{
    public class ProdutividadeTecnicoAnielBusiness
    {
        private readonly IProdutividadeTecnicoRepository _repoProdTecnico;
        private readonly IDocumentoProducaoAnielRepository _repoDocProd;
        private readonly IConfiguracoesGeraisRepository _repoConfig;

        public ProdutividadeTecnicoAnielBusiness(
            IProdutividadeTecnicoRepository repoProdTecnico,
            IDocumentoProducaoAnielRepository repoDocProd,
            IConfiguracoesGeraisRepository repoConfig)
        {
            this._repoProdTecnico = repoProdTecnico;
            this._repoDocProd = repoDocProd;
            this._repoConfig = repoConfig;
        }

        public RelatorioProdutividadeResponse GetRelatorio(RelatorioProdutividadeTecnicoDataTableRequest request)
        {
            Configuracoes_Gerais config = this._repoConfig.GetConfiguracoesGerais();
            return this._repoProdTecnico.GetRelatorioProdutividadeTecnico(
                request, 
                config.Considerar_Agendamento_Periodo,
                config.ApenasServicosProdutivosReincidencia
            );
        }

        public RelatorioProdutividadeResponse GetRelatorioPorSupervisor(RelatorioProdutividadeTecnicoDataTableRequest request)
        {
            Configuracoes_Gerais config = this._repoConfig.GetConfiguracoesGerais();
            return this._repoProdTecnico.GetRelatorioProdutividadeSupervisor(
                request, 
                config.Considerar_Agendamento_Periodo,
                config.ApenasServicosProdutivosReincidencia
            );
        }

        public RelatorioProdutividadeResponse GetRelatorioPorTecnico(RelatorioProdutividadeTecnicoDataTableRequest request)
        {
            Configuracoes_Gerais config = this._repoConfig.GetConfiguracoesGerais();
            return this._repoProdTecnico.GetRelatorioProdutividadeAgrupadoTecnico(
                request,
                config.Considerar_Agendamento_Periodo,
                config.ApenasServicosProdutivosReincidencia
            );
        }

        public RelatorioProdutividadeResponse GetRelatorioPorMonitor(RelatorioProdutividadeTecnicoDataTableRequest request)
        {
            Configuracoes_Gerais config = this._repoConfig.GetConfiguracoesGerais();
            return this._repoProdTecnico.GetRelatorioProdutividadeMonitor(
                request, 
                config.Considerar_Agendamento_Periodo,
                config.ApenasServicosProdutivosReincidencia
            );
        }

        public RelProdTecnicoExtratoProdutividadeResponse GetExtratoProdutividade(
            RelatorioProdutividadeTecnicoDataTableRequest request)
        {
            Configuracoes_Gerais config = this._repoConfig.GetConfiguracoesGerais();
            return this._repoDocProd.GetExtratoProdutividade(request, config.Considerar_Agendamento_Periodo);
        }

        public RelProdTecnicoExtratoPrazoResponse GetExtratoPrazo(
            RelatorioProdutividadeTecnicoDataTableRequest request)
        {
            Configuracoes_Gerais config = this._repoConfig.GetConfiguracoesGerais();
            return this._repoDocProd.GetExtratoPrazo(request, config.Considerar_Agendamento_Periodo);
        }

        public RelProdTecnicoExtratoReincidenciaResponse GetExtratoReincidencia(
            RelatorioProdutividadeTecnicoDataTableRequest request)
        {
            Configuracoes_Gerais config = this._repoConfig.GetConfiguracoesGerais();
            return this._repoDocProd.GetExtratoReincidencia(request, config.ApenasServicosProdutivosReincidencia);
        }
    }
}
