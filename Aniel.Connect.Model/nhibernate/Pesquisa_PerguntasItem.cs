using System;

namespace Aniel.Connect.Model.nhibernate
{
    public class Pesquisa_PerguntasItem
    {

        public virtual String Descricao { get; set; }

        public virtual Boolean Obrigatorio { get; set; }

        public virtual Boolean Status_Pesquisa_Pergunta { get; set; }

        public virtual String Escala_Linear_Ini { get; set; }

        public virtual String Escala_Linear_Fin { get; set; }

        public virtual String Pesquisa_Satisfacao { get; set; }

    }
}
