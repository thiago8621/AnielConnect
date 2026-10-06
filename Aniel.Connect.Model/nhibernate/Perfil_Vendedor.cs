using Aniel.Connect.ModelLog.nhibernate;
using Aniel.T5.Model.nhibernate;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using Sinapse.ModelLog;
using Aniel.Connect.Model;
using System;
using System.Collections.Generic;
using Resources;

namespace Aniel.Connect.Model.nhibernate
{
    public class Perfil_Vendedor : BaseModel 
    {
        public virtual string Descricao { get; set; }
        public virtual bool Area_Cobertura { get; set; }
        public virtual bool Tipo_Pessoa { get; set; }
        public virtual bool Campanha_Regiao { get; set; }
        public virtual bool Categoria_Cliente { get; set; }
        public virtual bool Meio_Capitacao { get; set; }
        public virtual bool Autorizacao_Vendas { get; set; }

        public virtual Status_Default Status { get; set; }

        public virtual List<LogHistoricoVenda> GeraLog(Perfil_Vendedor pOld, String pUsuario, String pNomeUsuario, TipoOperLog pTipoOperLog)
        {
            List<LogHistoricoVenda> logs = new List<LogHistoricoVenda>();

            if (pTipoOperLog != TipoOperLog.Alteracao)
            {
                Log_InsereTipoOper<Perfil_Vendedor, LogHistoricoVenda>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString());
            }
            else
            {
                Log_ComparaProperty<Perfil_Vendedor, LogHistoricoVenda>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.STATUS, pOld, this, p => p.Status);
                Log_ComparaProperty<Perfil_Vendedor, LogHistoricoVenda>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.DESCRICAO, pOld, this, p => p.Descricao);
                
            }

            return logs;

        }
    }
}
