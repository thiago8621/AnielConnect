using NHibernate;
using Sinapse.DAO.repository;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.ParametrosConsultaPreQualParamSetorDepto
{
    public class ParametrosConsultaPreQualParamSetorDeptoRepository : IParametrosConsultaPreQualParamSetorDeptoRepository
    {
        private readonly ISession _sessionNh;

        public ParametrosConsultaPreQualParamSetorDeptoRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public IQueryable<Sinapse.Model.ParametrosConsultaPreQualParamSetorDepto> GetAll()
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.ParametrosConsultaPreQualParamSetorDepto>(this._sessionNh))
            {
                return repoParamSetor.QueryableSelect();
            }
        }

        public Sinapse.Model.ParametrosConsultaPreQualParamSetorDepto GetById(long id)
        {
            return this.GetAll()
                       .Where(p => p.Id == id)
                       .FirstOrDefault();
        }

        public IEnumerable<Sinapse.Model.ParametrosConsultaPreQualParamSetorDepto> GetParamsByPerfil(long idPerfil)
        {
            return this.GetAll()
                       .Where(p => p.ParametrosConsultaPreQual.Id == idPerfil)
                       .ToArray();
        }

        public void Save(Sinapse.Model.ParametrosConsultaPreQualParamSetorDepto param)
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.ParametrosConsultaPreQualParamSetorDepto>(this._sessionNh))
            {
                repoParamSetor.Save(param).Wait();
            }
        }

        public void Update(Sinapse.Model.ParametrosConsultaPreQualParamSetorDepto param)
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.ParametrosConsultaPreQualParamSetorDepto>(this._sessionNh))
            {
                repoParamSetor.Update(param).Wait();
            }
        }

        public void Delete(Sinapse.Model.ParametrosConsultaPreQualParamSetorDepto param)
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.ParametrosConsultaPreQualParamSetorDepto>(this._sessionNh))
            {
                repoParamSetor.Remove(param).Wait();
            }
        }
    }
}
