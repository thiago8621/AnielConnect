using Aniel.Connect.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System.Linq;

namespace Aniel.Connect.Repository.MotivoInviabilidade
{
    public class MotivoInviabilidadeRepository : IMotivoInviabilidadeRepository
    {
        private ISession _session;
        public MotivoInviabilidadeRepository(ISession session)         
        {
            _session = session;
        }
        
        public IQueryable<Motivo_Inviabilidade> GetAll() 
        {
            using (var repo = new Conexao<Motivo_Inviabilidade>(_session)) 
            {
                return repo.QueryableSelect();
            }
        }
        public Motivo_Inviabilidade GetById(long Id)
        {
            return this.GetAll().Where(p => p.Id == Id).OrderByDescending(p => p.DataHoraCriacao).FirstOrDefault();
        }
    }
}
