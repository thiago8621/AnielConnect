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
    public class Endereco_LogradouroMap : ClassMapBase<Endereco_Logradouro>
    {
        public Endereco_LogradouroMap() : base()
        {
            Table("TB_ENDERECO_LOGRADOURO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_ENDERECO_LOGRADOURO");
            Map(x => x.Cep, "CEP");
            Map(x => x.Descricao, "DESCRICAO").Length(100);
            Map(x => x.CodAuxiliar, "COD_AUXILIAR").Length(100);

            References(x => x.Pais, "ID_PAIS").ForeignKey("FK_ENDERECO_LOGRADOURO_PAIS");
            References(x => x.UF, "ID_UF").ForeignKey("FK_ENDERECO_LOGRADOURO_UF");
            References(x => x.Cidade, "ID_CIDADE").ForeignKey("FK_ENDERECO_LOGRADOURO_CIDADE");
            References(x => x.Bairro, "ID_BAIRRO").ForeignKey("FK_ENDERECO_LOGRADOURO_BAIRRO");

            Map(x => x.Tipo, "TIPO").CustomType<Tipo_Endereco_Logradouro>();
            Map(x => x.Tipo_Zona, "TIPO_ZONA").CustomType<Tipo_Atendimento_Zona>();
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();

            Map(x => x.Cod_Area, "COD_AREA");
            Map(x => x.Cod_Escritorio, "COD_ESCRITORIO");
            Map(x => x.Cod_Localidade, "COD_LOCALIDADE");

        }
    }
}
