using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.API_Aniel.Models
{
    /// <summary>
    ///     Versão simplificada do <see cref="Aniel.Connect.Repository.Model.PendenciaEnderecoListModel"/> 
    ///     e <see cref="Aniel.Connect.Repository.Model.PendenciaFinanceiraListModel"/>
    ///     para uso no API Aniel
    /// </summary>
    public class ConsultaPendenciasListModel
    {
        public string CodFatura { get; set; }
        public string Data_Vencimento { get; set; }
        public string Valor_Total { get; set; }
        public string Cod_Contrato { get; set; }
    }
}