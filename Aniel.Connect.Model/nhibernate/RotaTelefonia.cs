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
    public class RotaTelefonia : BaseModel
    {
        public virtual string Nome { get; set; }
        public virtual Status_Default Status { get; set; }
        public virtual string CodigoAuxiliar { get; set; }

        public virtual List<LogRotaTelefonia> GeraLog(RotaTelefonia pOld, String pUsuario, String pNomeUsuario, TipoOperLog pTipoOperLog)
        {
            List<LogRotaTelefonia> logs = new List<LogRotaTelefonia>();

            if (pTipoOperLog != TipoOperLog.Alteracao)
            {
                Log_InsereTipoOper<Franquia, LogRotaTelefonia>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString());
            }
            else
            {
                Log_ComparaProperty<RotaTelefonia, LogRotaTelefonia>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.DESCRICAO, pOld, this, p => p.Nome);
            }



            return logs;
        }
    }


}
