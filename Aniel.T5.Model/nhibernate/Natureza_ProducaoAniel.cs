using Aniel.Connect.Model.enumerado;
using Aniel.T5.Model.enumerado;
using System;

namespace Aniel.T5.Model.nhibernate
{
    public class Natureza_ProducaoAniel
    {
        public virtual int Id { get; set; }
        public virtual string Descricao { get; set; }
        public virtual string Departamento { get; set; }
        public virtual bool Nao_Obriga_Agendamento { get; set; }
        public virtual int? QtdDias_GeraReincidencia { get; set; }
        public virtual TipoHabilidade? TipoHabilidade { get; set; }
        public virtual Tipo_PrioridadeT5? TipoPrioridade { get; set; }
        /// <summary>
        ///     Campo booleano. S ou N
        /// </summary>
        public virtual string GeraReincidencia { get; set; }

        public virtual double? MetaReincidencia { get; set; }
        public virtual double? MetaBaixaDentroPrazo { get; set; }
        public virtual double? MetaServicosProdutivos { get; set; }

        public virtual BasePrazoEnum BasePrazo { get; set; }
        public virtual Int16? IdDepartamentoPadrao { get; set; }
    }
}
