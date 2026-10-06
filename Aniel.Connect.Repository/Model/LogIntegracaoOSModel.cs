using System;

namespace Aniel.Connect.Repository.Model
{
    public class LogIntegracaoOSModel
    {
        public virtual String Servico { get; set; }
        public virtual String NumeroOS { get; set; }
        public virtual String Mensagem { get; set; }
        public virtual String Data { get; set; }
        public virtual String Hora { get; set; }
        public virtual MomentoImportacao? Momento { get; set; }
        public virtual String Status { get; set; }
        public virtual String Usuario { get; set; }
    }
}
