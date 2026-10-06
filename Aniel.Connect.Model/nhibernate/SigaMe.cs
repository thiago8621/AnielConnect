using Sinapse.Model.nhibernate;
using System;

namespace Aniel.Connect.Model.nhibernate
{
    public class SigaMe : BaseModel
    {
       public virtual int NumeroOrigem { get; set; }
       public virtual int NumeroDestino{ get; set; }
       public virtual bool DefinirHorario { get; set; }
       public virtual bool Segunda { get; set; }
       public virtual bool Terca { get; set; }
       public virtual bool Quarta { get; set; }
       public virtual bool Quinta { get; set; }
       public virtual bool Sexta { get; set; }
       public virtual bool Sabado { get; set; }
       public virtual bool Domingo { get; set; }       
       public virtual DateTime HorarioInicial { get; set; }
       public virtual DateTime HorarioFinal { get; set; }
       public virtual Plano Plano { get; set; }

    }        
}
