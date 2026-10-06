using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.nhibernate
{
    public class ConfigPerfilAtivacaoAniel
    {
        public virtual long Id { get; set; }
        public virtual string Codmat { get; set; }
        public virtual string Id_Perfil_Ativacao { get; set; }
        public virtual string Id_Perfil_Ativacao_Onu { get; set; }
        public virtual string Id_OLT { get; set; }
        public virtual string Usuario_Importacao { get; set; }
        public virtual DateTime? DataHora_Importacao { get; set; }
    }
}
