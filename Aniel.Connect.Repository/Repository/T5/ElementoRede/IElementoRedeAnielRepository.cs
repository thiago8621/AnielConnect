using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.Model.Hubsoft;
using Aniel.T5.Model.nhibernate;
using Sinapse.DAO.repository;
using Sinapse.Model;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.T5.ElementoRede
{
    public interface IElementoRedeAnielRepository
    {
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_ELEMENTO_REDE.
        /// </summary>
        IQueryable<Elemento_Rede> GetAll();
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_ELEMENTO_REDE_ENDERECOS.
        /// </summary>        
        IQueryable<Elemento_Rede_Enderecos> GetAllEnderecos();
        /// <summary>
        ///     Retorna uma consulta na TB_ELEMENTO_REDE_CAMPOS.
        /// </summary>        
        IQueryable<Elemento_RedeCampos> GetAllCamposDinamicos();        
        /// <summary>
        ///     Retorna uma consulta na TB_ELEMENTO_REDE_FIBRA.
        /// </summary>        
        IQueryable<Elemento_Rede_Fibra> GetAllFibras();
        /// <summary>
        ///     Retorna uma consulta na TB_ELEMENTO_REDE_ARQUIVO.
        /// </summary>
        /// <returns></returns>
        IQueryable<Elemento_RedeFotosArquivos> GetAllFiles();        
        /// <summary>
        ///     Retorna o registro do elemento de rede associado ao ID informado.
        /// </summary>
        /// <param name="id">Id do elemento de rede</param>
        Elemento_Rede GetById(long id);
        /// <summary>
        ///     Retorna uma listagem de Endereços associados ao registro do elemento de rede associado ao Id informado.
        /// </summary>
        /// <param name="id">Id do elemento de rede</param>
        /// <returns></returns>
        IEnumerable<Elemento_Rede_Enderecos> GetEnderecosByElement(long id);
       /// <summary>
       ///  Retorna uma listagem de campos dinamicos associados ao registros do elemento de rede associado ao Id informado.
       /// </summary>
       /// <param name="id"></param>
       /// <returns></returns>
        IEnumerable<Elemento_RedeCampos> GetCamposDinamicosByElement(long id);
        /// <summary>
        ///     Retorna uma listagem de fibras associadas ao registro de elemento de rede associado ao Id informado.
        /// </summary>
        /// <param name="id">Id do elemento de rede</param>
        /// <returns></returns>
        IEnumerable<Elemento_Rede_Fibra> GetFibrasByElement(long id);
        /// <summary>
        ///     Retorna uma listagem de arquivos associadas ao registro de elemento de rede associado ao Id informado.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IEnumerable<Elemento_RedeFotosArquivos> GetFilesByElement(long id);
        Elemento_Rede GetByDescription(string descricao);
        Tipo_Elemento_Rede GetTipoElementoRedeByCodAuxiliar(string codigoAuxiliar);
        Tipo_Elemento_Rede GetTipoElementoRedeById(long Id);
        IEnumerable<ImportacaoElementoHubSoft> ImportacaoIntegracao(string urlLogin, string elementoRede, LoginHubsoftModel login);
        void saveEnderecos(IEnumerable<Elemento_Rede_Enderecos> enderecos);
        void SaveOrUpdate(Elemento_Rede elemento);
        void SaveOrUpdatePorta(Elemento_Rede_Porta porta);
        /// <summary>
        ///     Retorna a representação de uma consulta na TB_ELEMENTO_REDE.
        /// </summary>
        IQueryable<Elemento_Rede_Porta> GetAllPortas();

    }
}
