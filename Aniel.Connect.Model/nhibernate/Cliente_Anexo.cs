using System;
using Sinapse.Model.nhibernate;

namespace Aniel.Connect.Model.nhibernate
{
    public class Cliente_Anexo : BaseModel
    {
        public virtual String Nome_Arquivo { get; set; }
        public virtual String Extensao_Arquivo { get; set; }
        public virtual String Titulo { get; set; }

        public virtual Cliente Cliente { get; set; }
        public virtual ClienteContrato ClienteContrato { get; set; }
        public virtual Boolean Foto_Perfil { get; set; }
        public virtual Boolean Contrato { get; set; }
        public virtual Boolean IsAnexoContratoAditivo { get; set; }
        public virtual Venda Venda { get; set; }
        public virtual byte[] Arquivo { get; set; }
        public virtual String DescricaoModeloUtilizado { get; set; }
    }
}
