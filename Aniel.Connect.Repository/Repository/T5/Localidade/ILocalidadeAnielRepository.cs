using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.nhibernate;
using System;
using System.Linq;

namespace Aniel.Connect.Repository.T5.Localidade
{
    public interface ILocalidadeAnielRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_LOCALIDADE
        /// </summary>
        IQueryable<LocalidadeAniel> GetAll();

        /// <summary>
        ///     Retorna a área associado ao código informado
        /// </summary>
        /// <param name="codEscritorio">código do grupo área</param>
        /// <param name="codArea">código da área</param>
        /// <param name="codLocalidade">código da localidade</param>
        LocalidadeAniel GetByCodigo(string codEscritorio, string codArea, int codLocalidade);

        /// <summary>
        /// Retorna dados da localidade filtrando por cidade(area) e bairro(localidade)
        /// </summary>
        /// <param name="Cidade"></param>
        /// <param name="Bairro"></param>
        /// <returns></returns>
        LocalidadeResponse GetByCidadeBairro(String Cidade, String Bairro);
        LocalidadeResponse GetByLocalidade(String Cidade, String Bairro, Config_Aniel_Integrator Config = null);
    }
}
