using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.T5.ReincidenciaDocProd
{
    public interface IPainelPerformanceTecnicoRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta às OSs reincidentes registradas no sistema
        /// </summary>
        PainelPerformanceTecnicoResponse GetAll(PainelPerformanceTecnicoDataTableRequest request);
    }
}
