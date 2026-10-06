using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.DAO.map
{
    public class DocTermoMap : ClassMapBase<DocTermoAnexo>
    {
        public DocTermoMap() : base()
        {
            Table("TB_TERMO_ADESAO_ANEXO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_TERMO_ADESAO_ANEXO");

            Map(x => x.Nome, "NOME");
            Map(x => x.Extensao, "EXTENSAO");
            Map(x => x.Url, "URL");
            Map(x => x.TipoDocumento, "TIPO").CustomType<Tipo_Doc_Termo_Anexo>();
            Map(x => x.Lado, "LADO").CustomType<Lado_Doc_Aceite>();
            Map(x => x.Status, "STATUS").CustomType<Status_Doc_Aceite>();
            References(x => x.Venda, "ID_VENDA").ForeignKey("FK_ADESAO_ANEXO");
            References(x => x.TrocaTitularidade, "ID_TROCA_TITULAR").ForeignKey("FK_TROCA_ANEXO");
        }
    }
}
