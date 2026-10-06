using Aniel.Connect.Model.nhibernate;
using FluentNHibernate.Mapping;

namespace Aniel.Connect.DAO.map
{
    public class Imagem_DocPord_MobileMap : ClassMap<Imagem_DocPord_Mobile>
    {
        public Imagem_DocPord_MobileMap() : base()
        {
            Table("TB_IMAGEM_DOCPROD_MOBILE");

            Id(u => u.Id, "ID").GeneratedBy.Identity();
            Map(x => x.CodCt, "CODCT").Length(4);
            Map(x => x.Projeto, "PROJETO").Length(20);
            Map(x => x.Num_Obra, "NUM_OBRA").Length(50);
            Map(x => x.Num_Doc, "NUM_DOC");
            Map(x => x.Tipo, "TIPO").Length(20);
            Map(x => x.Status, "STATUS");

            Map(x => x.Usuario, "USUARIO").Length(10);
            Map(x => x.Dia, "DIA").CustomSqlType("DATE");
            Map(x => x.Hora, "HORA").CustomSqlType("TIME");
            Map(x => x.Arquivo, "ARQUIVO");
            Map(x => x.Arquivo_Depois, "ARQUIVO_DEPOIS");
        }
    }
}
