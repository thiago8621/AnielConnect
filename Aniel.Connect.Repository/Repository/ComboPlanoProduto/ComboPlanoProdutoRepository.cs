using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Linq;

namespace Aniel.Connect.Repository.Repository.ComboPlanoProduto
{
    public class ComboPlanoProdutoRepository : IComboPlanoProdutoRepository
    {
        private readonly ISession _sessionNh;

        public ComboPlanoProdutoRepository(ISession sessionNh)
        {
            _sessionNh = sessionNh;
        }

        public IQueryable<Connect.Model.nhibernate.ComboPlanoProduto> GetAll()
        {
            using (var repoComboPlanoProduto = new Conexao<Connect.Model.nhibernate.ComboPlanoProduto>(this._sessionNh))
            {
                return repoComboPlanoProduto.QueryableSelect();
            }
        }

        public Connect.Model.nhibernate.ComboPlanoProduto GetById(long id)
        {
            return this.GetAll()
                       .Where(c => c.Combo.Id == id && c.Plano.Tipo_Tecnologia == Tipo_Tecnologia.Internet)
                       .FirstOrDefault();
        }

        public void Save(Connect.Model.nhibernate.ComboPlanoProduto combo_plano_produto)
        {
            using (var repoComboPlanoProduto = new Conexao<Aniel.Connect.Model.nhibernate.ComboPlanoProduto>(this._sessionNh))
            {
                repoComboPlanoProduto.Save(combo_plano_produto).Wait();
            }
        }

        public void Update(Connect.Model.nhibernate.ComboPlanoProduto combo_plano_produto)
        {
            using (var repoComboPlanoProduto = new Conexao<Aniel.Connect.Model.nhibernate.ComboPlanoProduto>(this._sessionNh))
            {
                repoComboPlanoProduto.Update(combo_plano_produto).Wait();
            }
        }
    }
}
