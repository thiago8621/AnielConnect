using Sinapse.Model;
using System.Linq;

namespace Aniel.Connect.Repository.ConsultaCredito
{
    public interface IConsultaCreditoRepository
    {
        /// <summary>
        ///     Retorna uma representação de uma consulta na TB_CONSULTA_SERASA
        /// </summary>
        IQueryable<ConsultaSerasaDados> GetAll();

        /// <summary>
        ///     Retorna a consulta de crédito associada ao ID informado
        /// </summary>
        /// <param name="id">ID da consulta de crédito</param>
        ConsultaSerasaDados GetById(long id);

        /// <summary>
        ///     Retorna a consulta de crédito mais atual feita para o cliente com o CPF informado
        /// </summary>
        /// <param name="cpf">CPF do cliente</param>
        ConsultaSerasaDados GetByCpf(string cpf);

        /// <summary>
        ///     Retorna a consulta de crédito mais atual feita para o cliente com o CNPJ informado
        /// </summary>
        /// <param name="cnpj">CNPJ do cliente</param>
        ConsultaSerasaDados GetByCnpj(string cnpj);
        
        /// <summary>
        ///     Grava uma nova consulta do Serasa no banco
        /// </summary>
        /// <param name="consultaSerasa">Objeto com os dados da consulta do Serasa</param>
        void Save(ConsultaSerasaDados consultaSerasa);
    }
}
