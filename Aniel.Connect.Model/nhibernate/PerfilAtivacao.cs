using Sinapse.Model;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class PerfilAtivacao : BaseModel
    {
        public virtual String Nome { get; set; }
        public virtual string LineProfile { get; set; }
        public virtual byte[] ScriptOLT { get; set; }
        public virtual byte[] ScriptOLTVoz { get; set; }

        public virtual String IpGerencia { get; set; }
        public virtual String Login { get; set; }
        public virtual String Senha { get; set; }
        public virtual Int32? PortaTelnet { get; set; }
        public virtual Int32? PortaHttp { get; set; }

        public virtual Int32? GemPort { get; set; }
        public virtual Int32? ServicePort { get; set; }
        public virtual Int32? SenorId { get; set; }

        public virtual byte[] ScriptONU { get; set; }
        public virtual bool ObrigaWifi { get; set; }
        private IList<Perfil_Ativacao_Grupo_Area> _gruposarea;
        public virtual IList<Perfil_Ativacao_Grupo_Area> GruposArea
        {
            get
            {
                if (_gruposarea == null)
                    _gruposarea = new List<Perfil_Ativacao_Grupo_Area>();
                return _gruposarea;
            }
            set
            {
                _gruposarea = value;
            }
        }
    }
}
