using Aniel.Connect.Repository.T5.Natureza;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.ConnectBusiness.T5
{
    public class NaturezaAnielBusiness
    {
        private readonly INaturezaAnielRepository _repoNatureza;

        public NaturezaAnielBusiness(INaturezaAnielRepository repoNatureza)
        {
            this._repoNatureza = repoNatureza;
        }

        /// <summary>
        /// Retorna o codigo da Natureza do tipo de Servico
        /// </summary>
        /// <param name="pCodTipoServ">Código do Tipo de Serviço</param>
        public Int32 GetNaturezaByCodTipoServ(Int32 pCodTipoServ,string pCodCt)
        {
            return this._repoNatureza.GetNaturezaByCodTipoServ(pCodTipoServ, pCodCt);
        }
    }
}
