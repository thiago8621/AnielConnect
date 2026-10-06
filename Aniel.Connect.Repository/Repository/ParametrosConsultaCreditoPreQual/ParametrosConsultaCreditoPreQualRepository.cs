using NHibernate;
using Sinapse.DAO.repository;
using System.Linq;

namespace Aniel.Connect.Repository.ParametrosConsultaPreQual
{
    public class ParametrosConsultaPreQualRepository : IParametrosConsultaPreQualRepository
    {
        private readonly ISession _sessionNh;

        public ParametrosConsultaPreQualRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public IQueryable<Sinapse.Model.ParametrosConsultaPreQual> GetAll()
        
        {
            using (var repoPerfil = new Conexao<Sinapse.Model.ParametrosConsultaPreQual>(this._sessionNh))
            {
                return repoPerfil.QueryableSelect();
            }
        }

        public Sinapse.Model.ParametrosConsultaPreQual GetById(long id)
        {
            return this.GetAll()
                       .Where(p => p.Id == id)
                       .FirstOrDefault();
        }

        public void Save(Sinapse.Model.ParametrosConsultaPreQual perfil)
        {
            using (var repoPerfil = new Conexao<Sinapse.Model.ParametrosConsultaPreQual>(this._sessionNh))
            {
                repoPerfil.Save(perfil).Wait();
            }
        }

        public void Update(Sinapse.Model.ParametrosConsultaPreQual perfil)
        {
            using (var repoPerfil = new Conexao<Sinapse.Model.ParametrosConsultaPreQual>(this._sessionNh))
            {
                repoPerfil.Update(perfil).Wait();
            }
        }
    }
}
