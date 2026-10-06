using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using Aniel.T5.Model.nhibernate;
using Sinapse.Model.enumerados;
using Sinapse.Model.enumerado;

namespace Aniel.T5.DAO.map
{
    public class Tipo_Elemento_RedeAnielMap : ClassMapBase<Tipo_Elemento_Rede>
    {
        public Tipo_Elemento_RedeAnielMap()
        {
            Table("TB_TIPO_ELEMENTO_REDE");

            Id(x => x.Id, "ID").GeneratedBy.Sequence("GN_TIPO_ELEMENTO_REDE");

            References(x => x.Icone, "ID_ICONE").ForeignKey("FK_TIPO_ELEMENTO_REDE_ICONE");
            //References(x => x.Espessura_linha, "ID_ESPESSURA_LINHA").ForeignKey("FK_TIPO_ELEMENTO_REDE_EL");
            References(x => x.Tipo_Cabo, "ID_TIPO_CABO").ForeignKey("FK_TIPO_ELEMENTO_REDE_TC");
            References(x => x.Modelo, "ID_MODELO").ForeignKey("FK_TIPO_ELEMENTO_REDE_MODELO");

            Map(x => x.Descricao, "DESCRICAO").Length(50);
            Map(x => x.Situacao, "SITUACAO").CustomType<Status_Default>();
            Map(x => x.Tipo_de_Cabo, "TIPO_DE_CABO").CustomType<Tipo_CaboEnum>();
            Map(x => x.Padrao, "PADRAO_DE_CORES").CustomType<Padrao_Cor>();

            Map(x => x.Num_Tubos_Loose, "NUM_LOOSE");
            Map(x => x.Total_Tubos_loose, "TOTAL_LOOSE");
            Map(x => x.Num_Pares_Fibra, "NUM_PARES_FIBRA");
            Map(x => x.Capacidade, "CAPACIDADE");
            Map(x => x.Fabricante, "FABRICANTE");

            Map(x => x.E_Cabo, "E_CABO");
            Map(x => x.E_Caixa, "E_CAIXA");

            Map(x => x.Espessura, "ESPESSURA").Length(50);

            Map(x => x.Estilo_Linha_Inativa, "ESTILO_LINHA_INATIVA").CustomType<Estilo_LinhaEnum>();
            Map(x => x.Estilo_Linha_Ativa, "ESTILO_LINHA_ATIVA").CustomType<Estilo_LinhaEnum>();

            Map(x => x.Cor, "COR").Length(50);
            Map(x => x.Cor_Linha_Inativa, "COR_LINHA_INATIVA").Length(50);
            Map(x => x.Cor_Linha_Ativa, "COR_LINHA_ATIVA").Length(50);


        }
    }
}
