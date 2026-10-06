using Aniel.Hibernate.Entity;
using AnielTV.Hibernate.Persistence;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aniel.Hibernate.Repository
{
    public class EquipeRepository : RepositoryBase
    {
        public EquipeRepository(ISession session) : base(session)
        {
            Session = session;
        }

        public List<Equipe> getEquipe(String Equipe)
        {
            return new Conexao<Equipe>(Session).select(p => p.Matricula == Equipe);
        }
    }
}
