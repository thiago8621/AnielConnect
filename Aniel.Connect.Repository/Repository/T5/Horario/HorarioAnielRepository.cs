using Aniel.T5.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;

namespace Aniel.Connect.Repository.T5.Horario
{
    public class HorarioAnielRepository : IHorarioAnielRepository
    {
        private readonly ISession _sessionT5Nh;

        public HorarioAnielRepository(ISession sessionT5Nh)
        {
            this._sessionT5Nh = sessionT5Nh;
        }

        public IEnumerable<Horario_Connect> GetAll()
        {
            using (Conexao<Horario_Connect> conexaoTurno = new Conexao<Horario_Connect>(this._sessionT5Nh))
            {
                return conexaoTurno.QueryableSelect();
            }
        }
    }
}
