using System.Linq;

namespace Aniel.Connect.Repository.PendenciaFinanceira
{
    public interface IPendenciaFinanceiraRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_PENDENCIA_FINANCEIRA
        /// </summary>
        IQueryable<Sinapse.Model.PendenciaFinanceira> GetAll();

        /// <summary>
        ///     Retorna a consulta de pendência financeira com o ID informado
        /// </summary>
        /// <param name="id">ID do registro da consulta</param>
        Sinapse.Model.PendenciaFinanceira GetConsultaPendenciaFinanceiraById(long id);
        
        /// <summary>
        ///     Retorna a consulta de pendência financeira mais recente registrada no sistema para determinado CPF
        /// </summary>
        /// <param name="cpf">CPF do cliente</param>
        Sinapse.Model.PendenciaFinanceira GetConsultaPendenciaFinanceiraByCpf(string cpf);

        /// <summary>
        ///     Retorna a consulta de pendência financeira mais recente registrada no sistema para determinado CNPJ
        /// </summary>
        /// <param name="cnpj">CNPJ do cliente</param>
        Sinapse.Model.PendenciaFinanceira GetConsultaPendenciaFinanceiraByCnpj(string cnpj);

        /// <summary>
        ///     Insere uma consulta de pendência financeira no banco
        /// </summary>
        /// <param name="consultaPendenciaFinanceira">Objeto com os dados da consulta de pendencia financeira</param>
        void Save(Sinapse.Model.PendenciaFinanceira consultaPendenciaFinanceira);
    }
}
