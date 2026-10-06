using Sinapse.Model.enumerados;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aniel.Connect.Model.enumerado;
using Sinapse.Model.enumerado;

namespace Aniel.T5.Model.nhibernate
{
    public class Elemento_Rede : BaseModel
    {
        public virtual String Bairro { get; set; }
        public virtual String Pais { get; set; }
        public virtual long? Cod_Caixa { get; set; }
        public virtual Elemento_Rede Cabo { get; set; }
        public virtual Elemento_Rede Caixa { get; set; }
        public virtual Elemento_Rede Poste { get; set; }
        public virtual int Capacidade { get; set; }
        public virtual String CEP { get; set; }
        public virtual bool Certificado { get; set; }
        public virtual String Cidade { get; set; }
        public virtual String Complemento { get; set; }
        public virtual String Contrato { get; set; }
        public virtual String Cor_linha_Ativa { get; set; }
        public virtual String Cor_linha_Inativa { get; set; }

        public virtual String Descricao { get; set; }
        public virtual String Endereco { get; set; }
        public virtual Int64? Id_POP { get; set; }
        public virtual Int64? Id_OLT { get; set; }
        public virtual Double? Latitude { get; set; }
        public virtual Double? Longitude { get; set; }
        public virtual String Numero { get; set; }
        public virtual String Observacao { get; set; }
        public virtual String Projeto { get; set; }
        public virtual Status_Elemento Situacao { get; set; }
        public virtual Tipo_Elemento_Rede Tipo { get; set; }
        public virtual Modelo_Elemento_Rede Modelo { get; set; }
        public virtual String UF { get; set; }
        public virtual String Coordenadas { get; set; }
        public virtual long? Num_Coordenadas { get; set; }
        public virtual Double? Metragem { get; set; }
        public virtual Padrao_Cor Padrao_Cores { get; set; }
        public virtual IEnumerable<Elemento_Rede_Fibra> Fibras { get; set; }
        public virtual IEnumerable<Elemento_Rede_Enderecos> EnderecosCadastrados { get; set; }
        public virtual String JSON_Ordenacao_Digarama { get; set; }

        //private IEnumerable<Elemento_Rede_Conexoes> Elementos_Destino { get; set; }
        //public virtual IEnumerable<Elemento_Rede_Conexoes> Conexoes
        //{
        //    get
        //    {
        //        if (Elementos_Destino == null)
        //        {
        //            Elementos_Destino = new List<Elemento_Rede_Conexoes>();
        //        }
        //        return Elementos_Destino;
        //    }
        //    set { Elementos_Destino = value; }
        //}
    }
    
}
