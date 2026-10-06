using System.Linq;
using Aniel.T5.Model.nhibernate;

namespace Aniel.Connect.Repository.T5.CodigoEncerramento
{
    public interface ICodigoEncerramentoAnielRepository
    {
        /// <summary>
        ///     Retorna uma representação de uma consulta na tabela TB_CODIGO_ENCERRAMENTO
        /// </summary>
        IQueryable<Codigo_EncerramentoAniel> GetAll();


        /// <summary>
        ///     Retorna a descrição de um registro de encerramento
        /// </summary>
        /// <param name="codEncerramento"> Codigo de encerramento</param>
        /// <param name="codCt">Codigo do contrato</param>
        string GetDescricao(string codEncerramento, string codCt);

        Codigo_EncerramentoAniel GetFirstCodigoEncerramento (string codEncerramento, string codCt);
    }
}
