using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;

namespace Aniel.Connect.DAO.map
{
    public class Cliente_AnexoMap : ClassMapBase<Cliente_Anexo>
    {
        public Cliente_AnexoMap() : base()
        {
            Table("TB_CLIENTE_ANEXO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CLIENTE_ANEXO");

            Map(x => x.Titulo, "TITULO").Length(50);
            Map(x => x.Nome_Arquivo, "NOME_ARQUIVO").Length(100);
            Map(x => x.Extensao_Arquivo, "EXTENSAO_ARQUIVO").Length(5);
            Map(x => x.Arquivo, "ARQUIVO");
            Map(x => x.IsAnexoContratoAditivo, "IS_ANEXO_CTT_ADITIVO");
            Map(x => x.DescricaoModeloUtilizado, "DESC_MODELO_UTIL");

            References(x => x.Cliente, "ID_CLIENTE").ForeignKey("FK_CLIENTE_AN_CLIENTE");
            References(x => x.Venda, "ID_VENDA").ForeignKey("FK_CLIENTE_AN_VENDA");
            References(x => x.ClienteContrato, "ID_CLIENTE_CTT").ForeignKey("FK_CLIENTE_AN_CLIENTE_CTT");
            Map(x => x.Foto_Perfil, "FOTO_PERFIL");            
        }
    }
}
