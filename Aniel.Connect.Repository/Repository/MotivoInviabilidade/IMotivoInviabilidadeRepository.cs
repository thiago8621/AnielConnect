using Aniel.Connect.Model.nhibernate;
using System.Linq;

namespace Aniel.Connect.Repository.MotivoInviabilidade
{
    public interface IMotivoInviabilidadeRepository
    {
         IQueryable <Motivo_Inviabilidade> GetAll();
         Motivo_Inviabilidade GetById(long Id);
    }
}
