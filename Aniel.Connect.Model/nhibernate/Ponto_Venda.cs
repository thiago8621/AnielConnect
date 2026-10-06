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
    public class Ponto_Venda : BaseModel
    {
        public virtual String Nome { get; set; }

        //TAMANHO MAXIMO 50 CARACTERES
        public virtual String Codigo { get; set; }

        public virtual String Cep { get; set; }

        public virtual String Logradouro { get; set; }

        public virtual String Numero { get; set; }

        public virtual String Bairro { get; set; }

        public virtual String Cidade { get; set; }
        public virtual String Complemento { get; set; }
        public virtual Double? Latitude { get; set; }
        public virtual Double? Longitude { get; set; }
        public virtual String Uf { get; set; }
        public virtual Int64? Id_Bairro { get; set; }
        public virtual Int64? Id_Cidade { get; set; }
        public virtual Status_Default Status { get; set; }
    }

    public class Checkins_Vendedor_Pdv : Ponto_Venda
    {
        public virtual Checkin_Vendedor Checkin_Vendedor { get; set; }

        public virtual DateTime? DataInicial { get; set; }
        public virtual DateTime? DataFinal { get; set; }
    }
}