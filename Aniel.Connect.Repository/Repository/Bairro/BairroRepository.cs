using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Linq;

namespace Aniel.Connect.Repository.Bairro
{
    public class BairroRepository : IBairroRepository
    {
        private readonly ISession _sessionNh;

        public BairroRepository(ISession sessionNh)
        {
            this._sessionNh = sessionNh;
        }

        public IQueryable<Aniel.Connect.Model.nhibernate.Bairro> GetAll()
        {
            using (var repo = new Conexao<Aniel.Connect.Model.nhibernate.Bairro>(this._sessionNh))
            {
                return repo.QueryableSelect();
            }
        }

        public Aniel.Connect.Model.nhibernate.Bairro GetByName(
            string nomeBairro, 
            string nomeCidade, 
            string siglaUF, 
            string nomePais
        )
        {
            return this.GetAll()
                       .Where(b => b.Pais.Nome == nomePais &&
                                   b.UF.Sigla == siglaUF &&
                                   b.Cidade.Descricao == nomeCidade &&
                                   b.Descricao == nomeBairro)
                       .FirstOrDefault();
        }

        public Aniel.Connect.Model.nhibernate.Bairro GetById(string idBairro, string idCidade, string idUF)
        {
            return this.GetAll()
                       .Where(c => c.Id == Convert.ToInt32(idBairro) && c.UF.Id == Convert.ToInt32(idUF) &&
                                   c.Cidade.Id == Convert.ToInt32(idCidade))
                       .FirstOrDefault();
        }

    }
}
