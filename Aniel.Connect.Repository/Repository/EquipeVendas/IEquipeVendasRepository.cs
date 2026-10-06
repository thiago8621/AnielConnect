using System.Linq;

namespace Aniel.Connect.Repository.EquipeVendas
{
    public interface IEquipeVendasRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_CIDADE
        /// </summary>
        IQueryable<Aniel.Connect.Model.nhibernate.EquipeVenda> GetAll();

        /// <summary>
        ///     Retorna a cidade registrada com o nome especificado
        /// </summary>
        /// <param name="nomeCidade">Nome da cidade</param>
        /// <param name="siglaUF">Sigla do estado onde a cidade está localizada</param>
        /// <param name="nomePais">Nome do país onde a cidade está localizada</param>
        Aniel.Connect.Model.nhibernate.EquipeVenda GetById(string idEquipe);
    }
}
