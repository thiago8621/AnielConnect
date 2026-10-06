using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.Atendimento
{
    public interface IAtendimentoRepository
    {
        IQueryable<Connect.Model.nhibernate.Atendimento> GetAll();
        IEnumerable<Connect.Model.nhibernate.Atendimento> GetAtendimentosByContratos(IEnumerable<long> planos);        
    }
}
