
using Sinapse.Model.enumerados;
using Sinapse.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Modelo_PainelVenda : BaseModel
    {
        public virtual String Descricao { get; set; }
        public virtual Status_Default Status { get; set; }
        public virtual enumerado.Visibilidade Visibilidade { get; set; }
        public virtual enumerado.Coluna_PainelVendaEnum? Coluna_Ordenar { get; set; }
        public virtual Padrao Padrao { get; set; }

        private IList<Coluna_PainelVenda> _colunas;

        public virtual IList<Coluna_PainelVenda> Colunas
        {
            get
            {
                if (_colunas == null)
                    _colunas = new List<Coluna_PainelVenda>();
                return _colunas;
            }
            set
            {
                _colunas = value;
            }
        }


        private IList<Filtro_PainelVenda> _filtros;

        public virtual IList<Filtro_PainelVenda> Filtros
        {
            get
            {
                if (_filtros == null)
                    _filtros = new List<Filtro_PainelVenda>();
                return _filtros;
            }
            set
            {
                _filtros = value;
            }
        }
    }
}
