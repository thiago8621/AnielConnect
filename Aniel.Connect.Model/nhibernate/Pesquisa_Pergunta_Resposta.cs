using Aniel.Connect.Model.enumerado;
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
    public class Pesquisa_Pergunta_Resposta : BaseModel
    {

        public virtual Pesquisa_Resposta Pesquisa_Resposta { get; set; }
        
        public virtual Pesquisa_Pergunta Pesquisa_Pergunta { get; set; }
        
        public virtual String Resposta { get; set; }
        public virtual string RespostaTexto { get; set; }

    }
}
