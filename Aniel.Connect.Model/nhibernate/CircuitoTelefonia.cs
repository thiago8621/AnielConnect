using Aniel.Connect.Model.enumerado;
using Aniel.Connect.ModelLog.nhibernate;
using Resources;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using Sinapse.ModelLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class CircuitoTelefonia : BaseModel
    {
        public virtual Cliente_PlanoProduto Clientes_PlanosProdutos { get; set; }
        public virtual RotaTelefonia RotaTelefonia { get; set; }
        public virtual String Numero_Sip { get; set; }
        public virtual String Senha { get; set; }
        public virtual Status_Default Status { get; set; }
        public virtual Status_PlanoTelefonia Status_Circuito { get; set; }
        public virtual TipoDiscagem TipoDiscagem { get; set; }
        public virtual Boolean HabilitarLc { get; set; }
        public virtual Boolean HabilitarLdn { get; set; }
        public virtual Boolean HabilitarLdi { get; set; }
        public virtual Boolean HabilitarVc1 { get; set; }
        public virtual Boolean HabilitarVc2 { get; set; }
        public virtual Boolean HabilitarVc3 { get; set; }
        public virtual Boolean HabilitarBina { get; set; }
        public virtual Boolean BloqueioChamadaCobrar { get; set; }
        public virtual Boolean HabilitarConsumoMinimo { get; set; }
        public virtual long Id_Venda { get; set; }
        public virtual String CodigoAuxiliar { get; set; }
        public virtual int? QuantidadeLigacoesSimultaneas { get; set; }
        private IList<Did> did { get; set; }
        public virtual IList<Did> Did
        {
            get
            {
                if (did == null)
                {
                    did = new List<Did>();
                }
                return did;
            }
            set { did = value; }
        }

    }


}
