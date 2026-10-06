using System;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.Feriado
{
    public interface IFeriadoRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_FERIADO
        /// </summary>
        IQueryable<Aniel.Connect.Model.nhibernate.Feriado> GetAll();

        /// <summary>
        ///     Retorna o feriado associado ao ID informado
        /// </summary>
        /// <param name="id">ID do feriado</param>
        Aniel.Connect.Model.nhibernate.Feriado GetById(long id);

        /// <summary>
        ///     Retorna os feriados registrados para a data informada, caso haja algum
        /// </summary>
        /// <param name="dia">Data dos feriados</param>
        /// <param name="uf">Sigla da UF para poder identificar os feriados estaduais para a data informada</param>
        /// <param name="cidade">Nome da Cidade para poder identificar os feriados municipais para a data informada</param>
        IEnumerable<Aniel.Connect.Model.nhibernate.Feriado> GetFeriadosDia(DateTime dia, string uf, string cidade);
    }
}
