using Aniel.Connect.ModelLog.nhibernate;
using Resources;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using Sinapse.ModelLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Franquia : BaseModel
    {
        public virtual string Descricao { get; set; }
        public virtual Int32? Minutos { get; set; }
        public virtual bool Vc1 { get; set; }
        public virtual bool Vc2 { get; set; }
        public virtual bool Vc3 { get; set; }
        public virtual bool Lc { get; set; }
        public virtual bool Ldn { get; set; }
        public virtual bool Ldi { get; set; }
        public virtual Status_Default Status { get; set; }
        public virtual String CodigoAuxiliar { get; set; }

        public virtual List<LogFranquia> GeraLog(Franquia pOld, String pUsuario, String pNomeUsuario,TipoOperLog pTipoOperLog)
        {
            List<LogFranquia> logs = new List<LogFranquia>();

            if (pTipoOperLog != TipoOperLog.Alteracao)
            {
                Log_InsereTipoOper<Franquia, LogFranquia>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString());
            }
            else
            {
                Log_ComparaProperty<Franquia, LogFranquia>(logs, pUsuario,pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.DESCRICAO, pOld, this, p => p.Descricao);
                Log_ComparaProperty<Franquia, LogFranquia>(logs, pUsuario,pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.STATUS, pOld, this, p => p.Status);
                Log_ComparaProperty<Franquia, LogFranquia>(logs, pUsuario,pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.VC1, pOld, this, p => p.Vc1);
                Log_ComparaProperty<Franquia, LogFranquia>(logs, pUsuario,pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.VC2, pOld, this, p => p.Vc2);
                Log_ComparaProperty<Franquia, LogFranquia>(logs, pUsuario,pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.VC3, pOld, this, p => p.Vc3);
                Log_ComparaProperty<Franquia, LogFranquia>(logs, pUsuario,pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.LC, pOld, this, p => p.Lc);
                Log_ComparaProperty<Franquia, LogFranquia>(logs, pUsuario,pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.LDN, pOld, this, p => p.Ldn);
                Log_ComparaProperty<Franquia, LogFranquia>(logs, pUsuario,pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.LDI, pOld, this, p => p.Ldi);
                Log_ComparaProperty<Franquia, LogFranquia>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.MINUTOS_FRANQUIA, pOld, this, p => p.Minutos);
            }



            return logs;
        }
    }


}
