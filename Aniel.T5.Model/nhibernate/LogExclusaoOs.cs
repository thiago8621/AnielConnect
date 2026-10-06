using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.nhibernate
{
    public class LogExclusaoOs
    {
        public virtual long Id { get; set; }
        public virtual DateTime Dia  { get; set; }
        public virtual DateTime DiaExclusao { get; set; }
        public virtual DateTime DataMaxima { get; set; }
        public virtual DateTime Hora { get; set; }
        public virtual DateTime HoraExclusao { get; set; }
        public virtual int NumDoc { get; set; }
        public virtual string NumObra { get; set; }
        public virtual string NumObraOriginal { get; set; }
        public virtual string CodCt { get; set; }
        public virtual string Projeto { get; set; }
        public virtual string Usuario { get; set; }
        public virtual string UsuarioExclusao { get; set; }
        public virtual string IpMaquinaOrigem { get; set; }
        public virtual string Hostname { get; set; }
        public virtual string obs { get; set; }


    }
}
