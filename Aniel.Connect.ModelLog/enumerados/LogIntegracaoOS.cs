using Sinapse.Model.nhibernate;
using System;

namespace Aniel.Connect.ModelLog.enumerados
{
    public class LogIntegracaoOS : BaseLog
    {
        public virtual String Servico { get; set; }
        public virtual String NumeroOS{ get; set; }
        public virtual String Mensagem { get; set; }
        public virtual DateTime? Data { get; set; }
        public virtual DateTime? Hora { get; set; }
        public virtual MomentoImportacao? Momento { get; set; }
        public virtual Boolean? Status { get; set; }
        public virtual string Projeto { get; set; }
        public virtual string Cod_CT { get; set; }
        public virtual string Json_Envio { get; set; }
        public virtual string Json_Retorno { get; set; }


        public virtual int Num_Doc { get; set; }
    }
}
