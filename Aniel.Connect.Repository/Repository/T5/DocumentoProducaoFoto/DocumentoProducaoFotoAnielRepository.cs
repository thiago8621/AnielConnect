using Aniel.Connect.Model.nhibernate;
using Aniel.Connect.Repository.T5.DocumentoProducao;
using NHibernate;
using Sinapse.DAO.repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aniel.Connect.Repository.Repository.T5.DocumentoProducaoFoto
{
    public class DocumentoProducaoFotoAnielRepository : IDocumentoProducaoFotoAnielRepository
    {
        private ISession _sessionT5;
        private ISession _sessionFoto;
        public DocumentoProducaoFotoAnielRepository(ISession sessionT5, ISession sessionFoto) 
        {
            this._sessionT5 = sessionT5;
            this._sessionFoto = sessionFoto;
        }
        public IQueryable<Arquivo_DocProd> GetAllArquivos()
        {
            using (var repoDoc = new Conexao<Documento_ProducaoAniel>(this._sessionT5)) 
            using (var repository = new Conexao<Arquivo_DocProd>(this._sessionFoto)) 
            {
                return repository.QueryableSelect();
            }
        }

        public IQueryable<Imagem_DocPord_Mobile> GetAllFotos()
        {
            using (var repoDoc = new Conexao<Documento_ProducaoAniel>(this._sessionT5))
            using (var repository = new Conexao<Imagem_DocPord_Mobile>(this._sessionFoto))
            {
                return repository.QueryableSelect();
            }
        }

        public IEnumerable<Arquivo_DocProd> GetArquivoByChave(string codCt, string projeto, string numDoc)
        {
            return this.GetAllArquivos()
                       .Where(p => p.Chave == String.Format("{0}&{1}&{2}", codCt, projeto, numDoc))
                       .OrderByDescending(p => p.Data);
        }

        public long GetMaxIdArquivo() 
        {
            return this.GetAllArquivos().Max(p => p.Id);
        }

        public long GetMaxIdFoto()
        {
            return this.GetAllArquivos().Max(p => p.Id);
        }

        public void SaveOrUpdateArquivos(IEnumerable<Arquivo_DocProd> arquivos)
        {
            using (var repository = new Conexao<Arquivo_DocProd>(this._sessionFoto))
            {                
                repository.SaveOrUpdate(arquivos).Wait();
            }
        }

        public void SaveOrUpdateArquivosSql(IEnumerable<Arquivo_DocProd> arquivos)
        {
            using (var trans = this._sessionFoto.BeginTransaction()) 
            { 
                foreach(var arquivo in arquivos)
                {
                    using (var com = this._sessionFoto.Connection.CreateCommand()) 
                    {

                        var id = GetMaxIdArquivo()+1;
                        StringBuilder sql = new StringBuilder();
                        sql.Append("INSERT INTO TB_ARQUIVO_DOCPROD(ID, CHAVE, NUM_OBRA, ");
                        sql.Append("TIPO, ARQUIVO, USUARIO, DATA, HORA) ");
                        sql.Append("values( ");
                        sql.Append(""+id+",@CHAVE, @NUM_OBRA, @TIPO, @ARQUIVO, ");
                        sql.Append("@USER, CURRENT_DATE, CURRENT_TIME) ");
                        
                        Dictionary<string, object> parametros = new Dictionary<string, object>();

                        parametros.Add("@CHAVE", arquivo.Chave);                    
                        parametros.Add("@NUM_OBRA", arquivo.Num_Obra);
                        parametros.Add("@TIPO", arquivo.Tipo);
                        parametros.Add("@ARQUIVO", arquivo.Arquivo.Length == 0 ? null : arquivo.Arquivo);
                        parametros.Add("@USER", arquivo.Usuario);

                        trans.Enlist(com);
                        CommandHelper.ExecuteNonQuery(com, sql, parametros, this._sessionFoto.Connection, com.Transaction).Wait();
                    }
                }
                trans.Commit();
            }
        }

        public void SaveOrUpdateFotos(IEnumerable<Imagem_DocPord_Mobile> fotos)
        {
            using (var repository = new Conexao<Imagem_DocPord_Mobile>(this._sessionFoto))
            {
                repository.SaveOrUpdate(fotos).Wait();
            }
        }

        public void SaveOrUpdateFotosSql(IEnumerable<Imagem_DocPord_Mobile> fotos)
        {
            using (var trans = this._sessionFoto.BeginTransaction())
            {
                foreach (var foto in fotos)
                {
                    using (var com = this._sessionFoto.Connection.CreateCommand())
                    {
                        StringBuilder sql = new StringBuilder();
                        sql.Append("INSERT INTO TB_IMAGEM_DOCPROD_MOBILE(CODCT, PROJETO, NUM_OBRA, NUM_DOC, ");
                        sql.Append("TIPO, ARQUIVO, USUARIO, DIA, HORA) ");
                        sql.Append("values( ");
                        sql.Append("@CODCT, @PROJETO, @NUM_OBRA, @NUM_DOC, @TIPO, @ARQUIVO, ");
                        sql.Append("@USER, CURRENT_DATE, CURRENT_TIME) ");

                        Dictionary<string, object> parametros = new Dictionary<string, object>();

                        parametros.Add("@CODCT", foto.CodCt);
                        parametros.Add("@PROJETO", foto.Projeto);
                        parametros.Add("@NUM_OBRA", foto.Num_Obra);
                        parametros.Add("@NUM_DOC", foto.Num_Doc);
                        parametros.Add("@TIPO", foto.Tipo);
                        parametros.Add("@ARQUIVO", foto.Arquivo.Length == 0 ? null : foto.Arquivo);
                        parametros.Add("@USER", foto.Usuario);

                        trans.Enlist(com);
                        CommandHelper.ExecuteNonQuery(com, sql, parametros, this._sessionFoto.Connection, com.Transaction).Wait();
                    }
                }
                trans.Commit();
            }
        }
    }
}
