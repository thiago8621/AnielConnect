using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.FormaPagamento;
using Sinapse.DAO.repository;
using System;
using System.Linq;

namespace Aniel.Connect.ConnectBusiness.Connect
{
    public class FormaPagamentoBusiness
    {
        private readonly IFormaPagamentoRepository _repository;

        public FormaPagamentoBusiness(IFormaPagamentoRepository repository)
        {
            this._repository = repository;
        }

        /// <summary>
        /// Retorna forma de pagamento associado ao id
        /// </summary>
        /// <param name="Id">Id da forma de pagamento</param>
        public Forma_Pagamento GetById(long Id)
        {
            return this._repository.GetFormaPagamentoById(Id);
        
        }         
  
    }
}
