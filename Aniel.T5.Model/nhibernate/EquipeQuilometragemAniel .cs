using Aniel.Connect.ModelLog.nhibernate;
using Resources;
using Sinapse.Model;
using Sinapse.Model.nhibernate;
using Sinapse.ModelLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.nhibernate
{
    public class EquipeQuilometragemAniel : BaseModel
    {
        public virtual Int64 Quantidade_OS { get; set; }
        public virtual EquipeAniel Equipe { get; set; }
        public virtual double DistanciaPlanejada { get; set; }
        public virtual double DistanciaRealizada { get; set; }
        public virtual double DistanciaRealRastreamento { get; set; }
        public virtual double TempoParalisadoMinutos { get; set; }
        public virtual DateTime Data { get; set; }

    }
}
