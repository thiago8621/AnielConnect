using System.Collections.Generic;
using System.Linq;
using Aniel.T5.Model.nhibernate;

namespace Aniel.Connect.Repository.T5.Horario
{
    public interface IHorarioAnielRepository
    {
        IEnumerable<Horario_Connect> GetAll();
    }
}
