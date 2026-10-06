using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class TipoServico_Cobranca : BaseModel
    {
        public virtual string Segmento { get; set; }
        public virtual string Tipo_Servico { get; set; }
        //public virtual string Desc_Sub_Tipo_Servico { get; set; }
        public virtual string Sub_Tipo_Servico { get; set; }
        public virtual bool TodasSubTipoServico { get; set; }
        public virtual bool TodasCod_Encerramento { get; set; }
        public virtual string Cod_Encerramento { get; set; }
        public virtual Cobranca_Servico Cobranca_Servico { get; set; }
    }
}
