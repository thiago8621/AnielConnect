using System.Linq;

namespace Aniel.Connect.Repository.Uf
{
    public interface IUfRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_UF
        /// </summary>
        /// <returns></returns>
        IQueryable<Aniel.Connect.Model.nhibernate.Uf> GetAll();

        /// <summary>
        ///     Retorna a UF registrada com a sigla informada
        /// </summary>
        /// <param name="siglaUf">Sigla da UF</param>
        /// <param name="nomePais">Nome do país onde se localiza a UF</param>
        Aniel.Connect.Model.nhibernate.Uf GetBySigla(string siglaUf, string nomePais);
    }
}
