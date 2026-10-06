using Aniel.Connect.Model.enumerado;
using Aniel.Connect.ModelLog.nhibernate;
using Resources;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.Model.nhibernate;
using Sinapse.ModelLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Atendimento : BaseModel
    {

        // Cliente
        public virtual Cliente Cliente { get; set; }
        // Cliente
        public virtual String Contrato { get; set; }

        //TrocaTitularidade
        public virtual TrocaTitularidade TrocaTitularidade { get; set; }
     

        public virtual TipoPreenchimentoAtendimento TipoPreenchimento { get; set; }

        // Plano/Produto        
        public virtual Cliente_PlanoProduto PlanoProduto { get; set; }

        // ClienteContrato
        public virtual ClienteContrato ClienteContrato { get; set; }
        

        // Sobre o Atendimento 
        public virtual String Numero { get; set; }

        public virtual Endereco_Cliente Endereco_Mudanca { get; set; }

        public virtual String Protocolo { get; set; }

        public virtual DateTime? Data_Hora_Abertura { get; set; }

        public virtual DateTime? Data_Hora_Fechamento { get; set; }

        public virtual Origem_Atendimento Origem_Atendimento { get; set; }

        public virtual String Assunto { get; set; }

        public virtual Tipo_Atendimento Tipo_Atendimento { get; set; }

        public virtual Categoria_Atendimento Categoria_Atendimento { get; set; }

        public virtual Departamento Setor { get; set; }

        public virtual IdentityUser Responsavel { get; set; }

        public virtual Prioridade_Atendimento? Prioridade { get; set; }

        public virtual Tipo_Pessoa_Contato Tipo_Pessoa_Contato { get; set; }

        public virtual long? IdPessoaContato { get; set; }
        public virtual String Pessoa_Contato { get; set; }
        public virtual String ContatosCopia { get; set; }

        public virtual String Grau_Ligacao { get; set; }

        public virtual String Problema_Relatado { get; set; }

        public virtual Motivo_Status_Atendimento Motivo_Status { get; set; }

        public virtual Status_Default Status { get; set; }
        
        public virtual Status_Atendimento Status_Atendimento { get; set; }

        public virtual DateTime? DataHora_Indicador { get; set; }
        public virtual Int64? Id_Email_Abertura { get; set; }
        public virtual String ConversationSid { get; set; }
        public virtual FluxoAtendimento EtapaFluxo { get; set; }
        public virtual DateTime? DataIndicador_EtapaFluxo { get; set; }
        public virtual DateTime? DataInicio_EtapaFluxo { get; set; }
        public virtual StatusAuditoria StatusAuditoria { get; set; }
        public virtual string JustificativaAuditoria { get; set; }
        public virtual string MessageID { get; set; }
        public virtual DateTime? DataHora_VinculoResponsavel { get; set; }
        public virtual Organizacao OrganizacaoCliente { get; set; }

        public virtual String IdJobNotificacaoSla { get; set; }


        private IList<Atendimento_Acao> Ultima_AcaoLista;

        public virtual IList<Atendimento_Acao> Ultima_Acao

        {
            get
            {
                if (Ultima_AcaoLista == null)
                    Ultima_AcaoLista = new List<Atendimento_Acao>();
                return Ultima_AcaoLista;
            }
            set
            {
                Ultima_AcaoLista = value;
            }
        }

        public virtual List<LogAtendimento> GeraLog(Atendimento pOld, String pUsuario, String pNomeUsuario, TipoOperLog pTipoOperLog)
        {
            List<LogAtendimento> logs = new List<LogAtendimento>();
            try
            {


            if (pTipoOperLog != TipoOperLog.Alteracao)
            {
                Log_InsereTipoOper<Atendimento, LogAtendimento>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString());
            }
            else
            {
                Log_ComparaProperty<Atendimento, LogAtendimento>(logs, pUsuario,pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.STATUS, pOld, this, p => p.Status);
                Log_ComparaProperty<Atendimento, LogAtendimento>(logs, pUsuario,pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.STATUS_ATEDIMENTO, pOld, this, p => p.Status_Atendimento);
                Log_ComparaProperty<Atendimento, LogAtendimento>(logs, pUsuario,pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.PROTOCOLO, pOld, this, p => p.Numero);
                Log_ComparaProperty<Atendimento, LogAtendimento>(logs, pUsuario,pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.PROBLEMA_RELATADO, pOld, this, p => p.Problema_Relatado);
                Log_ComparaProperty<Atendimento, LogAtendimento>(logs, pUsuario,pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.ASSUNTO, pOld, this, p => p.Assunto);
                Log_ComparaProperty<Atendimento, LogAtendimento>(logs, pUsuario,pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.DATAFECHAMENTO, pOld, this, p => p.Data_Hora_Fechamento);
                Log_ComparaProperty<Atendimento, LogAtendimento>(logs, pUsuario,pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.DATA_HORA_VENCIMENTO_OS, pOld, this, p => p.DataHora_Indicador);
                Log_ComparaProperty<Atendimento, LogAtendimento>(logs, pUsuario,pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.PRIORIDADE, pOld, this, p => p.Prioridade);
                Log_ComparaProperty<Atendimento, LogAtendimento>(logs, pUsuario,pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.TIPO_PESSOA_CONTATO, pOld, this, p => p.Tipo_Pessoa_Contato);
                Log_ComparaProperty<Atendimento, LogAtendimento>(logs, pUsuario,pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.PESSOA_QUE_ENTROU_EM_CONTATO, pOld, this, p => p.Pessoa_Contato);
                Log_ComparaProperty<Atendimento, LogAtendimento>(logs, pUsuario,pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.PESSOA_QUE_ENTROU_EM_CONTATO, pOld, this, p => p.IdPessoaContato);
                Log_ComparaProperty<Atendimento, LogAtendimento>(logs, pUsuario,pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.GRAU_DE_INSTRUCAO, pOld, this, p => p.Grau_Ligacao);
                Log_ComparaProperty<Atendimento, LogAtendimento>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.PRAZO_ETAPA_ATUAL, pOld, this, p => p.DataIndicador_EtapaFluxo);


                Log_ComparaProperty<Atendimento, LogAtendimento,Tipo_Atendimento>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.TIPO_ATENDIMENTO, pOld, this, p => p.Tipo_Atendimento,y => $"{y.Id} - {y.Nome}");
                Log_ComparaProperty<Atendimento, LogAtendimento, Origem_Atendimento>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.ORIGEM_ATENDIMENTO, pOld, this, p => p.Origem_Atendimento, y => $"{y.Id} - {y.Descricao}");
                Log_ComparaProperty<Atendimento, LogAtendimento, Cliente>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.CLIENTE, pOld, this, p => p.Cliente, y => $"{y.Id} - {y.NomeFantasia}");
                Log_ComparaProperty<Atendimento, LogAtendimento, Cliente_PlanoProduto>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.PLANO_PRODUTO, pOld, this, p => p.PlanoProduto, y => y.Plano != null ? $"{y.Id} - {y.Plano.Descricao}" : $"{y.Id} - {y.Produto.Descricao}");
                Log_ComparaProperty<Atendimento, LogAtendimento, Motivo_Status_Atendimento>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.MOTIVOS_STATUS_ATENDIMENTO, pOld, this, p => p.Motivo_Status, y => $"{y.Id} - {y.Descricao}");
                Log_ComparaProperty<Atendimento, LogAtendimento, Departamento>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.SETOR, pOld, this, p => p.Setor, y => $"{y.Id} - {y.Descricao}");
                Log_ComparaProperty<Atendimento, LogAtendimento, Categoria_Atendimento>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.CATEGORIA_ATENDIMENTO, pOld, this, p => p.Categoria_Atendimento, y => $"{y.Id} - {y.Descricao}");
                Log_ComparaProperty<Atendimento, LogAtendimento, IdentityUser>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.TIPO_ATENDIMENTO, pOld, this, p => p.Responsavel, y => $"{y.Id} - {y.UserName}",z => z.Id);
                Log_ComparaProperty<Atendimento, LogAtendimento, FluxoAtendimento>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.ETAPA_ATUAL, pOld, this, p => p.EtapaFluxo, y => $"{y.Id} - {y.Descricao}", z => z.Id);
                //Log_ComparaProperty<Atendimento, LogAtendimento, IdentityUser>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.PRAZO_ETAPA_ATUAL, pOld, this, p => p.DataIndicador_EtapaFluxo, y => $"{y.Id} - {y.UserName}", z => z.Id);

            }
            }
            catch
            {
                return logs;
            }


            return logs;
        }

    }
}
