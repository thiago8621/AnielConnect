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
    public class AtendimentoMap : ClassMapBase<Atendimento>
    {
        public AtendimentoMap() : base()
        {
            Table("TB_ATENDIMENTO");

            Id(u => u.Id, "ID").GeneratedBy.Sequence("GN_ATENDIMENTO");

            Map(x => x.Numero, "NUMERO").Length(50);

            Map(x => x.Protocolo, "PROTOCOLO").Length(200);

            Map(x => x.TipoPreenchimento, "TIPO_PREENCHIMENTO").CustomType<TipoPreenchimentoAtendimento>();

            Map(x => x.Data_Hora_Abertura, "DATA_HORA_ABERTURA");
            Map(x => x.Data_Hora_Fechamento, "DATA_HORA_FECHAMENTO");

            Map(x => x.Status, "STATUS").CustomType<Status_Default>();

            Map(x => x.Status_Atendimento, "STATUS_ATENDIMENTO").CustomType<Status_Atendimento>();
            Map(x => x.DataHora_Indicador, "DATAHORA_INDICADOR");
            

            Map(x => x.Prioridade, "PRIORIDADE_PADRAO").CustomType<Prioridade_Atendimento>();
            Map(x => x.StatusAuditoria, "STATUS_AUDITORIA").Default(null).CustomType<StatusAuditoria>();
            Map(x => x.JustificativaAuditoria, "JUSTIFICATIVA_AUDITORIA");
            //Map(x => x.Motivo_Status, "MOTIVO_STATUS").Length(200);
            Map(x => x.Assunto, "ASSUNTO").Length(100);
            Map(x => x.Problema_Relatado, "PROBLEMA_RELATADO").Length(200);

            
            Map(x => x.Tipo_Pessoa_Contato, "TIPO_PESSOA_CONTATO").CustomType<Tipo_Pessoa_Contato>();
            Map(x => x.Pessoa_Contato, "PESSOA_CONTATO").Length(200);
            Map(x => x.IdPessoaContato, "ID_PESSOA_CONTATO");
            Map(x => x.ContatosCopia, "CONTATOS_COPIA").Length(5000);
            Map(x => x.Grau_Ligacao, "GRAU_LIGACAO").Length(80);
            Map(x => x.DataIndicador_EtapaFluxo, "DATAINDICADOR_ETAPAFLUXO");
            Map(x => x.DataInicio_EtapaFluxo, "DATAINICIO_ETAPAFLUXO");
            Map(x => x.Contrato, "CONTRATO");

            References(x => x.Categoria_Atendimento, "ID_CATEGORIA_ATENDIMENTO").ForeignKey("FK_ATENDIMENTO_CAT_ANTEND");
            References(x => x.Cliente, "ID_CLIENTE").ForeignKey("FK_ATENDIMENTO_CLIENTE");
            References(x => x.Endereco_Mudanca, "ID_ENDERECO_MUDANCA").ForeignKey("FK_ATENDIMENTO_ENDMUD");
            References(x => x.Origem_Atendimento, "ID_ORIGEM_ATENDIMENTO").ForeignKey("FK_ATENDIMENTO_ORIG_ATEND");
            References(x => x.PlanoProduto, "ID_PLANOPRODUTO").ForeignKey("FK_ATENDIMENTO_PLANOPROD");
            References(x => x.ClienteContrato, "ID_CLIENTECONTRATO").ForeignKey("FK_ATENDIMENTO_CLIENTECONTRATO");
            References(x => x.TrocaTitularidade, "ID_TROCA_TITULAR").ForeignKey("FK_TROCA_TITULARIDADE");
            References(x => x.OrganizacaoCliente, "ID_ORGANIZACAO_CLIENTE").ForeignKey("FK_ORGANIZACAO_CLIENTE");
            Map(x => x.Id_Email_Abertura, "ID_EMAIL_ABERTURA");
            Map(x => x.ConversationSid, "CONVERSATIONSID");
            Map(x => x.MessageID, "MESSAGEID");
            Map(x => x.DataHora_VinculoResponsavel, "DATAHORA_VINC_RESP");

            References(x => x.Tipo_Atendimento, "ID_TIPO_ATENDIMENTO").ForeignKey("FK_ATENDIMENTO_TIPO_ATEND");
            References(x => x.EtapaFluxo, "ID_ETAPA_FLUXO").ForeignKey("FK_ATENDIMENTO_ETAPA_FLUXO");
            References(x => x.Setor, "ID_DEPARTAMENTO").ForeignKey("FK_ATENDIMENTO_DEPARTAMENTO");
            References(x => x.Responsavel, "ID_RESPONSAVEL").ForeignKey("FK_ATENDIMENTO_RESPONSAVEL");
            References(x => x.Motivo_Status, "ID_MOTIVO_STATUS").ForeignKey("FK_ATENDIMENTO_MOTIVO_STATUS");
            HasMany(x => x.Ultima_Acao).KeyColumn("ID_ATENDIMENTO").Cascade.All().Inverse();
            
        }
    }
}
