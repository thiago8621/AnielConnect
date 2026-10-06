using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;

namespace Aniel.Connect.Model.nhibernate
{
    public class AlteracaoPlanoContrato: BaseModel
    {        
        public virtual long IdVenda { get; set; }
        public virtual long? IdContratoAnterior { get; set; }        
        public virtual long? IdPlanoAnterior { get; set; }
        public virtual long? IdPlano { get; set; }
        public virtual TipoAlteracaoPlanoContratoEnum Alteracao { get; set; }
        public virtual StatusAlteracaoPlanoContratoEnum Status { get; set; }
    }
}
