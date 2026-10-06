using Aniel.Connect.Model.nhibernate;
using FluentNHibernate.Mapping;

namespace Aniel.Connect.DAO.map
{
    public class Arquivo_DocProdMap : ClassMap<Arquivo_DocProd>
    {
        public Arquivo_DocProdMap() : base()
        {
            Table("TB_ARQUIVO_DOCPROD");

            Id(u => u.Id, "ID").GeneratedBy.Assigned();
            Map(x => x.Chave, "CHAVE").Length(40);
            Map(x => x.Descricao, "DESCRICAO").Length(100);
            Map(x => x.Tipo, "TIPO").Length(20);
            Map(x => x.Arquivo, "ARQUIVO");
            Map(x => x.Usuario, "USUARIO").Length(10);
            Map(x => x.Data, "DATA").CustomSqlType("DATE");
            Map(x => x.Hora, "HORA").CustomSqlType("TIME");

            Map(x => x.Num_Obra, "NUM_OBRA").Length(50);
        }
    }
}
