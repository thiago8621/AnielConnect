using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Model;
using Aniel.Connect.Repository.T5.DocumentoProducao;
using Aniel.Connect.Repository.T5.ReincidenciaDocProd;
using Aniel.T5.Model.enumerado;
using System;
using System.Linq;

namespace Aniel.Connect.ConnectBusiness.T5
{
    public class PerformanceTecnicoAnielBusiness
    {
        private readonly IPainelPerformanceTecnicoRepository _repoPainelPerformance;

        public PerformanceTecnicoAnielBusiness(IPainelPerformanceTecnicoRepository repoDocProd)
        {
            this._repoPainelPerformance = repoDocProd;
        }

        public PainelPerformanceTecnicoResponse GetRelatorio(PainelPerformanceTecnicoDataTableRequest request)
        {
            var lista = _repoPainelPerformance.GetAll(request);

            return new PainelPerformanceTecnicoResponse
            {
                Total = lista.Total,
                Data = lista.Data
            };
        }
    }
}
