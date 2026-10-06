using Aniel.Connect.Model.enumerado;
using Aniel.Connect.ModelLog.nhibernate;
using Resources;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.Model.nhibernate;
using Sinapse.ModelLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Pesquisa_Resposta : BaseModel
    {

        public virtual Cliente Cliente { get; set; }
        
        public virtual Pesquisa_Satisfacao Pesquisa_Satisfacao { get; set; }
        
        public virtual DateTime? DataHora_Respondida { get; set; }

        public virtual String CodCt { get; set; }
        public virtual String Projeto { get; set; }
        public virtual String Num_Obra { get; set; }
        public virtual Int32 Num_Doc { get; set; }
        public virtual PesquisaSatisfacaoCategoriaCaixaTexto Categoria { get; set; }
        public virtual String Comentario { get; set; }
        public virtual String Obs { get; set; }
        public virtual Atendimento Atendimento { get; set; }

        public virtual List<LogEnvio> GeraLog(Pesquisa_Resposta pOld, String pUsuario, String pNomeUsuario, TipoOperLog pTipoOperLog)
        {
            List<LogEnvio> logs = new List<LogEnvio>();

            if (pTipoOperLog != TipoOperLog.Alteracao)
            {
                Log_InsereTipoOper<Pesquisa_Resposta, LogEnvio>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString());
            }
            else
            {
                Log_ComparaProperty<Pesquisa_Resposta, LogEnvio>(logs, pUsuario, pNomeUsuario, pTipoOperLog, this.Id.ToString(), RsCadastros.OBSERVACAO, pOld, this, p => p.Obs);
                
            }



            return logs;
        }
    }
}
