using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.nhibernate
{
    public class Tipo_Servico_Equipe_ImovelAniel: BaseModel
    {
        public virtual Tipo_Servico_EquipeAniel Tipo_Servico { get; set; }
        public virtual Tipo_ImovelAniel Tipo_Imovel { get; set; }

        public virtual DateTime Horas { get; set; }
    }
}
