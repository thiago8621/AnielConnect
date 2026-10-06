using NHibernate;
using Sinapse.DAO.repository;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.ParametrosConsultaPreQualParamRegiao
{
    public class ParametrosConsultaPreQualParamRegiaoRepository : IParametrosConsultaPreQualParamRegiaoRepository
    {
        private readonly ISession _sessionNh;

        public ParametrosConsultaPreQualParamRegiaoRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public IQueryable<Sinapse.Model.ParametrosConsultaPreQualParamRegiao> GetAll()
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.ParametrosConsultaPreQualParamRegiao>(this._sessionNh))
            {
                return repoParamSetor.QueryableSelect();
            }
        }

        public Sinapse.Model.ParametrosConsultaPreQualParamRegiao GetById(long id)
        {
            return this.GetAll()
                       .Where(p => p.Id == id)
                       .FirstOrDefault();
        }

        public IEnumerable<Sinapse.Model.ParametrosConsultaPreQualParamRegiao> GetParamsByPerfil(long idPerfil)
        {
            return this.GetAll()
                       .Where(p => p.ParametrosConsultaPreQual.Id == idPerfil)
                       .ToArray();
        }

        public void Save(Sinapse.Model.ParametrosConsultaPreQualParamRegiao param)
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.ParametrosConsultaPreQualParamRegiao>(this._sessionNh))
            {
                repoParamSetor.Save(param).Wait();
            }
        }

        public void Update(Sinapse.Model.ParametrosConsultaPreQualParamRegiao param)
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.ParametrosConsultaPreQualParamRegiao>(this._sessionNh))
            {
                repoParamSetor.Update(param).Wait();
            }
        }

        public void Delete(Sinapse.Model.ParametrosConsultaPreQualParamRegiao param)
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.ParametrosConsultaPreQualParamRegiao>(this._sessionNh))
            {
                repoParamSetor.Remove(param).Wait();
            }
        }
    }
}
