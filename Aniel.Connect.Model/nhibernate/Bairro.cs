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
    public class Bairro : BaseModel
    {
        public virtual String Descricao { get; set; }
        public virtual Cidade Cidade { get; set; }
        public virtual Pais Pais { get; set; }
        public virtual Uf UF { get; set; }
        public virtual Status_Default? Status { get; set; }
        public virtual string CodigoAuxiliar { get; set; }

        public virtual List<Log_Bairro> GeraLog(Bairro pOld, String pUsuario, String pNomeUsuario, TipoOperLog pTipoOperLog)
        {
            List<Log_Bairro> logs = new List<Log_Bairro>();

            if (pTipoOperLog != TipoOperLog.Alteracao)
            {
                Log_InsereTipoOper<Bairro, Log_Bairro>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString());
            }
            else
            {
                Log_ComparaProperty<Bairro, Log_Bairro>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.ID, pOld, this, p => p.Id);
                Log_ComparaProperty<Bairro, Log_Bairro>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.DESCRICAO, pOld, this, p => p.Descricao);
                Log_ComparaProperty<Bairro, Log_Bairro>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.PAIS, pOld, this, p => p.Pais);
                Log_ComparaProperty<Bairro, Log_Bairro>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.UF, pOld, this, p => p.UF);
                Log_ComparaProperty<Bairro, Log_Bairro>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.STATUS, pOld, this, p => p.Status);
                Log_ComparaProperty<Bairro, Log_Bairro>(logs, pUsuario, pNomeUsuario, pTipoOperLog, Id.ToString(), RsCadastros.CIDADE, pOld, this, p => p.Cidade);

            }

            return logs;
        }

    }
}
