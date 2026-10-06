using Aniel.T5.Model.nhibernate;
using System;
using System.Linq;

namespace Aniel.Connect.Repository.T5.Natureza
{
    public interface INaturezaAnielRepository
    {
        /// <summary>
        ///     Retorna uma representação de uma consulta na TB_NATUREZA_PRODUCAO
        /// </summary>
        IQueryable<Natureza_ProducaoAniel> GetAll();

        /// <summary>
        /// Retorna o codigo da Natureza do tipo de Servico
        /// </summary>
        /// <param name="pCodTipoServ">Código do Tipo de Serviço</param>
        /// <param name="pCodct">Código do Contrato</param>
        Int32 GetNaturezaByCodTipoServ(Int32 pCodTipoServ,string pCodct);
    }
}
