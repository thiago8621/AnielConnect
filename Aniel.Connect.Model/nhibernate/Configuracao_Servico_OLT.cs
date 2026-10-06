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
    public class Configuracao_Servico_OLT : BaseModel
    {
        public virtual string Descricao { get; set; }
        public virtual Sinapse.Model.enumerados.Marca_Modelo_OLT Marca_Modelo_OLT { get; set; }
        public virtual Periodicidade_ExecucaoEnum? Periodicidade_Execucao { get; set; }
        public virtual DateTime? Hora_Execucao { get; set; }
        public virtual Status_Default Status { get; set; }
        public virtual DateTime? Ultima_Data_Execucao { get; set; }
        public virtual bool Extrair_Diagnostico_Onu { get; set; }
        public virtual bool Consultar_Status_Onu { get; set; }
        public virtual int? Minutos_Execucao_Status_Onu { get; set; }
    }
}
