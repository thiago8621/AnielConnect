using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;

namespace Aniel.Connect.DAO.map
{
    public class EnderecoIntegracaoMap : ClassMapBase<ConsultaEnderecoIntegracao>
    {
        public EnderecoIntegracaoMap() : base()
        {
            Table("TB_ENDERECO_INTEGRACAOMK");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_ENDERECO_INTEGRACAOMK");
            
            Map(x => x.Cod_Logradouro, "COD_LOGRADOURO");
            Map(x => x.Logradouro, "LOGRADOURO").Length(100);
            Map(x => x.Cod_Bairro, "COD_BAIRRO");
            Map(x => x.Bairro, "BAIRRO");
            Map(x => x.Cep, "CEP");
            Map(x => x.Cod_Cidade, "COD_CIDADE");
            Map(x => x.Cidade, "CIDADE");


        }
    }
}
