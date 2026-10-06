using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aniel.ConnectAPI.Model.Atributo;
using Aniel.ConnectAPI.Dal.Helper;
using System.Reflection;

namespace Aniel.ConnectAPI.Dal
{
    public class DaoBase : IDisposable
    {
        private FbConnection _conn;
        private FbTransaction _transaction;

        protected DaoBase()
        {

        }

        public DaoBase(DbConnection pConnection, DbTransaction pTransaction)
        {
            _conn = (FbConnection)pConnection;
            _transaction = (FbTransaction)pTransaction;
        }

        protected FbConnection GetConnection()
        {
            return _conn;
        }

        protected FbTransaction GetTransaction()
        {
            return _transaction;
        }

        public void AdicionaRegistroPV(Type tipo)
        {
            object[] atributos = tipo.GetCustomAttributes(typeof(PVTabelaAttribute), false);

            if (atributos.Count() > 0)
            {
                PVTabelaAttribute pvtabela = (PVTabelaAttribute)atributos[0];

                List<PropertyInfo> chaves = new List<PropertyInfo>();
                foreach (PropertyInfo prop in tipo.GetProperties())
                {
                    if (prop.GetCustomAttributes(typeof(PVChave)).Count() > 0)
                    {
                        chaves.Add(prop);
                    }
                }

                AtualizaPV(tipo.Name, pvtabela.GetTabela(), chaves);
            }

        }

        protected void AtualizaPV(String NomeTabela, String NomeTabelaPV, List<PropertyInfo> Chaves)
        {
            StringBuilder sql = new StringBuilder();

            List<String> _chaves = new List<String>();
            List<String> _chavesPV = new List<String>();
            foreach (PropertyInfo prop in Chaves)
            {
                _chaves.Add(prop.Name);

                String _field = ((PVChave)prop.GetCustomAttributes(typeof(PVChave)).ToList()[0]).GetField();
                _chavesPV.Add(_field);
            }           

            sql.AppendLine("SELECT " + String.Join(',', _chaves));
            sql.AppendLine(String.Format("FROM {0} A", NomeTabela));

            FbDataReader reader = CommandHelper.ExecuteReader(sql, new Dictionary<String, String>(), GetConnection(), GetTransaction());

            while (reader.Read())
            {
                StringBuilder sqlPV = new StringBuilder();
                sqlPV.AppendLine(String.Format("INSERT INTO {0} (ACAO, USUARIO, DIA, HORA, EXECUTADO, {1})", NomeTabelaPV, String.Join(',', _chavesPV)));

                Dictionary<String, Object> parametros = new Dictionary<String, Object>();
                Int32 i = 0;
                foreach (PropertyInfo prop in Chaves)
                {
                    parametros.Add(String.Concat("@p", i), reader[prop.Name]);
                    i++;
                }
                sqlPV.AppendLine(String.Format("VALUES('I', 'DBA', CURRENT_DATE, CURRENT_TIME, 'N', {0})", String.Join(',', parametros.Keys)));

                CommandHelper.ExecuteNonQuery(sqlPV, parametros, GetConnection(), GetTransaction());
            }

            
        }

        #region IDisposable Support
        private Boolean disposedValue;// To detect redundant calls

        // IDisposable
        protected virtual void Dispose(Boolean disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {

                }

                // TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                // TODO: set large fields to null.
            }
            this.disposedValue = true;
        }

        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
