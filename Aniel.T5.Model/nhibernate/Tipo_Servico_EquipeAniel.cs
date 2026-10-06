
using Aniel.T5.Model.enumerado;
namespace Aniel.T5.Model.nhibernate
{
    public class Tipo_Servico_EquipeAniel
    {
        public virtual string CodCt { get; set; }
        public virtual short? Cod_Tipo_Serv { get; set; }
        public virtual string Descricao { get; set; }
        public virtual string Descricao_Aux { get; set; }
        public virtual int? Cod_Natureza { get; set; }
        public virtual string CodigoIntegrator { get; set; }

        /// <summary>
        ///     Objeto utilizado somente para leitura em consultas IQueryable
        ///     com o NHibernate
        /// </summary>
        public virtual Natureza_ProducaoAniel Natureza { get; set; }

        public virtual string Exige_Manutencao { get; set; }
        public virtual string Exige_Pagamento_Instalacao { get; set; }
        public virtual string Mudanca_Endereco { get; set; }
        public virtual int? TempoLimiteEncaixe { get; set; }
    }
}
