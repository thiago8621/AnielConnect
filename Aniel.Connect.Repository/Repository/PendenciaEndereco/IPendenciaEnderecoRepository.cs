using System.Linq;

namespace Aniel.Connect.Repository.PendenciaEndereco
{
    public interface IPendenciaEnderecoRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_PENDENCIA_ENDERECO
        /// </summary>
        IQueryable<Sinapse.Model.PendenciaEndereco> GetAll();

        /// <summary>
        ///     Retorna a consulta de pendência de endereço com o ID informado
        /// </summary>
        /// <param name="id">ID do registro da consulta</param>
        Sinapse.Model.PendenciaEndereco GetConsultaPendenciaEnderecoById(long id);

        /// <summary>
        ///     Retorna a consulta de pendência de endereço mais recente registrada no sistema para determinado endereço
        /// </summary>
        /// <param name="logradouro">Logradouro do endereço</param>
        /// <param name="numero">Número do endereço</param>
        Sinapse.Model.PendenciaEndereco GetConsultaPendenciaEndereco(string logradouro, string numero);

        /// <summary>
        ///     Insere uma consulta de pendência de endereço no banco
        /// </summary>
        /// <param name="consultaPendencia">Objeto com os dados da consulta de pendencia de endereço</param>
        void Save(Sinapse.Model.PendenciaEndereco consultaPendencia);
    }
}
