using Aniel.Connect.Repository.Model;
using Aniel.T5.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.T5.LimiteAgendamento
{
    public interface ILimiteAgendamentoAnielRepository
    {
        /// <summary>
        ///     Representa uma consulta na TB_LIMITE_AGENDAMENTO no banco do Aniel
        /// </summary>
        IQueryable<LimiteAgendamentoAniel> GetAll();

        /// <summary>
        ///     Grava um registro na TB_LIMITE_AGENDAMENTO
        /// </summary>
        /// <param name="limite">Objeto com os dados do limite que será gravado</param>
        void Save(LimiteAgendamentoAniel limite);

        /// <summary>
        ///     Atualiza um registro da TB_LIMITE_AGENDAMENTO
        /// </summary>
        /// <param name="limite">Objeto com os dados do limite que será atualizado</param>
        void Update(LimiteAgendamentoAniel limite);

        /// <summary>
        ///     Remove um registro da TB_LIMITE_AGENDAMENTO
        /// </summary>
        /// <param name="limite">Objeto com os dados do limite que será removido</param>
        void Remove(LimiteAgendamentoAniel limite);

        /// <summary>
        ///     Retorna o percentual de uma natureza configurado para uma data dentro da área especificada
        /// </summary>
        /// <param name="codEscritorio">Código do grupo área</param>
        /// <param name="codArea">Código da área</param>
        /// <param name="idNatureza">ID da natureza que será verificada</param>
        /// <param name="data">Data para verificar o percentual configurado</param>
        LimiteAgendamentoAniel GetPercNaturezaByAreaData(
            string codEscritorio, 
            string codArea, 
            int idNatureza, 
            DateTime data
        );

        /// <summary>
        ///     Retorna o percentual de uma natureza configurado para uma data dentro da área especificada
        /// </summary>
        /// <param name="codEscritorio">Código do grupo área</param>
        /// <param name="codArea">Código da área</param>
		/// <param name="data">Data para verificar o percentual configurado</param>
        /// <param name="idNatureza">ID da natureza que será verificada</param>
        List<LimiteAgendamentoAniel> GetPercNaturezaByArea(
            string codEscritorio,
            string codArea,
            int idNatureza
        );

        /// <summary>
        ///     Retorna o percentual de uma natureza configurado para uma data dentro da área especificada
        /// </summary>
        /// <param name="codEscritorio">Código do grupo área</param>
        /// <param name="codArea">Código da área</param>
        /// <param name="data">Data para verificar o percentual configurado</param>
        List<LimiteAgendamentoAniel> GetPercByAreaData(
            string codEscritorio,
            string codArea,
            DateTime data
        );

        /// <summary>
        ///     Retorna os dados para o preenchimento da listagem de limite de agendamento
        /// </summary>
        /// <param name="filtros">Filtros passados pelo Datatables para serem aplicados na listagem</param>
        LimiteAgendamentoListResponse GetListagem(LimiteAgendamentoDataTableRequest filtros);

        
    }
}
