using Aniel.Connect.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;

namespace Aniel.Connect.Model.nhibernate
{
    public class Endereco_Cliente : BaseModel
    {
        public virtual String Cep { get; set; }
        public virtual String Logradouro { get; set; }

        public virtual Atendimento Atendimento { get; set; }

        public virtual Tipo_Imovel Tipo_Imovel { get; set; }
        public virtual Pais Pais { get; set; }

        public virtual String Numero { get; set; }
        public virtual String Cod_Cliente { get; set; }
        public virtual String Cod_Contrato { get; set; }
        public virtual String Cod_Conexao { get; set; }
        public virtual Cliente Cliente { get; set; }
        public virtual String Cpf { get; set; }
        public virtual String Contrato_Cancelado { get; set; }
        public virtual String Conexao_Bloqueada { get; set; }
        public virtual String Cod_Fatura { get; set; }
        public virtual String Valor_Total { get; set; }
        public virtual String Liquidado { get; set; }
        public virtual String Descricao { get; set; }
        public virtual String Fatura_Suspensa { get; set; }
        public virtual String Vencimento { get; set; }
        public virtual String MotivoSuspensao { get; set; }
        public virtual String DescricaoDebito { get; set; }
        public virtual Int64? Id_Bairro { get; set; }
        public virtual String Bairro { get; set; }
        public virtual Int64? Id_Cidade { get; set; }
        public virtual String Cidade { get; set; }
        public virtual String UF { get; set; }
        public virtual String Complemento { get; set; }
        public virtual Busca_Endereco? Busca_Endereco { get; set; }
        public virtual Condominio_Predio Condominio_Predio { get; set; }
        public virtual Endereco_Logradouro Endereco_Logradouro { get; set; }
        public virtual string BlocoDescri { get; set; }
        public virtual long? Bloco { get; set; }
        public virtual string ApartamentoDescri { get; set; }
        public virtual long? Apartamento { get; set; }
        public virtual String Ponto_Referencia { get; set; }

        public virtual Int64? Id_UF { get; set; }
        //public virtual Int64? Id_Cliente_Integracao { get; set; }
        public virtual Double? Latitude { get; set; }
        public virtual Double? Longitude { get; set; }

        //public virtual String UF_Tes { get; set; }

        public virtual String Cod_Area { get; set; }


        public virtual String Cod_Escritorio { get; set; }


        public virtual Int32? Cod_Localidade { get; set; }

        public virtual String Localidade { get; set; }

        #region Mobile
        public virtual String Area { get; set; }
        public virtual string id_GrupoArea { get; set; }
        public virtual String desc_GrupoArea { get; set; }

        public virtual string id_Area { get; set; }
        public virtual String desc_Area { get; set; }

        public virtual string id_Localidade { get; set; }
        public virtual String desc_Localidade { get; set; }
        #endregion
    }
}
