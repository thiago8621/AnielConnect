using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.nhibernate;
using Sinapse.Model;
using System;
using System.Linq;

namespace Aniel.Connect.Repository.Repository.T5.Porta
{
    public interface IPortaAnielRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_AREA
        /// </summary>
        IQueryable<Elemento_Rede_Porta> GetAll();


        /// <summary>
        ///     Retorna a área associado ao código informado
        /// </summary>
        /// <param name="codEscritorio">código do grupo área</param>
        /// <param name="codArea">código da área</param>
        Elemento_Rede_Porta GetPortabyCliente( Int64 idCliente);

        void Update(Elemento_Rede_Porta elemento);


    }
}
