using System;

namespace Aniel.T5.Model.nhibernate
{
    public class QuizAnswerCapa
    {
        public virtual long Id { get; set; }

        public virtual DateTime? DiaInicio { get; set; }
        public virtual DateTime? HoraInicio { get; set; }
        public virtual string UsuarioResposta { get; set; }
        
        public virtual Quiz Quiz { get; set; }

        public virtual string Titulo { get; set; }
        public virtual string Protocolo { get; set; }
        public virtual string ProtocoloNomeOriginal { get; set; }
        public virtual long Repeticao { get; set; }
        public virtual string ObrigarImprodutivo { get; set; }
        public virtual string QuizInicioAtendimento { get; set; }
    }
}
