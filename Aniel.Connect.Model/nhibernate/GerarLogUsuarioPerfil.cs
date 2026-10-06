using Aniel.Connect.ModelLog.nhibernate;
using Sinapse.Model.Identity;
using Sinapse.Model.nhibernate;
using Sinapse.ModelLog;
using System;
using System.Collections.Generic;
using Resources;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class GerarLogUsuarioPerfil 
    {
        //    public virtual List<LogVincularPerfilAniel> GeraLog(IdentityUser pOld, String pUsuario, String pNomeUsuario, TipoOperLog pTipoOperLog)
        //    {
        //        List<LogVincularPerfilAniel> logs = new List<LogVincularPerfilAniel>();

        //        if (pTipoOperLog != TipoOperLog.Alteracao)
        //        {
        //            Log_InsereTipoOper<IdentityUser, LogVincularPerfilAniel>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString());
        //        }
        //        else
        //        {
        //            Log_ComparaProperty<IdentityUser, LogVincularPerfilAniel>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.USUARIO, pOld, this, p => p.UserName);
        //            Log_ComparaProperty<IdentityUser, LogVincularPerfilAniel>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.ATIVO, pOld, this, p => p.Ativo);

        //        }

        //        return logs;

      }
    }
