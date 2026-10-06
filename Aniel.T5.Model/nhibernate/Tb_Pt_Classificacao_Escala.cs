using System;
using System.Collections.Generic;
using System.Linq;
using Aniel.T5.Model.enumerado;
using Sinapse.Core;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;

namespace Aniel.T5.Model.nhibernate
{
    [Serializable]
    public class Tb_Pt_Classificacao_Escala : BaseModel
    {
        public virtual String Descricao { get; set; }
        public virtual String Sigla { get; set; }
        public virtual Int16? Interno { get; set; }
        public virtual Boolean? Permitir_Marcar { get; set; }
        public virtual Boolean? Enviar_Notificacao { get; set; }
        public virtual String Cor { get; set; }
        public virtual Int16? Escala_Especial { get; set; }
        public virtual Int16? Gera_Movimento { get; set; }
        public virtual Int16? Abonar_Debito { get; set; }
        public virtual Boolean Debita_Credita { get; set; }
        public virtual Boolean Falta { get; set; }
        public virtual Int16? Lancado_Gestor { get; set; }
        public virtual Status_Default? Status { get; set; }

        public virtual Int16? Evento { get; set; }

        public virtual SituacaoFuncionarioAniel? Situacao { get; set; }
        public virtual Boolean? PermtiteRoteirizar { get; set; }
    }
}
