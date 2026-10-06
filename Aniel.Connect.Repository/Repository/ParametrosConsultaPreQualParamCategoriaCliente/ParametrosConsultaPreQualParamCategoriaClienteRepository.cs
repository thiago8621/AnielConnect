using NHibernate;
using Sinapse.DAO.repository;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.ParametrosConsultaPreQualParamCategoriaCliente
{
    public class ParametrosConsultaPreQualParamCategoriaClienteRepository : IParametrosConsultaPreQualParamCategoriaClienteRepository
    {
        private readonly ISession _sessionNh;

        public ParametrosConsultaPreQualParamCategoriaClienteRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public IQueryable<Sinapse.Model.ParametrosConsultaPreQualParamCategoriaCliente> GetAll()
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.ParametrosConsultaPreQualParamCategoriaCliente>(this._sessionNh))
            {
                return repoParamSetor.QueryableSelect();
            }
        }

        public Sinapse.Model.ParametrosConsultaPreQualParamCategoriaCliente GetById(long id)
        {
            return this.GetAll()
                       .Where(p => p.Id == id)
                       .FirstOrDefault();
        }

        public IEnumerable<Sinapse.Model.ParametrosConsultaPreQualParamCategoriaCliente> GetParamsByPerfil(long idPerfil)
        {
            return this.GetAll()
                       .Where(p => p.ParametrosConsultaPreQual.Id == idPerfil)
                       .ToArray();
        }

        public void Save(Sinapse.Model.ParametrosConsultaPreQualParamCategoriaCliente param)
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.ParametrosConsultaPreQualParamCategoriaCliente>(this._sessionNh))
            {
                repoParamSetor.Save(param).Wait();
            }
        }

        public void Update(Sinapse.Model.ParametrosConsultaPreQualParamCategoriaCliente param)
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.ParametrosConsultaPreQualParamCategoriaCliente>(this._sessionNh))
            {
                repoParamSetor.Update(param).Wait();
            }
        }

        public void Delete(Sinapse.Model.ParametrosConsultaPreQualParamCategoriaCliente param)
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.ParametrosConsultaPreQualParamCategoriaCliente>(this._sessionNh))
            {
                repoParamSetor.Remove(param).Wait();
            }
        }
    }
}
