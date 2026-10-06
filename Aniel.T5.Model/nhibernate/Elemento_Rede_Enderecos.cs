using Sinapse.Model.nhibernate;

namespace Aniel.T5.Model.nhibernate
{
    public class Elemento_Rede_Enderecos : BaseModel
    {        
        public virtual long Id_EnderecoLogradouro { get; set; }
        public virtual Elemento_Rede Elemento_Rede { get; set; }

        /// <summary>
        ///     Número do endereço da caixa
        /// </summary>
        public virtual int? Numero { get; set; }
    }
}
