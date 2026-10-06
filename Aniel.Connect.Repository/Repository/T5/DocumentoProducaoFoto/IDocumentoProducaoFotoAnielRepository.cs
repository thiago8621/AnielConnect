using Aniel.Connect.Model.nhibernate;
using System.Collections.Generic;
using System.Linq;

namespace Aniel.Connect.Repository.T5.DocumentoProducao
{
    public interface IDocumentoProducaoFotoAnielRepository
    {
        /// <summary>
        ///     Retorna uma representação de uma consulta à tabela TB_ARQUIVO_DOCPROD.
        ///     no sistema
        /// </summary>
        IQueryable<Arquivo_DocProd> GetAllArquivos();
        IQueryable<Imagem_DocPord_Mobile> GetAllFotos();
        /// <summary>
        /// Realiza a gravação de registros na tabela TB_ARQUIVO_DOCPROD.
        /// </summary>
        /// <param name="arquivos">Lista de arquivos para serem gravados no banco.</param>
        void SaveOrUpdateArquivos(IEnumerable<Arquivo_DocProd> arquivos);
        void SaveOrUpdateArquivosSql(IEnumerable<Arquivo_DocProd> arquivos);
        void SaveOrUpdateFotos(IEnumerable<Imagem_DocPord_Mobile> fotos);
        void SaveOrUpdateFotosSql(IEnumerable<Imagem_DocPord_Mobile> fotos);
        long GetMaxIdArquivo();
        long GetMaxIdFoto();
        IEnumerable<Arquivo_DocProd> GetArquivoByChave(string codCt, string projeto, string numDoc);

    }
}
