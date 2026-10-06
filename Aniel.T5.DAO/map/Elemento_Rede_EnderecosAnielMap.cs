using Aniel.T5.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model;

namespace Aniel.T5.DAO.map
{
    public class Elemento_Rede_EnderecosAnielMap : ClassMapBase<Elemento_Rede_Enderecos>
    {
        public Elemento_Rede_EnderecosAnielMap()
        {
            Table("TB_ELEM_REDE_ENDERECOS");
            
            Id(p => p.Id, "ID").GeneratedBy.Sequence("GN_ELEM_REDE_ENDERECOS");
            References(p => p.Elemento_Rede, "ID_ELEM_REDE").ForeignKey("FK_ID_ELEM_REDE");
            Map(p => p.Id_EnderecoLogradouro, "ID_ENDERECO_LOGRADOURO");
            Map(p => p.Id_CondominioPredio, "ID_CONDOMINIO_PREDIO");
            Map(p => p.Numero, "NUMERO").Length(15);
            Map(p => p.Bloco, "BLOCO").Length(15);
            Map(p => p.Apto_numero, "APTO_NUMERO").Length(15);
            Map(p => p.Todos_Bloco, "TODOS_BLOCO");
            Map(p => p.Todos_Apto_Numero, "TODOS_APTO_NUMERO");
        }
    }
}
