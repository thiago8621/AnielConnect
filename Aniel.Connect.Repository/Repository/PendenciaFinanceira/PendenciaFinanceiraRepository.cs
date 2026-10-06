using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Linq;

namespace Aniel.Connect.Repository.PendenciaFinanceira
{
    public class PendenciaFinanceiraRepository : IPendenciaFinanceiraRepository
    {
        private readonly ISession _sessionNh;

        public PendenciaFinanceiraRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public IQueryable<Sinapse.Model.PendenciaFinanceira> GetAll()
        {
            using (var repo = new Conexao<Sinapse.Model.PendenciaFinanceira>(this._sessionNh))
            {
                return repo.QueryableSelect();
            }
        }

        public Sinapse.Model.PendenciaFinanceira GetConsultaPendenciaFinanceiraById(long id)
        {
            return this.GetAll()
                       .Where(p => p.Id == id)
                       .FirstOrDefault();
        }

        public Sinapse.Model.PendenciaFinanceira GetConsultaPendenciaFinanceiraByCpf(string cpf)
        {
            cpf = cpf.Replace(".", String.Empty)
                     .Replace("-", String.Empty);

            return this.GetAll()
                       .Where(p => p.Cpf == cpf)
                       .OrderByDescending(p => p.DataHoraCriacao)
                       .FirstOrDefault();
        }

        public Sinapse.Model.PendenciaFinanceira GetConsultaPendenciaFinanceiraByCnpj(string cnpj)
        {
            cnpj = cnpj.Replace(".", String.Empty)
                       .Replace("-", String.Empty)
                       .Replace("/", String.Empty);

            return this.GetAll()
                       .Where(p => p.Cnpj == cnpj)
                       .OrderByDescending(p => p.DataHoraCriacao)
                       .FirstOrDefault();
        }

        public void Save(Sinapse.Model.PendenciaFinanceira consultaPendenciaFinanceira)
        {
            using (var repo = new Conexao<Sinapse.Model.PendenciaFinanceira>(this._sessionNh))
            {
                repo.Save(consultaPendenciaFinanceira).Wait();
            }
        }
    }
}
