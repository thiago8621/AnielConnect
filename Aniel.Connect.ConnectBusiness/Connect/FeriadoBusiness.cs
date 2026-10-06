using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Feriado;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.ConnectBusiness.Connect
{
    public class FeriadoBusiness
    {
        private readonly IFeriadoRepository _repository;

        public FeriadoBusiness(IFeriadoRepository repository)
        {
            this._repository = repository;
        }

        public bool IsFeriado(DateTime dia, string uf, string cidade)
        {
            IEnumerable<Feriado> feriadosDia = this._repository.GetFeriadosDia(dia, uf, cidade);

            return feriadosDia != null && feriadosDia.Count() > 0;
        }
    }
}
