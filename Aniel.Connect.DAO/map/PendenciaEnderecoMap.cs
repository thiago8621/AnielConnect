using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;

namespace Aniel.Connect.DAO.map
{
    public class PendenciaEnderecoMap : ClassMapBase<PendenciaEndereco>
    {
        public PendenciaEnderecoMap() : base() 
        {
            Table("TB_PENDENCIA_ENDERECO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_PENDENCIA_ENDERECO");

            Map(u => u.Logradouro, "LOGRADOURO").Index("IDX_PEND_END_LOGRADOURO");
            Map(u => u.Numero, "NUMERO").Index("IDX_PEND_END_NUMERO");

            HasMany(u => u.DadosConsulta).KeyColumn("ID_CONSULTA").Cascade.All();
        }
    }
}
