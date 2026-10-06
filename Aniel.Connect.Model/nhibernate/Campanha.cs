using Aniel.Connect.Model.enumerado;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Campanha : BaseModel
    {
        public virtual String Titulo { get; set; }
        public virtual Status_Default? Camp_Tempo_Determinado { get; set; }
        public virtual DateTime? Data_Inicio { get; set; }
        public virtual DateTime? Data_Final { get; set; }
        public virtual Status_Default? Status { get; set; }

        //public virtual String Pais { get; set; }
        //public virtual String UF { get; set; }
        //public virtual String Cidade { get; set; }

        public virtual Int64? Id_Meio_Captacao { get; set; }

        public virtual Status_Default? Definir_Campanha_Regioes { get; set; }
        public virtual Status_Default? Definir_Area_Cobertura { get; set; }
        public virtual Status_Default? Definir_CampanhaEquipeVenda { get; set; }
        public virtual CampanhaUtilizadaPor? CampanhaUtilizadaPor { get; set; }
        public virtual String PrefixoCupom { get; set; }
        //public virtual String Area_Cobertura { get; set; }


        //public virtual Int64? Meta { get; set; }
        //public virtual String Setor_Departamento { get; set; }
        //public virtual Filial Filial { get; set; }
        //public virtual Funcionario Funcionario { get; set; }
        //public virtual Funcionario Supervisor { get; set; }


        private IList<Campanha_PlanoProduto> _planosprodutos;

        public virtual IList<Campanha_PlanoProduto> PlanosProdutos
        {
            get
            {
                if (_planosprodutos == null)
                    _planosprodutos = new List<Campanha_PlanoProduto>();
                return _planosprodutos;
            }
            set
            {
                _planosprodutos = value;
            }
        }

        private IList<Campanha_EquipeVendas> EquipeVendasLista;

        public virtual IList<Campanha_EquipeVendas> EquipeVendas

        {
            get
            {
                if (EquipeVendasLista == null)
                    EquipeVendasLista = new List<Campanha_EquipeVendas>();
                return EquipeVendasLista;
            }
            set
            {
                EquipeVendasLista = value;
            }
        }

        private IList<Campanha_Regioes> RegioesLista;

        public virtual IList<Campanha_Regioes> Regioes

        {
            get
            {
                if (RegioesLista == null)
                    RegioesLista = new List<Campanha_Regioes>();
                return RegioesLista;
            }
            set
            {
                RegioesLista = value;
            }
        }

        private IList<Campanha_AreaCobertura> AreaCoberturaLista;

        public virtual IList<Campanha_AreaCobertura> AreaCobertura

        {
            get
            {
                if (AreaCoberturaLista == null)
                    AreaCoberturaLista = new List<Campanha_AreaCobertura>();
                return AreaCoberturaLista;
            }
            set
            {
                AreaCoberturaLista = value;
            }
        }

        #region Mobile
        public virtual Boolean? Combo { get; set; }
        #endregion
    }
}
