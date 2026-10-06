using Aniel.Connect.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.CondominioPredio
{
    public class CondominioPredioRepository : ICondominioPredioRepository
    {
        private readonly ISession _sessionNh;        
        public CondominioPredioRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;       
        }

        public IQueryable<Condominio_Predio> GetAll()
        {
            using (var repo = new Conexao<Condominio_Predio>(this._sessionNh))
            {
                return repo.QueryableSelect();
            }
        }
        public IQueryable<Condominio_PredioBloco> GetAllBlocos()
        {
            using (var repo = new Conexao<Condominio_PredioBloco>(this._sessionNh))
            {
                return repo.QueryableSelect();
            }
        }
        public IQueryable<Condominio_PredioApartamento> GetAllApartamentos()
        {
            using (var repo = new Conexao<Condominio_PredioApartamento>(this._sessionNh))
            {
                return repo.QueryableSelect();
            }
        }

        public Condominio_Predio GetById(long id)
        {
            return this.GetAll()
                       .Where(c => c.Id == id)
                       .FirstOrDefault();
        }
        public IEnumerable<Condominio_PredioBloco> GetBlocosByCondominio(long id)
        {
            return this.GetAllBlocos()
                .Where(p => p.Condominio.Id == id)
                .ToList();
        }
        public IEnumerable<Condominio_PredioApartamento> GetApartamentosByCondominio(long id)
        {
            return this.GetAllApartamentos()
                .Where(p => p.Bloco.Condominio.Id == id)
                .ToList();
        }
        public IEnumerable<Condominio_PredioApartamento> GetApartamentosByBloco(long id)
        {
            return this.GetAllApartamentos()
                .Where(p => p.Bloco.Id == id)
                .ToList();
        }
        public void save(Condominio_Predio condominio) 
        {
            using (var repo = new Conexao<Condominio_Predio>(this._sessionNh)) 
            {
                repo.SaveOrUpdate(condominio).Wait();
            }
        }
        public void saveBlocos(Condominio_PredioBloco bloco)
        {
            using (var repo = new Conexao<Condominio_PredioBloco>(this._sessionNh))
            {
                repo.SaveOrUpdate(bloco).Wait();
            }
        }
        public void saveApartamentos(IEnumerable<Condominio_PredioApartamento> apartamento)
        {
            using (var repo = new Conexao<Condominio_PredioApartamento>(this._sessionNh))
            {
                repo.SaveOrUpdate(apartamento).Wait();
            }
        }

    }
}
