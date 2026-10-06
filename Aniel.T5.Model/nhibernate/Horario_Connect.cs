using Aniel.T5.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.nhibernate
{
    public class Horario_Connect : BaseModel
    {
        public virtual Status_Default Status { get; set; }

        public virtual Boolean Padrao { get; set; }

        public virtual String Codigo { get; set; }

        public virtual String Descricao { get; set; }

        public virtual Tipo_Hora_Connect Tipo { get; set; }

        public virtual Boolean Opcao_Trabalha_Feriado { get; set; }

        public virtual DateTime? Inicio_Feriado { get; set; }
        public virtual DateTime? Termino_Feriado { get; set; }
        public virtual DateTime? Inicio_Intervalo_Feriado { get; set; }
        public virtual DateTime? Termino_Intervalo_Feriado { get; set; }
        public virtual DateTime? Qtde_HE_Feriado { get; set; }

        public virtual DateTime? Inicio_Domingo { get; set; }
        public virtual DateTime? Termino_Domingo { get; set; }
        public virtual DateTime? Inicio_Intervalo_Domingo { get; set; }
        public virtual DateTime? Termino_Intervalo_Domingo { get; set; }
        public virtual DateTime? Qtde_HE_Domingo { get; set; }

        public virtual DateTime? Inicio_Segunda { get; set; }
        public virtual DateTime? Termino_Segunda { get; set; }
        public virtual DateTime? Inicio_Intervalo_Segunda { get; set; }
        public virtual DateTime? Termino_Intervalo_Segunda { get; set; }
        public virtual DateTime? Qtde_HE_Segunda { get; set; }

        public virtual DateTime? Inicio_Terca { get; set; }
        public virtual DateTime? Termino_Terca { get; set; }
        public virtual DateTime? Inicio_Intervalo_Terca { get; set; }
        public virtual DateTime? Termino_Intervalo_Terca { get; set; }
        public virtual DateTime? Qtde_HE_Terca { get; set; }

        public virtual DateTime? Inicio_Quarta { get; set; }
        public virtual DateTime? Termino_Quarta { get; set; }
        public virtual DateTime? Inicio_Intervalo_Quarta { get; set; }
        public virtual DateTime? Termino_Intervalo_Quarta { get; set; }
        public virtual DateTime? Qtde_HE_Quarta { get; set; }

        public virtual DateTime? Inicio_Quinta { get; set; }
        public virtual DateTime? Termino_Quinta { get; set; }
        public virtual DateTime? Inicio_Intervalo_Quinta { get; set; }
        public virtual DateTime? Termino_Intervalo_Quinta { get; set; }
        public virtual DateTime? Qtde_HE_Quinta { get; set; }

        public virtual DateTime? Inicio_Sexta { get; set; }
        public virtual DateTime? Termino_Sexta { get; set; }
        public virtual DateTime? Inicio_Intervalo_Sexta { get; set; }
        public virtual DateTime? Termino_Intervalo_Sexta { get; set; }
        public virtual DateTime? Qtde_HE_Sexta { get; set; }

        public virtual DateTime? Inicio_Sabado { get; set; }
        public virtual DateTime? Termino_Sabado { get; set; }
        public virtual DateTime? Inicio_Intervalo_Sabado { get; set; }
        public virtual DateTime? Termino_Intervalo_Sabado { get; set; }
        public virtual DateTime? Qtde_HE_Sabado { get; set; }

    }
}
