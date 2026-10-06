using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aniel.Connect.Model.enumerado;
using Aniel.T5.Model.enumerado;


namespace Aniel.T5.Model.nhibernate
{
    public class MonitorAcesso
    {
        public virtual String Usuario { get; set; }
        public virtual String Hostname { get; set; }
        public virtual DateTime? DataEntrada { get; set; }
        public virtual DateTime? HoraEntrada { get; set; }
        public virtual DateTime? DataSaida { get; set; }
        public virtual DateTime? HoraSaida { get; set; }
        public virtual string Ativo { get; set; }
        public virtual long ID { get; set; }
        public virtual DateTime? DataPing { get; set; }
        public virtual DateTime? HoraPing { get; set; }
        public virtual String Build { get; set; }
        public virtual String IpLocal { get; set; }
        public virtual String Recurso { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            MonitorAcesso other = obj as MonitorAcesso;
            if (other == null)
                return false;
            if (Usuario == other.Usuario && Hostname == other.Hostname && DataEntrada == other.DataEntrada && HoraEntrada == other.HoraEntrada)
                return true;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return (Usuario + "|" + Hostname + "|" + DataEntrada + "|" + HoraEntrada).GetHashCode();
        }

    }


}
