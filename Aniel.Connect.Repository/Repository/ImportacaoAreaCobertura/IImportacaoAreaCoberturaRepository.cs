using Aniel.Connect.Repository.Model.ImportacaoAreaCobertura;
using System.Collections.Generic;

namespace Aniel.Connect.Repository.Repository.ImportacaoAreaCobertura
{
    public interface IImportacaoAreaCoberturaRepository
    {
        /// <summary>
        ///     Retorna dados dos polígonos presentes na fonte de importação
        ///     de áreas de cobertura
        /// </summary>
        IEnumerable<AreaCoberturaImportacao> GetDadosAreaCobertura();
    }
}
