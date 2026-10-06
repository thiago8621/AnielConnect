using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sinapse.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using FluentNHibernate.Mapping;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;
namespace Aniel.T5.DAO.map
{
    public class Elemento_RedeAnielMap : ClassMapBase<Elemento_Rede>
    {
        public Elemento_RedeAnielMap()
        {
            Table("TB_ELEMENTO_REDE");
            Id(p => p.Id, "ID").GeneratedBy.Sequence("GN_ELEMENTO_REDE");

            Map(p => p.Cod_Caixa, "COD_CAIXA");

            Map(p => p.CEP, "CEP").Length(10);
            Map(p => p.Bairro, "BAIRRO").Length(50);
            Map(p => p.Pais, "PAIS").Length(50);

            Map(p => p.Endereco, "ENDERECO").Length(50);
            Map(p => p.Certificado, "CERTIFICADO");
            Map(p => p.Cidade, "CIDADE").Length(50);

            Map(p => p.Complemento, "COMPLEMENTO").Length(100);
            Map(p => p.Contrato, "CONTRATO").Length(90);
            Map(p => p.Descricao, "DESCRICAO").Length(50);

            Map(p => p.Id_POP, "POP");
            Map(p => p.Id_OLT, "OLT");
            Map(p => p.Latitude, "LATITUDE");
            
            Map(p => p.Longitude, "LONGITUDE");
            Map(p => p.Coordenadas, "COORDENADAS").Length(5000);
            Map(p => p.Num_Coordenadas, "NUM_COORDENADAS");

            Map(p => p.Numero, "NUMERO").Length(10);
            Map(p => p.Observacao, "OBS").Length(255);
            Map(p => p.Projeto, "PROJETO").Length(90);

            Map(p => p.Cor_linha_Ativa, "COR_LINHA_ATIVA").Length(50);
            Map(p => p.Cor_linha_Inativa, "COR_LINHA_INATIVA").Length(50);
            Map(p => p.Padrao_Cores, "PADRAO").CustomType<Padrao_Cor>();

            Map(p => p.UF, "UF").Length(2);
            
            Map(p => p.Situacao, "SITUACAO").CustomType<Status_Elemento>();
            Map(p => p.Metragem, "METRAGEM");

            Map(p => p.Capacidade, "CAPACIDADE");
            Map(p => p.JSON_Ordenacao_Digarama, "ORDENACAO_DIGARAMA").Length(16000);

            References(p => p.Tipo, "ID_TIPO").ForeignKey("FK_ELEMENTO_REDE_TIPO");
            References(p => p.Cabo, "ID_CABO").ForeignKey("FK_ELEMENTO_REDE_CABO");
            References(p => p.Caixa, "ID_CAIXA").ForeignKey("FK_ELEMENTO_REDE_CAIXA");
            References(p => p.Poste, "ID_POSTE").ForeignKey("FK_ELEMENTO_REDE_POSTE");
            //HasMany(p => p.Conexoes).KeyColumn("ID_ELEMENTO_REDE").Cascade.All().Inverse();
            //HasMany(p => p.EnderecosCadastrados).KeyColumn("ID_ELEMENTO_REDE").Cascade.All().Inverse();
            //References(p => p.Modelo, "ID_MODELO").ForeignKey("FK_ELEMENTO_REDE_MODELO");

        }
    }
}
