using Sinapse.Model.nhibernate;

namespace Aniel.Connect.Model.nhibernate
{
    public class ComboPlanoProduto : BaseModel
    {
        public virtual Combo Combo { get; set; }
        public virtual Plano Plano { get; set; }
        public virtual Produto Produto { get; set; }

        /// <summary>
        ///     Valor do Plano/Produto quando vendido dentro do combo.
        /// </summary>
        public virtual double Valor { get; set; }

        public virtual bool CobrarInstalacao { get; set; }
        /// <summary>
        ///     Valor de instalação do Plano/Produto quando vendido dentro do combo.
        /// </summary>
        public virtual double? ValorInstalacao { get; set; }
    }
}
