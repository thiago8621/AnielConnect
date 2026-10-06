using Aniel.Connect.Model.nhibernate;
using System.Linq;

namespace Aniel.Connect.Repository.TipoImovel
{
    public interface ITipoImovelRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_TIPO_IMOVEL
        /// </summary>
        IQueryable<Tipo_Imovel> GetAll();

        /// <summary>
        ///     Retorna o tipo de imóvel associado ao código de tipo de imóvel informado
        /// </summary>
        /// <param name="codTipoImovel">Código de tipo de imóvel</param>
        Tipo_Imovel GetByCodTipoImovel(int codTipoImovel);
    }
}
