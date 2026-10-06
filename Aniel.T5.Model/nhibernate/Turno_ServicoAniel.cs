using Sinapse.Model.enumerados;
using System;

namespace Aniel.T5.Model.nhibernate
{
    public class Turno_ServicoAniel
    {
        public virtual int Id { get; set; }
        public virtual string Descricao { get; set; }
        public virtual DateTime? Hora_Ini { get; set; }
        public virtual DateTime? Hora_Fim { get; set; }
        public virtual Status_Default Status { get; set; }
    }
}
