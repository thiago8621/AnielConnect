using Aniel.Connect.Model.nhibernate;
using System;
using System.Linq;

namespace Aniel.Connect.Repository.FormaPagamento
{
    public interface IFormaPagamentoRepository
    {
        /// <summary>
        /// Retorna forma pagamento 
        /// </summary>

        IQueryable<Forma_Pagamento> GetAllFormaPagamento();

        /// <summary>
        /// Retorna forma de pagamento por Id
        /// </summary>
        /// <param name="Id"></param>
        Forma_Pagamento GetFormaPagamentoById(long Id);
    }
}
