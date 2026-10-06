using NHibernate;
using Sinapse.DAO.repository;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.ParametrosConsultaPreQualParamTipoPessoa
{
    public class ParametrosConsultaPreQualParamTipoPessoaRepository : IParametrosConsultaPreQualParamTipoPessoaRepository
    {
        private readonly ISession _sessionNh;

        public ParametrosConsultaPreQualParamTipoPessoaRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public IQueryable<Sinapse.Model.ParametrosConsultaPreQualParamTipoPessoa> GetAll()
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.ParametrosConsultaPreQualParamTipoPessoa>(this._sessionNh))
            {
                return repoParamSetor.QueryableSelect();
            }
        }

        public Sinapse.Model.ParametrosConsultaPreQualParamTipoPessoa GetById(long id)
        {
            return this.GetAll()
                       .Where(p => p.Id == id)
                       .FirstOrDefault();
        }

        public IEnumerable<Sinapse.Model.ParametrosConsultaPreQualParamTipoPessoa> GetParamsByPerfil(long idPerfil)
        {
            return this.GetAll()
                       .Where(p => p.ParametrosConsultaPreQual.Id == idPerfil)
                       .ToArray();
        }

        public void Save(Sinapse.Model.ParametrosConsultaPreQualParamTipoPessoa param)
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.ParametrosConsultaPreQualParamTipoPessoa>(this._sessionNh))
            {
                repoParamSetor.Save(param).Wait();
            }
        }

        public void Update(Sinapse.Model.ParametrosConsultaPreQualParamTipoPessoa param)
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.ParametrosConsultaPreQualParamTipoPessoa>(this._sessionNh))
            {
                repoParamSetor.Update(param).Wait();
            }
        }

        public void Delete(Sinapse.Model.ParametrosConsultaPreQualParamTipoPessoa param)
        {
            using (var repoParamSetor = new Conexao<Sinapse.Model.ParametrosConsultaPreQualParamTipoPessoa>(this._sessionNh))
            {
                repoParamSetor.Remove(param).Wait();
            }
        }
    }
}
