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

namespace Aniel.T5.Model.nhibernate
{
    public class EscritorioAniel : BaseModel
    {
        public virtual String Cod_Escritorio { get; set; }
        public virtual String Descricao { get; set; }
        public virtual Status_Default Status { get; set; }
        public virtual String Observacao { get; set; }

        public virtual List<LogEscritorioAniel> GeraLog(EscritorioAniel pOld, String pUsuario, String pNomeUsuario, TipoOperLog pTipoOperLog)
        {
            List<LogEscritorioAniel> logs = new List<LogEscritorioAniel>();

            if (pTipoOperLog == TipoOperLog.Importacao)
            {
                Log_ComparaProperty<EscritorioAniel, LogEscritorioAniel>(logs, pUsuario, pNomeUsuario, TipoOperLog.Importacao, this.Id.ToString(), RsCadastros.IMPORTAR_GRUPO_AREA, pOld, this, p => p.Observacao);
            }
            else if (pTipoOperLog != TipoOperLog.Alteracao)
            {
                Log_InsereTipoOper<EscritorioAniel, LogEscritorioAniel>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString());

            }
            else
            {
                Log_ComparaProperty<EscritorioAniel, LogEscritorioAniel>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.STATUS, pOld, this, p => p.Status);
                Log_ComparaProperty<EscritorioAniel, LogEscritorioAniel>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.DESCRICAO_GRUPO_AREA, pOld, this, p => p.Descricao);
            }


            return logs;
        }

    }
}
