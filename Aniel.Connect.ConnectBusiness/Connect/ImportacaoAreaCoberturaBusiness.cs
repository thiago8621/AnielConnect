using Aniel.Connect.Repository.Model.ImportacaoAreaCobertura;
using Aniel.Connect.Repository.Repository.ImportacaoAreaCobertura;
using System.Collections.Generic;

namespace Aniel.Connect.ConnectBusiness.Connect
{
    public class ImportacaoAreaCoberturaBusiness
    {
        private readonly IImportacaoAreaCoberturaRepository _repoImportacaoArea;

        public ImportacaoAreaCoberturaBusiness(IImportacaoAreaCoberturaRepository repoImportacaoArea)
        {
            this._repoImportacaoArea = repoImportacaoArea;
        }

        /// <summary>
        ///     Retorna os dados das áreas de cobertura presentes na fonte de dados de importação
        /// </summary>
        public IEnumerable<AreaCoberturaImportacao> GetDadosAreaCobertura()
        {
            return this._repoImportacaoArea.GetDadosAreaCobertura();
        }
    }
}
