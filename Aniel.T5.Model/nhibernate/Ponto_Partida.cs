using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.Model.nhibernate
{
    public class Ponto_Partida : BaseModel
    {
        private IEnumerable<EquipeAniel> _equipes;

        public virtual String Descricao { get; set; }

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


        public virtual IEnumerable<EquipeAniel> Equipes
        {
            get
            {
                if (_equipes == null)
                    _equipes = new List<EquipeAniel>();
                return _equipes;
            }
            set
            {
                _equipes = value;
            }
        }


    }
}
