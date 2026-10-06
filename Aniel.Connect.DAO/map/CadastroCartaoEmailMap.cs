using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;

namespace Aniel.Connect.DAO.map
{
    public class CadastroCartaoEmailMap : ClassMapBase<CadastroCartaoEmail>
    {
        public CadastroCartaoEmailMap() : base()
        {
            Table("TB_CADASTRO_CARTAO");

            Id(u => u.Id, "ID");

            Map(x => x.IdCliente, "IDCLIENTE");

            Map(x => x.IdForma, "IDFORMA_PAGAMENTO");

            Map(x => x.DataExpiracao, "DATAEXPIRACAO");

            Map(x => x.Link, "LINK");
        }
    }
}
