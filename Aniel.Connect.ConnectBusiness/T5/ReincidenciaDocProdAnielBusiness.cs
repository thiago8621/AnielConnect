using Aniel.Connect.Repository.T5.ReincidenciaDocProd;
using Aniel.T5.Model.nhibernate;
using System;
using System.Linq;

namespace Aniel.Connect.ConnectBusiness.T5
{
    public class ReincidenciaDocProdAnielBusiness
    {
        private readonly IReincidenciaDocProdAnielRepository _repoReincidencia;

        public ReincidenciaDocProdAnielBusiness(IReincidenciaDocProdAnielRepository repoReincidencia)
        {
            this._repoReincidencia = repoReincidencia;
        }

        public void Gravar(ReincidenciaDocProdAniel reincidencia)
        {
            this._repoReincidencia.Gravar(reincidencia);
        }

        /// <summary>
        ///     Retorna a data da reincidência mais antiga registrada no sistema
        /// </summary>
        public DateTime? GetDataPrimeiraReincidencia()
        {
            int qtdRegistros = 
                this._repoReincidencia
                    .GetAll()
                    .Count();

            if (qtdRegistros == 0)
                return null;

            return this._repoReincidencia
                       .GetAll()
                       .Select(r => r.Data)
                       ?.Min();
        }
    }
}
