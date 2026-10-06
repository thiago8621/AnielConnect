using Aniel.Connect.Model.enumerado;
using Aniel.Connect.ModelLog.nhibernate;
using Sinapse.Model.nhibernate;
using Sinapse.ModelLog;
using System;
using System.Collections.Generic;
using Resources;

namespace Aniel.Connect.Model.nhibernate
{
    public class AtendimentoCampos : BaseModel
    {
        public virtual Boolean? CampoObrigatorio { get; set; }
        public virtual String Titulo { get; set; }
        public virtual TipoCampoGravar TipoCampoGravar { get; set; }
        public virtual Boolean Status { get; set; }
        public virtual bool VisualizacaoTotal { get; set; }

        public virtual List<LogAtendimentoCampos> GeraLog(AtendimentoCampos pOld, String pUsuario, String pNomeUsuario, TipoOperLog pTipoOperLog)
        {
            List<LogAtendimentoCampos> logs = new List<LogAtendimentoCampos>();

            if (pTipoOperLog != TipoOperLog.Alteracao)
            {
                Log_InsereTipoOper<AtendimentoCampos, LogAtendimentoCampos>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString());
            }
            else
            {
                Log_ComparaProperty<AtendimentoCampos, LogAtendimentoCampos>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.STATUS, pOld, this, p => p.Status);
                Log_ComparaProperty<AtendimentoCampos, LogAtendimentoCampos>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), "Campo Obrigatório", pOld, this, p => p.CampoObrigatorio);
                Log_ComparaProperty<AtendimentoCampos, LogAtendimentoCampos>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.TITULO, pOld, this, p => p.Titulo);
                Log_ComparaProperty<AtendimentoCampos, LogAtendimentoCampos>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), "Tipo do Campo", pOld, this, p => p.TipoCampoGravar);
                Log_ComparaProperty<AtendimentoCampos, LogAtendimentoCampos>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), "Usuário Alteração", pOld, this, p => p.UsuarioAlteracao);
                Log_ComparaProperty<AtendimentoCampos, LogAtendimentoCampos>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), "Data da Alteração", pOld, this, p => p.DataHoraAlteracao);
            }

            return logs;
        }
    }

    public class AtendimentoCamposRespostas : BaseModel
    {
        public virtual String RespostaPersonalizada {get;set;}
        public virtual AtendimentoCampos AtendimentoCampos { get; set; }

        public virtual List<LogAtendimentoCampos> GeraLog(AtendimentoCamposRespostas pOld, String pUsuario, String pNomeUsuario, TipoOperLog pTipoOperLog)
        {
            List<LogAtendimentoCampos> logs = new List<LogAtendimentoCampos>();

            if (pTipoOperLog != TipoOperLog.Alteracao)
            {
                Log_InsereTipoOper<AtendimentoCamposRespostas, LogAtendimentoCampos>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString());
            }
            else
            {
                Log_ComparaProperty<AtendimentoCamposRespostas, LogAtendimentoCampos>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.AtendimentoCampos.Id.ToString(), "Resposta Personalizada", pOld, this, p => p.RespostaPersonalizada);
                Log_ComparaProperty<AtendimentoCamposRespostas, LogAtendimentoCampos>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.AtendimentoCampos.Id.ToString(), "Usuário Alteração", pOld, this, p => p.UsuarioAlteracao);
                Log_ComparaProperty<AtendimentoCamposRespostas, LogAtendimentoCampos>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.AtendimentoCampos.Id.ToString(), "Data da Alteração", pOld, this, p => p.DataHoraAlteracao);
                Log_ComparaProperty<AtendimentoCamposRespostas, LogAtendimentoCampos, AtendimentoCampos>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.AtendimentoCampos.Id.ToString(), "Atendimento Campo Vinculado", pOld, this, p => p.AtendimentoCampos, y => $"{y.Id} - {y.Titulo}");
            }

            return logs;
        }
    } 
    
    public class AtendimentoCamposTipo : BaseModel
    {
        public virtual long? TipoAtendimento {get;set;}
        public virtual string DescricaoTipo {get;set;}
        public virtual AtendimentoCampos AtendimentoCampos { get; set; }

        public virtual List<LogAtendimentoCampos> GeraLog(AtendimentoCamposTipo pOld, String pUsuario, String pNomeUsuario, TipoOperLog pTipoOperLog)
        {
            List<LogAtendimentoCampos> logs = new List<LogAtendimentoCampos>();

            if (pTipoOperLog != TipoOperLog.Alteracao)
            {
                Log_InsereTipoOper<AtendimentoCamposTipo, LogAtendimentoCampos>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString());
            }
            else
            {
                Log_ComparaProperty<AtendimentoCamposTipo, LogAtendimentoCampos>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.AtendimentoCampos.Id.ToString(), "Tipo do Atendimento", pOld, this, p => p.TipoAtendimento);
                Log_ComparaProperty<AtendimentoCamposTipo, LogAtendimentoCampos>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.AtendimentoCampos.Id.ToString(), "Descrição do Tipo de Atendimento", pOld, this, p => p.DescricaoTipo);
                Log_ComparaProperty<AtendimentoCamposTipo, LogAtendimentoCampos>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.AtendimentoCampos.Id.ToString(), "Usuário Alteração", pOld, this, p => p.UsuarioAlteracao);
                Log_ComparaProperty<AtendimentoCamposTipo, LogAtendimentoCampos>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.AtendimentoCampos.Id.ToString(), "Data da Alteração", pOld, this, p => p.DataHoraAlteracao);
                Log_ComparaProperty<AtendimentoCamposTipo, LogAtendimentoCampos, AtendimentoCampos>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.AtendimentoCampos.Id.ToString(), "Atendimento Campo Vinculado", pOld, this, p => p.AtendimentoCampos, y => $"{y.Id} - {y.Titulo}");
            }
            return logs;
        }
    }

}