using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Atendimento_AcaoMap : ClassMapBase<Atendimento_Acao>
    {
        public Atendimento_AcaoMap() : base()
        {
            Table("TB_ATENDIMENTO_ACAO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_ATENDIMENTO_ACAO");

            
            Map(x => x.HTML, "HTML");

            Map(x => x.Status_Atendimento, "STATUS_ATENDIMENTO").CustomType<Status_Atendimento>();
            Map(x => x.Origem, "ORIGEM").CustomType<Origem_AtendimentoAcao>();
            Map(x => x.TipoAtividade, "TIPO_ATIVIDADE").CustomType<TipoAtividadeAtendimento>();
            Map(x => x.MessageID, "MESSAGE_ID").Length(300);


            References(x => x.Responsavel, "ID_RESPONSAVEL").ForeignKey("FK_ATENDIMENTO_ACAO_RESP");

            References(x => x.Atendimento, "ID_ATENDIMENTO").ForeignKey("FK_ATENDIMENTO_ATEND");
            HasMany(x => x.Arquivos_AtendimentoAcao).KeyColumn("ID_ATENDIMENTO_ACAO").Cascade.All().Inverse();

        }
    }
}
