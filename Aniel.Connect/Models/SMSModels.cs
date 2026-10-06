using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Models
{
    public class EnviarSMS_RequestModel
    {
        public virtual bool Salvar_Em_Banco { get; set; }
        public virtual string Mensagem { get; set; }
        public virtual string Num_Celular { get; set; }
        public virtual bool isReenvio { get; set; }
        public virtual long? Cliente { get; set; }
        public virtual Int64? Id_Integracao { get; set; }
        public virtual Tipo_Integracao_SMS Tipo_Integracao { get; set; }
    }

    public class EnviarSMS_ResponseModel
    {
        public virtual Int64? Id_Integracao { get; set; }
        public virtual bool Sucesso { get; set; }
        public virtual string MensagemErro { get; set; }
        public virtual List<String> MensagensErro { get; set; }
        public virtual string Mensagem { get; set; }
    }

    public class EnviarLoteSMS_RequestModel
    {
        public virtual bool Salvar_Em_Banco { get; set; }
        public virtual Tipo_Integracao_SMS Tipo_Integracao { get; set; }
        public virtual List<EnviarSMS_RequestModel> Mensagens { get; set; }
    }

    public class EnviarLoteSMS_ResponseModel
    {
        public virtual bool Sucesso { get; set; }
        public virtual string MensagemErro { get; set; }
        public virtual List<EnviarSMS_ResponseModel> Retorno { get; set; }
    }


}