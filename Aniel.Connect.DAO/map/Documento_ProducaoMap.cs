using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Documento_ProducaoMap : ClassMapBase<Documento_Producao>
    {
        public Documento_ProducaoMap() : base()
        {
            Table("TB_DOCUMENTO_PRODUCAO");

            Id(x => x.Id);

            References(x => x.Projeto, "ID_PROJETO");
            References(x => x.Equipe, "ID_EQUIPE");

            Map(x => x.Num_Obra, "NUM_OBRA").Length(50);
            Map(x => x.Num_Doc, "NUM_DOC");

            Map(x => x.Num_RVT, "NUM_RVT").Length(15);
            Map(x => x.Tarefa, "TAREFA").Length(15);

            References(x => x.Escritorio, "ID_ESCRITORIO");
            References(x => x.Area, "ID_AREA");
            References(x => x.Localidade, "ID_LOCALIDADE");
            References(x => x.Codigo_Encerramento, "ID_COD_ENCERRAMENTO");

            References(x => x.Tipo_Servico_Equipe, "ID_TIPO_SERVICO");
            References(x => x.Sub_Tipo_Servico, "ID_SUB_TIPO");

            Map(x => x.Data_Agendamento, "DATA_AGENDAMENTO");
            Map(x => x.Data_Producao, "DATA_PRODUCAO");
            Map(x => x.Hora_Producao, "HORA_PRODUCAO");
            Map(x => x.Observacao, "OBSERVACAO").Length(100);
            Map(x => x.Logradouro, "LOGRADOURO").Length(100);
            Map(x => x.Numero, "NUMERO").Length(10);
            Map(x => x.Complemento, "COMPLEMENTO").Length(100);
            Map(x => x.Bairro, "BAIRRO").Length(100);
            Map(x => x.Cep, "CEP").Length(20);
            Map(x => x.Latitude_Baixa, "LATITUDE_BAIXA");
            Map(x => x.Longitude_Baixa, "LONGITUDE_BAIXA");
        }
    }
}
