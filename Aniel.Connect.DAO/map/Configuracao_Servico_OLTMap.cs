using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Configuracao_Servico_OLTMap : ClassMapBase<Configuracao_Servico_OLT>
    {
        public Configuracao_Servico_OLTMap() : base()
        {
            Table("TB_CONFIG_SERVICO_OLT");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CONFIG_SERVICO_OLT");

            Map(x => x.Descricao, "DESCRICAO").Length(100);
            Map(x => x.Hora_Execucao, "HORA_EXECUCAO").CustomType("Time");
            Map(x => x.Marca_Modelo_OLT, "MARCA_MODELO").CustomType<Marca_Modelo_OLT>();
            Map(x => x.Periodicidade_Execucao, "PERIODICIDADE_EXECUCAO").CustomType<Periodicidade_ExecucaoEnum>();
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
            Map(x => x.Ultima_Data_Execucao, "DATA_ULTIMA_EXECUCAO");
            Map(x => x.Extrair_Diagnostico_Onu, "EXTRAIR_DIAGNOSTICO_ONU");
            Map(x => x.Consultar_Status_Onu, "CONSULTAR_STATUS_ONU");
            Map(x => x.Minutos_Execucao_Status_Onu, "MINUTOS_EXEC_STATUS_ONU");
        }
    }
}
