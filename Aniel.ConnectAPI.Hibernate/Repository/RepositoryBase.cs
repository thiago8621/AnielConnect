using NHibernate;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Aniel.Hibernate.Repository
{
    public class RepositoryBase : IDisposable
    {
        protected ISession Session { get; set; }

        protected RepositoryBase(ISession session)
        {
            Session = session;
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
