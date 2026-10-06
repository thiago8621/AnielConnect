using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using FluentNHibernate.Mapping;

namespace Aniel.Connect.DAO.map
{
    public class Endereco_ClienteMap : ClassMap<Endereco_Cliente>
    {
        public Endereco_ClienteMap()
        {
            Table("TB_ENDERECO_CLIENTE");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_ENDERECO_CLIENTE");

            Map(x => x.Id_Bairro, "ID_BAIRRO");
            Map(x => x.Id_Cidade, "ID_CIDADE");
       

            Map(x => x.Cep, "ENDERECO_CEP").Length(10);
            Map(x => x.Logradouro, "ENDERECO_LOGRADOURO").Length(100);

            Map(x => x.Numero, "ENDERECO_NUMERO").Length(10);
            Map(x => x.Bairro, "ENDERECO_BAIRRO").Length(80);
            Map(x => x.Cidade, "ENDERECO_CIDADE").Length(80);
            Map(x => x.UF, "ENDERECO_UF").Length(2);
            Map(x => x.Complemento, "ENDERECO_COMPLEMENTO").Length(100);
            Map(x => x.Ponto_Referencia, "ENDERECO_PONTO_REFERENCIA").Length(5000);

            Map(x => x.Latitude, "ENDERECO_LATITUDE");
            Map(x => x.Longitude, "ENDERECO_LONGITUDE");

            Map(x => x.Cod_Area, "COD_AREA").Length(4);
            Map(x => x.Cod_Escritorio, "COD_EESCRITORIO").Length(4);
            Map(x => x.Cod_Localidade, "COD_LOCALIDADE");
            Map(x => x.Localidade, "LOCALIDADE").Length(100);
            Map(x => x.Busca_Endereco, "BUSCA_ENDERECO").CustomType<Busca_Endereco>();
            //Map(x => x.Id_Cliente_Integracao, "ID_CLIENTE");

            References(x => x.Cliente, "ID_CLIENTE").ForeignKey("FK_END_CLIENTE_CLIENTE");
            References(x => x.Pais, "ID_PAIS").ForeignKey("FK_END_CLIENTE_PAIS");
            References(x => x.Tipo_Imovel, "ID_TIPO_IMOVEL").ForeignKey("FK_END_CLIENTE_TIMOVEL");
            References(x => x.Endereco_Logradouro, "ID_ENDERECO_LOGRADOURO").ForeignKey("FK_END_CLIENTE_LOGRADOURO");
            References(x => x.Condominio_Predio, "ID_CONDOMINIO_PREDIO").ForeignKey("FK_END_CLIENTE_CONDOMINIO");
            Map(x => x.Bloco, "BLOCO");
            Map(x => x.Apartamento, "APARTAMENTO");

            Map(p => p.UsuarioCriacao).Length(30);
            Map(p => p.DataHoraCriacao);
            Map(p => p.UsuarioAlteracao).Length(30);
            Map(p => p.DataHoraAlteracao);

            Map(p => p.Token).Length(50);
        }
    }
}
