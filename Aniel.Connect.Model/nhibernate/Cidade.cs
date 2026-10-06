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
    public class Cidade : BaseModel
    {
        public virtual String Codigo_IBGE { get; set; }
        public virtual String Descricao { get; set; }
        public virtual Pais Pais { get; set; }
        public virtual Uf Id_UF { get; set; }
        public virtual String UF { get; set; }
        public virtual Status_Default? Status { get; set; }
        public virtual List<Log_Cidade> GeraLog(Cidade pOld, String pUsuario, String pNomeUsuario, TipoOperLog pTipoOperLog)
        {
            List<Log_Cidade> logs = new List<Log_Cidade>();

            if (pTipoOperLog != TipoOperLog.Alteracao)
            {
                Log_InsereTipoOper<Cidade, Log_Cidade>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString());
            }
            else
            {
                Log_ComparaProperty<Cidade, Log_Cidade>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.ID, pOld, this, p => p.Id);
                Log_ComparaProperty<Cidade, Log_Cidade>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.DESCRICAO, pOld, this, p => p.Descricao);
                Log_ComparaProperty<Cidade, Log_Cidade>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.PAIS, pOld, this, p => p.Pais);
                Log_ComparaProperty<Cidade, Log_Cidade>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.UF, pOld, this, p => p.Id_UF);
                Log_ComparaProperty<Cidade, Log_Cidade>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.STATUS, pOld, this, p => p.Status);            

            }

            return logs;
        }

        public virtual String DDD { get; set; }
        public virtual string CodigoAuxiliar { get; set; }
    }

}
