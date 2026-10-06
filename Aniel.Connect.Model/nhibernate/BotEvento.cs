using Aniel.Connect.Model.enumerado;
using Sinapse.Model;
using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;

namespace Aniel.Connect.Model.nhibernate
{
    public class BotEvento : BaseModel
    {
        public virtual String Descricao { get; set; }
        public virtual String TextoEnviado { get; set; }
        public virtual String VariaveisDisponiveis { get; set; }
        public virtual Status_Default Status { get; set; }
        public virtual bool EventoInicial { get; set; }
        public virtual bool DisparoAutomatico { get; set; }
        public virtual BotEvento AutoProximoEvento { get; set; }
        public virtual bool UtilizaAcao { get; set; }
        public virtual Bot Bot { get; set; }
        public virtual Tipo_Bot_Evento_Acoes? AcaoExecutada { get; set; }
        public virtual Pesquisa_Satisfacao PesquisaSatisfacao { get; set; }
        public virtual bool CondicaoEvento { get; set; }
        public virtual BotEvento EventoFalso { get; set; }
        public virtual BotEvento EventoVerdadeiro { get; set; }
        public virtual Departamento SetorAtendimento { get; set; }
        public virtual EventoArmazenamento? EventoArmazenamento { get; set; }


        private IList<BotEventoResposta> _respostas;
        public virtual IList<BotEventoResposta> Respostas
        {
            get
            {
                if (_respostas == null)
                    _respostas = new List<BotEventoResposta>();
                return _respostas;
            }
            set
            {
                _respostas = value;
            }
        }

        private IList<BotEventoAnexo> _anexos;
        public virtual IList<BotEventoAnexo> Anexos
        {
            get
            {
                if (_anexos == null)
                    _anexos = new List<BotEventoAnexo>();
                return _anexos;
            }
            set
            {
                _anexos = value;
            }
        }

    }
}
