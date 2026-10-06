using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using Resources;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sinapse.ModelLog;
using Sinapse.Model.Identity;

using Sinapse.Model;
using Aniel.Connect.ModelLog.nhibernate;

namespace Aniel.Connect.Model.nhibernate
{
    public class Uf : BaseModel
    {

        public virtual Pais Pais { get; set; }
        public virtual String Sigla { get; set; }
        public virtual String Codigo_IBGE { get; set; }
        public virtual String Descricao { get; set; }
        public virtual Status_Default? Status { get; set; }
        public virtual string CodAuxiliar { get; set; }

        public virtual List<Log_Uf> GeraLog(Uf pOld, String pUsuario, String pNomeUsuario, TipoOperLog pTipoOperLog)
        {
            List<Log_Uf> logs = new List<Log_Uf>();

            if (pTipoOperLog != TipoOperLog.Alteracao)
            {
                Log_InsereTipoOper<Uf, Log_Uf>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString());
            }
            else
            {
                Log_ComparaProperty<Uf, Log_Uf>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.ID, pOld, this, p => p.Id);
                Log_ComparaProperty<Uf, Log_Uf>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.PAIS, pOld, this, p => p.Pais);
                Log_ComparaProperty<Uf, Log_Uf>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.CODIGO_IBGE, pOld, this, p => p.Codigo_IBGE);
                Log_ComparaProperty<Uf, Log_Uf>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.DESCRICAO, pOld, this, p => p.Descricao);
                Log_ComparaProperty<Uf, Log_Uf>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.SIGLA_UF, pOld, this, p => p.Sigla);
                Log_ComparaProperty<Uf, Log_Uf>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.STATUS, pOld, this, p => p.Status);
            }

            return logs;
        

    
        }
    }
}
