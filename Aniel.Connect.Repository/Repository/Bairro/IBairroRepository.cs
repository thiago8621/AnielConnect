using System.Linq;

namespace Aniel.Connect.Repository.Bairro
{
    public interface IBairroRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_BAIRRO
        /// </summary>
        IQueryable<Aniel.Connect.Model.nhibernate.Bairro> GetAll();

        /// <summary>
        ///     Retorna o bairro registrado com o nome especificado
        /// </summary>
        /// <param name="nomeBairro">Nome do bairro</param>
        /// <param name="nomeCidade">Nome da cidade onde o bairro está localizado</param>
        /// <param name="siglaUF">Sigla da UF onde o bairro está localizado</param>
        /// <param name="nomePais">Nome do país onde o bairro está localizado</param>
        Aniel.Connect.Model.nhibernate.Bairro GetByName(
            string nomeBairro, 
            string nomeCidade, 
            string siglaUF, 
            string nomePais
        );

        Aniel.Connect.Model.nhibernate.Bairro GetById(string idBairro, string idCidade, string idUF);
    }
}
