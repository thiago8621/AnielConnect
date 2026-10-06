namespace Aniel.Connect.Repository.Vencimento
{
    public interface IVencimentoRepository
    {
        /// <summary>
        /// Retorna vencimento da forma de pagamento
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Connect.Model.nhibernate.Vencimento GetByIdVencimento(long id);

    }
}
