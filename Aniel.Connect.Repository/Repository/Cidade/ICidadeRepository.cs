using System.Linq;

namespace Aniel.Connect.Repository.Cidade
{
    public interface ICidadeRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_CIDADE
        /// </summary>
        IQueryable<Aniel.Connect.Model.nhibernate.Cidade> GetAll();

        /// <summary>
        ///     Retorna a cidade registrada com o nome especificado
        /// </summary>
        /// <param name="nomeCidade">Nome da cidade</param>
        /// <param name="siglaUF">Sigla do estado onde a cidade está localizada</param>
        /// <param name="nomePais">Nome do país onde a cidade está localizada</param>
        Aniel.Connect.Model.nhibernate.Cidade GetByName(string nomeCidade, string siglaUF, string nomePais);
        Aniel.Connect.Model.nhibernate.Cidade GetById(string idCidade, string idUF);
        Aniel.Connect.Model.nhibernate.Cidade GetById(string idCidade);
        Aniel.Connect.Model.nhibernate.Cidade GetByNameUpperCase(string nomeCidade, string siglaUF);
    }
}
