using Sinapse.Model;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.nhibernate
{
    public class Equipe_PontoPartidaAniel : BaseModel
    {
        public virtual String Descricao { get; set; }

        public virtual String CEP { get; set; }
        public virtual String Logradouro { get; set; }
        public virtual String Bairro { get; set; }
        public virtual String Cidade { get; set; }
        public virtual String Numero { get; set; }
        public virtual Double? Latitude { get; set; }
        public virtual Double? Longitude { get; set; }

    }
}
