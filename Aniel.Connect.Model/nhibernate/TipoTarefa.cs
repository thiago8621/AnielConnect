using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;

namespace Aniel.Connect.Model.nhibernate
{
    public class TipoTarefa : IModel
    {
        public virtual long Id { get; set; }
        public virtual string Codigo { get; set; }
        public virtual Status_Default Status { get; set; }
        public virtual string Descricao { get; set; }
        public virtual string UsuarioCriacao { get; set; }
        public virtual DateTime? DataHoraCriacao { get; set; }
        public virtual string UsuarioAlteracao { get; set; }
        public virtual DateTime? DataHoraAlteracao { get; set; }
        public virtual string Token { get; set; }
    }
}
