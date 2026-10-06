using System.Linq;

namespace Aniel.Connect.Repository.ConsultaViabilidade
{
    public interface IConsultaViabilidadeRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_CONSULTA_VIABILIDADE
        /// </summary>
        IQueryable<Sinapse.Model.ConsultaViabilidade> GetAll();

        /// <summary>
        ///     Retorna a consulta de viabilidade com o ID informado
        /// </summary>
        /// <param name="id">ID da consulta de viabilidade</param>
        Sinapse.Model.ConsultaViabilidade GetById(long id);

        /// <summary>
        ///     Retorna os dados da última consulta de viabilidade realizada para determinada latitude e longitude
        /// </summary>
        /// <param name="lat">Latitude do local</param>
        /// <param name="lon">Longitude do local</param>
        Sinapse.Model.ConsultaViabilidade GetConsultaViabilidadeByLatLong(double lat, double lon);

        /// <summary>
        ///     Insere os dados de uma nova consulta de viabilidade.
        /// </summary>
        /// <param name="consultaViabilidade">Objeto com os dados da consulta de viabilidade</param>
        void Save(Sinapse.Model.ConsultaViabilidade consultaViabilidade);
    }
}
