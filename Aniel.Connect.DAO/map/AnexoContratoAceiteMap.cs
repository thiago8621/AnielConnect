using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using FluentNHibernate.Mapping;

namespace Aniel.Connect.DAO.map
{
    public class AnexoContratoAceiteMap : ClassMap<AnexoContratoAceiteModel>
    {
        public AnexoContratoAceiteMap() : base()
        {
            Table("TB_ANEXO_CONTRATO_ACEITE");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_ANEXO_CONTRATO_ACEITE");
            
            Map(u => u.ExigeAceite, "EXIGE_ACEITE");
            Map(u => u.IdVenda, "ID_VENDA");

            Map(u => u.Anexo, "ANEXO").Length(5000);
            Map(u => u.Descricao, "DESCRICAO");
            Map(u => u.NomeArquivo, "NOME_ARQUIVO");
            Map(u => u.Extensao, "EXTENSAO");
            Map(u => u.Status, "STATUS").CustomType<StatusAnexoContrato>();
        }
    }
}
