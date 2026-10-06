using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;

namespace Aniel.Connect.Model.nhibernate
{
    public class AnexoContratoAceiteModel : BaseModel
    {
        public virtual long IdVenda { get; set; }
        public virtual string Descricao { get; set; }
        public virtual string NomeArquivo { get; set; }
        public virtual string Extensao { get; set; }
        public virtual byte[] Anexo { get; set; }
        public virtual bool ExigeAceite { get; set; }
        public virtual StatusAnexoContrato Status { get; set; }
    }    
}

