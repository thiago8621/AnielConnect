using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Model.nhibernate
{
    public class Ponto_Acesso_Radio : BaseModel
    {
        public virtual String Descricao { get; set; }

        public virtual String Mac { get; set; }
        public virtual String Codigo_Auxiliar { get; set; }
        public virtual String Ip { get; set; }
        public virtual Status_Default Status { get; set; }

        private IList<Ponto_Acesso_Radio_Grupo_Area> _gruposarea;
        public virtual IList<Ponto_Acesso_Radio_Grupo_Area> GruposArea
        {
            get
            {
                if (_gruposarea == null)
                    _gruposarea = new List<Ponto_Acesso_Radio_Grupo_Area>();
                return _gruposarea;
            }
            set
            {
                _gruposarea = value;
            }
        }
    }

}