using Aniel.T5.Model.nhibernate;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Aniel.Connect.Repository.T5.Funcionario
{
    public interface IFuncionarioAnielRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta à TB_FUNCIONARIO
        /// </summary>
        /// <returns></returns>
        IQueryable<FuncionarioAniel> GetAll();

        /// <summary>
        ///     Retorna o funcionário associado à matrícula informada
        /// </summary>
        /// <param name="numFre">Matrícula do funcionário</param>
        FuncionarioAniel GetByNumFre(string numFre);

        /// <summary>
        ///     Obtem os emails de Funcionarios dado um tipo de evento
        /// </summary>
        /// <param name="tipoEvento"></param>
        /// <returns></returns>
        Task<List<string>> GetEmailsFuncionario(String tipoEvento);
    }
}
