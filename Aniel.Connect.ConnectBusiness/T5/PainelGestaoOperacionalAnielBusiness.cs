using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.ConfiguracoesGerais;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.T5.DocumentoProducao;
using Aniel.Connect.Repository.T5.ProdutividadeTecnico;

namespace Aniel.Connect.ConnectBusiness.T5
{
    public class PainelGestaoOperacionalAnielBusiness
    {
        private readonly IDocumentoProducaoAnielRepository _repoDocProd;
        private readonly IConfiguracoesGeraisRepository _repoConfig;

        public PainelGestaoOperacionalAnielBusiness(
            IDocumentoProducaoAnielRepository repoDocProd,
            IConfiguracoesGeraisRepository repoConfig)
        {
            this._repoDocProd = repoDocProd;
            this._repoConfig = repoConfig;
        }

        public GestaoOperacionalServicosResponse GetServicos(GestaoCapacidadeRoteirizacaoRequest request)
        {
            Configuracoes_Gerais config = this._repoConfig.GetConfiguracoesGerais();
            return this._repoDocProd.GetServicosPainelGestaoOperacional(
                request, 
                config.Tempo_Execucao_Padrao?.TimeOfDay,
                config.Tempo_Medio_Deslocamento?.TimeOfDay
            );
        }

    }
}
