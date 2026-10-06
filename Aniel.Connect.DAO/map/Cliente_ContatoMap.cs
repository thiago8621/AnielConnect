using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;

namespace Aniel.Connect.DAO.map
{
    public class Cliente_ContatoMap : ClassMapBase<Cliente_Contato>
    {
        public Cliente_ContatoMap() : base()
        {
            Table("TB_CLIENTE_CONTATO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_CLIENTE_CONTATO");
            Map(m => m.Telefone, "TELEFONE");
            Map(m => m.Email, "EMAIL");
            Map(m => m.Informacao_Adicional, "INFORMACAO_ADICIONAL");
            Map(m => m.Tipo_Contato, "TIPO_CONTATO").CustomType<Tipo_Contato_Cliente>();
            Map(x => x.NomeContato,"NOME_CONTATO").Length(100);

            References(m => m.Cliente, "ID_CLIENTE").ForeignKey("FK_CLIENTE_CONTATO");            
    }
    }
}
