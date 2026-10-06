
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.Financeiro.CobrancaFaturamento
{
    public interface ICobrancaFaturamentoRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_COBRANCA_FATURAMENTO
        /// </summary>
        IQueryable<Aniel.Connect.Model.nhibernate.CobrancaFaturamento> GetAll();

        /// <summary>
        ///     Retorna uma lista com as cobranças vencidas
        /// </summary>
        IEnumerable<Aniel.Connect.Model.nhibernate.CobrancaFaturamento> GetCobrancasVencidas();

        /// <summary>
        ///     Atualiza um registro na TB_COBRANCA_FATURAMENTO
        /// </summary>
        /// <param name="cobrancaFaturamento">Objeto do NHibernate com os dados do registro</param>
        void Update(Aniel.Connect.Model.nhibernate.CobrancaFaturamento cobrancaFaturamento);
    }
}
