using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Model.nhibernate
{
    public class EquipeVenda : BaseModel
    {
        public virtual String Descricao { get; set; }
        public virtual Status_Default Status { get; set; }

        #region Mobile
        private IList<SupervisorEquipeVenda> _supervisores;
        public virtual IList<SupervisorEquipeVenda> Supervisores
        {
            get
            {
                if (_supervisores == null)
                    _supervisores = new List<SupervisorEquipeVenda>();
                return _supervisores;
            }
            set
            {
                _supervisores = value;
            }


        }
        #endregion

    }
}