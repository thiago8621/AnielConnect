
using Sinapse.Model.enumerados;

using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Modelo_PainelAtendimento : BaseModel
    {
        public virtual String Descricao { get; set; }
        public virtual Status_Default Status { get; set; }
        public virtual Aniel.Connect.Model.enumerado.Visibilidade Visibilidade { get; set; }
        public virtual Padrao Padrao { get; set; }
        public virtual enumerado.Coluna_PainelAtendimentoEnum? Coluna_Ordenar { get; set; }

        private IList<Coluna_PainelAtendimento> _colunas;

        public virtual IList<Coluna_PainelAtendimento> Colunas
        {
            get
            {
                if (_colunas == null)
                    _colunas = new List<Coluna_PainelAtendimento>();
                return _colunas;
            }
            set
            {
                _colunas = value;
            }
        }


        private IList<Recurso_PainelAtendimento> _filtros;

        public virtual IList<Recurso_PainelAtendimento> Filtros
        {
            get
            {
                if (_filtros == null)
                    _filtros = new List<Recurso_PainelAtendimento>();
                return _filtros;
            }
            set
            {
                _filtros = value;
            }
        }

    }
}
