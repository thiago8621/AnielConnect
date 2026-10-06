using Aniel.Connect.Model.nhibernate;
using FluentNHibernate.Mapping;

namespace Aniel.Connect.DAO.map
{
    public class Ponto_Arquivo_AjusteMap : ClassMap<Ponto_Arquivo_Ajuste>
    {
        public Ponto_Arquivo_AjusteMap() : base()
        {
            Table("TB_PONTO_ARQUIVO_AJUSTE");

            Id(u => u.Id, "ID").GeneratedBy.Identity();
            Map(x => x.Id_Documento_Ajuste, "ID_DOCUMENTO_AJUSTE").Index("IDX_PONTO_ARQUIVO_AJUSTE_DOC");
            Map(x => x.Nome_Arquivo, "NOME_ARQUIVO").Length(100);
            Map(x => x.Tipo, "TIPO").Length(10);
            Map(x => x.Arquivo, "ARQUIVO");
        }
    }
}
