using Aniel.Connect.Model.enumerado;
using Aniel.Connect.Model.nhibernate;
using Sinapse.DAO.map;
using Sinapse.Model.enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.DAO.map
{
    public class Tipo_AtendimentoMap : ClassMapBase<Tipo_Atendimento>
    {
        public Tipo_AtendimentoMap() : base()
        {
            Table("TB_TIPO_ATENDIMENTO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_TIPO_ATENDIMENTO");

            Map(x => x.Nome, "NOME").Length(50);
            Map(x => x.Prioridade_Padrao, "PRIORIDADE_PADRAO").CustomType<Prioridade_Atendimento>();
            Map(x => x.Status, "STATUS").CustomType<Status_Default>();            
            Map(x => x.Criar_Os_Encerrar, "CRIAR_OS_ENCERRAR");
            Map(x => x.Exibir_Ola, "EXIBIR_APP_OLA");
            Map(x => x.Vincular_Motivos, "VINCULAR_MOTIVOS");
            Map(x => x.Solicitacao_Cancelamento, "SOLICITACAO_CANCELAMENTO");
            Map(x => x.Trava_Atendimento_OS, "TRAVA_ATENDIMENTO_os");
            Map(x => x.Encerrar_Atendimento_Junto_OS, "ENCERRAR_ATEND_JUNTO_OS");

            References(x => x.Script, "ID_SCRIPT").ForeignKey("FK_TIPO_ATEND_SCRIPT");
            References(x => x.Categoria_Atendimento, "ID_CATEGORIA_ATENDIMENTO").ForeignKey("FK_TIPO_ATEND_CAT_ATEND");

            Map(x => x.CodCt, "CODCT").Length(4);
            Map(x => x.Projeto, "PROJETO");
            Map(x => x.Desc_Tipo_Servico, "DESC_TIPO_SERVICO").Length(150);

            Map(x => x.Script_SuporteTecnico, "ID_SCRIPT_SUPORTETECNICO");

            HasMany(x => x.Motivos).KeyColumn("ID_TIPO_ATENDIMENTO").Cascade.All().Inverse();
        }
    }
}
