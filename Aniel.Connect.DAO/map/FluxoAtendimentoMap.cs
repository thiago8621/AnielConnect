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
   public class FluxoAtendimentoMap :ClassMapBase<FluxoAtendimento>
    {
        public FluxoAtendimentoMap() : base()
        {
            Table("TB_FLUXO_ATENDIMENTO");
            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_FLUXO_ATENDIMENTO");
            Map(m => m.Codigo, "CODIGO");
            Map(m => m.Sequencia, "SEQUENCIA");
            Map(m => m.Descricao, "DESCRICAO");
            Map(m => m.Setor_Departamento, "SETOR_DEPARTAMENTO");
            Map(m => m.AlterarSetor_Responsavel, "STATUS_SETOR_RESPONSAVEL");
            Map(m => m.Etapa_CriaOS, "STATUS_ETAPA_CRIAR_OS");
            Map(m => m.Baixar_OS, "STATUS_ETAPA_BAIXAR_OS");
            Map(m => m.Responsavel, "RESPONSAVEL");
            Map(m => m.Contrato, "CONTRATO");
            Map(m => m.Cod_Encerramento, "COD_ENCERRAMENTO");
            Map(m => m.Tempo_Exc, "TEMPO_EXC");
            Map(m => m.Encerra_Atendimento, "ENCERRA_ATENDIMENTO");
            Map(m => m.Etapa_Decisoria, "ETAPA_DECISORIA");
            Map(m => m.Proxima_Etapa, "PROXIMA_ETAPA");
            Map(m => m.Motivo_Status_Atendimento, "MOTIVO_STATUS");
            Map(m => m.Status, "STATUS").CustomType<Status_Default>();
            Map(m => m.StatusAtendimento, "STATUS_ATENDIMENTO").CustomType<Status_AtendimentoEtapa>();

            References(m => m.Tipo_Atendimento, "ID_TIPO_ATENDIMENTO").ForeignKey("FK_FLUXO_ATEND_TIPO");



        }
        }
}
