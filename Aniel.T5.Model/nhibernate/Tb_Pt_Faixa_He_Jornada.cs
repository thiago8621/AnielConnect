using Aniel.T5.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;

namespace Aniel.T5.Model.nhibernate
{
    [Serializable]
    public class Tb_Pt_Faixa_He_Jornada : BaseModel
    {

        public virtual Tb_Pt_Jornada Jornada { get; set; }
        //public virtual Enum_Condicao_Hora_Extra Condicao { get; set; }

        public virtual Enum_Condicao_Hora_Extra Condicao { get; set; }


        public virtual String Descricao { get; set; }
        public virtual Boolean Habilitado { get; set; }
        //public virtual String  Duracao { get; set; }

        public virtual DateTime? Duracao { get; set; }
        //public virtual String  Percentual { get; set; }

        public virtual Double? Percentual { get; set; }
        public virtual Double? Percentual_Apos { get; set; }
        public virtual Boolean Segunda { get; set; }

        //public virtual String Segunda { get; set; }
        public virtual Boolean Terca { get; set; }
        public virtual Boolean Quarta { get; set; }
        public virtual Boolean Quinta { get; set; }
        public virtual Boolean Sexta { get; set; }
        public virtual Boolean Sabado { get; set; }
        public virtual Boolean Domingo { get; set; }
        public virtual Boolean Feriado { get; set; }

    }
}