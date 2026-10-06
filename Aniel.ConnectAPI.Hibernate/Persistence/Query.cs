using NHibernate;
using NHibernate.Transform;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnielTV.Hibernate.Persistence
{
    /// <summary>
    /// Query com select feito pelo programador
    /// </summary>
    public class Query : IDisposable
    {
        private string sql;
        private Dictionary<string, object> parametros;
        private ISession consulta;
        private bool transaction = false;
        private ISession _session;

        public Query(ISession session)
        {
            _session = session;
        }

        public void startTransaction()
        {
            transaction = true;
        }

        public void commitTransaction()
        {
            if (consulta != null)
                consulta.Transaction.Commit();
        }

        public void rollbackTransaction()
        {
            if (consulta != null)
                consulta.Transaction.Rollback();
        }

        public void Select(string consulta)
        {
            this.sql = consulta;
        }

        public void setParametro(string nome, object valor)
        {
            if (parametros == null) parametros = new Dictionary<string, object>();
            if (!parametros.ContainsKey(nome))
            {
                if ((valor == null) || ((valor is string) && (string.IsNullOrEmpty((string)valor)))) parametros.Add(nome, null);
                else parametros.Add(nome, valor);
            }
        }

        public List<Hashtable> Values(string consulta)
        {
            this.Select(consulta);
            return this.Values();
        }

        private IQuery mount()
        {
            if (consulta == null)
            {
                consulta = _session;
                if (transaction) consulta.BeginTransaction();
            }

            var sqlQuery = consulta.CreateSQLQuery(this.sql);
            if (parametros != null)
            {
                foreach (string key in parametros.Keys)
                {
                    if (sqlQuery.NamedParameters.Where(p => p.Equals(key)).Count() > 0)
                    {
                        if (parametros[key] == null) sqlQuery.SetString(key, null);
                        else sqlQuery.SetParameter(key.Replace(":", ""), parametros[key]);
                    }
                }
            }

            return sqlQuery.SetResultTransformer(NHibernate.Transform.Transformers.AliasToEntityMap);
        }

        public List<Hashtable> Values()
        {
            List<Hashtable> retono = new List<Hashtable>();
            retono = new List<Hashtable>(this.mount().List<Hashtable>());
            this.clear();
            return retono;
        }

        public void UpdateOrInsert()
        {
            this.mount().ExecuteUpdate();
            this.clear();
        }        

        private void clear()
        {
            this.sql = string.Empty;
            this.parametros = null;
        }

        #region Disposable
        bool disposed = false;

        // método privado para controle
        // da liberação dos recursos
        private void Dispose(bool disposing)
        {
            // Verifique se Dispose já foi chamado.
            if (!this.disposed)
            {
                if (disposing)
                {
                    // Liberando recursos gerenciados

                }

                // Seta a variável booleana para true,
                // indicando que os recursos já foram liberados
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

            if ((transaction) && (consulta != null) && (consulta.Transaction != null) && (consulta.Transaction.IsActive))
                consulta.Transaction.Commit();
        }
        #endregion

    }
}
