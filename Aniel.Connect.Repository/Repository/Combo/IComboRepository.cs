using System.Linq;

namespace Aniel.Connect.Repository.Combo
{
    public interface IComboRepository
    {
        /// <summary>
        ///     Retorna uma representação de uma consulta na TB_COMBO
        /// </summary>
        /// <returns></returns>
        IQueryable<Aniel.Connect.Model.nhibernate.Combo> GetAll();

        /// <summary>
        ///     Retorna o combo associado ao ID informado
        /// </summary>
        /// <param name="id">ID do combo</param>
        Aniel.Connect.Model.nhibernate.Combo GetById(long id);
        
        /// <summary>
        ///     Grava um registro de combo no banco
        /// </summary>
        /// <param name="combo">Objeto com as informações do combo</param>
        void Save(Aniel.Connect.Model.nhibernate.Combo combo);

        /// <summary>
        ///     Atualiza um registro de combo no banco
        /// </summary>
        /// <param name="combo">Objeto com as informações do combo</param>
        void Update(Aniel.Connect.Model.nhibernate.Combo combo);
    }
}
