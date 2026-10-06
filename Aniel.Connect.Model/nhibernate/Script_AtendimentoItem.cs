using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;

namespace Aniel.Connect.Model.nhibernate
{
    /// <summary>
    ///     Entidade que serve de modelo para as questões que serão adicionadas
    ///     nos scripts dos atendimentos. A entidade que é associada aos scripts
    ///     de um atendimento específico é o ScriptAtendimentoQuestion
    /// </summary>
    public class Script_AtendimentoItem : BaseModel
    {
        public virtual long Ordem { get; set; }
        public virtual string Pergunta { get; set; }
        public virtual string Explicacao_Cliente { get; set; }
        public virtual string Explicacao_Operador { get; set; }
        public virtual string Respostas_Possiveis { get; set; }
        public virtual Script_Tipo_Resposta Tipo_Resposta { get; set; }
        public virtual string Observacoes { get; set; }

        public virtual Script_Atendimento Script { get; set; }
    }
}
