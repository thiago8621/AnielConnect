using Aniel.Connect.Model.enumerado;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.Identity;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Pesquisa_Satisfacao : BaseModel
    {
        public virtual String Titulo_Da_Pesquisa { get; set; }

        public virtual String Descricao { get; set; }

        public virtual Status_Default Status   { get; set; }

        public virtual PesquisaSatisfacaoDisparoAuto DisparoAuto { get; set; }

        public virtual PesquisaSatisfacaoQuemPreenche QuemPreenche { get; set; }

        private IList<Pesquisa_Pergunta> _perguntas;
        
        public virtual Boolean Tem_Caixa_Texto { get; set; }
        public virtual Boolean Envia_Msg_Emai_Sms { get; set; }
        public virtual String Modelo_Envio { get; set; }

        public virtual PesquisaSatisfacaoTipoCaixaTexto? TipoCaixaTexto { get; set; }

        public virtual IList<Pesquisa_Pergunta> Perguntas
        {
            get
            {
                if (_perguntas == null)
                    _perguntas = new List<Pesquisa_Pergunta>();
                return _perguntas;
            }
            set
            {
                _perguntas = value;
            }
        }


        private IList<Pesquisa_Resposta> _respostas;

        public virtual IList<Pesquisa_Resposta> Respostas
        {
            get
            {
                if (_respostas == null)
                    _respostas = new List<Pesquisa_Resposta>();
                return _respostas;
            }
            set
            {
                _respostas = value;
            }
        }
    }


}
