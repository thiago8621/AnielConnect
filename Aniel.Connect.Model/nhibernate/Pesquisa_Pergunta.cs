
using Sinapse.Model.enumerado;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Pesquisa_Pergunta : BaseModel
    {
                        
        public virtual String Descricao   { get; set; }

        public virtual String Escala_Linear_Ini  { get; set; }
        public virtual Padrao? Escala_Emoticos { get; set; }
        
        public virtual String Escala_Linear_Fin  { get; set; }
        
        public virtual Pesquisa_Satisfacao Pesquisa_Satisfacao  { get; set; }

        public virtual Int32 Ordem { get; set; }

        public virtual Boolean Obrigatorio { get; set; }

        public virtual Boolean Status_Pesquisa_Pergunta { get; set; }

        public virtual String Respostas_Possiveis { get; set; }

        public virtual Pesquisa_Tipo_Resposta Tipo_Resposta { get; set; }

    }
}
