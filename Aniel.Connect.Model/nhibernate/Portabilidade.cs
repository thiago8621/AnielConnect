using Aniel.Connect.Model.enumerado;
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
    public class Portabilidade : BaseModel
    {
        public virtual Int64 Codigo { get; set; }
        public virtual string  NumeroDid_Portabilidade { get; set; } //VARCHAR(16)
        public virtual Did  Did_Substituido { get; set; } //VARCHAR(16)
        public virtual CircuitoTelefonia  CircuitoSelecionado { get; set; } //VARCHAR(16)
        public virtual String Pais { get; set; }
        public virtual String UF { get; set; }
        public virtual String Cidade { get; set; }
        public virtual Cliente Cliente { get; set; }
        public virtual String Numero_Sip { get; set; }
        public virtual DateTime? Dia_Previsto_Portabilidade { get; set; }
        public virtual Status_Portabilidade Status { get; set; }

        public virtual List<LogFranquia> GeraLog(Portabilidade pOld, String pUsuario, String pNomeUsuario, TipoOperLog pTipoOperLog)
        {
            List<LogFranquia> logs = new List<LogFranquia>();

            if (pTipoOperLog != TipoOperLog.Alteracao)
            {
                Log_InsereTipoOper<Franquia, LogFranquia>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString());
            }
            else
            {
                Log_ComparaProperty<Portabilidade, LogFranquia>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.DESCRICAO, pOld, this, p => p.NumeroDid_Portabilidade);
                Log_ComparaProperty<Portabilidade, LogFranquia>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.PAIS, pOld, this, p => p.Pais);
                Log_ComparaProperty<Portabilidade, LogFranquia>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.CIDADE, pOld, this, p => p.Cidade);
                Log_ComparaProperty<Portabilidade, LogFranquia>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.UF, pOld, this, p => p.UF);
                Log_ComparaProperty<Portabilidade, LogFranquia>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.NUMERO_CIRCUITO, pOld, this, p => p.Numero_Sip);
                Log_ComparaProperty<Portabilidade, LogFranquia>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.DIA_PREVISTO_PORTABILIDADE, pOld, this, p => p.Dia_Previsto_Portabilidade);
                Log_ComparaProperty<Portabilidade, LogFranquia,Did>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.DID, pOld, this, p => p.Did_Substituido, y => $"{y.Id} - {y.Numero}");
                Log_ComparaProperty<Portabilidade, LogFranquia,CircuitoTelefonia>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.CIRCUITO, pOld, this, p => p.CircuitoSelecionado, y => $"{y.Id} - {y.Numero_Sip}");
                Log_ComparaProperty<Portabilidade, LogFranquia>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.STATUS, pOld, this, p => p.Status);
                
            }



            return logs;
        }


    }
}
