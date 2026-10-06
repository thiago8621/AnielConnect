using System.Collections.Generic;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;

namespace Aniel.Connect.Model.nhibernate
{
    public class Combo : BaseModel
    {
        private IList<ComboPlanoProduto> _planosProdutos = null;

        public virtual string Descricao { get; set; }
        public virtual string DetalhesCombo { get; set; }
        public virtual Status_Default Status { get; set; }
        public virtual bool EmiteContrato { get; set; }
        public virtual Modelo_Contrato ModeloContrato { get; set; }
        public virtual IList<ComboPlanoProduto> PlanosProdutos
        {
            get
            {
                if (this._planosProdutos == null)
                    this._planosProdutos = new List<ComboPlanoProduto>();

                return this._planosProdutos;
            }
            set
            {
                this._planosProdutos = value;
            }
        }

        public virtual string CodigoIntegracao { get; set; } //integração MK: codigo plano acesso se CodigoTipoPlano=12 informar códigos de produtos do CRM
        public virtual string CodigoTipoComboIntegracao { get; set; } //codigo tipo do plano
        public virtual string CodigoIntegracaoAuxiliar { get; set; } //tipo de serviço da OS
        public virtual string CodigoIntegracaoProcesso { get; set; } //codigo integracao processo atendimento
    }
}