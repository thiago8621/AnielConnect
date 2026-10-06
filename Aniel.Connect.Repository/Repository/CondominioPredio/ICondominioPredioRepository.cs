using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.repository;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.CondominioPredio
{
    public interface ICondominioPredioRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_CONDOMINIO_PREDIO
        /// </summary>
        IQueryable<Condominio_Predio> GetAll();
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_CONDOMINIO_PREDIO_BLOCO        
        /// </summary>        
        IQueryable<Condominio_PredioBloco> GetAllBlocos();
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_CONDOMINIO_PREDIO_APARTAMENTO
        /// </summary>        
        IQueryable<Condominio_PredioApartamento> GetAllApartamentos();

        /// <summary>
        ///     Retorna o condomínio associado ao ID informado
        /// </summary>
        /// <param name="id">ID do condomínio</param>
        Condominio_Predio GetById(long id);        
        /// <summary>
        ///     Retorna uma listagem de Blocos por Condominio.
        /// </summary>
        /// <param name="id"></param>        
        IEnumerable<Condominio_PredioBloco> GetBlocosByCondominio(long id);
        /// <summary>
        ///     Retorna uma listagem de Apartamentos por Condominio.
        /// </summary>
        /// <param name="id"></param>        
        IEnumerable<Condominio_PredioApartamento> GetApartamentosByBloco(long id);
        /// <summary>
        ///     Retorna uma listagem de Apartamentos por Condominio.
        /// </summary>
        /// <param name="id"></param>        
        IEnumerable<Condominio_PredioApartamento> GetApartamentosByCondominio(long id);
        void save(Condominio_Predio condominio);
        void saveBlocos(Condominio_PredioBloco bloco);
        void saveApartamentos(IEnumerable<Condominio_PredioApartamento> apartamento);
    }
}
