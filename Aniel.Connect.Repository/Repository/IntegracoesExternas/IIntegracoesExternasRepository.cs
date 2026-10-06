using System.Linq;

namespace Aniel.Connect.Repository.IntegracoesExternas
{
    public interface IIntegracoesExternasRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_INTEGRACOES_EXTERNAS
        /// </summary>
        IQueryable<Aniel.Connect.Model.nhibernate.IntegracoesExternas> GetAll();


    }
}
