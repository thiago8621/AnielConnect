using Aniel.T5.Model.nhibernate;
using System.Linq;

namespace Aniel.Connect.Repository.T5.SubtipoServico
{
    public interface ISubtipoServicoAnielRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="descricao"></param>
        /// <param name="codTipoServ"></param>
        /// <returns></returns>
        Sub_Tipo_ServicoAniel GetByDescricaoTipoServ(string descricao, short codTipoServ);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codSubtipo"></param>
        /// <param name="codTipo"></param>
        /// <returns></returns>
        Sub_Tipo_ServicoAniel GetByCodigo(string codSubtipo, short codTipo);
    }
}
