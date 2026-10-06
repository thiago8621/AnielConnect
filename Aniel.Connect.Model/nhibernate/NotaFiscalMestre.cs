using Sinapse.Model.enumerado;
using Sinapse.Model.nhibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aniel.Connect.API_Aniel.Models
{
    public class Nota_Fiscal_Mestre : BaseModel
    {
        public virtual String Razao_Social_Tomador { get; set; }
        public virtual String Nome_Fantasia_Tomador { get; set; }

        public virtual String Endereco_Tomador { get; set; }
        public virtual Int32 Numero_Tomador { get; set; }
        public virtual String Complemento_Tomador { get; set; }
        public virtual String Bairro_Tomador { get; set; }
        public virtual String Cidade_Tomador { get; set; }
        public virtual String Estado_Tomador { get; set; }
        public virtual String Cep_Tomador { get; set; }

        public virtual String Cnpj_Tomador { get; set; }
        public virtual String Insc_Estadual_Tomador { get; set; }
        public virtual String Insc_Municipal_Tomador { get; set; }
        public virtual String Cpf_Tomador { get; set; }
        public virtual String Rg_Tomador { get; set; }

        public virtual Int32 Dia_Vencimento { get; set; }
        public virtual long IdFatura { get; set; }
        public virtual String Modelo_Nota_Fiscal { get; set; }
        public virtual String Cfop { get; set; }
        public virtual String Telefone_Tomador { get; set; }
        public virtual String DDD_Tomador { get; set; }
        public virtual String Email_Tomador { get; set; }
        public virtual String Observacao { get; set; }

        public virtual String Cod_Consumidor { get; set; }
        public virtual String Tipo_Assinante { get; set; }
        public virtual String Tipo_Utilizacao { get; set; }
        public virtual DateTime Data_Emissao { get; set; }
        public virtual DateTime Data_Prestacao { get; set; }

        public virtual Int64 Num_Nota_Fiscal { get; set; }
        public virtual Int64 Sequencial_Nota_Fiscal { get; set; }
        public virtual String Serie_Nota_Fiscal { get; set; }

        public virtual Int64 Id_Conta_Receber { get; set; }
        public virtual Boolean Instalacao { get; set; }


        public virtual String Razao_Social_Prestador { get; set; }
        public virtual String Nome_Fantasia_Prestador { get; set; }
        public virtual String Endereco_Prestador { get; set; }
        public virtual Int32 Numero_Prestador { get; set; }
        public virtual String Complemento_Prestador { get; set; }
        public virtual String Bairro_Prestador { get; set; }
        public virtual String Cidade_Prestador { get; set; }
        public virtual String Estado_Prestador { get; set; }
        public virtual String Cep_Prestador { get; set; }

        public virtual String Cnpj_Prestador { get; set; }
        public virtual String Insc_Estadual_Prestador { get; set; }
        public virtual String Insc_Municipal_Prestador { get; set; }
        public virtual String Cpf_Prestador { get; set; }
        public virtual String Telefone_Prestador { get; set; }
        public virtual String DDD_Prestador { get; set; }
        public virtual String Email_Prestador { get; set; }

        public virtual String Processo_Suspensao { get; set; }
        public virtual Int64? Id_Configuracao_Tributaria { get; set; }
        public virtual String Cnae { get; set; }
        public virtual Natureza_TributacaoEnum Natureza_tributacao { get; set; }
        public virtual Boolean Optante_Simples_Nacional { get; set; }
        public virtual Boolean Incentivo_Cultural { get; set; }
        public virtual Boolean Incentivo_Fiscal { get; set; }
        public virtual Int64? Id_Atividade_NF { get; set; }
        public virtual String Id_Api_NF { get; set; }
        public virtual String Xml_NF { get; set; }
        public virtual String Chave_Cancelamento { get; set; }

        public virtual Int64 Id_Filial { get; set; } //Não existe no banco, é somente para atualizar o sequencial da NF na TB_FILIAL
    }
}