using Aniel.T5.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;

namespace Aniel.T5.Model.nhibernate
{
    public class Agendamento_EquipeAniel : BaseModel
    {
        /// <summary>
        ///     Valor mínimo para datas, usado quando o NHibernate precisa
        ///     colocar o valor de um campo TIME no banco de dados em um
        ///     campo DateTime em uma classe do C#
        /// </summary>
        private DateTime FIREBIRD_MIN_DATE = new DateTime(1753, 1, 1);

        public virtual EquipeAniel Equipe { get; set; }

        public virtual String Equipe_Tmp { get; set; }

        public virtual DateTime DataHora_Inicio { get; set; }

        public virtual DateTime DataHora_Fim { get; set; }

        public virtual Double Deslocamento { get; set; }

        public virtual Tipo_ImovelAniel Tipo_Imovel { get; set; }

        public virtual Tipo_Servico_EquipeAniel Tipo_Servico { get; set; }

        public virtual String Cod_Escritorio { get; set; }

        public virtual String Cod_Area { get; set; }

        public virtual Int32? Cod_Localidade { get; set; }

        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Num_Obra { get; set; }
        public virtual Int32 Num_Doc { get; set; }

        public virtual OrigemAgendamentoOS? Origem { get; set; }
        public virtual Double? Latitude { get; set; }
        public virtual Double? Longitude { get; set; }
        public virtual String Status_Cobranca { get; set; }
        public virtual String Log { get; set; }
        public virtual bool? Flexivel { get; set; }
        public virtual string IdJobNotificacaoAtraso { get; set; }
    }
}
