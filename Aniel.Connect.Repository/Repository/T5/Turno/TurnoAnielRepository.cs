using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;

namespace Aniel.Connect.Repository.T5.Turno
{
    public class TurnoAnielRepository : ITurnoAnielRepository
    {
        private readonly ISession _sessionT5Nh;

        public TurnoAnielRepository(ISession sessionT5Nh)
        {
            this._sessionT5Nh = sessionT5Nh;
        }

        public IEnumerable<Turno_ServicoAniel> GetAll()
        {
            using(Conexao<Turno_ServicoAniel> conexaoTurno = new Conexao<Turno_ServicoAniel>(this._sessionT5Nh))
            {
                return conexaoTurno.QueryableSelect();
            }
        }
    }
}
