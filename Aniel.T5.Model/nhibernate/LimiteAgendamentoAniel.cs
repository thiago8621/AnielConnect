using Sinapse.Model.nhibernate;
using System;

namespace Aniel.T5.Model.nhibernate
{
    /// <summary>
    ///     <para>
    ///         Define o limite máximo do percentual de horas consumidas
    ///         por serviços agendados para aceitar um serviço de determinada natureza
    ///         em um grupo e área específicos.
    ///     </para>
    ///     
    ///     <para>
    ///         Um limite de 30% para natureza de reparo significa que só serão aceitos serviços de 
    ///         reparo caso menos de 30% das horas de técnicos multiskill disponíveis para atendimento 
    ///         de serviço tenham sido consumidas.
    ///         
    ///         Caso mais de 30% das horas disponíveis já estejam sendo consumidas por
    ///         outros serviços, novos serviços de reparo não serão aceitos.
    ///     </para>
    /// </summary>
    public class LimiteAgendamentoAniel : BaseModel
    {
        /// <summary>
        ///     Cria um registro de limite de agendamento com base nos dados de outro limite de agendamento
        /// </summary>
        /// <param name="old">Objeto com os dados do limite de agendamento que será copiado</param>
        public static LimiteAgendamentoAniel CreateFrom(LimiteAgendamentoAniel old)
        {
            return new LimiteAgendamentoAniel
            {
                CodEscritorio = old.CodEscritorio,
                CodArea = old.CodArea,
                IdNatureza = old.IdNatureza,

                Percentual = old.Percentual,

                DataInicio = old.DataInicio,
                DataFim = old.DataFim
            };
        }

        public virtual string CodEscritorio { get; set; }
        public virtual string CodArea { get; set; }
        public virtual int IdNatureza { get; set; }

        public virtual int Percentual { get; set; }

        /// <summary>
        ///     Data inicial do período em que esse limite de percentual de horas
        ///     consumidas será válido
        /// </summary>
        public virtual DateTime DataInicio { get; set; }

        /// <summary>
        ///     Data final do período em que esse limite de percentual de horas
        ///     consumidas será válido
        /// </summary>
        public virtual DateTime DataFim { get; set; }
    }
}
