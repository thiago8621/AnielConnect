using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.Repository.BancoFoto.AssinaturaEquipe
{
  
    public interface IAssinaturaEquipeRepository
    {
        /// <summary>
        ///     Retorna se existe assinatura para a equipe na  TB_ASSINATURA_EQUIPE
        /// </summary>
        bool  ExisteAssinaturaEquipe(string equipe);

     

        /// <summary>
        /// Reseta a Assinatura do funcionario na TB_ASSINATURA_EQUIPE
        /// </summary>
        /// <param name="equipe"></param>
        /// <returns></returns>
       bool ResetaAssinatura(string equipe);
    }

}
