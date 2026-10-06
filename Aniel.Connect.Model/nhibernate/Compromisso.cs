using Aniel.Connect.Core;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aniel.Connect.Model.nhibernate
{
    public class Compromisso: BaseModel
    {
        public virtual String Assunto { get; set; }
        public virtual DateTime DataInicioDefault { get; set; }
        public virtual DateTime DataFimDefault { get; set; }
        public virtual DateTime HoraInicioDefault { get; set; }
        public virtual DateTime HoraFimDefault { get; set; }
        public virtual Int32 QtdTempoEvento { get; set; }
        public virtual Int16 TipoRecorrencia { get; set; }
        public virtual Boolean IsConcluido { get; set; }
        public virtual Int64 Id_funcionario { get; set; }
        public virtual Int64 Id_Evento { get; set; }
        public virtual String Local { get; set; }
        public virtual String Observacoes { get; set; }
        public virtual Boolean Deletado { get; set; }

        #region Mobile
        public virtual Int64 Id_Tarefa { get; set; }
        public virtual String Tarefa_Descricao { get; set; }
        public virtual Int64 Id_Venda { get; set; }
        public virtual Sinapse.Model.Funcionario Funcionario { get; set; }
        public virtual enumerado.NivelPrioridadeTarefaVendedorEnum Prioridades { get; set; }
        public virtual String Nome_Funcionario { get; set; }
        #endregion

    }

    public class RequestCompromissosModel
    {
        public virtual List<Compromisso> compromissos { get; set; }
        public virtual Int64 IdFuncionario { get; set; }
        public virtual DateTime dataInicio { get; set; }
        public virtual DateTime dataFim { get; set; }

    }
}
public class Compromisso_DataTableRequest : DataTableRequest
{

}
