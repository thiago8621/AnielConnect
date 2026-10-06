using Aniel.T5.Model.nhibernate;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Model.nhibernate
{
    public class LegendaTimelineTipoServicos : BaseModel
    {
        public virtual ContratoAniel Contrato { get; set; }
        public virtual Natureza_ProducaoAniel Natureza_Producao { get; set; }
        public virtual Tipo_Servico_EquipeAniel Tipo_Servico_Equipe { get; set; }
        public virtual String Cor { get; set; }
    }
}
