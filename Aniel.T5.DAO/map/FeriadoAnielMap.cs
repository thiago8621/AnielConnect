using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.T5.DAO.map
{
    public class FeriadoAnielMap : ClassMapBase<FeriadoAniel>
    {
        public FeriadoAnielMap() 
        {
            Table("TB_FERIADO");

            Id(u => u.Id, "ID").GeneratedBy.Assigned();

            Map(x => x.Descricao, "DESCRICAO").Length(40);
            Map(x => x.Data, "DATA");
            Map(x => x.Abrangencia, "ABRANGENCIA").CustomType<Abrangencia>();
            Map(x => x.Tipo_Feriado, "TIPO").CustomType<Tipo_Feriado>();
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
            Map(x => x.Pais, "PAIS");

            Map(x => x.Ano, "ANO");
            Map(x => x.Mes, "MES").Index("IDX_PT_FERIADAO_DIA_MES");
            Map(x => x.Dia, "DIA").Index("IDX_PT_FERIADAO_DIA_MES");

            Map(x => x.UF, "UF").Length(2);
            Map(x => x.Cidade, "CIDADE").Length(30);
            
            Map(x => x.Id_Municipio, "ID_MUNICIPIO");
        }
    }
}
