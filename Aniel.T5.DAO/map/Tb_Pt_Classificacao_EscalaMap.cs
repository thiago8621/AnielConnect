using Aniel.T5.Model.enumerado;
using Aniel.T5.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;

namespace Aniel.Connect.DAO.map
{
    public class Tb_Pt_Classificacao_EscalaMap : ClassMapBase<Tb_Pt_Classificacao_Escala>
    {
        public Tb_Pt_Classificacao_EscalaMap() : base()
        {
            Table("TB_PT_CLASSIFICACAO_ESCALA");

            Id(u => u.Id, "ID");

            Map(x => x.Descricao, "DESCRICAO");
            Map(x => x.Interno, "INTERNO");
            Map(x => x.Permitir_Marcar, "PERMITIR_MARCAR");
            Map(x => x.Enviar_Notificacao, "ENVIAR_NOTIFICACAO");
            Map(x => x.Cor, "COR");
            Map(x => x.Escala_Especial, "ESCALA_ESPECIAL");
            Map(x => x.Abonar_Debito, "ABONAR_DEBITO");

            Map(x => x.Gera_Movimento, "GERA_MOVIMENTO");
            Map(x => x.Debita_Credita, "DEBITA_CREDITA");
            Map(x => x.Falta, "FALTA");
            Map(x => x.Lancado_Gestor, "LANCADO_GESTOR");
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();
            Map(x => x.Sigla, "SIGLA");
            Map(x => x.Situacao, "SITUACAO").CustomType<SituacaoFuncionarioAniel>();
            Map(x => x.Evento, "EVENTO");
            Map(x => x.PermtiteRoteirizar, "PERMTITE_ROTEIRIZAR");
        }
    }
}
