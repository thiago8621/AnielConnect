using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;

namespace Aniel.Connect.Model.nhibernate
{
    /// <summary>
    ///     Entidade que é associada à pergunta de um script específico
    ///     para um atendimento.
    ///     
    ///     Essa entidade é uma cópia do modelo da pergunta que se encontra
    ///     na entidade Script_AtendimentoItem no momento da atribuição de
    ///     um script para um atendimento
    /// </summary>
    public class ScriptAtendimentoQuestion : BaseModel
    {
        public virtual long Ordem { get; set; }
        public virtual string Pergunta { get; set; }
        public virtual string Explicacao_Cliente { get; set; }
        public virtual string Explicacao_Operador { get; set; }
        public virtual string Respostas_Possiveis { get; set; }
        public virtual Script_Tipo_Resposta Tipo_Resposta { get; set; }
        public virtual string Observacoes { get; set; }

        public virtual Script_Atendimento Script { get; set; }

        /// <summary>
        ///     Cria uma entidade ScriptAtendimentoQuestion baseada em um modelo de questão
        ///     do script de atendimento
        /// </summary>
        /// <param name="modelo">Objeto com os dados da questão do script</param>
        public static ScriptAtendimentoQuestion FromPerguntaModelo(Script_AtendimentoItem modelo)
        {
            if (modelo == null)
                throw new ArgumentNullException(nameof(modelo));

            return new ScriptAtendimentoQuestion
            {
                DataHoraCriacao = DateTime.Now,

                Ordem = modelo.Ordem,
                Pergunta = modelo.Pergunta,
              
                Explicacao_Cliente = modelo.Explicacao_Cliente,
                Explicacao_Operador = modelo.Explicacao_Operador,
                
                Respostas_Possiveis = modelo.Respostas_Possiveis,
                Tipo_Resposta = modelo.Tipo_Resposta,

                Observacoes = modelo.Observacoes,

                Script = modelo.Script
            };
        }
    }
}
