using System;

namespace Aniel.Connect.API_Aniel.Models
{
    public class OnuList
    {
        public virtual String Onu { get; set; }
    }
    
    public class OnuRequest : ApiRequest
    {
        /// <summary>
        /// Codigo do Plano
        /// </summary>
        public virtual String Cod_Plano { get; set; }
        /// <summary>
        /// Cpf ou Cnpj
        /// </summary>
        public virtual String Cpf_Cnpj { get; set; }
    }

    public class AtivaEquipamento
    {
        public virtual long IdOLT { get; set; }
        public virtual long IdPON { get; set; }
        public virtual long IdONU { get; set; }

        public virtual string ScriptOLT { get; set; }
        public virtual string ScriptONU { get; set; }
    }
}