using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Desconto : BaseModel
    {
        private IList<Desconto_Meio_Captacao> _meios;


        public virtual Status_Default Status { get; set; }
        public virtual String Descricao { get; set; }
        public virtual Double? Percentual_Desconto { get; set; }
        public virtual Double? Valor_Desconto { get; set; }

        public virtual Tipo_Desconto Tipo_Desconto { get; set; }

        public virtual Desconto_Captacao? Desconto_Captacao { get; set; }
        public virtual Int64? Repeticoes_Desconto { get; set; }

        public virtual Boolean Desconto_Tempo_Indeterminado { get; set; }
        public virtual Boolean Valor_Unico { get; set; }
        public virtual Boolean Automatico { get; set; }
        private IList<Desconto_Valores> ValoresLista;

        public virtual IList<Desconto_Valores> Valores

        {
            get
            {
                if (ValoresLista == null)
                    ValoresLista = new List<Desconto_Valores>();
                return ValoresLista;
            }
            set
            {
                ValoresLista = value;
            }
        }

        private IList<Desconto_Regioes> RegioesLista;

        public virtual IList<Desconto_Regioes> Regioes

        {
            get
            {
                if (RegioesLista == null)
                    RegioesLista = new List<Desconto_Regioes>();
                return RegioesLista;
            }
            set
            {
                RegioesLista = value;
            }
        }

        private IList<Desconto_Condicoes> CondicoesLista;

        public virtual IList<Desconto_Condicoes> Condicoes

        {
            get
            {
                if (CondicoesLista == null)
                    CondicoesLista = new List<Desconto_Condicoes>();
                return CondicoesLista;
            }
            set
            {
                CondicoesLista = value;
            }
        }

        private IList<Desconto_CondominiosPredios> Condominios_PrediosLista;

        public virtual IList<Desconto_CondominiosPredios> Condominios_Predios

        {
            get
            {
                if (Condominios_PrediosLista == null)
                    Condominios_PrediosLista = new List<Desconto_CondominiosPredios>();
                return Condominios_PrediosLista;
            }
            set
            {
                Condominios_PrediosLista = value;
            }
        }

        //public virtual IList<Desconto_Meio_Captacao> Meios
        //{
        //    get
        //    {
        //        if (_meios == null)
        //            _meios = new List<Desconto_Meio_Captacao>();
        //        return _meios;
        //    }
        //    set
        //    {
        //        _meios = value;
        //    }
        //}
        public virtual String Numero_Contrato { get; set; }
        public virtual string CodigoAuxiliar { get; set; }
    }
}
