using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.DAO.map
{
    public class Condominio_PredioMap : ClassMapBase<Condominio_Predio>
    {
        public Condominio_PredioMap() : base()
        {
            Table("TB_CONDOMINIO_PREDIO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CONDOMINIO_PREDIO");
            
            Map(x => x.Cep, "CEP").Length(10);
            Map(x => x.Id_Pais, "ID_PAIS");
            Map(x => x.Id_Cidade, "ID_CIDADE");
            Map(x => x.Numero, "NUMERO");
            Map(x => x.Bairro, "BAIRRO").Length(80);
            Map(x => x.Logradouro, "LOGRADOURO").Length(100);
            Map(x => x.Descricao, "DESCRICAO").Length(100);
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
            Map(x => x.Uf, "UF").Length(2);
            Map(x => x.Definir_Caixa_Condominio_Predio, "DEFINIR_CAIXA_CONDOMINIO_PREDIO");
            
            References(x => x.Endereco_Logradouro, "ID_ENDERECO_LOGRADOURO").ForeignKey("FK_CONDOMINIO_PREDIO_ENDERECO_L");            
        }
    }
}
