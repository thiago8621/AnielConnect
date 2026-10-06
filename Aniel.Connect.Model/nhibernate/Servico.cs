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
    public class Servico : BaseModel
    {       

        // Sobre o Servico 

        public virtual String NumOs { get; set; }

        public virtual DateTime? Data_Hora_Abertura { get; set; }

        public virtual DateTime? Data_Hora_Fechamento { get; set; }

        public virtual IdentityUser Responsavel { get; set; }

        public virtual Int64? Id_Email_Abertura { get; set; }
        public virtual String ConversationSid { get; set; }





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

        //public virtual List<LogAtendimento> GeraLog(Servico pOld, String pUsuario, String pNomeUsuario, TipoOperLog pTipoOperLog)
        //{
        //    List<LogAtendimento> logs = new List<LogAtendimento>();

        //    if (pTipoOperLog != TipoOperLog.Alteracao)
        //    {
        //        Log_InsereTipoOper<Servico, LogAtendimento>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString());
        //    }
        //    else
        //    {
        //        Log_ComparaProperty<Servico, LogAtendimento>(logs, pUsuario,pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.STATUS, pOld, this, p => p.Status);
        //        Log_ComparaProperty<Servico, LogAtendimento>(logs, pUsuario,pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.STATUS_ATEDIMENTO, pOld, this, p => p.Status_Atendimento);
        //        Log_ComparaProperty<Servico, LogAtendimento>(logs, pUsuario,pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.PROTOCOLO, pOld, this, p => p.Numero);
        //        Log_ComparaProperty<Servico, LogAtendimento>(logs, pUsuario,pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.PROBLEMA_RELATADO, pOld, this, p => p.Problema_Relatado);
        //        Log_ComparaProperty<Servico, LogAtendimento>(logs, pUsuario,pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.ASSUNTO, pOld, this, p => p.Assunto);
        //        Log_ComparaProperty<Servico, LogAtendimento>(logs, pUsuario,pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.DATAFECHAMENTO, pOld, this, p => p.Data_Hora_Fechamento);
        //        Log_ComparaProperty<Servico, LogAtendimento>(logs, pUsuario,pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.DATA_HORA_VENCIMENTO_OS, pOld, this, p => p.DataHora_Indicador);
        //        Log_ComparaProperty<Servico, LogAtendimento>(logs, pUsuario,pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.PRIORIDADE, pOld, this, p => p.Prioridade);
        //        Log_ComparaProperty<Servico, LogAtendimento>(logs, pUsuario,pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.TIPO_PESSOA_CONTATO, pOld, this, p => p.Tipo_Pessoa_Contato);
        //        Log_ComparaProperty<Servico, LogAtendimento>(logs, pUsuario,pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.PESSOA_QUE_ENTROU_EM_CONTATO, pOld, this, p => p.Pessoa_Contato);
        //        Log_ComparaProperty<Servico, LogAtendimento>(logs, pUsuario,pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.GRAU_DE_INSTRUCAO, pOld, this, p => p.Grau_Ligacao);
        //        Log_ComparaProperty<Servico, LogAtendimento>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.PRAZO_ETAPA_ATUAL, pOld, this, p => p.DataIndicador_EtapaFluxo);


        //        Log_ComparaProperty<Servico, LogAtendimento,Tipo_Atendimento>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.TIPO_ATENDIMENTO, pOld, this, p => p.Tipo_Atendimento,y => $"{y.Id} - {y.Nome}");
        //        Log_ComparaProperty<Servico, LogAtendimento, Origem_Atendimento>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.ORIGEM_ATENDIMENTO, pOld, this, p => p.Origem_Atendimento, y => $"{y.Id} - {y.Descricao}");
        //        Log_ComparaProperty<Servico, LogAtendimento, Cliente>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.CLIENTE, pOld, this, p => p.Cliente, y => $"{y.Id} - {y.NomeFantasia}");
        //        Log_ComparaProperty<Servico, LogAtendimento, Cliente_PlanoProduto>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.PLANO_PRODUTO, pOld, this, p => p.PlanoProduto, y => y.Plano != null ? $"{y.Id} - {y.Plano.Descricao}" : $"{y.Id} - {y.Produto.Descricao}");
        //        Log_ComparaProperty<Servico, LogAtendimento, Motivo_Status_Atendimento>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.MOTIVOS_STATUS_ATENDIMENTO, pOld, this, p => p.Motivo_Status, y => $"{y.Id} - {y.Descricao}");
        //        Log_ComparaProperty<Servico, LogAtendimento, Departamento>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.SETOR, pOld, this, p => p.Setor, y => $"{y.Id} - {y.Descricao}");
        //        Log_ComparaProperty<Servico, LogAtendimento, Categoria_Atendimento>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.CATEGORIA_ATENDIMENTO, pOld, this, p => p.Categoria_Atendimento, y => $"{y.Id} - {y.Descricao}");
        //        Log_ComparaProperty<Servico, LogAtendimento, IdentityUser>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.TIPO_ATENDIMENTO, pOld, this, p => p.Responsavel, y => $"{y.Id} - {y.UserName}",z => z.Id);
        //        Log_ComparaProperty<Servico, LogAtendimento, FluxoAtendimento>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.ETAPA_ATUAL, pOld, this, p => p.EtapaFluxo, y => $"{y.Id} - {y.Descricao}", z => z.Id);
        //        //Log_ComparaProperty<Atendimento, LogAtendimento, IdentityUser>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.PRAZO_ETAPA_ATUAL, pOld, this, p => p.DataIndicador_EtapaFluxo, y => $"{y.Id} - {y.UserName}", z => z.Id);

        //    }



        //    return logs;
        //}

    }
}
