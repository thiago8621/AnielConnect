using System.Collections.Generic;
using System.Linq;
using Aniel.T5.Model.nhibernate;

namespace Aniel.Connect.Repository.T5.Turno
{
    public interface ITurnoAnielRepository
    {
        IEnumerable<Turno_ServicoAniel> GetAll();
    }
}
