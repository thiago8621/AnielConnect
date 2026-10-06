using System;

namespace Aniel.Connect.ExecucaoService.Data.Model
{
    /// <summary>
    ///     Classe com dados persistentes importantes para execução do serviço
    /// </summary>
    public class ServiceData
    {
        /// <summary>
        ///     Última data processada pela rotina de lançamento de reincidência
        ///     retroativo
        /// </summary>
        public DateTime? UltimaDataProcessadaReincidencia { get; set; }
    }
}
