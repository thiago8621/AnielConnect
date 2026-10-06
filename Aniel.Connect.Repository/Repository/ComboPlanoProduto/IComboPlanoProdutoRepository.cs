using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.Repository.ComboPlanoProduto
{
    public interface IComboPlanoProdutoRepository
    {
        IQueryable<Aniel.Connect.Model.nhibernate.ComboPlanoProduto> GetAll();

        Aniel.Connect.Model.nhibernate.ComboPlanoProduto GetById(long id);

        void Save(Aniel.Connect.Model.nhibernate.ComboPlanoProduto combo_plano_produto);

        void Update(Aniel.Connect.Model.nhibernate.ComboPlanoProduto combo_plano_produto);
    }
}
