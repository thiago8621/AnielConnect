using Aniel.Connect.Model.enumerado;
using Aniel.Connect.ModelLog.nhibernate;
using Resources;
using Sinapse.Model;
using Sinapse.Model.nhibernate;
using Sinapse.ModelLog;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Model.nhibernate
{
    public class EtapaVenda : BaseModel
    {
        public virtual Venda Venda { get; set; }
        public virtual Sinapse.Model.enumerados.EnumEtapaVenda Etapa { get; set; }
        public virtual string Descricao { get; set; }
        public virtual Sinapse.Model.enumerados.StatusEtapaVenda Status { get; set; }
        public virtual string MotivoAprovManual { get; set; }
        public virtual string MotivoAprovManualESC { get; set; }

        public virtual Cliente Cliente { get; set; }
        public virtual TrocaTitularidade TrocaTitularidade { get; set; }

        public virtual List<LogEtapaVendas> GeraLog(EtapaVenda pOld, String pUsuario, String pNomeUsuario, TipoOperLog pTipoOperLog)
        {
            List<LogEtapaVendas> logs = new List<LogEtapaVendas>();

            if (pTipoOperLog != TipoOperLog.Alteracao)
            {
                Log_InsereTipoOper<EtapaVenda, LogEtapaVendas>(logs, pUsuario, pNomeUsuario, pTipoOperLog, pOld.Venda.Id.ToString());
            }
            else
            {
                Log_ComparaProperty<EtapaVenda, LogEtapaVendas>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Venda != null ? this.Venda.Id.ToString() : this.TrocaTitularidade.Id.ToString(), RsCadastros.ETAPA + ": "+ this.Descricao, pOld, this, p => p.Status);
                Log_ComparaProperty<EtapaVenda, LogEtapaVendas>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Venda != null ? this.Venda.Id.ToString() : this.TrocaTitularidade.Id.ToString(), RsCadastros.DESCRICAO, pOld, this, p => p.Descricao);
                Log_ComparaProperty<EtapaVenda, LogEtapaVendas>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Venda != null ? this.Venda.Id.ToString() : this.TrocaTitularidade.Id.ToString(), RsCadastros.ETAPA_ALTERADA, pOld, this, p => p.Etapa);
                //Log_ComparaProperty<EtapaVenda, LogEtapaVendas>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Venda.Id.ToString(), RsCadastros.MOTIVO, pOld, this, p => p.MotivoAprovManual);
                            
            }

            return logs;
        }
    }
}
