using Sinapse.Model.nhibernate;
using System;

namespace Aniel.Connect.ModelLog.nhibernate
{
    public class Log_RoteirizacaoAniel : BaseModel
    {
        public virtual DateTime? DataHoraFinalizacao { get; set; }

        public virtual StatusRoteirizacao? Status { get; set; }
        public virtual bool Simulacao { get; set; }
        public virtual DateTime? DataRoteirizacao { get; set; }
        public virtual string ConfiguracoesGerais { get; set; }
        public virtual string ConfiguracoesRoteirizacao { get; set; }
        public virtual string TecnicosRoteirizacao { get; set; }
        public virtual string DocumentosValidos { get; set; }


    }
}
