
//using Sinapse.Model.enumerados;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Modelo_PainelServico : BaseModel
    {
        public virtual String Descricao { get; set; }
        public virtual Status_Default Status { get; set; }
        public virtual Aniel.Connect.Model.enumerado.Visibilidade Visibilidade { get; set; }
        public virtual enumerado.ColunaOrdenarPainelServicoEnum? Coluna_Ordenar { get; set; }
        public virtual Padrao Padrao { get; set; }

        private IList<Coluna_PainelServico> _colunas;

        public virtual IList<Coluna_PainelServico> Colunas
        {
            get
            {
                if (_colunas == null)
                    _colunas = new List<Coluna_PainelServico>();
                return _colunas;
            }
            set
            {
                _colunas = value;
            }
        }


        private IList<Filtro_PainelServico> _filtros;

        public virtual IList<Filtro_PainelServico> Filtros
        {
            get
            {
                if (_filtros == null)
                    _filtros = new List<Filtro_PainelServico>();
                return _filtros;
            }
            set
            {
                _filtros = value;
            }
        }

    }
}