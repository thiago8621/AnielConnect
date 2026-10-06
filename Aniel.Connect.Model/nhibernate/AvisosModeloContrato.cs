using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;

namespace Aniel.Connect.Model.nhibernate
{
    public class AvisoModeloContrato : IModel
    {
        public virtual long IdModeloContrato { get; set; }
        public virtual int QuantidadeDias { get; set; }
        public virtual long? IdEmail { get; set; }
        public virtual long? IdSMS { get; set; }
        public virtual long? IdNotificacao { get; set; }
        public virtual TipoAvisoModeloContratoEnum TipoAviso { get; set; }
        public virtual long Id { get; set; }
        public virtual string UsuarioCriacao { get; set; }
        public virtual DateTime? DataHoraCriacao { get; set; }
        public virtual string UsuarioAlteracao { get; set; }
        public virtual DateTime? DataHoraAlteracao { get; set; }
        public virtual string Token { get; set; }
    }
}
