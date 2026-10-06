using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Filtro_PainelServico : BaseModel
    {
        public virtual String Valor { get; set; }
        public virtual String ValorPai { get; set; }
        public virtual String ValorAvo { get; set; }
        public virtual Modelo_PainelServico Modelo_PainelServico { get; set; }
        public virtual Filtro_PainelServicoEnum Filtro { get; set; }

        /// <summary>
        /// USADO APENAS EM CAMPOS DE DATA. QUANDO ESSE CAMPO ESTA PREENCHIDO O CAMPO VALOR DEVE ESTAR NULO.
        /// </summary>
        public virtual ModeloPainel_Data? Data { get; set; }
        
    }
}
