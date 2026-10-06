using Aniel.Connect.Model.enumerado;
using Aniel.T5.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.nhibernate
{
    public class FeriadoAniel : BaseModel
    {
        public virtual DateTime? Data { get; set; }
        public virtual String Descricao { get; set; }
        public virtual Abrangencia? Abrangencia { get; set; }
        public virtual Status_Default Status { get; set; }
        public virtual Tipo_Feriado? Tipo_Feriado { get; set; }

        public virtual Int16 Ano { get; set; }

        public virtual Int16 Mes { get; set; }
        public virtual Int16 Dia { get; set; }

        public virtual String Pais { get; set; }
        public virtual String UF { get; set; }
        public virtual String Cidade { get; set; }
        public virtual Int64? Id_Municipio { get; set; }
    }
}
