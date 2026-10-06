using Aniel.Connect.Model.nhibernate;
using System.Linq;

namespace Aniel.Connect.Repository.ClienteContrato
{
    public interface IClienteContratoRepository
    {
        /// <summary>
        /// Retorna todos os contratos do cliente
        /// </summary>
        IQueryable<Aniel.Connect.Model.nhibernate.ClienteContrato> GetAll();

        /// <summary>
        /// Retorna contrato do cliente por id
        /// </summary>
        /// <param name="Id">Retorno id do cliente</param>
        Aniel.Connect.Model.nhibernate.ClienteContrato GetById(long Id);

        /// <summary>
        /// Salva objeto cliente contrado
        /// </summary>
        /// <param name="clientecontrato">objeto cliente contrato</param>
        void Save(Aniel.Connect.Model.nhibernate.ClienteContrato clientecontrato);

    }
}
