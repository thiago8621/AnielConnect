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
    public class VincularPerfilAniel : BaseModel
    {
        public virtual String Tecnico { get; set; }
        public virtual List<String> equipes { get; set; }
        public virtual String usuario { get; set; }
        public virtual Int64? id_perfil { get; set; }
        public virtual List<LogVincularPerfilAniel> GeraLog(VincularPerfilAniel pOld, String pUsuario, String pNomeUsuario, TipoOperLog pTipoOperLog)
        {
            List<LogVincularPerfilAniel> logs = new List<LogVincularPerfilAniel>();

            if (pTipoOperLog == TipoOperLog.Importacao)
            {
                Log_ComparaProperty<VincularPerfilAniel, LogVincularPerfilAniel>(logs, pUsuario, pNomeUsuario, TipoOperLog.Importacao, this.Id.ToString(), RsCadastros.IMPORTAR_LOCALIDADE, pOld, this, p => p.Tecnico);
            }
            else if (pTipoOperLog != TipoOperLog.Alteracao)
            {
                Log_InsereTipoOper<VincularPerfilAniel, LogVincularPerfilAniel>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString());
            }
            else
            {
                Log_ComparaProperty<VincularPerfilAniel, LogVincularPerfilAniel>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.STATUS, pOld, this, p => p.Tecnico);

            }

            return logs;

        }

    }
}
