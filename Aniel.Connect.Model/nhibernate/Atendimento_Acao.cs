using Aniel.Connect.Model.enumerado;
using Sinapse.Model.Identity;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Atendimento_Acao : BaseModel
    {
        public virtual String Usuario_Logado { get; set; }

        
        public virtual Status_Atendimento Status_Atendimento { get; set; }

        public virtual IdentityUser Responsavel { get; set; }
        public virtual String HTML { get; set; }
        public virtual Atendimento Atendimento { get; set; }
        public virtual Origem_AtendimentoAcao Origem { get; set; }
        public virtual TipoAtividadeAtendimento TipoAtividade { get; set; }

        public virtual string MessageID { get; set; }

        private IList<Arquivo_AtendimentoAcao> Arquivos_AtendimentoAcaoLista;

        public virtual IList<Arquivo_AtendimentoAcao> Arquivos_AtendimentoAcao
        {
            get
            {
                if (Arquivos_AtendimentoAcaoLista == null)
                    Arquivos_AtendimentoAcaoLista = new List<Arquivo_AtendimentoAcao>();
                return Arquivos_AtendimentoAcaoLista;
            }
            set
            {
                Arquivos_AtendimentoAcaoLista = value;
            }
        }

    }
}
